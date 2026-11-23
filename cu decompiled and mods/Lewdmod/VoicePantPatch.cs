using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000103 RID: 259
	[HarmonyPatch(typeof(PantSound), "Start")]
	internal static class VoicePantPatch
	{
		// Token: 0x060005A9 RID: 1449 RVA: 0x0005B0A8 File Offset: 0x000592A8
		private static void Postfix(PantSound __instance)
		{
			AudioSource audioSource = __instance.GetComponents<AudioSource>().FirstOrDefault<AudioSource>((AudioSource s) => s.clip && s.clip.name == "pant");
			AudioClip audioClip = (audioSource ? SkinVoice.Replacement(audioSource.clip) : null);
			if (audioSource && audioClip != audioSource.clip)
			{
				audioSource.clip = audioClip;
				audioSource.Play();
			}
			Plugin.Log.LogInfo("Player voice ready");
		}
	}
}
