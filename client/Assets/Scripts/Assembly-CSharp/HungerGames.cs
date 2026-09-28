using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Photon;
using UnityEngine;

public class HungerGames : Photon.MonoBehaviour
{
	private List<int> UsedBox = new List<int>();

	public ObscuredInt MaxScore = 20;

	private static HungerGames instance;

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.HungerGames)
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		instance = this;
		UIGameManager.SetActiveScore(true, 20);
		GameManager.SetStartDamageTime(3f);
		GameManager.SetFriendDamage(true);
		UIPanelManager.ShowPanel("Display");
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
			else if ((bool)GameManager.GetController().PlayerInput.Dead)
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
		OnCreatePlayer();
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
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		GameManager.UpdateScore(playerConnect);
		if (GameManager.GetRoundState() != RoundState.WaitPlayer)
		{
			CheckPlayers();
		}
		vp_Timer.In(1.5f, () =>
		{
			string text = Utils.ArrayToString(UsedBox.ToArray());
			if (!string.IsNullOrEmpty(text))
			{
				base.photonView.RPC("PhotonHideBoxes", playerConnect, text);
			}
		});
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
			base.photonView.RPC("OnCreatePlayer", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void OnCreatePlayer()
	{
		EventManager.Dispatch("StartRound");
		UsedBox.Clear();
		if (PhotonNetwork.player.GetTeam() != Team.None)
		{
			PlayerInput playerInput = GameManager.GetController().PlayerInput;
			playerInput.SetHealth(100);
			CameraManager.DeactiveAll();
			DrawElements playerIDSpawn = GameManager.GetPlayerIDSpawn();
			GameManager.GetController().ActivePlayer(playerIDSpawn.GetSpawnPosition(), playerIDSpawn.GetSpawnRotation());
			WeaponManager.SetKnifeType(4);
			WeaponManager.SetPistolType(0);
			WeaponManager.SetRifleType(0);
			playerInput.PlayerWeapon.UpdateWeaponAll(WeaponTypeList.Knife);
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		PlayerRoundManager.SetDeaths1();
		GameManager.OnStatus(Utils.KillerStatus(damageInfo), false, string.Empty);
		UIDeathScreen.Show(damageInfo);
		Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.GetController().PlayerInput.PlayerTransform.position, damageInfo.AttackPosition);
		CameraManager.ActiveDeadCamera(GameManager.GetController().PlayerInput.FPCamera.Transform.position, GameManager.GetController().PlayerInput.FPCamera.Transform.eulerAngles, ragdollForce * 100f);
		GameManager.GetController().DeactivePlayer(ragdollForce, damageInfo.HeadShot);
		base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.PlayerID), damageInfo);
		base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
		vp_Timer.In(3f, () =>
		{
			if ((bool)GameManager.GetController().PlayerInput.Dead)
			{
				CameraManager.ActiveSpectateCamera();
			}
		});
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
		GameManager.UpdateRoundState(RoundState.EndRound);
		if (GameManager.CheckScore())
		{
			GameManager.LoadNextLevel(GameMode.HungerGames);
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
		int num = -1;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (!playerList[i].GetDead())
			{
				if (num != -1)
				{
					flag = false;
					break;
				}
				num = playerList[i].ID;
				flag = true;
			}
		}
		if (flag)
		{
			++GameManager.BlueScore;
			GameManager.UpdateScore();
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
			GameManager.OnMainStatus(PhotonPlayer.Find(num).name + " @", false, 5f, "Win");
		}
	}

	public static void PickupBox(int id)
	{
		instance.photonView.RPC("MasterPickupBox", PhotonTargets.MasterClient, id);
	}

	[PunRPC]
	private void MasterPickupBox(int id, PhotonMessageInfo info)
	{
		if (!UsedBox.Contains(id))
		{
			base.photonView.RPC("EventPickupBox", PhotonTargets.All, id, info.sender.ID);
		}
	}

	[PunRPC]
	private void EventPickupBox(int id, int pickupPlayer)
	{
		UsedBox.Add(id);
		EventManager.Dispatch("EventPickupBox", id, pickupPlayer);
	}

	public static void HideBoxes(int[] idBoxes)
	{
		string text = Utils.ArrayToString(idBoxes);
		instance.photonView.RPC("PhotonHideBoxes", PhotonTargets.All, text);
	}

	[PunRPC]
	private void PhotonHideBoxes(string data)
	{
		int[] ids = Utils.StringToArrayInt(data);
		vp_Timer.In(0.1f, () =>
		{
			for (int i = 0; i < ids.Length; i++)
			{
				UsedBox.Add(ids[i]);
				EventManager.Dispatch("EventPickupBox", ids[i], -1);
			}
		});
	}

	public static void SetWeapon(int weaponID)
	{
		WeaponTypeList weapon = WeaponManager.GetWeaponType(weaponID).Weapon;
		PlayerInput playerInput = GameManager.GetController().PlayerInput;
		WeaponManager.SetWeaponType(weaponID);
		playerInput.PlayerWeapon.UpdateWeaponAll(weapon);
		vp_Timer.In(0.5f, () =>
		{
			playerInput.PlayerWeapon.GetWeaponData(WeaponTypeList.Rifle).AmmoMax = 0;
			playerInput.PlayerWeapon.GetWeaponData(WeaponTypeList.Pistol).AmmoMax = 0;
			UIGameManager.SetAmmoLabel(playerInput.PlayerWeapon.GetSelectedWeaponData().Ammo, playerInput.PlayerWeapon.GetSelectedWeaponData().AmmoMax);
		});
	}
}
