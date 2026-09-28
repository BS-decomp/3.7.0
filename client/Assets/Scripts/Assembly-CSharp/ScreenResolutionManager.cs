using UnityEngine;

public class ScreenResolutionManager : MonoBehaviour
{
	public float UpdateTime = 1f;

	public int RemovePixelsCount = 1;

	public int MinimalFPS = 24;

	public Vector2 screenSize;

	private float timer;

	private float accum;

	private int frames;

	private void Start()
	{
		screenSize = new Vector2(Screen.width, Screen.height);
		timer = UpdateTime;
	}

	private void UpdateScreen()
	{
		timer = UpdateTime;
		float num = accum / (float)frames;
		MonoBehaviour.print(num);
		if ((float)MinimalFPS > num)
		{
			Screen.SetResolution(480, 320, true);
		}
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		frames++;
		accum += Time.timeScale / Time.deltaTime;
		if (timer < 0f)
		{
			UpdateScreen();
		}
	}
}
