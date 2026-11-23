using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000B3 RID: 179
	[HarmonyPatch(typeof(Locale), "ChangeLanguage")]
	internal static class LewdLanguageFollowPatch
	{
		// Token: 0x06000412 RID: 1042 RVA: 0x000424F4 File Offset: 0x000406F4
		private static void Postfix()
		{
			Relief.ResetLines();
		}
	}
}
