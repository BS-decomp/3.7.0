using System.Text;
using UnityEngine;

public class UISelectTeam : MonoBehaviour
{
	public UILabel BluePlayersLabel;

	public UILabel RedPlayersLabel;

	public UILabel BlueCountLabel;

	public UILabel RedCountLabel;

	private int redPlayersCount;

	private int bluePlayersCount;

	private vp_Timer.Handle Timer = new vp_Timer.Handle();

	private static UISelectTeam instance;

	private void Awake()
	{
		instance = this;
	}

	public static void OnStart()
	{
		UIPanelManager.ShowPanel("SelectTeam");
		instance.UpdateList();
	}

	private void UpdateList()
	{
		PhotonPlayer[] otherPlayers = PhotonNetwork.otherPlayers;
		redPlayersCount = 0;
		bluePlayersCount = 0;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		for (int i = 0; i < otherPlayers.Length; i++)
		{
			if (otherPlayers[i].GetTeam() == Team.Blue)
			{
				stringBuilder2.AppendLine(otherPlayers[i].name);
				bluePlayersCount++;
			}
			else if (otherPlayers[i].GetTeam() == Team.Red)
			{
				stringBuilder.AppendLine(otherPlayers[i].name);
				redPlayersCount++;
			}
		}
		BluePlayersLabel.text = stringBuilder2.ToString();
		RedPlayersLabel.text = stringBuilder.ToString();
		BlueCountLabel.text = bluePlayersCount + "/" + PhotonNetwork.room.maxPlayers / 2;
		RedCountLabel.text = redPlayersCount + "/" + PhotonNetwork.room.maxPlayers / 2;
		if (!Timer.Active)
		{
			vp_Timer.In(0.1f, UpdateList, -1, 0.1f, Timer);
		}
	}

	public void SelectTeam(int team)
	{
		if (HasConnectTeam((Team)team))
		{
			GameManager.OnSelectTeam((Team)team);
			Timer.Cancel();
		}
	}

	private bool HasConnectTeam(Team team)
	{
		switch (team)
		{
		case Team.Blue:
			if (bluePlayersCount - redPlayersCount >= 1)
			{
				return false;
			}
			if (PhotonNetwork.room.maxPlayers / 2 == bluePlayersCount)
			{
				return false;
			}
			return true;
		case Team.Red:
			if (redPlayersCount - bluePlayersCount >= 1)
			{
				return false;
			}
			if (PhotonNetwork.room.maxPlayers / 2 == redPlayersCount)
			{
				return false;
			}
			return true;
		default:
			return false;
		}
	}
}
