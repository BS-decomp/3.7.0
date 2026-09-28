using UnityEngine;

public class SelectedWeaponAttribute : PropertyAttribute
{
	public bool AllWeapons;

	public WeaponTypeList weaponType;

	public int selected;

	public SelectedWeaponAttribute(WeaponTypeList weapon)
	{
		weaponType = weapon;
	}

	public SelectedWeaponAttribute()
	{
		AllWeapons = true;
	}
}
