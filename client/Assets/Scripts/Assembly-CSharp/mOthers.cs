using UnityEngine;

public class mOthers : MonoBehaviour
{
	private void Start()
	{
		InputManager.Init();
		if (PlayerPrefs.HasKey("KickInfo"))
		{
			mPopUp.ShowText(PlayerPrefs.GetString("KickInfo"), 3f, "Menu");
			PlayerPrefs.DeleteKey("KickInfo");
		}
		AchievementsManager.UpdateMoney();
		WeaponManager.Init();
		UISelectWeapon.AllWeapons = false;
		UISelectWeapon.SelectedUpdateWeaponManager = false;
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void ShowOthersGames()
	{
		Application.OpenURL("https://play.google.com/store/apps/dev?id=6363329851677974248");
	}

	public void ComingSoon()
	{
		UIToast.Show(Localization.Get("Coming soon"));
	}
}
