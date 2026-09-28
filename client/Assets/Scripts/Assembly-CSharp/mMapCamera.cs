using UnityEngine;

public class mMapCamera : MonoBehaviour
{
	public float Speed;

	public float Amplitude;

	public MeshAtlas[] AK47;

	public MeshAtlas[] M4A1;

	public MeshAtlas[] AWP;

	public MeshAtlas[] Galil;

	private Transform mTransform;

	private Vector3 StartPosition;

	private void Start()
	{
		mTransform = base.transform;
		StartPosition = mTransform.position;
		vp_Timer.In(0.1f, () =>
		{
			string spriteName = "1-" + WeaponManager.GetRandomWeaponSkin(1);
			for (int i = 0; i < AK47.Length; i++)
			{
				AK47[i].spriteName = spriteName;
			}
			string spriteName2 = "5-" + WeaponManager.GetRandomWeaponSkin(5);
			for (int j = 0; j < M4A1.Length; j++)
			{
				M4A1[j].spriteName = spriteName2;
			}
			string spriteName3 = "8-" + WeaponManager.GetRandomWeaponSkin(8);
			for (int k = 0; k < AWP.Length; k++)
			{
				AWP[k].spriteName = spriteName3;
			}
			string spriteName4 = "28-" + WeaponManager.GetRandomWeaponSkin(28);
			for (int l = 0; l < Galil.Length; l++)
			{
				Galil[l].spriteName = spriteName4;
			}
		});
	}

	private void LateUpdate()
	{
		mTransform.position = StartPosition + Vector3.forward * Mathf.Cos(Time.time * Speed) * Amplitude;
	}
}
