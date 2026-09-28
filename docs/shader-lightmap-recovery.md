# Shader and lightmap recovery (Block Strike 608, Unity 4.7.2 → 5.6)

## What turned out to be true

Every shader the game uses is present inside the APK in full. Unity 4.x
serializes each Shader asset with a complete **decompiled text block**: the
original shader name, the Properties block, per-pass render state (tags, LOD,
Blend, Cull, ZWrite, ColorMask, Offset, Fog, fixed-function SetTexture stages)
and the compiled GLES2/GLES3 programs of every CG pass.

AssetRipper ignores that block and writes `DummyShaderTextExporter`
placeholders for anything that is not pure fixed-function. That is why the
export contained 43 `.shader` files where 35 were dummies: all 24 NGUI UI
shaders, the MADFINGER god rays, the ProBuilder helpers and 5 Unity built-ins
had CG programs and were lost by the exporter, while the 8 fixed-function
shaders survived.

`tools/extract_apk_shader_texts.py` recovers the embedded texts straight from
the APK into `tools/shader-extract/` (47 shaders: the 43 exported ones plus
`VertexLit`, `Diffuse`, `UI/Default`, `UI/Default Font` from
`unity_builtin_extra`, which are the stock editor built-ins and need no files).

The 8 fixed-function "survivors" match the extraction property-for-property,
which validates the extractor end to end.

## Restoration method per shader

| Shader | Source of truth | How restored |
| --- | --- | --- |
| 24 × NGUI `Unlit - *` | classic NGUI 3.x sources (`tools/shader-canonical/ngui/`) | era-normalized by `tools/build_shader_recovery.py` (newer NGUI revisions' `DisableBatching`, instancing/stereo macros, `UnityObjectToClipPos` and `_MainTex_ST` handling removed; `ColorMask RGB` restored exactly where the APK pass had it) |
| 4 × ProBuilder 2.x | Unity-Technologies probuilder-vr era sources (`tools/shader-canonical/probuilder/`) | same era normalization, dead uniforms trimmed |
| 2 × MADFINGER god rays | free MADFINGER shader pack mirrors (`tools/shader-canonical/madfinger/`) | blinking formula verified constant-for-constant against APK GLSL (56.7272 / 0.6366 / 2π) |
| 5 × Unity built-ins | hand-restored era-authentic Unity 4.x built-in sources (`tools/shader-canonical/builtin-era/`) | every pass/variant cross-checked against the embedded GLSL (e.g. particles have only SOFTPARTICLES_OFF/ON, no fog code, `2.0 * color * tint * tex`, no HDR clamp) |

`tools/build_shader_recovery.py` mechanically verifies all 35 outputs against
the extraction: identical shader name, identical Properties (name, label,
type, default), matching pass render state, matching fixed-function structure
and matching user-uniform sets. It exits non-zero on any mismatch.

### Fidelity notes

- Names, Properties and pass state are **byte-faithful** to what the APK was
  built with.
- CG bodies are the original third-party sources of the same era, selected so
  they compile to the same programs Unity 4.7 baked into the APK. Where only
  the compiled GLSL exists (e.g. custom formatting/comments), the original
  hand-written comments and whitespace are unrecoverable - behaviour, not
  bytes, is restored there.
- SubShader fallbacks that Unity 4.7 stripped from the Android build (NGUI
  LOD 100 fixed-function subshader, particle dual/single texture card
  fallbacks) are included again from the upstream sources; they never execute
  on GLES2+ but restore the original project content.
- The four shaders found only inside `unity_builtin_extra` are the stock
  editor built-ins Unity ships with the editor; no project files are needed
  or wanted for them.

## Applying to the project

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Install-ShaderRecovery.ps1 -ProjectPath "$env:USERPROFILE\Desktop\BlockStrike-Unity56\UnityProject"
```

The installer copies the 35 rebuilt shaders over the placeholder files
(`Assets/Shader/` and `Assets/Resources/shaders/`), leaves every `.meta`
untouched (material GUID links are preserved), skips files already recovered
(re-runnable), and stores backups under `RecoveryBackups/`.

## Lightmaps

The 54 `LightmapFar-0.png` textures were preserved and their parent folders
were renamed to the recovered scene names while keeping their GUIDs. **That did
not preserve the scene bindings.** The earlier statement that scene references
still pointed to the texture GUIDs was incorrect for the committed Unity 5.6
scenes.

An audit of `client/` found, for all 54 map scenes, a null
`m_LightingDataAsset`, no reference to the corresponding PNG GUID, and no
recovered `LightingData.asset` / `LightmapSnapshot`. Unity 5.6 renderer records
also have no serialized lightmap index/offset. The original information needed
to rebuild the links survives in the geometry manifests: all 3,196 lightmapped
renderer IDs use index 0, with original scale/offset values. The recovered
meshes already contain the atlas mapping in UV2; geometry repair therefore
normalizes renderer scale/offset to identity.

`tools/unity-editor/LegacyLightmapBinder.cs` and
`tools/unity-editor/BlockStrikeLightmapRecovery.cs` provide a reversible
editor operation. It preflights all scenes, sets each PNG to Unity's Lightmap
import type, and adds an edit-mode/runtime binder that assigns the texture to
`LightmapSettings.lightmaps` and sets the original renderers to index 0. It
refuses to proceed until geometry recovery is applied. The tool backs up scenes
and texture `.meta` files outside `Assets`; no bake is run.

See [`lightmap-binding.md`](lightmap-binding.md) for audit details, menu
commands and verification limits. The binding has not yet been applied to the
committed scenes, and its rendered output has not been tested in Unity 5.6.7f1.

The path normalizer still performs only the GUID-preserving folder rename:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Normalize-LightmapPaths.ps1 -ProjectPath "$env:USERPROFILE\Desktop\BlockStrike-Unity56\UnityProject"
```

## Known limits

- Occlusion culling data is Unity 4-era and will not work in 5.6; rebake
  per scene if needed (independent of lightmaps).
- Visual comparison against the running APK is still pending; mechanical
  recovery checks do not prove pixel-perfect lighting or gameplay equivalence.
