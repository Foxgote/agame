using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x0200003B RID: 59
	[HarmonyPatch(typeof(Body), "PickUpItem")]
	internal static class GunPickUpPatch
	{
		// Token: 0x060000FE RID: 254 RVA: 0x0000EB37 File Offset: 0x0000CD37
		private static void Postfix(Body __instance, Item item, bool force)
		{
			if (!force)
			{
				GunFinds.Picked(__instance, item);
				AmmoLines.Picked(__instance, item);
			}
		}
	}
}
