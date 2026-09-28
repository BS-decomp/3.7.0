using UnityEngine;

public class TriggerTeleport : MonoBehaviour
{
	public Transform Target;

	public Vector3 Position;

	private void OnTriggerEnter(Collider other)
	{
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (!(component == null))
		{
			if (Position != Vector3.zero)
			{
				component.Controller.SetPosition(Position);
				return;
			}
			component.Controller.SetPosition(Target.position);
			component.FPCamera.SetRotation(Target.eulerAngles);
		}
	}
}
