using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

[Serializable]
public class WeaponType
{
	public ObscuredInt WeaponID;

	public ObscuredBool WeaponLock = false;

	public ObscuredBool WeaponFire = true;

	public ObscuredBool WeaponSecret = false;

	public WeaponTypeList Weapon = WeaponTypeList.Knife;

	public WeaponTypeList WeaponAnim = WeaponTypeList.Knife;

	public ObscuredString WeaponName;

	public ObscuredInt FaceDamage = 50;

	public ObscuredInt BodyDamage = 20;

	public ObscuredInt HandDamage = 10;

	public ObscuredInt LegDamage = 5;

	public ObscuredFloat FireRate = 0.1f;

	public ObscuredFloat Accuracy = 10f;

	public ObscuredFloat FireAccuracy = 10f;

	public ObscuredInt FireBullets = 1;

	public ObscuredFloat ReloadTime = 1f;

	public ObscuredInt Ammo = 20;

	public ObscuredInt MaxAmmo = 100;

	public ObscuredFloat Distance = 80f;

	public ObscuredFloat Mass = 0.01f;

	public ObscuredBool RifleScope = false;

	public ObscuredInt RifleScopeSize = 10;

	public ObscuredFloat RifleScopeSensitivity = 0.1f;

	public ObscuredFloat RifleScopeRecoil = 0f;

	public ObscuredFloat RifleScopeAccuracy = 0f;

	public WeaponSound FireSound;

	public WeaponSound ReloadSound;

	public GameObject FpsPrefab;

	public GameObject TpsPrefab;

	public static WeaponType CreateNewClass(WeaponType type)
	{
		WeaponType weaponType = new WeaponType();
		weaponType.WeaponID = type.WeaponID;
		weaponType.WeaponFire = type.WeaponFire;
		weaponType.Weapon = type.Weapon;
		weaponType.WeaponAnim = type.WeaponAnim;
		weaponType.WeaponName = type.WeaponName;
		weaponType.FaceDamage = type.FaceDamage;
		weaponType.BodyDamage = type.BodyDamage;
		weaponType.HandDamage = type.HandDamage;
		weaponType.LegDamage = type.LegDamage;
		weaponType.FireRate = type.FireRate;
		weaponType.Accuracy = type.Accuracy;
		weaponType.FireAccuracy = type.FireAccuracy;
		weaponType.FireBullets = type.FireBullets;
		weaponType.ReloadTime = type.ReloadTime;
		weaponType.Ammo = type.Ammo;
		weaponType.MaxAmmo = type.MaxAmmo;
		weaponType.Distance = type.Distance;
		weaponType.Mass = type.Mass;
		weaponType.RifleScope = type.RifleScope;
		weaponType.RifleScopeSize = type.RifleScopeSize;
		weaponType.RifleScopeSensitivity = type.RifleScopeSensitivity;
		weaponType.RifleScopeRecoil = type.RifleScopeRecoil;
		weaponType.RifleScopeAccuracy = type.RifleScopeAccuracy;
		weaponType.FpsPrefab = type.FpsPrefab;
		weaponType.TpsPrefab = type.TpsPrefab;
		weaponType.FireSound = type.FireSound;
		weaponType.ReloadSound = type.ReloadSound;
		return weaponType;
	}
}
