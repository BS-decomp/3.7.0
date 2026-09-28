using ExitGames.Client.Photon;

internal static class PhotonCustomValue
{
	public static void ClearProperties(this PhotonPlayer player)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["team"] = Team.None;
		hashtable["deaths"] = 0;
		hashtable["kills"] = 0;
		hashtable["dead"] = true;
		player.SetCustomProperties(hashtable);
	}

	public static void SetTeam(this PhotonPlayer player, Team team)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["team"] = team;
		player.SetCustomProperties(hashtable);
	}

	public static Team GetTeam(this PhotonPlayer player)
	{
		object value;
		if (player.customProperties.TryGetValue("team", out value))
		{
			return (Team)(int)value;
		}
		return Team.None;
	}

	public static void SetLevel(this PhotonPlayer player, int level)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["level"] = level;
		player.SetCustomProperties(hashtable);
	}

	public static int GetLevel(this PhotonPlayer player)
	{
		object value;
		if (player.customProperties.TryGetValue("level", out value))
		{
			return (int)value;
		}
		return 1;
	}

	public static void UpdatePing(this PhotonPlayer player)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["ping"] = PhotonNetwork.GetPing();
		player.SetCustomProperties(hashtable);
	}

	public static int GetPing(this PhotonPlayer player)
	{
		object value;
		if (player.customProperties.TryGetValue("ping", out value))
		{
			return (int)value;
		}
		return 200;
	}

	public static void SetDeaths(this PhotonPlayer player, int deaths)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["deaths"] = deaths;
		player.SetCustomProperties(hashtable);
	}

	public static void SetDeaths1(this PhotonPlayer player)
	{
		int num = 0;
		object value;
		if (player.customProperties.TryGetValue("deaths", out value))
		{
			num = (int)value;
		}
		num++;
		Hashtable hashtable = new Hashtable();
		hashtable["deaths"] = num;
		player.SetCustomProperties(hashtable);
	}

	public static int GetDeaths(this PhotonPlayer player)
	{
		object value;
		if (player.customProperties.TryGetValue("deaths", out value))
		{
			return (int)value;
		}
		return 0;
	}

	public static void SetKills(this PhotonPlayer player, int kills)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["kills"] = kills;
		player.SetCustomProperties(hashtable);
	}

	public static void SetKills1(this PhotonPlayer player)
	{
		int num = 0;
		object value;
		if (player.customProperties.TryGetValue("kills", out value))
		{
			num = (int)value;
		}
		num++;
		Hashtable hashtable = new Hashtable();
		hashtable["kills"] = num;
		player.SetCustomProperties(hashtable);
	}

	public static int GetKills(this PhotonPlayer player)
	{
		object value;
		if (player.customProperties.TryGetValue("kills", out value))
		{
			return (int)value;
		}
		return 0;
	}

	public static void SetDead(this PhotonPlayer player, bool dead)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["dead"] = dead;
		player.SetCustomProperties(hashtable);
	}

	public static bool GetDead(this PhotonPlayer player)
	{
		object value;
		if (player.customProperties.TryGetValue("dead", out value))
		{
			return (bool)value;
		}
		return false;
	}

	public static void SetPlayerID(this PhotonPlayer player, string id)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["playerid"] = id;
		player.SetCustomProperties(hashtable);
	}

	public static string GetPlayerID(this PhotonPlayer player)
	{
		object value;
		if (player.customProperties.TryGetValue("playerid", out value))
		{
			return (string)value;
		}
		return string.Empty;
	}

	public static void SetGameMode(this Room room, GameMode mode)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["mode"] = (int)mode;
		room.SetCustomProperties(hashtable);
	}

	public static GameMode GetGameMode(this Room room)
	{
		object value;
		if (room.customProperties.TryGetValue("mode", out value))
		{
			return (GameMode)(int)value;
		}
		return GameMode.TeamDeathmatch;
	}

	public static void SetOnlyWeapon(this Room room, int weaponID)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["onlyWeapon"] = weaponID;
		room.SetCustomProperties(hashtable);
	}

	public static int GetOnlyWeapon(this Room room)
	{
		object value;
		if (room.customProperties.TryGetValue("onlyWeapon", out value))
		{
			return (int)value;
		}
		return 1;
	}

	public static void SetRoundState(this Room room, RoundState state)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["roundstate"] = (int)state;
		room.SetCustomProperties(hashtable);
	}

	public static RoundState GetRoundState(this Room room)
	{
		object value;
		if (room.customProperties.TryGetValue("roundstate", out value))
		{
			return (RoundState)(int)value;
		}
		return RoundState.WaitPlayer;
	}

	public static GameMode GetGameMode(this RoomInfo room)
	{
		object value;
		if (room.customProperties.TryGetValue("mode", out value))
		{
			return (GameMode)(int)value;
		}
		return GameMode.TeamDeathmatch;
	}

	public static string GetSceneName(this RoomInfo room)
	{
		object value;
		if (room.customProperties.TryGetValue("curScn", out value))
		{
			return (string)value;
		}
		return string.Empty;
	}

	public static int GetOnlyWeapon(this RoomInfo room)
	{
		object value;
		if (room.customProperties.TryGetValue("onlyWeapon", out value))
		{
			return (int)value;
		}
		return 1;
	}

	public static Hashtable CreateRoomHashtable(this RoomInfo photonNetwork, string password, GameMode mode)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["password"] = password;
		hashtable["mode"] = (int)mode;
		return hashtable;
	}

	public static string GetPassword(this RoomInfo room)
	{
		object value;
		if (room.customProperties.TryGetValue("password", out value))
		{
			return (string)value;
		}
		return string.Empty;
	}
}
