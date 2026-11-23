using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x0200001A RID: 26
	[HarmonyPatch(typeof(TraderScript), "GenerateInventory")]
	internal static class AmmoStockPatch
	{
		// Token: 0x06000051 RID: 81 RVA: 0x0000556D File Offset: 0x0000376D
		private static void Postfix(TraderScript __instance)
		{
			AmmoTrade.Stock_(__instance);
		}
	}
}
