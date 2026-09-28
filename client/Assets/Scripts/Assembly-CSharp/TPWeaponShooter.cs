using UnityEngine;

public class TPWeaponShooter : TimerBehaviour
{
	public GameObject Weapon;

	public bool isMuzzle = true;

	public Transform Muzzle;

	[Header("Two Handed Weapon")]
	public bool TwoHandedWeapon;

	public GameObject TwoWeapon;

	public Transform TwoMuzzle;

	public Vector3 TwoWeaponPosition;

	public Vector3 TwoWeaponRotation;

	private bool isFireTwoWeapon;

	private Transform TwoWeaponRoot;

	[Disabled]
	public bool FireStat;

	public GameObject FireStatModel;

	public MeshAtlas[] FireStatCounters;

	public MeshAtlas[] WeaponAtlas;

	public bool UseSound = true;

	[Disabled]
	public int WeaponID;

	[Disabled]
	public int WeaponSkin;

	[Disabled]
	public WeaponSound FireSound;

	private int FireStatCounter = -1;

	public void SetData(int weaponID, int weaponSkin, int fireStat, Transform twoWeaponRoot)
	{
		WeaponID = weaponID;
		WeaponSkin = weaponSkin;
		FireSound = WeaponManager.GetWeapon(weaponID).FireSound;
		WeaponShopData.WeaponSkin weaponSkin2 = WeaponManager.GetWeaponSkin(weaponID, weaponSkin);
		if (TwoHandedWeapon)
		{
			TwoWeaponRoot = twoWeaponRoot;
			TwoWeapon.transform.SetParent(TwoWeaponRoot);
			TwoWeapon.transform.localPosition = TwoWeaponPosition;
			TwoWeapon.transform.localEulerAngles = TwoWeaponRotation;
		}
		if (weaponSkin2 != null && fireStat > -1)
		{
			FireStat = true;
			FireStatModel.SetActive(true);
			UpdateFireStat(fireStat);
		}
		UpdateAtlas();
	}

	public void Active()
	{
		Weapon.SetActive(true);
		if (TwoHandedWeapon)
		{
			TwoWeapon.SetActive(true);
		}
	}

	public void Deactive()
	{
		Weapon.SetActive(false);
		if (TwoHandedWeapon)
		{
			TwoWeapon.SetActive(false);
		}
	}

	public void Fire(bool isVisible)
	{
		if (!isVisible || !isMuzzle)
		{
			return;
		}
		if (TwoHandedWeapon && isFireTwoWeapon)
		{
			TwoMuzzle.gameObject.SetActive(true);
			vp_Timer.In(0.05f, () =>
			{
				TwoMuzzle.gameObject.SetActive(false);
			}, GetTimer());
		}
		else
		{
			Muzzle.gameObject.SetActive(true);
			vp_Timer.In(0.05f, () =>
			{
				Muzzle.gameObject.SetActive(false);
			}, GetTimer());
		}
		if (TwoHandedWeapon)
		{
			isFireTwoWeapon = !isFireTwoWeapon;
		}
	}

	public void UpdateFireStat(int counter)
	{
		if (FireStat && FireStatCounter != counter)
		{
			FireStatCounter = counter;
			string text = counter.ToString("D6");
			for (int i = 0; i < FireStatCounters.Length; i++)
			{
				FireStatCounters[i].spriteName = "f" + text[i];
			}
		}
	}

	private void UpdateAtlas()
	{
		string text = WeaponID + "-" + WeaponSkin;
		for (int i = 0; i < WeaponAtlas.Length; i++)
		{
			if (WeaponAtlas[i].mSpriteName != text)
			{
				MeshAtlas meshAtlas = WeaponAtlas[i];
				meshAtlas.spriteName = text;
			}
		}
	}
}
