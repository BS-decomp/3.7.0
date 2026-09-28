using System;
using System.Collections.Generic;
using UnityEngine;

public class mQuickPlay : MonoBehaviour
{
	public UIPopupList SelectModePopupList;

	public UIPopupList SelectMapPopupList;

	public GameObject SelectedMaxPlayers;

	public UIGrid Grid;

	private int MaxPlayers;

	public void Open()
	{
		SelectModePopupList.Clear();
		SelectModePopupList.AddItem("Any");
		for (int i = 0; i < Enum.GetValues(typeof(GameMode)).Length; i++)
		{
			SelectModePopupList.AddItem(((GameMode)i).ToString());
		}
		SelectModePopupList.value = SelectModePopupList.items[0];
		UpdateMaps();
	}

	private void UpdateMaps()
	{
		if (SelectModePopupList.value == "Any")
		{
			SelectMapPopupList.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			SelectMapPopupList.transform.parent.gameObject.SetActive(true);
			List<string> gameModeScenes = LevelManager.GetGameModeScenes((GameMode)(int)Enum.Parse(typeof(GameMode), SelectModePopupList.value));
			SelectMapPopupList.Clear();
			SelectMapPopupList.AddItem(Localization.Get("Any"));
			for (int i = 0; i < gameModeScenes.Count; i++)
			{
				SelectMapPopupList.AddItem(gameModeScenes[i]);
			}
			SelectMapPopupList.value = SelectMapPopupList.items[0];
		}
		Grid.repositionNow = true;
	}

	public void OnSelectGameMode()
	{
		UpdateMaps();
	}

	public void SetMaxPlayer(GameObject go)
	{
		if (go.name == "-")
		{
			MaxPlayers = 0;
		}
		else
		{
			MaxPlayers = int.Parse(go.name);
		}
		TweenPosition.Begin(SelectedMaxPlayers, 0.2f, go.transform.localPosition);
	}

	public void QuickPlay()
	{
		mPopUp.ShowText(Localization.Get("Search Server") + "...");
		vp_Timer.In(0.2f + UnityEngine.Random.value, () =>
		{
			mPhotonSettings.OnQuickPlay(SelectModePopupList.value, SelectMapPopupList.value, MaxPlayers);
		});
	}
}
