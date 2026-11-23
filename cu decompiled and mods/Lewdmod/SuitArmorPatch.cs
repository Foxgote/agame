using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200010F RID: 271
	[HarmonyPatch(typeof(Limb), "GetArmorReduction")]
	internal static class SuitArmorPatch
	{
		// Token: 0x06000664 RID: 1636 RVA: 0x00065F7C File Offset: 0x0006417C
		private static void Postfix(Limb __instance, ref float __result)
		{
			if (Suit.Stage <= 0 || !PlayerCamera.main || __instance.body != PlayerCamera.main.body)
			{
				return;
			}
			SpriteRenderer component = __instance.GetComponent<SpriteRenderer>();
			if (component && component.sprite && component.sprite.texture && Suit.Covers(component.sprite.texture.name, Suit.Stage))
			{
				__result += Suit.Armor[Mathf.Min(Suit.Stage, Suit.Armor.Length - 1)];
			}
		}
	}
}
