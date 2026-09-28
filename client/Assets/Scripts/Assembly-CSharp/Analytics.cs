using System;
using UnityEngine;

public class Analytics : MonoBehaviour
{
	private const string disableAnalyticsByUserOptOutPrefKey = "GoogleUniversalAnalytics_optOut";

	public string trackingID = "UA-XXXXXXX-Y";

	public bool useHTTPS;

	public bool useOfflineCache = true;

	public bool sendExceptions = true;

	public static GoogleUniversalAnalytics gua;

	private static Analytics instance;

	private string offlineCacheFileName = "GUA-offline-queue.dat";

	private string prevExceptionLogString = string.Empty;

	private string prevExceptionStrackTrace = string.Empty;

	public static Analytics Instance
	{
		get
		{
			return instance;
		}
	}

	private int getPOSIXTime()
	{
		return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	public static void setPlayerPref_disableAnalyticsByUserOptOut(bool analyticsDisabled)
	{
		if (gua != null)
		{
			gua.analyticsDisabled = analyticsDisabled;
		}
		PlayerPrefs.SetInt("GoogleUniversalAnalytics_optOut", analyticsDisabled ? 1 : 0);
		PlayerPrefs.Save();
	}

	private void Awake()
	{
		if ((bool)instance)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		string text = string.Empty;
		if (PlayerPrefs.HasKey("GoogleUniversalAnalytics_clientID"))
		{
			text = PlayerPrefs.GetString("GoogleUniversalAnalytics_clientID");
		}
		if (text.Length < 8 || !PlayerPrefs.HasKey("GoogleUniversalAnalytics_clientID"))
		{
			int num = SystemInfo.graphicsDeviceName.GetHashCode() ^ SystemInfo.graphicsDeviceVersion.GetHashCode() ^ SystemInfo.operatingSystem.GetHashCode() ^ SystemInfo.processorType.GetHashCode();
			text = getPOSIXTime().ToString("X8") + UnityEngine.Random.Range(0, int.MaxValue).ToString("x8") + num.ToString("X8");
			PlayerPrefs.SetString("GoogleUniversalAnalytics_clientID", text);
			PlayerPrefs.Save();
		}
		string offlineCacheFilePath = string.Empty;
		if (useOfflineCache && offlineCacheFileName != null && offlineCacheFileName.Length > 0)
		{
			offlineCacheFilePath = Application.persistentDataPath + '/' + offlineCacheFileName;
		}
		if (gua == null)
		{
			gua = GoogleUniversalAnalytics.Instance;
		}
		gua.initialize(this, trackingID, text, VersionManager.productName, VersionManager.bundleVersion, useHTTPS, offlineCacheFilePath);
		if (PlayerPrefs.HasKey("GoogleUniversalAnalytics_optOut"))
		{
			gua.analyticsDisabled = PlayerPrefs.GetInt("GoogleUniversalAnalytics_optOut", 0) != 0;
		}
		if (sendExceptions && Application.platform != RuntimePlatform.WindowsEditor)
		{
			ConsoleManager.LogCallback = (Action<string, string, LogType>)Delegate.Combine(ConsoleManager.LogCallback, new Action<string, string, LogType>(Callback_HandleLog));
		}
	}

	private void Callback_HandleLog(string logString, string stackTrace, LogType type)
	{
		if (type != LogType.Log && type != LogType.Warning && (type == LogType.Error || type == LogType.Exception || type == LogType.Assert))
		{
			bool isFatal = type != LogType.Error;
			gua.cancelHit();
			if (!prevExceptionLogString.Equals(logString) || !prevExceptionStrackTrace.Equals(stackTrace))
			{
				gua.sendExceptionHit("(" + type.ToString() + ")" + logString + ":\n" + stackTrace, isFatal);
			}
			prevExceptionLogString = logString ?? string.Empty;
			prevExceptionStrackTrace = stackTrace ?? string.Empty;
		}
	}

	private void OnDisable()
	{
		gua.closeOfflineCacheFile();
		if (sendExceptions)
		{
			ConsoleManager.LogCallback = (Action<string, string, LogType>)Delegate.Remove(ConsoleManager.LogCallback, new Action<string, string, LogType>(Callback_HandleLog));
		}
	}

	public static void changeScreen(string newScreenName)
	{
		gua.sendAppScreenHit(newScreenName);
	}

	public void sendSystemInfoEvent(string eventCategory, string eventAction, string eventLabel = null, int eventValue = -1)
	{
		if (!gua.analyticsDisabled)
		{
			gua.beginHit(GoogleUniversalAnalytics.HitType.Event);
			gua.addEventCategory(eventCategory);
			gua.addEventAction(eventAction);
			if (eventLabel != null)
			{
				gua.addEventLabel(eventLabel);
			}
			if (eventValue >= 0)
			{
				gua.addEventValue(eventValue);
			}
			gua.addNonInteractionHit();
			gua.sendHit();
		}
	}
}
