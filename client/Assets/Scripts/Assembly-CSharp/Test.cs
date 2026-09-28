using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MovementEffects;
using UnityEngine;

public class Test : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CCheck_003Ec__Iterator2F : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal WWW _003Cwww_003E__0;

		internal int _0024PC;

		internal float _0024current;

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
				_003Cwww_003E__0 = new WWW("https://api-project-106199818523.firebaseio.com/.info/connected");
				_0024current = Timing.WaitUntilDone(_003Cwww_003E__0);
				_0024PC = 1;
				return true;
			case 1u:
				MonoBehaviour.print(_003Cwww_003E__0.text);
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

	public int r;

	public bool t;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			Timing.RunCoroutine(Check());
		}
	}

	[DebuggerHidden]
	private IEnumerator<float> Check()
	{
		//yield-return decompiler failed: Could not find currentField
		return new _003CCheck_003Ec__Iterator2F();
	}
}
