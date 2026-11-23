using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000064 RID: 100
	[HarmonyPatch(typeof(Item), "SetupItems")]
	internal static class CounterRegisterPatch
	{
		// Token: 0x060001FA RID: 506 RVA: 0x00029195 File Offset: 0x00027395
		private static void Postfix()
		{
			Suppressant.RegisterLiquid();
			Suppressant.RegisterItem();
			Scalpel.RegisterItem();
		}
	}
}
