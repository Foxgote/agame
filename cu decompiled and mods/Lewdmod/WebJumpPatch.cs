using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200012F RID: 303
	[HarmonyPatch(typeof(Body), "actualJumpSpeed", 1)]
	internal static class WebJumpPatch
	{
		// Token: 0x06000737 RID: 1847 RVA: 0x0006FB51 File Offset: 0x0006DD51
		private static void Postfix(Body __instance, ref float __result)
		{
			if (Webs.OnSilk && PlayerCamera.main && __instance == PlayerCamera.main.body)
			{
				__result *= Mathf.Lerp(1f, Webs.Slow, 0.5f);
			}
		}
	}
}
