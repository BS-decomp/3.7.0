using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public enum CameraType
	{
		None = 0,
		Dead = 1,
		Static = 2,
		Spectate = 3
	}

	public CameraType SelectCameraType;

	public Camera m_Camera;

	private Rigidbody m_Rigidbody;

	private Transform m_Transform;

	private GameObject[] SpectatePoints;

	private int SelectPoint;

	private Transform Point;

	public float Distance;

	public float DistanceMin;

	public float DistanceMax;

	public float SpeedRotation;

	private float X;

	private float Y;

	private static CameraManager instance;

	private void Awake()
	{
		instance = this;
		m_Rigidbody = m_Camera.GetComponent<Rigidbody>();
		m_Transform = m_Camera.transform;
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent += GetButtonDown;
		InputManager.GetAxisEvent += GetAxis;
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent -= GetButtonDown;
		InputManager.GetAxisEvent -= GetAxis;
	}

	private void GetButtonDown(string name)
	{
		if (name == "Fire" && SelectCameraType == CameraType.Spectate)
		{
			UpdatePlayers();
		}
	}

	private void GetAxis(string name, float value)
	{
		switch (name)
		{
		case "Mouse X":
			X += value * SpeedRotation * Distance * 0.02f;
			break;
		case "Mouse Y":
			Y -= value * SpeedRotation * 0.02f;
			break;
		}
	}

	public static void ActiveDeadCamera(Vector3 position, Vector3 rotation, Vector3 force)
	{
		if (instance.SelectCameraType == CameraType.Spectate)
		{
			DeactiveSpectateCamera();
		}
		instance.SelectCameraType = CameraType.Dead;
		instance.m_Transform.gameObject.SetActive(true);
		instance.m_Camera.GetComponent<Collider>().isTrigger = false;
		instance.m_Rigidbody.isKinematic = false;
		instance.m_Transform.position = position;
		instance.m_Transform.eulerAngles = rotation;
		instance.m_Rigidbody.velocity = Vector3.zero;
		instance.m_Rigidbody.AddForce(force);
		instance.m_Rigidbody.AddRelativeForce(force);
	}

	public static void DeactiveDeadCamera()
	{
		instance.SelectCameraType = CameraType.None;
		instance.m_Rigidbody.isKinematic = true;
		instance.m_Camera.GetComponent<Collider>().isTrigger = true;
		instance.m_Transform.gameObject.SetActive(false);
	}

	public static void ActiveSpectateCamera()
	{
		if (instance.SelectCameraType == CameraType.Dead)
		{
			DeactiveDeadCamera();
		}
		instance.SelectCameraType = CameraType.Spectate;
		instance.m_Transform.gameObject.SetActive(true);
		instance.UpdatePlayers();
	}

	public static void DeactiveSpectateCamera()
	{
		instance.SelectCameraType = CameraType.None;
		instance.m_Transform.gameObject.SetActive(false);
	}

	public static void ActiveStaticCamera()
	{
		if (instance.SelectCameraType == CameraType.Dead)
		{
			DeactiveDeadCamera();
		}
		if (instance.SelectCameraType == CameraType.Spectate)
		{
			DeactiveSpectateCamera();
		}
		instance.SelectCameraType = CameraType.Static;
		instance.m_Transform.gameObject.SetActive(true);
		Transform transform = GameObject.FindGameObjectWithTag("StaticPoint").transform;
		instance.m_Transform.position = transform.position;
		instance.m_Transform.rotation = transform.rotation;
	}

	public static void DeactiveStaticCamera()
	{
		instance.SelectCameraType = CameraType.None;
		instance.m_Transform.gameObject.SetActive(false);
	}

	public static void DeactiveAll()
	{
		DeactiveDeadCamera();
		DeactiveSpectateCamera();
		DeactiveStaticCamera();
	}

	public static Transform GetActiveCamera()
	{
		if (instance.SelectCameraType == CameraType.None)
		{
			if (PlayerInput.instance == null)
			{
				return null;
			}
			return PlayerInput.instance.FPCamera.Transform;
		}
		return instance.m_Transform;
	}

	private void UpdatePlayers()
	{
		SpectatePoints = GameObject.FindGameObjectsWithTag("SpectatePoint");
		SelectPoint++;
		if (SelectPoint > SpectatePoints.Length - 1)
		{
			SelectPoint = 0;
		}
		if (SpectatePoints.Length != 0)
		{
			Point = SpectatePoints[SelectPoint].transform;
		}
		else
		{
			ActiveStaticCamera();
		}
	}

	private void LateUpdate()
	{
		if ((bool)Point && SelectCameraType == CameraType.Spectate)
		{
			Distance = DistanceMax;
			Y = Utils.ClampAngle(Y, -20f, 80f);
			Quaternion quaternion = Quaternion.Euler(Y, X, 0f);
			Vector3 vector = m_Transform.position - Point.position;
			Ray ray = new Ray(Point.position, vector.normalized);
			RaycastHit hitInfo;
			if (Physics.SphereCast(ray.origin, 0.25f, ray.direction, out hitInfo, Distance))
			{
				Distance = hitInfo.distance;
				Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
			}
			Vector3 vector2 = new Vector3(0f, 0f, 0f - Distance);
			Vector3 position = quaternion * vector2 + Point.position;
			m_Transform.rotation = quaternion;
			m_Transform.position = position;
		}
	}
}
