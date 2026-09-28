using UnityEngine;

public class DecalsManager : MonoBehaviour
{
	public float FrontOfWall = 0.02f;

	public Transform[] BulletHoles;

	private int LastBulletHole;

	private bool isBulletHole;

	public Transform BloodEffect;

	private vp_Timer.Handle BloodEffectTimer = new vp_Timer.Handle();

	private bool isBloodEffect;

	private static DecalsManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		EventManager.AddListener("UpdateSettings", UpdateSettings);
		UpdateSettings();
	}

	public static void FireWeapon(DecalInfo decalInfo)
	{
		for (int i = 0; i < decalInfo.Points.Count; i++)
		{
			if (decalInfo.BloodDecal == i)
			{
				if (instance.isBloodEffect)
				{
					instance.CreateBloodEffect(decalInfo.Points[i]);
				}
			}
			else if (!decalInfo.isKnife && instance.isBulletHole)
			{
				instance.CreateBulletHole(decalInfo.Points[i], decalInfo.Normals[i]);
			}
		}
	}

	public void CreateBulletHole(Vector3 point, Vector3 normal)
	{
		if (!(point == Vector3.zero) || !(normal == Vector3.zero))
		{
			if (LastBulletHole > BulletHoles.Length - 1)
			{
				LastBulletHole = 0;
			}
			Transform transform = BulletHoles[LastBulletHole];
			transform.position = point + normal * instance.FrontOfWall;
			transform.rotation = Quaternion.LookRotation(normal);
			Vector3 eulerAngles = transform.eulerAngles;
			transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, Random.value * 360f);
			LastBulletHole++;
		}
	}

	public static void ClearBulletHoles()
	{
		for (int i = 0; i < instance.BulletHoles.Length; i++)
		{
			instance.BulletHoles[i].position = Vector3.right * 5000f;
		}
		instance.LastBulletHole = 0;
	}

	public void CreateBloodEffect(Vector3 pos)
	{
		Transform activeCamera = CameraManager.GetActiveCamera();
		if (!(activeCamera == null))
		{
			if (BloodEffectTimer.Active)
			{
				BloodEffectTimer.Cancel();
			}
			BloodEffect.position = pos;
			BloodEffect.LookAt(activeCamera.position);
			Vector3 eulerAngles = BloodEffect.eulerAngles;
			BloodEffect.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, Random.value * 360f);
			vp_Timer.In(0.05f, () =>
			{
				BloodEffect.position = Vector3.right * 5000f;
			}, BloodEffectTimer);
		}
	}

	private void UpdateSettings()
	{
		isBulletHole = Settings.BulletHole;
		isBloodEffect = Settings.Blood;
		if (!isBulletHole)
		{
			ClearBulletHoles();
		}
	}
}
