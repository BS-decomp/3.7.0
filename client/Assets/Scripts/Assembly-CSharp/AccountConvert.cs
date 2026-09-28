using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FreeJSON;

public class AccountConvert
{
	private static List<string> a = new List<string>
	{
		"a", "e", "i", "o", "u", "y", "b", "c", "d", "f",
		"g", "h", "j", "k", "l", "m", "n", "p", "q", "r",
		"s", "t", "v", "w", "x", "z", "0", "1", "2", "3",
		"4", "5", "6", "7", "8", "9", "*"
	};

	private static List<string> b = new List<string>
	{
		"p", "h", "z", "r", "i", "q", "y", "b", "m", "s",
		"5", "6", "w", "k", "u", "9", "g", "j", "o", "d",
		"c", "v", "l", "e", "2", "7", "8", "a", "0", "1",
		"t", "n", "*", "4", "3", "f", "x"
	};

	public static AccountData Copy(AccountData data)
	{
		IFormatter formatter = new BinaryFormatter();
		Stream stream = new MemoryStream();
		using (stream)
		{
			formatter.Serialize(stream, data);
			stream.Seek(0L, SeekOrigin.Begin);
			return (AccountData)formatter.Deserialize(stream);
		}
	}

	public static void CopyDefaultValue(AccountData from, AccountData to)
	{
		to.GameVersion = from.GameVersion;
		to.AccountName = from.AccountName;
		to.Money = from.Money;
		to.Gold = from.Gold;
		to.XP = from.XP;
		to.Level = from.Level;
		to.OpenCase = from.OpenCase;
		to.Deaths = from.Deaths;
		to.Kills = from.Kills;
		to.Headshot = from.Headshot;
		to.SelectedRifle = from.SelectedRifle;
		to.SelectedPistol = from.SelectedPistol;
		to.SelectedKnife = from.SelectedKnife;
		to.SelectedPlayerSkin = from.SelectedPlayerSkin;
		to.PlayerSkins = from.PlayerSkins;
		to.InAppPurchase = from.InAppPurchase;
		to.AndroidID = from.AndroidID;
	}

	public static void CopyWeaponsValue(AccountData from, AccountData to)
	{
		to.Weapons = from.Weapons;
	}

	public static string Serialize(AccountData data, bool registerTime)
	{
		JsonObject jsonObject = new JsonObject();
		if (data.GameVersion == string.Empty)
		{
			jsonObject.Add("GameVersion", VersionManager.bundleVersion);
		}
		else
		{
			jsonObject.Add("GameVersion", (string)data.GameVersion);
		}
		jsonObject.Add("AccountName", (string)data.AccountName);
		jsonObject.Add("Money", (int)data.Money);
		jsonObject.Add("Gold", (int)data.Gold);
		jsonObject.Add("XP", (int)data.XP);
		jsonObject.Add("Level", (int)data.Level);
		jsonObject.Add("OpenCase", (int)data.OpenCase);
		jsonObject.Add("Deaths", (int)data.Deaths);
		jsonObject.Add("Kills", (int)data.Kills);
		jsonObject.Add("Headshot", (int)data.Headshot);
		jsonObject.Add("SelectedRifle", (int)data.SelectedRifle);
		jsonObject.Add("SelectedPistol", (int)data.SelectedPistol);
		jsonObject.Add("SelectedKnife", (int)data.SelectedKnife);
		jsonObject.Add("SelectedPlayerSkin", (int)data.SelectedPlayerSkin);
		jsonObject.Add("Clan", (string)data.Clan);
		JsonObject jsonObject2 = new JsonObject();
		for (int i = 0; i < data.PlayerSkins.Count; i++)
		{
			jsonObject2.Add(i.ToString(), (int)data.PlayerSkins[i]);
		}
		jsonObject.Add("PlayerSkins", jsonObject2);
		JsonArray jsonArray = new JsonArray();
		for (int j = 0; j < GameSettings.instance.Weapons.Count; j++)
		{
			AccountWeapon weaponData = GetWeaponData(GameSettings.instance.Weapons[j].WeaponID, data);
			JsonObject jsonObject3 = new JsonObject();
			jsonObject3.Add("ID", (int)weaponData.ID);
			jsonObject3.Add("Buy", (bool)weaponData.Buy);
			jsonObject3.Add("Upgrade", (int)weaponData.Upgrade);
			JsonObject jsonObject4 = new JsonObject();
			for (int k = 0; k < weaponData.Skins.Count; k++)
			{
				if ((int)weaponData.Skins[k] != 0)
				{
					jsonObject4.Add(k.ToString(), (int)weaponData.Skins[k]);
				}
			}
			jsonObject3.Add("Skins", jsonObject4);
			JsonObject jsonObject5 = new JsonObject();
			for (int l = 0; l < GameSettings.instance.WeaponsShop[j].Skins.Count; l++)
			{
				if (weaponData.FireStats.Count > l && (int)weaponData.FireStats[l] != -1)
				{
					jsonObject5.Add(l.ToString("D2"), (int)weaponData.FireStats[l]);
				}
			}
			jsonObject3.Add("FireStats", jsonObject5);
			jsonObject3.Add("SelectedSkin", (int)weaponData.SelectedSkin);
			jsonArray.Add(jsonObject3);
		}
		jsonObject.Add("Weapons", jsonArray);
		JsonObject jsonObject6 = new JsonObject();
		for (int m = 0; m < data.InAppPurchase.Count; m++)
		{
			jsonObject6.Add(m.ToString(), (string)data.InAppPurchase[m]);
		}
		jsonObject.Add("InAppPurchase", jsonObject6);
		if (data.AndroidID == string.Empty)
		{
			jsonObject.Add("AndroidID", AndroidNativeFunctions.GetAndroidID());
		}
		else
		{
			jsonObject.Add("AndroidID", (string)data.AndroidID);
		}
		if (registerTime)
		{
			jsonObject.Add("RegisterTime", JsonObject.Parse(Firebase.GetTimeStamp()));
		}
		return jsonObject.ToString();
	}

