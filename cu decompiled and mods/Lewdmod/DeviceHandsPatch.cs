using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000074 RID: 116
	[HarmonyPatch(typeof(Body), "UseItemInHand")]
	internal static class DeviceHandsPatch
	{
		// Token: 0x06000241 RID: 577 RVA: 0x0002C11C File Offset: 0x0002A31C
		private static bool Prefix(Body __instance)
		{
			if (!PlayerCamera.main || __instance != PlayerCamera.main.body)
			{
				return true;
			}
			if (Devices.HandsBound(__instance))
			{
				DeviceHandsPatch.Say(__instance, "dd_hands");
				return false;
			}
			Item item = (__instance.HoldingItem(__instance.handSlot) ? __instance.GetItem(__instance.handSlot) : null);
			if (item && Devices.Gagged(__instance) && DeviceUsePatch.Edible(item))
			{
				DeviceHandsPatch.Say(__instance, "dd_gag_eat");
				return false;
			}
			return true;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0002C1A2 File Offset: 0x0002A3A2
		internal static void Say(Body b, string key)
		{
			if (Time.time - DeviceHandsPatch.last < 2.5f)
			{
				return;
			}
			DeviceHandsPatch.last = Time.time;
			Relief.Say(b, key);
		}

		// Token: 0x04000249 RID: 585
		private static float last = -9f;
	}
}
