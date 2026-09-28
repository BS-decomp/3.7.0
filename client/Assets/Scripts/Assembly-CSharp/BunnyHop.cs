using Photon;
using UnityEngine;

public class BunnyHop : Photon.MonoBehaviour
{
	private Vector3 StartSpawnPosition;

	private Quaternion StartSpawnRotation;

	private static BunnyHop instance;

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			Object.Destroy(this);
		}
		else if (PhotonNetwork.room.GetGameMode() != GameMode.BunnyHop)
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		instance = this;
		GameManager.UpdateRoundState(RoundState.PlayRound);
		UIGameManager.SetActiveScore(true, 0);
		GameManager.SetStartDamageTime(1f);
		UIPanelManager.ShowPanel("Display");
		GameManager.SetChangeWeapons(false);
		CameraManager.ActiveStaticCamera();
		vp_Timer.In(0.5f, () =>
		{
			GameManager.OnSelectTeam(Team.Blue);
			StartSpawnPosition = GameManager.GetTeamSpawn(Team.Blue).GetTransform().position;
			StartSpawnRotation = GameManager.GetTeamSpawn(Team.Blue).GetTransform().rotation;
			PlayerInput playerInput = GameManager.GetController().PlayerInput;
			playerInput.BunnyHopEnabled = true;
			playerInput.FPController.MotorJumpForce = 0.2f;
			playerInput.FPController.MotorAirSpeed = 1f;
			OnRevivalPlayer();
			vp_Timer.In(1.5f, () =>
			{
				if (PhotonNetwork.isMasterClient)
				{
					base.photonView.RPC("StartTimer", PhotonTargets.All);
				}
			});
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
	}

	private void OnSpawnPlayer()
	{
		GameManager.GetController().SpawnPlayer(GameManager.GetTeamSpawn().GetSpawnPosition(), GameManager.GetTeamSpawn().GetSpawnRotation());
	}

	private void OnRevivalPlayer()
	{
		WeaponManager.SetPistolType(0);
		WeaponManager.SetRifleType(0);
		PlayerInput playerInput = GameManager.GetController().PlayerInput;
		playerInput.SetHealth(100);
		CameraManager.DeactiveAll();
		GameManager.GetController().ActivePlayer(GameManager.GetTeamSpawn().GetSpawnPosition(), GameManager.GetTeamSpawn().GetSpawnRotation());
		playerInput.PlayerWeapon.UpdateWeaponAll(WeaponTypeList.Knife);
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		++GameManager.RedScore;
		UIGameManager.UpdateScoreLabel(0, GameManager.BlueScore, GameManager.RedScore);
		OnSpawnPlayer();
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient && UIGameManager.instance.isScoreTimer)
		{
			base.photonView.RPC("UpdateTimer", playerConnect, UIGameManager.instance.ScoreTimer - Time.time);
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
	}

	[PunRPC]
	private void StartTimer(PhotonMessageInfo info)
	{
		float num = 600f;
		num -= (float)(PhotonNetwork.time - info.timestamp);
		UIGameManager.StartScoreTimer(num, StopTimer);
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
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.UpdateRoundState(RoundState.EndRound);
			GameManager.OnMainStatus("@", false, 5f, "Next Map");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessageInfo info)
	{
		GameManager.LoadNextLevel(GameMode.BunnyHop);
	}

	public static void FinishMap(int money = 0, int xp = 0)
	{
		Transform transform = GameManager.GetTeamSpawn().GetTransform();
		transform.position = instance.StartSpawnPosition;
		transform.rotation = instance.StartSpawnRotation;
		GameManager.OnMainStatus(PhotonNetwork.player.name + " @", false, 5f, "Finished map");
		if (money != 0)
		{
			PlayerRoundManager.SetMoney(money);
			UIToast.Show("+" + money + " " + Localization.Get("Money"));
		}
		PlayerRoundManager.SetXP(xp);
		instance.OnSpawnPlayer();
		PhotonNetwork.player.SetKills1();
		++GameManager.BlueScore;
		UIGameManager.UpdateScoreLabel(0, GameManager.BlueScore, GameManager.RedScore);
	}

	public static void SpawnDead()
	{
		instance.OnSpawnPlayer();
	}
}
