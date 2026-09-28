using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using MovementEffects;
using UnityEngine;

public class Firebase
{
	[CompilerGenerated]
	private sealed class _003CGetValueCoroutine_003Ec__Iterator29 : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal string url;

		internal WWW _003Cwww_003E__0;

		internal Action<string> success;

		internal Action<string> failed;

		internal int _0024PC;

		internal float _0024current;

		internal string _003C_0024_003Eurl;

		internal Action<string> _003C_0024_003Esuccess;

		internal Action<string> _003C_0024_003Efailed;

		internal Firebase _003C_003Ef__this;

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
				_003Cwww_003E__0 = new WWW(url);
				_0024current = Timing.WaitUntilDone(_003Cwww_003E__0);
				_0024PC = 1;
				return true;
			case 1u:
				if (string.IsNullOrEmpty(_003Cwww_003E__0.error))
				{
					if (success != null)
					{
						success(_003Cwww_003E__0.text);
					}
					if (FirebaseManager.DebugAction)
					{
						UnityEngine.Debug.Log("OnGetSuccess");
						UnityEngine.Debug.Log("Firebase: " + _003C_003Ef__this.FullURL);
						UnityEngine.Debug.Log("Json: " + _003Cwww_003E__0.text);
					}
				}
				else
				{
					if (failed != null)
					{
						failed(_003Cwww_003E__0.error);
					}
					if (FirebaseManager.DebugAction)
					{
						UnityEngine.Debug.Log("OnGetFailed");
						UnityEngine.Debug.Log("Firebase: " + _003C_003Ef__this.FullURL);
						UnityEngine.Debug.Log("Json: " + _003Cwww_003E__0.error);
					}
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

	[CompilerGenerated]
	private sealed class _003CSetValueCoroutine_003Ec__Iterator2A : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal Dictionary<string, string> _003Cdictionary_003E__0;

		internal string json;

		internal byte[] _003Cbytes_003E__1;

		internal string url;

		internal WWW _003Cwww_003E__2;

		internal Action<string> success;

		internal Action<string> failed;

		internal int _0024PC;

		internal float _0024current;

		internal string _003C_0024_003Ejson;

		internal string _003C_0024_003Eurl;

		internal Action<string> _003C_0024_003Esuccess;

		internal Action<string> _003C_0024_003Efailed;

		internal Firebase _003C_003Ef__this;

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
				_003Cdictionary_003E__0 = new Dictionary<string, string>();
				_003Cdictionary_003E__0.Add("Content-Type", "application/json");
				_003Cdictionary_003E__0.Add("X-HTTP-Method-Override", "PUT");
				_003Cbytes_003E__1 = Encoding.UTF8.GetBytes(json);
				_003Cwww_003E__2 = new WWW(url, _003Cbytes_003E__1, _003Cdictionary_003E__0);
				_0024current = Timing.WaitUntilDone(_003Cwww_003E__2);
				_0024PC = 1;
				return true;
			case 1u:
				if (string.IsNullOrEmpty(_003Cwww_003E__2.error))
				{
					if (success != null)
					{
						success(_003Cwww_003E__2.text);
					}
					if (FirebaseManager.DebugAction)
					{
						UnityEngine.Debug.Log("OnSetSuccess");
						UnityEngine.Debug.Log("Firebase: " + _003C_003Ef__this.FullURL);
						UnityEngine.Debug.Log("Json: " + _003Cwww_003E__2.text);
					}
				}
				else
				{
					if (failed != null)
					{
						failed(_003Cwww_003E__2.error);
					}
					if (FirebaseManager.DebugAction)
					{
						UnityEngine.Debug.Log("OnSetFailed");
						UnityEngine.Debug.Log("Firebase: " + _003C_003Ef__this.FullURL);
						UnityEngine.Debug.Log("Json: " + _003Cwww_003E__2.error);
					}
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

	[CompilerGenerated]
	private sealed class _003CUpdateValueCoroutine_003Ec__Iterator2B : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal Dictionary<string, string> _003Cdictionary_003E__0;

		internal string json;

		internal byte[] _003Cbytes_003E__1;

		internal string url;

		internal WWW _003Cwww_003E__2;

		internal Action<string> success;

		internal Action<string> failed;

		internal int _0024PC;

		internal float _0024current;

		internal string _003C_0024_003Ejson;

		internal string _003C_0024_003Eurl;

		internal Action<string> _003C_0024_003Esuccess;

		internal Action<string> _003C_0024_003Efailed;

		internal Firebase _003C_003Ef__this;

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
				_003Cdictionary_003E__0 = new Dictionary<string, string>();
				_003Cdictionary_003E__0.Add("Content-Type", "application/json");
				_003Cdictionary_003E__0.Add("X-HTTP-Method-Override", "PATCH");
				_003Cbytes_003E__1 = Encoding.UTF8.GetBytes(json);
				_003Cwww_003E__2 = new WWW(url, _003Cbytes_003E__1, _003Cdictionary_003E__0);
				_0024current = Timing.WaitUntilDone(_003Cwww_003E__2);
				_0024PC = 1;
				return true;
			case 1u:
				if (string.IsNullOrEmpty(_003Cwww_003E__2.error))
				{
					if (success != null)
					{
						success(_003Cwww_003E__2.text);
					}
					if (FirebaseManager.DebugAction)
					{
						UnityEngine.Debug.Log("OnUpdateSuccess");
						UnityEngine.Debug.Log("Firebase: " + _003C_003Ef__this.FullURL);
						UnityEngine.Debug.Log("Json: " + _003Cwww_003E__2.text);
					}
				}
				else
				{
					if (failed != null)
					{
						failed(_003Cwww_003E__2.error);
					}
					if (FirebaseManager.DebugAction)
					{
						UnityEngine.Debug.Log("OnUpdateFailed");
						UnityEngine.Debug.Log("Firebase: " + _003C_003Ef__this.FullURL);
						UnityEngine.Debug.Log("Json: " + _003Cwww_003E__2.error);
					}
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

	[CompilerGenerated]
	private sealed class _003CDeleteCoroutine_003Ec__Iterator2C : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal Dictionary<string, string> _003Cdictionary_003E__0;

		internal byte[] _003Cbytes_003E__1;

		internal string url;

		internal WWW _003Cwww_003E__2;

		internal Action<string> success;

		internal Action<string> failed;

		internal int _0024PC;

		internal float _0024current;

		internal string _003C_0024_003Eurl;

		internal Action<string> _003C_0024_003Esuccess;

		internal Action<string> _003C_0024_003Efailed;

		internal Firebase _003C_003Ef__this;

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
				_003Cdictionary_003E__0 = new Dictionary<string, string>();
				_003Cdictionary_003E__0.Add("Content-Type", "application/json");
				_003Cdictionary_003E__0.Add("X-HTTP-Method-Override", "DELETE");
				_003Cbytes_003E__1 = Encoding.GetEncoding("iso-8859-1").GetBytes("{ \"dummy\" : \"dummies\"}");
				_003Cwww_003E__2 = new WWW(url, _003Cbytes_003E__1, _003Cdictionary_003E__0);
				_0024current = Timing.WaitUntilDone(_003Cwww_003E__2);
				_0024PC = 1;
				return true;
			case 1u:
				if (string.IsNullOrEmpty(_003Cwww_003E__2.error))
				{
					if (success != null)
					{
						success(_003Cwww_003E__2.text);
					}
					if (FirebaseManager.DebugAction)
					{
						UnityEngine.Debug.Log("OnDeleteSuccess");
						UnityEngine.Debug.Log("Firebase: " + _003C_003Ef__this.FullURL);
						UnityEngine.Debug.Log("Json: " + _003Cwww_003E__2.text);
					}
				}
				else
				{
					if (failed != null)
					{
						failed(_003Cwww_003E__2.error);
					}
					if (FirebaseManager.DebugAction)
					{
						UnityEngine.Debug.Log("OnDeleteFailed");
						UnityEngine.Debug.Log("Firebase: " + _003C_003Ef__this.FullURL);
						UnityEngine.Debug.Log("Json: " + _003Cwww_003E__2.error);
					}
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

	public string Key;

	public string Auth;

	public string FullKey;

	public Firebase Parent;

	public string DataBase;

	public string FullURL
	{
		get
		{
			return "https://" + DataBase + FullKey + ".json";
		}
	}

	public Firebase()
	{
		Key = string.Empty;
		FullKey = string.Empty;
		Parent = null;
		DataBase = AesEncryptor.DecryptString("0j21ELYPwSENgcrPovQzmT/aWMcs1klonoCCQoeHBmwevovKpvc9bJILeCUNAWIKGBAsdnZkHQ6xXtbMpfDHGw==");
	}

	public Firebase(string databaseURL)
	{
		DataBase = databaseURL;
	}

	public Firebase(string databaseURL, string auth)
	{
		DataBase = databaseURL;
		Auth = auth;
	}

	private Firebase(Firebase parent, string key, string auth)
	{
		Parent = parent;
		Key = key;
		Auth = auth;
		FullKey = parent.FullKey + "/" + key;
		DataBase = parent.DataBase;
	}

	public Firebase Child(string key)
	{
		return new Firebase(this, key, Auth);
	}

	public Firebase Copy()
	{
		Firebase firebase = new Firebase();
		firebase.Key = Key;
		firebase.Auth = Auth;
		firebase.FullKey = FullKey;
		firebase.Parent = Parent;
		firebase.DataBase = DataBase;
		return firebase;
	}

	public void SetTimeStamp(string key)
	{
		Child(key).SetValue(GetTimeStamp());
	}

	public static string GetTimeStamp()
	{
		return "{\".sv\": \"timestamp\"}";
	}

	public void GetValue()
	{
		GetValue(string.Empty, null, null);
	}

	public void GetValue(Action<string> success, Action<string> failed)
	{
		GetValue(string.Empty, success, failed);
	}

	public void GetValue(FirebaseParam param)
	{
		GetValue(param.ToString(), null, null);
	}

	public void GetValue(FirebaseParam param, Action<string> success, Action<string> failed)
	{
		GetValue(param.ToString(), success, failed);
	}

	public void GetValue(string param, Action<string> success, Action<string> failed)
	{
		if (!string.IsNullOrEmpty(Auth))
		{
			param = new FirebaseParam(param).Auth(Auth).ToString();
		}
		string text = FullURL;
		param = WWW.EscapeURL(param);
		if (!string.IsNullOrEmpty(param))
		{
			text = text + "?" + param;
		}
		Timing.RunCoroutine(GetValueCoroutine(text, success, failed));
	}

	[DebuggerHidden]
	private IEnumerator<float> GetValueCoroutine(string url, Action<string> success, Action<string> failed)
	{
		//yield-return decompiler failed: Could not find currentField
		_003CGetValueCoroutine_003Ec__Iterator29 obj = new _003CGetValueCoroutine_003Ec__Iterator29();
		obj.url = url;
		obj.success = success;
		obj.failed = failed;
		obj._003C_0024_003Eurl = url;
		obj._003C_0024_003Esuccess = success;
		obj._003C_0024_003Efailed = failed;
		obj._003C_003Ef__this = this;
		return obj;
	}

	public void SetValue(string json)
	{
		SetValue(json, string.Empty, null, null);
	}

	public void SetValue(string json, Action<string> success, Action<string> failed)
	{
		SetValue(json, string.Empty, success, failed);
	}

	public void SetValue(string json, FirebaseParam param)
	{
		SetValue(json, param.ToString(), null, null);
	}

	public void SetValue(string json, FirebaseParam param, Action<string> success, Action<string> failed)
	{
		SetValue(json, param.ToString(), success, failed);
	}

	public void SetValue(string json, string param, Action<string> success, Action<string> failed)
	{
		if (!string.IsNullOrEmpty(Auth))
		{
			param = new FirebaseParam(param).Auth(Auth).ToString();
		}
		string text = FullURL;
		param = WWW.EscapeURL(param);
		if (!string.IsNullOrEmpty(param))
		{
			text = text + "?" + param;
		}
		Timing.RunCoroutine(SetValueCoroutine(text, json, success, failed));
	}

	[DebuggerHidden]
	private IEnumerator<float> SetValueCoroutine(string url, string json, Action<string> success, Action<string> failed)
	{
		//yield-return decompiler failed: Could not find currentField
		_003CSetValueCoroutine_003Ec__Iterator2A obj = new _003CSetValueCoroutine_003Ec__Iterator2A();
		obj.json = json;
		obj.url = url;
		obj.success = success;
		obj.failed = failed;
		obj._003C_0024_003Ejson = json;
		obj._003C_0024_003Eurl = url;
		obj._003C_0024_003Esuccess = success;
		obj._003C_0024_003Efailed = failed;
		obj._003C_003Ef__this = this;
		return obj;
	}

	public void UpdateValue(string json)
	{
		UpdateValue(json, string.Empty, null, null);
	}

	public void UpdateValue(string json, Action<string> success, Action<string> failed)
	{
		UpdateValue(json, string.Empty, success, failed);
	}

	public void UpdateValue(string json, FirebaseParam param)
	{
		UpdateValue(json, param.ToString(), null, null);
	}

	public void UpdateValue(string json, FirebaseParam param, Action<string> success, Action<string> failed)
	{
		UpdateValue(json, param.ToString(), success, failed);
	}

	public void UpdateValue(string json, string param, Action<string> success, Action<string> failed)
	{
		if (!string.IsNullOrEmpty(Auth))
		{
			param = new FirebaseParam(param).Auth(Auth).ToString();
		}
		string text = FullURL;
		param = WWW.EscapeURL(param);
		if (!string.IsNullOrEmpty(param))
		{
			text = text + "?" + param;
		}
		Timing.RunCoroutine(UpdateValueCoroutine(text, json, success, failed));
	}

	[DebuggerHidden]
	private IEnumerator<float> UpdateValueCoroutine(string url, string json, Action<string> success, Action<string> failed)
	{
		//yield-return decompiler failed: Could not find currentField
		_003CUpdateValueCoroutine_003Ec__Iterator2B obj = new _003CUpdateValueCoroutine_003Ec__Iterator2B();
		obj.json = json;
		obj.url = url;
		obj.success = success;
		obj.failed = failed;
		obj._003C_0024_003Ejson = json;
		obj._003C_0024_003Eurl = url;
		obj._003C_0024_003Esuccess = success;
		obj._003C_0024_003Efailed = failed;
		obj._003C_003Ef__this = this;
		return obj;
	}

	public void Delete()
	{
		Delete(string.Empty, null, null);
	}

	public void Delete(Action<string> success, Action<string> failed)
	{
		Delete(string.Empty, success, failed);
	}

	public void Delete(FirebaseParam param)
	{
		Delete(param.ToString(), null, null);
	}

	public void Delete(FirebaseParam param, Action<string> success, Action<string> failed)
	{
		Delete(param.ToString(), success, failed);
	}

	public void Delete(string param, Action<string> success, Action<string> failed)
	{
		if (!string.IsNullOrEmpty(Auth))
		{
			param = new FirebaseParam(param).Auth(Auth).ToString();
		}
		string text = FullURL;
		param = WWW.EscapeURL(param);
		if (!string.IsNullOrEmpty(param))
		{
			text = text + "?" + param;
		}
		Timing.RunCoroutine(DeleteCoroutine(text, success, failed));
	}

	[DebuggerHidden]
	private IEnumerator<float> DeleteCoroutine(string url, Action<string> success, Action<string> failed)
	{
		//yield-return decompiler failed: Could not find currentField
		_003CDeleteCoroutine_003Ec__Iterator2C obj = new _003CDeleteCoroutine_003Ec__Iterator2C();
		obj.url = url;
		obj.success = success;
		obj.failed = failed;
		obj._003C_0024_003Eurl = url;
		obj._003C_0024_003Esuccess = success;
		obj._003C_0024_003Efailed = failed;
		obj._003C_003Ef__this = this;
		return obj;
	}
}
