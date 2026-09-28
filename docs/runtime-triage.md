# Unity 5.6 runtime triage (user log, 2026-09-27)

Confirmed in supplied Windows Editor log:
- Assembly-CSharp-firstpass: compilation succeeded, 2 warnings.
- Assembly-CSharp: compilation succeeded, 96 warnings; assemblies reload.
This is editor compilation success, not a verified standalone/Android build.

Separate problems:
1. NGUI shader exports (Unlit/Text, Unlit/Transparent Colored, clipping variants,
   etc.) contain DummyShaderTextExporter. Original transparency/vertex-color/
   clipping behavior is not retained. This is a strong explanation for text and
   UI quads. No claim that fonts/atlas references are otherwise fully validated.
2. UIDrawCall.Create calls DontDestroyOnLoad even outside play mode; the NGUI
   panel invokes it from edit-mode LateUpdate. Added an Application.isPlaying
   guard preserving the original call during gameplay. Unity validation pending.
3. Better was played directly without a Photon room (PeerCreated). GetGameMode
   dereferences room.customProperties; instantiation explicitly rejects absence
   of a room. GameManager attempts to redirect to Menu. Do not blanket-suppress
   exceptions or default all modes; restore initialization flow or build a
   separate explicit editor test bootstrap.
4. Scene loading encrypts names with Utils.test (initially empty). UIFontControl
   initializes it from lengths of data fetched with WWW and then loads Logo.
   This boot process needs analysis/adaptation for Editor: APK-specific paths
   and byte lengths cannot simply be assumed identical after export. Therefore
   missing encrypted Menu name and Bad PKCS7 padding are not proof the Menu
   scene is absent, nor evidence TEA assembly recovery failed.
5. Appodeal invokes AndroidJavaClass in Windows Editor. Native ash load also
   fails there. Android .so files do not serve as Windows plugins. Platform
   adaptation must be explicit and preserve the Android functionality.
6. Old occlusion data, shader reconstruction, and static mesh rendering remain
   separate open issues. Compilation success does not resolve them.

Only the UIDrawCall guard is changed in this pass. UI shader reconstruction,
scene bootstrap and Android integration are NOT claimed fixed.
