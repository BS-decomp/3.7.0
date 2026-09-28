using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

[Serializable]
public class WeaponShopData
{
	[Serializable]
	public class WeaponUpgrade
	{
		public MoneyType Money;

		public ObscuredInt UpgradePrice;

		public ObscuredInt FaceDamage = 50;

		public ObscuredInt BodyDamage = 20;

		public ObscuredInt HandDamage = 10;

		public ObscuredInt LegDamage = 5;

		public ObscuredFloat FireRate = 0.1f;

		public ObscuredFloat Accuracy = 10f;

		public ObscuredFloat FireAccuracy = 10f;

		public ObscuredInt Ammo = 20;

		public ObscuredInt MaxAmmo = 100;

		public ObscuredFloat Mass = 0.01f;
	}

	[Serializable]
	public class WeaponSkin
	{
		public ObscuredInt Rarity;

		public ObscuredInt SkinID;

		public ObscuredString SkinName;

		public ObscuredInt Price;

		public Texture2D Source;
	}

	public MoneyType Money;

	public ObscuredInt WeaponPrice;

	public List<WeaponUpgrade> Upgrades = new List<WeaponUpgrade>();

	public List<WeaponSkin> Skins = new List<WeaponSkin>();
}
