using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000077 RID: 119
	[HarmonyPatch(typeof(Talker), "Talk", new Type[]
	{
		typeof(string),
		typeof(Limb),
		typeof(bool),
		typeof(bool)
	})]
	internal static class DeviceGagTalkPatch
	{
		// Token: 0x06000247 RID: 583 RVA: 0x0002C2D4 File Offset: 0x0002A4D4
		private static void Prefix(Talker __instance, ref string text)
		{
			if (string.IsNullOrEmpty(text) || !__instance || !__instance.body || !PlayerCamera.main || __instance.body != PlayerCamera.main.body)
			{
				return;
			}
			if (!Devices.Gagged(__instance.body))
			{
				return;
			}
			string[] array = (Plugin.Russian ? DeviceGagTalkPatch.Ru : DeviceGagTalkPatch.En);
			text = array[Mathf.Abs(text.GetHashCode()) % array.Length];
		}

		// Token: 0x0400024A RID: 586
		private static readonly string[] Ru = new string[] { "Ммф!", "Мм-мф...", "Ммммф!", "Хмф... мм...", "Ммф-мф!" };

		// Token: 0x0400024B RID: 587
		private static readonly string[] En = new string[] { "Mmph!", "Mm-mph...", "Mmmmph!", "Hmph... mm...", "Mmph-mph!" };
	}
}
