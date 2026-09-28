# Canonical upstream shader sources (2015-2016 era)

Original third-party shader sources matching the versions shipped in the
Block Strike 608 APK, retained as reproducible build inputs for
`tools/build_shader_recovery.py`. Each file is verified against the
APK-extracted ground truth in `tools/shader-extract/` (shader name,
Properties block, render state and compiled GLSL behavior).

- `ngui/` - classic NGUI 3.x "Unlit - *" family (24 files), fetched from the
  public NGUI mirror in Hengle/UnityGameFramework-1.
- `probuilder/` - ProBuilder 2.x shaders, fetched from
  Unity-Technologies/probuilder-vr (era-correct SixBySeven sources).
- `madfinger/` - MADFINGER/Transparent/GodRays + Blinking GodRays, fetched
  from cheng219/tianyu and fabriciolpu2/DevGamesManha mirrors of the free
  MADFINGER shader pack.
