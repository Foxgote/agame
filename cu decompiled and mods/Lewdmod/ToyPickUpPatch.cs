using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200009B RID: 155
	[HarmonyPatch(typeof(Body), "PickUpItem")]
	internal static class ToyPickUpPatch
	{
		// Token: 0x06000310 RID: 784 RVA: 0x00034BFC File Offset: 0x00032DFC
		private static bool Prefix(Body __instance, Item item, bool force)
		{
			return force || Toys.AllowPickUp(__instance, item);
		}
	}
}
