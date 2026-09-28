using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CodeStage.AntiCheat.ObscuredTypes;
using MovementEffects;
using UnityEngine;

public class PlayerSkin : TimerBehaviour
{
	[CompilerGenerated]
	private sealed class _003CCheckPosition_003Ec__Iterator1A : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal int _0024PC;

		internal float _0024current;

		internal PlayerSkin _003C_003Ef__this;

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
			case 1u:
				if (_003C_003Ef__this.PlayerRenderer.isVisible && (_003C_003Ef__this.m_Transform.position - _003C_003Ef__this.PhotonPosition).sqrMagnitude > 3f)
				{
					_003C_003Ef__this.PlayerRigidbody.MovePosition(_003C_003Ef__this.PhotonPosition);
				}
				_0024current = 0f;
				_0024PC = 1;
				return true;
			default:
				return false;
			}
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
	private sealed class _003CUpdatePosition_003Ec__Iterator1B : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal int _0024PC;

		internal float _0024current;

		internal PlayerSkin _003C_003Ef__this;

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
			case 1u:
				if (_003C_003Ef__this.PlayerRenderer.isVisible)
				{
					_003C_003Ef__this.PlayerRigidbody.MovePosition(_003C_003Ef__this.VectorLerp(_003C_003Ef__this.m_Transform.position, _003C_003Ef__this.PhotonPosition, Time.deltaTime * _003C_003Ef__this.PhotonSpeed));
					_003C_003Ef__this.PlayerRigidbody.MoveRotation(Quaternion.Lerp(_003C_003Ef__this.m_Transform.rotation, _003C_003Ef__this.PhotonRotation, Time.deltaTime * _003C_003Ef__this.PhotonSpeed));
				}
				else
				{
					_003C_003Ef__this.PlayerRigidbody.MovePosition(_003C_003Ef__this.PhotonPosition);
					_003C_003Ef__this.PlayerRigidbody.MoveRotation(_003C_003Ef__this.PhotonRotation);
				}
				_0024current = 0f;
				_0024PC = 1;
				return true;
			default:
				return false;
			}
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
	private sealed class _003CUpdateMove_003Ec__Iterator1C : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal int _0024PC;

		internal float _0024current;

		internal PlayerSkin _003C_003Ef__this;

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
			case 1u:
				if (_003C_003Ef__this.isPlayerActive)
				{
					if (_003C_003Ef__this.PlayerRenderer.isVisible && _003C_003Ef__this.PlayerAnimator.GetFloat(MoveHash) != _003C_003Ef__this.Move)
					{
						_003C_003Ef__this.PlayerAnimator.SetFloat(MoveHash, _003C_003Ef__this.Move);
					}
					if (!_003C_003Ef__this.Foostep && _003C_003Ef__this.Sound && _003C_003Ef__this.isPlayerActive && Mathf.Abs(_003C_003Ef__this.Move) >= 0.3f)
					{
						Timing.RunCoroutine(_003C_003Ef__this.UpdateFoosteps());
					}
				}
				_0024current = 0f;
				_0024PC = 1;
				return true;
			default:
				return false;
			}
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
	private sealed class _003CUpdateRotate_003Ec__Iterator1D : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal int _0024PC;

		internal float _0024current;

		internal PlayerSkin _003C_003Ef__this;

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
			case 1u:
				if (_003C_003Ef__this.isPlayerActive)
				{
					if (_003C_003Ef__this.PlayerRenderer.isVisible)
					{
						_003C_003Ef__this.RotateLast = Mathf.Lerp(_003C_003Ef__this.RotateLast, _003C_003Ef__this.Rotate, Time.deltaTime * _003C_003Ef__this.PhotonSpeed);
						if (_003C_003Ef__this.RotateLast != _003C_003Ef__this.Rotate)
						{
							_003C_003Ef__this.PlayerAnimator.SetFloat(RotateHash, _003C_003Ef__this.RotateLast);
						}
					}
					else
					{
						_003C_003Ef__this.RotateLast = _003C_003Ef__this.Rotate;
					}
				}
				_0024current = 0f;
				_0024PC = 1;
				return true;
			default:
				return false;
			}
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
	private sealed class _003CUpdateFoosteps_003Ec__Iterator1E : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal AudioClip _003Cclip_003E__0;

		internal int _0024PC;

		internal float _0024current;

		internal PlayerSkin _003C_003Ef__this;

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
				_003C_003Ef__this.Foostep = true;
				_003Cclip_003E__0 = _003C_003Ef__this.PlayerFoosteps[UnityEngine.Random.Range(0, _003C_003Ef__this.PlayerFoosteps.Length)];
				_003C_003Ef__this.m_AudioSource.pitch = UnityEngine.Random.Range(1f, 1.5f);
				_003C_003Ef__this.m_AudioSource.clip = _003Cclip_003E__0;
				_003C_003Ef__this.m_AudioSource.Play();
				_0024current = Timing.WaitForSeconds(0.3f);
				_0024PC = 1;
				return true;
			case 1u:
				_003C_003Ef__this.Foostep = false;
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

	public bool isPlayerActive;

	public Team PlayerTeam;

	public ControllerManager Controller;

	public Renderer PlayerRenderer;

	public Animator PlayerAnimator;

	public PlayerSkinRagdoll PlayerRagdoll;

	public SkinnedMeshAtlas PlayerAtlas;

	public Transform PlayerWeaponRoot;

	public Transform PlayerTwoWeaponRoot;

	public AudioClip[] PlayerFoosteps;

	public AudioSource m_AudioSource;

	public Rigidbody PlayerRigidbody;

	public GameObject PlayerRootIK;

	public PlayerSounds Sounds;

	private bool Sound = true;

	private bool ShowDamage;

	public float PhotonSpeed = 10f;

	[Disabled]
	public Vector3 PhotonPosition = Vector3.zero;

	[Disabled]
	public Quaternion PhotonRotation = Quaternion.identity;

	[Disabled]
	public bool Dead;

	private float Move;

	private float Rotate;

	private float RotateLast;

	private bool Grounded = true;

	private bool Foostep;

	[HideInInspector]
	public int PlayerSkinID = -1;

	[HideInInspector]
	public TPWeaponShooter SelectWeapon;

	private List<TPWeaponShooter> WeaponsList = new List<TPWeaponShooter>();

	private Transform m_Transform;

	private bool isStart;

	private static int MoveHash = Animator.StringToHash("Move");

	private static int RotateHash = Animator.StringToHash("Rotate");

	private static int GroundedHash = Animator.StringToHash("Grounded");

	private static int WeaponHash = Animator.StringToHash("Weapon");

	private void Start()
	{
		Controller = base.transform.root.GetComponent<ControllerManager>();
		m_Transform = base.transform;
		EventManager.AddListener("UpdateSettings", UpdateSettings);
		UpdateSettings();
		vp_Timer.In(0.1f, () =>
		{
			string text = Controller.name;
			Timing.RunCoroutine(CheckPosition(), Segment.SlowUpdate, "CheckPosition " + text);
			Timing.RunCoroutine(UpdateRotate(), Segment.LateUpdate, "UpdateRotate " + text);
			Timing.RunCoroutine(UpdateMove(), Segment.SlowUpdate, "UpdateMove " + text);
			Timing.RunCoroutine(UpdatePosition(), Segment.LateUpdate, "UpdatePosition " + text);
			isStart = true;
		}, GetTimer());
	}

	private void UpdateSettings()
	{
		Sound = Settings.Audio;
		ShowDamage = Settings.ShowDamage;
	}

	private void OnEnable()
	{
		Foostep = false;
		isPlayerActive = true;
		if (PlayerSkinID != -1)
		{
			UpdateSkin();
		}
		if (isStart)
		{
			string text = Controller.name;
			Timing.RunCoroutine(CheckPosition(), Segment.SlowUpdate, "CheckPosition " + text);
			Timing.RunCoroutine(UpdateRotate(), Segment.LateUpdate, "UpdateRotate " + text);
			Timing.RunCoroutine(UpdateMove(), Segment.SlowUpdate, "UpdateMove " + text);
			Timing.RunCoroutine(UpdatePosition(), Segment.LateUpdate, "UpdatePosition " + text);
		}
	}

	private void OnDisable()
	{
		isPlayerActive = false;
		if (isStart)
		{
			string text = Controller.name;
			Timing.KillCoroutines("CheckPosition " + text);
			Timing.KillCoroutines("UpdateRotate " + text);
			Timing.KillCoroutines("UpdateMove " + text);
			Timing.KillCoroutines("UpdatePosition " + text);
		}
	}

	[DebuggerHidden]
	private IEnumerator<float> CheckPosition()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CCheckPosition_003Ec__Iterator1A obj = new _003CCheckPosition_003Ec__Iterator1A();
		obj._003C_003Ef__this = this;
		return obj;
	}

	private Vector3 VectorLerp(Vector3 from, Vector3 to, float t)
	{
		return new Vector3(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
	}

	[DebuggerHidden]
	private IEnumerator<float> UpdatePosition()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CUpdatePosition_003Ec__Iterator1B obj = new _003CUpdatePosition_003Ec__Iterator1B();
		obj._003C_003Ef__this = this;
		return obj;
	}

	[DebuggerHidden]
	private IEnumerator<float> UpdateMove()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CUpdateMove_003Ec__Iterator1C obj = new _003CUpdateMove_003Ec__Iterator1C();
		obj._003C_003Ef__this = this;
		return obj;
	}

	[DebuggerHidden]
	private IEnumerator<float> UpdateRotate()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CUpdateRotate_003Ec__Iterator1D obj = new _003CUpdateRotate_003Ec__Iterator1D();
		obj._003C_003Ef__this = this;
		return obj;
	}

	public void SetMove(float move)
	{
		Move = move;
	}

	public void SetRotate(float rotate)
	{
		Rotate = rotate;
	}

	public void SetGrounded(bool grounded)
	{
		if (isPlayerActive && Grounded != grounded)
		{
			PlayerAnimator.SetBool(GroundedHash, grounded);
			Grounded = grounded;
		}
	}

	public void SetWeapon(WeaponType weaponType, int skinID, int fireStat)
	{
		PlayerAnimator.SetInteger(WeaponHash, (int)weaponType.WeaponAnim);
		if (!(SelectWeapon != null) || !((ObscuredString)SelectWeapon.name == weaponType.WeaponName))
		{
			if (SelectWeapon != null)
			{
				SelectWeapon.Deactive();
			}
			TPWeaponShooter tPWeaponShooter = ContainsWeapon(weaponType.WeaponName);
			if (tPWeaponShooter == null)
			{
				GameObject gameObject = Utils.AddChild(weaponType.TpsPrefab, PlayerWeaponRoot, weaponType.TpsPrefab.transform.position, weaponType.TpsPrefab.transform.rotation);
				SelectWeapon = gameObject.GetComponent<TPWeaponShooter>();
				SelectWeapon.name = weaponType.WeaponName;
				WeaponsList.Add(SelectWeapon);
				SelectWeapon.SetData(weaponType.WeaponID, skinID, fireStat, PlayerTwoWeaponRoot);
			}
			else
			{
				SelectWeapon = tPWeaponShooter;
				SelectWeapon.UpdateFireStat(fireStat);
			}
			SelectWeapon.Active();
		}
	}

	private TPWeaponShooter ContainsWeapon(string weaponName)
	{
		for (int i = 0; i < WeaponsList.Count; i++)
		{
			if (weaponName == WeaponsList[i].name)
			{
				return WeaponsList[i];
			}
		}
		return null;
	}

	public void Fire()
	{
		if (SelectWeapon != null)
		{
			SelectWeapon.Fire(PlayerRenderer.isVisible);
			if (SelectWeapon.UseSound)
			{
				Sounds.Play(SelectWeapon.FireSound);
			}
		}
	}

	public void Reload()
	{
	}

	public void Damage(DamageInfo damageInfo)
	{
		if ((PlayerTeam != damageInfo.AttackerTeam || GameManager.GetFriendDamage()) && !Dead)
		{
			if (ShowDamage)
			{
				UIToast.Show(Localization.Get("Damage") + ": " + damageInfo.Damage, 2f);
			}
			UICrosshair.Hit();
			Controller.Damage(damageInfo);
		}
	}

	[DebuggerHidden]
	private IEnumerator<float> UpdateFoosteps()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CUpdateFoosteps_003Ec__Iterator1E obj = new _003CUpdateFoosteps_003Ec__Iterator1E();
		obj._003C_003Ef__this = this;
		return obj;
	}

	public void SetPosition(Vector3 pos)
	{
		PhotonPosition = pos;
		if (m_Transform == null)
		{
			m_Transform = base.transform;
		}
		m_Transform.position = PhotonPosition;
	}

	public void SetRotation(Vector3 rot)
	{
		PhotonRotation = Quaternion.Euler(rot);
		if (m_Transform == null)
		{
			m_Transform = base.transform;
		}
		m_Transform.rotation = PhotonRotation;
	}

	public void UpdateSkin()
	{
		string playerSkin = (int)PlayerTeam + "-" + PlayerSkinID;
		if (PlayerAtlas.mSpriteName != playerSkin)
		{
			vp_Timer.In(0.01f, () =>
			{
				PlayerAtlas.spriteName = playerSkin;
			}, GetTimer());
		}
	}

	public void SetActiveObject(bool active)
	{
		PlayerRenderer.gameObject.SetActive(active);
		PlayerRootIK.SetActive(active);
	}
}
