using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000016 RID: 22
	[HarmonyPatch(typeof(GunScript), "Fire")]
	internal static class AmmoShotPatch
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00004FB8 File Offset: 0x000031B8
		private static void Postfix(GunScript __instance)
		{
			Body body = AmmoLines.Holder(__instance);
			if (body)
			{
				AmmoLines.Shot(body, __instance);
			}
		}
	}
}
