using UnityEngine;

public class SceneManagerHelper
{
	public static string ActiveSceneName
	{
		get
		{
			return LevelManager.GetSceneName();
		}
	}

	public static int ActiveSceneBuildIndex
	{
		get
		{
			return Application.loadedLevel;
		}
	}
}
