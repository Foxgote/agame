using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200006E RID: 110
	[HarmonyPatch(typeof(Body), "WearWearable")]
	internal static class DeviceWearPatch
	{
		// Token: 0x06000237 RID: 567 RVA: 0x0002BC2D File Offset: 0x00029E2D
		private static bool Prefix(Body __instance, Item item)
		{
			return Devices.BeforeWear(__instance, item);
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0002BC36 File Offset: 0x00029E36
		private static void Postfix(Body __instance, Item item)
		{
			Devices.AfterWear(__instance, item);
		}
	}
}
