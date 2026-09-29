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
billing) cannot be verified in the editor by definition. Per-map comparison
against the live APK remains an optional last-mile check. Android native/Java
plugins are not reconstructed in `client/`.

## 2026-09-28 — lightmaps bound and visually checked

An audit had found that the 54 map lightmap PNGs were not bound to the committed
scenes. That is now resolved: the reversible editor binder was run against
`client/`, so all 54 map scenes carry a `BS608_LegacyLightmaps` object with
`LegacyLightmapBinder`, and each map's `LightmapFar-0.png` is imported as a
`Lightmap` texture (later changed to a plain Default texture, see `android-lightmap-overexposure.md`). PNG bytes and asset GUIDs were not modified.

Checked in Unity 5.6.7f1: the project compiles, the scenes open, and the maps
were inspected by flying through them in the editor — baked shadowing renders
again. This was a visual pass, not an automated test. Play mode under real match
conditions and an Android device build are still unverified, and platform
lightmap decoding can differ from editor output.

Caveat on undo: binder backups are written to `RecoveryBackups/`, which is
gitignored. The in-editor Revert command therefore only works on the machine
where Bind was run; from a clean clone, revert via `git`.

## 2026-09-28 — pre-publication snapshot audit

An offline pass over all 3,891 static-batched renderers caught 24 serialized
`MeshFilter` references set to `{fileID: 0}` by `MeshAtlas` edit-mode clones:
4 lightmapped objects in Shooting Range and 20 non-lightmapped objects in Hill,
Military Range and Playground. Their exact per-renderer recovered mesh assets
were already present. Persistent references were restored; for the 20
non-lightmapped objects, `MeshAtlas.originalMesh` was also retargeted to those
assets, as the lightmap binder had already done for the other four.

`python -m unittest discover -s tools -p 'test_*.py'` now checks all 3,891
persistent mesh references and all 24 `MeshAtlas` sources. The nine tests
requiring the external raw export ZIP skip on a clean clone; they are not
reported as having passed. The four patched scenes have **not yet been
reopened and resaved in Unity 5.6.7f1**; do that, then rerun the offline tests
before describing this snapshot as editor-verified. Match play and Android
build/device testing remain open.
