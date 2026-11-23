using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000134 RID: 308
	[HarmonyPatch(typeof(Body), "actualMaxSpeed", 1)]
	internal static class BroodSpeedPatch
	{
		// Token: 0x06000758 RID: 1880 RVA: 0x0007114C File Offset: 0x0006F34C
		private static void Postfix(Body __instance, ref float __result)
		{
			if (PlayerCamera.main && __instance == PlayerCamera.main.body)
			{
				int pregStage = Womb.PregStage;
				if (pregStage == 2)
				{
					__result *= 0.92f;
				}
				else if (pregStage == 3)
				{
					__result *= 0.8f;
				}
				if (Plugin.S.filled > 70f)
				{
					__result *= 0.93f;
				}
			}
		}
	}
}
