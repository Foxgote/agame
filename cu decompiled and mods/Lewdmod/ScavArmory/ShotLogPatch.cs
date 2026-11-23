using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000026 RID: 38
	[HarmonyPatch(typeof(GunScript), "Fire")]
	internal static class ShotLogPatch
	{
		// Token: 0x0600006D RID: 109 RVA: 0x00007868 File Offset: 0x00005A68
		private static void Postfix(GunScript __instance)
		{
			if (!Plugin.LogShots.Value)
			{
				return;
			}
			Item component = __instance.GetComponent<Item>();
			Plugin.Log.LogInfo(string.Format("Shot: {0} {1}, {2} left, damage {3:0}, cond {4:0.00}", new object[]
			{
				component ? component.id : "?",
				__instance.ammoType,
				__instance.roundsInMag,
				__instance.animalDamage,
				component ? component.condition : 0f
			}));
		}
	}
}
