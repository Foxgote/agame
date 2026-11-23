using System;
using ScavArmory;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000A3 RID: 163
	internal static class HostConversion
	{
		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000323 RID: 803 RVA: 0x0003581B File Offset: 0x00033A1B
		// (set) Token: 0x06000324 RID: 804 RVA: 0x00035822 File Offset: 0x00033A22
		public static bool Active { get; private set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000325 RID: 805 RVA: 0x0003582A File Offset: 0x00033A2A
		// (set) Token: 0x06000326 RID: 806 RVA: 0x00035831 File Offset: 0x00033A31
		public static bool Done { get; private set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000327 RID: 807 RVA: 0x00035839 File Offset: 0x00033A39
		private static float TurnAt
		{
			get
			{
				return Mathf.Max(6f, Plugin.HostConversionSeconds.Value);
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000328 RID: 808 RVA: 0x0003584F File Offset: 0x00033A4F
		private static float FaintAt
		{
			get
			{
				return HostConversion.TurnAt * 0.7f;
			}
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0003585C File Offset: 0x00033A5C
		public static void Reset()
		{
			HostConversion.Active = (HostConversion.Done = false);
			HostConversion.hidden = null;
			HostConversion.CalmUntil = -1f;
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0003587C File Offset: 0x00033A7C
		public static void Begin(Body b)
		{
			if (HostConversion.Active)
			{
				return;
			}
			HostConversion.Active = true;
			HostConversion.Done = false;
			HostConversion.t = 0f;
			HostConversion.lineTimer = 1.5f;
			HostConversion.spoke2 = (HostConversion.spoke3 = (HostConversion.fainted = false));
			b.eyePanicTime = 1.5f;
			b.adrenaline += 30f;
			Sfx.PlayMix(b, 0.9f, new string[] { "slime_gloop", "slime_slurp" });
			Sfx.Play(b, 0.8f, new string[] { "muffled" });
			Relief.Say(b, "hslime_mask");
			SymbioteVoice.Say(b, "hivev_welcome", 1.2f);
			Plugin.Log.LogInfo("The host slime has closed over her face: she is becoming a host");
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00035944 File Offset: 0x00033B44
		public static void Update(Body b, float dt)
		{
			if (HostConversion.Done)
			{
				HostConversion.Hide(b);
				if (b && b.alive && (HostConversion.t += dt) > HostConversion.TurnAt + 5f)
				{
					b.brainHealth = 0f;
				}
				return;
			}
			if (!HostConversion.Active || !b)
			{
				return;
			}
			if (!b.alive)
			{
				HostConversion.Turn(b);
				return;
			}
			HostConversion.t += dt;
			Plugin.S.arousal = Mathf.Min(100f, Plugin.S.arousal + 8f * dt);
			b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 0.3f);
			if ((HostConversion.lineTimer -= dt) <= 0f && !HostConversion.fainted)
			{
				HostConversion.lineTimer = Random.Range(1.6f, 2.4f);
				Sfx.PlayMix(b, 0.6f, new string[] { "slime_squelch", "slime_bubble" });
				Sfx.Play(b, 0.7f, new string[] { "muffled", "moan_soft" });
			}
			if (!HostConversion.spoke3 && HostConversion.t > HostConversion.TurnAt * 0.45f)
			{
				HostConversion.spoke3 = true;
				Relief.Say(b, "hslime_fade");
			}
			if (!HostConversion.spoke2 && HostConversion.t > HostConversion.FaintAt)
			{
				HostConversion.spoke2 = true;
				Relief.Say(b, "hslime_last");
			}
			if (HostConversion.t > HostConversion.FaintAt)
			{
				HostConversion.fainted = true;
				b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 1f);
				b.rb.velocity = new Vector2(0f, b.rb.velocity.y);
			}
			if (HostConversion.t > HostConversion.TurnAt)
			{
				if (Diff.LethalScenes)
				{
					HostConversion.Turn(b);
					return;
				}
				HostConversion.Spare(b);
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600032C RID: 812 RVA: 0x00035B27 File Offset: 0x00033D27
		// (set) Token: 0x0600032D RID: 813 RVA: 0x00035B2E File Offset: 0x00033D2E
		public static float CalmUntil { get; private set; } = -1f;

		// Token: 0x0600032E RID: 814 RVA: 0x00035B36 File Offset: 0x00033D36
		public static void Abort(Body b)
		{
			if (HostConversion.Active && !HostConversion.Done && b)
			{
				HostConversion.Spare(b);
			}
		}

		// Token: 0x0600032F RID: 815 RVA: 0x00035B54 File Offset: 0x00033D54
		private static void Spare(Body b)
		{
			HostConversion.Active = false;
			Suit.Recede(b, 3);
			Heart.ReleaseAll(b, "the host slime let her go");
			Heart.KnockOut(b, Plugin.HostSparedKnockoutSeconds.Value, "hslime_spared");
			HostConversion.CalmUntil = Time.time + Plugin.HostSparedKnockoutSeconds.Value + 180f;
			Sfx.PlayMix(b, 0.8f, new string[] { "slime_slurp", "slime_gloop" });
			Plugin.Log.LogInfo("Deadly scenes are off: the host slime let her go (back to stage 3), she is out cold");
		}

		// Token: 0x06000330 RID: 816 RVA: 0x00035BDC File Offset: 0x00033DDC
		private static void Turn(Body b)
		{
			HostConversion.Done = true;
			Vector2 vector = b.transform.position;
			Scavenger.MakeHost(Scavengers.GroundNear(vector).GetValueOrDefault(vector), true, null);
			Scavengers.HostsStayNear(vector);
			Sfx.PlayMix(b, 1f, new string[] { "slime_gloop", "wet_slap" });
			HostConversion.hidden = b.GetComponentsInChildren<Renderer>(true);
			HostConversion.Hide(b);
			Plugin.Log.LogInfo("She is a slime host now: the run ends");
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00035C60 File Offset: 0x00033E60
		private static void Hide(Body b)
		{
			if (HostConversion.hidden == null)
			{
				return;
			}
			foreach (Renderer renderer in HostConversion.hidden)
			{
				if (renderer && renderer.enabled)
				{
					renderer.enabled = false;
				}
			}
		}

		// Token: 0x040002DD RID: 733
		private static float t;

		// Token: 0x040002DE RID: 734
		private static float lineTimer;

		// Token: 0x040002DF RID: 735
		private static bool spoke2;

		// Token: 0x040002E0 RID: 736
		private static bool spoke3;

		// Token: 0x040002E1 RID: 737
		private static bool fainted;

		// Token: 0x040002E2 RID: 738
		private static Renderer[] hidden;
	}
}
