using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using AntiSpeedHack4Android;
using CodeStage.AntiCheat.ObscuredTypes;
using FreeJSON;
using MovementEffects;
using UnityEngine;

public class CheatSystem : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CTest_003Ec__Iterator27 : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal string _003Caa1_003E__0;

		internal string _003Caa2_003E__1;

		internal string _003Caa3_003E__2;

		internal string _003Caa4_003E__3;

		internal string _003Caa5_003E__4;

		internal string _003Caa6_003E__5;

		internal string _003Ca1_003E__6;

		internal string _003Ca2_003E__7;

		internal string _003Ca3_003E__8;

		internal string _003Cb1_003E__9;

		internal string _003Cc1_003E__10;

		internal string _003Cd1_003E__11;

		internal string _003Cd2_003E__12;

		internal string _003Cd3_003E__13;

		internal string _003Ce1_003E__14;

		internal string _003Ce2_003E__15;

		internal string _003Ce3_003E__16;

		internal string _003Cf1_003E__17;

		internal string _003Cf2_003E__18;

		internal string _003Cg1_003E__19;

		internal string _003Cpath_003E__20;

		internal string[] _003Cpaths_003E__21;

		internal int _003Ci_003E__22;

		internal int _003Cj_003E__23;

		internal string _003Cfile_003E__24;

		internal int _003Ci_003E__25;

		internal string[] _003Cfiles_003E__26;

		internal int _003Cj_003E__27;

		internal string _003Cfile_003E__28;

		internal int _003Cindex_003E__29;

		internal int _003Cindex2_003E__30;

		internal int _003Cindex3_003E__31;

		internal int _0024PC;

		internal float _0024current;

		private static vp_Timer.Callback _003C_003Ef__am_0024cache22;

		private static vp_Timer.Callback _003C_003Ef__am_0024cache23;

		private static vp_Timer.Callback _003C_003Ef__am_0024cache24;

		private static vp_Timer.Callback _003C_003Ef__am_0024cache25;

		private static vp_Timer.Callback _003C_003Ef__am_0024cache26;

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
				_003Caa1_003E__0 = AesEncryptor.DecryptString("20r9qXMVXh27hj7LfRN2fCdnyClVneKJnS9YNFXNCiP0LEFz1HbcC3lus4rzAYmE");
				_003Caa2_003E__1 = AesEncryptor.DecryptString("EP1st5FOxyQSKq4ikYqDAr7ITIUDC+DhU7q7ODs5kqqcSM5ZlbWxic4Xa3/YzLaP");
				_003Caa3_003E__2 = AesEncryptor.DecryptString("ubqmxqhhMYMxxKl5rIZuAzfe+wAXhXbYsMkqDMVQbfc=");
				_003Caa4_003E__3 = AesEncryptor.DecryptString("X8Y5FSIgs68iAgAaqzQqn0S9xoLOBH0rffJfBowWCCEVsHPO2QUsBC+i1BLIhf+t");
				_003Caa5_003E__4 = AesEncryptor.DecryptString("BF+zq2M+jPCX9bEV9VTbASJHMfsAv9qlc3lkAXwbdKu7FIKGrQNVKp5EbMbCBgux");
				_003Caa6_003E__5 = AesEncryptor.DecryptString("Ym7A4DGPn9Zy/lDGZG9y4u3EGhHvu2K50/p4J/P1BwhlQCF8CDIAu3sMWLKiA2o8");
				_0024current = Timing.WaitForSeconds(0.1f);
				_0024PC = 1;
				break;
			case 1u:
				if (AndroidNativeFunctions.isInstalledApp(_003Caa1_003E__0))
				{
					AndroidNativeFunctions.ShowToast(_003Caa2_003E__1);
					vp_Timer.In(1f, () =>
					{
						Application.Quit();
					});
					_0024current = Timing.WaitForSeconds(2f);
					_0024PC = 2;
					break;
				}
				goto case 2u;
			case 2u:
				_0024current = Timing.WaitForSeconds(0.1f);
				_0024PC = 3;
				break;
			case 3u:
				if (AndroidNativeFunctions.isInstalledApp(_003Caa3_003E__2))
				{
					AndroidNativeFunctions.ShowToast(_003Caa4_003E__3);
					vp_Timer.In(1f, () =>
					{
						Application.Quit();
					});
					_0024current = Timing.WaitForSeconds(2f);
					_0024PC = 4;
					break;
				}
				goto case 4u;
			case 4u:
				_0024current = Timing.WaitForSeconds(0.1f);
				_0024PC = 5;
				break;
			case 5u:
				if (AndroidNativeFunctions.isInstalledApp(_003Caa5_003E__4))
				{
					AndroidNativeFunctions.ShowToast(_003Caa6_003E__5);
					vp_Timer.In(1f, () =>
					{
						Application.Quit();
					});
					_0024current = Timing.WaitForSeconds(2f);
					_0024PC = 6;
					break;
				}
				goto case 6u;
			case 6u:
				_0024current = Timing.WaitForSeconds(0.1f);
				_0024PC = 7;
				break;
			case 7u:
				_003Ca1_003E__6 = AesEncryptor.DecryptString("4GX6r3wIP8Z/OH2FBfnm694X4sRJFA5BseJ+y3MasvsgiLnuBdjDrgA8L1GOiBZi");
				_003Ca2_003E__7 = AesEncryptor.DecryptString("bkRLBNB/++jAUd9bG2uAu0Cn0Ur7d2xcqK8h19RWPGjT2uL2/sqoaxRlIJ4kWXr1");
				_003Ca3_003E__8 = AesEncryptor.DecryptString("m9PpsQYLpnCHcUzm/u2NvaySqPmDSjb6G5frBYZ3QIU=");
				_003Cb1_003E__9 = AesEncryptor.DecryptString("u62v4Skbxm8EimNSBstNMxN0ymaaQF5gF2HZk6prfwU=");
				_003Cc1_003E__10 = AesEncryptor.DecryptString("idw749SPn6m3kCJn70jajw/3xOO30FcsF1RjhuQS82A=");
				_003Cd1_003E__11 = AesEncryptor.DecryptString("siR7HDT3QO9op6oqe1FM5yTkupeEsw1xorwthvYgY2I=");
				_003Cd2_003E__12 = AesEncryptor.DecryptString("sIwTOEUhM+33QeX2xx/7l9ORCrgM4Ljxs43ghyYJu4A=");
				_003Cd3_003E__13 = AesEncryptor.DecryptString("svOJBfbcRTyGIwqmro+wu4YWIqJZ1WRNkQoXk6eL5/I=");
				_003Ce1_003E__14 = AesEncryptor.DecryptString("2JzRXxLY5k9vKiJgVZQCiTk1crT3OKS2NmzcTtB0qXg=");
				_003Ce2_003E__15 = AesEncryptor.DecryptString("DdY5wKUfP/LeLDDrnUYsvmHPs/QeRr6om6VyVxu4dU8=");
				_003Ce3_003E__16 = AesEncryptor.DecryptString("QYLptCnAajmwzBrdvYTSonuuuG11kEhKR3rM9aF8g40=");
				_003Cf1_003E__17 = AesEncryptor.DecryptString("4gqrqk2McS8z5PswKe9uzBeRWTX/VeoikCceCkV6Rmw=");
				_003Cf2_003E__18 = AesEncryptor.DecryptString("LbJF+i24khTT+kCckyuP1vHkZThDxlzJLChyKopGX5DcxpfhZeIAWiOzNNnvfmM+");
				_003Cg1_003E__19 = "Uh4QC037izr/U6ZH344v+AzzBbZsWBOmUY7UAfs5dRTdfwKT28kkyQ7Z3zHGcJ3L";
				_003Cpath_003E__20 = new AndroidJavaClass(_003Ca1_003E__6).CallStatic<AndroidJavaObject>(_003Ca2_003E__7, new object[0]).Call<string>(_003Ca3_003E__8, new object[0]);
				_003Cpaths_003E__21 = Directory.GetDirectories(_003Cpath_003E__20 + _003Cb1_003E__9);
				_003Ci_003E__22 = 0;
				goto IL_04a4;
			case 8u:
				if (Directory.Exists(_003Cpaths_003E__21[_003Ci_003E__22] + _003Cc1_003E__10))
				{
					for (_003Cj_003E__23 = 0; _003Cj_003E__23 < Directory.GetFiles(_003Cpaths_003E__21[_003Ci_003E__22] + _003Cc1_003E__10, _003Cd1_003E__11).Length; _003Cj_003E__23++)
					{
						_003Cfile_003E__24 = Directory.GetFiles(_003Cpaths_003E__21[_003Ci_003E__22] + _003Cc1_003E__10, _003Cd1_003E__11)[_003Cj_003E__23];
						if (_003Cfile_003E__24.Contains(_003Ce1_003E__14) || _003Cfile_003E__24.Contains(_003Ce2_003E__15) || _003Cfile_003E__24.Contains(_003Ce3_003E__16))
						{
							AndroidNativeFunctions.ShowToast(AesEncryptor.DecryptString(_003Cg1_003E__19));
							vp_Timer.In(1f, () =>
							{
								Application.Quit();
							});
						}
					}
				}
				_003Ci_003E__22++;
				goto IL_04a4;
			case 9u:
				if (Directory.Exists(_003Cpaths_003E__21[_003Ci_003E__25] + _003Cc1_003E__10))
				{
					_003Cfiles_003E__26 = Directory.GetFiles(_003Cpaths_003E__21[_003Ci_003E__25] + _003Cc1_003E__10, _003Cd2_003E__12);
					for (_003Cj_003E__27 = 0; _003Cj_003E__27 < _003Cfiles_003E__26.Length; _003Cj_003E__27++)
					{
						if (Path.GetFileName(_003Cfiles_003E__26[_003Cj_003E__27]) == _003Cd3_003E__13)
						{
							_003Cfile_003E__28 = File.ReadAllText(_003Cfiles_003E__26[_003Cj_003E__27]);
							if (_003Cfile_003E__28.Contains(VersionManager.bundleIdentifier))
							{
								_003Cindex_003E__29 = _003Cfile_003E__28.LastIndexOf(_003Cf1_003E__17);
								_003Cindex2_003E__30 = _003Cfile_003E__28.LastIndexOf(VersionManager.bundleIdentifier);
								_003Cindex3_003E__31 = _003Cfile_003E__28.LastIndexOf(_003Cf2_003E__18);
								if (_003Cindex3_003E__31 == -1 && _003Cindex2_003E__30 > _003Cindex_003E__29)
								{
									AndroidNativeFunctions.ShowToast(AesEncryptor.DecryptString(_003Cg1_003E__19));
									vp_Timer.In(1f, () =>
									{
										Application.Quit();
									});
								}
							}
						}
					}
				}
				_003Ci_003E__25++;
				goto IL_064e;
			default:
				{
					return false;
				}
				IL_04a4:
				if (_003Ci_003E__22 < _003Cpaths_003E__21.Length)
				{
					_0024current = Timing.WaitForSeconds(0.02f);
					_0024PC = 8;
					break;
				}
				_003Ci_003E__25 = 0;
				goto IL_064e;
				IL_064e:
				if (_003Ci_003E__25 < _003Cpaths_003E__21.Length)
				{
					_0024current = Timing.WaitForSeconds(0.02f);
					_0024PC = 9;
					break;
				}
				_0024PC = -1;
				goto default;
			}
			return true;
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

		private static void _003C_003Em__146()
		{
			Application.Quit();
		}

		private static void _003C_003Em__147()
		{
			Application.Quit();
		}

		private static void _003C_003Em__148()
		{
			Application.Quit();
		}

		private static void _003C_003Em__149()
		{
			Application.Quit();
		}

		private static void _003C_003Em__14A()
		{
			Application.Quit();
		}
	}

	private ObscuredInt serverFalsePositives;

	private ObscuredInt falsePositives;

	private void Start()
	{
		AntiSpeedHack.detectListener = (OnDetectListener)Delegate.Combine(AntiSpeedHack.detectListener, new OnDetectListener(Quit));
		OnApplicationFocus(true);
		CheckDevicesBans();
	}

	private void Quit()
	{
		if (PhotonNetwork.inRoom)
		{
			UIGameManager.instance.OnExitServer();
			vp_Timer.Handle handle = new vp_Timer.Handle();
			vp_Timer.In(0.3f, () =>
			{
				Application.Quit();
			}, handle);
			handle.CancelOnLoad = false;
		}
		else
		{
			Application.Quit();
		}
	}

	private void UpdateTime(double serverTime)
	{
		double time = PhotonNetwork.time;
		if (serverTime + 1.0 > time)
		{
			falsePositives = 0;
			if (serverTime - time >= 1.0)
			{
				++serverFalsePositives;
				if ((int)serverFalsePositives >= 3)
				{
					PlayerPrefs.SetString("KickInfo", Localization.Get("ServerAdminSpeedHack"));
					Quit();
				}
			}
		}
		else
		{
			++falsePositives;
			if ((int)falsePositives >= 3)
			{
				Quit();
			}
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		if (!PhotonNetwork.offlineMode)
		{
			vp_Timer.In(0.5f, () =>
			{
				EventManager.AddListener<double>("ServerTime", UpdateTime);
			});
			if (AccountManager.GetMoney() >= 650000 || AccountManager.GetGold() >= 7000)
			{
				Firebase firebase = new Firebase();
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("money", AccountManager.GetMoney());
				jsonObject.Add("gold", AccountManager.GetGold());
				jsonObject.Add("androidID", AndroidNativeFunctions.GetAndroidID());
				firebase.Child("Players").Child("CheckPlayers").Child(AccountManager.AccountID)
					.SetValue(jsonObject.ToString());
			}
		}
	}

	private void CheckDevicesBans()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Firebase firebase = new Firebase();
			firebase.Child("Players").Child("DevicesBans").GetValue(FirebaseParam.Default.OrderByValue().EqualTo(AndroidNativeFunctions.GetAndroidID()), CheckDevicesBansSuccess, CheckDevicesBansFailed);
		}
	}

	private void CheckDevicesBansSuccess(string json)
	{
		if (json.Contains(AndroidNativeFunctions.GetAndroidID()))
		{
			AndroidNativeFunctions.ShowAlert("Your device is banned in the game.", "Block Strike", "OK", string.Empty, string.Empty, CheckDevicesBansOnClick);
		}
	}

	private void CheckDevicesBansFailed(string json)
	{
		CheckDevicesBans();
	}

	private void CheckDevicesBansOnClick(DialogInterface dialog)
	{
		if (dialog == DialogInterface.Positive)
		{
			Application.Quit();
		}
	}

	private void OnApplicationFocus(bool pause)
	{
		if (pause && !Application.isEditor)
		{
			vp_Timer.In(0.5f, () =>
			{
				StopAllCoroutines();
				Timing.RunCoroutine(Test());
			});
		}
	}

	[DebuggerHidden]
	private IEnumerator<float> Test()
	{
		//yield-return decompiler failed: Could not find currentField
		return new _003CTest_003Ec__Iterator27();
	}
}
