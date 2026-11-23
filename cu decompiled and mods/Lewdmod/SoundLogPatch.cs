using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000058 RID: 88
	[HarmonyPatch(typeof(Sound), "Play", new Type[]
	{
		typeof(AudioClip),
		typeof(Vector2),
		typeof(bool),
		typeof(bool),
		typeof(Transform),
		typeof(float),
		typeof(float),
		typeof(bool),
		typeof(bool)
	})]
	internal static class SoundLogPatch
	{
		// Token: 0x060001AB RID: 427 RVA: 0x00025634 File Offset: 0x00023834
		private static void Postfix(AudioClip clip, float volume, Transform follow)
		{
			if (Time.time > SoundLog.Until || clip == null)
			{
				return;
			}
			Plugin.Log.LogInfo(string.Format("[Sound] {0} vol {1:0.00} on {2}", clip.name, volume, follow ? follow.name : "-"));
		}
	}
}
