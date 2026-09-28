using System;
using System.Collections;
using UnityEngine;

public class SpraysManager : MonoBehaviour
{
	public Transform Spray;

	public Material SprayMaterial;

	public string SprayUrl;

	public Transform CheckSpray;

	public float distance = 2f;

	public float size = 0.25f;

	public float pushDistance = 0.009f;

	private static SpraysManager instance;

	private void Start()
	{
		instance = this;
		SprayUrl = PlayerPrefs.GetString("SprayUrl", "https://lh3.googleusercontent.com/o20wzRU8vRMkFLMZnSlvyfAjIgT-Ihvqsgwdh2NNnJY7ckCcV5RM0FmC3i6YTPJm=w64");
	}

	private bool CheckPosition(RaycastHit hit)
	{
		CheckSpray.transform.position = hit.point + hit.normal * pushDistance;
		CheckSpray.transform.forward = -hit.normal;
		if (Physics.Raycast(CheckSpray.position, CheckSpray.forward, out hit, distance))
		{
			RaycastHit hitInfo = default(RaycastHit);
			Vector3 origin = Vector3.zero;
			Vector3 direction = Vector3.zero;
			for (int i = 1; i < 5; i++)
			{
				switch (i)
				{
				case 1:
					origin = CheckSpray.position - CheckSpray.right * size;
					direction = CheckSpray.forward;
					break;
				case 2:
					origin = CheckSpray.position + CheckSpray.right * size;
					direction = CheckSpray.forward;
					break;
				case 3:
					origin = CheckSpray.position - CheckSpray.up * size;
					direction = CheckSpray.forward;
					break;
				case 4:
					origin = CheckSpray.position + CheckSpray.up * size;
					direction = CheckSpray.forward;
					break;
				case 5:
					origin = CheckSpray.position - CheckSpray.forward * 0.1f;
					direction = Spray.right;
					break;
				case 6:
					origin = CheckSpray.position - CheckSpray.forward * 0.1f;
					direction = -Spray.right;
					break;
				case 7:
					origin = CheckSpray.position - CheckSpray.forward * 0.1f;
					direction = Spray.up;
					break;
				case 8:
					origin = CheckSpray.position - CheckSpray.forward * 0.1f;
					direction = -Spray.up;
					break;
				}
				if (Physics.Raycast(origin, direction, out hitInfo, distance))
				{
					if (!(hit.transform == hitInfo.transform) || Math.Round(hit.distance, 1) != Math.Round(hitInfo.distance, 1))
					{
						return false;
					}
					continue;
				}
				return false;
			}
			for (int j = 1; j < 5; j++)
			{
				switch (j)
				{
				case 1:
					origin = CheckSpray.position - CheckSpray.forward * 0.1f;
					direction = CheckSpray.right;
					break;
				case 2:
					origin = CheckSpray.position - CheckSpray.forward * 0.1f;
					direction = -CheckSpray.right;
					break;
				case 3:
					origin = CheckSpray.position - CheckSpray.forward * 0.1f;
					direction = CheckSpray.up;
					break;
				case 4:
					origin = CheckSpray.position - CheckSpray.forward * 0.1f;
					direction = -CheckSpray.up;
					break;
				}
				if (Physics.Raycast(origin, direction, out hitInfo, size))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private void CreateSpray(RaycastHit hit)
	{
		GameManager.CreateSpray(hit.normal, hit.point, SprayUrl);
	}

	private void RemoveSpray()
	{
		Spray.position = Vector3.zero;
	}

	public static void CreateSpray(Vector3 point, Vector3 normal, string url)
	{
		instance.Spray.gameObject.SetActive(true);
		instance.Spray.position = point + normal * instance.pushDistance;
		instance.Spray.forward = -normal;
		instance.StopAllCoroutines();
		instance.StartCoroutine(instance.LoadTexture(url));
	}

	private IEnumerator LoadTexture(string url)
	{
		PlayerPrefs.SetString("SprayUrl", url);
		WWW www = new WWW(url);
		yield return www;
		if (www.isDone && www.texture != null)
		{
			SprayMaterial.mainTexture = www.texture;
			float m = Mathf.Max(www.texture.width, www.texture.height);
			Spray.localScale = new Vector3((float)www.texture.width / m, (float)www.texture.height / m, 1f);
		}
	}
}
