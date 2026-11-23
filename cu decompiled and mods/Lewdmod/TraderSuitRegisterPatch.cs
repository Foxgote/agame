using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200011C RID: 284
	[HarmonyPatch(typeof(Item), "SetupItems")]
	internal static class TraderSuitRegisterPatch
	{
		// Token: 0x060006BA RID: 1722 RVA: 0x00069E38 File Offset: 0x00068038
		private static void Postfix()
		{
			TraderSuit.RegisterItem();
		}
	}
}
