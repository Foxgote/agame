using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000102 RID: 258
	[HarmonyPatch(typeof(PantSound), "Update")]
	internal static class VoicePantVolumePatch
	{
		// Token: 0x060005A7 RID: 1447 RVA: 0x0005B05C File Offset: 0x0005925C
		private unsafe static void Postfix(PantSound __instance)
		{
			AudioSource audioSource = *VoicePantVolumePatch.Source.Invoke(__instance);
			if (audioSource)
			{
				audioSource.volume *= SkinVoice.VoiceVolume.Value;
			}
		}

		// Token: 0x04000653 RID: 1619
		private static readonly AccessTools.FieldRef<PantSound, AudioSource> Source = AccessTools.FieldRefAccess<PantSound, AudioSource>("source");
	}
}
