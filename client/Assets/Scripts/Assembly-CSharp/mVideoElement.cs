using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FreeJSON;
using MovementEffects;
using UnityEngine;

public class mVideoElement : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CLoadImage_003Ec__Iterator24 : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal WWW _003Cwww_003E__0;

		internal byte[] _003Cbytes_003E__1;

		internal int _0024PC;

		internal float _0024current;

		internal mVideoElement _003C_003Ef__this;

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
				_003Cwww_003E__0 = new WWW("http://img.youtube.com/vi/" + _003C_003Ef__this.id + "/0.jpg");
				_0024current = Timing.WaitUntilDone(_003Cwww_003E__0);
				_0024PC = 1;
				return true;
			case 1u:
				if (string.IsNullOrEmpty(_003Cwww_003E__0.error))
				{
					_003C_003Ef__this.Icon.mainTexture = _003Cwww_003E__0.texture;
					_003Cbytes_003E__1 = _003Cwww_003E__0.texture.EncodeToJPG();
					CacheManager.SaveAsync(_003C_003Ef__this.id, "Videos", _003Cbytes_003E__1, true);
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

	public UITexture Icon;

	public UILabel TitleLabel;

	public UILabel RegionLabel;

	public UILabel ChannelLabel;

	private string id;

	public void SetData(string data, float delay)
	{
		vp_Timer.In(delay / 10f, () =>
		{
			JsonObject jsonObject = JsonObject.Parse(data);
			id = jsonObject.Get<string>("id");
			if (CacheManager.Exists(id, "Videos", true))
			{
				CacheManager.LoadAsync<byte[]>(CreateIcon, id, "Videos", true);
			}
			else
			{
				Timing.RunCoroutine(LoadImage());
			}
			TitleLabel.text = jsonObject.Get<string>("title");
			ChannelLabel.text = jsonObject.Get<string>("channel");
			if (jsonObject.ContainsKey("region"))
			{
				RegionLabel.text = Localization.Get("Language") + ": " + jsonObject.Get<string>("region");
			}
			TweenAlpha.Begin(base.gameObject, 0.5f, 1f);
		});
	}

	[DebuggerHidden]
	private IEnumerator<float> LoadImage()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CLoadImage_003Ec__Iterator24 obj = new _003CLoadImage_003Ec__Iterator24();
		obj._003C_003Ef__this = this;
		return obj;
	}

	private void CreateIcon(byte[] bytes)
	{
		Texture2D texture2D = new Texture2D(480, 360);
		texture2D.LoadImage(bytes);
		Icon.mainTexture = texture2D;
	}

	private void OnClick()
	{
		Application.OpenURL("https://youtu.be/" + id);
	}
}
