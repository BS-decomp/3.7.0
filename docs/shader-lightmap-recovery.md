# Shader and lightmap recovery notes for 608

## What is actually missing

The original APK contains all **43 shader names** used by the current export.
The binary package filenames that look like random hashes are Unity asset GUIDs,
not the scene-name encryption used for level files. Their embedded shader names
are readable, so shader-name decryption is not the blocker.

AssetRipper exported only **8 shaders as real fixed-function/source text**.
The remaining **35 shaders contain `DummyShaderTextExporter` placeholders**:

- 24 NGUI UI shaders:
  - 5 Premultiplied Colored variants;
  - 5 Text variants;
  - 6 Transparent Colored variants;
  - 4 Transparent Masked variants;
  - 4 Transparent Packed variants.
- 4 ProBuilder shaders:
  - Diffuse Vertex Color;
  - Unlit Solid Color;
  - UnlitVertexColor;
  - HideVertices.
- 7 gameplay/particle shaders:
  - Mobile/VertexLit;
  - Mobile/VertexLit (Only Directional Lights);
  - Particles/Additive;
  - Particles/Alpha Blended;
  - Unlit/Transparent;
  - MADFINGER/Transparent/GodRays;
  - MADFINGER/Transparent/Blinking GodRays.

All 43 names were found in the original APK data. Placeholder status therefore
means AssetRipper could not decompile those shader objects into editor-ready
ShaderLab, not that the APK lost them.

## What can be recovered faithfully

The binary shader objects preserve packaged shader metadata and GLES/GLES3
program text. That allows reconstruction of Unity 5.6-compatible ShaderLab with:

- original shader names;
- original render queue/render type/tags;
- ZWrite, Cull, Fog, Offset and ColorMask state;
- Blend equations;
- material properties;
- NGUI clipping masks and texture-clip logic;
- fixed-function texture/lightmap behavior where present;
- particle color and alpha behavior.

The APK does **not** contain the developer's original hand-written `.shader`
source files. A byte-identical or historically identical source restoration is
therefore impossible from this APK alone. The achievable target is functional
and visual equivalence, verified against the original rendering behavior.
Each replacement must state when it is a reconstructed equivalent rather than
proven original source.

## Practical restoration slices

1. Restore NGUI's 24 Resource shaders first: they use known NGUI clip/mask
   conventions and have complete packaged GLES logic. This should repair much
   of Menu/UI without touching gameplay code.
2. Restore the seven gameplay/particle placeholders, especially the two
   Mobile/VertexLit shaders used by map materials.
3. Restore ProBuilder helper shaders only where actually used; HideVertices is
   editor/helper-like and should not be treated as gameplay content.
4. Keep the existing real `Mobile/Unlit (Supports Lightmap)` implementation;
   do not replace it blindly. Its legacy `Vertex`, `VertexLM` and
   `VertexLMRGBM` passes are central to the original map appearance.

## Lightmaps and ugly directory names

All **54 scene lightmap textures** are present in the export as
`LightmapFar-0.png`. Only their parent directories retain the encrypted scene
file stems. The PNG GUIDs and `.meta` files survive, so normalizing directory
names is a path-migration task, not a lightmap-data recovery task.

A safe later migration should:

- move each scene's lightmap folder and `.meta` files under a readable
  map-specific directory;
- preserve GUIDs byte-for-byte so scene references remain valid;
- back up before moves and avoid touching scene contents during the same run;
- keep old lightmap indices/UV2 data already handled by geometry repair.

Occlusion culling warnings are separate: old Unity 4 occlusion data is not a
lightmap and may need rebuilding or explicit retirement after rendering is
correct. Do not claim that restoring paths fixes occlusion.
