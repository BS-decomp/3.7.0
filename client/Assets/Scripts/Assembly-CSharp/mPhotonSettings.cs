using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class mPhotonSettings : MonoBehaviour
{
	public int SelectRegion = -1;

	public string[] Regions;

	public static bool ConnectChat;

	private string SelectMap;

	private static mPhotonSettings instance;

	private bool newRegion;

	private void Awake()
	{
		PhotonNetwork.AddSendMonoMessageTargets(base.gameObject);
	}

	private void Start()
	{
		instance = this;
		PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.offlineMode = false;
	}

	public void OnConnectToPhoton(int sr)
	{
		if (instance.SelectRegion != sr)
		{
			if (PhotonNetwork.connected)
			{
				PhotonNetwork.Disconnect();
			}
			instance.SelectRegion = sr;
			mPopUp.ShowText(Localization.Get("Connecting") + "...");
			string bundleVersion = VersionManager.bundleVersion;
			string appID = AesEncryptor.DecryptString(GameSettings.instance.PhotonID);
			PhotonNetwork.ConnectToMaster(Regions[SelectRegion], 5055, appID, bundleVersion);
		}
		else
		{
			mPanelManager.ShowPanel("Server", true);
		}
	}

	public void OnConnectToPhotonBestRegion()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		if (PhotonNetwork.connected && !newRegion)
		{
			mPanelManager.ShowPanel("Server", true);
			return;
		}
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
		mPopUp.ShowText(Localization.Get("Connecting") + "...");
		string bundleVersion = VersionManager.bundleVersion;
		string appID = AesEncryptor.DecryptString(GameSettings.instance.PhotonID);
		PhotonNetwork.ConnectToBestCloudServer(bundleVersion, appID);
		newRegion = false;
	}

	public void OnSelectBestRegion(string region)
	{
		if (!(PlayerPrefs.GetString("SelectRegion") == region))
		{
			PlayerPrefs.SetString("SelectRegion", region);
			newRegion = true;
			mVersionManager.UpdateRegion();
		}
	}

	private void OnConnectedToPhoton()
	{
		mVersionManager.UpdateRegion();
		PhotonNetwork.playerName = AccountManager.AccountName;
		PhotonNetwork.player.SetLevel(AccountManager.GetLevel());
		mPopUp.HideAll("Server");
	}

	private void OnDisconnectedFromPhoton()
	{
		SelectRegion = -1;
		mPopUp.HideAll("Menu");
	}

	private void OnLeftRoom()
	{
		SelectRegion = -1;
	}

	private void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		UIToast.Show("Failed: " + cause);
	}

	private void OnConnectionFail(DisconnectCause cause)
	{
		UIToast.Show("Fail: " + cause);
	}

	public static void OnCreateServer()
	{
		ConnectChat = false;
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		mPopUp.ShowText(Localization.Get("Creating Server") + "...");
		PhotonNetwork.playerName = AccountManager.AccountName;
		PhotonNetwork.player.SetPlayerID(AccountManager.PlayerID);
		PhotonNetwork.player.ClearProperties();
		GameMode gameMode = mCreateServer.GetGameMode();
		string serverName = mCreateServer.GetServerName();
		int maxPlayers = mCreateServer.GetMaxPlayers();
		maxPlayers = Mathf.Clamp(maxPlayers, 4, 12);
		string password = mCreateServer.GetPassword();
		instance.SelectMap = mCreateServer.GetMap();
		Hashtable hashtable = PhotonNetwork.room.CreateRoomHashtable(password, gameMode);
		if (gameMode == GameMode.Only)
		{
			hashtable["onlyWeapon"] = mCreateServer.GetWeapon();
		}
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.maxPlayers = (byte)maxPlayers;
		roomOptions.isOpen = true;
		roomOptions.isVisible = true;
		roomOptions.customRoomProperties = hashtable;
		if (gameMode == GameMode.Only)
		{
			roomOptions.customRoomPropertiesForLobby = new string[4] { "curScn", "password", "mode", "onlyWeapon" };
		}
		else
		{
			roomOptions.customRoomPropertiesForLobby = new string[3] { "curScn", "password", "mode" };
		}
		PhotonNetwork.CreateRoom(serverName, roomOptions, null);
	}

	public static void OnJoinChat(string chatName)
	{
		ConnectChat = true;
		mPopUp.ShowText(Localization.Get("Please wait") + "...");
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.maxPlayers = 0;
		roomOptions.isOpen = true;
		roomOptions.isVisible = false;
		PhotonNetwork.JoinOrCreateRoom(chatName, roomOptions, null);
	}

	public static void OnQuickPlay(string mode, string map, int maxPlayers)
	{
		ConnectChat = false;
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		List<RoomInfo> list = new List<RoomInfo>();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (mode == "Any")
			{
				if (maxPlayers == 0)
				{
					if (roomList[i].playerCount != roomList[i].maxPlayers && roomList[i].GetPassword() == string.Empty)
					{
						list.Add(roomList[i]);
					}
				}
				else if (roomList[i].maxPlayers == maxPlayers && roomList[i].playerCount != roomList[i].maxPlayers && roomList[i].GetPassword() == string.Empty)
				{
					list.Add(roomList[i]);
				}
			}
			else
			{
				if (!(roomList[i].GetGameMode().ToString() == mode))
				{
					continue;
				}
				if (map == Localization.Get("Any"))
				{
					if (maxPlayers == 0)
					{
						if (roomList[i].playerCount != roomList[i].maxPlayers && roomList[i].GetPassword() == string.Empty)
						{
							list.Add(roomList[i]);
						}
					}
					else if (roomList[i].maxPlayers == maxPlayers && roomList[i].playerCount != roomList[i].maxPlayers && roomList[i].GetPassword() == string.Empty)
					{
						list.Add(roomList[i]);
					}
				}
				else
				{
					if (!(roomList[i].GetSceneName() == map))
					{
						continue;
					}
					if (maxPlayers == 0)
					{
						if (roomList[i].playerCount != roomList[i].maxPlayers && roomList[i].GetPassword() == string.Empty)
						{
							list.Add(roomList[i]);
						}
					}
					else if (roomList[i].maxPlayers == maxPlayers && roomList[i].playerCount != roomList[i].maxPlayers && roomList[i].GetPassword() == string.Empty)
					{
						list.Add(roomList[i]);
					}
				}
			}
		}
		if (list.Count != 0)
		{
			RoomInfo room = list[Random.Range(0, list.Count)];
			OnJoinServer(room);
			return;
		}
		mPopUp.HideAll("Server");
		mPopUp.ShowPopup(Localization.Get("The server with the selected data was not found. You want to create your own server?"), Localization.Get("Search Server"), Localization.Get("No"), () =>
		{
			mPopUp.HideAll("Server");
		}, Localization.Get("Yes"), () =>
		{
			mCreateServer.OpenPanel();
			mPopUp.HideAll("CreateServer");
		});
	}

	public void OnCreateServerOffline(string scene)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
		SelectMap = scene;
		PhotonNetwork.offlineMode = true;
		PhotonNetwork.CreateRoom(scene);
	}

	public static void OnJoinServer(RoomInfo room)
	{
		ConnectChat = false;
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account"));
			return;
		}
		mPopUp.ShowText(Localization.Get("Connecting") + "...");
		PhotonNetwork.playerName = AccountManager.AccountName;
		PhotonNetwork.player.SetPlayerID(AccountManager.PlayerID);
		PhotonNetwork.player.ClearProperties();
		instance.SelectMap = room.GetSceneName();
		PhotonNetwork.JoinRoom(room.name);
	}

	private void OnJoinedRoom()
	{
		if (PhotonNetwork.offlineMode)
		{
			mPopUp.ShowText(Localization.Get("Loading") + "...");
			LevelManager.LoadLevel(SelectMap);
		}
		else if (ConnectChat)
		{
			mPopUp.HideAll("ChatWindow", false);
		}
		else
		{
			mPopUp.ShowText(Localization.Get("Loading") + "...");
			PhotonNetwork.isMessageQueueRunning = false;
			PhotonNetwork.LoadLevel(SelectMap);
		}
	}

	private void OnPhotonJoinRoomFailed()
	{
		mPopUp.HideAll("ServerList", false);
		UIToast.Show(Localization.Get("The server is full"));
	}
}
