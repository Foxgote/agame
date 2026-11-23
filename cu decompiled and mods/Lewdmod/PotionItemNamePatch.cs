using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000E8 RID: 232
	[HarmonyPatch(typeof(Locale), "GetItem")]
	internal static class PotionItemNamePatch
	{
		// Token: 0x06000535 RID: 1333 RVA: 0x00056AB9 File Offset: 0x00054CB9
		private static void Postfix(string str, ref string __result)
		{
			if (str == "lewd_lustpotion")
			{
				__result = Plugin.T("potion_name");
				return;
			}
			if (str == "lewd_lustpotiondsc")
			{
				__result = Plugin.T("potion_desc");
			}
		}
	}
}
