# Export status

Export repeated successfully with the supplied AssetRipper 2.0.0 Linux x64.
The tool recognized 17 managed assemblies and completed Mono script decompilation
and project export. No separate .NET installation was needed.

The delivered archive is the unmodified ExportedProject directory, with its root
renamed UnityProject inside the ZIP. Auxiliary runtime assemblies are not injected
into Assets. Original APK and native libraries remain available in the repository
APK. All exported files and their .meta files are included. See manifest.json for
counts and SHA-256. Archive CRC validation passed.

The first scene is stored in mainData in this Unity version, followed by level0
through level54. The previous missing-Surf/Jet warning was a mapping error:
all 56 scenes were exported, including the Surf/Jet directory.

The export included 35 dummy shaders, which have since been recovered and
replaced in `client/`. The geometry repair described below is also complete in
the committed project; the old text about 55 scenes still needing repair was
stale. The raw AssetRipper ZIP used as input to several historical audit scripts
is not committed, so those particular tests are not runnable from a clean clone
without that external export.

## 2026-09-28 — MILESTONE: restoration declared complete

Project notes report that `client/` was run in Unity 5.6.7f1 and the Menu scene
was opened in Play mode. Confirmed working in that check: the scene renders with
correct UI (no more black-square text, no more transparent panels — the rebuilt
NGUI/Unlit shaders behave like the original), and level geometry repairs cleanly
with the universal recovery tool. The only runtime error seen was the expected
'Google account not found' from the Android Google Play path inside the editor.

Known honest limits: geometry repair is already applied in the committed
`client/`; no 55-scene repair remains. Online services (GPGS, PlayFab, ads,
billing) cannot be verified in the editor by definition. An audit found that the
54 map lightmap PNGs are not bound to the committed scenes; a reversible editor
binder is now included but has not yet been run or visually checked. Per-map
comparison against the live APK remains an optional last-mile check. Android
native/Java plugins are not reconstructed in `client/`.
