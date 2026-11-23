using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200007B RID: 123
	[HarmonyPatch(typeof(Body), "actualJumpSpeed", 1)]
	internal static class DeviceHobbleJumpPatch
	{
		// Token: 0x0600024C RID: 588 RVA: 0x0002C512 File Offset: 0x0002A712
		private static void Postfix(Body __instance, ref float __result)
		{
			if (PlayerCamera.main && __instance == PlayerCamera.main.body && Devices.Hobbled(__instance))
			{
				__result *= 0.4f;
			}
		}
	}
}
