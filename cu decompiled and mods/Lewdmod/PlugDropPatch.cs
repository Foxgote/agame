using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000E4 RID: 228
	[HarmonyPatch(typeof(Body), "DropWearable")]
	internal static class PlugDropPatch
	{
		// Token: 0x0600052A RID: 1322 RVA: 0x00056500 File Offset: 0x00054700
		private static void Postfix(Body __instance, Item item)
		{
			if (!Plugs.IsPlug(item) || __instance.GetWearableBySlotID(item.Stats.wearSlotId) == item)
			{
				return;
			}
			Plugs.Hide(item, false);
			if (PlayerCamera.main && __instance == PlayerCamera.main.body)
			{
				Relief.Say(__instance, "plug_out");
			}
		}
	}
}
