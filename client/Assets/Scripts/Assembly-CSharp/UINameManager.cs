using UnityEngine;

public class UINameManager : MonoBehaviour
{
	public Camera m_Camera;

	private UILabel Label;

	private float Timer = 0.1f;

	private string LastName;

	private string PlayerName;

	private void Start()
	{
		PlayerName = PhotonNetwork.playerName;
		Label = UIGameManager.instance.NameLabel;
	}

	private void OnDisable()
	{
		try
		{
			Label.text = string.Empty;
		}
		catch
		{
		}
	}

	private void Update()
	{
		Timer -= Time.deltaTime;
		if (!(Timer <= 0f))
		{
			return;
		}
		Timer = 0.1f;
		Ray ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f))
		{
			if (hitInfo.collider.CompareTag("PlayerSkin"))
			{
				string text = hitInfo.transform.root.name;
				if (text != LastName)
				{
					if (text != PlayerName)
					{
						ControllerManager component = hitInfo.transform.root.GetComponent<ControllerManager>();
						if (component.PlayerSkin.PlayerTeam == Team.Blue)
						{
							Label.effectColor = Color.blue;
						}
						else
						{
							Label.effectColor = Color.red;
						}
						LastName = text;
						Label.text = LastName;
					}
				}
				else
				{
					Label.text = LastName;
				}
			}
			else if (!string.IsNullOrEmpty(Label.text))
			{
				Label.text = string.Empty;
			}
		}
		else if (!string.IsNullOrEmpty(Label.text))
		{
			Label.text = string.Empty;
		}
	}
}
