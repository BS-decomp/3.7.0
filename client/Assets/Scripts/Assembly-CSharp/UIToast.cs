using UnityEngine;

public class UIToast : MonoBehaviour
{
	public UILabel label;

	public UISprite background;

	private vp_Timer.Handle Timer = new vp_Timer.Handle();

	private static UIToast instance;

	private void Awake()
	{
		instance = this;
	}

	public static void Show(string text)
	{
		Show(text, 2f);
	}

	public static void Show(string text, float duration)
	{
		if (instance.Timer.Active)
		{
			instance.label.alpha = 0f;
			instance.Timer.Cancel();
		}
		TweenAlpha.Begin(instance.label.cachedGameObject, 0.2f, 1f);
		instance.label.text = text;
		instance.background.UpdateAnchors();
		vp_Timer.In(duration, () =>
		{
			TweenAlpha.Begin(instance.label.cachedGameObject, 0.2f, 0f);
		}, instance.Timer);
	}
}
