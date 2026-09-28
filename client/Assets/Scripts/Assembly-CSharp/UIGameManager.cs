using System;
using System.Collections.Generic;
using UnityEngine;

public class UIGameManager : MonoBehaviour
{
	public UILabel NameLabel;

	[Header("Health")]
	public UILabel HealthLabel;

	public GameObject HealthPanel;

	[Header("Ammo")]
	public UILabel AmmoLabel;

	public GameObject AmmoPanel;

	[Header("Score")]
	public GameObject Score;

	public UILabel MaxScoreLabel;

	public UILabel BlueScoreLabel;

	public UILabel RedScoreLabel;

	public TweenScale BlueScoreTween;

	public TweenScale RedScoreTween;

	[HideInInspector]
	public bool isScoreTimer;

	[HideInInspector]
	public float ScoreTimer;

	private Action ScoreTimerAction;

	[Header("FPS Meter")]
	public UILabel FPSMeterLabel;

	private bool isFPSMeter;

	private float FPSMeterAccum;

	private float FPSMeterFrames;

	[Header("Controller")]
	public GameObject ControllerPanel;

	public GameObject RifleScopeButton;

	public GameObject SelectWeaponButton;

	[Header("Pause")]
	public GameObject PauseWeapons;

	[Header("UI Status")]
	public UILabel StatusLabel;

	private List<string> StatusTextList = new List<string>();

	[Header("UI Chat")]
	public UILabel ChatLabel;

	public UIInput ChatInput;

	private List<string> ChatTextList = new List<string>();

	public static List<string> ChatPlayersIgnore = new List<string>();

	[Header("UI Main Status")]
	public UILabel MainStatusLabel;

	private vp_Timer.Handle MainStatusTimer = new vp_Timer.Handle();

	public static UIGameManager instance;

	private void Awake()
	{
		instance = this;
		PhotonNetwork.AddSendMonoMessageTargets(base.gameObject);
	}

	private void Start()
	{
		EventManager.AddListener("UpdateSettings", UpdateSettings);
		UpdateSettings();
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent += GetButtonDown;
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent -= GetButtonDown;
	}

	private void GetButtonDown(string name)
	{
		switch (name)
		{
		case "Pause":
			UpdatePause();
			break;
		case "Chat":
			UpdateChat();
			break;
		}
	}

	private void Update()
	{
		UpdateFPSMeter();
		UpdateScore();
	}

	private void UpdatePause()
	{
		UIPanelManager.ShowPanel("Pause");
		if (!GameManager.GetChangeWeapons())
		{
			PauseWeapons.SetActive(false);
		}
	}

	public static void NewStatusLine(string text)
	{
		instance.StatusTextList.Add(text);
		instance.UpdateStatusLabel(true);
	}

	private void UpdateStatusLabel(bool clear)
	{
		string text = string.Empty;
		for (int i = 0; i < StatusTextList.Count; i++)
		{
			if (i > 0)
			{
				text += "\n";
			}
			text += StatusTextList[StatusTextList.Count - 1 - i];
		}
		StatusLabel.text = text;
		if (clear)
		{
			vp_Timer.In(5f, RemoveStatusLabel);
		}
	}

	private void RemoveStatusLabel()
	{
		StatusTextList.RemoveAt(0);
		UpdateStatusLabel(false);
	}

	private void UpdateChat()
	{
		if (!UICamera.inputHasFocus)
		{
			ChatLabel.text = string.Empty;
			vp_Timer.In(0.1f, () =>
			{
				UICamera.selectedObject = ChatInput.gameObject;
			});
		}
	}

	public void OnSubmitChat()
	{
		string value = ChatInput.value;
		value = value.Replace("\n", string.Empty);
		if (!string.IsNullOrEmpty(value))
		{
			ChatInput.value = string.Empty;
			ChatInput.isSelected = false;
			GameManager.OnChat(value);
		}
	}

	public static void NewChatLine(string text)
	{
		if (Settings.Chat)
		{
			instance.ChatLabel.supportEncoding = true;
			instance.ChatTextList.Add(text);
			instance.UpdateChatLabel(true);
		}
	}

	private void UpdateChatLabel(bool clear)
	{
		string text = string.Empty;
		for (int i = 0; i < ChatTextList.Count; i++)
		{
			if (i > 0)
			{
				text += "\n";
			}
			text += ChatTextList[ChatTextList.Count - 1 - i];
		}
		ChatLabel.text = text;
		if (clear)
		{
			vp_Timer.In(8f, RemoveChatLabel);
		}
	}

	private void RemoveChatLabel()
	{
		ChatTextList.RemoveAt(0);
		UpdateChatLabel(false);
	}

	public static void ShowMainStatusText(string text)
	{
		ShowMainStatusText(text, 5f);
	}

