using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x02000139 RID: 313
	internal static class Worm
	{
		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000785 RID: 1925 RVA: 0x00073A84 File Offset: 0x00071C84
		// (set) Token: 0x06000786 RID: 1926 RVA: 0x00073A8B File Offset: 0x00071C8B
		public static WormBeast Holder { get; private set; }

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000787 RID: 1927 RVA: 0x00073A93 File Offset: 0x00071C93
		public static bool Inside
		{
			get
			{
				return Worm.Holder;
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000788 RID: 1928 RVA: 0x00073A9F File Offset: 0x00071C9F
		// (set) Token: 0x06000789 RID: 1929 RVA: 0x00073AA6 File Offset: 0x00071CA6
		public static bool Tentacled { get; private set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600078A RID: 1930 RVA: 0x00073AAE File Offset: 0x00071CAE
		public static bool Hunting
		{
			get
			{
				return Worm.Beasts.Any<WormBeast>((WormBeast w) => w && (w.mode == WormBeast.Mode.Hunt || w.mode == WormBeast.Mode.Lunge));
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600078B RID: 1931 RVA: 0x00073AD9 File Offset: 0x00071CD9
		public static float InnerHealth
		{
			get
			{
				return Worm.innerHealth;
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x0600078C RID: 1932 RVA: 0x00073AE0 File Offset: 0x00071CE0
		public static bool Sealed
		{
			get
			{
				return Worm.Inside && Worm.sealedIn;
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x0600078D RID: 1933 RVA: 0x00073AF0 File Offset: 0x00071CF0
		public static bool Sex
		{
			get
			{
				return Worm.Inside && Worm.Tentacled;
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x0600078E RID: 1934 RVA: 0x00073B00 File Offset: 0x00071D00
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x0600078F RID: 1935 RVA: 0x00073B07 File Offset: 0x00071D07
		public static bool CanHunt(Body b)
		{
			return !Scenes.Busy;
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x00073B14 File Offset: 0x00071D14
		public static void SeedLayer(Body b)
		{
			Worm.HeardThisLayer = false;
			if (!Biome.Desert || Random.value >= Plugin.WormChance.Value)
			{
				Plugin.Log.LogInfo(string.Format("Layer {0}: no giant worm{1}", WorldGeneration.world.biomeDepth, Biome.Desert ? "" : " (not the desert)"));
				return;
			}
			WorldGeneration world = WorldGeneration.world;
			for (int i = 0; i < 100; i++)
			{
				Vector2 vector;
				vector..ctor(Random.Range(-world.halfWidth + 10f, world.halfWidth - 10f), Random.Range(-world.halfHeight + 10f, world.halfHeight - 10f));
				if (Vector2.Distance(vector, b.transform.position) >= 50f)
				{
					Worm.Spawn(vector);
					Plugin.Log.LogInfo(string.Format("Layer {0}: a giant worm roams at {1}", world.biomeDepth, vector));
					return;
				}
			}
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x00073C24 File Offset: 0x00071E24
		public static void ClearAll(Body b)
		{
			if (Worm.Inside && b)
			{
				Worm.Leave(b);
			}
			foreach (WormBeast wormBeast in Worm.Beasts.ToArray())
			{
				if (wormBeast)
				{
					Object.Destroy(wormBeast.gameObject);
				}
			}
			Worm.Beasts.Clear();
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x00073C80 File Offset: 0x00071E80
		public static WormBeast Spawn(Vector2 at)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				WormArt.Ensure(spriteRenderer.sprite.pixelsPerUnit);
				TentacleArt.Ensure(spriteRenderer.sprite.pixelsPerUnit);
			}
			if (WormArt.Head.Length == 0 || !WormArt.Seg)
			{
				return null;
			}
			WormBeast wormBeast = new GameObject("LewdGiantWorm")
			{
				transform = 
				{
					position = at
				}
			}.AddComponent<WormBeast>();
			wormBeast.Build(at, spriteRenderer ? spriteRenderer.sortingLayerID : 0, spriteRenderer ? spriteRenderer.sortingOrder : 0);
			Worm.Beasts.Add(wormBeast);
			return wormBeast;
		}

		// Token: 0x06000793 RID: 1939 RVA: 0x00073D64 File Offset: 0x00071F64
		public static void DebugSpawn(Body b)
		{
			float num = 30f + ((WormArt.Head.Length != 0) ? (WormArt.Head[0].bounds.size.x * 2f * 0.5f) : 0f);
			Vector2 vector = b.transform.position + new Vector2(b.isRight ? num : (-num), -6f);
			WormBeast wormBeast = Worm.Spawn(vector);
			if (!wormBeast)
			{
				return;
			}
			wormBeast.dir = (b.transform.position - vector).normalized;
			wormBeast.StartHunt(b.transform.position, "is sent at her (debug)");
			Plugin.Log.LogInfo(string.Format("Debug: a giant worm hunts her (head {0:0.0} long, her height ~{1:0.0}, maw {2:0.0} away)", WormArt.Head[0].bounds.size.x * 2f, (from r in b.GetComponentsInChildren<SpriteRenderer>()
				select r.bounds.max.y).Max() - (from r in b.GetComponentsInChildren<SpriteRenderer>()
				select r.bounds.min.y).Min(), Vector2.Distance(wormBeast.Maw, b.transform.position)));
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x00073EE8 File Offset: 0x000720E8
		public static void OnKilled(WormBeast w)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && Worm.Holder == w)
			{
				Worm.Leave(body);
			}
			Plugin.Log.LogInfo("The giant worm was killed");
		}

		// Token: 0x06000795 RID: 1941 RVA: 0x00073F3C File Offset: 0x0007213C
		public static void Swallow(Body b, WormBeast w)
		{
			Worm.Holder = w;
			Worm.holdPoint = b.limbs[1].transform.position;
			w.mode = WormBeast.Mode.Holding;
			w.Hidden = true;
			Worm.timer = 0f;
			Worm.forced = 0;
			Worm.climaxes = 0;
			Worm.innerHealth = Plugin.WormInnerHealth.Value;
			Worm.attemptCooldown = 1f;
			Worm.pulseTimer = 2f;
			Worm.lineTimer = 2.5f;
			Worm.climaxTimer = Random.Range(14f, 20f);
			Worm.squeezeTimer = 3f;
			Worm.close = 0f;
			Worm.sealedIn = (Worm.closingSaid = false);
			Worm.Tentacled = Worm.DebugForceTentacles ?? (Random.value < Mathf.Clamp01(Plugin.WormTentacleChance.Value + Worm.S.corruption * 0.003f));
			Worm.DebugForceTentacles = null;
			Relief.ForceStop();
			b.Ragdoll();
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			b.eyePanicTime = 1.5f;
			b.adrenaline += 50f;
			Sound.Play("megabite", b.transform.position, false, true, b.transform, 1f, 0.6f, false, false);
			Sound.Play("eatFlesh", b.transform.position, false, true, b.transform, 1f, 0.7f, false, false);
			Worm.BuildInside(b);
			Worm.wetLoop = Sfx.Loop(b, "loop_wet");
			if (Worm.wetLoop)
			{
				Worm.wetLoop.volume = 0f;
				Worm.wetLoop.pitch = 0.6f;
			}
			Relief.Say(b, "worm_swallowed");
			Plugin.Log.LogInfo(string.Format("Swallowed whole by the giant worm: {0} (maw {1:0.0} from her)", Worm.Tentacled ? "tentacles inside" : "digestion", Vector2.Distance(w.Maw, b.transform.position)));
		}

		// Token: 0x06000796 RID: 1942 RVA: 0x00074174 File Offset: 0x00072374
		private static void LiftBody(Body b, int layer)
		{
			Worm.lifted.Clear();
			foreach (SpriteRenderer spriteRenderer in b.GetComponentsInChildren<SpriteRenderer>(true))
			{
				Worm.lifted.Add(new ValueTuple<SpriteRenderer, int, int>(spriteRenderer, spriteRenderer.sortingLayerID, spriteRenderer.sortingOrder));
				spriteRenderer.sortingLayerID = layer;
				spriteRenderer.sortingOrder += 20000;
			}
		}

		// Token: 0x06000797 RID: 1943 RVA: 0x000741DC File Offset: 0x000723DC
		private static void LowerBody()
		{
			foreach (ValueTuple<SpriteRenderer, int, int> valueTuple in Worm.lifted)
			{
				SpriteRenderer item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				int item3 = valueTuple.Item3;
				if (item)
				{
					item.sortingLayerID = item2;
					item.sortingOrder = item3;
				}
			}
			Worm.lifted.Clear();
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x0007425C File Offset: 0x0007245C
		private static void BuildInside(Body b)
		{
			Worm.<>c__DisplayClass56_0 CS$<>8__locals1;
			CS$<>8__locals1.layer = ((SortingLayer.layers.Length != 0) ? SortingLayer.layers[SortingLayer.layers.Length - 1].id : 0);
			Worm.LiftBody(b, CS$<>8__locals1.layer);
			SpriteRenderer[] componentsInChildren = b.GetComponentsInChildren<SpriteRenderer>(true);
			int num = componentsInChildren.Min<SpriteRenderer>((SpriteRenderer r) => r.sortingOrder);
			int num2 = componentsInChildren.Max<SpriteRenderer>((SpriteRenderer r) => r.sortingOrder);
			CS$<>8__locals1.mat = (WorldGeneration.world ? WorldGeneration.world.defaultMat : null);
			Worm.gut = Worm.<BuildInside>g__Make|56_2("LewdWormGut", WormArt.Gut, num - 30, ref CS$<>8__locals1);
			Worm.wallL = Worm.<BuildInside>g__Make|56_2("LewdWormWall", WormArt.Wall, num - 5, ref CS$<>8__locals1);
			Worm.wallR = Worm.<BuildInside>g__Make|56_2("LewdWormWall", WormArt.Wall, num - 5, ref CS$<>8__locals1);
			Worm.wallR.flipX = true;
			GameObject gameObject = new GameObject("LewdWormGlow");
			gameObject.transform.SetParent(b.limbs[1].transform, false);
			Worm.glow = gameObject.AddComponent<Light2D>();
			Worm.glow.lightType = 3;
			Worm.glow.color = new Color32(byte.MaxValue, 120, 130, byte.MaxValue);
			Worm.glow.intensity = 0.9f;
			Worm.glow.pointLightOuterRadius = 30f;
			if (Worm.Tentacled && TentacleArt.Seg)
			{
				for (int i = 0; i < Worm.Holds.Length; i++)
				{
					SpriteRenderer[] array = new SpriteRenderer[7];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = Worm.<BuildInside>g__Make|56_2("LewdWormTentacle", TentacleArt.Seg, num2 + 4, ref CS$<>8__locals1);
					}
					Worm.chains.Add(array);
					Worm.tips.Add(Worm.<BuildInside>g__Make|56_2("LewdWormTentacleTip", TentacleArt.Tip, num2 + 5, ref CS$<>8__locals1));
				}
			}
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x00074468 File Offset: 0x00072668
		private static void ClearInside()
		{
			Worm.LowerBody();
			foreach (SpriteRenderer spriteRenderer in new SpriteRenderer[]
			{
				Worm.gut,
				Worm.wallL,
				Worm.wallR
			})
			{
				if (spriteRenderer)
				{
					Object.Destroy(spriteRenderer.gameObject);
				}
			}
			if (Worm.glow)
			{
				Object.Destroy(Worm.glow.gameObject);
			}
			foreach (SpriteRenderer[] array in Worm.chains)
			{
				foreach (SpriteRenderer spriteRenderer2 in array)
				{
					if (spriteRenderer2)
					{
						Object.Destroy(spriteRenderer2.gameObject);
					}
				}
			}
			foreach (SpriteRenderer spriteRenderer3 in Worm.tips)
			{
				if (spriteRenderer3)
				{
					Object.Destroy(spriteRenderer3.gameObject);
				}
			}
			Worm.chains.Clear();
			Worm.tips.Clear();
			Worm.gut = (Worm.wallL = (Worm.wallR = null));
			Worm.glow = null;
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x000745BC File Offset: 0x000727BC
		public static void Update(Body b, float dt)
		{
			Worm.Beasts.RemoveAll((WormBeast w) => !w);
			if (Worm.attemptCooldown > 0f)
			{
				Worm.attemptCooldown -= dt;
			}
			if (Worm.stabCooldown > 0f)
			{
				Worm.stabCooldown -= dt;
			}
			if (!Worm.Inside)
			{
				return;
			}
			if (!Worm.Holder || !b.alive)
			{
				Worm.Leave(b);
				return;
			}
			Worm.timer += dt;
			b.shock = Mathf.Max(b.shock, 25f);
			Worm.close = Mathf.Clamp01(Worm.timer * Diff.Lethal / Mathf.Max(5f, Plugin.WormCloseSeconds.Value));
			if (!Worm.closingSaid && Worm.close >= 0.5f)
			{
				Worm.closingSaid = true;
				Relief.Say(b, "worm_closing");
				Sound.Play("goo", b.transform.position, false, true, b.transform, 0.9f, 0.35f, false, false);
			}
			if (!Worm.sealedIn && Worm.close >= 1f)
			{
				Worm.sealedIn = true;
				Relief.Say(b, "worm_sealed");
				Sound.Play("eatFlesh", b.transform.position, false, true, b.transform, 1f, 0.45f, false, false);
				b.limbs[1].pain = Mathf.Min(100f, b.limbs[1].pain + 12f);
				Plugin.Log.LogInfo(string.Format("The worm's walls have sealed around her after {0:0} s: no way out now", Worm.timer));
			}
			if (Keys.Down(Plugin.RemoveKey.Value))
			{
				Worm.Struggle(b);
			}
			if (Keys.Down(Plugin.WormDropKey.Value) && Worm.DropBomb(b))
			{
				return;
			}
			float num = Worm.DrawInside(b);
			Worm.Squeeze(b, num, dt);
			if (Worm.Tentacled)
			{
				Worm.UpdateScene(b, dt);
				return;
			}
			Worm.UpdateDigestion(b, dt);
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x000747DC File Offset: 0x000729DC
		public static string InsideState(Body b)
		{
			if (!Worm.Inside || !Worm.wallL)
			{
				return "not inside";
			}
			Worm.<>c__DisplayClass59_0 CS$<>8__locals1;
			CS$<>8__locals1.c = b.limbs[1].transform.position;
			return string.Format("close {0:0.00}, torso order {1}; L {2}; R {3}; gut {4}", new object[]
			{
				Worm.close,
				b.limbs[1].GetComponent<SpriteRenderer>().sortingOrder,
				Worm.<InsideState>g__R|59_0(Worm.wallL, ref CS$<>8__locals1),
				Worm.<InsideState>g__R|59_0(Worm.wallR, ref CS$<>8__locals1),
				Worm.<InsideState>g__R|59_0(Worm.gut, ref CS$<>8__locals1)
			});
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x00074884 File Offset: 0x00072A84
		private static float DrawInside(Body b)
		{
			Camera main = Camera.main;
			Vector2 vector = b.limbs[1].transform.position;
			float num = (main ? (main.orthographicSize * 2f) : 20f);
			float num2 = (main ? (num * main.aspect) : 36f);
			Vector2 vector2 = (main ? main.transform.position : vector);
			float num3 = 0.5f + 0.5f * Mathf.Sin(Time.time * 1.1f);
			float num4 = Mathf.Lerp(1.4f, 0.75f, num3);
			float num5 = Mathf.Lerp(0.5f, 0.4f, num3);
			float num6 = Mathf.Lerp(num4, num5, Mathf.SmoothStep(0f, 1f, Worm.close));
			if (Worm.gut && Worm.gut.sprite)
			{
				Vector2 vector3 = Worm.gut.sprite.bounds.size;
				float num7 = Mathf.Max(num2 / vector3.x, num / vector3.y) * 1.15f;
				Worm.gut.transform.position = vector2;
				Worm.gut.transform.localScale = new Vector3(num7, num7, 1f) * (1f + 0.02f * num3);
			}
			if (Worm.wallL && Worm.wallL.sprite)
			{
				float num8 = num * 1.7f / Worm.wallL.sprite.bounds.size.y;
				float num9 = Mathf.Sin(Time.time * 2.3f) * 0.05f;
				Worm.wallL.transform.localScale = new Vector3(num8 * 0.6f, num8, 1f);
				Worm.wallR.transform.localScale = new Vector3(num8 * 0.6f, num8, 1f);
				float num10 = 0f;
				float num11 = vector2.y;
				int[] wallInset = WormArt.WallInset;
				if (wallInset.Length != 0)
				{
					float pixelsPerUnit = Worm.wallL.sprite.pixelsPerUnit;
					float num12 = num8 / pixelsPerUnit;
					float num13 = num8 * 0.6f / pixelsPerUnit;
					num11 = vector.y - ((float)WormArt.WallWaist + 0.5f - (float)wallInset.Length * 0.5f) * num12;
					num10 = (float)wallInset[WormArt.WallWaist] * num13;
				}
				Worm.wallL.transform.position = new Vector2(vector.x - num6 + num10 + num9, num11);
				Worm.wallR.transform.position = new Vector2(vector.x + num6 - num10 - num9, num11);
			}
			return num6;
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x00074B78 File Offset: 0x00072D78
		private static void Squeeze(Body b, float gap, float dt)
		{
			Vector2 vector = b.limbs[1].transform.position;
			foreach (Limb limb in b.limbs)
			{
				if (limb && !limb.dismembered && limb.rb)
				{
					Vector2 vector2 = limb.transform.position;
					float num = gap * Mathf.Lerp(0.7f, 0.55f, Worm.close);
					float num2 = vector2.x - vector.x;
					if (Mathf.Abs(num2) > num)
					{
						limb.rb.velocity = new Vector2(Mathf.Lerp(limb.rb.velocity.x, -Mathf.Sign(num2) * Mathf.Lerp(3f, 7f, Worm.close), dt * 10f), limb.rb.velocity.y);
					}
				}
			}
			Vector2 vector3 = Worm.holdPoint;
			Limb limb2 = b.limbs[1];
			if (limb2 && limb2.rb)
			{
				limb2.rb.velocity = Vector2.Lerp(limb2.rb.velocity, (vector3 - limb2.transform.position) * 3f, dt * 6f);
			}
			if ((Worm.squeezeTimer -= dt) <= 0f)
			{
				Worm.squeezeTimer = Random.Range(5f, 9f) * (1f - 0.6f * Worm.close);
				b.limbs[1].pain = Mathf.Min(100f, b.limbs[1].pain + 3f * (1f + 2f * Worm.close));
				Sound.Play("goo", b.transform.position, false, true, b.transform, 0.6f + 0.3f * Worm.close, 0.4f, false, false);
			}
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x00074DA4 File Offset: 0x00072FA4
		private static void UpdateDigestion(Body b, float dt)
		{
			if (Worm.wetLoop)
			{
				Worm.wetLoop.volume = Mathf.MoveTowards(Worm.wetLoop.volume, 0.35f, dt);
			}
			float num = Plugin.WormAcid.Value * (Worm.sealedIn ? 1.5f : 1f);
			foreach (Limb limb in b.limbs)
			{
				if (!limb.dismembered)
				{
					float armorReduction = limb.GetArmorReduction();
					limb.skinHealth -= 1.1f * num * Diff.Lethal / armorReduction * dt;
					limb.muscleHealth -= 0.35f * num * Diff.Lethal / armorReduction * dt;
					limb.pain = Mathf.Min(100f, limb.pain + 1.4f * num / armorReduction * dt);
					limb.DamageWearables(0.15f * num * dt);
				}
			}
			b.bloodOxygen = Mathf.Max(0f, b.bloodOxygen - Plugin.WormAirLoss.Value * Diff.Lethal * (Worm.sealedIn ? 1.5f : 1f) * dt);
			b.wetness = Mathf.Min(100f, b.wetness + 4f * dt);
			if ((Worm.pulseTimer -= dt) <= 0f)
			{
				Worm.pulseTimer = Random.Range(4f, 7f);
				Sound.Play("gore", b.transform.position, false, true, b.transform, 0.45f, 0.6f, false, false);
				Sfx.Play(b, 0.7f, new string[] { "muffled" });
			}
			if ((Worm.lineTimer -= dt) <= 0f)
			{
				Worm.lineTimer = Random.Range(6f, 10f);
				Relief.Say(b, "worm_digest");
			}
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x00074F98 File Offset: 0x00073198
		private static void UpdateScene(Body b, float dt)
		{
			float num = Mathf.Clamp01(Worm.timer / 40f);
			if (Worm.wetLoop)
			{
				Worm.wetLoop.volume = Mathf.MoveTowards(Worm.wetLoop.volume, 0.55f, dt);
			}
			b.bloodOxygen = Mathf.Max(0f, b.bloodOxygen - (Worm.sealedIn ? (0.75f * Diff.Lethal) : 0.15f) * dt);
			Worm.DrawTentacles(b, num);
			Worm.S.arousal = Mathf.Min(100f, Worm.S.arousal + (2.4f + 2.5f * num) * dt);
			Worm.S.lust = Mathf.Min(100f, Worm.S.lust + 0.4f * dt);
			Worm.S.submission = Mathf.Min(100f, Worm.S.submission + 0.06f * dt);
			Worm.S.exhaustion = Mathf.Min(100f, Worm.S.exhaustion + 0.2f * dt);
			b.stamina = Mathf.Max(0f, b.stamina - 0.6f * dt);
			if ((Worm.pulseTimer -= dt) <= 0f)
			{
				Worm.pulseTimer = Mathf.Lerp(2.5f, 1.2f, num);
				Sound.Play("goo", b.transform.position, false, true, b.transform, 0.4f + 0.3f * num, Random.Range(0.7f, 1f), false, false);
				Sfx.Play(b, 0.6f + 0.3f * num, new string[] { "muffled", "moan" });
				Limb limb = b.LimbByName("DownTorso");
				if (limb && limb.rb)
				{
					limb.rb.velocity += Vector2.up * 2.5f;
				}
			}
			if (Worm.S.arousal >= 99f && Worm.S.orgasmTimer < 13f && b.conscious)
			{
				Plugin.Orgasm(b, false);
				Worm.forced++;
				Worm.S.exhaustion = Mathf.Min(100f, Worm.S.exhaustion + Plugin.ExhaustionPerOrgasm.Value);
				Worm.S.arousal = Mathf.Max(Worm.S.arousal, 55f);
				Relief.Say(b, (Worm.S.exhaustion > 70f) ? "tent_toomuch" : "worm_orgasm");
				Plugin.Log.LogInfo(string.Format("Worm tentacles forced orgasm {0}; exhaustion {1:0}", Worm.forced, Worm.S.exhaustion));
			}
			if ((Worm.climaxTimer -= dt) <= 0f)
			{
				Worm.climaxTimer = Random.Range(15f, 22f);
				Worm.climaxes++;
				Womb.Fill(b, 20f);
				Semen.Add(b, 10f);
				Sfx.Play(b, 0.6f, new string[] { "drip" });
				bool flag = Womb.TryImpregnate(b, 1f);
				Relief.Say(b, "tent_climax");
				Plugin.Log.LogInfo(string.Format("Worm tentacles climax {0} in her (filled {1:0}{2})", Worm.climaxes, Worm.S.filled, flag ? ", seeded" : ""));
			}
			if ((Worm.lineTimer -= dt) <= 0f)
			{
				Worm.lineTimer = Random.Range(7f, 12f);
				Relief.Say(b, (Random.value < 0.35f) ? "tent_hole" : "worm_scene");
			}
			if (Worm.S.exhaustion >= 100f)
			{
				Heart.Overworked(b, dt);
			}
			if (Worm.timer > Plugin.WormSceneSeconds.Value && !Worm.sealedIn)
			{
				Relief.Say(b, "worm_spat_used");
				Plugin.Log.LogInfo(string.Format("The worm's tentacles are done with her after {0:0} s ({1} orgasms): it spits her out", Worm.timer, Worm.forced));
				Worm.SpitOut(b, false);
			}
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x000753FC File Offset: 0x000735FC
		private static void DrawTentacles(Body b, float intensity)
		{
			if (!TentacleArt.Seg || Worm.chains.Count == 0 || !Worm.wallL)
			{
				return;
			}
			float x = TentacleArt.Seg.bounds.size.x;
			for (int i = 0; i < Worm.chains.Count; i++)
			{
				Limb limb = b.LimbByName(Worm.Holds[i]);
				bool flag = limb && !limb.dismembered && (i < 5 || intensity > 0.5f);
				bool flag2 = i % 2 == 0;
				Vector2 vector;
				vector..ctor((flag2 ? Worm.wallL : Worm.wallR).transform.position.x, b.limbs[1].transform.position.y + ((float)i - 2.5f) * 0.6f);
				Vector2 vector2 = (flag ? limb.transform.position : (vector + new Vector2(flag2 ? 0.6f : (-0.6f), -0.3f))) - vector;
				Vector2 normalized = new Vector2(-vector2.y, vector2.x).normalized;
				Vector2 vector3 = vector;
				SpriteRenderer[] array = Worm.chains[i];
				for (int j = 0; j < array.Length; j++)
				{
					float num = ((float)j + 1f) / (float)array.Length;
					Vector2 vector4 = vector + vector2 * num + normalized * Mathf.Sin(Time.time * 4f + (float)i * 1.7f + num * 5f) * 0.12f * Mathf.Sin(num * 3.1415927f);
					Vector2 vector5 = vector4 - vector3;
					array[j].transform.position = vector3;
					array[j].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector5.y, vector5.x) * 57.29578f);
					array[j].transform.localScale = new Vector3(vector5.magnitude / x + 0.12f, Mathf.Lerp(1.4f, 0.8f, num), 1f);
					vector3 = vector4;
				}
				if (i < Worm.tips.Count)
				{
					Worm.tips[i].transform.position = vector3;
					Worm.tips[i].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector2.y, vector2.x) * 57.29578f);
				}
			}
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x000756D0 File Offset: 0x000738D0
		private static void Struggle(Body b)
		{
			if (Worm.attemptCooldown > 0f || !b.conscious)
			{
				return;
			}
			Worm.attemptCooldown = 1.2f;
			b.stamina = Mathf.Max(0f, b.stamina - 10f);
			if (Worm.sealedIn)
			{
				Relief.Say(b, "worm_sealed_fail");
				Sound.Play("goo", b.transform.position, false, true, b.transform, 0.7f, 0.4f, false, false);
				Plugin.Log.LogInfo("Struggling inside the worm: sealed in, no way out");
				return;
			}
			float num = Mathf.Clamp(b.stamina / 100f, 0.1f, 1f) * (1f - Worm.S.exhaustion / 100f);
			float num2 = Plugin.WormEscapeChance.Value * Diff.Escape * num * (1f - 0.6f * Worm.close);
			bool flag = Random.value < num2;
			Plugin.Log.LogInfo(string.Format("Struggling inside the worm: chance {0:P1}, {1}", num2, flag ? "squirmed out" : "held"));
			if (flag)
			{
				Relief.Say(b, "worm_escape");
				Worm.SpitOut(b, false);
				return;
			}
			Relief.Say(b, "worm_fail");
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.7f, 0.5f, false, false);
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x00075844 File Offset: 0x00073A44
		public static bool Stab(Body b, AttackInfo atk)
		{
			if (Worm.stabCooldown > 0f || !b.conscious)
			{
				return false;
			}
			Worm.stabCooldown = Mathf.Max(0.45f, atk.cooldown);
			if (Worm.sealedIn)
			{
				b.stamina = Mathf.Max(0f, b.stamina - 2f);
				Sound.Play("goo", b.transform.position, false, true, b.transform, 0.6f, 0.5f, false, false);
				if (Random.value < 0.3f)
				{
					Relief.Say(b, "worm_sealed_stab");
				}
				return true;
			}
			float num = Mathf.Max(5f, atk.damage);
			b.stamina = Mathf.Max(0f, b.stamina - Mathf.Max(2f, atk.staminaUse * 3f));
			Worm.innerHealth -= num;
			Sound.Play("gore", b.transform.position, false, true, b.transform, 0.9f, 0.8f, false, false);
			if (atk.swingSounds != null && atk.swingSounds.Length != 0)
			{
				Sound.Play(atk.swingSounds[Random.Range(0, atk.swingSounds.Length)], b.transform.position, false, true, b.transform, 0.5f, 1f, false, false);
			}
			b.limbs[1].pain = Mathf.Min(100f, b.limbs[1].pain + 4f);
			Plugin.Log.LogInfo(string.Format("She wounds the worm from inside: {0:0} damage, {1:0} left", num, Mathf.Max(0f, Worm.innerHealth)));
			if (Worm.innerHealth <= 0f)
			{
				Relief.Say(b, "worm_spat_wound");
				Plugin.Log.LogInfo("The wounded worm spits her out");
				Worm.SpitOut(b, true);
			}
			else if (Random.value < 0.4f)
			{
				Relief.Say(b, "worm_stab");
			}
			return true;
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x00075A4F File Offset: 0x00073C4F
		public static void LetGo(Body b)
		{
			if (Worm.Inside)
			{
				Worm.SpitOut(b, false);
			}
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x00075A60 File Offset: 0x00073C60
		private static bool DropBomb(Body b)
		{
			Item item = null;
			for (int i = 0; i < b.slots.Length; i++)
			{
				if (b.slots[i].isHand && b.HoldingItem(i))
				{
					Item item2 = b.GetItem(i);
					if (item2 != null && item2 && item2.id == "dynamite")
					{
						item = item2;
						break;
					}
				}
			}
			if (!item)
			{
				Relief.Say(b, "worm_drop_none");
				return false;
			}
			WormBeast holder = Worm.Holder;
			CustomItemBehaviour component = item.GetComponent<CustomItemBehaviour>();
			if (component)
			{
				component.CancelInvoke("DynamiteExplode");
			}
			Object.Destroy(item.gameObject);
			Relief.Say(b, "worm_drop_bomb");
			Plugin.Log.LogInfo("She let a stick of dynamite fall deeper into the giant worm");
			Worm.SpitOut(b, true);
			if (holder)
			{
				holder.BombInside(1.5f);
			}
			return true;
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x00075B40 File Offset: 0x00073D40
		public static void Blast(Vector2 at, float range)
		{
			Func<Vector2, float> <>9__0;
			Func<Vector2, float> <>9__1;
			foreach (WormBeast wormBeast in Worm.Beasts.ToArray())
			{
				if (wormBeast && !wormBeast.leaving)
				{
					if (Vector2.Distance(wormBeast.Maw, at) >= Mathf.Max(2.5f, range * 0.25f))
					{
						if (Worm.Holder == wormBeast && Worm.Inside)
						{
							IEnumerable<Vector2> joints = wormBeast.joints;
							Func<Vector2, float> func;
							if ((func = <>9__0) == null)
							{
								func = (<>9__0 = (Vector2 j) => Vector2.Distance(j, at));
							}
							if (joints.Min<Vector2>(func) < Mathf.Max(8f, range))
							{
								goto IL_00AA;
							}
						}
						IEnumerable<Vector2> joints2 = wormBeast.joints;
						Func<Vector2, float> func2;
						if ((func2 = <>9__1) == null)
						{
							func2 = (<>9__1 = (Vector2 j) => Vector2.Distance(j, at));
						}
						float num = joints2.Min<Vector2>(func2);
						float num2 = Mathf.Max(3f, range * 0.6f);
						if (num < num2)
						{
							wormBeast.TakeHit(Plugin.WormBlastDamage.Value * (1f - num / num2), "a blast");
							goto IL_0129;
						}
						goto IL_0129;
					}
					IL_00AA:
					Plugin.Log.LogInfo("A blast in the giant worm's maw");
					wormBeast.Explode();
				}
				IL_0129:;
			}
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x00075C84 File Offset: 0x00073E84
		private static void SpitOut(Body b, bool wounded)
		{
			WormBeast holder = Worm.Holder;
			Worm.Leave(b);
			if (!holder)
			{
				return;
			}
			Vector2 vector = ((holder.dir.sqrMagnitude > 0.01f) ? holder.dir : Vector2.right);
			foreach (Limb limb in b.limbs)
			{
				if (limb.rb)
				{
					limb.rb.velocity = vector * 7f + Vector2.up * 4f;
				}
			}
			b.wetness = 100f;
			Semen.Add(b, Worm.Tentacled ? 40f : 15f);
			Sound.Play("vomit", b.transform.position, false, true, b.transform, 1f, 0.6f, false, false);
			Sound.Play("growl", holder.HeadPos, false, true, null, 1f, 0.4f, false, false);
			holder.dir = -vector;
			if (holder.Enraged)
			{
				holder.Flee(2f);
				holder.Roar(1f);
			}
			else
			{
				holder.Retreat(wounded ? 22f : 12f, wounded ? 240f : 150f, b);
			}
			if (wounded && !holder.Enraged)
			{
				holder.Roar(1f);
			}
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x00075DF4 File Offset: 0x00073FF4
		private static void Leave(Body b)
		{
			WormBeast holder = Worm.Holder;
			Worm.Holder = null;
			Worm.ClearInside();
			if (holder)
			{
				holder.Hidden = false;
				if (holder.mode == WormBeast.Mode.Holding)
				{
					holder.Flee(6f);
				}
			}
			if (Worm.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(Worm.wetLoop, 0.8f));
			}
			Worm.wetLoop = null;
			if (b)
			{
				Traverse.Create(b).Field("movingAllowed").SetValue(true);
			}
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x00075EFC File Offset: 0x000740FC
		[CompilerGenerated]
		internal static SpriteRenderer <BuildInside>g__Make|56_2(string name, Sprite s, int order, ref Worm.<>c__DisplayClass56_0 A_3)
		{
			SpriteRenderer spriteRenderer = new GameObject(name).AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = s;
			if (A_3.mat)
			{
				spriteRenderer.sharedMaterial = A_3.mat;
			}
			spriteRenderer.sortingLayerID = A_3.layer;
			spriteRenderer.sortingOrder = order;
			return spriteRenderer;
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x00075F4C File Offset: 0x0007414C
		[CompilerGenerated]
		internal static string <InsideState>g__R|59_0(SpriteRenderer r, ref Worm.<>c__DisplayClass59_0 A_1)
		{
			if (!r)
			{
				return "-";
			}
			return string.Format("{0} x {1:0.00} (bounds {2:0.00}..{3:0.00}) order {4} layer {5} on {6}", new object[]
			{
				r.name,
				r.transform.position.x - A_1.c.x,
				r.bounds.min.x - A_1.c.x,
				r.bounds.max.x - A_1.c.x,
				r.sortingOrder,
				r.sortingLayerID,
				r.enabled
			});
		}

		// Token: 0x040007BC RID: 1980
		public static readonly List<WormBeast> Beasts = new List<WormBeast>();

		// Token: 0x040007BE RID: 1982
		public static bool HeardThisLayer;

		// Token: 0x040007C0 RID: 1984
		private static Vector2 holdPoint;

		// Token: 0x040007C1 RID: 1985
		private static float timer;

		// Token: 0x040007C2 RID: 1986
		private static float innerHealth;

		// Token: 0x040007C3 RID: 1987
		private static float attemptCooldown;

		// Token: 0x040007C4 RID: 1988
		private static float stabCooldown;

		// Token: 0x040007C5 RID: 1989
		private static float pulseTimer;

		// Token: 0x040007C6 RID: 1990
		private static float lineTimer;

		// Token: 0x040007C7 RID: 1991
		private static float climaxTimer;

		// Token: 0x040007C8 RID: 1992
		private static float squeezeTimer;

		// Token: 0x040007C9 RID: 1993
		private static float close;

		// Token: 0x040007CA RID: 1994
		private static bool sealedIn;

		// Token: 0x040007CB RID: 1995
		private static bool closingSaid;

		// Token: 0x040007CC RID: 1996
		private static int forced;

		// Token: 0x040007CD RID: 1997
		private static int climaxes;

		// Token: 0x040007CE RID: 1998
		private static SpriteRenderer gut;

		// Token: 0x040007CF RID: 1999
		private static SpriteRenderer wallL;

		// Token: 0x040007D0 RID: 2000
		private static SpriteRenderer wallR;

		// Token: 0x040007D1 RID: 2001
		private static Light2D glow;

		// Token: 0x040007D2 RID: 2002
		private static AudioSource wetLoop;

		// Token: 0x040007D3 RID: 2003
		private static readonly List<SpriteRenderer[]> chains = new List<SpriteRenderer[]>();

		// Token: 0x040007D4 RID: 2004
		private static readonly List<SpriteRenderer> tips = new List<SpriteRenderer>();

		// Token: 0x040007D5 RID: 2005
		private static readonly string[] Holds = new string[] { "HandF", "HandB", "FootF", "FootB", "DownTorso", "Head" };

		// Token: 0x040007D6 RID: 2006
		public static bool? DebugForceTentacles;

		// Token: 0x040007D7 RID: 2007
		[TupleElementNames(new string[] { "r", "layer", "order" })]
		private static readonly List<ValueTuple<SpriteRenderer, int, int>> lifted = new List<ValueTuple<SpriteRenderer, int, int>>();

		// Token: 0x040007D8 RID: 2008
		private const int Lift = 20000;
	}
}
