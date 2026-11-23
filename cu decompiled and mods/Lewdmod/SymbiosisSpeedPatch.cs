using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000110 RID: 272
	[HarmonyPatch(typeof(Body), "actualMaxSpeed", 1)]
	internal static class SymbiosisSpeedPatch
	{
		// Token: 0x06000665 RID: 1637 RVA: 0x00066020 File Offset: 0x00064220
		private static void Postfix(Body __instance, ref float __result)
		{
			if (PlayerCamera.main && __instance == PlayerCamera.main.body)
			{
				if (Suit.Symbiosis)
				{
					__result *= 1.2f;
					return;
				}
				if (Suit.DroneMerged)
				{
					__result *= 1.35f;
				}
			}
		}
	}
}
