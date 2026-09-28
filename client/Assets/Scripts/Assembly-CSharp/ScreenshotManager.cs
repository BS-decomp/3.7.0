using System;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.U))
		{
			GameObject gameObject = GameObject.Find("UI Root");
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			string filename = "C:/Users/Tibers/Desktop/WeaponScreenshot " + DateTime.Now.ToString("ddhhmmss") + ".png";
			Application.CaptureScreenshot(filename, 2);
		}
	}
}
