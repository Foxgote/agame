using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000013 RID: 19
	[HarmonyPatch(typeof(PlayerCamera), "HandleGunMenu")]
	internal static class AmmoHudPatch
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00004CBC File Offset: 0x00002EBC
		private static void Postfix(PlayerCamera __instance)
		{
			AmmoInfo.Hud(__instance);
		}
	}
}
