# Android: everything is blown out (GitHub issue #1)

## Symptom

On an Android build every map is far too bright (lightmapped surfaces are white/washed out). The
editor on Windows looks fine. The workaround from the issue was Color Space = Linear plus
OpenGLES3 first in the Android graphics APIs.

## Cause

The maps use `Mobile/Unlit (Supports Lightmap)` (54 maps, 60 materials), plus a few
`Mobile/VertexLit` and `Mobile/VertexLit (Only Directional Lights)` materials.

* The Unity 4.7 originals are fixed-function shaders with two lightmap passes: `VertexLM`
  (dLDR: `lightmap * main * double`) and `VertexLMRGBM` (`lightmap * lightmap.a * double`, then
  `* main * quad`). The engine chooses one.
* The recovered `LightmapFar-0.png` files are **dLDR** data from the Android build: RGB, no
  alpha, stored as intensity / 2. The 4.7 GLES program in the APK decodes them as
  `main.rgb * (2.0 * lightmap.rgb)` (see `tools/shader-extract/shader_37_*`).
* On Android/iOS targets Unity 5.5/5.6 runs these legacy passes as RGBM (known bug, Unity case
  882363: "Mobile/Unlit (Supports Lightmap) causes overbright scene on Android or iOS target").
  With alpha = 1 the RGBM pass multiplies by 8 instead of 2, so the lighting comes out about
  4x too bright. In Linear colour space the same multipliers are applied in linear space, which is
  why the workaround appears to help.
* We only ever checked lighting in the editor (Windows target), which selects a different path.

The shaders were not wrong in themselves: they are byte-faithful to the APK. The 4.7 to 5.6
engine change is what broke them. The Android result was not verified on a device before this fix.

## Fix

The decode no longer depends on which encoding the engine assumes.

1. `Mobile-Lightmap-Unlit.shader`, `Mobile-VertexLit.shader`: `VertexLM` and `VertexLMRGBM` are
   now the same CG program: `rgb = main.rgb * (2.0 * unity_Lightmap.rgb)`, with the same UV
   channels as the originals (UV0 = main, UV1 = baked atlas) and tiling/offset, fog kept. The
   non-lightmapped `Vertex` pass and the shadow caster are unchanged.
2. `Mobile-VertexLit-OnlyDirectionalLights.shader`: `DecodeLightmap()` replaced by the explicit
   `2.0 * texture`, the same expression the 4.7 GLES code contains.
3. The 54 `LightmapFar-0.png` importers are switched from the `Lightmap` type to Default
   (`textureType: 0`). The `Lightmap` type makes Unity re-encode the pixels per platform (RGBM on
   desktop, dLDR on mobile). With Default the raw pixels reach the shader on every platform, so
   Android and the editor match. `BlockStrikeLightmapRecovery` was updated to do this too.

No change to Color Space (stays Gamma, as in the original game) or to the Android graphics APIs
(OpenGLES2, `m_APIs: 08000000`) is needed. **Do not switch to Linear**: the shaders and the NGUI
UI were built for Gamma, and the lightmap maths above assumes Gamma.

The client shaders intentionally differ from `tools/shader-canonical/builtin-era/` for the two
VertexLit files (those keep the byte-faithful 4.7 sources for reference). The installer only
replaces `DummyShaderTextExporter` placeholders, so it will not undo the fix.

## Verification status

Done:
* Shader, `.meta` and editor-tool edits; `tools/test_lightmap_binding.py` and
  `tools/test_lightmap_shaders.py` pass. The maths were checked against the APK GLES code.
* Checked in Unity 5.6.7f1 by the project owner (visual, by eye): the three shaders compile, and the
  maps look the same as in the original game. Switching the editor build target between
  Windows (PC) and Android gives the same picture (no over-exposure on Android). Colour Space is
  Gamma on both targets (`m_ActiveColorSpace: 0`); Android graphics API is still OpenGLES2.

Not done:
* A build installed and run on a real Android device.
* A pixel-level comparison against the original APK (the comparison so far is by eye).

If lightmapped surfaces ever look flat/unlit instead of too bright on some target, the engine did
not pick a `VertexLM*` pass there; the shader can then be switched to an always-on pass.
