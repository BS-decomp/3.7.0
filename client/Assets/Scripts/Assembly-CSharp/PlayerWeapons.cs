using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
	[Serializable]
	public class WeaponData
	{
		public bool Enabled;

		public ObscuredInt WeaponID;

		public ObscuredBool WeaponFire;

		public ObscuredString WeaponName;

		public FPWeaponShooter WeaponScript;

		public ObscuredInt Damage;

		public ObscuredFloat FireRate;

		public ObscuredFloat Accuracy;

		public ObscuredFloat FireAccuracy;

		public ObscuredInt FireBullets;

		public ObscuredFloat ReloadTime;

		public ObscuredFloat LastFireRate;

		public ObscuredInt Ammo;

		public ObscuredInt AmmoFirst;

		public ObscuredInt AmmoMax;

		public ObscuredFloat Distance;

		public ObscuredFloat Mass;

		public ObscuredBool RifleScope;

		public ObscuredInt RifleScopeSize;

		public ObscuredFloat RifleScopeSensitivity;

		public ObscuredFloat RifleScopeRecoil;

		public ObscuredFloat RifleScopeAccuracy;

		public WeaponSound FireSound;

		public WeaponSound ReloadSound;
	}

	public WeaponTypeList SelectedWeapon;

	public bool CanFire = true;

	public bool isDebug;

	[Disabled]
	public bool isFire;

	[Disabled]
	public bool isScope;

	[Disabled]
	public bool isReload;

	private vp_Timer.Handle ReloadTime = new vp_Timer.Handle();

	[Disabled]
	public bool Wielded;

	private vp_Timer.Handle WieldedTime = new vp_Timer.Handle();

	[Disabled]
	public bool InfiniteAmmo;

	[Header("Weapons Data")]
	public WeaponData KnifeData = new WeaponData();

	public WeaponData PistolData = new WeaponData();

	public WeaponData RifleData = new WeaponData();

	private bool isUpdateWeaponData;

	[Header("RigidBody")]
	public bool PushRigidbody;

	public float PushRigidbodyForce = 1000f;

	[Header("Sounds")]
	public PlayerSounds Sounds;

	[Header("Others")]
	public LayerMask FireLayers;

	public PlayerInput m_PlayerInput;

	public Camera PlayerCamera;

	public Camera WeaponCamera;

	private Dictionary<string, GameObject> Weapons = new Dictionary<string, GameObject>();

	private void Start()
	{
		EventManager.AddListener<DamageInfo>("KillPlayer", KillPlayer);
	}

	private void OnEnable()
	{
		UICrosshair.SetActiveCrosshair(true);
		InputManager.GetButtonDownEvent += GetButtonDown;
		InputManager.GetButtonUpEvent += GetButtonUp;
	}

	private void OnDisable()
	{
		UICrosshair.SetActiveCrosshair(false);
		InputManager.GetButtonDownEvent -= GetButtonDown;
		InputManager.GetButtonUpEvent -= GetButtonUp;
		isReload = false;
		isFire = false;
		if (ReloadTime.Active)
		{
			ReloadTime.Cancel();
		}
		if (WieldedTime.Active)
		{
			WieldedTime.Cancel();
		}
	}

	private void GetButtonDown(string name)
	{
		switch (name)
		{
		case "Fire":
			isFire = true;
			break;
		case "Aim":
			ScopeWeapon();
			break;
		case "Reload":
			ReloadWeapon();
			break;
		case "Pause":
			DeactiveScope();
			break;
		case "SelectWeapon":
			UpdateSelectWeapon();
			break;
		}
	}

	private void GetButtonUp(string name)
	{
		if (name == "Fire")
		{
			isFire = false;
		}
	}

	private void Update()
	{
		if (isFire)
		{
			FireWeapon();
		}
	}

	private void UpdateSelectWeapon()
	{
		if (SelectedWeapon == WeaponTypeList.Knife)
		{
			if (RifleData.Enabled)
			{
				SetWeapon(WeaponTypeList.Rifle);
			}
			else if (PistolData.Enabled)
			{
				SetWeapon(WeaponTypeList.Pistol);
			}
		}
		else if (SelectedWeapon == WeaponTypeList.Pistol)
		{
			if (KnifeData.Enabled)
			{
				SetWeapon(WeaponTypeList.Knife);
			}
			else if (RifleData.Enabled)
			{
				SetWeapon(WeaponTypeList.Rifle);
			}
		}
		else if (SelectedWeapon == WeaponTypeList.Rifle)
		{
			if (PistolData.Enabled)
			{
				SetWeapon(WeaponTypeList.Pistol);
			}
			else if (KnifeData.Enabled)
			{
				SetWeapon(WeaponTypeList.Knife);
			}
		}
	}

	public WeaponData GetSelectedWeaponData()
	{
		return GetWeaponData(SelectedWeapon);
	}

	public WeaponData GetWeaponData(WeaponTypeList weapon)
	{
		switch (weapon)
		{
		case WeaponTypeList.Knife:
			return KnifeData;
		case WeaponTypeList.Pistol:
			return PistolData;
		case WeaponTypeList.Rifle:
			return RifleData;
		default:
			return null;
		}
	}

	public void UpdateWeaponAll()
	{
		UpdateWeaponAll(WeaponManager.DefaultWeaponType);
	}

	public void UpdateWeaponAll(WeaponTypeList defaultWeapon)
	{
		int knifeID = WeaponManager.GetKnifeID();
		int pistolID = WeaponManager.GetPistolID();
		int rifleID = WeaponManager.GetRifleID();
		vp_Timer.In(0.03f, () =>
		{
			isUpdateWeaponData = true;
			DeactiveAll();
			WeaponManager.SetKnifeType(knifeID);
			WeaponManager.SetPistolType(pistolID);
			WeaponManager.SetRifleType(rifleID);
			UpdateWeaponData(WeaponTypeList.Knife);
			UpdateWeaponData(WeaponTypeList.Pistol);
			UpdateWeaponData(WeaponTypeList.Rifle);
			vp_Timer.In(0.05f, () =>
			{
				SetWeapon(defaultWeapon, false);
				isUpdateWeaponData = false;
			});
			int num = 0;
			if (KnifeData.Enabled)
			{
				num++;
			}
			if (PistolData.Enabled)
			{
				num++;
			}
			if (RifleData.Enabled)
			{
				num++;
			}
			if (num >= 2)
			{
				UIGameManager.SetActiveSelectWeapon(true);
			}
			else
			{
				UIGameManager.SetActiveSelectWeapon(false);
			}
		});
	}

	public void UpdateWeaponData(WeaponTypeList weaponType)
	{
		WeaponType weaponType2 = null;
		WeaponData weaponData = null;
		switch (weaponType)
		{
		case WeaponTypeList.Knife:
			if (WeaponManager.HasKnifeType())
			{
				weaponType2 = WeaponManager.GetKnifeType();
				weaponData = KnifeData;
			}
			break;
		case WeaponTypeList.Pistol:
			if (WeaponManager.HasPistolType())
			{
				weaponType2 = WeaponManager.GetPistolType();
				weaponData = PistolData;
			}
			break;
		case WeaponTypeList.Rifle:
			if (WeaponManager.HasRifleType())
			{
				weaponType2 = WeaponManager.GetRifleType();
				weaponData = RifleData;
			}
			break;
		}
		if (weaponType2 != null && (weaponData.WeaponScript == null || weaponData.WeaponName != weaponType2.WeaponName) && !Weapons.ContainsKey(weaponType2.WeaponName))
		{
			GameObject fpsPrefab = weaponType2.FpsPrefab;
			fpsPrefab = Utils.AddChild(fpsPrefab, m_PlayerInput.FPCamera.transform);
			Weapons.Add(weaponType2.WeaponName, fpsPrefab);
			fpsPrefab.SetActive(true);
		}
		WeaponData weaponData2 = new WeaponData();
		if (weaponType2 != null)
		{
			weaponData2.Enabled = true;
			weaponData2.WeaponID = weaponType2.WeaponID;
			weaponData2.WeaponName = weaponType2.WeaponName;
			weaponData2.WeaponFire = weaponType2.WeaponFire;
			weaponData2.WeaponScript = Weapons[weaponType2.WeaponName].GetComponent<FPWeaponShooter>();
			weaponData2.Damage = weaponType2.BodyDamage;
			weaponData2.FireRate = weaponType2.FireRate;
			weaponData2.Accuracy = weaponType2.Accuracy;
			weaponData2.FireAccuracy = weaponType2.FireAccuracy;
			weaponData2.FireBullets = weaponType2.FireBullets;
			weaponData2.ReloadTime = weaponType2.ReloadTime;
			weaponData2.Ammo = weaponType2.Ammo;
			weaponData2.AmmoFirst = weaponType2.Ammo;
			weaponData2.AmmoMax = weaponType2.MaxAmmo;
			weaponData2.Distance = weaponType2.Distance;
			weaponData2.Mass = weaponType2.Mass;
			weaponData2.RifleScope = weaponType2.RifleScope;
			weaponData2.RifleScopeSize = weaponType2.RifleScopeSize;
			weaponData2.RifleScopeSensitivity = weaponType2.RifleScopeSensitivity;
			weaponData2.RifleScopeRecoil = weaponType2.RifleScopeRecoil;
			weaponData2.RifleScopeAccuracy = weaponType2.RifleScopeAccuracy;
			weaponData2.FireSound = weaponType2.FireSound;
			weaponData2.ReloadSound = weaponType2.ReloadSound;
			weaponData2.WeaponScript.UpdateHandAtlas();
			weaponData2.WeaponScript.UpdateWeaponAtlas(weaponData2.WeaponID);
		}
		else
		{
			weaponData2.Enabled = false;
		}
		weaponData = weaponData2;
		switch (weaponType)
		{
		case WeaponTypeList.Knife:
			KnifeData = weaponData;
			break;
		case WeaponTypeList.Pistol:
			PistolData = weaponData;
			break;
		case WeaponTypeList.Rifle:
			RifleData = weaponData;
			break;
		}
	}

	public void SetWeapon(WeaponTypeList weapon, bool checkSelectedWeapon = true)
	{
		if ((!checkSelectedWeapon || SelectedWeapon != weapon) && GetWeaponData(weapon).Enabled)
		{
			SelectedWeapon = weapon;
			DeactiveScope();
			DeactiveAll();
			WeaponData selectedWeaponData = GetSelectedWeaponData();
			UICrosshair.SetAccuracy(selectedWeaponData.Accuracy);
			UIGameManager.SetAmmoLabel(selectedWeaponData.Ammo, selectedWeaponData.AmmoMax, InfiniteAmmo);
			UIGameManager.SetActiveRifleScope(selectedWeaponData.RifleScope);
			m_PlayerInput.SetPlayerSpeed(selectedWeaponData.Mass);
			if (ReloadTime.Active)
			{
				ReloadTime.Cancel();
				isReload = false;
			}
			Sounds.Stop();
			if (WieldedTime.Active)
			{
				WieldedTime.Cancel();
			}
			Wielded = true;
			vp_Timer.In(0.5f, () =>
			{
				Wielded = false;
			}, WieldedTime);
			switch (weapon)
			{
			case WeaponTypeList.Knife:
				KnifeData.WeaponScript.Active();
				break;
			case WeaponTypeList.Pistol:
				PistolData.WeaponScript.Active();
				break;
			case WeaponTypeList.Rifle:
				RifleData.WeaponScript.Active();
				break;
			}
			m_PlayerInput.Controller.SetWeapon(selectedWeaponData.WeaponID);
		}
	}

	private void DeactiveAll()
	{
		if (KnifeData.WeaponScript != null)
		{
			KnifeData.WeaponScript.Deactive();
		}
		if (PistolData.WeaponScript != null)
		{
			PistolData.WeaponScript.Deactive();
		}
		if (RifleData.WeaponScript != null)
		{
			RifleData.WeaponScript.Deactive();
		}
	}

	public void FireWeapon()
	{
		if (CanFire && !isReload && GameManager.GetRoundState() != RoundState.EndRound)
		{
			switch (SelectedWeapon)
			{
			case WeaponTypeList.Knife:
				Fire(KnifeData);
				break;
			case WeaponTypeList.Pistol:
				Fire(PistolData);
				break;
			case WeaponTypeList.Rifle:
				Fire(RifleData);
				break;
			}
		}
	}

	private void Fire(WeaponData weapon)
	{
		if (Wielded || (float)weapon.LastFireRate > Time.time || isUpdateWeaponData || !weapon.WeaponFire)
		{
			return;
		}
		if (weapon.WeaponScript.KnifeWeapon)
		{
			weapon.LastFireRate = Time.time + (float)weapon.FireRate;
			Sounds.Play(weapon.FireSound);
			weapon.WeaponScript.Fire();
			vp_Timer.In(weapon.WeaponScript.KnifeDelay, () =>
			{
				FireData(weapon);
			});
		}
		else if ((int)weapon.Ammo > 0)
		{
			--weapon.Ammo;
			weapon.LastFireRate = Time.time + (float)weapon.FireRate;
			Sounds.Play(weapon.FireSound);
			weapon.WeaponScript.Fire();
			FireData(weapon);
			if (isScope && (float)weapon.RifleScopeRecoil != 0f)
			{
				m_PlayerInput.FPCamera.Pitch -= weapon.RifleScopeRecoil;
				float num = (float)weapon.RifleScopeRecoil / 2f;
				m_PlayerInput.FPCamera.Yaw += UnityEngine.Random.Range(0f - num, num);
			}
		}
		else if ((int)weapon.AmmoMax == 0)
		{
			DryFire(weapon);
		}
		else
		{
			Reload(weapon);
		}
	}

	private void FireData(WeaponData weapon)
	{
		UIGameManager.SetAmmoLabel(weapon.Ammo, weapon.AmmoMax, InfiniteAmmo);
		Vector2 vector = Vector3.zero;
		DecalInfo decalInfo = new DecalInfo();
		if (SelectedWeapon == WeaponTypeList.Knife)
		{
			decalInfo.isKnife = true;
		}
		for (int i = 0; i < (int)weapon.FireBullets; i++)
		{
			vector = ((!isScope) ? UICrosshair.Fire(weapon.FireAccuracy) : UICrosshair.Fire(weapon.RifleScopeAccuracy));
			vector = Utils.RandomAccuracy(vector);
			Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f + vector.x, 0.5f + vector.y, 0f));
			RaycastHit hitInfo;
			if (!Physics.Raycast(ray, out hitInfo, weapon.Distance, FireLayers))
			{
				continue;
			}
			if (hitInfo.collider.CompareTag("PlayerSkin"))
			{
				if (decalInfo.BloodDecal == -1)
				{
					decalInfo.BloodDecal = decalInfo.Points.Count;
					decalInfo.Points.Add(hitInfo.point);
					decalInfo.Normals.Add(hitInfo.normal);
				}
				DamageInfo value = DamageInfo.Create(weapon.Damage, m_PlayerInput.PlayerTransform.position, m_PlayerInput.PlayerTeam, weapon.WeaponID, PhotonNetwork.player.ID);
				hitInfo.transform.SendMessage("Damage", value, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				if (hitInfo.collider.CompareTag("IgnoreDecal"))
				{
					continue;
				}
				if (hitInfo.collider.CompareTag("RigidbodyObject"))
				{
					if (PushRigidbody)
					{
						hitInfo.transform.GetComponent<RigidbodyObject>().Force(PlayerCamera.transform.forward * PushRigidbodyForce);
					}
				}
				else if (hitInfo.collider.CompareTag("DamageObject"))
				{
					DamageInfo value2 = DamageInfo.Create(weapon.Damage, m_PlayerInput.PlayerTransform.position, m_PlayerInput.PlayerTeam, weapon.WeaponID, PhotonNetwork.player.ID);
					hitInfo.transform.SendMessage("Damage", value2, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					decalInfo.Points.Add(hitInfo.point);
					decalInfo.Normals.Add(hitInfo.normal);
				}
			}
		}
		EventManager.Dispatch("Fire", decalInfo);
	}

	private void DryFire(WeaponData weapon)
	{
		weapon.LastFireRate = Time.time + (float)weapon.FireRate * 2f;
		weapon.WeaponScript.DryFire();
		Sounds.Play(WeaponSound.AmmoEmpty);
	}

	private void ReloadWeapon()
	{
		if (!isReload)
		{
			switch (SelectedWeapon)
			{
			case WeaponTypeList.Knife:
				KnifeData.WeaponScript.ShowWeapon();
				break;
			case WeaponTypeList.Pistol:
				Reload(PistolData);
				break;
			case WeaponTypeList.Rifle:
				Reload(RifleData);
				break;
			}
		}
	}

	private void Reload(WeaponData weapon)
	{
		if (isScope)
		{
			DeactiveScope();
		}
		if ((int)weapon.Ammo == (int)weapon.AmmoFirst || (int)weapon.AmmoMax == 0)
		{
			weapon.WeaponScript.ShowWeapon();
			return;
		}
		isReload = true;
		Sounds.Play(weapon.ReloadSound);
		weapon.WeaponScript.Reload(weapon.ReloadTime);
		vp_Timer.In((float)weapon.ReloadTime + 0.5f, () =>
		{
			isReload = false;
			if (InfiniteAmmo)
			{
				weapon.Ammo = weapon.AmmoFirst;
			}
			else if ((int)weapon.AmmoMax > (int)weapon.AmmoFirst)
			{
				WeaponData weaponData = weapon;
				weaponData.AmmoMax = (int)weaponData.AmmoMax - ((int)weapon.AmmoFirst - (int)weapon.Ammo);
				weapon.Ammo = weapon.AmmoFirst;
			}
			else
			{
				int num = weapon.Ammo;
				WeaponData weaponData2 = weapon;
				weaponData2.Ammo = (int)weaponData2.Ammo + (int)weapon.AmmoMax;
				weapon.Ammo = Mathf.Min(weapon.AmmoFirst, weapon.Ammo);
				WeaponData weaponData3 = weapon;
				weaponData3.AmmoMax = (int)weaponData3.AmmoMax - ((int)weapon.Ammo - num);
				weapon.AmmoMax = Mathf.Max(0, weapon.AmmoMax);
			}
			UIGameManager.SetAmmoLabel(weapon.Ammo, weapon.AmmoMax, InfiniteAmmo);
		}, ReloadTime);
	}

	private void ScopeWeapon(bool check = true)
	{
		if ((!check || !isReload) && ((bool)GetSelectedWeaponData().RifleScope || !check))
		{
			isScope = !isScope;
			PlayerCamera.fieldOfView = ((!isScope) ? 60 : ((int)GetSelectedWeaponData().RifleScopeSize));
			WeaponCamera.fieldOfView = (isScope ? 1 : 60);
			UICrosshair.SetActiveRifleScope(isScope);
			GetSelectedWeaponData().WeaponScript.ScopeRifle();
		}
	}

	public void DeactiveScope()
	{
		if (isScope)
		{
			ScopeWeapon(false);
		}
		if ((bool)m_PlayerInput.Dead)
		{
			UICrosshair.SetActiveCrosshair(false);
		}
	}

	private void KillPlayer(DamageInfo damageInfo)
	{
		WeaponData weaponData = null;
		if (PistolData.Enabled && (int)PistolData.WeaponID == damageInfo.WeaponID && PistolData.WeaponScript.FireStat)
		{
			weaponData = PistolData;
		}
		else if (RifleData.Enabled && (int)RifleData.WeaponID == damageInfo.WeaponID && RifleData.WeaponScript.FireStat)
		{
			weaponData = RifleData;
		}
		if (weaponData != null)
		{
			int num = AccountManager.GetFireStatCounter(damageInfo.WeaponID, damageInfo.WeaponSkinID) + 1;
			weaponData.WeaponScript.UpdateFireStat(num);
			AccountManager.SetFireStatCounter(damageInfo.WeaponID, damageInfo.WeaponSkinID, num);
			m_PlayerInput.Controller.SetFireStatCounter(weaponData.WeaponID, num);
		}
	}
}
