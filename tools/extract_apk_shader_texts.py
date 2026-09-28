#!/usr/bin/env python3
"""Extract embedded decompiled ShaderLab texts from Block Strike 608 (Unity 4.7.2) APK.

Unity 4.x serializes Shader assets with a full decompiled text block:
original shader name, Properties, subshader/pass render state, plus the
compiled GLES/GLES3 programs of every programmable pass. AssetRipper ignores
that block and writes DummyShaderTextExporter placeholders instead, so this
extractor recovers the real data straight from the APK.

Usage:
    python3 tools/extract_apk_shader_texts.py \
        --apk samples/com.rexetstudio.blockstrike-608.apk \
        --out scratch/shader-extract

Writes:
    <out>/shader_<NN>_<safe-name>.txt  - raw decompiled texts
    <out>/inventory.json               - index: name, properties, source file, offset
"""
import argparse
import io
import json
import os
import re
import struct
import zipfile

TEXT_BYTES = set(range(32, 127)) | {9, 10, 13}


def looks_like_text(buf: bytes) -> bool:
    return all(b in TEXT_BYTES for b in buf)


def find_shader_texts(data: bytes):
    """Yield (offset, text) for every length-framed 'Shader \"...\"' text block."""
    start = 0
    marker = b'Shader "'
    while True:
        i = data.find(marker, start)
        if i < 0:
            return
        start = i + len(marker)
        if i < 4:
            continue
        (ln,) = struct.unpack_from('<i', data, i - 4)
        if not (8 < ln < 300000):
            continue
        blob = data[i:i + ln]
        if len(blob) != ln or not looks_like_text(blob):
            continue
        try:
            text = blob.decode('ascii')
        except UnicodeDecodeError:
            continue
        # A real block must contain a Properties or SubShader section.
        if 'SubShader' not in text:
            continue
        yield i, text


def parse_shader(text: str):
    name = re.search(r'Shader\s+"([^"]+)"', text).group(1)
    props = []
    m = re.search(r'Properties\s*\{(.*?)\n\}', text, re.S)
    if m:
        for line in m.group(1).splitlines():
            line = line.strip()
            pm = re.match(r'(\w+)\s+\("([^"]*)",\s*([\w()\d,\. ]+)\)\s*=\s*(.*)', line)
            if pm:
                props.append({
                    'name': pm.group(1),
                    'label': pm.group(2),
                    'type': pm.group(3).strip(),
                    'default': pm.group(4).strip(),
                })
    has_program = 'Program "vp"' in text or 'SubProgram' in text
    states = []
    for kw in ('LOD', 'Blend ', 'ZWrite', 'Cull', 'ColorMask', 'Offset', 'Fog',
               'AlphaTest', 'Lighting', 'SetTexture', 'Tags {'):
        if kw in text:
            states.append(kw.strip())
    return name, props, has_program, states


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--apk', required=True)
    ap.add_argument('--out', required=True)
    args = ap.parse_args()

    os.makedirs(args.out, exist_ok=True)
    zf = zipfile.ZipFile(args.apk)
    # Concatenate split serialized files from assets/bin/Data.
    members = {}
    for info in zf.infolist():
        p = info.filename
        if not p.startswith('assets/bin/Data/'):
            continue
        base = os.path.basename(p)
        if base.endswith('.resS') or base in ('Managed', 'Resources', 'settings.xml',
                                              'splash.png', 'unity default resources'):
            continue
        # group split parts
        root = re.sub(r'\.split\d+$', '', base)
        members.setdefault(root, []).append(p)

    inventory = []
    seen = set()
    idx = 0
    for root in sorted(members):
        parts = sorted(members[root], key=lambda p: (len(p), p))
        blob = b''
        offsets = []
        for p in parts:
            offsets.append(len(blob))
            blob += zf.read(p)
        for off, text in find_shader_texts(blob):
            name, props, has_program, states = parse_shader(text)
            key = (root, off)
            if key in seen:
                continue
            seen.add(key)
            idx += 1
            safe = re.sub(r'[^A-Za-z0-9._-]+', '_', name)
            fname = f'shader_{idx:02d}_{safe}.txt'
            with open(os.path.join(args.out, fname), 'w', newline='\n') as fh:
                fh.write(text)
            inventory.append({
                'index': idx,
                'file': fname,
                'shaderName': name,
                'container': root,
                'hasCompiledPrograms': has_program,
                'properties': props,
                'stateTokens': states,
            })
            print(f'{idx:3d}  {name}')

    with open(os.path.join(args.out, 'inventory.json'), 'w', newline='\n') as fh:
        json.dump(inventory, fh, indent=2)
    print(f'extracted {len(inventory)} shader texts -> {args.out}')


if __name__ == '__main__':
    main()
