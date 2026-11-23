using System;
using HarmonyLib;
using PoseKit;

namespace LewdStatus
{
	// Token: 0x020000EF RID: 239
	[HarmonyPatch(typeof(Body), "Ragdoll")]
	internal static class PoseNoRagdollPatch
	{
		// Token: 0x0600054E RID: 1358 RVA: 0x00057945 File Offset: 0x00055B45
		private static bool Prefix(Body __instance)
		{
			return !HerPuppet.Holds(__instance) || !__instance.alive;
		}
	}
}
