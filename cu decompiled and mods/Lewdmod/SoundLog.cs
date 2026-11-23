using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000057 RID: 87
	internal static class SoundLog
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x000254C8 File Offset: 0x000236C8
		public static void Watch(Body b)
		{
			if (!Plugin.DebugKeys.Value || !b || Time.unscaledTime < SoundLog.next)
			{
				return;
			}
			SoundLog.next = Time.unscaledTime + 1f;
			List<string> list = (from s in Object.FindObjectsOfType<AudioSource>().Where<AudioSource>(delegate(AudioSource s)
				{
					if (!s || !s.isPlaying || !s.clip || s.volume <= 0.02f)
					{
						return false;
					}
					if (s.spatialBlend >= 0.5f)
					{
						return Vector2.Distance(s.transform.position, b.transform.position) < 10f;
					}
					return s.transform.IsChildOf(b.transform);
				})
				select string.Format("{0}{1} {2:0.00} on {3}", new object[]
				{
					s.clip.name,
					s.loop ? "(loop)" : "",
					s.volume,
					s.gameObject.name
				}) into n
				where !n.StartsWith("music") && !n.Contains("on MusicManager")
				orderby n
				select n).ToList<string>();
			string text = string.Join(", ", list);
			if (text == SoundLog.last)
			{
				return;
			}
			SoundLog.last = text;
			Plugin.Log.LogInfo(string.Format("[Heard] temp {0:0.0} stamina {1:0} wet {2:0}: {3}", new object[]
			{
				b.temperature,
				b.stamina,
				b.wetness,
				(list.Count == 0) ? "nothing" : text
			}));
		}

		// Token: 0x040001B6 RID: 438
		public static float Until = -1f;

		// Token: 0x040001B7 RID: 439
		private static float next;

		// Token: 0x040001B8 RID: 440
		private static string last;
	}
}
