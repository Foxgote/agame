using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200012E RID: 302
	[HarmonyPatch(typeof(Body), "actualMaxSpeed", 1)]
	internal static class WebSlowPatch
	{
		// Token: 0x06000736 RID: 1846 RVA: 0x0006FB20 File Offset: 0x0006DD20
		private static void Postfix(Body __instance, ref float __result)
		{
			if (Webs.OnSilk && PlayerCamera.main && __instance == PlayerCamera.main.body)
			{
				__result *= Webs.Slow;
			}
		}
	}
}
