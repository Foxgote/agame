using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200011B RID: 283
	[HarmonyPatch(typeof(TraderScript), "MeetPlayer")]
	internal static class TraderSuitPatch
	{
		// Token: 0x060006B9 RID: 1721 RVA: 0x00069DC4 File Offset: 0x00067FC4
		private static void Postfix(TraderScript __instance)
		{
			try
			{
				TraderSuit.Offer(__instance);
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Trader and suit removal: " + ex.Message);
			}
			try
			{
				TraderSuit.Meet(__instance);
			}
			catch (Exception ex2)
			{
				Plugin.Log.LogWarning("Trader and suit: " + ex2.Message);
			}
		}
	}
}
