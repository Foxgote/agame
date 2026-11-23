using System;
using HarmonyLib;
using LewdStatus;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x0200002D RID: 45
	[HarmonyPatch(typeof(Body), "Update")]
	internal static class CaptureTickPatch
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x0000C9FC File Offset: 0x0000ABFC
		private static void Postfix(Body __instance)
		{
			if ((!Plugin.ScavEnabled.Value && !Plugin.HostsEnabled.Value) || !PlayerCamera.main || PlayerCamera.main.body != __instance)
			{
				return;
			}
			float num = Mathf.Min(Time.deltaTime, 0.05f);
			if (num <= 0f)
			{
				return;
			}
			Capture.Update(__instance, num);
			Deal.Update(__instance, num);
			if (!Capture.Active && Keys.Down(Plugin.DealKey.Value))
			{
				Deal.Propose(__instance);
			}
			if (!Capture.Active && Keys.Down(Plugin.SurrenderKey.Value) && !Deal.Accept(__instance))
			{
				Capture.Surrender(__instance);
			}
		}
	}
}
