"""Offline checks for every committed static-batch repair (no Unity/export ZIP needed)."""

import json
import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / "client" / "Assets"
MANIFESTS = ROOT / "tools" / "unity-editor" / "MapGeometry"


class RecoveredSceneGeometryTests(unittest.TestCase):
    def test_every_manifest_renderer_keeps_its_persistent_mesh(self):
        # MeshAtlas creates a HideAndDontSave clone in edit mode. If a scene is
        # saved while that clone is active, Unity can serialize m_Mesh: {fileID: 0}.
        # Check all 3,891 renderers, not only the 3,196 with baked lightmaps.
        guid_paths = {}
        for meta in ASSETS.glob("RecoveredGeometry/MapGeometry-*/*/Renderer-*.asset.meta"):
            guid = re.search(r"(?m)^guid: ([0-9a-f]{32})$", meta.read_text()).group(1)
            self.assertNotIn(guid, guid_paths, str(meta))
            guid_paths[guid] = meta.with_suffix("")

        atlas_guid = re.search(
            r"(?m)^guid: ([0-9a-f]{32})$",
            (ASSETS / "Scripts/Assembly-CSharp/MeshAtlas.cs.meta").read_text(),
        ).group(1)
        manifests = sorted(MANIFESTS.glob("*.json"))
        self.assertEqual(len(manifests), 56)
        renderers = atlases = 0
        for manifest_file in manifests:
            manifest = json.loads(manifest_file.read_text())
            scene = (ROOT / "client" / manifest["scenePath"]).read_text(errors="replace")
            blocks = {
                int(m.group(2)): (int(m.group(1)), m.group(3))
                for m in re.finditer(r"(?ms)^--- !u!(\d+) &(\d+)\r?\n(.*?)(?=^--- !u!|\Z)", scene)
            }
            for record in manifest["renderers"]:
                renderer_id = record["rendererId"]
                label = manifest["sceneName"] + "/Renderer-" + str(renderer_id)
                self.assertEqual(blocks[renderer_id][0], 23, label)
                kind, filter_body = blocks[record["filterId"]]
                self.assertEqual(kind, 33, label)
                match = re.search(
                    r"(?m)^  m_Mesh: \{fileID: 4300000, guid: ([0-9a-f]{32}), type: 2\}$",
                    filter_body,
                )
                self.assertIsNotNone(match, label + " persistent MeshFilter reference")
                mesh_guid = match.group(1)
                path = guid_paths.get(mesh_guid)
                self.assertIsNotNone(path, label + " recovered mesh asset")
                self.assertEqual(path.name, "Renderer-" + str(renderer_id) + ".asset", label)
                self.assertEqual(path.parent.name, manifest["sceneName"], label)
                self.assertTrue(path.is_file(), label)

                game_object_id = int(re.search(
                    r"m_GameObject: \{fileID: (\d+)\}", filter_body
                ).group(1))
                component_ids = re.findall(
                    r"- component: \{fileID: (\d+)\}", blocks[game_object_id][1]
                )
                for component_id in component_ids:
                    component_type, body = blocks[int(component_id)]
                    if component_type != 114 or not re.search(
                        r"m_Script: \{fileID: 11500000, guid: " + atlas_guid + r", type: 3\}", body
                    ):
                        continue
                    self.assertRegex(
                        body,
                        r"originalMesh: \{fileID: 4300000, guid: " + mesh_guid + r", type: 2\}",
                        label + " MeshAtlas must clone the recovered source mesh",
                    )
                    self.assertRegex(body, r"(?m)^  meshSettings: 0$", label)
                    atlases += 1
                renderers += 1
        self.assertEqual(renderers, 3891)
        self.assertEqual(atlases, 24)


if __name__ == "__main__":
    unittest.main()
