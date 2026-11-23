using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x0200003C RID: 60
	[HarmonyPatch(typeof(Body), "AutoPickUpItem")]
	internal static class GunAutoPickUpPatch
	{
		// Token: 0x060000FF RID: 255 RVA: 0x0000EB4A File Offset: 0x0000CD4A
		private static void Postfix(Body __instance, Item item)
		{
			GunFinds.Picked(__instance, item);
			AmmoLines.Picked(__instance, item);
		}
	}
}
