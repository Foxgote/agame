using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000095 RID: 149
	[HarmonyPatch(typeof(Body), "GetTotalInsulation")]
	internal static class BlanketWarmthPatch
	{
		// Token: 0x060002FE RID: 766 RVA: 0x00033BDB File Offset: 0x00031DDB
		private static void Postfix(Body __instance, ref float __result)
		{
			if (Blanket.Wrapped && PlayerCamera.main && PlayerCamera.main.body == __instance)
			{
				__result += Plugin.BlanketWarmth.Value;
			}
		}
	}
}
