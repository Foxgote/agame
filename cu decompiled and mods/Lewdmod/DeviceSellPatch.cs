using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200007D RID: 125
	[HarmonyPatch(typeof(TraderScript), "GiveItem")]
	internal static class DeviceSellPatch
	{
		// Token: 0x0600024F RID: 591 RVA: 0x0002C5C6 File Offset: 0x0002A7C6
		private static bool Prefix(Item item)
		{
			return !DeviceSellPatch.Refused(item);
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0002C5D4 File Offset: 0x0002A7D4
		internal static bool Refused(Item item)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (!body || !item)
			{
				return false;
			}
			if (Devices.IsLocked(item) && Devices.OnHer(body, item))
			{
				DeviceSellPatch.Refuse(body, item, "dd_locked");
				return true;
			}
			if (Devices.HandsBound(body))
			{
				DeviceSellPatch.Refuse(body, item, "dd_hands");
				return true;
			}
			return false;
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0002C644 File Offset: 0x0002A844
		private static void Refuse(Body b, Item item, string line)
		{
			if (item == DeviceSellPatch.lastItem && Time.frameCount == DeviceSellPatch.lastFrame)
			{
				return;
			}
			DeviceSellPatch.lastItem = item;
			DeviceSellPatch.lastFrame = Time.frameCount;
			DeviceHandsPatch.Say(b, line);
			Plugin.Log.LogInfo(string.Concat(new string[] { "Devices: ", item.id, " not given to the trader (", line, ")" }));
		}

		// Token: 0x0400024C RID: 588
		private static Item lastItem;

		// Token: 0x0400024D RID: 589
		private static int lastFrame = -1;
	}
}
