using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000018 RID: 24
	[HarmonyPatch(typeof(RecipeResult), "SpawnResult")]
	internal static class CraftedBoxPatch
	{
		// Token: 0x0600004E RID: 78 RVA: 0x0000543C File Offset: 0x0000363C
		private static void Prefix(RecipeResult __instance, out HashSet<int> __state)
		{
			__state = null;
			if (__instance == null || __instance.isLiquid || AmmoTrade.CraftedRounds(__instance.id) <= 0)
			{
				return;
			}
			__state = new HashSet<int>(from i in Object.FindObjectsOfType<Item>()
				where i && i.id == __instance.id
				select i.GetInstanceID());
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000054C4 File Offset: 0x000036C4
		private static void Postfix(RecipeResult __instance, HashSet<int> __state)
		{
			if (__state == null)
			{
				return;
			}
			int num = AmmoTrade.CraftedRounds(__instance.id);
			foreach (Item item in Object.FindObjectsOfType<Item>())
			{
				AmmoScript ammoScript;
				if (item && item.id == __instance.id && !__state.Contains(item.GetInstanceID()) && item.TryGetComponent<AmmoScript>(ref ammoScript))
				{
					ammoScript.rounds = Mathf.Min(num, ammoScript.maxRounds);
					Plugin.Log.LogInfo(string.Format("Crafted {0}: {1} rounds", item.id, ammoScript.rounds));
				}
			}
		}
	}
}
