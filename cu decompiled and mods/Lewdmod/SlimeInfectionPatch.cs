using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000115 RID: 277
	[HarmonyPatch(typeof(Limb), "GetInfectionSpeed")]
	internal static class SlimeInfectionPatch
	{
		// Token: 0x0600066B RID: 1643 RVA: 0x000664D8 File Offset: 0x000646D8
		private static void Postfix(Limb __instance, ref float __result)
		{
			if (!__instance.body || !PlayerCamera.main || PlayerCamera.main.body != __instance.body)
			{
				return;
			}
			if (Suit.Slime && Suit.Stage >= 4)
			{
				__result *= Plugin.SlimeInfectionMult.Value;
				return;
			}
			if (Suit.ParasiteProtects)
			{
				float value = Plugin.ParasiteInfectionMult.Value;
				__result *= (Suit.Symbiote ? (value * value) : value);
			}
		}
	}
}