	private static AccountWeapon GetWeaponData(int id, AccountData data)
	{
		for (int i = 0; i < data.Weapons.Count; i++)
		{
			if (id == (int)data.Weapons[i].ID)
			{
				return data.Weapons[i];
			}
		}
		AccountWeapon accountWeapon = new AccountWeapon();
		accountWeapon.ID = id;
		return accountWeapon;
	}

	public static AccountData Deserialize(string text)
	{
		JsonObject jsonObject = JsonObject.Parse(text);
		AccountData accountData = new AccountData();
		accountData.GameVersion = jsonObject.Get<string>("GameVersion");
		accountData.AccountName = jsonObject.Get<string>("AccountName");
		accountData.Money = jsonObject.Get<int>("Money");
		accountData.Gold = jsonObject.Get<int>("Gold");
		accountData.XP = jsonObject.Get<int>("XP");
		accountData.Level = jsonObject.Get<int>("Level");
		accountData.OpenCase = jsonObject.Get<int>("OpenCase");
		accountData.Deaths = jsonObject.Get<int>("Deaths");
		accountData.Kills = jsonObject.Get<int>("Kills");
		accountData.Headshot = jsonObject.Get<int>("Headshot");
		accountData.SelectedRifle = jsonObject.Get<int>("SelectedRifle");
		accountData.SelectedPistol = jsonObject.Get<int>("SelectedPistol");
		accountData.SelectedKnife = jsonObject.Get<int>("SelectedKnife");
		accountData.SelectedPlayerSkin = jsonObject.Get<int>("SelectedPlayerSkin");
		accountData.Clan = jsonObject.Get<string>("Clan");
		List<int> list = jsonObject.Get<List<int>>("PlayerSkins");
		for (int i = 0; i < list.Count; i++)
		{
			accountData.PlayerSkins.Add(list[i]);
		}
		JsonArray jsonArray = jsonObject.Get<JsonArray>("Weapons");
		for (int j = 0; j < GameSettings.instance.Weapons.Count; j++)
		{
			AccountWeapon accountWeapon = new AccountWeapon();
			if (jsonArray.Length > j)
			{
				JsonObject jsonObject2 = jsonArray.Get<JsonObject>(j);
				accountWeapon.ID = jsonObject2.Get<int>("ID");
				accountWeapon.Buy = jsonObject2.Get<bool>("Buy");
				accountWeapon.Upgrade = jsonObject2.Get<int>("Upgrade");
				List<int> list2 = jsonObject2.Get<List<int>>("Skins");
				for (int k = 0; k < list2.Count; k++)
				{
					accountWeapon.Skins.Add(list2[k]);
				}
				for (int l = 0; l < GameSettings.instance.WeaponsShop[(int)accountWeapon.ID - 1].Skins.Count; l++)
				{
					accountWeapon.FireStats.Add(-1);
				}
				JsonObject jsonObject3 = jsonObject2.Get<JsonObject>("FireStats");
				for (int m = 0; m < jsonObject3.Length; m++)
				{
					int result = 0;
					int num = jsonObject3.Get<int>(jsonObject3.GetKey(m));
					int.TryParse(jsonObject3.GetKey(m), out result);
					if (result == 0)
					{
						continue;
					}
					if (accountWeapon.FireStats.Count > result)
					{
						accountWeapon.FireStats[result] = num;
						continue;
					}
					for (int n = accountWeapon.FireStats.Count - 1; n < result; n++)
					{
						accountWeapon.FireStats.Add(-1);
					}
					accountWeapon.FireStats[accountWeapon.FireStats.Count - 1] = num;
				}
				accountWeapon.SelectedSkin = jsonObject2.Get<int>("SelectedSkin");
			}
			else
			{
				AccountWeapon accountWeapon2 = new AccountWeapon();
				accountWeapon2.ID = j + 1;
				accountWeapon = accountWeapon2;
			}
			accountData.Weapons.Add(accountWeapon);
		}
		List<string> list3 = jsonObject.Get<List<string>>("InAppPurchase");
		for (int num2 = 0; num2 < list3.Count; num2++)
		{
			accountData.InAppPurchase.Add(list3[num2]);
		}
		accountData.AndroidID = jsonObject.Get<string>("AndroidID");
		return accountData;
	}

