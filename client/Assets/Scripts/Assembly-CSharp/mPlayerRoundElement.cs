using FreeJSON;
using UnityEngine;

public class mPlayerRoundElement : MonoBehaviour
{
	public UILabel TitleLabel;

	public UILabel TextLabel;

	private JsonObject Data;

	private int Index;

	private mPlayerRoundManager Manager;

	public void SetData(string data, int index, mPlayerRoundManager manager)
	{
		Data = JsonObject.Parse(data);
		Manager = manager;
		Index = index;
		TitleLabel.text = Localization.Get("Reward") + " " + index.ToString("D2");
		TextLabel.text = Localization.Get("Money") + ": +" + Data.Get<string>("m") + "\nXP: +" + Data.Get<string>("x") + "\n" + Localization.Get("Kills") + ": +" + Data.Get<string>("k") + "\n" + Localization.Get("HeadshotKills") + ": +" + Data.Get<string>("h") + "\n" + Localization.Get("Deaths") + ": +" + Data.Get<string>("d");
	}

	private void OnClick()
	{
		if (!(Manager == null))
		{
			AdsManager.ShowRewardedVideo(Complete, Failed, Failed);
		}
	}

	private void Complete()
	{
		Manager.SaveData(Data, Index - 1);
	}

	private void Failed()
	{
		UIToast.Show(Localization.Get("Cancel"));
	}
}
