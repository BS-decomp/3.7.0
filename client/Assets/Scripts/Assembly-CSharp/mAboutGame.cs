using UnityEngine;

public class mAboutGame : MonoBehaviour
{
	public void Vkontakte()
	{
		Application.OpenURL("https://vk.com/rexetstudio");
		Analytics.gua.sendSocialHit("Vkontakte", "Click", "https://vk.com/rexetstudio");
	}

	public void Facebook()
	{
		Application.OpenURL("https://www.facebook.com/Block-Strike-1493507804286160/");
		Analytics.gua.sendSocialHit("Facebook", "Click", "https://www.facebook.com/Block-Strike-1493507804286160/");
	}

	public void Twitter()
	{
		Application.OpenURL("https://twitter.com/RexetStudio");
		Analytics.gua.sendSocialHit("Twitter", "Click", "https://twitter.com/RexetStudio");
	}

	public void Share()
	{
		AndroidNativeFunctions.ShareText(Localization.Get("ShareText") + "https://play.google.com/store/apps/details?id=com.rexetstudio.blockstrike", "Block Strike", Localization.Get("Share"));
		Analytics.gua.sendSocialHit("Share", "Text", "https://play.google.com/store/apps/details?id=com.rexetstudio.blockstrike");
	}
}
