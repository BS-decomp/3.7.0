using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Crypto;
using MovementEffects;
using UnityEngine;

public class CheckValueManager : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CStartAuto_003Ec__Iterator28 : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal int _003Ci_003E__0;

		internal int _0024PC;

		internal float _0024current;

		internal CheckValueManager _003C_003Ef__this;

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
				goto IL_008f;
			case 1u:
				{
					_003Ci_003E__0++;
					goto IL_008f;
				}
				IL_008f:
				if (_003Ci_003E__0 < _003C_003Ef__this.AutoValue.Length)
				{
					if (_003C_003Ef__this.AutoValue[_003Ci_003E__0] == Mathf.Abs(_003C_003Ef__this.AutoValueCheck[_003Ci_003E__0]))
					{
						_0024current = Timing.WaitForSeconds(0.05f);
						_0024PC = 1;
						return true;
					}
					Application.Quit();
				}
				_003C_003Ef__this.Invoke("CheckAuto", _003C_003Ef__this.Duration);
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

	public float Duration;

	public bool Auto;

	private float[] AutoValue;

	public float[] AutoValueCheck;

	public CryptoInt[] Int;

	private int[] IntDefault;

	public CryptoFloat[] Float;

	private float[] FloatDefault;

	private void Start()
	{
		IntDefault = new int[Int.Length];
		for (int i = 0; i < Int.Length; i++)
		{
			IntDefault[i] = Int[i];
		}
		FloatDefault = new float[Float.Length];
		for (int j = 0; j < Float.Length; j++)
		{
			FloatDefault[j] = Float[j];
		}
		InvokeRepeating("Check", Duration, Duration);
		if (Auto)
		{
			Invoke("CheckAuto", Duration);
			AutoValue = new float[1000];
			AutoValueCheck = new float[1000];
			for (int k = 0; k < AutoValue.Length; k++)
			{
				AutoValue[k] = k;
				AutoValueCheck[k] = -k;
			}
		}
	}

	private void Check()
	{
		for (int i = 0; i < Int.Length; i++)
		{
			if ((int)Int[i] != IntDefault[i])
			{
				Application.Quit();
			}
		}
		for (int j = 0; j < Float.Length; j++)
		{
			if ((float)Float[j] != FloatDefault[j])
			{
				Application.Quit();
			}
		}
	}

	private void CheckAuto()
	{
		Timing.RunCoroutine(StartAuto());
	}

	[DebuggerHidden]
	private IEnumerator<float> StartAuto()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CStartAuto_003Ec__Iterator28 obj = new _003CStartAuto_003Ec__Iterator28();
		obj._003C_003Ef__this = this;
		return obj;
	}
}
