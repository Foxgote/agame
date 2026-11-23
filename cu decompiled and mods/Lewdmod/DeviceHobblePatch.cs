using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200007A RID: 122
	[HarmonyPatch(typeof(Body), "actualMaxSpeed", 1)]
	internal static class DeviceHobblePatch
	{
		// Token: 0x0600024B RID: 587 RVA: 0x0002C4E0 File Offset: 0x0002A6E0
		private static void Postfix(Body __instance, ref float __result)
		{
			if (PlayerCamera.main && __instance == PlayerCamera.main.body && Devices.Hobbled(__instance))
			{
				__result *= 0.55f;
			}
		}
	}
}
