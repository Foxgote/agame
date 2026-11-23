using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000053 RID: 83
	[HarmonyPatch(typeof(Locale), "GetItem")]
	internal static class AntivenomNamePatch
	{
		// Token: 0x06000197 RID: 407 RVA: 0x0001ABC9 File Offset: 0x00018DC9
		private static void Postfix(string str, ref string __result)
		{
			if (str == "lewd_antivenom")
			{
				__result = Plugin.T("antivenom_name");
				return;
			}
			if (str == "lewd_antivenomdsc")
			{
				__result = Plugin.T("antivenom_desc");
			}
		}
	}
}
