using UnityEngine;

public class mPlayerStats : MonoBehaviour
{
	public UILabel NameLabel;

	public UILabel PlayerIDLabel;

	public UILabel LevelLabel;

	public UILabel XPLabel;

	public UILabel DeathsLabel;

	public UILabel KillsLabel;

	public UILabel HeadshotKillsLabel;

	public UILabel OpenCaseLabel;

	public UILabel TotalSkinLabel;

	public UILabel LegendarySkinLabel;

	public UILabel ProfessionalSkinLabel;

	public UILabel BasicSkinLabel;

	public UILabel NormalSkinLabel;

	public UILabel RateLabel;

	public GameObject Panel;

	public void UpdateData()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		Panel.SetActive(true);
		NameLabel.text = AccountManager.AccountName;
		PlayerIDLabel.text = AccountManager.PlayerID;
		LevelLabel.text = AccountManager.GetLevel().ToString();
		XPLabel.text = AccountManager.GetXP() + "/" + AccountManager.GetMaxXP();
		DeathsLabel.text = AccountManager.GetDeaths().ToString();
		KillsLabel.text = AccountManager.GetKills().ToString();
		HeadshotKillsLabel.text = AccountManager.GetHeadshot().ToString();
		OpenCaseLabel.text = AccountManager.GetOpenCase().ToString();
		TotalSkinLabel.text = GetOpenSkins() + "/" + GetTotalSkins();
		LegendarySkinLabel.text = GetOpenSkins(4) + "/" + GetTotalSkins(4);
		ProfessionalSkinLabel.text = GetOpenSkins(3) + "/" + GetTotalSkins(3);
		BasicSkinLabel.text = GetOpenSkins(2) + "/" + GetTotalSkins(2);
		NormalSkinLabel.text = GetOpenSkins(1) + "/" + GetTotalSkins(1);
		RateLabel.text = GetRate();
	}

	private int GetOpenSkins(int rarity = 0)
	{
		int num = 0;
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			for (int j = 1; j < GameSettings.instance.WeaponsShop[i].Skins.Count; j++)
			{
				if (((int)GameSettings.instance.WeaponsShop[i].Skins[j].Rarity == rarity || rarity == 0) && AccountManager.GetWeaponSkin(i + 1, j))
				{
					num++;
				}
			}
		}
		return num;
	}

	private int GetTotalSkins(int rarity = 0)
	{
		int num = 0;
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			for (int j = 1; j < GameSettings.instance.WeaponsShop[i].Skins.Count; j++)
			{
				if ((int)GameSettings.instance.WeaponsShop[i].Skins[j].Rarity == rarity || rarity == 0)
				{
					num++;
				}
			}
		}
		return num;
	}

	private string GetRate()
	{
		float value = (float)AccountManager.GetKills() * 100f / (float)AccountManager.GetDeaths();
		float value2 = (float)AccountManager.GetHeadshot() * 100f / (float)AccountManager.GetKills();
		float value3 = (float)GetOpenSkins() * 100f / (float)GetTotalSkins();
		value = Mathf.Clamp(value, 0f, 100f);
		value2 = Mathf.Clamp(value2, 0f, 100f);
		value3 = Mathf.Clamp(value3, 0f, 100f);
		float num = (value + value2 + value3) / 3f;
		if (num >= 85f)
		{
			return "A";
		}
		if (num >= 70f)
		{
			return "B";
		}
		if (num >= 50f)
		{
			return "C";
		}
		return "D";
	}
}
