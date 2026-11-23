using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000123 RID: 291
	[HarmonyPatch(typeof(Body), "FixedUpdate")]
	internal static class WallCrawlPhysicsPatch
	{
		// Token: 0x060006D0 RID: 1744 RVA: 0x0006A7D9 File Offset: 0x000689D9
		private static void Postfix(Body __instance)
		{
			WallCrawl.Physics(__instance);
		}
	}
}
