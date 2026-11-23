using System;
using System.Linq;
using HarmonyLib;
using LewdStatus;

namespace ScavArmory
{
	// Token: 0x02000034 RID: 52
	[HarmonyPatch(typeof(Container), "CanHoldItem")]
	internal static class PouchCountPatch
	{
		// Token: 0x060000E1 RID: 225 RVA: 0x0000DC3C File Offset: 0x0000BE3C
		private static void Postfix(Container __instance, ref bool __result)
		{
			if (!__result || __instance.tagRestriction == null || !__instance.tagRestriction.Contains("lewd_magazine"))
			{
				return;
			}
			Item component = __instance.GetComponent<Item>();
			int num;
			if (!component)
			{
				num = 4;
			}
			else
			{
				OutfitDef outfitDef = Outfits.ById(component.id);
				num = ((outfitDef != null) ? outfitDef.Pouches : 4);
			}
			int num2 = num;
			if (__instance.itemCount >= num2)
			{
				__result = false;
			}
		}
	}
}
