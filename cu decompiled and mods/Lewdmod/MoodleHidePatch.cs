using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000BD RID: 189
	[HarmonyPatch(typeof(MoodleManager), "AddMoodle")]
	internal static class MoodleHidePatch
	{
		// Token: 0x0600043C RID: 1084 RVA: 0x00044850 File Offset: 0x00042A50
		private static bool Prefix(string icon)
		{
			return !MoodleFilter.Hidden(icon);
		}
	}
}
