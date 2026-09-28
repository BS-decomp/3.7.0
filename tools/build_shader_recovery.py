#!/usr/bin/env python3
"""Build replacement .shader files for the 35 DummyShaderTextExporter placeholders.

Sources (all era-authentic, verified against the APK-extracted ground truth in
tools/shader-extract/):

  * tools/shader-canonical/ngui/       - classic NGUI 3.x family (24)
  * tools/shader-canonical/probuilder/ - ProBuilder 2.x (4)
  * tools/shader-canonical/madfinger/  - MADFINGER god rays (2)
  * tools/shader-canonical/builtin-era/- Unity 4.x built-ins (5)

NGUI files additionally get era-normalized (newer NGUI revisions added
DisableBatching tags, GPU instancing/stereo macros, UnityObjectToClipPos
calls and _MainTex_ST transforms that do not exist in the 608 build):
  - "DisableBatching" tag lines are removed
  - instancing/stereo macro lines are removed
  - UnityObjectToClipPos(x) -> mul(UNITY_MATRIX_MVP, x)
  - _MainTex_ST transform is removed when the APK program does not use it
  - ColorMask RGB is restored in the CG pass only where the APK pass had it

Every produced file is then verified against its extracted ground truth:
identical shader name, identical Properties block, matching pass render state
and matching user-uniform sets. Exits non-zero on the first verification
failure.
"""
import io
import json
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
EXTRACT_DIR = os.path.join(ROOT, 'tools', 'shader-extract')
CANON_DIR = os.path.join(ROOT, 'tools', 'shader-canonical')
OUT_DIR = os.path.join(ROOT, 'tools', 'unity-editor', 'ShaderRecovery')

# ---------------------------------------------------------------- extraction


def load_inventory():
    with open(os.path.join(EXTRACT_DIR, 'inventory.json'), encoding='utf-8') as fh:
        inv = json.load(fh)
    for e in inv:
        path = os.path.join(EXTRACT_DIR, e['file'])
        with io.open(path, encoding='utf-8') as fh:
            e['text'] = fh.read()
    return {e['shaderName']: e for e in inv}


BUILTIN_UNIFORMS = {
    '_Time', '_SinTime', '_CosTime', '_ProjectionParams', '_ScreenParams',
    '_ZBufferParams', '_WorldSpaceLightPos0', '_LightColor0',
    '_CameraDepthTexture', '_Object2World', '_World2Object',
    '_LightPositionRange', '_LightMatrix0', '_ShadowOffsets', '_Rotation',
    '_LightBuffer', '_LightShadowData', '_LightTexture0', '_LightTextureB0',
    '_ShadowMapTexture', '_LightSplitsFar', '_LightSplitsNear', '_ShadowMap',
    '_ShadowColor', '_ShadowMatrix',
}


def is_builtin_uniform(name):
    return name in BUILTIN_UNIFORMS or name.startswith(('unity_', 'glstate_'))


def extraction_state_header(entry):
    text = entry['text']
    cut = text.find('Program "vp"')
    return text[:cut] if cut >= 0 else text


def extraction_uniforms(entry):
    uniforms = set()
    for prog in re.findall(r'"!!GLES(?!3)(.*?)"\n\}', entry['text'], re.S):
        src = prog.encode().decode('unicode_escape')
        for m in re.finditer(r'uniform\s+(?:highp |mediump |lowp )*\w+\s+(\w+)', src):
            name = m.group(1)
            if name.startswith('glstate_'):
                continue
            uniforms.add(name)
    return {u for u in uniforms if not u.startswith('glstate_')}


def extraction_properties(entry):
    return [(p['name'], p['label'], p['type'], p['default']) for p in entry['properties']]

# ---------------------------------------------------------------- helpers


def norm_type(t):
    t = re.sub(r'\s+', '', t).lower()
    return re.sub(r'\.0+(?=[,)])', '', t)


def norm_default(d):
    d = d.strip()
    m = re.match(r'^"?\s*\(([^)]*)\)"?$', d)
    if m:  # color/vector: normalize float formatting
        parts = [re.sub(r'\.0+$', '', p.strip()) for p in m.group(1).split(',')]
        return '(' + ','.join(parts) + ')'
    return re.sub(r'\.0+($|[ \t])', r'\1', d).lower()


