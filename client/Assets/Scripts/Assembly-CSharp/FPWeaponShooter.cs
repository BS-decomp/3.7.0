using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DG.Tweening;
using FreeJSON;
using MovementEffects;
using UnityEngine;

public class FPWeaponShooter : MonoBehaviour
{
	[Serializable]
	public class ShowWeaponSettings
	{
		public float Duration = 0.5f;

		public Vector3 Position;

		public Vector3 Rotation;
	}

	[CompilerGenerated]
	private sealed class _003CShowWeaponCoroutine_003Ec__Iterator21 : IDisposable, IEnumerator, IEnumerator<float>
	{
		internal int _003Ci_003E__0;

		internal int _0024PC;

		internal float _0024current;

		internal FPWeaponShooter _003C_003Ef__this;

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
				if (_003C_003Ef__this.ShowHandLeft != null)
				{
					if (_003C_003Ef__this.ShowHadLeftDefault == 0f)
					{
						_003C_003Ef__this.ShowHadLeftDefault = _003C_003Ef__this.ShowHandLeft.localPosition.y;
					}
					_003C_003Ef__this.ShowHandLeftTweener = _003C_003Ef__this.ShowHandLeft.DOLocalMoveY(-1f, 0.5f);
					vp_Timer.In(0.5f, () =>
					{
						if (_003C_003Ef__this.Show)
						{
							_003C_003Ef__this.ShowHandLeft.gameObject.SetActive(false);
						}
					});
				}
				_003Ci_003E__0 = 0;
				goto IL_01a8;
			case 1u:
				_003Ci_003E__0++;
				goto IL_01a8;
			case 2u:
				_003C_003Ef__this.ShowHandLeft.gameObject.SetActive(true);
				_003C_003Ef__this.ShowHandLeftTweener = _003C_003Ef__this.ShowHandLeft.DOLocalMoveY(_003C_003Ef__this.ShowHadLeftDefault, 0.2f);
				_0024current = Timing.WaitForSeconds(0.2f);
				_0024PC = 3;
				break;
			case 3u:
				_003C_003Ef__this.Show = false;
				goto IL_0294;
			default:
				{
					return false;
				}
				IL_0294:
				_0024PC = -1;
				goto default;
				IL_01a8:
				if (_003Ci_003E__0 < _003C_003Ef__this.ShowWeaponList.Length)
				{
					if (_003C_003Ef__this.Show)
					{
						_003C_003Ef__this.FPWeapon.StopSprings();
						if (_003C_003Ef__this.TwoHandedWeapon)
						{
							_003C_003Ef__this.OneHandWeapon.StopSprings();
							_003C_003Ef__this.TwoHandWeapon.StopSprings();
						}
						_003C_003Ef__this.FPWeapon.AddSoftForce(_003C_003Ef__this.ShowWeaponList[_003Ci_003E__0].Position, _003C_003Ef__this.ShowWeaponList[_003Ci_003E__0].Rotation, (int)(_003C_003Ef__this.ShowWeaponList[_003Ci_003E__0].Duration * 60f));
						_0024current = Timing.WaitForSeconds(_003C_003Ef__this.ShowWeaponList[_003Ci_003E__0].Duration);
						_0024PC = 1;
						break;
					}
					goto case 1u;
				}
				if (_003C_003Ef__this.ShowHandLeft != null)
				{
					_0024current = Timing.WaitForSeconds(_003C_003Ef__this.ShowWeaponList[_003C_003Ef__this.ShowWeaponList.Length - 1].Duration * 0.15f + 0.2f);
					_0024PC = 2;
					break;
				}
				_003C_003Ef__this.Show = false;
				goto IL_0294;
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

		internal void _003C_003Em__126()
		{
			if (_003C_003Ef__this.Show)
			{
				_003C_003Ef__this.ShowHandLeft.gameObject.SetActive(false);
			}
		}
	}

	[Header("Motion")]
	public Vector3 MotionPositionRecoil = new Vector3(0f, 0f, -0.035f);

	public Vector3 MotionRotationRecoil = new Vector3(-10f, 0f, 0f);

	[Header("Fire Reload Settings")]
	public bool FireReload;

