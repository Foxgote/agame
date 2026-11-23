using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000076 RID: 118
	[HarmonyPatch(typeof(Body), "Drink")]
	internal static class DeviceDrinkPatch
	{
		// Token: 0x06000246 RID: 582 RVA: 0x0002C2A8 File Offset: 0x0002A4A8
		private static bool Prefix(Body __instance)
		{
			return !PlayerCamera.main || !(__instance == PlayerCamera.main.body) || !Devices.Gagged(__instance);
		}
	}
}
