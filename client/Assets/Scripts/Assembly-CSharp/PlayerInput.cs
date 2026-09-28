using System;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public ObscuredInt Health = 100;

	public Team PlayerTeam;

	public ObscuredFloat PlayerSpeed = 0.18f;

	public ObscuredBool Dead = true;

	public ObscuredBool NoDamage;

	public ObscuredBool Move = true;

	public ObscuredBool Zombie = false;

	public ObscuredInt MaxHealth = 100;

	public ObscuredBool Climb = false;

	public ObscuredBool Water = false;

	[Header("UFPS")]
	public vp_FPController FPController;

	public vp_FPPlayerEventHandler FPlayerEvent;

	public vp_FPCamera FPCamera;

	[Header("Player")]
	public CharacterController mCharacterController;

	public Transform PlayerTransform;

	public Camera PlayerCamera;

	public PlayerWeapons PlayerWeapon;

	public ControllerManager Controller;

	public AudioClip[] PlayerFoosteps;

	[Disabled]
	public Vector2 MoveAxis;

	[Disabled]
	public Vector2 LookAxis;

	[Disabled]
	public float RotateCamera;

	[Header("Fall Damage")]
	public ObscuredBool FallDamage;

	public ObscuredFloat FallDamageThreshold = 10f;

	private ObscuredBool FallingDamage = false;

	private float StartFallDamage;

	[Header("Bunny Hop")]
	public ObscuredBool BunnyHopEnabled;

	public ObscuredFloat BunnyHopSpeed = 0.4f;

	public ObscuredFloat BunnyHopLerp = 5f;

	public ObscuredFloat BunnyHopDefaultLerp = 0.5f;

	public ObscuredFloat BunnyHopDefaultSpeed = 0.18f;

	private Tween BunnyHopTween;

	private ObscuredBool BunnyHopActive = false;

	private ObscuredBool BunnyHopAutoJump;

	[Header("Surf")]
	public bool SurfEnabled;

	public float SurfAcceleration = 0.0001f;

	public float SurfMaxSpeed;

	public float SurfSpeed;

	public bool Surf;

	private bool isStopSurf;

	[Header("Others")]
	public AudioSource m_AudioSource;

	private Tweener ZombieMove;

	private bool isJump;

	private bool Sound;

	private vp_Timer.Handle MoveTimer = new vp_Timer.Handle();

	private bool isCursor = true;

	public static PlayerInput instance;

	public ObscuredBool Grounded
	{
		get
		{
			if ((bool)Water || Surf)
			{
				return false;
			}
			return mCharacterController.isGrounded;
		}
	}

	private void Start()
	{
		instance = this;
		Controller = base.transform.root.GetComponent<ControllerManager>();
		SetHealth(Health);
		EventManager.AddListener("UpdateSettings", UpdateSettings);
		UpdateSettings();
	}

	private void OnEnable()
	{
		vp_FPCamera fPCamera = FPCamera;
		fPCamera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Combine(fPCamera.BobStepCallback, new vp_FPCamera.BobStepDelegate(PlayFoosteps));
		InputManager.GetButtonDownEvent += GetButtonDown;
		InputManager.GetButtonUpEvent += GetButtonUp;
		InputManager.GetAxisEvent += GetAxis;
		if (GameManager.isStartDamage())
		{
			StartNoDamage();
		}
		if ((bool)Climb)
		{
			SetClimb(false);
		}
		if ((bool)Water)
		{
			SetWater(false);
		}
		Dead = false;
	}

	private void OnDisable()
	{
		vp_FPCamera fPCamera = FPCamera;
		fPCamera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Remove(fPCamera.BobStepCallback, new vp_FPCamera.BobStepDelegate(PlayFoosteps));
		InputManager.GetButtonDownEvent -= GetButtonDown;
		InputManager.GetButtonUpEvent -= GetButtonUp;
		InputManager.GetAxisEvent -= GetAxis;
		MoveAxis = Vector2.zero;
		LookAxis = Vector2.zero;
		if ((bool)BunnyHopActive)
		{
			BunnyHopAutoJump = false;
		}
		if (SurfEnabled)
		{
			SurfSpeed = 0f;
		}
		Dead = true;
		isJump = false;
	}

	private void GetButtonDown(string name)
	{
		if (name == "Jump")
		{
			isJump = true;
		}
	}

	private void GetButtonUp(string name)
	{
		if (name == "Jump")
		{
			isJump = false;
		}
	}

	private void GetAxis(string name, float value)
	{
		switch (name)
		{
		case "Horizontal":
			MoveAxis.x = value;
			break;
		case "Vertical":
			MoveAxis.y = value;
			break;
		case "Mouse X":
			LookAxis.x = value;
			break;
		case "Mouse Y":
			LookAxis.y = value;
			break;
		}
	}

	private void Update()
	{
		UpdateMove();
		UpdateLook();
		UpdateJump();
		UpdateBunnyHop();
		UpdateSurf();
		UpdateFallDamage();
		UpdateVelocity();
	}

	private void UpdateCursor()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			isCursor = !isCursor;
		}
		Screen.lockCursor = isCursor;
		Screen.showCursor = !isCursor;
	}

	private void UpdateMove()
	{
		if ((bool)Move)
		{
			if ((bool)Climb || (bool)Water)
			{
				MoveAxis /= 2.5f;
			}
			FPController.OnValue_InputMoveVector = MoveAxis;
		}
	}

	private void UpdateLook()
	{
		if (PlayerWeapon.isScope)
		{
			LookAxis *= (float)PlayerWeapon.GetSelectedWeaponData().RifleScopeSensitivity;
		}
		FPCamera.UpdateLook(LookAxis);
		RotateCamera = 0f - (float)Math.Round(FPCamera.Pitch / 60f, 1);
	}

	private void UpdateJump()
	{
		if ((bool)BunnyHopAutoJump)
		{
			if ((bool)Grounded)
			{
				if (FPController.CanStartJump())
				{
					FPController.OnStartJump();
				}
			}
			else
			{
				FPController.OnStopJump();
			}
		}
		else if (isJump && !Climb && !Water)
		{
			if (FPController.CanStartJump())
			{
				FPController.OnStartJump();
			}
		}
		else
		{
			FPController.OnStopJump();
		}
	}

	private void UpdateBunnyHop()
	{
		if (!BunnyHopEnabled)
		{
			return;
		}
		if (!Grounded && mCharacterController.velocity.sqrMagnitude > 20f)
		{
			if (!BunnyHopActive)
			{
				BunnyHopActive = true;
				if (BunnyHopTween != null)
				{
					BunnyHopTween.Kill();
				}
				BunnyHopTween = DOTween.To(() => FPController.MotorAcceleration, (float x) =>
				{
					FPController.MotorAcceleration = x;
				}, BunnyHopSpeed, BunnyHopLerp);
			}
		}
		else if ((bool)BunnyHopActive)
		{
			BunnyHopActive = false;
			if (BunnyHopTween != null)
			{
				BunnyHopTween.Kill();
			}
			BunnyHopTween = DOTween.To(() => FPController.MotorAcceleration, (float x) =>
			{
				FPController.MotorAcceleration = x;
			}, BunnyHopDefaultSpeed, BunnyHopDefaultLerp);
		}
	}

	private void UpdateSurf()
	{
		if (!SurfEnabled)
		{
			return;
		}
		if (isStopSurf)
		{
			Surf = false;
			SurfSpeed = 0f;
			FPController.Stop();
			isStopSurf = false;
			return;
		}
		if (FPController.GroundAngle > 30f && mCharacterController.isGrounded)
		{
			if (!Surf)
			{
				SurfSpeed += SurfAcceleration + mCharacterController.velocity.magnitude * SurfAcceleration;
			}
			else
			{
				SurfSpeed += SurfAcceleration;
			}
			Surf = true;
		}
		else if (FPController.GroundAngle < 30f && mCharacterController.isGrounded)
		{
			Surf = false;
			SurfSpeed = 0f;
		}
		else if (SurfSpeed > 0f)
		{
			Surf = false;
			SurfSpeed -= SurfAcceleration / 3f;
		}
		SurfSpeed = Mathf.Clamp(SurfSpeed, 0f, SurfMaxSpeed);
		if (SurfSpeed > 0f)
		{
			FPController.AddForce(FPCamera.Forward * (SurfSpeed * 0.0001f + MoveAxis.y / 100f));
		}
	}

	private void UpdateFallDamage()
	{
		if (!FallDamage)
		{
			return;
		}
		if ((bool)Grounded)
		{
			if ((bool)FallingDamage)
			{
				FallingDamage = false;
				if (PlayerTransform.position.y < StartFallDamage - (float)FallDamageThreshold)
				{
					int damage = (int)(StartFallDamage - PlayerTransform.position.y);
					DamageInfo damageInfo = DamageInfo.Create(damage, Vector3.zero, Team.None, 0, -1);
					Damage(damageInfo);
				}
			}
		}
		else if (!FallingDamage)
		{
			FallingDamage = true;
			StartFallDamage = PlayerTransform.position.y;
		}
	}

	private void UpdateVelocity()
	{
		if (mCharacterController.velocity.y < -100f)
		{
			DamageInfo damageInfo = DamageInfo.Create(1000, Vector3.zero, Team.None, 0, -1);
			Damage(damageInfo);
		}
	}

	public void SetBunnyHopAutoJump(bool active)
	{
		BunnyHopAutoJump = active;
	}

	public void Damage(DamageInfo damageInfo)
	{
		if ((bool)Dead || (bool)NoDamage)
		{
			return;
		}
		Health = (int)Health - damageInfo.Damage;
		Health = Mathf.Clamp(Health, 0, MaxHealth);
		UIGameManager.SetHealthLabel(Health);
		if (damageInfo.AttackPosition != Vector3.zero)
		{
			UIDamage.Damage(damageInfo.AttackPosition, FPCamera.Transform);
		}
		if ((int)Health <= 0)
		{
			GameManager.OnDeadPlayer(damageInfo);
			PlayerWeapon.DeactiveScope();
			return;
		}
		if ((bool)Zombie)
		{
			FPController.MotorAcceleration = 0.14f;
			if (ZombieMove != null && ZombieMove.IsActive())
			{
				ZombieMove.Kill();
			}
			ZombieMove = DOTween.To(() => FPController.MotorAcceleration, (float x) =>
			{
				FPController.MotorAcceleration = x;
			}, 0.2f, 1.5f);
		}
		FPCamera.AddRollForce(UnityEngine.Random.Range(-2, 2));
	}

	private void PlayFoosteps()
	{
		if (!Water && !Climb && (bool)Grounded)
		{
			UpdateFoosteps();
		}
	}

	public void UpdateFoosteps()
	{
		if (Sound)
		{
			AudioClip clip = PlayerFoosteps[UnityEngine.Random.Range(0, PlayerFoosteps.Length)];
			m_AudioSource.pitch = UnityEngine.Random.Range(1f, 1.5f);
			m_AudioSource.clip = clip;
			m_AudioSource.Play();
		}
	}

	public void StartNoDamage()
	{
		NoDamage = true;
		try
		{
			float startDamageTime = GameManager.GetStartDamageTime();
			if (startDamageTime != -1f)
			{
				vp_Timer.In(GameManager.GetStartDamageTime(), () =>
				{
					NoDamage = false;
				});
			}
		}
		catch
		{
			NoDamage = false;
		}
	}

	private void UpdateSettings()
	{
		vp_Timer.In(0.1f, () =>
		{
			float num = Settings.Sensitivity * 16f;
			FPCamera.MouseSensitivity = new Vector2(num, num);
			Sound = Settings.Audio;
		});
	}

	public void SetHealth(int health)
	{
		Health = health;
		UIGameManager.SetHealthLabel(Health);
	}

	public void SetMove(bool move)
	{
		Move = move;
	}

	public void SetMove(bool move, float duration)
	{
		Move = move;
		if (MoveTimer != null && MoveTimer.Active)
		{
			MoveTimer.Cancel();
		}
		vp_Timer.In(duration, () =>
		{
			Move = !move;
		}, MoveTimer);
	}

	public void UpdatePlayerSpeed(float speed)
	{
		PlayerSpeed = speed;
		FPController.MotorAcceleration = PlayerSpeed;
	}

	public void SetPlayerSpeed(float mass)
	{
		FPController.MotorAcceleration = (float)PlayerSpeed - mass;
	}

	public void SetClimb(bool active)
	{
		Climb = active;
		if ((bool)Climb)
		{
			FPController.Stop();
			FPController.PhysicsGravityModifier = 0f;
			FPController.MotorFreeFly = true;
		}
		else
		{
			FPController.PhysicsGravityModifier = 0.2f;
			FPController.MotorFreeFly = false;
		}
	}

	public void SetWater(bool active)
	{
		Water = active;
		if ((bool)Water)
		{
			FPController.PhysicsGravityModifier = 0.01f;
			FPController.MotorFreeFly = true;
		}
		else
		{
			FPController.PhysicsGravityModifier = 0.2f;
			FPController.MotorFreeFly = false;
		}
	}

	public void StopSurf()
	{
		if (Surf)
		{
			isStopSurf = true;
		}
	}
}
