using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000DE RID: 222
	[HarmonyPatch(typeof(MoodleManager), "AddAllMoodles")]
	internal static class MoodlesPatch
	{
		// Token: 0x06000516 RID: 1302 RVA: 0x00054D4C File Offset: 0x00052F4C
		private static void Postfix(MoodleManager __instance)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body == null || !body.alive)
			{
				return;
			}
			State s = Plugin.S;
			bool sideMoodles = __instance.sideMoodles;
			try
			{
				MoodlesPatch.AddAll(__instance, s);
			}
			finally
			{
				__instance.sideMoodles = sideMoodles;
			}
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x00054DB4 File Offset: 0x00052FB4
		private static bool S_exh(State s)
		{
			return s.exhaustion >= 60f;
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x00054DC8 File Offset: 0x00052FC8
		private static bool MainRow(string icon, bool critical)
		{
			return critical || !Plugin.MoodlesCollapse.Value || (icon.StartsWith("lewd_suit_") && icon != "lewd_suit_latex" && (Plugin.MoodlesSuitsMain.Value || Suit.Alarming)) || icon == "lewd_tentacle" || icon == "lewd_worm" || icon == "lewd_web" || icon == "lewd_flesh" || icon == "lewd_slime" || icon == "lewd_gas";
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x00054E62 File Offset: 0x00053062
		private static void Put(MoodleManager m, int intensity, string icon, string name, string desc, bool critical = false)
		{
			if (!LewdSettings.On && MoodlesPatch.BaseIcons.Contains(icon))
			{
				return;
			}
			m.sideMoodles = !MoodlesPatch.MainRow(icon, critical);
			m.AddMoodle(intensity, icon, name, desc, critical, false);
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x00054E98 File Offset: 0x00053098
		private static void AddAll(MoodleManager __instance, State s)
		{
			MoodlesPatch.<>c__DisplayClass5_0 CS$<>8__locals1;
			CS$<>8__locals1.__instance = __instance;
			if (s.suitStage > 0)
			{
				string text = "lewd_suit_" + ParasiteArt.ColorNames[Mathf.Clamp(s.suitColor, 0, ParasiteArt.ColorNames.Length - 1)];
				int num = ((s.suitStage >= 4) ? ((Suit.Slime && !Suit.HostSlime && s.suitStage >= 5) ? 5 : 8) : s.suitStage);
				int num2 = Mathf.Min(s.suitStage, Suit.Armor.Length - 1);
				string text2 = Plugin.T(Suit.Key + s.suitStage.ToString() + "dsc").Replace("<p>", (s.suitStage < 4 || (Suit.HostSlime && s.suitStage == 4)) ? Mathf.FloorToInt(s.suitProgress).ToString() : "100").Replace("<a>", Mathf.RoundToInt(Suit.Armor[num2] * 100f).ToString())
					.Replace("<r>", Mathf.RoundToInt(Suit.RemoveChance[num2] * 100f).ToString())
					.Replace("<k>", Plugin.RemoveKey.Value.ToString())
					.Replace("<w>", Mathf.FloorToInt(s.willpower).ToString())
					.Replace("<n>", Mathf.FloorToInt(s.droneNeed).ToString());
				if (CS$<>8__locals1.__instance.icons.ContainsKey(text))
				{
					MoodlesPatch.Put(CS$<>8__locals1.__instance, num, text, Plugin.T(Suit.Key + s.suitStage.ToString()), text2, s.suitStage >= 4 && !Suit.Symbiote && (!Suit.Slime || Suit.HostSlime));
				}
			}
			if (s.milkTimer > 0f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_milk", "milking", false, ref CS$<>8__locals1);
			}
			if (PlayerCamera.main && LatexMood.Wearing(PlayerCamera.main.body))
			{
				if (s.latexComfort >= 70f)
				{
					MoodlesPatch.<AddAll>g__Add|5_0(5, "lewd_suit_latex", "latex3", false, ref CS$<>8__locals1);
				}
				else if (s.latexComfort >= 30f)
				{
					MoodlesPatch.<AddAll>g__Add|5_0(0, "lewd_suit_latex", "latex2", false, ref CS$<>8__locals1);
				}
				else
				{
					MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_suit_latex", "latex1", false, ref CS$<>8__locals1);
				}
			}
			NudeMood.Cover cover = NudeMood.Covered(PlayerCamera.main ? PlayerCamera.main.body : null);
			if (cover != NudeMood.Cover.Dressed)
			{
				string text3 = ((cover == NudeMood.Cover.Naked) ? "nude" : "undies");
				int tier = NudeMood.Tier;
				MoodlesPatch.<AddAll>g__Add|5_0((tier == 0) ? ((cover == NudeMood.Cover.Naked) ? 2 : 1) : ((tier == 1) ? 0 : 5), "lewd_nude", text3 + (tier + 1).ToString(), false, ref CS$<>8__locals1);
			}
			if (s.cum > 70f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_milk", "cum3", false, ref CS$<>8__locals1);
			}
			else if (s.cum > 35f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_milk", "cum2", false, ref CS$<>8__locals1);
			}
			else if (s.cum > 5f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(0, "lewd_milk", "cum1", false, ref CS$<>8__locals1);
			}
			if (Worm.Inside)
			{
				MoodlesPatch.Put(CS$<>8__locals1.__instance, 8, "lewd_worm", Plugin.T(Worm.Tentacled ? "worm_scene_m" : "worm_digest_m"), Plugin.T(Worm.Tentacled ? "worm_scene_mdsc" : "worm_digest_mdsc").Replace("<h>", Mathf.CeilToInt(Mathf.Max(0f, Worm.InnerHealth)).ToString()), true);
			}
			else if (Worm.Hunting)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(3, "lewd_worm", "worm_hunt", true, ref CS$<>8__locals1);
			}
			if (Tentacles.Holding)
			{
				MoodlesPatch.<AddAll>g__Add|5_0((Tentacles.Insatiable && MoodlesPatch.S_exh(s)) ? 3 : 8, "lewd_tentacle", Tentacles.Insatiable ? "tent_held_insatiable" : "tent_held", true, ref CS$<>8__locals1);
			}
			if (Flowers.Holding)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(8, "lewd_tentacle", "flower_hold", true, ref CS$<>8__locals1);
			}
			if (Slits.Holding)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(8, "lewd_tentacle", "slit_hold", true, ref CS$<>8__locals1);
			}
			if (s.exhaustion >= 85f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(3, "lewd_exhausted", "exhaust3", true, ref CS$<>8__locals1);
			}
			else if (s.exhaustion >= 50f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_exhausted", "exhaust2", false, ref CS$<>8__locals1);
			}
			else if (s.exhaustion >= 20f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_exhausted", "exhaust1", false, ref CS$<>8__locals1);
			}
			if (Womb.InLabor)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(3, "lewd_pregnant", "preg_labor_m", true, ref CS$<>8__locals1);
			}
			else if (s.pregnant && Womb.PregStage == 3)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_pregnant", "preg3_m", false, ref CS$<>8__locals1);
			}
			else if (s.pregnant && Womb.PregStage == 2)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_pregnant", "preg2_m", false, ref CS$<>8__locals1);
			}
			else if (s.pregnant)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(0, "lewd_pregnant", "preg1_m", false, ref CS$<>8__locals1);
			}
			if (s.filled > 70f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_filled", "filled3", false, ref CS$<>8__locals1);
			}
			else if (s.filled > 40f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_filled", "filled2", false, ref CS$<>8__locals1);
			}
			else if (s.filled > 10f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(0, "lewd_filled", "filled1", false, ref CS$<>8__locals1);
			}
			if (SlimeBog.Under)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(3, "lewd_slime", "slime_drowning", true, ref CS$<>8__locals1);
			}
			else if (SlimeBog.Stuck)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_slime", "slime_trapped", true, ref CS$<>8__locals1);
			}
			if (FleshZones.OnFlesh && !Flesh.Holding)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_flesh", "flesh_slow", false, ref CS$<>8__locals1);
			}
			if (Suppressant.Active)
			{
				MoodlesPatch.Put(CS$<>8__locals1.__instance, 0, "lewd_satisfied", Plugin.T("supp_m"), Plugin.T("supp_mdsc").Replace("<m>", Mathf.CeilToInt(s.suppressTimer / 60f).ToString()), false);
			}
			if (SuitCut.Active)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(3, "lewd_suit_pink", "cut_m", true, ref CS$<>8__locals1);
			}
			if (CS$<>8__locals1.__instance.icons.ContainsKey("lewd_plug") && PlayerCamera.main)
			{
				ToyDef toyDef = Plugs.Worn(PlayerCamera.main.body);
				if (toyDef != null)
				{
					MoodlesPatch.Put(CS$<>8__locals1.__instance, (toyDef.Size == "l") ? 2 : 1, "lewd_plug", Plugin.T("plug_m"), Plugin.T("plug_mdsc").Replace("<n>", Toys.Name(toyDef)), false);
				}
			}
			if (CS$<>8__locals1.__instance.icons.ContainsKey("lewd_plug") && PlayerCamera.main)
			{
				ToyDef toyDef2 = Plugs.Vibe(PlayerCamera.main.body);
				if (toyDef2 != null)
				{
					MoodlesPatch.Put(CS$<>8__locals1.__instance, 2, "lewd_plug", Plugin.T("vibe_m"), Plugin.T("vibe_mdsc").Replace("<n>", Toys.Name(toyDef2)), false);
				}
			}
			if (Devices.On && CS$<>8__locals1.__instance.icons.ContainsKey("lewd_dd") && PlayerCamera.main && Devices.HandsBound(PlayerCamera.main.body))
			{
				MoodlesPatch.Put(CS$<>8__locals1.__instance, 3, "lewd_dd", Plugin.T(Devices.Worn(PlayerCamera.main.body, Devices.Kind.Jacket) ? "dd_bound_jacket_m" : "dd_bound_m"), Plugin.T("dd_bound_mdsc").Replace("<k>", Plugin.RemoveKey.Value.ToString()), true);
			}
			if (Devices.On && CS$<>8__locals1.__instance.icons.ContainsKey("lewd_dd") && PlayerCamera.main && PlayerCamera.main.body)
			{
				List<Item> list = PlayerCamera.main.body.GetAllWearables().Where<Item>(delegate(Item w)
				{
					if (w)
					{
						OutfitDef outfitDef = Devices.ById(w.id);
						if (outfitDef != null && outfitDef.IsDevice)
						{
							return outfitDef.Locks;
						}
					}
					return false;
				}).ToList<Item>();
				if (list.Count > 0)
				{
					string text4 = string.Join(", ", list.Select<Item, string>((Item w) => Devices.ById(w.id).Name + (Devices.IsLocked(w) ? Plugin.T("dd_m_locked") : "")));
					MoodleManager _instance = CS$<>8__locals1.__instance;
					int num3 = Mathf.Min(3, list.Count);
					string text5 = "lewd_dd";
					string text6 = Plugin.T("dd_m");
					string text7 = Plugin.T("dd_mdsc").Replace("<l>", text4).Replace("<k>", Plugin.RemoveKey.Value.ToString());
					IEnumerable<Item> enumerable = list;
					Func<Item, bool> func;
					if ((func = MoodlesPatch.<>O.<0>__IsLocked) == null)
					{
						func = (MoodlesPatch.<>O.<0>__IsLocked = new Func<Item, bool>(Devices.IsLocked));
					}
					MoodlesPatch.Put(_instance, num3, text5, text6, text7, enumerable.Any<Item>(func));
				}
			}
			if (LustGas.Exposure > 0.5f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_gas", "gas_thick_m", false, ref CS$<>8__locals1);
			}
			else if (LustGas.Exposure > 0.05f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_gas", "gas_m", false, ref CS$<>8__locals1);
			}
			if (Webs.Suffocating)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(3, "lewd_web", "cocoon_choking", true, ref CS$<>8__locals1);
			}
			else if (Webs.InCocoon)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(3, "lewd_web", "cocoon_m", true, ref CS$<>8__locals1);
			}
			else if (Webs.Caught)
			{
				MoodlesPatch.Put(CS$<>8__locals1.__instance, 3, "lewd_web", Plugin.T("web_caught_m"), Plugin.T("web_caught_mdsc").Replace("<t>", Mathf.RoundToInt(Webs.Tangle).ToString()).Replace("<k>", Plugin.RemoveKey.Value.ToString()), Webs.Tangle > 50f);
			}
			else if (Webs.OnSilk)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_web", "web_slow", false, ref CS$<>8__locals1);
			}
			if (FleshWall.Holding)
			{
				MoodlesPatch.Put(CS$<>8__locals1.__instance, 8, "lewd_flesh", Plugin.T(FleshWall.Choking ? "fwall_choke_m" : "fwall_m"), Plugin.T("fwall_mdsc").Replace("<p>", Mathf.RoundToInt(FleshWall.Progress * 100f).ToString()).Replace("<o>", FleshWall.Orgasms.ToString())
					.Replace("<k>", Plugin.RemoveKey.Value.ToString()), true);
			}
			if (Pits.InMaw)
			{
				MoodlesPatch.Put(CS$<>8__locals1.__instance, 8, "lewd_flesh", Plugin.T(Pits.Ravishing ? "pit_rm" : "pit_m"), Plugin.T(Pits.Ravishing ? "pit_rmdsc" : "pit_mdsc").Replace("<o>", Pits.Orgasms.ToString()).Replace("<k>", Plugin.RemoveKey.Value.ToString()), true);
			}
			if (Slits.Inside)
			{
				MoodlesPatch.Put(CS$<>8__locals1.__instance, 8, "lewd_flesh", Plugin.T(Slits.Flooding ? "slit_flood_m" : (Slits.Ravishing ? "pit_rm" : "slit_m")), Plugin.T(Slits.Flooding ? (Slits.Drowning ? "slit_drown_mdsc" : "slit_flood_mdsc") : (Slits.Ravishing ? "pit_rmdsc" : "slit_mdsc")).Replace("<o>", Slits.Orgasms.ToString()).Replace("<k>", Plugin.RemoveKey.Value.ToString()), true);
			}
			if (Flesh.InScene)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(8, "lewd_flesh", "flesh_scene", true, ref CS$<>8__locals1);
			}
			else if (Flesh.Digesting)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(3, "lewd_flesh", "flesh_digest", true, ref CS$<>8__locals1);
			}
			else if (Flesh.Holding)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(3, "lewd_flesh", "flesh_grabbed", true, ref CS$<>8__locals1);
			}
			if (Relief.Active)
			{
				MoodlesPatch.Put(CS$<>8__locals1.__instance, 6, "lewd_relief", Plugin.T("relief"), Plugin.T("reliefdsc").Replace("<>", Mathf.RoundToInt(s.arousal).ToString()), false);
			}
			if (s.satisfiedTimer > 0f && s.orgasmTimer <= 0f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(5, "lewd_satisfied", "satisfied", false, ref CS$<>8__locals1);
			}
			if (s.frustrationTimer > 0f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_arousal", "frustrated", false, ref CS$<>8__locals1);
			}
			if (s.orgasmTimer > 0f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0((s.orgasmTimer > 17f) ? 8 : 7, "lewd_orgasm", (s.orgasmTimer > 17f) ? "orgasm" : "afterglow", s.orgasmTimer > 17f, ref CS$<>8__locals1);
			}
			if (s.arousal >= 90f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(3, "lewd_arousal", "arousal4", true, ref CS$<>8__locals1);
			}
			else if (s.arousal >= 70f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_arousal", "arousal3", false, ref CS$<>8__locals1);
			}
			else if (s.arousal >= 45f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_arousal", "arousal2", false, ref CS$<>8__locals1);
			}
			else if (s.arousal >= 25f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(0, "lewd_arousal", "arousal1", false, ref CS$<>8__locals1);
			}
			if (s.inHeat)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_heat", "heat", false, ref CS$<>8__locals1);
			}
			if (s.lust >= 85f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_lust", "lust3", false, ref CS$<>8__locals1);
			}
			else if (s.lust >= 60f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_lust", "lust2", false, ref CS$<>8__locals1);
			}
			else if (s.lust >= 30f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(0, "lewd_lust", "lust1", false, ref CS$<>8__locals1);
			}
			if (s.submission >= 75f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_submission", "submission3", false, ref CS$<>8__locals1);
			}
			else if (s.submission >= 50f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(1, "lewd_submission", "submission2", false, ref CS$<>8__locals1);
			}
			else if (s.submission >= 25f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(0, "lewd_submission", "submission1", false, ref CS$<>8__locals1);
			}
			if (s.corruption >= 80f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(3, "lewd_corruption", "corruption3", false, ref CS$<>8__locals1);
				return;
			}
			if (s.corruption >= 50f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(2, "lewd_corruption", "corruption2", false, ref CS$<>8__locals1);
				return;
			}
			if (s.corruption >= 20f)
			{
				MoodlesPatch.<AddAll>g__Add|5_0(0, "lewd_corruption", "corruption1", false, ref CS$<>8__locals1);
			}
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x00055E8B File Offset: 0x0005408B
		[CompilerGenerated]
		internal static void <AddAll>g__Add|5_0(int intensity, string icon, string key, bool critical = false, ref MoodlesPatch.<>c__DisplayClass5_0 A_4)
		{
			if (A_4.__instance.icons.ContainsKey(icon))
			{
				MoodlesPatch.Put(A_4.__instance, intensity, icon, Plugin.T(key), Plugin.T(key + "dsc"), critical);
			}
		}

		// Token: 0x04000611 RID: 1553
		private static readonly HashSet<string> BaseIcons = new HashSet<string>
		{
			"lewd_arousal", "lewd_lust", "lewd_heat", "lewd_filled", "lewd_milk", "lewd_pregnant", "lewd_exhausted", "lewd_satisfied", "lewd_orgasm", "lewd_relief",
			"lewd_nude", "lewd_corruption", "lewd_submission", "lewd_pleasure", "lewd_frustrated"
		};

		// Token: 0x0200020B RID: 523
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000A71 RID: 2673
			public static Func<Item, bool> <0>__IsLocked;
		}
	}
}
