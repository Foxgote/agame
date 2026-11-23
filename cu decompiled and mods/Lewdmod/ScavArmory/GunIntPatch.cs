using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x0200003A RID: 58
	[HarmonyPatch(typeof(Item), "SetupItems")]
	internal static class GunIntPatch
	{
		// Token: 0x060000FD RID: 253 RVA: 0x0000EB30 File Offset: 0x0000CD30
		[HarmonyPriority(0)]
		private static void Postfix()
		{
			GunFinds.LowerIntelligence();
		}
	}
}
