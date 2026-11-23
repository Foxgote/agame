using System;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000105 RID: 261
	internal static class SlimeBog
	{
		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060005BB RID: 1467 RVA: 0x0005C2AA File Offset: 0x0005A4AA
		// (set) Token: 0x060005BC RID: 1468 RVA: 0x0005C2B1 File Offset: 0x0005A4B1
		public static bool Stuck { get; private set; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060005BD RID: 1469 RVA: 0x0005C2B9 File Offset: 0x0005A4B9
		// (set) Token: 0x060005BE RID: 1470 RVA: 0x0005C2C0 File Offset: 0x0005A4C0
		public static bool Under { get; private set; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x0005C2C8 File Offset: 0x0005A4C8
		// (set) Token: 0x060005C0 RID: 1472 RVA: 0x0005C2CF File Offset: 0x0005A4CF
		public static bool Dark { get; private set; }

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x0005C2D7 File Offset: 0x0005A4D7
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x0005C2E0 File Offset: 0x0005A4E0
		public static void Update(Body b, float dt)
		{
			if (SlimeBog.attemptCooldown > 0f)
			{
				SlimeBog.attemptCooldown -= dt;
			}
			if (SlimeBog.graceTimer > 0f)
			{
				SlimeBog.graceTimer -= dt;
			}
			if (SlimeBog.weak > 0f)
			{
				SlimeBog.weak -= dt;
			}
			if (SlimeBog.haul > 0f && b.alive)
			{
				SlimeBog.Hauling(b, dt);
			}
			bool flag = b.limbs.Any<Limb>((Limb l) => !l.dismembered && SlimeFluid.At(l.transform.position));
			bool flag2 = SlimeFluid.At(b.limbs[1].transform.position);
			SlimeBog.Under = SlimeFluid.At(b.limbs[0].transform.position);
			bool stuck = SlimeBog.Stuck;
			bool flag3 = b.alive && (flag2 || (flag && SlimeBog.timeIn > 0f));
			SlimeBog.outTimer = (flag3 ? 0f : (SlimeBog.outTimer + dt));
			SlimeBog.Stuck = flag3 || (stuck && b.alive && SlimeBog.outTimer < 0.8f);
			if (!SlimeBog.Stuck)
			{
				if (stuck)
				{
					SlimeBog.Leave(b);
				}
				SlimeBog.timeIn = 0f;
				SlimeBog.Dark = false;
				return;
			}
			bool flag4;
			if (!SlimeFluid.DarkAt(b.limbs[1].transform.position))
			{
				flag4 = b.limbs.Any<Limb>((Limb l) => l && !l.dismembered && SlimeFluid.DarkAt(l.transform.position));
			}
			else
			{
				flag4 = true;
			}
			SlimeBog.Dark = flag4;
			if (!stuck)
			{
				SlimeBog.Enter(b);
			}
			SlimeBog.timeIn += dt;
			Burning.Douse(b);
			if (Keys.Down(Plugin.RemoveKey.Value))
			{
				SlimeBog.Struggle(b);
			}
			if (SlimeBog.graceTimer <= 0f)
			{
				float num = Mathf.Lerp(1.5f, 4.5f, Mathf.Clamp01(SlimeBog.timeIn / 20f)) * (1f + SlimeBog.S.submission * 0.005f);
				Vector2 vector = b.moveDir;
				if (Keys.Held(KeyBinds.GetBind("jump")) || Keys.Held(KeyBinds.GetBind("up")) || Keys.Held(32) || Keys.Held(119))
				{
					vector.y = 1f;
				}
				if (Bridge.SwimUp)
				{
					vector = Bridge.SwimDir;
				}
				if (b.conscious && vector.sqrMagnitude > 0.04f)
				{
					float num2 = Plugin.SlimeSwimSpeed.Value * ((b.stamina < 15f) ? 0.6f : 1f) * (1f - SlimeBog.S.submission * 0.003f) * (SlimeBog.Under ? 0.85f : 1f);
					Vector2 vector2 = (vector.normalized * num2 + Vector2.down * num * 0.12f) * dt;
					Vector2 vector3 = b.limbs[1].transform.position;
					int mask = LayerMask.GetMask(new string[] { "Ground" });
					if (vector2.x != 0f && Physics2D.Raycast(vector3, new Vector2(Mathf.Sign(vector2.x), 0f), Mathf.Abs(vector2.x) + 0.6f, mask))
					{
						vector2.x = 0f;
					}
					if (vector2.y != 0f && Physics2D.Raycast(vector3, new Vector2(0f, Mathf.Sign(vector2.y)), Mathf.Abs(vector2.y) + 0.6f, mask))
					{
						vector2.y = 0f;
					}
					if (Mathf.Abs(vector.x) > 0.3f && !SlimeBog.Under && SlimeBog.Mantle(b, Mathf.Sign(vector.x)))
					{
						return;
					}
					b.transform.position += vector2;
					b.rb.velocity = new Vector2(b.rb.velocity.x * 0.5f, Mathf.Max(0f, b.rb.velocity.y) * 0.5f);
					if (vector.y > 0.1f)
					{
						b.stamina = Mathf.Max(0f, b.stamina - 2.5f * dt);
					}
				}
				else
				{
					b.rb.velocity = new Vector2(b.rb.velocity.x * (1f - 2f * dt), Mathf.Min(b.rb.velocity.y, b.rb.velocity.y - num * dt));
					if (b.rb.velocity.y > 1f)
					{
						b.rb.velocity = new Vector2(b.rb.velocity.x, 1f);
					}
				}
			}
			b.wetness = Mathf.Min(100f, b.wetness + 5f * dt);
			SlimeBog.S.arousal = Mathf.Min(100f, SlimeBog.S.arousal + (SlimeBog.Under ? 1.6f : 1f) * (1f + SlimeBog.S.lust / 200f) * dt);
			SlimeBog.S.lust = Mathf.Min(100f, SlimeBog.S.lust + 0.15f * dt);
			SlimeBog.S.submission = Mathf.Min(100f, SlimeBog.S.submission + 0.03f * dt);
			if (SlimeBog.wetLoop)
			{
				SlimeBog.wetLoop.volume = Mathf.MoveTowards(SlimeBog.wetLoop.volume, SlimeBog.Under ? 0.3f : 0.45f, dt);
			}
			if ((SlimeBog.pulseTimer -= dt) <= 0f)
			{
				SlimeBog.pulseTimer = Random.Range(4f, 7f);
				SlimeBog.S.arousal = Mathf.Min(100f, SlimeBog.S.arousal + 6f);
				b.eatTime = Mathf.Max(b.eatTime, 0.5f);
				Sound.Play("goo", b.transform.position, false, true, b.transform, 0.6f, Random.Range(0.6f, 0.9f), false, false);
				Sfx.PlayMix(b, 0.55f, new string[] { "slime_squelch", "slime_slurp", "slime_bubble" });
				Sfx.Play(b, 0.7f, new string[]
				{
					SlimeBog.Under ? "muffled" : "moan_soft",
					"moan"
				});
			}
			if ((SlimeBog.lineTimer -= dt) <= 0f)
			{
				SlimeBog.lineTimer = Random.Range(6f, 11f);
				Relief.Say(b, (SlimeBog.Dark ? "hbog_" : "slime_") + (SlimeBog.Under ? "under" : "stuck"));
			}
			if (SlimeBog.Under)
			{
				b.bloodOxygen = Mathf.Max(0f, b.bloodOxygen - Plugin.SlimeSuffocation.Value * Diff.Lethal * dt);
			}
			if ((SlimeBog.suitRoll -= dt) <= 0f)
			{
				SlimeBog.suitRoll = 4f;
				float num3 = Plugin.SlimeSuitChance.Value * (1f + SlimeBog.S.submission * 0.01f + SlimeBog.S.corruption * 0.005f) * (SlimeBog.Dark ? 1.6f : 1f);
				if (Suit.Stage == 0 && !Suit.CoolingDown && Random.value < num3)
				{
					Suit.Attach(b, SlimeBog.Dark ? 7 : 6);
					Plugin.Log.LogInfo(SlimeBog.Dark ? "The hosts' slime crawls onto her" : "The slime crawls onto her as a sticky suit");
					return;
				}
				if (SlimeBog.Dark && Suit.Slime && !Suit.HostSlime && Random.value < num3 * 2f)
				{
					Suit.Darken(b);
				}
			}
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x0005CB6C File Offset: 0x0005AD6C
		private static void Enter(Body b)
		{
			Relief.ForceStop();
			b.eyePanicTime = 0.8f;
			b.adrenaline += 15f;
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.6f, false, false);
			Sfx.PlayMix(b, 0.85f, new string[] { "slime_gloop", "wet_slap" });
			SlimeBog.wetLoop = Sfx.Loop(b, "loop_wet");
			if (SlimeBog.wetLoop)
			{
				SlimeBog.wetLoop.volume = 0f;
				SlimeBog.wetLoop.pitch = 0.8f;
			}
			SlimeBog.pulseTimer = 3f;
			SlimeBog.lineTimer = 1.5f;
			SlimeBog.suitRoll = 4f;
			bool flag;
			if (!SlimeFluid.DarkAt(b.limbs[1].transform.position))
			{
				flag = b.limbs.Any<Limb>((Limb l) => l && !l.dismembered && SlimeFluid.DarkAt(l.transform.position));
			}
			else
			{
				flag = true;
			}
			SlimeBog.Dark = flag;
			Relief.Say(b, SlimeBog.Dark ? "hbog_enter" : "slime_enter");
			Plugin.Log.LogInfo("She fell into a slime bog" + (SlimeBog.Dark ? " of the hosts' slime" : ""));
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x0005CCD4 File Offset: 0x0005AED4
		private static void Leave(Body b)
		{
			if (SlimeBog.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(SlimeBog.wetLoop, 0.6f));
			}
			SlimeBog.wetLoop = null;
			SlimeBog.Under = false;
			if (b && b.alive)
			{
				Relief.Say(b, "slime_out");
				Plugin.Log.LogInfo(string.Format("Out of the slime after {0:0} s", SlimeBog.timeIn));
			}
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x0005CD50 File Offset: 0x0005AF50
		public static void LetGo(Body b)
		{
			if (!SlimeBog.Stuck)
			{
				return;
			}
			SlimeBog.graceTimer = 3f;
			SlimeBog.Haul(b);
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x0005CD6C File Offset: 0x0005AF6C
		private static bool Mantle(Body b, float side)
		{
			int mask = LayerMask.GetMask(new string[] { "Ground" });
			Vector2 vector = b.limbs[1].transform.position;
			float num = Scenes.FeetY(b);
			RaycastHit2D raycastHit2D = Physics2D.Raycast(vector + Vector2.down * 0.8f, Vector2.right * side, 1.2f, mask);
			if (!raycastHit2D)
			{
				return false;
			}
			Vector2 vector2;
			vector2..ctor(raycastHit2D.point.x + side * 0.6f, num + 3.2f);
			RaycastHit2D raycastHit2D2 = Physics2D.Raycast(vector2, Vector2.down, 3.4f, mask);
			if (!raycastHit2D2 || raycastHit2D2.distance < 0.05f)
			{
				return false;
			}
			float num2 = raycastHit2D2.point.y - num + 0.08f;
			if (num2 > 3f || num2 < 0f)
			{
				return false;
			}
			if (Physics2D.OverlapBox(new Vector2(vector2.x, raycastHit2D2.point.y + 2.1f), new Vector2(0.7f, 3.6f), 0f, mask))
			{
				return false;
			}
			b.transform.position += new Vector3(raycastHit2D.point.x + side * 0.7f - b.transform.position.x, num2, 0f);
			b.rb.velocity = Vector2.zero;
			Physics2D.SyncTransforms();
			SlimeBog.graceTimer = 1f;
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.8f, 1.1f, false, false);
			Plugin.Log.LogInfo("She hauled herself out over the edge of the slime");
			return true;
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x0005CF3D File Offset: 0x0005B13D
		private static void Haul(Body b)
		{
			SlimeBog.haul = 3f;
			SlimeBog.haulSide = SlimeBog.ShoreSide(b);
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x0005CF54 File Offset: 0x0005B154
		private static void Hauling(Body b, float dt)
		{
			SlimeBog.haul -= dt;
			bool flag = b.limbs.Any<Limb>((Limb l) => l && !l.dismembered && SlimeFluid.At(l.transform.position));
			int mask = LayerMask.GetMask(new string[] { "Ground" });
			Vector2 vector = b.limbs[1].transform.position;
			Vector2 vector2;
			if (flag)
			{
				vector2 = new Vector2(SlimeBog.haulSide * 0.5f, 1f).normalized;
				if (Physics2D.Raycast(vector, vector2, 1.2f, mask))
				{
					vector2 = Vector2.up;
				}
				if (Physics2D.Raycast(vector, Vector2.up, 1.6f, mask))
				{
					vector2..ctor(SlimeBog.haulSide, 0f);
				}
				b.transform.position += vector2 * 7f * dt;
				b.rb.velocity = Vector2.zero;
				Physics2D.SyncTransforms();
				return;
			}
			RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, Vector2.down, 4f, mask);
			if (raycastHit2D && raycastHit2D.distance < 4.6f && !SlimeFluid.At(raycastHit2D.point + Vector2.up * 0.4f) && !SlimeFluid.At(raycastHit2D.point + new Vector2(-SlimeBog.haulSide * 0.6f, 0.4f)))
			{
				SlimeBog.haul = 0f;
				return;
			}
			vector2 = new Vector2(SlimeBog.haulSide, 0.05f).normalized;
			if (Physics2D.Raycast(vector, vector2, 0.8f, mask))
			{
				vector2 = Vector2.up;
			}
			b.transform.position += vector2 * 5f * dt;
			b.rb.velocity = Vector2.zero;
			Physics2D.SyncTransforms();
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x0005D164 File Offset: 0x0005B364
		private static void Struggle(Body b)
		{
			if (SlimeBog.attemptCooldown > 0f || !b.conscious)
			{
				return;
			}
			SlimeBog.attemptCooldown = 1f;
			b.stamina = Mathf.Max(0f, b.stamina - 7f);
			float num = Plugin.SlimeEscapeChance.Value * Diff.Escape * ((b.stamina < 15f) ? 0.75f : 1f) * (SlimeBog.Under ? 0.75f : 1f) * (1f - SlimeBog.S.submission * 0.003f) * ((SlimeBog.S.orgasmTimer > 17f) ? 0.5f : 1f);
			num = Mathf.Max(num, Plugin.SlimeEscapeChance.Value * 0.35f);
			bool flag = Random.value < num;
			Plugin.Log.LogInfo(string.Format("Struggling in the slime: chance {0:P0}, {1}", num, flag ? "pulled free" : "held"));
			if (flag)
			{
				SlimeBog.graceTimer = 3f;
				SlimeBog.Haul(b);
				Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 1.2f, false, false);
				Sfx.PlayMix(b, 0.8f, new string[] { "slime_slurp", "slime_gloop" });
				Relief.Say(b, "slime_escape");
				return;
			}
			SlimeBog.timeIn += 1.5f;
			SlimeBog.S.arousal = Mathf.Min(100f, SlimeBog.S.arousal + 3f);
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.7f, 0.5f, false, false);
			Sfx.PlayMix(b, 0.7f, new string[] { "slime_squelch", "slime_bubble" });
			Relief.Say(b, "slime_fail");
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x0005D368 File Offset: 0x0005B568
		private static float ShoreSide(Body b)
		{
			Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(b.transform.position);
			while (SlimeFluid.At(vector2Int + Vector2Int.up))
			{
				vector2Int += Vector2Int.up;
			}
			for (int i = 1; i <= 20; i++)
			{
				if (!SlimeFluid.At(vector2Int + Vector2Int.left * i))
				{
					return -1f;
				}
				if (!SlimeFluid.At(vector2Int + Vector2Int.right * i))
				{
					return 1f;
				}
			}
			if (!b.isRight)
			{
				return -1f;
			}
			return 1f;
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x0005D40C File Offset: 0x0005B60C
		public static void DropPuddle(Body b)
		{
			SlimeFluid.Pour(b.transform.position + Vector2.down * 0.8f, 6, null);
		}

		// Token: 0x04000660 RID: 1632
		private static float outTimer;

		// Token: 0x04000661 RID: 1633
		private static float timeIn;

		// Token: 0x04000662 RID: 1634
		private static float attemptCooldown;

		// Token: 0x04000663 RID: 1635
		private static float graceTimer;

		// Token: 0x04000664 RID: 1636
		private static float suitRoll = 4f;

		// Token: 0x04000665 RID: 1637
		private static float pulseTimer = 3f;

		// Token: 0x04000666 RID: 1638
		private static float lineTimer = 2f;

		// Token: 0x04000667 RID: 1639
		private static float weak;

		// Token: 0x04000668 RID: 1640
		private static AudioSource wetLoop;

		// Token: 0x04000669 RID: 1641
		private static float haul;

		// Token: 0x0400066A RID: 1642
		private static float haulSide;
	}
}
