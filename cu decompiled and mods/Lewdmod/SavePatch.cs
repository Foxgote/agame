using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000DF RID: 223
	[HarmonyPatch(typeof(SaveSystem), "SaveGame")]
	internal static class SavePatch
	{
		// Token: 0x0600051D RID: 1309 RVA: 0x00055EC6 File Offset: 0x000540C6
		private static void Postfix()
		{
			Plugin.Save();
		}
	}
}
