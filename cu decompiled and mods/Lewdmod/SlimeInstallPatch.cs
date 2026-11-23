using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000106 RID: 262
	[HarmonyPatch(typeof(FluidManager), "Start")]
	internal static class SlimeInstallPatch
	{
		// Token: 0x060005CD RID: 1485 RVA: 0x0005D45A File Offset: 0x0005B65A
		private static void Postfix(FluidManager __instance)
		{
			SlimeFluid.Install(__instance);
			SemenFluid.Install(__instance);
		}
	}
}