	public static void ShowMainStatusText(string text, float duration)
	{
		instance.MainStatusLabel.text = text;
		if (instance.MainStatusTimer.Active)
		{
			instance.MainStatusTimer.Cancel();
		}
		vp_Timer.In(duration, instance.HideMainStatusText, instance.MainStatusTimer);
	}

	private void HideMainStatusText()
	{
		MainStatusLabel.text = string.Empty;
	}

	private void UpdateSettings()
	{
		isFPSMeter = Settings.FPSMeter;
		CancelInvoke("UpdateFPSMeterLabel");
		if (isFPSMeter)
		{
			InvokeRepeating("UpdateFPSMeterLabel", 0.5f, 0.5f);
		}
		FPSMeterLabel.gameObject.SetActive(isFPSMeter);
	}

	private void UpdateFPSMeter()
	{
		if (isFPSMeter)
		{
			FPSMeterAccum += Time.timeScale / Time.deltaTime;
			FPSMeterFrames++;
		}
	}

	private void UpdateFPSMeterLabel()
	{
		float num = FPSMeterAccum / FPSMeterFrames;
		string text = string.Format("{0:F2} FPS", num);
		FPSMeterAccum = 0f;
		FPSMeterFrames = 0f;
		FPSMeterLabel.text = text;
	}

	public static void SetHealthLabel(int health)
	{
		if (health == 0)
		{
			instance.HealthLabel.text = string.Empty;
			instance.HealthPanel.SetActive(false);
			instance.AmmoLabel.text = string.Empty;
			instance.AmmoPanel.SetActive(false);
		}
		else
		{
			if (!instance.HealthPanel.activeSelf)
			{
				instance.HealthPanel.SetActive(true);
			}
			instance.HealthLabel.text = "+" + health;
		}
	}

	public static void SetAmmoLabel(int ammo, int maxAmmo)
	{
		SetAmmoLabel(ammo, maxAmmo, false);
	}

	public static void SetAmmoLabel(int ammo, int maxAmmo, bool infinity)
	{
		if (maxAmmo == -1)
		{
			instance.AmmoLabel.text = string.Empty;
			instance.AmmoPanel.SetActive(false);
			return;
		}
		if (!instance.AmmoPanel.activeSelf)
		{
			instance.AmmoPanel.SetActive(true);
		}
		if (infinity)
		{
			instance.AmmoLabel.text = ammo + "/∞";
		}
		else
		{
			instance.AmmoLabel.text = ammo + "/" + maxAmmo;
		}
	}

	private void UpdateScore()
	{
		if (!isScoreTimer)
		{
			return;
		}
		float num = ScoreTimer - Time.time;
		int num2 = (int)num / 60;
		int num3 = (int)num - num2 * 60;
		MaxScoreLabel.text = string.Format("{0:0}:{1:00}", num2, num3);
		if (ScoreTimer <= Time.time)
		{
			isScoreTimer = false;
			if (ScoreTimerAction != null)
			{
				ScoreTimerAction();
			}
		}
	}

	public static void SetActiveScore(bool active, int maxScore)
	{
		instance.Score.SetActive(active);
		instance.MaxScoreLabel.text = maxScore.ToString();
	}

	public static void UpdateScoreLabel(int maxScore, int blueScore, int redScore)
	{
		if (instance.BlueScoreLabel.text != blueScore.ToString())
		{
			instance.BlueScoreTween.Toggle();
		}
		if (instance.RedScoreLabel.text != redScore.ToString())
		{
			instance.RedScoreTween.Toggle();
		}
		instance.MaxScoreLabel.text = maxScore.ToString();
		instance.BlueScoreLabel.text = blueScore.ToString();
		instance.RedScoreLabel.text = redScore.ToString();
	}

	public static void StartScoreTimer(float time, Action finishAction)
	{
		instance.isScoreTimer = true;
		instance.ScoreTimer = time + Time.time;
		instance.ScoreTimerAction = finishAction;
	}

	public static void StopScoreTimer()
	{
		instance.isScoreTimer = false;
	}

	public static bool ScopeTimer()
	{
		return instance.isScoreTimer;
	}

	public static void SetActiveRifleScope(bool active)
	{
		instance.RifleScopeButton.SetActive(active);
	}

	public static void SetActiveSelectWeapon(bool active)
	{
		instance.SelectWeaponButton.SetActive(active);
	}

	public void OnExitServer()
	{
		PhotonNetwork.LeaveRoom();
	}

	private void OnLeftRoom()
	{
		PlayerRoundManager.Show();
		LevelManager.LoadLevel("Menu");
	}

	public void OnDefaultKeys()
	{
		cInput.ResetInputs();
	}
}
