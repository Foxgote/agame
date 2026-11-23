using System;
using HarmonyLib;
using LewdStatus;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000025 RID: 37
	[HarmonyPatch(typeof(Body), "Update")]
	internal static class DebugLoadPatch
	{
		// Token: 0x0600006C RID: 108 RVA: 0x000076E8 File Offset: 0x000058E8
		private static void Postfix(Body __instance)
		{
			if (!Plugin.DebugOn || !DebugBinds.Down(DebugBinds.LoadGun) || !PlayerCamera.main || PlayerCamera.main.body != __instance)
			{
				return;
			}
			Item item = (__instance.HoldingItem(__instance.handSlot) ? __instance.GetItem(__instance.handSlot) : null);
			GunScript gunScript;
			if (!item || !item.TryGetComponent<GunScript>(ref gunScript))
			{
				if (item)
				{
					__instance.DropItem(__instance.handSlot);
				}
				GameObject gameObject = Utils.Create("pistol", __instance.limbs[1].transform.position, 0f);
				if (!gameObject || !gameObject.TryGetComponent<Item>(ref item))
				{
					return;
				}
				__instance.PickUpItem(item, __instance.handSlot, false);
				Plugin.Log.LogInfo("Debug: a pistol in her hand");
			}
			GunScript gunScript2;
			if (!item || !item.TryGetComponent<GunScript>(ref gunScript2))
			{
				return;
			}
			if (gunScript2.feedType == null)
			{
				GameObject gameObject2 = Resources.Load(AmmoScript.AmmoTypeToMagazine(gunScript2.ammoType)) as GameObject;
				gunScript2.hasMag = true;
				AmmoScript ammoScript;
				gunScript2.roundsInMag = ((gameObject2 && gameObject2.TryGetComponent<AmmoScript>(ref ammoScript)) ? ammoScript.maxRounds : 10);
			}
			else
			{
				gunScript2.roundsInMag = gunScript2.magCapacity;
			}
			gunScript2.roundInChamber = 0;
			gunScript2.racked = false;
			gunScript2.safe = false;
			Plugin.Log.LogInfo(string.Format("Debug: {0} filled ({1} + 1)", item.id, gunScript2.roundsInMag));
		}
	}
}
