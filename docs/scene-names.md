# Readable scene names (608)

`tools/Restore-SceneNames.ps1 -ProjectPath <UnityProject>` restores all 56 names
with Unity closed. Requires adjacent `scene-names.json`. It moves each scene
and its original .meta together (GUIDs unchanged), replaces paths in Build
Settings without changing order/enabled flags, and changes LevelManager to load
and report plain names. It supports original/API-updated scene APIs by modifying
only the subsequent crypto lines. It checks all targets first, backs up outside
Assets, rolls back on caught write failures, and supports repeat runs.
Do not re-extract the original ZIP over a migrated/renamed project.

Entry point is `Assets/Levels/AwakeScene.unity`, followed by `Logo.unity`, then
`Menu.unity` or the first-run tutorial. These are decrypted names, not invented
labels. The original ZIP stays unchanged.

## Evidence and reproduction

UIFontControl's AES strings decode to jar:file:// plus Application.dataPath,
with suffixes !/classes.dex and !/assets/bin/Data/Managed/Assembly-CSharp.dll.
Serialized fontSize is zero. The original APK entries have lengths 7645196 and
1921552. Concatenation yields Utils.test = "76451961921552". PBKDF2-HMAC-SHA1,
555 iterations, salt IvmD123A12 yields the eight-byte DES key. The file format
prefix is ten bytes; the effective DES CBC IV is its first eight bytes. All 56
names decrypt with valid PKCS7 padding and unique readable results, matching map
folders (including actual name "Shooting Range").

Reproduce with Python 3.9+ and pycryptodome:
`python tools/recover_scene_names.py`.
Static checks: `python tools/test_scene_names.py` (2 tests passed).
The checks verify every scene/meta, all build paths/order, unique names, no other
exported encrypted-name references, and both original/API-updated loader forms.
PowerShell/Unity execution was not available in the sandbox; downloading portable
PowerShell failed at the GitHub release-assets TLS connection. Windows application
and actual scene transitions still need validation.

## Scope

Only scene naming/loading is changed. General Utils crypto, preferences, Photon
room names, game modes and Android services are not removed or bypassed. The
old UIFontControl data reads still run, even though LevelManager no longer needs
their key for scene loading. Android-specific initialization and shader fixes
remain separate. This does not assert a working full client yet.

## Windows PowerShell compatibility correction

The initial Windows run stopped at the mapping-count check before any project
writes. Windows PowerShell 5.1 emits a JSON array as a single pipeline object;
wrapping that pipeline directly in @() produced a nested array with Count = 1.
The importer now assigns ConvertFrom-Json output first, then normalizes the
assigned value with @($parsedMap). The 56-entry validation remains enabled.
This correction has not been executed under Windows PowerShell in the sandbox.
