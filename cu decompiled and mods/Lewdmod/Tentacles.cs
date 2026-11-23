using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PoseKit;
using ScavArmory;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000119 RID: 281
	internal static class Tentacles
	{
		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000688 RID: 1672 RVA: 0x00067585 File Offset: 0x00065785
		// (set) Token: 0x06000689 RID: 1673 RVA: 0x0006758C File Offset: 0x0006578C
		public static TentacleTrap Holder { get; private set; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x0600068A RID: 1674 RVA: 0x00067594 File Offset: 0x00065794
		public static bool Holding
		{
			get
			{
				return Tentacles.Holder;
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x0600068B RID: 1675 RVA: 0x000675A0 File Offset: 0x000657A0
		// (set) Token: 0x0600068C RID: 1676 RVA: 0x000675A7 File Offset: 0x000657A7
		public static bool Insatiable { get; private set; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x000675AF File Offset: 0x000657AF
		public static bool Reaching
		{
			get
			{
				return Tentacles.reaching != null;
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x000675BC File Offset: 0x000657BC
		// (set) Token: 0x0600068F RID: 1679 RVA: 0x000675C3 File Offset: 0x000657C3
		public static bool MouthUsed { get; private set; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000690 RID: 1680 RVA: 0x000675CB File Offset: 0x000657CB
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000691 RID: 1681 RVA: 0x000675D2 File Offset: 0x000657D2
		// (set) Token: 0x06000692 RID: 1682 RVA: 0x000675D9 File Offset: 0x000657D9
		public static bool Anal { get; private set; }

		// Token: 0x06000693 RID: 1683 RVA: 0x000675E1 File Offset: 0x000657E1
		public static bool HolderIs(TentacleTrap t)
		{
			return Tentacles.Holder == t || Tentacles.reaching == t;
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x00067600 File Offset: 0x00065800
		public static void SeedLayer(Body b)
		{
			int num = Diff.Count(Plugin.TentaclesPerLayer.Value);
			int num2 = 0;
			int num3 = 0;
			while (num3 < 800 && num2 < num)
			{
				Vector2? vector = Tentacles.RandomCeiling(b.transform.position, 25f, 1E+09f, null, 0);
				if (vector != null)
				{
					Tentacles.Spawn(vector.Value);
					num2++;
				}
				num3++;
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: placed {1} ceiling tentacles ({2} spots tried)", WorldGeneration.world.biomeDepth, num2, num3));
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x000676A8 File Offset: 0x000658A8
		public static void ClearAll(Body b)
		{
			if (Tentacles.Holding && b)
			{
				Tentacles.Release(b, true);
			}
			foreach (TentacleTrap tentacleTrap in Tentacles.Traps.ToArray())
			{
				if (tentacleTrap)
				{
					Object.Destroy(tentacleTrap.gameObject);
				}
			}
			Tentacles.Traps.Clear();
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x00067708 File Offset: 0x00065908
		private static Vector2? RandomCeiling(Vector2 from, float minDist, float maxDist, Vector2Int? near = null, int radius = 0)
		{
			WorldGeneration world = WorldGeneration.world;
			for (int i = 0; i < 60; i++)
			{
				int num;
				int num2;
				if (near != null)
				{
					num = near.Value.x + Random.Range(-radius, radius + 1);
					num2 = near.Value.y + Random.Range(0, radius + 1);
				}
				else
				{
					num = Random.Range(8, (int)(world.width - 8U));
					num2 = Random.Range(8, (int)(world.height - 8U));
				}
				if (num >= 2 && num2 >= 12 && (long)num < (long)((ulong)(world.width - 2U)) && (long)num2 < (long)((ulong)(world.height - 2U)) && world.GetBlock(new Vector2Int(num, num2)) == 0 && world.GetBlock(new Vector2Int(num, num2 + 1)) != 0)
				{
					int num3 = -1;
					for (int j = 1; j <= 9; j++)
					{
						if (world.GetBlock(new Vector2Int(num, num2 - j)) != 0 || world.GetBlock(new Vector2Int(num - 1, num2 - j)) != 0 || world.GetBlock(new Vector2Int(num + 1, num2 - j)) != 0)
						{
							num3 = j;
							break;
						}
					}
					if (num3 >= 5)
					{
						Vector2 p = world.BlockToWorldPos(new Vector2Int(num, num2)) + Vector2.up * 0.5f;
						float num4 = Vector2.Distance(p, from);
						if (num4 >= minDist && num4 <= maxDist && !Tentacles.Traps.Any<TentacleTrap>((TentacleTrap t) => t && Vector2.Distance(t.transform.position, p) < 10f) && !Camps.InQuiet(p))
						{
							return new Vector2?(p);
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x000678CC File Offset: 0x00065ACC
		public static TentacleTrap Spawn(Vector2 ceiling)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				TentacleArt.Ensure(spriteRenderer.sprite.pixelsPerUnit);
			}
			if (TentacleArt.Root.Length == 0 || !TentacleArt.Seg)
			{
				return null;
			}
			GameObject gameObject = new GameObject("LewdTentacles");
			gameObject.transform.position = ceiling;
			TentacleTrap t = gameObject.AddComponent<TentacleTrap>();
			t.root = gameObject.AddComponent<SpriteRenderer>();
			t.root.sprite = TentacleArt.Root[0];
			Material material = (WorldGeneration.world ? WorldGeneration.world.defaultMat : null);
			if (material)
			{
				t.root.sharedMaterial = material;
			}
			int num = (spriteRenderer ? spriteRenderer.sortingLayerID : 0);
			int num2 = (spriteRenderer ? spriteRenderer.sortingOrder : 0);
			t.root.sortingLayerID = num;
			t.root.sortingOrder = num2 + 30;
			for (int i = 0; i < 7; i++)
			{
				SpriteRenderer[] array = new SpriteRenderer[8];
				for (int j = 0; j < array.Length; j++)
				{
					SpriteRenderer spriteRenderer2 = new GameObject("LewdTentacleSeg").AddComponent<SpriteRenderer>();
					spriteRenderer2.sprite = TentacleArt.Seg;
					if (material)
					{
						spriteRenderer2.sharedMaterial = material;
					}
					spriteRenderer2.sortingLayerID = num;
					spriteRenderer2.sortingOrder = num2 + 28;
					array[j] = spriteRenderer2;
				}
				t.chains.Add(array);
				SpriteRenderer spriteRenderer3 = new GameObject("LewdTentacleTip").AddComponent<SpriteRenderer>();
				spriteRenderer3.sprite = TentacleArt.Tip;
				if (material)
				{
					spriteRenderer3.sharedMaterial = material;
				}
				spriteRenderer3.sortingLayerID = num;
				spriteRenderer3.sortingOrder = num2 + 29;
				t.tips.Add(spriteRenderer3);
				t.idleLen[i] = Random.Range(1.2f, 2.6f);
			}
			BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
			boxCollider2D.isTrigger = false;
			Bounds bounds = t.root.sprite.bounds;
			boxCollider2D.size = new Vector2(Mathf.Max(bounds.size.x, 1.6f), 2.6f);
			boxCollider2D.offset = new Vector2(bounds.center.x, bounds.min.y - 1.1f);
			gameObject.layer = LayerMask.NameToLayer("Descriptor");
			BuildingEntity buildingEntity = gameObject.AddComponent<BuildingEntity>();
			buildingEntity.animal = true;
			buildingEntity.health = 100000f;
			buildingEntity.id = "lewd_tentacles";
			buildingEntity.ignoreBodyOptimize = true;
			buildingEntity.itemsDropOnDestroy = new ItemDrop[0];
			buildingEntity.alwaysDrop = new ItemDrop[0];
			buildingEntity.itemCategoriesToAdd = new string[0];
			buildingEntity.hitSound = Resources.Load<AudioClip>("Sounds/goo");
			Burning.Make(gameObject, "ceiling tentacles", Plugin.TentacleHealth.Value, t.root, delegate
			{
				Tentacles.OnHit(t, 0f);
			}, delegate
			{
				Tentacles.Kill(t);
			});
			WorldGeneration world = WorldGeneration.world;
			if (world)
			{
				Vector2Int vector2Int = world.WorldToBlockPos(ceiling + Vector2.up * 0.5f);
				foreach (int num3 in new int[] { 0, -1, 1 })
				{
					Vector2Int vector2Int2;
					vector2Int2..ctor(vector2Int.x + num3, vector2Int.y);
					if (world.GetBlock(vector2Int2) != 0)
					{
						t.Holds.Add(vector2Int2);
					}
				}
				if (t.Holds.Contains(vector2Int))
				{
					t.RootHold = vector2Int;
				}
			}
			Tentacles.Traps.Add(t);
			return t;
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x00067D00 File Offset: 0x00065F00
		public static void RootNear(Vector2 at)
		{
			WorldGeneration world = WorldGeneration.world;
			Vector2? vector = Tentacles.RandomCeiling(at, 0f, 30f, new Vector2Int?(world.WorldToBlockPos(at)), 12);
			if (vector == null)
			{
				Plugin.Log.LogInfo(string.Format("A larva tried to take root near {0}, but found no ceiling", at));
				return;
			}
			Tentacles.Spawn(vector.Value);
			Plugin.Log.LogInfo(string.Format("A larva took root on the ceiling at {0}", vector.Value));
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x00067D84 File Offset: 0x00065F84
		public static void DebugSpawn(Body b)
		{
			WorldGeneration world = WorldGeneration.world;
			Vector2Int vector2Int = world.WorldToBlockPos(b.transform.position);
			for (int i = 2; i < 14; i++)
			{
				if (world.GetBlock(new Vector2Int(vector2Int.x, vector2Int.y + i)) != 0)
				{
					Tentacles.Spawn(world.BlockToWorldPos(new Vector2Int(vector2Int.x, vector2Int.y + i - 1)) + Vector2.up * 0.5f);
					Plugin.Log.LogInfo(string.Format("Debug: tentacles on the ceiling {0} blocks above her", i));
					return;
				}
			}
			Tentacles.Spawn(b.transform.position + Vector2.up * 5f);
			Plugin.Log.LogInfo("Debug: tentacles hanging in the air above her");
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x00067E64 File Offset: 0x00066064
		public static void Update(Body b, float dt)
		{
			Tentacles.Traps.RemoveAll((TentacleTrap t) => !t);
			foreach (TentacleTrap tentacleTrap in Tentacles.Traps.ToArray())
			{
				if (tentacleTrap.RockGone())
				{
					Plugin.Log.LogInfo(string.Format("The ceiling tentacles at {0} lost the rock they hung from", tentacleTrap.transform.position));
					Tentacles.Kill(tentacleTrap);
				}
			}
			if (Tentacles.grabCooldown > 0f)
			{
				Tentacles.grabCooldown -= dt;
			}
			if (Tentacles.attemptCooldown > 0f)
			{
				Tentacles.attemptCooldown -= dt;
			}
			Tentacles.S.exhaustion = (Tentacles.Holding ? Tentacles.S.exhaustion : Mathf.Max(0f, Tentacles.S.exhaustion - Plugin.ExhaustionRecovery.Value * dt));
			if (Tentacles.Holding && (!Tentacles.Holder || !b.alive))
			{
				Tentacles.Release(b, !b.alive);
				return;
			}
			if (Tentacles.Holding)
			{
				if (Keys.Down(Plugin.RemoveKey.Value))
				{
					Tentacles.Struggle(b);
				}
				if (Tentacles.Holding)
				{
					Tentacles.UpdateScene(b, dt);
				}
				return;
			}
			if (Tentacles.reaching)
			{
				Tentacles.UpdateReach(b, dt);
				return;
			}
			if (!b.alive || Tentacles.grabCooldown > 0f || Scenes.Busy || HerPuppet.Active)
			{
				return;
			}
			foreach (TentacleTrap tentacleTrap2 in Tentacles.Traps)
			{
				if (tentacleTrap2 && tentacleTrap2.sated <= 0f)
				{
					Vector2 vector = b.transform.position - tentacleTrap2.transform.position;
					if (Mathf.Abs(vector.x) < 1.8f && vector.y < -0.5f && vector.y > -9.5f)
					{
						Tentacles.reaching = tentacleTrap2;
						Tentacles.reachTime = 0f;
						Sound.Play("goo", tentacleTrap2.transform.position, false, true, null, 1f, 0.6f, false, false);
						break;
					}
				}
			}
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x000680E0 File Offset: 0x000662E0
		private static void UpdateReach(Body b, float dt)
		{
			Tentacles.reachTime += dt;
			TentacleTrap tentacleTrap = Tentacles.reaching;
			if (!tentacleTrap)
			{
				Tentacles.reaching = null;
				return;
			}
			float num = Mathf.Clamp01(Tentacles.reachTime / 0.5f);
			for (int i = 0; i < 7; i++)
			{
				Limb limb = b.LimbByName(Tentacles.Holds[i]);
				Vector2 vector = (limb ? limb.transform.position : b.transform.position);
				Vector2 vector2 = tentacleTrap.Anchor(i);
				Tentacles.DrawChain(tentacleTrap, i, vector2, Vector2.Lerp(vector2 + Vector2.down * tentacleTrap.idleLen[i], vector, num), 0.5f);
			}
			if (Tentacles.reachTime < 0.5f)
			{
				return;
			}
			Vector2 vector3 = b.transform.position - tentacleTrap.transform.position;
			Tentacles.reaching = null;
			if (b.alive && !Scenes.Busy && !HerPuppet.Active && Mathf.Abs(vector3.x) < 2.6f && vector3.y < 0f && vector3.y > -10.5f)
			{
				Tentacles.Grab(b, tentacleTrap);
				return;
			}
			tentacleTrap.sated = 3f;
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x00068230 File Offset: 0x00066430
		private static void Grab(Body b, TentacleTrap t)
		{
			Tentacles.Holder = t;
			Tentacles.timer = 0f;
			Tentacles.thrustPhase = 0f;
			Tentacles.climaxes = 0;
			Tentacles.forced = 0;
			Tentacles.climaxTimer = Random.Range(12f, 18f);
			Tentacles.lineTimer = 1f;
			Tentacles.attemptCooldown = 0.6f;
			float num = Mathf.Clamp01(Plugin.TentacleInsatiable.Value + Tentacles.S.corruption * 0.002f + Tentacles.S.submission * 0.002f);
			Tentacles.Insatiable = Random.value < num;
			Relief.ForceStop();
			Tentacles.Anal = Random.value < 0.45f;
			Tentacles.posed = Plugin.TentaclePosed.Value && Poses.All.ContainsKey("tent_hang") && HerPuppet.Begin(b);
			if (!Tentacles.posed)
			{
				b.Ragdoll();
				Traverse.Create(b).Field("movingAllowed").SetValue(false);
			}
			b.eyePanicTime = 1f;
			b.adrenaline += 30f;
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.5f, false, false);
			Sound.Play("ripclothing", b.transform.position, false, true, b.transform, 0.6f, 1f, false, false);
			Tentacles.wetLoop = Sfx.Loop(b, "loop_wet");
			if (Tentacles.wetLoop)
			{
				Tentacles.wetLoop.volume = 0f;
			}
			Relief.Say(b, "tent_grab");
			Plugin.Log.LogInfo(string.Format("Ceiling tentacles caught her ({0}, chance {1:P0})", Tentacles.Insatiable ? "insatiable" : "will be sated", num));
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x00068414 File Offset: 0x00066614
		private static void UpdateScene(Body b, float dt)
		{
			Tentacles.timer += dt;
			TentacleTrap holder = Tentacles.Holder;
			if (!Tentacles.posed)
			{
				b.shock = Mathf.Max(b.shock, 25f);
			}
			float num = Mathf.Clamp01(Tentacles.timer / 40f);
			Tentacles.MouthUsed = num > 0.6f && b.LimbByName("Head");
			float num2 = Mathf.Lerp(2.2f, 5.5f, num) * (Tentacles.Insatiable ? 1.2f : 1f);
			float num3 = Mathf.Sin(Tentacles.thrustPhase);
			Tentacles.thrustPhase += num2 * dt;
			float num4 = Mathf.Sin(Tentacles.thrustPhase);
			bool flag = num3 < 0.9f && num4 >= 0.9f;
			Vector2 vector = holder.transform.position + Vector2.down * 3.1f;
			if (Tentacles.posed)
			{
				float num5 = (num4 + 1f) * 0.5f;
				ScenePose scenePose;
				HerPuppet.Show(Poses.All["tent_hang"], Poses.All.TryGetValue("tent_hang_b", out scenePose) ? scenePose : Poses.All["tent_hang"], num5, holder.transform.position, b.isRight ? 1 : (-1));
			}
			else
			{
				Tentacles.Pull(b, "HandF", vector + new Vector2(-1f, 1.6f) + Tentacles.Sway(1), 7f, dt);
				Tentacles.Pull(b, "HandB", vector + new Vector2(1f, 1.6f) + Tentacles.Sway(2), 7f, dt);
				Tentacles.Pull(b, "UpTorso", vector + new Vector2(0f, 0.6f), 4f, dt);
				Tentacles.Pull(b, "DownTorso", vector + new Vector2(0f, -0.2f + num4 * 0.22f), 6f, dt);
				Tentacles.Pull(b, "FootF", vector + new Vector2(-1.1f, -1.9f) + Tentacles.Sway(3), 5f, dt);
				Tentacles.Pull(b, "FootB", vector + new Vector2(1.1f, -1.9f) + Tentacles.Sway(4), 5f, dt);
				if (b.rb)
				{
					b.rb.velocity = Vector2.zero;
				}
			}
			for (int i = 0; i < 7; i++)
			{
				Limb limb = b.LimbByName(Tentacles.Holds[i]);
				bool flag2 = limb && !limb.dismembered && (i < 5 || (i == 5 && num > 0.6f) || (i == 6 && Tentacles.timer > 6f));
				Vector2 vector2 = holder.Anchor(i);
				Vector2 vector3 = (flag2 ? limb.transform.position : (vector2 + Vector2.down * holder.idleLen[i]));
				if (i == 4 && flag2)
				{
					Vector2 vector4 = limb.transform.TransformPoint(Tentacles.Anal ? Tentacles.AnalPoint : Tentacles.VagPoint);
					Vector2 normalized = (vector4 - vector2).normalized;
					vector3 = vector4 + normalized * (0.1f + 0.35f * (num4 + 1f) * 0.5f);
				}
				if (i == 6 && flag2)
				{
					vector3 = limb.transform.TransformPoint(Tentacles.NipplePoint) + Random.insideUnitCircle * 0.02f;
				}
				Tentacles.DrawChain(holder, i, vector2, vector3, (i == 4) ? 1.4f : 1f);
			}
			Tentacles.S.arousal = Mathf.Min(100f, Tentacles.S.arousal + (2.2f + 2.5f * num) * dt);
			Tentacles.S.lust = Mathf.Min(100f, Tentacles.S.lust + 0.4f * dt);
			Tentacles.S.submission = Mathf.Min(100f, Tentacles.S.submission + 0.05f * dt);
			b.stamina = Mathf.Max(0f, b.stamina - 0.8f * dt);
			Tentacles.S.exhaustion = Mathf.Min(100f, Tentacles.S.exhaustion + 0.25f * dt);
			if (Tentacles.wetLoop)
			{
				Tentacles.wetLoop.volume = Mathf.MoveTowards(Tentacles.wetLoop.volume, 0.55f, dt);
			}
			if (flag)
			{
				Sound.Play("goo", b.transform.position, false, true, b.transform, 0.35f + 0.3f * num, Random.Range(0.8f, 1.1f), false, false);
				if (Random.value < 0.5f + 0.3f * num)
				{
					Sfx.Play(b, 0.55f + 0.3f * num, new string[]
					{
						(num > 0.6f) ? "muffled" : "moan_soft",
						"moan"
					});
				}
				b.eatTime = Mathf.Max(b.eatTime, 0.4f);
			}
			if (Tentacles.S.arousal >= 99f && Tentacles.S.orgasmTimer < 13f && b.conscious)
			{
				Plugin.Orgasm(b, false);
				Tentacles.forced++;
				Tentacles.S.exhaustion = Mathf.Min(100f, Tentacles.S.exhaustion + Plugin.ExhaustionPerOrgasm.Value);
				Tentacles.S.arousal = Mathf.Max(Tentacles.S.arousal, 55f);
				Relief.Say(b, (Tentacles.S.exhaustion > 70f) ? "tent_toomuch" : "tent_orgasm");
				Plugin.Log.LogInfo(string.Format("Tentacles forced orgasm {0}; exhaustion {1:0}", Tentacles.forced, Tentacles.S.exhaustion));
			}
			if ((Tentacles.climaxTimer -= dt) <= 0f)
			{
				Tentacles.climaxTimer = Random.Range(14f, 22f);
				Tentacles.climaxes++;
				Womb.Fill(b, 22f);
				Semen.Add(b, 10f);
				Sfx.Play(b, 0.6f, new string[] { "drip" });
				Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.5f, false, false);
				Tentacles.S.arousal = Mathf.Min(100f, Tentacles.S.arousal + 12f);
				bool flag3 = Womb.TryImpregnate(b, 1f);
				Relief.Say(b, "tent_climax");
				Plugin.Log.LogInfo(string.Format("Tentacle climax {0} inside her {1} (filled {2:0}{3})", new object[]
				{
					Tentacles.climaxes,
					Tentacles.Anal ? "anus" : "vagina",
					Tentacles.S.filled,
					flag3 ? ", seeded" : ""
				}));
				if (Random.value < 0.3f)
				{
					Tentacles.Anal = !Tentacles.Anal;
					Relief.Say(b, Tentacles.Anal ? "tent_switch_anal" : "tent_switch_vag");
				}
			}
			if ((Tentacles.lineTimer -= dt) <= 0f)
			{
				Tentacles.lineTimer = Random.Range(7f, 12f);
				Relief.Say(b, Tentacles.MouthUsed ? "tent_mouth" : ((Random.value < 0.35f) ? ((Random.value < 0.6f) ? (Tentacles.Anal ? "tent_hole_anal" : "tent_hole_vag") : "tent_hole") : ((num > 0.6f) ? "tent_deep" : "tent_scene")));
			}
			if (Tentacles.S.exhaustion >= 100f)
			{
				Heart.Overworked(b, dt);
			}
			if (!Tentacles.Insatiable && (Tentacles.climaxes >= 3 || Tentacles.timer > Plugin.TentacleSceneSeconds.Value))
			{
				Tentacles.Holder.sated = 150f;
				Relief.Say(b, "tent_sated");
				Plugin.Log.LogInfo(string.Format("The tentacles are sated after {0:0} s ({1} orgasms, {2} climaxes)", Tentacles.timer, Tentacles.forced, Tentacles.climaxes));
				Tentacles.Release(b, false);
			}
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x00068D00 File Offset: 0x00066F00
		private static Vector2 Sway(int seed)
		{
			return new Vector2(Mathf.Sin(Time.time * 1.3f + (float)seed * 2.1f), Mathf.Cos(Time.time * 1.1f + (float)seed)) * 0.12f;
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x00068D40 File Offset: 0x00066F40
		private static void Pull(Body b, string limbName, Vector2 target, float stiffness, float dt)
		{
			Limb limb = b.LimbByName(limbName);
			if (!limb || limb.dismembered || !limb.rb)
			{
				return;
			}
			Vector2 vector = (target - limb.transform.position) * stiffness;
			limb.rb.velocity = Vector2.Lerp(limb.rb.velocity, Vector2.ClampMagnitude(vector, 14f), Mathf.Clamp01(dt * 12f));
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x00068DC4 File Offset: 0x00066FC4
		private static void Struggle(Body b)
		{
			if (Tentacles.attemptCooldown > 0f || !b.conscious)
			{
				return;
			}
			Tentacles.attemptCooldown = 1f;
			float num = Mathf.Clamp(b.stamina / 100f, 0.1f, 1f) * (1f - Tentacles.S.exhaustion / 100f);
			float num2 = Plugin.TentacleEscapeChance.Value * Diff.Escape * num * ((Tentacles.S.orgasmTimer > 17f) ? 0.3f : 1f) * (1f - Tentacles.S.submission * 0.004f);
			num2 = Mathf.Max(num2, 0.01f);
			b.stamina = Mathf.Max(0f, b.stamina - 10f);
			bool flag = Random.value < num2;
			Plugin.Log.LogInfo(string.Format("Struggling in the tentacles: chance {0:P0} (strength {1:P0}), {2}", num2, num, flag ? "free" : "held"));
			if (flag)
			{
				Tentacles.Holder.sated = 8f;
				Relief.Say(b, "tent_escape");
				Tentacles.Release(b, false);
				return;
			}
			Tentacles.S.arousal = Mathf.Min(100f, Tentacles.S.arousal + 4f);
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.7f, 0.6f, false, false);
			Relief.Say(b, (Tentacles.S.exhaustion > 60f) ? "tent_weak" : "tent_fail");
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x00068F68 File Offset: 0x00067168
		public static void OnHit(TentacleTrap t, float damage)
		{
			if (!t)
			{
				return;
			}
			BuildingEntity component = t.GetComponent<BuildingEntity>();
			if (component)
			{
				component.health = 100000f;
			}
			if (Tentacles.Holder == t)
			{
				return;
			}
			if (PlayerCamera.main)
			{
				Body body = PlayerCamera.main.body;
			}
			t.sated = Mathf.Max(t.sated, Plugin.TentacleHurtSeconds.Value);
			Sound.Play("goo", t.transform.position, false, true, null, 0.9f, Random.Range(1.1f, 1.4f), false, false);
			if (Tentacles.reaching == t)
			{
				Tentacles.reaching = null;
				Tentacles.grabCooldown = Mathf.Max(Tentacles.grabCooldown, 2f);
			}
			Plugin.Log.LogInfo(string.Format("The tentacles are hurt ({0:0}): no grabbing for {1:0} s", damage, t.sated));
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x00069058 File Offset: 0x00067258
		internal static void Kill(TentacleTrap t)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && Tentacles.Holder == t)
			{
				Tentacles.Release(body, false);
			}
			if (Tentacles.reaching == t)
			{
				Tentacles.reaching = null;
			}
			Sound.Play("gore", t.transform.position, false, true, null, 1f, 1f, false, false);
			Sound.Play("goo", t.transform.position, false, true, null, 1f, 0.5f, false, false);
			if (body && body.alive)
			{
				Relief.Say(body, "tent_killed");
			}
			Plugin.Log.LogInfo(string.Format("The ceiling tentacles at {0} are dead", t.transform.position));
			Tentacles.Traps.Remove(t);
			Object.Destroy(t.gameObject);
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x00069158 File Offset: 0x00067358
		public static void OnDestroyed(TentacleTrap t)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && Tentacles.Holder == t)
			{
				Tentacles.Release(body, false);
			}
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0006919B File Offset: 0x0006739B
		public static void LetGo(Body b)
		{
			if (!Tentacles.Holding)
			{
				return;
			}
			if (Tentacles.Holder)
			{
				Tentacles.Holder.sated = 20f;
			}
			Tentacles.Release(b, false);
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x000691C8 File Offset: 0x000673C8
		private static void Release(Body b, bool quiet)
		{
			Tentacles.Holder = null;
			Tentacles.reaching = null;
			Tentacles.Insatiable = false;
			Tentacles.MouthUsed = false;
			if (Tentacles.posed)
			{
				HerPuppet.End();
				Tentacles.posed = false;
			}
			Tentacles.grabCooldown = 4f;
			if (Tentacles.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(Tentacles.wetLoop, 0.8f));
			}
			Tentacles.wetLoop = null;
			if (b)
			{
				Traverse.Create(b).Field("movingAllowed").SetValue(true);
				if (!quiet)
				{
					Sound.Play("goo", b.transform.position, false, true, b.transform, 0.8f, 1.2f, false, false);
				}
			}
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x00069290 File Offset: 0x00067490
		public static void DrawIdle(TentacleTrap t)
		{
			for (int i = 0; i < 7; i++)
			{
				Vector2 vector = t.Anchor(i);
				float num = t.idleLen[i] * ((t.sated > 0f) ? 0.6f : 1f);
				Vector2 vector2 = vector + new Vector2(Mathf.Sin(Time.time * 0.9f + (float)i * 1.7f) * 0.35f, -num);
				Tentacles.DrawChain(t, i, vector, vector2, 0.4f);
			}
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x00069310 File Offset: 0x00067510
		private static void DrawChain(TentacleTrap t, int i, Vector2 from, Vector2 to, float wiggle)
		{
			if (i >= t.chains.Count || !TentacleArt.Seg)
			{
				return;
			}
			SpriteRenderer[] array = t.chains[i];
			float x = TentacleArt.Seg.bounds.size.x;
			Vector2 vector = to - from;
			Vector2 normalized = new Vector2(-vector.y, vector.x).normalized;
			Vector2 vector2 = from;
			for (int j = 0; j < array.Length; j++)
			{
				float num = ((float)j + 1f) / (float)array.Length;
				float num2 = Mathf.Sin(Time.time * 4f + (float)i * 1.9f + num * 5f) * 0.18f * wiggle * Mathf.Sin(num * 3.1415927f);
				Vector2 vector3 = from + vector * num + normalized * num2;
				Vector2 vector4 = vector3 - vector2;
				SpriteRenderer spriteRenderer = array[j];
				if (spriteRenderer)
				{
					spriteRenderer.transform.position = vector2;
					spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector4.y, vector4.x) * 57.29578f);
					float num3 = Mathf.Lerp(1.5f, 0.8f, num);
					spriteRenderer.transform.localScale = new Vector3(vector4.magnitude / x + 0.12f, num3, 1f);
					vector2 = vector3;
				}
			}
			SpriteRenderer spriteRenderer2 = ((i < t.tips.Count) ? t.tips[i] : null);
			if (spriteRenderer2)
			{
				Vector2 vector5 = to - array[array.Length - 1].transform.position;
				spriteRenderer2.transform.position = vector2;
				spriteRenderer2.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector5.y, vector5.x) * 57.29578f);
			}
		}

		// Token: 0x040006F9 RID: 1785
		public static readonly List<TentacleTrap> Traps = new List<TentacleTrap>();

		// Token: 0x040006FD RID: 1789
		private static TentacleTrap reaching;

		// Token: 0x040006FE RID: 1790
		private static float reachTime;

		// Token: 0x040006FF RID: 1791
		private static float timer;

		// Token: 0x04000700 RID: 1792
		private static float thrustPhase;

		// Token: 0x04000701 RID: 1793
		private static float climaxTimer;

		// Token: 0x04000702 RID: 1794
		private static float lineTimer;

		// Token: 0x04000703 RID: 1795
		private static float attemptCooldown;

		// Token: 0x04000704 RID: 1796
		private static float grabCooldown;

		// Token: 0x04000705 RID: 1797
		private static int climaxes;

		// Token: 0x04000706 RID: 1798
		private static int forced;

		// Token: 0x04000707 RID: 1799
		private static AudioSource wetLoop;

		// Token: 0x04000708 RID: 1800
		private static readonly string[] Holds = new string[] { "HandF", "HandB", "FootF", "FootB", "DownTorso", "Head", "UpTorso" };

		// Token: 0x0400070A RID: 1802
		private static bool posed;

		// Token: 0x0400070B RID: 1803
		private static readonly Vector2 VagPoint = new Vector2(0.22f, -0.55f);

		// Token: 0x0400070C RID: 1804
		private static readonly Vector2 AnalPoint = new Vector2(-0.3f, -0.42f);

		// Token: 0x0400070D RID: 1805
		private static readonly Vector2 NipplePoint = new Vector2(0.4f, 0.02f);
	}
}
