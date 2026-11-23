using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000CF RID: 207
	[HarmonyPatch(typeof(Item), "SetupItems")]
	internal static class OutfitRegisterPatch
	{
		// Token: 0x06000481 RID: 1153 RVA: 0x00049302 File Offset: 0x00047502
		private static void Postfix()
		{
			Outfits.Register();
		}
	}
}