	public float FireReloadDelay = 0.2f;

	public float FireReloadDuration = 0.5f;

	public int FireReloadForce = 60;

	public Vector3 FireReloadPosition;

	public Vector3 FireReloadRotation;

	[Header("Knife Settings")]
	public bool KnifeWeapon;

	public float KnifeDelay = 0.1f;

	public int KnifeDelayForce = 50;

	public Vector3 KnifeDelayForcePosition;

	public Vector3 KnifeDelayForceRotation;

	public float KnifeAttackTime = 0.2f;

	public int KnifeAttackForce = 50;

	public Vector3 KnifeAttackForcePosition;

	public Vector3 KnifeAttackForceRotation;

	[Header("Two Handed Weapon")]
	public bool TwoHandedWeapon;

	public FPWeaponTwoHanded OneHandWeapon;

	public FPWeaponTwoHanded TwoHandWeapon;

	public Transform TwoMuzzle;

	private bool isTwoHandWeapon;

	[Header("Show Settings")]
	public int ShowForce = 50;

	public Vector3 ShowPosition;

	public Vector3 ShowRotation;

	public float ShowDuration = 0.5f;

	public Vector3 ShowPosition2;

	public Vector3 ShowRotation2;

	public Transform ShowHandLeft;

	private float ShowHadLeftDefault;

	private Tweener ShowHandLeftTweener;

	public ShowWeaponSettings[] ShowWeaponList;

	[Disabled]
	public bool Show;

	[Header("FireStat")]
	public GameObject FireStatModel;

	public MeshAtlas[] FireStatCounters;

	[Disabled]
	public bool FireStat;

	[Header("Others")]
	public vp_FPWeapon FPWeapon;

	public Transform Muzzle;

	public MeshAtlas[] WeaponAtlas;

	public MeshAtlas[] HandsAtlas;

	private bool isDryFire;

	[ContextMenu("Save")]
	public void Save()
	{
		if (Application.isPlaying && ShowWeaponList.Length != 0)
		{
			JsonArray jsonArray = new JsonArray();
			for (int i = 0; i < ShowWeaponList.Length; i++)
			{
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("p", ShowWeaponList[i].Position);
				jsonObject.Add("r", ShowWeaponList[i].Rotation);
				jsonObject.Add("d", ShowWeaponList[i].Duration);
				jsonArray.Add(jsonObject);
			}
			PlayerPrefs.SetString("ShowWeapon" + base.name, jsonArray.ToString());
		}
	}

	[ContextMenu("Load")]
	public void Load()
	{
		JsonArray jsonArray = JsonArray.Parse(PlayerPrefs.GetString("ShowWeapon" + base.name));
		List<ShowWeaponSettings> list = new List<ShowWeaponSettings>();
		for (int i = 0; i < jsonArray.Length; i++)
		{
			ShowWeaponSettings showWeaponSettings = new ShowWeaponSettings();
			JsonObject jsonObject = jsonArray.Get<JsonObject>(i);
			showWeaponSettings.Position = jsonObject.Get<Vector3>("p");
			showWeaponSettings.Rotation = jsonObject.Get<Vector3>("r");
			showWeaponSettings.Duration = jsonObject.Get<float>("d");
			list.Add(showWeaponSettings);
		}
		ShowWeaponList = list.ToArray();
	}

	private void Start()
	{
		UpdateHandAtlas();
	}

	private void OnEnable()
	{
		InputManager.GetButtonUpEvent += GetButtonUp;
	}

	private void OnDisable()
	{
		InputManager.GetButtonUpEvent -= GetButtonUp;
		if (!Show)
		{
			return;
		}
		Show = false;
		Timing.KillCoroutines("ShowWeapon");
		if (ShowHandLeft != null)
		{
			ShowHandLeft.gameObject.SetActive(true);
			if (ShowHandLeftTweener.IsActive())
			{
				ShowHandLeftTweener.Kill();
			}
			ShowHandLeft.localPosition = new Vector3(ShowHandLeft.localPosition.x, ShowHadLeftDefault, ShowHandLeft.localPosition.z);
		}
	}

