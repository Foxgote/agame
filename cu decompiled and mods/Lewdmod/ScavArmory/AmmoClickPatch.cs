using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000015 RID: 21
	[HarmonyPatch(typeof(GunScript), "Update")]
	internal static class AmmoClickPatch
	{
		// Token: 0x06000045 RID: 69 RVA: 0x00004F70 File Offset: 0x00003170
		private static void Prefix(GunScript __instance)
		{
			if (!__instance.triggerPressed || __instance.firingPinStruck || __instance.safe || __instance.roundInChamber == null)
			{
				return;
			}
			Body body = AmmoLines.Holder(__instance);
			if (body)
			{
				AmmoLines.Click(body, __instance);
			}
		}
	}
}
