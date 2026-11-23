using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using TMPro;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000BB RID: 187
	internal static class MoodleFilter
	{
		// Token: 0x06000435 RID: 1077 RVA: 0x00044468 File Offset: 0x00042668
		public static void Snapshot(MoodleManager m)
		{
			MoodleFilter.stale.Clear();
			if (m && m.moodles)
			{
				foreach (object obj in m.moodles)
				{
					Transform transform = (Transform)obj;
					MoodleFilter.stale.Add(transform);
				}
			}
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x000444E8 File Offset: 0x000426E8
		public static void Reload()
		{
			MoodleFilter.hidden = null;
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x000444F0 File Offset: 0x000426F0
		public static bool Hidden(string icon)
		{
			if (MoodleFilter.hidden == null)
			{
				ConfigEntry<string> moodlesHidden = Plugin.MoodlesHidden;
				MoodleFilter.hidden = new HashSet<string>(from x in (((moodlesHidden != null) ? moodlesHidden.Value : null) ?? "").Split(new char[] { ',' })
					select x.Trim().ToLowerInvariant() into x
					where x.Length > 0
					select x);
			}
			if (MoodleFilter.hidden.Count == 0 || icon == null)
			{
				return false;
			}
			string text = icon.ToLowerInvariant();
			if (MoodleFilter.hidden.Contains(text))
			{
				return true;
			}
			if (text.StartsWith("lewd_"))
			{
				string text2 = text.Substring(5);
				if (MoodleFilter.hidden.Contains(text2))
				{
					return true;
				}
				if (text2.StartsWith("suit_") && text2 != "suit_latex" && MoodleFilter.hidden.Contains("suit"))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x000445FC File Offset: 0x000427FC
		public static void Relayout(MoodleManager m)
		{
			if (!m || !m.moodles)
			{
				return;
			}
			List<Moodle> list = new List<Moodle>();
			GameObject gameObject = null;
			foreach (object obj in m.moodles)
			{
				Transform transform = (Transform)obj;
				if (transform && transform.gameObject.activeSelf && !MoodleFilter.stale.Contains(transform))
				{
					Moodle component = transform.GetComponent<Moodle>();
					if (component)
					{
						list.Add(component);
					}
					else if (!gameObject)
					{
						gameObject = transform.gameObject;
					}
					else
					{
						transform.gameObject.SetActive(false);
						Object.Destroy(transform.gameObject);
					}
				}
			}
			List<Moodle> list2 = list.Where<Moodle>((Moodle x) => !x.isSide).Concat<Moodle>(list.Where<Moodle>((Moodle x) => x.isSide)).ToList<Moodle>();
			for (int i = 0; i < list2.Count; i++)
			{
				RectTransform component2 = list2[i].GetComponent<RectTransform>();
				component2.anchoredPosition = new Vector2((float)(i * 70), component2.anchoredPosition.y);
			}
			int num = list.Count<Moodle>((Moodle x) => !x.isSide);
			int num2 = list2.Count - num;
			if (num2 > 0)
			{
				if (!gameObject && m.bonusMoodlePrefab)
				{
					gameObject = Object.Instantiate<GameObject>(m.bonusMoodlePrefab, m.moodles);
				}
				if (gameObject)
				{
					gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)(num * 70 - 10), 0f);
					TextMeshProUGUI componentInChildren = gameObject.GetComponentInChildren<TextMeshProUGUI>(true);
					if (componentInChildren)
					{
						componentInChildren.text = "+" + num2.ToString();
						return;
					}
				}
			}
			else if (gameObject)
			{
				Object.Destroy(gameObject);
			}
		}

		// Token: 0x040003D5 RID: 981
		private static HashSet<string> hidden;

		// Token: 0x040003D6 RID: 982
		private static readonly HashSet<Transform> stale = new HashSet<Transform>();
	}
}
