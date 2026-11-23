using System;
using System.Collections.Generic;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000078 RID: 120
	[HarmonyPatch(typeof(Talker), "Talk", new Type[]
	{
		typeof(List<string>),
		typeof(Limb),
		typeof(bool),
		typeof(bool)
	})]
	internal static class DeviceGagTalkListPatch
	{
		// Token: 0x06000249 RID: 585 RVA: 0x0002C3D0 File Offset: 0x0002A5D0
		private static void Prefix(Talker __instance, ref List<string> lines)
		{
			if (lines == null || lines.Count == 0 || !__instance || !__instance.body || !PlayerCamera.main || __instance.body != PlayerCamera.main.body)
			{
				return;
			}
			if (!Devices.Gagged(__instance.body))
			{
				return;
			}
			string text = lines[0];
			DeviceGagTalkPatchHelper.Muffle(ref text);
			lines = new List<string> { text };
		}
	}
}
