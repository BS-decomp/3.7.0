using UnityEngine;

public class AmbientManager : MonoBehaviour
{
	public AudioSource Source;

	public AudioClip[] Ambients;

	private void Start()
	{
		EventManager.AddListener("UpdateSettings", UpdateSettings);
		Play();
	}

	private void Play()
	{
		if (!Settings.AmbientAudio)
		{
			return;
		}
		string sceneName = LevelManager.GetSceneName();
		for (int i = 0; i < GameSettings.instance.Ambients.Count; i++)
		{
			if (GameSettings.instance.Ambients[i].Map == sceneName)
			{
				Source.clip = Ambients[GameSettings.instance.Ambients[i].Ambient];
				Source.volume = GameSettings.instance.Ambients[i].Volume;
				Source.Play();
				break;
			}
		}
	}

	private void UpdateSettings()
	{
		if (Settings.AmbientAudio)
		{
			if (!Source.isPlaying)
			{
				Play();
			}
		}
		else
		{
			Source.Stop();
			Source.clip = null;
		}
	}
}
