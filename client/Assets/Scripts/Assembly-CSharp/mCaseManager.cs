using System;
using System.Collections.Generic;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class mCaseManager : MonoBehaviour
{
	[Serializable]
	public class Case
	{
		public string Name;

		public ObscuredBool PriceGold;

		public ObscuredInt Price;

		public UILabel NameLabel;

		public UILabel InfoLabel;

		public ObscuredBool Money;

		public ObscuredInt Normal;

		public ObscuredInt Base;

		public ObscuredInt Professional;

		public ObscuredInt Legendary;

		public ObscuredInt FireStat;

		public ObscuredInt SecretWeapon;
	}

	[Header("Cases")]
	public Case[] Cases;

	[Header("Case Wheel")]
	private string SelectCaseName;

	public ObscuredBool CaseRotate;

	private float StartRotate;

	public AnimationCurve Curve;

	public ObscuredFloat Duration = 10f;

	public ObscuredFloat Lerp = 0.05f;

	public Vector2 FinishInterval;

	private float FinishPosition;

	public AudioSource SoundSource;

	public float SoundInterval;

	private float LastSoundInterval;

	public mCaseItem[] CaseItems;

	public mCaseItem FinishItem;

	public Transform CaseItemsRoot;

	[Header("Finish Panel")]
	public UIPanel FinishPanel;

	public UISprite FinishBackground;

	public UITexture FinishEffect1;

	public UITexture FinishEffect2;

	public UILabel FinishLabel;

	public UITexture FinishWeaponTexture;

	public GameObject FinishAlreadyAvailable;

	public UITexture FinishAlreadyAvailableTexture;

	public UILabel FinishAlreadyAvailableLabel;

	public UITexture FinishFireStatEffect;

	public UITexture FinishSecretWeaponEffect;

	private ObscuredBool isFinishWeapon;

	private ObscuredBool isFinishGold;

	private WeaponType FinishWeapon;

	private WeaponShopData.WeaponSkin FinishWeaponSkin;

	private List<KeyValuePair<int, int>> NormalSkins = new List<KeyValuePair<int, int>>();

	private List<KeyValuePair<int, int>> BaseSkins = new List<KeyValuePair<int, int>>();

	private List<KeyValuePair<int, int>> ProfessionalSkins = new List<KeyValuePair<int, int>>();

	private List<KeyValuePair<int, int>> LegendarySkins = new List<KeyValuePair<int, int>>();

	private List<KeyValuePair<int, int>> SecretWeaponSkins = new List<KeyValuePair<int, int>>();

	[Header("Money & Gold Data")]
	public Texture2D MoneyTexture;

	public Texture2D GoldTexture;

	[Header("Others")]
	public GameObject InAppPanel;

	public GameObject ShareButton;

	public GameObject BackButton;

	private void Update()
	{
		UpdateCaseWheel();
	}

	public void Show()
	{
		for (int i = 0; i < Cases.Length; i++)
		{
			Cases[i].NameLabel.text = Localization.Get(Cases[i].Name);
			Cases[i].InfoLabel.text = GetCaseInfo(Cases[i]);
		}
		UpdateSkinsList();
	}

	private string GetCaseInfo(Case selectCase)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Localization.Get("ChanceOfDrop") + ":");
		stringBuilder.AppendLine(string.Empty);
		if ((bool)selectCase.Money)
		{
			stringBuilder.AppendLine(Localization.Get("Money") + " & " + Localization.Get("Gold") + " -  50%");
		}
		if ((int)selectCase.Normal != 0)
		{
			stringBuilder.AppendLine(string.Concat(Localization.Get("Normal quality"), " - ", selectCase.Normal, "%"));
		}
		if ((int)selectCase.Base != 0)
		{
			stringBuilder.AppendLine(string.Concat("[00aff0]", Localization.Get("Basic quality"), "[-] - ", selectCase.Base, "%"));
		}
		if ((int)selectCase.Professional != 0)
		{
			stringBuilder.AppendLine(string.Concat("[ff0000]", Localization.Get("Professional quality"), "[-] - ", selectCase.Professional, "%"));
		}
		if ((int)selectCase.Legendary != 0)
		{
			stringBuilder.AppendLine(string.Concat("[E00061]", Localization.Get("Legendary quality"), "[-] - ", selectCase.Legendary, "%"));
		}
		if ((int)selectCase.SecretWeapon != 0)
		{
			stringBuilder.AppendLine(string.Concat("[757575]", Localization.Get("Secret Weapon"), "[-] - ", selectCase.SecretWeapon, "%"));
		}
		return stringBuilder.ToString();
	}

	private Case GetCase(string caseName)
	{
		for (int i = 0; i < Cases.Length; i++)
		{
			if (Cases[i].Name == caseName)
			{
				return Cases[i];
			}
		}
		return null;
	}

	private void UpdateSkinsList()
	{
		if (NormalSkins.Count != 0)
		{
			return;
		}
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((bool)GameSettings.instance.Weapons[i].WeaponLock || (bool)GameSettings.instance.Weapons[i].WeaponSecret)
			{
				continue;
			}
			for (int j = 0; j < GameSettings.instance.WeaponsShop[i].Skins.Count; j++)
			{
				if ((int)GameSettings.instance.WeaponsShop[i].Skins[j].Price == 0)
				{
					switch (GameSettings.instance.WeaponsShop[i].Skins[j].Rarity)
					{
					case 1:
						NormalSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].WeaponID, GameSettings.instance.WeaponsShop[i].Skins[j].SkinID));
						break;
					case 2:
						BaseSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].WeaponID, GameSettings.instance.WeaponsShop[i].Skins[j].SkinID));
						break;
					case 3:
						ProfessionalSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].WeaponID, GameSettings.instance.WeaponsShop[i].Skins[j].SkinID));
						break;
					case 4:
						LegendarySkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].WeaponID, GameSettings.instance.WeaponsShop[i].Skins[j].SkinID));
						break;
					}
				}
			}
		}
		for (int k = 0; k < GameSettings.instance.Weapons.Count; k++)
		{
			if ((bool)GameSettings.instance.Weapons[k].WeaponLock || !GameSettings.instance.Weapons[k].WeaponSecret)
			{
				continue;
			}
			for (int l = 0; l < GameSettings.instance.WeaponsShop[k].Skins.Count; l++)
			{
				if ((int)GameSettings.instance.WeaponsShop[k].Skins[l].Rarity != 0 && (int)GameSettings.instance.WeaponsShop[k].Skins[l].Price == 0)
				{
					SecretWeaponSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[k].WeaponID, GameSettings.instance.WeaponsShop[k].Skins[l].SkinID));
				}
			}
		}
	}

	public void StartCaseWheel(string caseName)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		SelectCaseName = caseName;
		Case obj = GetCase(caseName);
		if ((int)obj.Price != 0)
		{
			if ((bool)obj.PriceGold)
			{
				if ((int)obj.Price > AccountManager.GetGold())
				{
					InAppPanel.SetActive(true);
					UIToast.Show(Localization.Get("Not enough money"));
					return;
				}
				AccountManager.SetGold(AccountManager.GetGold() - (int)obj.Price, true);
			}
			else
			{
				if ((int)obj.Price > AccountManager.GetMoney())
				{
					InAppPanel.SetActive(true);
					UIToast.Show(Localization.Get("Not enough money"));
					return;
				}
				AccountManager.SetMoney1(-(int)obj.Price, true);
			}
		}
		TweenAlpha component = FinishPanel.GetComponent<TweenAlpha>();
		if (component != null)
		{
			component.enabled = false;
		}
		FinishPanel.alpha = 0f;
		mPanelManager.ShowPanel("CaseWheel", false);
		CaseItemsRoot.localPosition = new Vector3(0f, CaseItemsRoot.localPosition.y, 0f);
		StartRotate = Time.time;
		CaseRotate = true;
		LastSoundInterval = SoundInterval / 2f;
		FinishPosition = (int)UnityEngine.Random.Range(FinishInterval.x, FinishInterval.y);
		bool flag = obj.Money;
		bool finishMoneyData = false;
		UpdateOthersCaseItems();
		if (flag)
		{
			if (UnityEngine.Random.value > 0.25f)
			{
				if (UnityEngine.Random.value > 0.8f)
				{
					finishMoneyData = true;
				}
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			SetFinishMoneyData(finishMoneyData);
		}
		else
		{
			SetFinishWeaponData(obj);
		}
	}

	private void UpdateOthersCaseItems()
	{
		Case obj = GetCase(SelectCaseName);
		bool flag = obj.Money;
		bool flag2 = false;
		for (int i = 0; i < CaseItems.Length; i++)
		{
			flag = obj.Money;
			flag2 = false;
			if (flag)
			{
				if (UnityEngine.Random.value > 0.4f)
				{
					if (UnityEngine.Random.value > 0.6f)
					{
						flag2 = true;
					}
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				SetCaseMoneyItem(CaseItems[i], flag2);
				continue;
			}
			int num = UnityEngine.Random.Range(0, 100);
			KeyValuePair<int, int> keyValuePair = default(KeyValuePair<int, int>);
			keyValuePair = ((num < 25) ? (((int)obj.Normal == 0) ? BaseSkins[UnityEngine.Random.Range(0, BaseSkins.Count)] : NormalSkins[UnityEngine.Random.Range(0, NormalSkins.Count)]) : ((num < 50) ? BaseSkins[UnityEngine.Random.Range(0, BaseSkins.Count)] : ((num < 75) ? ProfessionalSkins[UnityEngine.Random.Range(0, ProfessionalSkins.Count)] : ((i <= 10 || (int)obj.SecretWeapon == 0 || num <= 98) ? LegendarySkins[UnityEngine.Random.Range(0, LegendarySkins.Count)] : SecretWeaponSkins[UnityEngine.Random.Range(0, SecretWeaponSkins.Count)]))));
			SetCaseWeaponItem(CaseItems[i], keyValuePair.Key, keyValuePair.Value);
		}
	}

	private void UpdateCaseWheel()
	{
		if ((bool)CaseRotate)
		{
			float time = (Time.time - StartRotate) / (float)Duration;
			float num = Curve.Evaluate(time);
			float x = num * FinishPosition;
			float num2 = 1f - num + (float)Lerp;
			CaseItemsRoot.localPosition = Vector3.Lerp(CaseItemsRoot.localPosition, new Vector3(x, CaseItemsRoot.localPosition.y, 0f), Time.deltaTime * num2);
			if (0f - CaseItemsRoot.localPosition.x > LastSoundInterval)
			{
				SoundSource.PlayOneShot(SoundSource.clip);
				LastSoundInterval += SoundInterval;
			}
			if (CaseItemsRoot.localPosition.x <= FinishPosition + 2f)
			{
				CaseRotate = false;
				StartFinishPanel();
			}
		}
	}

	private void SetFinishWeaponData(Case selectCase)
	{
		isFinishWeapon = true;
		int randomRarity = GetRandomRarity(selectCase);
		bool flag = (int)selectCase.SecretWeapon >= UnityEngine.Random.Range(1, 100);
		KeyValuePair<int, int> keyValuePair = default(KeyValuePair<int, int>);
		if (flag)
		{
			keyValuePair = SecretWeaponSkins[UnityEngine.Random.Range(0, SecretWeaponSkins.Count)];
		}
		else
		{
			switch (randomRarity)
			{
			case 1:
				keyValuePair = NormalSkins[UnityEngine.Random.Range(0, NormalSkins.Count)];
				break;
			case 2:
				keyValuePair = BaseSkins[UnityEngine.Random.Range(0, BaseSkins.Count)];
				break;
			case 3:
				keyValuePair = ProfessionalSkins[UnityEngine.Random.Range(0, ProfessionalSkins.Count)];
				break;
			case 4:
				keyValuePair = LegendarySkins[UnityEngine.Random.Range(0, LegendarySkins.Count)];
				break;
			}
		}
		FinishWeapon = WeaponManager.GetWeapon(keyValuePair.Key);
		FinishWeaponSkin = WeaponManager.GetWeaponSkin(keyValuePair.Key, keyValuePair.Value);
		SetCaseWeaponItem(FinishItem, keyValuePair.Key, keyValuePair.Value);
	}

	private int GetRandomRarity(Case selectCase)
	{
		int num = UnityEngine.Random.Range(0, 100);
		int num2 = ((!selectCase.Money) ? 1 : 2);
		if (((int)selectCase.Normal + (int)selectCase.Base + (int)selectCase.Professional) * num2 < num)
		{
			return 4;
		}
		if (((int)selectCase.Normal + (int)selectCase.Base) * num2 < num)
		{
			return 3;
		}
		if ((int)selectCase.Normal * num2 < num)
		{
			return 2;
		}
		return 1;
	}

	private void SetFinishMoneyData(bool isGold)
	{
		isFinishWeapon = false;
		isFinishGold = isGold;
		SetCaseMoneyItem(FinishItem, isGold);
	}

	private void SetCaseWeaponItem(mCaseItem caseItem, int weaponID, int skinID)
	{
		WeaponShopData.WeaponSkin weaponSkin = WeaponManager.GetWeaponSkin(weaponID, skinID);
		caseItem.ItemTexture.mainTexture = weaponSkin.Source;
		caseItem.ItemTexture.width = (int)GameSettings.instance.CaseSize[weaponID - 1].x;
		caseItem.ItemTexture.height = (int)GameSettings.instance.CaseSize[weaponID - 1].y;
		caseItem.ItemTexture.uvRect = GameSettings.instance.CaseUVRect[weaponID - 1];
		caseItem.ItemLabel.text = weaponSkin.SkinName;
		caseItem.ItemLabelSprite.color = GetRarityColor(weaponSkin.Rarity);
	}

	private void SetCaseMoneyItem(mCaseItem caseItem, bool gold)
	{
		caseItem.ItemTexture.mainTexture = ((!gold) ? MoneyTexture : GoldTexture);
		caseItem.ItemTexture.width = 80;
		caseItem.ItemTexture.height = 80;
		caseItem.ItemTexture.uvRect = new Rect(0f, 0f, 1f, 1f);
		caseItem.ItemLabel.text = Localization.Get((!gold) ? "Money" : "Gold");
		caseItem.ItemLabelSprite.color = GetRarityColor(1);
	}

	private Color GetRarityColor(int rarity)
	{
		switch (rarity)
		{
		case 0:
		case 1:
			return new Color(0.63f, 0.63f, 0.63f, 1f);
		case 2:
			return new Color(0.07f, 0.65f, 0.87f, 1f);
		case 3:
			return new Color(0.9f, 0f, 0f, 1f);
		case 4:
			return new Color(0.87f, 0f, 0.38f, 1f);
		default:
			return new Color(0.63f, 0.63f, 0.63f, 1f);
		}
	}

	public void StartFinishPanel()
	{
		TweenAlpha.Begin(FinishPanel.cachedGameObject, 0.5f, 1f);
		ShareButton.SetActive(isFinishWeapon);
		AccountManager.SetOpenCase1();
		if ((bool)isFinishWeapon)
		{
			Color rarityColor = GetRarityColor(FinishWeaponSkin.Rarity);
			bool flag = false;
			if ((int)FinishWeaponSkin.Rarity > 1 && FinishWeapon.Weapon != WeaponTypeList.Knife)
			{
				if ((int)GetCase(SelectCaseName).FireStat >= UnityEngine.Random.Range(1, 100))
				{
					flag = true;
					AccountManager.SetFireStat(FinishWeapon.WeaponID, FinishWeaponSkin.SkinID);
					FinishFireStatEffect.cachedTransform.parent.gameObject.SetActive(true);
					FinishFireStatEffect.color = rarityColor;
				}
				else
				{
					FinishFireStatEffect.cachedTransform.parent.gameObject.SetActive(false);
				}
			}
			else
			{
				FinishFireStatEffect.cachedTransform.parent.gameObject.SetActive(false);
			}
			if ((bool)FinishWeapon.WeaponSecret)
			{
				AccountManager.SetWeapon(FinishWeapon.WeaponID);
				FinishSecretWeaponEffect.cachedTransform.parent.gameObject.SetActive(true);
				FinishSecretWeaponEffect.color = rarityColor;
			}
			else
			{
				FinishSecretWeaponEffect.cachedTransform.parent.gameObject.SetActive(false);
			}
			FinishBackground.color = rarityColor;
			FinishBackground.alpha = 0.6f;
			FinishEffect1.color = rarityColor;
			FinishEffect1.alpha = 0.7f;
			FinishEffect2.color = rarityColor;
			FinishLabel.text = string.Concat(FinishWeapon.WeaponName, " | ", FinishWeaponSkin.SkinName);
			FinishWeaponTexture.mainTexture = FinishWeaponSkin.Source;
			FinishWeaponTexture.width = (int)GameSettings.instance.CaseSize[(int)FinishWeapon.WeaponID - 1].x * 2;
			FinishWeaponTexture.height = (int)GameSettings.instance.CaseSize[(int)FinishWeapon.WeaponID - 1].y * 2;
			FinishWeaponTexture.uvRect = GameSettings.instance.CaseUVRect[(int)FinishWeapon.WeaponID - 1];
			if (flag)
			{
				bool fireStat = AccountManager.GetFireStat(FinishWeapon.WeaponID, FinishWeaponSkin.SkinID);
				FinishAlreadyAvailable.SetActive(fireStat);
				if (fireStat)
				{
					SetRarityMoney(FinishWeaponSkin.Rarity, true, false, GetCase(SelectCaseName).Money);
				}
			}
			else
			{
				bool weaponSkin = AccountManager.GetWeaponSkin(FinishWeapon.WeaponID, FinishWeaponSkin.SkinID);
				FinishAlreadyAvailable.SetActive(weaponSkin);
				if (weaponSkin)
				{
					SetRarityMoney(FinishWeaponSkin.Rarity, false, FinishWeapon.WeaponSecret, GetCase(SelectCaseName).Money);
				}
			}
			AccountManager.SetWeaponSkin(FinishWeapon.WeaponID, FinishWeaponSkin.SkinID);
		}
		else
		{
			Color rarityColor2 = GetRarityColor(1);
			FinishBackground.color = rarityColor2;
			FinishBackground.alpha = 0.6f;
			FinishEffect1.color = rarityColor2;
			FinishEffect1.alpha = 0.7f;
			FinishEffect2.color = rarityColor2;
			FinishLabel.text = Localization.Get((!isFinishGold) ? "Money" : "Gold") + " | " + ((!isFinishGold) ? "+55" : "+2");
			FinishWeaponTexture.mainTexture = ((!isFinishGold) ? MoneyTexture : GoldTexture);
			FinishWeaponTexture.width = 100;
			FinishWeaponTexture.height = 100;
			FinishWeaponTexture.uvRect = new Rect(0f, 0f, 1f, 1f);
			if ((bool)isFinishGold)
			{
				AccountManager.SetGold1(2);
			}
			else
			{
				AccountManager.SetMoney1(55);
			}
			FinishAlreadyAvailable.SetActive(false);
			FinishFireStatEffect.cachedTransform.parent.gameObject.SetActive(false);
			FinishSecretWeaponEffect.cachedTransform.parent.gameObject.SetActive(false);
		}
		EventManager.Dispatch("AccountUpdate");
		AccountManager.UpdateDefaultData(null, null);
		AccountManager.UpdateWeaponsData(null, null);
	}

	private void SetRarityMoney(int rarity, bool firestat, bool secret, bool moneyCase)
	{
		int num = 0;
		int gold = 0;
		switch (rarity)
		{
		case 0:
		case 1:
			if (secret)
			{
				gold = 15;
			}
			else
			{
				num = 100;
			}
			break;
		case 2:
			if (secret)
			{
				gold = 20;
			}
			else if (firestat)
			{
				gold = 8;
			}
			else
			{
				num = 300;
			}
			break;
		case 3:
			gold = ((!secret) ? ((!firestat) ? 5 : 12) : 25);
			break;
		case 4:
			gold = ((!secret) ? ((!firestat) ? 8 : 16) : 30);
			break;
		}
		if (moneyCase)
		{
			FinishAlreadyAvailableTexture.cachedGameObject.SetActive(false);
			return;
		}
		FinishAlreadyAvailableTexture.cachedGameObject.SetActive(true);
		AccountManager.SetMoney1(num);
		AccountManager.SetGold1(gold);
		if (num > 0)
		{
			FinishAlreadyAvailableTexture.mainTexture = MoneyTexture;
			FinishAlreadyAvailableLabel.text = num.ToString();
		}
		else
		{
			FinishAlreadyAvailableTexture.mainTexture = GoldTexture;
			FinishAlreadyAvailableLabel.text = gold.ToString();
		}
	}

	public void StartRewardedCase()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		mPopUp.ShowText(Localization.Get("Please wait") + "...");
		vp_Timer.In(0.5f, () =>
		{
			AdsManager.ShowRewardedVideo(RewardedVideoComplete, RewardedVideoFailed, RewardedVideoAborted, "CaseVideo");
		});
	}

	private void RewardedVideoComplete()
	{
		vp_Timer.In(0.3f, () =>
		{
			mPopUp.HideAll("CaseWheel", false);
			StartCaseWheel("Free");
		});
	}

	private void RewardedVideoAborted()
	{
		vp_Timer.In(0.3f, () =>
		{
			UIToast.Show(Localization.Get("Cancel"));
			mPopUp.HideAll("Cases", true);
		});
	}

	private void RewardedVideoFailed()
	{
		UIToast.Show(Localization.Get("Video not available"));
		mPopUp.HideAll("Cases", true);
	}

	public void ShareScreenshot()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			BackButton.SetActive(false);
			ShareButton.SetActive(false);
			string text = string.Concat(FinishWeapon.WeaponName, " | ", FinishWeaponSkin.SkinName, "_", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
			AndroidNativeFunctions.TakeScreenshot(text, "Block Strike", FinishScreenshot);
		}
	}

	private void FinishScreenshot(string path)
	{
		BackButton.SetActive(true);
		ShareButton.SetActive(true);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("#BlockStrike #BS");
		stringBuilder.AppendLine(string.Concat("I just got ", FinishWeapon.WeaponName, " | ", FinishWeaponSkin.SkinName, " in game Block Strike"));
		stringBuilder.AppendLine("http://bit.ly/blockstrike");
		AndroidNativeFunctions.ShareScreenshot(stringBuilder.ToString(), "Block Strike", Localization.Get("Share"), path);
		Analytics.gua.sendSocialHit("Share", "Weapon Skin", string.Concat(FinishWeapon.WeaponName, " | ", FinishWeaponSkin.SkinName));
	}
}
