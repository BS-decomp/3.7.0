using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	[SelectedWeapon(WeaponTypeList.Knife)]
	public int SelectedKnife;

	[SelectedWeapon(WeaponTypeList.Pistol)]
	public int SelectedPistol;

	[SelectedWeapon(WeaponTypeList.Rifle)]
	public int SelectedRifle;

	public static WeaponTypeList DefaultWeaponType = WeaponTypeList.Rifle;

	public static bool SelectWeaponInGame = true;

	public static ObscuredBool MaxDamage = false;

	private static WeaponManager instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public static void Init()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("WeaponManager");
			gameObject.AddComponent<WeaponManager>();
		}
		UpdateData();
	}

	public static void UpdateData()
	{
		SelectWeaponInGame = true;
		MaxDamage = false;
		instance.SelectedKnife = AccountManager.GetWeaponSelected(WeaponTypeList.Knife);
		instance.SelectedPistol = AccountManager.GetWeaponSelected(WeaponTypeList.Pistol);
		instance.SelectedRifle = AccountManager.GetWeaponSelected(WeaponTypeList.Rifle);
	}

	public static int GetKnifeID()
	{
		return instance.SelectedKnife;
	}

	public static int GetPistolID()
	{
		return instance.SelectedPistol;
	}

	public static int GetRifleID()
	{
		return instance.SelectedRifle;
	}

	public static WeaponType GetKnifeType()
	{
		return GetWeaponType(instance.SelectedKnife);
	}

	public static WeaponType GetPistolType()
	{
		return GetWeaponType(instance.SelectedPistol);
	}

	public static WeaponType GetRifleType()
	{
		return GetWeaponType(instance.SelectedRifle);
	}

	public static void SetKnifeType(int weaponID)
	{
		instance.SelectedKnife = weaponID;
	}

	public static void SetPistolType(int weaponID)
	{
		instance.SelectedPistol = weaponID;
	}

	public static void SetRifleType(int weaponID)
	{
		instance.SelectedRifle = weaponID;
	}

	public static void SetWeaponType(int weaponID)
	{
		if (!SelectWeaponInGame)
		{
			return;
		}
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].WeaponID == weaponID)
			{
				SetWeaponType(GameSettings.instance.Weapons[i].Weapon, weaponID);
				break;
			}
		}
	}

	public static void SetWeaponType(WeaponTypeList weapon, int weaponID)
	{
		if (SelectWeaponInGame)
		{
			switch (weapon)
			{
			case WeaponTypeList.Knife:
				SetKnifeType(weaponID);
				break;
			case WeaponTypeList.Pistol:
				SetPistolType(weaponID);
				break;
			case WeaponTypeList.Rifle:
				SetRifleType(weaponID);
				break;
			}
		}
	}

	public static bool HasKnifeType()
	{
		if (instance.SelectedKnife == 0)
		{
			return false;
		}
		return true;
	}

	public static bool HasPistolType()
	{
		if (instance.SelectedPistol == 0)
		{
			return false;
		}
		return true;
	}

	public static bool HasRifleType()
	{
		if (instance.SelectedRifle == 0)
		{
			return false;
		}
		return true;
	}

	public static WeaponType GetWeaponType(int weaponID)
	{
		WeaponType weaponType = new WeaponType();
		weaponID--;
		weaponType = WeaponType.CreateNewClass(GameSettings.instance.Weapons[weaponID]);
		int weaponUpgrade = AccountManager.GetWeaponUpgrade(weaponType.WeaponID);
		if (weaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade2 = GameSettings.instance.WeaponsShop[weaponID].Upgrades[weaponUpgrade - 1];
			weaponType.FaceDamage = weaponUpgrade2.FaceDamage;
			weaponType.BodyDamage = weaponUpgrade2.BodyDamage;
			weaponType.HandDamage = weaponUpgrade2.HandDamage;
			weaponType.LegDamage = weaponUpgrade2.LegDamage;
			weaponType.FireRate = weaponUpgrade2.FireRate;
			weaponType.Accuracy = weaponUpgrade2.Accuracy;
			weaponType.FireAccuracy = weaponUpgrade2.FireAccuracy;
			weaponType.Ammo = weaponUpgrade2.Ammo;
			weaponType.MaxAmmo = weaponUpgrade2.MaxAmmo;
			weaponType.Mass = weaponUpgrade2.Mass;
		}
		return weaponType;
	}

	public static int GetMemberDamage(PlayerSkinMember member, int weaponID)
	{
		return GetMemberDamage(member, GetWeaponType(weaponID));
	}

	public static int GetMemberDamage(PlayerSkinMember member, WeaponType weapon)
	{
		if ((bool)MaxDamage)
		{
			return 100;
		}
		switch (member)
		{
		case PlayerSkinMember.Face:
			if ((int)weapon.FaceDamage == 100)
			{
				return 100;
			}
			return (int)weapon.FaceDamage + Random.Range(-5, 5);
		case PlayerSkinMember.Body:
			if ((int)weapon.BodyDamage == 100)
			{
				return 100;
			}
			return (int)weapon.BodyDamage + Random.Range(-4, 4);
		case PlayerSkinMember.Hands:
			if ((int)weapon.HandDamage == 100)
			{
				return 100;
			}
			return (int)weapon.HandDamage + Random.Range(-3, 3);
		case PlayerSkinMember.Legs:
			if ((int)weapon.LegDamage == 100)
			{
				return 100;
			}
			return (int)weapon.LegDamage + Random.Range(-2, 2);
		default:
			return weapon.BodyDamage;
		}
	}

	public static int GetRandomWeapon()
	{
		List<WeaponType> list = new List<WeaponType>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((bool)GameSettings.instance.Weapons[i].WeaponLock)
			{
				continue;
			}
			if ((bool)GameSettings.instance.Weapons[i].WeaponSecret)
			{
				if (AccountManager.GetWeapon(GameSettings.instance.Weapons[i].WeaponID))
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
			}
			else
			{
				list.Add(GameSettings.instance.Weapons[i]);
			}
		}
		return list[Random.Range(0, list.Count)].WeaponID;
	}

	public static int GetRandomWeapon(WeaponTypeList type)
	{
		List<WeaponType> list = new List<WeaponType>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].Weapon != type || (bool)GameSettings.instance.Weapons[i].WeaponLock)
			{
				continue;
			}
			if ((bool)GameSettings.instance.Weapons[i].WeaponSecret)
			{
				if (AccountManager.GetWeapon(GameSettings.instance.Weapons[i].WeaponID))
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
			}
			else
			{
				list.Add(GameSettings.instance.Weapons[i]);
			}
		}
		return list[Random.Range(0, list.Count)].WeaponID;
	}

	public static int GetRandomWeapon(bool rifle, bool pistol, bool knife, bool secret)
	{
		List<WeaponType> list = new List<WeaponType>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((bool)GameSettings.instance.Weapons[i].WeaponLock)
			{
				continue;
			}
			if ((bool)GameSettings.instance.Weapons[i].WeaponSecret)
			{
				if (secret && AccountManager.GetWeapon(GameSettings.instance.Weapons[i].WeaponID))
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
				continue;
			}
			switch (GameSettings.instance.Weapons[i].Weapon)
			{
			case WeaponTypeList.Rifle:
				if (rifle)
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
				break;
			case WeaponTypeList.Pistol:
				if (pistol)
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
				break;
			case WeaponTypeList.Knife:
				if (knife)
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
				break;
			}
		}
		return list[Random.Range(0, list.Count)].WeaponID;
	}

	public static string GetWeaponName(int weaponID)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].WeaponID == weaponID)
			{
				return GameSettings.instance.Weapons[i].WeaponName;
			}
		}
		return string.Empty;
	}

	public static int GetWeaponID(string weaponName)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].WeaponName == (ObscuredString)weaponName)
			{
				return GameSettings.instance.Weapons[i].WeaponID;
			}
		}
		return -1;
	}

	public static WeaponType GetWeapon(int weaponID)
	{
		return GameSettings.instance.Weapons[weaponID - 1];
	}

	public static WeaponShopData.WeaponSkin GetWeaponSkin(int weaponID, int skinID)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].WeaponID != weaponID)
			{
				continue;
			}
			for (int j = 0; j < GameSettings.instance.WeaponsShop[i].Skins.Count; j++)
			{
				if ((int)GameSettings.instance.WeaponsShop[i].Skins[j].SkinID == skinID)
				{
					return GameSettings.instance.WeaponsShop[i].Skins[j];
				}
			}
		}
		return null;
	}

	public static bool HasWeaponLock(int weaponID)
	{
		return GetWeapon(weaponID).WeaponLock;
	}

	public static int GetRandomWeaponSkin(int weaponID)
	{
		return GameSettings.instance.WeaponsShop[weaponID].Skins[Random.Range(0, GameSettings.instance.WeaponsShop[weaponID].Skins.Count)].SkinID;
	}
}
