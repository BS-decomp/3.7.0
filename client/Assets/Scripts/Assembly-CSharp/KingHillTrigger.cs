using UnityEngine;

public class KingHillTrigger : MonoBehaviour
{
	public float Distance = 4f;

	public float UpdateTime = 2f;

	public Color GizmosColor = Color.cyan;

	private Vector3 Position;

	private void Start()
	{
		Position = base.transform.position;
		InvokeRepeating("UpdateCollider", UpdateTime, UpdateTime);
	}

	private void UpdateCollider()
	{
		if (!PhotonNetwork.isMasterClient || PhotonNetwork.playerList.Length < 2)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		if (!GameManager.GetController().PlayerInput.Dead && isZone(GameManager.GetController().PlayerInput.PlayerTransform))
		{
			if (GameManager.GetController().PlayerInput.PlayerTeam == Team.Blue)
			{
				flag = true;
			}
			else
			{
				flag2 = true;
			}
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("SpectatePoint");
		for (int i = 0; i < array.Length; i++)
		{
			ControllerManager component = array[i].transform.root.GetComponent<ControllerManager>();
			if (component != null && !component.PlayerSkin.Dead && isZone(component.PlayerSkin.transform))
			{
				if (component.PlayerSkin.PlayerTeam == Team.Blue)
				{
					flag = true;
				}
				else
				{
					flag2 = true;
				}
				if (flag2 && flag)
				{
					break;
				}
			}
		}
		if (flag && !flag2)
		{
			KingHillMode.OnScore(Team.Blue);
		}
		else if (!flag && flag2)
		{
			KingHillMode.OnScore(Team.Red);
		}
	}

	private bool isZone(Transform target)
	{
		if (Mathf.Abs(Vector3.Distance(Position, target.position)) <= Distance)
		{
			return true;
		}
		return false;
	}
}
