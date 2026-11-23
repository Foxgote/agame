using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000DC RID: 220
	[HarmonyPatch(typeof(Body), "Update")]
	internal static class BodyUpdatePatch
	{
		// Token: 0x06000514 RID: 1300 RVA: 0x00054C55 File Offset: 0x00052E55
		private static void Postfix(Body __instance)
		{
			if (PlayerCamera.main && PlayerCamera.main.body == __instance && Plugin.Instance != null)
			{
				Plugin.Instance.Tick(__instance);
			}
		}
	}
}
