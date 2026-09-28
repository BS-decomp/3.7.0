namespace UnityEngine.SceneManagement
{
	public class SceneManager
	{
		public static void LoadScene(string name)
		{
			LevelManager.LoadLevel(name);
		}

		public static void LoadScene(int buildIndex)
		{
			Application.LoadLevel(buildIndex);
		}
	}
}
