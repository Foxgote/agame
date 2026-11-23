using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000090 RID: 144
	[HarmonyPatch(typeof(PlayerCamera), "HandleScreenShaders")]
	internal static class FlowerBlindness
	{
		// Token: 0x060002E3 RID: 739 RVA: 0x00032A8C File Offset: 0x00030C8C
		[HarmonyPostfix]
		[HarmonyPriority(0)]
		private static void Postfix(PlayerCamera __instance)
		{
			if (Flowers.Blind <= 0f || !__instance || !__instance.damagePass)
			{
				return;
			}
			__instance.damagePass.SetFloat("_EyeDamage", Mathf.Max(__instance.damagePass.GetFloat("_EyeDamage"), Flowers.Blind));
		}
	}
}
