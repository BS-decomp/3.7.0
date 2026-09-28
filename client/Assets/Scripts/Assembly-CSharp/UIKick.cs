using System.Collections.Generic;
using UnityEngine;

public class UIKick : MonoBehaviour
{
	private bool CanKick = true;

	private int Positive;

	private int Negative;

	private bool isKick;

	private int IDNotification;

	private PhotonPlayer KickPlayer;

	private static UIKick instance;

	public static List<string> KickedServers = new List<string>();

	private void Awake()
	{
		instance = this;
	}

	public void Kick()
	{
		if (!CanKick)
		{
			return;
		}
		if (AccountManager.GetLevel() < 15)
		{
			UIToast.Show(Localization.Get("Required level greater than 15"));
			return;
		}
		if (!PhotonNetwork.isMasterClient)
		{
			UIToast.Show(Localization.Get("Only Admin"));
			return;
		}
		if (isKick)
		{
			UIToast.Show(Localization.Get("Previous voting is not finished"));
			return;
		}
		KickPlayer = UIPlayerStatistics.SelectPlayer;
		if (PhotonNetwork.room.playerCount < 3)
		{
			GameManager.KickPlayer(KickPlayer);
			return;
		}
		UIToast.Show(Localization.Get("Voting started"));
		GameManager.StartKickPlayer(KickPlayer);
	}

	public static void AddVote(bool positive)
	{
		if (positive)
		{
			instance.Positive++;
		}
		else
		{
			instance.Negative++;
		}
		string text = Localization.Get("Kick") + " " + instance.KickPlayer.name + ": " + Localization.Get("Yes") + " " + instance.Positive + ", " + Localization.Get("No") + " " + instance.Negative;
		GameManager.OnStatus(text, true, string.Empty);
	}

	public static void StartKickPlayer(double time, PhotonPlayer player)
	{
		instance.Positive = 1;
		instance.Negative = 0;
		instance.IDNotification = (int)time;
		instance.isKick = true;
		instance.KickPlayer = player;
		if (!PhotonNetwork.isMasterClient && PhotonNetwork.player.ID != player.ID)
		{
			UINotification.Add(instance.IDNotification, Localization.Get("You want to kick a player") + ": " + player.name, Localization.Get("Yes"), Localization.Get("No"), instance.ClickPositive, instance.ClickNegative);
		}
		int num = (int)(time + 20.0 - PhotonNetwork.time);
		vp_Timer.In(num, instance.FinishKickPlayer);
	}

	private void FinishKickPlayer()
	{
		isKick = false;
		UINotification.Remove(IDNotification);
		if (PhotonNetwork.isMasterClient && Positive > Negative)
		{
			CanKick = false;
			GameManager.KickPlayer(KickPlayer);
		}
	}

	private void ClickPositive()
	{
		GameManager.AddKickPlayerVote(true);
	}

	private void ClickNegative()
	{
		GameManager.AddKickPlayerVote(false);
	}

	public static bool HasKick()
	{
		return instance.CanKick;
	}
}
