using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200007F RID: 127
	[HarmonyPatch(typeof(Container), "LoadItem")]
	internal static class DeviceBagPatch
	{
		// Token: 0x06000257 RID: 599 RVA: 0x0002C6F8 File Offset: 0x0002A8F8
		private static bool Prefix(Item item)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (!body || !item || !Devices.IsLocked(item) || !Devices.OnHer(body, item))
			{
				return true;
			}
			DeviceHandsPatch.Say(body, "dd_locked");
			Plugin.Log.LogInfo("Devices: " + item.id + " not put in a bag (locked on her)");
			return false;
		}
	}
}