def final_properties(text):
    m = re.search(r'Properties\s*\{(.*?)\n\s*\}', text, re.S)
    props = []
    if m:
        for line in m.group(1).splitlines():
            line = line.strip()
            pm = re.match(r'(\w+)\s*\("([^"]*)",\s*(.+)\)\s*=\s*(.*)$', line)
            if pm:
                props.append((pm.group(1), pm.group(2), pm.group(3).strip(), pm.group(4).strip()))
    return props


def final_name(text):
    return re.search(r'Shader\s+"([^"]+)"', text).group(1)

# ---------------------------------------------------------------- transforms


def strip_bom(text):
    return text.lstrip('﻿')


def era_transform_common(text):
    text = strip_bom(text)
    # drop the Unity5-era 'Upgrade NOTE' banner comment
    text = re.sub(r'^// Upgrade NOTE:.*\n', '', text)
    # newer revisions added these tags; absent in the 608 build
    text = re.sub(r'^[ \t]*"DisableBatching"[ \t]*=[ \t]*"True"[ \t]*\n', '', text, flags=re.M)
    # instancing/stereo macros are newer than this project
    for macro in ('UNITY_VERTEX_INPUT_INSTANCE_ID', 'UNITY_VERTEX_OUTPUT_STEREO',
                  'UNITY_SETUP_INSTANCE_ID(v);', 'UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);',
                  'UNITY_SETUP_INSTANCE_ID(IN);', 'UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);'):
        text = re.sub(r'^[ \t]*' + re.escape(macro) + r'[ \t]*\n', '', text, flags=re.M)
    text = re.sub(r'UnityObjectToClipPos\s*\(([^;\n]+?)\)', r'mul(UNITY_MATRIX_MVP, \1)', text)
    return text


def era_transform_no_st(text):
    """Remove _MainTex_ST scale/offset handling (APK program does not use it)."""
    text = re.sub(r'^[ \t]*\w*4 _MainTex_ST;[ \t]*\n', '', text, flags=re.M)
    text = re.sub(r'TRANSFORM_TEX\s*\(\s*v\.texcoord\s*,\s*_MainTex\s*\)', 'v.texcoord', text)
    return text


def ensure_colormask(text):
    """Insert ColorMask RGB right after the Blend line of the CG pass."""
    lines = text.splitlines(keepends=True)
    pass_start = None
    for i, line in enumerate(lines):
        if 'CGPROGRAM' in line:
            pass_start = i
            break
    if pass_start is None:
        raise SystemExit('no CGPROGRAM found for ColorMask placement')
    for i in range(pass_start - 1, -1, -1):
        if re.match(r'^\s*Blend\s+\w+', lines[i]):
            indent = lines[i][:len(lines[i]) - len(lines[i].lstrip())]
            lines.insert(i + 1, f'{indent}ColorMask RGB\n')
            return ''.join(lines)
    raise SystemExit('no Blend line found for ColorMask placement')


def build_ngui(name, canonical_text, entry):
    text = era_transform_common(canonical_text)
    if '_MainTex_ST' not in extraction_uniforms(entry):
        text = era_transform_no_st(text)
    if 'ColorMask' in extraction_state_header(entry):
        text = ensure_colormask(text)
    return text


def build_probuilder(name, canonical_text, entry, src_file):
    text = era_transform_common(canonical_text)
    if src_file == 'pb_UnlitVertexColor.shader':
        # dead leftovers from other ProCore shaders; the APK program ignores them
        text = re.sub(r'^[ \t]*float4 _Color;[ \t]*\n', '', text, flags=re.M)
        text = re.sub(r'^[ \t]*float _Scale;[ \t]*\n', '', text, flags=re.M)
    return text


def build_madfinger(name, canonical_text, entry):
    return era_transform_common(canonical_text)


def build_builtin(name, canonical_text, entry):
    return strip_bom(canonical_text)

# ---------------------------------------------------------------- mapping

NGUI_DEST = 'Assets/Resources/shaders'
SHADER_DEST = 'Assets/Shader'

