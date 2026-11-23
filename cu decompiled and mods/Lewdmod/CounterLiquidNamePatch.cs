using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000068 RID: 104
	[HarmonyPatch(typeof(Locale), "GetOther")]
	internal static class CounterLiquidNamePatch
	{
		// Token: 0x060001FE RID: 510 RVA: 0x00029260 File Offset: 0x00027460
		private static void Postfix(string str, ref string __result)
		{
			if (str == "lewd_suppressant")
			{
				__result = Plugin.T("supp_liquid");
				return;
			}
			if (str == "lewd_suppressantdsc")
			{
				__result = Plugin.T("supp_liquiddsc");
			}
		}
	}
}
