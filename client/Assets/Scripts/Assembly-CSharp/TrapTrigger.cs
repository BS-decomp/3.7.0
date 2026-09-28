using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component != null)
		{
			DamageInfo damageInfo = DamageInfo.Create(1000, Vector3.zero, Team.None, 0, -1);
			component.Damage(damageInfo);
		}
	}
}
