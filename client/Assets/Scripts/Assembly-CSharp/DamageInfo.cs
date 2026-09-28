using UnityEngine;

public class DamageInfo
{
	public int Damage;

	public Vector3 AttackPosition;

	public Team AttackerTeam;

	public int WeaponID;

	public int WeaponSkinID;

	public int PlayerID;

	public bool HeadShot;

	public static DamageInfo Create(int damage, Vector3 attackPosition, Team attackerTeam, int weaponID, int playerID)
	{
		DamageInfo damageInfo = new DamageInfo();
		damageInfo.Damage = damage;
		damageInfo.AttackPosition = attackPosition;
		damageInfo.AttackerTeam = attackerTeam;
		damageInfo.WeaponID = weaponID;
		damageInfo.WeaponSkinID = AccountManager.GetWeaponSkinSelected(weaponID);
		damageInfo.PlayerID = playerID;
		return damageInfo;
	}
}
