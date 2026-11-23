using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000022 RID: 34
	[HarmonyPatch(typeof(AmmoScript), "AmmoTypeToItem")]
	internal static class RoundPatch
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00007693 File Offset: 0x00005893
		private static bool Prefix(GunScript.AmmoType ammo, ref string __result)
		{
			if (ammo == 4)
			{
				__result = "arm_762round";
				return false;
			}
			if (ammo != 3)
			{
				return true;
			}
			__result = "arm_45round";
			return false;
		}
	}
}
