using UnityEngine;

public class TrapStart : MonoBehaviour
{
	public GameObject Target;

	private vp_Timer.Handle Timer = new vp_Timer.Handle();

	private void Start()
	{
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", WaitPlayer);
	}

	private void StartRound()
	{
		Target.SetActive(true);
		if (Timer.Active)
		{
			Timer.Cancel();
		}
		vp_Timer.In(5f, () =>
		{
			Target.SetActive(false);
		});
	}

	private void WaitPlayer()
	{
		if (Timer.Active)
		{
			Timer.Cancel();
		}
		Target.SetActive(false);
	}
}
