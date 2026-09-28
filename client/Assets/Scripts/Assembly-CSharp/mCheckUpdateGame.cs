using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeStage.AntiCheat.ObscuredTypes;
using MovementEffects;
using UnityEngine;

public class mCheckUpdateGame : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CCheckGame_003Ec__Iterator22 : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal string _003Curl_003E__0;

		internal WWW _003Cwww_003E__1;

		internal int _0024PC;

		internal float _0024current;

		internal mCheckUpdateGame _003C_003Ef__this;

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
				_003Curl_003E__0 = "https://play.google.com/store/apps/details?id=com.rexetstudio.blockstrike&hl=en";
				_003Cwww_003E__1 = new WWW(_003Curl_003E__0);
				_0024current = Timing.WaitUntilDone(_003Cwww_003E__1);
				_0024PC = 1;
				return true;
			case 1u:
				if (string.IsNullOrEmpty(_003Cwww_003E__1.error))
				{
					_003C_003Ef__this.data = _003Cwww_003E__1.text;
					if (_003C_003Ef__this.StringToInt(VersionManager.bundleVersion) < _003C_003Ef__this.StringToInt(_003C_003Ef__this.GetVersion()))
					{
						ObscuredPrefs.SetBool("NewVersion" + VersionManager.bundleVersion, true);
						mPopUp.ShowPopup(Localization.Get("Available new version of the game"), Localization.Get("New Version"), Localization.Get("Download"), _003C_003Ef__this.Download);
					}
				}
				else
				{
					Timing.RunCoroutine(_003C_003Ef__this.CheckGame());
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

	private string data;

	private void Start()
	{
		if (ObscuredPrefs.GetBool("NewVersion" + VersionManager.bundleVersion, false))
		{
			mPopUp.ShowPopup(Localization.Get("Available new version of the game"), Localization.Get("New Version"), Localization.Get("Download"), Download);
		}
		else if (!AccountManager.isConnect)
		{
			Timing.RunCoroutine(CheckGame());
		}
	}

	[DebuggerHidden]
	private IEnumerator<float> CheckGame()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CCheckGame_003Ec__Iterator22 obj = new _003CCheckGame_003Ec__Iterator22();
		obj._003C_003Ef__this = this;
		return obj;
	}

	private void Download()
	{
		AndroidNativeFunctions.OpenGooglePlay("com.rexetstudio.blockstrike");
	}

	private string GetVersion()
	{
		string text = data;
		int count = text.LastIndexOf("softwareVersion") + 18;
		text = text.Remove(0, count);
		count = text.IndexOf("</div>") - 2;
		return text.Remove(count);
	}

	private int StringToInt(string text)
	{
		string text2 = string.Empty;
		for (int i = 0; i < text.Length; i++)
		{
			if (char.IsDigit(text[i]))
			{
				text2 += text[i];
			}
		}
		return int.Parse(text2);
	}
}
