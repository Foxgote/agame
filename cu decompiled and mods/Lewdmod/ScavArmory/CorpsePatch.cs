using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000027 RID: 39
	[HarmonyPatch(typeof(CorpseScript), "Start")]
	internal static class CorpsePatch
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00007902 File Offset: 0x00005B02
		private static void Postfix(CorpseScript __instance)
		{
			if (!__instance.animalCorpse)
			{
				Armory.Drop(__instance.transform.position, Plugin.CorpseGunChance.Value, Plugin.CorpseAmmoChance.Value, "by a corpse", false);
			}
		}
	}
}
