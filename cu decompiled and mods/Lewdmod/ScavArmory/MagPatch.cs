using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000023 RID: 35
	[HarmonyPatch(typeof(AmmoScript), "AmmoTypeToMagazine")]
	internal static class MagPatch
	{
		// Token: 0x0600006A RID: 106 RVA: 0x000076B0 File Offset: 0x000058B0
		private static bool Prefix(GunScript.AmmoType ammo, ref string __result)
		{
			if (ammo == 4)
			{
				__result = "arm_box762";
				return false;
			}
			if (ammo != 3)
			{
				return true;
			}
			__result = "arm_45mag";
			return false;
		}
	}
}
