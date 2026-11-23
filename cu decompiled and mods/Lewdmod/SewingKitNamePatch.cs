using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000FE RID: 254
	[HarmonyPatch(typeof(Locale), "GetItem")]
	internal static class SewingKitNamePatch
	{
		// Token: 0x06000585 RID: 1413 RVA: 0x00059BCC File Offset: 0x00057DCC
		private static void Postfix(string str, ref string __result)
		{
			if (str == "lewd_sewingkit")
			{
				__result = Plugin.T("sewkit_name");
				return;
			}
			if (str == "lewd_sewingkitdsc")
			{
				__result = Plugin.T("sewkit_desc");
			}
		}
	}
}
