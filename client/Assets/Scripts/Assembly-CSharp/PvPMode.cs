using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using FreeJSON;
using Photon;
using UnityEngine;

public class PvPMode : Photon.MonoBehaviour
{
	public ObscuredInt MaxScore = 20;

	public static List<int> BluePlayers = new List<int>();

	public static List<int> RedPlayers = new List<int>();

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.PvP)
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		UIGameManager.SetActiveScore(true, 20);
		GameManager.SetStartDamageTime(1f);
		UIPanelManager.ShowPanel("Display");
		CameraManager.ActiveStaticCamera();
		if (PhotonNetwork.isMasterClient)
		{
			vp_Timer.In(0.5f, () =>
			{
				ActivationWaitPlayer();
			});
		}
		else
		{
			UISelectTeam.OnStart();
		}
		EventManager.AddListener<Team>("SelectTeam", OnSelectTeam);
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
	}

	private void OnSelectTeam(Team team)
	{
		UIPanelManager.ShowPanel("Display");
		if ((bool)GameManager.GetController().PlayerInput.Dead)
		{
			CameraManager.ActiveSpectateCamera();
		}
	}

	private void ActivationWaitPlayer()
	{
		EventManager.Dispatch("WaitPlayer");
		GameManager.UpdateRoundState(RoundState.WaitPlayer);
		GameManager.OnSelectTeam(Team.Blue);
		OnWaitPlayer();
		OnCreatePlayer(0);
	}

	private void OnWaitPlayer()
	{
		GameManager.OnStatus(Localization.Get("Waiting for other players"), true, string.Empty);
		vp_Timer.In(4f, () =>
		{
			if (GameManager.GetRoundState() == RoundState.WaitPlayer)
			{
				if (PhotonNetwork.playerList.Length <= 1)
				{
					OnWaitPlayer();
				}
				else
				{
					vp_Timer.In(4f, () =>
					{
						OnStartRound();
					});
				}
			}
		});
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.UpdateScore(playerConnect);
			if (GameManager.GetRoundState() != RoundState.WaitPlayer)
			{
				CheckPlayers();
			}
			if (UIGameManager.instance.isScoreTimer)
			{
				base.photonView.RPC("UpdateTimer", playerConnect, UIGameManager.instance.ScoreTimer - Time.time);
			}
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			CheckPlayers();
		}
	}

	private void OnStartRound()
	{
		DecalsManager.ClearBulletHoles();
		if (PhotonNetwork.playerList.Length <= 1)
		{
			ActivationWaitPlayer();
		}
		else
		{
			if (!PhotonNetwork.isMasterClient)
			{
				return;
			}
			PhotonPlayer[] playerList = PhotonNetwork.playerList;
			List<PhotonPlayer> list = new List<PhotonPlayer>();
			List<PhotonPlayer> list2 = new List<PhotonPlayer>();
			for (int i = 0; i < playerList.Length; i++)
			{
				if (playerList[i].GetTeam() == Team.Blue)
				{
					list.Add(playerList[i]);
				}
				else if (playerList[i].GetTeam() == Team.Red)
				{
					list2.Add(playerList[i]);
				}
			}
			list.Sort(UIPlayerStatistics.SortByKills);
			list2.Sort(UIPlayerStatistics.SortByKills);
			JsonArray jsonArray = new JsonArray();
			JsonArray jsonArray2 = new JsonArray();
			foreach (PhotonPlayer item in list)
			{
				jsonArray.Add(item.ID);
			}
			foreach (PhotonPlayer item2 in list2)
			{
				jsonArray2.Add(item2.ID);
			}
			JsonObject jsonObject = new JsonObject();
			jsonObject.Add("Blue", jsonArray);
			jsonObject.Add("Red", jsonArray2);
			GameManager.UpdateRoundState(RoundState.PlayRound);
			base.photonView.RPC("StartTimer", PhotonTargets.All, jsonObject.ToString());
		}
	}

	[PunRPC]
	private void StartTimer(string jsonString, PhotonMessageInfo info)
	{
		JsonObject jsonObject = JsonObject.Parse(jsonString);
		BluePlayers = jsonObject.Get<List<int>>("Blue");
		RedPlayers = jsonObject.Get<List<int>>("Red");
		JsonArray jsonArray = jsonObject.Get<JsonArray>((PhotonNetwork.player.GetTeam() != Team.Red) ? "Blue" : "Red");
		int spawn = 0;
		for (int i = 0; i < jsonArray.Length; i++)
		{
			if (PhotonNetwork.player.ID == jsonArray.Get<int>(i))
			{
				spawn = ((PhotonNetwork.player.GetTeam() != Team.Blue) ? (i * 2 + 1) : (i * 2));
				break;
			}
		}
		float num = 20f;
		num -= (float)(PhotonNetwork.time - info.timestamp);
		UIGameManager.StartScoreTimer(num, StopTimer);
		OnCreatePlayer(spawn);
	}

	private void OnCreatePlayer(int spawn)
	{
		if (PhotonNetwork.player.GetTeam() != Team.None)
		{
			PlayerInput playerInput = GameManager.GetController().PlayerInput;
			playerInput.SetHealth(100);
			CameraManager.DeactiveAll();
			GameManager.GetController().ActivePlayer(GameManager.GetSpawn(spawn).GetSpawnPosition(), GameManager.GetSpawn(spawn).GetSpawnRotation());
			playerInput.PlayerWeapon.UpdateWeaponAll(WeaponTypeList.Rifle);
		}
	}

	[PunRPC]
	private void UpdateTimer(float time, PhotonMessageInfo info)
	{
		vp_Timer.In(1.5f, () =>
		{
			time -= (float)(PhotonNetwork.time - info.timestamp);
			UIGameManager.StartScoreTimer(time, StopTimer);
		});
	}

	private void StopTimer()
	{
		if (!PhotonNetwork.isMasterClient || GameManager.GetRoundState() == RoundState.EndRound)
		{
			return;
		}
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue && !playerList[i].GetDead())
			{
				num++;
			}
			else if (playerList[i].GetTeam() == Team.Red && !playerList[i].GetDead())
			{
				num2++;
			}
		}
		if (num > num2)
		{
			++GameManager.BlueScore;
			GameManager.UpdateScore();
			GameManager.OnMainStatus("@", false, 5f, "Blue Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
		else if (num < num2)
		{
			++GameManager.RedScore;
			GameManager.UpdateScore();
			GameManager.OnMainStatus("@", false, 5f, "Red Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
		else
		{
			GameManager.UpdateScore();
			GameManager.OnMainStatus("@", false, 5f, "Draw");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		PlayerRoundManager.SetDeaths1();
		GameManager.OnStatus(Utils.KillerStatus(damageInfo), false, string.Empty);
		Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.GetController().PlayerInput.PlayerTransform.position, damageInfo.AttackPosition);
		CameraManager.ActiveDeadCamera(GameManager.GetController().PlayerInput.FPCamera.Transform.position, GameManager.GetController().PlayerInput.FPCamera.Transform.eulerAngles, ragdollForce * 100f);
		GameManager.GetController().DeactivePlayer(ragdollForce, damageInfo.HeadShot);
		base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.PlayerID), damageInfo);
		base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
		UIDeathScreen.Show(damageInfo);
		GameManager.BalanceTeam(true);
		vp_Timer.In(3f, () =>
		{
			if ((bool)GameManager.GetController().PlayerInput.Dead)
			{
				CameraManager.ActiveSpectateCamera();
			}
		});
	}

	public static int SortByPvP(PhotonPlayer a, PhotonPlayer b)
	{
		List<int> list = ((a.GetTeam() != Team.Blue) ? RedPlayers : BluePlayers);
		int num = 10;
		int value = 10;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == a.ID)
			{
				num = i;
			}
			if (list[i] == b.ID)
			{
				value = i;
			}
		}
		return num.CompareTo(value);
	}

	[PunRPC]
	private void OnKilledPlayer(DamageInfo damageInfo)
	{
		EventManager.Dispatch("KillPlayer", damageInfo);
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetKills1();
		AchievementsManager.UpdateKills(damageInfo);
		if (damageInfo.HeadShot)
		{
			PlayerRoundManager.SetXP(12);
			PlayerRoundManager.SetMoney(10);
			PlayerRoundManager.SetHeadshot1();
		}
		else
		{
			PlayerRoundManager.SetXP(6);
			PlayerRoundManager.SetMoney(5);
		}
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessageInfo info)
	{
		UIGameManager.StopScoreTimer();
		GameManager.UpdateRoundState(RoundState.EndRound);
		GameManager.BalanceTeam(true);
		if (GameManager.CheckScore())
		{
			GameManager.LoadNextLevel(GameMode.PvP);
			return;
		}
		float delay = 8f - (float)(PhotonNetwork.time - info.timestamp);
		vp_Timer.In(delay, () =>
		{
			OnStartRound();
		});
	}

	[PunRPC]
	private void CheckPlayers()
	{
		if (!PhotonNetwork.isMasterClient || GameManager.GetRoundState() == RoundState.EndRound)
		{
			return;
		}
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue && !playerList[i].GetDead())
			{
				flag = true;
				break;
			}
		}
		for (int j = 0; j < playerList.Length; j++)
		{
			if (playerList[j].GetTeam() == Team.Red && !playerList[j].GetDead())
			{
				flag2 = true;
				break;
			}
		}
		if (!flag)
		{
			++GameManager.RedScore;
			GameManager.UpdateScore();
			GameManager.OnMainStatus("@", false, 5f, "Red Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
		else if (!flag2)
		{
			++GameManager.BlueScore;
			GameManager.UpdateScore();
			GameManager.OnMainStatus("@", false, 5f, "Blue Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}
}
