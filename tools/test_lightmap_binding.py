"""Offline structural checks for the reversible legacy-lightmap binder.

Does not require Unity or the omitted AssetRipper export ZIP. It accepts the
repository's pre-binding state and the state after the editor menu is applied.
"""
import json
import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
CLIENT = ROOT / "client"
TOOL_MANIFESTS = ROOT / "tools" / "unity-editor" / "MapGeometry"
CLIENT_MANIFESTS = CLIENT / "Assets" / "Editor" / "BlockStrikeRecovery" / "MapGeometry"
BINDER_SOURCE = ROOT / "tools" / "unity-editor" / "LegacyLightmapBinder.cs"
EDITOR_SOURCE = ROOT / "tools" / "unity-editor" / "BlockStrikeLightmapRecovery.cs"
BINDER_CLIENT = CLIENT / "Assets" / "Scripts" / "Assembly-CSharp" / "LegacyLightmapBinder.cs"
EDITOR_CLIENT = CLIENT / "Assets" / "Editor" / "BlockStrikeRecovery" / "BlockStrikeLightmapRecovery.cs"


def guid_from_meta(path):
    match = re.search(r"^guid: ([0-9a-f]{32})$", path.read_text(), re.MULTILINE)
    if not match:
        raise AssertionError("missing GUID in " + str(path))
    return match.group(1)


def guid_paths_for_test(guid, client_root):
    for meta in client_root.glob("Assets/**/*.meta"):
        if guid_from_meta(meta) == guid:
            return meta.with_suffix("").as_posix()
    return ""


