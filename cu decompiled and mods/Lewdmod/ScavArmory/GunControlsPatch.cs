using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000037 RID: 55
	[HarmonyPatch(typeof(PlayerCamera), "HandleGunMenu")]
	internal static class GunControlsPatch
	{
		// Token: 0x060000F1 RID: 241 RVA: 0x0000E49E File Offset: 0x0000C69E
		private static void Postfix(PlayerCamera __instance)
		{
			GunControls.Update(__instance);
		}
	}
}
