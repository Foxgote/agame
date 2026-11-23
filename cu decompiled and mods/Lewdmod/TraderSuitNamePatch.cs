using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200011E RID: 286
	[HarmonyPatch(typeof(Locale), "GetItem")]
	internal static class TraderSuitNamePatch
	{
		// Token: 0x060006BC RID: 1724 RVA: 0x00069E56 File Offset: 0x00068056
		private static void Postfix(string str, ref string __result)
		{
			if (str == "lewd_suitoff")
			{
				__result = Plugin.T("tsuitoff_name");
				return;
			}
			if (str == "lewd_suitoffdsc")
			{
				__result = Plugin.T("tsuitoff_desc");
			}
		}
	}
}
