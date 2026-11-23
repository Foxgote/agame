using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000135 RID: 309
	[HarmonyPatch(typeof(Body), "actualJumpSpeed", 1)]
	internal static class BroodJumpPatch
	{
		// Token: 0x06000759 RID: 1881 RVA: 0x000711B6 File Offset: 0x0006F3B6
		private static void Postfix(Body __instance, ref float __result)
		{
			if (PlayerCamera.main && __instance == PlayerCamera.main.body && Womb.PregStage == 3)
			{
				__result *= 0.85f;
			}
		}
	}
}
