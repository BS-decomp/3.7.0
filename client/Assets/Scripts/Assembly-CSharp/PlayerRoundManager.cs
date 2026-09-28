using Crypto;
using FreeJSON;

public class PlayerRoundManager
{
	private static CryptoInt XP = 0;

	private static CryptoInt Money = 0;

	private static CryptoInt Kills = 0;

	private static CryptoInt Headshot = 0;

	private static CryptoInt Deaths = 0;

	private static vp_Timer.Handle TimerData = new vp_Timer.Handle();

	public static void SetXP(int xp)
	{
		XP = (int)XP + xp;
	}

	public static void SetMoney(int money)
	{
		Money = (int)Money + money;
	}

	public static void SetKills1()
	{
		Kills = (int)Kills + 1;
	}

	public static void SetHeadshot1()
	{
		Headshot = (int)Headshot + 1;
	}

	public static void SetDeaths1()
	{
		Deaths = (int)Deaths + 1;
	}

	private static string GetPopupText()
	{
		if ((int)XP == 0 && (int)Money == 0 && (int)Kills == 0 && (int)Headshot == 0 && (int)Deaths == 0)
		{
			return string.Empty;
		}
		return string.Concat(Localization.Get("Money"), ": +", Money, "\nXP: +", XP, "\n", Localization.Get("Kills"), ": +", Kills, "\n", Localization.Get("HeadshotKills"), ": +", Headshot, "\n", Localization.Get("Deaths"), ": +", Deaths);
	}

	public static void Show()
	{
		if (PhotonNetwork.offlineMode)
		{
			return;
		}
		TimerData = new vp_Timer.Handle();
		vp_Timer.In(0.8f, () =>
		{
			if (AccountManager.GetInAppPurchase().Count > 0)
			{
				ShowPopup();
			}
			else
			{
				AdsManager.ShowRewardedVideo(ShowPopup, FailedAds, FailedAds, "ExitServer");
			}
		}, TimerData);
		TimerData.CancelOnLoad = false;
	}

	private static void ShowPopup()
	{
		vp_Timer.In(0.3f, () =>
		{
			string popupText = GetPopupText();
			if (!string.IsNullOrEmpty(popupText))
			{
				mPopUp.ShowPopup(popupText, Localization.Get("Game Information"), "Ok", ClosePopup);
				SetData();
			}
		});
	}

	private static void ClosePopup()
	{
		mPopUp.HideAll("Menu");
		EventManager.Dispatch("AccountUpdate");
	}

	private static void FailedAds()
	{
		UIToast.Show(Localization.Get("Cancel"));
		SaveData();
	}

	private static void SetData()
	{
		if ((int)XP != 0 || (int)Money != 0 || (int)Kills != 0 || (int)Headshot != 0 || (int)Deaths != 0)
		{
			AccountManager.SetMoney1(Money);
			AccountManager.SetXP1(XP);
			AccountManager.SetKills1(Kills);
			AccountManager.SetDeaths1(Deaths);
			AccountManager.SetHeadshot1(Headshot);
			ClearData();
			AccountManager.UpdateDefaultData(null, null);
			AccountManager.UpdateWeaponsData(null, null);
		}
	}

	private static void ClearData()
	{
		Money = 0;
		XP = 0;
		Kills = 0;
		Deaths = 0;
		Headshot = 0;
	}

	private static void SaveData()
	{
		JsonArray jsonArray = JsonArray.Parse(CryptoPrefs.GetString("PlayerDataRound", "[]"));
		JsonObject jsonObject = new JsonObject();
		jsonObject.Add("m", (int)Money);
		jsonObject.Add("x", (int)XP);
		jsonObject.Add("k", (int)Kills);
		jsonObject.Add("d", (int)Deaths);
		jsonObject.Add("h", (int)Headshot);
		jsonArray.Add(jsonObject);
		CryptoPrefs.SetString("PlayerDataRound", jsonArray.ToString());
		ClearData();
		EventManager.Dispatch("UpdatePlayerRoundData");
	}
}
