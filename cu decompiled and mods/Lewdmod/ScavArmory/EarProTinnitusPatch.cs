using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000031 RID: 49
	[HarmonyPatch(typeof(TurretScript), "Shoot")]
	internal static class EarProTinnitusPatch
	{
		// Token: 0x060000DD RID: 221 RVA: 0x0000DB74 File Offset: 0x0000BD74
		private static void Prefix(FireInfo info)
		{
			if (info != null && info.doTinnitus && EarPro.On)
			{
				info.doTinnitus = false;
			}
		}
	}
}
