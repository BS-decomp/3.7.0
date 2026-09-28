using UnityEngine;

public class mAccount : MonoBehaviour
{
	private void Start()
	{
		if ((bool)AccountManager.isConnect)
		{
			return;
		}
		string playerID = string.Empty;
		string[] emails = AndroidNativeFunctions.GetEmails();
		if (emails.Length != 0)
		{
			playerID = emails[0];
			playerID = playerID.Remove(playerID.LastIndexOf("@"));
			playerID = playerID.Replace(".", "*");
			mPopUp.SetActiveWait(true, Localization.Get("Connect to the account") + ": " + playerID);
			playerID = playerID.ToLower();
			vp_Timer.In(1f, () =>
			{
				AccountManager.Login(playerID, Login, LoginError);
			});
		}
		else
		{
			mPopUp.ShowPopup(Localization.Get("Error") + ": " + Localization.Get("Your device is not found a Google Account"), "Account", Localization.Get("Exit"), Exit, Localization.Get("Connect"), Start);
		}
	}

	private void Login(bool isCreated)
	{
		if (isCreated)
		{
			mPopUp.SetActiveWait(false);
			EventManager.Dispatch("AccountUpdate");
			WeaponManager.UpdateData();
			CheckClan();
			if (string.IsNullOrEmpty(AccountManager.AccountName))
			{
				SetPlayerName();
			}
		}
		else
		{
			mPopUp.SetActiveWait(false);
			mPopUp.ShowText(Localization.Get("Please wait") + "...");
			AccountManager.Register(RegisterComplete, RegisterError);
		}
	}

	private void LoginError(string error)
	{
		mPopUp.ShowPopup(Localization.Get("Error") + ": " + error, "Account", Localization.Get("Exit"), Exit, Localization.Get("Connect"), LoginErrorTry);
		mPopUp.SetActiveWait(false);
	}

	private void LoginErrorTry()
	{
		if (mPopUp.ActivePopup)
		{
			mPopUp.HideAll("Menu");
		}
		AccountManager.Login(Login, LoginError);
		mPopUp.SetActiveWait(true, Localization.Get("Connect to the account") + ": " + AccountManager.AccountID);
	}

	private void RegisterComplete()
	{
		SetPlayerName();
	}

	private void RegisterError(string error)
	{
		mPopUp.ShowPopup(Localization.Get("Error") + ": " + error, "Account", Localization.Get("Exit"), Exit, Localization.Get("Connect"), RegisterErrorTry);
	}

	private void RegisterErrorTry()
	{
		if (mPopUp.ActivePopup)
		{
			mPopUp.HideAll("Menu");
		}
		AccountManager.Register(RegisterComplete, RegisterError);
	}

	private void SetPlayerName()
	{
		mPopUp.SetActiveWait(false);
		mPopUp.ShowInput(string.Empty, Localization.Get("ChangeName"), 12, UIInput.KeyboardType.Default, SetPlayerNameSubmit, SetPlayerNameChange, Localization.Get("Back"), null, "Ok", SetPlayerNameSave);
	}

	private void SetPlayerNameSave()
	{
		string text = NGUIText.StripSymbols(mPopUp.GetInputText());
		string text2 = mChangeName.UpdateSymbols(text);
		if (text != text2)
		{
			mPopUp.SetInputText(text2);
		}
		else if (text.Length <= 3 || text == "Null")
		{
			text = "Player " + Random.Range(0, 99999);
			mPopUp.SetInputText(text);
		}
		else
		{
			AccountManager.UpdateName(text, SetPlayerNameComplete, SetPlayerNameError);
			mPopUp.ShowText(Localization.Get("Please wait") + "...");
		}
	}

	private void SetPlayerNameSubmit()
	{
		string text = mPopUp.GetInputText();
		if (text.Length <= 3 || text == "Null")
		{
			text = "Player " + Random.Range(0, 99999);
		}
		text = NGUIText.StripSymbols(text);
		mPopUp.SetInputText(text);
	}

	private void SetPlayerNameChange()
	{
		string inputText = mPopUp.GetInputText();
		string text = mChangeName.UpdateSymbols(inputText);
		if (inputText != text)
		{
			mPopUp.SetInputText(text);
		}
	}

	private void SetPlayerNameComplete(string playerName)
	{
		EventManager.Dispatch("AccountUpdate");
		WeaponManager.UpdateData();
		mPopUp.HideAll("Menu");
	}

	private void SetPlayerNameError(string error)
	{
		SetPlayerName();
		string text = error;
		if (text == "Name already taken")
		{
			text = Localization.Get("Name already taken");
		}
		UIToast.Show(Localization.Get("Error") + ": " + text, 3f);
	}

	private void CheckClan()
	{
		if (string.IsNullOrEmpty(AccountManager.GetClan()))
		{
			return;
		}
		ClansManager.GetData(null, (string error) =>
		{
			mPopUp.ShowPopup(Localization.Get("Error") + ": " + error, "Clan", "Ok", () =>
			{
				mPopUp.HideAll("Menu");
			});
		});
	}

	private void Exit()
	{
		Application.Quit();
	}
}
