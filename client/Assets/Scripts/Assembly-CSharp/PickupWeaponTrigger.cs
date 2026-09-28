using UnityEngine;

public class PickupWeaponTrigger : MonoBehaviour
{
	[SelectedWeapon(WeaponTypeList.Rifle)]
	public int Weapon;

	private void OnTriggerEnter(Collider other)
	{
		PlayerInput player = other.GetComponent<PlayerInput>();
		if (player != null && !player.PlayerWeapon.GetWeaponData(WeaponTypeList.Rifle).Enabled)
		{
			WeaponManager.SetRifleType(Weapon);
			player.PlayerWeapon.UpdateWeaponData(WeaponTypeList.Rifle);
			vp_Timer.In(0.05f, () =>
			{
				player.PlayerWeapon.SetWeapon(WeaponTypeList.Rifle, false);
			});
		}
	}
}
