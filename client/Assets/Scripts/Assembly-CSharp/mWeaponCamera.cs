using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class mWeaponCamera : MonoBehaviour
{
	[Serializable]
	public class WeaponData
	{
		[SelectedWeapon]
		public string Weapon;

		public GameObject Target;

		public GameObject FireStat;

		public MeshAtlas[] FireStatCounters;
	}

	public Transform Point;

	public float RotateSpeed = 200f;

	public List<WeaponData> Weapons = new List<WeaponData>();

	private int SelectedWeapon = -1;

	private Camera mCamera;

	private Tweener mTween;

	private static mWeaponCamera instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		mCamera = base.GetComponent<Camera>();
		RotateSpeed = Mathf.Sqrt(RotateSpeed) / Mathf.Sqrt(Screen.dpi);
	}

	public static void Show(string weapon)
	{
		instance.mCamera.enabled = true;
		ResetRotateX(false);
		if (instance.SelectedWeapon != -1)
		{
			instance.Weapons[instance.SelectedWeapon].Target.SetActive(false);
			if (instance.Weapons[instance.SelectedWeapon].FireStat != null)
			{
				instance.Weapons[instance.SelectedWeapon].FireStat.SetActive(false);
			}
			instance.SelectedWeapon = -1;
		}
		for (int i = 0; i < instance.Weapons.Count; i++)
		{
			if (instance.Weapons[i].Weapon == weapon)
			{
				instance.Weapons[i].Target.SetActive(true);
				instance.SelectedWeapon = i;
				break;
			}
		}
	}

	public static void Close()
	{
		instance.mCamera.enabled = false;
		if (instance.SelectedWeapon != -1)
		{
			instance.Weapons[instance.SelectedWeapon].Target.SetActive(false);
			instance.SelectedWeapon = -1;
		}
	}

	public static void SetViewportRect(Rect rect, float duration)
	{
		if (instance.mTween != null && instance.mTween.IsActive())
		{
			if (duration == 0f)
			{
				instance.mTween.Kill();
				instance.mCamera.rect = rect;
			}
			else
			{
				instance.mTween = instance.mTween.ChangeEndValue(rect, duration);
			}
		}
		else if (duration == 0f)
		{
			instance.mCamera.rect = rect;
		}
		else
		{
			instance.mTween = instance.mCamera.DORect(rect, duration);
		}
	}

	public static void SetSkin(int weaponID, int skin)
	{
		if (instance.SelectedWeapon == -1)
		{
			return;
		}
		MeshAtlas[] componentsInChildren = instance.Weapons[instance.SelectedWeapon].Target.GetComponentsInChildren<MeshAtlas>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name != "FireStat" && componentsInChildren[i].name != "1" && componentsInChildren[i].name != "2" && componentsInChildren[i].name != "3" && componentsInChildren[i].name != "4" && componentsInChildren[i].name != "5" && componentsInChildren[i].name != "6")
			{
				componentsInChildren[i].spriteName = weaponID + "-" + skin;
			}
		}
		if (AccountManager.GetFireStatCounter(weaponID, skin) > -1 && instance.Weapons[instance.SelectedWeapon].FireStat != null)
		{
			instance.Weapons[instance.SelectedWeapon].FireStat.SetActive(true);
			string text = AccountManager.GetFireStatCounter(weaponID, skin).ToString("D6");
			for (int j = 0; j < text.Length; j++)
			{
				instance.Weapons[instance.SelectedWeapon].FireStatCounters[j].spriteName = "f" + text[j];
			}
		}
		else if (instance.Weapons[instance.SelectedWeapon].FireStat != null)
		{
			instance.Weapons[instance.SelectedWeapon].FireStat.SetActive(false);
		}
	}

	public static void Rotate(Vector2 rotate, bool onlyY)
	{
		if (onlyY)
		{
			instance.Point.Rotate(new Vector2(0f, (0f - rotate.x) * instance.RotateSpeed), Space.Self);
		}
		else
		{
			instance.Point.Rotate(new Vector2(rotate.y * instance.RotateSpeed, (0f - rotate.x) * instance.RotateSpeed), Space.World);
		}
	}

	public static void ResetRotateX(bool isTween)
	{
		if (isTween)
		{
			instance.Point.DOLocalRotate(Vector3.zero, 0.5f);
		}
		else
		{
			instance.Point.localEulerAngles = Vector3.zero;
		}
	}
}
