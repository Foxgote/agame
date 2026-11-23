using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000E6 RID: 230
	[HarmonyPatch(typeof(Item), "SetupItems")]
	internal static class PotionRegisterPatch
	{
		// Token: 0x06000533 RID: 1331 RVA: 0x00056A8D File Offset: 0x00054C8D
		private static void Postfix()
		{
			Potion.RegisterLiquid();
			Potion.RegisterItem();
		}
	}
}
