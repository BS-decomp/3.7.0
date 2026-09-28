"""Static export checks. Does not substitute for Unity or PowerShell execution."""
import json
import re
import unittest
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
EXPORT_ZIP = ROOT / 'exports/BlockStrike-608-Unity-4.7.2f1.zip'


@unittest.skipUnless(EXPORT_ZIP.is_file(), "requires the external AssetRipper export ZIP")
class SceneNamesTests(unittest.TestCase):
    def test_mapping_and_references(self):
        entries = json.loads((ROOT / 'tools/scene-names.json').read_text())
        self.assertEqual(len(entries), 56)
        self.assertEqual(len({e['new'].lower() for e in entries}), 56)
        self.assertEqual(len({e['name'].lower() for e in entries}), 56)
        with zipfile.ZipFile(ROOT / 'exports/BlockStrike-608-Unity-4.7.2f1.zip') as z:
            build = z.read('UnityProject/ProjectSettings/EditorBuildSettings.asset').decode()
            old_paths = re.findall(r'    path: (.+)', build)
            for e in entries:
                self.assertIn('UnityProject/' + e['old'], z.namelist())
                self.assertIn('UnityProject/' + e['old'] + '.meta', z.namelist())
                self.assertNotIn('UnityProject/' + e['new'], z.namelist())
                self.assertEqual(build.count(e['old']), 1)
                build = build.replace(e['old'], e['new'])
            new_paths = re.findall(r'    path: (.+)', build)
            mapping = {e['old']: e['new'] for e in entries}
            self.assertEqual(new_paths, [mapping[p] for p in old_paths])
            self.assertEqual(new_paths[:3], ['Assets/Levels/AwakeScene.unity', 'Assets/Levels/Logo.unity', 'Assets/Levels/Menu.unity'])
            # No other serialized/script encrypted-name references to rewrite.
            for p in z.namelist():
                if p.endswith(('.cs', '.unity', '.prefab', '.asset')) and not p.endswith('EditorBuildSettings.asset'):
                    self.assertNotIn(b'SXZtRDE', z.read(p), p)

    def test_build_recovery_template(self):
        template = (ROOT / 'tools/EditorBuildSettings.original.asset').read_bytes()
        entries = json.loads((ROOT / 'tools/scene-names.json').read_text())
        with zipfile.ZipFile(ROOT / 'exports/BlockStrike-608-Unity-4.7.2f1.zip') as z:
            self.assertEqual(template, z.read('UnityProject/ProjectSettings/EditorBuildSettings.asset'))
            text = template.decode()
            for entry in entries:
                meta = z.read('UnityProject/' + entry['old'] + '.meta').decode()
                guid = re.search(r'^guid: ([0-9a-fA-F]{32})\s*$', meta, re.M).group(1)
                line = '    path: ' + entry['old']
                self.assertEqual(text.count(line), 1)
                text = text.replace(line, line + '\n    guid: ' + guid)
                text = text.replace(entry['old'], entry['new'])
            self.assertEqual(len(re.findall(r'^    guid: [0-9a-f]{32}$', text, re.M)), 56)
            self.assertEqual(len(re.findall(r'^  - enabled: 1$', text, re.M)), 56)
            self.assertNotIn('SXZtRDE', text)
            self.assertEqual(re.findall(r'^    path: (.+)$', text, re.M)[:3],
                             ['Assets/Levels/AwakeScene.unity', 'Assets/Levels/Logo.unity', 'Assets/Levels/Menu.unity'])

    def test_loader_patches(self):
        with zipfile.ZipFile(ROOT / 'exports/BlockStrike-608-Unity-4.7.2f1.zip') as z:
            code = z.read('UnityProject/Assets/Scripts/Assembly-CSharp/LevelManager.cs').decode()
        pairs = [
            ('\t\tloadedLevelName = loadedLevelName.Replace("#", "/");\n\t\treturn Utils.Decrypt(loadedLevelName);', '\t\treturn loadedLevelName; // Recovered readable scene names.'),
            ('\t\tstring text = Utils.Encrypt(name);\n\t\treturn text.Replace("/", "#");', '\t\treturn name; // Recovered readable scene names.'),
        ]
        # Support both original source and the API Updater's replacement API.
        for text in [code, code.replace('Application.loadedLevelName', 'UnityEngine.SceneManagement.SceneManager.GetActiveScene().name')]:
            for old, new in pairs:
                self.assertEqual(text.count(old), 1)
                text = text.replace(old, new)
                self.assertEqual(text.count(new), 1)
            self.assertNotIn('Utils.Decrypt', text)
            self.assertNotIn('Utils.Encrypt', text)


if __name__ == '__main__':
    unittest.main()
