using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveLoadManager
{
	public static bool HasFriend(string friend)
	{
		if (PlayerPrefs.HasKey("Friends"))
		{
			string text = PlayerPrefs.GetString("Friends");
			string[] source = text.Split("|"[0]);
			return source.Contains(friend);
		}
		return false;
	}

	public static string[] GetFriends()
	{
		if (PlayerPrefs.HasKey("Friends"))
		{
			string text = PlayerPrefs.GetString("Friends");
			return text.Split("|"[0]);
		}
		return new string[0];
	}

	public static void SetFriend(string friend)
	{
		List<string> list = GetFriends().ToList();
		list.Add(friend);
		SetFriends(list.ToArray());
	}

	public static void SetFriends(string[] friends)
	{
		string text = string.Empty;
		for (int i = 0; i < friends.Length; i++)
		{
			text = ((friends.Length - 1 == i) ? (text + friends[i]) : (text + friends[i] + "|"));
		}
		PlayerPrefs.SetString("Friends", text);
	}

	public static string GetFriendsLine()
	{
		if (PlayerPrefs.HasKey("Friends"))
		{
			return PlayerPrefs.GetString("Friends");
		}
		return string.Empty;
	}

	public static void SetFriendsLine(string friends)
	{
		PlayerPrefs.SetString("Friends", friends);
	}
}
