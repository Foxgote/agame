using System;
using ScavArmory;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000A0 RID: 160
	internal static class Heart
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000318 RID: 792 RVA: 0x000351ED File Offset: 0x000333ED
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000319 RID: 793 RVA: 0x000351F4 File Offset: 0x000333F4
		public static bool KnockedOut
		{
			get
			{
				return Heart.knockout > 0f;
			}
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00035204 File Offset: 0x00033404
		public static void Overworked(Body b, float dt)
		{
			if (!b || !b.alive || Heart.KnockedOut)
			{
				return;
			}
			if (Time.time - Heart.overworkedAt > 2f)
			{
				Heart.worstFib = 0f;
			}
			Heart.overworkedAt = Time.time;
			b.heartRate = Mathf.Max(b.heartRate, 190f);
			if (b.fibrillationProgress <= 0f)
			{
				b.TryStartFibrillation(true);
				Plugin.Log.LogInfo("Exhausted past her limit: her heart goes into fibrillation");
			}
			b.fibrillationForced = true;
			b.fibrillationProgress = Mathf.Min(100f, b.fibrillationProgress + Plugin.ExhaustionDeathRate.Value * dt);
			Heart.worstFib = Mathf.Max(Heart.worstFib, b.fibrillationProgress);
		}

		// Token: 0x0600031B RID: 795 RVA: 0x000352C8 File Offset: 0x000334C8
		public static void Update(Body b, float dt)
		{
			if (!b)
			{
				return;
			}
			if (Heart.knockout <= 0f)
			{
				if (b.alive && Time.time - Heart.overworkedAt < 1.5f && Heart.worstFib > 10f && b.fibrillationProgress <= 0.5f)
				{
					Heart.worstFib = 0f;
					Heart.overworkedAt = -100f;
					Plugin.Log.LogInfo("Her heart was brought back while she was being worked to death: she passes out, it lets her go");
					Heart.ReleaseAll(b, "revived");
					b.fibrillationForced = false;
					b.fibrillationProgress = 0f;
					b.heartRate = 100f;
					Heart.S.exhaustion = Mathf.Min(Heart.S.exhaustion, 90f);
					Heart.knockout = Mathf.Max(5f, Plugin.ReviveKnockoutSeconds.Value);
					Relief.Say(b, "revived_out");
				}
				return;
			}
			if (!b.alive)
			{
				Heart.knockout = 0f;
				return;
			}
			Heart.knockout -= dt;
			b.painShock = 1f;
			b.fibrillationForced = false;
			if (b.fibrillationProgress > 0f)
			{
				b.fibrillationProgress = 0f;
			}
			b.heartRate = Mathf.Min(b.heartRate, 110f);
			b.bloodOxygen = Mathf.Max(b.bloodOxygen, 70f);
			if (Heart.knockout <= 0f)
			{
				b.painShock = 0.5f;
				if (Heart.wakeLine != null)
				{
					Relief.Say(b, Heart.wakeLine);
				}
				Heart.wakeLine = null;
				Plugin.Log.LogInfo("She comes to");
			}
		}

		// Token: 0x0600031C RID: 796 RVA: 0x00035470 File Offset: 0x00033670
		public static void ReleaseAll(Body b, string why)
		{
			bool held = Scenes.Held;
			Tentacles.LetGo(b);
			Worm.LetGo(b);
			Flesh.LetGo(b);
			FleshWall.LetGo(b);
			Pits.LetGo(b);
			Flowers.LetGo(b);
			Webs.LetGo(b);
			SlimeBog.LetGo(b);
			LatexPond.LetGo(b);
			LatexMites.LetGo(b);
			if (Capture.Active)
			{
				Capture.Release(b, false, true);
			}
			if (held)
			{
				Plugin.Log.LogInfo("Released from every scene: " + why);
			}
		}

		// Token: 0x0600031D RID: 797 RVA: 0x000354E4 File Offset: 0x000336E4
		public static void KnockOut(Body b, float seconds, string onWake = null)
		{
			if (!b || !b.alive)
			{
				return;
			}
			Heart.knockout = Mathf.Max(Heart.knockout, Mathf.Max(5f, seconds));
			Heart.wakeLine = onWake;
		}

		// Token: 0x0600031E RID: 798 RVA: 0x00035517 File Offset: 0x00033717
		public static void Reset()
		{
			Heart.knockout = 0f;
			Heart.worstFib = 0f;
			Heart.overworkedAt = -100f;
			Heart.wakeLine = null;
		}

		// Token: 0x040002D7 RID: 727
		private static float overworkedAt = -100f;

		// Token: 0x040002D8 RID: 728
		private static float worstFib;

		// Token: 0x040002D9 RID: 729
		private static float knockout;

		// Token: 0x040002DA RID: 730
		private static string wakeLine;
	}
}
