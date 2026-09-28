using System.Collections.Generic;
using Photon;
using UnityEngine;

public class HotKnifeMode : Photon.MonoBehaviour
{
	private int KnifeID;

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.HotKnife)
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		UIGameManager.SetActiveScore(true, 20);
		GameManager.SetStartDamageTime(1f);
		UIPanelManager.ShowPanel("Display");
		WeaponManager.MaxDamage = true;
		GameManager.SetChangeWeapons(false);
		GameManager.SetGlobalChat(false);
		CameraManager.ActiveStaticCamera();
		vp_Timer.In(0.5f, () =>
		{
			GameManager.OnSelectTeam(Team.Blue);
			if (PhotonNetwork.isMasterClient)
			{
				ActivationWaitPlayer();
			}
			else
			{
				CameraManager.ActiveSpectateCamera();
			}
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
	}

	private void ActivationWaitPlayer()
	{
		EventManager.Dispatch("WaitPlayer");
		GameManager.UpdateRoundState(RoundState.WaitPlayer);
		OnWaitPlayer();
		OnCreatePlayer(true);
		HitPlayer(-2);
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
		else if (PhotonNetwork.isMasterClient)
		{
			GameManager.UpdateRoundState(RoundState.PlayRound);
			int iD = PhotonNetwork.playerList[Random.Range(0, PhotonNetwork.playerList.Length)].ID;
			base.photonView.RPC("StartTimer", PhotonTargets.All, iD, true);
		}
	}

	[PunRPC]
	private void StartTimer(int knife, bool spawn, PhotonMessageInfo info)
	{
		UIGameManager.StopScoreTimer();
		KnifeID = knife;
		float num = 30f;
		num -= (float)(PhotonNetwork.time - info.timestamp);
		UIGameManager.StartScoreTimer(num, StopTimer);
		OnCreatePlayer(spawn);
	}

	private void StopTimer()
	{
		if (PhotonNetwork.player.ID == KnifeID)
		{
			PlayerInput playerInput = GameManager.GetController().PlayerInput;
			DamageInfo damageInfo = DamageInfo.Create(101, Vector3.zero, Team.Blue, playerInput.PlayerWeapon.GetSelectedWeaponData().WeaponID, PhotonNetwork.player.ID);
			playerInput.Damage(damageInfo);
		}
	}

	private void OnCreatePlayer(bool spawn)
	{
		if (PhotonNetwork.player.GetTeam() != Team.None)
		{
			PlayerInput playerInput = GameManager.GetController().PlayerInput;
			if (spawn)
			{
				playerInput.SetHealth(100);
				CameraManager.DeactiveAll();
				DrawElements playerIDSpawn = GameManager.GetPlayerIDSpawn();
				GameManager.GetController().ActivePlayer(playerIDSpawn.GetSpawnPosition(), playerIDSpawn.GetSpawnRotation());
				HitPlayer(KnifeID);
			}
			else if (!PhotonNetwork.player.GetDead())
			{
				HitPlayer(KnifeID);
			}
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		if (damageInfo.PlayerID == PhotonNetwork.player.ID)
		{
			PhotonNetwork.player.SetDeaths1();
			PlayerRoundManager.SetDeaths1();
			GameManager.OnStatus(Utils.KillerStatus(damageInfo), false, string.Empty);
			Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.GetController().PlayerInput.PlayerTransform.position, damageInfo.AttackPosition);
			CameraManager.ActiveDeadCamera(GameManager.GetController().PlayerInput.FPCamera.Transform.position, GameManager.GetController().PlayerInput.FPCamera.Transform.eulerAngles, ragdollForce * 100f);
			GameManager.GetController().DeactivePlayer(ragdollForce, damageInfo.HeadShot);
			base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
			vp_Timer.In(3f, () =>
			{
				if ((bool)GameManager.GetController().PlayerInput.Dead)
				{
					CameraManager.ActiveSpectateCamera();
				}
			});
		}
		else
		{
			GameManager.OnStatus("@: " + Utils.GetTeamHexColor(PhotonNetwork.player), false, "HotKnife");
			base.photonView.RPC("HitPlayer", PhotonTargets.All, PhotonNetwork.player.ID);
		}
	}

	[PunRPC]
	private void HitPlayer(int knife)
	{
		KnifeID = knife;
		if (PhotonNetwork.player.GetTeam() != Team.None && !PhotonNetwork.player.GetDead())
		{
			PlayerInput playerInput = GameManager.GetController().PlayerInput;
			playerInput.SetHealth(100);
			if (PhotonNetwork.player.ID == knife)
			{
				GameManager.OnSelectTeam(Team.Red);
				GameManager.GetController().SetTeam(Team.Red);
				WeaponManager.SetKnifeType(4);
				WeaponManager.SetPistolType(0);
				WeaponManager.SetRifleType(0);
				playerInput.PlayerWeapon.UpdateWeaponAll(WeaponTypeList.Knife);
			}
			else if (WeaponManager.HasKnifeType())
			{
				GameManager.OnSelectTeam(Team.Blue);
				GameManager.GetController().SetTeam(Team.Blue);
				WeaponManager.SetKnifeType(0);
				WeaponManager.SetPistolType(3);
				WeaponManager.SetRifleType(0);
				vp_Timer.In(0.1f, () =>
				{
					playerInput.PlayerWeapon.GetWeaponData(WeaponTypeList.Pistol).AmmoMax = 0;
					playerInput.PlayerWeapon.GetWeaponData(WeaponTypeList.Pistol).Ammo = 0;
					UIGameManager.SetAmmoLabel(playerInput.PlayerWeapon.GetSelectedWeaponData().Ammo, playerInput.PlayerWeapon.GetSelectedWeaponData().AmmoMax);
				});
				playerInput.PlayerWeapon.UpdateWeaponAll(WeaponTypeList.Pistol);
			}
		}
		if (PhotonNetwork.isMasterClient && !UIGameManager.ScopeTimer() && GameManager.GetRoundState() == RoundState.PlayRound)
		{
			CheckPlayers();
		}
	}

	[PunRPC]
	private void OnFinishRound(int id, PhotonMessageInfo info)
	{
		UIGameManager.StopScoreTimer();
		GameManager.UpdateRoundState(RoundState.EndRound);
		if (PhotonNetwork.player.ID == id)
		{
			PhotonNetwork.player.SetKills1();
			PlayerRoundManager.SetXP(12);
			PlayerRoundManager.SetMoney(10);
		}
		if (GameManager.CheckScore())
		{
			GameManager.LoadNextLevel(GameMode.HotKnife);
			return;
		}
		float delay = 8f - (float)(PhotonNetwork.time - info.timestamp);
		vp_Timer.In(delay, () =>
		{
			OnStartRound();
		});
	}

	private void SelectKnife()
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<int> list = new List<int>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (!playerList[i].GetDead())
			{
				list.Add(i);
			}
		}
		int iD = playerList[list[Random.Range(0, list.Count)]].ID;
		base.photonView.RPC("StartTimer", PhotonTargets.All, iD, false);
		GameManager.OnStatus("@: " + Utils.GetTeamHexColor(PhotonPlayer.Find(iD)), false, "HotKnife2");
	}

	[PunRPC]
	private void CheckPlayers()
	{
		if (!PhotonNetwork.isMasterClient || GameManager.GetRoundState() == RoundState.EndRound)
		{
			return;
		}
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		int num = -1;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (!playerList[i].GetDead())
			{
				num = ((num != -1) ? (-2) : playerList[i].ID);
			}
		}
		if (num == -1)
		{
			GameManager.OnMainStatus("@", false, 5f, "Draw");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All, -2);
			return;
		}
		if (num >= 0)
		{
			++GameManager.BlueScore;
			GameManager.UpdateScore();
			PhotonPlayer photonPlayer = PhotonPlayer.Find(num);
			GameManager.OnMainStatus(photonPlayer.name + " @", false, 5f, "Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All, photonPlayer.ID);
			return;
		}
		bool flag = false;
		for (int j = 0; j < playerList.Length; j++)
		{
			if (playerList[j].ID == KnifeID && !playerList[j].GetDead())
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			SelectKnife();
		}
	}
}
