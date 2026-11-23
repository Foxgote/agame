using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200006F RID: 111
	[HarmonyPatch(typeof(PlayerCamera), "TryPerformWorldActions")]
	internal static class DevicePlayerTakePatch
	{
		// Token: 0x06000239 RID: 569 RVA: 0x0002BC3F File Offset: 0x00029E3F
		private static void Prefix()
		{
			Devices.PlayerTaking = true;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0002BC47 File Offset: 0x00029E47
		private static void Finalizer()
		{
			Devices.PlayerTaking = false;
		}
	}
}
