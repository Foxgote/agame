using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000073 RID: 115
	[HarmonyPatch(typeof(Body), "DropWearable")]
	internal static class DeviceDropPatch
	{
		// Token: 0x06000240 RID: 576 RVA: 0x0002C113 File Offset: 0x0002A313
		private static bool Prefix(Body __instance, Item item)
		{
			return Devices.AllowDrop(__instance, item);
		}
	}
}
