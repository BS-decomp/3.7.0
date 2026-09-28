using System.Collections.Generic;
using Photon;

public class ZombieModeManager : MonoBehaviour
{
	public ZombieBlock[] Blocks;

	private static ZombieModeManager instance;

	private void Awake()
	{
		PhotonNetwork.AddSendMonoMessageTargets(base.gameObject);
		instance = this;
	}

	public static void AddDamage(byte id)
	{
		instance.photonView.RPC("PhotonAddDamage", PhotonTargets.All, id);
	}

	[PunRPC]
	private void PhotonAddDamage(byte id)
	{
		for (int i = 0; i < Blocks.Length; i++)
		{
			if (Blocks[i].ID == id)
			{
				Blocks[i].Attack();
				if (PhotonNetwork.isMasterClient && (int)Blocks[i].CountAttack == 0)
				{
					base.photonView.RPC("DeactiveBlock", PhotonTargets.All, id);
				}
			}
		}
	}

	[PunRPC]
	private void DeactiveBlock(byte id)
	{
		for (int i = 0; i < Blocks.Length; i++)
		{
			if (Blocks[i].ID == id)
			{
				Blocks[i].SetActive(false);
			}
		}
	}

	[PunRPC]
	private void DeactiveBlocks(byte[] ids)
	{
		for (int i = 0; i < Blocks.Length; i++)
		{
			for (int j = 0; j < ids.Length; j++)
			{
				if (Blocks[i].ID == ids[j])
				{
					Blocks[i].SetActive(false);
				}
			}
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		List<byte> list = new List<byte>();
		for (int i = 0; i < Blocks.Length; i++)
		{
			if (!Blocks[i].GetActive())
			{
				list.Add((byte)Blocks[i].ID);
			}
		}
		base.photonView.RPC("DeactiveBlocks", playerConnect, list.ToArray());
	}
}
