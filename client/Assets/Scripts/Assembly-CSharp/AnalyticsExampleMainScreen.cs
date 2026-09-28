using UnityEngine;

public class AnalyticsExampleMainScreen : MonoBehaviour
{
	private void OnGUI()
	{
		if (Analytics.Instance == null)
		{
			GUILayout.BeginVertical();
			GUILayout.Label(" ERROR! No Analytics object in scene!");
			GUILayout.Label(" Add Analytics script to an active game object.");
			GUILayout.EndVertical();
			return;
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("v");
		GUILayout.BeginVertical();
		GUILayout.Label("- Google Universal Analytics for Unity");
		GUILayout.Label(" Current scene: " + Application.loadedLevelName + "\n");
		GUILayout.Label("Scene switch sends automatic screen view events:");
		if (GUILayout.Button("Go to Secondary Scene\n(Opt-out example & more)"))
		{
			Application.LoadLevel("AnalyticsExampleSecondaryScene");
		}
		GUILayout.Label("Buttons to send imaginary screen switch events:");
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("\"Menuscreen A\""))
		{
			Analytics.changeScreen("AnalyticsExample - Menuscreen A");
		}
		if (GUILayout.Button("\"Menuscreen B\""))
		{
			Analytics.changeScreen("AnalyticsExample - Menuscreen B");
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("\nSocial hits and events - Links to Strobotnik:");
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Google+"))
		{
			Analytics.gua.sendSocialHit("GooglePlus", "plus", "StrobotnikGooglePlus");
			Application.OpenURL("http://plus.google.com/101873213646861422131");
		}
		if (GUILayout.Button("Facebook"))
		{
			Analytics.gua.sendSocialHit("Facebook", "like", "StrobotnikFacebook");
			Application.OpenURL("http://facebook.com/strobotnik");
		}
		if (GUILayout.Button("Twitter"))
		{
			Analytics.gua.sendSocialHit("Twitter", "follow", "StrobotnikTwitter");
			Application.OpenURL("http://twitter.com/strobotnik");
		}
		if (GUILayout.Button(" Web "))
		{
			Analytics.gua.sendEventHit("OpenWebsite", "Strobotnik.com");
			Application.OpenURL("http://strobotnik.com");
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("\nSend errors and exceptions to analytics:");
		if (Analytics.Instance.sendExceptions)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Log Error"))
			{
				Debug.LogError("Logged Error to analytics", this);
			}
			if (GUILayout.Button("Divide by zero"))
			{
				int num = 0;
				int num2 = 31337 / num;
				Debug.Log(string.Empty + num2);
			}
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.Label("(Analytics.sendExceptions is disabled)");
		}
		GUILayout.Label("---");
		GUILayout.Label("Remaining entries in offline hit cache:");
		if (Analytics.gua != null)
		{
			GUILayout.Label(Analytics.gua.remainingEntriesInOfflineCache.ToString());
		}
		if (GUILayout.Button("Quit"))
		{
			if (!Analytics.gua.analyticsDisabled)
			{
				Analytics.gua.beginHit(GoogleUniversalAnalytics.HitType.Screenview);
				Analytics.gua.addScreenName("AnalyticsExample - Quit");
				Analytics.gua.addSessionControl(false);
				Analytics.gua.sendHit();
			}
			base.gameObject.SetActive(false);
			Application.Quit();
		}
		GUILayout.Label("Verified internet access: " + Analytics.gua.internetReachable);
		string text = "Unity NetworkReachability: none";
		if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
		{
			text = "Unity NetworkReachability: via carrier data network";
		}
		else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			text = "Unity NetworkReachability: via local area network";
		}
		GUILayout.Label(text);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	private void Update()
	{
		float fixedTime = Time.fixedTime;
		Camera.main.backgroundColor = new Color(Mathf.Sin(fixedTime * 0.39f) * 0.2f + 0.25f, Mathf.Sin(fixedTime * 0.23f) * 0.2f + 0.25f, Mathf.Sin(fixedTime * 0.55f) * 0.2f + 0.25f);
	}
}
