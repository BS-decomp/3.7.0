using UnityEngine;

public class BunnyAutoJump : MonoBehaviour
{
	private PlayerInput Player;

	public float jumpTime = 2f;

	private vp_Timer.Handle Timer = new vp_Timer.Handle();

	private void OnTriggerEnter(Collider other)
	{
		Player = other.GetComponent<PlayerInput>();
		if (Player != null)
		{
			Player.SetBunnyHopAutoJump(true);
			vp_Timer.In(jumpTime, () =>
			{
				BunnyHop.SpawnDead();
			}, Timer);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (Player != null)
		{
			Player.SetBunnyHopAutoJump(false);
			Player = null;
			Timer.Cancel();
		}
	}
}
