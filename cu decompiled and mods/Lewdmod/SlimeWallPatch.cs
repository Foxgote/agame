using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200012A RID: 298
	[HarmonyPatch(typeof(Body), "HandleGroundedState")]
	internal static class SlimeWallPatch
	{
		// Token: 0x060006F0 RID: 1776 RVA: 0x0006C253 File Offset: 0x0006A453
		private static void Postfix(Body __instance)
		{
			SlimeWall.Apply(__instance);
		}
	}
}
