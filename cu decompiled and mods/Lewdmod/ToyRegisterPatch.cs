using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000098 RID: 152
	[HarmonyPatch(typeof(Item), "SetupItems")]
	internal static class ToyRegisterPatch
	{
		// Token: 0x0600030D RID: 781 RVA: 0x00034B58 File Offset: 0x00032D58
		private static void Postfix()
		{
			Toys.Register();
		}
	}
}
