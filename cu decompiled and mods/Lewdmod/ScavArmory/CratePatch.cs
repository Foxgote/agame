using System;
using System.Collections.Generic;
using HarmonyLib;
using LewdStatus;

namespace ScavArmory
{
	// Token: 0x02000028 RID: 40
	[HarmonyPatch(typeof(BuildingEntity), "CheckDamage")]
	internal static class CratePatch
	{
		// Token: 0x0600006F RID: 111 RVA: 0x0000793C File Offset: 0x00005B3C
		private static void Prefix(BuildingEntity __instance, bool ___didDestroyRoutine)
		{
			if (___didDestroyRoutine || __instance.health >= 0.5f || __instance.animal)
			{
				return;
			}
			if (!Outfits.IsLootCrate(__instance) || !CratePatch.opened.Add(__instance.GetInstanceID()))
			{
				return;
			}
			string text;
			if ((text = __instance.id) == null)
			{
				text = __instance.name ?? "";
			}
			string text2 = text.ToLowerInvariant();
			float num = CratePatch.CrateKind(text2);
			if (num <= 0f)
			{
				return;
			}
			Armory.Drop(__instance.transform.position, Plugin.CrateGunChance.Value * num, Plugin.CrateAmmoChance.Value * num, "in a crate (" + text2 + ")", true);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000079EC File Offset: 0x00005BEC
		private static float CrateKind(string id)
		{
			if (id.Contains("food"))
			{
				return 0f;
			}
			if (id.Contains("med"))
			{
				return 0.25f;
			}
			if (id.Contains("container") || id.Contains("cargo") || id.Contains("dropcapsule"))
			{
				return 1.2f;
			}
			return 1f;
		}

		// Token: 0x04000096 RID: 150
		private static readonly HashSet<int> opened = new HashSet<int>();
	}
}
