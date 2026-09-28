using System.Collections.Generic;
using UnityEngine;

public class UIPlayerStatisticsElement : MonoBehaviour
{
	public UILabel PlayerNameLabel;

	public UILabel LevelLabel;

	public UILabel KillsLabel;

	public UILabel DeathsLabel;

	public UILabel PingLabel;

	public UISprite Background;

	public UIWidget Widget;

	public PhotonPlayer PlayerInfo;

	private vp_Timer.Handle Timer = new vp_Timer.Handle();

	private Transform CacheTransform;

	public Transform Transform
	{
		get
		{
			if (CacheTransform == null)
			{
				CacheTransform = base.transform;
			}
			return CacheTransform;
		}
	}

	private void OnDisable()
	{
		if (Timer.Active)
		{
			Timer.Cancel();
		}
	}

	public void SetData(PhotonPlayer playerInfo)
	{
		PlayerInfo = playerInfo;
		if (PhotonNetwork.room.GetGameMode() == GameMode.PvP)
		{
			PlayerNameLabel.text = GetPlayerNamePvP();
		}
		else
		{
			PlayerNameLabel.text = PlayerInfo.name;
		}
		LevelLabel.text = playerInfo.GetLevel().ToString();
		KillsLabel.text = PlayerInfo.GetKills().ToString();
		DeathsLabel.text = PlayerInfo.GetDeaths().ToString();
		PingLabel.text = PlayerInfo.GetPing().ToString();
		base.name = PlayerNameLabel.text;
		if (playerInfo.GetDead())
		{
			Widget.alpha = 0.5f;
		}
		else
		{
			Widget.alpha = 1f;
		}
		if (playerInfo.isLocal)
		{
			PlayerNameLabel.color = Color.green;
			LevelLabel.color = Color.green;
			KillsLabel.color = Color.green;
			DeathsLabel.color = Color.green;
			PingLabel.color = Color.green;
		}
		else if (playerInfo.isMasterClient)
		{
			PlayerNameLabel.color = Color.magenta;
			LevelLabel.color = Color.magenta;
			KillsLabel.color = Color.magenta;
			DeathsLabel.color = Color.magenta;
			PingLabel.color = Color.magenta;
		}
		else
		{
			PlayerNameLabel.color = Color.white;
			LevelLabel.color = Color.white;
			KillsLabel.color = Color.white;
			DeathsLabel.color = Color.white;
			PingLabel.color = Color.white;
		}
		if (!Timer.Active)
		{
			vp_Timer.In(3f, UpdateData, -1, 3f, Timer);
		}
	}

	private void UpdateData()
	{
		SetData(PlayerInfo);
	}

	private string GetPlayerNamePvP()
	{
		List<int> list = ((PlayerInfo.GetTeam() != Team.Blue) ? PvPMode.RedPlayers : PvPMode.BluePlayers);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == PlayerInfo.ID)
			{
				return i + 1 + "-" + PlayerInfo.name;
			}
		}
		return PlayerInfo.name;
	}
}
