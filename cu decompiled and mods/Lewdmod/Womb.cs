using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000131 RID: 305
	internal static class Womb
	{
		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000741 RID: 1857 RVA: 0x0006FF38 File Offset: 0x0006E138
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000742 RID: 1858 RVA: 0x0006FF3F File Offset: 0x0006E13F
		public static int PregStage
		{
			get
			{
				if (!Womb.S.pregnant)
				{
					return 0;
				}
				if (Womb.S.pregProgress < 30f)
				{
					return 1;
				}
				if (Womb.S.pregProgress >= 70f)
				{
					return 3;
				}
				return 2;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000743 RID: 1859 RVA: 0x0006FF76 File Offset: 0x0006E176
		public static bool InLabor
		{
			get
			{
				return Womb.laborTimer > 0f;
			}
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x0006FF84 File Offset: 0x0006E184
		public static void Fill(Body b, float amount)
		{
			Womb.S.filled = Mathf.Min(100f, Womb.S.filled + amount);
			InvasionPopup.OnCumInside(b, amount);
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x0006FFB0 File Offset: 0x0006E1B0
		public static bool TryImpregnate(Body b, float mult = 1f)
		{
			if (Womb.S.pregnant || !b.alive)
			{
				return false;
			}
			float num = Plugin.PregnancyChance.Value * mult * (Womb.S.inHeat ? 3f : 1f) * (1f + Womb.S.filled / 100f);
			bool flag = Random.value < Mathf.Clamp01(num);
			Plugin.Log.LogInfo(string.Format("Tentacle seed: impregnation chance {0:P0}, {1}", Mathf.Clamp01(num), flag ? "it took" : "nothing"));
			if (!flag)
			{
				return false;
			}
			Womb.S.pregnant = true;
			Womb.S.pregProgress = 0f;
			Womb.S.brood = Random.Range(2, 5);
			Womb.symptomTimer = Random.Range(60f, 120f);
			return true;
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x00070091 File Offset: 0x0006E291
		public static void Update(Body b, float dt)
		{
			Womb.UpdateFill(b, dt);
			if (Womb.S.pregnant)
			{
				Womb.UpdatePregnancy(b, dt);
			}
			else
			{
				Womb.laborTimer = 0f;
			}
			Belly.Update(b);
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x000700C0 File Offset: 0x0006E2C0
		private static void UpdateFill(Body b, float dt)
		{
			if (Womb.S.filled <= 0f)
			{
				return;
			}
			float num = Plugin.FillDrain.Value * (b.standing ? 1f : 0.5f) * ((Mathf.Abs(b.rb.velocity.x) > 2f) ? 1.5f : 1f);
			Womb.S.filled = Mathf.Max(0f, Womb.S.filled - num * dt);
			Womb.S.lust = Mathf.Min(100f, Womb.S.lust + 0.02f * (Womb.S.filled / 100f) * dt);
			if (Womb.S.filled > 50f && Womb.S.corruption < 50f)
			{
				Mood.Change(b, -0.01f * dt);
			}
			if (Womb.S.filled > 8f && (Womb.leakTimer -= dt) <= 0f)
			{
				Womb.leakTimer = Random.Range(6f, 12f) * (1.5f - Womb.S.filled / 100f);
				Sfx.Play(b, 0.35f, new string[] { "drip" });
				Semen.Add(b, 1.5f);
				if (Womb.S.filled > 40f && Random.value < 0.3f)
				{
					Relief.Say(b, "fill_leak");
				}
			}
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x00070250 File Offset: 0x0006E450
		private static void UpdatePregnancy(Body b, float dt)
		{
			Womb.S.inHeat = false;
			Womb.S.heatTimer = 0f;
			if (Womb.InLabor)
			{
				Womb.UpdateLabor(b, dt);
				return;
			}
			int pregStage = Womb.PregStage;
			Womb.S.pregProgress += 100f / Mathf.Max(1f, Plugin.PregnancyMinutes.Value * 60f) * (b.sleeping ? 1.5f : 1f) * dt;
			if (Womb.PregStage != pregStage)
			{
				Relief.Say(b, "preg_stage" + Womb.PregStage.ToString());
				Plugin.Log.LogInfo(string.Format("Tentacle brood: stage {0} ({1:0}%)", Womb.PregStage, Womb.S.pregProgress));
			}
			int pregStage2 = Womb.PregStage;
			b.hunger = Mathf.Max(0f, b.hunger - 0.004f * (float)pregStage2 * dt);
			b.thirst = Mathf.Max(0f, b.thirst - 0.003f * (float)pregStage2 * dt);
			Womb.S.lust = Mathf.Min(100f, Womb.S.lust + 0.02f * (float)pregStage2 * dt);
			Womb.S.corruption = Mathf.Min(100f, Womb.S.corruption + 0.002f * (float)pregStage2 * dt);
			if ((Womb.symptomTimer -= dt) <= 0f)
			{
				Womb.symptomTimer = Random.Range(70f, 140f);
				if (pregStage2 == 1)
				{
					b.sicknessAmount += Random.Range(3f, 6f);
					Relief.Say(b, "preg_sick");
				}
				else
				{
					Relief.Say(b, "preg_line" + pregStage2.ToString() + Random.Range(1, 3).ToString());
				}
			}
			if (pregStage2 == 3 && (Womb.kickTimer -= dt) <= 0f)
			{
				Womb.kickTimer = Random.Range(25f, 55f);
				Limb limb = b.LimbByName("DownTorso");
				if (limb)
				{
					limb.pain = Mathf.Min(100f, limb.pain + 6f);
				}
				b.eatTime = Mathf.Max(b.eatTime, 0.5f);
				Sfx.Play(b, 0.55f, new string[] { "moan_soft" });
				Relief.Say(b, "preg_kick");
			}
			if (Womb.S.pregProgress >= 100f && b.conscious && !Scenes.Held)
			{
				Womb.StartLabor(b);
			}
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x000704FA File Offset: 0x0006E6FA
		private static void StartLabor(Body b)
		{
			Womb.laborTimer = 22f;
			Womb.laborLine = 0f;
			Relief.ForceStop();
			b.Ragdoll();
			Relief.Say(b, "preg_labor");
			Plugin.Log.LogInfo("Labour starts");
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00070538 File Offset: 0x0006E738
		private static void UpdateLabor(Body b, float dt)
		{
			Womb.laborTimer -= dt;
			b.shock = Mathf.Max(b.shock, 22f);
			Limb limb = b.LimbByName("DownTorso");
			if (limb)
			{
				limb.pain = Mathf.Min(100f, limb.pain + 3f * dt);
			}
			b.stamina = Mathf.Max(0f, b.stamina - 2f * dt);
			Womb.S.arousal = Mathf.Min(100f, Womb.S.arousal + 1.5f * dt);
			if ((Womb.laborLine -= dt) <= 0f)
			{
				Womb.laborLine = Random.Range(3f, 5f);
				b.eatTime = Mathf.Max(b.eatTime, 1f);
				Sfx.Play(b, 0.9f, new string[] { "moan", "climax" });
				Relief.Say(b, "preg_push");
			}
			if (Womb.laborTimer <= 0f)
			{
				Womb.Birth(b);
			}
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x0007065C File Offset: 0x0006E85C
		private static void Birth(Body b)
		{
			Womb.laborTimer = 0f;
			int num = Mathf.Max(1, Womb.S.brood);
			Vector2 vector = (b.LimbByName("DownTorso") ? b.LimbByName("DownTorso").transform.position : b.transform.position);
			for (int i = 0; i < num; i++)
			{
				Larva.Spawn(vector + new Vector2(Random.Range(-0.3f, 0.3f), 0.1f), i == 0 && Random.value < Plugin.BroodRootChance.Value);
			}
			Sound.Play("goo", vector, false, true, b.transform, 1f, 0.7f, false, false);
			Sfx.Play(b, 1f, new string[] { "climax" });
			Womb.S.pregnant = false;
			Womb.S.pregProgress = 0f;
			Womb.S.brood = 0;
			Womb.S.filled = 0f;
			Womb.S.births++;
			Womb.S.corruption = Mathf.Min(100f, Womb.S.corruption + 10f);
			Womb.S.submission = Mathf.Min(100f, Womb.S.submission + 5f);
			Womb.S.orgasmTimer = Mathf.Max(Womb.S.orgasmTimer, 10f);
			b.stamina = 0f;
			Mood.Change(b, (Womb.S.corruption > 50f) ? 10f : (-20f));
			InvasionPopup.Quiet = true;
			Semen.Add(b, 30f);
			InvasionPopup.Quiet = false;
			Relief.Say(b, (Womb.S.corruption > 50f) ? "preg_born_corrupt" : "preg_born");
			Plugin.Log.LogInfo(string.Format("Gave birth to {0} tentacle larvae (birth #{1})", num, Womb.S.births));
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x00070880 File Offset: 0x0006EA80
		public static void DebugAdvance(Body b)
		{
			if (!Womb.S.pregnant)
			{
				Womb.S.pregnant = true;
				Womb.S.pregProgress = 0f;
				Womb.S.brood = Random.Range(2, 5);
			}
			else
			{
				Womb.S.pregProgress = Mathf.Min(99.5f, Womb.S.pregProgress + 25f);
			}
			Plugin.Log.LogInfo(string.Format("Debug: brood at {0:0}% (stage {1})", Womb.S.pregProgress, Womb.PregStage));
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x0600074D RID: 1869 RVA: 0x00070918 File Offset: 0x0006EB18
		public static int BellyLevel
		{
			get
			{
				int num = ((Womb.S.filled > 70f) ? 3 : ((Womb.S.filled > 40f) ? 2 : ((Womb.S.filled > 15f) ? 1 : 0)));
				int num2 = ((!Womb.S.pregnant) ? 0 : ((Womb.S.pregProgress < 20f) ? 0 : ((Womb.S.pregProgress < 40f) ? 1 : ((Womb.S.pregProgress < 60f) ? 2 : ((Womb.S.pregProgress < 80f) ? 3 : ((Womb.S.pregProgress < 95f) ? 4 : 5))))));
				return Mathf.Max(num, num2);
			}
		}

		// Token: 0x04000775 RID: 1909
		private static float leakTimer = 5f;

		// Token: 0x04000776 RID: 1910
		private static float symptomTimer = 60f;

		// Token: 0x04000777 RID: 1911
		private static float kickTimer = 40f;

		// Token: 0x04000778 RID: 1912
		private static float laborTimer;

		// Token: 0x04000779 RID: 1913
		private static float laborLine;
	}
}
