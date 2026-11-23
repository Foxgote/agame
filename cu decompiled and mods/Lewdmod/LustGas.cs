using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x020000B7 RID: 183
	internal static class LustGas
	{
		// Token: 0x06000418 RID: 1048 RVA: 0x000427EC File Offset: 0x000409EC
		public static bool Near(Vector2 p, float pad)
		{
			return LustGas.zones.Any<LustGas.Zone>((LustGas.Zone z) => Vector2.Distance(z.center, p) < z.radius + pad) || LustGas.vents.Any<LustGas.Vent>((LustGas.Vent v) => Vector2.Distance(v.pos, p) < pad);
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00042840 File Offset: 0x00040A40
		private static Material Unlit()
		{
			if (LustGas.unlit)
			{
				return LustGas.unlit;
			}
			Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") ?? Shader.Find("Sprites/Default");
			if (shader)
			{
				LustGas.unlit = new Material(shader);
			}
			return LustGas.unlit;
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600041A RID: 1050 RVA: 0x00042890 File Offset: 0x00040A90
		// (set) Token: 0x0600041B RID: 1051 RVA: 0x00042897 File Offset: 0x00040A97
		public static float Exposure { get; private set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x0004289F File Offset: 0x00040A9F
		// (set) Token: 0x0600041D RID: 1053 RVA: 0x000428A6 File Offset: 0x00040AA6
		public static bool InZone { get; private set; }

		// Token: 0x0600041E RID: 1054 RVA: 0x000428B0 File Offset: 0x00040AB0
		private static void EnsureArt()
		{
			if (LustGas.puffSprites != null)
			{
				return;
			}
			LustGas.puffSprites = new Sprite[3];
			for (int i = 0; i < 3; i++)
			{
				int num = 16;
				Texture2D texture2D = new Texture2D(num, num, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				Random rnd = new Random(11 + i);
				ValueTuple<int, int, float>[] array = (from _ in Enumerable.Range(0, 4)
					select new ValueTuple<int, int, float>(4 + rnd.Next(8), 4 + rnd.Next(8), 3.5f + (float)rnd.NextDouble() * 2.5f)).ToArray<ValueTuple<int, int, float>>();
				int[,] array2 = new int[,]
				{
					{ 0, 8, 2, 10 },
					{ 12, 4, 14, 6 },
					{ 3, 11, 1, 9 },
					{ 15, 7, 13, 5 }
				};
				int num5;
				int y;
				for (y = 0; y < num; y = num5 + 1)
				{
					int x;
					for (x = 0; x < num; x = num5 + 1)
					{
						float num2 = array.Max<ValueTuple<int, int, float>>(([TupleElementNames(new string[] { "x", "y", "r" })] ValueTuple<int, int, float> l) => 1f - Vector2.Distance(new Vector2((float)x + 0.5f, (float)y + 0.5f), new Vector2((float)l.Item1, (float)l.Item2)) / l.Item3);
						float num3 = ((float)array2[y % 4, x % 4] + 0.5f) / 16f;
						bool flag = num2 > 0f && num2 * 1.6f > num3;
						float num4 = 0.85f + 0.15f * Mathf.Clamp01(num2 * 1.5f);
						texture2D.SetPixel(x, y, flag ? new Color(num4, num4, num4, Mathf.Clamp01(0.35f + num2)) : Color.clear);
						num5 = x;
					}
					num5 = y;
				}
				texture2D.Apply(false);
				LustGas.puffSprites[i] = Sprite.Create(texture2D, new Rect(0f, 0f, (float)num, (float)num), new Vector2(0.5f, 0.5f), 16f);
			}
			int num6 = 24;
			int num7 = 10;
			Texture2D texture2D2 = new Texture2D(num6, num7, 4, false)
			{
				filterMode = 0,
				wrapMode = 1
			};
			Color color = new Color32(70, 58, 72, byte.MaxValue);
			Color color2 = new Color32(42, 34, 46, byte.MaxValue);
			Color color3 = new Color32(250, 140, 214, byte.MaxValue);
			Color color4 = new Color32(170, 70, 140, byte.MaxValue);
			for (int j = 0; j < num7; j++)
			{
				for (int k = 0; k < num6; k++)
				{
					float num8 = (float)(num7 - 1) - 4.5f * Mathf.Exp(-Mathf.Pow(((float)k - (float)num6 / 2f) / 8f, 2f)) - 3f;
					Color color5 = Color.clear;
					if ((float)j <= num8 + 4.5f && j <= num7 - 1 && j >= 0 && (float)(num7 - 1 - j) <= 4.5f * Mathf.Exp(-Mathf.Pow(((float)k - (float)num6 / 2f) / 8f, 2f)) + 2.5f)
					{
						color5 = (((k + j) % 3 == 0) ? color2 : color);
					}
					if (color5.a > 0f && Mathf.Abs((float)k - (float)num6 / 2f + 0.5f) < 1.6f && j > 2)
					{
						color5 = color3;
					}
					else if (color5.a > 0f && Mathf.Abs((float)k - (float)num6 / 2f + 0.5f) < 2.6f && j > 3)
					{
						color5 = color4;
					}
					texture2D2.SetPixel(k, j, color5);
				}
			}
			texture2D2.Apply(false);
			LustGas.ventSprite = Sprite.Create(texture2D2, new Rect(0f, 0f, (float)num6, (float)num7), new Vector2(0.5f, 0f), 16f);
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00042CC0 File Offset: 0x00040EC0
		public static void SeedLayer(Body b)
		{
			LustGas.EnsureArt();
			int num = 0;
			for (int i = 0; i < Diff.Count(Plugin.GasGeysersPerLayer.Value); i++)
			{
				Vector2? vector = Spots.RandomFloor(25f, b.transform.position);
				if (vector != null && LustGas.AddVent(vector.Value + Vector2.down * 0.5f) != null)
				{
					num++;
				}
			}
			bool flag = Random.value < Plugin.GasBiomeChance.Value;
			if (flag)
			{
				Vector2? vector2 = Spots.RandomFloor(35f, b.transform.position);
				if (vector2 != null)
				{
					LustGas.Build(vector2.Value, Plugin.GasBiomeRadius.Value);
				}
				else
				{
					flag = false;
				}
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: {1} lust gas geysers{2}", WorldGeneration.world.biomeDepth, num, flag ? ", a lust gas biome" : ""));
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00042DC4 File Offset: 0x00040FC4
		public static void Build(Vector2 center, float radius)
		{
			LustGas.EnsureArt();
			LustGas.Zone zone = new LustGas.Zone
			{
				center = center,
				radius = radius
			};
			LustGas.zones.Add(zone);
			int num = 0;
			int num2 = 0;
			while (num2 < 60 && num < Mathf.RoundToInt(Plugin.GasGeysersPerBiome.Value))
			{
				Vector2 vector = center + Random.insideUnitCircle * radius * 0.8f;
				Vector2? f = LustGas.FloorBelow(vector, 6);
				if (f != null && !LustGas.vents.Any<LustGas.Vent>((LustGas.Vent v) => Vector2.Distance(v.pos, f.Value) < 4f))
				{
					LustGas.AddVent(f.Value);
					num++;
				}
				num2++;
			}
			GameObject gameObject = new GameObject("LewdGasBiomeGlow");
			gameObject.transform.position = center;
			Light2D light2D = gameObject.AddComponent<Light2D>();
			light2D.lightType = 3;
			light2D.color = LustGas.Pink;
			light2D.intensity = 0.3f;
			light2D.pointLightOuterRadius = radius * 1.15f;
			light2D.pointLightInnerRadius = radius * 0.3f;
			LustGas.spawned.Add(gameObject);
			for (int i = 0; i < 70; i++)
			{
				LustGas.AmbientPuff(zone, true);
			}
			Plugin.Log.LogInfo(string.Format("Lust gas biome at {0}, radius {1:0}, {2} vents", center, radius, num));
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00042F2C File Offset: 0x0004112C
		private static Vector2? FloorBelow(Vector2 p, int depth)
		{
			WorldGeneration world = WorldGeneration.world;
			Vector2Int vector2Int = world.WorldToBlockPos(p);
			if (world.GetBlock(vector2Int) != 0)
			{
				return null;
			}
			for (int i = 0; i < depth; i++)
			{
				if (world.GetBlock(vector2Int + Vector2Int.down) != 0)
				{
					return new Vector2?(world.BlockToWorldPos(vector2Int) + Vector2.down * 0.5f);
				}
				vector2Int += Vector2Int.down;
			}
			return null;
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00042FB0 File Offset: 0x000411B0
		private static LustGas.Vent AddVent(Vector2 floor)
		{
			LustGas.EnsureArt();
			GameObject gameObject = new GameObject("LewdGasVent");
			gameObject.transform.position = floor;
			gameObject.transform.localScale = Vector3.one * 2f;
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = LustGas.ventSprite;
			if (WorldGeneration.world)
			{
				spriteRenderer.sharedMaterial = WorldGeneration.world.defaultMat;
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer2 = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer2)
			{
				spriteRenderer.sortingLayerID = spriteRenderer2.sortingLayerID;
				spriteRenderer.sortingOrder = spriteRenderer2.sortingOrder - 14;
			}
			GameObject gameObject2 = new GameObject("Glow");
			gameObject2.transform.SetParent(gameObject.transform, false);
			gameObject2.transform.localPosition = Vector3.up * 0.3f;
			Light2D light2D = gameObject2.AddComponent<Light2D>();
			light2D.lightType = 3;
			light2D.color = LustGas.Pink;
			light2D.intensity = 0.5f;
			light2D.pointLightOuterRadius = 2.2f;
			LustGas.spawned.Add(gameObject);
			LustGas.Vent vent = new LustGas.Vent
			{
				go = gameObject,
				sr = spriteRenderer,
				glow = light2D,
				pos = floor,
				cooldown = Random.Range(0f, 4f)
			};
			LustGas.vents.Add(vent);
			BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
			boxCollider2D.isTrigger = false;
			boxCollider2D.size = new Vector2(LustGas.ventSprite.bounds.size.x * 0.8f, LustGas.ventSprite.bounds.size.y);
			boxCollider2D.offset = new Vector2(0f, LustGas.ventSprite.bounds.size.y * 0.5f);
			gameObject.layer = LayerMask.NameToLayer("Descriptor");
			BuildingEntity buildingEntity = gameObject.AddComponent<BuildingEntity>();
			buildingEntity.animal = true;
			buildingEntity.health = Mathf.Max(1f, Plugin.GasVentHealth.Value);
			buildingEntity.id = "lewd_gasvent";
			buildingEntity.ignoreBodyOptimize = true;
			buildingEntity.itemsDropOnDestroy = new ItemDrop[0];
			buildingEntity.alwaysDrop = new ItemDrop[0];
			buildingEntity.itemCategoriesToAdd = new string[0];
			gameObject.AddComponent<GasVentHit>().vent = vent;
			return vent;
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x0004322C File Offset: 0x0004142C
		public static void VentHit(LustGas.Vent v)
		{
			if (v == null || !v.go)
			{
				return;
			}
			Sound.Play("geyserrumble", v.pos, false, false, null, 1f, 1f, false, false);
			LustGas.MakePuff(v.pos + Vector2.up * 0.6f, new Vector2(Random.Range(-1f, 1f), Random.Range(1.5f, 3f)), 2.5f, 6f, 0.4f);
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x000432BC File Offset: 0x000414BC
		public static void BreakVent(LustGas.Vent v, string why)
		{
			if (v == null || !LustGas.vents.Contains(v))
			{
				return;
			}
			LustGas.vents.Remove(v);
			for (int i = 0; i < 8; i++)
			{
				LustGas.MakePuff(v.pos + Vector2.up * 0.6f, new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 4f)), Random.Range(2.5f, 4f), Random.Range(6f, 10f), 0.5f);
			}
			Sound.Play("geyseractivate", v.pos, false, false, null, 1f, 1f, false, false);
			if (v.go)
			{
				Object.Destroy(v.go);
			}
			Plugin.Log.LogInfo(string.Format("A lust gas geyser at {0} is broken ({1})", v.pos, why));
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x000433B0 File Offset: 0x000415B0
		public static void Blast(Vector2 at, float range)
		{
			foreach (LustGas.Vent vent in LustGas.vents.ToArray())
			{
				if (Vector2.Distance(vent.pos, at) < Mathf.Max(3f, range))
				{
					LustGas.BreakVent(vent, "a blast");
				}
			}
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00043400 File Offset: 0x00041600
		public static void Clear()
		{
			foreach (LustGas.Puff puff in LustGas.puffs)
			{
				if (puff.go)
				{
					Object.Destroy(puff.go);
				}
			}
			LustGas.puffs.Clear();
			foreach (GameObject gameObject in LustGas.spawned)
			{
				if (gameObject)
				{
					Object.Destroy(gameObject);
				}
			}
			LustGas.spawned.Clear();
			LustGas.vents.Clear();
			LustGas.zones.Clear();
			LustGas.RestoreFog();
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x000434D8 File Offset: 0x000416D8
		private static LustGas.Puff MakePuff(Vector2 at, Vector2 vel, float size, float life, float alpha)
		{
			if (LustGas.puffs.Count >= 260)
			{
				return null;
			}
			LustGas.EnsureArt();
			GameObject gameObject = new GameObject("LewdGasPuff");
			gameObject.transform.position = at;
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = LustGas.puffSprites[Random.Range(0, LustGas.puffSprites.Length)];
			spriteRenderer.color = new Color(LustGas.Pink.r, LustGas.Pink.g, LustGas.Pink.b, 0f);
			gameObject.transform.localScale = Vector3.one * size * 0.5f;
			Material material = LustGas.Unlit();
			if (material)
			{
				spriteRenderer.sharedMaterial = material;
			}
			else if (WorldGeneration.world)
			{
				spriteRenderer.sharedMaterial = WorldGeneration.world.defaultMat;
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer2 = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer2)
			{
				spriteRenderer.sortingLayerID = spriteRenderer2.sortingLayerID;
				spriteRenderer.sortingOrder = spriteRenderer2.sortingOrder + 40;
			}
			spriteRenderer.flipX = Random.value < 0.5f;
			LustGas.Puff puff = new LustGas.Puff
			{
				go = gameObject,
				sr = spriteRenderer,
				pos = at,
				vel = vel,
				size = size,
				life = life,
				alpha = alpha
			};
			LustGas.puffs.Add(puff);
			return puff;
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00043670 File Offset: 0x00041870
		private static void AmbientPuff(LustGas.Zone z, bool anyAge)
		{
			LustGas.Puff puff = LustGas.MakePuff(z.center + Random.insideUnitCircle * z.radius, Random.insideUnitCircle * 0.25f, Random.Range(4.5f, 7f), Random.Range(10f, 18f), Random.Range(0.14f, 0.24f));
			if (puff == null)
			{
				return;
			}
			puff.ambient = true;
			puff.home = z;
			if (anyAge)
			{
				puff.age = Random.Range(0f, puff.life * 0.8f);
			}
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x0004370C File Offset: 0x0004190C
		public static void Update(Body b, float dt)
		{
			Vector2 her = b.transform.position;
			Vector2 vector = (b.limbs[0] ? b.limbs[0].transform.position : her);
			WorldGeneration world = WorldGeneration.world;
			foreach (LustGas.Vent vent in LustGas.vents.ToArray())
			{
				if (!vent.go)
				{
					LustGas.vents.Remove(vent);
				}
				else if (world && world.GetBlock(world.WorldToBlockPos(vent.pos + Vector2.down * 0.5f)) == 0)
				{
					LustGas.BreakVent(vent, "the rock under it is gone");
				}
				else
				{
					if (vent.cooldown > 0f)
					{
						vent.cooldown -= dt;
					}
					float num = Vector2.Distance(vent.pos, her);
					if (vent.rumble < 0f && vent.spout < 0f && vent.cooldown <= 0f && num < Plugin.GasTriggerRange.Value)
					{
						vent.rumble = 1f;
						Sound.Play("geyserrumble", vent.pos, false, false, null, 1f, 1f, false, false);
					}
					if (vent.rumble >= 0f)
					{
						vent.rumble -= dt;
						vent.go.transform.position = vent.pos + Vector2.right * Random.Range(-0.12f, 0.12f);
						if (vent.rumble < 0f)
						{
							vent.go.transform.position = vent.pos;
							vent.spout = 4.5f;
							Sound.Play("geyseractivate", vent.pos, false, false, null, 1f, 1f, false, false);
						}
					}
					if (vent.spout >= 0f)
					{
						vent.spout -= dt;
						vent.glow.intensity = 1.2f + 0.3f * Mathf.Sin(Time.time * 20f);
						if (Random.value < dt * 14f)
						{
							LustGas.MakePuff(vent.pos + Vector2.up * 0.6f, new Vector2(Random.Range(-0.8f, 0.8f), Random.Range(3f, 5.5f)), Random.Range(2.4f, 4f), Random.Range(9f, 14f), Random.Range(0.4f, 0.6f));
						}
						if (vent.spout < 0f)
						{
							vent.cooldown = Plugin.GasGeyserCooldown.Value;
							vent.glow.intensity = 0.5f;
						}
					}
					else if (vent.rumble < 0f)
					{
						vent.glow.intensity = 0.45f + 0.1f * Mathf.Sin(Time.time * 2f + vent.pos.x);
					}
				}
			}
			float num2 = 0f;
			for (int j = LustGas.puffs.Count - 1; j >= 0; j--)
			{
				LustGas.Puff puff = LustGas.puffs[j];
				if (!puff.go)
				{
					LustGas.puffs.RemoveAt(j);
				}
				else
				{
					puff.age += dt;
					if (puff.age >= puff.life)
					{
						Object.Destroy(puff.go);
						LustGas.puffs.RemoveAt(j);
						if (puff.ambient && LustGas.zones.Contains(puff.home))
						{
							LustGas.AmbientPuff(puff.home, false);
						}
					}
					else
					{
						puff.vel *= Mathf.Pow(0.55f, dt);
						puff.vel += new Vector2(Mathf.Sin(Time.time * 0.7f + (float)j) * 0.15f, puff.ambient ? 0.02f : 0.12f) * dt;
						puff.pos += puff.vel * dt;
						if (WorldGeneration.world && WorldGeneration.world.GetBlock(WorldGeneration.world.WorldToBlockPos(puff.pos)) != 0)
						{
							puff.pos -= puff.vel * dt;
							puff.vel = -puff.vel * 0.3f;
						}
						float num3 = puff.age / puff.life;
						float num4 = puff.size * (0.5f + 1.1f * num3);
						float num5 = puff.alpha * Mathf.Clamp01(num3 * 6f) * (1f - num3);
						puff.go.transform.position = puff.pos;
						puff.go.transform.localScale = Vector3.one * num4;
						puff.sr.color = new Color(LustGas.Pink.r, LustGas.Pink.g, LustGas.Pink.b, num5);
						float num6 = Vector2.Distance(puff.pos, vector);
						if (num6 < num4 * 0.55f)
						{
							num2 += num5 * (1f - num6 / (num4 * 0.55f)) * 1.6f;
						}
					}
				}
			}
			LustGas.InZone = LustGas.zones.FirstOrDefault<LustGas.Zone>((LustGas.Zone z) => Vector2.Distance(z.center, her) < z.radius) != null;
			if (LustGas.InZone)
			{
				num2 += 0.25f;
			}
			LustGas.Exposure = Mathf.Clamp01(num2);
			LustGas.Fog(LustGas.InZone, dt);
			if (LustGas.Exposure > 0.02f && LewdSettings.On)
			{
				LustGas.Breathe(b, dt);
			}
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x00043D74 File Offset: 0x00041F74
		private static void Breathe(Body b, float dt)
		{
			State s = Plugin.S;
			float num = Counter.MaskFactor(b, dt);
			if (num < 0.9f && (LustGas.maskLine -= dt) <= 0f)
			{
				LustGas.maskLine = 60f;
				Relief.Say(b, "gas_mask");
			}
			float num2 = LustGas.Exposure * Plugin.GasStrength.Value * (s.inHeat ? 1.4f : 1f) * (b.breathing ? 1f : 0f) * num * Suppressant.Factor;
			s.lust = Mathf.Min(100f, s.lust + 1.2f * num2 * dt);
			s.arousal = Mathf.Min(100f, s.arousal + 1.8f * num2 * dt);
			if (LustGas.Exposure > 0.5f)
			{
				b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 0.05f);
				s.corruption = Mathf.Min(100f, s.corruption + 0.004f * num2 * dt);
			}
			if ((LustGas.lineTimer -= dt) <= 0f)
			{
				LustGas.lineTimer = Random.Range(12f, 22f);
				Relief.Say(b, (LustGas.Exposure > 0.5f) ? "gas_thick" : "gas_in");
			}
			if (LustGas.Exposure > 0.6f && (LustGas.coughTimer -= dt) <= 0f)
			{
				LustGas.coughTimer = Random.Range(6f, 12f);
				Sfx.Play(b, 0.5f, new string[] { "breath", "sigh" });
			}
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00043F20 File Offset: 0x00042120
		private static void Fog(bool inside, float dt)
		{
			WorldGeneration world = WorldGeneration.world;
			if (!world || !world.fogSprite)
			{
				return;
			}
			LustGas.fogBlend = Mathf.MoveTowards(LustGas.fogBlend, inside ? 1f : 0f, dt * 0.5f);
			if (LustGas.fogBlend <= 0f)
			{
				if (LustGas.fogTouched)
				{
					LustGas.RestoreFog();
				}
				return;
			}
			LustGas.fogTouched = true;
			Color color;
			color..ctor(0.8f, 0.8f, 0.8f, world.fogAmount);
			Color color2;
			color2..ctor(0.95f, 0.55f, 0.82f, Mathf.Max(world.fogAmount, Plugin.GasFog.Value));
			world.fogSprite.color = Color.Lerp(color, color2, LustGas.fogBlend);
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00043FEC File Offset: 0x000421EC
		private static void RestoreFog()
		{
			WorldGeneration world = WorldGeneration.world;
			LustGas.fogBlend = 0f;
			if (LustGas.fogTouched && world && world.fogSprite)
			{
				world.fogSprite.color = new Color(0.8f, 0.8f, 0.8f, world.fogAmount);
			}
			LustGas.fogTouched = false;
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00044050 File Offset: 0x00042250
		public static void DebugVent(Body b)
		{
			float num = Scenes.FeetY(b);
			Vector2? vector = LustGas.FloorBelow(new Vector2(b.transform.position.x + (b.isRight ? 3f : (-3f)), num + 1.5f), 12);
			Vector2? vector2 = ((vector != null) ? vector : LustGas.FloorBelow(new Vector2(b.transform.position.x, num + 1.5f), 12));
			if (vector2 == null)
			{
				return;
			}
			LustGas.AddVent(vector2.Value).cooldown = 0f;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x000440ED File Offset: 0x000422ED
		public static void DebugBiome(Body b)
		{
			LustGas.Build(b.transform.position, Plugin.GasBiomeRadius.Value);
		}

		// Token: 0x040003C4 RID: 964
		private static readonly List<LustGas.Puff> puffs = new List<LustGas.Puff>();

		// Token: 0x040003C5 RID: 965
		private static readonly List<LustGas.Vent> vents = new List<LustGas.Vent>();

		// Token: 0x040003C6 RID: 966
		private static readonly List<LustGas.Zone> zones = new List<LustGas.Zone>();

		// Token: 0x040003C7 RID: 967
		private static readonly List<GameObject> spawned = new List<GameObject>();

		// Token: 0x040003C8 RID: 968
		private static Sprite[] puffSprites;

		// Token: 0x040003C9 RID: 969
		private static Material unlit;

		// Token: 0x040003CA RID: 970
		private static Sprite ventSprite;

		// Token: 0x040003CB RID: 971
		private static float lineTimer;

		// Token: 0x040003CC RID: 972
		private static float coughTimer;

		// Token: 0x040003CD RID: 973
		private const int MaxPuffs = 260;

		// Token: 0x040003CE RID: 974
		private static readonly Color Pink = new Color32(236, 120, 196, byte.MaxValue);

		// Token: 0x040003D1 RID: 977
		private static float maskLine;

		// Token: 0x040003D2 RID: 978
		private static float fogBlend;

		// Token: 0x040003D3 RID: 979
		private static bool fogTouched;

		// Token: 0x020001DE RID: 478
		private class Puff
		{
			// Token: 0x04000A02 RID: 2562
			public GameObject go;

			// Token: 0x04000A03 RID: 2563
			public SpriteRenderer sr;

			// Token: 0x04000A04 RID: 2564
			public Vector2 pos;

			// Token: 0x04000A05 RID: 2565
			public Vector2 vel;

			// Token: 0x04000A06 RID: 2566
			public float age;

			// Token: 0x04000A07 RID: 2567
			public float life;

			// Token: 0x04000A08 RID: 2568
			public float size;

			// Token: 0x04000A09 RID: 2569
			public float alpha;

			// Token: 0x04000A0A RID: 2570
			public bool ambient;

			// Token: 0x04000A0B RID: 2571
			public LustGas.Zone home;
		}

		// Token: 0x020001DF RID: 479
		internal class Vent
		{
			// Token: 0x04000A0C RID: 2572
			public GameObject go;

			// Token: 0x04000A0D RID: 2573
			public SpriteRenderer sr;

			// Token: 0x04000A0E RID: 2574
			public Light2D glow;

			// Token: 0x04000A0F RID: 2575
			public Vector2 pos;

			// Token: 0x04000A10 RID: 2576
			public float rumble = -1f;

			// Token: 0x04000A11 RID: 2577
			public float spout = -1f;

			// Token: 0x04000A12 RID: 2578
			public float cooldown;
		}

		// Token: 0x020001E0 RID: 480
		private class Zone
		{
			// Token: 0x04000A13 RID: 2579
			public Vector2 center;

			// Token: 0x04000A14 RID: 2580
			public float radius;
		}
	}
}
