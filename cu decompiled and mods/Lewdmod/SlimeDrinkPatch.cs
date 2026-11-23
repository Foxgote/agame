using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200010A RID: 266
	[HarmonyPatch(typeof(FluidManager), "DrinkLiquid")]
	internal static class SlimeDrinkPatch
	{
		// Token: 0x060005D1 RID: 1489 RVA: 0x0005D512 File Offset: 0x0005B712
		[HarmonyPriority(800)]
		private static bool Prefix(FluidManager __instance, Vector2Int pos, Body body)
		{
			if (__instance.GetLiquid(pos.x, pos.y) != SlimeFluid.Id)
			{
				return true;
			}
			body.talker.RefuseEat();
			return false;
		}
	}
}
