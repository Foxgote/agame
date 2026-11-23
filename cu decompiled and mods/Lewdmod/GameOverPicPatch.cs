using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000093 RID: 147
	[HarmonyPatch(typeof(PlayerCamera), "HandleDeathScreen")]
	internal static class GameOverPicPatch
	{
		// Token: 0x060002FA RID: 762 RVA: 0x00033AC0 File Offset: 0x00031CC0
		private static void Prefix(PlayerCamera __instance, out bool __state)
		{
			__state = __instance.didDeathScreen;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00033ACC File Offset: 0x00031CCC
		private static void Postfix(PlayerCamera __instance, bool __state)
		{
			if (__state != __instance.didDeathScreen)
			{
				Plugin.Log.LogInfo("Death screen " + (__instance.didDeathScreen ? "up" : "down"));
			}
			if (!__state && __instance.didDeathScreen)
			{
				GameOverPic.Show(__instance.endScreen);
				RunStats.Show(__instance.endScreen);
				return;
			}
			if (__state && !__instance.didDeathScreen)
			{
				GameOverPic.Hide();
				RunStats.Hide();
			}
		}
	}
}
