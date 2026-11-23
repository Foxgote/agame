using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000AF RID: 175
	internal static class LatexTakeover
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060003EB RID: 1003 RVA: 0x0004054B File Offset: 0x0003E74B
		// (set) Token: 0x060003EC RID: 1004 RVA: 0x00040552 File Offset: 0x0003E752
		public static bool Active { get; private set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060003ED RID: 1005 RVA: 0x0004055A File Offset: 0x0003E75A
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00040561 File Offset: 0x0003E761
		public static void Reset()
		{
			LatexTakeover.Active = (LatexTakeover.done = false);
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x00040570 File Offset: 0x0003E770
		public static void Tick(Body b, float dt)
		{
			if (!b)
			{
				return;
			}
			if (LatexTakeover.done)
			{
				if (b.alive && (LatexTakeover.t += dt) > 11f)
				{
					b.brainHealth = 0f;
				}
				return;
			}
			if (!b.alive)
			{
				LatexTakeover.Active = false;
				return;
			}
			if (LatexTakeover.Active)
			{
				LatexTakeover.Run(b, dt);
				return;
			}
			float num = Mathf.Max(0.5f, Plugin.LatexTakeoverMinutes.Value);
			if (Suit.Latex)
			{
				LatexTakeover.S.latexCorruption = Mathf.Min(100f, LatexTakeover.S.latexCorruption + 100f / (num * 60f) * dt);
				if (LatexTakeover.S.latexCorruption >= 100f)
				{
					LatexTakeover.Begin(b);
					return;
				}
			}
			else if (LatexTakeover.S.latexCorruption > 0f)
			{
				LatexTakeover.S.latexCorruption = Mathf.Max(0f, LatexTakeover.S.latexCorruption - 100f / (num * 120f) * dt);
			}
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x00040674 File Offset: 0x0003E874
		private static void Begin(Body b)
		{
			LatexTakeover.Active = true;
			LatexTakeover.t = 0f;
			LatexTakeover.spoke = false;
			LatexTakeover.lineTimer = 1.5f;
			b.eyePanicTime = 1.5f;
			Relief.Say(b, "latex_take");
			SymbioteVoice.Say(b, "latexv_take", 1.4f);
			Plugin.Log.LogInfo("The living latex has corrupted her fully: it takes her will");
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x000406D8 File Offset: 0x0003E8D8
		private static void Run(Body b, float dt)
		{
			LatexTakeover.t += dt;
			LatexTakeover.S.arousal = Mathf.Min(100f, LatexTakeover.S.arousal + 6f * dt);
			LatexTakeover.S.submission = Mathf.Min(100f, LatexTakeover.S.submission + 10f * dt);
			if (b.rb)
			{
				b.rb.velocity = new Vector2(0f, b.rb.velocity.y);
			}
			if ((LatexTakeover.lineTimer -= dt) <= 0f)
			{
				LatexTakeover.lineTimer = Random.Range(1.8f, 2.6f);
				Sfx.Play(b, 0.6f, new string[] { "muffled", "moan_soft" });
			}
			if (!LatexTakeover.spoke && LatexTakeover.t > 4.4f)
			{
				LatexTakeover.spoke = true;
				Relief.Say(b, "latex_take_last");
				SymbioteVoice.Say(b, "latexv_take_last", 1.4f);
			}
			if (LatexTakeover.t > 5.6f)
			{
				b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 1f);
			}
			if (LatexTakeover.t < 8f)
			{
				return;
			}
			if (Diff.LethalScenes)
			{
				LatexTakeover.done = true;
				Plugin.Log.LogInfo("She is the living latex's now: the run ends");
				return;
			}
			LatexTakeover.Spare(b);
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00040840 File Offset: 0x0003EA40
		public static void Spare(Body b)
		{
			if (!LatexTakeover.Active || LatexTakeover.done || !b)
			{
				return;
			}
			LatexTakeover.Active = false;
			LatexTakeover.S.latexCorruption = 60f;
			Heart.KnockOut(b, 20f, "latex_take_spared");
			Plugin.Log.LogInfo(string.Format("The living latex let her mind go (deadly scenes off): its hold back to {0:0}", 60f));
		}

		// Token: 0x0400038F RID: 911
		private static bool done;

		// Token: 0x04000390 RID: 912
		private static bool spoke;

		// Token: 0x04000391 RID: 913
		private static float t;

		// Token: 0x04000392 RID: 914
		private static float lineTimer;

		// Token: 0x04000393 RID: 915
		private const float TurnAt = 8f;

		// Token: 0x04000394 RID: 916
		private const float Spared = 60f;
	}
}
