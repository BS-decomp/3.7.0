using CodeStage.AntiCheat.ObscuredTypes;
using Photon;
using UnityEngine;

public class ZombieBlock : Photon.MonoBehaviour
{
	[Range(1f, 50f)]
	public int ID;

	public ObscuredInt CountAttack = 50;

	[Range(1f, 20f)]
	public int Button;

	public GameObject ActiveBlock;

	private ObscuredInt StartCountAttack = 50;

	private GameObject CacheGameObject;

	private void Start()
	{
		StartCountAttack = CountAttack;
		CacheGameObject = base.gameObject;
		EventManager.AddListener("Button" + Button, ButtonClick);
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
	}

	private void StartRound()
	{
		SetActive(false);
	}

	private void ButtonClick()
	{
		SetActive(true);
		CountAttack = StartCountAttack;
	}

	public void Damage(DamageInfo info)
	{
		if (info.AttackerTeam == Team.Red)
		{
			UICrosshair.Hit();
			ZombieModeManager.AddDamage((byte)ID);
		}
	}

	public void Attack()
	{
		CountAttack = (int)CountAttack - 1;
		CountAttack = Mathf.Clamp(CountAttack, 0, StartCountAttack);
	}

	public void SetActive(bool active)
	{
		CacheGameObject.SetActive(active);
		if (ActiveBlock != null)
		{
			ActiveBlock.SetActive(!active);
		}
	}

	public bool GetActive()
	{
		return CacheGameObject.activeSelf;
	}
}
