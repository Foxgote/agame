using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000120 RID: 288
	[HarmonyPatch(typeof(TraderScript), "ItemPrice")]
	internal static class TraderSuitPricePatch
	{
		// Token: 0x060006BE RID: 1726 RVA: 0x00069ED4 File Offset: 0x000680D4
		private static bool Prefix(TraderItem item, ref int __result)
		{
			if (item == null || item.id != "lewd_suitoff")
			{
				return true;
			}
			__result = Mathf.Max(0, TraderSuit.RemovalPrice.Value);
			return false;
		}
	}
}
