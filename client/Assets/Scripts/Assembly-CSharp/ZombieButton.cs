using UnityEngine;

public class ZombieButton : MonoBehaviour
{
	public Team PlayerTeam = Team.Blue;

	[Range(1f, 20f)]
	public int Button;

	public KeyCode Keycode;

	public MeshRenderer ButtonRenderer;

	public Transform ButtonRedBlock;

	private bool isTrigger;

	private bool isClickButton;

	private bool Active = true;

	private void Start()
	{
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
		EventManager.AddListener("Button" + Button, DeactiveButton);
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent += GetButtonDown;
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent -= GetButtonDown;
	}

	private void GetButtonDown(string name)
	{
		if (name == "Fire" && isTrigger && !isClickButton && ButtonRenderer.isVisible)
		{
			ClickButton();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isClickButton)
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == PlayerTeam)
			{
				isTrigger = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!isClickButton)
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == PlayerTeam)
			{
				isTrigger = false;
			}
		}
	}

	private void StartRound()
	{
		isClickButton = false;
		isTrigger = false;
		Active = false;
		vp_Timer.In(0.2f, () =>
		{
			Active = true;
			if (ButtonRedBlock != null)
			{
				ButtonRedBlock.localPosition = Vector3.zero;
			}
		});
	}

	private void ClickButton()
	{
		if (Active)
		{
			GameManager.OnEventManager("Button" + Button);
			isClickButton = true;
			isTrigger = false;
		}
	}

	public void DeactiveButton()
	{
		isClickButton = true;
		isTrigger = false;
		if (ButtonRedBlock != null)
		{
			ButtonRedBlock.localPosition = Vector3.back * 0.02f;
		}
	}

	[ContextMenu("Click Button")]
	private void GetClickButton()
	{
		ClickButton();
	}
}