SPECS = []
NGUI_FILES = [
    'Unlit - Premultiplied Colored (TextureClip).shader',
    'Unlit - Premultiplied Colored 1.shader',
    'Unlit - Premultiplied Colored 2.shader',
    'Unlit - Premultiplied Colored 3.shader',
    'Unlit - Premultiplied Colored.shader',
    'Unlit - Text (TextureClip).shader',
    'Unlit - Text 1.shader',
    'Unlit - Text 2.shader',
    'Unlit - Text 3.shader',
    'Unlit - Text.shader',
    'Unlit - Transparent Colored (Packed) (TextureClip).shader',
    'Unlit - Transparent Colored (TextureClip).shader',
    'Unlit - Transparent Colored 1.shader',
    'Unlit - Transparent Colored 2.shader',
    'Unlit - Transparent Colored 3.shader',
    'Unlit - Transparent Colored.shader',
    'Unlit - Transparent Masked 1.shader',
    'Unlit - Transparent Masked 2.shader',
    'Unlit - Transparent Masked 3.shader',
    'Unlit - Transparent Masked.shader',
    'Unlit - Transparent Packed 1.shader',
    'Unlit - Transparent Packed 2.shader',
    'Unlit - Transparent Packed 3.shader',
    'Unlit - Transparent Packed.shader',
]
for f in NGUI_FILES:
    SPECS.append({
        'kind': 'ngui',
        'src': os.path.join(CANON_DIR, 'ngui', f),
        'dest': os.path.join(NGUI_DEST, f),
        'shaderName': None,  # taken from the file, checked against extraction
    })

for dest_name, src_name in [
    ('DiffuseVertexColor.shader', 'DiffuseVertexColor.shader'),
    ('UnlitSolidColor.shader', 'UnlitSolidColor.shader'),
    ('pb_HideVertices.shader', 'pb_HideVertices.shader'),
    ('pb_UnlitVertexColor.shader', 'pb_UnlitVertexColor.shader'),
]:
    SPECS.append({
        'kind': 'probuilder',
        'src': os.path.join(CANON_DIR, 'probuilder', src_name),
        'dest': os.path.join(SHADER_DEST, dest_name),
        'shell': '',
        'shaderName': None,
        'srcName': src_name,
    })

for dest_name, src_name in [
    ('MADFINGER-god-rays.shader', 'MADFINGER-god-rays.shader'),
    ('MADFINGER-blinking-god-rays.shader', 'MADFINGER-blinking-god-rays.shader'),
]:
    SPECS.append({
        'kind': 'madfinger',
        'src': os.path.join(CANON_DIR, 'madfinger', src_name),
        'dest': os.path.join(SHADER_DEST, dest_name),
        'shaderName': None,
    })

for dest_name, src_name in [
    ('Mobile-VertexLit.shader', 'Mobile-VertexLit.shader'),
    ('Mobile-VertexLit-OnlyDirectionalLights.shader', 'Mobile-VertexLit-OnlyDirectionalLights.shader'),
    ('Particle Add.shader', 'Particle Add.shader'),
    ('Particle Alpha Blend.shader', 'Particle Alpha Blend.shader'),
    ('Unlit-Alpha.shader', 'Unlit-Alpha.shader'),
]:
    SPECS.append({
        'kind': 'builtin',
        'src': os.path.join(CANON_DIR, 'builtin-era', src_name),
        'dest': os.path.join(SHADER_DEST, dest_name),
        'shaderName': None,
    })

# ---------------------------------------------------------------- build

STATE_PATTERNS = [
    r'LOD\s+\d+', r'ZWrite\s+\w+', r'Cull\s+\w+', r'Blend\s+\w+\s+\w+',
    r'ColorMask\s+\w+', r'Offset\s+[-\d]+\s*,\s*[-\d]+', r'AlphaTest\s+\w+\s+[\d\.]+',
    r'Lighting\s+\w+', r'Fog\s*\{\s*Mode\s+\w+\s*\}', r'Fog\s*\{\s*Color\s*\([^)]*\)\s*\}',
]


def normalize_state_token(tok):
    tok = re.sub(r'\s+', ' ', tok).strip().lower()
    return re.sub(r'\b0\.(\d)', r'.\1', tok)


def state_tokens(text):
    tokens = set()
    head = text[:text.find('Program "vp"')] if 'Program "vp"' in text else text
    for pat in STATE_PATTERNS:
        for m in re.finditer(pat, head):
            tokens.add(normalize_state_token(m.group(0)))
    return tokens


