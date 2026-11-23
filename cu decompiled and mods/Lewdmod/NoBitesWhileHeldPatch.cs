using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000A2 RID: 162
	[HarmonyPatch(typeof(SpiderHandler), "CheckForLimbDamage")]
	internal static class NoBitesWhileHeldPatch
	{
		// Token: 0x06000322 RID: 802 RVA: 0x000357B8 File Offset: 0x000339B8
		private static bool Prefix(SpiderHandler __instance, Collision2D collision)
		{
			if (!Scenes.Held || collision == null)
			{
				return true;
			}
			Limb limb = Body.LimbFromObject(collision.gameObject, __instance.transform.position);
			return !limb || !PlayerCamera.main || !(limb.body == PlayerCamera.main.body);
		}
	}
}
