using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000044 RID: 68
	[HarmonyPatch(typeof(Body), "ThrowItem")]
	internal static class ThrownByHerPatch
	{
		// Token: 0x06000159 RID: 345 RVA: 0x00015F01 File Offset: 0x00014101
		private static void Prefix(Body __instance)
		{
			if (!PlayerCamera.main || PlayerCamera.main.body != __instance)
			{
				return;
			}
			if (__instance.HoldingItem(__instance.handSlot))
			{
				ThrownByHer.Mark(__instance.GetItem(__instance.handSlot));
			}
		}
	}
}
