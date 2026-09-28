using UnityEngine;

public class InputChangeKey : MonoBehaviour
{
	public string Button;

	private UILabel Label;

	private void Start()
	{
		Label = GetComponent<UILabel>();
		OnChangeKey();
	}

	private void OnEnable()
	{
		cInput.OnKeyChanged += OnChangeKey;
	}

	private void OnDisable()
	{
		cInput.OnKeyChanged -= OnChangeKey;
	}

	private void OnChangeKey()
	{
		Label.text = cInput.GetText(Button);
	}

	private void OnClick()
	{
		cInput.ChangeKey(Button);
		OnChangeKey();
	}
}
