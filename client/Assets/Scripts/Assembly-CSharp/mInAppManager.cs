using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class mInAppManager : MonoBehaviour
{
	private ObscuredInt RewardedVideoMoney = 50;

	private ObscuredInt RewardedVideoGold = 1;

	private MoneyType Money;

	private bool isRewardedVideo;

	public void OnRewardedVideo(int money)
	{
		if (!isRewardedVideo)
		{
			Money = (MoneyType)money;
			UIToast.Show(Localization.Get("Please wait") + "...");
			isRewardedVideo = true;
			vp_Timer.In(0.5f, () =>
			{
				AdsManager.ShowRewardedVideo(RewardedVideoComplete, RewardedVideoFailed, RewardedVideoAborted, (Money != MoneyType.Gold) ? "RewardedMoney" : "RewardedGold");
			});
		}
	}

	private void RewardedVideoComplete()
	{
		vp_Timer.In(0.3f, () =>
		{
			if (Money == MoneyType.Money)
			{
				AccountManager.SetMoney1(RewardedVideoMoney, true);
				UIToast.Show("+50 " + Localization.Get("Money"));
			}
			else
			{
				AccountManager.SetGold1(RewardedVideoGold, true);
				UIToast.Show("+1 " + Localization.Get("Gold"));
			}
			EventManager.Dispatch("AccountUpdate");
			isRewardedVideo = false;
		});
	}

	private void RewardedVideoAborted()
	{
		UIToast.Show(Localization.Get("Cancel"));
		isRewardedVideo = false;
	}

	private void RewardedVideoFailed()
	{
		isRewardedVideo = false;
		UIToast.Show(Localization.Get("Video not available"), 3f);
	}
}
