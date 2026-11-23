using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200009D RID: 157
	[HarmonyPatch(typeof(Container), "LoadItem")]
	internal static class ToyContainerPatch
	{
		// Token: 0x06000312 RID: 786 RVA: 0x00034C14 File Offset: 0x00032E14
		private static bool Prefix(Container __instance, Item item)
		{
			Body body = (__instance ? __instance.GetComponentInParent<Body>() : null);
			return !body || Toys.AllowPickUp(body, item);
		}
	}
}
