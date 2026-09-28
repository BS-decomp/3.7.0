using UnityEngine;

public class TrapButtonActive : MonoBehaviour
{
	[Range(1f, 25f)]
	public int Key;

	public TrapButton[] Buttons;

	private void Start()
	{
		EventManager.AddListener("Button" + Key, DeactiveButtons);
	}

	private void DeactiveButtons()
	{
		for (int i = 0; i < Buttons.Length; i++)
		{
			Buttons[i].DeactiveButton();
		}
	}
}
