using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PoseKit;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200008F RID: 143
	internal static class Flowers
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x0003084E File Offset: 0x0002EA4E
		// (set) Token: 0x060002C3 RID: 707 RVA: 0x00030855 File Offset: 0x0002EA55
		public static FlowerPlant Holder { get; private set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x0003085D File Offset: 0x0002EA5D
		public static bool Holding
		{
			get
			{
				return Flowers.Holder;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x00030869 File Offset: 0x0002EA69
		// (set) Token: 0x060002C6 RID: 710 RVA: 0x00030870 File Offset: 0x0002EA70
		public static float Blind { get; private set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x00030878 File Offset: 0x0002EA78
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0003087F File Offset: 0x0002EA7F
		public static bool Drives(FlowerPlant p)
		{
			return p && (p == Flowers.Holder || p == Flowers.reaching);
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x000308A8 File Offset: 0x0002EAA8
		public static FlowerPlant Plant(Pit p)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				FlowerArt.Ensure(spriteRenderer.sprite.pixelsPerUnit / Mathf.Max(0.01f, Mathf.Abs(spriteRenderer.transform.lossyScale.y)));
			}
			if (!FlowerArt.Ready)
			{
				return null;
			}
			GameObject gameObject = new GameObject("LewdFlower");
			gameObject.transform.position = p.Mouth + Vector2.up * 0.5f;
			FlowerPlant f = gameObject.AddComponent<FlowerPlant>();
			f.pit = p;
			p.Plant = f;
			f.flip = Random.value < 0.5f;
			f.sr = gameObject.AddComponent<SpriteRenderer>();
			f.Set("idle", 0);
			if (WorldGeneration.world)
			{
				f.sr.sharedMaterial = WorldGeneration.world.defaultMat;
			}
			if (spriteRenderer)
			{
				f.sr.sortingLayerID = spriteRenderer.sortingLayerID;
				f.sr.sortingOrder = spriteRenderer.sortingOrder - 15;
			}
			BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
			boxCollider2D.isTrigger = false;
			Bounds bounds = f.sr.sprite.bounds;
			boxCollider2D.size = new Vector2(bounds.size.x * 0.7f, bounds.size.y * 0.8f);
			f.boxX = bounds.center.x;
			boxCollider2D.offset = new Vector2(f.flip ? (-bounds.center.x) : bounds.center.x, bounds.center.y);
			gameObject.layer = LayerMask.NameToLayer("Descriptor");
			BuildingEntity buildingEntity = gameObject.AddComponent<BuildingEntity>();
			buildingEntity.animal = true;
			buildingEntity.health = 100000f;
			buildingEntity.id = "lewd_flower";
			buildingEntity.ignoreBodyOptimize = true;
			buildingEntity.itemsDropOnDestroy = new ItemDrop[0];
			buildingEntity.alwaysDrop = new ItemDrop[0];
			buildingEntity.itemCategoriesToAdd = new string[0];
			buildingEntity.hitSound = Resources.Load<AudioClip>("Sounds/goo");
			Burning.Make(gameObject, "carnivorous flower", Plugin.FlowerHealth.Value, f.sr, delegate
			{
				Flowers.OnHit(f, 0f);
			}, delegate
			{
				Flowers.Kill(f);
			});
			Flowers.Plants.Add(f);
			Plugin.Log.LogInfo(string.Format("A carnivorous flower at {0} ({1})", gameObject.transform.position, p.Milk ? "it milks" : "it digests"));
			return f;
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00030BC8 File Offset: 0x0002EDC8
		public static void Clear(Body b)
		{
			if (Flowers.Holding && b)
			{
				Flowers.Release(b, true);
			}
			Flowers.reaching = null;
			foreach (FlowerPlant flowerPlant in Flowers.Plants.ToArray())
			{
				if (flowerPlant)
				{
					Object.Destroy(flowerPlant.gameObject);
				}
			}
			Flowers.Plants.Clear();
			Flowers.DestroyLimbs();
			Flowers.blackout = 0f;
			Flowers.Blind = 0f;
			if (Flowers.veil)
			{
				Object.Destroy(Flowers.veil);
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00030C5C File Offset: 0x0002EE5C
		private static Vector2 Reach(FlowerPlant p)
		{
			Vector2[] array;
			if (!FlowerArt.Meta("flower_lunge_3", out array))
			{
				return new Vector2(4f, 4f);
			}
			Vector2 vector = FlowerArt.Offset(array[3], false);
			Vector2 vector2 = FlowerArt.Offset(array[2], false);
			return new Vector2(vector.x, vector2.y);
		}

		// Token: 0x060002CC RID: 716 RVA: 0x00030CB4 File Offset: 0x0002EEB4
		public static void Update(Body b, float dt)
		{
			Flowers.Plants.RemoveAll((FlowerPlant x) => !x);
			foreach (FlowerPlant flowerPlant in Flowers.Plants)
			{
				if (flowerPlant.burrow == 2 && (flowerPlant.underTimer -= dt) <= 0f && Mathf.Abs(b.transform.position.x - flowerPlant.transform.position.x) > 3f)
				{
					Flowers.Emerge(b, flowerPlant);
				}
			}
			if (Flowers.grabCooldown > 0f)
			{
				Flowers.grabCooldown -= dt;
			}
			if (Flowers.attemptCooldown > 0f)
			{
				Flowers.attemptCooldown -= dt;
			}
			Flowers.Veil(dt);
			if (Flowers.Holding && (!Flowers.Holder || !b.alive))
			{
				Flowers.Release(b, !b.alive);
				return;
			}
			if (Flowers.Holding)
			{
				if (!Flowers.pulling && Keys.Down(Plugin.RemoveKey.Value))
				{
					Flowers.Struggle(b);
				}
				if (Flowers.Holding)
				{
					Flowers.Hold(b, dt);
				}
				return;
			}
			if (Flowers.reaching)
			{
				Flowers.UpdateReach(b, dt);
				return;
			}
			if (!b.alive || Flowers.grabCooldown > 0f || Scenes.Busy || HerPuppet.Active)
			{
				return;
			}
			Vector2 vector = b.limbs[1].transform.position;
			foreach (FlowerPlant flowerPlant2 in Flowers.Plants)
			{
				if (flowerPlant2 && !flowerPlant2.dead && !flowerPlant2.Down && flowerPlant2.pit != null && !flowerPlant2.pit.Dead && Pits.Holder != flowerPlant2.pit)
				{
					Vector2 vector2 = flowerPlant2.transform.position;
					float num = vector.x - vector2.x;
					if (Mathf.Abs(num) <= 16f && Mathf.Abs(vector.y - vector2.y) <= 10f)
					{
						if (Mathf.Abs(num) > 0.5f)
						{
							flowerPlant2.Face(num < 0f);
						}
						if (flowerPlant2.sated <= 0f && !flowerPlant2.Turning)
						{
							Vector2 vector3 = Flowers.Reach(flowerPlant2);
							float num2 = Mathf.Abs(num);
							if (num2 > 0.6f && num2 < vector3.x + 0.6f && vector.y > vector2.y - 1f && vector.y < vector2.y + vector3.y)
							{
								Flowers.reaching = flowerPlant2;
								Flowers.reachTime = 0f;
								Sound.Play("megabite", vector2, false, true, null, 0.7f, 0.9f, false, false);
								Sound.Play("goo", vector2, false, true, null, 0.8f, 0.7f, false, false);
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00031040 File Offset: 0x0002F240
		private static void UpdateReach(Body b, float dt)
		{
			FlowerPlant flowerPlant = Flowers.reaching;
			if (!flowerPlant || flowerPlant.dead)
			{
				Flowers.reaching = null;
				Flowers.HideLimbs();
				return;
			}
			Flowers.reachTime += dt;
			float num = Mathf.Clamp01(Flowers.reachTime / 0.55f);
			flowerPlant.Set("lunge", Mathf.Min(3, (int)(num * 4f)));
			Flowers.EnsureLimbs(b);
			Vector2 vector = flowerPlant.Point(1);
			Vector2 vector2 = Flowers.Waist(b);
			Flowers.Chain(Flowers.tongue, vector, Vector2.Lerp(vector, vector2, Mathf.Clamp01(num * 1.5f - 0.3f)), 1f, 0.6f, false);
			if (Flowers.reachTime < 0.55f)
			{
				return;
			}
			Flowers.reaching = null;
			float num2 = Mathf.Abs(b.limbs[1].transform.position.x - flowerPlant.transform.position.x);
			if (b.alive && !Scenes.Busy && !HerPuppet.Active && num2 < Flowers.Reach(flowerPlant).x + 1.6f && Vector2.Distance(vector, vector2) < Flowers.Reach(flowerPlant).x + 2.5f)
			{
				Flowers.Grab(b, flowerPlant);
				return;
			}
			flowerPlant.sated = 3f;
			Flowers.HideLimbs();
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00031188 File Offset: 0x0002F388
		private static void Grab(Body b, FlowerPlant p)
		{
			Flowers.Holder = p;
			Flowers.holdTime = 0f;
			Flowers.pulling = false;
			Flowers.attemptCooldown = 0.6f;
			Flowers.lineTimer = 3f;
			Flowers.suckTimer = 1f;
			Relief.ForceStop();
			b.Ragdoll();
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			b.eyePanicTime = 1f;
			b.adrenaline += 30f;
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.5f, false, false);
			Flowers.wetLoop = Sfx.Loop(b, "loop_wet");
			if (Flowers.wetLoop)
			{
				Flowers.wetLoop.volume = 0f;
			}
			Relief.Say(b, "flower_grab");
			Plugin.Log.LogInfo(string.Format("The carnivorous flower caught her ({0:0} s to get free before it swallows her)", Plugin.FlowerHoldSeconds.Value));
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00031298 File Offset: 0x0002F498
		private static Vector2 Waist(Body b)
		{
			Limb limb = b.LimbByName("DownTorso");
			if (!limb || limb.dismembered)
			{
				return b.limbs[1].transform.position;
			}
			return limb.transform.position;
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x000312EC File Offset: 0x0002F4EC
		private static void Hold(Body b, float dt)
		{
			FlowerPlant holder = Flowers.Holder;
			b.shock = Mathf.Max(b.shock, 25f);
			if (b.rb)
			{
				b.rb.velocity = Vector2.zero;
			}
			if (Flowers.pulling)
			{
				Flowers.Pull(b, dt);
				return;
			}
			Flowers.holdTime += dt;
			holder.Set("hold", (int)(Time.time * 5f) % 4);
			Vector2 vector = holder.Point(0);
			Vector2 vector2 = holder.Point(1);
			Vector2 mouthFront = holder.MouthFront;
			Vector2 normalized = (mouthFront - vector).normalized;
			Vector2 vector3 = new Vector2(Mathf.Sin(Time.time * 1.3f), Mathf.Cos(Time.time * 1.1f)) * 0.1f;
			Vector2 vector4 = mouthFront + normalized * 0.6f + Vector2.down * 0.4f + vector3;
			Flowers.Spring(b, "DownTorso", vector4, 6f, dt);
			Flowers.Spring(b, "UpTorso", vector4 + Vector2.up * 0.9f - new Vector2(normalized.x, 0f) * 0.15f, 3.5f, dt);
			Flowers.Chain(Flowers.tongue, vector2, Flowers.Waist(b), 1.1f, 0.3f, true);
			float num = Mathf.Clamp01((Flowers.holdTime - 0.8f) / 1.2f);
			Limb limb = b.LimbByName("UpTorso");
			for (int i = 0; i < 2; i++)
			{
				Vector2 vector5 = ((limb && !limb.dismembered) ? limb.transform.TransformPoint((i == 0) ? Flowers.Nipple : Flowers.NippleFar) : vector4);
				Vector2 vector6 = vector2 + new Vector2(0f, (i == 0) ? 0.12f : (-0.12f));
				Vector2 vector7 = Vector2.Lerp(vector6, vector5, num) + ((num >= 1f) ? (Random.insideUnitCircle * 0.015f) : Vector2.zero);
				Flowers.Chain(Flowers.milkers[i], vector6, vector7, 0.75f, 0.5f, false);
				Flowers.Tip(Flowers.suckers[i], Flowers.milkers[i], vector7, num > 0.2f);
			}
			if (num >= 1f)
			{
				Flowers.Milk(b, dt);
			}
			if ((Flowers.lineTimer -= dt) <= 0f)
			{
				Flowers.lineTimer = Random.Range(6f, 10f);
				Relief.Say(b, (num < 1f) ? "flower_held" : ((Flowers.S.exhaustion > 60f) ? "flower_weak" : "flower_milk"));
			}
			if (Flowers.holdTime > Plugin.FlowerHoldSeconds.Value)
			{
				Flowers.pulling = true;
				Flowers.pullTime = 0f;
				Sound.Play("megabite", holder.transform.position, false, true, null, 1f, 0.6f, false, false);
				Relief.Say(b, "flower_gulp");
				Plugin.Log.LogInfo("The flower drags her into its mouth");
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00031638 File Offset: 0x0002F838
		public static void Milk(Body b, float dt)
		{
			Flowers.S.milkTimer = Mathf.Max(Flowers.S.milkTimer, 1.5f);
			Flowers.S.arousal = Mathf.Min(100f, Flowers.S.arousal + 2.2f * dt);
			Flowers.S.lust = Mathf.Min(100f, Flowers.S.lust + 0.3f * dt);
			Flowers.S.exhaustion = Mathf.Min(100f, Flowers.S.exhaustion + Plugin.FlowerMilkDrain.Value * dt);
			b.stamina = Mathf.Max(0f, b.stamina - 0.8f * dt);
			if (Flowers.wetLoop)
			{
				Flowers.wetLoop.volume = Mathf.MoveTowards(Flowers.wetLoop.volume, 0.5f, dt);
			}
			if ((Flowers.suckTimer -= dt) <= 0f)
			{
				Flowers.suckTimer = Random.Range(1.4f, 2.6f);
				Sfx.PlayMix(b, 0.6f, new string[] { "tentacle_suck" });
				if (Random.value < 0.4f)
				{
					Sfx.Play(b, 0.5f, new string[] { "moan_soft", "moan" });
				}
			}
			if (Flowers.S.arousal >= 99f && Flowers.S.orgasmTimer < 13f && b.conscious)
			{
				Plugin.Orgasm(b, false);
				Flowers.S.exhaustion = Mathf.Min(100f, Flowers.S.exhaustion + Plugin.ExhaustionPerOrgasm.Value);
				Flowers.S.arousal = Mathf.Max(Flowers.S.arousal, 55f);
				Relief.Say(b, "flower_orgasm");
				Plugin.Log.LogInfo(string.Format("The flower milked an orgasm out of her; exhaustion {0:0}", Flowers.S.exhaustion));
			}
			if (Flowers.S.exhaustion >= 100f)
			{
				Heart.Overworked(b, dt);
			}
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00031858 File Offset: 0x0002FA58
		private static void Pull(Body b, float dt)
		{
			FlowerPlant holder = Flowers.Holder;
			Flowers.pullTime += dt;
			float num = Mathf.Clamp01(Flowers.pullTime / 1.3f);
			holder.Set("swallow", 0);
			Vector2 vector = holder.Point(1);
			Vector2 mouthFront = holder.MouthFront;
			Vector2 vector2 = Vector2.Lerp(mouthFront, vector, num);
			Flowers.Spring(b, "Head", vector2, 9f, dt);
			Flowers.Spring(b, "UpTorso", vector2, 8f, dt);
			Flowers.Spring(b, "DownTorso", Vector2.Lerp(mouthFront + (mouthFront - vector).normalized * 0.8f, vector, num * 0.8f), 7f, dt);
			Flowers.Chain(Flowers.tongue, vector, Flowers.Waist(b), 1.1f, 0.2f, true);
			for (int i = 0; i < 2; i++)
			{
				Flowers.Chain(Flowers.milkers[i], vector, vector, 0.75f, 0f, false);
				if (Flowers.suckers[i])
				{
					Flowers.suckers[i].enabled = false;
				}
			}
			Flowers.Blind = Mathf.SmoothStep(0f, 2f, num);
			if (num < 1f)
			{
				return;
			}
			Flowers.blackout = 1f;
			Flowers.veilHold = 0.35f;
			Flowers.Holder = null;
			Flowers.pulling = false;
			Flowers.HideLimbs();
			if (Flowers.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(Flowers.wetLoop, 0.5f));
			}
			Flowers.wetLoop = null;
			holder.swallowAt = Time.time;
			Sound.Play("eatFlesh", holder.transform.position, false, true, null, 1f, 0.6f, false, false);
			Pits.SwallowWhole(b, holder.pit);
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00031A28 File Offset: 0x0002FC28
		private static void Spring(Body b, string limbName, Vector2 target, float stiffness, float dt)
		{
			Limb limb = b.LimbByName(limbName);
			if (!limb || limb.dismembered || !limb.rb)
			{
				return;
			}
			Vector2 vector = (target - limb.transform.position) * stiffness;
			limb.rb.velocity = Vector2.Lerp(limb.rb.velocity, Vector2.ClampMagnitude(vector, 14f), Mathf.Clamp01(dt * 12f));
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00031AAC File Offset: 0x0002FCAC
		private static void Struggle(Body b)
		{
			if (Flowers.attemptCooldown > 0f || !b.conscious)
			{
				return;
			}
			Flowers.attemptCooldown = 1f;
			float num = Mathf.Clamp(b.stamina / 100f, 0.1f, 1f) * (1f - Flowers.S.exhaustion / 100f);
			float num2 = Mathf.Max(0.01f, Plugin.FlowerEscape.Value * Diff.Escape * num * ((Flowers.S.orgasmTimer > 17f) ? 0.3f : 1f) * (1f - Flowers.S.submission * 0.004f));
			b.stamina = Mathf.Max(0f, b.stamina - 10f);
			foreach (string text in new string[] { "FootF", "FootB", "HandF", "HandB" })
			{
				Limb limb = b.LimbByName(text);
				if (limb && !limb.dismembered && limb.rb)
				{
					limb.rb.velocity += Random.insideUnitCircle * 6f;
				}
			}
			bool flag = Random.value < num2;
			Plugin.Log.LogInfo(string.Format("Struggling in the flower's tongue: chance {0:P0} (strength {1:P0}), {2}", num2, num, flag ? "free" : "held"));
			if (flag)
			{
				FlowerPlant holder = Flowers.Holder;
				holder.sated = 20f;
				Relief.Say(b, "flower_escape");
				Flowers.Release(b, false);
				Vector2 vector;
				vector..ctor(holder.flip ? (-1f) : 1f, 0.6f);
				foreach (Limb limb2 in b.limbs)
				{
					if (limb2 && !limb2.dismembered && limb2.rb)
					{
						limb2.rb.velocity = vector * 4f;
					}
				}
				return;
			}
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.7f, 0.6f, false, false);
			Relief.Say(b, (Flowers.S.exhaustion > 60f) ? "flower_weak" : "flower_fail");
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00031D3C File Offset: 0x0002FF3C
		public static void OnHit(FlowerPlant p, float damage)
		{
			if (!p)
			{
				return;
			}
			BuildingEntity component = p.GetComponent<BuildingEntity>();
			if (component)
			{
				component.health = 100000f;
			}
			if (p.Down || (p.pit != null && Time.time - p.pit.SpatAt < 4f))
			{
				return;
			}
			p.hits = ((Time.time - p.lastHit < 6f) ? (p.hits + 1) : 1);
			p.lastHit = Time.time;
			if ((float)p.hits >= Plugin.FlowerBurrowHits.Value && (p.pit == null || Pits.Holder != p.pit))
			{
				Flowers.Burrow(p);
				return;
			}
			if (Flowers.Holder == p)
			{
				return;
			}
			p.sated = Mathf.Max(p.sated, Plugin.TentacleHurtSeconds.Value);
			Sound.Play("goo", p.transform.position, false, true, null, 0.9f, Random.Range(1.1f, 1.4f), false, false);
			if (Flowers.reaching == p)
			{
				Flowers.reaching = null;
				Flowers.HideLimbs();
				Flowers.grabCooldown = Mathf.Max(Flowers.grabCooldown, 2f);
			}
			Plugin.Log.LogInfo(string.Format("The flower flinches ({0:0}): no striking for {1:0} s", damage, p.sated));
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00031EA4 File Offset: 0x000300A4
		private static void Burrow(FlowerPlant p)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (Flowers.Holder == p)
			{
				Flowers.Release(body, false);
			}
			if (Flowers.reaching == p)
			{
				Flowers.reaching = null;
				Flowers.HideLimbs();
			}
			p.burrow = 1;
			p.sated = 0f;
			p.sink = 0f;
			p.hits = 0;
			BoxCollider2D component = p.GetComponent<BoxCollider2D>();
			if (component)
			{
				component.enabled = false;
			}
			Sound.Play("goo", p.transform.position, false, true, null, 1f, 0.45f, false, false);
			Sound.Play("megabite", p.transform.position, false, true, null, 0.6f, 0.6f, false, false);
			PlayerCamera main = PlayerCamera.main;
			if (main != null)
			{
				main.shaker.Shake(30f);
			}
			if (body && body.alive && Vector2.Distance(body.transform.position, p.transform.position) < 20f)
			{
				Relief.Say(body, "flower_burrow");
			}
			Plugin.Log.LogInfo(string.Format("The flower at {0} burrows under the ground ({1:0} s)", p.transform.position, Plugin.FlowerBurrowSeconds.Value));
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00032019 File Offset: 0x00030219
		public static void Buried(FlowerPlant p)
		{
			p.burrow = 2;
			p.underTimer = Plugin.FlowerBurrowSeconds.Value;
			p.gameObject.SetActive(false);
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00032040 File Offset: 0x00030240
		private static void Emerge(Body b, FlowerPlant p)
		{
			p.gameObject.SetActive(true);
			p.burrow = 3;
			p.sink = 0f;
			Sound.Play("goo", p.transform.position, false, true, null, 0.9f, 0.6f, false, false);
			if (b && b.alive && Vector2.Distance(b.transform.position, p.transform.position) < 20f)
			{
				Relief.Say(b, "flower_emerge");
			}
			Plugin.Log.LogInfo(string.Format("The flower at {0} grows up out of the ground again", p.transform.position));
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00032100 File Offset: 0x00030300
		public static void Kill(FlowerPlant p)
		{
			if (!p || p.dead)
			{
				return;
			}
			p.dead = true;
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (Flowers.Holder == p)
			{
				Flowers.Release(body, false);
			}
			if (Flowers.reaching == p)
			{
				Flowers.reaching = null;
				Flowers.HideLimbs();
			}
			Sound.Play("gore", p.transform.position, false, true, null, 1f, 1f, false, false);
			Sound.Play("goo", p.transform.position, false, true, null, 1f, 0.5f, false, false);
			if (body && body.alive && Vector2.Distance(body.transform.position, p.transform.position) < 16f)
			{
				Relief.Say(body, "flower_killed");
			}
			Plugin.Log.LogInfo(string.Format("The carnivorous flower at {0} is dead", p.transform.position));
			if (p.pit != null)
			{
				Pits.FlowerDied(body, p.pit);
			}
			Flowers.Plants.Remove(p);
			Object.Destroy(p.gameObject);
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00032255 File Offset: 0x00030455
		public static void LetGo(Body b)
		{
			if (!Flowers.Holding)
			{
				return;
			}
			Flowers.Holder.sated = 20f;
			Flowers.Release(b, false);
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00032278 File Offset: 0x00030478
		private static void Release(Body b, bool quiet)
		{
			Flowers.Holder = null;
			Flowers.reaching = null;
			Flowers.pulling = false;
			if (Flowers.blackout <= 0f)
			{
				Flowers.Blind = 0f;
			}
			Flowers.grabCooldown = 4f;
			Flowers.HideLimbs();
			if (Flowers.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(Flowers.wetLoop, 0.8f));
			}
			Flowers.wetLoop = null;
			if (!b)
			{
				return;
			}
			Traverse.Create(b).Field("movingAllowed").SetValue(true);
			if (!quiet)
			{
				Sound.Play("goo", b.transform.position, false, true, b.transform, 0.8f, 1.2f, false, false);
			}
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00032344 File Offset: 0x00030544
		private static void Veil(float dt)
		{
			if (Flowers.blackout <= 0f)
			{
				if (Flowers.veilSr)
				{
					Flowers.veilSr.enabled = false;
				}
				if (!Flowers.pulling)
				{
					Flowers.Blind = 0f;
				}
				return;
			}
			if (Flowers.veilHold > 0f)
			{
				Flowers.veilHold -= dt;
			}
			else
			{
				Flowers.blackout = Mathf.Max(0f, Flowers.blackout - dt / 0.7f);
			}
			Flowers.Blind = 2f * Flowers.blackout;
			Camera camera = (PlayerCamera.main ? PlayerCamera.main.GetComponent<Camera>() : Camera.main);
			if (!camera)
			{
				return;
			}
			if (!Flowers.veil)
			{
				Texture2D texture2D = new Texture2D(4, 4, 4, false);
				texture2D.SetPixels32(Enumerable.Repeat<Color32>(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), 16).ToArray<Color32>());
				texture2D.Apply();
				Flowers.veil = new GameObject("LewdFlowerVeil");
				Flowers.veilSr = Flowers.veil.AddComponent<SpriteRenderer>();
				Flowers.veilSr.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f), 4f);
				SortingLayer[] layers = SortingLayer.layers;
				if (layers.Length != 0)
				{
					Flowers.veilSr.sortingLayerID = layers[layers.Length - 1].id;
				}
				Flowers.veilSr.sortingOrder = 32763;
			}
			Flowers.veil.transform.SetParent(camera.transform, false);
			Flowers.veil.transform.localPosition = new Vector3(0f, 0f, 1f);
			float num = camera.orthographicSize * 2f + 2f;
			Flowers.veil.transform.localScale = new Vector3(num * camera.aspect + 2f, num, 1f);
			Flowers.veilSr.enabled = true;
			Flowers.veilSr.color = new Color(0f, 0f, 0f, Mathf.Clamp01(Flowers.blackout * 1.2f));
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00032578 File Offset: 0x00030778
		private static void EnsureLimbs(Body b)
		{
			Flowers.<>c__DisplayClass55_0 CS$<>8__locals1 = new Flowers.<>c__DisplayClass55_0();
			if (Flowers.tongue != null && Flowers.tongue[0])
			{
				return;
			}
			Flowers.DestroyLimbs();
			CS$<>8__locals1.torso = b.limbs[1].GetComponent<SpriteRenderer>();
			Flowers.tongue = (from i in Enumerable.Range(0, 12)
				select base.<EnsureLimbs>g__Make|0("LewdFlowerTongue", FlowerArt.Seg, 26)).ToArray<SpriteRenderer>();
			for (int j = 0; j < 2; j++)
			{
				SpriteRenderer[][] array = Flowers.milkers;
				int num = j;
				IEnumerable<int> enumerable = Enumerable.Range(0, 9);
				Func<int, SpriteRenderer> func;
				if ((func = CS$<>8__locals1.<>9__2) == null)
				{
					func = (CS$<>8__locals1.<>9__2 = (int i) => base.<EnsureLimbs>g__Make|0("LewdFlowerSucker", FlowerArt.Seg, 27));
				}
				array[num] = enumerable.Select<int, SpriteRenderer>(func).ToArray<SpriteRenderer>();
				Flowers.suckers[j] = CS$<>8__locals1.<EnsureLimbs>g__Make|0("LewdFlowerSuckerTip", FlowerArt.Sucker, 28);
			}
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0003263C File Offset: 0x0003083C
		private static void HideLimbs()
		{
			foreach (SpriteRenderer[] array2 in new SpriteRenderer[][]
			{
				Flowers.tongue,
				Flowers.milkers[0],
				Flowers.milkers[1]
			})
			{
				if (array2 != null)
				{
					foreach (SpriteRenderer spriteRenderer in array2)
					{
						if (spriteRenderer)
						{
							spriteRenderer.enabled = false;
						}
					}
				}
			}
			foreach (SpriteRenderer spriteRenderer2 in Flowers.suckers)
			{
				if (spriteRenderer2)
				{
					spriteRenderer2.enabled = false;
				}
			}
		}

		// Token: 0x060002DF RID: 735 RVA: 0x000326D8 File Offset: 0x000308D8
		private static void DestroyLimbs()
		{
			foreach (SpriteRenderer[] array2 in new SpriteRenderer[][]
			{
				Flowers.tongue,
				Flowers.milkers[0],
				Flowers.milkers[1]
			})
			{
				if (array2 != null)
				{
					foreach (SpriteRenderer spriteRenderer in array2)
					{
						if (spriteRenderer)
						{
							Object.Destroy(spriteRenderer.gameObject);
						}
					}
				}
			}
			foreach (SpriteRenderer spriteRenderer2 in Flowers.suckers)
			{
				if (spriteRenderer2)
				{
					Object.Destroy(spriteRenderer2.gameObject);
				}
			}
			Flowers.tongue = null;
			Flowers.milkers[0] = (Flowers.milkers[1] = null);
			Flowers.suckers[0] = (Flowers.suckers[1] = null);
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x000327A8 File Offset: 0x000309A8
		private static void Chain(SpriteRenderer[] segs, Vector2 from, Vector2 to, float thick, float wiggle, bool coil)
		{
			if (segs == null || !FlowerArt.Seg)
			{
				return;
			}
			float x = FlowerArt.Seg.bounds.size.x;
			int num = (coil ? (segs.Length - 3) : segs.Length);
			Vector2 vector = to - from;
			Vector2 normalized = new Vector2(-vector.y, vector.x).normalized;
			Vector2 vector2 = from;
			float num2 = Time.time * 2.6f;
			for (int i = 0; i < segs.Length; i++)
			{
				SpriteRenderer spriteRenderer = segs[i];
				if (spriteRenderer)
				{
					float num3 = Mathf.Min(1f, ((float)i + 1f) / (float)num);
					Vector2 vector3 = ((i < num) ? (from + vector * num3 + normalized * Mathf.Sin(Time.time * 4.5f + num3 * 5f) * 0.2f * wiggle * Mathf.Sin(num3 * 3.1415927f)) : (to + new Vector2(Mathf.Cos(num2 + (float)(i - num) * 2.1f) * 0.32f, Mathf.Sin(num2 + (float)(i - num) * 2.1f) * 0.12f)));
					Vector2 vector4 = vector3 - vector2;
					spriteRenderer.enabled = vector4.sqrMagnitude > 1E-06f;
					spriteRenderer.transform.position = vector2;
					spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector4.y, vector4.x) * 57.29578f);
					spriteRenderer.transform.localScale = new Vector3(vector4.magnitude / x + 0.12f, thick * Mathf.Lerp(1.3f, 0.75f, num3), 1f);
					vector2 = vector3;
				}
			}
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0003299C File Offset: 0x00030B9C
		private static void Tip(SpriteRenderer tip, SpriteRenderer[] chain, Vector2 at, bool show)
		{
			if (!tip || chain == null)
			{
				return;
			}
			tip.enabled = show;
			if (!show)
			{
				return;
			}
			Vector2 vector = (chain[chain.Length - 1] ? chain[chain.Length - 1].transform.position : at);
			Vector2 vector2 = at - vector;
			tip.transform.position = at;
			tip.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector2.y, vector2.x) * 57.29578f);
		}

		// Token: 0x0400029D RID: 669
		public static readonly List<FlowerPlant> Plants = new List<FlowerPlant>();

		// Token: 0x040002A0 RID: 672
		private const float ReachSeconds = 0.55f;

		// Token: 0x040002A1 RID: 673
		private const float PullSeconds = 1.3f;

		// Token: 0x040002A2 RID: 674
		private static FlowerPlant reaching;

		// Token: 0x040002A3 RID: 675
		private static float reachTime;

		// Token: 0x040002A4 RID: 676
		private static float holdTime;

		// Token: 0x040002A5 RID: 677
		private static float pullTime;

		// Token: 0x040002A6 RID: 678
		private static float attemptCooldown;

		// Token: 0x040002A7 RID: 679
		private static float grabCooldown;

		// Token: 0x040002A8 RID: 680
		private static float lineTimer;

		// Token: 0x040002A9 RID: 681
		private static float suckTimer;

		// Token: 0x040002AA RID: 682
		private static float blackout;

		// Token: 0x040002AB RID: 683
		private static float veilHold;

		// Token: 0x040002AC RID: 684
		private static bool pulling;

		// Token: 0x040002AD RID: 685
		private static AudioSource wetLoop;

		// Token: 0x040002AE RID: 686
		private static SpriteRenderer[] tongue;

		// Token: 0x040002AF RID: 687
		private static readonly SpriteRenderer[][] milkers = new SpriteRenderer[2][];

		// Token: 0x040002B0 RID: 688
		private static readonly SpriteRenderer[] suckers = new SpriteRenderer[2];

		// Token: 0x040002B1 RID: 689
		private static GameObject veil;

		// Token: 0x040002B2 RID: 690
		private static SpriteRenderer veilSr;

		// Token: 0x040002B3 RID: 691
		private static readonly Vector2 Nipple = new Vector2(0.4f, 0.02f);

		// Token: 0x040002B4 RID: 692
		private static readonly Vector2 NippleFar = new Vector2(0.3f, 0.07f);
	}
}
