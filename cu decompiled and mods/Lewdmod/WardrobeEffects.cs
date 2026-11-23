using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000CB RID: 203
	internal static class WardrobeEffects
	{
		// Token: 0x06000477 RID: 1143 RVA: 0x00048BB0 File Offset: 0x00046DB0
		private static string LineKey(OutfitDef d, string suffix)
		{
			string text = d.NameKey;
			while (!Plugin.Text.ContainsKey(text + suffix) && !Plugin.Text.ContainsKey(text + suffix + "1") && text.Contains("_"))
			{
				text = text.Substring(0, text.LastIndexOf('_'));
			}
			return text + suffix;
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000478 RID: 1144 RVA: 0x00048C15 File Offset: 0x00046E15
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00048C1C File Offset: 0x00046E1C
		public static void Update(Body b, float dt)
		{
			int num = 0;
			bool flag = false;
			foreach (OutfitDef outfitDef in Outfits.WornAll(b))
			{
				if (outfitDef.Lingerie)
				{
					num++;
				}
				if (outfitDef.Thermal)
				{
					flag = true;
				}
			}
			if ((WardrobeEffects.scan -= dt) <= 0f)
			{
				WardrobeEffects.scan = 0.5f;
				HashSet<string> hashSet = new HashSet<string>(from d in Outfits.WornAll(b)
					select d.Id);
				foreach (string text in hashSet)
				{
					if (!WardrobeEffects.wornBefore.Contains(text))
					{
						OutfitDef outfitDef2 = Outfits.ById(text);
						if (WardrobeEffects.S.wearSaid == null)
						{
							WardrobeEffects.S.wearSaid = new List<string>();
						}
						bool flag2 = !WardrobeEffects.S.wearSaid.Contains(text);
						if (flag2)
						{
							WardrobeEffects.S.wearSaid.Add(text);
						}
						float num2;
						if (flag2 && (!outfitDef2.IsDevice || Time.time - Devices.LastLine >= 2f))
						{
							Relief.Say(b, WardrobeEffects.LineKey(outfitDef2, "_wear"));
						}
						else if (!flag2 && (!outfitDef2.IsDevice || Time.time - Devices.LastLine >= 2f) && WardrobeEffects.rng.NextDouble() < 0.5 && (!WardrobeEffects.rewornAt.TryGetValue(text, out num2) || Time.time - num2 > 30f))
						{
							WardrobeEffects.rewornAt[text] = Time.time;
							string text2 = WardrobeEffects.LineKey(outfitDef2, "_rewear");
							string text3 = ((Plugin.Text.ContainsKey(text2) || Plugin.Text.ContainsKey(text2 + "1")) ? text2 : (outfitDef2.IsDevice ? "dd_rewear" : "outfit_rewear"));
							Relief.Say(b, text3);
							Plugin.Log.LogInfo(string.Concat(new string[] { "On again: ", text, " (", text3, ")" }));
						}
						Plugin.Log.LogInfo("She put on " + text + (flag2 ? "" : " (seen before)"));
					}
				}
				WardrobeEffects.wornBefore.Clear();
				WardrobeEffects.wornBefore.UnionWith(hashSet);
				if ((WardrobeEffects.wornLine -= 0.5f) <= 0f && !Scenes.Busy && !Relief.Active && b.conscious)
				{
					WardrobeEffects.wornLine = Random.Range(110f, 200f);
					List<string> list = (from d in Outfits.WornAll(b)
						select WardrobeEffects.LineKey(d, "_worn") into k
						where Plugin.Text.ContainsKey(k) || Plugin.Text.ContainsKey(k + "1")
						select k).ToList<string>();
					if (list.Count > 0)
					{
						Relief.Say(b, list[Random.Range(0, list.Count)]);
					}
				}
			}
			if (num > 0)
			{
				WardrobeEffects.S.lust = Mathf.Min(100f, WardrobeEffects.S.lust + 0.01f * (float)num * dt);
			}
			if (flag)
			{
				b.temperature = Mathf.MoveTowards(b.temperature, 37f, 0.01f * dt);
			}
			if (WardrobeEffects.Wears(b, Outfits.Wetsuit))
			{
				b.wetness = Mathf.Min(b.wetness, 15f);
				if (b.inWater)
				{
					b.temperature = Mathf.MoveTowards(b.temperature, 37f, 0.02f * dt);
				}
			}
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x0004904C File Offset: 0x0004724C
		private static bool Wears(Body b, OutfitDef d)
		{
			return Outfits.WornAll(b).Contains(d);
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x0004905A File Offset: 0x0004725A
		public static float SightScale(Body b)
		{
			if (!b || !WardrobeEffects.Wears(b, Outfits.SneakingSuit))
			{
				return 1f;
			}
			return 0.7f;
		}

		// Token: 0x04000447 RID: 1095
		private static readonly HashSet<string> wornBefore = new HashSet<string>();

		// Token: 0x04000448 RID: 1096
		private static readonly Dictionary<string, float> rewornAt = new Dictionary<string, float>();

		// Token: 0x04000449 RID: 1097
		private static readonly Random rng = new Random();

		// Token: 0x0400044A RID: 1098
		private static float scan;

		// Token: 0x0400044B RID: 1099
		private static float wornLine = 60f;
	}
}
