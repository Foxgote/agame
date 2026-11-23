using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x02000089 RID: 137
	internal static class Flesh
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000280 RID: 640 RVA: 0x0002D8F0 File Offset: 0x0002BAF0
		// (set) Token: 0x06000281 RID: 641 RVA: 0x0002D8F7 File Offset: 0x0002BAF7
		public static FleshPatch Holder { get; private set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000282 RID: 642 RVA: 0x0002D8FF File Offset: 0x0002BAFF
		public static bool Holding
		{
			get
			{
				return Flesh.phase > Flesh.Phase.None;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000283 RID: 643 RVA: 0x0002D909 File Offset: 0x0002BB09
		public static bool Swallowed
		{
			get
			{
				return Flesh.phase == Flesh.Phase.Swallowed;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000284 RID: 644 RVA: 0x0002D913 File Offset: 0x0002BB13
		public static bool Digesting
		{
			get
			{
				return Flesh.phase == Flesh.Phase.Swallowed && !Flesh.scene;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000285 RID: 645 RVA: 0x0002D927 File Offset: 0x0002BB27
		public static bool InScene
		{
			get
			{
				return Flesh.phase == Flesh.Phase.Swallowed && Flesh.scene;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000286 RID: 646 RVA: 0x0002D938 File Offset: 0x0002BB38
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0002D940 File Offset: 0x0002BB40
		public static void SeedLayer(Body b)
		{
			WorldGeneration world = WorldGeneration.world;
			int num = Diff.Count(Plugin.FleshPerLayer.Value);
			int num2 = 0;
			int mask = LayerMask.GetMask(new string[] { "Ground" });
			int num3 = 0;
			while (num3 < 600 && num2 < num)
			{
				Vector2 vector;
				vector..ctor(Random.Range(-world.halfWidth, world.halfWidth), Random.Range(-world.halfHeight, world.halfHeight));
				if (Vector2.Distance(vector, b.transform.position) >= 30f && !Physics2D.OverlapPoint(vector, mask))
				{
					RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, Vector2.down, 20f, mask);
					if (raycastHit2D && Flesh.Flat(raycastHit2D.point, 2.2f, mask))
					{
						Flesh.Spawn(raycastHit2D.point);
						num2++;
					}
				}
				num3++;
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: placed {1} living flesh patches", world.biomeDepth, num2));
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0002DA60 File Offset: 0x0002BC60
		private static bool Flat(Vector2 p, float halfWidth, int ground)
		{
			foreach (float num in new float[]
			{
				-halfWidth,
				halfWidth
			})
			{
				RaycastHit2D raycastHit2D = Physics2D.Raycast(p + new Vector2(num, 1f), Vector2.down, 2f, ground);
				if (!raycastHit2D || Mathf.Abs(raycastHit2D.point.y - p.y) > 0.45f)
				{
					return false;
				}
			}
			return !Physics2D.OverlapBox(p + Vector2.up * 1.5f, new Vector2(halfWidth * 2f, 2f), 0f, ground);
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0002DB14 File Offset: 0x0002BD14
		public static void ClearAll(Body b)
		{
			if (Flesh.Holding && b)
			{
				Flesh.Release(b, false, true);
			}
			foreach (FleshPatch fleshPatch in Flesh.Patches.ToArray())
			{
				if (fleshPatch)
				{
					Object.Destroy(fleshPatch.gameObject);
				}
			}
			Flesh.Patches.Clear();
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0002DB74 File Offset: 0x0002BD74
		public static FleshPatch Spawn(Vector2 floorPoint)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				FleshArt.Ensure(spriteRenderer.sprite.pixelsPerUnit);
			}
			if (FleshArt.Floor.Length == 0)
			{
				return null;
			}
			GameObject go = new GameObject("LewdLivingFlesh");
			go.transform.position = floorPoint + Vector2.down * 0.15f;
			SpriteRenderer spriteRenderer2 = go.AddComponent<SpriteRenderer>();
			spriteRenderer2.sprite = FleshArt.Floor[0];
			if (WorldGeneration.world)
			{
				spriteRenderer2.sharedMaterial = WorldGeneration.world.defaultMat;
			}
			if (spriteRenderer)
			{
				spriteRenderer2.sortingLayerID = spriteRenderer.sortingLayerID;
				spriteRenderer2.sortingOrder = spriteRenderer.sortingOrder - 15;
			}
			BoxCollider2D boxCollider2D = go.AddComponent<BoxCollider2D>();
			boxCollider2D.isTrigger = false;
			boxCollider2D.size = spriteRenderer2.sprite.bounds.size;
			boxCollider2D.offset = spriteRenderer2.sprite.bounds.center;
			go.layer = LayerMask.NameToLayer("Descriptor");
			BuildingEntity buildingEntity = go.AddComponent<BuildingEntity>();
			buildingEntity.animal = true;
			buildingEntity.health = 100000f;
			buildingEntity.id = "lewd_flesh";
			buildingEntity.ignoreBodyOptimize = true;
			buildingEntity.itemsDropOnDestroy = new ItemDrop[0];
			buildingEntity.alwaysDrop = new ItemDrop[0];
			buildingEntity.itemCategoriesToAdd = new string[0];
			buildingEntity.hitSound = Resources.Load<AudioClip>("Sounds/goo");
			Light2D light2D = go.AddComponent<Light2D>();
			light2D.lightType = 3;
			light2D.color = new Color32(220, 60, 80, byte.MaxValue);
			light2D.intensity = 0.55f;
			light2D.pointLightOuterRadius = 3.5f;
			FleshPatch patch = go.AddComponent<FleshPatch>();
			Burning.Make(go, "living flesh", Plugin.FleshHealth.Value, spriteRenderer2, delegate
			{
				Flesh.OnPatchHit(patch);
				patch.sated = Mathf.Max(patch.sated, Plugin.FleshStunSeconds.Value);
			}, delegate
			{
				Flesh.OnPatchDestroyed(patch);
				Flesh.Patches.Remove(patch);
				Object.Destroy(go);
			});
			Flesh.Patches.Add(patch);
			return patch;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0002DDE4 File Offset: 0x0002BFE4
		public static void Update(Body b, float dt)
		{
			if (Flesh.grabCooldown > 0f)
			{
				Flesh.grabCooldown -= dt;
			}
			if (Flesh.attemptCooldown > 0f)
			{
				Flesh.attemptCooldown -= dt;
			}
			Flesh.Patches.RemoveAll((FleshPatch p) => !p);
			if (Flesh.Holding && (!Flesh.Holder || !b.alive))
			{
				Flesh.Release(b, false, !b.alive);
				return;
			}
			if (Flesh.Holding && Keys.Down(Plugin.RemoveKey.Value))
			{
				Flesh.Struggle(b);
			}
			switch (Flesh.phase)
			{
			case Flesh.Phase.None:
				if (!Scenes.Busy && !Tentacles.Reaching)
				{
					Flesh.Lure(b, dt);
					return;
				}
				break;
			case Flesh.Phase.Grabbed:
				Flesh.UpdateGrab(b, dt);
				return;
			case Flesh.Phase.Swallowed:
				Flesh.UpdateInside(b, dt);
				break;
			default:
				return;
			}
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0002DED4 File Offset: 0x0002C0D4
		private static void Lure(Body b, float dt)
		{
			if (!b.alive)
			{
				return;
			}
			foreach (FleshPatch fleshPatch in Flesh.Patches)
			{
				if (fleshPatch && fleshPatch.sated <= 0f)
				{
					Vector2 vector = b.transform.position - fleshPatch.Mouth;
					float num = fleshPatch.HalfWidth + 5f;
					if (Mathf.Abs(vector.x) <= num && vector.y <= 3f && vector.y >= -1.5f)
					{
						float num2 = 1f - Mathf.Abs(vector.x) / num;
						b.rb.velocity += new Vector2(-Mathf.Sign(vector.x) * 7f * num2 * dt, 0f);
						if (Mathf.Abs(vector.x) < fleshPatch.HalfWidth * 0.85f && vector.y < 1.8f && Flesh.grabCooldown <= 0f)
						{
							Flesh.Grab(b, fleshPatch);
							break;
						}
					}
				}
			}
		}

		// Token: 0x0600028D RID: 653 RVA: 0x0002E038 File Offset: 0x0002C238
		private static void Grab(Body b, FleshPatch p)
		{
			if (Scenes.Busy)
			{
				return;
			}
			Flesh.Holder = p;
			Flesh.phase = Flesh.Phase.Grabbed;
			Flesh.timer = 0f;
			Flesh.attemptCooldown = 0.6f;
			Relief.ForceStop();
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			b.eyePanicTime = 1f;
			b.adrenaline += 30f;
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.5f, false, false);
			Sound.Play("fleshrip", b.transform.position, false, true, b.transform, 0.5f, 1f, false, false);
			Flesh.grabbed.Clear();
			Flesh.grabbed.AddRange((from l in b.limbs
				where !l.dismembered
				orderby l.transform.position.y
				select l).Take<Limb>(2));
			Flesh.grabbed.Add(b.limbs[1]);
			foreach (Limb limb in Flesh.grabbed)
			{
				Flesh.tentacles.Add(Flesh.MakeTentacle(b));
			}
			Relief.Say(b, "flesh_grab");
			Plugin.Log.LogInfo("Living flesh grabbed her");
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0002E1EC File Offset: 0x0002C3EC
		private static void UpdateGrab(Body b, float dt)
		{
			Flesh.timer += dt;
			Vector2 vector = Flesh.Holder.Mouth - b.transform.position;
			b.rb.velocity = Vector2.Lerp(b.rb.velocity, new Vector2(Mathf.Clamp(vector.x * 3f, -3f, 3f), Mathf.Min(b.rb.velocity.y, 0f)), dt * 5f);
			Flesh.S.arousal = Mathf.Min(100f, Flesh.S.arousal + 0.6f * dt);
			Flesh.DrawTentacles(b);
			if (Flesh.timer > Plugin.FleshSwallowSeconds.Value)
			{
				Flesh.Swallow(b);
			}
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0002E2C4 File Offset: 0x0002C4C4
		private static void Swallow(Body b)
		{
			Flesh.phase = Flesh.Phase.Swallowed;
			Flesh.timer = 0f;
			Flesh.ClearTentacles();
			float num = Mathf.Clamp(0.5f + Flesh.S.corruption * 0.003f + Flesh.S.submission * 0.002f + ((Flesh.S.lust > 60f) ? 0.1f : 0f), 0.1f, 0.9f);
			Flesh.scene = Random.value < num;
			b.bodyAnimator.Play("ExperimentLayDown");
			b.armsAnimator.Play("ArmsLayDown");
			Sound.Play("megabite", b.transform.position, false, true, b.transform, 0.9f, 1f, false, false);
			Sound.Play("eatFlesh", b.transform.position, false, true, b.transform, 1f, 1f, false, false);
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			Flesh.mound = new GameObject("LewdFleshMound").AddComponent<SpriteRenderer>();
			Flesh.mound.sprite = ((FleshArt.Mound.Length != 0) ? FleshArt.Mound[0] : null);
			Flesh.mound.sharedMaterial = Flesh.Holder.GetComponent<SpriteRenderer>().sharedMaterial;
			Flesh.mound.sortingLayerID = component.sortingLayerID;
			Flesh.mound.sortingOrder = b.GetComponentsInChildren<SpriteRenderer>(true).Max<SpriteRenderer>((SpriteRenderer r) => r.sortingOrder) + 10;
			Flesh.PlaceMound(b);
			Flesh.wetLoop = Sfx.Loop(b, "loop_wet");
			Flesh.pulseTimer = 2f;
			Flesh.lineTimer = 3f;
			Relief.Say(b, "flesh_swallowed");
			Plugin.Log.LogInfo(string.Format("Swallowed by living flesh: {0} (scene chance {1:P0})", Flesh.scene ? "tentacle scene" : "digestion", num));
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0002E4C8 File Offset: 0x0002C6C8
		private static void UpdateInside(Body b, float dt)
		{
			Flesh.timer += dt;
			Vector3 position = b.transform.position;
			position.x = Mathf.MoveTowards(position.x, Flesh.Holder.Mouth.x, 3f * dt);
			b.transform.position = position;
			b.rb.velocity = new Vector2(0f, Mathf.Min(b.rb.velocity.y, 0f));
			Flesh.PlaceMound(b);
			if ((Flesh.frameTime -= dt) <= 0f && FleshArt.Mound.Length != 0)
			{
				Flesh.frameTime = (Flesh.scene ? 0.35f : 0.6f);
				Flesh.moundFrame = (Flesh.moundFrame + 1) % FleshArt.Mound.Length;
				Flesh.mound.sprite = FleshArt.Mound[Flesh.moundFrame];
			}
			if (Flesh.scene)
			{
				Flesh.S.arousal = Mathf.Min(100f, Flesh.S.arousal + 2.5f * dt);
				Flesh.S.lust = Mathf.Min(100f, Flesh.S.lust + 0.5f * dt);
				if (Flesh.wetLoop)
				{
					Flesh.wetLoop.volume = Mathf.MoveTowards(Flesh.wetLoop.volume, 0.5f, dt);
				}
				if ((Flesh.pulseTimer -= dt) <= 0f)
				{
					Flesh.pulseTimer = Random.Range(3.5f, 6f);
					Flesh.S.arousal = Mathf.Min(100f, Flesh.S.arousal + 8f);
					b.eatTime = Mathf.Max(b.eatTime, 0.6f);
					Sfx.Play(b, 0.75f, new string[] { "muffled", "moan" });
					Sfx.Play(b, 0.4f, new string[] { "drip" });
					Semen.Add(b, 3f);
					Womb.Fill(b, 4f);
				}
				if ((Flesh.lineTimer -= dt) <= 0f)
				{
					Flesh.lineTimer = Random.Range(6f, 10f);
					Relief.Say(b, "flesh_scene");
				}
				if (Flesh.timer > Plugin.FleshSceneSeconds.Value)
				{
					Flesh.SpitOut(b);
					return;
				}
			}
			else
			{
				if (Flesh.wetLoop)
				{
					Flesh.wetLoop.volume = Mathf.MoveTowards(Flesh.wetLoop.volume, 0.35f, dt);
					Flesh.wetLoop.pitch = 0.7f;
				}
				foreach (Limb limb in b.limbs)
				{
					if (!limb.dismembered)
					{
						float armorReduction = limb.GetArmorReduction();
						limb.skinHealth -= 1f * Diff.Lethal / armorReduction * dt;
						limb.muscleHealth -= 0.4f * Diff.Lethal / armorReduction * dt;
						limb.pain = Mathf.Min(100f, limb.pain + 1.5f / armorReduction * dt);
					}
				}
				b.bloodOxygen = Mathf.Max(0f, b.bloodOxygen - 1.2f * Diff.Lethal * dt);
				if ((Flesh.pulseTimer -= dt) <= 0f)
				{
					Flesh.pulseTimer = Random.Range(4f, 7f);
					Sound.Play("gore", b.transform.position, false, true, b.transform, 0.5f, 1f, false, false);
					Sfx.Play(b, 0.7f, new string[] { "muffled", "moan_soft" });
				}
				if ((Flesh.lineTimer -= dt) <= 0f)
				{
					Flesh.lineTimer = Random.Range(5f, 9f);
					Relief.Say(b, "flesh_digest");
				}
			}
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0002E8C4 File Offset: 0x0002CAC4
		private static void PlaceMound(Body b)
		{
			if (!Flesh.mound || !Flesh.mound.sprite)
			{
				return;
			}
			Bounds? bounds = null;
			foreach (SpriteRenderer spriteRenderer in b.GetComponentsInChildren<SpriteRenderer>())
			{
				if (spriteRenderer.enabled && spriteRenderer.sprite && !spriteRenderer.gameObject.name.StartsWith("LewdTentacle"))
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
			Vector2 vector = Flesh.mound.sprite.bounds.size;
			float num = Mathf.Max(new float[]
			{
				value2.size.x * 1.35f / vector.x,
				value2.size.y * 1.25f / vector.y,
				1f
			});
			Flesh.mound.transform.localScale = new Vector3(num, num, 1f);
			Flesh.mound.transform.position = new Vector3(value2.center.x, value2.min.y - 0.15f, 0f);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0002EA50 File Offset: 0x0002CC50
		private static void Struggle(Body b)
		{
			if (Flesh.attemptCooldown > 0f || !b.conscious)
			{
				return;
			}
			Flesh.attemptCooldown = 1f;
			b.stamina = Mathf.Max(0f, b.stamina - 8f);
			float num = ((Flesh.phase == Flesh.Phase.Grabbed) ? Plugin.FleshEscapeGrab.Value : Plugin.FleshEscapeInside.Value) * Diff.Escape * ((b.stamina < 15f) ? 0.5f : 1f) * (1f - Flesh.S.submission * 0.003f) * (Flesh.Digesting ? 1.2f : 1f);
			bool flag = Random.value < num;
			Plugin.Log.LogInfo(string.Format("Struggling against the flesh ({0}): chance {1:P0}, {2}", Flesh.phase, num, flag ? "free" : "held"));
			if (flag)
			{
				Flesh.Escaped(b);
				return;
			}
			if (Flesh.phase == Flesh.Phase.Grabbed)
			{
				Flesh.timer += 0.5f;
			}
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.8f, 0.6f, false, false);
			Relief.Say(b, (Flesh.phase == Flesh.Phase.Grabbed) ? "flesh_fail" : "flesh_fail_inside");
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0002EBAC File Offset: 0x0002CDAC
		private static void SpitOut(Body b)
		{
			Flesh.S.corruption = Mathf.Min(100f, Flesh.S.corruption + 8f);
			Flesh.S.submission = Mathf.Min(100f, Flesh.S.submission + 5f);
			Semen.Add(b, 70f);
			Womb.Fill(b, 35f);
			if (Flesh.Holder)
			{
				Flesh.Holder.sated = 90f;
			}
			Sound.Play("vomit", b.transform.position, false, true, b.transform, 0.8f, 1f, false, false);
			Flesh.Release(b, true, true);
			Relief.Say(b, "flesh_spat");
			Plugin.Log.LogInfo("The flesh is sated and spits her out");
		}

		// Token: 0x06000294 RID: 660 RVA: 0x0002EC84 File Offset: 0x0002CE84
		public static void OnPatchHit(FleshPatch p)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && Flesh.Holder == p && Flesh.phase == Flesh.Phase.Grabbed && Random.value < 0.25f)
			{
				Flesh.Escaped(body);
			}
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0002ECDA File Offset: 0x0002CEDA
		public static void LetGo(Body b)
		{
			if (Flesh.Holding)
			{
				Flesh.Escaped(b);
			}
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0002ECEC File Offset: 0x0002CEEC
		private static void Escaped(Body b)
		{
			FleshPatch holder = Flesh.Holder;
			Flesh.Release(b, true, false);
			if (holder)
			{
				holder.sated = Mathf.Max(holder.sated, 8f);
			}
			Flesh.grabCooldown = 6f;
			Relief.Say(b, "flesh_escape");
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0002ED3C File Offset: 0x0002CF3C
		public static void OnPatchDestroyed(FleshPatch p)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && Flesh.Holder == p)
			{
				Flesh.Release(body, true, false);
			}
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0002ED80 File Offset: 0x0002CF80
		private static void Release(Body b, bool toss, bool quiet)
		{
			bool flag = Flesh.phase == Flesh.Phase.Swallowed;
			Flesh.phase = Flesh.Phase.None;
			Flesh.ClearTentacles();
			if (Flesh.mound)
			{
				Object.Destroy(Flesh.mound.gameObject);
			}
			if (Flesh.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(Flesh.wetLoop, 0.5f));
			}
			Flesh.wetLoop = null;
			Flesh.grabCooldown = 3f;
			if (b)
			{
				Traverse.Create(b).Field("movingAllowed").SetValue(true);
				if (flag && b.alive)
				{
					b.bodyAnimator.Play("Grounded");
					b.armsAnimator.Play("Grounded");
				}
				if (toss && Flesh.Holder && b.alive)
				{
					float num = Mathf.Sign(b.transform.position.x - Flesh.Holder.transform.position.x);
					if (num == 0f)
					{
						num = 1f;
					}
					b.transform.position = Flesh.Holder.Mouth + new Vector2(num * (Flesh.Holder.HalfWidth + 0.6f), 0.8f);
					Physics2D.SyncTransforms();
					b.rb.velocity = new Vector2(num * 6f, 7f);
					if (!quiet)
					{
						Sound.Play("fleshrip", b.transform.position, false, true, b.transform, 0.7f, 1f, false, false);
					}
				}
			}
			Flesh.Holder = null;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0002EF38 File Offset: 0x0002D138
		private static SpriteRenderer[] MakeTentacle(Body b)
		{
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			SpriteRenderer[] array = new SpriteRenderer[7];
			for (int i = 0; i < array.Length; i++)
			{
				SpriteRenderer spriteRenderer = new GameObject("LewdTentacle").AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = FleshArt.Tendril;
				spriteRenderer.sharedMaterial = Flesh.Holder.GetComponent<SpriteRenderer>().sharedMaterial;
				spriteRenderer.sortingLayerID = component.sortingLayerID;
				spriteRenderer.sortingOrder = component.sortingOrder + 25;
				spriteRenderer.color = Color.Lerp(new Color32(168, 52, 72, byte.MaxValue), new Color32(214, 96, 110, byte.MaxValue), (float)i / (float)array.Length);
				array[i] = spriteRenderer;
			}
			return array;
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0002F000 File Offset: 0x0002D200
		private static void DrawTentacles(Body b)
		{
			if (!FleshArt.Tendril)
			{
				return;
			}
			float x = FleshArt.Tendril.bounds.size.x;
			int num = 0;
			while (num < Flesh.tentacles.Count && num < Flesh.grabbed.Count)
			{
				Limb limb = Flesh.grabbed[num];
				if (limb)
				{
					Vector2 vector = Flesh.Holder.Mouth + new Vector2((float)(num - 1) * Flesh.Holder.HalfWidth * 0.35f, -0.1f);
					Vector2 vector2 = limb.transform.position - vector;
					Vector2 normalized = new Vector2(-vector2.y, vector2.x).normalized;
					SpriteRenderer[] array = Flesh.tentacles[num];
					Vector2 vector3 = vector;
					for (int i = 0; i < array.Length; i++)
					{
						float num2 = ((float)i + 1f) / (float)array.Length;
						float num3 = Mathf.Sin(Time.time * 5f + (float)num * 1.7f + num2 * 4f) * 0.25f * Mathf.Sin(num2 * 3.1415927f);
						Vector2 vector4 = vector + vector2 * num2 + normalized * num3;
						Vector2 vector5 = vector4 - vector3;
						SpriteRenderer spriteRenderer = array[i];
						spriteRenderer.transform.position = vector3;
						spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector5.y, vector5.x) * 57.29578f);
						float num4 = Mathf.Lerp(1.6f, 0.7f, num2);
						spriteRenderer.transform.localScale = new Vector3(vector5.magnitude / x + 0.15f, num4, 1f);
						vector3 = vector4;
					}
				}
				num++;
			}
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0002F1F8 File Offset: 0x0002D3F8
		private static void ClearTentacles()
		{
			foreach (SpriteRenderer[] array in Flesh.tentacles)
			{
				foreach (SpriteRenderer spriteRenderer in array)
				{
					if (spriteRenderer)
					{
						Object.Destroy(spriteRenderer.gameObject);
					}
				}
			}
			Flesh.tentacles.Clear();
			Flesh.grabbed.Clear();
		}

		// Token: 0x0400026D RID: 621
		public static readonly List<FleshPatch> Patches = new List<FleshPatch>();

		// Token: 0x0400026F RID: 623
		private static Flesh.Phase phase;

		// Token: 0x04000270 RID: 624
		private static bool scene;

		// Token: 0x04000271 RID: 625
		private static float timer;

		// Token: 0x04000272 RID: 626
		private static float attemptCooldown;

		// Token: 0x04000273 RID: 627
		private static float grabCooldown;

		// Token: 0x04000274 RID: 628
		private static float pulseTimer;

		// Token: 0x04000275 RID: 629
		private static float lineTimer;

		// Token: 0x04000276 RID: 630
		private static float frameTime;

		// Token: 0x04000277 RID: 631
		private static readonly List<SpriteRenderer[]> tentacles = new List<SpriteRenderer[]>();

		// Token: 0x04000278 RID: 632
		private static readonly List<Limb> grabbed = new List<Limb>();

		// Token: 0x04000279 RID: 633
		private static SpriteRenderer mound;

		// Token: 0x0400027A RID: 634
		private static AudioSource wetLoop;

		// Token: 0x0400027B RID: 635
		private static int moundFrame;

		// Token: 0x020001A8 RID: 424
		private enum Phase
		{
			// Token: 0x04000964 RID: 2404
			None,
			// Token: 0x04000965 RID: 2405
			Grabbed,
			// Token: 0x04000966 RID: 2406
			Swallowed
		}
	}
}
