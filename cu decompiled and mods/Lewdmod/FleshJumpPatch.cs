using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200008C RID: 140
	[HarmonyPatch(typeof(Body), "actualJumpSpeed", 1)]
	internal static class FleshJumpPatch
	{
		// Token: 0x060002AD RID: 685 RVA: 0x0002FDCF File Offset: 0x0002DFCF
		private static void Postfix(Body __instance, ref float __result)
		{
			if (FleshZones.OnFlesh && PlayerCamera.main && __instance == PlayerCamera.main.body)
			{
				__result *= Mathf.Lerp(1f, FleshZones.Slow, 0.6f);
			}
		}
	}
}
