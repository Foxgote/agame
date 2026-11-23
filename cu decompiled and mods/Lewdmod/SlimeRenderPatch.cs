using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000107 RID: 263
	[HarmonyPatch(typeof(FluidManager), "RenderFluids")]
	internal static class SlimeRenderPatch
	{
		// Token: 0x060005CE RID: 1486 RVA: 0x0005D468 File Offset: 0x0005B668
		[HarmonyPriority(0)]
		private static void Postfix()
		{
			SlimeFluid.Tint();
		}
	}
}
