using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000075 RID: 117
	[HarmonyPatch(typeof(Body), "UseItem")]
	internal static class DeviceUsePatch
	{
		// Token: 0x06000244 RID: 580 RVA: 0x0002C1D4 File Offset: 0x0002A3D4
		internal static bool Edible(Item it)
		{
			return it && it.Stats != null && (it.Stats.category == "food" || it.Stats.category == "water" || it.Stats.category == "drug" || it.GetComponent<WaterContainerItem>());
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0002C248 File Offset: 0x0002A448
		private static bool Prefix(Body __instance, Item item)
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
			if (Devices.Gagged(__instance) && DeviceUsePatch.Edible(item))
			{
				DeviceHandsPatch.Say(__instance, "dd_gag_eat");
				return false;
			}
			return true;
		}
	}
}
