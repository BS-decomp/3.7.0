using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class UISelectWeapon : MonoBehaviour
{
	public WeaponTypeList Weapon;

	public UILabel WeaponNameLabel;

	public UITexture WeaponIcon;

	public float Size = 1f;

	private List<string> WeaponList = new List<string>();

	private int SelectWeapon;

	public static ObscuredBool AllWeapons = false;

	public static ObscuredBool SelectedUpdateWeaponManager = false;

	private void Start()
	{
		UpdateWeaponsName();
		GetSelectWeapon();
		UpdateSelectedWeapon();
	}

	public void Left()
	{
		SelectWeapon--;
		if (SelectWeapon < 0)
		{
			SelectWeapon = WeaponList.Count - 1;
		}
		if (!AllWeapons)
		{
			AccountManager.SetWeaponSelected(Weapon, WeaponManager.GetWeaponID(WeaponList[SelectWeapon]));
		}
		UpdateSelectedWeapon();
	}

	public void Right()
	{
		SelectWeapon++;
		if (SelectWeapon > WeaponList.Count - 1)
		{
			SelectWeapon = 0;
		}
		if (!AllWeapons)
		{
			AccountManager.SetWeaponSelected(Weapon, WeaponManager.GetWeaponID(WeaponList[SelectWeapon]));
		}
		UpdateSelectedWeapon();
	}

	private void GetSelectWeapon()
	{
		int weaponSelected = AccountManager.GetWeaponSelected(Weapon);
		string weaponName = WeaponManager.GetWeaponName(weaponSelected);
		for (int i = 0; i < WeaponList.Count; i++)
		{
			if (WeaponList[i] == weaponName)
			{
				SelectWeapon = i;
				break;
			}
		}
	}

	private void UpdateWeaponsName()
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].Weapon != Weapon)
			{
				continue;
			}
			int num = GameSettings.instance.Weapons[i].WeaponID;
			string item = GameSettings.instance.Weapons[i].WeaponName;
			if (!AccountManager.GetWeapon(num) && !AllWeapons)
			{
				continue;
			}
			if (num == 4 || num == 3 || num == 12)
			{
				WeaponList.Insert(0, item);
			}
			else
			{
				if ((bool)GameSettings.instance.Weapons[i].WeaponLock)
				{
					continue;
				}
				if ((bool)GameSettings.instance.Weapons[i].WeaponSecret)
				{
					if (AccountManager.GetWeapon(GameSettings.instance.Weapons[i].WeaponID))
					{
						WeaponList.Add(item);
					}
				}
				else
				{
					WeaponList.Add(item);
				}
			}
		}
	}

	private void UpdateSelectedWeapon()
	{
		string text = WeaponList[SelectWeapon];
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].Weapon != Weapon || !(GameSettings.instance.Weapons[i].WeaponName == (ObscuredString)text))
			{
				continue;
			}
			WeaponManager.SetWeaponType(Weapon, GameSettings.instance.Weapons[i].WeaponID);
			int weaponSkinSelected = AccountManager.GetWeaponSkinSelected(GameSettings.instance.Weapons[i].WeaponID);
			for (int j = 0; j < GameSettings.instance.WeaponsShop[i].Skins.Count; j++)
			{
				if ((int)GameSettings.instance.WeaponsShop[i].Skins[j].SkinID == weaponSkinSelected)
				{
					WeaponNameLabel.text = text + "  |  " + GetWeaponSkinRarityColor(GameSettings.instance.WeaponsShop[i].Skins[j]);
					WeaponIcon.mainTexture = GameSettings.instance.WeaponsShop[i].Skins[j].Source;
					WeaponIcon.width = (int)(GameSettings.instance.CaseSize[i].x * Size);
					WeaponIcon.height = (int)(GameSettings.instance.CaseSize[i].y * Size);
					WeaponIcon.uvRect = GameSettings.instance.CaseUVRect[i];
					if ((bool)SelectedUpdateWeaponManager)
					{
						PlayerInput.instance.PlayerWeapon.UpdateWeaponAll();
					}
					break;
				}
			}
			break;
		}
	}

	private string GetWeaponSkinRarityColor(WeaponShopData.WeaponSkin skin)
	{
		switch (skin.Rarity)
		{
		case 0:
		case 1:
			return Utils.ColorToHex(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), skin.SkinName);
		case 2:
			return Utils.ColorToHex(new Color32(54, 189, byte.MaxValue, byte.MaxValue), skin.SkinName);
		case 3:
			return Utils.ColorToHex(new Color32(byte.MaxValue, 0, 0, byte.MaxValue), skin.SkinName);
		case 4:
			return Utils.ColorToHex(new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue), skin.SkinName);
		default:
			return skin.SkinName;
		}
	}
}
