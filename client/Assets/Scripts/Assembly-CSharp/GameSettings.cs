using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSettings : ScriptableObject
{
	[Serializable]
	public class AmbientSettings
	{
		public string Map;

		public int Ambient;

		public float Volume;
	}

	public string PhotonID;

	public GameObject PlayerController;

	public GameObject PlayerSkin;

	public float PlayerDefaultMove = 0.18f;

	public List<WeaponType> Weapons = new List<WeaponType>();

	public List<WeaponShopData> WeaponsShop = new List<WeaponShopData>();

	public List<Rect> CaseUVRect = new List<Rect>();

	public List<Vector2> CaseSize = new List<Vector2>();

	public List<StorePlayerSkinData> PlayerSkinShop = new List<StorePlayerSkinData>();

	public UIAtlas WeaponAtlas;

	public AudioClip ConnectDeveloperAudio;

	public List<AmbientSettings> Ambients = new List<AmbientSettings>();

	private static GameSettings Instance;

	public static GameSettings instance
	{
		get
		{
			if (Instance == null)
			{
				Instance = Resources.Load("Others/GameSettings") as GameSettings;
			}
			return Instance;
		}
	}
}
