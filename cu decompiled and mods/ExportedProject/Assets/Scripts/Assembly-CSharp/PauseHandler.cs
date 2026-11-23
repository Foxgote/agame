using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseHandler : MonoBehaviour
{
	public static PauseHandler main;

	public bool isPaused;

	public GameObject pauseContainer;

	public List<RectTransform> buttons;

	public Image quitButton;

	public float pauseProgress;

	public Image background;

	public RectTransform center;

	private List<Vector2> buttonPos;

	public TextMeshProUGUI pauseText;

	public int timesQuitPressed;

	public Sprite exitSprite;

	public static bool paused
	{
		get
		{
			if ((bool)main)
			{
				return main.isPaused;
			}
			return false;
		}
	}

	public bool disallowPause
	{
		get
		{
			if (!PlayerCamera.main.didDeathScreen && !PlayerCamera.main.selfDestructActive && !WorldGeneration.world.generatingWorld)
			{
				return WorldGeneration.world.cutsceneActive;
			}
			return true;
		}
	}

	public static void TogglePause()
	{
		if ((bool)main && !SettingsMenu.instance && (main.isPaused || !main.disallowPause))
		{
			main.isPaused = !main.isPaused;
			if (main.isPaused)
			{
				PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.InventoryOpen, 0.6f);
			}
			else
			{
				PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.InventoryClose, 0.6f);
			}
		}
	}

	public void TryTogglePause()
	{
		TogglePause();
	}

	public void ResetQuitPressed()
	{
		timesQuitPressed = 0;
		quitButton.color = Color.white;
		quitButton.GetComponent<UITooltip>().tipName = Locale.GetOther("pausegiveup");
		quitButton.GetComponent<UITooltip>().tipDesc = Locale.GetOther("pausegiveupdsc");
	}

	public void TryQuit()
	{
		if (WorldGeneration.world.biomeOverride == WorldGeneration.OverrideSceneType.Tutorial)
		{
			PlayerCamera.main.ToMainMenu();
			return;
		}
		timesQuitPressed++;
		if (timesQuitPressed == 1)
		{
			PlayerCamera.PlayUISound("selfharmtap");
			quitButton.color = new Color(1f, 0.8f, 0.8f);
			quitButton.GetComponent<UITooltip>().tipName = Locale.GetOther("pausegiveup2");
		}
		if (timesQuitPressed == 2)
		{
			PlayerCamera.PlayUISound("selfharmtap", 0.75f);
			quitButton.color = new Color(1f, 0.5f, 0.5f);
			quitButton.GetComponent<UITooltip>().tipName = Locale.GetOther("pausegiveup3");
		}
		if (timesQuitPressed == 3)
		{
			PlayerCamera.PlayUISound("selfharmtap", 0.6f);
			quitButton.color = new Color(1f, 0f, 0f);
			quitButton.GetComponent<UITooltip>().tipName = Locale.GetOther("pausegiveup4");
		}
		if (timesQuitPressed != 4)
		{
			return;
		}
		PlayerCamera.PlayUISound("selfharmtap", 0.35f);
		if (WorldGeneration.unchipped)
		{
			GlobalDark.main.SetDarkAmount(1f);
			GlobalDark.main.SetStayDark(stay: false);
			GlobalDark.main.SetSpeedMultiplier(1f);
			PlayerCamera.main.ToMainMenu();
			return;
		}
		quitButton.color = new Color(0.3f, 0f, 0f);
		quitButton.GetComponent<UITooltip>().tipName = Locale.GetOther("pausegiveup5");
		quitButton.GetComponent<UITooltip>().tipDesc = Locale.GetOther("pausegiveup5dsc");
		if (!PlayerCamera.main.selfDestructActive)
		{
			TogglePause();
		}
		PlayerCamera.main.StartSelfDestruct();
	}

	private void Start()
	{
		buttonPos = new List<Vector2>();
		foreach (RectTransform button in buttons)
		{
			buttonPos.Add(button.localPosition);
		}
		if (WorldGeneration.world.biomeOverride == WorldGeneration.OverrideSceneType.Tutorial)
		{
			quitButton.sprite = exitSprite;
			quitButton.GetComponent<UITooltip>().tipName = Locale.GetOther("pauseexit");
			quitButton.GetComponent<UITooltip>().tipDesc = "";
			quitButton.GetComponent<UITooltip>().skipLocale = true;
		}
	}

	private void Awake()
	{
		main = this;
	}

	public void Settings()
	{
		if (paused)
		{
			SettingsMenu.OpenMenu(base.transform);
		}
	}

	public void Update()
	{
		if (paused && disallowPause)
		{
			TogglePause();
		}
		float target = (isPaused ? 1f : 0f);
		pauseProgress = Mathf.MoveTowards(pauseProgress, target, Time.unscaledDeltaTime * 1.66f);
		if (pauseProgress > 0f && !pauseContainer.activeSelf)
		{
			pauseContainer.SetActive(value: true);
			string type = "normal";
			if (WorldGeneration.world.biomeOverride == WorldGeneration.OverrideSceneType.Tutorial)
			{
				type = "normal";
			}
			else if ((bool)EndLabScript.instance && EndLabScript.instance.doingEscape)
			{
				type = "labescape";
			}
			else if (PlayerCamera.main.body.hasCargo)
			{
				type = "cargoembedded";
			}
			else if (PlayerCamera.main.threatMusicTime > 0f)
			{
				type = ((PlayerCamera.main.currentThreatTheme != 10 && PlayerCamera.main.currentThreatTheme != 15) ? "combat" : "combatelder");
			}
			else if (PlayerCamera.main.body.isDying)
			{
				type = "dying";
			}
			else if (PlayerCamera.main.body.totalHappiness > 50f)
			{
				type = "happy";
			}
			else if (PlayerCamera.main.body.totalHappiness <= -50f)
			{
				type = "depressed";
			}
			pauseText.text = Locale.GetPauseQuote(type);
		}
		if (pauseProgress <= 0f && pauseContainer.activeSelf)
		{
			pauseContainer.SetActive(value: false);
		}
		if (pauseProgress >= 1f && Time.timeScale != 0f)
		{
			PlayerCamera.main.SetTimeScale(PlayerCamera.SpeedType.Paused, switchSound: false, force: true);
		}
		if (pauseProgress < 1f && Time.timeScale == 0f)
		{
			PlayerCamera.main.SetTimeScale(PlayerCamera.SpeedType.Normal, switchSound: false, force: true);
		}
		if (!(pauseProgress > 0f))
		{
			return;
		}
		background.color = Color.black.WithAlpha(Utils.InOutCubic(pauseProgress));
		center.localScale = Vector3.one * Utils.OutCubic(pauseProgress);
		float num = 0f;
		foreach (RectTransform button in buttons)
		{
			button.localScale = Vector3.one * Utils.InOutQuint(Mathf.Clamp01(pauseProgress + num));
			num += 0.1f;
		}
		num = 0f;
		for (int i = 0; i < buttons.Count; i++)
		{
			Vector2 vector = buttonPos[i];
			vector.y *= 0.5f + Utils.InOutCubic(pauseProgress + num) * 0.5f;
			vector.x *= 0.5f + Utils.OutCubic(pauseProgress + num) * 0.5f;
			buttons[i].localPosition = vector;
			num += 0.1f;
		}
	}
}
