using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	public static SettingsMenu instance;

	private List<GameObject> spawnedSettings;

	public List<Button> buttons;

	public Sprite buttonClosed;

	public Sprite buttonOpen;

	public RectTransform content;

	public static void OpenMenu(Transform parent)
	{
		if (!instance)
		{
			instance = Utils.Create("Special/SettingsMenu", parent).GetComponent<SettingsMenu>();
		}
	}

	public void SelectTab(int index)
	{
		SelectTab((Setting.SettingCategory)index);
	}

	public void ResetToDefault()
	{
		Settings.settings = Settings.DefaultSettings();
		Settings.ApplyAll();
		Settings.SaveSettings();
		SelectTab(0);
	}

	public void SelectTab(Setting.SettingCategory category)
	{
		foreach (GameObject spawnedSetting in spawnedSettings)
		{
			UnityEngine.Object.Destroy(spawnedSetting);
		}
		spawnedSettings.Clear();
		float num = 0f;
		foreach (Setting setting in Settings.GetAllSettings())
		{
			if (setting.category != category)
			{
				continue;
			}
			GameObject g = null;
			bool flag = true;
			SettingFloat setfloat = setting as SettingFloat;
			if (setfloat != null)
			{
				g = Utils.Create("Special/GameSettingFloat", content);
				Slider component = g.transform.GetChild(1).GetComponent<Slider>();
				component.minValue = setfloat.min;
				component.maxValue = setfloat.max;
				component.SetValueWithoutNotify(setfloat.value);
				g.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = setfloat.GetDisplayString();
				component.onValueChanged.AddListener((float x) =>
				{
					setting.SetValue(x);
					setting.Apply();
					g.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = setfloat.GetDisplayString();
				});
			}
			else
			{
				SettingInt setint = setting as SettingInt;
				if (setint != null)
				{
					g = Utils.Create("Special/GameSettingInt", content);
					g.transform.GetChild(1).GetComponent<TMP_InputField>().SetTextWithoutNotify(setint.value.ToString());
					g.transform.GetChild(1).GetComponent<TMP_InputField>().onEndEdit.AddListener((string x) =>
					{
						if (!int.TryParse(x, out var result))
						{
							result = setint.value;
						}
						if (result > setint.max)
						{
							result = setint.max;
						}
						if (result < setint.min)
						{
							result = setint.min;
						}
						g.transform.GetChild(1).GetComponent<TMP_InputField>().SetTextWithoutNotify(result.ToString());
						setting.SetValue(result);
						setting.Apply();
					});
				}
				else if (setting is SettingDropdown settingDropdown)
				{
					g = Utils.Create("Special/GameSettingDropdown", content);
					List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
					string[] choices = settingDropdown.choices;
					foreach (string text in choices)
					{
						list.Add(new TMP_Dropdown.OptionData(Locale.GetOther("gameset" + setting.name + text)));
					}
					g.transform.GetChild(1).GetComponent<TMP_Dropdown>().AddOptions(list);
					g.transform.GetChild(1).GetComponent<TMP_Dropdown>().SetValueWithoutNotify(settingDropdown.value);
					g.transform.GetChild(1).GetComponent<TMP_Dropdown>().onValueChanged.AddListener((int x) =>
					{
						setting.SetValue(x);
						setting.Apply();
					});
				}
				else if (setting is SettingBool)
				{
					g = Utils.Create("Special/GameSettingBool", content);
					g.transform.GetChild(1).GetComponent<Toggle>().SetIsOnWithoutNotify((bool)setting.GetValue());
					g.transform.GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener((bool x) =>
					{
						setting.SetValue(x);
						setting.Apply();
					});
				}
				else
				{
					SettingKeybind keybind = setting as SettingKeybind;
					if (keybind != null)
					{
						g = Utils.Create("Special/GameSettingInput", content);
						Button component2 = g.transform.GetChild(1).GetComponent<Button>();
						TextMeshProUGUI txt = component2.GetComponentInChildren<TextMeshProUGUI>();
						txt.text = keybind.GetDisplayString();
						component2.onClick.AddListener(() =>
						{
							StartCoroutine(WaitForKeyPress(keybind, txt));
						});
						flag = false;
					}
				}
			}
			g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("gameset" + setting.name);
			UITooltip uITooltip = g.transform.GetChild(0).gameObject.AddComponent<UITooltip>();
			uITooltip.skipLocale = true;
			uITooltip.tipName = Locale.GetOther("gameset" + setting.name);
			if (flag)
			{
				uITooltip.tipDesc = Locale.GetOther("gameset" + setting.name + "dsc");
			}
			g.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f - num - g.GetComponent<RectTransform>().sizeDelta.y * 0.5f);
			num += g.GetComponent<RectTransform>().sizeDelta.y;
			spawnedSettings.Add(g);
		}
		if (category == Setting.SettingCategory.Language)
		{
			foreach (MiniLang lang in Locale.GetAllLanguages())
			{
				GameObject gameObject = Utils.Create("Special/GameSettingLanguage", content);
				UITooltip uITooltip2 = gameObject.AddComponent<UITooltip>();
				uITooltip2.skipLocale = true;
				uITooltip2.tipName = lang.name;
				uITooltip2.tipDesc = lang.description;
				gameObject.GetComponent<Button>().onClick.AddListener(() =>
				{
					Locale.ChangeLanguage(lang.reference);
				});
				gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = lang.name;
				gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f - num - gameObject.GetComponent<RectTransform>().sizeDelta.y * 0.5f);
				num += gameObject.GetComponent<RectTransform>().sizeDelta.y;
				if ((bool)WorldGeneration.world)
				{
					gameObject.GetComponent<Button>().interactable = false;
				}
				spawnedSettings.Add(gameObject);
			}
		}
		content.sizeDelta = new Vector2(content.sizeDelta.x, num);
		for (int num3 = 0; num3 < buttons.Count; num3++)
		{
			buttons[num3].GetComponent<Image>().sprite = ((num3 == (int)category) ? buttonOpen : buttonClosed);
		}
		PlayerCamera.PlayUISound("miniClick");
	}

	private IEnumerator WaitForKeyPress(SettingKeybind keybind, TextMeshProUGUI txt)
	{
		txt.text = "...";
		bool keySet = false;
		while (!keySet)
		{
			foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
			{
				if (Input.GetKeyDown(value))
				{
					keybind.SetValue(value);
					keybind.Apply();
					txt.text = keybind.GetDisplayString();
					keySet = true;
					break;
				}
			}
			yield return null;
		}
	}

	private void Start()
	{
		spawnedSettings = new List<GameObject>();
		SelectTab(Setting.SettingCategory.Video);
	}

	public void Close()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		Settings.SaveSettings();
	}
}
