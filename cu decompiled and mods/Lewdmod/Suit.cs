using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x0200010E RID: 270
	internal static class Suit
	{
		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000621 RID: 1569 RVA: 0x00062080 File Offset: 0x00060280
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000622 RID: 1570 RVA: 0x00062087 File Offset: 0x00060287
		public static int Stage
		{
			get
			{
				return Suit.S.suitStage;
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000623 RID: 1571 RVA: 0x00062093 File Offset: 0x00060293
		public static bool CoolingDown
		{
			get
			{
				return Suit.S.suitCooldown > 0f;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000624 RID: 1572 RVA: 0x000620A6 File Offset: 0x000602A6
		public static bool Milking
		{
			get
			{
				return Suit.S.milkTimer > 0f;
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000625 RID: 1573 RVA: 0x000620BC File Offset: 0x000602BC
		public static bool Enjoying
		{
			get
			{
				return Suit.Stage >= 2 && !Suit.Drone && ((Suit.Slime && Suit.Stage >= 5) || Suit.S.suitWorn * (1f + Suit.S.corruption / 100f + Suit.S.submission / 200f) >= Plugin.SuitEnjoyMinutes.Value * 60f);
			}
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x00062132 File Offset: 0x00060332
		private static string Love(string what)
		{
			return (Suit.Slime ? "slimelove_" : "suitlove_") + what;
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x00062150 File Offset: 0x00060350
		private static void UpdateLove(Body b, float dt)
		{
			if (Habit.Paused(b))
			{
				return;
			}
			Suit.S.suitWorn += dt;
			if (!Suit.Enjoying)
			{
				return;
			}
			if (!Suit.S.suitLoved)
			{
				Suit.S.suitLoved = true;
				Suit.loveLine = Random.Range(40f, 70f);
				Relief.Say(b, Suit.Love("first"));
				Plugin.Log.LogInfo(string.Format("She has come to like the {0} suit ({1:0.0} min)", Suit.Key, Suit.S.suitWorn / 60f));
				return;
			}
			if ((Suit.loveLine -= dt) <= 0f)
			{
				Suit.loveLine = Random.Range(150f, 280f);
				if (!Suit.Milking)
				{
					Relief.Say(b, Suit.Love("idle"));
				}
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000628 RID: 1576 RVA: 0x00062229 File Offset: 0x00060429
		public static bool Symbiote
		{
			get
			{
				return Suit.S.suitColor == 3;
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000629 RID: 1577 RVA: 0x00062238 File Offset: 0x00060438
		public static bool Symbiosis
		{
			get
			{
				return Suit.Symbiote && Suit.Stage >= 5;
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600062A RID: 1578 RVA: 0x0006224E File Offset: 0x0006044E
		public static bool Drone
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x0600062B RID: 1579 RVA: 0x00062251 File Offset: 0x00060451
		public static bool Slime
		{
			get
			{
				return Suit.S.suitColor == 6 || Suit.S.suitColor == 7;
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x0600062C RID: 1580 RVA: 0x0006226F File Offset: 0x0006046F
		public static bool HostSlime
		{
			get
			{
				return Suit.S.suitColor == 7;
			}
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x0600062D RID: 1581 RVA: 0x0006227E File Offset: 0x0006047E
		public static bool DroneMerged
		{
			get
			{
				return Suit.Drone && Suit.Stage >= 5;
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x0600062E RID: 1582 RVA: 0x00062294 File Offset: 0x00060494
		public static bool Latex
		{
			get
			{
				return Suit.S.suitColor == 8 && Suit.Stage >= 5;
			}
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600062F RID: 1583 RVA: 0x000622B0 File Offset: 0x000604B0
		public static bool LatexPartial
		{
			get
			{
				return Suit.S.suitColor == 8 && Suit.Stage > 0 && Suit.Stage < 5;
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000630 RID: 1584 RVA: 0x000622D4 File Offset: 0x000604D4
		internal static Suit.Look Mine
		{
			get
			{
				Suit.Look look;
				if (Suit.Stage != 0 || Latexes.CreepStage <= 0)
				{
					look = new Suit.Look
					{
						stage = Suit.Stage,
						color = Suit.S.suitColor,
						milking = Suit.Milking,
						piece = (Suit.LatexPartial ? Suit.S.latexPiece : 0)
					};
					return look;
				}
				look = new Suit.Look
				{
					stage = Latexes.CreepStage,
					color = 8
				};
				return look;
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000631 RID: 1585 RVA: 0x0006235C File Offset: 0x0006055C
		public static string Key
		{
			get
			{
				if (Suit.Latex)
				{
					return "latex";
				}
				if (Suit.LatexPartial)
				{
					return "latexp";
				}
				if (Suit.Symbiote)
				{
					return "symb";
				}
				if (Suit.Drone)
				{
					return "drone";
				}
				if (Suit.HostSlime)
				{
					return "hslime";
				}
				if (!Suit.Slime)
				{
					return "suit";
				}
				return "slime";
			}
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000632 RID: 1586 RVA: 0x000623BC File Offset: 0x000605BC
		public static float[] Armor
		{
			get
			{
				if (Suit.Drone)
				{
					return Suit.ArmorDrone;
				}
				if (Suit.Latex)
				{
					return Suit.ArmorLatex;
				}
				if (Suit.LatexPartial)
				{
					return Suit.ArmorLatexPiece;
				}
				if (Suit.Symbiote)
				{
					return Suit.ArmorSymb;
				}
				if (Suit.HostTamed)
				{
					return Suit.ArmorCloth;
				}
				if (Suit.Slime)
				{
					return Suit.ArmorSlime;
				}
				if (!Suit.Tamed)
				{
					return Suit.ArmorCloth;
				}
				return Suit.ArmorTamed;
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000633 RID: 1587 RVA: 0x00062429 File Offset: 0x00060629
		public static bool Tamed
		{
			get
			{
				return Suit.S.suitTamed && Suit.Stage > 0 && !Suit.Symbiote && !Suit.Slime;
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000634 RID: 1588 RVA: 0x00062450 File Offset: 0x00060650
		public static bool HostTamed
		{
			get
			{
				return Suit.S.hostTamed && Suit.HostSlime && Suit.Stage > 0;
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000635 RID: 1589 RVA: 0x0006246F File Offset: 0x0006066F
		public static bool Obeys
		{
			get
			{
				return Suit.Tamed || Suit.HostTamed || (Suit.Slime && !Suit.HostSlime && Suit.Stage >= 5) || Suit.Symbiosis;
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000636 RID: 1590 RVA: 0x0006249C File Offset: 0x0006069C
		public static bool HostCalm
		{
			get
			{
				return Suit.HostSlime && Suit.Stage > 0 && (Suit.S.hostTamed || Suit.S.hostStableLeft > 0f);
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000637 RID: 1591 RVA: 0x000624D0 File Offset: 0x000606D0
		public static bool Alarming
		{
			get
			{
				return Suit.Stage > 0 && (Puppet.Active || (Suit.HostSlime && !Suit.HostCalm) || (!Suit.Symbiote && !Suit.Slime && !Suit.Tamed && Suit.Stage >= 4));
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000638 RID: 1592 RVA: 0x00062520 File Offset: 0x00060720
		public static float Touch
		{
			get
			{
				return Mathf.Max(0.05f, Plugin.TouchScale.Value) * ((Suit.Tamed || Suit.HostTamed) ? 0.5f : 1f);
			}
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00062554 File Offset: 0x00060754
		private static void ForgetBond()
		{
			Suit.S.takeoverEscapes = 0;
			Suit.S.takeoverRetry = false;
			Suit.S.suitTamed = false;
			Suit.S.symbClothesOk = false;
			Suit.S.symbClothesNo = 0;
			Suit.S.hostStableLeft = 0f;
			Suit.S.hostDoses = 0;
			Suit.S.hostTamed = false;
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x0600063A RID: 1594 RVA: 0x000625C0 File Offset: 0x000607C0
		public static float[] RemoveChance
		{
			get
			{
				if (Suit.LatexPartial)
				{
					return Suit.RemoveLatexPiece;
				}
				if (!Suit.Latex)
				{
					if (Suit.Drone)
					{
						return Suit.RemoveDrone;
					}
					if (Suit.Symbiote)
					{
						return Suit.RemoveSymb;
					}
					if (!Suit.Slime)
					{
						return Suit.RemoveCloth;
					}
					return Suit.RemoveSlime;
				}
				else
				{
					if (Suit.S.latexLooseLeft <= 0f)
					{
						return Suit.RemoveLatex;
					}
					return new float[]
					{
						0f,
						0f,
						0f,
						0f,
						0f,
						Plugin.LatexRemoveLoose.Value
					};
				}
			}
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x00062638 File Offset: 0x00060838
		public static float AttachChance(Body b)
		{
			if (!b.conscious || b.sleeping)
			{
				return 0.95f;
			}
			float num = 0.4f + Suit.S.corruption * 0.002f + Suit.S.submission * 0.002f;
			if (b.limbs[1].GetLimbWearables().Any<Item>())
			{
				num -= 0.15f;
			}
			return Mathf.Clamp(num, 0.05f, 0.9f);
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x000626B0 File Offset: 0x000608B0
		public static void Attach(Body b, int color)
		{
			if (!LewdSettings.ParasitesOn)
			{
				return;
			}
			Suit.S.suitWorn = 0f;
			Suit.S.suitLoved = false;
			Suit.ForgetBond();
			Suit.S.suitStage = 1;
			Suit.S.suitProgress = 0f;
			Suit.S.suitColor = color;
			Suit.fadeIn = 0f;
			Suit.S.arousal = Mathf.Min(100f, Suit.S.arousal + 15f);
			b.eyePanicTime = 0.8f;
			b.adrenaline += 20f;
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.8f, false, false);
			if (!Suit.Drone)
			{
				Sfx.PlayMix(b, 0.8f, new string[] { "slime_gloop", "wet_slap" });
			}
			Relief.Say(b, Suit.Key + "_attach");
			InvasionPopup.SuitStage(b, 1);
			BioWarning.Show();
			Plugin.Log.LogInfo("Suit attached (" + ParasiteArt.ColorNames[color] + ")");
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x000627EC File Offset: 0x000609EC
		public static void AttachLatex(Body b)
		{
			Suit.S.suitWorn = 0f;
			Suit.S.suitLoved = false;
			Suit.ForgetBond();
			Suit.S.suitStage = 5;
			Suit.S.suitProgress = 0f;
			Suit.S.suitColor = 8;
			Suit.S.latexLooseLeft = 0f;
			Suit.S.milkTimer = 0f;
			Suit.fadeIn = 0f;
			Suit.S.arousal = Mathf.Min(100f, Suit.S.arousal + 20f);
			Suit.S.submission = Mathf.Min(100f, Suit.S.submission + 8f);
			b.eyePanicTime = 0.8f;
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.6f, false, false);
			Sfx.PlayMix(b, 0.9f, new string[] { "slime_gloop", "wet_slap" });
			Sfx.Play(b, 0.8f, new string[] { "muffled", "moan" });
			Relief.Say(b, "latex_attach");
			InvasionPopup.SuitStage(b, 4);
			Suit.thoughtLine = Random.Range(40f, 70f);
			Plugin.Log.LogInfo("The living latex is on her (stage 5)");
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x00062960 File Offset: 0x00060B60
		internal static void BeginLatexPiece(Body b, int piece, int stage)
		{
			Suit.S.suitWorn = 0f;
			Suit.S.suitLoved = false;
			Suit.ForgetBond();
			Suit.S.suitStage = Mathf.Clamp(stage, 1, 4);
			Suit.S.suitProgress = 0f;
			Suit.S.suitColor = 8;
			Suit.S.latexPiece = piece;
			Suit.S.latexLooseLeft = 0f;
			Suit.S.milkTimer = 0f;
			Suit.fadeIn = 0f;
			b.eyePanicTime = 0.8f;
			Suit.S.arousal = Mathf.Min(100f, Suit.S.arousal + 10f);
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x00062A1C File Offset: 0x00060C1C
		internal static void LatexPieceStage(Body b, int stage)
		{
			Suit.S.suitStage = Mathf.Clamp(stage, 1, 4);
			Suit.fadeIn = 0f;
			b.eyePanicTime = 0.6f;
			Suit.S.arousal = Mathf.Min(100f, Suit.S.arousal + 8f);
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.7f, false, false);
			Sfx.PlayMix(b, 0.75f, new string[] { "slime_slurp", "slime_gloop" });
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x00062AC4 File Offset: 0x00060CC4
		public static void Strip(Body b)
		{
			if (Suit.Stage == 0)
			{
				return;
			}
			Suit.S.latexPiece = 0;
			Suit.S.latexSpread = 0f;
			Suit.S.suitWorn = 0f;
			Suit.S.suitLoved = false;
			Suit.ForgetBond();
			Suit.S.suitStage = 0;
			Suit.S.suitProgress = 0f;
			Suit.S.slimeBond = 0f;
			Suit.S.milkTimer = 0f;
			Suit.ReleaseControl(b);
			Suit.Update(b, 0f);
			foreach (SpriteRenderer spriteRenderer in b.GetComponentsInChildren<SpriteRenderer>(true))
			{
				if (spriteRenderer.gameObject.name == "LewdSuitLens")
				{
					spriteRenderer.enabled = false;
				}
			}
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x00062B94 File Offset: 0x00060D94
		public static void TryRemove(Body b)
		{
			if (Suit.Stage == 0 || Scenes.Held)
			{
				return;
			}
			if (Puppet.Active)
			{
				Puppet.Struggle(b);
				return;
			}
			if (Suit.Slime && Suit.Stage >= 5)
			{
				Relief.Say(b, "slime_keep");
				return;
			}
			if (SuitCut.TryStart(b))
			{
				return;
			}
			if (Suit.S.removeCooldown > 0f)
			{
				Relief.Say(b, "suit_resist_wait");
				return;
			}
			if (b.bothHandsUnusable || !b.conscious)
			{
				Relief.Say(b, "suit_nohands");
				return;
			}
			Suit.S.removeCooldown = 8f;
			b.stamina = Mathf.Max(0f, b.stamina - 20f);
			float num = Suit.RemoveChance[Suit.Stage] * Diff.Escape * (1f - Suit.S.submission * 0.004f) * (1f - Suit.S.corruption * 0.004f) * ((b.stamina < 20f) ? 0.5f : 1f);
			bool flag = Random.value < num;
			Limb limb = b.limbs[1];
			Plugin.Log.LogInfo(string.Format("Tearing the suit off at stage {0}: chance {1:P0}, {2}", Suit.Stage, num, flag ? "success" : "failed"));
			if (flag)
			{
				limb.pain += 15f + 8f * (float)Suit.Stage;
				limb.skinHealth -= 6f * (float)Suit.Stage;
				limb.bleedAmount += 2f * (float)Suit.Stage;
				Sound.Play("ripclothing", b.transform.position, false, true, b.transform, 1f, 1f, false, false);
				Sfx.PlayMix(b, 0.9f, new string[] { "wet_slap", "slime_gloop" });
				Vector2 vector = (b.isRight ? Vector2.left : Vector2.right) * 1.5f;
				if (Suit.Slime)
				{
					SlimeBog.DropPuddle(b);
				}
				else if (!Suit.Latex)
				{
					Parasites.Spawn(b.transform.position + vector, Suit.S.suitColor, true);
				}
				string text = Suit.Key + "_removed";
				Suit.S.latexPiece = 0;
				Suit.S.latexSpread = 0f;
				Suit.S.suitStage = 0;
				Suit.S.suitProgress = 0f;
				Suit.S.slimeBond = 0f;
				Suit.S.milkTimer = 0f;
				Suit.S.suitWorn = 0f;
				Suit.S.suitLoved = false;
				Suit.S.latexLooseLeft = 0f;
				Suit.ForgetBond();
				Suit.S.suitCooldown = 20f;
				Suit.ReleaseControl(b);
				Relief.Say(b, text);
				return;
			}
			limb.pain += 15f;
			Suit.S.arousal = Mathf.Min(100f, Suit.S.arousal + 15f);
			Suit.S.submission = Mathf.Min(100f, Suit.S.submission + 3f);
			Suit.S.suitProgress = Mathf.Min(99f, Suit.S.suitProgress + 4f);
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.9f, false, false);
			b.eatTime = Mathf.Max(b.eatTime, 0.6f);
			Relief.Say(b, "suit_resist_fail");
			if (Suit.Latex)
			{
				SymbioteVoice.Say(b, "latexv_keep", 1.2f);
			}
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x00062F7C File Offset: 0x0006117C
		public static bool RemoveByTrader(Body b)
		{
			if (Suit.Stage == 0 || !b)
			{
				return false;
			}
			string key = Suit.Key;
			bool flag = Suit.Enjoying || Suit.S.suitLoved || Suit.Symbiosis || (Suit.Slime && Suit.Stage >= 5);
			if (Puppet.Active)
			{
				Puppet.Release(b, true);
			}
			Limb limb = b.limbs[1];
			limb.pain += 8f + 3f * (float)Suit.Stage;
			limb.skinHealth -= 2f * (float)Suit.Stage;
			Sound.Play("ripclothing", b.transform.position, false, true, b.transform, 1f, 1f, false, false);
			Sfx.PlayMix(b, 0.9f, new string[] { "wet_slap", "slime_gloop" });
			Vector2 vector = (b.isRight ? Vector2.left : Vector2.right) * 1.5f;
			if (Suit.Slime)
			{
				SlimeBog.DropPuddle(b);
			}
			else if (!Suit.Latex && !Suit.LatexPartial)
			{
				Parasites.Spawn(b.transform.position + vector, Suit.S.suitColor, true);
			}
			Suit.S.suitStage = 0;
			Suit.S.suitProgress = 0f;
			Suit.S.slimeBond = 0f;
			Suit.S.milkTimer = 0f;
			Suit.S.suitWorn = 0f;
			Suit.S.suitLoved = false;
			Suit.S.latexLooseLeft = 0f;
			Suit.S.latexCorruption = 0f;
			Suit.S.latexPiece = 0;
			Suit.S.latexSpread = 0f;
			Suit.ForgetBond();
			Suit.S.suitCooldown = 20f;
			Suit.ReleaseControl(b);
			Suit.Update(b, 0f);
			Plugin.Log.LogInfo("A trader took her suit off (" + key + (flag ? ", she liked it" : "") + ")");
			return flag;
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x000631AC File Offset: 0x000613AC
		private static void SlimeBond(Body b, float dt)
		{
			if (Suit.Stage != 4)
			{
				Mood.Change(b, 0.01f * dt);
				foreach (Limb limb in b.limbs)
				{
					if (limb && limb.infected)
					{
						limb.infected = false;
					}
				}
				if ((Suit.bondLine -= dt) <= 0f)
				{
					Suit.bondLine = Random.Range(120f, 220f);
					Relief.Say(b, "slime_bond");
				}
				return;
			}
			if (Habit.Paused(b))
			{
				return;
			}
			float num = 1f + Suit.S.corruption / 100f + Suit.S.submission / 200f;
			Suit.S.slimeBond += dt * num;
			if (Suit.S.slimeBond >= Plugin.SlimeBondMinutes.Value * 60f)
			{
				Suit.StageUp(b);
				Plugin.Log.LogInfo("The slime suit has bonded with her (stage 5)");
			}
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x000632A8 File Offset: 0x000614A8
		private static void UpdateHostCalm(Body b, float dt)
		{
			if (Suit.S.hostTamed || Suit.S.hostStableLeft <= 0f)
			{
				return;
			}
			Suit.S.hostStableLeft -= dt;
			if (Suit.S.hostStableLeft > 0f)
			{
				return;
			}
			Suit.S.hostStableLeft = 0f;
			Relief.Say(b, "hslime_stable_end");
			SymbioteVoice.Say(b, "hivev_welcome", 1f);
			Plugin.Log.LogInfo("The antiserum has worn off: the hosts' slime moves again");
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00063330 File Offset: 0x00061530
		private static void CalmBoons(Body b, float dt)
		{
			Mood.Change(b, 0.01f * dt);
			foreach (Limb limb in b.limbs)
			{
				if (limb && limb.infected)
				{
					limb.infected = false;
				}
			}
			if (Suit.HostTamed && Suit.Stage >= 3)
			{
				Suit.Heal(b, dt, (float)(Suit.Stage - 2));
			}
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x00063398 File Offset: 0x00061598
		private static void Heal(Body b, float dt, float k)
		{
			foreach (Limb limb in b.limbs)
			{
				if (!limb.dismembered)
				{
					limb.skinHealth = Mathf.Min(100f, limb.skinHealth + 0.04f * k * dt);
					limb.muscleHealth = Mathf.Min(100f, limb.muscleHealth + 0.02f * k * dt);
					limb.bleedAmount = Mathf.Max(0f, limb.bleedAmount - 0.03f * k * dt);
				}
			}
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x00063428 File Offset: 0x00061628
		public static void TakeAntiserum(Body b, float ml)
		{
			if (Suit.LatexPartial && ml >= 10f)
			{
				Plugin.Log.LogInfo(string.Format("Antiserum ({0:0} ml): the latex mite's piece lets go of her", ml));
				LatexMites.PieceOff(b, "latexp_serum");
				return;
			}
			if (Suit.Latex && ml >= 10f)
			{
				Suit.S.latexLooseLeft = Mathf.Max(Suit.S.latexLooseLeft, Plugin.LatexLooseMinutes.Value * 60f);
				Sfx.PlayMix(b, 0.7f, new string[] { "slime_bubble", "slime_squelch" });
				Relief.Say(b, "latex_loose");
				Plugin.Log.LogInfo(string.Format("Antiserum ({0:0} ml): the living latex is loose for {1:0} min", ml, Plugin.LatexLooseMinutes.Value));
				return;
			}
			if (!Suit.HostSlime || Suit.Stage == 0 || Suit.Stage >= 5 || HostConversion.Active || Suit.S.hostTamed || ml < 10f)
			{
				return;
			}
			int num = Mathf.Max(1, Mathf.RoundToInt(ml / 50f));
			Suit.S.hostDoses += num;
			int num2 = Mathf.Max(1, Plugin.StabilizeDoses.Value);
			Sfx.PlayMix(b, 0.7f, new string[] { "slime_bubble", "slime_squelch" });
			if (Suit.S.hostDoses >= num2)
			{
				Suit.S.hostTamed = true;
				Suit.S.hostStableLeft = 0f;
				Suit.S.suitProgress = 0f;
				Relief.Say(b, "hslime_tamed");
				Plugin.Log.LogInfo(string.Format("Antiserum ({0:0} ml, {1} doses): the hosts' slime is stable for good", ml, Suit.S.hostDoses));
				return;
			}
			Suit.S.hostStableLeft = Mathf.Max(Suit.S.hostStableLeft, Plugin.StabilizeMinutes.Value * 60f);
			if (Suit.Stage == 4)
			{
				Suit.S.suitProgress = Mathf.Min(Suit.S.suitProgress, 50f);
			}
			Relief.Say(b, "hslime_stable");
			Plugin.Log.LogInfo(string.Format("Antiserum ({0:0} ml, {1}/{2} doses): the hosts' slime is quiet for {3:0} min", new object[]
			{
				ml,
				Suit.S.hostDoses,
				num2,
				Plugin.StabilizeMinutes.Value
			}));
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x0006369C File Offset: 0x0006189C
		private static void HostCreep(Body b, float dt)
		{
			if (Suit.Stage != 4 || HostConversion.Active)
			{
				return;
			}
			float num = 100f / (Plugin.HostTakeoverMinutes.Value * 60f) * (1f + Suit.S.arousal * 0.005f + Suit.S.submission * 0.005f) * (SlimeBog.Dark ? 3f : 1f);
			Suit.S.suitProgress += num * dt;
			if ((Suit.creepLine -= dt) <= 0f)
			{
				Suit.creepLine = Random.Range(25f, 40f);
				Relief.Say(b, (Suit.S.suitProgress > 70f) ? "hslime_face" : "hslime_creep");
			}
			if (Suit.S.suitProgress >= 100f)
			{
				Suit.StageUp(b);
			}
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00063784 File Offset: 0x00061984
		public static void Darken(Body b)
		{
			if (!Suit.Slime || Suit.HostSlime)
			{
				return;
			}
			Suit.S.suitColor = 7;
			if (Suit.S.suitStage > 4)
			{
				Suit.S.suitStage = 4;
				Suit.S.suitProgress = 0f;
			}
			Suit.fadeIn = 0f;
			Relief.Say(b, "hslime_takeover");
			Plugin.Log.LogInfo("The hosts' slime takes over the bog slime suit");
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x000637F8 File Offset: 0x000619F8
		public static int HostDose(Body b)
		{
			if (Suit.Stage == 0)
			{
				Suit.Attach(b, 7);
				if (Suit.Stage <= 0)
				{
					return 3;
				}
				return 0;
			}
			else
			{
				if (Suit.Slime && !Suit.HostSlime)
				{
					Suit.Darken(b);
				}
				if (!Suit.HostSlime)
				{
					return 3;
				}
				if (Suit.HostCalm)
				{
					return 3;
				}
				if (Suit.Stage >= 5 || HostConversion.Active)
				{
					return 2;
				}
				Suit.StageUp(b);
				if (Suit.Stage < 5)
				{
					return 1;
				}
				return 2;
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x0600064B RID: 1611 RVA: 0x00063867 File Offset: 0x00061A67
		public static bool ParasiteProtects
		{
			get
			{
				return Suit.Stage > 0 && !Suit.Slime && !Suit.Drone && Suit.Stage >= Mathf.Max(1, Plugin.ParasiteProtectStage.Value - ((Suit.Symbiote > false) ? 1 : 0));
			}
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x000638A4 File Offset: 0x00061AA4
		private static void SealWounds(Body b, float dt)
		{
			bool flag = false;
			float num = (Suit.Latex ? Plugin.LatexSeal.Value : (Suit.Slime ? Plugin.SlimeSeal.Value : (Plugin.ParasiteSeal.Value * (Suit.Symbiote ? 1.5f : 1f))));
			foreach (Limb limb in b.limbs)
			{
				if (limb && !limb.dismembered && limb.bleedAmount > 0f)
				{
					float bleedAmount = limb.bleedAmount;
					limb.bleedAmount = Mathf.Max(0f, limb.bleedAmount - (0.012f * limb.bleedAmount + num) * dt);
					if (limb.skinHealth < 100f)
					{
						limb.skinHealth = Mathf.Min(100f, limb.skinHealth + 0.02f * dt);
					}
					if (bleedAmount > 0.5f)
					{
						flag = true;
					}
				}
			}
			Suit.sealLine -= dt;
			if (flag && Suit.sealLine <= 0f)
			{
				Suit.sealLine = 90f;
				Relief.Say(b, Suit.Key + "_seal");
				Sfx.PlayMix(b, 0.4f, new string[] { "slime_squelch", "slime_slurp" });
				Plugin.Log.LogInfo("The " + Suit.Key + " suit is sealing her wounds");
			}
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00063A1C File Offset: 0x00061C1C
		public static void Update(Body b, float dt)
		{
			Antiserum.Ensure();
			if (Suit.S.suitCooldown > 0f)
			{
				Suit.S.suitCooldown -= dt;
			}
			if (Suit.S.removeCooldown > 0f)
			{
				Suit.S.removeCooldown -= dt;
			}
			if (Keys.Down(Plugin.RemoveKey.Value))
			{
				Suit.TryRemove(b);
			}
			if (Suit.S.suitStage > 0 && (Suit.S.suitColor == 4 || Suit.S.suitColor == 5))
			{
				Suit.S.suitColor = 0;
			}
			Suit.UpdateOverlays(b, dt, Suit.Mine);
			if (Suit.Stage == 0)
			{
				return;
			}
			Suit.UpdateLove(b, dt);
			if (Suit.Latex)
			{
				Suit.UpdateLatex(b, dt);
				return;
			}
			if (Suit.LatexPartial)
			{
				LatexMites.UpdatePiece(b, dt);
				return;
			}
			if (Suit.HostSlime)
			{
				Suit.UpdateHostCalm(b, dt);
			}
			if (Suit.HostSlime && Suit.HostCalm)
			{
				if (Suit.Stage >= 4)
				{
					Suit.SealWounds(b, dt);
				}
				Suit.CalmBoons(b, dt);
			}
			else if (Suit.HostSlime && Suit.Stage >= 4)
			{
				Suit.SealWounds(b, dt);
				Suit.HostCreep(b, dt);
			}
			else if (Suit.Slime && Suit.Stage >= 4)
			{
				Suit.SealWounds(b, dt);
				Suit.SlimeBond(b, dt);
			}
			else if (Suit.ParasiteProtects)
			{
				Suit.SealWounds(b, dt);
			}
			if (Suit.Slime)
			{
				Suit.UpdateSlime(b, dt);
				return;
			}
			float num = Suit.S.arousal * 0.01f;
			if (Suit.Stage < 4 || (Suit.Symbiote && Suit.Stage == 4))
			{
				float num2 = ((Suit.Stage == 4) ? 0.18f : Suit.GrowRate[Suit.Stage]) * (1f + num + Suit.S.submission * 0.005f + Suit.S.corruption * 0.005f) * (b.sleeping ? 2f : 1f) * Plugin.SuitGrowth.Value;
				Suit.S.suitProgress += num2 * dt;
				if (Suit.S.suitProgress >= 100f)
				{
					Suit.StageUp(b);
				}
			}
			float num3 = (Suit.Symbiote ? 0.4f : 1f);
			Suit.S.arousal = Mathf.Min(100f, Suit.S.arousal + Suit.Stim[Suit.Stage] * num3 * (1f + Suit.S.submission * 0.005f) * dt);
			Suit.S.submission = Mathf.Min(100f, Suit.S.submission + 0.01f * (float)Suit.Stage * num3 * dt);
			Suit.S.corruption = Mathf.Min(100f, Suit.S.corruption + 0.005f * (float)Suit.Stage * dt);
			if (Suit.Stage >= 4)
			{
				Suit.S.arousal = Mathf.Max(Suit.S.arousal, Suit.Symbiote ? 20f : 40f);
				Suit.S.lust = Mathf.Max(Suit.S.lust, Suit.Symbiote ? 30f : 50f);
			}
			Suit.pulseTimer -= dt;
			if (Suit.pulseTimer <= 0f)
			{
				Suit.pulseTimer = Random.Range(35f, 60f) / Mathf.Sqrt((float)Suit.Stage) * (Suit.Symbiote ? 2.5f : 1f) / Suit.Touch;
				Suit.Pulse(b);
			}
			float num4 = (Suit.Symbiosis ? 1.5f : 1f);
			b.hunger = Mathf.Max(0f, b.hunger - 0.0045f * (float)Suit.Stage * num4 * dt);
			b.thirst = Mathf.Max(0f, b.thirst - 0.0045f * (float)Suit.Stage * dt);
			if (Suit.Stage >= 3)
			{
				Suit.Heal(b, dt, (float)(Suit.Stage - 2) * (Suit.Tamed ? 2f : 1f));
				if (Suit.Tamed)
				{
					foreach (Limb limb in b.limbs)
					{
						if (!limb.dismembered)
						{
							limb.pain = Mathf.Max(0f, limb.pain - 0.3f * dt);
						}
					}
				}
			}
			if (Suit.Symbiosis)
			{
				foreach (Limb limb2 in b.limbs)
				{
					if (!limb2.dismembered)
					{
						limb2.skinHealth = Mathf.Min(100f, limb2.skinHealth + 0.08f * dt);
						limb2.muscleHealth = Mathf.Min(100f, limb2.muscleHealth + 0.05f * dt);
						limb2.bleedAmount = Mathf.Max(0f, limb2.bleedAmount - 0.06f * dt);
						limb2.pain = Mathf.Max(0f, limb2.pain - 0.4f * dt);
					}
				}
				if ((Suit.hungerLine -= dt) <= 0f)
				{
					Suit.hungerLine = Random.Range(150f, 260f);
					if (b.hunger < 60f)
					{
						Relief.Say(b, "symb_hungry");
					}
				}
			}
			Suit.UpdateMilking(b, dt);
			if (Suit.Stage == 4 && !Suit.Symbiote && !Suit.Tamed)
			{
				Suit.UpdateControl(b, dt);
				Suit.takeoverRollTimer -= dt;
				if (Suit.takeoverRollTimer <= 0f)
				{
					Suit.takeoverRollTimer = 60f;
					Puppet.TryBegin(b, 1f);
				}
			}
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00063FDC File Offset: 0x000621DC
		private static void UpdateLatex(Body b, float dt)
		{
			Suit.SealWounds(b, dt);
			Suit.Heal(b, dt, Plugin.LatexHeal.Value);
			if (Suit.S.latexLooseLeft > 0f)
			{
				Suit.S.latexLooseLeft -= dt;
				if (Suit.S.latexLooseLeft <= 0f)
				{
					Suit.S.latexLooseLeft = 0f;
					Relief.Say(b, "latex_loose_end");
				}
			}
			Suit.S.arousal = Mathf.Min(100f, Suit.S.arousal + Suit.Stim[3] * dt);
			Suit.S.arousal = Mathf.Max(Suit.S.arousal, 30f);
			Suit.S.lust = Mathf.Max(Suit.S.lust, 40f);
			if (!Habit.Paused(b))
			{
				Suit.S.submission = Mathf.Min(100f, Suit.S.submission + Plugin.LatexMind.Value * dt);
				Suit.S.corruption = Mathf.Min(100f, Suit.S.corruption + Plugin.LatexMind.Value * 0.5f * dt);
			}
			if ((Suit.thoughtLine -= dt) <= 0f)
			{
				Suit.thoughtLine = Random.Range(70f, 140f);
				Relief.Say(b, "latex_thought");
				b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 0.6f);
			}
			Suit.pulseTimer -= dt;
			if (Suit.pulseTimer <= 0f)
			{
				Suit.pulseTimer = Random.Range(40f, 70f) / Suit.Touch;
				Suit.Pulse(b);
			}
			b.hunger = Mathf.Max(0f, b.hunger - 0.01f * dt);
			Suit.UpdateMilking(b, dt);
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x000641C0 File Offset: 0x000623C0
		private static void UpdateSlime(Body b, float dt)
		{
			if (Suit.Stage < 4 && (!Suit.HostSlime || Suit.S.hostStableLeft <= 0f))
			{
				float num = Suit.GrowRate[Suit.Stage] * 1.3f * (1f + Suit.S.arousal * 0.01f) * Plugin.SuitGrowth.Value;
				if (SlimeBog.Stuck)
				{
					num *= 3f;
				}
				Suit.S.suitProgress += num * dt;
				if (Suit.S.suitProgress >= 100f)
				{
					Suit.StageUp(b);
				}
			}
			Suit.S.arousal = Mathf.Min(100f, Suit.S.arousal + Suit.Stim[Suit.Stage] * 0.6f * dt);
			Suit.pulseTimer -= dt;
			if (Suit.pulseTimer <= 0f)
			{
				Suit.pulseTimer = Random.Range(45f, 75f) / Mathf.Sqrt((float)Suit.Stage) / Suit.Touch;
				Suit.Pulse(b);
			}
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x000642D4 File Offset: 0x000624D4
		private static void StageUp(Body b)
		{
			Suit.S.suitStage++;
			Suit.S.suitProgress = 0f;
			Suit.fadeIn = 0f;
			b.eyePanicTime = 0.6f;
			Suit.S.arousal = Mathf.Min(100f, Suit.S.arousal + 10f);
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.7f, false, false);
			if (!Suit.Drone)
			{
				Sfx.PlayMix(b, 0.75f, new string[] { "slime_slurp", "slime_gloop" });
			}
			Sfx.Play(b, 0.8f, new string[] { "moan" });
			Relief.Say(b, Suit.Key + "_stage" + Suit.S.suitStage.ToString());
			InvasionPopup.SuitStage(b, Suit.S.suitStage);
			if (Suit.S.suitStage < 5)
			{
				BioWarning.Show();
			}
			if (Suit.HostSlime && Suit.S.suitStage >= 5)
			{
				HostConversion.Begin(b);
			}
			Plugin.Log.LogInfo(string.Format("{0} grew to stage {1}", Suit.Symbiote ? "Symbiote" : (Suit.Slime ? "Slime suit" : "Suit"), Suit.S.suitStage));
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00064450 File Offset: 0x00062650
		public static void Recede(Body b, int stage)
		{
			if (Suit.Stage <= stage)
			{
				return;
			}
			Suit.S.suitStage = stage;
			Suit.S.suitProgress = 0f;
			Suit.fadeIn = 0f;
			Plugin.Log.LogInfo(string.Format("{0} slid back to stage {1}", Suit.Slime ? "Slime suit" : "Suit", stage));
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x000644B8 File Offset: 0x000626B8
		public static void ForceNextStage(Body b)
		{
			if (Suit.Stage == 0)
			{
				Suit.Attach(b, Random.Range(0, 3));
				return;
			}
			if (Suit.Stage < 4 || (Suit.Symbiote && Suit.Stage == 4))
			{
				Suit.StageUp(b);
				return;
			}
			if (Suit.Stage == 4 && Suit.HostSlime)
			{
				Suit.StageUp(b);
				return;
			}
			if (Suit.Stage == 4 && !Suit.Slime)
			{
				Puppet.Begin(b);
			}
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00064524 File Offset: 0x00062724
		private static void Pulse(Body b)
		{
			if (Suit.Slime)
			{
				b.wetness = Mathf.Min(Mathf.Max(b.wetness, 15f), b.wetness + 6f);
			}
			Suit.S.arousal = Mathf.Min(100f, Suit.S.arousal + 6f + 4f * (float)Suit.Stage);
			b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 0.8f);
			b.eatTime = Mathf.Max(b.eatTime, 0.6f);
			Sfx.Play(b, 0.5f, new string[] { "drip" });
			InvasionPopup.SuitPulse(b);
			if (!Suit.Drone)
			{
				Sfx.PlayMix(b, 0.4f + 0.06f * (float)Suit.Stage, new string[] { "slime_squelch", "slime_slurp", "slime_bubble" });
			}
			if (Puppet.Active || (Suit.Symbiote && Suit.Stage >= 4))
			{
				Sfx.Play(b, 0.7f, new string[] { "muffled", "moan_soft" });
			}
			else if (Suit.Stage <= 2)
			{
				Sfx.Play(b, 0.55f, new string[] { "moan_soft", "moan" });
			}
			else
			{
				Sfx.Play(b, 0.75f, new string[] { "moan", "moan_soft" });
			}
			if (Suit.Stage >= 3 && !Suit.Drone && !Suit.Slime)
			{
				Semen.Add(b, Suit.Symbiote ? 2f : 4f);
			}
			if (Suit.Enjoying && !Puppet.Active && Random.value < 0.75f)
			{
				Relief.Say(b, Suit.Love("pulse"));
				return;
			}
			Relief.Say(b, Puppet.Active ? "puppet_line" : (Suit.Latex ? "latex_pulse" : (Suit.Symbiote ? "symb_pulse" : (Suit.Slime ? (Suit.Key + "_pulse") : "suit_pulse"))));
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00064744 File Offset: 0x00062944
		private static void UpdateMilking(Body b, float dt)
		{
			if (Suit.Stage < 2)
			{
				return;
			}
			if (Suit.S.milkTimer <= 0f)
			{
				Suit.milkRollTimer -= dt;
				if (Suit.milkRollTimer <= 0f)
				{
					Suit.milkRollTimer = 60f;
					if (Random.value < (0.2f + 0.1f * (float)Suit.Stage) * (Suit.Symbiote ? 0.3f : (Suit.Latex ? 0.5f : 1f)) * Suit.Touch)
					{
						Suit.S.milkTimer = 20f;
						Relief.Say(b, Suit.Enjoying ? "suitlove_milk" : "suit_milk_start");
						Plugin.Log.LogInfo("Suit starts milking");
					}
				}
				return;
			}
			Suit.S.milkTimer -= dt;
			Suit.S.arousal = Mathf.Min(100f, Suit.S.arousal + 0.8f * dt);
			b.thirst = Mathf.Max(0f, b.thirst - 0.25f * dt);
			b.hunger = Mathf.Max(0f, b.hunger - 0.1f * dt);
			Suit.S.suitProgress = Mathf.Min(99.9f, Suit.S.suitProgress + 0.15f * dt);
			b.limbs[1].skinHealth = Mathf.Min(100f, b.limbs[1].skinHealth + 0.1f * dt);
			if (Random.value < dt * 0.5f)
			{
				Sfx.Play(b, 0.35f, new string[] { "drip" });
			}
			if (!Suit.Drone && Random.value < dt * 0.35f)
			{
				Sfx.PlayMix(b, 0.35f, new string[] { "slime_squelch", "slime_slurp" });
			}
			if (Random.value < dt * 0.15f)
			{
				Sfx.Play(b, 0.45f, new string[] { "moan_soft", "moan" });
			}
			if (Suit.S.milkTimer <= 0f)
			{
				Relief.Say(b, Suit.Enjoying ? "suitlove_milk_end" : "suit_milk_end");
				return;
			}
			if (Suit.Enjoying && Random.value < dt * 0.12f)
			{
				Relief.Say(b, "suitlove_milking");
			}
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x000649AC File Offset: 0x00062BAC
		private static void UpdateControl(Body b, float dt)
		{
			if (Suit.lostControlTimer > 0f)
			{
				Suit.lostControlTimer -= dt;
				b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 0.1f);
				if (Suit.lostControlTimer <= 0f)
				{
					Suit.ReleaseControl(b);
				}
				return;
			}
			if (Suit.Stage >= 4 && Suit.S.arousal > 70f && !Relief.Active && b.standing && Random.value < dt / 40f * Suit.Touch)
			{
				Suit.lostControlTimer = 2.5f;
				Traverse.Create(b).Field("movingAllowed").SetValue(false);
				Sfx.Play(b, 0.8f, new string[] { "moan" });
				Relief.Say(b, "suit_control");
			}
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00064A80 File Offset: 0x00062C80
		private static void ReleaseControl(Body b)
		{
			if (Suit.lostControlTimer > 0f || !Relief.Active)
			{
				Traverse.Create(b).Field("movingAllowed").SetValue(true);
			}
			Suit.lostControlTimer = 0f;
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00064ABC File Offset: 0x00062CBC
		public static void OnOrgasm(Body b)
		{
			if (Suit.Stage == 0)
			{
				return;
			}
			Puppet.OnOrgasm(b);
			if (Suit.Drone)
			{
				Suit.S.droneNeed = 0f;
				Relief.Say(b, "drone_feed");
				return;
			}
			if (Suit.Stage == 4 && !Suit.Symbiote && !Suit.Slime)
			{
				Puppet.TryBegin(b, 3f);
			}
			if (Suit.Stage < 4 || (Suit.Symbiote && Suit.Stage == 4))
			{
				Suit.S.suitProgress = Mathf.Min(99.9f, Suit.S.suitProgress + 8f);
			}
			if (Suit.Slime)
			{
				return;
			}
			Relief.Say(b, Suit.Key + "_feed");
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x00064B72 File Offset: 0x00062D72
		public static bool Covers(string texName, int stage)
		{
			return Suit.Covers(texName, stage, Suit.Mine);
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x00064B80 File Offset: 0x00062D80
		public static bool Covers(string texName, int stage, Suit.Look look)
		{
			if (stage <= 0 || texName == null || !texName.StartsWith("experiment"))
			{
				return false;
			}
			if (look.Latex && look.piece > 0 && stage < 5)
			{
				return LatexMites.PieceCovers(look.piece, stage, texName);
			}
			if (look.Symbiote && stage == 4)
			{
				stage = 5;
			}
			if (look.Slime && !look.HostSlime && stage > 4)
			{
				stage = 4;
			}
			int num = 1;
			while (num <= stage && num < Suit.Coverage.Length)
			{
				foreach (string text in Suit.Coverage[num])
				{
					if (texName.StartsWith("experiment" + text))
					{
						return true;
					}
				}
				num++;
			}
			return false;
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00064C38 File Offset: 0x00062E38
		private static Suit.OverlaySet SetFor(Body b)
		{
			Suit.OverlaySet overlaySet;
			if (!Suit.overlaySets.TryGetValue(b, out overlaySet))
			{
				foreach (Body body in Suit.overlaySets.Keys.Where<Body>((Body k) => !k).ToList<Body>())
				{
					Suit.overlaySets.Remove(body);
				}
				overlaySet = (Suit.overlaySets[b] = new Suit.OverlaySet());
			}
			return overlaySet;
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x00064CE0 File Offset: 0x00062EE0
		private static void UpdateOverlays(Body b, float dt, Suit.Look look)
		{
			Suit.OverlaySet overlaySet = Suit.SetFor(b);
			List<Suit.Overlay> list = overlaySet.list;
			ref float ptr = (ref PlayerCamera.main && b == PlayerCamera.main.body ? ref Suit.fadeIn : ref overlaySet.fadeIn);
			overlaySet.refresh -= dt;
			if (overlaySet.refresh <= 0f)
			{
				overlaySet.refresh = 1f;
				list.RemoveAll((Suit.Overlay o) => !o.parent || !o.sr);
				SpriteRenderer[] componentsInChildren = b.GetComponentsInChildren<SpriteRenderer>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					SpriteRenderer r = componentsInChildren[i];
					if (!(r.gameObject.name == "LewdSuit") && !(r.gameObject.name == "LewdSuitLens") && r.sprite && r.sprite.texture && ParasiteArt.SuitTextures.ContainsKey(r.sprite.texture.name) && !list.Any<Suit.Overlay>((Suit.Overlay o) => o.parent == r))
					{
						GameObject gameObject = new GameObject("LewdSuit", new Type[] { typeof(SpriteRenderer) });
						gameObject.transform.SetParent(r.transform, false);
						SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
						component.sharedMaterial = r.sharedMaterial;
						list.Add(new Suit.Overlay
						{
							parent = r,
							sr = component
						});
					}
				}
			}
			ptr = Mathf.MoveTowards(ptr, 1f, dt / 3f);
			Color color = ParasiteArt.Colors[Mathf.Clamp(look.color, 0, ParasiteArt.Colors.Length - 1)];
			foreach (Suit.Overlay overlay in list)
			{
				if (overlay.parent && overlay.sr)
				{
					Sprite sprite = overlay.parent.sprite;
					string text = ((sprite && sprite.texture) ? sprite.texture.name : null);
					bool flag = Suit.Covers(text, look.stage - 1, look);
					bool flag2 = overlay.parent.enabled && Suit.Covers(text, look.stage, look);
					overlay.sr.enabled = flag2;
					if (flag2)
					{
						bool flag3 = (look.stage >= 5 && (!look.Slime || look.HostSlime)) || (look.Symbiote && look.stage >= 4);
						if (sprite != overlay.lastParentSprite || flag3 != overlay.lastSealed || look.HostSlime != overlay.lastHost)
						{
							overlay.lastParentSprite = sprite;
							overlay.lastSealed = flag3;
							overlay.lastHost = look.HostSlime;
							overlay.sr.sprite = Suit.SuitSprite(sprite, flag3, look.HostSlime);
						}
						overlay.sr.flipX = overlay.parent.flipX;
						overlay.sr.flipY = overlay.parent.flipY;
						overlay.sr.sortingLayerID = overlay.parent.sortingLayerID;
						overlay.sr.sortingOrder = overlay.parent.sortingOrder + 1;
						float num = (flag ? 1f : ptr);
						Color color2 = color;
						if (look.milking && text != null && text.Contains("UpTorso"))
						{
							color2 = Color.Lerp(color, Color.white, 0.25f + 0.15f * Mathf.Sin(Time.time * 6f));
						}
						if (look.HostSlime)
						{
							num *= 0.9f + 0.06f * Mathf.Sin(Time.time * 2f + (float)overlay.parent.sortingOrder);
						}
						else if (look.Slime)
						{
							num *= 0.55f + 0.08f * Mathf.Sin(Time.time * 2.5f + (float)overlay.parent.sortingOrder);
						}
						overlay.sr.color = new Color(color2.r, color2.g, color2.b, num * overlay.parent.color.a);
						Suit.UpdateLens(overlay, text, num, look);
					}
				}
			}
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x000651FC File Offset: 0x000633FC
		private static void UpdateLens(Suit.Overlay o, string tex, float alpha, Suit.Look look)
		{
			if (look.Latex)
			{
				Suit.LatexEye(o, tex, alpha, look);
				return;
			}
			if (((!look.Symbiote || look.stage < 4) && (!look.Drone || look.stage < 5) && (!look.Latex || look.stage < 5)) || tex == null || !tex.StartsWith("experimentHead") || !ParasiteArt.LensTextures.ContainsKey(tex))
			{
				if (o.lens)
				{
					o.lens.enabled = false;
				}
				return;
			}
			if (!o.lens)
			{
				GameObject gameObject = new GameObject("LewdSuitLens", new Type[] { typeof(SpriteRenderer) });
				gameObject.transform.SetParent(o.parent.transform, false);
				o.lens = gameObject.GetComponent<SpriteRenderer>();
				o.lens.sharedMaterial = o.parent.sharedMaterial;
			}
			Sprite sprite = o.parent.sprite;
			if (sprite != o.lastLensParent)
			{
				o.lastLensParent = sprite;
				Sprite sprite2;
				if (!Suit.lensSprites.TryGetValue(sprite, out sprite2))
				{
					Texture2D texture2D = ParasiteArt.LensTextures[tex];
					Rect rect = sprite.rect;
					sprite2 = Sprite.Create(texture2D, new Rect(rect.x, rect.y, Mathf.Min(rect.width, (float)texture2D.width), Mathf.Min(rect.height, (float)texture2D.height)), new Vector2(sprite.pivot.x / rect.width, sprite.pivot.y / rect.height), sprite.pixelsPerUnit);
					Suit.lensSprites[sprite] = sprite2;
				}
				o.lens.sprite = sprite2;
			}
			o.lens.enabled = true;
			o.lens.flipX = o.parent.flipX;
			o.lens.flipY = o.parent.flipY;
			o.lens.sortingLayerID = o.parent.sortingLayerID;
			o.lens.sortingOrder = o.parent.sortingOrder + 2;
			Color color = (look.Latex ? new Color(0.78f, 0.5f, 1f) : Color.white);
			Suit.Glow(o, look.Latex, alpha);
			o.lens.color = new Color(color.r, color.g, color.b, alpha * o.parent.color.a);
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x00065494 File Offset: 0x00063694
		private static void LatexEye(Suit.Overlay o, string tex, float alpha, Suit.Look look)
		{
			if (look.stage < 5 || tex == null || !tex.StartsWith("experimentEye") || !o.parent.enabled || !o.parent.sprite)
			{
				if (o.lens)
				{
					o.lens.enabled = false;
					Suit.Glow(o, false, 0f);
					Transform transform = o.lens.transform.Find("LewdLatexHalo");
					if (transform)
					{
						transform.gameObject.SetActive(false);
					}
				}
				return;
			}
			Transform transform2 = (o.lens ? o.lens.transform.Find("LewdLatexHalo") : null);
			if (transform2)
			{
				transform2.gameObject.SetActive(true);
			}
			if (!o.lens)
			{
				GameObject gameObject = new GameObject("LewdSuitLens", new Type[] { typeof(SpriteRenderer) });
				gameObject.transform.SetParent(o.parent.transform, false);
				o.lens = gameObject.GetComponent<SpriteRenderer>();
				o.lens.sharedMaterial = o.parent.sharedMaterial;
			}
			o.lens.sprite = Suit.EyeMask(o.parent.sprite);
			if (Suit.Unlit)
			{
				o.lens.sharedMaterial = Suit.Unlit;
			}
			o.lens.enabled = true;
			o.lens.flipX = o.parent.flipX;
			o.lens.flipY = o.parent.flipY;
			o.lens.sortingLayerID = o.parent.sortingLayerID;
			o.lens.sortingOrder = o.parent.sortingOrder + 3;
			float num = 0.85f + 0.15f * Mathf.Sin(Time.time * 2.2f);
			o.lens.color = new Color(0.72f + 0.2f * num, 0.38f + 0.2f * num, 1f, alpha * o.parent.color.a);
			Suit.Halo(o, alpha * num);
			Suit.Glow(o, true, alpha);
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x0600065E RID: 1630 RVA: 0x000656D8 File Offset: 0x000638D8
		private static Material Unlit
		{
			get
			{
				if (!Suit.unlit)
				{
					Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") ?? Shader.Find("Sprites/Default");
					if (shader)
					{
						Suit.unlit = new Material(shader);
					}
				}
				return Suit.unlit;
			}
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x00065724 File Offset: 0x00063924
		private static Sprite EyeMask(Sprite eye)
		{
			Sprite sprite;
			if (Suit.eyeMasks.TryGetValue(eye, out sprite) && sprite)
			{
				return sprite;
			}
			Texture2D texture2D = Bust.Readable(eye.texture);
			Rect textureRect = eye.textureRect;
			int num = Mathf.RoundToInt(textureRect.width);
			int num2 = Mathf.RoundToInt(textureRect.height);
			Texture2D texture2D2 = new Texture2D(num, num2, 4, false)
			{
				filterMode = 0,
				wrapMode = 1
			};
			Color32[] array = new Color32[num * num2];
			try
			{
				Color[] pixels = texture2D.GetPixels(Mathf.RoundToInt(textureRect.x), Mathf.RoundToInt(textureRect.y), num, num2);
				for (int i = 0; i < pixels.Length; i++)
				{
					array[i] = ((pixels[i].a < 0.5f) ? new Color32(0, 0, 0, 0) : ((pixels[i].grayscale > 0.35f) ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : new Color32(150, 150, 150, byte.MaxValue)));
				}
			}
			catch
			{
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				}
			}
			texture2D2.SetPixels32(array);
			texture2D2.Apply(false);
			sprite = Sprite.Create(texture2D2, new Rect(0f, 0f, (float)num, (float)num2), new Vector2(eye.pivot.x / textureRect.width, eye.pivot.y / textureRect.height), eye.pixelsPerUnit);
			return Suit.eyeMasks[eye] = sprite;
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x00065900 File Offset: 0x00063B00
		private static void Halo(Suit.Overlay o, float strength)
		{
			Transform transform = o.lens.transform.Find("LewdLatexHalo");
			SpriteRenderer spriteRenderer = (transform ? transform.GetComponent<SpriteRenderer>() : null);
			if (!spriteRenderer)
			{
				if (!Suit.haloSprite)
				{
					Texture2D texture2D = new Texture2D(32, 32, 4, false)
					{
						filterMode = 1,
						wrapMode = 1
					};
					for (int i = 0; i < 32; i++)
					{
						for (int j = 0; j < 32; j++)
						{
							float num = Mathf.Clamp01(Vector2.Distance(new Vector2((float)j + 0.5f, (float)i + 0.5f), new Vector2(16f, 16f)) / 16f);
							texture2D.SetPixel(j, i, new Color(1f, 1f, 1f, (1f - num) * (1f - num)));
						}
					}
					texture2D.Apply();
					Suit.haloSprite = Sprite.Create(texture2D, new Rect(0f, 0f, 32f, 32f), new Vector2(0.5f, 0.5f), 35.555557f);
				}
				GameObject gameObject = new GameObject("LewdLatexHalo");
				gameObject.transform.SetParent(o.lens.transform, false);
				spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = Suit.haloSprite;
				if (Suit.Unlit)
				{
					spriteRenderer.sharedMaterial = Suit.Unlit;
				}
			}
			spriteRenderer.transform.position = o.lens.bounds.center;
			spriteRenderer.sortingLayerID = o.lens.sortingLayerID;
			spriteRenderer.sortingOrder = o.lens.sortingOrder - 1;
			spriteRenderer.color = new Color(0.62f, 0.25f, 1f, 0.55f * strength);
			spriteRenderer.enabled = o.lens.enabled;
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x00065AE8 File Offset: 0x00063CE8
		private static void Glow(Suit.Overlay o, bool on, float alpha)
		{
			if (!o.lens)
			{
				return;
			}
			Light2D light2D = o.lens.GetComponent<Light2D>();
			if (!on)
			{
				if (light2D)
				{
					light2D.enabled = false;
				}
				return;
			}
			if (!light2D)
			{
				light2D = o.lens.gameObject.AddComponent<Light2D>();
				light2D.lightType = 3;
				light2D.color = new Color(0.62f, 0.3f, 1f);
				light2D.pointLightInnerRadius = 0f;
				light2D.pointLightOuterRadius = 0.9f;
				light2D.shadowIntensity = 0f;
				light2D.volumeIntensity = 0f;
			}
			light2D.enabled = true;
			light2D.intensity = (1.2f + 0.35f * Mathf.Sin(Time.time * 2.2f)) * alpha * Plugin.LatexGlow.Value;
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x00065BC0 File Offset: 0x00063DC0
		private static Sprite SuitSprite(Sprite parent, bool sealedMask, bool host)
		{
			if (!parent)
			{
				return null;
			}
			Dictionary<Sprite, Sprite> dictionary = (host ? (sealedMask ? Suit.hostSealedSprites : Suit.hostSprites) : (sealedMask ? Suit.sealedSprites : Suit.suitSprites));
			Sprite sprite;
			if (dictionary.TryGetValue(parent, out sprite))
			{
				return sprite;
			}
			Texture2D texture2D = null;
			string name = parent.texture.name;
			if (host)
			{
				if (sealedMask)
				{
					ParasiteArt.HostFullTextures.TryGetValue(name, out texture2D);
				}
				if (!texture2D)
				{
					ParasiteArt.HostTextures.TryGetValue(name, out texture2D);
				}
			}
			if (!texture2D && sealedMask)
			{
				ParasiteArt.SuitFullTextures.TryGetValue(name, out texture2D);
			}
			if (!texture2D && !ParasiteArt.SuitTextures.TryGetValue(name, out texture2D))
			{
				return null;
			}
			Rect rect = parent.rect;
			sprite = Sprite.Create(texture2D, new Rect(rect.x, rect.y, Mathf.Min(rect.width, (float)texture2D.width), Mathf.Min(rect.height, (float)texture2D.height)), new Vector2(parent.pivot.x / rect.width, parent.pivot.y / rect.height), parent.pixelsPerUnit);
			sprite.name = (sealedMask ? "suitfull_" : "suit_") + parent.name;
			dictionary[parent] = sprite;
			return sprite;
		}

		// Token: 0x040006AB RID: 1707
		private static float loveLine = 30f;

		// Token: 0x040006AC RID: 1708
		public const int SymbioteColor = 3;

		// Token: 0x040006AD RID: 1709
		public const int SlimeColor = 6;

		// Token: 0x040006AE RID: 1710
		public const int HostSlimeColor = 7;

		// Token: 0x040006AF RID: 1711
		public const int LatexColor = 8;

		// Token: 0x040006B0 RID: 1712
		private static readonly string[][] Coverage = new string[][]
		{
			new string[0],
			new string[] { "UpTorso", "DownTorso" },
			new string[] { "Thigh", "UpArm" },
			new string[] { "Crus", "DownArm", "Foot", "HandF", "HandB", "Tail" },
			new string[] { "Head" },
			new string[] { "Eye" }
		};

		// Token: 0x040006B1 RID: 1713
		private static readonly float[] ArmorCloth = new float[] { 0f, 0.5f, 0.8f, 1.2f, 1.8f, 2f };

		// Token: 0x040006B2 RID: 1714
		private static readonly float[] ArmorSymb = new float[] { 0f, 0.5f, 0.8f, 1.2f, 1.8f, 2.2f };

		// Token: 0x040006B3 RID: 1715
		private static readonly float[] RemoveCloth = new float[] { 0f, 0.55f, 0.3f, 0.12f, 0.02f, 0f };

		// Token: 0x040006B4 RID: 1716
		private static readonly float[] RemoveSymb = new float[] { 0f, 0.55f, 0.3f, 0.12f, 0.05f, 0.03f };

		// Token: 0x040006B5 RID: 1717
		private static readonly float[] ArmorDrone = new float[] { 0f, 0f, 0f, 0f, 0f, 1.5f };

		// Token: 0x040006B6 RID: 1718
		private static readonly float[] RemoveDrone = new float[] { 0f, 0f, 0f, 0f, 0f, 0.03f };

		// Token: 0x040006B7 RID: 1719
		private static readonly float[] ArmorSlime = new float[6];

		// Token: 0x040006B8 RID: 1720
		private static readonly float[] RemoveSlime = new float[] { 0f, 0.6f, 0.45f, 0.3f, 0.2f, 0.2f };

		// Token: 0x040006B9 RID: 1721
		private static readonly float[] ArmorTamed = new float[] { 0f, 0.5f, 0.8f, 1.2f, 2f, 2f };

		// Token: 0x040006BA RID: 1722
		private static readonly float[] RemoveLatex = new float[] { 0f, 0f, 0f, 0f, 0f, 0.01f };

		// Token: 0x040006BB RID: 1723
		private static readonly float[] ArmorLatex = new float[] { 0f, 0f, 0f, 0f, 0f, 1f };

		// Token: 0x040006BC RID: 1724
		private static readonly float[] ArmorLatexPiece = new float[] { 0f, 0.2f, 0.3f, 0.4f, 0.5f, 0f };

		// Token: 0x040006BD RID: 1725
		private static readonly float[] RemoveLatexPiece = new float[] { 0f, 0.5f, 0.35f, 0.2f, 0.1f, 0f };

		// Token: 0x040006BE RID: 1726
		private static readonly float[] GrowRate = new float[] { 0f, 0.35f, 0.28f, 0.22f, 0f, 0f };

		// Token: 0x040006BF RID: 1727
		private static readonly float[] Stim = new float[] { 0f, 0.15f, 0.35f, 0.6f, 0.9f, 1.1f };

		// Token: 0x040006C0 RID: 1728
		private static float pulseTimer = 20f;

		// Token: 0x040006C1 RID: 1729
		private static float milkRollTimer = 60f;

		// Token: 0x040006C2 RID: 1730
		private static float takeoverRollTimer = 60f;

		// Token: 0x040006C3 RID: 1731
		private static float hungerLine = 120f;

		// Token: 0x040006C4 RID: 1732
		private static float lostControlTimer;

		// Token: 0x040006C5 RID: 1733
		private static float fadeIn;

		// Token: 0x040006C6 RID: 1734
		private static float sealLine;

		// Token: 0x040006C7 RID: 1735
		private static float bondLine;

		// Token: 0x040006C8 RID: 1736
		private static float creepLine;

		// Token: 0x040006C9 RID: 1737
		private static float thoughtLine = 60f;

		// Token: 0x040006CA RID: 1738
		private static readonly Dictionary<Body, Suit.OverlaySet> overlaySets = new Dictionary<Body, Suit.OverlaySet>();

		// Token: 0x040006CB RID: 1739
		private static readonly Dictionary<Sprite, Sprite> suitSprites = new Dictionary<Sprite, Sprite>();

		// Token: 0x040006CC RID: 1740
		private static readonly Dictionary<Sprite, Sprite> lensSprites = new Dictionary<Sprite, Sprite>();

		// Token: 0x040006CD RID: 1741
		private static readonly Dictionary<Sprite, Sprite> eyeMasks = new Dictionary<Sprite, Sprite>();

		// Token: 0x040006CE RID: 1742
		private static Material unlit;

		// Token: 0x040006CF RID: 1743
		private static Sprite haloSprite;

		// Token: 0x040006D0 RID: 1744
		private static readonly Dictionary<Sprite, Sprite> sealedSprites = new Dictionary<Sprite, Sprite>();

		// Token: 0x040006D1 RID: 1745
		private static readonly Dictionary<Sprite, Sprite> hostSprites = new Dictionary<Sprite, Sprite>();

		// Token: 0x040006D2 RID: 1746
		private static readonly Dictionary<Sprite, Sprite> hostSealedSprites = new Dictionary<Sprite, Sprite>();

		// Token: 0x02000233 RID: 563
		internal struct Look
		{
			// Token: 0x17000145 RID: 325
			// (get) Token: 0x06000B05 RID: 2821 RVA: 0x0007D0C4 File Offset: 0x0007B2C4
			public bool Symbiote
			{
				get
				{
					return this.color == 3;
				}
			}

			// Token: 0x17000146 RID: 326
			// (get) Token: 0x06000B06 RID: 2822 RVA: 0x0007D0CF File Offset: 0x0007B2CF
			public bool Slime
			{
				get
				{
					return this.color == 6 || this.color == 7;
				}
			}

			// Token: 0x17000147 RID: 327
			// (get) Token: 0x06000B07 RID: 2823 RVA: 0x0007D0E5 File Offset: 0x0007B2E5
			public bool HostSlime
			{
				get
				{
					return this.color == 7;
				}
			}

			// Token: 0x17000148 RID: 328
			// (get) Token: 0x06000B08 RID: 2824 RVA: 0x0007D0F0 File Offset: 0x0007B2F0
			public bool Drone
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000149 RID: 329
			// (get) Token: 0x06000B09 RID: 2825 RVA: 0x0007D0F3 File Offset: 0x0007B2F3
			public bool Latex
			{
				get
				{
					return this.color == 8;
				}
			}

			// Token: 0x04000AFB RID: 2811
			public int stage;

			// Token: 0x04000AFC RID: 2812
			public int color;

			// Token: 0x04000AFD RID: 2813
			public bool milking;

			// Token: 0x04000AFE RID: 2814
			public int piece;
		}

		// Token: 0x02000234 RID: 564
		private class Overlay
		{
			// Token: 0x04000AFF RID: 2815
			public SpriteRenderer parent;

			// Token: 0x04000B00 RID: 2816
			public SpriteRenderer sr;

			// Token: 0x04000B01 RID: 2817
			public Sprite lastParentSprite;

			// Token: 0x04000B02 RID: 2818
			public bool lastSealed;

			// Token: 0x04000B03 RID: 2819
			public bool lastHost;

			// Token: 0x04000B04 RID: 2820
			public SpriteRenderer lens;

			// Token: 0x04000B05 RID: 2821
			public Sprite lastLensParent;
		}

		// Token: 0x02000235 RID: 565
		private class OverlaySet
		{
			// Token: 0x04000B06 RID: 2822
			public readonly List<Suit.Overlay> list = new List<Suit.Overlay>();

			// Token: 0x04000B07 RID: 2823
			public float refresh;

			// Token: 0x04000B08 RID: 2824
			public float fadeIn = 1f;
		}
	}
}
