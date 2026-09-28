# Universal static-geometry repair for Unity 5.6

This supersedes the one-scene Bust pilot. It repairs the Unity 4 static batch
data that caused map objects to draw the wrong pieces of combined meshes. Menu
has the same mesh data and is included; its camera, UI, events and scripts are
not replaced.

## Install (Windows)

Close Unity and run from the repository clone:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Install-GeometryRecovery.ps1 -ProjectPath "$env:USERPROFILE\Desktop\BlockStrike-Unity56\UnityProject"
```

Expected output reports **1 script and 56 manifests**. The installer copies only
editor-time files into `Assets/Editor/BlockStrikeRecovery`. It does not change a
scene, runtime script, camera, event, collider or material. A stale pilot
`BustGeometry.json`, if present in the project, is backed up and removed because
the universal tool uses `MapGeometry/*.json` instead.

## Repair

Open the project in Unity 5.6.7f1 and choose:

**Tools > Block Strike Recovery > Repair ALL scene geometry**

The tool asks to save modified scenes, then processes all 56 manifests:

- 3,891 static-batched renderers are known to the manifests;
- Menu contributes 12 static renderers and is repaired like a map;
- Logo and AwakeScene contain none and are skipped;
- a Bust pilot repair that is already present is detected and skipped safely;
- any partially repaired scene is rejected instead of being split a second time.

For each modified scene, the tool maps original local file IDs for the exact
MeshFilter/MeshRenderer/Transform triples. It checks vertex layout, exact
position hashes, subset triangle hashes, material counts and transforms before
writing. It also compares the imported mesh GUID with the source manifest; if
Unity 5.6 remaps that GUID, the mismatch is logged and the exact content hashes
must still match before the mesh is accepted. It preserves object names, enabled state, hierarchy, transforms,
colliders, scripts, static flags, cameras, UI/events and material order.

Converted mesh assets are written under a unique
`Assets/RecoveredGeometry/MapGeometry-<token>/...` folder. Original combined
meshes are not edited or deleted. Baked lightmap UV2 is retained; only the
renderer-side duplicate atlas transform is replaced with identity for real
lightmapped renderers. This repair does **not** claim to fix placeholder
shaders, baked-lightmap quality, old occlusion data, startup, Android services,
accounts or Photon.

## Backups, partial progress and revert

Before a scene is saved, its exact `.unity` and `.unity.meta` bytes are copied
under `RecoveryBackups/MapGeometry/<token>`, outside Assets. Scenes are
committed one at a time: if a later scene fails, earlier repaired scenes remain,
while the failing scene attempts rollback and its newly created mesh folder is
removed. A crash during file writes is not a guaranteed transaction; do not
close Unity while the menu command is running.

To restore all scenes changed by the last full run:

**Tools > Block Strike Recovery > Revert last full geometry repair**

To restore Bust from its separate pilot receipt:

**Tools > Block Strike Recovery > Revert Bust pilot repair**

Reverting warns before replacing scene files and intentionally leaves recovered
mesh assets on disk so references from other scenes are not broken.

## Validation performed offline

- `python tools/build_geometry_manifests.py --all` regenerates all 56 manifests.
- `python tools/test_geometry_manifests.py` checks reproducibility, exact
  counts/order, full subset coverage and every scene's byte-identical split
  data. It reports 3,891 renderers.
- `python tools/test_bust_recovery.py` keeps the proven Bust hierarchy and
  world/local/float32 roundtrip checks.
- `python tools/test_scene_names.py` remains green.
- The C# source parses with tree-sitter-c-sharp.

No Unity editor exists in this sandbox, so Unity API typechecking and actual
full-map rendering still require the user's editor. The Bust pilot, whose
algorithm this reuses, was successfully run and visually confirmed in Unity
5.6.7f1.
