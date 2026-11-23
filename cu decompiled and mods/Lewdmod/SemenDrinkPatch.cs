using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000F9 RID: 249
	[HarmonyPatch(typeof(FluidManager), "DrinkLiquid")]
	internal static class SemenDrinkPatch
	{
		// Token: 0x0600057C RID: 1404 RVA: 0x000595B2 File Offset: 0x000577B2
		[HarmonyPriority(800)]
		private static bool Prefix(FluidManager __instance, Vector2Int pos, Body body)
		{
			if (SemenFluid.Id == 0 || __instance.GetLiquid(pos.x, pos.y) != SemenFluid.Id)
			{
				return true;
			}
			body.talker.RefuseEat();
			return false;
		}
	}
}