	private void GetButtonUp(string name)
	{
		if (name == "Fire")
		{
			isDryFire = false;
		}
	}

	public void Active()
	{
		FPWeapon.Activate();
		FPWeapon.Wield();
		if (TwoHandedWeapon)
		{
			OneHandWeapon.Wield();
			TwoHandWeapon.Wield();
		}
	}

	public void Deactive()
	{
		FPWeapon.Deactivate();
	}

	public void Reload(float duration)
	{
		if (!KnifeWeapon)
		{
			FPWeapon.SetState("Reload");
			vp_Timer.In(duration, () =>
			{
				FPWeapon.SetState("Reload", false);
			});
		}
	}

	public void ShowWeapon()
	{
		if (!Show && ShowWeaponList.Length != 0)
		{
			Show = true;
			Timing.RunCoroutine(ShowWeaponCoroutine(), "ShowWeapon");
		}
	}

	[DebuggerHidden]
	private IEnumerator<float> ShowWeaponCoroutine()
	{
		//yield-return decompiler failed: Could not find currentField
		_003CShowWeaponCoroutine_003Ec__Iterator21 obj = new _003CShowWeaponCoroutine_003Ec__Iterator21();
		obj._003C_003Ef__this = this;
		return obj;
	}

	public void Fire()
	{
		if (Show)
		{
			FPWeapon.StopSprings();
			if (TwoHandedWeapon)
			{
				OneHandWeapon.StopSprings();
				TwoHandWeapon.StopSprings();
			}
			Show = false;
			if (ShowHandLeft != null)
			{
				ShowHandLeft.gameObject.SetActive(true);
				if (ShowHandLeftTweener.IsActive())
				{
					ShowHandLeftTweener.Kill();
				}
				ShowHandLeft.localPosition = new Vector3(ShowHandLeft.localPosition.x, ShowHadLeftDefault, ShowHandLeft.localPosition.z);
			}
		}
		if (KnifeWeapon)
		{
			FireKnife();
		}
		else
		{
			FireWeapon();
		}
	}

	private void FireWeapon()
	{
		if (!KnifeWeapon && UnityEngine.Random.value > 0.2f)
		{
			if (TwoHandedWeapon && isTwoHandWeapon)
			{
				TwoMuzzle.localEulerAngles = new Vector3(TwoMuzzle.localEulerAngles.x, TwoMuzzle.localEulerAngles.y, UnityEngine.Random.value * 360f);
				TwoMuzzle.gameObject.SetActive(true);
				vp_Timer.In(0.02f, () =>
				{
					TwoMuzzle.gameObject.SetActive(false);
				});
			}
			else
			{
				Muzzle.localEulerAngles = new Vector3(Muzzle.localEulerAngles.x, Muzzle.localEulerAngles.y, UnityEngine.Random.value * 360f);
				Muzzle.gameObject.SetActive(true);
				vp_Timer.In(0.02f, () =>
				{
					Muzzle.gameObject.SetActive(false);
				});
			}
		}
		FPWeapon.ResetSprings(0.5f, 0.5f, 1f, 1f);
		if (MotionRotationRecoil.z == 0f)
		{
			if (TwoHandedWeapon)
			{
				if (isTwoHandWeapon)
				{
					TwoHandWeapon.AddForce2(MotionPositionRecoil, MotionRotationRecoil);
				}
				else
				{
					OneHandWeapon.AddForce2(MotionPositionRecoil, MotionRotationRecoil);
				}
				isTwoHandWeapon = !isTwoHandWeapon;
			}
			else
			{
				FPWeapon.AddForce2(MotionPositionRecoil, MotionRotationRecoil);
			}
		}
		else if (TwoHandedWeapon)
		{
			if (isTwoHandWeapon)
			{
				TwoHandWeapon.AddForce2(MotionPositionRecoil, Vector3.Scale(MotionRotationRecoil, Vector3.one + Vector3.back) + ((!(UnityEngine.Random.value < 0.5f)) ? Vector3.back : Vector3.forward) * UnityEngine.Random.Range(MotionRotationRecoil.z * 0.5f, MotionRotationRecoil.z));
			}
			else
			{
				OneHandWeapon.AddForce2(MotionPositionRecoil, Vector3.Scale(MotionRotationRecoil, Vector3.one + Vector3.back) + ((!(UnityEngine.Random.value < 0.5f)) ? Vector3.back : Vector3.forward) * UnityEngine.Random.Range(MotionRotationRecoil.z * 0.5f, MotionRotationRecoil.z));
			}
			isTwoHandWeapon = !isTwoHandWeapon;
		}
		else
		{
			FPWeapon.AddForce2(MotionPositionRecoil, Vector3.Scale(MotionRotationRecoil, Vector3.one + Vector3.back) + ((!(UnityEngine.Random.value < 0.5f)) ? Vector3.back : Vector3.forward) * UnityEngine.Random.Range(MotionRotationRecoil.z * 0.5f, MotionRotationRecoil.z));
		}
		if (!FireReload)
		{
			return;
		}
		vp_Timer.In(FireReloadDelay, () =>
		{
			FPWeapon.AddSoftForce(FireReloadPosition, FireReloadRotation, FireReloadForce);
			vp_Timer.In(FireReloadDuration, () =>
			{
				FPWeapon.StopSprings();
				if (TwoHandedWeapon)
				{
					OneHandWeapon.StopSprings();
					TwoHandWeapon.StopSprings();
				}
			});
		});
	}

