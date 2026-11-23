using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000E9 RID: 233
	[HarmonyPatch(typeof(Locale), "GetOther")]
	internal static class PotionLiquidNamePatch
	{
		// Token: 0x06000536 RID: 1334 RVA: 0x00056AEE File Offset: 0x00054CEE
		private static void Postfix(string str, ref string __result)
		{
			if (str == "lewd_lust")
			{
				__result = Plugin.T("potion_liquid");
				return;
			}
			if (str == "lewd_lustdsc")
			{
				__result = Plugin.T("potion_liquiddsc");
			}
		}
	}
}
