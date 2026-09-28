using UnityEngine;

public class ZombieSounds : TimerBehaviour
{
	public AudioClip[] Clips;

	public AudioSource Source;

	private bool Active;

	private void OnEnable()
	{
		Active = true;
		PlaySound();
	}

	private void OnDisable()
	{
		Active = false;
		Source.Stop();
	}

	private void PlaySound()
	{
		vp_Timer.In(Random.Range(5, 10), () =>
		{
			if (Active && !Source.isPlaying)
			{
				AudioClip clip = Clips[Random.Range(0, Clips.Length)];
				Source.clip = clip;
				Source.Play();
				PlaySound();
			}
		});
	}
}
