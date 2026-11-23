using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200011F RID: 287
	[HarmonyPatch(typeof(PlayerCamera), "RefreshTraderInventories")]
	internal static class TraderSuitShowPatch
	{
		// Token: 0x060006BD RID: 1725 RVA: 0x00069E8C File Offset: 0x0006808C
		private static void Prefix(PlayerCamera __instance)
		{
			try
			{
				TraderSuit.Sync(__instance.currentTrader);
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Trader suit removal: " + ex.Message);
			}
		}
	}
}
