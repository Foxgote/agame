using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000069 RID: 105
	[HarmonyPatch(typeof(PlayerCamera), "HandleWorldUI")]
	internal static class CrouchGuidePatch
	{
		// Token: 0x060001FF RID: 511 RVA: 0x00029295 File Offset: 0x00027495
		private static void Postfix(PlayerCamera __instance)
		{
			if (Plugin.HideCrouchGuide.Value && __instance.crouchGuide)
			{
				__instance.crouchGuide.color = Color.clear;
			}
		}
	}
}
