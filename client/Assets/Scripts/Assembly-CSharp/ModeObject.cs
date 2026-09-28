using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MovementEffects;
using UnityEngine;

public class ModeObject : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CLoadSync_003Ec__Iterator16 : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal int _003Ci_003E__0;

		internal int _0024PC;

		internal float _0024current;

		internal ModeObject _003C_003Ef__this;

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
				_003Ci_003E__0 = 0;
				goto IL_006f;
			case 1u:
				{
					_003Ci_003E__0++;
					goto IL_006f;
				}
				IL_006f:
				if (_003Ci_003E__0 < _003C_003Ef__this.Targets.Length)
				{
					_003C_003Ef__this.Targets[_003Ci_003E__0].SetActive(true);
					_0024current = Timing.WaitForSeconds(0.01f);
					_0024PC = 1;
					return true;
				}
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

	public GameMode Mode;

	public GameObject[] Targets;

	private void Start()
	{
		if (PhotonNetwork.room.GetGameMode() == Mode)
		{
			Timing.RunCoroutine(LoadSync());
		}
	}

	[DebuggerHidden]
	private IEnumerator<float> LoadSync()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CLoadSync_003Ec__Iterator16 obj = new _003CLoadSync_003Ec__Iterator16();
		obj._003C_003Ef__this = this;
		return obj;
	}
}
