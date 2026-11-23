using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000C8 RID: 200
	internal static class LatexMood
	{
		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x0004813F File Offset: 0x0004633F
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00048146 File Offset: 0x00046346
		public static bool Wearing(Body b)
		{
			OutfitDef outfitDef = Outfits.Worn(b);
			return outfitDef != null && outfitDef.IsLatex;
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0004815C File Offset: 0x0004635C
		public static void Update(Body b, float dt)
		{
			if (Habit.Paused(b))
			{
				return;
			}
			if (!LatexMood.Wearing(b))
			{
				LatexMood.S.latexComfort = Mathf.Max(0f, LatexMood.S.latexComfort - 0.01f * dt);
				return;
			}
			float num = Plugin.LatexComfortRate.Value * (1f + LatexMood.S.lust / 200f + LatexMood.S.corruption / 100f);
			LatexMood.S.latexComfort = Mathf.Min(100f, LatexMood.S.latexComfort + num * dt);
			float latexComfort = LatexMood.S.latexComfort;
			if (latexComfort < 30f)
			{
				Mood.Change(b, -0.02f * dt);
			}
			else if (latexComfort >= 70f)
			{
				Mood.Change(b, 0.02f * dt);
				LatexMood.S.lust = Mathf.Min(100f, LatexMood.S.lust + 0.02f * dt);
			}
			if ((LatexMood.lineTimer -= dt) <= 0f)
			{
				LatexMood.lineTimer = ((latexComfort < 30f) ? Random.Range(70f, 140f) : ((latexComfort < 70f) ? Random.Range(240f, 420f) : Random.Range(180f, 330f)));
				string text = ((latexComfort < 30f) ? "latex_dislike" : ((latexComfort < 70f) ? "latex_usedto" : "latex_like"));
				Relief.Say(b, text);
			}
		}

		// Token: 0x04000435 RID: 1077
		private static float lineTimer = 20f;
	}
}
