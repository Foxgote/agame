using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000F6 RID: 246
	[HarmonyPatch(typeof(FluidManager), "RenderFluids")]
	internal static class SemenRenderPatch
	{
		// Token: 0x06000579 RID: 1401 RVA: 0x0005951F File Offset: 0x0005771F
		[HarmonyPriority(0)]
		private static void Postfix()
		{
			SemenFluid.Tint();
		}
	}
}
