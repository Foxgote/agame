using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LewdStatus
{
	// Token: 0x020000B5 RID: 181
	[HarmonyPatch(typeof(SettingsMenu), "Start")]
	internal static class LewdSettingsTabPatch
	{
		// Token: 0x06000414 RID: 1044 RVA: 0x00042538 File Offset: 0x00040738
		private static void Prefix(SettingsMenu __instance)
		{
			try
			{
				List<Button> buttons = __instance.buttons;
				if (buttons != null && buttons.Count != 0 && buttons.Count <= 5)
				{
					Button button = buttons[buttons.Count - 1];
					RectTransform rectTransform = (RectTransform)button.transform;
					Button button2 = Object.Instantiate<Button>(button, button.transform.parent);
					button2.name = "LewdTab";
					button2.onClick = new Button.ButtonClickedEvent();
					button2.onClick.AddListener(delegate
					{
						__instance.SelectTab(5);
					});
					UILocalizer[] componentsInChildren = button2.GetComponentsInChildren<UILocalizer>(true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						Object.Destroy(componentsInChildren[i]);
					}
					TextMeshProUGUI componentInChildren = button2.GetComponentInChildren<TextMeshProUGUI>(true);
					if (componentInChildren)
					{
						componentInChildren.text = Plugin.T("gamesetlewd_tab");
					}
					buttons.Add(button2);
					if (!button2.transform.parent.GetComponent<HorizontalLayoutGroup>())
					{
						List<RectTransform> list = buttons.Select<Button, RectTransform>((Button b) => (RectTransform)b.transform).ToList<RectTransform>();
						float num = list[0].anchoredPosition.x - list[0].sizeDelta.x * 0.5f;
						float num2 = rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x * 0.5f;
						float num3 = ((list.Count > 2) ? (list[1].anchoredPosition.x - list[0].anchoredPosition.x - list[0].sizeDelta.x) : 0f);
						float num4 = (num2 - num - num3 * (float)(list.Count - 1)) / (float)list.Count;
						for (int j = 0; j < list.Count; j++)
						{
							list[j].sizeDelta = new Vector2(num4, list[j].sizeDelta.y);
							list[j].anchoredPosition = new Vector2(num + num4 * 0.5f + (float)j * (num4 + num3), rectTransform.anchoredPosition.y);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Could not add the Lewd settings tab: " + ex.Message);
			}
		}
	}
}
