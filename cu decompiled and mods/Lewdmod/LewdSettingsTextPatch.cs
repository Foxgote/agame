using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000B4 RID: 180
	[HarmonyPatch(typeof(Locale), "GetOther")]
	internal static class LewdSettingsTextPatch
	{
		// Token: 0x06000413 RID: 1043 RVA: 0x000424FB File Offset: 0x000406FB
		private static void Postfix(string str, ref string __result)
		{
			if (str != null && str.StartsWith("gamesetlewd_"))
			{
				__result = Plugin.T(str);
				if (__result.Contains("{0}"))
				{
					__result = __result.Replace("{0}", AboutMod.Version);
				}
			}
		}
	}
}
