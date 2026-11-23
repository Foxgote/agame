using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000111 RID: 273
	[HarmonyPatch(typeof(Body), "actualJumpSpeed", 1)]
	internal static class SymbiosisJumpPatch
	{
		// Token: 0x06000666 RID: 1638 RVA: 0x00066070 File Offset: 0x00064270
		private static void Postfix(Body __instance, ref float __result)
		{
			if (PlayerCamera.main && __instance == PlayerCamera.main.body)
			{
				if (Suit.Symbiosis)
				{
					__result *= 1.15f;
					return;
				}
				if (Suit.DroneMerged)
				{
					__result *= 1.1f;
				}
			}
		}
	}
}
