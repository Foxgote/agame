using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x0200008A RID: 138
	internal static class FleshZones
	{
		// Token: 0x0600029D RID: 669 RVA: 0x0002F29C File Offset: 0x0002D49C
		public static bool Near(Vector2 p, float pad)
		{
			return FleshZones.zones.Any<FleshZones.Zone>((FleshZones.Zone z) => Vector2.Distance(z.center, p) < z.radius + pad);
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600029E RID: 670 RVA: 0x0002F2D3 File Offset: 0x0002D4D3
		// (set) Token: 0x0600029F RID: 671 RVA: 0x0002F2DA File Offset: 0x0002D4DA
		public static float Slow { get; private set; } = 1f;

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060002A0 RID: 672 RVA: 0x0002F2E2 File Offset: 0x0002D4E2
		public static bool OnFlesh
		{
			get
			{
				return FleshZones.Slow < 0.99f;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x0002F2F0 File Offset: 0x0002D4F0
		// (set) Token: 0x060002A2 RID: 674 RVA: 0x0002F2F7 File Offset: 0x0002D4F7
		public static bool InZone { get; private set; }

		// Token: 0x060002A3 RID: 675 RVA: 0x0002F2FF File Offset: 0x0002D4FF
		private static bool Solid(WorldGeneration w, int x, int y)
		{
			return x >= 0 && y >= 0 && (long)x < (long)((ulong)w.width) && (long)y < (long)((ulong)w.height) && w.GetBlock(new Vector2Int(x, y)) > 0;
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0002F334 File Offset: 0x0002D534
		public static void SeedLayer(Body b)
		{
			WorldGeneration world = WorldGeneration.world;
			int num = Diff.Count(Plugin.FleshZonesPerLayer.Value);
			float value = Plugin.FleshZoneRadius.Value;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				Vector2 vector = Vector2.zero;
				int num3 = 0;
				for (int j = 0; j < 300; j++)
				{
					int num4 = Random.Range(8, (int)(world.width - 8U));
					int num5 = Random.Range(8, (int)(world.height - 8U));
					if (!FleshZones.Solid(world, num4, num5) && FleshZones.Solid(world, num4, num5 - 1))
					{
						Vector2 c = world.BlockToWorldPos(new Vector2Int(num4, num5)) + Vector2.up;
						if (Vector2.Distance(c, b.transform.position) >= 30f && !FleshZones.zones.Any<FleshZones.Zone>((FleshZones.Zone z) => Vector2.Distance(z.center, c) < z.radius * 2.5f))
						{
							int num6 = FleshZones.CountFloor(world, c, value);
							if (num6 > num3)
							{
								num3 = num6;
								vector = c;
							}
							if (j > 40 && num3 >= 30)
							{
								break;
							}
						}
					}
				}
				if (num3 >= 3 && FleshZones.Build(vector, value, b.transform.position) > 0)
				{
					num2++;
				}
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: grew {1} flesh biomes", world.biomeDepth, num2));
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0002F4BC File Offset: 0x0002D6BC
		private static int CountFloor(WorldGeneration w, Vector2 center, float radius)
		{
			Vector2Int vector2Int = w.WorldToBlockPos(center);
			int num = Mathf.CeilToInt(radius);
			int num2 = 0;
			for (int i = vector2Int.y - num; i <= vector2Int.y + num; i++)
			{
				for (int j = vector2Int.x - num; j <= vector2Int.x + num; j++)
				{
					if (Vector2.Distance(new Vector2((float)j, (float)i), vector2Int) <= radius && !FleshZones.Solid(w, j, i) && FleshZones.Solid(w, j, i - 1))
					{
						num2++;
					}
				}
			}
			return num2;
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0002F54C File Offset: 0x0002D74C
		public static int Build(Vector2 center, float radius, Vector2 keepClear)
		{
			WorldGeneration world = WorldGeneration.world;
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				FleshArt.Ensure(spriteRenderer.sprite.pixelsPerUnit);
			}
			if (!world || FleshArt.Carpet.Length == 0)
			{
				return 0;
			}
			FleshZones.Zone zone = new FleshZones.Zone
			{
				center = center,
				radius = radius
			};
			FleshZones.buildMark = null;
			Vector2Int vector2Int = world.WorldToBlockPos(center);
			int num = Mathf.CeilToInt(radius);
			int num2 = Mathf.Max(1, Mathf.RoundToInt(FleshArt.Carpet[0].bounds.size.x * 0.8f));
			List<Vector2> list = new List<Vector2>();
			int num3 = 0;
			for (int i = vector2Int.y - num; i <= vector2Int.y + num; i++)
			{
				int num4 = int.MinValue;
				int num5 = int.MinValue;
				for (int j = vector2Int.x - num; j <= vector2Int.x + num; j++)
				{
					float num6 = Vector2.Distance(new Vector2((float)j, (float)i), vector2Int);
					if (num6 <= radius)
					{
						Vector2 vector = world.BlockToWorldPos(new Vector2Int(j, i));
						if (!FleshZones.Solid(world, j, i))
						{
							if (FleshZones.Solid(world, j, i - 1))
							{
								Vector2 vector2;
								vector2..ctor(vector.x, vector.y - 0.5f);
								zone.carpet.Add(vector2);
								if (j - num4 >= num2)
								{
									FleshZones.Decor(FleshArt.Carpet[Random.Range(0, FleshArt.Carpet.Length)], vector2 + Vector2.down * 0.05f, spriteRenderer);
									num4 = j;
								}
								bool flag = true;
								int num7 = -1;
								while (num7 <= 1 && flag)
								{
									flag = FleshZones.Solid(world, j + num7, i - 1) && !FleshZones.Solid(world, j + num7, i) && !FleshZones.Solid(world, j + num7, i + 1);
									num7++;
								}
								if (flag)
								{
									list.Add(vector2);
								}
							}
							if (FleshZones.Solid(world, j, i + 1) && j - num5 >= num2 && Random.value < 0.75f)
							{
								FleshZones.Decor(FleshArt.Hang[Random.Range(0, FleshArt.Hang.Length)], new Vector2(vector.x, vector.y + 0.5f), spriteRenderer);
								num5 = j;
							}
						}
						else if (FleshArt.Vein.Length != 0 && num3 < 300 && (!FleshZones.Solid(world, j + 1, i) || !FleshZones.Solid(world, j - 1, i) || !FleshZones.Solid(world, j, i + 1) || !FleshZones.Solid(world, j, i - 1)) && Random.value > num6 / radius * 0.55f)
						{
							FleshZones.Decor(FleshArt.Vein[Random.Range(0, FleshArt.Vein.Length)], vector + Random.insideUnitCircle * 0.2f, spriteRenderer).transform.rotation = Quaternion.Euler(0f, 0f, (float)Random.Range(0, 4) * 90f);
							num3++;
						}
					}
				}
			}
			if (zone.carpet.Count < 3)
			{
				FleshZones.RemoveLast();
				return 0;
			}
			GameObject gameObject = new GameObject("LewdFleshBiomeGlow");
			gameObject.transform.position = center;
			Light2D light2D = gameObject.AddComponent<Light2D>();
			light2D.lightType = 3;
			light2D.color = new Color32(200, 40, 70, byte.MaxValue);
			light2D.intensity = 0.35f;
			light2D.pointLightOuterRadius = radius * 1.1f;
			light2D.pointLightInnerRadius = radius * 0.3f;
			FleshZones.spawned.Add(gameObject);
			int num8 = 0;
			using (IEnumerator<Vector2> enumerator = list.OrderBy<Vector2, float>((Vector2 _) => Random.value).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Vector2 p = enumerator.Current;
					if (num8 >= Mathf.RoundToInt(Plugin.FleshZoneTraps.Value))
					{
						break;
					}
					if (Vector2.Distance(p, keepClear) >= 5f && !Flesh.Patches.Any<FleshPatch>((FleshPatch f) => f && Vector2.Distance(f.transform.position, p) < 4f) && Flesh.Spawn(p) != null)
					{
						num8++;
					}
				}
			}
			FleshZones.zones.Add(zone);
			Plugin.Log.LogInfo(string.Format("Flesh biome at {0}: {1} floor blocks, {2} growths, {3} traps", new object[]
			{
				center,
				zone.carpet.Count,
				num3,
				num8
			}));
			return zone.carpet.Count;
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0002FA84 File Offset: 0x0002DC84
		private static GameObject Decor(Sprite sprite, Vector2 at, SpriteRenderer like)
		{
			GameObject gameObject = new GameObject("LewdFleshGrowth");
			gameObject.transform.position = at;
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprite;
			spriteRenderer.flipX = Random.value < 0.5f;
			if (WorldGeneration.world)
			{
				spriteRenderer.sharedMaterial = WorldGeneration.world.defaultMat;
			}
			if (like)
			{
				spriteRenderer.sortingLayerID = like.sortingLayerID;
				spriteRenderer.sortingOrder = like.sortingOrder - 16;
			}
			FleshZones.spawned.Add(gameObject);
			if (FleshZones.buildMark == null)
			{
				FleshZones.buildMark = gameObject;
			}
			return gameObject;
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0002FB28 File Offset: 0x0002DD28
		private static void RemoveLast()
		{
			if (!FleshZones.buildMark)
			{
				return;
			}
			int num = FleshZones.spawned.IndexOf(FleshZones.buildMark);
			if (num >= 0)
			{
				for (int i = FleshZones.spawned.Count - 1; i >= num; i--)
				{
					if (FleshZones.spawned[i])
					{
						Object.Destroy(FleshZones.spawned[i]);
					}
				}
				FleshZones.spawned.RemoveRange(num, FleshZones.spawned.Count - num);
			}
			FleshZones.buildMark = null;
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0002FBAC File Offset: 0x0002DDAC
		public static void Clear()
		{
			foreach (GameObject gameObject in FleshZones.spawned)
			{
				if (gameObject)
				{
					Object.Destroy(gameObject);
				}
			}
			FleshZones.spawned.Clear();
			FleshZones.zones.Clear();
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0002FC1C File Offset: 0x0002DE1C
		public static void Update(Body b, float dt)
		{
			Vector2 pos = b.transform.position;
			FleshZones.Zone zone = FleshZones.zones.FirstOrDefault<FleshZones.Zone>((FleshZones.Zone q) => Vector2.Distance(q.center, pos) < q.radius + 2f);
			FleshZones.InZone = zone != null;
			FleshZones.Slow = 1f;
			if (zone == null)
			{
				return;
			}
			float feet = Scenes.FeetY(b);
			if (zone.carpet.Any<Vector2>((Vector2 c) => Mathf.Abs(c.x - pos.x) < 0.8f && feet - c.y < 1.2f && feet - c.y > -0.6f))
			{
				FleshZones.Slow = Plugin.FleshZoneSlow.Value;
				if (b.rb.velocity.sqrMagnitude > 0.5f)
				{
					b.stamina = Mathf.Max(0f, b.stamina - 0.4f * dt);
				}
			}
			Plugin.S.arousal = Mathf.Min(100f, Plugin.S.arousal + 0.08f * dt);
			if ((FleshZones.ambient -= dt) <= 0f)
			{
				FleshZones.ambient = Random.Range(2.5f, 6f);
				Sound.Play((Random.value < 0.5f) ? "goo" : "drip", pos + Random.insideUnitCircle * 6f, false, true, null, 0.35f, Random.Range(0.5f, 0.8f), false, false);
			}
		}

		// Token: 0x0400027C RID: 636
		private static readonly List<FleshZones.Zone> zones = new List<FleshZones.Zone>();

		// Token: 0x0400027D RID: 637
		private static readonly List<GameObject> spawned = new List<GameObject>();

		// Token: 0x0400027E RID: 638
		private static float ambient;

		// Token: 0x04000281 RID: 641
		private static GameObject buildMark;

		// Token: 0x020001AB RID: 427
		private class Zone
		{
			// Token: 0x0400096E RID: 2414
			public Vector2 center;

			// Token: 0x0400096F RID: 2415
			public float radius;

			// Token: 0x04000970 RID: 2416
			public readonly List<Vector2> carpet = new List<Vector2>();
		}
	}
}
