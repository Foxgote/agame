using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreRunScript : MonoBehaviour
{
	public static PreRunScript instance;

	public List<Sprite> introImages;

	private static bool didIntro;

	public Button loadButton;

	public Image loadButtonImage;

	public TextMeshProUGUI saveTimeText;

	public List<AudioClip> menuSongs;

	public AudioSource menuSource;

	public TextMeshProUGUI versionText;

	public GameObject runSettingsScreen;

	public Image radioBeep;

	public GameObject contentWarning;

	public Image cover;

	public AudioSource windSource;

	public GameObject tutorialWarning;

	private bool customSettingsActive;

	public RectTransform presetsMenu;

	public RectTransform customSettingsMenu;

	public float customSettingMenuClosedX;

	public float customSettingMenuOpenX;

	public float presetMenuX;

	public float presetMenuScale;

	public float presetMenuXPushed;

	public float presetMenuScalePushed;

	private float customSettingActiveAnim;

	private bool quitting;

	public Dictionary<string, object> runSettings;

	public List<RunSettingDisplay> runSettingObjects;

	public RectTransform customSettingContent;

	public int currentPreset;

	public List<RectTransform> presetRects;

	public Image presetSelectImage;

	public RectTransform sidebarArrow;

	public Canvas mainCanvas;

	public GameObject creditsScreen;

	public static int characterSelected;

	public TextMeshProUGUI charSelectName;

	public TextMeshProUGUI charSelectStats;

	public TextMeshProUGUI charSelectNotes;

	public TextMeshProUGUI charSelectQuirks;

	public List<Image> charImages;

	public List<Image> charInnerImages;

	public List<Image> charPodiums;

	private int savedCharacter;

	public GameObject[] halloweenLayers;

	public GameObject[] holidayLayers;

	public List<Image> completionImages;

	private bool startedLoading;

	public void BackFromCredits()
	{
		StartCoroutine(SwitchOffCredits());
	}

	public IEnumerator SwitchToCredits()
	{
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		creditsScreen.SetActive(value: true);
	}

	public IEnumerator SwitchOffCredits()
	{
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		creditsScreen.SetActive(value: false);
	}

	public void ToggleCustomSettings()
	{
		customSettingsActive = !customSettingsActive;
	}

	private void Awake()
	{
		instance = this;
		GlobalDark.CheckForDark();
		ConsoleScript.CheckForConsole();
		runSettings = new Dictionary<string, object>(RunSettings.GetPreset("normal").presetValues);
		characterSelected = 0;
		for (int i = 1; i < charImages.Count; i++)
		{
			if (PlayerPrefs.GetInt("wonchar" + (i - 1)) == 0)
			{
				charImages[i].GetComponent<Button>().interactable = false;
			}
		}
	}

	public void Quit()
	{
		if (!quitting)
		{
			StartCoroutine(QuitCoroutine());
		}
	}

	public IEnumerator QuitCoroutine()
	{
		quitting = true;
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		Application.Quit();
	}

	private void Start()
	{
		Settings.EnsureLoaded();
		if (didIntro)
		{
			contentWarning.SetActive(value: false);
		}
		menuSource.clip = menuSongs.PickRandom();
		menuSource.volume = Settings.Get<SettingFloat>("musicvolume").value * 0.6666f;
		menuSource.Play();
		Time.timeScale = 1f;
		loadButton.interactable = SaveSystem.HasSave();
		if (loadButton.interactable)
		{
			try
			{
				JObject jObject = JObject.Parse(SaveSystem.Unzip(File.ReadAllBytes(Application.persistentDataPath + "\\save.sv")));
				loadButtonImage.sprite = Resources.Load<Sprite>("BiomeIcon/" + ((int)jObject["biome"] + 1));
				loadButtonImage.enabled = true;
				TimeSpan timeSpan = TimeSpan.FromSeconds((float)jObject["runTime"]);
				saveTimeText.text = Locale.GetOther("savecontinuemenu") + " (" + timeSpan.ToString("hh\\:mm\\:ss") + ")";
				savedCharacter = (int)jObject["character"];
			}
			catch (Exception ex)
			{
				loadButtonImage.enabled = false;
				saveTimeText.text = "<color=\"red\">Save is corrupted!";
				ConsoleScript.instance.ExecuteCommand("log A save file exists at \"" + Application.persistentDataPath + "\\save.sv\", but it could not be parsed. The save is invalid or corrupted.\n" + ex.Message + "\n" + ex.StackTrace);
			}
		}
		else
		{
			saveTimeText.text = Locale.GetOther("savecontinuemenu");
			loadButtonImage.enabled = false;
		}
		runSettingObjects = new List<RunSettingDisplay>();
		float num = 0f;
		foreach (RunSetting settingType in RunSettings.settingTypes)
		{
			if (!string.IsNullOrEmpty(settingType.divider))
			{
				GameObject obj = Utils.Create("Special/RunSettingDivider", customSettingContent);
				RectTransform component = obj.GetComponent<RectTransform>();
				component.anchoredPosition = new Vector2(0f, 0f - (num + component.sizeDelta.y * 0.5f));
				num += component.sizeDelta.y - 3f;
				obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("rundiv" + settingType.divider);
			}
			GameObject gameObject = null;
			if (settingType is RunSettingBool)
			{
				gameObject = Utils.Create("Special/RunSettingBool", customSettingContent);
			}
			if (settingType is RunSettingFloat)
			{
				gameObject = Utils.Create("Special/RunSettingFloat", customSettingContent);
			}
			if (settingType is RunSettingDropdown)
			{
				gameObject = Utils.Create("Special/RunSettingDropdown", customSettingContent);
			}
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
			component2.anchoredPosition = new Vector2(0f, 0f - (num + component2.sizeDelta.y * 0.5f));
			RunSettingDisplay runSettingDisplay = gameObject.AddComponent<RunSettingDisplay>();
			runSettingDisplay.SetAssociated(settingType);
			runSettingDisplay.UpdateSettingDisplay(runSettings);
			runSettingObjects.Add(runSettingDisplay);
			num += component2.sizeDelta.y - 3f;
			customSettingContent.sizeDelta = new Vector2(customSettingContent.sizeDelta.x, num);
		}
		versionText.text = "v" + Application.version;
		for (int i = 0; i < charImages.Count; i++)
		{
			if (PlayerPrefs.GetInt("wonchar" + (i - 1)) == 0 && i != 0)
			{
				UITooltip uITooltip = charImages[i].gameObject.AddComponent<UITooltip>();
				uITooltip.skipLocale = true;
				uITooltip.tipName = Locale.GetOther("characterunlock");
				uITooltip.tipDesc = Locale.GetOther("characterunlockdsc");
			}
		}
		SeasonalManager.SeasonalEventType seasonalEvent = SeasonalManager.GetSeasonalEvent();
		if (seasonalEvent == SeasonalManager.SeasonalEventType.Halloween)
		{
			GameObject[] array = halloweenLayers;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].SetActive(value: true);
			}
		}
		if (seasonalEvent == SeasonalManager.SeasonalEventType.Holidays)
		{
			GameObject[] array = holidayLayers;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].SetActive(value: true);
			}
		}
		UpdateCharacterText();
		UpdateEndImages();
	}

	public void UpdateEndImages()
	{
		bool flag = !Settings.Get<SettingBool>("hidemenuprogression").value;
		completionImages[0].enabled = (PlayerPrefs.GetInt("didbiopod") == 1) & flag;
		completionImages[1].enabled = (PlayerPrefs.GetInt("didescape") == 1) & flag;
		completionImages[2].enabled = (PlayerPrefs.GetInt("didmerc") == 1) & flag;
	}

	public void ApplyPreset(string presetName)
	{
		runSettings.Clear();
		foreach (KeyValuePair<string, object> presetValue in RunSettings.GetPreset("normal").presetValues)
		{
			runSettings.Add(presetValue.Key, presetValue.Value);
		}
		RunSettingsPreset preset = RunSettings.GetPreset(presetName);
		currentPreset = RunSettings.presets.IndexOf(preset);
		foreach (KeyValuePair<string, object> presetValue2 in preset.presetValues)
		{
			runSettings[presetValue2.Key] = presetValue2.Value;
		}
		UpdateAllSettingDisplays();
	}

	public void UpdateAllSettingDisplays()
	{
		foreach (RunSettingDisplay runSettingObject in runSettingObjects)
		{
			runSettingObject.UpdateSettingDisplay(runSettings);
		}
	}

	private void Update()
	{
		radioBeep.color = Color.Lerp(radioBeep.color, (Time.unscaledTime % 1f > 0.8f) ? Color.white : Color.clear, Time.deltaTime * 6f);
		windSource.volume = (1f - cover.color.a) * 0.3f;
		if (!ScrollableText.textExists && !contentWarning.activeSelf)
		{
			cover.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(cover.color.a, 0f, Time.unscaledDeltaTime * 0.2f));
		}
		HandleRunScreen();
		HandleCharacterSelect();
	}

	private void HandleCharacterSelect()
	{
		for (int i = 0; i < charImages.Count; i++)
		{
			if (i != 0 && PlayerPrefs.GetInt("wonchar" + (i - 1)) <= 0)
			{
				charImages[i].color = Color.Lerp(charImages[i].color, new Color(0.3f, 0.3f, 0.3f), Time.unscaledDeltaTime * 5f);
				charInnerImages[i].color = Color.black;
			}
			else if (i == characterSelected)
			{
				charImages[i].color = Color.Lerp(charImages[i].color, new Color(1f, 1f, 1f), Time.unscaledDeltaTime * 5f);
				charInnerImages[i].color = Color.Lerp(charInnerImages[i].color, new Color(1f, 1f, 1f), Time.unscaledDeltaTime * 5f);
				float a = Mathf.PingPong(Time.unscaledTime * 0.5f, 0.5f);
				charPodiums[i].color = Color.Lerp(charPodiums[i].color, new Color(1f, 1f, 1f, a), Time.unscaledDeltaTime * 5f);
			}
			else
			{
				charImages[i].color = Color.Lerp(charImages[i].color, new Color(0.5f, 0.5f, 0.5f), Time.unscaledDeltaTime * 5f);
				charInnerImages[i].color = Color.Lerp(charInnerImages[i].color, new Color(0.3f, 0.3f, 0.3f), Time.unscaledDeltaTime * 5f);
				charPodiums[i].color = Color.Lerp(charPodiums[i].color, new Color(1f, 1f, 1f, 0f), Time.unscaledDeltaTime * 5f);
			}
		}
	}

	public void SelectCharacter(int chr)
	{
		characterSelected = chr;
		PlayerCamera.PlayUISound("click");
		UpdateCharacterText();
	}

	private void UpdateCharacterText()
	{
		charSelectName.text = Locale.GetOther($"char{characterSelected}name");
		charSelectNotes.text = Locale.GetOther($"char{characterSelected}desc");
		charSelectQuirks.text = Locale.GetOther($"char{characterSelected}quirks");
		int[] array = Skills.BaseSkills(characterSelected);
		string text = "[STR] " + array[0] + "\n";
		text = text + "[RES] " + array[1] + "\n";
		text = text + "[INT] " + array[2] + "\n";
		charSelectStats.text = text;
	}

	public void CopyToClipboard()
	{
		string input = JsonConvert.SerializeObject(SaveSystem.DicToTupleList(runSettings), Formatting.None, new JsonSerializerSettings
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			Formatting = Formatting.None
		});
		foreach (KeyValuePair<string, object> runSetting in runSettings)
		{
			MonoBehaviour.print(runSetting.Key + " " + runSetting.Value.ToString());
		}
		GUIUtility.systemCopyBuffer = CompressString(input);
	}

	public void PasteFromClipboard()
	{
		string systemCopyBuffer = GUIUtility.systemCopyBuffer;
		try
		{
			systemCopyBuffer = DecompressString(systemCopyBuffer);
			JArray jArray = JArray.Parse(systemCopyBuffer);
			foreach (var item in jArray.ToObject<List<(string, string)>>())
			{
				MonoBehaviour.print(item.Item1 + " " + item.Item2);
			}
			foreach (KeyValuePair<string, object> item2 in SaveSystem.TupleListToDic(jArray.ToObject<List<(string, string)>>()))
			{
				runSettings[item2.Key] = item2.Value;
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError(ex.ToString());
		}
		currentPreset = 6;
		UpdateAllSettingDisplays();
	}

	private void HandleRunScreen()
	{
		if (runSettingsScreen.activeSelf)
		{
			customSettingActiveAnim = Mathf.MoveTowards(customSettingActiveAnim, customSettingsActive ? 1f : 0f, Time.unscaledDeltaTime * 2f);
			float num = Utils.InOutQuad(customSettingActiveAnim);
			customSettingsMenu.anchoredPosition = new Vector2(Mathf.Lerp(customSettingMenuClosedX, customSettingMenuOpenX, num), customSettingsMenu.anchoredPosition.y);
			presetsMenu.offsetMax = new Vector2(Mathf.Lerp(presetMenuX, presetMenuXPushed, num), presetsMenu.offsetMax.y);
			sidebarArrow.eulerAngles = new Vector3(0f, 0f, num * 180f);
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			RectTransform rectTransform = presetRects[currentPreset];
			zero = rectTransform.position;
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			zero2.x = Mathf.Abs(array[2].x - array[0].x) + 35f + Mathf.PingPong(Time.unscaledTime * 24f, 24f);
			zero2.y = Mathf.Abs(array[2].y - array[0].y) + 35f + Mathf.PingPong(Time.unscaledTime * 24f, 24f);
			zero2 /= (Vector2)mainCanvas.transform.localScale;
			presetSelectImage.transform.position = Vector3.Lerp(presetSelectImage.transform.position, zero, Time.unscaledDeltaTime * 20f);
			presetSelectImage.rectTransform.sizeDelta = Vector3.Lerp(presetSelectImage.rectTransform.sizeDelta, zero2, Time.unscaledDeltaTime * 20f);
			presetSelectImage.color = new Color(1f, 0.765f, 0f);
		}
	}

	public void TryLore()
	{
		if (!didIntro && !Settings.Get<SettingBool>("skipintro").value)
		{
			didIntro = true;
			ScrollableText.CreateText(Locale.GetOther("startlorenote"), cover: true, introImages);
		}
	}

	public void OpenURL(string str)
	{
		Application.OpenURL(str);
	}

	public void OpenCustomMusicFolder()
	{
		OpenInFileBrowser(Application.dataPath + "/custommusic");
	}

	public void OpenLocaleFolder()
	{
		OpenInFileBrowser(Application.dataPath + "\\Lang");
	}

	public static void OpenInFileBrowser(string path)
	{
		Process.Start("explorer.exe", path.Replace("/", "\\"));
	}

	public IEnumerator WaitLoad()
	{
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		SceneManager.LoadScene("SampleScene");
	}

	public void StartRun()
	{
		if (!startedLoading)
		{
			if (PlayerPrefs.GetInt("didbasiccourse") == 0)
			{
				tutorialWarning.SetActive(value: true);
				runSettingsScreen.SetActive(value: false);
				return;
			}
			SaveSystem.loadedRun = false;
			WorldGeneration.runSettings = runSettings;
			startedLoading = true;
			StartCoroutine(WaitLoad());
		}
	}

	public void MarkTutorialCompleted()
	{
		PlayerPrefs.SetInt("didbasiccourse", 1);
		tutorialWarning.SetActive(value: false);
	}

	public void LoadRun()
	{
		if (!startedLoading)
		{
			if (PlayerPrefs.GetInt("didbasiccourse") == 0)
			{
				tutorialWarning.SetActive(value: true);
				runSettingsScreen.SetActive(value: false);
				return;
			}
			characterSelected = savedCharacter;
			SaveSystem.loadedRun = true;
			startedLoading = true;
			StartCoroutine(WaitLoad());
		}
	}

	public void StartTutorial()
	{
		if (!startedLoading)
		{
			SaveSystem.loadedRun = false;
			PlayerPrefs.SetInt("tutorial", 1);
			WorldGeneration.runSettings = null;
			startedLoading = true;
			StartCoroutine(WaitLoad());
		}
	}

	public static string CompressString(string input)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(input);
		using MemoryStream memoryStream = new MemoryStream();
		using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
		{
			gZipStream.Write(bytes, 0, bytes.Length);
		}
		return Convert.ToBase64String(memoryStream.ToArray());
	}

	public static string DecompressString(string compressedBase64)
	{
		using MemoryStream stream = new MemoryStream(Convert.FromBase64String(compressedBase64));
		using GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress);
		using MemoryStream memoryStream = new MemoryStream();
		gZipStream.CopyTo(memoryStream);
		return Encoding.UTF8.GetString(memoryStream.ToArray());
	}
}
