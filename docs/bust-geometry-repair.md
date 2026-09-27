# Bust geometry repair for Unity 5.6

## Install and use (Windows)

Close Unity and run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Install-GeometryRecovery.ps1 -ProjectPath "$env:USERPROFILE\Desktop\BlockStrike-Unity56\UnityProject"
```

Open the project and choose **Tools > Block Strike Recovery > Repair Bust geometry**.
Accept the confirmation. The tool asks to save any currently modified scenes,
opens Bust itself (readable or original name), validates the imported data, then
repairs **only Bust**. It does not require Play mode or a Photon connection.
Inspect the result in Scene view, without Play. No menu/startup fixes are bundled.

The installer copies two editor-only files into Assets/Editor/BlockStrikeRecovery.
It changes no scenes or runtime scripts. Updating this installer does not require
re-running scene renaming. Existing tool files are backed up outside Assets.

## What the repair does

- Identifies original renderer/filter/transform local file IDs, not ambiguous names.
- Checks mesh GUID, 9,973-vertex / 248-submesh layout, exact position and selected index hashes,
  material counts, local object transforms and batch-field access before writes.
- Splits the shared mesh into 153 independently renderable meshes, keeping triangle
  order and material order. Copies colors and available UV sets. Transforms combined
  world-space positions back to each object's local space, with a roundtrip check.
- Transforms normals by inverse transpose and tangents by the linear transform
  (including handedness). Does not recalculate normals or generate UVs.
- Validates selected UV2 against the original atlas rectangles. Keeps those baked
  atlas coordinates and sets each lightmapped renderer's scale/offset to identity,
  avoiding a second atlas transform. Keeps current lightmap indices/textures.
- Clears the renderer's old static batch linkage; keeps object transforms, names,
  hierarchy, enabled state, static flags, colliders, materials, scripts and references.
- Creates mesh assets in a unique Assets/RecoveredGeometry/Bust-* folder; original
  combined meshes are not edited or removed.

## Backup, failure and repeat runs

After ALL source checks and in-memory mesh preparation pass, original scene/meta
bytes are copied into RecoveryBackups/BustGeometry/<timestamp-id>, outside Assets.
The original scene is saved only after assigning the prepared assets. A caught
failure after modification attempts to restore the scene from backup and remove
that attempt's new mesh assets. Rollback failures are explicitly logged with the
backup path; process crashes or disk failures are not a guaranteed transaction.
The installer and repair should not be interrupted during writes.

Reapplying to already repaired geometry is rejected rather than splitting twice.
**Tools > Block Strike Recovery > Revert last Bust repair** restores the saved
scene/meta bytes. It warns that subsequent Bust edits will be lost. Recovered mesh
assets are retained on explicit revert, so references from other scenes are not
broken. The receipt is stored in RecoveryBackups/BustGeometry/last.json.

## Executed tests and remaining uncertainty

- `python tools/build_bust_manifest.py`: regenerate manifest with source identities,
  index SHA256s and verification of the original baked lightmap UV rectangles.
- `python tools/test_bust_recovery.py`: 3 passed. Reproducible manifest; exact
  248-submesh coverage; all 9,973 selected vertex records world/local/world roundtrip
  through original hierarchy with float32 local positions. Maximum measured error
  1.20e-7 world units (rounded up). UV rectangle and index-preservation checks pass.
- `python tools/test_scene_names.py`: 3 existing regression tests passed.
- C# source parsed without syntax errors using tree-sitter-c-sharp. This is NOT
  a C# typecheck or compilation against Unity 5.6 assemblies.

No Unity editor is installed in the sandbox. The C# API integration, Windows
PowerShell installer and actual visual result remain unverified. The tool fails
closed on unfamiliar imported meshes/IDs/batch fields, rather than guessing.
Only Bust is enabled for the first real-editor validation; extending the operation
to every map before that would expose more scenes to unverified conversion.

This repair does not reconstruct placeholder shaders, rebake lightmaps/occlusion,
adapt Android plugins or initialize accounts/Photon. These are independent tasks;
a restored wall mesh alone does not prove the entire game is recovered.
