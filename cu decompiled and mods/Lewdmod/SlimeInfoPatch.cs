using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000108 RID: 264
	[HarmonyPatch(typeof(FluidManager), "WaterInfo")]
	internal static class SlimeInfoPatch
	{
		// Token: 0x060005CF RID: 1487 RVA: 0x0005D46F File Offset: 0x0005B66F
		[HarmonyPriority(0)]
		private static void Postfix(FluidManager __instance, Vector2Int pos, [TupleElementNames(new string[] { "buoyancy", "drag", "type" })] ref ValueTuple<float, float, int> __result)
		{
			if (__instance.GetLiquid(pos.x, pos.y) == SlimeFluid.Id)
			{
				__result = new ValueTuple<float, float, int>(0.25f, 0.82f, (int)SlimeFluid.Id);
			}
		}
	}
}
