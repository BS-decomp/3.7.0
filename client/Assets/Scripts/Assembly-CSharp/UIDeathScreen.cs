using UnityEngine;

public class UIDeathScreen : MonoBehaviour
{
	public UIPanel Panel;

	public UILabel PlayerLabel;

	public UILabel WeaponLabel;

	public UILabel DamageLabel;

	public UILabel HeadshotLabel;

	public UITexture WeaponTexture;

	private vp_Timer.Handle Timer = new vp_Timer.Handle();

	private static UIDeathScreen instance;

	private void Start()
	{
		instance = this;
	}

	public static void Show(DamageInfo damageInfo)
	{
		if (damageInfo.PlayerID != -1 && damageInfo.PlayerID != PhotonNetwork.player.ID)
		{
			if (instance.Timer.Active)
			{
				instance.Timer.Cancel();
				instance.Panel.alpha = 0f;
			}
			TweenAlpha.Begin(instance.Panel.cachedGameObject, 0.2f, 1f);
			instance.PlayerLabel.text = PhotonPlayer.Find(damageInfo.PlayerID).name;
			instance.DamageLabel.text = Localization.Get("Damage") + ": " + damageInfo.Damage + "  [" + (int)Vector3.Distance(damageInfo.AttackPosition, PlayerInput.instance.PlayerTransform.position) + "m]";
			instance.HeadshotLabel.text = ((!damageInfo.HeadShot) ? string.Empty : Localization.Get("Headshot"));
			instance.SetWeaponData(damageInfo);
			vp_Timer.In(3f, () =>
			{
				TweenAlpha.Begin(instance.Panel.cachedGameObject, 0.2f, 0f);
			}, instance.Timer);
		}
	}

	private void SetWeaponData(DamageInfo damageInfo)
	{
		WeaponType weapon = WeaponManager.GetWeapon(damageInfo.WeaponID);
		WeaponShopData.WeaponSkin weaponSkin = WeaponManager.GetWeaponSkin(damageInfo.WeaponID, damageInfo.WeaponSkinID);
		WeaponLabel.text = string.Concat(weapon.WeaponName, " | ", weaponSkin.SkinName);
		WeaponLabel.color = GetWeaponSkinRarityColor(weaponSkin.Rarity);
		WeaponTexture.mainTexture = weaponSkin.Source;
		WeaponTexture.width = (int)GameSettings.instance.CaseSize[(int)weapon.WeaponID - 1].x;
		WeaponTexture.height = (int)GameSettings.instance.CaseSize[(int)weapon.WeaponID - 1].y;
		WeaponTexture.uvRect = GameSettings.instance.CaseUVRect[(int)weapon.WeaponID - 1];
	}

	private Color GetWeaponSkinRarityColor(int rarity)
	{
		switch (rarity)
		{
		case 0:
		case 1:
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		case 2:
			return new Color32(54, 189, byte.MaxValue, byte.MaxValue);
		case 3:
			return new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
		case 4:
			return new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
		default:
			return Color.white;
		}
	}
}
