using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MovementEffects
{
	public class Timing : MonoBehaviour
	{
		private class WaitingProcess
		{
			public class ProcessData
			{
				public IEnumerator<float> Task;

				public string Tag;

				public Segment Segment;

				public double PauseTime;
			}

			public IEnumerator<float> Trigger;

			public string TriggerTag;

			public bool Killed;

			public readonly List<ProcessData> Tasks = new List<ProcessData>();
		}

		private struct ProcessIndex : IEquatable<ProcessIndex>
		{
			public Segment seg;

			public int i;

			public bool Equals(ProcessIndex other)
			{
				return seg == other.seg && i == other.i;
			}

			public override bool Equals(object other)
			{
				if (other is ProcessIndex)
				{
					return Equals((ProcessIndex)other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (int)(seg - 2) * 715827882 + i;
			}

			public static bool operator ==(ProcessIndex a, ProcessIndex b)
			{
				return a.seg == b.seg && a.i == b.i;
			}

			public static bool operator !=(ProcessIndex a, ProcessIndex b)
			{
				return a.seg != b.seg || a.i != b.i;
			}
		}

		public enum DebugInfoType
		{
			None = 0,
			SeperateCoroutines = 1,
			SeperateTags = 2
		}

		[CompilerGenerated]
		private sealed class _003CInjectDelay_003Ec__Iterator0 : IDisposable, IEnumerator<float>, IEnumerator
		{
			internal double returnAt;

			internal IEnumerator<float> proc;

			internal int _0024PC;

			internal float _0024current;

			internal double _003C_0024_003EreturnAt;

			internal IEnumerator<float> _003C_0024_003Eproc;

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
					_0024current = (float)returnAt;
					_0024PC = 1;
					break;
				case 1u:
					ReplacementFunction = (IEnumerator<float> input, Segment timing, string tag) => proc;
					_0024current = float.NaN;
					_0024PC = 2;
					break;
				case 2u:
					_0024PC = -1;
					goto default;
				default:
					return false;
				}
				return true;
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

			internal IEnumerator<float> _003C_003Em__6(IEnumerator<float> input, Segment timing, string tag)
			{
				return proc;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_StartWhenDone_003Ec__Iterator1 : IDisposable, IEnumerator<float>, IEnumerator
		{
			internal WaitingProcess processData;

			internal int _0024PC;

			internal float _0024current;

			internal WaitingProcess _003C_0024_003EprocessData;

			internal Timing _003C_003Ef__this;

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
				bool flag = false;
				switch (num)
				{
				case 0u:
					num = 4294967293u;
					goto case 1u;
				case 1u:
				case 2u:
					try
					{
						switch (num)
						{
						default:
							if (!processData.Killed)
							{
								if ((double)processData.Trigger.Current > _003C_003Ef__this.localTime)
								{
									_0024current = processData.Trigger.Current;
									_0024PC = 1;
									flag = true;
									goto end_IL_0011;
								}
								break;
							}
							_003C_003Ef__this.CloseWaitingProcess(processData);
							goto end_IL_002a;
						case 1u:
							if (!processData.Killed)
							{
								break;
							}
							_003C_003Ef__this.CloseWaitingProcess(processData);
							goto end_IL_002a;
						case 2u:
							if (!processData.Killed)
							{
								break;
							}
							_003C_003Ef__this.CloseWaitingProcess(processData);
							goto end_IL_002a;
						}
						if (processData.Trigger.MoveNext())
						{
							_0024current = processData.Trigger.Current;
							_0024PC = 2;
							flag = true;
							break;
						}
						goto IL_013f;
						end_IL_002a:;
					}
					finally
					{
						if (!flag)
						{
							_003C_003E__Finally0();
						}
					}
					goto default;
				default:
					{
						return false;
					}
					IL_013f:
					_0024PC = -1;
					goto default;
					end_IL_0011:
					break;
				}
				return true;
			}

			[DebuggerHidden]
			public void Dispose()
			{
				uint num = (uint)_0024PC;
				_0024PC = -1;
				switch (num)
				{
				case 1u:
				case 2u:
					try
					{
						break;
					}
					finally
					{
						_003C_003E__Finally0();
					}
				case 0u:
					break;
				}
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			private void _003C_003E__Finally0()
			{
				_003C_003Ef__this.CloseWaitingProcess(processData);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_StartWhenDone_003Ec__Iterator2 : IDisposable, IEnumerator<float>, IEnumerator
		{
			internal WWW www;

			internal IEnumerator<float> pausedProc;

			internal int _0024PC;

			internal float _0024current;

			internal WWW _003C_0024_003Ewww;

			internal IEnumerator<float> _003C_0024_003EpausedProc;

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
					if (!www.isDone)
					{
						_0024current = 0f;
						_0024PC = 1;
					}
					else
					{
						ReplacementFunction = delegate
						{
							return pausedProc;
						};
						_0024current = float.NaN;
						_0024PC = 2;
					}
					return true;
				case 2u:
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

			internal IEnumerator<float> _003C_003Em__7(IEnumerator<float> P_0, Segment P_1, string P_2)
			{
				return pausedProc;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_StartWhenDone_003Ec__Iterator3 : IDisposable, IEnumerator<float>, IEnumerator
		{
			internal AsyncOperation operation;

			internal IEnumerator<float> pausedProc;

			internal int _0024PC;

			internal float _0024current;

			internal AsyncOperation _003C_0024_003Eoperation;

			internal IEnumerator<float> _003C_0024_003EpausedProc;

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
					if (!operation.isDone)
					{
						_0024current = 0f;
						_0024PC = 1;
					}
					else
					{
						ReplacementFunction = delegate
						{
							return pausedProc;
						};
						_0024current = float.NaN;
						_0024PC = 2;
					}
					return true;
				case 2u:
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

			internal IEnumerator<float> _003C_003Em__8(IEnumerator<float> P_0, Segment P_1, string P_2)
			{
				return pausedProc;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_CallDelayBack_003Ec__Iterator4<TRef> : IDisposable, IEnumerator<float>, IEnumerator
		{
			internal float delay;

			internal TRef reference;

			internal Action<TRef> action;

			internal int _0024PC;

			internal float _0024current;

			internal float _003C_0024_003Edelay;

			internal TRef _003C_0024_003Ereference;

			internal Action<TRef> _003C_0024_003Eaction;

			internal Timing _003C_003Ef__this;

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
					_0024current = (float)_003C_003Ef__this.localTime + delay;
					_0024PC = 1;
					return true;
				case 1u:
					CallDelayed(reference, -1f, action);
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
		private sealed class _003C_CallDelayBack_003Ec__Iterator5 : IDisposable, IEnumerator<float>, IEnumerator
		{
			internal float delay;

			internal Action action;

			internal int _0024PC;

			internal float _0024current;

			internal float _003C_0024_003Edelay;

			internal Action _003C_0024_003Eaction;

			internal Timing _003C_003Ef__this;

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
					_0024current = (float)_003C_003Ef__this.localTime + delay;
					_0024PC = 1;
					return true;
				case 1u:
					CallDelayed(-1f, action);
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
		private sealed class _003C_CallContinuously_003Ec__Iterator6 : IDisposable, IEnumerator<float>, IEnumerator
		{
			internal double _003CstartTime_003E__0;

			internal float timeframe;

			internal float period;

			internal Action action;

			internal Action onDone;

			internal int _0024PC;

			internal float _0024current;

			internal float _003C_0024_003Etimeframe;

			internal float _003C_0024_003Eperiod;

			internal Action _003C_0024_003Eaction;

			internal Action _003C_0024_003EonDone;

			internal Timing _003C_003Ef__this;

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
					_003CstartTime_003E__0 = _003C_003Ef__this.localTime;
					goto IL_005f;
				case 1u:
					{
						action();
						goto IL_005f;
					}
					IL_005f:
					if (_003C_003Ef__this.localTime <= _003CstartTime_003E__0 + (double)timeframe)
					{
						_0024current = WaitForSeconds(period);
						_0024PC = 1;
						return true;
					}
					if (onDone != null)
					{
						onDone();
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
		private sealed class _003C_CallContinuously_003Ec__Iterator7<T> : IDisposable, IEnumerator<float>, IEnumerator
		{
			internal double _003CstartTime_003E__0;

			internal float timeframe;

			internal float period;

			internal Action<T> action;

			internal T reference;

			internal Action<T> onDone;

			internal int _0024PC;

			internal float _0024current;

			internal float _003C_0024_003Etimeframe;

			internal float _003C_0024_003Eperiod;

			internal Action<T> _003C_0024_003Eaction;

			internal T _003C_0024_003Ereference;

			internal Action<T> _003C_0024_003EonDone;

			internal Timing _003C_003Ef__this;

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
					_003CstartTime_003E__0 = _003C_003Ef__this.localTime;
					goto IL_0065;
				case 1u:
					{
						action(reference);
						goto IL_0065;
					}
					IL_0065:
					if (_003C_003Ef__this.localTime <= _003CstartTime_003E__0 + (double)timeframe)
					{
						_0024current = WaitForSeconds(period);
						_0024PC = 1;
						return true;
					}
					if (onDone != null)
					{
						onDone(reference);
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

		private const ushort FramesUntilMaintenance = 64;

		private const int ProcessArrayChunkSize = 64;

		private const int InitialBufferSizeLarge = 256;

		private const int InitialBufferSizeMedium = 64;

		private const int InitialBufferSizeSmall = 8;

		public float TimeBetweenSlowUpdateCalls = 1f / 7f;

		public DebugInfoType ProfilerDebugAmount = DebugInfoType.SeperateCoroutines;

		public int UpdateCoroutines;

		public int FixedUpdateCoroutines;

		public int LateUpdateCoroutines;

		public int SlowUpdateCoroutines;

		[HideInInspector]
		public double localTime;

		[HideInInspector]
		public float deltaTime;

		private bool _runningUpdate;

		private bool _runningFixedUpdate;

		private bool _runningLateUpdate;

		private bool _runningSlowUpdate;

		private int _nextUpdateProcessSlot;

		private int _nextLateUpdateProcessSlot;

		private int _nextFixedUpdateProcessSlot;

		private int _nextSlowUpdateProcessSlot;

		private double _lastUpdateTime;

		private double _lastLateUpdateTime;

		private double _lastFixedUpdateTime;

		private double _lastSlowUpdateTime;

		private ushort _framesSinceUpdate;

		private ushort _expansions = 1;

		public Action<Exception> OnError;

		public static Func<IEnumerator<float>, Segment, string, IEnumerator<float>> ReplacementFunction;

		private readonly List<WaitingProcess> _waitingProcesses = new List<WaitingProcess>();

		private readonly Queue<Exception> _exceptions = new Queue<Exception>();

		private readonly Dictionary<ProcessIndex, string> _processTags = new Dictionary<ProcessIndex, string>();

		private readonly Dictionary<string, HashSet<ProcessIndex>> _taggedProcesses = new Dictionary<string, HashSet<ProcessIndex>>();

		private IEnumerator<float>[] UpdateProcesses = new IEnumerator<float>[256];

		private IEnumerator<float>[] LateUpdateProcesses = new IEnumerator<float>[8];

		private IEnumerator<float>[] FixedUpdateProcesses = new IEnumerator<float>[64];

		private IEnumerator<float>[] SlowUpdateProcesses = new IEnumerator<float>[64];

		private static Timing _instance;

		public static float LocalTime
		{
			get
			{
				return (float)Instance.localTime;
			}
		}

		public static float DeltaTime
		{
			get
			{
				return Instance.deltaTime;
			}
		}

		public static Timing Instance
		{
			get
			{
				if (_instance == null || !_instance.gameObject)
				{
					GameObject gameObject = GameObject.Find("Movement Effects");
					Type type = Type.GetType("MovementEffects.Movement, MovementOverTime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
					if (gameObject == null)
					{
						GameObject gameObject2 = new GameObject();
						gameObject2.name = "Movement Effects";
						gameObject = gameObject2;
						UnityEngine.Object.DontDestroyOnLoad(gameObject);
						if (type != null)
						{
							gameObject.AddComponent(type);
						}
						_instance = gameObject.AddComponent<Timing>();
					}
					else
					{
						if (type != null && gameObject.GetComponent(type) == null)
						{
							gameObject.AddComponent(type);
						}
						_instance = gameObject.GetComponent<Timing>() ?? gameObject.AddComponent<Timing>();
					}
				}
				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
			}
			else
			{
				deltaTime = _instance.deltaTime;
			}
		}

		private void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = null;
			}
		}

		private void Update()
		{
			if (_lastSlowUpdateTime + (double)TimeBetweenSlowUpdateCalls < (double)Time.realtimeSinceStartup && _nextSlowUpdateProcessSlot > 0)
			{
				ProcessIndex key = new ProcessIndex
				{
					seg = Segment.SlowUpdate
				};
				_runningSlowUpdate = true;
				UpdateTimeValues(key.seg);
				key.i = 0;
				while (key.i < _nextSlowUpdateProcessSlot)
				{
					if (SlowUpdateProcesses[key.i] != null && !(Time.realtimeSinceStartup < SlowUpdateProcesses[key.i].Current))
					{
						if (ProfilerDebugAmount != DebugInfoType.None)
						{
						}
						try
						{
							if (!SlowUpdateProcesses[key.i].MoveNext())
							{
								SlowUpdateProcesses[key.i] = null;
							}
							else if (SlowUpdateProcesses[key.i] != null && float.IsNaN(SlowUpdateProcesses[key.i].Current))
							{
								if (ReplacementFunction == null)
								{
									SlowUpdateProcesses[key.i] = null;
								}
								else
								{
									SlowUpdateProcesses[key.i] = ReplacementFunction(SlowUpdateProcesses[key.i], key.seg, (!_processTags.ContainsKey(key)) ? null : _processTags[key]);
									ReplacementFunction = null;
									key.i--;
								}
							}
						}
						catch (Exception ex)
						{
							if (OnError == null)
							{
								_exceptions.Enqueue(ex);
							}
							else
							{
								OnError(ex);
							}
							SlowUpdateProcesses[key.i] = null;
						}
						if (ProfilerDebugAmount == DebugInfoType.None)
						{
						}
					}
					key.i++;
				}
				_runningSlowUpdate = false;
			}
			if (_nextUpdateProcessSlot > 0)
			{
				ProcessIndex key2 = default(ProcessIndex);
				_runningUpdate = true;
				UpdateTimeValues(key2.seg);
				key2.i = 0;
				while (key2.i < _nextUpdateProcessSlot)
				{
					if (UpdateProcesses[key2.i] != null && !(localTime < (double)UpdateProcesses[key2.i].Current))
					{
						if (ProfilerDebugAmount != DebugInfoType.None)
						{
						}
						try
						{
							if (!UpdateProcesses[key2.i].MoveNext())
							{
								UpdateProcesses[key2.i] = null;
							}
							else if (UpdateProcesses[key2.i] != null && float.IsNaN(UpdateProcesses[key2.i].Current))
							{
								if (ReplacementFunction == null)
								{
									UpdateProcesses[key2.i] = null;
								}
								else
								{
									UpdateProcesses[key2.i] = ReplacementFunction(UpdateProcesses[key2.i], key2.seg, (!_processTags.ContainsKey(key2)) ? null : _processTags[key2]);
									ReplacementFunction = null;
									key2.i--;
								}
							}
						}
						catch (Exception ex2)
						{
							if (OnError == null)
							{
								_exceptions.Enqueue(ex2);
							}
							else
							{
								OnError(ex2);
							}
							UpdateProcesses[key2.i] = null;
						}
						if (ProfilerDebugAmount == DebugInfoType.None)
						{
						}
					}
					key2.i++;
				}
				_runningUpdate = false;
			}
			if (++_framesSinceUpdate > 64)
			{
				_framesSinceUpdate = 0;
				if (ProfilerDebugAmount != DebugInfoType.None)
				{
				}
				RemoveUnused();
				if (ProfilerDebugAmount == DebugInfoType.None)
				{
				}
			}
			if (_exceptions.Count > 0)
			{
				throw _exceptions.Dequeue();
			}
		}

		private void FixedUpdate()
		{
			if (_nextFixedUpdateProcessSlot > 0)
			{
				ProcessIndex key = new ProcessIndex
				{
					seg = Segment.FixedUpdate
				};
				_runningFixedUpdate = true;
				UpdateTimeValues(key.seg);
				key.i = 0;
				while (key.i < _nextFixedUpdateProcessSlot)
				{
					if (FixedUpdateProcesses[key.i] != null && !(localTime < (double)FixedUpdateProcesses[key.i].Current))
					{
						if (ProfilerDebugAmount != DebugInfoType.None)
						{
						}
						try
						{
							if (!FixedUpdateProcesses[key.i].MoveNext())
							{
								FixedUpdateProcesses[key.i] = null;
							}
							else if (FixedUpdateProcesses[key.i] != null && float.IsNaN(FixedUpdateProcesses[key.i].Current))
							{
								if (ReplacementFunction == null)
								{
									FixedUpdateProcesses[key.i] = null;
								}
								else
								{
									FixedUpdateProcesses[key.i] = ReplacementFunction(FixedUpdateProcesses[key.i], key.seg, (!_processTags.ContainsKey(key)) ? null : _processTags[key]);
									ReplacementFunction = null;
									key.i--;
								}
							}
						}
						catch (Exception ex)
						{
							if (OnError == null)
							{
								_exceptions.Enqueue(ex);
							}
							else
							{
								OnError(ex);
							}
							FixedUpdateProcesses[key.i] = null;
						}
						if (ProfilerDebugAmount == DebugInfoType.None)
						{
						}
					}
					key.i++;
				}
				_runningFixedUpdate = false;
			}
			if (_exceptions.Count > 0)
			{
				throw _exceptions.Dequeue();
			}
		}

		private void LateUpdate()
		{
			if (_nextLateUpdateProcessSlot > 0)
			{
				ProcessIndex key = new ProcessIndex
				{
					seg = Segment.LateUpdate
				};
				_runningLateUpdate = true;
				UpdateTimeValues(key.seg);
				key.i = 0;
				while (key.i < _nextLateUpdateProcessSlot)
				{
					if (LateUpdateProcesses[key.i] != null && !(localTime < (double)LateUpdateProcesses[key.i].Current))
					{
						if (ProfilerDebugAmount != DebugInfoType.None)
						{
						}
						try
						{
							if (!LateUpdateProcesses[key.i].MoveNext())
							{
								LateUpdateProcesses[key.i] = null;
							}
							else if (LateUpdateProcesses[key.i] != null && float.IsNaN(LateUpdateProcesses[key.i].Current))
							{
								if (ReplacementFunction == null)
								{
									LateUpdateProcesses[key.i] = null;
								}
								else
								{
									LateUpdateProcesses[key.i] = ReplacementFunction(LateUpdateProcesses[key.i], key.seg, (!_processTags.ContainsKey(key)) ? null : _processTags[key]);
									ReplacementFunction = null;
									key.i--;
								}
							}
						}
						catch (Exception ex)
						{
							if (OnError == null)
							{
								_exceptions.Enqueue(ex);
							}
							else
							{
								OnError(ex);
							}
							LateUpdateProcesses[key.i] = null;
						}
						if (ProfilerDebugAmount == DebugInfoType.None)
						{
						}
					}
					key.i++;
				}
				_runningLateUpdate = false;
			}
			if (_exceptions.Count > 0)
			{
				throw _exceptions.Dequeue();
			}
		}

		private void UpdateTimeValues(Segment segment)
		{
			switch (segment)
			{
			case Segment.Update:
				deltaTime = Time.deltaTime;
				_lastUpdateTime += deltaTime;
				localTime = _lastUpdateTime;
				break;
			case Segment.LateUpdate:
				deltaTime = Time.deltaTime;
				_lastLateUpdateTime += deltaTime;
				localTime = _lastLateUpdateTime;
				break;
			case Segment.FixedUpdate:
				deltaTime = Time.deltaTime;
				_lastFixedUpdateTime += deltaTime;
				localTime = _lastFixedUpdateTime;
				break;
			case Segment.SlowUpdate:
				if (_lastSlowUpdateTime == 0.0)
				{
					deltaTime = TimeBetweenSlowUpdateCalls;
				}
				else
				{
					deltaTime = Time.realtimeSinceStartup - (float)_lastSlowUpdateTime;
				}
				localTime = (_lastSlowUpdateTime = Time.realtimeSinceStartup);
				break;
			}
		}

		private void SetTimeValues(Segment segment)
		{
			switch (segment)
			{
			case Segment.Update:
				deltaTime = Time.deltaTime;
				localTime = _lastUpdateTime;
				break;
			case Segment.LateUpdate:
				deltaTime = Time.deltaTime;
				localTime = _lastLateUpdateTime;
				break;
			case Segment.FixedUpdate:
				deltaTime = Time.deltaTime;
				localTime = _lastFixedUpdateTime;
				break;
			case Segment.SlowUpdate:
				deltaTime = Time.realtimeSinceStartup - (float)_lastSlowUpdateTime;
				localTime = (_lastSlowUpdateTime = Time.realtimeSinceStartup);
				break;
			}
		}

		private double GetSegmentTime(Segment segment)
		{
			switch (segment)
			{
			case Segment.Update:
				return _lastUpdateTime;
			case Segment.LateUpdate:
				return _lastLateUpdateTime;
			case Segment.FixedUpdate:
				return _lastFixedUpdateTime;
			case Segment.SlowUpdate:
				return _lastSlowUpdateTime;
			default:
				return 0.0;
			}
		}

		public void ResetTimeCountOnInstance()
		{
			localTime = 0.0;
			_lastUpdateTime = 0.0;
			_lastLateUpdateTime = 0.0;
			_lastFixedUpdateTime = 0.0;
		}

		public static int PauseCoroutines()
		{
			return (!(_instance == null)) ? _instance.PauseCoroutinesOnInstance() : 0;
		}

		public int PauseCoroutinesOnInstance()
		{
			base.enabled = false;
			return _nextUpdateProcessSlot + _nextLateUpdateProcessSlot + _nextFixedUpdateProcessSlot + _nextSlowUpdateProcessSlot;
		}

		public static int PauseCoroutines(string tag)
		{
			return (!(_instance == null)) ? _instance.PauseCoroutinesOnInstance(tag) : 0;
		}

		public int PauseCoroutinesOnInstance(string tag)
		{
			if (tag == null)
			{
				return 0;
			}
			HashSet<ProcessIndex> value;
			if (!_taggedProcesses.TryGetValue(tag, out value))
			{
				return 0;
			}
			WaitingProcess waitingProcess = new WaitingProcess();
			int num = 0;
			HashSet<ProcessIndex>.Enumerator enumerator = value.GetEnumerator();
			while (enumerator.MoveNext() && _taggedProcesses.ContainsKey(tag))
			{
				IEnumerator<float> enumerator2 = CoindexExtract(enumerator.Current);
				if (enumerator2 == null)
				{
					RemoveTag(enumerator.Current);
					enumerator = value.GetEnumerator();
					continue;
				}
				WaitingProcess.ProcessData processData = new WaitingProcess.ProcessData();
				processData.Segment = enumerator.Current.seg;
				processData.Tag = RemoveTag(enumerator.Current);
				processData.Task = enumerator2;
				processData.PauseTime = ((!((double)enumerator2.Current > GetSegmentTime(enumerator.Current.seg))) ? 0.0 : ((double)enumerator2.Current - GetSegmentTime(enumerator.Current.seg)));
				WaitingProcess.ProcessData item = processData;
				waitingProcess.Tasks.Add(item);
				enumerator = value.GetEnumerator();
				num++;
			}
			_waitingProcesses.Add(waitingProcess);
			return num;
		}

		public static int ResumeCoroutines()
		{
			return (!(_instance == null)) ? _instance.ResumeCoroutinesOnInstance() : 0;
		}

		public int ResumeCoroutinesOnInstance()
		{
			base.enabled = true;
			int num = _nextUpdateProcessSlot + _nextLateUpdateProcessSlot + _nextFixedUpdateProcessSlot + _nextSlowUpdateProcessSlot;
			for (int i = 0; i < _waitingProcesses.Count; i++)
			{
				if (_waitingProcesses[i].Trigger != null)
				{
					continue;
				}
				for (int j = 0; j < _waitingProcesses[i].Tasks.Count; j++)
				{
					WaitingProcess.ProcessData processData = _waitingProcesses[i].Tasks[j];
					IEnumerator<float> coroutine;
					if (processData.PauseTime > 0.0)
					{
						IEnumerator<float> enumerator = InjectDelay(processData.Task, localTime + processData.PauseTime);
						coroutine = enumerator;
					}
					else
					{
						coroutine = processData.Task;
					}
					RunCoroutineOnInstance(coroutine, processData.Segment, processData.Tag);
					num++;
				}
				_waitingProcesses.RemoveAt(i--);
			}
			return num;
		}

		public static int ResumeCoroutines(string tag)
		{
			return (!(_instance == null)) ? _instance.ResumeCoroutinesOnInstance(tag) : 0;
		}

		public int ResumeCoroutinesOnInstance(string tag)
		{
			if (tag == null)
			{
				return 0;
			}
			int num = 0;
			for (int num2 = _waitingProcesses.Count - 1; num2 >= 0; num2--)
			{
				if (_waitingProcesses[num2].Trigger == null)
				{
					for (int num3 = _waitingProcesses[num2].Tasks.Count - 1; num3 >= 0; num3--)
					{
						if (_waitingProcesses[num2].Tasks[num3].Tag == tag)
						{
							WaitingProcess.ProcessData processData = _waitingProcesses[num2].Tasks[num3];
							IEnumerator<float> coroutine;
							if (processData.PauseTime > 0.0)
							{
								IEnumerator<float> enumerator = InjectDelay(processData.Task, GetSegmentTime(processData.Segment) + processData.PauseTime);
								coroutine = enumerator;
							}
							else
							{
								coroutine = processData.Task;
							}
							RunCoroutineOnInstance(coroutine, processData.Segment, processData.Tag);
							_waitingProcesses[num2].Tasks.RemoveAt(num3);
							num++;
						}
					}
					if (_waitingProcesses[num2].Tasks.Count == 0)
					{
						_waitingProcesses.RemoveAt(num2);
					}
				}
			}
			return num;
		}

		private void RemoveUnused()
		{
			ProcessIndex processIndex = default(ProcessIndex);
			ProcessIndex coindexTo = default(ProcessIndex);
			processIndex.seg = (coindexTo.seg = Segment.Update);
			processIndex.i = (coindexTo.i = 0);
			while (processIndex.i < _nextUpdateProcessSlot)
			{
				if (UpdateProcesses[processIndex.i] != null)
				{
					if (processIndex.i != coindexTo.i)
					{
						UpdateProcesses[coindexTo.i] = UpdateProcesses[processIndex.i];
						MoveTag(processIndex, coindexTo);
					}
					coindexTo.i++;
				}
				processIndex.i++;
			}
			processIndex.i = coindexTo.i;
			while (processIndex.i < _nextUpdateProcessSlot)
			{
				UpdateProcesses[processIndex.i] = null;
				RemoveTag(processIndex);
				processIndex.i++;
			}
			UpdateCoroutines = (_nextUpdateProcessSlot = coindexTo.i);
			processIndex.seg = (coindexTo.seg = Segment.FixedUpdate);
			processIndex.i = (coindexTo.i = 0);
			while (processIndex.i < _nextFixedUpdateProcessSlot)
			{
				if (FixedUpdateProcesses[processIndex.i] != null)
				{
					if (processIndex.i != coindexTo.i)
					{
						FixedUpdateProcesses[coindexTo.i] = FixedUpdateProcesses[processIndex.i];
						MoveTag(processIndex, coindexTo);
					}
					coindexTo.i++;
				}
				processIndex.i++;
			}
			processIndex.i = coindexTo.i;
			while (processIndex.i < _nextFixedUpdateProcessSlot)
			{
				FixedUpdateProcesses[processIndex.i] = null;
				RemoveTag(processIndex);
				processIndex.i++;
			}
			FixedUpdateCoroutines = (_nextFixedUpdateProcessSlot = coindexTo.i);
			processIndex.seg = (coindexTo.seg = Segment.LateUpdate);
			processIndex.i = (coindexTo.i = 0);
			while (processIndex.i < _nextLateUpdateProcessSlot)
			{
				if (LateUpdateProcesses[processIndex.i] != null)
				{
					if (processIndex.i != coindexTo.i)
					{
						LateUpdateProcesses[coindexTo.i] = LateUpdateProcesses[processIndex.i];
						MoveTag(processIndex, coindexTo);
					}
					coindexTo.i++;
				}
				processIndex.i++;
			}
			processIndex.i = coindexTo.i;
			while (processIndex.i < _nextLateUpdateProcessSlot)
			{
				LateUpdateProcesses[processIndex.i] = null;
				RemoveTag(processIndex);
				processIndex.i++;
			}
			LateUpdateCoroutines = (_nextLateUpdateProcessSlot = coindexTo.i);
			processIndex.seg = (coindexTo.seg = Segment.SlowUpdate);
			processIndex.i = (coindexTo.i = 0);
			while (processIndex.i < _nextSlowUpdateProcessSlot)
			{
				if (SlowUpdateProcesses[processIndex.i] != null)
				{
					if (processIndex.i != coindexTo.i)
					{
						SlowUpdateProcesses[coindexTo.i] = SlowUpdateProcesses[processIndex.i];
						MoveTag(processIndex, coindexTo);
					}
					coindexTo.i++;
				}
				processIndex.i++;
			}
			processIndex.i = coindexTo.i;
			while (processIndex.i < _nextSlowUpdateProcessSlot)
			{
				SlowUpdateProcesses[processIndex.i] = null;
				RemoveTag(processIndex);
				processIndex.i++;
			}
			SlowUpdateCoroutines = (_nextSlowUpdateProcessSlot = coindexTo.i);
		}

		private void AddTag(string tag, ProcessIndex coindex)
		{
			_processTags.Add(coindex, tag);
			if (_taggedProcesses.ContainsKey(tag))
			{
				_taggedProcesses[tag].Add(coindex);
				return;
			}
			_taggedProcesses.Add(tag, new HashSet<ProcessIndex> { coindex });
		}

		private string RemoveTag(ProcessIndex coindex)
		{
			if (_processTags.ContainsKey(coindex))
			{
				string text = _processTags[coindex];
				if (_taggedProcesses[text].Count > 1)
				{
					_taggedProcesses[text].Remove(coindex);
				}
				else
				{
					_taggedProcesses.Remove(text);
				}
				_processTags.Remove(coindex);
				return text;
			}
			return null;
		}

		private void MoveTag(ProcessIndex coindexFrom, ProcessIndex coindexTo)
		{
			RemoveTag(coindexTo);
			if (_processTags.ContainsKey(coindexFrom))
			{
				_taggedProcesses[_processTags[coindexFrom]].Remove(coindexFrom);
				_taggedProcesses[_processTags[coindexFrom]].Add(coindexTo);
				_processTags.Add(coindexTo, _processTags[coindexFrom]);
				_processTags.Remove(coindexFrom);
			}
		}

		public static IEnumerator<float> RunCoroutine(IEnumerator<float> coroutine)
		{
			IEnumerator<float> result;
			if (coroutine == null)
			{
				IEnumerator<float> enumerator = null;
				result = enumerator;
			}
			else
			{
				result = Instance.RunCoroutineOnInstance(coroutine, Segment.Update, null);
			}
			return result;
		}

		public static IEnumerator<float> RunCoroutine(IEnumerator<float> coroutine, string tag)
		{
			IEnumerator<float> result;
			if (coroutine == null)
			{
				IEnumerator<float> enumerator = null;
				result = enumerator;
			}
			else
			{
				result = Instance.RunCoroutineOnInstance(coroutine, Segment.Update, tag);
			}
			return result;
		}

		public static IEnumerator<float> RunCoroutine(IEnumerator<float> coroutine, Segment timing)
		{
			IEnumerator<float> result;
			if (coroutine == null)
			{
				IEnumerator<float> enumerator = null;
				result = enumerator;
			}
			else
			{
				result = Instance.RunCoroutineOnInstance(coroutine, timing);
			}
			return result;
		}

		public static IEnumerator<float> RunCoroutine(IEnumerator<float> coroutine, Segment timing, string tag)
		{
			IEnumerator<float> result;
			if (coroutine == null)
			{
				IEnumerator<float> enumerator = null;
				result = enumerator;
			}
			else
			{
				result = Instance.RunCoroutineOnInstance(coroutine, timing, tag);
			}
			return result;
		}

		public IEnumerator<float> RunCoroutineOnInstance(IEnumerator<float> coroutine)
		{
			IEnumerator<float> result;
			if (coroutine == null)
			{
				IEnumerator<float> enumerator = null;
				result = enumerator;
			}
			else
			{
				result = RunCoroutineOnInstance(coroutine, Segment.Update, null);
			}
			return result;
		}

		public IEnumerator<float> RunCoroutineOnInstance(IEnumerator<float> coroutine, string tag)
		{
			IEnumerator<float> result;
			if (coroutine == null)
			{
				IEnumerator<float> enumerator = null;
				result = enumerator;
			}
			else
			{
				result = RunCoroutineOnInstance(coroutine, Segment.Update, tag);
			}
			return result;
		}

		public IEnumerator<float> RunCoroutineOnInstance(IEnumerator<float> coroutine, Segment timing)
		{
			IEnumerator<float> result;
			if (coroutine == null)
			{
				IEnumerator<float> enumerator = null;
				result = enumerator;
			}
			else
			{
				result = RunCoroutineOnInstance(coroutine, timing, null);
			}
			return result;
		}

		public IEnumerator<float> RunCoroutineOnInstance(IEnumerator<float> coroutine, Segment timing, string tag)
		{
			if (coroutine == null)
			{
				return null;
			}
			ProcessIndex processIndex = new ProcessIndex
			{
				seg = timing
			};
			switch (timing)
			{
			case Segment.Update:
				if (_nextUpdateProcessSlot >= UpdateProcesses.Length)
				{
					IEnumerator<float>[] updateProcesses = UpdateProcesses;
					UpdateProcesses = new IEnumerator<float>[UpdateProcesses.Length + 64 * _expansions++];
					for (int k = 0; k < updateProcesses.Length; k++)
					{
						UpdateProcesses[k] = updateProcesses[k];
					}
				}
				processIndex.i = _nextUpdateProcessSlot++;
				UpdateProcesses[processIndex.i] = coroutine;
				if (tag != null)
				{
					AddTag(tag, processIndex);
				}
				if (!_runningUpdate)
				{
					try
					{
						_runningUpdate = true;
						SetTimeValues(processIndex.seg);
						if (!UpdateProcesses[processIndex.i].MoveNext())
						{
							UpdateProcesses[processIndex.i] = null;
						}
						else if (UpdateProcesses[processIndex.i] != null && float.IsNaN(UpdateProcesses[processIndex.i].Current))
						{
							if (ReplacementFunction == null)
							{
								UpdateProcesses[processIndex.i] = null;
							}
							else
							{
								UpdateProcesses[processIndex.i] = ReplacementFunction(UpdateProcesses[processIndex.i], timing, (!_processTags.ContainsKey(processIndex)) ? null : _processTags[processIndex]);
								ReplacementFunction = null;
								if (UpdateProcesses[processIndex.i] != null)
								{
									UpdateProcesses[processIndex.i].MoveNext();
								}
							}
						}
					}
					catch (Exception ex3)
					{
						if (OnError == null)
						{
							_exceptions.Enqueue(ex3);
						}
						else
						{
							OnError(ex3);
						}
						UpdateProcesses[processIndex.i] = null;
					}
					finally
					{
						_runningUpdate = false;
					}
				}
				return coroutine;
			case Segment.FixedUpdate:
				if (_nextFixedUpdateProcessSlot >= FixedUpdateProcesses.Length)
				{
					IEnumerator<float>[] fixedUpdateProcesses = FixedUpdateProcesses;
					FixedUpdateProcesses = new IEnumerator<float>[FixedUpdateProcesses.Length + 64 * _expansions++];
					for (int j = 0; j < fixedUpdateProcesses.Length; j++)
					{
						FixedUpdateProcesses[j] = fixedUpdateProcesses[j];
					}
				}
				processIndex.i = _nextFixedUpdateProcessSlot++;
				FixedUpdateProcesses[processIndex.i] = coroutine;
				if (tag != null)
				{
					AddTag(tag, processIndex);
				}
				if (!_runningFixedUpdate)
				{
					try
					{
						_runningFixedUpdate = true;
						SetTimeValues(processIndex.seg);
						if (!FixedUpdateProcesses[processIndex.i].MoveNext())
						{
							FixedUpdateProcesses[processIndex.i] = null;
						}
						else if (FixedUpdateProcesses[processIndex.i] != null && float.IsNaN(FixedUpdateProcesses[processIndex.i].Current))
						{
							if (ReplacementFunction == null)
							{
								FixedUpdateProcesses[processIndex.i] = null;
							}
							else
							{
								FixedUpdateProcesses[processIndex.i] = ReplacementFunction(FixedUpdateProcesses[processIndex.i], timing, (!_processTags.ContainsKey(processIndex)) ? null : _processTags[processIndex]);
								ReplacementFunction = null;
								if (FixedUpdateProcesses[processIndex.i] != null)
								{
									FixedUpdateProcesses[processIndex.i].MoveNext();
								}
							}
						}
					}
					catch (Exception ex2)
					{
						if (OnError == null)
						{
							_exceptions.Enqueue(ex2);
						}
						else
						{
							OnError(ex2);
						}
						FixedUpdateProcesses[processIndex.i] = null;
					}
					finally
					{
						_runningFixedUpdate = false;
					}
				}
				return coroutine;
			case Segment.LateUpdate:
				if (_nextLateUpdateProcessSlot >= LateUpdateProcesses.Length)
				{
					IEnumerator<float>[] lateUpdateProcesses = LateUpdateProcesses;
					LateUpdateProcesses = new IEnumerator<float>[LateUpdateProcesses.Length + 64 * _expansions++];
					for (int l = 0; l < lateUpdateProcesses.Length; l++)
					{
						LateUpdateProcesses[l] = lateUpdateProcesses[l];
					}
				}
				processIndex.i = _nextLateUpdateProcessSlot++;
				LateUpdateProcesses[processIndex.i] = coroutine;
				if (tag != null)
				{
					AddTag(tag, processIndex);
				}
				if (!_runningLateUpdate)
				{
					try
					{
						_runningLateUpdate = true;
						SetTimeValues(processIndex.seg);
						if (!LateUpdateProcesses[processIndex.i].MoveNext())
						{
							LateUpdateProcesses[processIndex.i] = null;
						}
						else if (LateUpdateProcesses[processIndex.i] != null && float.IsNaN(LateUpdateProcesses[processIndex.i].Current))
						{
							if (ReplacementFunction == null)
							{
								LateUpdateProcesses[processIndex.i] = null;
							}
							else
							{
								LateUpdateProcesses[processIndex.i] = ReplacementFunction(LateUpdateProcesses[processIndex.i], timing, (!_processTags.ContainsKey(processIndex)) ? null : _processTags[processIndex]);
								ReplacementFunction = null;
								if (LateUpdateProcesses[processIndex.i] != null)
								{
									LateUpdateProcesses[processIndex.i].MoveNext();
								}
							}
						}
					}
					catch (Exception ex4)
					{
						if (OnError == null)
						{
							_exceptions.Enqueue(ex4);
						}
						else
						{
							OnError(ex4);
						}
						LateUpdateProcesses[processIndex.i] = null;
					}
					finally
					{
						_runningLateUpdate = false;
					}
				}
				return coroutine;
			case Segment.SlowUpdate:
				if (_nextSlowUpdateProcessSlot >= SlowUpdateProcesses.Length)
				{
					IEnumerator<float>[] slowUpdateProcesses = SlowUpdateProcesses;
					SlowUpdateProcesses = new IEnumerator<float>[SlowUpdateProcesses.Length + 64 * _expansions++];
					for (int i = 0; i < slowUpdateProcesses.Length; i++)
					{
						SlowUpdateProcesses[i] = slowUpdateProcesses[i];
					}
				}
				processIndex.i = _nextSlowUpdateProcessSlot++;
				SlowUpdateProcesses[processIndex.i] = coroutine;
				if (tag != null)
				{
					AddTag(tag, processIndex);
				}
				if (!_runningSlowUpdate)
				{
					try
					{
						_runningSlowUpdate = true;
						SetTimeValues(processIndex.seg);
						if (!SlowUpdateProcesses[processIndex.i].MoveNext())
						{
							SlowUpdateProcesses[processIndex.i] = null;
						}
						else if (SlowUpdateProcesses[processIndex.i] != null && float.IsNaN(SlowUpdateProcesses[processIndex.i].Current))
						{
							if (ReplacementFunction == null)
							{
								SlowUpdateProcesses[processIndex.i] = null;
							}
							else
							{
								SlowUpdateProcesses[processIndex.i] = ReplacementFunction(SlowUpdateProcesses[processIndex.i], timing, (!_processTags.ContainsKey(processIndex)) ? null : _processTags[processIndex]);
								ReplacementFunction = null;
								if (SlowUpdateProcesses[processIndex.i] != null)
								{
									SlowUpdateProcesses[processIndex.i].MoveNext();
								}
							}
						}
					}
					catch (Exception ex)
					{
						if (OnError == null)
						{
							_exceptions.Enqueue(ex);
						}
						else
						{
							OnError(ex);
						}
						SlowUpdateProcesses[processIndex.i] = null;
					}
					finally
					{
						_runningSlowUpdate = false;
					}
				}
				return coroutine;
			default:
				return null;
			}
		}

		private bool CoindexKill(ProcessIndex coindex)
		{
			switch (coindex.seg)
			{
			case Segment.Update:
			{
				bool result = UpdateProcesses[coindex.i] != null;
				UpdateProcesses[coindex.i] = null;
				return result;
			}
			case Segment.FixedUpdate:
			{
				bool result = FixedUpdateProcesses[coindex.i] != null;
				FixedUpdateProcesses[coindex.i] = null;
				return result;
			}
			case Segment.LateUpdate:
			{
				bool result = LateUpdateProcesses[coindex.i] != null;
				LateUpdateProcesses[coindex.i] = null;
				return result;
			}
			case Segment.SlowUpdate:
			{
				bool result = SlowUpdateProcesses[coindex.i] != null;
				SlowUpdateProcesses[coindex.i] = null;
				return result;
			}
			default:
				return false;
			}
		}

		private IEnumerator<float> CoindexExtract(ProcessIndex coindex)
		{
			switch (coindex.seg)
			{
			case Segment.Update:
			{
				IEnumerator<float> result = UpdateProcesses[coindex.i];
				UpdateProcesses[coindex.i] = null;
				return result;
			}
			case Segment.FixedUpdate:
			{
				IEnumerator<float> result = FixedUpdateProcesses[coindex.i];
				FixedUpdateProcesses[coindex.i] = null;
				return result;
			}
			case Segment.LateUpdate:
			{
				IEnumerator<float> result = LateUpdateProcesses[coindex.i];
				LateUpdateProcesses[coindex.i] = null;
				return result;
			}
			case Segment.SlowUpdate:
			{
				IEnumerator<float> result = SlowUpdateProcesses[coindex.i];
				SlowUpdateProcesses[coindex.i] = null;
				return result;
			}
			default:
				return null;
			}
		}

		private bool CoindexMatches(ProcessIndex coindex, IEnumerator<float> handle)
		{
			switch (coindex.seg)
			{
			case Segment.Update:
				return UpdateProcesses[coindex.i] == handle;
			case Segment.FixedUpdate:
				return FixedUpdateProcesses[coindex.i] == handle;
			case Segment.LateUpdate:
				return LateUpdateProcesses[coindex.i] == handle;
			case Segment.SlowUpdate:
				return SlowUpdateProcesses[coindex.i] == handle;
			default:
				return false;
			}
		}

		[DebuggerHidden]
		private static IEnumerator<float> InjectDelay(IEnumerator<float> proc, double returnAt)
		{
			//yield-return decompiler failed: Could not find currentField
			_003CInjectDelay_003Ec__Iterator0 obj = new _003CInjectDelay_003Ec__Iterator0();
			obj.returnAt = returnAt;
			obj.proc = proc;
			obj._003C_0024_003EreturnAt = returnAt;
			obj._003C_0024_003Eproc = proc;
			return obj;
		}

		public static void KillAllCoroutines()
		{
			if (_instance != null)
			{
				_instance.KillAllCoroutinesOnInstance();
			}
		}

		public void KillAllCoroutinesOnInstance()
		{
			UpdateProcesses = new IEnumerator<float>[256];
			UpdateCoroutines = 0;
			_nextUpdateProcessSlot = 0;
			LateUpdateProcesses = new IEnumerator<float>[8];
			LateUpdateCoroutines = 0;
			_nextLateUpdateProcessSlot = 0;
			FixedUpdateProcesses = new IEnumerator<float>[64];
			FixedUpdateCoroutines = 0;
			_nextFixedUpdateProcessSlot = 0;
			SlowUpdateProcesses = new IEnumerator<float>[64];
			SlowUpdateCoroutines = 0;
			_nextSlowUpdateProcessSlot = 0;
			_processTags.Clear();
			_taggedProcesses.Clear();
			_waitingProcesses.Clear();
			_exceptions.Clear();
			_expansions = (ushort)(_expansions / 2 + 1);
			ResetTimeCountOnInstance();
		}

		[Obsolete("Please use coroutine tags to identify any coroutines you want to kill.")]
		public static int KillCoroutines(IEnumerator<float> coroutine)
		{
			return (!(_instance == null)) ? _instance.KillCoroutinesOnInstance(coroutine) : 0;
		}

		[Obsolete("Please use coroutine tags to identify any coroutines you want to kill.")]
		public int KillCoroutinesOnInstance(IEnumerator<float> coroutine)
		{
			int num = 0;
			for (int i = 0; i < _nextUpdateProcessSlot; i++)
			{
				if (UpdateProcesses[i] == coroutine)
				{
					UpdateProcesses[i] = null;
					num++;
				}
			}
			for (int j = 0; j < _nextFixedUpdateProcessSlot; j++)
			{
				if (FixedUpdateProcesses[j] == coroutine)
				{
					FixedUpdateProcesses[j] = null;
					num++;
				}
			}
			for (int k = 0; k < _nextLateUpdateProcessSlot; k++)
			{
				if (LateUpdateProcesses[k] == coroutine)
				{
					LateUpdateProcesses[k] = null;
					num++;
				}
			}
			for (int l = 0; l < _nextSlowUpdateProcessSlot; l++)
			{
				if (SlowUpdateProcesses[l] == coroutine)
				{
					SlowUpdateProcesses[l] = null;
					num++;
				}
			}
			for (int m = 0; m < _waitingProcesses.Count; m++)
			{
				if (_waitingProcesses[m].Trigger == coroutine && !_waitingProcesses[m].Killed && !_waitingProcesses[m].Killed)
				{
					_waitingProcesses[m].Killed = true;
					num++;
				}
				for (int n = 0; n < _waitingProcesses[m].Tasks.Count; n++)
				{
					if (_waitingProcesses[m].Tasks[n].Task == coroutine && _waitingProcesses[m].Tasks[n].Task != null)
					{
						_waitingProcesses[m].Tasks[n].Task = null;
						num++;
					}
				}
			}
			return num;
		}

		public static int KillCoroutines(string tag)
		{
			return (!(_instance == null)) ? _instance.KillCoroutinesOnInstance(tag) : 0;
		}

		public int KillCoroutinesOnInstance(string tag)
		{
			int num = 0;
			while (_taggedProcesses.ContainsKey(tag))
			{
				HashSet<ProcessIndex>.Enumerator enumerator = _taggedProcesses[tag].GetEnumerator();
				enumerator.MoveNext();
				if (CoindexKill(enumerator.Current))
				{
					num++;
				}
				RemoveTag(enumerator.Current);
			}
			for (int i = 0; i < _waitingProcesses.Count; i++)
			{
				if (_waitingProcesses[i].TriggerTag == tag && !_waitingProcesses[i].Killed)
				{
					_waitingProcesses[i].Killed = true;
					num++;
				}
				for (int j = 0; j < _waitingProcesses[i].Tasks.Count; j++)
				{
					if (_waitingProcesses[i].Tasks[j].Tag == tag && _waitingProcesses[i].Tasks[j].Task != null)
					{
						_waitingProcesses[i].Tasks[j].Task = null;
						num++;
					}
				}
			}
			return num;
		}

		[Obsolete("Please use coroutine tags to identify any coroutines you want to kill.")]
		public static int KillAllCoroutines(IEnumerator<float> coroutine, string tag)
		{
			return (!(_instance == null)) ? _instance.KillAllCoroutinesOnInstance(coroutine, tag) : 0;
		}

		[Obsolete("Please use coroutine tags to identify any coroutines you want to kill.")]
		public int KillAllCoroutinesOnInstance(IEnumerator<float> coroutine, string tag)
		{
			int num = 0;
			if (_taggedProcesses.ContainsKey(tag))
			{
				foreach (ProcessIndex item in _taggedProcesses[tag])
				{
					if (CoindexMatches(item, coroutine))
					{
						CoindexKill(item);
						_processTags.Remove(item);
						num++;
					}
				}
				if (num == _taggedProcesses[tag].Count)
				{
					_taggedProcesses.Remove(tag);
				}
			}
			for (int i = 0; i < _waitingProcesses.Count; i++)
			{
				if (_waitingProcesses[i].Trigger == coroutine && _waitingProcesses[i].TriggerTag == tag && !_waitingProcesses[i].Killed && !_waitingProcesses[i].Killed)
				{
					_waitingProcesses[i].Killed = true;
					num++;
				}
				for (int j = 0; j < _waitingProcesses[i].Tasks.Count; j++)
				{
					if (_waitingProcesses[i].Tasks[j].Task == coroutine && _waitingProcesses[i].Tasks[j].Tag == tag && _waitingProcesses[i].Tasks[j].Task != null)
					{
						_waitingProcesses[i].Tasks[j].Task = null;
						num++;
					}
				}
			}
			return num;
		}

		public static float WaitUntilDone(IEnumerator<float> otherCoroutine)
		{
			return WaitUntilDone(otherCoroutine, true, Instance);
		}

		public static float WaitUntilDone(IEnumerator<float> otherCoroutine, bool warnOnIssue)
		{
			return WaitUntilDone(otherCoroutine, warnOnIssue, Instance);
		}

		public static float WaitUntilDone(IEnumerator<float> otherCoroutine, bool warnOnIssue, Timing instance)
		{
			if (instance == null || !instance.gameObject)
			{
				throw new ArgumentNullException();
			}
			if (otherCoroutine == null)
			{
				if (warnOnIssue)
				{
					throw new ArgumentNullException();
				}
				return -1f;
			}
			for (int i = 0; i < instance._waitingProcesses.Count; i++)
			{
				if (instance._waitingProcesses[i].Trigger == otherCoroutine)
				{
					WaitingProcess proc = instance._waitingProcesses[i];
					ReplacementFunction = (IEnumerator<float> input, Segment segment, string tag) =>
					{
						proc.Tasks.Add(new WaitingProcess.ProcessData
						{
							Task = input,
							Tag = tag,
							Segment = segment,
							PauseTime = ((!((double)input.Current > instance.GetSegmentTime(segment))) ? 0.0 : ((double)input.Current - instance.GetSegmentTime(segment)))
						});
						return (IEnumerator<float>)null;
					};
					return float.NaN;
				}
				for (int num = 0; num < instance._waitingProcesses[i].Tasks.Count; num++)
				{
					if (instance._waitingProcesses[i].Tasks[num].Task == otherCoroutine)
					{
						WaitingProcess proc2 = new WaitingProcess
						{
							Trigger = otherCoroutine
						};
						instance._waitingProcesses[i].Tasks[num].Task = instance._StartWhenDone(proc2);
						ReplacementFunction = (IEnumerator<float> input, Segment segment, string tag) =>
						{
							proc2.Tasks.Add(new WaitingProcess.ProcessData
							{
								Task = input,
								Tag = tag,
								Segment = segment,
								PauseTime = ((!((double)input.Current > instance.GetSegmentTime(segment))) ? 0.0 : ((double)input.Current - instance.GetSegmentTime(segment)))
							});
							instance._waitingProcesses.Add(proc2);
							return (IEnumerator<float>)null;
						};
						return float.NaN;
					}
				}
			}
			WaitingProcess newProcess = new WaitingProcess
			{
				Trigger = otherCoroutine
			};
			if (instance.ReplaceCoroutine(otherCoroutine, instance._StartWhenDone(newProcess), out newProcess.TriggerTag))
			{
				ReplacementFunction = (IEnumerator<float> input, Segment segment, string tag) =>
				{
					newProcess.Tasks.Add(new WaitingProcess.ProcessData
					{
						Task = input,
						Tag = tag,
						Segment = segment,
						PauseTime = ((!((double)input.Current > instance.GetSegmentTime(segment))) ? 0.0 : ((double)input.Current - instance.GetSegmentTime(segment)))
					});
					instance._waitingProcesses.Add(newProcess);
					return (IEnumerator<float>)null;
				};
				return float.NaN;
			}
			if (warnOnIssue)
			{
				UnityEngine.Debug.LogWarning("WaitUntilDone cannot hold: The coroutine instance that was passed in was not found.\n" + otherCoroutine);
			}
			return -1f;
		}

		[DebuggerHidden]
		private IEnumerator<float> _StartWhenDone(WaitingProcess processData)
		{
			//yield-return decompiler failed: Could not find currentField
			_003C_StartWhenDone_003Ec__Iterator1 obj = new _003C_StartWhenDone_003Ec__Iterator1();
			obj.processData = processData;
			obj._003C_0024_003EprocessData = processData;
			obj._003C_003Ef__this = this;
			return obj;
		}

		private void CloseWaitingProcess(WaitingProcess processData)
		{
			if (!_waitingProcesses.Contains(processData))
			{
				return;
			}
			_waitingProcesses.Remove(processData);
			for (int i = 0; i < processData.Tasks.Count; i++)
			{
				IEnumerator<float> coroutine;
				if (processData.Tasks[i].PauseTime > 0.0)
				{
					IEnumerator<float> enumerator = InjectDelay(processData.Tasks[i].Task, GetSegmentTime(processData.Tasks[i].Segment) + processData.Tasks[i].PauseTime);
					coroutine = enumerator;
				}
				else
				{
					coroutine = processData.Tasks[i].Task;
				}
				RunCoroutineOnInstance(coroutine, processData.Tasks[i].Segment, processData.Tasks[i].Tag);
			}
		}

		private bool ReplaceCoroutine(IEnumerator<float> coroutine, IEnumerator<float> replacement, out string tagFound)
		{
			ProcessIndex coindex = default(ProcessIndex);
			coindex.i = 0;
			while (coindex.i < _nextUpdateProcessSlot)
			{
				if (UpdateProcesses[coindex.i] == coroutine)
				{
					coindex.seg = Segment.Update;
					UpdateProcesses[coindex.i] = replacement;
					tagFound = RemoveTag(coindex);
					return true;
				}
				coindex.i++;
			}
			coindex.i = 0;
			while (coindex.i < _nextFixedUpdateProcessSlot)
			{
				if (FixedUpdateProcesses[coindex.i] == coroutine)
				{
					coindex.seg = Segment.FixedUpdate;
					FixedUpdateProcesses[coindex.i] = replacement;
					tagFound = RemoveTag(coindex);
					return true;
				}
				coindex.i++;
			}
			coindex.i = 0;
			while (coindex.i < _nextLateUpdateProcessSlot)
			{
				if (LateUpdateProcesses[coindex.i] == coroutine)
				{
					coindex.seg = Segment.LateUpdate;
					LateUpdateProcesses[coindex.i] = replacement;
					tagFound = RemoveTag(coindex);
					return true;
				}
				coindex.i++;
			}
			coindex.i = 0;
			while (coindex.i < _nextSlowUpdateProcessSlot)
			{
				if (SlowUpdateProcesses[coindex.i] == coroutine)
				{
					coindex.seg = Segment.SlowUpdate;
					SlowUpdateProcesses[coindex.i] = replacement;
					tagFound = RemoveTag(coindex);
					return true;
				}
				coindex.i++;
			}
			tagFound = null;
			return false;
		}

		public static float WaitUntilDone(WWW wwwObject)
		{
			ReplacementFunction = (IEnumerator<float> input, Segment timing, string tag) => _StartWhenDone(wwwObject, input);
			return float.NaN;
		}

		[DebuggerHidden]
		private static IEnumerator<float> _StartWhenDone(WWW www, IEnumerator<float> pausedProc)
		{
			//yield-return decompiler failed: Could not find currentField
			_003C_StartWhenDone_003Ec__Iterator2 obj = new _003C_StartWhenDone_003Ec__Iterator2();
			obj.www = www;
			obj.pausedProc = pausedProc;
			obj._003C_0024_003Ewww = www;
			obj._003C_0024_003EpausedProc = pausedProc;
			return obj;
		}

		public static float WaitUntilDone(AsyncOperation operation)
		{
			if (operation == null || operation.isDone)
			{
				return 0f;
			}
			ReplacementFunction = (IEnumerator<float> input, Segment timing, string tag) => _StartWhenDone(operation, input);
			return float.NaN;
		}

		[DebuggerHidden]
		private static IEnumerator<float> _StartWhenDone(AsyncOperation operation, IEnumerator<float> pausedProc)
		{
			//yield-return decompiler failed: Could not find currentField
			_003C_StartWhenDone_003Ec__Iterator3 obj = new _003C_StartWhenDone_003Ec__Iterator3();
			obj.operation = operation;
			obj.pausedProc = pausedProc;
			obj._003C_0024_003Eoperation = operation;
			obj._003C_0024_003EpausedProc = pausedProc;
			return obj;
		}

		public static float WaitForSeconds(float waitTime)
		{
			if (float.IsNaN(waitTime))
			{
				waitTime = 0f;
			}
			return LocalTime + waitTime;
		}

		public float WaitForSecondsOnInstance(float waitTime)
		{
			if (float.IsNaN(waitTime))
			{
				waitTime = 0f;
			}
			return (float)localTime + waitTime;
		}

		public static void CallDelayed<TRef>(TRef reference, float delay, Action<TRef> action)
		{
			if (action != null)
			{
				if (delay >= -0.001f)
				{
					RunCoroutine(Instance._CallDelayBack(reference, delay, action));
				}
				else
				{
					action(reference);
				}
			}
		}

		public void CallDelayedOnInstance<TRef>(TRef reference, float delay, Action<TRef> action)
		{
			if (action != null)
			{
				if (delay >= -0.001f)
				{
					RunCoroutineOnInstance(_CallDelayBack(reference, delay, action));
				}
				else
				{
					action(reference);
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator<float> _CallDelayBack<TRef>(TRef reference, float delay, Action<TRef> action)
		{
			//yield-return decompiler failed: Could not find currentField
			_003C_CallDelayBack_003Ec__Iterator4<TRef> obj = new _003C_CallDelayBack_003Ec__Iterator4<TRef>();
			obj.delay = delay;
			obj.reference = reference;
			obj.action = action;
			obj._003C_0024_003Edelay = delay;
			obj._003C_0024_003Ereference = reference;
			obj._003C_0024_003Eaction = action;
			obj._003C_003Ef__this = this;
			return obj;
		}

		public static void CallDelayed(float delay, Action action)
		{
			if (action != null)
			{
				if (delay >= -0.0001f)
				{
					RunCoroutine(Instance._CallDelayBack(delay, action));
				}
				else
				{
					action();
				}
			}
		}

		public void CallDelayedOnInstance(float delay, Action action)
		{
			if (action != null)
			{
				if (delay >= -0.0001f)
				{
					RunCoroutineOnInstance(_CallDelayBack(delay, action));
				}
				else
				{
					action();
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator<float> _CallDelayBack(float delay, Action action)
		{
			//yield-return decompiler failed: Could not find currentField
			_003C_CallDelayBack_003Ec__Iterator5 obj = new _003C_CallDelayBack_003Ec__Iterator5();
			obj.delay = delay;
			obj.action = action;
			obj._003C_0024_003Edelay = delay;
			obj._003C_0024_003Eaction = action;
			obj._003C_003Ef__this = this;
			return obj;
		}

		public static void CallPeriodically(float timeframe, float period, Action action, Action onDone = null)
		{
			if (action != null)
			{
				RunCoroutine(Instance._CallContinuously(timeframe, period, action, onDone), Segment.Update);
			}
		}

		public void CallPeriodicallyOnInstance(float timeframe, float period, Action action, Action onDone = null)
		{
			if (action != null)
			{
				RunCoroutineOnInstance(_CallContinuously(timeframe, period, action, onDone), Segment.Update);
			}
		}

		public static void CallPeriodically(float timeframe, float period, Action action, Segment timing, Action onDone = null)
		{
			if (action != null)
			{
				RunCoroutine(Instance._CallContinuously(timeframe, period, action, onDone), timing);
			}
		}

		public void CallPeriodicallyOnInstance(float timeframe, float period, Action action, Segment timing, Action onDone = null)
		{
			if (action != null)
			{
				RunCoroutineOnInstance(_CallContinuously(timeframe, period, action, onDone), timing);
			}
		}

		public static void CallContinuously(float timeframe, Action action, Action onDone = null)
		{
			if (action != null)
			{
				RunCoroutine(Instance._CallContinuously(timeframe, 0f, action, onDone), Segment.Update);
			}
		}

		public void CallContinuouslyOnInstance(float timeframe, Action action, Action onDone = null)
		{
			if (action != null)
			{
				RunCoroutineOnInstance(_CallContinuously(timeframe, 0f, action, onDone), Segment.Update);
			}
		}

		public static void CallContinuously(float timeframe, Action action, Segment timing, Action onDone = null)
		{
			if (action != null)
			{
				RunCoroutine(Instance._CallContinuously(timeframe, 0f, action, onDone), timing);
			}
		}

		public void CallContinuouslyOnInstance(float timeframe, Action action, Segment timing, Action onDone = null)
		{
			if (action != null)
			{
				RunCoroutineOnInstance(_CallContinuously(timeframe, 0f, action, onDone), timing);
			}
		}

		[DebuggerHidden]
		private IEnumerator<float> _CallContinuously(float timeframe, float period, Action action, Action onDone)
		{
			//yield-return decompiler failed: Could not find currentField
			_003C_CallContinuously_003Ec__Iterator6 obj = new _003C_CallContinuously_003Ec__Iterator6();
			obj.timeframe = timeframe;
			obj.period = period;
			obj.action = action;
			obj.onDone = onDone;
			obj._003C_0024_003Etimeframe = timeframe;
			obj._003C_0024_003Eperiod = period;
			obj._003C_0024_003Eaction = action;
			obj._003C_0024_003EonDone = onDone;
			obj._003C_003Ef__this = this;
			return obj;
		}

		public static void CallPeriodically<T>(T reference, float timeframe, float period, Action<T> action, Action<T> onDone = null)
		{
			if (action != null)
			{
				RunCoroutine(Instance._CallContinuously(reference, timeframe, period, action, onDone), Segment.Update);
			}
		}

		public void CallPeriodicallyOnInstance<T>(T reference, float timeframe, float period, Action<T> action, Action<T> onDone = null)
		{
			if (action != null)
			{
				RunCoroutineOnInstance(_CallContinuously(reference, timeframe, period, action, onDone), Segment.Update);
			}
		}

		public static void CallPeriodically<T>(T reference, float timeframe, float period, Action<T> action, Segment timing, Action<T> onDone = null)
		{
			if (action != null)
			{
				RunCoroutine(Instance._CallContinuously(reference, timeframe, period, action, onDone), timing);
			}
		}

		public void CallPeriodicallyOnInstance<T>(T reference, float timeframe, float period, Action<T> action, Segment timing, Action<T> onDone = null)
		{
			if (action != null)
			{
				RunCoroutineOnInstance(_CallContinuously(reference, timeframe, period, action, onDone), timing);
			}
		}

		public static void CallContinuously<T>(T reference, float timeframe, Action<T> action, Action<T> onDone = null)
		{
			if (action != null)
			{
				RunCoroutine(Instance._CallContinuously(reference, timeframe, 0f, action, onDone), Segment.Update);
			}
		}

		public void CallContinuouslyOnInstance<T>(T reference, float timeframe, Action<T> action, Action<T> onDone = null)
		{
			if (action != null)
			{
				RunCoroutineOnInstance(_CallContinuously(reference, timeframe, 0f, action, onDone), Segment.Update);
			}
		}

		public static void CallContinuously<T>(T reference, float timeframe, Action<T> action, Segment timing, Action<T> onDone = null)
		{
			if (action != null)
			{
				RunCoroutine(Instance._CallContinuously(reference, timeframe, 0f, action, onDone), timing);
			}
		}

		public void CallContinuouslyOnInstance<T>(T reference, float timeframe, Action<T> action, Segment timing, Action<T> onDone = null)
		{
			if (action != null)
			{
				RunCoroutineOnInstance(_CallContinuously(reference, timeframe, 0f, action, onDone), timing);
			}
		}

		[DebuggerHidden]
		private IEnumerator<float> _CallContinuously<T>(T reference, float timeframe, float period, Action<T> action, Action<T> onDone = null)
		{
			//yield-return decompiler failed: Could not find currentField
			_003C_CallContinuously_003Ec__Iterator7<T> obj = new _003C_CallContinuously_003Ec__Iterator7<T>();
			obj.timeframe = timeframe;
			obj.period = period;
			obj.action = action;
			obj.reference = reference;
			obj.onDone = onDone;
			obj._003C_0024_003Etimeframe = timeframe;
			obj._003C_0024_003Eperiod = period;
			obj._003C_0024_003Eaction = action;
			obj._003C_0024_003Ereference = reference;
			obj._003C_0024_003EonDone = onDone;
			obj._003C_003Ef__this = this;
			return obj;
		}

		[Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
		public new Coroutine StartCoroutine(IEnumerator routine)
		{
			return null;
		}

		[Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
		public new Coroutine StartCoroutine(string methodName, object value)
		{
			return null;
		}

		[Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
		public new Coroutine StartCoroutine(string methodName)
		{
			return null;
		}

		[Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
		public new Coroutine StartCoroutine_Auto(IEnumerator routine)
		{
			return null;
		}

		[Obsolete("Unity coroutine function, use KillCoroutine instead.", true)]
		public new void StopCoroutine(string methodName)
		{
		}

		[Obsolete("Unity coroutine function, use KillCoroutine instead.", true)]
		public new void StopCoroutine(IEnumerator routine)
		{
		}

		[Obsolete("Unity coroutine function, use KillCoroutine instead.", true)]
		public new void StopCoroutine(Coroutine routine)
		{
		}

		[Obsolete("Unity coroutine function, use KillAllCoroutines instead.", true)]
		public new void StopAllCoroutines()
		{
		}

		[Obsolete("Use your own GameObject for this.", true)]
		public new static void Destroy(UnityEngine.Object obj)
		{
		}

		[Obsolete("Use your own GameObject for this.", true)]
		public new static void Destroy(UnityEngine.Object obj, float f)
		{
		}

		[Obsolete("Use your own GameObject for this.", true)]
		public new static void DestroyObject(UnityEngine.Object obj)
		{
		}

		[Obsolete("Use your own GameObject for this.", true)]
		public new static void DestroyObject(UnityEngine.Object obj, float f)
		{
		}

		[Obsolete("Use your own GameObject for this.", true)]
		public new static void DestroyImmediate(UnityEngine.Object obj)
		{
		}

		[Obsolete("Use your own GameObject for this.", true)]
		public new static void DestroyImmediate(UnityEngine.Object obj, bool b)
		{
		}

		[Obsolete("Just.. no.", true)]
		public new static T FindObjectOfType<T>() where T : UnityEngine.Object
		{
			return (T)null;
		}

		[Obsolete("Just.. no.", true)]
		public new static UnityEngine.Object FindObjectOfType(Type t)
		{
			return null;
		}

		[Obsolete("Just.. no.", true)]
		public new static T[] FindObjectsOfType<T>() where T : UnityEngine.Object
		{
			return null;
		}

		[Obsolete("Just.. no.", true)]
		public new static UnityEngine.Object[] FindObjectsOfType(Type t)
		{
			return null;
		}

		[Obsolete("Just.. no.", true)]
		public new static void print(object message)
		{
		}
	}
}
