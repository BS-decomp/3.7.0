using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[AddComponentMenu("NGUI/Atlas3D/MeshAtlas")]
[ExecuteInEditMode]
public class MeshAtlas : MonoBehaviour
{
	public static UIAtlas lastUsedAtlas;

	public Mesh originalMesh;

	private MeshFilter _mf;

	private Mesh mesh;

	public bool meshSettings;

	public Vector3 scale = Vector3.one;

	public Vector3 pivot = Vector3.zero;

	public bool mirrorX;

	public bool mirrorY;

	public bool mirrorZ;

	public bool flip;

	public Material originalMaterial;

	public Material customMaterial;

	public UIAtlas mAtlas;

	public string mSpriteName;

	public UISpriteData mSprite;

	private bool mSpriteSet;

	public MeshFilter mf
	{
		get
		{
			if (_mf == null)
			{
				_mf = GetComponent<MeshFilter>();
			}
			return _mf;
		}
	}

	[SerializeField]
	public UIAtlas atlas
	{
		get
		{
			return mAtlas;
		}
		set
		{
			if (!(mAtlas != value))
			{
				return;
			}
			mAtlas = value;
			mSpriteSet = false;
			mSprite = null;
			if (!(mAtlas != null))
			{
				return;
			}
			lastUsedAtlas = value;
			customMaterial = mAtlas.spriteMaterial;
			if (originalMaterial != null && !string.IsNullOrEmpty(originalMaterial.name))
			{
				for (int i = 0; i < mAtlas.spriteList.Count; i++)
				{
					if (mAtlas.spriteList[i].name == originalMaterial.name)
					{
						SetAtlasSprite(mAtlas.spriteList[i]);
						mSpriteName = mSprite.name;
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(mSpriteName) && mAtlas != null && mAtlas.spriteList.Count > 0)
			{
				SetAtlasSprite(mAtlas.spriteList[0]);
				mSpriteName = mSprite.name;
			}
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				string text = mSpriteName;
				mSpriteName = string.Empty;
				spriteName = text;
			}
		}
	}

	[SerializeField]
	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					mSpriteName = string.Empty;
					mSprite = null;
					mSpriteSet = false;
				}
			}
			else if (mSpriteName != value)
			{
				mSpriteName = value;
				mSprite = null;
				mSpriteSet = false;
				UpdateUVs();
			}
		}
	}

	public bool isValid
	{
		get
		{
			return GetAtlasSprite() != null;
		}
	}

	private void OnEnable()
	{
		UpdateMesh();
	}

	private void Reset()
	{
		DisableMesh();
		if (Application.isEditor)
		{
			UpdateMesh();
		}
	}

	public void UpdateMesh()
	{
		if (originalMesh == null)
		{
			if (Application.isEditor)
			{
				originalMesh = GetComponent<MeshFilter>().sharedMesh;
			}
			else
			{
				originalMesh = GetComponent<MeshFilter>().mesh;
			}
			originalMaterial = mf.GetComponent<Renderer>().sharedMaterial;
			if (atlas == null && lastUsedAtlas != null)
			{
				atlas = lastUsedAtlas;
			}
		}
		if (base.enabled)
		{
			EnableMesh();
		}
		else
		{
			DisableMesh();
		}
	}

	public void EnableMesh()
	{
		if (mesh == null)
		{
			mesh = (Mesh)Object.Instantiate(originalMesh);
			mesh.name = originalMesh.name + "_Atlas";
			mesh.hideFlags = HideFlags.HideAndDontSave;
			UpdateUVs();
			UpdateMeshSettings();
			if (customMaterial != null)
			{
				mf.GetComponent<Renderer>().sharedMaterial = customMaterial;
			}
			mf.mesh = mesh;
		}
	}

	public void DisableMesh()
	{
		if (mesh != null)
		{
			customMaterial = mf.GetComponent<Renderer>().sharedMaterial;
			Object.DestroyImmediate(mesh);
			mesh = null;
		}
		if (originalMesh != null)
		{
			mf.mesh = originalMesh;
		}
		if (originalMaterial != null)
		{
			mf.GetComponent<Renderer>().sharedMaterial = originalMaterial;
		}
	}

	public void UpdateUVs()
	{
		if (mesh == null || atlas == null || string.IsNullOrEmpty(spriteName))
		{
			return;
		}
		UISpriteData sprite = atlas.GetSprite(spriteName);
		if (sprite != null && !(atlas.texture == null))
		{
			mSprite = sprite;
			Rect rect = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
			Rect rect2 = NGUIMath.ConvertToTexCoords(rect, atlas.texture.width, atlas.texture.height);
			Vector2[] uv = originalMesh.uv;
			for (int i = 0; i < uv.Length; i++)
			{
				uv[i].x = uv[i].x * rect2.width + rect2.x;
				uv[i].y = uv[i].y * rect2.height + rect2.y;
			}
			mesh.uv = uv;
		}
	}

	public void UpdateMeshSettings()
	{
		if (!(mesh == null) && meshSettings)
		{
			Vector3[] vertices = originalMesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i].x = (vertices[i].x + pivot.x) * scale.x * (float)((!mirrorX) ? 1 : (-1));
				vertices[i].y = (vertices[i].y + pivot.y) * scale.y * (float)((!mirrorY) ? 1 : (-1));
				vertices[i].z = (vertices[i].z + pivot.z) * scale.z * (float)((!mirrorZ) ? 1 : (-1));
			}
			if (flip)
			{
				mesh.triangles = originalMesh.triangles.Reverse().ToArray();
			}
			else
			{
				mesh.triangles = originalMesh.triangles;
			}
			mesh.vertices = vertices;
		}
	}

	public UISpriteData GetAtlasSprite()
	{
		if (!mSpriteSet)
		{
			mSprite = null;
		}
		if (mSprite == null && mAtlas != null)
		{
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				UISpriteData sprite = mAtlas.GetSprite(mSpriteName);
				if (sprite == null)
				{
					return null;
				}
				SetAtlasSprite(sprite);
			}
			if (mSprite == null && mAtlas.spriteList.Count > 0)
			{
				UISpriteData uISpriteData = mAtlas.spriteList[0];
				if (uISpriteData == null)
				{
					return null;
				}
				SetAtlasSprite(uISpriteData);
				if (mSprite == null)
				{
					Debug.LogError(mAtlas.name + " seems to have a null sprite!");
					return null;
				}
				mSpriteName = mSprite.name;
			}
		}
		return mSprite;
	}

	private void SetAtlasSprite(UISpriteData sp)
	{
		mSpriteSet = true;
		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = ((mSprite == null) ? string.Empty : mSprite.name);
			mSprite = sp;
		}
	}

	public void UpdateAllMeshes()
	{
		MeshAtlas[] array = NGUITools.FindActive<MeshAtlas>();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			MeshAtlas meshAtlas = array[i];
			if (meshAtlas.enabled && originalMesh == meshAtlas.originalMesh)
			{
				meshAtlas.UpdateMesh();
				meshAtlas.UpdateMeshSettings();
			}
		}
	}

	public void UpdateMeshTextures()
	{
		MeshAtlas[] array = NGUITools.FindActive<MeshAtlas>();
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			MeshAtlas meshAtlas = array[i];
			if (UIAtlas.CheckIfRelated(atlas, meshAtlas.atlas))
			{
				meshAtlas.UpdateMesh();
			}
		}
	}
}
