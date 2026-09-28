using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using MovementEffects;
using UnityEngine;

public class AdsManager : MonoBehaviour, IRewardedVideoAdListener, ISkippableVideoAdListener
{
	[CompilerGenerated]
	private sealed class _003CCheckHostsFile_003Ec__Iterator17 : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal WWW _003Cwww_003E__0;

		internal int _0024PC;

		internal float _0024current;

		float IEnumerator<float>.Current
		{
			[DebuggerHidden]
			get
			{
				return System_002ECollections_002EGeneric_002EIEnumerator_003Cfloat_003E_002Eget_Current();
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		[DebuggerHidden]
		private float System_002ECollections_002EGeneric_002EIEnumerator_003Cfloat_003E_002Eget_Current()
		{
			return _0024current;
		}

		public bool MoveNext()
		{
			uint num = (uint)_0024PC;
			_0024PC = -1;
			switch (num)
			{
			case 0u:
				_003Cwww_003E__0 = new WWW("file:///etc/hosts");
				_0024current = Timing.WaitUntilDone(_003Cwww_003E__0);
				_0024PC = 1;
				return true;
			case 1u:
				if (_003Cwww_003E__0.error == string.Empty && _003Cwww_003E__0.size > 100)
				{
					isBlockAds = true;
				}
				_0024PC = -1;
				break;
			}
			return false;
		}

		[DebuggerHidden]
		public void Dispose()
		{
			_0024PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}
	}

	public static bool isBlockAds = false;

	private static Action RewardedVideoComplete;

	private static Action RewardedVideoFailed;

	private static Action RewardedVideoAborted;

	private static vp_Timer.Handle Timer = new vp_Timer.Handle();

	private static int FinishRewardedVideo = -1;

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Init();
	}

	public void Init()
	{
		Timing.RunCoroutine(CheckHostsFile());
		Appodeal.disableNetwork("avocarrot");
		Appodeal.disableNetwork("cheetah");
		Appodeal.disableNetwork("facebook");
		Appodeal.disableNetwork("flurry");
		Appodeal.disableNetwork("pubnative");
		Appodeal.disableNetwork("yandex");
		UserSettings userSettings = new UserSettings();
		userSettings.setInterests("games, action, coca-cola, pepsi, casino, poker, blackjack, match 3, race, sport, word, trivia, shooter, new year, merry christmas");
		Appodeal.confirm(2);
		Appodeal.setRewardedVideoCallbacks(this);
		Appodeal.setSkippableVideoCallbacks(this);
		Appodeal.initialize("fd12215587d2499bf80048b3ef940e61a2f341760ce6d1b4", 130);
	}

	[DebuggerHidden]
	private IEnumerator<float> CheckHostsFile()
	{
		//yield-return decompiler failed: Could not find currentField
		return new _003CCheckHostsFile_003Ec__Iterator17();
	}

	public static void ShowInterstitial()
	{
		ShowInterstitial("default");
	}

	public static void ShowInterstitial(string placement)
	{
	}

	public static void ShowRewardedVideo(Action complete, Action failed, Action aborted)
	{
		ShowRewardedVideo(complete, failed, aborted, "default");
	}

	public static void ShowRewardedVideo(Action complete, Action failed, Action aborted, string placement)
	{
		RewardedVideoComplete = complete;
		RewardedVideoFailed = failed;
		RewardedVideoAborted = aborted;
		FinishRewardedVideo = -1;
		if (Appodeal.isLoaded(128))
		{
			Appodeal.show(128, placement);
		}
		else if (Appodeal.isLoaded(2))
		{
			Appodeal.show(2, placement);
		}
		else
		{
			RewardedVideoFinished(2);
		}
	}

	private static void RewardedVideoFinished(int isComplete)
	{
		if (FinishRewardedVideo != 0)
		{
			FinishRewardedVideo = isComplete;
		}
		if (Timer.Active)
		{
			return;
		}
		vp_Timer.In(0.2f, () =>
		{
			if (isComplete == 0)
			{
				if (RewardedVideoComplete != null)
				{
					RewardedVideoComplete();
				}
			}
			else if (isComplete == 1)
			{
				if (RewardedVideoAborted != null)
				{
					RewardedVideoAborted();
				}
			}
			else if (RewardedVideoFailed != null)
			{
				RewardedVideoFailed();
			}
			RewardedVideoComplete = null;
			RewardedVideoFailed = null;
			RewardedVideoAborted = null;
		}, Timer);
	}

	public void onRewardedVideoLoaded()
	{
		if (Settings.Console)
		{
			vp_Timer.In(0.1f, () =>
			{
				MonoBehaviour.print("Rewarded Video loaded");
			});
		}
	}

	public void onRewardedVideoFailedToLoad()
	{
		RewardedVideoFinished(1);
		if (Settings.Console)
		{
			vp_Timer.In(0.1f, () =>
			{
				MonoBehaviour.print("Rewarded Video failed");
			});
		}
	}

	public void onRewardedVideoShown()
	{
		if (Settings.Console)
		{
			vp_Timer.In(0.1f, () =>
			{
				MonoBehaviour.print("Rewarded Video shown");
			});
		}
	}

	public void onRewardedVideoClosed()
	{
		if (Settings.Console)
		{
			vp_Timer.In(0.1f, () =>
			{
				MonoBehaviour.print("Rewarded Video closed");
			});
		}
	}

	public void onRewardedVideoFinished(int amount, string name)
	{
		RewardedVideoFinished(0);
		if (Settings.Console)
		{
			vp_Timer.In(0.1f, () =>
			{
				MonoBehaviour.print("Rewarded Video finished: Reward: " + amount + name);
			});
		}
	}

	public void onSkippableVideoLoaded()
	{
		if (Settings.Console)
		{
			vp_Timer.In(0.1f, () =>
			{
				MonoBehaviour.print("Video loaded");
			});
		}
	}

	public void onSkippableVideoFailedToLoad()
	{
		RewardedVideoFinished(2);
		if (Settings.Console)
		{
			vp_Timer.In(0.1f, () =>
			{
				MonoBehaviour.print("Video failed");
			});
		}
	}

	public void onSkippableVideoShown()
	{
		if (Settings.Console)
		{
			vp_Timer.In(0.1f, () =>
			{
				MonoBehaviour.print("Video shown");
			});
		}
	}

	public void onSkippableVideoFinished()
	{
		RewardedVideoFinished(0);
		if (Settings.Console)
		{
			vp_Timer.In(0.1f, () =>
			{
				MonoBehaviour.print("Video finished");
			});
		}
	}

	public void onSkippableVideoClosed()
	{
		RewardedVideoFinished(1);
		if (Settings.Console)
		{
			vp_Timer.In(0.1f, () =>
			{
				MonoBehaviour.print("Video closed");
			});
		}
	}
}
