using UnityEngine;

public class AnalyticsExampleSecondaryScreen : MonoBehaviour
{
	private void OnGUI()
	{
		if (Analytics.gua == null)
		{
			GUILayout.BeginVertical();
			GUILayout.Label("Error: No Analytics.gua object!\n");
			GUILayout.Label("AnalyticsExampleSecondaryScene works only when switched to from the main AnalyticsExample scene.");
			GUILayout.EndVertical();
			return;
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label(" ");
		GUILayout.BeginVertical();
		GUILayout.Label(" Current scene: " + Application.loadedLevelName);
		GUILayout.Label(" ");
		GUILayout.Label(" This scene demonstrates automatic screen switch\n events sent by the analytics example, and is an\n example of options screen allowing user to\n opt-out from analytics.");
		GUILayout.Label(" ");
		GUILayout.Label(" This app sends anonymous usage statistics over internet.");
		bool analyticsDisabled = Analytics.gua.analyticsDisabled;
		bool flag = GUILayout.Toggle(analyticsDisabled, "Opt-out from anonymous statistics.");
		if (analyticsDisabled != flag)
		{
			Analytics.setPlayerPref_disableAnalyticsByUserOptOut(flag);
		}
		GUILayout.Label((!analyticsDisabled) ? " \n" : " :-(\n");
		GUILayout.Label("\nMore from Strobotnik:");
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Pixel-Perfect\nDynamic Text\n(!!)"))
		{
			Analytics.gua.sendEventHit("OpenWebsite", "bitly.com/DynTextUnity");
			Application.OpenURL("http://bitly.com/DynTextUnity");
		}
		if (GUILayout.Button("Internet\nReachability\nVerifier"))
		{
			Analytics.gua.sendEventHit("OpenWebsite", "j.mp/IRVUNAS");
			Application.OpenURL("http://j.mp/IRVUNAS");
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("\n");
		if (GUILayout.Button("Back to Main"))
		{
			Application.LoadLevel("AnalyticsExample");
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
}