def user_uniforms_final(text):
    cg = re.findall(r'CGPROGRAM(.*?)ENDCG', text, re.S) + re.findall(r'CGINCLUDE(.*?)ENDCG', text, re.S)
    found = set()
    body = '\n'.join(cg) if cg else text
    for m in re.finditer(r'(?<![\w.])(_\w+)', body):
        name = m.group(1)
        if name.startswith('_gles') or is_builtin_uniform(name):
            continue
        found.add(name)
    return found


def verify(name, final_text, entry, problems):
    ok = True

    def fail(msg):
        nonlocal ok
        ok = False
        problems.append(f'  [{name}] {msg}')

    if final_name(final_text) != name:
        fail(f' shader name mismatch: {final_name(final_text)!r}')

    # Properties, order-sensitive
    want = extraction_properties(entry)
    got = final_properties(final_text)
    if len(want) != len(got):
        fail(f' property count {len(got)} != {len(want)}')
    else:
        for w, g in zip(want, got):
            if w[0] != g[0] or w[1] != g[1]:
                fail(f' property {w[0]} mismatch: {g}')
            elif norm_type(w[2]) != norm_type(g[2]):
                fail(f' property {w[0]} type {g[2]!r} != {w[2]!r}')
            elif norm_default(w[3]) != norm_default(g[3]):
                fail(f' property {w[0]} default {g[3]!r} != {w[3]!r}')

    # Render state: every state token found in the APK text must exist in the final file
    final_tokens = state_tokens(final_text)
    normalized_final = {t.lower() for t in final_tokens}
    for tok in state_tokens(entry['text']):
        if tok.lower() not in normalized_final:
            fail(f' missing render state: {tok}')

    # Fixed-function structure parity (only checked when the APK text has such passes)
    ext_head = extraction_state_header(entry)
    for marker in ('unity_Lightmap', 'unity_LightmapMatrix', 'SetTexture', 'BindChannels', 'Material'):
        if marker in ext_head and marker not in final_text:
            fail(f' missing fixed-function structure: {marker}')

    # User-uniform parity between the APK programs and the final CG source.
    # Unity derives <Property>_ST transform uniforms automatically (notably in
    # surface shaders), so they are not required to appear in the source text.
    derived = {p[0] + '_ST' for p in extraction_properties(entry)}
    ext = {u for u in extraction_uniforms(entry) if not is_builtin_uniform(u)}
    fin = user_uniforms_final(final_text)
    missing = (ext - fin) - derived
    extra = (fin - ext) - derived
    if missing:
        fail(f' uniforms used by APK but absent in file: {sorted(missing)}')
    if extra:
        fail(f' uniforms declared in file but absent in APK: {sorted(extra)}')
    return ok


def main():
    inventory = load_inventory()
    problems = []
    produced = []
    os.makedirs(OUT_DIR, exist_ok=True)
    for spec in SPECS:
        with io.open(spec['src'], encoding='utf-8') as fh:
            canonical = fh.read()
        name = final_name(canonical)
        entry = inventory.get(name)
        if entry is None:
            problems.append(f'  [{name}] no extracted ground truth found')
            continue
        builder = {'ngui': build_ngui,
                   'probuilder': None,
                   'madfinger': build_madfinger,
                   'builtin': build_builtin}[spec['kind']]
        if spec['kind'] == 'probuilder':
            final = build_probuilder(name, canonical, entry, spec['srcName'])
        else:
            final = builder(name, canonical, entry)
        if verify(name, final, entry, problems):
            produced.append((spec['dest'], name))
        dest = os.path.join(OUT_DIR, spec['dest'])
        os.makedirs(os.path.dirname(dest), exist_ok=True)
        text = '// Restored from the Block Strike 608 APK ground truth + era-authentic source.\n// See tools/shader-extract/ and docs/shader-lightmap-recovery.md.\n' + final if spec['kind'] == 'builtin' else final
        with open(dest, 'w', newline='\n') as fh:
            fh.write(text)

    print(f'built {len(SPECS)} shader files -> {os.path.relpath(OUT_DIR, ROOT)}')
    if problems:
        print(f'VERIFICATION FAILED ({len(problems)}):')
        for p in problems:
            print(p)
        sys.exit(1)
    print(f'all {len(produced)} files verified against the 608 APK ground truth:')
    for dest, name in sorted(produced):
        print(f'  {dest}  ::  {name}')


if __name__ == '__main__':
    main()
