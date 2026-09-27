"""Generate immutable static-geometry manifests for the Unity 5.6 repair tool.

The manifests describe only data recovered from the original export.  They allow
an editor tool to verify imported scenes before changing anything.  Use --all to
write every scene; without it, write the historical Bust pilot manifest.
"""
import argparse
import hashlib
import json
import re
import struct
import sys
import zipfile
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from audit_static_batches import ROOT, blocks, field, ref, mesh_data

EXPORT_ZIP = ROOT / "exports" / "BlockStrike-608-Unity-4.7.2f1.zip"
SCENE_NAMES = ROOT / "tools" / "scene-names.json"
OUT_DIR = ROOT / "tools" / "unity-editor" / "MapGeometry"
PILOT_OUT = ROOT / "tools" / "unity-editor" / "MapGeometry" / "28_Bust.json"

# Unity YAML formats used by this export: 0=float32, 1=float16, 2=unorm8.
_NUMERIC_FORMATS = {
    0: ("f", 4),
    1: ("e", 2),
}


def vector(body, key):
    return [float(x) for x in re.findall(r"[xyzw]: ([^,}]+)", field(body, key))]


def mesh_channels(text):
    vertex = text.split("  m_VertexData:\n", 1)[1].split("  m_CompressedMesh:", 1)[0]
    section = vertex.split("    m_Channels:\n", 1)[1].split("    m_Streams:", 1)[0]
    return [tuple(map(int, m.groups())) for m in re.finditer(
        r"    - stream: (\d+)\n\s+offset: (\d+)\n\s+format: (\d+)\n\s+dimension: (\d+)",
        section,
    )]


def decode_channel(raw, stride, channel, index):
    stream, offset, fmt, dimension = channel
    assert stream == 0, "This validator supports only the known single-stream layout."
    assert fmt in _NUMERIC_FORMATS, "Unsupported numeric UV format: %r" % (fmt,)
    code, _size = _NUMERIC_FORMATS[fmt]
    return struct.unpack_from("<" + code * dimension, raw, index * stride + offset)


def validate_atlas_uv2(raw, stride, channels, indices, scale_offset):
    """Return True when UV2 already contains this renderer's atlas rectangle."""
    if len(channels) <= 4:
        return False
    uv2 = channels[4]
    if uv2[3] < 2:
        return False
    for index in sorted(set(indices)):
        uv = decode_channel(raw, stride, uv2, index)
        for axis in range(2):
            origin = scale_offset[axis + 2]
            scale = scale_offset[axis]
            if not (origin - 0.002 <= uv[axis] <= origin + scale + 0.002):
                return False
    return True


def generate_one(mapping):
    with zipfile.ZipFile(EXPORT_ZIP) as z:
        objects = blocks(z.read("UnityProject/" + mapping["old"]).decode())
        guids = {
            field(z.read(path).decode(), "guid"): path[:-5]
            for path in z.namelist()
            if path.endswith(".meta")
        }
        filters = {
            ref(body, "m_GameObject"): (file_id, body)
            for file_id, (kind, body) in objects.items()
            if kind == 33
        }
        transforms = {
            ref(body, "m_GameObject"): (file_id, body)
            for file_id, (kind, body) in objects.items()
            if kind == 4
        }
        records = []
        meshes = {}
        for renderer_id, (kind, body) in objects.items():
            if kind != 23 or not field(body, "m_SubsetIndices"):
                continue
            raw_subsets = bytes.fromhex(field(body, "m_SubsetIndices"))
            assert len(raw_subsets) % 4 == 0
            subsets = list(struct.unpack("<" + "I" * (len(raw_subsets) // 4), raw_subsets))
            game_object = ref(body, "m_GameObject")
            filter_id, filter_body = filters[game_object]
            transform_id, transform_body = transforms[game_object]
            guid = re.search(r"guid: (\w+)", field(filter_body, "m_Mesh"))[1]
            if guid not in meshes:
                text = z.read(guids[guid]).decode()
                raw, stride, submeshes = mesh_data(text)
                meshes[guid] = (raw, stride, submeshes, mesh_channels(text))
            raw, stride, submeshes, channels = meshes[guid]
            indices = [index for subset in subsets for index in submeshes[subset]]
            index_hash = hashlib.sha256(
                struct.pack("<" + "i" * len(indices), *indices)
            ).hexdigest()
            position_hash = hashlib.sha256(
                b"".join(raw[i:i + 12] for i in range(0, len(raw), stride))
            ).hexdigest()
            lightmap_scale_offset = vector(body, "m_LightmapTilingOffset")
            lightmap_index = int(field(body, "m_LightmapIndex"))
            # Every surveyed lightmapped static renderer in 608 already has UV2
            # transformed into the atlas rectangle.  Fail closed if that changes.
            if lightmap_index < 254:
                assert validate_atlas_uv2(raw, stride, channels, indices, lightmap_scale_offset), (
                    mapping["name"],
                    renderer_id,
                    "UV2 does not match the original lightmap atlas rectangle",
                )
            records.append({
                "rendererId": renderer_id,
                "filterId": filter_id,
                "transformId": transform_id,
                "objectName": field(objects[game_object][1], "m_Name"),
                "meshGuid": guid,
                "vertexCount": len(raw) // stride,
                "subMeshCount": len(submeshes),
                "positionHash": position_hash,
                "subsets": subsets,
                "indexHash": index_hash,
                "batchRootId": ref(body, "m_StaticBatchRoot"),
                "lightmapIndex": lightmap_index,
                "lightmapScaleOffset": lightmap_scale_offset,
                "localPosition": vector(transform_body, "m_LocalPosition"),
                "localRotation": vector(transform_body, "m_LocalRotation"),
                "localScale": vector(transform_body, "m_LocalScale"),
            })
        if mapping["name"] == "Bust":
            assert len(records) == 153
        assert all(record["batchRootId"] == 0 for record in records), mapping["name"]
        return {
            "version": 1,
            "sceneName": mapping["name"],
            "scenePath": mapping["new"],
            "originalScenePath": mapping["old"],
            "renderers": records,
        }


def mappings():
    return json.loads(SCENE_NAMES.read_text())


def generate():
    return generate_one(next(item for item in mappings() if item["name"] == "Bust"))


def output_name(index, name):
    safe = re.sub(r"[^A-Za-z0-9 _.-]+", "_", name)
    return "%02d_%s.json" % (index, safe)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--all", action="store_true", help="write all scene manifests")
    args = parser.parse_args()
    if not args.all:
        PILOT_OUT.parent.mkdir(parents=True, exist_ok=True)
        PILOT_OUT.write_text(json.dumps(generate(), separators=(",", ":")) + "\n")
        print("153 renderers; position, index and baked UV2 data verified.")
        return
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    counts = []
    for old_file in OUT_DIR.glob("*.json"):
        old_file.unlink()
    for index, mapping in enumerate(mappings()):
        result = generate_one(mapping)
        (OUT_DIR / output_name(index, mapping["name"])).write_text(
            json.dumps(result, separators=(",", ":")) + "\n"
        )
        counts.append((mapping["name"], len(result["renderers"])))
    print("%d scene manifests; %d static-batched renderers." % (
        len(counts), sum(count for _name, count in counts)
    ))


if __name__ == "__main__":
    main()
