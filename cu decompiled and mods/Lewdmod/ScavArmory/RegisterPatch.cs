using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x0200001F RID: 31
	[HarmonyPatch(typeof(Item), "SetupItems")]
	internal static class RegisterPatch
	{
		// Token: 0x06000066 RID: 102 RVA: 0x00007604 File Offset: 0x00005804
		private static void Postfix()
		{
			Armory.Register();
		}
	}
}
