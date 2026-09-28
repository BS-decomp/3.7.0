using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MovementEffects;
using UnityEngine;

public class PlayerSkinRagdoll : TimerBehaviour
{
	[CompilerGenerated]
	private sealed class _003CActiveCoroutine_003Ec__Iterator1F : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal int _003Ci_003E__0;

		internal int _003Ci_003E__1;

		internal int _003Ci_003E__2;

		internal int _0024PC;

		internal float _0024current;

		internal PlayerSkinRagdoll _003C_003Ef__this;

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
				if (!_003C_003Ef__this.ActiveRagdoll)
				{
					vp_Timer.In(2f, () =>
					{
						if (_003C_003Ef__this.ActiveRagdoll)
						{
							_003C_003Ef__this.Deactive();
							_003C_003Ef__this.gameObject.SetActive(false);
						}
					}, _003C_003Ef__this.GetTimer());
				}
				_003C_003Ef__this.ActiveRagdoll = true;
				if (_003C_003Ef__this.Ragdoll)
				{
					if (_003C_003Ef__this.ForceVector.magnitude < 0.5f)
					{
						_003C_003Ef__this.ForceVector = new Vector3(0f, 0f, UnityEngine.Random.Range(-1, 1));
					}
					_003C_003Ef__this.m_PlayerSkin.PlayerAnimator.enabled = false;
					for (_003Ci_003E__0 = 0; _003Ci_003E__0 < _003C_003Ef__this.Colliders.Length; _003Ci_003E__0++)
					{
						_003C_003Ef__this.Colliders[_003Ci_003E__0].isTrigger = false;
					}
					for (_003Ci_003E__1 = 0; _003Ci_003E__1 < _003C_003Ef__this.Rigidbodies.Length; _003Ci_003E__1++)
					{
						_003C_003Ef__this.Rigidbodies[_003Ci_003E__1].detectCollisions = true;
						_003C_003Ef__this.Rigidbodies[_003Ci_003E__1].isKinematic = false;
						_003C_003Ef__this.Rigidbodies[_003Ci_003E__1].velocity = Vector3.zero;
					}
					_003Ci_003E__2 = 0;
					goto IL_01ea;
				}
				_003C_003Ef__this.m_PlayerSkin.PlayerAnimator.SetBool(_003C_003Ef__this.DeadHash, true);
				goto IL_0228;
			case 1u:
				{
					_003Ci_003E__2++;
					goto IL_01ea;
				}
				IL_0228:
				_0024PC = -1;
				break;
				IL_01ea:
				if (_003Ci_003E__2 < _003C_003Ef__this.Rigidbodies.Length)
				{
					_003C_003Ef__this.Rigidbodies[_003Ci_003E__2].AddForce(_003C_003Ef__this.ForceVector * 300f);
					_0024current = Timing.WaitForSeconds(0.01f);
					_0024PC = 1;
					return true;
				}
				goto IL_0228;
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

		internal void _003C_003Em__A0()
		{
			if (_003C_003Ef__this.ActiveRagdoll)
			{
				_003C_003Ef__this.Deactive();
				_003C_003Ef__this.gameObject.SetActive(false);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CDeactiveCoroutine_003Ec__Iterator20 : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal int _003Ci_003E__0;

		internal int _003Ci_003E__1;

		internal int _003Ci_003E__2;

		internal int _0024PC;

		internal float _0024current;

		internal PlayerSkinRagdoll _003C_003Ef__this;

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
				_003C_003Ef__this.ActiveRagdoll = false;
				if (_003C_003Ef__this.Ragdoll)
				{
					_003C_003Ef__this.m_PlayerSkin.PlayerAnimator.enabled = true;
					_003Ci_003E__0 = 0;
					goto IL_00b9;
				}
				_003C_003Ef__this.m_PlayerSkin.PlayerAnimator.SetBool(1293411866, false);
				goto IL_01d1;
			case 1u:
				{
					_003Ci_003E__0++;
					goto IL_00b9;
				}
				IL_01d1:
				_0024PC = -1;
				break;
				IL_00b9:
				if (_003Ci_003E__0 < _003C_003Ef__this.Rigidbodies.Length)
				{
					_003C_003Ef__this.Rigidbodies[_003Ci_003E__0].detectCollisions = false;
					_003C_003Ef__this.Rigidbodies[_003Ci_003E__0].isKinematic = true;
					_0024current = Timing.WaitForSeconds(0.01f);
					_0024PC = 1;
					return true;
				}
				for (_003Ci_003E__1 = 0; _003Ci_003E__1 < _003C_003Ef__this.Colliders.Length; _003Ci_003E__1++)
				{
					_003C_003Ef__this.Colliders[_003Ci_003E__1].isTrigger = true;
				}
				for (_003Ci_003E__2 = 0; _003Ci_003E__2 < _003C_003Ef__this.Transforms.Length; _003Ci_003E__2++)
				{
					_003C_003Ef__this.Transforms[_003Ci_003E__2].localPosition = _003C_003Ef__this.Positions[_003Ci_003E__2];
					_003C_003Ef__this.Transforms[_003Ci_003E__2].localRotation = _003C_003Ef__this.Rotations[_003Ci_003E__2];
				}
				goto IL_01d1;
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

	public PlayerSkin m_PlayerSkin;

	public float Force = 100f;

	public Collider[] Colliders;

	public Rigidbody[] Rigidbodies;

	public Transform[] Transforms;

	public Vector3[] Positions;

	public Quaternion[] Rotations;

	private bool ActiveRagdoll;

	private Vector3 ForceVector;

	private bool Ragdoll;

	private int DeadHash;

	private void Awake()
	{
		Ragdoll = true;
	}

	private void Start()
	{
		for (int i = 0; i < Rigidbodies.Length; i++)
		{
			Rigidbodies[i].detectCollisions = false;
		}
		DeadHash = Animator.StringToHash("Dead");
	}

	public void Active()
	{
		Active(Vector3.zero);
	}

	public void Active(Vector3 vector, bool headShot = false)
	{
		ForceVector = vector;
		if (m_PlayerSkin.PlayerRenderer.isVisible)
		{
			Timing.RunCoroutine(ActiveCoroutine());
			return;
		}
		if (ActiveRagdoll)
		{
			Deactive();
		}
		base.gameObject.SetActive(false);
	}

	[DebuggerHidden]
	private IEnumerator<float> ActiveCoroutine()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CActiveCoroutine_003Ec__Iterator1F obj = new _003CActiveCoroutine_003Ec__Iterator1F();
		obj._003C_003Ef__this = this;
		return obj;
	}

	public void Deactive()
	{
		Timing.RunCoroutine(DeactiveCoroutine());
	}

	[DebuggerHidden]
	private IEnumerator<float> DeactiveCoroutine()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CDeactiveCoroutine_003Ec__Iterator20 obj = new _003CDeactiveCoroutine_003Ec__Iterator20();
		obj._003C_003Ef__this = this;
		return obj;
	}
}
