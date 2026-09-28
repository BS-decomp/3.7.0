using System.Collections.Generic;
using Crypto;
using FreeJSON;
using UnityEngine;

public class mPlayerRoundManager : MonoBehaviour
{
	public GameObject Icon;

	public GameObject Scroll;

	public GameObject Element;

	private List<GameObject> ElementList = new List<GameObject>();

	private JsonArray Array;

	private void Start()
	{
		EventManager.AddListener("UpdatePlayerRoundData", CheckData);
		CheckData();
	}

	private void CheckData()
	{
		Icon.SetActive(CryptoPrefs.HasKey("PlayerDataRound"));
	}

	public void Open()
	{
		Clear();
		Array = JsonArray.Parse(CryptoPrefs.GetString("PlayerDataRound"));
		for (int i = 0; i < Array.Length; i++)
		{
			GameObject gameObject = NGUITools.AddChild(Scroll, Element);
			gameObject.SetActive(true);
			gameObject.transform.localPosition = new Vector3(i * 240 - 240, 0f, 0f);
			gameObject.GetComponent<mPlayerRoundElement>().SetData(Array.Get<string>(i), i + 1, this);
			ElementList.Add(gameObject);
		}
	}

	private void Clear()
	{
		for (int i = 0; i < ElementList.Count; i++)
		{
			Object.Destroy(ElementList[i]);
		}
		ElementList.Clear();
	}

	public void SaveData(JsonObject json, int index)
	{
		AccountManager.SetMoney1(json.Get<int>("m"));
		AccountManager.SetXP1(json.Get<int>("x"));
		AccountManager.SetKills1(json.Get<int>("k"));
		AccountManager.SetDeaths1(json.Get<int>("d"));
		AccountManager.SetHeadshot1(json.Get<int>("h"));
		Array.RemoveAt(index);
		if (Array.Length == 0)
		{
			CryptoPrefs.DeleteKey("PlayerDataRound");
		}
		else
		{
			CryptoPrefs.SetString("PlayerDataRound", Array.ToString());
		}
		Open();
		CheckData();
		AccountManager.UpdateDefaultData(null, null);
		AccountManager.UpdateWeaponsData(null, null);
		EventManager.Dispatch("AccountUpdate");
	}
}
