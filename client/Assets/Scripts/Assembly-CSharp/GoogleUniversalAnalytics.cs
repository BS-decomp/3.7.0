using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public sealed class GoogleUniversalAnalytics
{
	public enum HitType
	{
		Pageview = 0,
		Screenview = 1,
		Event = 2,
		Transaction = 3,
		Item = 4,
		Social = 5,
		Exception = 6,
		Timing = 7,
		None = 8
	}

	public enum ProductAction
	{
		Detail = 0,
		Click = 1,
		Add = 2,
		Remove = 3,
		Checkout = 4,
		CheckoutOption = 5,
		Purchase = 6,
		Refund = 7
	}

	private enum NetAccessStatus
	{
		Offline = 0,
		PendingVerification = 1,
		Error = 2,
		Mismatch = 3,
		Functional = 4
	}

	private delegate string Delegate_escapeString(string s);

	private static readonly string[] hitTypeNames = new string[9] { "pageview", "screenview", "event", "transaction", "item", "social", "exception", "timing", "none" };

	private static readonly string[] productActionNames = new string[8] { "detail", "click", "add", "remove", "checkout", "checkout_option", "purchase", "refund" };

	private static readonly string httpCollectUrl = "http://www.google-analytics.com/collect";

	private static readonly string httpsCollectUrl = "https://ssl.google-analytics.com/collect";

	private static readonly string guaVersionData = "v=1";

	private string defaultHitData;

	private StringBuilder sb = new StringBuilder(256, 8192);

	private HitType hitType = HitType.None;

	private Delegate_escapeString escapeString = WWW.EscapeURL;

	private static readonly GoogleUniversalAnalytics instance = new GoogleUniversalAnalytics();

	public bool useHTTPS;

	public MonoBehaviour helperBehaviour;

	public string trackingID;

	public string clientID;

	public string userID;

	public string userIP;

	public string userAgent;

	public string appName;

	public string appVersion;

	public string appID;

	public bool anonymizeIP;

	public string userLanguage;

	private bool analyticsDisabled_;

	public bool useOfflineCache;

	private string offlineCacheFilePath;

	private StreamReader offlineCacheReader;

	private StreamWriter offlineCacheWriter;

	private int offlineQueueLength = -1;

	private int offlineQueueSentHits = -1;

	private NetAccessStatus netAccessStatus;

	private WaitForSeconds defaultReachabilityCheckPeriod = new WaitForSeconds(5f);

	private WaitForSeconds hitBeingBuiltRetryTime = new WaitForSeconds(0.25f);

	private WaitForSeconds netVerificationErrorRetryTime = new WaitForSeconds(20f);

	private WaitForSeconds netVerificationMismatchRetryTime = new WaitForSeconds(10f);

	private WaitForSeconds cachedHitSendThrottleTime = new WaitForSeconds(1f);

	public Dictionary<string, string> customHeaders = new Dictionary<string, string>();

	private DateTime? epoch;

	private static string offlineQueueLengthPrefKey = "GoogleUniversalAnalytics_offlineQueueLength";

	private static string offlineQueueSentHitsPrefKey = "GoogleUniversalAnalytics_offlineQueueSentHits";

	private static char[] tabRowSplitter = new char[1] { '\t' };

	public static GoogleUniversalAnalytics Instance
	{
		get
		{
			return instance;
		}
	}

	public bool analyticsDisabled
	{
		get
		{
			return analyticsDisabled_;
		}
		set
		{
			analyticsDisabled_ = value;
			if (analyticsDisabled_)
			{
				clearOfflineQueue();
			}
		}
	}

	public int remainingEntriesInOfflineCache
	{
		get
		{
			if (offlineQueueLength == -1)
			{
				offlineQueueLength = PlayerPrefs.GetInt(offlineQueueLengthPrefKey, 0);
			}
			if (offlineQueueSentHits == -1)
			{
				offlineQueueSentHits = PlayerPrefs.GetInt(offlineQueueSentHitsPrefKey, 0);
			}
			return offlineQueueLength - offlineQueueSentHits;
		}
	}

	private bool isHitBeingBuilt
	{
		get
		{
			return hitType != HitType.None || sb.Length > 0;
		}
	}

	public bool internetReachable
	{
		get
		{
			if (useOfflineCache)
			{
				return netAccessStatus == NetAccessStatus.Functional && Application.internetReachability != NetworkReachability.NotReachable;
			}
			return Application.internetReachability != NetworkReachability.NotReachable;
		}
	}

	public void setOfflineQueueNetActivityTimes(float defaultReachabilityCheckPeriodSeconds, float hitBeingBuiltRetryTimeSeconds, float netVerificationErrorRetryTimeSeconds, float netVerificationMismatchRetryTimeSeconds, float cachedHitSendThrottleTimeSeconds)
	{
		defaultReachabilityCheckPeriod = new WaitForSeconds(defaultReachabilityCheckPeriodSeconds);
		hitBeingBuiltRetryTime = new WaitForSeconds(hitBeingBuiltRetryTimeSeconds);
		netVerificationErrorRetryTime = new WaitForSeconds(netVerificationErrorRetryTimeSeconds);
		netVerificationMismatchRetryTime = new WaitForSeconds(netVerificationMismatchRetryTimeSeconds);
		cachedHitSendThrottleTime = new WaitForSeconds(cachedHitSendThrottleTimeSeconds);
	}

	private static string returnStringAsIs(string s)
	{
		return s;
	}

	private IEnumerator netActivity()
	{
		NetworkReachability prevReachability = Application.internetReachability;
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			netAccessStatus = NetAccessStatus.PendingVerification;
		}
		else
		{
			netAccessStatus = NetAccessStatus.Offline;
		}
		while (useOfflineCache)
		{
			if (netAccessStatus == NetAccessStatus.Error)
			{
				yield return netVerificationErrorRetryTime;
				netAccessStatus = NetAccessStatus.PendingVerification;
			}
			else if (netAccessStatus == NetAccessStatus.Mismatch)
			{
				yield return netVerificationMismatchRetryTime;
				netAccessStatus = NetAccessStatus.PendingVerification;
			}
			NetworkReachability networkReachability = Application.internetReachability;
			if (prevReachability != networkReachability)
			{
				if (networkReachability != NetworkReachability.NotReachable)
				{
					netAccessStatus = NetAccessStatus.PendingVerification;
				}
				else if (networkReachability == NetworkReachability.NotReachable)
				{
					netAccessStatus = NetAccessStatus.Offline;
				}
				prevReachability = Application.internetReachability;
			}
			if (netAccessStatus == NetAccessStatus.PendingVerification)
			{
				if (isHitBeingBuilt)
				{
					yield return hitBeingBuiltRetryTime;
					continue;
				}
				beginHit(HitType.Screenview);
				addNonInteractionHit();
				addCommonOptionalHitParams();
				WWW www = finalizeAndSendHit(true);
				yield return www;
				if (www.error != null && www.error.Length > 0)
				{
					netAccessStatus = NetAccessStatus.Error;
					continue;
				}
				byte[] result = www.bytes;
				if (result == null || result.Length <= 3 || result[0] != 71 || result[1] != 73 || result[2] != 70)
				{
					netAccessStatus = NetAccessStatus.Mismatch;
					continue;
				}
				netAccessStatus = NetAccessStatus.Functional;
			}
			if (pendingQueuedOfflineHits() && netAccessStatus == NetAccessStatus.Functional)
			{
				if (isHitBeingBuilt)
				{
					yield return hitBeingBuiltRetryTime;
					continue;
				}
				sendOnePendingOfflineHit();
				yield return cachedHitSendThrottleTime;
			}
			else
			{
				yield return defaultReachabilityCheckPeriod;
			}
		}
	}

	public void initialize(MonoBehaviour analyticsHelperBehaviour, string trackingID, string anonymousClientID, string appName = "", string appVersion = "", bool useHTTPS = false, string offlineCacheFilePath = "")
	{
		helperBehaviour = analyticsHelperBehaviour;
		this.trackingID = escapeString(trackingID);
		this.useHTTPS = useHTTPS;
		clientID = WWW.EscapeURL(anonymousClientID);
		this.appName = ((appName == null || appName.Length <= 0) ? null : WWW.EscapeURL(appName));
		this.appVersion = ((appVersion == null || appVersion.Length <= 0) ? null : WWW.EscapeURL(appVersion));
		analyticsDisabled = false;
		this.offlineCacheFilePath = offlineCacheFilePath;
		if (offlineCacheFilePath != null && offlineCacheFilePath.Length > 0)
		{
			useOfflineCache = true;
		}
		sb.Length = 0;
		SystemLanguage systemLanguage = Application.systemLanguage;
		userLanguage = systemLanguage.ToString();
		CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
		for (int i = 0; i < cultures.Length; i++)
		{
			if (userLanguage.Contains(cultures[i].EnglishName))
			{
				userLanguage = cultures[i].Name;
				break;
			}
		}
		sb.Append(guaVersionData);
		sb.Append("&tid=");
		sb.Append(this.trackingID);
		sb.Append("&cid=");
		sb.Append(clientID);
		defaultHitData = sb.ToString();
		sb.Length = 0;
		if (useOfflineCache)
		{
			helperBehaviour.StartCoroutine(netActivity());
		}
	}

	public void setStringEscaping(bool useStringEscaping)
	{
		if (useStringEscaping)
		{
			escapeString = WWW.EscapeURL;
		}
		else
		{
			escapeString = returnStringAsIs;
		}
	}

	public bool beginHit(HitType hitType)
	{
		string value = hitTypeNames[(int)hitType];
		if (!internetReachable && !useOfflineCache)
		{
			return false;
		}
		this.hitType = hitType;
		sb.Length = 0;
		sb.Append(defaultHitData);
		sb.Append("&t=");
		sb.Append(value);
		return true;
	}

	public void cancelHit()
	{
		sb.Length = 0;
		hitType = HitType.None;
	}

	public void setAnonymizeIP(bool enabled = true)
	{
		anonymizeIP = enabled;
	}

	public void addQueueTime(int ms)
	{
		if (ms >= 0)
		{
			sb.Append("&qt=");
			sb.Append(ms);
		}
	}

	public void setUserID(string userID)
	{
		if (userID == null || userID.Length == 0)
		{
			this.userID = string.Empty;
		}
		else
		{
			this.userID = escapeString(userID);
		}
	}

	public void addSessionControl(bool type)
	{
		sb.Append((!type) ? "&sc=end" : "&sc=start");
	}

	public void setIPOverride(string ip)
	{
		if (ip == null || ip.Length == 0)
		{
			userIP = string.Empty;
			return;
		}
		string text = escapeString(ip);
		userIP = text;
	}

	public void setUserAgentOverride(string userAgent, bool allowNonEscaped = false)
	{
		if (userAgent == null || userAgent.Length == 0)
		{
			this.userAgent = string.Empty;
			return;
		}
		string text = ((!allowNonEscaped) ? WWW.EscapeURL(userAgent) : escapeString(userAgent));
		this.userAgent = text;
	}

	public void addDocumentReferrer(string url, bool allowNonEscaped = false)
	{
		string value = ((!allowNonEscaped) ? WWW.EscapeURL(url) : escapeString(url));
		sb.Append("&dr=");
		sb.Append(value);
	}

	public void addCampaignName(string text)
	{
		string value = escapeString(text);
		sb.Append("&cn=");
		sb.Append(value);
	}

	public void addCampaignSource(string text)
	{
		string value = escapeString(text);
		sb.Append("&cs=");
		sb.Append(value);
	}

	public void addCampaignMedium(string text)
	{
		string value = escapeString(text);
		sb.Append("&cm=");
		sb.Append(value);
	}

	public void addCampaignKeyword(string text, bool allowNonEscaped = false)
	{
		string value = ((!allowNonEscaped) ? WWW.EscapeURL(text) : escapeString(text));
		sb.Append("&ck=");
		sb.Append(value);
	}

	public void addCampaignContent(string text, bool allowNonEscaped = false)
	{
		string value = ((!allowNonEscaped) ? WWW.EscapeURL(text) : escapeString(text));
		sb.Append("&cc=");
		sb.Append(value);
	}

	public void addCampaignID(string text)
	{
		string value = escapeString(text);
		sb.Append("&ci=");
		sb.Append(value);
	}

	public void addGoogleAdWordsID(string text)
	{
		string value = escapeString(text);
		sb.Append("&gclid=");
		sb.Append(value);
	}

	public void addGoogleDisplayAdsID(string text)
	{
		string value = escapeString(text);
		sb.Append("&dclid=");
		sb.Append(value);
	}

	public void addScreenResolution(int width, int height)
	{
		sb.Append("&sr=");
		sb.Append(width);
		sb.Append('x');
		sb.Append(height);
	}

	public void addViewportSize(int width, int height)
	{
		sb.Append("&vp=");
		sb.Append(width);
		sb.Append('x');
		sb.Append(height);
	}

	public void addDocumentEncoding(string text)
	{
		string value = escapeString(text);
		sb.Append("&de=");
		sb.Append(value);
	}

	public void addScreenColors(int depthBits)
	{
		sb.Append("&sd=");
		sb.Append(depthBits);
		sb.Append("-bits");
	}

	public void setUserLanguage(string text)
	{
		if (text == null || text.Length == 0)
		{
			userLanguage = string.Empty;
			return;
		}
		string text2 = escapeString(text);
		userLanguage = text2;
	}

	public void addJavaEnabled(bool enabled)
	{
		sb.Append((!enabled) ? "&je=0" : "&je=1");
	}

	public void addFlashVersion(int major, int minor, int revision)
	{
		sb.Append("&fl=");
		sb.Append(major);
		sb.Append("%20");
		sb.Append(minor);
		sb.Append("%20r");
		sb.Append(revision);
	}

	public void addNonInteractionHit()
	{
		sb.Append("&ni=1");
	}

	public void addDocumentLocationURL(string url, bool allowNonEscaped = false)
	{
		string value = ((!allowNonEscaped) ? WWW.EscapeURL(url) : escapeString(url));
		sb.Append("&dl=");
		sb.Append(value);
	}

	public void addDocumentHostName(string text)
	{
		string value = escapeString(text);
		sb.Append("&dh=");
		sb.Append(value);
	}

	public void addDocumentPath(string text, bool allowNonEscaped = false)
	{
		string value = ((!allowNonEscaped) ? WWW.EscapeURL(text) : escapeString(text));
		sb.Append("&dp=");
		sb.Append(value);
	}

	public void addDocumentTitle(string text)
	{
		string value = escapeString(text);
		sb.Append("&dt=");
		sb.Append(value);
	}

	public void addScreenName(string text)
	{
		string value = escapeString(text);
		sb.Append("&cd=");
		sb.Append(value);
	}

	public void addLinkID(string text)
	{
		string value = escapeString(text);
		sb.Append("&linkid=");
		sb.Append(value);
	}

	public void setApplicationName(string text)
	{
		if (text == null || text.Length == 0)
		{
			appName = string.Empty;
			return;
		}
		string text2 = escapeString(text);
		appName = text2;
	}

	public void setApplicationID(string text)
	{
		if (text == null || text.Length == 0)
		{
			appID = string.Empty;
			return;
		}
		string text2 = escapeString(text);
		appID = text2;
	}

	public void setApplicationVersion(string text)
	{
		if (text == null || text.Length == 0)
		{
			appVersion = string.Empty;
			return;
		}
		string text2 = escapeString(text);
		appVersion = text2;
	}

	public void addApplicationInstallerID(string text)
	{
		string value = escapeString(text);
		sb.Append("&aiid=");
		sb.Append(value);
	}

	public void addEventCategory(string text)
	{
		if (hitType == HitType.Event)
		{
			string value = escapeString(text);
			sb.Append("&ec=");
			sb.Append(value);
		}
	}

	public void addEventAction(string text)
	{
		if (hitType == HitType.Event)
		{
			string value = escapeString(text);
			sb.Append("&ea=");
			sb.Append(value);
		}
	}

	public void addEventLabel(string text)
	{
		if (hitType == HitType.Event)
		{
			string value = escapeString(text);
			sb.Append("&el=");
			sb.Append(value);
		}
	}

	public void addEventValue(int value)
	{
		if (hitType == HitType.Event && value >= 0)
		{
			sb.Append("&ev=");
			sb.Append(value);
		}
	}

	public void addTransactionID(string text)
	{
		string value = escapeString(text);
		sb.Append("&ti=");
		sb.Append(value);
	}

	public void addTransactionAffiliation(string text)
	{
		string value = escapeString(text);
		sb.Append("&ta=");
		sb.Append(value);
	}

	public void addTransactionRevenue(double currency)
	{
		sb.Append("&tr=");
		sb.AppendFormat("{0:F6}", currency);
	}

	public void addTransactionShipping(double currency)
	{
		sb.Append("&ts=");
		sb.AppendFormat("{0:F6}", currency);
	}

	public void addTransactionTax(double currency)
	{
		sb.Append("&tt=");
		sb.AppendFormat("{0:F6}", currency);
	}

	public void addItemName(string text)
	{
		if (hitType == HitType.Item)
		{
			string value = escapeString(text);
			sb.Append("&in=");
			sb.Append(value);
		}
	}

	public void addItemPrice(double currency)
	{
		if (hitType == HitType.Item)
		{
			sb.Append("&ip=");
			sb.AppendFormat("{0:F6}", currency);
		}
	}

	public void addItemQuantity(int value)
	{
		if (hitType == HitType.Item)
		{
			sb.Append("&iq=");
			sb.Append(value);
		}
	}

	public void addItemCode(string text)
	{
		if (hitType == HitType.Item)
		{
			string value = escapeString(text);
			sb.Append("&ic=");
			sb.Append(value);
		}
	}

	public void addItemCategory(string text)
	{
		if (hitType == HitType.Item)
		{
			string value = escapeString(text);
			sb.Append("&iv=");
			sb.Append(value);
		}
	}

	public void addCurrencyCode(string text)
	{
		if (hitType == HitType.Transaction || hitType == HitType.Item)
		{
			string value = escapeString(text);
			sb.Append("&cu=");
			sb.Append(value);
		}
	}

	private int clampEnhancedECommerceIndex(int index)
	{
		return Mathf.Clamp(index, 1, 200);
	}

	public void addProductAction(ProductAction action)
	{
		string value = productActionNames[(int)action];
		sb.Append("&pa=");
		sb.Append(value);
	}

	public void addProductSKU(int productIndex, string text)
	{
		productIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&pr");
		sb.Append(productIndex);
		sb.Append("id=");
		sb.Append(value);
	}

	public void addProductName(int productIndex, string text)
	{
		productIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&pr");
		sb.Append(productIndex);
		sb.Append("nm=");
		sb.Append(value);
	}

	public void addProductBrand(int productIndex, string text)
	{
		productIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&pr");
		sb.Append(productIndex);
		sb.Append("br=");
		sb.Append(value);
	}

	public void addProductCategory(int productIndex, string text)
	{
		productIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&pr");
		sb.Append(productIndex);
		sb.Append("ca=");
		sb.Append(value);
	}

	public void addProductVariant(int productIndex, string text)
	{
		productIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&pr");
		sb.Append(productIndex);
		sb.Append("va=");
		sb.Append(value);
	}

	public void addProductPrice(int productIndex, double currency)
	{
		productIndex = clampEnhancedECommerceIndex(productIndex);
		sb.Append("&pr");
		sb.Append(productIndex);
		sb.Append("pr=");
		sb.AppendFormat("{0:F6}", currency);
	}

	public void addProductQuantity(int productIndex, int value)
	{
		productIndex = clampEnhancedECommerceIndex(productIndex);
		sb.Append("&pr");
		sb.Append(productIndex);
		sb.Append("qt=");
		sb.Append(value);
	}

	public void addProductCouponCode(int productIndex, string text)
	{
		productIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&pr");
		sb.Append(productIndex);
		sb.Append("cc=");
		sb.Append(value);
	}

	public void addProductPosition(int productIndex, int value)
	{
		productIndex = clampEnhancedECommerceIndex(productIndex);
		sb.Append("&pr");
		sb.Append(productIndex);
		sb.Append("ps=");
		sb.Append(value);
	}

	public void addProductCustomDimension(int productIndex, int dimensionIndex, string text)
	{
		productIndex = clampEnhancedECommerceIndex(productIndex);
		dimensionIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&pr");
		sb.Append(productIndex);
		sb.Append("cd");
		sb.Append(dimensionIndex);
		sb.Append('=');
		sb.Append(value);
	}

	public void addProductCustomMetric(int productIndex, int dimensionIndex, long value)
	{
		productIndex = clampEnhancedECommerceIndex(productIndex);
		dimensionIndex = clampEnhancedECommerceIndex(productIndex);
		sb.Append("&pr");
		sb.Append(productIndex);
		sb.Append("cm");
		sb.Append(dimensionIndex);
		sb.Append('=');
		sb.Append(value);
	}

	public void addTransactionCouponCode(string text)
	{
		string value = escapeString(text);
		sb.Append("&tcc=");
		sb.Append(value);
	}

	public void addProductActionList(string text)
	{
		string value = escapeString(text);
		sb.Append("&pal=");
		sb.Append(value);
	}

	public void addCheckoutStep(int value)
	{
		sb.Append("&cos=");
		sb.Append(value);
	}

	public void addCheckoutStepOption(string text)
	{
		string value = escapeString(text);
		sb.Append("&col=");
		sb.Append(value);
	}

	public void addProductImpressionListName(int listIndex, string text)
	{
		listIndex = clampEnhancedECommerceIndex(listIndex);
		string value = escapeString(text);
		sb.Append("&il");
		sb.Append(listIndex);
		sb.Append("nm=");
		sb.Append(value);
	}

	public void addProductImpressionSKU(int listIndex, int productIndex, string text)
	{
		listIndex = clampEnhancedECommerceIndex(listIndex);
		productIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&il");
		sb.Append(listIndex);
		sb.Append("pi");
		sb.Append(productIndex);
		sb.Append("id=");
		sb.Append(value);
	}

	public void addProductImpressionName(int listIndex, int productIndex, string text)
	{
		listIndex = clampEnhancedECommerceIndex(listIndex);
		productIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&il");
		sb.Append(listIndex);
		sb.Append("pi");
		sb.Append(productIndex);
		sb.Append("nm=");
		sb.Append(value);
	}

	public void addProductImpressionBrand(int listIndex, int productIndex, string text)
	{
		listIndex = clampEnhancedECommerceIndex(listIndex);
		productIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&il");
		sb.Append(listIndex);
		sb.Append("pi");
		sb.Append(productIndex);
		sb.Append("br=");
		sb.Append(value);
	}

	public void addProductImpressionCategory(int listIndex, int productIndex, string text)
	{
		listIndex = clampEnhancedECommerceIndex(listIndex);
		productIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&il");
		sb.Append(listIndex);
		sb.Append("pi");
		sb.Append(productIndex);
		sb.Append("ca=");
		sb.Append(value);
	}

	public void addProductImpressionVariant(int listIndex, int productIndex, string text)
	{
		listIndex = clampEnhancedECommerceIndex(listIndex);
		productIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&il");
		sb.Append(listIndex);
		sb.Append("pi");
		sb.Append(productIndex);
		sb.Append("va=");
		sb.Append(value);
	}

	public void addProductImpressionPosition(int listIndex, int productIndex, int value)
	{
		listIndex = clampEnhancedECommerceIndex(listIndex);
		productIndex = clampEnhancedECommerceIndex(productIndex);
		sb.Append("&il");
		sb.Append(listIndex);
		sb.Append("pi");
		sb.Append(productIndex);
		sb.Append("ps=");
		sb.Append(value);
	}

	public void addProductImpressionPrice(int listIndex, int productIndex, double currency)
	{
		listIndex = clampEnhancedECommerceIndex(listIndex);
		productIndex = clampEnhancedECommerceIndex(productIndex);
		sb.Append("&il");
		sb.Append(listIndex);
		sb.Append("pi");
		sb.Append(productIndex);
		sb.Append("pr=");
		sb.AppendFormat("{0:F6}", currency);
	}

	public void addProductImpressionCustomDimension(int listIndex, int productIndex, int dimensionIndex, string text)
	{
		listIndex = clampEnhancedECommerceIndex(listIndex);
		productIndex = clampEnhancedECommerceIndex(productIndex);
		dimensionIndex = clampEnhancedECommerceIndex(productIndex);
		string value = escapeString(text);
		sb.Append("&il");
		sb.Append(listIndex);
		sb.Append("pi");
		sb.Append(productIndex);
		sb.Append("cd");
		sb.Append(dimensionIndex);
		sb.Append('=');
		sb.Append(value);
	}

	public void addProductImpressionCustomMetric(int listIndex, int productIndex, int dimensionIndex, long value)
	{
		listIndex = clampEnhancedECommerceIndex(listIndex);
		productIndex = clampEnhancedECommerceIndex(productIndex);
		dimensionIndex = clampEnhancedECommerceIndex(productIndex);
		sb.Append("&il");
		sb.Append(listIndex);
		sb.Append("pi");
		sb.Append(productIndex);
		sb.Append("cm");
		sb.Append(dimensionIndex);
		sb.Append('=');
		sb.Append(value);
	}

	public void addPromotionAction(string text)
	{
		string value = escapeString(text);
		sb.Append("&promoa=");
		sb.Append(value);
	}

	public void addPromotionID(int promotionIndex, string text)
	{
		promotionIndex = clampEnhancedECommerceIndex(promotionIndex);
		string value = escapeString(text);
		sb.Append("&promo");
		sb.Append(promotionIndex);
		sb.Append("id=");
		sb.Append(value);
	}

	public void addPromotionName(int promotionIndex, string text)
	{
		promotionIndex = clampEnhancedECommerceIndex(promotionIndex);
		string value = escapeString(text);
		sb.Append("&promo");
		sb.Append(promotionIndex);
		sb.Append("nm=");
		sb.Append(value);
	}

	public void addPromotionCreative(int promotionIndex, string text)
	{
		promotionIndex = clampEnhancedECommerceIndex(promotionIndex);
		string value = escapeString(text);
		sb.Append("&promo");
		sb.Append(promotionIndex);
		sb.Append("cr=");
		sb.Append(value);
	}

	public void addPromotionPosition(int promotionIndex, string text)
	{
		promotionIndex = clampEnhancedECommerceIndex(promotionIndex);
		string value = escapeString(text);
		sb.Append("&promo");
		sb.Append(promotionIndex);
		sb.Append("ps=");
		sb.Append(value);
	}

	public void addSocialNetwork(string text)
	{
		if (hitType == HitType.Social)
		{
			string value = escapeString(text);
			sb.Append("&sn=");
			sb.Append(value);
		}
	}

	public void addSocialAction(string text)
	{
		if (hitType == HitType.Social)
		{
			string value = escapeString(text);
			sb.Append("&sa=");
			sb.Append(value);
		}
	}

	public void addSocialActionTarget(string text, bool allowNonEscaped = false)
	{
		if (hitType == HitType.Social)
		{
			string value = ((!allowNonEscaped) ? WWW.EscapeURL(text) : escapeString(text));
			sb.Append("&st=");
			sb.Append(value);
		}
	}

	public void addUserTimingCategory(string text)
	{
		if (hitType == HitType.Timing)
		{
			string value = escapeString(text);
			sb.Append("&utc=");
			sb.Append(value);
		}
	}

	public void addUserTimingVariableName(string text)
	{
		if (hitType == HitType.Timing)
		{
			string value = escapeString(text);
			sb.Append("&utv=");
			sb.Append(value);
		}
	}

	public void addUserTimingTime(int value)
	{
		if (hitType == HitType.Timing)
		{
			sb.Append("&utt=");
			sb.Append(value);
		}
	}

	public void addUserTimingLabel(string text)
	{
		if (hitType == HitType.Timing)
		{
			string value = escapeString(text);
			sb.Append("&utl=");
			sb.Append(value);
		}
	}

	public void addPageLoadTime(int value)
	{
		if (hitType == HitType.Timing)
		{
			sb.Append("&plt=");
			sb.Append(value);
		}
	}

	public void addDNSTime(int value)
	{
		if (hitType == HitType.Timing)
		{
			sb.Append("&dns=");
			sb.Append(value);
		}
	}

	public void addPageDownloadTime(int value)
	{
		if (hitType == HitType.Timing)
		{
			sb.Append("&pdt=");
			sb.Append(value);
		}
	}

	public void addRedirectResponseTime(int value)
	{
		if (hitType == HitType.Timing)
		{
			sb.Append("&rrt=");
			sb.Append(value);
		}
	}

	public void addTCPConnectTime(int value)
	{
		if (hitType == HitType.Timing)
		{
			sb.Append("&tcp=");
			sb.Append(value);
		}
	}

	public void addServerResponseTime(int value)
	{
		if (hitType == HitType.Timing)
		{
			sb.Append("&srt=");
			sb.Append(value);
		}
	}

	public void addExceptionDescription(string text)
	{
		if (hitType == HitType.Exception)
		{
			string value = escapeString(text);
			sb.Append("&exd=");
			sb.Append(value);
		}
	}

	public void addExceptionIsFatal(bool value)
	{
		if (hitType == HitType.Exception)
		{
			sb.Append((!value) ? "&exf=0" : "&exf=1");
		}
	}

	public void addCustomDimension(int index, string text)
	{
		if (index >= 1 && index <= 200)
		{
			string value = escapeString(text);
			sb.Append("&cd");
			sb.Append(index);
			sb.Append('=');
			sb.Append(value);
		}
	}

	public void addCustomMetric(int index, long value)
	{
		if (index >= 1 && index <= 200)
		{
			sb.Append("&cm");
			sb.Append(index);
			sb.Append('=');
			sb.Append(value);
		}
	}

	public void addExperimentID(string text)
	{
		string value = escapeString(text);
		sb.Append("&xid=");
		sb.Append(value);
	}

	public void addExperimentVariant(string text)
	{
		string value = escapeString(text);
		sb.Append("&xvar=");
		sb.Append(value);
	}

	public void sendPageViewHit(string documentHostName, string documentPath, string documentTitle)
	{
		if (!analyticsDisabled)
		{
			beginHit(HitType.Pageview);
			addDocumentHostName(documentHostName);
			addDocumentPath(documentPath);
			addDocumentTitle(documentTitle);
			sendHit();
		}
	}

	public void sendEventHit(string eventCategory, string eventAction, string eventLabel = null, int eventValue = -1)
	{
		if (!Application.isEditor && !analyticsDisabled)
		{
			beginHit(HitType.Event);
			addEventCategory(eventCategory);
			addEventAction(eventAction);
			if (eventLabel != null)
			{
				addEventLabel(eventLabel);
			}
			if (eventValue >= 0)
			{
				addEventValue(eventValue);
			}
			sendHit();
		}
	}

	public void sendTransactionHit(string transactionID, string affiliation, double revenue, double shipping, double tax, string currencyCode)
	{
		if (!analyticsDisabled)
		{
			beginHit(HitType.Transaction);
			addTransactionID(transactionID);
			if (affiliation != null && affiliation.Length > 0)
			{
				addTransactionAffiliation(affiliation);
			}
			if (revenue != 0.0)
			{
				addTransactionRevenue(revenue);
			}
			if (shipping != 0.0)
			{
				addTransactionShipping(shipping);
			}
			if (tax != 0.0)
			{
				addTransactionTax(tax);
			}
			if (currencyCode != null && currencyCode.Length > 0)
			{
				addCurrencyCode(currencyCode);
			}
			sendHit();
		}
	}

	public void sendItemHit(string transactionID, string itemName, double price, int quantity, string itemCode, string itemCategory, string currencyCode)
	{
		if (!analyticsDisabled)
		{
			beginHit(HitType.Item);
			addTransactionID(transactionID);
			addItemName(itemName);
			if (price != 0.0)
			{
				addItemPrice(price);
			}
			if (quantity != 0)
			{
				addItemQuantity(quantity);
			}
			if (itemCode != null && itemCode.Length > 0)
			{
				addItemCode(itemCode);
			}
			if (itemCategory != null && itemCategory.Length > 0)
			{
				addItemCategory(itemCategory);
			}
			if (currencyCode != null && currencyCode.Length > 0)
			{
				addCurrencyCode(currencyCode);
			}
			sendHit();
		}
	}

	public void sendSocialHit(string network, string action, string target)
	{
		if (!analyticsDisabled)
		{
			beginHit(HitType.Social);
			addSocialNetwork(network);
			addSocialAction(action);
			addSocialActionTarget(target);
			sendHit();
		}
	}

	public void sendExceptionHit(string description, bool isFatal)
	{
		if (!analyticsDisabled)
		{
			beginHit(HitType.Exception);
			if (description != null && description.Length > 0)
			{
				addExceptionDescription(description);
			}
			addExceptionIsFatal(isFatal);
			sendHit();
		}
	}

	public void sendUserTimingHit(string category, string variable, int time, string label)
	{
		if (!analyticsDisabled)
		{
			beginHit(HitType.Timing);
			if (category != null && category.Length > 0)
			{
				addUserTimingCategory(category);
			}
			if (variable != null && variable.Length > 0)
			{
				addUserTimingVariableName(variable);
			}
			if (time >= 0)
			{
				addUserTimingTime(time);
			}
			if (label != null && label.Length > 0)
			{
				addUserTimingLabel(label);
			}
			sendHit();
		}
	}

	public void sendBrowserTimingHit(int dnsTime, int pageDownloadTime, int redirectTime, int tcpConnectTime, int serverResponseTime)
	{
		if (!analyticsDisabled)
		{
			beginHit(HitType.Timing);
			if (dnsTime >= 0)
			{
				addDNSTime(dnsTime);
			}
			if (pageDownloadTime >= 0)
			{
				addPageDownloadTime(pageDownloadTime);
			}
			if (redirectTime >= 0)
			{
				addRedirectResponseTime(redirectTime);
			}
			if (tcpConnectTime >= 0)
			{
				addTCPConnectTime(tcpConnectTime);
			}
			if (serverResponseTime >= 0)
			{
				addServerResponseTime(serverResponseTime);
			}
			sendHit();
		}
	}

	public void sendAppScreenHit(string screenName)
	{
		if (!analyticsDisabled)
		{
			beginHit(HitType.Screenview);
			addScreenName(screenName);
			sendHit();
		}
	}

	private long getPOSIXTimeMilliseconds()
	{
		if (!epoch.HasValue)
		{
			epoch = new DateTime(1970, 1, 1);
		}
		return (long)(DateTime.UtcNow - epoch.Value).TotalMilliseconds;
	}

	private void stopOfflineCacheReader()
	{
		if (offlineCacheReader != null)
		{
			offlineCacheReader.Close();
			offlineCacheReader.Dispose();
			offlineCacheReader = null;
		}
	}

	private void stopOfflineCacheWriter()
	{
		if (offlineCacheWriter != null)
		{
			try
			{
				offlineCacheWriter.Close();
			}
			catch (Exception)
			{
			}
			offlineCacheWriter.Dispose();
			offlineCacheWriter = null;
		}
	}

	public void closeOfflineCacheFile()
	{
		stopOfflineCacheReader();
		stopOfflineCacheWriter();
		PlayerPrefs.Save();
	}

	private void increasePlayerPrefOfflineQueueLength()
	{
		if (offlineQueueLength == -1)
		{
			offlineQueueLength = PlayerPrefs.GetInt(offlineQueueLengthPrefKey, 0);
		}
		offlineQueueLength++;
		PlayerPrefs.SetInt(offlineQueueLengthPrefKey, offlineQueueLength);
	}

	private void increasePlayerPrefOfflineQueueSentHits()
	{
		if (offlineQueueSentHits == -1)
		{
			offlineQueueSentHits = PlayerPrefs.GetInt(offlineQueueSentHitsPrefKey, 0);
		}
		offlineQueueSentHits++;
		PlayerPrefs.SetInt(offlineQueueSentHitsPrefKey, offlineQueueSentHits);
	}

	private void clearOfflineQueue()
	{
		stopOfflineCacheReader();
		stopOfflineCacheWriter();
		try
		{
			File.Delete(offlineCacheFilePath);
		}
		catch (Exception)
		{
			offlineQueueSentHits = offlineQueueLength;
			PlayerPrefs.SetInt(offlineQueueSentHitsPrefKey, offlineQueueSentHits);
			PlayerPrefs.Save();
			return;
		}
		PlayerPrefs.DeleteKey(offlineQueueSentHitsPrefKey);
		PlayerPrefs.DeleteKey(offlineQueueLengthPrefKey);
		PlayerPrefs.Save();
		offlineQueueLength = 0;
		offlineQueueSentHits = 0;
	}

	private bool saveHitToOfflineQueue(string hitData)
	{
		stopOfflineCacheReader();
		if (offlineCacheWriter == null)
		{
			try
			{
				offlineCacheWriter = File.AppendText(offlineCacheFilePath);
			}
			catch (Exception)
			{
				sb.Length = 0;
				hitType = HitType.None;
				return false;
			}
			offlineCacheWriter.AutoFlush = false;
			offlineCacheWriter.NewLine = "\n";
		}
		try
		{
			long pOSIXTimeMilliseconds = getPOSIXTimeMilliseconds();
			offlineCacheWriter.Write("0\t");
			offlineCacheWriter.Write(pOSIXTimeMilliseconds);
			offlineCacheWriter.Write('\t');
			offlineCacheWriter.WriteLine(hitData);
			offlineCacheWriter.Flush();
			increasePlayerPrefOfflineQueueLength();
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public bool pendingQueuedOfflineHits()
	{
		if (!internetReachable || analyticsDisabled)
		{
			return false;
		}
		if (offlineQueueLength == -1)
		{
			offlineQueueLength = PlayerPrefs.GetInt(offlineQueueLengthPrefKey, 0);
		}
		if (offlineQueueSentHits == -1)
		{
			offlineQueueSentHits = PlayerPrefs.GetInt(offlineQueueSentHitsPrefKey, 0);
		}
		return offlineQueueLength > offlineQueueSentHits;
	}

	public bool sendOnePendingOfflineHit()
	{
		if (!pendingQueuedOfflineHits())
		{
			return false;
		}
		if (isHitBeingBuilt)
		{
			return false;
		}
		stopOfflineCacheWriter();
		if (offlineCacheReader == null)
		{
			try
			{
				offlineCacheReader = File.OpenText(offlineCacheFilePath);
			}
			catch (Exception)
			{
				return false;
			}
			try
			{
				for (int i = 0; i < offlineQueueSentHits; i++)
				{
					offlineCacheReader.ReadLine();
				}
			}
			catch (Exception)
			{
				clearOfflineQueue();
				return false;
			}
		}
		long pOSIXTimeMilliseconds = getPOSIXTimeMilliseconds();
		string text = null;
		bool flag = false;
		try
		{
			text = offlineCacheReader.ReadLine();
		}
		catch (Exception)
		{
			flag = true;
		}
		if (text == null || text.Length < 20)
		{
			flag = true;
		}
		if (flag)
		{
			clearOfflineQueue();
			return false;
		}
		string[] array = text.Split(tabRowSplitter);
		if (array.Length < 3 || !array[0].Equals("0"))
		{
			increasePlayerPrefOfflineQueueSentHits();
			return false;
		}
		long result;
		if (!long.TryParse(array[1], out result))
		{
			increasePlayerPrefOfflineQueueSentHits();
			return false;
		}
		sb.Append(array[2]);
		bool flag2 = false;
		long num = 129600000L;
		long num2 = pOSIXTimeMilliseconds - result;
		if (num2 > 0)
		{
			if (num2 > num)
			{
				flag2 = true;
			}
			else
			{
				addQueueTime((int)num2);
			}
		}
		else if (num2 < -num)
		{
			flag2 = true;
		}
		increasePlayerPrefOfflineQueueSentHits();
		if (flag2)
		{
			cancelHit();
		}
		else
		{
			finalizeAndSendHit();
		}
		if (offlineQueueSentHits >= offlineQueueLength)
		{
			clearOfflineQueue();
		}
		return true;
	}

	private void addCommonOptionalHitParams()
	{
		if (appName != null && appName.Length > 0)
		{
			sb.Append("&an=");
			sb.Append(appName);
		}
		if (appID != null && appID.Length > 0)
		{
			sb.Append("&aid=");
			sb.Append(appID);
		}
		if (appVersion != null && appVersion.Length > 0)
		{
			sb.Append("&av=");
			sb.Append(appVersion);
		}
		if (userID != null && userID.Length > 0)
		{
			sb.Append("&uid=");
			sb.Append(userID);
		}
		if (userLanguage != null && userLanguage.Length > 0)
		{
			sb.Append("&ul=");
			sb.Append(userLanguage);
		}
		if (userIP != null && userIP.Length > 0)
		{
			sb.Append("&uip=");
			sb.Append(userIP);
		}
		if (anonymizeIP)
		{
			sb.Append("&aip=1");
		}
		if (userAgent != null && userAgent.Length > 0)
		{
			sb.Append("&ua=");
			sb.Append(userAgent);
		}
	}

	public bool sendHit()
	{
		if (analyticsDisabled)
		{
			cancelHit();
			return false;
		}
		if (hitType == HitType.None)
		{
			return false;
		}
		addCommonOptionalHitParams();
		if (!internetReachable)
		{
			if (!useOfflineCache)
			{
				sb.Length = 0;
				hitType = HitType.None;
				return false;
			}
			string hitData = sb.ToString();
			sb.Length = 0;
			hitType = HitType.None;
			return saveHitToOfflineQueue(hitData);
		}
		finalizeAndSendHit();
		return true;
	}

	private WWW beginWWWRequest(string postDataString)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(postDataString);
		if (customHeaders != null)
		{
			return new WWW((!useHTTPS) ? httpCollectUrl : httpsCollectUrl, bytes, customHeaders);
		}
		return new WWW((!useHTTPS) ? httpCollectUrl : httpsCollectUrl, bytes);
	}

	private IEnumerator doWWWRequestAndCheckResult(string postDataString)
	{
		WWW www = beginWWWRequest(postDataString);
		yield return www;
		bool sendFailed_saveToOffline;
		if (www.error != null && www.error.Length > 0)
		{
			netAccessStatus = NetAccessStatus.Error;
			sendFailed_saveToOffline = true;
		}
		else
		{
			byte[] result = www.bytes;
			if (result != null && result.Length > 3 && result[0] == 71 && result[1] == 73 && result[2] == 70)
			{
				sendFailed_saveToOffline = false;
			}
			else
			{
				netAccessStatus = NetAccessStatus.Mismatch;
				sendFailed_saveToOffline = true;
			}
		}
		if (sendFailed_saveToOffline)
		{
			saveHitToOfflineQueue(postDataString);
		}
	}

	private WWW finalizeAndSendHit(bool needReturnWWW = false)
	{
		sb.Append("&z=");
		sb.Append(UnityEngine.Random.Range(0, int.MaxValue) ^ 0x13377AA7);
		string postDataString = sb.ToString();
		sb.Length = 0;
		hitType = HitType.None;
		if (useOfflineCache && !needReturnWWW)
		{
			helperBehaviour.StartCoroutine(doWWWRequestAndCheckResult(postDataString));
			return null;
		}
		return beginWWWRequest(postDataString);
	}
}
