using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x0200004B RID: 75
	[HarmonyPatch(typeof(GunScript), "Fire")]
	internal static class UltimatumShotPatch
	{
		// Token: 0x0600017B RID: 379 RVA: 0x00019CF8 File Offset: 0x00017EF8
		private static void Postfix(GunScript __instance)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && __instance && __instance.transform.IsChildOf(body.transform.root))
			{
				Ultimatum.HerAttack(body);
			}
		}
	}
}