	public static JsonObject CompareDefaultValue(AccountData defaultData, AccountData data)
	{
		JsonObject jsonObject = new JsonObject();
		if (defaultData.GameVersion != VersionManager.bundleVersion)
		{
			data.GameVersion = VersionManager.bundleVersion;
			jsonObject.Add("GameVersion", (string)data.GameVersion);
		}
		if ((int)defaultData.Money != (int)data.Money)
		{
			jsonObject.Add("Money", (int)data.Money);
		}
		if ((int)defaultData.Gold != (int)data.Gold)
		{
			jsonObject.Add("Gold", (int)data.Gold);
		}
		if ((int)defaultData.XP != (int)data.XP)
		{
			jsonObject.Add("XP", (int)data.XP);
		}
		if ((int)defaultData.Level != (int)data.Level)
		{
			jsonObject.Add("Level", (int)data.Level);
		}
		if ((int)defaultData.OpenCase != (int)data.OpenCase)
		{
			jsonObject.Add("OpenCase", (int)data.OpenCase);
		}
		if ((int)defaultData.Deaths != (int)data.Deaths)
		{
			jsonObject.Add("Deaths", (int)data.Deaths);
		}
		if ((int)defaultData.Kills != (int)data.Kills)
		{
			jsonObject.Add("Kills", (int)data.Kills);
		}
		if ((int)defaultData.Headshot != (int)data.Headshot)
		{
			jsonObject.Add("Headshot", (int)data.Headshot);
		}
		if ((int)defaultData.SelectedRifle != (int)data.SelectedRifle)
		{
			jsonObject.Add("SelectedRifle", (int)data.SelectedRifle);
		}
		if ((int)defaultData.SelectedPistol != (int)data.SelectedPistol)
		{
			jsonObject.Add("SelectedPistol", (int)data.SelectedPistol);
		}
		if ((int)defaultData.SelectedKnife != (int)data.SelectedKnife)
		{
			jsonObject.Add("SelectedKnife", (int)data.SelectedKnife);
		}
		if ((int)defaultData.SelectedPlayerSkin != (int)data.SelectedPlayerSkin)
		{
			jsonObject.Add("SelectedPlayerSkin", (int)data.SelectedPlayerSkin);
		}
		if ((string)defaultData.Clan != (string)data.Clan)
		{
			jsonObject.Add("Clan", (string)data.Clan);
		}
		if (defaultData.PlayerSkins.Count != data.PlayerSkins.Count)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < data.PlayerSkins.Count; i++)
			{
				list.Add(data.PlayerSkins[i]);
			}
			jsonObject.Add("PlayerSkins", list);
		}
		if (defaultData.InAppPurchase.Count != data.InAppPurchase.Count)
		{
			List<string> list2 = new List<string>();
			for (int j = 0; j < data.InAppPurchase.Count; j++)
			{
				list2.Add(data.InAppPurchase[j]);
			}
			jsonObject.Add("InAppPurchase", list2);
		}
		if (defaultData.AndroidID != AndroidNativeFunctions.GetAndroidID())
		{
			data.AndroidID = AndroidNativeFunctions.GetAndroidID();
			jsonObject.Add("AndroidID", (string)data.AndroidID);
		}
		return jsonObject;
	}

	public static JsonObject CompareWeaponValue(AccountData defaultData, AccountData data)
	{
		JsonObject jsonObject = new JsonObject();
		Dictionary<string, JsonObject> dictionary = new Dictionary<string, JsonObject>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			AccountWeapon weaponData = GetWeaponData(GameSettings.instance.Weapons[i].WeaponID, data);
			if ((bool)defaultData.Weapons[i].Buy != (bool)data.Weapons[i].Buy && !dictionary.ContainsKey(i.ToString()))
			{
				dictionary.Add(i.ToString(), GetWeaponToJson(weaponData));
			}
			if ((int)defaultData.Weapons[i].Upgrade != (int)data.Weapons[i].Upgrade && !dictionary.ContainsKey(i.ToString()))
			{
				dictionary.Add(i.ToString(), GetWeaponToJson(weaponData));
			}
			if ((int)defaultData.Weapons[i].SelectedSkin != (int)data.Weapons[i].SelectedSkin && !dictionary.ContainsKey(i.ToString()))
			{
				dictionary.Add(i.ToString(), GetWeaponToJson(weaponData));
			}
			if (defaultData.Weapons[i].Skins.Count != data.Weapons[i].Skins.Count && !dictionary.ContainsKey(i.ToString()))
			{
				dictionary.Add(i.ToString(), GetWeaponToJson(weaponData));
			}
			if (dictionary.ContainsKey(i.ToString()))
			{
				continue;
			}
			if (defaultData.Weapons[i].FireStats.Count != data.Weapons[i].FireStats.Count)
			{
				dictionary.Add(i.ToString(), GetWeaponToJson(weaponData));
				continue;
			}
			for (int j = 0; j < data.Weapons[i].FireStats.Count; j++)
			{
				if ((int)defaultData.Weapons[i].FireStats[j] != (int)data.Weapons[i].FireStats[j])
				{
					dictionary.Add(i.ToString(), GetWeaponToJson(weaponData));
					break;
				}
			}
		}
		if (dictionary.Count != 0)
		{
			jsonObject.Add("Weapons", dictionary);
		}
		return jsonObject;
	}

	private static JsonObject GetWeaponToJson(AccountWeapon weapon)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.Add("ID", (int)weapon.ID);
		jsonObject.Add("Buy", (bool)weapon.Buy);
		jsonObject.Add("Upgrade", (int)weapon.Upgrade);
		jsonObject.Add("SelectedSkin", (int)weapon.SelectedSkin);
		List<int> list = new List<int>();
		for (int i = 0; i < weapon.Skins.Count; i++)
		{
			list.Add(weapon.Skins[i]);
		}
		jsonObject.Add("Skins", list);
		JsonObject jsonObject2 = new JsonObject();
		for (int j = 0; j < weapon.FireStats.Count; j++)
		{
			if ((int)weapon.FireStats[j] != -1)
			{
				jsonObject2.Add(j.ToString("D2"), (int)weapon.FireStats[j]);
			}
		}
		jsonObject.Add("FireStats", jsonObject2);
		return jsonObject;
	}

	public static string ConvertAccountID(string id, bool encrypt)
	{
		string text = string.Empty;
		id = id.ToLower();
		bool flag = false;
		if (encrypt)
		{
			for (int i = 0; i < id.Length; i++)
			{
				flag = false;
				for (int j = 0; j < a.Count; j++)
				{
					if (id[i].ToString() == a[j])
					{
						text += b[j];
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					text += id[i];
				}
			}
			return text.ToUpper();
		}
		for (int k = 0; k < id.Length; k++)
		{
			flag = false;
			for (int l = 0; l < b.Count; l++)
			{
				if (id[k].ToString() == b[l])
				{
					text += a[l];
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				text += id[k];
			}
		}
		return text;
	}
}
