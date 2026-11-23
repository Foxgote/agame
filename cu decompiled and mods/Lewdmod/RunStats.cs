using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LewdStatus
{
	// Token: 0x02000085 RID: 133
	internal static class RunStats
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600026A RID: 618 RVA: 0x0002CBCE File Offset: 0x0002ADCE
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0002CBD8 File Offset: 0x0002ADD8
		public static void Track(Body b)
		{
			bool held = Scenes.Held;
			if (held && !RunStats.wasHeld)
			{
				RunStats.S.trapsCaught++;
			}
			if (!held && RunStats.wasHeld && b.alive)
			{
				RunStats.S.trapsSurvived++;
			}
			RunStats.wasHeld = held;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0002CC2F File Offset: 0x0002AE2F
		public static void OnLine(string key)
		{
			if (RunStats.S != null && !RunStats.Muted && RunStats.EscapeLines.Contains(key))
			{
				RunStats.S.escapes++;
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0002CC5E File Offset: 0x0002AE5E
		public static void Escaped()
		{
			if (RunStats.S != null)
			{
				RunStats.S.escapes++;
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0002CC7C File Offset: 0x0002AE7C
		public static void Show(Image endScreen)
		{
			RunStats.Hide();
			PlayerCamera main = PlayerCamera.main;
			if (!LewdSettings.On || RunStats.S == null || !main || !main.deathStats || main.deathStats.childCount < 14)
			{
				return;
			}
			RectTransform deathStats = main.deathStats;
			List<RectTransform> list = new List<RectTransform>();
			foreach (Button button in deathStats.GetComponentsInChildren<Button>(true))
			{
				if (button.transform != deathStats)
				{
					list.Add((RectTransform)button.transform);
				}
			}
			if (list.Count < 2)
			{
				Plugin.Log.LogWarning(string.Format("Death receipt: {0} buttons, no room for page 2", list.Count));
				return;
			}
			list.Sort((RectTransform x, RectTransform y) => y.anchoredPosition.y.CompareTo(x.anchoredPosition.y));
			RectTransform rectTransform = list[0];
			RectTransform rectTransform2 = list[1];
			GameObject gameObject = Object.Instantiate<GameObject>(rectTransform.gameObject, rectTransform.parent);
			gameObject.name = "LewdPage2";
			UILocalizer[] componentsInChildren2 = gameObject.GetComponentsInChildren<UILocalizer>(true);
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				Object.Destroy(componentsInChildren2[i]);
			}
			((RectTransform)gameObject.transform).anchoredPosition = rectTransform.anchoredPosition + (rectTransform.anchoredPosition - rectTransform2.anchoredPosition);
			Button component = gameObject.GetComponent<Button>();
			component.onClick = new Button.ButtonClickedEvent();
			UnityEvent onClick = component.onClick;
			UnityAction unityAction;
			if ((unityAction = RunStats.<>O.<0>__Flip) == null)
			{
				unityAction = (RunStats.<>O.<0>__Flip = new UnityAction(RunStats.Flip));
			}
			onClick.AddListener(unityAction);
			RunStats.label = gameObject.GetComponentInChildren<TextMeshProUGUI>(true);
			if (RunStats.label)
			{
				RunStats.label.text = Plugin.T("stats_page2");
				RunStats.label.color = new Color32(150, 40, 90, byte.MaxValue);
			}
			RunStats.shown = gameObject;
			RunStats.page1 = null;
			RunStats.onPage2 = false;
			Plugin.Log.LogInfo(string.Format("Run stats: {0} orgasms, {1} traps, {2} survived, {3} escapes (page 2 of the receipt)", new object[]
			{
				RunStats.S.orgasms,
				RunStats.S.trapsCaught,
				RunStats.S.trapsSurvived,
				RunStats.S.escapes
			}));
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0002CEE6 File Offset: 0x0002B0E6
		private static TextMeshProUGUI Line(int i)
		{
			return PlayerCamera.main.deathStats.GetChild(i).GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0002CF00 File Offset: 0x0002B100
		private static void Flip()
		{
			Plugin.Log.LogInfo("Receipt: page flip");
			PlayerCamera main = PlayerCamera.main;
			if (!main || !main.deathStats)
			{
				return;
			}
			if (RunStats.page1 == null)
			{
				RunStats.page1 = new string[14];
				for (int i = 0; i < 14; i++)
				{
					RunStats.page1[i] = (RunStats.Line(i) ? RunStats.Line(i).text : "");
				}
			}
			RunStats.onPage2 = !RunStats.onPage2;
			string[] array = (RunStats.onPage2 ? RunStats.Page2() : RunStats.page1);
			for (int j = 0; j < 14; j++)
			{
				if (RunStats.Line(j))
				{
					RunStats.Line(j).text = array[j];
				}
			}
			if (RunStats.label)
			{
				RunStats.label.text = Plugin.T(RunStats.onPage2 ? "stats_page1" : "stats_page2");
			}
			Sound.Play("receiptPrint", Vector2.zero, true, false, null, 0.3f, 1.4f, true, true);
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0002D014 File Offset: 0x0002B214
		private static string[] Page2()
		{
			return new string[]
			{
				Plugin.T("stats_title"),
				RunStats.page1[1],
				GameOverPic.Fate ?? RunStats.page1[2],
				(GameOverPic.Fate != null) ? Plugin.T("fate_subject").Replace("<n>", GameOverPic.Subject.ToString()) : RunStats.page1[3],
				Plugin.T("stats_orgasms"),
				string.Format("{0} ({1} {2})", RunStats.S.orgasms, Plugin.T("stats_self"), RunStats.S.selfOrgasms),
				Plugin.T("stats_traps"),
				RunStats.S.trapsCaught.ToString(),
				Plugin.T("stats_survived"),
				RunStats.S.trapsSurvived.ToString(),
				Plugin.T("stats_escapes"),
				RunStats.S.escapes.ToString(),
				Plugin.T("stats_corruption"),
				Mathf.RoundToInt(RunStats.S.corruptionPeak).ToString() + "%"
			};
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0002D15B File Offset: 0x0002B35B
		public static void Hide()
		{
			if (RunStats.shown)
			{
				Object.Destroy(RunStats.shown);
			}
			RunStats.shown = null;
			RunStats.page1 = null;
			RunStats.onPage2 = false;
		}

		// Token: 0x04000253 RID: 595
		private static readonly HashSet<string> EscapeLines = new HashSet<string> { "flesh_escape", "fwall_escape", "pit_escape", "tent_escape", "worm_escape", "web_free", "cocoon_free", "slime_escape", "flower_escape" };

		// Token: 0x04000254 RID: 596
		private static bool wasHeld;

		// Token: 0x04000255 RID: 597
		private static GameObject shown;

		// Token: 0x04000256 RID: 598
		public static bool Muted;

		// Token: 0x04000257 RID: 599
		private const int Lines = 14;

		// Token: 0x04000258 RID: 600
		private static string[] page1;

		// Token: 0x04000259 RID: 601
		private static bool onPage2;

		// Token: 0x0400025A RID: 602
		private static TextMeshProUGUI label;

		// Token: 0x020001A5 RID: 421
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000959 RID: 2393
			public static UnityAction <0>__Flip;
		}
	}
}
