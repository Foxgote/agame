using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000C1 RID: 193
	[HarmonyPatch(typeof(GunScript), "Fire")]
	internal static class NoiseGunPatch
	{
		// Token: 0x06000447 RID: 1095 RVA: 0x00045264 File Offset: 0x00043464
		private static void Postfix(GunScript __instance)
		{
			Noise.Make(__instance.transform.position, Plugin.WormNoiseGun.Value, "gun", 0.1f);
		}
	}
}
