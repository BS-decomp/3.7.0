"""Source-data and transform tests, not an execution of the Unity editor tool."""
import json
import math
import struct
import unittest
import zipfile
from audit_static_batches import ROOT, audit, blocks, field, ref, mesh_data
from build_bust_manifest import generate, vector


def add(a, b): return [x+y for x, y in zip(a, b)]
def neg(a): return [-x for x in a]
def cross(a, b): return [a[1]*b[2]-a[2]*b[1], a[2]*b[0]-a[0]*b[2], a[0]*b[1]-a[1]*b[0]]
def rotate(v, q):
    length = math.sqrt(sum(x*x for x in q))
    q = [x/length for x in q]
    t = [2*x for x in cross(q[:3], v)]
    return add(v, add([q[3]*x for x in t], cross(q[:3], t)))


class BustRecoveryTests(unittest.TestCase):
    def test_manifest_reproducible(self):
        saved = json.loads((ROOT/'tools/unity-editor/BustGeometry.json').read_text())
        self.assertEqual(saved, generate())  # Includes original atlas UV bounds checks.
        self.assertEqual(len({r['rendererId'] for r in saved['renderers']}), 153)
        self.assertTrue(all(r['batchRootId'] == 0 for r in saved['renderers']))

    def test_no_lost_or_duplicate_submeshes(self):
        report = audit('Bust')
        self.assertEqual(report['renderers'], 153)
        self.assertEqual(len(report['coverage']), 1)
        coverage = report['coverage'][0]
        self.assertEqual(coverage['selected'], 248)
        self.assertEqual(coverage['missing'], [])
        self.assertEqual(coverage['repeated'], {})

    def test_all_selected_positions_world_local_roundtrip(self):
        data = generate()
        with zipfile.ZipFile(ROOT/'exports/BlockStrike-608-Unity-4.7.2f1.zip') as z:
            scene = blocks(z.read('UnityProject/'+data['originalScenePath']).decode())
            raw, stride, subsets = mesh_data(z.read('UnityProject/Assets/Mesh/Combined Mesh (root_ scene)_41.asset').decode())
        checked = 0
        max_error = 0
        for record in data['renderers']:
            tr_id = record['transformId']
            chain = []
            visited = set()
            while tr_id:
                self.assertNotIn(tr_id, visited)
                visited.add(tr_id)
                kind, body = scene[tr_id]
                self.assertEqual(kind, 4)
                chain.append((vector(body, 'm_LocalPosition'), vector(body, 'm_LocalRotation'), vector(body, 'm_LocalScale')))
                tr_id = ref(body, 'm_Father')
            indices = {v for sub in record['subsets'] for v in subsets[sub]}
            for index in indices:
                world = struct.unpack_from('<fff', raw, index*stride)
                local = list(world)
                for pos, rot, scale in reversed(chain):
                    local = rotate(add(local, neg(pos)), [-rot[0], -rot[1], -rot[2], rot[3]])
                    self.assertTrue(all(abs(v) > 1e-10 for v in scale))
                    local = [v/s for v, s in zip(local, scale)]
                # Match the stored float32 local mesh positions, not unlimited precision.
                restored = list(struct.unpack('<fff', struct.pack('<fff', *local)))
                for pos, rot, scale in chain:
                    restored = add(rotate([v*s for v, s in zip(restored, scale)], rot), pos)
                error = math.sqrt(sum((a-b)**2 for a, b in zip(world, restored)))
                max_error = max(max_error, error)
                self.assertLess(error, .0001)
                checked += 1
        self.assertGreater(checked, 9000)
        print('Bust selected vertex records:', checked, 'max float32 roundtrip error:', max_error)


if __name__ == '__main__':
    unittest.main()
