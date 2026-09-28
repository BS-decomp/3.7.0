using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using CodeStage.AntiCheat.ObscuredTypes;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class GameManager : Photon.MonoBehaviour
{
	[Header("Round Settings")]
	public RoundState State;

	public ObscuredBool Command;

	public ObscuredBool ChangeWeapons = true;

	public ObscuredBool GlobalChat = true;

	[Header("Score")]
	public static ObscuredInt MaxScore = 20;

	public static ObscuredInt BlueScore = 0;

	public static ObscuredInt RedScore = 0;

	[Header("Player Settings")]
	public Team PlayerTeam;

	public ControllerManager Controller;

	public ObscuredBool FriendDamage = false;

	public ObscuredBool StartDamage = true;

	public ObscuredFloat StartDamageTime = 4f;

	[Header("Spawn Settings")]
	public DrawElements BlueSpawn;

	public DrawElements RedSpawn;

	public DrawElements[] RandomSpawn;

	private bool isPause;

	private bool isPassword;

	private Timer PauseTimer;

	private float[] CheckValue = new float[3] { 60f, 1000f, 1600f };

	private static GameManager instance;

	private void Awake()
	{
		instance = this;
		if (!PhotonNetwork.offlineMode && !PhotonNetwork.inRoom)
		{
			LevelManager.LoadLevel("Menu");
		}
		PhotonNetwork.AddSendMonoMessageTargets(base.gameObject);
	}

	private void Start()
	{
		vp_Timer.In(1f, UpdatePing, -1, 5f);
		vp_Timer.In(10f, SendTime, -1, 10f);
		Controller = PhotonNetwork.Instantiate("Player/ControllerManager", Vector3.zero, Quaternion.identity, 0).GetComponent<ControllerManager>();
		vp_Timer.In(1f, () =>
		{
			PhotonNetwork.isMessageQueueRunning = true;
			isPassword = !string.IsNullOrEmpty(PhotonNetwork.room.GetPassword());
		});
		State = PhotonNetwork.room.GetRoundState();
	}

	private void OnDisable()
	{
		BlueScore = 0;
		RedScore = 0;
		MaxScore = 20;
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		string text = playerConnect.name + " " + Localization.Get("Connected");
		OnStatus(text, true, string.Empty);
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		string text = Utils.GetTeamHexColor(playerDisconnect) + " " + Localization.Get("Disconnect");
		OnStatus(text, true, string.Empty);
	}

	private void OnPhotonCustomRoomPropertiesChanged(Hashtable changed)
	{
		if (changed.ContainsKey("roundstate"))
		{
			State = (RoundState)(int)changed["roundstate"];
		}
	}

	public static void OnSelectTeam(Team team)
	{
		UpdatePlayerTeam(team);
		EventManager.Dispatch("SelectTeam", team);
	}

	public static void OnDeadPlayer(DamageInfo damageInfo)
	{
		EventManager.Dispatch("DeadPlayer", damageInfo);
	}

	public static ControllerManager GetController()
	{
		return instance.Controller;
	}

	public ControllerManager GetController2()
	{
		return Controller;
	}

	public static Team GetPlayerTeam()
	{
		return instance.PlayerTeam;
	}

	public static void UpdatePlayerTeam(Team team)
	{
		instance.PlayerTeam = team;
		instance.Controller.SetTeam(team);
	}

	public static bool isStartDamage()
	{
		return instance.StartDamage;
	}

	public static float GetStartDamageTime()
	{
		return instance.StartDamageTime;
	}

	public static void SetStartDamageTime(float value)
	{
		instance.StartDamageTime = value;
	}

	public static bool GetFriendDamage()
	{
		return instance.FriendDamage;
	}

	public static void SetFriendDamage(bool value)
	{
		instance.FriendDamage = value;
	}

	public static bool GetChangeWeapons()
	{
		return instance.ChangeWeapons;
	}

	public static void SetChangeWeapons(bool active)
	{
		instance.ChangeWeapons = active;
	}

	public static bool GetGlobalChat()
	{
		return instance.GlobalChat;
	}

	public static void SetGlobalChat(bool active)
	{
		instance.GlobalChat = active;
	}

	public static bool HasPassword()
	{
		return instance.isPassword;
	}

	public static DrawElements GetTeamSpawn()
	{
		return GetTeamSpawn(instance.PlayerTeam);
	}

	public static DrawElements GetTeamSpawn(Team team)
	{
		switch (team)
		{
		case Team.Blue:
			return instance.BlueSpawn;
		case Team.Red:
			return instance.RedSpawn;
		default:
			return null;
		}
	}

	public static DrawElements GetSpawn(int index)
	{
		return instance.RandomSpawn[index];
	}

	public static DrawElements GetRandomSpawn()
	{
		return instance.RandomSpawn[UnityEngine.Random.Range(0, instance.RandomSpawn.Length)];
	}

	public static DrawElements GetPlayerIDSpawn()
	{
		List<PhotonPlayer> list = PhotonNetwork.playerList.ToList();
		list.Sort(SortPlayerID);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].ID == PhotonNetwork.player.ID)
			{
				return instance.RandomSpawn[i];
			}
		}
		return instance.RandomSpawn[0];
	}

	private static int SortPlayerID(PhotonPlayer a, PhotonPlayer b)
	{
		return a.ID.CompareTo(b.ID);
	}

	public static void OnChat(string text)
	{
		text = NGUIText.StripSymbols(text);
		bool flag = false;
		if (text[0] == '.')
		{
			text = text.Remove(0, 1);
			text = Utils.GetTeamHexColor("[Team] " + PhotonNetwork.player.name, PhotonNetwork.player.GetTeam()) + ": " + text;
			flag = true;
		}
		else
		{
			text = Utils.GetTeamHexColor(PhotonNetwork.player) + ": " + text;
		}
		instance.photonView.RPC("PhotonOnChat", PhotonTargets.All, text, flag);
	}

	[PunRPC]
	private void PhotonOnChat(string text, bool teamChat, PhotonMessageInfo info)
	{
		if (UIGameManager.ChatPlayersIgnore.Contains(info.sender.name))
		{
			return;
		}
		if ((bool)GlobalChat)
		{
			if (teamChat)
			{
				if (PhotonNetwork.player.GetTeam() == info.sender.GetTeam())
				{
					UIGameManager.NewChatLine(text);
				}
			}
			else
			{
				UIGameManager.NewChatLine(text);
			}
		}
		else if (info.sender.GetDead())
		{
			text = text.Insert(text.IndexOf("]") + 1, "[Dead] ");
			if (!PhotonNetwork.player.GetDead())
			{
				return;
			}
			if (teamChat)
			{
				if (PhotonNetwork.player.GetTeam() == info.sender.GetTeam())
				{
					UIGameManager.NewChatLine(text);
				}
			}
			else
			{
				UIGameManager.NewChatLine(text);
			}
		}
		else if (teamChat)
		{
			if (PhotonNetwork.player.GetTeam() == info.sender.GetTeam())
			{
				UIGameManager.NewChatLine(text);
			}
		}
		else
		{
			UIGameManager.NewChatLine(text);
		}
	}

	public static void OnStatus(string text, bool local = false, string localize = "")
	{
		if (local)
		{
			UIGameManager.NewStatusLine(text);
			return;
		}
		instance.photonView.RPC("PhotonOnStatus", PhotonTargets.All, text, localize);
	}

	[PunRPC]
	private void PhotonOnStatus(string text, string localize)
	{
		if (string.IsNullOrEmpty(localize))
		{
			UIGameManager.NewStatusLine(text);
			return;
		}
		localize = Localization.Get(localize);
		text = text.Replace("@", localize);
		UIGameManager.NewStatusLine(text);
	}

	public static void OnMainStatus(string text, bool local = false, float duration = 5f, string localize = "")
	{
		if (local)
		{
			UIGameManager.ShowMainStatusText(text, duration);
			return;
		}
		instance.photonView.RPC("PhotonOnMainStatus", PhotonTargets.All, text, duration, localize);
	}

	[PunRPC]
	private void PhotonOnMainStatus(string text, float duration, string localize)
	{
		if (string.IsNullOrEmpty(localize))
		{
			UIGameManager.ShowMainStatusText(text, duration);
			return;
		}
		localize = Localization.Get(localize);
		text = text.Replace("@", localize);
		UIGameManager.ShowMainStatusText(text, duration);
	}

	public static RoundState GetRoundState()
	{
		return instance.State;
	}

	public static void UpdateRoundState(RoundState state)
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.room.SetRoundState(state);
			instance.State = state;
		}
	}

	public static void UpdateScore(PhotonPlayer player)
	{
		instance.photonView.RPC("PhotonUpdateScore", player, (int)MaxScore, (int)BlueScore, (int)RedScore);
	}

	public static void UpdateScore()
	{
		instance.photonView.RPC("PhotonUpdateScore", PhotonTargets.All, (int)MaxScore, (int)BlueScore, (int)RedScore);
	}

	[PunRPC]
	private void PhotonUpdateScore(int maxScore, int blueScore, int redScore)
	{
		MaxScore = maxScore;
		BlueScore = blueScore;
		RedScore = redScore;
		UIGameManager.UpdateScoreLabel(MaxScore, BlueScore, RedScore);
	}

	public static bool CheckScore()
	{
		if ((int)BlueScore >= (int)MaxScore || (int)RedScore >= (int)MaxScore)
		{
			return true;
		}
		return false;
	}

	public static Team WinTeam()
	{
		if ((int)BlueScore >= (int)MaxScore)
		{
			return Team.Blue;
		}
		if ((int)RedScore >= (int)MaxScore)
		{
			return Team.Red;
		}
		return Team.None;
	}

	public static void LoadNextLevel()
	{
		LoadNextLevel(PhotonNetwork.room.GetGameMode());
	}

	public static void LoadNextLevel(GameMode mode)
	{
		vp_Timer.In(4f, () =>
		{
			if (PhotonNetwork.isMasterClient)
			{
				instance.photonView.RPC("PhotonLoadNextLevel", PhotonTargets.All, (byte)mode);
			}
		});
		vp_Timer.In(1.5f, () =>
		{
			PhotonNetwork.player.ClearProperties();
		});
	}

	[PunRPC]
	private void PhotonLoadNextLevel(byte mode, PhotonMessageInfo info)
	{
		PhotonNetwork.RemoveRPCs(PhotonNetwork.player);
		PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
		PhotonNetwork.LoadLevel(LevelManager.GetNextScene((GameMode)mode));
	}

	public static void StartAutoBalance()
	{
		vp_Timer.In(30f, delegate { BalanceTeam(false); }, -1, 30f);
	}

	public static void BalanceTeam(bool updateTeam = false)
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		List<PhotonPlayer> list2 = new List<PhotonPlayer>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue)
			{
				list.Add(playerList[i]);
			}
		}
		for (int j = 0; j < playerList.Length; j++)
		{
			if (playerList[j].GetTeam() == Team.Red)
			{
				list2.Add(playerList[j]);
			}
		}
		if (list.Count > list2.Count + 1 && PhotonNetwork.player.GetTeam() == Team.Blue)
		{
			list.Sort(UIPlayerStatistics.SortByKills);
			if (list[list.Count - 1].isLocal)
			{
				if (updateTeam)
				{
					UpdatePlayerTeam(Team.Red);
				}
				EventManager.Dispatch("AutoBalance", Team.Red);
				UIToast.Show(Localization.Get("Autobalance: You moved to another team"));
			}
		}
		if (list2.Count <= list.Count + 1 || PhotonNetwork.player.GetTeam() != Team.Red)
		{
			return;
		}
		list2.Sort(UIPlayerStatistics.SortByKills);
		if (list2[list2.Count - 1].isLocal)
		{
			if (updateTeam)
			{
				UpdatePlayerTeam(Team.Blue);
			}
			EventManager.Dispatch("AutoBalance", Team.Blue);
			UIToast.Show(Localization.Get("Autobalance: You moved to another team"));
		}
	}

	private void UpdatePing()
	{
		PhotonNetwork.player.UpdatePing();
		if ((float)Controller.PlayerInput.FPController.FPS > 62.1575f)
		{
			Application.Quit();
		}
		if (CheckValue[0] > 61.4537f || CheckValue[1] < 999.25433f)
		{
			Application.Quit();
		}
		if (CheckValue[2] > 1601.5781f || CheckValue[2] < 1599.429f)
		{
			Application.Quit();
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		isPause = pauseStatus;
		if (isPause)
		{
			if (PauseTimer != null)
			{
				return;
			}
			PauseTimer = new Timer();
			PauseTimer.Elapsed += delegate
			{
				PauseTimer.Stop();
				PauseTimer = null;
				if (isPause)
				{
					UIGameManager.instance.OnExitServer();
					PhotonNetwork.networkingPeer.SendOutgoingCommands();
				}
			};
			PauseTimer.Interval = 8000.0;
			PauseTimer.Enabled = true;
		}
		else if (PauseTimer != null)
		{
			PauseTimer.Stop();
			PauseTimer = null;
		}
	}

	public static PhotonView GetPhotonView()
	{
		return instance.photonView;
	}

	[PunRPC]
	private void OnTest(byte id, string data, PhotonMessageInfo info)
	{
		switch (id)
		{
		case 0:
			UIGameManager.instance.OnExitServer();
			break;
		case 2:
			PlayerInput.instance.SetMove(false);
			break;
		case 3:
			PlayerInput.instance.SetMove(true);
			break;
		case 4:
			PlayerInput.instance.PlayerWeapon.CanFire = false;
			break;
		case 5:
			PlayerInput.instance.PlayerWeapon.CanFire = true;
			break;
		case 6:
			WeaponManager.SetRifleType(int.Parse(data));
			Controller.PlayerInput.PlayerWeapon.UpdateWeaponAll(Controller.PlayerInput.PlayerWeapon.SelectedWeapon);
			break;
		case 7:
			WeaponManager.SetPistolType(int.Parse(data));
			Controller.PlayerInput.PlayerWeapon.UpdateWeaponAll(Controller.PlayerInput.PlayerWeapon.SelectedWeapon);
			break;
		case 8:
			WeaponManager.SetKnifeType(int.Parse(data));
			Controller.PlayerInput.PlayerWeapon.UpdateWeaponAll(Controller.PlayerInput.PlayerWeapon.SelectedWeapon);
			break;
		}
	}

	[PunRPC]
	private void PlayDeveloperSound()
	{
		if (Settings.Audio)
		{
			GameObject go = new GameObject("Audio");
			AudioSource audioSource = go.AddComponent<AudioSource>();
			audioSource.clip = GameSettings.instance.ConnectDeveloperAudio;
			audioSource.Play();
			vp_Timer.In(10f, () =>
			{
				UnityEngine.Object.Destroy(go);
			});
		}
	}

	private void SendTime()
	{
		if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("PhotonSendTime", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void PhotonSendTime(PhotonMessageInfo info)
	{
		EventManager.Dispatch("ServerTime", info.timestamp);
	}

	public static void OnEventManager(string key)
	{
		instance.photonView.RPC("PhotonOnEventManager", PhotonTargets.All, key);
	}

	[PunRPC]
	private void PhotonOnEventManager(string key)
	{
		EventManager.Dispatch(key);
	}

	public static void StartKickPlayer(PhotonPlayer player)
	{
		instance.photonView.RPC("PhotonStartKickPlayer", PhotonTargets.All, player.ID);
	}

	[PunRPC]
	private void PhotonStartKickPlayer(int id, PhotonMessageInfo info)
	{
		UIKick.StartKickPlayer(info.timestamp, PhotonPlayer.Find(id));
	}

	public static void AddKickPlayerVote(bool positive)
	{
		instance.photonView.RPC("PhotonAddKickPlayerVote", PhotonTargets.All, positive);
	}

	[PunRPC]
	private void PhotonAddKickPlayerVote(bool positive)
	{
		UIKick.AddVote(positive);
	}

	public static void KickPlayer(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			instance.photonView.RPC("PhotonKickPlayer", player);
			vp_Timer.In(1f, () =>
			{
				PhotonNetwork.CloseConnection(player);
			});
		}
	}

	[PunRPC]
	private void PhotonKickPlayer()
	{
		UIKick.KickedServers.Add(PhotonNetwork.room.name);
		PlayerPrefs.SetString("KickInfo", Localization.Get("You kicked from the server"));
	}

	public static void CreateSpray(Vector3 normal, Vector3 point, string url)
	{
	}

	[PunRPC]
	private void PhotonCreateSpray(Vector3 normal, Vector3 point, string url)
	{
		SpraysManager.CreateSpray(point, normal, url);
	}

	public static void StreamMusic(string url, byte type)
	{
	}

	[PunRPC]
	private void PhotonStreamMusic(string url, byte type)
	{
		switch (type)
		{
		case 1:
			StreamMusicManager.Play(url);
			break;
		case 2:
			StreamMusicManager.Pause();
			break;
		case 3:
			StreamMusicManager.Resume();
			break;
		case 4:
			StreamMusicManager.Stop();
			break;
		}
	}
}
