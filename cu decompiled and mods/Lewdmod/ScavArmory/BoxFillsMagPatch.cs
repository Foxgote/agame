using System;
using HarmonyLib;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000035 RID: 53
	[HarmonyPatch(typeof(AmmoScript), "LoadRound")]
	internal static class BoxFillsMagPatch
	{
		// Token: 0x060000E2 RID: 226 RVA: 0x0000DCA0 File Offset: 0x0000BEA0
		private static bool Prefix(AmmoScript __instance, AmmoScript ammo)
		{
			if (!__instance || !ammo || __instance == ammo)
			{
				return true;
			}
			Item component = ammo.GetComponent<Item>();
			Item component2 = __instance.GetComponent<Item>();
			if (!Armory.IsBox(component) || Armory.IsBox(component2) || __instance.itemType != 1)
			{
				return true;
			}
			if (ammo.ammoType != __instance.ammoType || ammo.rounds <= 0 || __instance.rounds >= __instance.maxRounds)
			{
				return false;
			}
			int num = Mathf.Min(ammo.rounds, __instance.maxRounds - __instance.rounds);
			__instance.rounds += num;
			ammo.rounds -= num;
			Sound.Play("gunloadshell", __instance.transform.position, false, true, null, 1f, 1f, false, false);
			Plugin.Log.LogInfo(string.Format("{0} rounds from {1} into {2}: {3}/{4}, {5} left in the box", new object[] { num, component.id, component2.id, __instance.rounds, __instance.maxRounds, ammo.rounds }));
			return false;
		}
	}
}
