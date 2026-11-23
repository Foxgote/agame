using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using HarmonyLib;
using PoseKit;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000AC RID: 172
	internal static class LatexMites
	{
		// Token: 0x060003A8 RID: 936 RVA: 0x0003CAC8 File Offset: 0x0003ACC8
		public static void Bind(ConfigFile c)
		{
			LatexMites.PerLayer = c.Bind<float>("LatexMite", "PerLayer", 3f, "Latex mites on a layer (small drops of the living latex that hop at her).");
			LatexMites.FirstLayer = c.Bind<float>("LatexMite", "FirstLayer", 1f, "The first layer with latex mites (0 is the first layer of a run, 1 the second).");
			LatexMites.SpreadMinutes = c.Bind<float>("LatexMite", "SpreadMinutes", 15f, "Minutes a mite's piece of latex (stockings, tights, a body, gloves) takes to spread over all of her and close over her head (the living latex suit).");
			LatexMites.HeadMinutes = c.Bind<float>("LatexMite", "HeadMinutes", 2f, "Two mites won: the latex is on all of her but her head at once, and over her head this many minutes later.");
			LatexMites.StandPresses = c.Bind<int>("LatexMite", "StandPresses", 8, "Presses of the tear-off key (J) to tear one mite off her, standing (fewer with stamina, the difficulty's escape bonus).");
			LatexMites.StandSeconds = c.Bind<float>("LatexMite", "StandSeconds", 6f, "Seconds she has to tear one mite off before it runs into a piece of latex on her.");
			LatexMites.GroundPresses = c.Bind<int>("LatexMite", "GroundPresses", 14, "Presses to throw two mites off her, down on her back.");
			LatexMites.GroundSeconds = c.Bind<float>("LatexMite", "GroundSeconds", 9f, "Seconds she has against two mites.");
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060003A9 RID: 937 RVA: 0x0003CBC6 File Offset: 0x0003ADC6
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060003AA RID: 938 RVA: 0x0003CBCD File Offset: 0x0003ADCD
		public static bool Busy
		{
			get
			{
				return LatexMites.now > LatexMites.Phase.None;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060003AB RID: 939 RVA: 0x0003CBD7 File Offset: 0x0003ADD7
		public static bool Wants
		{
			get
			{
				return Suit.Stage == 0 || Suit.LatexPartial;
			}
		}

		// Token: 0x060003AC RID: 940 RVA: 0x0003CBE8 File Offset: 0x0003ADE8
		public static void SeedLayer(Body b, WorldGeneration w)
		{
			if (!w || w.biomeOverride != null || ParasiteArt.MiteFrames.Length == 0)
			{
				return;
			}
			if (w.biomeDepth < Mathf.RoundToInt(LatexMites.FirstLayer.Value))
			{
				Plugin.Log.LogInfo(string.Format("Layer {0}: no latex mites (too shallow)", w.biomeDepth));
				return;
			}
			int num = Diff.Count(LatexMites.PerLayer.Value);
			int num2 = 0;
			int mask = LayerMask.GetMask(new string[] { "Ground" });
			int num3 = 0;
			while (num3 < 300 && num2 < num)
			{
				Vector2 vector;
				vector..ctor(Random.Range(-w.halfWidth, w.halfWidth), Random.Range(-w.halfHeight, w.halfHeight));
				if (Vector2.Distance(vector, b.transform.position) >= 25f && !Physics2D.OverlapPoint(vector, mask))
				{
					RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, Vector2.down, 20f, mask);
					if (raycastHit2D && Parasites.Spawn(raycastHit2D.point + Vector2.up * 0.3f, 8, false))
					{
						num2++;
					}
				}
				num3++;
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: {1} of {2} latex mites", w.biomeDepth, num2, num));
		}

		// Token: 0x060003AD RID: 941 RVA: 0x0003CD5C File Offset: 0x0003AF5C
		public static void OnLatch(Body b, ParasiteAI ai)
		{
			if (!ai || LatexMites.on.Contains(ai))
			{
				return;
			}
			if (LatexMites.now == LatexMites.Phase.Standing)
			{
				LatexMites.Hold(b, ai);
				LatexMites.GoDown(b);
				return;
			}
			if (LatexMites.now == LatexMites.Phase.Falling || LatexMites.now == LatexMites.Phase.Ground)
			{
				if (LatexMites.on.Count < 2)
				{
					LatexMites.Hold(b, ai);
					return;
				}
				LatexMites.Bounce(b, ai);
				return;
			}
			else
			{
				if (Suit.LatexPartial)
				{
					Object.Destroy(ai.gameObject);
					LatexMites.S.latexSpread += LatexMites.StepSeconds;
					Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.8f, false, false);
					Sfx.PlayMix(b, 0.8f, new string[] { "slime_gloop", "wet_slap" });
					Relief.Say(b, "latexp_more");
					Plugin.Log.LogInfo("A latex mite runs into the piece on her: it spreads a step further");
					return;
				}
				if (Suit.Stage > 0 || Scenes.Busy || HerPuppet.Active)
				{
					LatexMites.Bounce(b, ai);
					return;
				}
				LatexMites.Begin(b, ai);
				return;
			}
		}

		// Token: 0x060003AE RID: 942 RVA: 0x0003CE80 File Offset: 0x0003B080
		private static void Bounce(Body b, ParasiteAI ai)
		{
			Sound.Play("goo", ai.transform.position, false, true, ai.transform, 1f, 1.1f, false, false);
			if (ai.Rb)
			{
				ai.Rb.velocity = new Vector2(-Mathf.Sign(b.transform.position.x - ai.transform.position.x) * 4f, 3f);
			}
			ai.Flee(3f);
			if (Suit.Stage > 0 && !Suit.LatexPartial)
			{
				Relief.Say(b, "mite_slide");
			}
		}

		// Token: 0x060003AF RID: 943 RVA: 0x0003CF30 File Offset: 0x0003B130
		private static void Begin(Body b, ParasiteAI ai)
		{
			LatexMites.now = LatexMites.Phase.Standing;
			LatexMites.on.Clear();
			LatexMites.Hold(b, ai);
			LatexMites.timer = 0f;
			LatexMites.progress = 0f;
			LatexMites.presses = 0;
			LatexMites.need = (float)Mathf.Max(1, LatexMites.StandPresses.Value);
			LatexMites.limit = Mathf.Max(1f, LatexMites.StandSeconds.Value);
			LatexMites.lineTimer = 1.5f;
			LatexMites.facing = (b.isRight ? 1 : (-1));
			LatexMites.anchor = b.transform.position;
			Relief.ForceStop();
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			LatexMites.posed = Poses.All.ContainsKey("mite_pull") && HerPuppet.Begin(b);
			b.eyePanicTime = 1f;
			b.adrenaline += 20f;
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.8f, false, false);
			Sfx.PlayMix(b, 0.8f, new string[] { "slime_gloop", "wet_slap" });
			Relief.Say(b, "mite_grab");
			Plugin.Log.LogInfo(string.Format("A latex mite is on her: tear it off ({0} presses in {1:0} s)", LatexMites.need, LatexMites.limit));
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x0003D0B0 File Offset: 0x0003B2B0
		private static void Hold(Body b, ParasiteAI ai)
		{
			LatexMites.on.Add(ai);
			ai.enabled = false;
			if (ai.Rb)
			{
				ai.Rb.velocity = Vector2.zero;
				ai.Rb.simulated = false;
			}
			LatexMites.herOrder = (from r in b.GetComponentsInChildren<SpriteRenderer>(true)
				where r.enabled
				select r.sortingOrder).DefaultIfEmpty(0).Max();
			foreach (ParasiteAI parasiteAI in LatexMites.on)
			{
				if (parasiteAI && parasiteAI.Sr)
				{
					parasiteAI.Sr.sortingOrder = LatexMites.herOrder + 3;
				}
			}
			b.eyePanicTime = 0.8f;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0003D1C8 File Offset: 0x0003B3C8
		private static void GoDown(Body b)
		{
			if (LatexMites.posed)
			{
				HerPuppet.End();
				LatexMites.posed = false;
			}
			LatexMites.now = LatexMites.Phase.Falling;
			LatexMites.timer = 0f;
			LatexMites.progress *= 0.4f;
			LatexMites.need = (float)Mathf.Max(1, LatexMites.GroundPresses.Value);
			LatexMites.limit = Mathf.Max(1f, LatexMites.GroundSeconds.Value);
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			b.Ragdoll();
			float num = (float)(-(float)LatexMites.facing);
			foreach (Limb limb in b.limbs)
			{
				if (limb && !limb.dismembered && limb.rb)
				{
					string name = limb.name;
					bool flag = name.Contains("UpTorso") || name.Contains("Head") || name.Contains("Arm") || name.Contains("Hand");
					bool flag2 = name.Contains("Thigh") || name.Contains("Crus") || name.Contains("Foot");
					limb.rb.velocity = (flag ? new Vector2(num * 5f, 1.5f) : (flag2 ? new Vector2(-num * 2f, 3f) : new Vector2(num * 2f, 1f)));
				}
			}
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.7f, false, false);
			Sfx.Play(b, 0.8f, new string[] { "moan" });
			Relief.Say(b, "mite_down");
			Plugin.Log.LogInfo(string.Format("A second latex mite: she goes down on her back ({0} presses in {1:0} s)", LatexMites.need, LatexMites.limit));
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0003D3DC File Offset: 0x0003B5DC
		public static void Update(Body b, float dt)
		{
			if (!LatexMites.Busy)
			{
				return;
			}
			LatexMites.on.RemoveAll((ParasiteAI x) => !x);
			if (LatexMites.on.Count == 0)
			{
				LatexMites.End(b);
				return;
			}
			if (!b.alive)
			{
				LatexMites.LetGo(b);
				return;
			}
			LatexMites.timer += dt;
			LatexMites.pressFlash = Mathf.Max(0f, LatexMites.pressFlash - dt);
			LatexMites.progress = Mathf.Max(0f, LatexMites.progress - 0.06f * dt);
			if (LatexMites.now != LatexMites.Phase.Falling && Keys.Down(Plugin.RemoveKey.Value))
			{
				LatexMites.Press(b);
			}
			LatexMites.Glue(b);
			LatexMites.S.arousal = Mathf.Min(100f, LatexMites.S.arousal + 0.6f * (float)LatexMites.on.Count * dt);
			b.shock = Mathf.Max(b.shock, (LatexMites.now == LatexMites.Phase.Standing) ? 0f : 25f);
			switch (LatexMites.now)
			{
			case LatexMites.Phase.Standing:
				if (LatexMites.posed)
				{
					ValueTuple<ScenePose, ScenePose, float> valueTuple = Poses.At("mite_pull", 0f, 1f);
					ScenePose item = valueTuple.Item1;
					ScenePose item2 = valueTuple.Item2;
					HerPuppet.Show(item, item2, (LatexMites.pressFlash > 0f) ? 1f : (0.25f + 0.25f * Mathf.Sin(LatexMites.timer * 7f)), LatexMites.anchor, LatexMites.facing);
				}
				break;
			case LatexMites.Phase.Falling:
				if (LatexMites.timer >= 1.1f)
				{
					LatexMites.now = LatexMites.Phase.Ground;
					LatexMites.timer = 0f;
					LatexMites.lineTimer = 1f;
				}
				break;
			case LatexMites.Phase.Ground:
				LatexMites.HoldDown(b, dt);
				break;
			}
			if (LatexMites.now != LatexMites.Phase.Falling)
			{
				if ((LatexMites.lineTimer -= dt) <= 0f)
				{
					LatexMites.lineTimer = Random.Range(2.5f, 4f);
					Relief.Say(b, (LatexMites.now == LatexMites.Phase.Ground) ? "mite_ground" : "mite_struggle");
				}
				if (LatexMites.progress >= 1f)
				{
					LatexMites.Win(b);
					return;
				}
				if (LatexMites.timer >= LatexMites.limit)
				{
					LatexMites.Lose(b);
					return;
				}
			}
			BioWarning.Bar(LatexMites.progress, (LatexMites.now == LatexMites.Phase.Falling) ? 1f : (1f - LatexMites.timer / LatexMites.limit));
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x0003D64C File Offset: 0x0003B84C
		private static void Press(Body b)
		{
			if (!b.conscious)
			{
				return;
			}
			float num = Mathf.Clamp(b.stamina / 100f, 0.35f, 1f) * Mathf.Max(0.3f, Diff.Escape) * (1f - LatexMites.S.submission * 0.004f);
			LatexMites.progress += num / LatexMites.need;
			LatexMites.presses++;
			b.stamina = Mathf.Max(0f, b.stamina - 1.5f);
			LatexMites.pressFlash = 0.18f;
			if (LatexMites.presses % 3 == 0)
			{
				Sound.Play("goo", b.transform.position, false, true, b.transform, 0.6f, Random.Range(0.9f, 1.2f), false, false);
			}
			if (LatexMites.presses % 4 == 0)
			{
				Sfx.Play(b, 0.6f, new string[] { "effort", "moan_soft" });
			}
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0003D758 File Offset: 0x0003B958
		private static void Glue(Body b)
		{
			Limb limb = ((b.limbs.Length > 1) ? b.limbs[1] : null);
			Limb limb2 = b.LimbByName("DownTorso");
			for (int i = 0; i < LatexMites.on.Count; i++)
			{
				ParasiteAI parasiteAI = LatexMites.on[i];
				Limb limb3 = ((i == 0) ? limb : limb2);
				if (parasiteAI && limb3)
				{
					Vector2 vector = limb3.transform.position;
					float num = ((parasiteAI.Sr && parasiteAI.Sr.sprite) ? parasiteAI.Sr.sprite.bounds.size.y : 0.4f);
					Vector2 vector2 = ((LatexMites.now == LatexMites.Phase.Standing) ? (vector + new Vector2((float)LatexMites.facing * 0.22f, -num * 0.45f)) : (vector + Vector2.up * 0.12f));
					if (LatexMites.pressFlash > 0f)
					{
						vector2 += new Vector2((float)LatexMites.facing * 0.12f, 0.05f);
					}
					parasiteAI.transform.position = vector2;
					if (parasiteAI.Sr)
					{
						parasiteAI.Sr.flipX = LatexMites.facing < 0;
					}
				}
			}
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0003D8C8 File Offset: 0x0003BAC8
		private static void HoldDown(Body b, float dt)
		{
			Limb limb = b.limbs[1];
			if (!limb)
			{
				return;
			}
			Vector2 vector = limb.transform.position + Vector2.up * 0.35f;
			LatexMites.Spring(b.LimbByName("HandF"), vector + new Vector2((float)LatexMites.facing * 0.15f, (LatexMites.pressFlash > 0f) ? 0.25f : 0f), 7f, dt);
			LatexMites.Spring(b.LimbByName("HandB"), vector + new Vector2((float)(-(float)LatexMites.facing) * 0.1f, (LatexMites.pressFlash > 0f) ? 0.2f : 0f), 7f, dt);
			if ((LatexMites.kick -= dt) <= 0f)
			{
				LatexMites.kick = Random.Range(0.5f, 1.2f);
				foreach (string text in new string[] { "FootF", "FootB" })
				{
					Limb limb2 = b.LimbByName(text);
					if (limb2 && limb2.rb && !limb2.dismembered)
					{
						limb2.rb.velocity += new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(2f, 4f));
					}
				}
			}
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0003DA50 File Offset: 0x0003BC50
		private static void Spring(Limb l, Vector2 target, float stiffness, float dt)
		{
			if (!l || l.dismembered || !l.rb)
			{
				return;
			}
			Vector2 vector = (target - l.transform.position) * stiffness;
			l.rb.velocity = Vector2.Lerp(l.rb.velocity, Vector2.ClampMagnitude(vector, 10f), Mathf.Clamp01(dt * 10f));
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x0003DACC File Offset: 0x0003BCCC
		private static void Win(Body b)
		{
			Plugin.Log.LogInfo(string.Format("She tears the latex mite{0} off ({1} presses, {2:0.0} s)", (LatexMites.on.Count > 1) ? "s" : "", LatexMites.presses, LatexMites.timer));
			bool flag = LatexMites.now != LatexMites.Phase.Standing;
			LatexMites.Fling(b);
			Relief.Say(b, "mite_escape");
			RunStats.Escaped();
			LatexMites.End(b);
			if (flag)
			{
				b.shock = Mathf.Min(b.shock, 15f);
			}
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0003DB5C File Offset: 0x0003BD5C
		private static void Lose(Body b)
		{
			bool flag = LatexMites.now != LatexMites.Phase.Standing && LatexMites.on.Count >= 2;
			foreach (ParasiteAI parasiteAI in LatexMites.on)
			{
				if (parasiteAI)
				{
					Object.Destroy(parasiteAI.gameObject);
				}
			}
			LatexMites.on.Clear();
			LatexMites.End(b);
			if (flag)
			{
				Suit.BeginLatexPiece(b, 5, 4);
				LatexMites.S.latexSpread = Mathf.Max(0f, LatexMites.SpreadMinutes.Value - LatexMites.HeadMinutes.Value) * 60f;
				Relief.Say(b, "latexp_attach_5");
				Plugin.Log.LogInfo(string.Format("Two latex mites won: the latex is on all of her but her head (it closes over that in {0:0.#} min)", LatexMites.HeadMinutes.Value));
			}
			else
			{
				int num = Random.Range(1, 5);
				Suit.BeginLatexPiece(b, num, 1);
				LatexMites.S.latexSpread = 0f;
				Relief.Say(b, "latexp_attach_" + num.ToString());
				Plugin.Log.LogInfo("The latex mite runs into a piece on her: " + LatexMites.PieceName(num));
			}
			LatexMites.headWarned = false;
			LatexMites.idleLine = Random.Range(60f, 100f);
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.7f, false, false);
			Sfx.PlayMix(b, 0.85f, new string[] { "slime_slurp", "slime_gloop" });
			Sfx.Play(b, 0.8f, new string[] { "moan" });
			BioWarning.Show();
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x0003DD28 File Offset: 0x0003BF28
		private static void Fling(Body b)
		{
			int num = 0;
			foreach (ParasiteAI parasiteAI in LatexMites.on)
			{
				if (parasiteAI)
				{
					parasiteAI.enabled = true;
					if (parasiteAI.Rb)
					{
						parasiteAI.Rb.simulated = true;
						parasiteAI.Rb.velocity = new Vector2((float)((num++ % 2 == 0) ? LatexMites.facing : (-(float)LatexMites.facing)) * Random.Range(5f, 7f), Random.Range(4f, 6f));
					}
					parasiteAI.weakened = true;
					parasiteAI.Flee(7f);
					if (parasiteAI.Sr)
					{
						parasiteAI.Sr.sortingOrder = LatexMites.herOrder - 1;
					}
				}
			}
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 1.2f, false, false);
		}

		// Token: 0x060003BA RID: 954 RVA: 0x0003DE4C File Offset: 0x0003C04C
		private static void End(Body b)
		{
			if (LatexMites.posed)
			{
				HerPuppet.End();
				LatexMites.posed = false;
			}
			LatexMites.now = LatexMites.Phase.None;
			LatexMites.on.Clear();
			BioWarning.HideBar();
			if (b)
			{
				Traverse.Create(b).Field("movingAllowed").SetValue(true);
			}
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0003DEA4 File Offset: 0x0003C0A4
		public static void LetGo(Body b)
		{
			if (!LatexMites.Busy)
			{
				return;
			}
			LatexMites.Fling(b);
			LatexMites.End(b);
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060003BC RID: 956 RVA: 0x0003DEBA File Offset: 0x0003C0BA
		private static float StepSeconds
		{
			get
			{
				return Mathf.Max(0.1f, LatexMites.SpreadMinutes.Value) * 60f / 4f;
			}
		}

		// Token: 0x060003BD RID: 957 RVA: 0x0003DEDC File Offset: 0x0003C0DC
		public static string PieceName(int piece)
		{
			if (piece == 1)
			{
				return "stockings";
			}
			if (piece == 2)
			{
				return "tights";
			}
			if (piece == 3)
			{
				return "body";
			}
			if (piece == 4)
			{
				return "gloves";
			}
			if (piece != 5)
			{
				return "none";
			}
			return "all but her head";
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0003DF18 File Offset: 0x0003C118
		public static bool PieceCovers(int piece, int stage, string texName)
		{
			if (piece <= 0 || piece >= LatexMites.PieceSpread.Length)
			{
				return false;
			}
			string[][] array = LatexMites.PieceSpread[piece];
			int num = 0;
			while (num < stage && num < array.Length)
			{
				foreach (string text in array[num])
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

		// Token: 0x060003BF RID: 959 RVA: 0x0003DF80 File Offset: 0x0003C180
		public static void UpdatePiece(Body b, float dt)
		{
			LatexMites.S.latexSpread += dt;
			float num = Mathf.Max(0.1f, LatexMites.SpreadMinutes.Value) * 60f;
			int num2 = ((LatexMites.S.latexPiece == 5) ? ((LatexMites.S.latexSpread >= num) ? 5 : 4) : Mathf.Clamp(1 + Mathf.FloorToInt(LatexMites.S.latexSpread / LatexMites.StepSeconds), 1, 5));
			if (num2 >= 5)
			{
				Plugin.Log.LogInfo("The latex mite's " + LatexMites.PieceName(LatexMites.S.latexPiece) + " closes over her head: the living latex suit");
				LatexMites.S.latexPiece = 0;
				LatexMites.S.latexSpread = 0f;
				Suit.AttachLatex(b);
				return;
			}
			if (num2 > Suit.Stage)
			{
				Suit.LatexPieceStage(b, num2);
				Relief.Say(b, Suit.Enjoying ? "latexp_love" : ("latexp_stage" + num2.ToString()));
				BioWarning.Show();
				Plugin.Log.LogInfo(string.Format("The latex ({0}) spreads over her: stage {1}", LatexMites.PieceName(LatexMites.S.latexPiece), num2));
			}
			if (Suit.Stage >= 4 && !LatexMites.headWarned && num - LatexMites.S.latexSpread < 25f)
			{
				LatexMites.headWarned = true;
				Relief.Say(b, "latexp_head");
			}
			LatexMites.S.arousal = Mathf.Min(100f, LatexMites.S.arousal + 0.08f * (float)Suit.Stage * dt);
			LatexMites.S.lust = Mathf.Min(100f, LatexMites.S.lust + 0.02f * (float)Suit.Stage * dt);
			if ((LatexMites.idleLine -= dt) <= 0f)
			{
				LatexMites.idleLine = Random.Range(80f, 150f);
				Relief.Say(b, Suit.Enjoying ? "latexp_love" : "latexp_idle");
				Sfx.PlayMix(b, 0.35f, new string[] { "slime_squelch", "slime_slurp" });
			}
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0003E198 File Offset: 0x0003C398
		public static void PieceOff(Body b, string line)
		{
			Vector2 vector = (b.isRight ? Vector2.left : Vector2.right) * 1.2f;
			Suit.Strip(b);
			LatexMites.S.suitCooldown = 20f;
			ParasiteAI parasiteAI = Parasites.Spawn(b.transform.position + vector, 8, true);
			if (parasiteAI)
			{
				parasiteAI.Flee(8f);
			}
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.9f, false, false);
			Sfx.PlayMix(b, 0.8f, new string[] { "slime_bubble", "wet_slap" });
			Relief.Say(b, line);
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0003E264 File Offset: 0x0003C464
		public static string DebugSpawn(Body b, int n)
		{
			if (ParasiteArt.MiteFrames.Length == 0)
			{
				return "no mite frames (lewd/parasite/mite_*.png)";
			}
			int num = (b.isRight ? 1 : (-1));
			int num2 = 0;
			for (int i = 0; i < n; i++)
			{
				if (Parasites.Spawn(b.transform.position + new Vector2((float)num * (3f + (float)i * 0.6f), 0.6f), 8, false))
				{
					num2++;
				}
			}
			return string.Format("{0} latex mite{1} ahead of her", num2, (num2 == 1) ? "" : "s");
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0003E2FC File Offset: 0x0003C4FC
		public static string Info(Body b)
		{
			string text = "mites {0}; phase {1} {2:0.0}/{3:0.0}s, progress {4:P0}, on her {5}; ";
			object[] array = new object[6];
			array[0] = Parasites.Alive.Count<ParasiteAI>((ParasiteAI x) => x && x.Mite);
			array[1] = LatexMites.now;
			array[2] = LatexMites.timer;
			array[3] = LatexMites.limit;
			array[4] = LatexMites.progress;
			array[5] = LatexMites.on.Count;
			return string.Format(text, array) + string.Format("piece {0} stage {1} (partial {2}, full {3}), spread {4:0}s", new object[]
			{
				LatexMites.PieceName(LatexMites.S.latexPiece),
				Suit.Stage,
				Suit.LatexPartial,
				Suit.Latex,
				LatexMites.S.latexSpread
			});
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x0003E3F0 File Offset: 0x0003C5F0
		public static string DebugPiece(Body b, int piece)
		{
			if (Suit.Stage > 0)
			{
				Suit.Strip(b);
			}
			if (piece == 5)
			{
				Suit.BeginLatexPiece(b, 5, 4);
				LatexMites.S.latexSpread = Mathf.Max(0f, LatexMites.SpreadMinutes.Value - LatexMites.HeadMinutes.Value) * 60f;
			}
			else
			{
				Suit.BeginLatexPiece(b, Mathf.Clamp(piece, 1, 4), 1);
				LatexMites.S.latexSpread = 0f;
			}
			BioWarning.Show();
			return string.Format("latex piece {0} on her, stage {1}", LatexMites.PieceName(LatexMites.S.latexPiece), Suit.Stage);
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x0003E48E File Offset: 0x0003C68E
		public static string DebugSpread(Body b, float seconds)
		{
			if (!Suit.LatexPartial)
			{
				return "no latex piece on her";
			}
			LatexMites.S.latexSpread += seconds;
			return string.Format("spread +{0:0}s: now {1:0}s", seconds, LatexMites.S.latexSpread);
		}

		// Token: 0x0400035A RID: 858
		public const int Stockings = 1;

		// Token: 0x0400035B RID: 859
		public const int Tights = 2;

		// Token: 0x0400035C RID: 860
		public const int BodyPiece = 3;

		// Token: 0x0400035D RID: 861
		public const int Gloves = 4;

		// Token: 0x0400035E RID: 862
		public const int AllButHead = 5;

		// Token: 0x0400035F RID: 863
		private static readonly string[] AllButHeadKeys = new string[] { "UpTorso", "DownTorso", "Thigh", "Crus", "Foot", "UpArm", "DownArm", "Hand", "Tail" };

		// Token: 0x04000360 RID: 864
		private static readonly string[][][] PieceSpread = new string[][][]
		{
			null,
			new string[][]
			{
				new string[] { "Thigh", "Crus", "Foot" },
				new string[] { "DownTorso", "Tail" },
				new string[] { "UpTorso" },
				new string[] { "UpArm", "DownArm", "Hand" }
			},
			new string[][]
			{
				new string[] { "DownTorso", "Thigh", "Crus", "Foot", "Tail" },
				new string[] { "UpTorso" },
				new string[] { "UpArm" },
				new string[] { "DownArm", "Hand" }
			},
			new string[][]
			{
				new string[] { "UpTorso", "DownTorso" },
				new string[] { "Thigh", "UpArm", "Tail" },
				new string[] { "Crus", "DownArm" },
				new string[] { "Foot", "Hand" }
			},
			new string[][]
			{
				new string[] { "Hand", "DownArm" },
				new string[] { "UpArm" },
				new string[] { "UpTorso", "DownTorso" },
				new string[] { "Thigh", "Crus", "Foot", "Tail" }
			},
			new string[][]
			{
				LatexMites.AllButHeadKeys,
				new string[0],
				new string[0],
				new string[0]
			}
		};

		// Token: 0x04000361 RID: 865
		internal static ConfigEntry<float> PerLayer;

		// Token: 0x04000362 RID: 866
		internal static ConfigEntry<float> FirstLayer;

		// Token: 0x04000363 RID: 867
		internal static ConfigEntry<float> SpreadMinutes;

		// Token: 0x04000364 RID: 868
		internal static ConfigEntry<float> HeadMinutes;

		// Token: 0x04000365 RID: 869
		internal static ConfigEntry<float> StandSeconds;

		// Token: 0x04000366 RID: 870
		internal static ConfigEntry<float> GroundSeconds;

		// Token: 0x04000367 RID: 871
		internal static ConfigEntry<int> StandPresses;

		// Token: 0x04000368 RID: 872
		internal static ConfigEntry<int> GroundPresses;

		// Token: 0x04000369 RID: 873
		private static LatexMites.Phase now;

		// Token: 0x0400036A RID: 874
		private static readonly List<ParasiteAI> on = new List<ParasiteAI>();

		// Token: 0x0400036B RID: 875
		private static float timer;

		// Token: 0x0400036C RID: 876
		private static float progress;

		// Token: 0x0400036D RID: 877
		private static float need;

		// Token: 0x0400036E RID: 878
		private static float limit;

		// Token: 0x0400036F RID: 879
		private static float pressFlash;

		// Token: 0x04000370 RID: 880
		private static float lineTimer;

		// Token: 0x04000371 RID: 881
		private static float kick;

		// Token: 0x04000372 RID: 882
		private static int facing = 1;

		// Token: 0x04000373 RID: 883
		private static int herOrder;

		// Token: 0x04000374 RID: 884
		private static int presses;

		// Token: 0x04000375 RID: 885
		private static Vector2 anchor;

		// Token: 0x04000376 RID: 886
		private static bool posed;

		// Token: 0x04000377 RID: 887
		private static float idleLine = 90f;

		// Token: 0x04000378 RID: 888
		private static bool headWarned;

		// Token: 0x020001CB RID: 459
		private enum Phase
		{
			// Token: 0x040009C9 RID: 2505
			None,
			// Token: 0x040009CA RID: 2506
			Standing,
			// Token: 0x040009CB RID: 2507
			Falling,
			// Token: 0x040009CC RID: 2508
			Ground
		}
	}
}
