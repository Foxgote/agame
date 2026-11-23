using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200006B RID: 107
	internal static class DebugBinds
	{
		// Token: 0x06000207 RID: 519 RVA: 0x000299A8 File Offset: 0x00027BA8
		public static void Init(ConfigFile c)
		{
			DebugBinds.<>c__DisplayClass53_0 CS$<>8__locals1;
			CS$<>8__locals1.c = c;
			DebugBinds.Arousal = DebugBinds.<Init>g__B|53_0("Arousal", 257, 0, "+25 arousal", 0, ref CS$<>8__locals1);
			DebugBinds.Heat = DebugBinds.<Init>g__B|53_0("Heat", 258, 0, "Heat on/off", 0, ref CS$<>8__locals1);
			DebugBinds.Submission = DebugBinds.<Init>g__B|53_0("Submission", 259, 0, "+20 submission", 0, ref CS$<>8__locals1);
			DebugBinds.Corruption = DebugBinds.<Init>g__B|53_0("Corruption", 260, 0, "+20 corruption", 0, ref CS$<>8__locals1);
			DebugBinds.ResetStats = DebugBinds.<Init>g__B|53_0("ResetStats", 261, 0, "Reset all her statuses", 0, ref CS$<>8__locals1);
			DebugBinds.NextBust = DebugBinds.<Init>g__B|53_0("NextBust", 262, 0, "Next bust size", 0, ref CS$<>8__locals1);
			DebugBinds.Fill = DebugBinds.<Init>g__B|53_0("Fill", 263, 0, "+40 filled", 0, ref CS$<>8__locals1);
			DebugBinds.Pregnancy = DebugBinds.<Init>g__B|53_0("Pregnancy", 264, 0, "Start a tentacle brood / +25% of it", 0, ref CS$<>8__locals1);
			DebugBinds.NextSuitStage = DebugBinds.<Init>g__B|53_0("NextSuitStage", 265, 0, "Next stage of the living suit (none on her: living cloth of a random colour)", 0, ref CS$<>8__locals1);
			DebugBinds.Symbiote = DebugBinds.<Init>g__B|53_0("Symbiote", 256, 0, "The symbiote: onto her (no suit on) or one nearby", 0, ref CS$<>8__locals1);
			DebugBinds.SlimeSuit = DebugBinds.<Init>g__B|53_0("SlimeSuit", 267, 0, "The slime suit onto her (no suit on)", 0, ref CS$<>8__locals1);
			DebugBinds.ParasiteNearby = DebugBinds.<Init>g__B|53_0("ParasiteNearby", 266, 0, "A parasite nearby", 0, ref CS$<>8__locals1);
			DebugBinds.SpacesuitAndCum = DebugBinds.<Init>g__B|53_0("SpacesuitAndCum", 268, 0, "Put the spacesuit on her and +40 covered in semen", 0, ref CS$<>8__locals1);
			DebugBinds.LatexOutfit = DebugBinds.<Init>g__B|53_0("LatexOutfit", 271, 0, "Put the latex outfit on her", 0, ref CS$<>8__locals1);
			DebugBinds.NextOutfit = DebugBinds.<Init>g__B|53_0("NextOutfit", 270, 0, "Put the next outfit of the wardrobe on her", 0, ref CS$<>8__locals1);
			DebugBinds.TakeOffOutfit = DebugBinds.<Init>g__B|53_0("TakeOffOutfit", 269, 0, "Take her full suit off", 0, ref CS$<>8__locals1);
			DebugBinds.FleshBiome = DebugBinds.<Init>g__B|53_0("FleshBiome", 257, 306, "A flesh biome round her", 0, ref CS$<>8__locals1);
			DebugBinds.LivingFlesh = DebugBinds.<Init>g__B|53_0("LivingFlesh", 258, 306, "Living flesh nearby", 0, ref CS$<>8__locals1);
			DebugBinds.Tentacles = DebugBinds.<Init>g__B|53_0("Tentacles", 259, 306, "Ceiling tentacles above her", 0, ref CS$<>8__locals1);
			DebugBinds.SlimeBog = DebugBinds.<Init>g__B|53_0("SlimeBog", 260, 306, "A slime bog", 0, ref CS$<>8__locals1);
			DebugBinds.HiddenPit = DebugBinds.<Init>g__B|53_0("HiddenPit", 261, 306, "A hidden pit (maw / slime / carnivorous flower in turn)", 0, ref CS$<>8__locals1);
			DebugBinds.FleshWall = DebugBinds.<Init>g__B|53_0("FleshWall", 262, 306, "A flesh wall", 0, ref CS$<>8__locals1);
			DebugBinds.SlimeWall = DebugBinds.<Init>g__B|53_0("SlimeWall", 263, 306, "A slime wall", 0, ref CS$<>8__locals1);
			DebugBinds.WebBiome = DebugBinds.<Init>g__B|53_0("WebBiome", 264, 306, "A web biome", 0, ref CS$<>8__locals1);
			DebugBinds.WebCocoon = DebugBinds.<Init>g__B|53_0("WebCocoon", 265, 306, "A web on her; again: a cocoon", 0, ref CS$<>8__locals1);
			DebugBinds.GiantWorm = DebugBinds.<Init>g__B|53_0("GiantWorm", 256, 306, "A giant worm hunting her", 0, ref CS$<>8__locals1);
			DebugBinds.GasGeyser = DebugBinds.<Init>g__B|53_0("GasGeyser", 266, 306, "A lust gas geyser", 0, ref CS$<>8__locals1);
			DebugBinds.GasBiome = DebugBinds.<Init>g__B|53_0("GasBiome", 267, 306, "A lust gas biome", 0, ref CS$<>8__locals1);
			DebugBinds.ToysNearby = DebugBinds.<Init>g__B|53_0("ToysNearby", 268, 306, "A toy, a blanket, the suppressant and a scalpel nearby", 0, ref CS$<>8__locals1);
			DebugBinds.RaiderNearby = DebugBinds.<Init>g__B|53_0("RaiderNearby", 269, 306, "A raider nearby", 0, ref CS$<>8__locals1);
			DebugBinds.SlimeHosts = DebugBinds.<Init>g__B|53_0("SlimeHosts", 270, 306, "A pack of slime hosts nearby", 0, ref CS$<>8__locals1);
			DebugBinds.LoadGun = DebugBinds.<Init>g__B|53_0("LoadGun", 271, 306, "Load the gun in her hand", 0, ref CS$<>8__locals1);
			DebugBinds.LiquidLatex = DebugBinds.<Init>g__B|53_0("LiquidLatex", 257, 306, "A liquid latex by her: a crawler, one on the ceiling, a puddle, in turn", 308, ref CS$<>8__locals1);
			DebugBinds.LatexPond = DebugBinds.<Init>g__B|53_0("LatexPond", 258, 306, "A latex pond dug in the floor a few steps ahead of her", 308, ref CS$<>8__locals1);
			DebugBinds.LatexMite = DebugBinds.<Init>g__B|53_0("LatexMite", 259, 306, "A latex mite by her", 308, ref CS$<>8__locals1);
			DebugBinds.LatexMites = DebugBinds.<Init>g__B|53_0("LatexMites", 260, 306, "Two latex mites by her (both at once: she goes down)", 308, ref CS$<>8__locals1);
			DebugBinds.WristCuffs = DebugBinds.<Init>g__B|53_0("WristCuffs", 257, 308, "Wrist cuffs on her (again: off)", 0, ref CS$<>8__locals1);
			DebugBinds.AnkleCuffs = DebugBinds.<Init>g__B|53_0("AnkleCuffs", 258, 308, "Ankle cuffs on her (again: off)", 0, ref CS$<>8__locals1);
			DebugBinds.Gag = DebugBinds.<Init>g__B|53_0("Gag", 259, 308, "A gag on her (again: off)", 0, ref CS$<>8__locals1);
			DebugBinds.Collar = DebugBinds.<Init>g__B|53_0("Collar", 260, 308, "A collar on her (again: off)", 0, ref CS$<>8__locals1);
			DebugBinds.ChastityBelt = DebugBinds.<Init>g__B|53_0("ChastityBelt", 261, 308, "A chastity belt on her (again: off)", 0, ref CS$<>8__locals1);
			DebugBinds.Straitjacket = DebugBinds.<Init>g__B|53_0("Straitjacket", 262, 308, "A straitjacket on her (again: off)", 0, ref CS$<>8__locals1);
			DebugBinds.LockingLatex = DebugBinds.<Init>g__B|53_0("LockingLatex", 269, 308, "The locking latex suit on her (again: off)", 0, ref CS$<>8__locals1);
			DebugBinds.Harness = DebugBinds.<Init>g__B|53_0("Harness", 270, 308, "The leather harness on her (again: off)", 0, ref CS$<>8__locals1);
			DebugBinds.AllRestraintsOff = DebugBinds.<Init>g__B|53_0("AllRestraintsOff", 263, 308, "Every restraint off her", 0, ref CS$<>8__locals1);
			DebugBinds.KeyAndLockpicks = DebugBinds.<Init>g__B|53_0("KeyAndLockpicks", 264, 308, "A restraint key and lockpicks by her", 0, ref CS$<>8__locals1);
			DebugBinds.NextPlug = DebugBinds.<Init>g__B|53_0("NextPlug", 265, 308, "The next plug (plug, tail plug, inflatable, vibrator, none)", 0, ref CS$<>8__locals1);
			DebugBinds.Camp = DebugBinds.<Init>g__B|53_0("Camp", 256, 308, "A neutral camp nearby", 0, ref CS$<>8__locals1);
			DebugBinds.Trader = DebugBinds.<Init>g__B|53_0("Trader", 266, 308, "A trader nearby", 0, ref CS$<>8__locals1);
			DebugBinds.BBSuit = DebugBinds.<Init>g__B|53_0("BBSuit", 267, 308, "The BB Corps suit on her", 0, ref CS$<>8__locals1);
			DebugBinds.SewingKit = DebugBinds.<Init>g__B|53_0("SewingKit", 268, 308, "A sewing kit by her", 0, ref CS$<>8__locals1);
			DebugBinds.SecondChance = DebugBinds.<Init>g__B|53_0("SecondChance", 271, 308, "The game's second chance (frees her)", 0, ref CS$<>8__locals1);
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000208 RID: 520 RVA: 0x0002A041 File Offset: 0x00028241
		private static bool CtrlHeld
		{
			get
			{
				return Input.GetKey(306) || Input.GetKey(305);
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000209 RID: 521 RVA: 0x0002A05B File Offset: 0x0002825B
		private static bool AltHeld
		{
			get
			{
				return Input.GetKey(308) || Input.GetKey(307);
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600020A RID: 522 RVA: 0x0002A075 File Offset: 0x00028275
		private static bool ShiftHeld
		{
			get
			{
				return Input.GetKey(304) || Input.GetKey(303);
			}
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0002A090 File Offset: 0x00028290
		public static bool Down(ConfigEntry<KeyboardShortcut> e)
		{
			if (e == null)
			{
				return false;
			}
			KeyboardShortcut value = e.Value;
			if (value.MainKey == null || !Keys.Down(value.MainKey))
			{
				return false;
			}
			KeyCode[] array = value.Modifiers.ToArray<KeyCode>();
			bool flag = array.Contains(306) || array.Contains(305);
			bool flag2 = array.Contains(308) || array.Contains(307);
			bool flag3 = array.Contains(304) || array.Contains(303);
			if (DebugBinds.CtrlHeld != flag || DebugBinds.AltHeld != flag2 || DebugBinds.ShiftHeld != flag3)
			{
				return false;
			}
			IEnumerable<KeyCode> enumerable = array.Where<KeyCode>((KeyCode m) => m != 306 && m != 305 && m != 308 && m != 307 && m != 304 && m != 303);
			Func<KeyCode, bool> func;
			if ((func = DebugBinds.<>O.<0>__GetKey) == null)
			{
				func = (DebugBinds.<>O.<0>__GetKey = new Func<KeyCode, bool>(Input.GetKey));
			}
			return enumerable.All<KeyCode>(func);
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0002A184 File Offset: 0x00028384
		[CompilerGenerated]
		internal static ConfigEntry<KeyboardShortcut> <Init>g__B|53_0(string name, KeyCode key, KeyCode mod, string what, KeyCode mod2 = 0, ref DebugBinds.<>c__DisplayClass53_0 A_5)
		{
			return A_5.c.Bind<KeyboardShortcut>("DebugKeys", name, (mod == null) ? new KeyboardShortcut(key, Array.Empty<KeyCode>()) : ((mod2 == null) ? new KeyboardShortcut(key, new KeyCode[] { mod }) : new KeyboardShortcut(key, new KeyCode[] { mod, mod2 })), what + " (a debug key: [Debug] DebugKeys on). A key, or a key with modifiers like \"Keypad1 + LeftControl\"; None switches it off.");
		}

		// Token: 0x040001F7 RID: 503
		private const string Section = "DebugKeys";

		// Token: 0x040001F8 RID: 504
		public static ConfigEntry<KeyboardShortcut> Arousal;

		// Token: 0x040001F9 RID: 505
		public static ConfigEntry<KeyboardShortcut> Heat;

		// Token: 0x040001FA RID: 506
		public static ConfigEntry<KeyboardShortcut> Submission;

		// Token: 0x040001FB RID: 507
		public static ConfigEntry<KeyboardShortcut> Corruption;

		// Token: 0x040001FC RID: 508
		public static ConfigEntry<KeyboardShortcut> ResetStats;

		// Token: 0x040001FD RID: 509
		public static ConfigEntry<KeyboardShortcut> NextBust;

		// Token: 0x040001FE RID: 510
		public static ConfigEntry<KeyboardShortcut> Fill;

		// Token: 0x040001FF RID: 511
		public static ConfigEntry<KeyboardShortcut> Pregnancy;

		// Token: 0x04000200 RID: 512
		public static ConfigEntry<KeyboardShortcut> NextSuitStage;

		// Token: 0x04000201 RID: 513
		public static ConfigEntry<KeyboardShortcut> Symbiote;

		// Token: 0x04000202 RID: 514
		public static ConfigEntry<KeyboardShortcut> SlimeSuit;

		// Token: 0x04000203 RID: 515
		public static ConfigEntry<KeyboardShortcut> ParasiteNearby;

		// Token: 0x04000204 RID: 516
		public static ConfigEntry<KeyboardShortcut> SpacesuitAndCum;

		// Token: 0x04000205 RID: 517
		public static ConfigEntry<KeyboardShortcut> LatexOutfit;

		// Token: 0x04000206 RID: 518
		public static ConfigEntry<KeyboardShortcut> NextOutfit;

		// Token: 0x04000207 RID: 519
		public static ConfigEntry<KeyboardShortcut> TakeOffOutfit;

		// Token: 0x04000208 RID: 520
		public static ConfigEntry<KeyboardShortcut> FleshBiome;

		// Token: 0x04000209 RID: 521
		public static ConfigEntry<KeyboardShortcut> LivingFlesh;

		// Token: 0x0400020A RID: 522
		public static ConfigEntry<KeyboardShortcut> Tentacles;

		// Token: 0x0400020B RID: 523
		public static ConfigEntry<KeyboardShortcut> SlimeBog;

		// Token: 0x0400020C RID: 524
		public static ConfigEntry<KeyboardShortcut> HiddenPit;

		// Token: 0x0400020D RID: 525
		public static ConfigEntry<KeyboardShortcut> FleshWall;

		// Token: 0x0400020E RID: 526
		public static ConfigEntry<KeyboardShortcut> SlimeWall;

		// Token: 0x0400020F RID: 527
		public static ConfigEntry<KeyboardShortcut> WebBiome;

		// Token: 0x04000210 RID: 528
		public static ConfigEntry<KeyboardShortcut> WebCocoon;

		// Token: 0x04000211 RID: 529
		public static ConfigEntry<KeyboardShortcut> GiantWorm;

		// Token: 0x04000212 RID: 530
		public static ConfigEntry<KeyboardShortcut> GasGeyser;

		// Token: 0x04000213 RID: 531
		public static ConfigEntry<KeyboardShortcut> GasBiome;

		// Token: 0x04000214 RID: 532
		public static ConfigEntry<KeyboardShortcut> ToysNearby;

		// Token: 0x04000215 RID: 533
		public static ConfigEntry<KeyboardShortcut> RaiderNearby;

		// Token: 0x04000216 RID: 534
		public static ConfigEntry<KeyboardShortcut> SlimeHosts;

		// Token: 0x04000217 RID: 535
		public static ConfigEntry<KeyboardShortcut> LoadGun;

		// Token: 0x04000218 RID: 536
		public static ConfigEntry<KeyboardShortcut> LiquidLatex;

		// Token: 0x04000219 RID: 537
		public static ConfigEntry<KeyboardShortcut> LatexPond;

		// Token: 0x0400021A RID: 538
		public static ConfigEntry<KeyboardShortcut> LatexMite;

		// Token: 0x0400021B RID: 539
		public static ConfigEntry<KeyboardShortcut> LatexMites;

		// Token: 0x0400021C RID: 540
		public static ConfigEntry<KeyboardShortcut> WristCuffs;

		// Token: 0x0400021D RID: 541
		public static ConfigEntry<KeyboardShortcut> AnkleCuffs;

		// Token: 0x0400021E RID: 542
		public static ConfigEntry<KeyboardShortcut> Gag;

		// Token: 0x0400021F RID: 543
		public static ConfigEntry<KeyboardShortcut> Collar;

		// Token: 0x04000220 RID: 544
		public static ConfigEntry<KeyboardShortcut> ChastityBelt;

		// Token: 0x04000221 RID: 545
		public static ConfigEntry<KeyboardShortcut> Straitjacket;

		// Token: 0x04000222 RID: 546
		public static ConfigEntry<KeyboardShortcut> LockingLatex;

		// Token: 0x04000223 RID: 547
		public static ConfigEntry<KeyboardShortcut> Harness;

		// Token: 0x04000224 RID: 548
		public static ConfigEntry<KeyboardShortcut> AllRestraintsOff;

		// Token: 0x04000225 RID: 549
		public static ConfigEntry<KeyboardShortcut> KeyAndLockpicks;

		// Token: 0x04000226 RID: 550
		public static ConfigEntry<KeyboardShortcut> NextPlug;

		// Token: 0x04000227 RID: 551
		public static ConfigEntry<KeyboardShortcut> Camp;

		// Token: 0x04000228 RID: 552
		public static ConfigEntry<KeyboardShortcut> Trader;

		// Token: 0x04000229 RID: 553
		public static ConfigEntry<KeyboardShortcut> BBSuit;

		// Token: 0x0400022A RID: 554
		public static ConfigEntry<KeyboardShortcut> SewingKit;

		// Token: 0x0400022B RID: 555
		public static ConfigEntry<KeyboardShortcut> SecondChance;

		// Token: 0x02000196 RID: 406
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400092A RID: 2346
			public static Func<KeyCode, bool> <0>__GetKey;
		}
	}
}