class LightmapBindingTests(unittest.TestCase):
    def test_tool_and_client_sources_and_metas_match(self):
        self.assertEqual(BINDER_SOURCE.read_bytes(), BINDER_CLIENT.read_bytes())
        self.assertEqual(EDITOR_SOURCE.read_bytes(), EDITOR_CLIENT.read_bytes())
        self.assertEqual(guid_from_meta(BINDER_SOURCE.with_suffix(".cs.meta")),
                         guid_from_meta(BINDER_CLIENT.with_suffix(".cs.meta")))
        self.assertEqual(guid_from_meta(EDITOR_SOURCE.with_suffix(".cs.meta")),
                         guid_from_meta(EDITOR_CLIENT.with_suffix(".cs.meta")))

    def test_all_lightmap_manifest_data_is_present_and_resolvable(self):
        tool_files = sorted(TOOL_MANIFESTS.glob("*.json"))
        client_files = sorted(CLIENT_MANIFESTS.glob("*.json"))
        self.assertEqual(len(tool_files), 56)
        self.assertEqual([p.name for p in tool_files], [p.name for p in client_files])

        binder_guid = guid_from_meta(BINDER_CLIENT.with_suffix(".cs.meta"))
        guid_paths = {}
        for meta in CLIENT.glob("Assets/**/*.meta"):
            match = re.search(r"^guid: ([0-9a-f]{32})$", meta.read_text(errors="replace"), re.MULTILINE)
            if match:
                guid_paths[match.group(1)] = meta.with_suffix("").as_posix()
        lightmapped_scenes = 0
        lightmapped_renderers = 0
        for tool_manifest, client_manifest in zip(tool_files, client_files):
            self.assertEqual(tool_manifest.read_bytes(), client_manifest.read_bytes())
            manifest = json.loads(tool_manifest.read_text())
            scene_path = CLIENT / manifest["scenePath"]
            scene = scene_path.read_text(errors="replace")
            records = [r for r in manifest["renderers"] if r["lightmapIndex"] < 254]
            if not records:
                continue

            lightmapped_scenes += 1
            lightmapped_renderers += len(records)
            self.assertTrue(all(r["lightmapIndex"] == 0 for r in records), manifest["sceneName"])
            renderer_ids = [str(r["rendererId"]) for r in records]
            self.assertEqual(len(renderer_ids), len(set(renderer_ids)), manifest["sceneName"])
            filter_blocks = {
                int(match.group(1)): match.group(2)
                for match in re.finditer(r"(?ms)^--- !u!33 &(\d+)\n(.*?)(?=^--- !u!|\Z)", scene)
            }
            for record in records:
                self.assertRegex(scene, r"(?m)^--- !u!23 &" + re.escape(str(record["rendererId"])) + r"\s*$",
                                 manifest["sceneName"] + "/" + record["objectName"])
                mesh_filter = filter_blocks.get(record["filterId"])
                self.assertIsNotNone(mesh_filter, manifest["sceneName"] + " MeshFilter ID")
                mesh_guid = re.search(r"m_Mesh: \{fileID: \d+, guid: ([0-9a-f]{32})", mesh_filter)
                self.assertIsNotNone(mesh_guid, manifest["sceneName"] + " recovered MeshFilter reference")
                mesh_path = guid_paths.get(mesh_guid.group(1), "")
                suffix = "/" + manifest["sceneName"] + "/Renderer-" + str(record["rendererId"]) + ".asset"
                self.assertIn("/RecoveredGeometry/MapGeometry-", mesh_path)
                self.assertTrue(mesh_path.endswith(suffix), manifest["sceneName"] + " recovered mesh path")

            texture_path = (CLIENT / Path(manifest["scenePath"]).parent /
                            Path(manifest["scenePath"]).stem / "LightmapFar-0.png")
            self.assertTrue(texture_path.is_file(), str(texture_path))
            self.assertTrue(Path(str(texture_path) + ".meta").is_file(), str(texture_path) + ".meta")
            texture_guid = guid_from_meta(Path(str(texture_path) + ".meta"))

            binders_in_scene = scene.count("guid: " + binder_guid)
            self.assertIn(binders_in_scene, (0, 1), manifest["sceneName"] + " binder count")
            if binders_in_scene:
                self.assertIn("guid: " + texture_guid, scene,
                              manifest["sceneName"] + " binder must reference its own lightmap")
                refs = re.findall(r"(?m)^\s*- \{fileID: (\d+)\}$", scene)
                ref_ids = set(refs)
                self.assertTrue(set(renderer_ids).issubset(ref_ids),
                                manifest["sceneName"] + " binder must retain renderer references")
            else:
                # This is the original problem the editor command repairs.
                self.assertNotIn("guid: " + texture_guid, scene, manifest["sceneName"])
                self.assertRegex(scene, r"m_LightingDataAsset:\s*\{fileID: 0\}", manifest["sceneName"])

        self.assertEqual(lightmapped_scenes, 54)
        self.assertEqual(lightmapped_renderers, 3196)

    def test_mesh_atlas_override_is_detected_and_retargeted_only_by_bind(self):
        source = EDITOR_CLIENT.read_text()
        self.assertIn("IndexSerializedMeshGuids", source)
        self.assertIn("MeshAtlas has ExecuteInEditMode", source)
        self.assertIn("meshAtlas.DisableMesh()", source)
        self.assertIn("meshAtlas.originalMesh = geometry.recoveredMesh", source)
        self.assertIn("Validate made no scene changes", source)

        atlas_guid = guid_from_meta(CLIENT / "Assets/Scripts/Assembly-CSharp/MeshAtlas.cs.meta")
        manifest = json.loads((CLIENT_MANIFESTS / "50_Shooting Range.json").read_text())
        scene_path = CLIENT / manifest["scenePath"]
        scene = scene_path.read_text(errors="replace")
        blocks = {
            int(match.group(2)): (int(match.group(1)), match.group(3))
            for match in re.finditer(r"(?ms)^--- !u!(\d+) &(\d+)\n(.*?)(?=^--- !u!|\Z)", scene)
        }
        atlas_renderers = []
        for record in manifest["renderers"]:
            if record["lightmapIndex"] >= 254:
                continue
            renderer_body = blocks[record["rendererId"]][1]
            game_object_id = int(re.search(r"m_GameObject: \{fileID: (\d+)\}", renderer_body).group(1))
            game_object_body = blocks[game_object_id][1]
            component_ids = [int(value) for value in re.findall(r"- component: \{fileID: (\d+)\}", game_object_body)]
            atlas_body = None
            for component_id in component_ids:
                component_type, component_body = blocks[component_id]
                if component_type == 114 and re.search(
                        r"m_Script: \{fileID: 11500000, guid: " + atlas_guid + r", type: 3\}", component_body):
                    atlas_body = component_body
                    break
            if atlas_body is None:
                continue

            mesh_filter = blocks[record["filterId"]][1]
            recovered_guid = re.search(r"m_Mesh: \{fileID: \d+, guid: ([0-9a-f]{32})", mesh_filter).group(1)
            atlas_original_guid = re.search(
                r"originalMesh: \{fileID: \d+, guid: ([0-9a-f]{32})", atlas_body).group(1)
            mesh_settings = re.search(r"(?m)^\s*meshSettings: (\d+)", atlas_body).group(1)
            self.assertIn("/RecoveredGeometry/MapGeometry-", guid_paths_for_test(recovered_guid, CLIENT))
            self.assertNotEqual(recovered_guid, atlas_original_guid)
            self.assertEqual(mesh_settings, "0")
            atlas_renderers.append(record["rendererId"])

        self.assertEqual(len(atlas_renderers), 4)

    def test_editor_tool_is_opt_in_reversible_and_strict(self):
        text = EDITOR_SOURCE.read_text()
        self.assertIn('MenuItem("Tools/Block Strike Recovery/Validate ALL legacy lightmaps")', text)
        self.assertIn('MenuItem("Tools/Block Strike Recovery/Bind ALL legacy lightmaps")', text)
        self.assertIn('MenuItem("Tools/Block Strike Recovery/Revert last legacy lightmap binding")', text)
        self.assertIn("TextureImporterType.Lightmap", text)
        self.assertIn("RestoreReceipt(receipt)", text)
        self.assertIn("RecoveredGeometry/MapGeometry-", text)
        self.assertIn("ExpectedLightmappedRendererCount = 3196", text)
        self.assertIn("string meshGuid = null;", text)


if __name__ == "__main__":
    unittest.main()
