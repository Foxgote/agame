using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000CD RID: 205
	[HarmonyPatch(typeof(Body), "FootStep")]
	internal static class OutfitStepPatch
	{
		// Token: 0x0600047F RID: 1151 RVA: 0x00049104 File Offset: 0x00047304
		private static void Postfix(Body __instance, float vol)
		{
			if (!Plugin.SuitSteps.Value || !PlayerCamera.main || PlayerCamera.main.body != __instance)
			{
				return;
			}
			Vector2 vector = __instance.transform.position + Vector2.down * 1.2f;
			bool flag = SlimeFluid.At(WorldGeneration.world ? WorldGeneration.world.WorldToBlockPos(vector) : Vector2Int.zero);
			bool flag2 = Suit.Stage > 0 && !Suit.Drone;
			if (flag || flag2)
			{
				Sfx.Play(__instance, Mathf.Clamp01(vol * 2f) * Plugin.SuitStepVolume.Value * (flag ? 1.2f : (0.6f + 0.1f * (float)Suit.Stage)), new string[] { "step_slime" });
			}
			if (__instance.bodyAffect && __instance.bodyAffect.wasWater)
			{
				return;
			}
			OutfitDef outfitDef = Outfits.Worn(__instance);
			OutfitDef outfitDef2 = Outfits.WornIn(__instance, "feet");
			string text;
			if ((text = ((outfitDef2 != null) ? outfitDef2.Shoes : null)) == null)
			{
				text = ((outfitDef == null) ? null : (outfitDef.Shoes ?? (outfitDef.IsLatex ? "heels_stiletto" : (outfitDef.IsSpacesuit ? "heels_boot" : null))));
			}
			string text2 = text;
			if (text2 == null)
			{
				return;
			}
			Sfx.Play(__instance, Mathf.Clamp01(vol * 2f) * Plugin.SuitStepVolume.Value * (1f - __instance.crouchAmount * 0.6f), new string[] { text2 });
			if (text2 != OutfitStepPatch.lastSet)
			{
				Plugin.Log.LogInfo(string.Concat(new string[]
				{
					"Shoe sounds: ",
					text2,
					" (",
					Sfx.Has(text2) ? "clips loaded" : "NO CLIPS",
					")"
				}));
			}
			OutfitStepPatch.lastSet = text2;
		}

		// Token: 0x0400044D RID: 1101
		private static string lastSet;
	}
}
