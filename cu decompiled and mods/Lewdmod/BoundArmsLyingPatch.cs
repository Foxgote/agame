using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000072 RID: 114
	[HarmonyPatch(typeof(Body), "FixedUpdate")]
	internal static class BoundArmsLyingPatch
	{
		// Token: 0x0600023F RID: 575 RVA: 0x0002C102 File Offset: 0x0002A302
		private static void Postfix(Body __instance)
		{
			if (!__instance.standing)
			{
				BoundArms.Apply(__instance, true);
			}
		}
	}
}
