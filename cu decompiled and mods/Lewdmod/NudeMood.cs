using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000C9 RID: 201
	internal static class NudeMood
	{
		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600046E RID: 1134 RVA: 0x000482E4 File Offset: 0x000464E4
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x0600046F RID: 1135 RVA: 0x000482EB File Offset: 0x000464EB
		public static int Tier
		{
			get
			{
				if (NudeMood.S.nudeComfort < 30f)
				{
					return 0;
				}
				if (NudeMood.S.nudeComfort >= 70f && NudeMood.S.corruption >= 35f)
				{
					return 2;
				}
				return 1;
			}
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x00048328 File Offset: 0x00046528
		public static NudeMood.Cover Covered(Body b)
		{
			if (!b)
			{
				return NudeMood.Cover.Dressed;
			}
			if (Suit.Stage >= 1)
			{
				return NudeMood.Cover.Dressed;
			}
			bool flag = false;
			foreach (Item item in b.GetAllWearables())
			{
				if (item && item.Stats != null)
				{
					string desiredWearLimb = item.Stats.desiredWearLimb;
					OutfitDef outfitDef = Outfits.ById(item.id);
					if (outfitDef != null && outfitDef.Lingerie)
					{
						flag = true;
					}
					else if ((outfitDef == null || !outfitDef.IsDevice || Devices.Covers(outfitDef.Kind)) && (desiredWearLimb == "UpTorso" || desiredWearLimb == "DownTorso"))
					{
						return NudeMood.Cover.Dressed;
					}
				}
			}
			if (!flag)
			{
				return NudeMood.Cover.Naked;
			}
			return NudeMood.Cover.Underwear;
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x0004840C File Offset: 0x0004660C
		public static void Update(Body b, float dt)
		{
			if (Habit.Paused(b))
			{
				return;
			}
			if ((NudeMood.scan -= dt) <= 0f)
			{
				NudeMood.scan = 0.5f;
				NudeMood.cached = NudeMood.Covered(b);
			}
			NudeMood.Cover cover = NudeMood.cached;
			if (cover == NudeMood.Cover.Dressed)
			{
				NudeMood.S.nudeComfort = Mathf.Max(0f, NudeMood.S.nudeComfort - 0.008f * dt);
				NudeMood.last = cover;
				return;
			}
			float num = ((cover == NudeMood.Cover.Naked) ? 1f : 0.5f);
			if (NudeMood.last == NudeMood.Cover.Dressed)
			{
				NudeMood.lineTimer = 2f;
				Plugin.Log.LogInfo(string.Format("She is {0} (comfort {1:0})", (cover == NudeMood.Cover.Naked) ? "naked" : "in her underwear", NudeMood.S.nudeComfort));
			}
			NudeMood.last = cover;
			float num2 = Plugin.NudeHabitRate.Value * num * (1f + NudeMood.S.corruption / 100f + NudeMood.S.submission / 150f);
			NudeMood.S.nudeComfort = Mathf.Min(100f, NudeMood.S.nudeComfort + num2 * dt);
			int tier = NudeMood.Tier;
			if (tier != 0)
			{
				if (tier == 2)
				{
					Mood.Change(b, 0.012f * dt);
					NudeMood.S.lust = Mathf.Min(100f, NudeMood.S.lust + 0.012f * num * dt);
				}
			}
			else
			{
				Mood.Change(b, -0.03f * num * dt);
				NudeMood.S.arousal = Mathf.Min(100f, NudeMood.S.arousal + 0.004f * num * dt);
			}
			if ((NudeMood.lineTimer -= dt) <= 0f)
			{
				NudeMood.lineTimer = ((NudeMood.Tier == 0) ? Random.Range(45f, 90f) : ((NudeMood.Tier == 1) ? Random.Range(260f, 450f) : Random.Range(200f, 360f)));
				string text = ((cover == NudeMood.Cover.Naked) ? "nude" : "undies");
				Relief.Say(b, text + ((NudeMood.Tier == 0) ? "_shy" : ((NudeMood.Tier == 1) ? "_used" : "_like")));
			}
		}

		// Token: 0x04000436 RID: 1078
		private static float lineTimer = 15f;

		// Token: 0x04000437 RID: 1079
		private static float scan;

		// Token: 0x04000438 RID: 1080
		private static NudeMood.Cover last = NudeMood.Cover.Dressed;

		// Token: 0x04000439 RID: 1081
		private static NudeMood.Cover cached = NudeMood.Cover.Dressed;

		// Token: 0x020001F3 RID: 499
		public enum Cover
		{
			// Token: 0x04000A43 RID: 2627
			Dressed,
			// Token: 0x04000A44 RID: 2628
			Underwear,
			// Token: 0x04000A45 RID: 2629
			Naked
		}
	}
}
