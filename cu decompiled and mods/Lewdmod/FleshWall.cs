using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200012B RID: 299
	internal static class FleshWall
	{
		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060006F1 RID: 1777 RVA: 0x0006C25B File Offset: 0x0006A45B
		// (set) Token: 0x060006F2 RID: 1778 RVA: 0x0006C262 File Offset: 0x0006A462
		public static LewdWall Holder { get; private set; }

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060006F3 RID: 1779 RVA: 0x0006C26A File Offset: 0x0006A46A
		public static bool Holding
		{
			get
			{
				return FleshWall.Holder != null;
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060006F4 RID: 1780 RVA: 0x0006C277 File Offset: 0x0006A477
		// (set) Token: 0x060006F5 RID: 1781 RVA: 0x0006C27E File Offset: 0x0006A47E
		public static float Progress { get; private set; }

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060006F6 RID: 1782 RVA: 0x0006C286 File Offset: 0x0006A486
		public static bool Choking
		{
			get
			{
				return FleshWall.Holding && FleshWall.Progress >= 0.75f;
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060006F7 RID: 1783 RVA: 0x0006C2A0 File Offset: 0x0006A4A0
		// (set) Token: 0x060006F8 RID: 1784 RVA: 0x0006C2A7 File Offset: 0x0006A4A7
		public static int Orgasms { get; private set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060006F9 RID: 1785 RVA: 0x0006C2AF File Offset: 0x0006A4AF
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x0006C2B8 File Offset: 0x0006A4B8
		public static void Update(Body b, float dt)
		{
			if (FleshWall.grabCooldown > 0f)
			{
				FleshWall.grabCooldown -= dt;
			}
			if (FleshWall.attemptCooldown > 0f)
			{
				FleshWall.attemptCooldown -= dt;
			}
			if (b.alive && FleshWall.frozen.Count > 0)
			{
				FleshWall.Unentomb(b);
			}
			if (FleshWall.Holding && (!FleshWall.Holder || !b.alive))
			{
				if (!b.alive && FleshWall.Holder)
				{
					Plugin.Log.LogInfo(string.Format("She died in the flesh wall ({0} orgasms, absorbed {1:P0})", FleshWall.Orgasms, FleshWall.Progress));
					FleshWall.Entomb(b);
					return;
				}
				FleshWall.Release(b, false);
				return;
			}
			else
			{
				if (!FleshWall.Holding)
				{
					FleshWall.Lure(b, dt);
					return;
				}
				if (Keys.Down(Plugin.RemoveKey.Value))
				{
					FleshWall.Struggle(b);
				}
				if (FleshWall.Holding)
				{
					FleshWall.Absorb(b, dt);
				}
				return;
			}
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x0006C3AC File Offset: 0x0006A5AC
		private static bool Busy()
		{
			return Flowers.Holding || Flesh.Holding || Pits.InMaw || Pits.InThroat || Slits.Holding || Slits.Inside || Tentacles.Holding || Worm.Inside || Webs.Caught || SlimeBog.Stuck || External.ScavHolding || Womb.InLabor;
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0006C410 File Offset: 0x0006A610
		private static void Lure(Body b, float dt)
		{
			if (!b.alive || FleshWall.Busy())
			{
				return;
			}
			Vector2 vector = b.limbs[1].transform.position;
			using (List<LewdWall>.Enumerator enumerator = Walls.All.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LewdWall w = enumerator.Current;
					float num;
					if (w && w.Kind == WallKind.Flesh && w.sated <= 0f && w.Near(vector, 5f, out num))
					{
						float num2 = 1f - Mathf.Clamp01(num / 5f);
						b.rb.velocity += new Vector2((float)(-(float)w.Facing) * 12f * num2 * dt, 0f);
						float num3 = (from l in b.limbs
							where l && !l.dismembered
							select l.transform.position into q
							where Mathf.Abs(q.y - w.Face.y) < w.Height * 0.5f + 0.2f
							select (q.x - w.Face.x) * (float)w.Facing).DefaultIfEmpty(99f).Min();
						if ((FleshWall.nearLog -= dt) <= 0f)
						{
							FleshWall.nearLog = 1f;
							Plugin.Log.LogInfo(string.Format("Near a flesh wall: torso {0:0.00} out, closest limb {1:0.00}", num, num3));
						}
						if (num3 < 0.75f && FleshWall.grabCooldown <= 0f)
						{
							FleshWall.Catch(b, w);
							break;
						}
					}
				}
			}
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0006C61C File Offset: 0x0006A81C
		private static void Catch(Body b, LewdWall w)
		{
			FleshWall.Holder = w;
			FleshWall.Progress = 0f;
			FleshWall.Orgasms = 0;
			FleshWall.attemptCooldown = 0.6f;
			FleshWall.pulseTimer = 2f;
			FleshWall.lineTimer = 4f;
			FleshWall.orgasmGap = 0f;
			Relief.ForceStop();
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			FleshWall.savedGravity = b.rb.gravityScale;
			FleshWall.pinY = Mathf.Clamp(b.transform.position.y, w.Face.y - w.Height * 0.5f + 0.8f, w.Face.y + w.Height * 0.5f);
			b.eyePanicTime = 1f;
			b.adrenaline += 30f;
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.5f, false, false);
			Sound.Play("fleshrip", b.transform.position, false, true, b.transform, 0.5f, 1f, false, false);
			Plugin.Log.LogInfo("A flesh wall caught her");
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			FleshWall.engulf = new GameObject("LewdWallEngulf").AddComponent<SpriteRenderer>();
			FleshWall.engulf.sprite = ((WallArt.Engulf.Length != 0) ? WallArt.Engulf[0] : null);
			FleshWall.engulf.sharedMaterial = w.GetComponent<SpriteRenderer>().sharedMaterial;
			FleshWall.engulf.sortingLayerID = component.sortingLayerID;
			FleshWall.engulf.sortingOrder = b.GetComponentsInChildren<SpriteRenderer>(true).Max<SpriteRenderer>((SpriteRenderer r) => r.sortingOrder) + 10;
			FleshWall.wetLoop = Sfx.Loop(b, "loop_wet");
			Relief.Say(b, "fwall_grab");
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x0006C828 File Offset: 0x0006AA28
		private static void Absorb(Body b, float dt)
		{
			LewdWall holder = FleshWall.Holder;
			float num = 1f / Mathf.Max(5f, Plugin.FleshWallAbsorbSeconds.Value) * (1f + FleshWall.S.submission * 0.004f);
			FleshWall.Progress = Mathf.Min(1f, FleshWall.Progress + num * dt);
			Vector3 position = b.transform.position;
			float num2 = holder.Face.x + (float)holder.Facing * Mathf.Lerp(0.7f, 0.35f, FleshWall.Progress);
			position.x = Mathf.MoveTowards(position.x, num2, 2f * dt);
			position.y = Mathf.MoveTowards(position.y, FleshWall.pinY, 2f * dt);
			b.transform.position = position;
			b.rb.velocity = Vector2.zero;
			b.rb.gravityScale = 0f;
			foreach (Limb limb in b.limbs)
			{
				if (limb && !limb.dismembered && limb.rb)
				{
					if (!FleshWall.limbGravity.ContainsKey(limb))
					{
						FleshWall.limbGravity[limb] = limb.rb.gravityScale;
					}
					limb.rb.gravityScale = 0f;
					if (b.conscious || !FleshWall.limbPose.ContainsKey(limb))
					{
						FleshWall.limbPose[limb] = limb.rb.position - b.transform.position;
					}
					Vector2 vector = b.transform.position + FleshWall.limbPose[limb] - limb.rb.position;
					limb.rb.velocity = (b.conscious ? Vector2.ClampMagnitude(limb.rb.velocity, 1f) : (vector * 4f));
				}
			}
			FleshWall.PlaceEngulf(b, holder);
			if ((FleshWall.frameTime -= dt) <= 0f && WallArt.Engulf.Length != 0 && FleshWall.engulf)
			{
				FleshWall.frameTime = 0.4f;
				FleshWall.engulfFrame = (FleshWall.engulfFrame + 1) % WallArt.Engulf.Length;
				FleshWall.engulf.sprite = WallArt.Engulf[FleshWall.engulfFrame];
			}
			float num3 = Plugin.FleshWallArousal.Value * Mathf.Max(0.5f, Suppressant.Factor);
			FleshWall.S.arousal = Mathf.Min(100f, FleshWall.S.arousal + (1.5f + 7f * FleshWall.Progress) * num3 * dt);
			FleshWall.S.lust = Mathf.Min(100f, FleshWall.S.lust + 0.6f * dt);
			if (FleshWall.wetLoop)
			{
				FleshWall.wetLoop.volume = Mathf.MoveTowards(FleshWall.wetLoop.volume, 0.3f + 0.3f * FleshWall.Progress, dt);
			}
			if ((FleshWall.pulseTimer -= dt) <= 0f)
			{
				FleshWall.pulseTimer = Random.Range(3f, 5.5f) * (1.2f - FleshWall.Progress * 0.6f);
				FleshWall.S.arousal = Mathf.Min(100f, FleshWall.S.arousal + 6f * num3);
				Sfx.Play(b, 0.75f, new string[] { (FleshWall.Progress > 0.6f) ? "muffled" : "moan" });
				Sfx.Play(b, 0.4f, new string[] { "drip" });
				Semen.Add(b, 2f);
				Womb.Fill(b, 3f);
				if (FleshWall.Progress > 0.6f)
				{
					b.eatTime = Mathf.Max(b.eatTime, 0.6f);
				}
			}
			if ((FleshWall.lineTimer -= dt) <= 0f)
			{
				FleshWall.lineTimer = Random.Range(6f, 10f);
				Relief.Say(b, (FleshWall.Progress < 0.35f) ? "fwall_sink" : ((FleshWall.Progress < 0.75f) ? "fwall_scene" : "fwall_deep"));
			}
			FleshWall.orgasmGap -= dt;
			if (FleshWall.S.arousal >= 99f && FleshWall.orgasmGap <= 0f && b.conscious)
			{
				FleshWall.orgasmGap = 4f;
				Plugin.Orgasm(b, false);
				FleshWall.Orgasms++;
				FleshWall.S.arousal = Mathf.Max(FleshWall.S.arousal, 40f + 30f * FleshWall.Progress);
				b.bloodOxygen = Mathf.Max(0f, b.bloodOxygen - Plugin.FleshWallOrgasmAir.Value * Diff.Lethal);
				b.stamina = Mathf.Max(0f, b.stamina - 20f);
				Plugin.Log.LogInfo(string.Format("The flesh wall forced orgasm {0} out of her (absorbed {1:P0}, oxygen {2:0})", FleshWall.Orgasms, FleshWall.Progress, b.bloodOxygen));
				if (FleshWall.Orgasms >= Plugin.FleshWallOrgasmsToDeath.Value && Diff.LethalScenes)
				{
					Relief.Say(b, "fwall_overload");
					b.bloodOxygen = 0f;
				}
			}
			if (FleshWall.Progress >= 0.75f)
			{
				b.bloodOxygen = Mathf.Max(0f, b.bloodOxygen - Plugin.FleshWallSuffocation.Value * Diff.Lethal * ((FleshWall.Progress >= 1f) ? 1.5f : 1f) * dt);
			}
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x0006CE14 File Offset: 0x0006B014
		private static void PlaceEngulf(Body b, LewdWall w)
		{
			if (!FleshWall.engulf || !FleshWall.engulf.sprite)
			{
				return;
			}
			Bounds? bounds = null;
			foreach (SpriteRenderer spriteRenderer in b.GetComponentsInChildren<SpriteRenderer>())
			{
				if (spriteRenderer.enabled && spriteRenderer.sprite && !(spriteRenderer == FleshWall.engulf))
				{
					if (bounds == null)
					{
						bounds = new Bounds?(spriteRenderer.bounds);
					}
					else
					{
						Bounds value = bounds.Value;
						value.Encapsulate(spriteRenderer.bounds);
						bounds = new Bounds?(value);
					}
				}
			}
			if (bounds == null)
			{
				return;
			}
			Bounds value2 = bounds.Value;
			float num = Mathf.Abs(((w.Facing > 0) ? value2.max.x : value2.min.x) - w.Face.x) * 1.25f + 0.2f;
			float num2 = Mathf.Lerp(0.6f, num * 2f, Mathf.SmoothStep(0f, 1f, FleshWall.Progress));
			float num3 = value2.size.y * Mathf.Lerp(0.55f, 1.3f, FleshWall.Progress);
			Vector2 vector = FleshWall.engulf.sprite.bounds.size;
			FleshWall.engulf.transform.localScale = new Vector3(num2 / vector.x, num3 / vector.y, 1f);
			FleshWall.engulf.transform.position = new Vector3(w.Face.x, Mathf.Clamp(value2.center.y, FleshWall.pinY - 0.6f, FleshWall.pinY + 1.2f), 0f);
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x0006CFFC File Offset: 0x0006B1FC
		private static void Struggle(Body b)
		{
			if (FleshWall.attemptCooldown > 0f || !b.conscious)
			{
				return;
			}
			FleshWall.attemptCooldown = 1f;
			b.stamina = Mathf.Max(0f, b.stamina - 8f);
			float num = Plugin.FleshWallEscape.Value * Diff.Escape * (1f - FleshWall.Progress * 0.8f) * ((b.stamina < 15f) ? 0.5f : 1f) * (1f - FleshWall.S.submission * 0.003f) * ((FleshWall.S.orgasmTimer > 15f) ? 0.5f : 1f);
			bool flag = Random.value < num;
			Plugin.Log.LogInfo(string.Format("Struggling in the flesh wall: chance {0:P0}, {1}", num, flag ? "free" : "held"));
			if (flag)
			{
				FleshWall.Escaped(b);
				return;
			}
			FleshWall.Progress = Mathf.Min(1f, FleshWall.Progress + 0.03f);
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.8f, 0.6f, false, false);
			Relief.Say(b, "fwall_fail");
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0006D148 File Offset: 0x0006B348
		public static void OnHit(LewdWall w)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && FleshWall.Holder == w && Random.value < 0.2f * (1f - FleshWall.Progress * 0.7f))
			{
				FleshWall.Escaped(body);
			}
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x0006D1A8 File Offset: 0x0006B3A8
		public static void OnDestroyed(LewdWall w)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && FleshWall.Holder == w)
			{
				FleshWall.Release(body, true);
			}
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x0006D1EB File Offset: 0x0006B3EB
		public static void LetGo(Body b)
		{
			if (FleshWall.Holding)
			{
				FleshWall.Escaped(b);
			}
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x0006D1FC File Offset: 0x0006B3FC
		private static void Escaped(Body b)
		{
			LewdWall holder = FleshWall.Holder;
			FleshWall.Release(b, true);
			if (holder)
			{
				holder.sated = Mathf.Max(holder.sated, 10f);
			}
			FleshWall.grabCooldown = 6f;
			Relief.Say(b, "fwall_escape");
			Plugin.Log.LogInfo("She tore herself out of the flesh wall");
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x0006D258 File Offset: 0x0006B458
		public static void ReleaseIfHeld(Body b)
		{
			if (FleshWall.Holding)
			{
				FleshWall.Release(b, false);
			}
			FleshWall.Unentomb(b);
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x0006D270 File Offset: 0x0006B470
		private static void Entomb(Body b)
		{
			FleshWall.Holder = null;
			if (FleshWall.wetLoop)
			{
				b.StartCoroutine(Sfx.FadeOut(FleshWall.wetLoop, 1.5f));
			}
			FleshWall.wetLoop = null;
			FleshWall.tomb = FleshWall.engulf;
			FleshWall.engulf = null;
			foreach (Rigidbody2D rigidbody2D in new Rigidbody2D[] { b.rb }.Concat<Rigidbody2D>(from l in b.limbs
				where l && !l.dismembered && l.rb
				select l.rb))
			{
				rigidbody2D.velocity = Vector2.zero;
				rigidbody2D.angularVelocity = 0f;
				rigidbody2D.bodyType = 1;
				FleshWall.frozen.Add(rigidbody2D);
			}
			FleshWall.limbGravity.Clear();
			FleshWall.limbPose.Clear();
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x0006D38C File Offset: 0x0006B58C
		private static void Unentomb(Body b)
		{
			if (FleshWall.frozen.Count == 0 && !FleshWall.tomb)
			{
				return;
			}
			foreach (Rigidbody2D rigidbody2D in FleshWall.frozen)
			{
				if (rigidbody2D)
				{
					rigidbody2D.bodyType = 0;
				}
			}
			FleshWall.frozen.Clear();
			if (FleshWall.tomb)
			{
				Object.Destroy(FleshWall.tomb.gameObject);
			}
			FleshWall.tomb = null;
			if (b)
			{
				b.rb.gravityScale = ((FleshWall.savedGravity > 0f) ? FleshWall.savedGravity : 1f);
			}
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x0006D458 File Offset: 0x0006B658
		private static void Release(Body b, bool toss)
		{
			LewdWall holder = FleshWall.Holder;
			FleshWall.Holder = null;
			if (FleshWall.engulf)
			{
				Object.Destroy(FleshWall.engulf.gameObject);
			}
			FleshWall.engulf = null;
			if (FleshWall.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(FleshWall.wetLoop, 0.5f));
			}
			FleshWall.wetLoop = null;
			FleshWall.grabCooldown = Mathf.Max(FleshWall.grabCooldown, 3f);
			if (!b)
			{
				return;
			}
			Traverse.Create(b).Field("movingAllowed").SetValue(true);
			b.rb.gravityScale = ((FleshWall.savedGravity > 0f) ? FleshWall.savedGravity : 1f);
			foreach (KeyValuePair<Limb, float> keyValuePair in FleshWall.limbGravity)
			{
				if (keyValuePair.Key && keyValuePair.Key.rb)
				{
					keyValuePair.Key.rb.gravityScale = keyValuePair.Value;
				}
			}
			FleshWall.limbGravity.Clear();
			FleshWall.limbPose.Clear();
			if (toss && holder && b.alive)
			{
				b.transform.position = new Vector3(holder.Face.x + (float)holder.Facing * 1.3f, b.transform.position.y, 0f);
				Physics2D.SyncTransforms();
				b.rb.velocity = new Vector2((float)holder.Facing * 6f, 4f);
				b.wetness = Mathf.Max(b.wetness, 20f);
				Sound.Play("fleshrip", b.transform.position, false, true, b.transform, 0.7f, 1f, false, false);
			}
		}

		// Token: 0x04000743 RID: 1859
		private static float attemptCooldown;

		// Token: 0x04000744 RID: 1860
		private static float grabCooldown;

		// Token: 0x04000745 RID: 1861
		private static float pulseTimer;

		// Token: 0x04000746 RID: 1862
		private static float lineTimer;

		// Token: 0x04000747 RID: 1863
		private static float frameTime;

		// Token: 0x04000748 RID: 1864
		private static float orgasmGap;

		// Token: 0x04000749 RID: 1865
		private static float savedGravity;

		// Token: 0x0400074A RID: 1866
		private static int engulfFrame;

		// Token: 0x0400074B RID: 1867
		private static float pinY;

		// Token: 0x0400074C RID: 1868
		private static readonly Dictionary<Limb, float> limbGravity = new Dictionary<Limb, float>();

		// Token: 0x0400074D RID: 1869
		private static readonly Dictionary<Limb, Vector2> limbPose = new Dictionary<Limb, Vector2>();

		// Token: 0x0400074E RID: 1870
		private static SpriteRenderer engulf;

		// Token: 0x0400074F RID: 1871
		private static AudioSource wetLoop;

		// Token: 0x04000750 RID: 1872
		private static float nearLog;

		// Token: 0x04000751 RID: 1873
		private static SpriteRenderer tomb;

		// Token: 0x04000752 RID: 1874
		private static readonly List<Rigidbody2D> frozen = new List<Rigidbody2D>();
	}
}
