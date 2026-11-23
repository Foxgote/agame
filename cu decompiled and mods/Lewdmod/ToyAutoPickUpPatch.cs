using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200009C RID: 156
	[HarmonyPatch(typeof(Body), "AutoPickUpItem")]
	internal static class ToyAutoPickUpPatch
	{
		// Token: 0x06000311 RID: 785 RVA: 0x00034C0A File Offset: 0x00032E0A
		private static bool Prefix(Body __instance, Item item)
		{
			return Toys.AllowPickUp(__instance, item);
		}
	}
}
