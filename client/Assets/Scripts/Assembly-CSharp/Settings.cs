using UnityEngine;

public class Settings
{
	public static bool FPSMeter
	{
		get
		{
			if (PlayerPrefs.GetInt("FPSMeter", 0) == 0)
			{
				return false;
			}
			return true;
		}
		set
		{
			PlayerPrefs.SetInt("FPSMeter", value ? 1 : 0);
		}
	}

	public static bool Console
	{
		get
		{
			if (PlayerPrefs.GetInt("Console", 0) == 0)
			{
				return false;
			}
			return true;
		}
		set
		{
			PlayerPrefs.SetInt("Console", value ? 1 : 0);
			Utils.SetActiveConsole(value);
		}
	}

	public static bool Chat
	{
		get
		{
			if (PlayerPrefs.GetInt("Chat", 1) == 0)
			{
				return false;
			}
			return true;
		}
		set
		{
			PlayerPrefs.SetInt("Chat", value ? 1 : 0);
		}
	}

	public static bool ShowDamage
	{
		get
		{
			if (PlayerPrefs.GetInt("ShowDamage", 0) == 0)
			{
				return false;
			}
			return true;
		}
		set
		{
			PlayerPrefs.SetInt("ShowDamage", value ? 1 : 0);
		}
	}

	public static bool BulletHole
	{
		get
		{
			if (PlayerPrefs.GetInt("BulletHole", 1) == 0)
			{
				return false;
			}
			return true;
		}
		set
		{
			PlayerPrefs.SetInt("BulletHole", value ? 1 : 0);
		}
	}

	public static bool Blood
	{
		get
		{
			if (PlayerPrefs.GetInt("Blood", 1) == 0)
			{
				return false;
			}
			return true;
		}
		set
		{
			PlayerPrefs.SetInt("Blood", value ? 1 : 0);
		}
	}

	public static bool HitMaker
	{
		get
		{
			if (PlayerPrefs.GetInt("HitMaker", 1) == 0)
			{
				return false;
			}
			return true;
		}
		set
		{
			PlayerPrefs.SetInt("HitMaker", value ? 1 : 0);
		}
	}

	public static int ColorCrosshair
	{
		get
		{
			return PlayerPrefs.GetInt("ColorCrosshair", 0);
		}
		set
		{
			PlayerPrefs.SetInt("ColorCrosshair", value);
		}
	}

	public static float Sensitivity
	{
		get
		{
			return PlayerPrefs.GetFloat("Sensitivity", 0.2f);
		}
		set
		{
			PlayerPrefs.SetFloat("Sensitivity", value);
		}
	}

	public static float Volume
	{
		get
		{
			return AudioListener.volume = PlayerPrefs.GetFloat("Volume", 0.8f);
		}
		set
		{
			PlayerPrefs.SetFloat("Volume", value);
			AudioListener.volume = value;
		}
	}

	public static bool Audio
	{
		get
		{
			if (PlayerPrefs.GetInt("Audio", 1) == 1)
			{
				return true;
			}
			return false;
		}
		set
		{
			PlayerPrefs.SetInt("Audio", value ? 1 : 0);
		}
	}

	public static bool AmbientAudio
	{
		get
		{
			if (PlayerPrefs.GetInt("AmbientAudio", 1) == 1)
			{
				return true;
			}
			return false;
		}
		set
		{
			PlayerPrefs.SetInt("AmbientAudio", value ? 1 : 0);
		}
	}

	public static bool Ragdoll
	{
		get
		{
			if (PlayerPrefs.GetInt("Ragdoll", 1) == 1)
			{
				return true;
			}
			return false;
		}
		set
		{
			PlayerPrefs.SetInt("Ragdoll", value ? 1 : 0);
		}
	}

	public static bool Lefty
	{
		get
		{
			if (PlayerPrefs.GetInt("Lefty", 0) == 1)
			{
				return true;
			}
			return false;
		}
		set
		{
			PlayerPrefs.SetInt("Lefty", value ? 1 : 0);
		}
	}

	public static float ButtonAlpha
	{
		get
		{
			float value = PlayerPrefs.GetFloat("ButtonAlpha", 1f);
			return Mathf.Clamp(value, 0.01f, 1f);
		}
		set
		{
			PlayerPrefs.SetFloat("ButtonAlpha", value);
		}
	}
}
