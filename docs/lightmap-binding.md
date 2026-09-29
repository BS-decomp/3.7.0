# Restoring legacy lightmaps in Unity 5.6

## Audit findings

The 54 map PNGs were preserved, but their scene bindings were not. In the committed `client/` project:

- each of the 54 map scenes has `m_LightingDataAsset: {fileID: 0}`;
- none of those scenes references its `LightmapFar-0.png` GUID, and no `LightingData.asset` / `LightmapSnapshot` was recovered;
- Unity 5.6 scene `MeshRenderer` records consequently have no serialized lightmap index/offset;
- the 56 geometry manifests retain the original renderer IDs, `lightmapIndex` and `lightmapScaleOffset`; 3,196 renderers use lightmap index 0;
- each target scene has one sibling `LightmapFar-0.png`, and the recovered meshes retain the original atlas UV2 data.

An earlier version of `docs/shader-lightmap-recovery.md` said the scene references had been preserved. That claim was wrong for the committed 5.6 scenes. Folder names/GUIDs were normalized, but the `LightingData` relationship was lost when the old project was opened/saved in the newer editor.

This is a binding/import problem, not a OnePlus 13 performance problem. A lightmap is a texture sampled by the renderer; restoring it does not require a runtime light bake.

## Recovery tool

The recovery is opt-in and reversible. The tooling was added first, and the Bind operation has since been **run against this repository**: all 54 map scenes are now bound (see "Current state" below). `Logo` and `AwakeScene` contain no lightmapped renderers and are skipped. The editor operation:

1. Preflights all 56 manifests and all 54 target scenes before writing anything. It resolves renderers by their serialized local IDs, refuses name-based guesses, verifies each serialized `MeshFilter` GUID points to its expected `RecoveredGeometry/MapGeometry-*/<Map>/Renderer-<id>.asset`, verifies each texture, and checks the expected 54 scenes / 3,196 renderer records. This deliberately checks the scene's persistent reference, not only `MeshFilter.sharedMesh` after scripts run.
2. Backs up each target `.unity`, scene `.meta`, and lightmap `.meta` under `RecoveryBackups/LightmapBinding/<token>/` (outside `Assets`, gitignored).
3. Sets each `LightmapFar-0.png` importer to a plain **Default** texture (not the `Lightmap` type; see [`android-lightmap-overexposure.md`](android-lightmap-overexposure.md)). The original PNG bytes and GUIDs are not changed; `.meta` backups make this reversible.
4. Adds one `BS608_LegacyLightmaps` object with `LegacyLightmapBinder` to each target scene and saves it. The component binds the scene's texture to `LightmapSettings.lightmaps`, sets `LightmapsMode.NonDirectional`, and sets the original lightmapped renderers to index 0.
5. If a lightmapped renderer has `MeshAtlas`, the explicit Bind operation retargets `MeshAtlas.originalMesh` to that renderer's recovered mesh before rebuilding its temporary atlas clone. This preserves the repaired geometry and UV2 when the `[ExecuteInEditMode]` script runs. The scene backup includes this change; `Validate` only reports it and makes no scene changes.

**Run geometry repair first.** `BlockStrikeGeometryRecovery` splits the Unity 4 static batches, extracts each object's atlas portion, keeps the baked atlas coordinates in UV2, and sets `lightmapScaleOffset` to identity. Applying the original manifest scale/offset a second time would be wrong. The lightmap binder is intentionally strict and stops if those recovered meshes are absent.

Two non-map scenes (`Logo` and `AwakeScene`) contain no lightmapped renderers and are skipped. No rebake is run. Existing GI workflow mode remains on-demand.

## Using it

Unity 5.6.7f1 only; close Unity before installing the scripts into a project.

For this repository, the binder scripts are already in `client/`. Open the project, allow compilation, then run:

1. `Tools > Block Strike Recovery > Validate ALL legacy lightmaps` — read-only preflight; it verifies serialized mesh references and textures without changing scenes or importer settings. If it finds a `MeshAtlas` runtime clone, it explains that the explicit Bind command will retarget its source mesh.
2. `Tools > Block Strike Recovery > Bind ALL legacy lightmaps` — review the confirmation (including any `MeshAtlas` source updates), then apply. The operation remains opt-in and reversible.

To install into another recovered export, run `Install-AllRecovery.ps1`; it installs the geometry manifests/tool and the lightmap binder. After opening Unity, repair geometry first, then validate and bind lightmaps.

To undo the last successful operation, run `Tools > Block Strike Recovery > Revert last legacy lightmap binding`. Revert restores scene files and texture-importer metadata from the backup. It discards later edits to those scenes/import settings; backup files are retained.

## Current state

The Bind operation has been applied to this repository. Concretely:

- all 54 map scenes contain a `BS608_LegacyLightmaps` object carrying `LegacyLightmapBinder`;
- each map's `LightmapFar-0.png` importer is a plain Default texture (`textureType: 0`) with the importer metadata in the Unity 5.6 serialization format. It was first bound as the `Lightmap` type (`textureType: 6`) and switched to Default when the Android over-exposure was fixed;
- the PNG bytes and all asset GUIDs are unchanged.

## Verification status

Verified in Unity 5.6.7f1: the project compiles, the scenes open, and lighting renders. The maps were inspected in the editor by flying through them, and baked shadowing is visibly present again. This was a visual pass over the scenes, not an automated test.

Still unverified: Play mode under real match conditions, an Android device build, and per-map pixel comparison against the live APK. Platform-specific lightmap decoding can differ from editor output, so a device check is still worthwhile before shipping.

## Reverting

`Tools > Block Strike Recovery > Revert last legacy lightmap binding` restores scenes and texture-importer metadata from `RecoveryBackups/LightmapBinding/<token>/`.

**That backup directory is gitignored and is not part of the repository.** Revert therefore only works on the machine where Bind was originally run. From a clean clone there is no backup to restore from — undo the binding with `git` instead (the commit that applied it is self-contained), or re-run Bind on a fresh export to regenerate backups.
