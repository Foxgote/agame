using System;
using HarmonyLib;

namespace PoseKit
{
	// Token: 0x02000008 RID: 8
	[HarmonyPatch(typeof(Body), "Update")]
	internal static class PuppetDrivePatch
	{
		// Token: 0x06000012 RID: 18 RVA: 0x00002D4B File Offset: 0x00000F4B
		private static void Prefix(Body __instance)
		{
			HerPuppet.Drive(__instance);
		}
	}
}
