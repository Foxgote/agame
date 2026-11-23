using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000F8 RID: 248
	[HarmonyPatch(typeof(FluidManager), "LiquidName")]
	internal static class SemenNamePatch
	{
		// Token: 0x0600057B RID: 1403 RVA: 0x00059564 File Offset: 0x00057764
		[HarmonyPriority(0)]
		private static void Postfix(FluidManager __instance, Vector2Int pos, ref ValueTuple<string, string> __result)
		{
			if (SemenFluid.Id != 0 && __instance.GetLiquid(pos.x, pos.y) == SemenFluid.Id)
			{
				__result = new ValueTuple<string, string>(Plugin.T("semen_name"), Plugin.T("semen_desc"));
			}
		}
	}
}
