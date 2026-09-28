"""Validate static-batch subset extraction from the original export, without edits.

Outputs a compact report. This is NOT yet a Unity scene repair tool.
Only handles the explicitly checked Unity 4 uncompressed, interleaved layout.
"""
import argparse
import collections
import json
import re
import struct
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent


def field(text, name):
    m = re.search(r'^[ \t]*' + re.escape(name) + r':[ \t]*(.*)$', text, re.M)
    if not m:
        raise ValueError('Missing field ' + name)
    return m[1]


def blocks(text):
    return {int(i): (int(t), body) for t, i, body in re.findall(
        r'--- !u!(\d+) &(\d+)\n(.*?)(?=--- !u!|\Z)', text, re.S)}


def ref(text, name):
    return int(re.search(r'fileID: (\d+)', field(text, name))[1])


def mesh_data(text):
    sub_section = text.split('  m_SubMeshes:\n', 1)[1].split('  m_Shapes:', 1)[0]
    submeshes = []
    index_bytes = bytes.fromhex(field(text, 'm_IndexBuffer'))
    assert len(index_bytes) % 2 == 0
    vertex_section = text.split('  m_VertexData:\n', 1)[1].split('  m_CompressedMesh:', 1)[0]
    vertex_count = int(field(vertex_section, 'm_VertexCount'))
    streams = vertex_section.split('    m_Streams:\n', 1)[1].split('    m_DataSize:', 1)[0]
    stream_blocks = re.split(r'    - channelMask: ', streams)[1:]
    assert int(stream_blocks[0].splitlines()[0]) != 0
    assert all(int(b.splitlines()[0]) == 0 for b in stream_blocks[1:])
    assert int(field(stream_blocks[0], 'offset')) == 0
    stride = int(field(stream_blocks[0], 'stride'))
    channels = vertex_section.split('    m_Channels:\n', 1)[1].split('    m_Streams:', 1)[0]
    position_channel = re.split(r'    - stream: ', channels)[1]
    assert position_channel.splitlines()[0] == '0'
    assert int(field(position_channel, 'offset')) == 0
    assert int(field(position_channel, 'format')) == 0
    assert int(field(position_channel, 'dimension')) == 3
    raw = bytes.fromhex(field(vertex_section, '_typelessdata'))
    assert len(raw) == vertex_count * stride == int(field(vertex_section, 'm_DataSize'))
    for body in re.split(r'  - serializedVersion: 2\n', sub_section)[1:]:
        first, count = int(field(body, 'firstByte')), int(field(body, 'indexCount'))
        assert int(field(body, 'topology')) == 0 and count % 3 == 0
        assert first % 2 == 0 and first + count * 2 <= len(index_bytes)
        indices = struct.unpack('<' + 'H' * count, index_bytes[first:first + count * 2])
        assert all(i < vertex_count for i in indices)
        submeshes.append(indices)
    return raw, stride, submeshes


def audit(scene):
    with zipfile.ZipFile(ROOT / 'exports/BlockStrike-608-Unity-4.7.2f1.zip') as z:
        names = json.loads((ROOT / 'tools/scene-names.json').read_text())
        entry = next(e for e in names if e['name'] == scene)
        objects = blocks(z.read('UnityProject/' + entry['old']).decode())
        guids = {}
        for name in z.namelist():
            if name.endswith('.meta'):
                guids[field(z.read(name).decode(), 'guid')] = name[:-5]
        filters = {ref(b, 'm_GameObject'): b for t, b in objects.values() if t == 33}
        meshes, usage, output = {}, collections.defaultdict(collections.Counter), []
        for renderer_id, (kind, body) in objects.items():
            if kind != 23:
                continue
            subset_hex = field(body, 'm_SubsetIndices')
            if not subset_hex:
                continue
            subset_raw = bytes.fromhex(subset_hex)
            assert len(subset_raw) % 4 == 0
            subsets = struct.unpack('<' + 'I' * (len(subset_raw) // 4), subset_raw)
            go = ref(body, 'm_GameObject')
            guid = re.search(r'guid: (\w+)', field(filters[go], 'm_Mesh'))[1]
            if guid not in meshes:
                meshes[guid] = mesh_data(z.read(guids[guid]).decode())
            raw, stride, submeshes = meshes[guid]
            materials = body.split('  m_Materials:\n', 1)[1].split('  m_SubsetIndices:', 1)[0]
            material_count = len(re.findall(r'^  - ', materials, re.M))
            assert len(subsets) == material_count, (renderer_id, 'material count')
            selected = [submeshes[i] for i in subsets]
            # Split/reindex while retaining every byte of every used vertex.
            # No transform/UV changes here: those need separate reconstruction.
            used = sorted({v for indices in selected for v in indices})
            remap = {v: i for i, v in enumerate(used)}
            new_raw = b''.join(raw[i*stride:(i+1)*stride] for i in used)
            for indices in selected:
                rebuilt = [remap[i] for i in indices]
                assert len(rebuilt) == len(indices)
                for old, new in zip(indices, rebuilt):
                    assert new_raw[new*stride:(new+1)*stride] == raw[old*stride:(old+1)*stride]
            positions = [struct.unpack_from('<fff', new_raw, i*stride) for i in range(len(used))]
            bounds = [[min(p[a] for p in positions), max(p[a] for p in positions)] for a in range(3)]
            usage[guid].update(subsets)
            output.append(dict(renderer=renderer_id, game_object=go,
                               name=field(objects[go][1], 'm_Name'), mesh=guids[guid],
                               subsets=list(subsets), materials=material_count,
                               vertices=len(used), triangles=sum(map(len, selected)) // 3,
                               combined_space_bounds=bounds))
        coverage = []
        for guid, (_, stride, subs) in meshes.items():
            coverage.append(dict(mesh=guids[guid], stride=stride, submeshes=len(subs),
                                 selected=len(usage[guid]), missing=sorted(set(range(len(subs))) - set(usage[guid])),
                                 repeated={str(i): count for i, count in usage[guid].items() if count > 1}))
        return dict(scene=scene, renderers=len(output), coverage=coverage, objects=output,
                    note='Byte-exact subset split validated, not yet local-space/UV reconstruction or Unity rendering.')


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--scene', default='Bust')
    parser.add_argument('--output', default='.cache/bust-static-batch-audit.json')
    args = parser.parse_args()
    result = audit(args.scene)
    out = Path(args.output)
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps({k: result[k] for k in ['scene', 'renderers', 'coverage', 'note']}, indent=2))
