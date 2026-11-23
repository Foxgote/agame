using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000C0 RID: 192
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
	internal static class NoiseSoundPatch
	{
		// Token: 0x06000446 RID: 1094 RVA: 0x00045238 File Offset: 0x00043438
		private static void Postfix(AudioClip clip, Vector2 pos, float volume)
		{
			try
			{
				Noise.OnSound(clip, pos, volume);
			}
			catch
			{
			}
		}
	}
}
