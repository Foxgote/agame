using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000124 RID: 292
	[HarmonyPatch(typeof(PlayerCamera), "HandleInput")]
	internal static class WallCrawlInputPatch
	{
		// Token: 0x060006D1 RID: 1745 RVA: 0x0006A7E1 File Offset: 0x000689E1
		private static void Postfix(PlayerCamera __instance)
		{
			if (__instance.body && !PauseHandler.paused)
			{
				WallCrawl.Steer(__instance.body);
			}
		}
	}
}
