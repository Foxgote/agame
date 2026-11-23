using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using PoseKit;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000AE RID: 174
	internal static class LatexPond
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060003C9 RID: 969 RVA: 0x0003E76D File Offset: 0x0003C96D
		// (set) Token: 0x060003CA RID: 970 RVA: 0x0003E774 File Offset: 0x0003C974
		public static LatexPond.Pond Holder { get; private set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060003CB RID: 971 RVA: 0x0003E77C File Offset: 0x0003C97C
		public static bool Holding
		{
			get
			{
				return LatexPond.Holder != null && LatexPond.now == LatexPond.Phase.Sinking;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060003CC RID: 972 RVA: 0x0003E78F File Offset: 0x0003C98F
		public static bool Inside
		{
			get
			{
				return LatexPond.Holder != null && (LatexPond.now == LatexPond.Phase.Under || LatexPond.now == LatexPond.Phase.Spitting);
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060003CD RID: 973 RVA: 0x0003E7AC File Offset: 0x0003C9AC
		public static bool Busy
		{
			get
			{
				return LatexPond.Holder != null;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060003CE RID: 974 RVA: 0x0003E7B6 File Offset: 0x0003C9B6
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060003CF RID: 975 RVA: 0x0003E7BD File Offset: 0x0003C9BD
		private static WorldGeneration W
		{
			get
			{
				return WorldGeneration.world;
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0003E7C4 File Offset: 0x0003C9C4
		public static void SeedLayer(Body b)
		{
			if (!LatexPond.W || LatexPond.W.biomeOverride != null || !LatexArt.Ready)
			{
				return;
			}
			if (LatexPond.W.biomeDepth < Mathf.RoundToInt(Plugin.PondFirstLayer.Value))
			{
				Plugin.Log.LogInfo(string.Format("Layer {0}: no latex pond (too shallow)", LatexPond.W.biomeDepth));
				return;
			}
			int num = Diff.Count(Plugin.PondPerLayer.Value);
			int num2 = 0;
			int num3 = 0;
			while (num3 < 12000 && num2 < num)
			{
				Vector2Int vector2Int;
				vector2Int..ctor(Random.Range(10, (int)(LatexPond.W.width - 10U)), Random.Range(10, (int)(LatexPond.W.height - 10U)));
				if (LatexPond.W.GetBlock(vector2Int) == 0)
				{
					int num4 = 0;
					while (num4 < 40 && vector2Int.y > 2 && LatexPond.W.GetBlock(vector2Int) == 0)
					{
						vector2Int += Vector2Int.down;
						num4++;
					}
					if (Vector2.Distance(LatexPond.W.BlockToWorldPos(vector2Int), b.transform.position) >= 30f && LatexPond.Fits(vector2Int, false) && !LatexPond.TooClose(vector2Int) && LatexPond.Make(vector2Int, b) != null)
					{
						num2++;
					}
				}
				num3++;
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: {1} of {2} latex ponds", LatexPond.W.biomeDepth, num2, num));
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x0003E940 File Offset: 0x0003CB40
		private static IEnumerable<Vector2Int> Cells()
		{
			int num;
			for (int x = -4; x <= 4; x = num + 1)
			{
				yield return new Vector2Int(x, 0);
				num = x;
			}
			foreach (Vector2Int vector2Int in Pits.Bowl(4, 5))
			{
				yield return vector2Int;
			}
			List<Vector2Int>.Enumerator enumerator = default(List<Vector2Int>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0003E94C File Offset: 0x0003CB4C
		private static bool Fits(Vector2Int top, bool debug = false)
		{
			string text;
			return LatexPond.Fits(top, debug, out text);
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0003E964 File Offset: 0x0003CB64
		private static bool Fits(Vector2Int top, bool debug, out string why)
		{
			int num = (debug ? 8 : 30);
			why = "the world's edge";
			if (top.y - 5 < (debug ? 3 : 30) || (long)top.y > (long)((ulong)LatexPond.W.height - (ulong)((long)num)) || top.x - 4 - 4 < 2 || (long)(top.x + 4 + 4) > (long)((ulong)(LatexPond.W.width - 2U)))
			{
				return false;
			}
			for (int i = -6; i <= 6; i++)
			{
				why = "the floor is not level";
				if (LatexPond.W.GetBlock(new Vector2Int(top.x + i, top.y)) == 0)
				{
					return false;
				}
				why = "no room over the floor";
				for (int j = 1; j <= 3; j++)
				{
					if (LatexPond.W.GetBlock(new Vector2Int(top.x + i, top.y + j)) != 0)
					{
						return false;
					}
				}
			}
			FluidManager main = FluidManager.main;
			int num2 = 0;
			int num3 = 0;
			foreach (Vector2Int vector2Int in LatexPond.Cells())
			{
				Vector2Int vector2Int2 = top + vector2Int;
				ushort block = LatexPond.W.GetBlock(vector2Int2);
				num3++;
				why = "too hard a floor";
				if (block == 0)
				{
					num2++;
				}
				else if (!debug && LatexPond.W.GetBlockInfo(block).health > 2000f)
				{
					return false;
				}
				why = "liquid there";
				if (main && main.GetLiquid(vector2Int2.x, vector2Int2.y) != 0)
				{
					return false;
				}
			}
			why = "hollow under the floor";
			return debug || num2 <= num3 / 4;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x0003EB30 File Offset: 0x0003CD30
		private static bool TooClose(Vector2Int top)
		{
			Vector2 at = LatexPond.W.BlockToWorldPos(top);
			return LatexPond.All.Any<LatexPond.Pond>((LatexPond.Pond p) => Vector2.Distance(p.Mouth, at) < 25f) || Pits.All.Any<Pit>((Pit p) => Vector2.Distance(p.Mouth, at) < 15f);
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0003EB84 File Offset: 0x0003CD84
		public static LatexPond.Pond Make(Vector2Int top, Body b)
		{
			SpriteRenderer spriteRenderer = (b ? b.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				LatexArt.Ensure(spriteRenderer.sprite.pixelsPerUnit);
			}
			Sprite sprite = LatexArt.Get("pond_0");
			if (!sprite)
			{
				return null;
			}
			FluidManager main = FluidManager.main;
			foreach (Vector2Int vector2Int in LatexPond.Cells())
			{
				Vector2Int vector2Int2 = top + vector2Int;
				if (LatexPond.W.GetBlock(vector2Int2) != 0)
				{
					LatexPond.W.SetBlock(vector2Int2, 0);
				}
				if (main != null)
				{
					main.SetLiquid(vector2Int2.x, vector2Int2.y, 0);
				}
			}
			if (b)
			{
				LatexPond.herOrder = b.GetComponentsInChildren<SpriteRenderer>(true).Max<SpriteRenderer>((SpriteRenderer r) => r.sortingOrder);
			}
			LatexPond.Pond pond = new LatexPond.Pond
			{
				Top = top
			};
			pond.go = new GameObject("LewdLatexPond");
			pond.go.transform.position = new Vector3(pond.Mouth.x, pond.Mouth.y + 0.5f, 0f);
			pond.sr = pond.go.AddComponent<SpriteRenderer>();
			pond.sr.sprite = sprite;
			if (LatexPond.W)
			{
				pond.sr.sharedMaterial = LatexPond.W.defaultMat;
			}
			if (spriteRenderer)
			{
				pond.sr.sortingLayerID = spriteRenderer.sortingLayerID;
			}
			pond.sr.sortingOrder = LatexPond.herOrder + 4;
			pond.go.layer = LayerMask.NameToLayer("Descriptor");
			BoxCollider2D boxCollider2D = pond.go.AddComponent<BoxCollider2D>();
			boxCollider2D.size = new Vector2(9f, 1.2f);
			boxCollider2D.offset = new Vector2(0f, -0.7f);
			BuildingEntity buildingEntity = pond.go.AddComponent<BuildingEntity>();
			buildingEntity.animal = true;
			buildingEntity.health = 100000f;
			buildingEntity.id = "lewd_latexpond";
			buildingEntity.ignoreBodyOptimize = true;
			buildingEntity.itemsDropOnDestroy = new ItemDrop[0];
			buildingEntity.alwaysDrop = new ItemDrop[0];
			buildingEntity.itemCategoriesToAdd = new string[0];
			buildingEntity.hitSound = Resources.Load<AudioClip>("Sounds/goo");
			pond.go.AddComponent<LatexPondHit>().pond = pond;
			GameObject gameObject = new GameObject("LewdLatexPondBulge");
			pond.bulge = gameObject.AddComponent<SpriteRenderer>();
			if (LatexPond.W)
			{
				pond.bulge.sharedMaterial = LatexPond.W.defaultMat;
			}
			if (spriteRenderer)
			{
				pond.bulge.sortingLayerID = spriteRenderer.sortingLayerID;
			}
			pond.bulge.sortingOrder = LatexPond.herOrder + 5;
			pond.bulge.enabled = false;
			LatexPond.All.Add(pond);
			Plugin.Log.LogInfo(string.Format("Latex pond at {0}", pond.Mouth));
			return pond;
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0003EEB4 File Offset: 0x0003D0B4
		public static void Clear(Body b)
		{
			if (b && LatexPond.Busy)
			{
				LatexPond.Release(b, false);
			}
			foreach (LatexPond.Pond pond in LatexPond.All)
			{
				if (pond.go)
				{
					Object.Destroy(pond.go);
				}
				if (pond.bulge)
				{
					Object.Destroy(pond.bulge.gameObject);
				}
			}
			LatexPond.All.Clear();
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x0003EF54 File Offset: 0x0003D154
		public static void Update(Body b, float dt)
		{
			LatexPond.All.RemoveAll((LatexPond.Pond p) => !p.go);
			foreach (LatexPond.Pond pond in LatexPond.All)
			{
				if (pond.sated > 0f)
				{
					pond.sated -= dt;
				}
				pond.frame += dt;
				Sprite sprite = LatexArt.Get("pond_" + ((int)(pond.frame * 2.5f) % 4).ToString());
				if (sprite)
				{
					pond.sr.sprite = sprite;
				}
				pond.sr.color = ((LatexPond.Holder == pond && LatexPond.Inside) ? new Color(1f, 1f, 1f, 0.55f) : Color.white);
			}
			if (!b || !LatexPond.W)
			{
				return;
			}
			if (LatexPond.attemptCooldown > 0f)
			{
				LatexPond.attemptCooldown -= dt;
			}
			if (LatexPond.Busy && !b.alive)
			{
				LatexPond.Release(b, false);
				return;
			}
			switch (LatexPond.now)
			{
			case LatexPond.Phase.Sinking:
				LatexPond.Sink(b, dt);
				break;
			case LatexPond.Phase.Under:
				LatexPond.Under(b, dt);
				break;
			case LatexPond.Phase.Spitting:
				LatexPond.Spit(b, dt);
				break;
			default:
				if (b.alive && !Scenes.Busy && !HerPuppet.Active)
				{
					LatexPond.Watch(b);
				}
				break;
			}
			if (LatexPond.Busy)
			{
				LatexPond.DrawTendrils(b);
			}
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x0003F114 File Offset: 0x0003D314
		private static bool InPond(LatexPond.Pond p, Vector2 at)
		{
			return Mathf.Abs(at.x - p.Mouth.x) < 4.4f && at.y < p.Surface && at.y > p.Bottom - 0.5f;
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x0003F164 File Offset: 0x0003D364
		private static void Watch(Body b)
		{
			foreach (LatexPond.Pond pond in LatexPond.All)
			{
				if (pond.sated <= 0f)
				{
					foreach (string text in new string[] { "FootF", "FootB" })
					{
						Limb limb = b.LimbByName(text);
						if (limb && !limb.dismembered && LatexPond.InPond(pond, limb.transform.position))
						{
							LatexPond.Begin(b, pond);
							return;
						}
					}
				}
			}
		}

		// Token: 0x060003DA RID: 986 RVA: 0x0003F228 File Offset: 0x0003D428
		private static void Begin(Body b, LatexPond.Pond p)
		{
			LatexPond.Holder = p;
			LatexPond.now = LatexPond.Phase.Sinking;
			LatexPond.timer = 0f;
			LatexPond.attemptCooldown = 0.6f;
			LatexPond.lineTimer = 2.5f;
			LatexPond.pulse = 2f;
			LatexPond.orgasmGap = 0f;
			LatexPond.orgasms = 0;
			LatexPond.drowning = false;
			LatexPond.herOrder = b.GetComponentsInChildren<SpriteRenderer>(true).Max<SpriteRenderer>((SpriteRenderer r) => r.sortingOrder);
			p.sr.sortingOrder = LatexPond.herOrder + 4;
			p.bulge.sortingOrder = LatexPond.herOrder + 5;
			Relief.ForceStop();
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			LatexPond.MakeTendrils(LatexPond.LegHold);
			LatexPond.wetLoop = (Sfx.Has("loop_slime") ? Sfx.Loop(b, "loop_slime") : Sfx.Loop(b, "loop_wet"));
			Sound.Play("goo", b.transform.position, false, true, b.transform, 1f, 0.6f, false, false);
			Relief.Say(b, "pond_grab");
			Plugin.Log.LogInfo(string.Format("The latex pond at {0} has her", p.Mouth));
		}

		// Token: 0x060003DB RID: 987 RVA: 0x0003F380 File Offset: 0x0003D580
		private static void Sink(Body b, float dt)
		{
			LatexPond.Pond holder = LatexPond.Holder;
			LatexPond.timer += dt;
			Vector2 vector;
			vector..ctor(holder.Mouth.x, holder.Bottom + 1.2f);
			foreach (string text in new string[] { "FootF", "FootB" })
			{
				LatexPond.Spring(b.LimbByName(text), vector + new Vector2((text == "FootF") ? 0.3f : (-0.3f), 0f), Plugin.PondPull.Value, dt);
			}
			if (b.rb)
			{
				b.rb.velocity += new Vector2((holder.Mouth.x - b.transform.position.x) * 2f, -Plugin.PondPull.Value) * dt;
			}
			b.shock = Mathf.Max(b.shock, 10f);
			LatexPond.S.arousal = Mathf.Min(100f, LatexPond.S.arousal + 0.8f * dt);
			if (Keys.Down(Plugin.RemoveKey.Value))
			{
				LatexPond.Struggle(b, false);
			}
			if (!LatexPond.Busy)
			{
				return;
			}
			if ((LatexPond.lineTimer -= dt) <= 0f)
			{
				LatexPond.lineTimer = Random.Range(3f, 5f);
				Relief.Say(b, "pond_sink");
			}
			Limb limb = b.LimbByName("Head");
			if (limb && limb.transform.position.y < holder.Surface - 0.25f)
			{
				LatexPond.now = LatexPond.Phase.Under;
				LatexPond.timer = 0f;
				LatexPond.MakeTendrils(LatexPond.UnderHold);
				Sound.Play("goo", limb.transform.position, false, true, null, 1f, 0.45f, false, false);
				Relief.Say(b, "pond_under");
				Plugin.Log.LogInfo("The latex pond closed over her head");
			}
		}

		// Token: 0x060003DC RID: 988 RVA: 0x0003F5A8 File Offset: 0x0003D7A8
		private static void Under(Body b, float dt)
		{
			LatexPond.Pond holder = LatexPond.Holder;
			LatexPond.timer += dt;
			Vector2 vector;
			vector..ctor(holder.Mouth.x, (holder.Surface + holder.Bottom) * 0.5f);
			float num = Mathf.Sin(Time.time * 2.2f) * 0.1f;
			LatexPond.Spring(b.LimbByName("UpTorso"), vector + new Vector2(0f, 0.6f + num), 5f, dt);
			LatexPond.Spring(b.LimbByName("DownTorso"), vector + new Vector2(0f, -0.2f), 5f, dt);
			LatexPond.Spring(b.LimbByName("HandF"), vector + new Vector2(1.6f, 1.3f), 4f, dt);
			LatexPond.Spring(b.LimbByName("HandB"), vector + new Vector2(-1.6f, 1.3f), 4f, dt);
			LatexPond.Spring(b.LimbByName("FootF"), vector + new Vector2(1.2f, -1.6f), 4f, dt);
			LatexPond.Spring(b.LimbByName("FootB"), vector + new Vector2(-1.2f, -1.6f), 4f, dt);
			b.shock = Mathf.Max(b.shock, 12f);
			if (LatexPond.drowning)
			{
				b.bloodOxygen = 0f;
				return;
			}
			if (Keys.Down(Plugin.RemoveKey.Value))
			{
				LatexPond.Struggle(b, true);
			}
			if (!LatexPond.Busy)
			{
				return;
			}
			LatexPond.S.arousal = Mathf.Min(100f, LatexPond.S.arousal + 2.2f * dt);
			LatexPond.S.lust = Mathf.Min(100f, LatexPond.S.lust + 0.4f * dt);
			b.bloodOxygen = Mathf.Max(5f, b.bloodOxygen - 1.5f * dt);
			if (LatexPond.wetLoop)
			{
				LatexPond.wetLoop.volume = Mathf.MoveTowards(LatexPond.wetLoop.volume, 0.5f, dt);
			}
			if ((LatexPond.pulse -= dt) <= 0f)
			{
				LatexPond.pulse = Random.Range(2.5f, 4.5f);
				LatexPond.S.arousal = Mathf.Min(100f, LatexPond.S.arousal + 8f);
				b.eatTime = Mathf.Max(b.eatTime, 0.6f);
				Sfx.Play(b, 0.75f, new string[] { "muffled", "moan" });
				Sfx.PlayMix(b, 0.7f, new string[] { "tentacle_thrust" });
			}
			LatexPond.orgasmGap -= dt;
			if (LatexPond.S.arousal >= 99f && LatexPond.orgasmGap <= 0f && b.conscious)
			{
				LatexPond.orgasmGap = 5f;
				LatexPond.orgasms++;
				Plugin.Orgasm(b, false);
				LatexPond.S.arousal = Mathf.Max(LatexPond.S.arousal, 50f);
				Relief.Say(b, "pond_come");
			}
			if ((LatexPond.lineTimer -= dt) <= 0f)
			{
				LatexPond.lineTimer = Random.Range(5f, 8f);
				Relief.Say(b, "pond_inside");
			}
			if (LatexPond.timer < Plugin.PondUnderSeconds.Value)
			{
				return;
			}
			if (Diff.LethalScenes)
			{
				LatexPond.drowning = true;
				Relief.Say(b, "pond_end");
				Plugin.Log.LogInfo("The latex pond kept her under: she drowns in it");
				return;
			}
			LatexPond.now = LatexPond.Phase.Spitting;
			LatexPond.timer = 0f;
			Plugin.Log.LogInfo("The latex pond is done with her: it spits her out");
		}

		// Token: 0x060003DD RID: 989 RVA: 0x0003F988 File Offset: 0x0003DB88
		private static void Spit(Body b, float dt)
		{
			LatexPond.Pond holder = LatexPond.Holder;
			LatexPond.timer += dt;
			Sprite sprite = LatexArt.Get("pondbulge_" + Mathf.Clamp((int)(LatexPond.timer / 1.3f * 3f), 0, 2).ToString());
			if (sprite)
			{
				holder.bulge.sprite = sprite;
				holder.bulge.enabled = true;
				holder.bulge.transform.position = new Vector3(holder.Mouth.x, holder.Surface - 0.1f, 0f);
			}
			if (LatexPond.timer < 1.3f)
			{
				return;
			}
			holder.bulge.enabled = false;
			Sound.Play("goo", holder.bulge.transform.position, false, true, null, 1f, 0.4f, false, false);
			Relief.Say(b, "pond_spat");
			LatexPond.Release(b, true);
			holder.sated = Mathf.Max(1.5f, Plugin.PondSatedSeconds.Value);
		}

		// Token: 0x060003DE RID: 990 RVA: 0x0003FAA0 File Offset: 0x0003DCA0
		private static void Struggle(Body b, bool under)
		{
			if (LatexPond.attemptCooldown > 0f || !b.conscious)
			{
				return;
			}
			LatexPond.attemptCooldown = 1f;
			float num = Mathf.Clamp(b.stamina / 100f, 0.15f, 1f) * (1f - LatexPond.S.exhaustion / 120f);
			float num2;
			if (under)
			{
				num2 = Plugin.PondEscapeUnder.Value * (1f - Mathf.Clamp01(LatexPond.timer / Mathf.Max(1f, Plugin.PondUnderSeconds.Value)));
			}
			else
			{
				Limb limb = b.LimbByName("Head");
				float num3 = (limb ? Mathf.Clamp01((LatexPond.Holder.Surface + 2.5f - limb.transform.position.y) / 3f) : 0.5f);
				num2 = Plugin.PondEscape.Value * (1f - num3 * 0.6f);
			}
			num2 = Mathf.Max(0.01f, num2 * Diff.Escape * num * (1f - LatexPond.S.submission * 0.004f));
			b.stamina = Mathf.Max(0f, b.stamina - 9f);
			bool flag = Random.value < num2;
			Plugin.Log.LogInfo(string.Format("Struggling in the latex pond ({0}): chance {1:P0}, {2}", under ? "under" : "sinking", num2, flag ? "free" : "held"));
			if (flag)
			{
				Relief.Say(b, "pond_escape");
				LatexPond.Pond holder = LatexPond.Holder;
				LatexPond.Release(b, true);
				holder.sated = 1.5f;
				return;
			}
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.8f, 0.6f, false, false);
			Relief.Say(b, "pond_fail");
		}

		// Token: 0x060003DF RID: 991 RVA: 0x0003FC80 File Offset: 0x0003DE80
		public static void OnHit(LatexPond.Pond p)
		{
			if (p == null)
			{
				return;
			}
			Sound.Play("goo", p.Mouth, false, true, null, 0.9f, Random.Range(1.1f, 1.4f), false, false);
			if (LatexPond.Holder != p || LatexPond.now != LatexPond.Phase.Sinking)
			{
				return;
			}
			p.hits = ((Time.time - p.lastHit < 6f) ? (p.hits + 1) : 1);
			p.lastHit = Time.time;
			if ((float)p.hits < Plugin.PondLetGoHits.Value)
			{
				return;
			}
			p.hits = 0;
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			Plugin.Log.LogInfo("Shot at, the latex pond lets go of her");
			if (body)
			{
				Relief.Say(body, "pond_escape");
				LatexPond.Release(body, true);
			}
			p.sated = 1.5f;
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x0003FD64 File Offset: 0x0003DF64
		public static void LetGo(Body b)
		{
			if (LatexPond.Busy)
			{
				LatexPond.Release(b, true);
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0003FD74 File Offset: 0x0003DF74
		private static void Release(Body b, bool toss)
		{
			LatexPond.Pond holder = LatexPond.Holder;
			LatexPond.Holder = null;
			LatexPond.now = LatexPond.Phase.None;
			LatexPond.drowning = false;
			LatexPond.ClearTendrils();
			if (holder != null)
			{
				if (holder.bulge)
				{
					holder.bulge.enabled = false;
				}
				if (holder.sr)
				{
					holder.sr.color = Color.white;
				}
			}
			if (LatexPond.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(LatexPond.wetLoop, 0.6f));
			}
			LatexPond.wetLoop = null;
			if (!b)
			{
				return;
			}
			Traverse.Create(b).Field("movingAllowed").SetValue(true);
			if (toss && holder != null && b.alive)
			{
				float num = ((b.transform.position.x >= holder.Mouth.x) ? 1f : (-1f));
				Vector2 vector;
				vector..ctor(num * 6.5f, 13f);
				foreach (Limb limb in b.limbs)
				{
					if (limb && limb.rb)
					{
						limb.rb.velocity = vector;
					}
				}
				if (b.rb)
				{
					b.rb.velocity = vector;
				}
				b.wetness = 100f;
			}
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x0003FEE8 File Offset: 0x0003E0E8
		private static void MakeTendrils(string[] limbs)
		{
			LatexPond.ClearTendrils();
			LatexPond.Pond holder = LatexPond.Holder;
			for (int i = 0; i < limbs.Length; i++)
			{
				SpriteRenderer[] array = new SpriteRenderer[10];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = LatexPond.Part("LewdPondArm", LatexArt.Get("seg"), LatexPond.herOrder + 2);
				}
				LatexPond.tendrils.Add(new ValueTuple<SpriteRenderer[], SpriteRenderer, string, float>(array, LatexPond.Part("LewdPondArmTip", LatexArt.Get("tip"), LatexPond.herOrder + 3), limbs[i], ((float)i / ((float)limbs.Length - 1f) - 0.5f) * 2f * 3f));
			}
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x0003FF98 File Offset: 0x0003E198
		private static SpriteRenderer Part(string name, Sprite sp, int order)
		{
			SpriteRenderer spriteRenderer = new GameObject(name).AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sp;
			if (LatexPond.W)
			{
				spriteRenderer.sharedMaterial = LatexPond.W.defaultMat;
			}
			SpriteRenderer spriteRenderer2 = (PlayerCamera.main ? PlayerCamera.main.body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer2)
			{
				spriteRenderer.sortingLayerID = spriteRenderer2.sortingLayerID;
			}
			spriteRenderer.sortingOrder = order;
			return spriteRenderer;
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x00040018 File Offset: 0x0003E218
		private static void DrawTendrils(Body b)
		{
			LatexPond.Pond holder = LatexPond.Holder;
			foreach (ValueTuple<SpriteRenderer[], SpriteRenderer, string, float> valueTuple in LatexPond.tendrils)
			{
				SpriteRenderer[] item = valueTuple.Item1;
				SpriteRenderer item2 = valueTuple.Item2;
				string item3 = valueTuple.Item3;
				float item4 = valueTuple.Item4;
				Limb limb = b.LimbByName(item3);
				if (!limb || limb.dismembered)
				{
					limb = b.limbs[1];
				}
				Vector2 vector;
				vector..ctor(holder.Mouth.x + item4, holder.Bottom + 0.4f);
				Latexes.Chain(item, item2, vector, limb.transform.position, 0.3f);
			}
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x000400F4 File Offset: 0x0003E2F4
		private static void ClearTendrils()
		{
			foreach (ValueTuple<SpriteRenderer[], SpriteRenderer, string, float> valueTuple in LatexPond.tendrils)
			{
				SpriteRenderer[] item = valueTuple.Item1;
				SpriteRenderer item2 = valueTuple.Item2;
				foreach (SpriteRenderer spriteRenderer in item)
				{
					if (spriteRenderer)
					{
						Object.Destroy(spriteRenderer.gameObject);
					}
				}
				if (item2)
				{
					Object.Destroy(item2.gameObject);
				}
			}
			LatexPond.tendrils.Clear();
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00040198 File Offset: 0x0003E398
		private static void Spring(Limb l, Vector2 target, float stiffness, float dt)
		{
			if (!l || l.dismembered || !l.rb)
			{
				return;
			}
			Vector2 vector = (target - l.transform.position) * stiffness;
			l.rb.velocity = Vector2.Lerp(l.rb.velocity, Vector2.ClampMagnitude(vector, 12f), Mathf.Clamp01(dt * 12f));
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x00040214 File Offset: 0x0003E414
		public static string DebugMake(Body b)
		{
			if (!LatexPond.W)
			{
				return "no world";
			}
			Vector2Int vector2Int = LatexPond.W.WorldToBlockPos(b.transform.position);
			int num = (b.isRight ? 1 : (-1));
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			for (int i = 7; i <= 24; i++)
			{
				foreach (int num2 in new int[]
				{
					num,
					-num
				})
				{
					Vector2Int vector2Int2;
					vector2Int2..ctor(vector2Int.x + num2 * i, vector2Int.y + 4);
					int num3 = 0;
					while (num3 < 14 && vector2Int2.y > 2 && LatexPond.W.GetBlock(vector2Int2) == 0)
					{
						vector2Int2 += Vector2Int.down;
						num3++;
					}
					string text;
					if (LatexPond.W.GetBlock(vector2Int2) == 0 || LatexPond.W.GetBlock(vector2Int2 + Vector2Int.up) != 0)
					{
						LatexPond.Count(dictionary, "no floor under open air");
					}
					else if (!LatexPond.Fits(vector2Int2, true, out text))
					{
						LatexPond.Count(dictionary, text);
					}
					else
					{
						LatexPond.Pond pond = LatexPond.Make(vector2Int2, b);
						if (pond == null)
						{
							return "no art";
						}
						return string.Format("latex pond at {0}, {1:0.0} from her", pond.Mouth, Vector2.Distance(pond.Mouth, b.transform.position));
					}
				}
			}
			return "no place for a latex pond near her: " + string.Join(", ", from kv in dictionary
				orderby kv.Value descending
				select string.Format("{0} x{1}", kv.Key, kv.Value));
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x000403EC File Offset: 0x0003E5EC
		private static void Count(Dictionary<string, int> d, string k)
		{
			int num;
			d[k] = (d.TryGetValue(k, out num) ? (num + 1) : 1);
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00040414 File Offset: 0x0003E614
		public static string Info(Body b)
		{
			return string.Format("{0} ponds; phase {1} for {2:0.0}s, orgasms {3}, drowning {4}, oxygen {5:0}; ", new object[]
			{
				LatexPond.All.Count,
				LatexPond.now,
				LatexPond.timer,
				LatexPond.orgasms,
				LatexPond.drowning,
				b ? b.bloodOxygen : 0f
			}) + string.Join(", ", LatexPond.All.Select<LatexPond.Pond, string>((LatexPond.Pond p) => string.Format("{0:0} sated {1:0}", p.Mouth, p.sated)));
		}

		// Token: 0x0400037A RID: 890
		public const int Half = 4;

		// Token: 0x0400037B RID: 891
		public const int Depth = 5;

		// Token: 0x0400037C RID: 892
		private const float SurfaceDrop = 0.375f;

		// Token: 0x0400037D RID: 893
		private const float SpitSeconds = 1.3f;

		// Token: 0x0400037E RID: 894
		private const float Grace = 1.5f;

		// Token: 0x0400037F RID: 895
		public static readonly List<LatexPond.Pond> All = new List<LatexPond.Pond>();

		// Token: 0x04000381 RID: 897
		private static LatexPond.Phase now;

		// Token: 0x04000382 RID: 898
		private static float timer;

		// Token: 0x04000383 RID: 899
		private static float attemptCooldown;

		// Token: 0x04000384 RID: 900
		private static float lineTimer;

		// Token: 0x04000385 RID: 901
		private static float pulse;

		// Token: 0x04000386 RID: 902
		private static float orgasmGap;

		// Token: 0x04000387 RID: 903
		private static bool drowning;

		// Token: 0x04000388 RID: 904
		private static int orgasms;

		// Token: 0x04000389 RID: 905
		private static AudioSource wetLoop;

		// Token: 0x0400038A RID: 906
		[TupleElementNames(new string[] { "segs", "tip", "limb", "root" })]
		private static readonly List<ValueTuple<SpriteRenderer[], SpriteRenderer, string, float>> tendrils = new List<ValueTuple<SpriteRenderer[], SpriteRenderer, string, float>>();

		// Token: 0x0400038B RID: 907
		private static readonly string[] LegHold = new string[] { "FootF", "FootB", "ThighF", "DownTorso" };

		// Token: 0x0400038C RID: 908
		private static readonly string[] UnderHold = new string[] { "FootF", "FootB", "HandF", "HandB", "DownTorso" };

		// Token: 0x0400038D RID: 909
		private static int herOrder;

		// Token: 0x020001CD RID: 461
		internal class Pond
		{
			// Token: 0x17000134 RID: 308
			// (get) Token: 0x060009C7 RID: 2503 RVA: 0x0007A2E5 File Offset: 0x000784E5
			public Vector2 Mouth
			{
				get
				{
					return WorldGeneration.world.BlockToWorldPos(this.Top);
				}
			}

			// Token: 0x17000135 RID: 309
			// (get) Token: 0x060009C8 RID: 2504 RVA: 0x0007A2F7 File Offset: 0x000784F7
			public float Surface
			{
				get
				{
					return this.Mouth.y + 0.5f - 0.375f;
				}
			}

			// Token: 0x17000136 RID: 310
			// (get) Token: 0x060009C9 RID: 2505 RVA: 0x0007A310 File Offset: 0x00078510
			public float Bottom
			{
				get
				{
					return this.Mouth.y - 5f;
				}
			}

			// Token: 0x040009D2 RID: 2514
			public Vector2Int Top;

			// Token: 0x040009D3 RID: 2515
			public GameObject go;

			// Token: 0x040009D4 RID: 2516
			public SpriteRenderer sr;

			// Token: 0x040009D5 RID: 2517
			public SpriteRenderer bulge;

			// Token: 0x040009D6 RID: 2518
			public float sated;

			// Token: 0x040009D7 RID: 2519
			public float frame;

			// Token: 0x040009D8 RID: 2520
			public float lastHit = -99f;

			// Token: 0x040009D9 RID: 2521
			public int hits;
		}

		// Token: 0x020001CE RID: 462
		private enum Phase
		{
			// Token: 0x040009DB RID: 2523
			None,
			// Token: 0x040009DC RID: 2524
			Sinking,
			// Token: 0x040009DD RID: 2525
			Under,
			// Token: 0x040009DE RID: 2526
			Spitting
		}
	}
}
