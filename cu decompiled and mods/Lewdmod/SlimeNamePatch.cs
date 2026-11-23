using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000109 RID: 265
	[HarmonyPatch(typeof(FluidManager), "LiquidName")]
	internal static class SlimeNamePatch
	{
		// Token: 0x060005D0 RID: 1488 RVA: 0x0005D4A8 File Offset: 0x0005B6A8
		[HarmonyPriority(0)]
		private static void Postfix(FluidManager __instance, Vector2Int pos, ref ValueTuple<string, string> __result)
		{
			if (__instance.GetLiquid(pos.x, pos.y) == SlimeFluid.Id)
			{
				__result = (SlimeFluid.Dark(pos) ? new ValueTuple<string, string>(Plugin.T("hbog_name"), Plugin.T("hbog_desc")) : new ValueTuple<string, string>(Plugin.T("slime_name"), Plugin.T("slime_desc")));
			}
		}
	}
}
