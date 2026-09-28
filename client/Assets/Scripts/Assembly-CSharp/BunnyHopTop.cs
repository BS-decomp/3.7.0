using UnityEngine;

public class BunnyHopTop : MonoBehaviour
{
	public string LevelName;

	private vp_Timer.Handle Timer;

	private static BunnyHopTop instance;

	private void Start()
	{
		instance = this;
		FirebaseManager.DebugAction = true;
		Firebase firebase = new Firebase();
		firebase.Child("GameModes").Child("BunnyHop").Child("Top")
			.Child(LevelName)
			.GetValue(FirebaseParam.Default.OrderByValue().StartAt(10).LimitToFirst(5));
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			StartTimer();
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			MonoBehaviour.print(Timer.Duration);
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			StopTimer();
		}
	}

	public static void StartTimer()
	{
		if (instance.Timer == null)
		{
			instance.Timer = new vp_Timer.Handle();
		}
		if (instance.Timer.Active)
		{
			instance.Timer.Cancel();
		}
		vp_Timer.Start(instance.Timer);
	}

	public static void StopTimer()
	{
		if (instance.Timer != null && instance.Timer.Active)
		{
			instance.Timer.Cancel();
		}
	}

	public static void UpdateTime()
	{
	}
}
