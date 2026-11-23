using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000114 RID: 276
	[HarmonyPatch(typeof(Body), "GetTotalInsulation")]
	internal static class SlimeSuitWarmthPatch
	{
		// Token: 0x0600066A RID: 1642 RVA: 0x0006642C File Offset: 0x0006462C
		private static void Postfix(Body __instance, ref float __result)
		{
			if (Suit.Stage <= 0 || Suit.Drone || !PlayerCamera.main || PlayerCamera.main.body != __instance || !WorldGeneration.world)
			{
				return;
			}
			if (WorldGeneration.world.ambientTemperature >= __instance.temperature)
			{
				return;
			}
			float num = (Suit.Slime ? Plugin.SlimeWarmth.Value : (Suit.Latex ? Plugin.LatexWarmth.Value : Plugin.ParasiteWarmth.Value));
			__result += num * (float)(Suit.Latex ? Suit.Stage : Mathf.Min(Suit.Stage, 4));
		}
	}
}
