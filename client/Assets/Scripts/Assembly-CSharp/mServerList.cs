using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MovementEffects;
using UnityEngine;

public class mServerList : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CCreateServerList_003Ec__Iterator23 : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal bool updateRoomList;

		internal int _003Ccount_003E__0;

		internal int _003Ci_003E__1;

		internal int _003CstartIndex_003E__2;

		internal int _003CmaxLength_003E__3;

		internal int _003Ci_003E__4;

		internal Transform _003Celement_003E__5;

		internal int _003Ci_003E__6;

		internal Transform _003Celement_003E__7;

		internal int _0024PC;

		internal float _0024current;

		internal bool _003C_0024_003EupdateRoomList;

		internal mServerList _003C_003Ef__this;

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
				_003C_003Ef__this.isCreatingServerList = true;
				_0024current = Timing.WaitForSeconds(0.01f);
				_0024PC = 1;
				break;
			case 1u:
				_003C_003Ef__this.ClearServerList();
				_003C_003Ef__this.MaxPlayers = 0;
				_003C_003Ef__this.MaxServers = 0;
				if (updateRoomList)
				{
					_003C_003Ef__this.RoomList = PhotonNetwork.GetRoomList();
				}
				_003Ccount_003E__0 = -1;
				for (_003Ci_003E__1 = 0; _003Ci_003E__1 < _003C_003Ef__this.RoomList.Length; _003Ci_003E__1++)
				{
					if (_003C_003Ef__this.SelectMode == -1 || (_003C_003Ef__this.SelectMode != -1 && _003C_003Ef__this.SelectMode == (int)_003C_003Ef__this.RoomList[_003Ci_003E__1].GetGameMode()))
					{
						_003C_003Ef__this.MaxServers++;
						_003C_003Ef__this.MaxPlayers += _003C_003Ef__this.RoomList[_003Ci_003E__1].playerCount;
					}
				}
				_003C_003Ef__this.UpdateServerInfo();
				if (_003C_003Ef__this.SelectMode == -1 && _003C_003Ef__this.RoomList.Length / 30 >= 1)
				{
					if (_003C_003Ef__this.MaxAllModeList == 0)
					{
						_003C_003Ef__this.MaxAllModeList = Mathf.CeilToInt((float)_003C_003Ef__this.RoomList.Length / 30f);
					}
					_003C_003Ef__this.ServerListSwitch.SetActive(true);
					_003C_003Ef__this.ServerListSwitch.transform.localPosition = Vector3.up * 150f;
					_003CstartIndex_003E__2 = (_003C_003Ef__this.SelectAllModeList - 1) * 30;
					_003CmaxLength_003E__3 = 30;
					if (_003C_003Ef__this.SelectAllModeList * 30 > _003C_003Ef__this.RoomList.Length)
					{
						_003CmaxLength_003E__3 = _003C_003Ef__this.RoomList.Length - (_003C_003Ef__this.SelectAllModeList - 1) * 30;
					}
					_003C_003Ef__this.ServerListSwitchLabel.text = _003CstartIndex_003E__2 + 1 + "-" + (_003CstartIndex_003E__2 + _003CmaxLength_003E__3);
					_003C_003Ef__this.ServerListSwitchLabel2.text = _003CstartIndex_003E__2 + 1 + "-" + (_003CstartIndex_003E__2 + _003CmaxLength_003E__3);
					_003Ci_003E__4 = _003CstartIndex_003E__2;
					goto IL_0393;
				}
				_003C_003Ef__this.ServerListSwitch.SetActive(false);
				_003C_003Ef__this.ServerListSwitch2.SetActive(false);
				_003Ci_003E__6 = 0;
				goto IL_0511;
			case 2u:
				_003Ci_003E__4++;
				goto IL_0393;
			case 3u:
				_003Ci_003E__6++;
				goto IL_0511;
			default:
				{
					return false;
				}
				IL_0393:
				if (_003Ci_003E__4 < _003CstartIndex_003E__2 + _003CmaxLength_003E__3)
				{
					_003Celement_003E__5 = _003C_003Ef__this.GetElement();
					_003Ccount_003E__0++;
					_003Celement_003E__5.GetComponent<mServerInfo>().SetData(_003C_003Ef__this.RoomList[_003Ci_003E__4]);
					_003Celement_003E__5.localPosition = Vector3.up * (102 - 48 * _003Ccount_003E__0);
					_003Celement_003E__5.gameObject.SetActive(true);
					_003C_003Ef__this.ServerList.Add(_003Celement_003E__5.gameObject);
					_0024current = Timing.WaitForSeconds(0.01f);
					_0024PC = 2;
					break;
				}
				_003C_003Ef__this.ServerListSwitch2.SetActive(true);
				_003C_003Ef__this.ServerListSwitch2.transform.localPosition = Vector3.up * (102 - 48 * (_003Ccount_003E__0 + 1));
				goto IL_0529;
				IL_0511:
				if (_003Ci_003E__6 < _003C_003Ef__this.RoomList.Length)
				{
					if (_003C_003Ef__this.SelectMode == -1 || _003C_003Ef__this.SelectMode == (int)_003C_003Ef__this.RoomList[_003Ci_003E__6].GetGameMode())
					{
						_003Celement_003E__7 = _003C_003Ef__this.GetElement();
						_003Ccount_003E__0++;
						_003Celement_003E__7.GetComponent<mServerInfo>().SetData(_003C_003Ef__this.RoomList[_003Ci_003E__6]);
						_003Celement_003E__7.localPosition = Vector3.up * (150 - 48 * _003Ccount_003E__0);
						_003Celement_003E__7.gameObject.SetActive(true);
						_003C_003Ef__this.ServerList.Add(_003Celement_003E__7.gameObject);
						_0024current = Timing.WaitForSeconds(0.01f);
						_0024PC = 3;
						break;
					}
					goto case 3u;
				}
				goto IL_0529;
				IL_0529:
				_003C_003Ef__this.isCreatingServerList = false;
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
	}

	private int SelectMode = -1;

	public UILabel ServerInfoLabel;

	public UIPopupList ModePopupList;

	public GameObject ServerListElement;

	public GameObject ServerListSwitch;

	public UILabel ServerListSwitchLabel;

	public GameObject ServerListSwitch2;

	public UILabel ServerListSwitchLabel2;

	public GameObject ServerListParent;

	private List<GameObject> ServerList = new List<GameObject>();

	private List<GameObject> ServerListPool = new List<GameObject>();

	private bool isCreatingServerList;

	private int MaxPlayers;

	private int MaxServers;

	private RoomInfo[] RoomList;

	private int SelectAllModeList = 1;

	private int MaxAllModeList;

	public void Open()
	{
		ModePopupList.Clear();
		ModePopupList.AddItem("All", -1);
		for (int i = 0; i < Enum.GetValues(typeof(GameMode)).Length; i++)
		{
			ModePopupList.AddItem(((GameMode)i).ToString(), i);
		}
		ModePopupList.value = "All";
	}

	public void UpdateServerList()
	{
		MaxAllModeList = 0;
		SelectAllModeList = 1;
		if (!isCreatingServerList)
		{
			ServerListParent.GetComponent<UIScrollView>().ResetPosition();
			Timing.RunCoroutine(CreateServerList(true), "CreateServerList");
		}
	}

	[DebuggerHidden]
	private IEnumerator<float> CreateServerList(bool updateRoomList)
	{
		//yield-return decompiler failed: Could not find currentField
		_003CCreateServerList_003Ec__Iterator23 obj = new _003CCreateServerList_003Ec__Iterator23();
		obj.updateRoomList = updateRoomList;
		obj._003C_0024_003EupdateRoomList = updateRoomList;
		obj._003C_003Ef__this = this;
		return obj;
	}

	private Transform GetElement()
	{
		GameObject gameObject = null;
		if (ServerListPool.Count != 0)
		{
			gameObject = ServerListPool[0];
			ServerListPool.RemoveAt(0);
		}
		else
		{
			gameObject = NGUITools.AddChild(ServerListParent, ServerListElement);
		}
		return gameObject.transform;
	}

	private void ClearServerList()
	{
		for (int i = 0; i < ServerList.Count; i++)
		{
			ServerList[i].SetActive(false);
			ServerListPool.Add(ServerList[i]);
		}
		ServerList.Clear();
	}

	private void UpdateServerInfo()
	{
		string text = Localization.Get("Players") + ": " + MaxPlayers + "\n" + Localization.Get("Servers") + ": " + MaxServers + "\n" + Localization.Get("Ping") + ": " + PhotonNetwork.GetPing();
		ServerInfoLabel.text = text;
	}

	public void OnSelectMode()
	{
		if (SelectMode != (int)ModePopupList.data)
		{
			SelectMode = (int)ModePopupList.data;
			MaxAllModeList = 0;
			SelectAllModeList = 1;
			if (isCreatingServerList)
			{
				isCreatingServerList = false;
				Timing.KillCoroutines("CreateServerList");
			}
			ServerListParent.GetComponent<UIScrollView>().ResetPosition();
			Timing.RunCoroutine(CreateServerList(true), "CreateServerList");
		}
	}

	public void RightSwitch()
	{
		SelectAllModeList++;
		if (SelectAllModeList > MaxAllModeList)
		{
			SelectAllModeList = 1;
		}
		if (isCreatingServerList)
		{
			isCreatingServerList = false;
			Timing.KillCoroutines("CreateServerList");
		}
		ServerListParent.GetComponent<UIScrollView>().ResetPosition();
		Timing.RunCoroutine(CreateServerList(false), "CreateServerList");
	}

	public void LeftSwitch()
	{
		SelectAllModeList--;
		if (SelectAllModeList <= 0)
		{
			SelectAllModeList = MaxAllModeList;
		}
		if (isCreatingServerList)
		{
			isCreatingServerList = false;
			Timing.KillCoroutines("CreateServerList");
		}
		ServerListParent.GetComponent<UIScrollView>().ResetPosition();
		Timing.RunCoroutine(CreateServerList(false));
	}
}
