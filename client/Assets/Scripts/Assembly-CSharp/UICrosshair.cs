using DG.Tweening;
using UnityEngine;

public class UICrosshair : MonoBehaviour
{
	public Transform LeftSprite;

	public Transform RightSprite;

	public Transform TopSprite;

	public Transform BottomSprite;

	public float MaxAccuracy;

	public float Accuracy;

	public float Duration;

	private Tweener Tween;

	[Header("Hit Settings")]
	public UIWidget HitSprite;

	public float HitDuration;

	private Tweener HitTween;

	private bool HitMarker = true;

	[Header("Rifle Scope")]
	public GameObject RifleScope;

	private int SelectColor;

	private static UICrosshair instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		EventManager.AddListener("UpdateSettings", UpdateSettings);
		UpdateSettings();
		Tween = DOTween.To(() => Accuracy, (float x) =>
		{
			Accuracy = x;
		}, 5f, Duration).SetAutoKill(false);
		Tween.OnUpdate(() =>
		{
			LeftSprite.localPosition = Vector3.left * Accuracy;
			RightSprite.localPosition = Vector3.right * Accuracy;
			TopSprite.localPosition = Vector3.up * Accuracy;
			BottomSprite.localPosition = Vector3.down * Accuracy;
		});
		HitTween = DOTween.To(() => instance.HitSprite.alpha, (float x) =>
		{
			instance.HitSprite.alpha = x;
		}, 0f, instance.HitDuration).SetAutoKill(false);
	}

	public static void SetAccuracy(float accuracy)
	{
		instance.Tween.ChangeEndValue(accuracy * 1.5f, true);
		instance.Accuracy = accuracy * 1.5f;
		instance.UpdateCrosshair();
	}

	public static Vector2 Fire(float accuracy)
	{
		Vector2 result = Vector3.zero;
		if (accuracy != 0f)
		{
			result = new Vector2(instance.Accuracy / 1600f, instance.Accuracy / 960f);
			instance.Accuracy += accuracy * 1.5f;
			instance.Accuracy = Mathf.Min(instance.Accuracy, instance.MaxAccuracy);
			instance.UpdateCrosshair();
		}
		return result;
	}

	private void UpdateCrosshair()
	{
		Tween.ChangeStartValue(Accuracy).Restart();
	}

	public static void Hit()
	{
		if (instance.HitMarker)
		{
			instance.HitSprite.alpha = 1f;
			instance.HitTween.ChangeStartValue(instance.HitSprite.alpha).Restart();
		}
	}

	public static void SetActiveRifleScope(bool active)
	{
		instance.RifleScope.SetActive(active);
		SetActiveCrosshair(!active);
	}

	public static void SetActiveCrosshair(bool active)
	{
		try
		{
			instance.LeftSprite.gameObject.SetActive(active);
			instance.RightSprite.gameObject.SetActive(active);
			instance.TopSprite.gameObject.SetActive(active);
			instance.BottomSprite.gameObject.SetActive(active);
		}
		catch
		{
		}
	}

	private void UpdateSettings()
	{
		HitMarker = Settings.HitMaker;
		int colorCrosshair = Settings.ColorCrosshair;
		if (SelectColor != colorCrosshair)
		{
			SelectColor = colorCrosshair;
			Color color = Utils.GetColor(colorCrosshair);
			LeftSprite.GetComponent<UISprite>().color = color;
			RightSprite.GetComponent<UISprite>().color = color;
			TopSprite.GetComponent<UISprite>().color = color;
			BottomSprite.GetComponent<UISprite>().color = color;
		}
	}
}
