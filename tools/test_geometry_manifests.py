"""Regression tests for every static-geometry manifest used by the editor tool."""
import json
import unittest

from audit_static_batches import ROOT, audit
from build_geometry_manifests import EXPORT_ZIP, generate_one, mappings, output_name


@unittest.skipUnless(EXPORT_ZIP.is_file(), "requires the external AssetRipper export ZIP")
class GeometryManifestTests(unittest.TestCase):
    def test_all_manifests_reproducible_and_order_preserved(self):
        names = mappings()
        self.assertEqual(len(names), 56)
        total = 0
        counts = {}
        for index, mapping in enumerate(names):
            path = ROOT / "tools" / "unity-editor" / "MapGeometry" / output_name(index, mapping["name"])
            saved = json.loads(path.read_text())
            generated = generate_one(mapping)  # Includes position, index and atlas UV checks.
            self.assertEqual(saved, generated, mapping["name"])
            self.assertEqual(saved["sceneName"], mapping["name"])
            self.assertNotIn("/", output_name(index, mapping["name"]))
            counts[mapping["name"]] = len(saved["renderers"])
            total += counts[mapping["name"]]
        self.assertEqual(total, 3891)
        self.assertEqual(counts["Bust"], 153)
        self.assertEqual(counts["Menu"], 12)
        self.assertEqual(counts["Logo"], 0)
        self.assertEqual(counts["AwakeScene"], 0)

    def test_extraction_invariants_for_every_scene(self):
        for mapping in mappings():
            report = audit(mapping["name"])
            self.assertEqual(sum(len(mesh["missing"]) for mesh in report["coverage"]), 0)
            self.assertEqual(sum(len(mesh["repeated"]) for mesh in report["coverage"]), 0)

    def test_manifest_scene_and_component_ids(self):
        for mapping in mappings():
            data = generate_one(mapping)
            self.assertTrue(data["scenePath"].endswith(".unity"))
            self.assertTrue(data["originalScenePath"].endswith(".unity"))
            ids = set()
            for record in data["renderers"]:
                self.assertGreater(record["rendererId"], 0)
                self.assertGreater(record["filterId"], 0)
                self.assertGreater(record["transformId"], 0)
                self.assertEqual(record["batchRootId"], 0)
                self.assertGreater(len(record["subsets"]), 0)
                self.assertEqual(len(record["localPosition"]), 3)
                self.assertEqual(len(record["localRotation"]), 4)
                self.assertEqual(len(record["localScale"]), 3)
                self.assertNotIn(record["rendererId"], ids)
                ids.add(record["rendererId"])


if __name__ == "__main__":
    unittest.main()
