using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000F2 RID: 242
	[HarmonyPatch(typeof(Body), "TryLastStand")]
	internal static class SecondChancePatch
	{
		// Token: 0x06000565 RID: 1381 RVA: 0x0005843D File Offset: 0x0005663D
		private static void Prefix(Body __instance, out bool __state)
		{
			__state = __instance.succesfullyRolledLastStand;
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x00058448 File Offset: 0x00056648
		private static void Postfix(Body __instance, bool __state)
		{
			if (__state || !__instance.succesfullyRolledLastStand || !PlayerCamera.main || PlayerCamera.main.body != __instance)
			{
				return;
			}
			try
			{
				SecondChance.Take(__instance);
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Second chance: " + ex.Message);
			}
		}
	}
}
