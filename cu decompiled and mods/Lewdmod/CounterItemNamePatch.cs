using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000067 RID: 103
	[HarmonyPatch(typeof(Locale), "GetItem")]
	internal static class CounterItemNamePatch
	{
		// Token: 0x060001FD RID: 509 RVA: 0x000291EC File Offset: 0x000273EC
		private static void Postfix(string str, ref string __result)
		{
			if (str == "lewd_suppressant_pills")
			{
				__result = Plugin.T("supp_name");
				return;
			}
			if (str == "lewd_suppressant_pillsdsc")
			{
				__result = Plugin.T("supp_desc");
				return;
			}
			if (str == "lewd_scalpel")
			{
				__result = Plugin.T("scalpel_name");
				return;
			}
			if (str == "lewd_scalpeldsc")
			{
				__result = Plugin.T("scalpel_desc");
			}
		}
	}
}
