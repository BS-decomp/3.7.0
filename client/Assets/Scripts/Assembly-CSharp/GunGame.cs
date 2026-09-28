using CodeStage.AntiCheat.ObscuredTypes;
using Photon;
using UnityEngine;

public class GunGame : Photon.MonoBehaviour
{
	public ObscuredInt MaxScore = 100;

	private ObscuredBool isFinishRound;

	private int PlayerKills;

	private int[] Weapons = new int[31]
	{
		3, 27, 13, 36, 6, 2, 21, 37, 9, 25,
		26, 14, 24, 12, 7, 18, 29, 19, 28, 1,
		5, 15, 8, 30, 23, 38, 11, 10, 16, 4,
		22
	};

	private int SelectWeapon;

	private WeaponType WeaponData;

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.GunGame)
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		GameManager.UpdateRoundState(RoundState.PlayRound);
		CameraManager.ActiveStaticCamera();
		UIGameManager.SetActiveScore(true, MaxScore);
		WeaponManager.SetKnifeType(0);
		WeaponManager.SetPistolType(3);
		WeaponManager.SetRifleType(0);
		GameManager.MaxScore = MaxScore;
		GameManager.SetChangeWeapons(false);
		GameManager.StartAutoBalance();
		WeaponData = WeaponType.CreateNewClass(WeaponManager.GetWeaponType(3));
		UISelectTeam.OnStart();
		EventManager.AddListener<Team>("SelectTeam", OnSelectTeam);
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
		EventManager.AddListener<Team>("AutoBalance", OnAutoBalance);
	}

	private void OnSelectTeam(Team team)
	{
		UIPanelManager.ShowPanel("Display");
		OnRevivalPlayer();
	}

	private void OnAutoBalance(Team team)
	{
		GameManager.UpdatePlayerTeam(team);
		OnRevivalPlayer();
	}

	private void OnRevivalPlayer()
	{
		PlayerInput playerInput = GameManager.GetController().PlayerInput;
		playerInput.SetHealth(100);
		CameraManager.DeactiveAll();
		GameManager.GetController().ActivePlayer(GameManager.GetTeamSpawn().GetSpawnPosition(), GameManager.GetTeamSpawn().GetSpawnRotation());
		playerInput.PlayerWeapon.UpdateWeaponAll(WeaponData.Weapon);
		if (WeaponData.Weapon != WeaponTypeList.Knife)
		{
			vp_Timer.In(0.1f, () =>
			{
				PlayerWeapons.WeaponData weaponData = playerInput.PlayerWeapon.GetWeaponData(WeaponData.Weapon);
				weaponData.AmmoMax = (int)weaponData.AmmoMax * 2;
				UIGameManager.SetAmmoLabel(playerInput.PlayerWeapon.GetSelectedWeaponData().Ammo, playerInput.PlayerWeapon.GetSelectedWeaponData().AmmoMax);
			});
		}
	}

	private void OnUpdateWeapon()
	{
		if (SelectWeapon >= Weapons.Length - 1)
		{
			SelectWeapon = 0;
		}
		else
		{
			SelectWeapon++;
		}
		WeaponManager.SetKnifeType(0);
		WeaponManager.SetPistolType(0);
		WeaponManager.SetRifleType(0);
		WeaponData = WeaponManager.GetWeaponType(Weapons[SelectWeapon]);
		UIToast.Show(WeaponData.WeaponName);
		switch (WeaponData.Weapon)
		{
		case WeaponTypeList.Knife:
			WeaponManager.SetKnifeType(WeaponData.WeaponID);
			break;
		case WeaponTypeList.Pistol:
			WeaponManager.SetPistolType(WeaponData.WeaponID);
			break;
		case WeaponTypeList.Rifle:
			WeaponManager.SetRifleType(WeaponData.WeaponID);
			break;
		}
		PlayerInput playerInput = GameManager.GetController().PlayerInput;
		if ((bool)playerInput.Dead)
		{
			return;
		}
		playerInput.PlayerWeapon.CanFire = false;
		vp_Timer.In(0.2f, () =>
		{
			if (!playerInput.Dead)
			{
				playerInput.PlayerWeapon.UpdateWeaponAll(WeaponData.Weapon);
				vp_Timer.In(0.1f, () =>
				{
					playerInput.PlayerWeapon.CanFire = true;
					if (WeaponData.Weapon != WeaponTypeList.Knife)
					{
						PlayerWeapons.WeaponData weaponData = playerInput.PlayerWeapon.GetWeaponData(WeaponData.Weapon);
						weaponData.AmmoMax = (int)weaponData.AmmoMax * 2;
						UIGameManager.SetAmmoLabel(playerInput.PlayerWeapon.GetSelectedWeaponData().Ammo, playerInput.PlayerWeapon.GetSelectedWeaponData().AmmoMax);
					}
				});
			}
			else
			{
				playerInput.PlayerWeapon.CanFire = true;
			}
		});
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		PlayerRoundManager.SetDeaths1();
		if (damageInfo.PlayerID != -1)
		{
			GameManager.OnStatus(Utils.KillerStatus(damageInfo), false, string.Empty);
			OnScore(damageInfo.AttackerTeam);
			UIDeathScreen.Show(damageInfo);
		}
		Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.GetController().PlayerInput.PlayerTransform.position, damageInfo.AttackPosition);
		CameraManager.ActiveDeadCamera(GameManager.GetController().PlayerInput.FPCamera.Transform.position, GameManager.GetController().PlayerInput.FPCamera.Transform.eulerAngles, ragdollForce * 100f);
		GameManager.GetController().DeactivePlayer(ragdollForce, damageInfo.HeadShot);
		base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.PlayerID), damageInfo);
		vp_Timer.In(3f, () =>
		{
			OnRevivalPlayer();
		});
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.UpdateScore(playerConnect);
		}
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
			PlayerRoundManager.SetXP(10);
			PlayerRoundManager.SetMoney(6);
			PlayerRoundManager.SetHeadshot1();
		}
		else
		{
			PlayerRoundManager.SetXP(5);
			PlayerRoundManager.SetMoney(3);
		}
		if (PlayerKills >= 1 || (PlayerKills >= 0 && WeaponData.Weapon == WeaponTypeList.Knife))
		{
			PlayerKills = 0;
			OnUpdateWeapon();
		}
		else
		{
			PlayerKills++;
		}
	}

	public void OnScore(Team team)
	{
		base.photonView.RPC("PhotonOnScore", PhotonTargets.MasterClient, (int)team);
	}

	[PunRPC]
	private void PhotonOnScore(int intTeam)
	{
		switch ((Team)intTeam)
		{
		case Team.Blue:
			++GameManager.BlueScore;
			break;
		case Team.Red:
			++GameManager.RedScore;
			break;
		}
		GameManager.UpdateScore();
		if (GameManager.CheckScore())
		{
			GameManager.UpdateRoundState(RoundState.EndRound);
			if (GameManager.WinTeam() == Team.Blue)
			{
				GameManager.OnMainStatus("@", false, 5f, "Blue Win");
			}
			else if (GameManager.WinTeam() == Team.Red)
			{
				GameManager.OnMainStatus("@", false, 5f, "Red Win");
			}
			base.photonView.RPC("PhotonNextLevel", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void PhotonNextLevel()
	{
		GameManager.LoadNextLevel(GameMode.GunGame);
	}
}
