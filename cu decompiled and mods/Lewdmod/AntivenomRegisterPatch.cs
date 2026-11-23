using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000051 RID: 81
	[HarmonyPatch(typeof(Item), "SetupItems")]
	internal static class AntivenomRegisterPatch
	{
		// Token: 0x06000195 RID: 405 RVA: 0x0001ABA2 File Offset: 0x00018DA2
		private static void Postfix()
		{
			Antivenom.RegisterItem();
		}
	}
}
