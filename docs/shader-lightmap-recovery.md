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

All 54 `LightmapFar-0.png` textures exist in the export; only their parent
folders still carry the DES-encrypted scene stems
(`Assets/Levels/.../<Map>/SXZtRDEyM0Ex...`). Scene references point at the
texture GUIDs, so normalizing the folders while keeping every `.meta`
preserves all links.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Normalize-LightmapPaths.ps1 -ProjectPath "$env:USERPROFILE\Desktop\BlockStrike-Unity56\UnityProject"
```

The normalizer renames each encrypted folder to the recovered scene name
(`Better/LightmapFar-0.png` next to `Better.unity` etc.), moves the folder
`.meta` too, verifies every texture still has its `.meta`, and reports per
scene. Unity must be closed; it reimports on next open.

## Known limits

- Occlusion culling data is Unity 4-era and will not work in 5.6; rebake
  per scene if needed (independent of lightmaps).
- The visual check against the running APK is still pending the user's PC;
  all mechanical equivalence checks pass.
