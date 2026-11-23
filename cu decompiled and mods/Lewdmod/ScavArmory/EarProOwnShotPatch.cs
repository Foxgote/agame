using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000032 RID: 50
	[HarmonyPatch(typeof(GunScript), "Fire")]
	internal static class EarProOwnShotPatch
	{
		// Token: 0x060000DE RID: 222 RVA: 0x0000DB90 File Offset: 0x0000BD90
		private static void Prefix(out float __state)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			__state = (body ? body.hearingLoss : (-1f));
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000DBD0 File Offset: 0x0000BDD0
		private static void Postfix(float __state)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && __state >= 0f && body.hearingLoss > __state && EarPro.On)
			{
				body.hearingLoss = __state;
			}
		}
	}
}
