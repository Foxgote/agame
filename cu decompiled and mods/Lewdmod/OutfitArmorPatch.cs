using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000D2 RID: 210
	[HarmonyPatch(typeof(Limb), "GetArmorReduction")]
	internal static class OutfitArmorPatch
	{
		// Token: 0x06000484 RID: 1156 RVA: 0x00049390 File Offset: 0x00047590
		private static void Postfix(Limb __instance, ref float __result)
		{
			if (!__instance.body)
			{
				return;
			}
			foreach (OutfitDef outfitDef in Outfits.WornAll(__instance.body))
			{
				if (outfitDef.Armor > 0f && Outfits.Covers(outfitDef, __instance))
				{
					__result += outfitDef.Armor;
				}
			}
		}
	}
}
