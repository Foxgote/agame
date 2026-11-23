using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000101 RID: 257
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
	internal static class VoicePlayPatch
	{
		// Token: 0x060005A6 RID: 1446 RVA: 0x0005B038 File Offset: 0x00059238
		private static void Prefix(ref AudioClip clip, ref float volume)
		{
			if (!SkinVoice.IsHers(clip))
			{
				return;
			}
			clip = SkinVoice.Replacement(clip);
			volume *= SkinVoice.VoiceVolume.Value;
		}
	}
}