	private void FireKnife()
	{
		if (KnifeDelay != 0f)
		{
			FPWeapon.AddSoftForce(KnifeDelayForcePosition, KnifeDelayForceRotation, KnifeDelayForce);
		}
		vp_Timer.In(KnifeDelay, () =>
		{
			FPWeapon.StopSprings();
			FPWeapon.AddSoftForce(KnifeAttackForcePosition, KnifeAttackForceRotation, KnifeAttackForce);
			vp_Timer.In(KnifeAttackTime, () =>
			{
				FPWeapon.StopSprings();
			});
		});
	}

	public void DryFire()
	{
		if (KnifeWeapon || isDryFire)
		{
			return;
		}
		isDryFire = true;
		if (TwoHandedWeapon)
		{
			if (isTwoHandWeapon)
			{
				TwoHandWeapon.AddForce2(MotionPositionRecoil * -0.1f, MotionRotationRecoil * -0.1f);
			}
			else
			{
				OneHandWeapon.AddForce2(MotionPositionRecoil * -0.1f, MotionRotationRecoil * -0.1f);
			}
			isTwoHandWeapon = !isTwoHandWeapon;
		}
		else
		{
			FPWeapon.AddForce2(MotionPositionRecoil * -0.1f, MotionRotationRecoil * -0.1f);
		}
	}

	public void ScopeRifle()
	{
		if (Show)
		{
			FPWeapon.StopSprings();
			Show = false;
		}
	}

	public void UpdateHandAtlas()
	{
		string playerSkin = Utils.GetPlayerSkin(PlayerInput.instance.PlayerTeam);
		for (int i = 0; i < HandsAtlas.Length; i++)
		{
			if (HandsAtlas[i].spriteName != playerSkin)
			{
				HandsAtlas[i].spriteName = playerSkin;
			}
		}
	}

	public void UpdateWeaponAtlas(int weaponID)
	{
		int weaponSkinSelected = AccountManager.GetWeaponSkinSelected(weaponID);
		for (int i = 0; i < WeaponAtlas.Length; i++)
		{
			if (WeaponAtlas[i].mSpriteName != weaponID + "-" + weaponSkinSelected)
			{
				WeaponAtlas[i].spriteName = weaponID + "-" + weaponSkinSelected;
			}
		}
		if (!KnifeWeapon && AccountManager.GetFireStat(weaponID, weaponSkinSelected))
		{
			FireStat = true;
			FireStatModel.SetActive(true);
			UpdateFireStat(AccountManager.GetFireStatCounter(weaponID, weaponSkinSelected));
		}
	}

	public void UpdateFireStat(int counter)
	{
		if (FireStat)
		{
			string text = counter.ToString("D6");
			for (int i = 0; i < text.Length; i++)
			{
				FireStatCounters[i].spriteName = "f" + text[i];
			}
		}
	}
}
