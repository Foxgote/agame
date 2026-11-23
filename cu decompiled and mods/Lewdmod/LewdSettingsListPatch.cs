using System;
using System.Collections.Generic;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000B2 RID: 178
	[HarmonyPatch(typeof(Settings), "GetAllSettings")]
	internal static class LewdSettingsListPatch
	{
		// Token: 0x06000411 RID: 1041 RVA: 0x000424D5 File Offset: 0x000406D5
		private static void Postfix(ref List<Setting> __result)
		{
			if (LewdSettings.Enabled == null)
			{
				return;
			}
			__result = new List<Setting>(__result);
			__result.AddRange(LewdSettings.Entries());
		}
	}
}
