using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MovementEffects
{
	public static class MECExtensionMethods
	{
		[CompilerGenerated]
		private sealed class _003CCancelWith_003Ec__Iterator8 : IDisposable, IEnumerator<float>, IEnumerator
		{
			internal GameObject gameObject;

			internal IEnumerator<float> coroutine;

			internal int _0024PC;

			internal float _0024current;

			internal GameObject _003C_0024_003EgameObject;

			internal IEnumerator<float> _003C_0024_003Ecoroutine;

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
					if ((bool)gameObject && gameObject.activeInHierarchy && coroutine.MoveNext())
					{
						_0024current = coroutine.Current;
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

		[CompilerGenerated]
		private sealed class _003CCancelWith_003Ec__Iterator9 : IDisposable, IEnumerator<float>, IEnumerator
		{
			internal GameObject gameObject1;

			internal GameObject gameObject2;

			internal IEnumerator<float> coroutine;

			internal int _0024PC;

			internal float _0024current;

			internal GameObject _003C_0024_003EgameObject1;

			internal GameObject _003C_0024_003EgameObject2;

			internal IEnumerator<float> _003C_0024_003Ecoroutine;

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
					if ((bool)gameObject1 && gameObject1.activeInHierarchy && (bool)gameObject2 && gameObject2.activeInHierarchy && coroutine.MoveNext())
					{
						_0024current = coroutine.Current;
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

		[CompilerGenerated]
		private sealed class _003CCancelWith_003Ec__IteratorA : IDisposable, IEnumerator<float>, IEnumerator
		{
			internal GameObject gameObject1;

			internal GameObject gameObject2;

			internal GameObject gameObject3;

			internal IEnumerator<float> coroutine;

			internal int _0024PC;

			internal float _0024current;

			internal GameObject _003C_0024_003EgameObject1;

			internal GameObject _003C_0024_003EgameObject2;

			internal GameObject _003C_0024_003EgameObject3;

			internal IEnumerator<float> _003C_0024_003Ecoroutine;

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
					if ((bool)gameObject1 && gameObject1.activeInHierarchy && (bool)gameObject2 && gameObject2.activeInHierarchy && (bool)gameObject3 && gameObject3.activeInHierarchy && coroutine.MoveNext())
					{
						_0024current = coroutine.Current;
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

		[DebuggerHidden]
		public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine, GameObject gameObject)
		{
			//yield-return decompiler failed: Could not find currentField
			_003CCancelWith_003Ec__Iterator8 obj = new _003CCancelWith_003Ec__Iterator8();
			obj.gameObject = gameObject;
			obj.coroutine = coroutine;
			obj._003C_0024_003EgameObject = gameObject;
			obj._003C_0024_003Ecoroutine = coroutine;
			return obj;
		}

		[DebuggerHidden]
		public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine, GameObject gameObject1, GameObject gameObject2)
		{
			//yield-return decompiler failed: Could not find currentField
			_003CCancelWith_003Ec__Iterator9 obj = new _003CCancelWith_003Ec__Iterator9();
			obj.gameObject1 = gameObject1;
			obj.gameObject2 = gameObject2;
			obj.coroutine = coroutine;
			obj._003C_0024_003EgameObject1 = gameObject1;
			obj._003C_0024_003EgameObject2 = gameObject2;
			obj._003C_0024_003Ecoroutine = coroutine;
			return obj;
		}

		[DebuggerHidden]
		public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine, GameObject gameObject1, GameObject gameObject2, GameObject gameObject3)
		{
			//yield-return decompiler failed: Could not find currentField
			_003CCancelWith_003Ec__IteratorA obj = new _003CCancelWith_003Ec__IteratorA();
			obj.gameObject1 = gameObject1;
			obj.gameObject2 = gameObject2;
			obj.gameObject3 = gameObject3;
			obj.coroutine = coroutine;
			obj._003C_0024_003EgameObject1 = gameObject1;
			obj._003C_0024_003EgameObject2 = gameObject2;
			obj._003C_0024_003EgameObject3 = gameObject3;
			obj._003C_0024_003Ecoroutine = coroutine;
			return obj;
		}
	}
}
