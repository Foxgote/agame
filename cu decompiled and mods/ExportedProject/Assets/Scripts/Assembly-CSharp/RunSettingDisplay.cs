using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunSettingDisplay : MonoBehaviour
{
	public RunSetting associated;

	public Type settingType;

	private bool didInitialize;

	public bool isFloat => settingType.Equals(typeof(RunSettingFloat));

	public bool isBool => settingType.Equals(typeof(RunSettingBool));

	public bool isDropdown => settingType.Equals(typeof(RunSettingDropdown));

	public void SetAssociated(RunSetting ass)
	{
		associated = ass;
		settingType = ass.GetType();
		base.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Locale.GetOther("runset" + ass.name);
		UITooltip uITooltip = base.transform.GetChild(0).gameObject.AddComponent<UITooltip>();
		uITooltip.skipLocale = true;
		uITooltip.tipName = Locale.GetOther("runset" + ass.name);
		uITooltip.tipDesc = Locale.GetOther("runset" + ass.name + "dsc");
	}

	public void UpdateSetting(Dictionary<string, object> settings)
	{
		if (isFloat)
		{
			settings[associated.name] = base.transform.GetChild(1).GetComponent<Slider>().value;
		}
		else if (isBool)
		{
			settings[associated.name] = base.transform.GetChild(1).GetComponent<Toggle>().isOn;
		}
		else if (isDropdown)
		{
			settings[associated.name] = base.transform.GetChild(1).GetComponent<TMP_Dropdown>().value;
		}
		PreRunScript.instance.currentPreset = 6;
		UpdateSettingDisplay(settings);
	}

	public void UpdateSettingDisplay(Dictionary<string, object> settings)
	{
		if (!didInitialize)
		{
			didInitialize = true;
			if (isFloat && associated is RunSettingFloat runSettingFloat)
			{
				base.transform.GetChild(1).GetComponent<Slider>().minValue = runSettingFloat.limits.min;
				base.transform.GetChild(1).GetComponent<Slider>().maxValue = runSettingFloat.limits.max;
				base.transform.GetChild(1).GetComponent<Slider>().wholeNumbers = runSettingFloat.wholeNum;
				base.transform.GetChild(1).GetComponent<Slider>().onValueChanged.AddListener(delegate
				{
					UpdateSetting(settings);
				});
			}
			if (isBool)
			{
				base.transform.GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener(delegate
				{
					UpdateSetting(settings);
				});
			}
			else if (isDropdown && associated is RunSettingDropdown runSettingDropdown)
			{
				List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
				string[] choices = runSettingDropdown.choices;
				foreach (string text in choices)
				{
					list.Add(new TMP_Dropdown.OptionData(Locale.GetOther("runset" + associated.name + text)));
				}
				base.transform.GetChild(1).GetComponent<TMP_Dropdown>().AddOptions(list);
				base.transform.GetChild(1).GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate
				{
					UpdateSetting(settings);
				});
			}
		}
		if (isFloat && associated is RunSettingFloat runSettingFloat2)
		{
			base.transform.GetChild(1).GetComponent<Slider>().SetValueWithoutNotify((float)settings[associated.name]);
			float num2 = (float)settings[associated.name];
			string text2 = "";
			text2 = ((!(num2 >= 10f) && !(num2 < 0f)) ? num2.ToString("0.##") : num2.ToString("0.#"));
			base.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = text2 + runSettingFloat2.postfix;
		}
		else if (isBool)
		{
			base.transform.GetChild(1).GetComponent<Toggle>().SetIsOnWithoutNotify((bool)settings[associated.name]);
		}
		else if (isDropdown)
		{
			base.transform.GetChild(1).GetComponent<TMP_Dropdown>().SetValueWithoutNotify((int)settings[associated.name]);
		}
	}
}
