using UnityEngine;

public class mWeaponTexture : MonoBehaviour
{
	[SelectedWeapon]
	public int Weapon;

	public float Size = 1f;

	public bool OnlyStart;

	public UILabel SkinNameLabel;

	private UITexture WeaponIcon;

	private void Start()
	{
		WeaponIcon = GetComponent<UITexture>();
		SetTexture();
	}

	private void OnEnable()
	{
		if (!(WeaponIcon == null) && !OnlyStart)
		{
			SetTexture();
		}
	}

	private void SetTexture()
	{
		if (WeaponIcon == null)
		{
			return;
		}
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].WeaponID == Weapon)
			{
				WeaponShopData.WeaponSkin weaponSkin = GameSettings.instance.WeaponsShop[i].Skins[Random.Range(1, GameSettings.instance.WeaponsShop[i].Skins.Count)];
				WeaponIcon.mainTexture = weaponSkin.Source;
				WeaponIcon.width = (int)(GameSettings.instance.CaseSize[i].x * Size);
				WeaponIcon.height = (int)(GameSettings.instance.CaseSize[i].y * Size);
				WeaponIcon.uvRect = GameSettings.instance.CaseUVRect[i];
				if (SkinNameLabel != null)
				{
					SetSkinName(weaponSkin);
				}
				break;
			}
		}
	}

	private void SetSkinName(WeaponShopData.WeaponSkin skinData)
	{
		switch (skinData.Rarity)
		{
		case 1:
			SkinNameLabel.text = skinData.SkinName;
			break;
		case 2:
			SkinNameLabel.text = "[00aff0]" + skinData.SkinName;
			break;
		case 3:
			SkinNameLabel.text = "[ff0000]" + skinData.SkinName;
			break;
		case 4:
			SkinNameLabel.text = "[E00061]" + skinData.SkinName;
			break;
		}
	}
}
