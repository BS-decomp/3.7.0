// Editor-only recovery for the 608 export in Unity 5.6. No runtime components.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

public static class BlockStrikeGeometryRecovery
{
    const string DataPath = "Assets/Editor/BlockStrikeRecovery/BustGeometry.json";
    const string Prefix = "BS608_Recovered_";
    [Serializable] public class Manifest
    {
        public int version;
        public string scenePath, originalScenePath;
        public Record[] renderers;
    }
    [Serializable] public class Record
    {
        public int rendererId, filterId, transformId, vertexCount, subMeshCount, batchRootId, lightmapIndex;
        public string objectName, meshGuid, indexHash, positionHash;
        public int[] subsets;
        public float[] lightmapScaleOffset, localPosition, localRotation, localScale;
    }
    [Serializable] public class Receipt
    {
        public string scenePath, backupScene, backupMeta, assetFolder;
    }
    class Pending
    {
        public Record record;
        public MeshFilter filter;
        public MeshRenderer renderer;
        public Mesh mesh;
    }
    static string ProjectRoot { get { return Directory.GetParent(Application.dataPath).FullName; } }
    static string ReceiptPath { get { return Path.Combine(ProjectRoot, "RecoveryBackups/BustGeometry/last.json"); } }
    static void Require(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
    static long LocalId(Object obj)
    {
        var serialized = new SerializedObject(obj);
        var mode = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Require(mode != null, "This editor does not expose serialized local IDs.");
        mode.SetValue(serialized, Enum.ToObject(mode.PropertyType, 1), null);
        var property = serialized.FindProperty("m_LocalIdentfierInFile"); // Unity's spelling
        if (property == null) property = serialized.FindProperty("m_LocalIdentifierInFile");
        Require(property != null, "Cannot read local ID for " + obj.name);
        return property.longValue;
    }
    static Vector3 V3(float[] v) { return new Vector3(v[0], v[1], v[2]); }
    static Vector4 V4(float[] v) { return new Vector4(v[0], v[1], v[2], v[3]); }
    static bool Finite(float v) { return !float.IsNaN(v) && !float.IsInfinity(v); }
    static T Get<T>(Dictionary<long, Object> objects, long id) where T : Object
    {
        Object obj;
        Require(objects.TryGetValue(id, out obj) && obj is T, "Missing/wrong imported component ID " + id + " (" + typeof(T).Name + "). No guessing by name.");
        return (T)obj;
    }
    static Dictionary<long, Object> Index(UnityEngine.SceneManagement.Scene scene)
    {
        var result = new Dictionary<long, Object>();
        foreach (GameObject root in scene.GetRootGameObjects())
        foreach (Transform tr in root.GetComponentsInChildren<Transform>(true))
        {
            Add(result, tr);
            var filter = tr.GetComponent<MeshFilter>();
            var renderer = tr.GetComponent<MeshRenderer>();
            if (filter != null) Add(result, filter);
            if (renderer != null) Add(result, renderer);
        }
        return result;
    }
    static void Add(Dictionary<long, Object> objects, Object obj)
    {
        long id = LocalId(obj);
        Require(id != 0 && !objects.ContainsKey(id), "Invalid/duplicate local ID for " + obj.name);
        objects.Add(id, obj);
    }
    static void BatchFields(MeshRenderer renderer, bool apply)
    {
        var serialized = new SerializedObject(renderer);
        var batch = serialized.FindProperty("m_StaticBatchInfo");
        var subsets = serialized.FindProperty("m_SubsetIndices");
        Require(batch != null || (subsets != null && subsets.isArray), "Cannot access static batch fields on " + renderer.name);
        if (batch != null)
        {
            var first = batch.FindPropertyRelative("firstSubMesh");
            var count = batch.FindPropertyRelative("subMeshCount");
            Require(first != null && count != null, "Unknown static batch layout.");
            if (apply) { first.intValue = 0; count.intValue = 0; }
        }
        if (apply)
        {
            if (subsets != null && subsets.isArray) subsets.ClearArray();
            var batchRoot = serialized.FindProperty("m_StaticBatchRoot");
            if (batchRoot != null) batchRoot.objectReferenceValue = null;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
    static string HashIndices(List<int[]> submeshes)
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                foreach (int[] indices in submeshes) foreach (int index in indices) writer.Write(index);
                writer.Flush();
                using (var sha = SHA256.Create())
                    return BitConverter.ToString(sha.ComputeHash(stream.ToArray())).Replace("-", "").ToLowerInvariant();
            }
        }
    }
    static string HashPositions(Vector3[] vertices)
    {
        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            foreach (Vector3 v in vertices) { writer.Write(v.x); writer.Write(v.y); writer.Write(v.z); }
            writer.Flush();
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(stream.ToArray())).Replace("-", "").ToLowerInvariant();
        }
    }
    static T[] Select<T>(T[] source, List<int> used, int vertexCount, string label)
    {
        if (source.Length == 0) return new T[0];
        Require(source.Length == vertexCount, "Unexpected " + label + " array length.");
        var output = new T[used.Count];
        for (int i = 0; i < used.Count; i++) output[i] = source[used[i]];
        return output;
    }
    static Mesh Split(Mesh source, Transform tr, Record record)
    {
        Require(source.vertexCount == record.vertexCount && source.subMeshCount == record.subMeshCount,
            "Imported mesh layout differs from export: " + tr.name);
        Require(HashPositions(source.vertices) == record.positionHash, "Imported vertex positions differ from the original export: " + tr.name);
        Require(source.blendShapeCount == 0 && source.bindposes.Length == 0 && source.boneWeights.Length == 0,
            "Skinned geometry is not supported by this static repair.");
        var original = new List<int[]>();
        foreach (int subset in record.subsets)
        {
            Require(subset >= 0 && subset < source.subMeshCount && source.GetTopology(subset) == MeshTopology.Triangles, "Invalid source subset.");
            original.Add(source.GetTriangles(subset));
        }
        Require(HashIndices(original) == record.indexHash, "Imported triangle data differs from original export: " + tr.name);
        var used = new List<int>();
        var remap = new Dictionary<int, int>();
        var triangles = new List<int[]>();
        foreach (int[] indices in original)
        {
            var rebuilt = new int[indices.Length];
            for (int j = 0; j < indices.Length; j++)
            {
                int old = indices[j], mapped;
                Require(old >= 0 && old < source.vertexCount, "Index outside vertex buffer.");
                if (!remap.TryGetValue(old, out mapped)) { mapped = used.Count; remap.Add(old, mapped); used.Add(old); }
                rebuilt[j] = mapped;
            }
            triangles.Add(rebuilt);
        }
        Require(used.Count > 0 && used.Count <= 65534, "Unsupported recovered mesh size.");
        // Bust's original batch root is null: combined vertices are in world space.
        Require(record.batchRootId == 0, "Non-world batch root requires a different conversion.");
        Matrix4x4 toLocal = tr.worldToLocalMatrix;
        Require(Finite(toLocal.determinant) && Mathf.Abs(toLocal.determinant) > 1e-10f, "Singular transform: " + tr.name);
        Vector3[] vertices = Select(source.vertices, used, source.vertexCount, "vertices");
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 world = vertices[i];
            vertices[i] = toLocal.MultiplyPoint3x4(world);
            Vector3 roundtrip = tr.localToWorldMatrix.MultiplyPoint3x4(vertices[i]);
            Require(Finite(vertices[i].x) && Finite(vertices[i].y) && Finite(vertices[i].z) && (roundtrip - world).sqrMagnitude < 0.0001f,
                "World/local roundtrip failed: " + tr.name);
        }
        Vector3[] normals = Select(source.normals, used, source.vertexCount, "normals");
        Matrix4x4 normalMatrix = toLocal.inverse.transpose;
        for (int i = 0; i < normals.Length; i++) normals[i] = normalMatrix.MultiplyVector(normals[i]).normalized;
        Vector4[] tangents = Select(source.tangents, used, source.vertexCount, "tangents");
        float sign = toLocal.determinant < 0 ? -1f : 1f;
        for (int i = 0; i < tangents.Length; i++)
        {
            Vector3 xyz = toLocal.MultiplyVector(new Vector3(tangents[i].x, tangents[i].y, tangents[i].z)).normalized;
            tangents[i] = new Vector4(xyz.x, xyz.y, xyz.z, tangents[i].w * sign);
        }
        var mesh = new Mesh();
        try
        {
            mesh.name = Prefix + record.rendererId + "_" + record.objectName;
            mesh.vertices = vertices;
            if (normals.Length > 0) mesh.normals = normals;
            if (tangents.Length > 0) mesh.tangents = tangents;
            mesh.colors32 = Select(source.colors32, used, source.vertexCount, "colors");
            mesh.uv = Select(source.uv, used, source.vertexCount, "UV0");
            // Batch UV2 already includes atlas scale/offset; keep UV2, set renderer ST to identity.
            mesh.uv2 = Select(source.uv2, used, source.vertexCount, "UV2");
            mesh.uv3 = Select(source.uv3, used, source.vertexCount, "UV3");
            mesh.uv4 = Select(source.uv4, used, source.vertexCount, "UV4");
            if (record.lightmapIndex < 254)
            {
                Require(mesh.uv2.Length == vertices.Length, "Missing baked lightmap coordinates.");
                Vector4 st = V4(record.lightmapScaleOffset);
                foreach (Vector2 uv in mesh.uv2)
                    Require(uv.x >= st.z - .002f && uv.x <= st.z + st.x + .002f && uv.y >= st.w - .002f && uv.y <= st.w + st.y + .002f,
                        "Imported UV2 is not the expected baked atlas data.");
            }
            mesh.subMeshCount = triangles.Count;
            for (int i = 0; i < triangles.Count; i++) mesh.SetTriangles(triangles[i], i);
            mesh.RecalculateBounds();
            return mesh;
        }
        catch { Object.DestroyImmediate(mesh); throw; }
    }

    [MenuItem("Tools/Block Strike Recovery/Repair Bust geometry")]
    public static void RepairBust()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) { EditorUtility.DisplayDialog("Recovery", "Stop Play first.", "OK"); return; }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (!EditorUtility.DisplayDialog("Repair Bust", "This repairs only Bust's static geometry. The scene and meta are backed up outside Assets. Scripts, colliders and materials are not replaced.\n\nProceed?", "Repair", "Cancel")) return;
        var pending = new List<Pending>();
        Receipt receipt = null;
        bool sceneTouched = false;
        string folder = null;
        try
        {
            var manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(DataPath));
            Require(manifest.version == 1 && manifest.renderers.Length == 153, "Invalid Bust manifest.");
            string scenePath = File.Exists(manifest.scenePath) ? manifest.scenePath : manifest.originalScenePath;
            Require(File.Exists(scenePath), "Bust scene not found.");
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var objects = Index(scene);
            foreach (Record record in manifest.renderers)
            {
                var renderer = Get<MeshRenderer>(objects, record.rendererId);
                var filter = Get<MeshFilter>(objects, record.filterId);
                var tr = Get<Transform>(objects, record.transformId);
                Require(renderer.gameObject == filter.gameObject && tr.gameObject == renderer.gameObject && tr.name == record.objectName, "Component identity mismatch.");
                Require((tr.localPosition - V3(record.localPosition)).sqrMagnitude < 1e-8f && (tr.localScale - V3(record.localScale)).sqrMagnitude < 1e-8f,
                    "Object transform was edited: " + tr.name);
                var q = record.localRotation;
                Require(Quaternion.Angle(tr.localRotation, new Quaternion(q[0], q[1], q[2], q[3])) < .05f, "Object rotation was edited: " + tr.name);
                Require(filter.sharedMesh != null, "Missing source mesh: " + tr.name);
                Require(!filter.sharedMesh.name.StartsWith(Prefix), "Bust geometry is already repaired. Use Revert before reapplying.");
                Require(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(filter.sharedMesh)) == record.meshGuid, "Source mesh GUID mismatch: " + tr.name);
                Require(renderer.sharedMaterials.Length == record.subsets.Length, "Material count differs: " + tr.name);
                BatchFields(renderer, false);
                pending.Add(new Pending { record = record, filter = filter, renderer = renderer, mesh = Split(filter.sharedMesh, tr, record) });
            }
            // All preflight checks and mesh construction succeeded; only now write assets.
            string token = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            string backup = Path.Combine(ProjectRoot, "RecoveryBackups/BustGeometry/" + token);
            Directory.CreateDirectory(backup);
            folder = "Assets/RecoveredGeometry/Bust-" + token;
            receipt = new Receipt { scenePath = scenePath, backupScene = Path.Combine(backup, "Bust.unity"), backupMeta = Path.Combine(backup, "Bust.unity.meta"), assetFolder = folder };
            File.Copy(scenePath, receipt.backupScene);
            File.Copy(scenePath + ".meta", receipt.backupMeta);
            File.WriteAllText(Path.Combine(backup, "receipt.json"), JsonUtility.ToJson(receipt, true));
            Directory.CreateDirectory(folder);
            AssetDatabase.Refresh();
            foreach (Pending item in pending)
                AssetDatabase.CreateAsset(item.mesh, folder + "/Renderer-" + item.record.rendererId + ".asset");
            AssetDatabase.SaveAssets();
            sceneTouched = true;
            foreach (Pending item in pending)
            {
                BatchFields(item.renderer, true);
                item.filter.sharedMesh = item.mesh;
                if (item.record.lightmapIndex < 254)
                    item.renderer.lightmapScaleOffset = new Vector4(1, 1, 0, 0);
                EditorUtility.SetDirty(item.filter);
                EditorUtility.SetDirty(item.renderer);
            }
            EditorSceneManager.MarkSceneDirty(scene);
            Require(EditorSceneManager.SaveScene(scene), "Could not save repaired scene.");
            File.WriteAllText(ReceiptPath, JsonUtility.ToJson(receipt, true));
            Debug.Log("[BS608 Recovery] Bust: repaired 153 renderers / 248 submeshes. Backup: " + backup);
            EditorUtility.DisplayDialog("Bust geometry repaired", "Repaired 153 renderers. Bust is open: inspect it in Scene view, without Play.\n\nThis does not fix shaders, Android startup or old occlusion data.\nBackup: " + backup, "OK");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            try
            {
                if (sceneTouched && receipt != null) Restore(receipt);
                if (folder != null && AssetDatabase.IsValidFolder(folder)) AssetDatabase.DeleteAsset(folder);
            }
            catch (Exception rollback) { Debug.LogError("[BS608 Recovery] Rollback needs attention. Backup: " + (receipt == null ? "none" : receipt.backupScene) + "\n" + rollback); }
            EditorUtility.DisplayDialog("Recovery stopped", ex.Message + "\n\nSee Console for details. No missing data was substituted.", "OK");
        }
        finally
        {
            foreach (Pending item in pending)
                if (item.mesh != null && !AssetDatabase.Contains(item.mesh)) Object.DestroyImmediate(item.mesh);
        }
    }
    static void Restore(Receipt receipt)
    {
        Require(File.Exists(receipt.backupScene) && File.Exists(receipt.backupMeta), "Backup is incomplete.");
        // Caller explicitly agreed to discard the repaired scene. Close it before restoring bytes.
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        File.Copy(receipt.backupScene, receipt.scenePath, true);
        File.Copy(receipt.backupMeta, receipt.scenePath + ".meta", true);
        AssetDatabase.ImportAsset(receipt.scenePath, ImportAssetOptions.ForceUpdate);
        EditorSceneManager.OpenScene(receipt.scenePath, OpenSceneMode.Single);
    }
    [MenuItem("Tools/Block Strike Recovery/Revert last Bust repair")]
    public static void RevertBust()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (!File.Exists(ReceiptPath)) { EditorUtility.DisplayDialog("Recovery", "No successful repair receipt found.", "OK"); return; }
        if (!EditorUtility.DisplayDialog("Revert Bust", "Replace Bust with its pre-repair backup? Later edits to Bust will be lost. Recovered mesh assets will be left in place to avoid breaking other references.", "Revert", "Cancel")) return;
        try
        {
            var receipt = JsonUtility.FromJson<Receipt>(File.ReadAllText(ReceiptPath));
            Restore(receipt);
            File.Delete(ReceiptPath);
            Debug.Log("[BS608 Recovery] Restored Bust from " + receipt.backupScene);
        }
        catch (Exception ex) { Debug.LogException(ex); EditorUtility.DisplayDialog("Revert failed", ex.Message, "OK"); }
    }
}
