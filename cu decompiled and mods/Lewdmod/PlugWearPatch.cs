using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000E3 RID: 227
	[HarmonyPatch(typeof(Body), "WearWearable")]
	internal static class PlugWearPatch
	{
		// Token: 0x06000527 RID: 1319 RVA: 0x000562E8 File Offset: 0x000544E8
		private static bool Prefix(Body __instance, Item item)
		{
			if (!Plugs.IsPlug(item) || !PlayerCamera.main || __instance != PlayerCamera.main.body || Time.timeSinceLevelLoad < 5f)
			{
				return true;
			}
			if (Devices.BeltOn(__instance))
			{
				if (Time.time - PlugWearPatch.last > 3f)
				{
					PlugWearPatch.last = Time.time;
					Relief.Say(__instance, "toy_belt");
					PlayerCamera.main.PlayUISound(6, 1f);
				}
				return false;
			}
			if (Plugin.S.lust < Plugin.ToyMinLust.Value && Plugin.S.arousal < Plugin.ToyMinLust.Value)
			{
				if (Time.time - PlugWearPatch.last > 3f)
				{
					PlugWearPatch.last = Time.time;
					Relief.Say(__instance, "plug_nowear");
					PlayerCamera.main.PlayUISound(6, 1f);
				}
				return false;
			}
			return true;
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x000563D8 File Offset: 0x000545D8
		private static void Postfix(Body __instance, Item item)
		{
			ToyDef toyDef = Toys.ById(item ? item.id : null);
			if (!Plugs.IsPlug(item) || __instance.GetWearableBySlotID(item.Stats.wearSlotId) != item)
			{
				return;
			}
			Plugs.Hide(item, true);
			if (PlayerCamera.main && __instance == PlayerCamera.main.body && Time.timeSinceLevelLoad > 5f)
			{
				if (toyDef.Kind == "inflplug" && Devices.On)
				{
					Devices.Lock(item);
					Relief.Say(__instance, "inflplug_in");
				}
				else
				{
					Relief.Say(__instance, (toyDef.Kind == "vibe") ? "vibe_in" : ((toyDef.Kind == "tailplug") ? "tailplug_in" : ((toyDef.Size == "l") ? "plug_in_big" : "plug_in")));
				}
				Plugin.Log.LogInfo("She put in the " + item.id);
			}
		}

		// Token: 0x04000614 RID: 1556
		private static float last = -9f;
	}
}
