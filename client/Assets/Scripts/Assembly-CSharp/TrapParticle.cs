using UnityEngine;

public class TrapParticle : MonoBehaviour
{
	[Range(1f, 20f)]
	public int Key = 1;

	public ParticleSystem Target;

	public float DelayIn;

	public float DelayOut = 3f;

	private bool isTrigger;

	private bool Active;

	private bool Activated;

	private PlayerInput Player;

	private vp_Timer.Handle Timer = new vp_Timer.Handle();

	private void Start()
	{
		Target.Stop();
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
		EventManager.AddListener("Button" + Key, ActiveTrap);
	}

	private void ActiveTrap()
	{
		if (Activated)
		{
			return;
		}
		Activated = true;
		vp_Timer.In(DelayIn, () =>
		{
			Active = true;
			Target.Play();
			if (isTrigger && Player != null)
			{
				DamageInfo damageInfo = DamageInfo.Create(1000, Vector3.zero, Team.None, 0, -1);
				Player.Damage(damageInfo);
			}
			if (DelayOut != 0f)
			{
				vp_Timer.In(DelayOut, () =>
				{
					if (Activated)
					{
						Active = false;
						Target.Stop();
					}
				});
			}
		}, Timer);
	}

	private void OnTriggerEnter(Collider other)
	{
		Player = other.GetComponent<PlayerInput>();
		if (Player != null)
		{
			isTrigger = true;
			if (Active)
			{
				DamageInfo damageInfo = DamageInfo.Create(1000, Vector3.zero, Team.None, 0, -1);
				Player.Damage(damageInfo);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Player = other.GetComponent<PlayerInput>();
		if (Player != null)
		{
			isTrigger = false;
		}
	}

	private void StartRound()
	{
		Target.Stop();
		Activated = false;
		Active = false;
		Timer.Cancel();
	}
}
