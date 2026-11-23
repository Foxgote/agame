using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000FF RID: 255
	internal static class Sfx
	{
		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x00059C01 File Offset: 0x00057E01
		public static int Count
		{
			get
			{
				return Sfx.Sets.Sum<KeyValuePair<string, List<AudioClip>>>((KeyValuePair<string, List<AudioClip>> s) => s.Value.Count);
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000587 RID: 1415 RVA: 0x00059C2C File Offset: 0x00057E2C
		public static string Summary
		{
			get
			{
				return string.Join(", ", Sfx.Sets.Select<KeyValuePair<string, List<AudioClip>>, string>((KeyValuePair<string, List<AudioClip>> s) => string.Format("{0} {1}", s.Key, s.Value.Count)).ToArray<string>());
			}
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x00059C68 File Offset: 0x00057E68
		public static void Load(string root)
		{
			if (!Directory.Exists(root))
			{
				return;
			}
			foreach (string text in Directory.GetFiles(root, "*.wav"))
			{
				Sfx.Add("moan", text);
			}
			foreach (string text2 in Directory.GetDirectories(root))
			{
				foreach (string text3 in Directory.GetFiles(text2, "*.wav"))
				{
					Sfx.Add(Path.GetFileName(text2).ToLowerInvariant(), text3);
				}
			}
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x00059CFC File Offset: 0x00057EFC
		private static void Add(string set, string file)
		{
			AudioClip audioClip = Wav.Load(file, set + "/" + Path.GetFileNameWithoutExtension(file));
			if (!audioClip)
			{
				Plugin.Log.LogWarning("Not a PCM wav, skipped: " + file);
				return;
			}
			List<AudioClip> list;
			if (!Sfx.Sets.TryGetValue(set, out list))
			{
				list = (Sfx.Sets[set] = new List<AudioClip>());
			}
			list.Add(audioClip);
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x00059D68 File Offset: 0x00057F68
		public static bool Has(string set)
		{
			List<AudioClip> list;
			return Sfx.Sets.TryGetValue(set, out list) && list.Count > 0;
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x00059D90 File Offset: 0x00057F90
		public static AudioClip Pick(string set)
		{
			List<AudioClip> list;
			if (!Sfx.Sets.TryGetValue(set, out list) || list.Count == 0)
			{
				return null;
			}
			AudioClip audioClip;
			Sfx.Last.TryGetValue(set, out audioClip);
			AudioClip audioClip2 = list[Random.Range(0, list.Count)];
			if (audioClip2 == audioClip && list.Count > 1)
			{
				audioClip2 = list[(list.IndexOf(audioClip2) + 1 + Random.Range(0, list.Count - 1)) % list.Count];
			}
			Sfx.Last[set] = audioClip2;
			return audioClip2;
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x00059E1C File Offset: 0x0005801C
		public static void Play(Body b, float volume, params string[] sets)
		{
			foreach (string text in sets)
			{
				AudioClip audioClip = Sfx.Pick(text);
				if (audioClip)
				{
					if (Sfx.VoiceSets.Contains(text) && Plugin.MoanVolume != null)
					{
						volume *= Plugin.MoanVolume.Value;
					}
					Sound.Play(audioClip, b.transform.position, false, true, b.transform, volume, 1f, false, false);
					return;
				}
			}
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x00059E98 File Offset: 0x00058098
		public static void PlayMix(Body b, float volume, params string[] sets)
		{
			Func<string, bool> func;
			if ((func = Sfx.<>O.<0>__Has) == null)
			{
				func = (Sfx.<>O.<0>__Has = new Func<string, bool>(Sfx.Has));
			}
			string[] array = sets.Where<string>(func).ToArray<string>();
			if (array.Length != 0)
			{
				Sfx.Play(b, volume, new string[] { array[Random.Range(0, array.Length)] });
			}
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x00059EEC File Offset: 0x000580EC
		public static AudioSource Loop(Body b, string set)
		{
			AudioClip audioClip = Sfx.Pick(set);
			if (!audioClip)
			{
				return null;
			}
			AudioSource audioSource = b.gameObject.AddComponent<AudioSource>();
			audioSource.clip = audioClip;
			audioSource.loop = true;
			audioSource.spatialBlend = 1f;
			audioSource.dopplerLevel = 0f;
			audioSource.volume = 0f;
			if (WorldGeneration.world)
			{
				audioSource.outputAudioMixerGroup = WorldGeneration.world.soundMixerGroup;
			}
			audioSource.time = Random.Range(0f, audioClip.length * 0.9f);
			audioSource.Play();
			return audioSource;
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x00059F84 File Offset: 0x00058184
		public static IEnumerator FadeOut(AudioSource src, float seconds)
		{
			if (!src)
			{
				yield break;
			}
			float start = src.volume;
			float t = 0f;
			while (t < seconds && src)
			{
				src.volume = Mathf.Lerp(start, 0f, t / seconds);
				yield return null;
				t += Time.deltaTime;
			}
			if (src)
			{
				Object.Destroy(src);
			}
			yield break;
		}

		// Token: 0x0400063F RID: 1599
		private static readonly Dictionary<string, List<AudioClip>> Sets = new Dictionary<string, List<AudioClip>>();

		// Token: 0x04000640 RID: 1600
		private static readonly Dictionary<string, AudioClip> Last = new Dictionary<string, AudioClip>();

		// Token: 0x04000641 RID: 1601
		private static readonly HashSet<string> VoiceSets = new HashSet<string> { "moan_soft", "moan", "climax", "breath", "sigh", "muffled" };

		// Token: 0x02000217 RID: 535
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000A9C RID: 2716
			public static Func<string, bool> <0>__Has;
		}
	}
}
