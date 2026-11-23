using System;
using System.Collections.Generic;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000CC RID: 204
	[HarmonyPatch(typeof(BuildingEntity), "CheckDamage")]
	internal static class OutfitCratePatch
	{
		// Token: 0x0600047D RID: 1149 RVA: 0x000490A8 File Offset: 0x000472A8
		private static void Prefix(BuildingEntity __instance, bool ___didDestroyRoutine)
		{
			if (___didDestroyRoutine || __instance.health >= 0.5f || __instance.animal || !LewdSettings.On)
			{
				return;
			}
			if (!Outfits.IsLootCrate(__instance) || !OutfitCratePatch.opened.Add(__instance.GetInstanceID()))
			{
				return;
			}
			Outfits.FillCrate(__instance);
		}

		// Token: 0x0400044C RID: 1100
		private static readonly HashSet<int> opened = new HashSet<int>();
	}
}
