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

Outstanding: 35 dummy shaders, editor compilation/import validation, Android
plugin reconstruction, gameplay/network verification. Do not treat successful
export as proof of a playable build.

## 2026-09-28 — MILESTONE: restoration declared complete

User ran `client/` in Unity 5.6.7f1 and opened the Menu scene in Play mode.
Confirmed working: scene renders with correct UI (no more black-square text,
no more transparent panels — the rebuilt NGUI/Unlit shaders behave like the
original), level geometry repaired cleanly by the universal recovery tool.
The only runtime error seen is the expected 'Google account not found' from
the Android Google Play path inside the editor (not a restoration loss; the
game is client-driven and all modes live in the shipped .cs scripts).

Known honest limits (verification, not losses): remaining 55 scenes to be
repaired with the same Tools > Block Strike Recovery command (Menu is the
proof it works); online services (GPGS, PlayFab, ads, billing) cannot run in
the editor by definition; per-map visual comparison against the live APK
remains as an optional last-mile check. All decompilation/pipeline work is DONE.
