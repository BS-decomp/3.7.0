"""Reproduce the 608 scene map; requires pycryptodome. Does not modify the export."""
import base64
import hashlib
import json
import re
import zipfile
from pathlib import Path
from Crypto.Cipher import AES, DES
from Crypto.Util.Padding import unpad

root = Path(__file__).resolve().parent.parent
with zipfile.ZipFile(root / 'samples/com.rexetstudio.blockstrike-608.apk') as apk, zipfile.ZipFile(root / 'exports/BlockStrike-608-Unity-4.7.2f1.zip') as export:
    source = export.read('UnityProject/Assets/Scripts/Assembly-CSharp/UIFontControl.cs').decode()
    strings = []
    for text in re.findall(r'DecryptString\("([^"]+)', source):
        data = base64.b64decode(text)
        strings.append(unpad(AES.new(b'defaultKeyString', AES.MODE_CBC, data[:16]).decrypt(data[16:]), 16).decode())
    assert strings[::2] == ['jar:file://', 'jar:file://']
    sizes = [apk.getinfo(s.removeprefix('!/')).file_size for s in strings[1::2]]
    # Serialized UIFontControl.fontSize is zero in the first build scene.
    guid = export.read('UnityProject/Assets/Scripts/Assembly-CSharp/UIFontControl.cs.meta').decode().split('guid: ')[1].splitlines()[0]
    boot = [export.read(n).decode() for n in export.namelist() if n.endswith('.unity') and guid.encode() in export.read(n)]
    assert len(boot) == 1 and re.search(r'fontSize: 0\s*$', boot[0])
    password = ''.join(map(str, sizes)).encode()
    salt = b'IvmD123A12'
    key = hashlib.pbkdf2_hmac('sha1', password, salt, 555, 8)
    scenes = []
    for path in export.namelist():
        if not path.endswith('.unity'):
            continue
        old = Path(path).stem
        data = base64.b64decode(old.replace('#', '/'))
        assert data[:10] == salt
        # Original Mono DES uses the first eight bytes of the supplied IV;
        # all ten bytes remain in the salt and serialized prefix.
        name = unpad(DES.new(key, DES.MODE_CBC, salt[:8]).decrypt(data[10:]), 8).decode()
        assert name and not re.search(r'[<>:"/\\|?*\x00-\x1f]', name)
        scenes.append(dict(old=path.removeprefix('UnityProject/'), new=str(Path(path).with_name(name + '.unity')).removeprefix('UnityProject/'), name=name))
    assert len(scenes) == 56 and len({s['name'].lower() for s in scenes}) == 56
    (root / 'tools/scene-names.json').write_text(json.dumps(scenes, indent=2) + '\n')
    print('Recovered 56 unique scene names from APK-derived key.')
