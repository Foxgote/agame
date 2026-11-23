using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200013A RID: 314
	[HarmonyPatch(typeof(Body), "Attack")]
	internal static class WormStabPatch
	{
		// Token: 0x060007AB RID: 1963 RVA: 0x0007601C File Offset: 0x0007421C
		private static bool Prefix(Body __instance, AttackInfo atk, ref bool __result)
		{
			if (!Worm.Inside || !PlayerCamera.main || __instance != PlayerCamera.main.body)
			{
				return true;
			}
			__result = Worm.Stab(__instance, atk);
			return false;
		}
	}
}
