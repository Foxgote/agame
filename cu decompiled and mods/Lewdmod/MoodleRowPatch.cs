using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000BC RID: 188
	[HarmonyPatch(typeof(MoodleManager), "UpdateMoodles")]
	internal static class MoodleRowPatch
	{
		// Token: 0x0600043A RID: 1082 RVA: 0x00044840 File Offset: 0x00042A40
		[HarmonyPriority(800)]
		private static void Prefix(MoodleManager __instance)
		{
			MoodleFilter.Snapshot(__instance);
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x00044848 File Offset: 0x00042A48
		[HarmonyPriority(0)]
		private static void Postfix(MoodleManager __instance)
		{
			MoodleFilter.Relayout(__instance);
		}
	}
}
