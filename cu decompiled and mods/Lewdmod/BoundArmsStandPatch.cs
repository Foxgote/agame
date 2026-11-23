using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000071 RID: 113
	[HarmonyPatch(typeof(Body), "Update")]
	internal static class BoundArmsStandPatch
	{
		// Token: 0x0600023E RID: 574 RVA: 0x0002C0F1 File Offset: 0x0002A2F1
		private static void Postfix(Body __instance)
		{
			if (__instance.standing)
			{
				BoundArms.Apply(__instance, false);
			}
		}
	}
}
