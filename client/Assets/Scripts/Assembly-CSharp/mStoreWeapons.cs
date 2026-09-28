using System;
using System.Collections.Generic;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class mStoreWeapons : MonoBehaviour
{
	[Header("Weapon Data")]
	public GameObject WeaponDataPanel;

	public UILabel WeaponNameLabel;

	public UILabel DamageLabel;

	public UIProgressBar DamageProgressBar;

	public UILabel FireRateLabel;

	public UIProgressBar FireRateProgressBar;

	public UILabel AccuracyLabel;

	public UIProgressBar AccuracyProgressBar;

	public UILabel AmmoLabel;

	public UIProgressBar AmmoProgressBar;

	public UILabel MaxAmmoLabel;

	public UIProgressBar MaxAmmoProgressBar;

	public UILabel MobilityLabel;

	public UIProgressBar MobilityProgressBar;

	public UILabel RangeLabel;

	public UIProgressBar RangeProgressBar;

	[Space(10f)]
	public ObscuredFloat MaxDamage;

	public ObscuredFloat MaxFireRate;

	public ObscuredFloat MaxAccuracy;

	public ObscuredFloat MaxAmmo;

	public ObscuredFloat MaxMaxAmmo;

	public ObscuredFloat MaxMobility;

	public ObscuredFloat MaxRange;

	[Header("Weapon Panel")]
	public UISprite SelectWeaponButton;

	public UILabel SelectWeaponButtonLabel;

	public GameObject WeaponBuyButton;

	public UILabel WeaponBuyButtonLabel;

	public UITexture WeaponBuyButtonTexture;

	public GameObject WeaponUpgradeButton;

	public UILabel WeaponUpgradeButtonLabel;

	public UIGrid WeaponButtonsGrid;

	private List<string> WeaponsList = new List<string>();

	private int Weapon;

	private WeaponTypeList WeaponType;

	private WeaponType WeaponData;

	private WeaponShopData WeaponStoreData;

	private int WeaponUpgrade;

	private int WeaponSkin;

	[Header("Weapon Skin")]
	public Color NormalColor = Color.gray;

	public Color BaseColor = Color.cyan;

	public Color ProfessionalColor = Color.red;

	public Color LegendaryColor = Color.magenta;

	public UILabel SkinNameLabel;

	public UILabel SkinRarityLabel;

	public GameObject SkinBackground;

	public UISprite SelectSkinButton;

	public UILabel SelectSkinButtonLabel;

	public GameObject SkinBuyButton;

	public UILabel SkinBuyButtonLabel;

	public GameObject SkinDropInCase;

	public UIGrid SkinButtonsGrid;

	[Header("Others")]
	public GameObject WeaponPanel;

	public GameObject SkinPanel;

	public GameObject InAppPanel;

	public Texture2D MoneyTexture;

	public Texture2D GoldTexture;

	public GameObject ShareButton;

	private ObscuredBool isSkinPanel;

	public ObscuredBool AverageSkinBackground;

	public float LimitSkinBackgroundColor = 100f;

	private bool Active;

	private void Start()
	{
		UIEventListener uIEventListener = UIEventListener.Get(SkinBackground);
		uIEventListener.onDrag = RotateWeapon;
		PhotonNetwork.AddSendMonoMessageTargets(base.gameObject);
	}

	private void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Close();
	}

	private void RotateWeapon(GameObject go, Vector2 drag)
	{
		mWeaponCamera.Rotate(drag, !isSkinPanel);
	}

	public void Show(int weaponType)
	{
		Active = true;
		Weapon = 0;
		WeaponType = (WeaponTypeList)weaponType;
		GetWeaponList();
		UpdateWeapon();
	}

	public void Close()
	{
		if (Active)
		{
			mPanelManager.SetActivePlayerData(true);
			TweenPosition component = SkinNameLabel.GetComponent<TweenPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			Vector3 localPosition = SkinNameLabel.cachedTransform.localPosition;
			localPosition.y = 150f;
			SkinNameLabel.cachedTransform.localPosition = localPosition;
			mWeaponCamera.ResetRotateX(false);
			mWeaponCamera.SetViewportRect(new Rect(0.35f, 0.08f, 1f, 0.728f), 0f);
			mWeaponCamera.Close();
			isSkinPanel = false;
			TweenAlpha component2 = SkinPanel.GetComponent<TweenAlpha>();
			if (component2 != null)
			{
				component2.enabled = false;
			}
			component2 = WeaponPanel.GetComponent<TweenAlpha>();
			if (component2 != null)
			{
				component2.enabled = false;
			}
			SkinPanel.GetComponent<UIPanel>().alpha = 0f;
			WeaponPanel.GetComponent<UIPanel>().alpha = 1f;
			Active = false;
		}
	}

	public void UpdateAccount()
	{
		AccountManager.UpdateDefaultData(null, null);
		AccountManager.UpdateWeaponsData(null, null);
	}

	public void NextWeapon()
	{
		if (!isSkinPanel)
		{
			Weapon++;
			if (Weapon >= WeaponsList.Count)
			{
				Weapon = 0;
			}
			UpdateWeapon();
		}
	}

	public void LastWeapon()
	{
		if (!isSkinPanel)
		{
			Weapon--;
			if (Weapon <= -1)
			{
				Weapon = WeaponsList.Count - 1;
			}
			UpdateWeapon();
		}
	}

	private void UpdateWeapon()
	{
		GetWeaponData();
		GetWeaponStoreData();
		WeaponUpgrade = AccountManager.GetWeaponUpgrade(WeaponData.WeaponID);
		WeaponSkin = AccountManager.GetWeaponSkinSelected(WeaponData.WeaponID);
		mWeaponCamera.SetViewportRect(new Rect(0.35f, 0.08f, 1f, 0.728f), 0f);
		mWeaponCamera.Show(WeaponsList[Weapon]);
		bool flag = AccountManager.GetWeapon(WeaponData.WeaponID);
		if ((int)WeaponStoreData.WeaponPrice == 0)
		{
			flag = true;
		}
		if (flag)
		{
			SelectedWeaponPanel();
		}
		else
		{
			BuyWeaponPanel();
		}
		UpdateWeaponData();
		UpdateWeaponSkin();
	}

	private void SelectedWeaponPanel()
	{
		SelectWeaponButton.cachedGameObject.SetActive(true);
		if (AccountManager.GetWeaponSelected(WeaponType) == (int)WeaponData.WeaponID)
		{
			SelectWeaponButtonLabel.text = Localization.Get("Selected");
			SelectWeaponButton.alpha = 0.5f;
		}
		else
		{
			SelectWeaponButtonLabel.text = Localization.Get("Select");
			SelectWeaponButton.alpha = 1f;
		}
		WeaponBuyButton.SetActive(false);
		if (WeaponUpgrade == 3 || WeaponStoreData.Upgrades.Count == 0)
		{
			WeaponUpgradeButton.SetActive(false);
		}
		else
		{
			WeaponUpgradeButton.SetActive(true);
			WeaponUpgradeButtonLabel.text = WeaponStoreData.Upgrades[WeaponUpgrade].UpgradePrice.ToString();
		}
		WeaponButtonsGrid.repositionNow = true;
	}

	public void SelectWeapon()
	{
		if (AccountManager.GetWeaponSelected(WeaponType) != (int)WeaponData.WeaponID)
		{
			AccountManager.SetWeaponSelected(WeaponType, WeaponData.WeaponID);
			SelectedWeaponPanel();
			WeaponManager.UpdateData();
			UpdateWeaponSkin();
		}
	}

	public void UpgradeWeapon()
	{
		if ((int)WeaponStoreData.Upgrades[WeaponUpgrade].UpgradePrice > AccountManager.GetGold())
		{
			InAppPanel.SetActive(true);
			UIToast.Show(Localization.Get("Not enough money"));
			return;
		}
		AccountManager.SetGold(AccountManager.GetGold() - (int)WeaponStoreData.Upgrades[WeaponUpgrade].UpgradePrice);
		AccountManager.SetWeaponUpgrade(WeaponData.WeaponID);
		WeaponUpgrade = AccountManager.GetWeaponUpgrade(WeaponData.WeaponID);
		SelectedWeaponPanel();
		UpdateWeaponData();
		EventManager.Dispatch("AccountUpdate");
		Analytics.gua.sendEventHit("Store", "Upgrade Weapon", WeaponData.WeaponName, AccountManager.GetWeaponUpgrade(WeaponData.WeaponID));
	}

	private void BuyWeaponPanel()
	{
		SelectWeaponButton.cachedGameObject.SetActive(false);
		SelectWeaponButton.alpha = 1f;
		WeaponUpgradeButton.SetActive(false);
		WeaponBuyButton.SetActive(true);
		WeaponBuyButtonLabel.text = WeaponStoreData.WeaponPrice.ToString("n0");
		WeaponBuyButtonTexture.mainTexture = ((WeaponStoreData.Money != MoneyType.Gold) ? MoneyTexture : GoldTexture);
		WeaponButtonsGrid.repositionNow = true;
	}

	public void BuyWeapon()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		int num = ((WeaponStoreData.Money != MoneyType.Gold) ? AccountManager.GetMoney() : AccountManager.GetGold());
		if ((int)WeaponStoreData.WeaponPrice > num)
		{
			InAppPanel.SetActive(true);
			UIToast.Show(Localization.Get("Not enough money"));
			return;
		}
		if (WeaponStoreData.Money == MoneyType.Gold)
		{
			AccountManager.SetGold(num - (int)WeaponStoreData.WeaponPrice);
		}
		else
		{
			AccountManager.SetMoney(num - (int)WeaponStoreData.WeaponPrice);
		}
		AccountManager.SetWeapon(WeaponData.WeaponID);
		AccountManager.SetWeaponSelected(WeaponType, WeaponData.WeaponID);
		SelectedWeaponPanel();
		EventManager.Dispatch("AccountUpdate");
		WeaponManager.UpdateData();
		Analytics.gua.sendEventHit("Store", "Buy Weapon", WeaponData.WeaponName);
	}

	private void UpdateWeaponData()
	{
		WeaponDataPanel.SetActive(true);
		WeaponNameLabel.text = WeaponData.WeaponName;
		SetDamage();
		SetFireRate();
		SetAccuracy();
		SetAmmo();
		SetMaxAmmo();
		SetMobility();
	}

	private void UpdateWeaponSkin()
	{
		SkinNameLabel.text = string.Concat(WeaponStoreData.Skins[WeaponSkin].SkinName, (!AccountManager.GetFireStat(WeaponData.WeaponID, WeaponStoreData.Skins[WeaponSkin].SkinID)) ? string.Empty : " | FireStat");
		mWeaponCamera.SetSkin(WeaponData.WeaponID, WeaponStoreData.Skins[WeaponSkin].SkinID);
		ShareButton.SetActive(false);
		if (AccountManager.GetWeaponSkin(WeaponData.WeaponID, WeaponStoreData.Skins[WeaponSkin].SkinID))
		{
			SelectSkinButton.cachedGameObject.SetActive(true);
			if (AccountManager.GetWeaponSkinSelected(WeaponData.WeaponID) == (int)WeaponStoreData.Skins[WeaponSkin].SkinID)
			{
				SelectSkinButtonLabel.text = Localization.Get("Selected");
				SelectSkinButton.alpha = 0.5f;
				if (AccountManager.GetWeaponSelected(WeaponType) == (int)WeaponData.WeaponID)
				{
					ShareButton.SetActive(true);
				}
			}
			else
			{
				SelectSkinButtonLabel.text = Localization.Get("Select");
				SelectSkinButton.alpha = 1f;
			}
			SkinDropInCase.SetActive(false);
			SkinBuyButton.SetActive(false);
			SkinButtonsGrid.repositionNow = true;
		}
		else
		{
			if ((int)WeaponStoreData.Skins[WeaponSkin].Price == 0)
			{
				SkinDropInCase.SetActive(true);
				SkinBuyButton.SetActive(false);
			}
			else
			{
				SkinDropInCase.SetActive(false);
				SkinBuyButton.SetActive(true);
				SkinBuyButtonLabel.text = WeaponStoreData.Skins[WeaponSkin].Price.ToString();
			}
			SelectSkinButton.cachedGameObject.SetActive(false);
			SkinButtonsGrid.repositionNow = true;
		}
		switch (WeaponStoreData.Skins[WeaponSkin].Rarity)
		{
		case 0:
			SkinRarityLabel.text = Localization.Get("Normal quality");
			if (!AverageSkinBackground)
			{
				TweenColor.Begin(SkinBackground, 0.5f, NormalColor);
			}
			break;
		case 1:
			SkinRarityLabel.text = Localization.Get("Normal quality");
			if (!AverageSkinBackground)
			{
				TweenColor.Begin(SkinBackground, 0.5f, NormalColor);
			}
			break;
		case 2:
			SkinRarityLabel.text = Localization.Get("Basic quality");
			if (!AverageSkinBackground)
			{
				TweenColor.Begin(SkinBackground, 0.5f, BaseColor);
			}
			break;
		case 3:
			SkinRarityLabel.text = Localization.Get("Professional quality");
			if (!AverageSkinBackground)
			{
				TweenColor.Begin(SkinBackground, 0.5f, ProfessionalColor);
			}
			break;
		case 4:
			SkinRarityLabel.text = Localization.Get("Legendary quality");
			if (!AverageSkinBackground)
			{
				TweenColor.Begin(SkinBackground, 0.5f, LegendaryColor);
			}
			break;
		}
		if ((bool)AverageSkinBackground)
		{
			TweenColor.Begin(SkinBackground, 0.5f, GetAverageColor(WeaponStoreData.Skins[WeaponSkin].Source));
		}
	}

	public void NextSkin()
	{
		if ((bool)WeaponData.WeaponSecret)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < WeaponStoreData.Skins.Count; i++)
			{
				if (AccountManager.GetWeaponSkin(WeaponData.WeaponID, WeaponStoreData.Skins[i].SkinID))
				{
					list.Add(i);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (WeaponSkin == list[j])
				{
					if (WeaponSkin == list[list.Count - 1])
					{
						WeaponSkin = list[0];
					}
					else
					{
						WeaponSkin = list[j + 1];
					}
					break;
				}
			}
		}
		else
		{
			WeaponSkin++;
			if (WeaponSkin >= WeaponStoreData.Skins.Count)
			{
				WeaponSkin = 0;
			}
		}
		UpdateWeaponSkin();
	}

	public void LastSkin()
	{
		if ((bool)WeaponData.WeaponSecret)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < WeaponStoreData.Skins.Count; i++)
			{
				if (AccountManager.GetWeaponSkin(WeaponData.WeaponID, WeaponStoreData.Skins[i].SkinID))
				{
					list.Add(i);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (WeaponSkin == list[j])
				{
					if (WeaponSkin == list[0])
					{
						WeaponSkin = list[list.Count - 1];
					}
					else
					{
						WeaponSkin = list[j - 1];
					}
					break;
				}
			}
		}
		else
		{
			WeaponSkin--;
			if (WeaponSkin < 0)
			{
				WeaponSkin = WeaponStoreData.Skins.Count - 1;
			}
		}
		UpdateWeaponSkin();
	}

	public void SelectSkin()
	{
		AccountManager.SetWeaponSkinSelected(WeaponData.WeaponID, WeaponStoreData.Skins[WeaponSkin].SkinID);
		UpdateWeaponSkin();
	}

	public void BuySkin()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		if ((int)WeaponStoreData.Skins[WeaponSkin].Price > AccountManager.GetGold())
		{
			InAppPanel.SetActive(true);
			UIToast.Show(Localization.Get("Not enough money"));
			return;
		}
		AccountManager.SetGold(AccountManager.GetGold() - (int)WeaponStoreData.Skins[WeaponSkin].Price);
		AccountManager.SetWeaponSkin(WeaponData.WeaponID, WeaponStoreData.Skins[WeaponSkin].SkinID);
		AccountManager.SetWeaponSkinSelected(WeaponData.WeaponID, WeaponStoreData.Skins[WeaponSkin].SkinID);
		UpdateWeaponSkin();
		EventManager.Dispatch("AccountUpdate");
		Analytics.gua.sendEventHit("Store", "Buy Skin", string.Concat(WeaponData.WeaponName, " | ", WeaponStoreData.Skins[WeaponSkin].SkinName));
	}

	private Color GetAverageColor(Texture2D texture)
	{
		Color32[] pixels = texture.GetPixels32();
		int num = 0;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < pixels.Length; i++)
		{
			if (pixels[i].r != 0 && pixels[i].g != 0 && pixels[i].b != 0 && pixels[i].a != 0)
			{
				num2 += (float)(int)pixels[i].r;
				num3 += (float)(int)pixels[i].g;
				num4 += (float)(int)pixels[i].b;
				num++;
			}
		}
		Color32 color = new Color32((byte)(num2 / (float)num), (byte)(num3 / (float)num), (byte)(num4 / (float)num), byte.MaxValue);
		if ((float)(int)color.r < LimitSkinBackgroundColor)
		{
			color.r *= 2;
		}
		if ((float)(int)color.g < LimitSkinBackgroundColor)
		{
			color.g *= 2;
		}
		if ((float)(int)color.b < LimitSkinBackgroundColor)
		{
			color.b *= 2;
		}
		return color;
	}

	private void GetWeaponList()
	{
		WeaponsList.Clear();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].Weapon != WeaponType)
			{
				continue;
			}
			int num = GameSettings.instance.Weapons[i].WeaponID;
			if (num == 4 || num == 3 || num == 12)
			{
				WeaponsList.Insert(0, GameSettings.instance.Weapons[i].WeaponName);
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
						WeaponsList.Add(GameSettings.instance.Weapons[i].WeaponName);
					}
				}
				else
				{
					WeaponsList.Add(GameSettings.instance.Weapons[i].WeaponName);
				}
			}
		}
	}

	private void GetWeaponData()
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].WeaponName == (ObscuredString)WeaponsList[Weapon])
			{
				WeaponData = GameSettings.instance.Weapons[i];
				break;
			}
		}
	}

	private void GetWeaponStoreData()
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].WeaponName == (ObscuredString)WeaponsList[Weapon])
			{
				WeaponStoreData = GameSettings.instance.WeaponsShop[i];
				break;
			}
		}
	}

	private void SetDamage()
	{
		int num = Mathf.FloorToInt(((int)WeaponData.FaceDamage * (int)WeaponData.FireBullets + (int)WeaponData.BodyDamage * (int)WeaponData.FireBullets + (int)WeaponData.HandDamage * (int)WeaponData.FireBullets + (int)WeaponData.LegDamage * (int)WeaponData.FireBullets) / 4);
		int num2 = 0;
		if (WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[WeaponUpgrade - 1];
			num2 = Mathf.FloorToInt(((int)weaponUpgrade.FaceDamage * (int)WeaponData.FireBullets + (int)weaponUpgrade.BodyDamage * (int)WeaponData.FireBullets + (int)weaponUpgrade.HandDamage * (int)WeaponData.FireBullets + (int)weaponUpgrade.LegDamage * (int)WeaponData.FireBullets) / 4);
			num2 -= num;
			if (num2 != 0)
			{
				DamageLabel.text = num + "[00Ff22] + " + num2;
				DamageProgressBar.value = (float)(num + num2) / (float)MaxDamage;
			}
			else
			{
				DamageLabel.text = num.ToString();
				DamageProgressBar.value = (float)num / (float)MaxDamage;
			}
		}
		else if ((bool)WeaponData.WeaponFire)
		{
			DamageLabel.text = num.ToString();
			DamageProgressBar.value = (float)num / (float)MaxDamage;
		}
		else
		{
			DamageLabel.text = "-";
			DamageProgressBar.value = 0f;
		}
	}

	private void SetFireRate()
	{
		if (WeaponData.Weapon == WeaponTypeList.Knife || !WeaponData.WeaponFire)
		{
			FireRateLabel.text = "-";
			FireRateProgressBar.value = 0f;
			return;
		}
		int num = Mathf.FloorToInt(100f - (float)WeaponData.FireRate * 100f / 1.5f);
		int num2 = 0;
		if (WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[WeaponUpgrade - 1];
			num2 = Mathf.FloorToInt(100f - (float)weaponUpgrade.FireRate * 100f / 1.5f);
			num2 -= num;
			if (num2 != 0)
			{
				FireRateLabel.text = num + "[00Ff22] + " + num2;
				FireRateProgressBar.value = (float)(num + num2) / (float)MaxFireRate;
			}
			else
			{
				FireRateLabel.text = num.ToString();
				FireRateProgressBar.value = (float)num / (float)MaxFireRate;
			}
		}
		else
		{
			FireRateLabel.text = num.ToString();
			FireRateProgressBar.value = (float)num / (float)MaxFireRate;
		}
	}

	private void SetAccuracy()
	{
		if (WeaponData.Weapon == WeaponTypeList.Knife || !WeaponData.WeaponFire)
		{
			AccuracyLabel.text = "-";
			AccuracyProgressBar.value = 0f;
			return;
		}
		int num = Mathf.FloorToInt(100f - (float)WeaponData.FireAccuracy - (float)WeaponData.Accuracy);
		int num2 = 0;
		if (WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[WeaponUpgrade - 1];
			num2 = Mathf.FloorToInt(100f - (float)weaponUpgrade.FireAccuracy - (float)weaponUpgrade.Accuracy);
			num2 -= num;
			if (num2 != 0)
			{
				AccuracyLabel.text = num + "[00Ff22] + " + num2;
				AccuracyProgressBar.value = (float)(num + num2) / (float)MaxAccuracy;
			}
			else
			{
				AccuracyLabel.text = num.ToString();
				AccuracyProgressBar.value = (float)num / (float)MaxAccuracy;
			}
		}
		else
		{
			AccuracyLabel.text = num.ToString();
			AccuracyProgressBar.value = (float)num / (float)MaxAccuracy;
		}
	}

	private void SetAmmo()
	{
		if (WeaponData.Weapon == WeaponTypeList.Knife || !WeaponData.WeaponFire)
		{
			AmmoLabel.text = "-";
			AmmoProgressBar.value = 0f;
			return;
		}
		int num = WeaponData.Ammo;
		int num2 = 0;
		if (WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[WeaponUpgrade - 1];
			num2 = weaponUpgrade.Ammo;
			num2 -= num;
			if (num2 != 0)
			{
				AmmoLabel.text = num + "[00Ff22] + " + num2;
				AmmoProgressBar.value = (float)(num + num2) / (float)MaxAmmo;
			}
			else
			{
				AmmoLabel.text = num.ToString();
				AmmoProgressBar.value = (float)num / (float)MaxAmmo;
			}
		}
		else
		{
			AmmoLabel.text = num.ToString();
			AmmoProgressBar.value = (float)num / (float)MaxAmmo;
		}
	}

	private void SetMaxAmmo()
	{
		if (WeaponData.Weapon == WeaponTypeList.Knife || !WeaponData.WeaponFire)
		{
			MaxAmmoLabel.text = "-";
			MaxAmmoProgressBar.value = 0f;
			return;
		}
		int num = WeaponData.MaxAmmo;
		int num2 = 0;
		if (WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[WeaponUpgrade - 1];
			num2 = weaponUpgrade.MaxAmmo;
			num2 -= num;
			if (num2 != 0)
			{
				MaxAmmoLabel.text = num + "[00Ff22] + " + num2;
				MaxAmmoProgressBar.value = (float)(num + num2) / (float)MaxMaxAmmo;
			}
			else
			{
				MaxAmmoLabel.text = num.ToString();
				MaxAmmoProgressBar.value = (float)num / (float)MaxMaxAmmo;
			}
		}
		else
		{
			MaxAmmoLabel.text = num.ToString();
			MaxAmmoProgressBar.value = (float)num / (float)MaxMaxAmmo;
		}
	}

	private void SetMobility()
	{
		int num = Mathf.FloorToInt(100f - (float)WeaponData.Mass * 1000f);
		int num2 = 0;
		if (WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[WeaponUpgrade - 1];
			num2 = Mathf.FloorToInt(100f - (float)weaponUpgrade.Mass * 1000f);
			num2 -= num;
			if (num2 != 0)
			{
				MobilityLabel.text = num + "[00Ff22] + " + num2;
				MobilityProgressBar.value = (float)(num + num2) / (float)MaxMobility;
			}
			else
			{
				MobilityLabel.text = num.ToString();
				MobilityProgressBar.value = (float)num / (float)MaxMobility;
			}
		}
		else if (!WeaponData.WeaponFire)
		{
			MobilityLabel.text = "-";
		}
		else
		{
			MobilityLabel.text = num.ToString();
			MobilityProgressBar.value = (float)num / (float)MaxMobility;
		}
	}

	public void ShowSkinPanel()
	{
		if (!isSkinPanel)
		{
			mPanelManager.SetActivePlayerData(false);
			Vector3 localPosition = SkinNameLabel.cachedTransform.localPosition;
			localPosition.y = 214f;
			TweenPosition.Begin(SkinNameLabel.cachedGameObject, 0.7f, localPosition);
			mWeaponCamera.SetViewportRect(new Rect(0f, 0f, 1f, 1f), 1f);
			isSkinPanel = true;
			TweenAlpha.Begin(WeaponPanel, 0.5f, 0f);
			TweenAlpha.Begin(SkinPanel, 0.7f, 1f);
		}
	}

	public void ShowWeaponPanel()
	{
		mPanelManager.SetActivePlayerData(true);
		Vector3 localPosition = SkinNameLabel.cachedTransform.localPosition;
		localPosition.y = 150f;
		TweenPosition.Begin(SkinNameLabel.cachedGameObject, 0.7f, localPosition);
		mWeaponCamera.ResetRotateX(true);
		mWeaponCamera.SetViewportRect(new Rect(0.35f, 0.08f, 1f, 0.728f), 1f);
		isSkinPanel = false;
		TweenAlpha.Begin(SkinPanel, 0.5f, 0f);
		TweenAlpha.Begin(WeaponPanel, 0.7f, 1f);
		WeaponSkin = AccountManager.GetWeaponSkinSelected(WeaponData.WeaponID);
		UpdateWeaponSkin();
	}

	public void ShareScreenshot()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			SkinPanel.SetActive(false);
			string text = string.Concat(WeaponData.WeaponName, " | ", WeaponStoreData.Skins[WeaponSkin].SkinName, "_", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
			AndroidNativeFunctions.TakeScreenshot(text, "Block Strike", FinishScreenshot);
		}
	}

	private void FinishScreenshot(string path)
	{
		SkinPanel.SetActive(true);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("#BlockStrike #BS");
		stringBuilder.AppendLine(string.Concat("My weapon ", WeaponData.WeaponName, " | ", WeaponStoreData.Skins[WeaponSkin].SkinName, " in game Block Strike"));
		stringBuilder.AppendLine("http://bit.ly/blockstrike");
		AndroidNativeFunctions.ShareScreenshot(stringBuilder.ToString(), "Block Strike", Localization.Get("Share"), path);
		Analytics.gua.sendSocialHit("Share", "Weapon", string.Concat(WeaponData.WeaponName, " | ", WeaponStoreData.Skins[WeaponSkin].SkinName));
	}
}
