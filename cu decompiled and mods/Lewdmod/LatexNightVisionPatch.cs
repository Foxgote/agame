using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x02000113 RID: 275
	[HarmonyPatch(typeof(WorldGeneration), "UpdateAmbientLight")]
	internal static class LatexNightVisionPatch
	{
		// Token: 0x06000668 RID: 1640 RVA: 0x00066370 File Offset: 0x00064570
		private static void Postfix(WorldGeneration __instance)
		{
			Light2D ambientLight = __instance.ambientLight;
			if (!ambientLight)
			{
				return;
			}
			if (LatexNightVisionPatch.own == null)
			{
				LatexNightVisionPatch.own = new Color?(ambientLight.color);
			}
			if (Suit.Latex && Plugin.LatexNightVision.Value > 0f)
			{
				ambientLight.intensity = Mathf.Max(ambientLight.intensity, Plugin.LatexNightVision.Value);
				ambientLight.color = LatexNightVisionPatch.Violet;
				return;
			}
			if (ambientLight.color != LatexNightVisionPatch.own.Value)
			{
				ambientLight.color = LatexNightVisionPatch.own.Value;
			}
		}

		// Token: 0x040006D3 RID: 1747
		private static Color? own;

		// Token: 0x040006D4 RID: 1748
		private static readonly Color Violet = new Color(0.72f, 0.48f, 1f);
	}
}
