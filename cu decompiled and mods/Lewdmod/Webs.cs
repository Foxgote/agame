using System;
using System.Collections.Generic;
using System.Linq;
using PoseKit;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x0200012D RID: 301
	internal static class Webs
	{
		// Token: 0x06000710 RID: 1808 RVA: 0x0006D9D0 File Offset: 0x0006BBD0
		public static bool Near(Vector2 p, float pad)
		{
			return Webs.zones.Any<Webs.Zone>((Webs.Zone z) => Vector2.Distance(z.center, p) < z.radius + pad);
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000711 RID: 1809 RVA: 0x0006DA07 File Offset: 0x0006BC07
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x0006DA0E File Offset: 0x0006BC0E
		private static void Litter(GameObject go)
		{
			if (go && Plugin.WebDebrisSeconds.Value > 0f)
			{
				Webs.debris.Add(new KeyValuePair<GameObject, float>(go, Time.time));
			}
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x0006DA40 File Offset: 0x0006BC40
		private static void Tidy()
		{
			float value = Plugin.WebDebrisSeconds.Value;
			if (value <= 0f)
			{
				return;
			}
			for (int i = Webs.debris.Count - 1; i >= 0; i--)
			{
				GameObject go = Webs.debris[i].Key;
				float num = Time.time - Webs.debris[i].Value;
				if (!go)
				{
					Webs.debris.RemoveAt(i);
				}
				else if (num >= value)
				{
					SpriteRenderer component = go.GetComponent<SpriteRenderer>();
					float num2 = 1f - Mathf.Clamp01((num - value) / 4f);
					if (component)
					{
						Color color = component.color;
						color.a = num2;
						component.color = color;
					}
					if (num2 <= 0f)
					{
						Webs.sheets.RemoveAll((Webs.Sheet s) => s.go == go);
						Webs.spawned.Remove(go);
						Object.Destroy(go);
						Webs.debris.RemoveAt(i);
					}
				}
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000714 RID: 1812 RVA: 0x0006DB6B File Offset: 0x0006BD6B
		// (set) Token: 0x06000715 RID: 1813 RVA: 0x0006DB72 File Offset: 0x0006BD72
		public static float Slow { get; private set; } = 1f;

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000716 RID: 1814 RVA: 0x0006DB7A File Offset: 0x0006BD7A
		public static bool OnSilk
		{
			get
			{
				return Webs.Slow < 0.99f;
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000717 RID: 1815 RVA: 0x0006DB88 File Offset: 0x0006BD88
		public static bool Caught
		{
			get
			{
				return Webs.caught != null;
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000718 RID: 1816 RVA: 0x0006DB92 File Offset: 0x0006BD92
		// (set) Token: 0x06000719 RID: 1817 RVA: 0x0006DB99 File Offset: 0x0006BD99
		public static bool InCocoon { get; private set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x0600071A RID: 1818 RVA: 0x0006DBA1 File Offset: 0x0006BDA1
		// (set) Token: 0x0600071B RID: 1819 RVA: 0x0006DBA8 File Offset: 0x0006BDA8
		public static float Tangle { get; private set; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x0600071C RID: 1820 RVA: 0x0006DBB0 File Offset: 0x0006BDB0
		// (set) Token: 0x0600071D RID: 1821 RVA: 0x0006DBB7 File Offset: 0x0006BDB7
		public static float Air { get; private set; }

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x0600071E RID: 1822 RVA: 0x0006DBBF File Offset: 0x0006BDBF
		public static bool Suffocating
		{
			get
			{
				return Webs.InCocoon && Webs.Air <= 0f;
			}
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x0006DBD9 File Offset: 0x0006BDD9
		private static bool Solid(WorldGeneration w, int x, int y)
		{
			return x >= 0 && y >= 0 && (long)x < (long)((ulong)w.width) && (long)y < (long)((ulong)w.height) && w.GetBlock(new Vector2Int(x, y)) > 0;
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x0006DC0C File Offset: 0x0006BE0C
		public static void SeedLayer(Body b)
		{
			WorldGeneration world = WorldGeneration.world;
			int num = Diff.Count(Plugin.WebZonesPerLayer.Value) + ((world.biomeDepth >= 3 && Random.value < 0.5f) ? 1 : 0);
			float value = Plugin.WebZoneRadius.Value;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				Vector2 vector = Vector2.zero;
				int num3 = 0;
				for (int j = 0; j < 300; j++)
				{
					int num4 = Random.Range(8, (int)(world.width - 8U));
					int num5 = Random.Range(8, (int)(world.height - 8U));
					if (!Webs.Solid(world, num4, num5) && Webs.Solid(world, num4, num5 - 1))
					{
						Vector2 c = world.BlockToWorldPos(new Vector2Int(num4, num5)) + Vector2.up;
						if (Vector2.Distance(c, b.transform.position) >= 30f && !Webs.zones.Any<Webs.Zone>((Webs.Zone z) => Vector2.Distance(z.center, c) < z.radius * 2.5f))
						{
							int count = Webs.Passages(world, c, value).Count;
							if (count > num3)
							{
								num3 = count;
								vector = c;
							}
							if (j > 40 && num3 >= 8)
							{
								break;
							}
						}
					}
				}
				if (num3 >= 2 && Webs.Build(vector, value, b.transform.position) > 0)
				{
					num2++;
				}
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: spun {1} web biomes", world.biomeDepth, num2));
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x0006DDB0 File Offset: 0x0006BFB0
		private static List<Vector2> Passages(WorldGeneration w, Vector2 center, float radius)
		{
			List<Vector2> list = new List<Vector2>();
			Vector2Int vector2Int = w.WorldToBlockPos(center);
			int num = Mathf.CeilToInt(radius);
			for (int i = vector2Int.y - num; i <= vector2Int.y + num; i++)
			{
				for (int j = vector2Int.x - num; j <= vector2Int.x + num; j++)
				{
					if (Vector2.Distance(new Vector2((float)j, (float)i), vector2Int) <= radius && !Webs.Solid(w, j, i) && Webs.Solid(w, j, i - 1))
					{
						int num2 = 0;
						while (num2 < 7 && !Webs.Solid(w, j, i + num2))
						{
							num2++;
						}
						if (num2 >= 3 && num2 <= 6)
						{
							list.Add(w.BlockToWorldPos(new Vector2Int(j, i)) + Vector2.down * 0.5f);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x0006DE9C File Offset: 0x0006C09C
		public static int Build(Vector2 center, float radius, Vector2 keepClear)
		{
			WorldGeneration world = WorldGeneration.world;
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				WebArt.Ensure(spriteRenderer.sprite.pixelsPerUnit);
			}
			if (!world || !WebArt.Ready)
			{
				return 0;
			}
			Webs.Zone zone = new Webs.Zone
			{
				center = center,
				radius = radius
			};
			int count = Webs.spawned.Count;
			int count2 = Webs.sheets.Count;
			Vector2Int vector2Int = world.WorldToBlockPos(center);
			int num = Mathf.CeilToInt(radius);
			int num2 = Mathf.Max(1, Mathf.RoundToInt(WebArt.Silk[0].bounds.size.x * 0.8f));
			int num3 = 0;
			for (int i = vector2Int.y - num; i <= vector2Int.y + num; i++)
			{
				int num4 = int.MinValue;
				int num5 = int.MinValue;
				for (int j = vector2Int.x - num; j <= vector2Int.x + num; j++)
				{
					if (Vector2.Distance(new Vector2((float)j, (float)i), vector2Int) <= radius && !Webs.Solid(world, j, i))
					{
						Vector2 vector = world.BlockToWorldPos(new Vector2Int(j, i));
						if (Webs.Solid(world, j, i - 1))
						{
							Vector2 vector2;
							vector2..ctor(vector.x, vector.y - 0.5f);
							zone.silk.Add(vector2);
							if (j - num4 >= num2 && Random.value < 0.85f)
							{
								Webs.Decor(WebArt.Silk[Random.Range(0, WebArt.Silk.Length)], vector2 + Vector2.down * 0.05f, spriteRenderer, -14);
								num4 = j;
							}
						}
						if (Webs.Solid(world, j, i + 1))
						{
							bool flag = Webs.Solid(world, j - 1, i);
							bool flag2 = Webs.Solid(world, j + 1, i);
							if ((flag || flag2) && WebArt.Corner && Random.value < 0.7f)
							{
								GameObject gameObject = Webs.Decor(WebArt.Corner, new Vector2(flag ? (vector.x - 0.5f) : (vector.x + 0.5f), vector.y + 0.5f), spriteRenderer, -15);
								gameObject.GetComponent<SpriteRenderer>().flipX = !flag;
								gameObject.transform.localScale = Vector3.one * Random.Range(1.2f, 1.8f) / Mathf.Max(0.01f, WebArt.Corner.bounds.size.x);
								if (!flag)
								{
									gameObject.transform.position += Vector3.right * 0f;
								}
							}
							else if (j - num5 >= 2 && Random.value < 0.3f)
							{
								Webs.Decor(WebArt.Hang[Random.Range(0, WebArt.Hang.Length)], new Vector2(vector.x + Random.Range(-0.3f, 0.3f), vector.y + 0.5f), spriteRenderer, -15);
								num5 = j;
							}
							else if (num3 < 3 && WebArt.Old.Length != 0 && !Webs.Solid(world, j, i - 1) && !Webs.Solid(world, j, i - 2) && Random.value < 0.05f)
							{
								Webs.Decor(WebArt.Old[Random.Range(0, WebArt.Old.Length)], new Vector2(vector.x, vector.y + 0.5f), spriteRenderer, -13);
								num3++;
							}
						}
					}
				}
			}
			if (zone.silk.Count < 3)
			{
				Webs.Remove(count, count2);
				return 0;
			}
			GameObject gameObject2 = new GameObject("LewdWebBiomeGlow");
			gameObject2.transform.position = center;
			Light2D light2D = gameObject2.AddComponent<Light2D>();
			light2D.lightType = 3;
			light2D.color = new Color32(190, 205, 230, byte.MaxValue);
			light2D.intensity = 0.25f;
			light2D.pointLightOuterRadius = radius * 1.1f;
			light2D.pointLightInnerRadius = radius * 0.3f;
			Webs.spawned.Add(gameObject2);
			int num6 = 0;
			float value = Plugin.WebSheetSize.Value;
			foreach (Vector2 vector3 in from _ in Webs.Passages(world, center, radius)
				orderby Random.value
				select _)
			{
				if (num6 >= Mathf.RoundToInt(Plugin.WebSheetsPerBiome.Value))
				{
					break;
				}
				Vector2 at = vector3 + Vector2.up * 1.35f;
				if (Vector2.Distance(at, keepClear) >= 4f && !Webs.sheets.Any<Webs.Sheet>((Webs.Sheet s) => Vector2.Distance(s.center, at) < 4f))
				{
					GameObject gameObject3 = Webs.Decor(WebArt.Sheet[Random.Range(0, WebArt.Sheet.Length)], at, spriteRenderer, -12);
					gameObject3.name = "LewdWebSheet";
					gameObject3.transform.localScale = Vector3.one * value / Mathf.Max(0.01f, gameObject3.GetComponent<SpriteRenderer>().sprite.bounds.size.x);
					gameObject3.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-15f, 15f));
					Webs.sheets.Add(new Webs.Sheet
					{
						go = gameObject3,
						sr = gameObject3.GetComponent<SpriteRenderer>(),
						center = at
					});
					num6++;
				}
			}
			Webs.zones.Add(zone);
			Plugin.Log.LogInfo(string.Format("Web biome at {0}: {1} floor blocks, {2} webs, {3} old cocoons", new object[]
			{
				center,
				zone.silk.Count,
				num6,
				num3
			}));
			return zone.silk.Count;
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x0006E55C File Offset: 0x0006C75C
		private static GameObject Decor(Sprite sprite, Vector2 at, SpriteRenderer like, int order)
		{
			GameObject gameObject = new GameObject("LewdWeb");
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
				spriteRenderer.sortingOrder = like.sortingOrder + order;
			}
			Webs.spawned.Add(gameObject);
			return gameObject;
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x0006E5F0 File Offset: 0x0006C7F0
		private static void Remove(int from, int fromSheet)
		{
			for (int i = Webs.spawned.Count - 1; i >= from; i--)
			{
				if (Webs.spawned[i])
				{
					Object.Destroy(Webs.spawned[i]);
				}
			}
			if (from < Webs.spawned.Count)
			{
				Webs.spawned.RemoveRange(from, Webs.spawned.Count - from);
			}
			if (fromSheet < Webs.sheets.Count)
			{
				Webs.sheets.RemoveRange(fromSheet, Webs.sheets.Count - fromSheet);
			}
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x0006E680 File Offset: 0x0006C880
		public static void Clear(Body b)
		{
			if (Webs.Caught || Webs.InCocoon)
			{
				Webs.Release(b, false);
			}
			foreach (GameObject gameObject in Webs.spawned)
			{
				if (gameObject)
				{
					Object.Destroy(gameObject);
				}
			}
			Webs.spawned.Clear();
			Webs.sheets.Clear();
			Webs.zones.Clear();
			Webs.debris.Clear();
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x0006E718 File Offset: 0x0006C918
		public static void Update(Body b, float dt)
		{
			Webs.Tidy();
			if (Webs.tugCooldown > 0f)
			{
				Webs.tugCooldown -= dt;
			}
			if (Webs.immune > 0f)
			{
				Webs.immune -= dt;
			}
			if (!b.alive)
			{
				if (Webs.Caught)
				{
					Webs.Die(b);
				}
				return;
			}
			if (Webs.Caught)
			{
				Webs.Held(b, dt);
				return;
			}
			Vector2 pos = b.transform.position;
			Webs.Zone zone = Webs.zones.FirstOrDefault<Webs.Zone>((Webs.Zone q) => Vector2.Distance(q.center, pos) < q.radius + 2f);
			Webs.Slow = 1f;
			if (zone == null)
			{
				return;
			}
			float feet = Scenes.FeetY(b);
			if (zone.silk.Any<Vector2>((Vector2 c) => Mathf.Abs(c.x - pos.x) < 0.8f && feet - c.y < 1.2f && feet - c.y > -0.6f))
			{
				Webs.Slow = Plugin.WebSlow.Value;
			}
			if ((Webs.ambient -= dt) <= 0f)
			{
				Webs.ambient = Random.Range(4f, 9f);
				Sound.Play("bushbig", pos + Random.insideUnitCircle * 6f, false, true, null, 0.18f, Random.Range(1.3f, 1.7f), false, false);
			}
			if (Webs.immune > 0f || Webs.Busy())
			{
				return;
			}
			Vector2 chest = b.limbs[1].transform.position;
			Webs.Sheet sheet = Webs.sheets.FirstOrDefault<Webs.Sheet>((Webs.Sheet s) => !s.torn && s.go && Vector2.Distance(s.center, chest) < Plugin.WebSheetSize.Value * 0.38f);
			if (sheet != null)
			{
				Webs.Catch(b, sheet);
			}
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x0006E8AC File Offset: 0x0006CAAC
		private static bool Busy()
		{
			return Flowers.Holding || Tentacles.Holding || Worm.Inside || Flesh.Holding || FleshWall.Holding || Pits.InMaw || Pits.InThroat || Slits.Holding || Slits.Inside || SlimeBog.Stuck || Womb.InLabor || External.ScavHolding || HerPuppet.Active;
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x0006E914 File Offset: 0x0006CB14
		public static void Catch(Body b, Vector2 at)
		{
			GameObject gameObject = new GameObject("LewdWebSheet");
			gameObject.transform.position = at;
			Webs.Sheet sheet = new Webs.Sheet
			{
				go = gameObject,
				center = at
			};
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			if (component && component.sprite)
			{
				WebArt.Ensure(component.sprite.pixelsPerUnit);
			}
			if (WebArt.Ready)
			{
				Object.Destroy(gameObject);
				sheet.go = Webs.Decor(WebArt.Sheet[0], at, component, -12);
				sheet.go.transform.localScale = Vector3.one * Plugin.WebSheetSize.Value / Mathf.Max(0.01f, WebArt.Sheet[0].bounds.size.x);
				sheet.sr = sheet.go.GetComponent<SpriteRenderer>();
			}
			Webs.sheets.Add(sheet);
			Webs.Catch(b, sheet);
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x0006EA18 File Offset: 0x0006CC18
		private static void BehindHer(Body b, SpriteRenderer sr)
		{
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			if (!sr || !component)
			{
				return;
			}
			int num = component.sortingOrder;
			Limb[] limbs = b.limbs;
			for (int i = 0; i < limbs.Length; i++)
			{
				SpriteRenderer spriteRenderer;
				if (limbs[i].TryGetComponent<SpriteRenderer>(ref spriteRenderer) && spriteRenderer.sortingLayerID == component.sortingLayerID)
				{
					num = Mathf.Min(num, spriteRenderer.sortingOrder);
				}
			}
			sr.sortingLayerID = component.sortingLayerID;
			sr.sortingOrder = Mathf.Max(num - 1, component.sortingOrder - 15);
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x0006EAAC File Offset: 0x0006CCAC
		private static void BehindAllOfHer(Body b, SpriteRenderer sr)
		{
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			if (!sr || !component)
			{
				return;
			}
			int num = component.sortingOrder;
			foreach (Limb limb in b.limbs)
			{
				SpriteRenderer spriteRenderer;
				if (limb && limb.TryGetComponent<SpriteRenderer>(ref spriteRenderer) && spriteRenderer.enabled && spriteRenderer.sortingLayerID == component.sortingLayerID)
				{
					num = Mathf.Min(num, spriteRenderer.sortingOrder);
				}
			}
			sr.sortingLayerID = component.sortingLayerID;
			sr.sortingOrder = num - 1;
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x0006EB48 File Offset: 0x0006CD48
		private static void Catch(Body b, Webs.Sheet s)
		{
			Webs.caught = s;
			Webs.BehindHer(b, s.sr);
			Webs.facing = (b.isRight ? 1 : (-1));
			Relief.ForceStop();
			Webs.posed = Poses.All.ContainsKey("web_stuck") && HerPuppet.Begin(b);
			if (!Webs.posed)
			{
				b.Ragdoll();
			}
			Webs.Tangle = Random.Range(15f, 30f);
			Webs.InCocoon = false;
			Webs.Air = Plugin.CocoonAirSeconds.Value;
			Webs.stillTime = 0f;
			Webs.lineTimer = 6f;
			b.eyePanicTime = 0.6f;
			b.adrenaline += 10f;
			Sound.Play("ropeplace", s.center, false, true, null, 0.9f, 0.8f, false, false);
			Sound.Play("bushbig", s.center, false, true, null, 0.6f, 1.4f, false, false);
			Relief.Say(b, "web_caught");
			Plugin.Log.LogInfo(string.Format("She is stuck in a web (tangle {0:0}, {1})", Webs.Tangle, Webs.posed ? "posed" : "ragdoll"));
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x0006EC80 File Offset: 0x0006CE80
		private static void Held(Body b, float dt)
		{
			Vector2 center = Webs.caught.center;
			if (Webs.posed)
			{
				Webs.twitch = Mathf.MoveTowards(Webs.twitch, 0f, dt * 2.2f);
				string text = (Webs.InCocoon ? "cocoon" : "web_stuck");
				ScenePose scenePose = Poses.All[text];
				ScenePose scenePose3;
				ScenePose scenePose2 = (Poses.All.TryGetValue(text + "_b", out scenePose3) ? scenePose3 : scenePose);
				float num = (Webs.InCocoon ? (Webs.twitch * 0.8f) : Mathf.Max(Webs.twitch, 0.15f + 0.15f * Mathf.Sin(Time.time * 1.7f)));
				HerPuppet.Show(scenePose, scenePose2, Mathf.Clamp01(num), center, Webs.facing);
			}
			else
			{
				foreach (Limb limb in b.limbs)
				{
					if (limb.rb)
					{
						limb.rb.velocity *= 0.2f;
					}
				}
				Vector2 vector = center - b.limbs[1].transform.position;
				if (vector.sqrMagnitude > 0.04f)
				{
					foreach (Limb limb2 in b.limbs)
					{
						if (limb2.rb)
						{
							limb2.rb.velocity += vector * 6f;
						}
					}
				}
			}
			if (Keys.Down(Plugin.RemoveKey.Value))
			{
				Webs.stillTime = 0f;
				Webs.Tug(b);
				if (!Webs.Caught)
				{
					return;
				}
			}
			else
			{
				Webs.stillTime += dt;
			}
			if (!Webs.InCocoon)
			{
				if (Webs.stillTime > 2f)
				{
					Webs.Tangle = Mathf.Max(5f, Webs.Tangle - Plugin.WebLoosen.Value * dt);
				}
				if ((Webs.lineTimer -= dt) <= 0f)
				{
					Webs.lineTimer = Random.Range(8f, 14f);
					Relief.Say(b, (Webs.Tangle > 70f) ? "web_tight" : ((Webs.stillTime > 2f) ? "web_still" : "web_stuck"));
				}
				return;
			}
			Webs.Air = Mathf.Max(0f, Webs.Air - dt * ((b.stamina < 20f) ? 1.5f : 1f));
			if (Webs.Air <= 0f)
			{
				b.bloodOxygen = Mathf.Max(0f, b.bloodOxygen - Plugin.CocoonSuffocation.Value * Diff.Lethal * dt);
			}
			b.stamina = Mathf.Max(0f, b.stamina - 0.3f * dt);
			b.temperature = Mathf.MoveTowards(b.temperature, 38f, 0.02f * dt);
			if (Webs.cocoonGo)
			{
				Webs.Follow(b);
			}
			if ((Webs.rustleTimer -= dt) <= 0f)
			{
				Webs.rustleTimer = Random.Range(5f, 9f);
				Sfx.Play(b, 0.45f, new string[] { "muffled" });
			}
			if ((Webs.lineTimer -= dt) <= 0f)
			{
				Webs.lineTimer = Random.Range(7f, 12f);
				Relief.Say(b, (Webs.Air <= 0f) ? "cocoon_air" : "cocoon_in");
			}
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x0006F01C File Offset: 0x0006D21C
		private static void Tug(Body b)
		{
			if (Webs.tugCooldown > 0f || !b.conscious)
			{
				return;
			}
			Webs.tugCooldown = 0.8f;
			Webs.twitch = 1f;
			b.stamina = Mathf.Max(0f, b.stamina - (Webs.InCocoon ? 6f : 5f));
			float num = ((b.stamina < 15f) ? 0.7f : ((b.stamina < 40f) ? 0.85f : 1f));
			float num2 = Mathf.Clamp01(1f - b.averagePain / 100f);
			float num3;
			if (Webs.InCocoon)
			{
				num3 = Plugin.CocoonEscapeChance.Value * Diff.Escape * num * ((b.bloodOxygen < 70f) ? 0.8f : 1f) + Webs.worn;
			}
			else
			{
				num3 = Plugin.WebEscapeChance.Value * Diff.Escape * num * Mathf.Lerp(0.6f, 1f, num2) * (1f - Webs.Tangle / 140f);
			}
			bool flag = Random.value < num3;
			Plugin.Log.LogInfo(string.Format("Tugging at the {0}: chance {1:P0}, {2} (tangle {3:0})", new object[]
			{
				Webs.InCocoon ? "cocoon" : "web",
				num3,
				flag ? "tore free" : "held",
				Webs.Tangle
			}));
			if (flag)
			{
				Webs.Release(b, true);
				return;
			}
			Sound.Play("bushbig", Webs.caught.center, false, true, null, 0.5f, Random.Range(1.1f, 1.5f), false, false);
			if (Webs.InCocoon)
			{
				Webs.worn += Plugin.CocoonWear.Value;
				Sfx.Play(b, 0.6f, new string[] { "muffled" });
				Relief.Say(b, "cocoon_fail");
				return;
			}
			Webs.Tangle += Random.Range(9f, 17f) * Plugin.WebTangleSpeed.Value * ((b.stamina < 30f) ? 1.3f : 1f);
			if (Webs.Tangle >= 100f)
			{
				Webs.Wrap(b);
				return;
			}
			Relief.Say(b, (Webs.Tangle > 70f) ? "web_tight" : "web_fail");
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x0006F280 File Offset: 0x0006D480
		private static void Wrap(Body b)
		{
			Webs.Tangle = 100f;
			Webs.InCocoon = true;
			Webs.worn = 0f;
			Webs.Air = Plugin.CocoonAirSeconds.Value;
			Webs.lineTimer = 4f;
			Webs.twitch = 1f;
			if (Webs.caught.sr)
			{
				Webs.caught.sr.color = new Color(1f, 1f, 1f, 0.55f);
			}
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			if (WebArt.Cocoon)
			{
				Webs.cocoonGo = new GameObject("LewdCocoon");
				SpriteRenderer spriteRenderer = Webs.cocoonGo.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = WebArt.Cocoon;
				if (WorldGeneration.world)
				{
					spriteRenderer.sharedMaterial = WorldGeneration.world.defaultMat;
				}
				if (component)
				{
					spriteRenderer.sortingLayerID = component.sortingLayerID;
					int num = component.sortingOrder;
					Limb[] limbs = b.limbs;
					for (int i = 0; i < limbs.Length; i++)
					{
						foreach (SpriteRenderer spriteRenderer2 in limbs[i].GetComponentsInChildren<SpriteRenderer>())
						{
							if (spriteRenderer2.sortingLayerID == component.sortingLayerID)
							{
								num = Mathf.Max(num, spriteRenderer2.sortingOrder);
							}
						}
					}
					spriteRenderer.sortingOrder = num + 5;
				}
				Webs.Follow(b);
			}
			Sound.Play("bandage", Webs.caught.center, false, true, null, 1f, 0.7f, false, false);
			Sound.Play("bushbig", Webs.caught.center, false, true, null, 0.8f, 0.9f, false, false);
			Relief.Say(b, "cocoon_wrapped");
			Plugin.Log.LogInfo("She has wound herself into a cocoon");
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x0006F448 File Offset: 0x0006D648
		private static void Follow(Body b)
		{
			if (!Webs.cocoonGo || !WebArt.Cocoon)
			{
				return;
			}
			Vector2 vector = b.limbs[0].transform.position;
			List<Limb> list = b.limbs.Where<Limb>((Limb l) => !l.dismembered && l.name.Contains("Foot")).ToList<Limb>();
			if (list.Count == 0)
			{
				list = b.limbs.Where<Limb>((Limb l) => !l.dismembered && l.name.Contains("Crus")).ToList<Limb>();
			}
			Vector2 vector2;
			if (list.Count <= 0)
			{
				vector2 = b.limbs[1].transform.position + Vector2.down;
			}
			else
			{
				vector2 = list.Aggregate(Vector3.zero, (Vector3 s, Limb l) => s + l.transform.position) / (float)list.Count;
			}
			Vector2 vector3 = vector2;
			Vector2 vector4 = vector - vector3;
			Vector2 vector5 = ((vector4.sqrMagnitude > 0.0001f) ? vector4.normalized : Vector2.up);
			Vector2 vector6;
			vector6..ctor(-vector5.y, vector5.x);
			float num = float.MaxValue;
			float num2 = float.MinValue;
			float num3 = 0f;
			foreach (Limb limb in b.limbs)
			{
				if (!limb.dismembered)
				{
					foreach (SpriteRenderer spriteRenderer in limb.GetComponentsInChildren<SpriteRenderer>())
					{
						if (spriteRenderer.enabled && spriteRenderer.sprite && !(spriteRenderer.gameObject == Webs.cocoonGo))
						{
							Bounds bounds = spriteRenderer.bounds;
							for (int k = 0; k < 4; k++)
							{
								Vector2 vector7 = new Vector2((k % 2 == 0) ? bounds.min.x : bounds.max.x, (k < 2) ? bounds.min.y : bounds.max.y) - vector3;
								float num4 = Vector2.Dot(vector7, vector5);
								float num5 = Mathf.Abs(Vector2.Dot(vector7, vector6));
								num = Mathf.Min(num, num4);
								num2 = Mathf.Max(num2, num4);
								num3 = Mathf.Max(num3, num5);
							}
						}
					}
				}
			}
			if (num > num2)
			{
				num = -0.3f;
				num2 = vector4.magnitude + 0.3f;
			}
			float num6 = Mathf.Max(0.5f, (num2 - num) * 1.08f);
			float num7 = Mathf.Clamp(num3 * 1.5f, num6 * 0.3f, num6 * 0.5f);
			Vector2 vector8 = vector3 + vector5 * ((num + num2) * 0.5f);
			Vector2 vector9 = WebArt.Cocoon.bounds.size;
			Webs.cocoonGo.transform.position = vector8;
			Webs.cocoonGo.transform.rotation = Quaternion.FromToRotation(Vector2.up, vector5);
			Webs.cocoonGo.transform.localScale = new Vector3(num7 / vector9.x, num6 / vector9.y, 1f);
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x0006F7BF File Offset: 0x0006D9BF
		public static void LetGo(Body b)
		{
			if (Webs.Caught)
			{
				Webs.Release(b, true);
			}
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x0006F7D0 File Offset: 0x0006D9D0
		private static void Release(Body b, bool escaped)
		{
			Webs.Sheet sheet = Webs.caught;
			Webs.caught = null;
			if (Webs.posed)
			{
				HerPuppet.End();
				Webs.posed = false;
			}
			if (sheet != null)
			{
				sheet.torn = true;
				if (sheet.sr && WebArt.SheetTorn)
				{
					sheet.sr.sprite = WebArt.SheetTorn;
					sheet.sr.color = Color.white;
				}
				if (b)
				{
					Webs.BehindAllOfHer(b, sheet.sr);
				}
				Webs.Litter(sheet.go);
			}
			if (Webs.cocoonGo)
			{
				if (escaped && WebArt.CocoonTorn)
				{
					SpriteRenderer component = Webs.cocoonGo.GetComponent<SpriteRenderer>();
					component.sprite = WebArt.CocoonTorn;
					if (b)
					{
						Webs.BehindAllOfHer(b, component);
					}
					Webs.spawned.Add(Webs.cocoonGo);
					Webs.Litter(Webs.cocoonGo);
				}
				else if (!escaped && b && !b.alive)
				{
					Webs.spawned.Add(Webs.cocoonGo);
				}
				else
				{
					Object.Destroy(Webs.cocoonGo);
				}
				Webs.cocoonGo = null;
			}
			bool inCocoon = Webs.InCocoon;
			Webs.InCocoon = false;
			Webs.Tangle = 0f;
			if (!escaped || !b)
			{
				return;
			}
			Webs.immune = 4f;
			Sound.Play("receiptRip", b.transform.position, false, true, null, 1f, inCocoon ? 0.7f : 0.9f, false, false);
			Sound.Play("bushbig", b.transform.position, false, true, null, 0.7f, 1.2f, false, false);
			Vector2 vector;
			vector..ctor((float)(-(float)Webs.facing) * 2.5f, 3f);
			b.rb.velocity = vector;
			foreach (Limb limb in b.limbs)
			{
				if (limb.rb)
				{
					limb.rb.velocity = vector;
				}
			}
			b.wetness = Mathf.Max(b.wetness, 10f);
			Relief.Say(b, inCocoon ? "cocoon_free" : "web_free");
			Plugin.Log.LogInfo(inCocoon ? "She broke out of the cocoon" : "She tore herself out of the web");
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x0006FA1F File Offset: 0x0006DC1F
		private static void Die(Body b)
		{
			if (Webs.cocoonGo)
			{
				Webs.Follow(b);
			}
			Plugin.Log.LogInfo(Webs.InCocoon ? "She suffocated in the cocoon" : "She died stuck in a web");
			Webs.Release(b, false);
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x0006FA57 File Offset: 0x0006DC57
		public static void DebugBiome(Body b)
		{
			Webs.Build(b.transform.position, Plugin.WebZoneRadius.Value, b.transform.position);
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x0006FA8C File Offset: 0x0006DC8C
		public static void DebugCocoon(Body b)
		{
			if (!Webs.Caught)
			{
				if (!Webs.Busy())
				{
					Webs.Catch(b, b.limbs[1].transform.position + Vector2.up * 0.4f);
					return;
				}
			}
			else if (!Webs.InCocoon)
			{
				Webs.Wrap(b);
			}
		}

		// Token: 0x0400075D RID: 1885
		private static readonly List<Webs.Zone> zones = new List<Webs.Zone>();

		// Token: 0x0400075E RID: 1886
		private static readonly List<Webs.Sheet> sheets = new List<Webs.Sheet>();

		// Token: 0x0400075F RID: 1887
		private static readonly List<GameObject> spawned = new List<GameObject>();

		// Token: 0x04000760 RID: 1888
		private static readonly List<KeyValuePair<GameObject, float>> debris = new List<KeyValuePair<GameObject, float>>();

		// Token: 0x04000765 RID: 1893
		private static Webs.Sheet caught;

		// Token: 0x04000766 RID: 1894
		private static bool posed;

		// Token: 0x04000767 RID: 1895
		private static float tugCooldown;

		// Token: 0x04000768 RID: 1896
		private static float stillTime;

		// Token: 0x04000769 RID: 1897
		private static float lineTimer;

		// Token: 0x0400076A RID: 1898
		private static float rustleTimer;

		// Token: 0x0400076B RID: 1899
		private static float immune;

		// Token: 0x0400076C RID: 1900
		private static float twitch;

		// Token: 0x0400076D RID: 1901
		private static float ambient;

		// Token: 0x0400076E RID: 1902
		private static GameObject cocoonGo;

		// Token: 0x0400076F RID: 1903
		private static int facing = 1;

		// Token: 0x04000770 RID: 1904
		private static float worn;

		// Token: 0x0200024C RID: 588
		private class Zone
		{
			// Token: 0x04000B56 RID: 2902
			public Vector2 center;

			// Token: 0x04000B57 RID: 2903
			public float radius;

			// Token: 0x04000B58 RID: 2904
			public readonly List<Vector2> silk = new List<Vector2>();
		}

		// Token: 0x0200024D RID: 589
		private class Sheet
		{
			// Token: 0x04000B59 RID: 2905
			public GameObject go;

			// Token: 0x04000B5A RID: 2906
			public SpriteRenderer sr;

			// Token: 0x04000B5B RID: 2907
			public Vector2 center;

			// Token: 0x04000B5C RID: 2908
			public bool torn;
		}
	}
}
