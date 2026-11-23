using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000121 RID: 289
	[HarmonyPatch(typeof(TraderScript), "TryPurchase")]
	internal static class TraderSuitPurchasePatch
	{
		// Token: 0x060006BF RID: 1727 RVA: 0x00069F00 File Offset: 0x00068100
		private static bool Prefix(TraderScript __instance, TraderItem item)
		{
			if (item == null || item.id != "lewd_suitoff")
			{
				return true;
			}
			bool flag;
			try
			{
				flag = TraderSuit.Purchase(__instance, item);
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Trader suit removal: " + ex.Message);
				flag = true;
			}
			if (!flag)
			{
				return true;
			}
			if (PlayerCamera.main)
			{
				PlayerCamera.main.RefreshTraderInventories();
				PlayerCamera.main.UpdateTradeTexts();
			}
			return false;
		}
	}
}
