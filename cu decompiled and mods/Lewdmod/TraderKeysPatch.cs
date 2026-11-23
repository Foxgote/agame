using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000080 RID: 128
	[HarmonyPatch(typeof(TraderScript), "MeetPlayer")]
	internal static class TraderKeysPatch
	{
		// Token: 0x06000258 RID: 600 RVA: 0x0002C770 File Offset: 0x0002A970
		private static void Postfix(TraderScript __instance)
		{
			try
			{
				TraderKeys.Meet(__instance);
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Trader and restraints: " + ex.Message);
			}
		}
	}
}
