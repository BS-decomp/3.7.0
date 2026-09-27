# Bust geometry audit (original 608 export)

User reports floating presents/trees and missing map surfaces, without a screenshot.
Inspected Bust, NOT Bust 2, directly in the unchanged ZIP:
`Assets/Levels/Maps/Others/Bust/SXZtRDEyM0ExMkeyUq#HDyzp.unity`.

- 271 MeshRenderers; 153 have nonempty `m_SubsetIndices`.
- Shared mesh GUID `77426febc8ca99542aee36fb28903b7a` is referenced 153 times.
- It resolves to `Assets/Mesh/Combined Mesh (root_ scene)_41.asset`:
  9,973 vertices and 248 submeshes.
- Wall GO 152, renderer 1360, filter 1632: enabled; four materials;
  subset hex `c8000000c9000000ca000000cb000000` = [200,201,202,203].
  Wall transform position (29,10,-29). Submesh 200 has 42 indices, 68 vertices,
  bounds center (29.25,9,-38), extents (0.25,3,9).
- Present GO 109 uses the SAME mesh but subset [17].
- Tree GO 280 uses the SAME mesh but subset [2].
- Submeshes 0 and 1 have small bounds, about 0.24-0.33 horizontal extents;
  submesh 2 has tree-scale bounds centered (-12,11.733156,-47).
- These renderers store legacy subset indices and a zero static batch root,
  not a modern `m_StaticBatchInfo` structure.
- 54 of 56 exported scenes contain nonempty legacy subset arrays.

This is evidence for investigating lost static-batch/submesh selection on Unity
migration. If imported as an ordinary MeshFilter/Renderer, objects can draw the
wrong first submeshes and apply transforms to already combined coordinates.
It is NOT yet proof of how Unity 5.6 imported the user's binary scene, nor that
all renderer/shader problems have this one cause. Wall geometry does exist in
this exported mesh; claiming it is simply absent from the APK is unwarranted.

Next: compare imported renderer batch state, and reconstruct independent meshes
using the exact subset arrays and correct batch-to-object transforms; preserve
material order and handle baked UV2/lightmap scale-offset without double applying
it. Do not reset all transforms, replace all materials or merely clear batching
flags. Colliders, scene hierarchy, game scripts and asset references must remain
intact. No geometry repair shipped with this audit.
