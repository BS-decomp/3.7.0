using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FreeJSON;
using MovementEffects;
using UnityEngine;

public class mVideosManager : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CUpdateOthersVideosCorountine_003Ec__Iterator25 : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal string data;

		internal JsonObject _003Cjson_003E__0;

		internal int _003Ci_003E__1;

		internal GameObject _003Cgo_003E__2;

		internal string _003Ckey_003E__3;

		internal int _0024PC;

		internal float _0024current;

		internal string _003C_0024_003Edata;

		internal mVideosManager _003C_003Ef__this;

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
				if (!_003C_003Ef__this.isLoadedVideos)
				{
					mPopUp.SetActiveWait(false);
					_003C_003Ef__this.isLoadedVideos = true;
					_003Cjson_003E__0 = JsonObject.Parse(data);
					if (_003C_003Ef__this.LastIndex < 0)
					{
						_003C_003Ef__this.LastIndex = int.Parse(_003Cjson_003E__0.GetKey(_003Cjson_003E__0.Length - 1));
						_003C_003Ef__this.StartIndex = _003C_003Ef__this.LastIndex;
					}
					_003Ci_003E__1 = 0;
					goto IL_023a;
				}
				goto IL_0250;
			case 1u:
				{
					_003Cgo_003E__2 = NGUITools.AddChild(_003C_003Ef__this.VideoScroll.gameObject, _003C_003Ef__this.VideoElement);
					_003Cgo_003E__2.transform.localPosition = new Vector3((_003C_003Ef__this.StartIndex - _003C_003Ef__this.LastIndex) * 240 - 240, 0f, 0f);
					_003Ckey_003E__3 = _003C_003Ef__this.LastIndex.ToString();
					_003C_003Ef__this.VideosJson.Add(_003Ckey_003E__3, _003Cjson_003E__0.Get<JsonObject>(_003Ckey_003E__3));
					_003Cgo_003E__2.GetComponent<mVideoElement>().SetData(_003Cjson_003E__0.Get<string>(_003Ckey_003E__3), (float)_003Ci_003E__1 + 1f);
					_003C_003Ef__this.LastIndex--;
					if (_003Ci_003E__1 == _003C_003Ef__this.MaxLoad - 1 && _003C_003Ef__this.LastIndex > 0)
					{
						_003C_003Ef__this.NextLoadButton.SetActive(true);
						_003C_003Ef__this.NextLoadButton.transform.localPosition = new Vector3((_003C_003Ef__this.VideosJson.Length - 1) * 240 - 80, 0f, 0f);
					}
					_003Ci_003E__1++;
					goto IL_023a;
				}
				IL_023a:
				if (_003Ci_003E__1 < _003Cjson_003E__0.Length)
				{
					_0024current = Timing.WaitForSeconds(0.02f);
					_0024PC = 1;
					return true;
				}
				goto IL_0250;
				IL_0250:
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

	public int MaxLoad = 5;

	public GameObject VideoElement;

	public UIScrollView VideoScroll;

	public GameObject NextLoadButton;

	public GameObject VideosBackButton;

	public GameObject NewVideoIcon;

	private bool isLoadedVideos;

	private JsonObject VideosJson = new JsonObject();

	private int LastIndex = -2;

	private int StartIndex = -2;

	private void Start()
	{
		if (!AccountManager.isConnect && !PlayerPrefs.HasKey("LastVideosUpdateClick"))
		{
			Firebase firebase = new Firebase();
			firebase.Child("Others").Child("LastVideosUpdate").GetValue(GetLastVideosUpdateSuccess, null);
		}
		else if (PlayerPrefs.HasKey("LastVideosUpdateClick"))
		{
			NewVideoIcon.SetActive(true);
		}
	}

	private void GetLastVideosUpdateSuccess(string value)
	{
		if (PlayerPrefs.GetString("LastVideosUpdate") != value)
		{
			PlayerPrefs.SetString("LastVideosUpdate", value);
			PlayerPrefs.SetInt("LastVideosUpdateClick", 1);
			NewVideoIcon.SetActive(true);
		}
	}

	public void Load()
	{
		if (!isLoadedVideos)
		{
			Firebase firebase = new Firebase();
			firebase.Child("Videos").GetValue(FirebaseParam.Default.OrderByKey().LimitToLast(MaxLoad), LoadSuccess, LoadFailed);
			mPopUp.SetActiveWait(true, Localization.Get("Loading") + "...");
			VideosBackButton.SetActive(false);
		}
		else
		{
			UpdateOthersVideos(string.Empty);
		}
		if (PlayerPrefs.HasKey("LastVideosUpdateClick"))
		{
			NewVideoIcon.SetActive(false);
			PlayerPrefs.DeleteKey("LastVideosUpdateClick");
		}
	}

	public void LoadNext()
	{
		isLoadedVideos = false;
		NextLoadButton.SetActive(false);
		Firebase firebase = new Firebase();
		firebase.Child("Videos").GetValue(FirebaseParam.Default.OrderByKey().StartAt((LastIndex - MaxLoad + 1).ToString()).EndAt(LastIndex.ToString()), LoadSuccess, LoadFailed);
		mPopUp.SetActiveWait(true, Localization.Get("Loading") + "...");
		VideosBackButton.SetActive(false);
	}

	private void UpdateOthersVideos(string data)
	{
		Timing.RunCoroutine(UpdateOthersVideosCorountine(data));
	}

	[DebuggerHidden]
	private IEnumerator<float> UpdateOthersVideosCorountine(string data)
	{
		//yield-return decompiler failed: Could not find currentField
		_003CUpdateOthersVideosCorountine_003Ec__Iterator25 obj = new _003CUpdateOthersVideosCorountine_003Ec__Iterator25();
		obj.data = data;
		obj._003C_0024_003Edata = data;
		obj._003C_003Ef__this = this;
		return obj;
	}

	private void LoadSuccess(string json)
	{
		UpdateOthersVideos(json);
		VideosBackButton.SetActive(true);
	}

	private void LoadFailed(string error)
	{
		VideosBackButton.SetActive(true);
		mPopUp.SetActiveWait(false);
		UIToast.Show("Error: " + error, 3f);
	}
}
