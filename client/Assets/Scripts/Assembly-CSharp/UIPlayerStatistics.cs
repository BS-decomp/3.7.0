using System.Collections.Generic;
using UnityEngine;

public class UIPlayerStatistics : MonoBehaviour
{
	[Header("Parents")]
	public Transform BlueTeamParent;

	public Transform RedTeamParent;

	[Header("Panel Labels")]
	public UILabel BlueLabel;

	public UILabel RedLabel;

	public UILabel SpectatorsLabel;

	[Header("Server")]
	public UILabel ServerNameLabel;

	public UILabel ModeLabel;

	public UILabel MapLabel;

	public UILabel PlayersLabel;

	[Header("Player Info")]
	public GameObject PlayerInfoPanel;

	public UILabel PlayerInfoNameLabel;

	public UILabel PlayerInfoPlayfabLabel;

	public UISprite PlayerInfoKickButton;

	public UILabel PlayerChatLabel;

	[Header("Others")]
	public GameObject Container;

	public GameObject Root;

	public static PhotonPlayer SelectPlayer;

	private bool isShow;

	private List<UIPlayerStatisticsElement> PlayerList = new List<UIPlayerStatisticsElement>();

	private List<UIPlayerStatisticsElement> PlayerListPool = new List<UIPlayerStatisticsElement>();

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent += GetButtonDown;
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent -= GetButtonDown;
	}

	private void GetButtonDown(string name)
	{
		if (name == "Statistics")
		{
			Show();
		}
	}

	private void Show()
	{
		if (PhotonNetwork.offlineMode)
		{
			return;
		}
		UIPanelManager.ShowPanel("Statistics");
		ServerNameLabel.text = PhotonNetwork.room.name;
		PlayersLabel.text = Localization.Get("Players") + ": " + PhotonNetwork.room.playerCount + "/" + PhotonNetwork.room.maxPlayers;
		MapLabel.text = Localization.Get("Map") + ": " + PhotonNetwork.room.GetSceneName();
		if (PhotonNetwork.room.GetGameMode() == GameMode.Only)
		{
			ModeLabel.text = Localization.Get(PhotonNetwork.room.GetGameMode().ToString()) + " (" + WeaponManager.GetWeaponName(PhotonNetwork.room.GetOnlyWeapon()) + ")";
		}
		else
		{
			ModeLabel.text = Localization.Get("Mode") + ": " + Localization.Get(PhotonNetwork.room.GetGameMode().ToString());
		}
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		List<PhotonPlayer> list2 = new List<PhotonPlayer>();
		List<string> list3 = new List<string>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue)
			{
				list.Add(playerList[i]);
			}
			else if (playerList[i].GetTeam() == Team.Red)
			{
				list2.Add(playerList[i]);
			}
			else if (playerList[i].GetTeam() == Team.None)
			{
				list3.Add(playerList[i].name);
			}
		}
		if (PhotonNetwork.room.GetGameMode() == GameMode.PvP)
		{
			list.Sort(PvPMode.SortByPvP);
			list2.Sort(PvPMode.SortByPvP);
		}
		else
		{
			list.Sort(SortByKills);
			list2.Sort(SortByKills);
		}
		if (PhotonNetwork.room.GetGameMode() == GameMode.ZombieSurvival)
		{
			BlueLabel.text = Localization.Get("Survivors");
		}
		else
		{
			BlueLabel.text = Localization.Get("Blue Team");
		}
		if (PhotonNetwork.room.GetGameMode() == GameMode.ZombieSurvival)
		{
			RedLabel.text = Localization.Get("Zombie");
		}
		else
		{
			RedLabel.text = Localization.Get("Red Team");
		}
		SpectatorsLabel.text = Localization.Get("Spectators") + ": " + string.Join(",", list3.ToArray());
		for (int j = 0; j < list.Count; j++)
		{
			UIPlayerStatisticsElement playerContainer = GetPlayerContainer(list[j].name);
			playerContainer.SetData(list[j]);
			playerContainer.Transform.SetParent(BlueTeamParent);
			if (j == 0)
			{
				playerContainer.Transform.localPosition = Vector3.zero;
			}
			else
			{
				playerContainer.Transform.localPosition = Vector3.down * 25f * j;
			}
			PlayerList.Add(playerContainer);
		}
		for (int k = 0; k < list2.Count; k++)
		{
			UIPlayerStatisticsElement playerContainer2 = GetPlayerContainer(list2[k].name);
			playerContainer2.SetData(list2[k]);
			playerContainer2.Transform.SetParent(RedTeamParent);
			if (k == 0)
			{
				playerContainer2.Transform.localPosition = Vector3.zero;
			}
			else
			{
				playerContainer2.Transform.localPosition = Vector3.down * 25f * k;
			}
			PlayerList.Add(playerContainer2);
		}
	}

	public void Close()
	{
		UIPanelManager.ShowPanel("Display");
		ClearList();
	}

	private UIPlayerStatisticsElement GetPlayerContainer(string name)
	{
		if (PlayerListPool.Count != 0)
		{
			UIPlayerStatisticsElement uIPlayerStatisticsElement = null;
			for (int i = 0; i < PlayerListPool.Count; i++)
			{
				if (PlayerListPool[i].PlayerNameLabel.text == name)
				{
					uIPlayerStatisticsElement = PlayerListPool[i];
					PlayerListPool.RemoveAt(i);
					return uIPlayerStatisticsElement;
				}
			}
			uIPlayerStatisticsElement = PlayerListPool[0];
			PlayerListPool.RemoveAt(0);
			return uIPlayerStatisticsElement;
		}
		GameObject gameObject = NGUITools.AddChild(Root, Container);
		return gameObject.GetComponent<UIPlayerStatisticsElement>();
	}

	private void ClearList()
	{
		if (PlayerList.Count != 0)
		{
			for (int i = 0; i < PlayerList.Count; i++)
			{
				PlayerListPool.Add(PlayerList[i]);
			}
			PlayerList.Clear();
			for (int j = 0; j < PlayerListPool.Count; j++)
			{
				PlayerListPool[j].Widget.alpha = 0f;
			}
		}
	}

	public static int SortByKills(PhotonPlayer a, PhotonPlayer b)
	{
		if (a.GetKills() == b.GetKills())
		{
			if (a.GetDeaths() == b.GetDeaths())
			{
				if (a.GetLevel() == b.GetLevel())
				{
					return b.name.CompareTo(a.name);
				}
				return b.GetLevel().CompareTo(a.GetLevel());
			}
			return a.GetDeaths().CompareTo(b.GetDeaths());
		}
		return b.GetKills().CompareTo(a.GetKills());
	}

	public static int GetPlayerStatsPosition(PhotonPlayer player)
	{
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			if (PhotonNetwork.playerList[i].GetTeam() == player.GetTeam())
			{
				list.Add(PhotonNetwork.playerList[i]);
			}
		}
		list.Sort(SortByKills);
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j] == player)
			{
				return j + 1;
			}
		}
		return 1;
	}

	public void OnSelectPlayer(UIPlayerStatisticsElement player)
	{
		if (player.PlayerInfo != null && player.PlayerInfo.ID != PhotonNetwork.player.ID)
		{
			SelectPlayer = player.PlayerInfo;
			PlayerInfoPanel.SetActive(true);
			PlayerInfoNameLabel.text = player.PlayerInfo.name;
			PlayerInfoPlayfabLabel.text = player.PlayerInfo.GetPlayerID();
			if (PhotonNetwork.isMasterClient && UIKick.HasKick())
			{
				PlayerInfoKickButton.alpha = 1f;
			}
			else
			{
				PlayerInfoKickButton.alpha = 0.5f;
			}
			if (UIGameManager.ChatPlayersIgnore.Contains(SelectPlayer.name))
			{
				PlayerChatLabel.text = Localization.Get("Chat") + ": " + Localization.Get("Off");
			}
			else
			{
				PlayerChatLabel.text = Localization.Get("Chat") + ": " + Localization.Get("On");
			}
		}
	}

	public void OnChatIgnore()
	{
		if (UIGameManager.ChatPlayersIgnore.Contains(SelectPlayer.name))
		{
			UIGameManager.ChatPlayersIgnore.Remove(SelectPlayer.name);
			PlayerChatLabel.text = Localization.Get("Chat") + ": " + Localization.Get("On");
		}
		else
		{
			UIGameManager.ChatPlayersIgnore.Add(SelectPlayer.name);
			PlayerChatLabel.text = Localization.Get("Chat") + ": " + Localization.Get("Off");
		}
	}
}
