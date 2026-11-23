using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000F7 RID: 247
	[HarmonyPatch(typeof(FluidManager), "WaterInfo")]
	internal static class SemenInfoPatch
	{
		// Token: 0x0600057A RID: 1402 RVA: 0x00059526 File Offset: 0x00057726
		[HarmonyPriority(0)]
		private static void Postfix(FluidManager __instance, Vector2Int pos, [TupleElementNames(new string[] { "buoyancy", "drag", "type" })] ref ValueTuple<float, float, int> __result)
		{
			if (SemenFluid.Id != 0 && __instance.GetLiquid(pos.x, pos.y) == SemenFluid.Id)
			{
				__result = new ValueTuple<float, float, int>(0.35f, 0.7f, (int)SemenFluid.Id);
			}
		}
	}
}
