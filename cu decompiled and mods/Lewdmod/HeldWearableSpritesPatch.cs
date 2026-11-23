using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200009E RID: 158
	[HarmonyPatch(typeof(Body), "PickUpItem")]
	internal static class HeldWearableSpritesPatch
	{
		// Token: 0x06000313 RID: 787 RVA: 0x00034C44 File Offset: 0x00032E44
		private static void Postfix(Body __instance, Item item)
		{
			Wearable wearable;
			if (item && __instance.HoldingItem(item) && item.TryGetComponent<Wearable>(ref wearable))
			{
				wearable.ClearSprites();
			}
		}
	}
}
