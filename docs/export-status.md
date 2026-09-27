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
