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

The recovery is deliberately opt-in. This commit adds the runtime component and editor command, but does **not** change the 56 scene files. The editor operation:

1. Preflights all 56 manifests and all 54 target scenes before writing anything. It resolves renderers by their serialized local IDs, refuses name-based guesses, checks that each renderer references its expected `RecoveredGeometry/MapGeometry-*/<Map>/Renderer-<id>.asset`, verifies each texture, and checks the expected 54 scenes / 3,196 renderer records.
2. Backs up each target `.unity`, scene `.meta`, and lightmap `.meta` under `RecoveryBackups/LightmapBinding/<token>/` (outside `Assets`, gitignored).
3. Sets each `LightmapFar-0.png` importer to Unity's `Lightmap` type. The original PNG bytes and GUIDs are not changed; `.meta` backups make this reversible.
4. Adds one `BS608_LegacyLightmaps` object with `LegacyLightmapBinder` to each target scene and saves it. The component binds the scene's texture to `LightmapSettings.lightmaps`, sets `LightmapsMode.NonDirectional`, and sets the original lightmapped renderers to index 0.

**Run geometry repair first.** `BlockStrikeGeometryRecovery` splits the Unity 4 static batches, extracts each object's atlas portion, keeps the baked atlas coordinates in UV2, and sets `lightmapScaleOffset` to identity. Applying the original manifest scale/offset a second time would be wrong. The lightmap binder is intentionally strict and stops if those recovered meshes are absent.

Two non-map scenes (`Logo` and `AwakeScene`) contain no lightmapped renderers and are skipped. No rebake is run. Existing GI workflow mode remains on-demand.

## Using it

Unity 5.6.7f1 only; close Unity before installing the scripts into a project.

For this repository, the binder scripts are already in `client/`. Open the project, allow compilation, then run:

1. `Tools > Block Strike Recovery > Validate ALL legacy lightmaps` — read-only preflight; it verifies references and geometry without changing scenes or importer settings.
2. `Tools > Block Strike Recovery > Bind ALL legacy lightmaps` — review the confirmation, then apply.

To install into another recovered export, run `Install-AllRecovery.ps1`; it installs the geometry manifests/tool and the lightmap binder. After opening Unity, repair geometry first, then validate and bind lightmaps.

To undo the last successful operation, run `Tools > Block Strike Recovery > Revert last legacy lightmap binding`. Revert restores scene files and texture-importer metadata from the backup. It discards later edits to those scenes/import settings; backup files are retained.

## Verification limits

The current repository scenes have not been bound by this operation. The code has been statically checked and the on-disk manifests, scene local IDs, geometry mesh references, texture paths and PNG assets were checked offline. Unity 5.6.7f1 is not available in this environment, so compilation inside Unity, rendered output, Play mode and an Android build remain unverified. The first real visual check should be one map (Battleforce or Bust) in Scene/Game view; use Revert if the result is darker/brighter or otherwise unexpected. The importer and scene changes are backed up specifically because platform-specific lightmap decoding cannot be visually confirmed here.
