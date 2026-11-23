using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200008B RID: 139
	[HarmonyPatch(typeof(Body), "actualMaxSpeed", 1)]
	internal static class FleshSlowPatch
	{
		// Token: 0x060002AC RID: 684 RVA: 0x0002FD9E File Offset: 0x0002DF9E
		private static void Postfix(Body __instance, ref float __result)
		{
			if (FleshZones.OnFlesh && PlayerCamera.main && __instance == PlayerCamera.main.body)
			{
				__result *= FleshZones.Slow;
			}
		}
	}
}
