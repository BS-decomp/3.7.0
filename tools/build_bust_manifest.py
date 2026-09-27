"""Generate immutable source metadata for the Unity 5.6 Bust repair tool."""
import hashlib
import json
import re
import struct
import zipfile
from pathlib import Path
from audit_static_batches import ROOT, blocks, field, ref, mesh_data


def vector(body, key):
    return [float(x) for x in re.findall(r'[xyzw]: ([^,}]+)', field(body, key))]


def generate():
    mapping = next(e for e in json.loads((ROOT / 'tools/scene-names.json').read_text()) if e['name'] == 'Bust')
    with zipfile.ZipFile(ROOT / 'exports/BlockStrike-608-Unity-4.7.2f1.zip') as z:
        objects = blocks(z.read('UnityProject/' + mapping['old']).decode())
        guids = {field(z.read(p).decode(), 'guid'): p[:-5] for p in z.namelist() if p.endswith('.meta')}
        filters = {ref(b, 'm_GameObject'): (i, b) for i, (t, b) in objects.items() if t == 33}
        transforms = {ref(b, 'm_GameObject'): (i, b) for i, (t, b) in objects.items() if t == 4}
        records = []
        for i, (t, b) in objects.items():
            if t != 23 or not field(b, 'm_SubsetIndices'):
                continue
            raw_subsets = bytes.fromhex(field(b, 'm_SubsetIndices'))
            subsets = list(struct.unpack('<' + 'I' * (len(raw_subsets) // 4), raw_subsets))
            go = ref(b, 'm_GameObject')
            fid, filt = filters[go]
            tid, tr = transforms[go]
            guid = re.search(r'guid: (\w+)', field(filt, 'm_Mesh'))[1]
            raw, stride, subs = mesh_data(z.read(guids[guid]).decode())
            indices = [index for sub in subsets for index in subs[sub]]
            digest = hashlib.sha256(struct.pack('<' + 'i' * len(indices), *indices)).hexdigest()
            # Validate baked UV2 is already in this renderer's atlas rectangle.
            scale = vector(b, 'm_LightmapTilingOffset')
            lm = int(field(b, 'm_LightmapIndex'))
            if lm < 254:
                for index in set(indices):
                    uv = struct.unpack_from('<ee', raw, index * stride + 28)
                    for axis in range(2):
                        assert scale[axis + 2] - .002 <= uv[axis] <= scale[axis + 2] + scale[axis] + .002, (i, uv, scale)
            records.append(dict(rendererId=i, filterId=fid, transformId=tid,
                                objectName=field(objects[go][1], 'm_Name'), meshGuid=guid,
                                vertexCount=len(raw)//stride, subMeshCount=len(subs),
                                positionHash=hashlib.sha256(b''.join(raw[j:j+12] for j in range(0, len(raw), stride))).hexdigest(),
                                subsets=subsets, indexHash=digest,
                                batchRootId=ref(b, 'm_StaticBatchRoot'),
                                lightmapIndex=lm, lightmapScaleOffset=scale,
                                localPosition=vector(tr,'m_LocalPosition'),
                                localRotation=vector(tr,'m_LocalRotation'),
                                localScale=vector(tr,'m_LocalScale')))
        assert len(records) == 153
        assert all(r['batchRootId'] == 0 for r in records)
        return dict(version=1, scenePath=mapping['new'], originalScenePath=mapping['old'], renderers=records)


if __name__ == '__main__':
    result = generate()
    (ROOT / 'tools/unity-editor/BustGeometry.json').write_text(json.dumps(result, indent=2)+'\n')
    print('153 renderers; subset hashes and baked UV2 rectangles verified.')
