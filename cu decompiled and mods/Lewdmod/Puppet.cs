using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x020000EB RID: 235
	internal static class Puppet
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600053E RID: 1342 RVA: 0x00056F62 File Offset: 0x00055162
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x0600053F RID: 1343 RVA: 0x00056F69 File Offset: 0x00055169
		public static bool Active
		{
			get
			{
				return Puppet.S.suitStage >= 5 && !Suit.Symbiote && !Suit.Drone && !Suit.Slime && !Suit.Latex;
			}
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00056F98 File Offset: 0x00055198
		public static void TryBegin(Body b, float chanceMultiplier)
		{
			if (Puppet.S.suitStage != 4 || Suit.Symbiote || Suit.Drone || Suit.Slime || Puppet.S.suitTamed || Puppet.S.takeoverCooldown > 0f || !b.conscious)
			{
				return;
			}
			float num = Plugin.TakeoverChance.Value * chanceMultiplier * (1f + Puppet.S.submission * 0.01f + Puppet.S.corruption * 0.01f);
			if (Random.value < num)
			{
				Puppet.Begin(b);
			}
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x00057030 File Offset: 0x00055230
		public static void Begin(Body b)
		{
			Puppet.S.suitStage = 5;
			Puppet.S.willpower = 0f;
			Puppet.S.takeoverTime = 0f;
			Puppet.phaseTimer = 4f;
			Puppet.Facing = (b.isRight ? 1 : (-1));
			b.eyePanicTime = 1f;
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.6f, false, false);
			Sfx.Play(b, 0.9f, new string[] { "muffled", "moan" });
			Relief.Say(b, "puppet_begin");
			Plugin.Log.LogInfo("Suit takes over her body (stage 5)");
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x000570F8 File Offset: 0x000552F8
		public static void Release(Body b, bool sated)
		{
			if (!Puppet.Active)
			{
				return;
			}
			Puppet.S.suitStage = 4;
			Puppet.S.suitProgress = 0f;
			Puppet.S.takeoverCooldown = 180f;
			Puppet.S.willpower = 0f;
			Puppet.MoveDir = Vector2.zero;
			Relief.ForceStop();
			if (Puppet.eyeGlow)
			{
				Puppet.eyeGlow.enabled = false;
			}
			Plugin.Log.LogInfo("She is herself again (" + (sated ? "the suit is sated" : "she broke free") + ")");
			if (sated)
			{
				Relief.Say(b, "puppet_sated");
				return;
			}
			Puppet.S.takeoverEscapes++;
			int num = Mathf.Max(1, Plugin.TameEscapes.Value);
			Plugin.Log.LogInfo(string.Format("Takeovers she broke out of: {0}/{1}", Puppet.S.takeoverEscapes, num));
			if (Puppet.S.takeoverEscapes >= num)
			{
				Puppet.Tame(b);
				return;
			}
			Puppet.S.takeoverRetry = true;
			Relief.Say(b, (Puppet.S.takeoverEscapes >= num - 1) ? "puppet_free_last" : "puppet_free");
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x00057230 File Offset: 0x00055430
		private static void Tame(Body b)
		{
			Puppet.S.suitTamed = true;
			Puppet.S.takeoverRetry = false;
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.8f, 1.2f, false, false);
			Sfx.Play(b, 0.7f, new string[] { "sigh" });
			Relief.Say(b, "suit_tamed");
			Plugin.Log.LogInfo("The living cloth gives in: it obeys her now (no more takeovers, stage-5 armour, faster healing)");
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x000572B8 File Offset: 0x000554B8
		private static void Retry(Body b)
		{
			if (!Puppet.S.takeoverRetry || Puppet.S.takeoverCooldown > 0f || Puppet.S.suitStage != 4 || Puppet.S.suitTamed || Suit.Symbiote || Suit.Slime)
			{
				return;
			}
			if (!b.conscious || b.sleeping || Scenes.Busy || Relief.Active)
			{
				return;
			}
			Puppet.S.takeoverRetry = false;
			Plugin.Log.LogInfo(string.Format("The living cloth tries to take her again ({0} escapes so far)", Puppet.S.takeoverEscapes));
			Puppet.Begin(b);
			Relief.Say(b, "puppet_again");
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x00057368 File Offset: 0x00055568
		public static void Struggle(Body b)
		{
			float num = 9f * (1f - Puppet.S.submission / 200f) * ((b.stamina > 10f) ? 1f : 0.6f);
			int num2 = Mathf.FloorToInt(Puppet.S.willpower / 25f);
			Puppet.S.willpower = Mathf.Min(100f, Puppet.S.willpower + num);
			b.stamina = Mathf.Max(0f, b.stamina - 1.5f);
			if (Mathf.FloorToInt(Puppet.S.willpower / 25f) > num2)
			{
				Plugin.Log.LogInfo(string.Format("Willpower {0:0}%", Puppet.S.willpower));
			}
			if (Time.time > Puppet.struggleLine)
			{
				Puppet.struggleLine = Time.time + 4f;
				Relief.Say(b, "puppet_struggle");
			}
			if (Puppet.S.willpower >= 100f)
			{
				Puppet.Release(b, false);
			}
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x00057478 File Offset: 0x00055678
		public static void OnOrgasm(Body b)
		{
			if (Puppet.Active)
			{
				Puppet.S.willpower = Mathf.Max(0f, Puppet.S.willpower - 25f);
			}
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x000574A8 File Offset: 0x000556A8
		public static void Update(Body b, float dt)
		{
			Puppet.UpdateGlow(b);
			if (Puppet.S.takeoverCooldown > 0f)
			{
				Puppet.S.takeoverCooldown -= dt;
			}
			if (!Puppet.Active)
			{
				Puppet.MoveDir = Vector2.zero;
				Puppet.Retry(b);
				return;
			}
			Puppet.S.takeoverTime += dt;
			Puppet.S.willpower = Mathf.Max(0f, Puppet.S.willpower - 1.5f * dt);
			if (Puppet.S.takeoverTime > Plugin.TakeoverMinutes.Value * 60f && !Relief.Active)
			{
				Puppet.Release(b, true);
				return;
			}
			if (!b.conscious)
			{
				Puppet.MoveDir = Vector2.zero;
				return;
			}
			if (Relief.Active || Scenes.Busy)
			{
				Puppet.MoveDir = Vector2.zero;
				return;
			}
			Puppet.phaseTimer -= dt;
			if (Puppet.phaseTimer <= 0f && b.standing)
			{
				Puppet.phaseTimer = Random.Range(8f, 16f) / Suit.Touch;
				Puppet.MoveDir = Vector2.zero;
				Relief.StartForced(b);
				return;
			}
			Puppet.Wander(b, dt);
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x000575D8 File Offset: 0x000557D8
		private static void Wander(Body b, float dt)
		{
			int mask = LayerMask.GetMask(new string[] { "Ground" });
			Vector2 vector = b.transform.position;
			Vector2 vector2;
			vector2..ctor((float)Puppet.Facing, 0f);
			bool flag = Physics2D.Raycast(vector, vector2, 1.4f, mask);
			if (!Physics2D.Raycast(vector + vector2 * 1.2f, Vector2.down, 5f, mask))
			{
				Puppet.Facing = -Puppet.Facing;
			}
			else if (flag)
			{
				Puppet.stuckTimer += dt;
				if (Puppet.stuckTimer > 0.4f && Puppet.stuckTimer < 0.5f)
				{
					Puppet.AllowJump = true;
					b.Jump();
					Puppet.AllowJump = false;
				}
				if (Puppet.stuckTimer > 1.6f)
				{
					Puppet.Facing = -Puppet.Facing;
					Puppet.stuckTimer = 0f;
				}
			}
			else
			{
				Puppet.stuckTimer = 0f;
			}
			if (Random.value < dt / 25f)
			{
				Puppet.Facing = -Puppet.Facing;
			}
			Puppet.MoveDir = new Vector2((float)Puppet.Facing * 0.55f, 0f);
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x000576FC File Offset: 0x000558FC
		private static void UpdateGlow(Body b)
		{
			bool flag = Suit.Drone && Puppet.S.suitStage >= 5;
			if (!Puppet.Active && !flag)
			{
				if (Puppet.eyeGlow)
				{
					Puppet.eyeGlow.enabled = false;
				}
				return;
			}
			if (!Puppet.eyeGlow)
			{
				FacialExpression componentInChildren = b.GetComponentInChildren<FacialExpression>(true);
				if (!componentInChildren || !componentInChildren.head)
				{
					return;
				}
				GameObject gameObject = new GameObject("LewdMaskGlow");
				gameObject.transform.SetParent(componentInChildren.head.transform, false);
				Puppet.eyeGlow = gameObject.AddComponent<Light2D>();
				Puppet.eyeGlow.lightType = 3;
				Puppet.eyeGlow.intensity = 1.2f;
				Puppet.eyeGlow.pointLightOuterRadius = 0.9f;
				Puppet.eyeGlow.pointLightInnerRadius = 0.05f;
			}
			Puppet.eyeGlow.enabled = true;
			Puppet.eyeGlow.color = Color.Lerp(ParasiteArt.Colors[Mathf.Clamp(Puppet.S.suitColor, 0, 2)], Color.white, 0.3f);
			Puppet.eyeGlow.intensity = 1f + 0.4f * Mathf.Sin(Time.time * 2.5f);
		}

		// Token: 0x0400061F RID: 1567
		public static Vector2 MoveDir;

		// Token: 0x04000620 RID: 1568
		public static int Facing = 1;

		// Token: 0x04000621 RID: 1569
		public static bool AllowJump;

		// Token: 0x04000622 RID: 1570
		private static float phaseTimer = 6f;

		// Token: 0x04000623 RID: 1571
		private static float stuckTimer;

		// Token: 0x04000624 RID: 1572
		private static float struggleLine;

		// Token: 0x04000625 RID: 1573
		private static Light2D eyeGlow;
	}
}
