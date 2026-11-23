using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using LewdStatus;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ScavArmory
{
	// Token: 0x0200002A RID: 42
	internal static class Camps
	{
		// Token: 0x06000074 RID: 116 RVA: 0x00007A8C File Offset: 0x00005C8C
		public static bool IsGuard(Scavenger s)
		{
			return s && Camps.Current != null && Camps.Current.Guard == s && !Camps.Current.Betrayed;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00007ABE File Offset: 0x00005CBE
		public static bool IsKeeper(Scavenger s)
		{
			return s && Camps.Current != null && Camps.Current.Keeper == s && !Camps.Current.Betrayed;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00007AF0 File Offset: 0x00005CF0
		public static bool IsCampSide(Scavenger s)
		{
			return Camps.IsGuard(s) || Camps.IsKeeper(s);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00007B02 File Offset: 0x00005D02
		public static bool IsTraderBody(Scavenger s)
		{
			return s && Camps.Current != null && Camps.Current.Keeper == s;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00007B25 File Offset: 0x00005D25
		public static bool InQuiet(Vector2 p)
		{
			return Camps.Current != null && !Camps.Current.Betrayed && Vector2.Distance(p, Camps.Current.Center) < 25f;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00007B54 File Offset: 0x00005D54
		public static void Place(Body b)
		{
			Camps.Clear();
			WorldGeneration world = WorldGeneration.world;
			if (!world || !b || !Plugin.CampEnabled.Value || !Plugin.ScavEnabled.Value || Random.value >= Plugin.CampChance.Value)
			{
				return;
			}
			Array.Clear(Camps.rejected, 0, Camps.rejected.Length);
			Camps.wet = 0;
			for (int i = 0; i < 1500; i++)
			{
				Vector2? vector = Camps.Spot(world, b.transform.position);
				if (vector != null)
				{
					Vector2 valueOrDefault = vector.GetValueOrDefault();
					Camps.Build(valueOrDefault);
					return;
				}
			}
			Plugin.Log.LogInfo(string.Format("Neutral camp: no safe flat stretch of floor for it on this layer (turned down: {0} no level dry floor ({1} of them wet), ", Camps.rejected[0], Camps.wet) + string.Format("{0} near her, {1} raiders, {2} biomes, {3} elders, {4} the worm)", new object[]
			{
				Camps.rejected[1],
				Camps.rejected[2],
				Camps.rejected[3],
				Camps.rejected[4],
				Camps.rejected[5]
			}));
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00007C88 File Offset: 0x00005E88
		public static string DebugHere(Body b)
		{
			WorldGeneration world = WorldGeneration.world;
			Vector2Int vector2Int = world.WorldToBlockPos(b.transform.position);
			for (int i = 8; i <= 80; i += 2)
			{
				foreach (int num in new int[] { 1, -1 })
				{
					for (int k = 12; k >= -30; k -= 3)
					{
						Vector2? vector = Camps.FlatAt(world, vector2Int.x + num * i, vector2Int.y + k);
						if (vector != null)
						{
							Vector2 valueOrDefault = vector.GetValueOrDefault();
							Camps.Clear();
							Camps.Build(valueOrDefault);
							return string.Format("camp at {0} ({1:0} m from her)", valueOrDefault, Vector2.Distance(valueOrDefault, b.transform.position));
						}
					}
				}
			}
			return "no floor near her";
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00007D74 File Offset: 0x00005F74
		private static Vector2? Spot(WorldGeneration w, Vector2 her)
		{
			int num = Random.Range(12, (int)(w.width - 12U));
			int num2 = Random.Range(12, (int)(w.height - 12U));
			Vector2? vector = Camps.FlatAt(w, num, num2);
			if (vector == null)
			{
				Camps.rejected[0]++;
				return null;
			}
			Vector2 at = vector.Value;
			if (Vector2.Distance(at, her) < 50f)
			{
				Camps.rejected[1]++;
				return null;
			}
			if (Scavengers.All.Any<Scavenger>((Scavenger s) => s && !Camps.IsCampSide(s) && Vector2.Distance(s.pos, at) < 30f))
			{
				Camps.rejected[2]++;
				return null;
			}
			if (FleshZones.Near(at, 15f) || Webs.Near(at, 15f) || LustGas.Near(at, 15f))
			{
				Camps.rejected[3]++;
				return null;
			}
			if (Object.FindObjectsOfType<ElderThornbackBehaviour>().Any<ElderThornbackBehaviour>((ElderThornbackBehaviour e) => Vector2.Distance(e.transform.position, at) < 55f))
			{
				Camps.rejected[4]++;
				return null;
			}
			if (Worm.Beasts.Any<WormBeast>((WormBeast wb) => wb && Vector2.Distance(wb.transform.position, at) < 50f))
			{
				Camps.rejected[5]++;
				return null;
			}
			return vector;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00007EFC File Offset: 0x000060FC
		private static void Quiet(Vector2 at)
		{
			float num = 25f;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			foreach (Scavenger scavenger in Scavengers.All.ToArray())
			{
				if (scavenger && !Camps.IsCampSide(scavenger) && Vector2.Distance(scavenger.pos, at) < num)
				{
					if (scavenger.Holding && body)
					{
						Capture.Release(body, false, false);
					}
					Scavengers.All.Remove(scavenger);
					Object.Destroy(scavenger.gameObject);
					num2++;
				}
			}
			foreach (ParasiteAI parasiteAI in Parasites.Alive.ToArray())
			{
				if (parasiteAI && Vector2.Distance(parasiteAI.transform.position, at) < num)
				{
					Parasites.Alive.Remove(parasiteAI);
					Object.Destroy(parasiteAI.gameObject);
					num3++;
				}
			}
			foreach (TentacleTrap tentacleTrap in Tentacles.Traps.ToArray())
			{
				if (tentacleTrap && !Tentacles.HolderIs(tentacleTrap) && Vector2.Distance(tentacleTrap.transform.position, at) < num)
				{
					Tentacles.Traps.Remove(tentacleTrap);
					Object.Destroy(tentacleTrap.gameObject);
					num3++;
				}
			}
			foreach (FleshPatch fleshPatch in Flesh.Patches.ToArray())
			{
				if (fleshPatch && fleshPatch != Flesh.Holder && Vector2.Distance(fleshPatch.transform.position, at) < num)
				{
					Flesh.Patches.Remove(fleshPatch);
					Object.Destroy(fleshPatch.gameObject);
					num3++;
				}
			}
			foreach (LewdWall lewdWall in Walls.All.ToArray())
			{
				if (lewdWall && Vector2.Distance(lewdWall.Face, at) < num)
				{
					Walls.All.Remove(lewdWall);
					Object.Destroy(lewdWall.gameObject);
					num3++;
				}
			}
			num3 += Pits.RemoveNear(at, num);
			num3 += Slits.RemoveNear(at, num);
			foreach (Transform transform in Object.FindObjectsOfType<Transform>())
			{
				if (transform && Vector2.Distance(transform.position, at) < num)
				{
					string text = transform.name.Replace("(Clone)", "").Trim().ToLowerInvariant();
					if (Camps.GameHazards.Contains(text))
					{
						Object.Destroy(transform.gameObject);
						num4++;
					}
				}
			}
			Plugin.Log.LogInfo(string.Format("Neutral camp: its ground cleared of {0} raiders and hosts, {1} of the mod's traps and creatures, {2} of the game's traps", num2, num3, num4));
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00008210 File Offset: 0x00006410
		private static Vector2? FlatAt(WorldGeneration w, int x, int y)
		{
			if (w.GetBlock(new Vector2Int(x, y)) != 0)
			{
				return null;
			}
			int num = 0;
			while (num < 40 && w.GetBlock(new Vector2Int(x, y - 1)) == 0)
			{
				y--;
				num++;
			}
			if (w.GetBlock(new Vector2Int(x, y - 1)) == 0)
			{
				return null;
			}
			if (!Pits.Rough(new Vector2Int(x, y - 1), 5, 2, 6))
			{
				return null;
			}
			if (FluidManager.main)
			{
				for (int i = -6; i <= 6; i++)
				{
					for (int j = 0; j <= 3; j++)
					{
						if (FluidManager.main.GetLiquid(x + i, y + j) != 0)
						{
							Camps.wet++;
							return null;
						}
					}
				}
			}
			return new Vector2?(w.BlockToWorldPos(new Vector2Int(x, y)) + Vector2.down * 0.5f);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00008304 File Offset: 0x00006504
		private static void Build(Vector2 at)
		{
			WorldGeneration world = WorldGeneration.world;
			if (world)
			{
				Pits.Level(world.WorldToBlockPos(at + Vector2.down * 0.5f), 10, 6);
			}
			Camp camp = new Camp
			{
				Center = at,
				LookKind = ((Camps.ForceLook >= 0) ? (Camps.ForceLook % Camps.LookNames.Length) : Random.Range(0, Camps.LookNames.Length))
			};
			Camps.Current = camp;
			camp.Tent = new GameObject("NeutralCampTent");
			camp.Tent.transform.position = at + new Vector2(-3.2f, 0f);
			SpriteRenderer spriteRenderer = camp.Tent.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = Camps.TentSprite();
			if (WorldGeneration.world)
			{
				spriteRenderer.sharedMaterial = WorldGeneration.world.defaultMat;
			}
			spriteRenderer.sortingOrder = -700;
			Camps.Dressing(camp);
			GameObject gameObject = Resources.Load("campfire") as GameObject;
			if (gameObject)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, at + new Vector2(0.4f, 0.6f), Quaternion.identity);
				camp.Fire = gameObject2.GetComponent<Item>();
				foreach (Heater heater in gameObject2.GetComponentsInChildren<Heater>(true))
				{
					Plugin.Log.LogInfo(string.Format("Neutral camp: the campfire's heater {0:0.0}° within {1:0.0} m (strength {2:0.00}) -> 37.5° within 7 m", heater.desiredTemp, heater.maxDistance, heater.tempLerpStrength));
					heater.desiredTemp = Mathf.Min(heater.desiredTemp, 37.5f);
					heater.maxDistance = Mathf.Min(heater.maxDistance, 7f);
				}
				if (!gameObject2.GetComponentInChildren<Heater>(true))
				{
					Heater heater2 = gameObject2.AddComponent<Heater>();
					heater2.maxDistance = 6f;
					heater2.desiredTemp = 37.5f;
					heater2.tempLerpStrength = 0.06f;
				}
			}
			camp.TraderFeet = at + new Vector2(-1.6f, 0f);
			GameObject gameObject3 = Resources.Load("trader" + Random.Range(1, 4).ToString()) as GameObject;
			if (gameObject3)
			{
				GameObject gameObject4 = Object.Instantiate<GameObject>(gameObject3, camp.TraderFeet + Vector2.up * 3f, Quaternion.identity);
				camp.Trader = gameObject4.GetComponent<TraderScript>();
				Camps.Dress(camp);
				TraderScript trader = camp.Trader;
				RangeF rangeF = default(RangeF);
				rangeF.min = camp.TraderFeet.x - 6f - 2f;
				rangeF.max = camp.TraderFeet.x + 6f + 2f;
				trader.MoveRange = rangeF;
				if (camp.Build)
				{
					camp.Build.cantHit = false;
					camp.LastHealth = camp.Build.health;
				}
			}
			Vector2 vector = Scavengers.GroundNear(at + new Vector2(3f, 0.5f)) ?? (at + new Vector2(3f, 0f));
			WeaponDef weaponDef = null;
			int num = 0;
			while (num < 10 && (weaponDef == null || !weaponDef.Gun))
			{
				weaponDef = Scavengers.PickWeapon(WorldGeneration.world ? WorldGeneration.world.biomeDepth : 0);
				num++;
			}
			if (weaponDef == null || !weaponDef.Gun)
			{
				weaponDef = Scavengers.Weapons.First<WeaponDef>((WeaponDef x) => x.Id == "pistol");
			}
			camp.Guard = Scavenger.Make(vector, weaponDef, Random.value < 0.5f, null, false, ScavArt.RandomSpacesuit());
			camp.Guard.MakeGuard(at);
			if (camp.Trader)
			{
				WeaponDef weaponDef2 = Scavengers.Weapons.First<WeaponDef>((WeaponDef x) => x.Id == "pistol");
				Vector2 vector2 = Scavengers.GroundNear(camp.TraderFeet + Vector2.up * 0.5f) ?? camp.TraderFeet;
				camp.Keeper = Scavenger.Make(vector2, weaponDef2, Random.value < 0.5f, null, false, ScavArt.RandomSpacesuit());
				camp.Keeper.MakeGuard(camp.TraderFeet);
			}
			Camps.Quiet(at);
			Plugin.Log.LogInfo(string.Format("Neutral camp at {0} ({1}): trader {2} as {3}, guard {4} with {5}, fire {6}", new object[]
			{
				at,
				Camps.LookNames[camp.LookKind],
				camp.Trader ? camp.Trader.character.ToString() : "none",
				camp.Keeper ? camp.Keeper.skin : "-",
				camp.Guard.skin,
				weaponDef.Id,
				camp.Fire ? "lit" : "none"
			}));
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00008860 File Offset: 0x00006A60
		private static void Dress(Camp c)
		{
			TraderScript trader = c.Trader;
			foreach (Transform transform in new Transform[] { trader.head, trader.torso, trader.lArm, trader.rArm, trader.lThigh, trader.rThigh, trader.lFoot, trader.rFoot })
			{
				if (transform)
				{
					SpriteRenderer[] componentsInChildren = transform.GetComponentsInChildren<SpriteRenderer>(true);
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						componentsInChildren[j].enabled = false;
					}
				}
			}
			if (trader.eyes)
			{
				trader.eyes.enabled = false;
			}
			if (trader.headRender)
			{
				trader.headRender.enabled = false;
			}
			float num = (from f in new Transform[] { trader.lFoot, trader.rFoot }
				where f
				select f.position.y).DefaultIfEmpty(trader.transform.position.y).Min();
			trader.transform.position += new Vector3(0f, c.TraderFeet.y - num + 0.3f, 0f);
			c.LookOffset = c.TraderFeet - trader.transform.position;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00008A0D File Offset: 0x00006C0D
		private static Sprite TentSprite()
		{
			if (!Camps.tentSprite)
			{
				Camps.tentSprite = Camps.PropSprite("tent");
			}
			return Camps.tentSprite;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00008A30 File Offset: 0x00006C30
		private static Sprite PropSprite(string name)
		{
			Sprite sprite;
			if (Camps.props.TryGetValue(name, out sprite) && sprite)
			{
				return sprite;
			}
			string text = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "camp", name + ".png");
			Texture2D texture2D = new Texture2D(2, 2, 4, false)
			{
				filterMode = 0,
				wrapMode = 1
			};
			if (!File.Exists(text) || !ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text)))
			{
				Plugin.Log.LogWarning("Neutral camp: no picture at " + text);
				return null;
			}
			sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0f), 8f);
			Camps.props[name] = sprite;
			return sprite;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00008B10 File Offset: 0x00006D10
		private static void Dressing(Camp c)
		{
			WorldGeneration world = WorldGeneration.world;
			if (!world)
			{
				return;
			}
			Vector2Int vector2Int = world.WorldToBlockPos(c.Center + Vector2.up * 0.5f);
			switch (c.LookKind)
			{
			case 1:
			{
				for (int i = 0; i < 3; i++)
				{
					for (int j = i; j < 5 - i; j++)
					{
						world.SetBlock(new Vector2Int(vector2Int.x + 5 + j, vector2Int.y + i), 24);
					}
				}
				Camps.Prop(c, "block", 3.4f, true);
				return;
			}
			case 2:
			{
				for (int k = -7; k <= 7; k++)
				{
					world.SetBlock(new Vector2Int(vector2Int.x + k, vector2Int.y + 6), 11);
				}
				for (int l = -7; l <= 7; l += 14)
				{
					world.SetBlock(new Vector2Int(vector2Int.x + l, vector2Int.y + 6 + 1), 24);
				}
				Camps.Post(c, vector2Int, -7, 6);
				Camps.Post(c, vector2Int, 7, 6);
				return;
			}
			case 3:
				Camps.Shuttle(c, vector2Int, false);
				return;
			case 4:
				Camps.Shuttle(c, vector2Int, true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00008C4C File Offset: 0x00006E4C
		private static void Prop(Camp c, string name, float x, bool front)
		{
			Sprite sprite = Camps.PropSprite(name);
			if (!sprite)
			{
				return;
			}
			GameObject gameObject = new GameObject("NeutralCamp_" + name);
			gameObject.transform.position = c.Center + new Vector2(x, 0f);
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprite;
			if (WorldGeneration.world)
			{
				spriteRenderer.sharedMaterial = WorldGeneration.world.defaultMat;
			}
			spriteRenderer.sortingOrder = (front ? (-680) : (-710));
			c.Props.Add(gameObject);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00008CEC File Offset: 0x00006EEC
		private static void Post(Camp c, Vector2Int f, int x, int h)
		{
			WorldGeneration world = WorldGeneration.world;
			Tile tile = world.tiles[24] as Tile;
			if (!tile || !tile.sprite)
			{
				return;
			}
			for (int i = 0; i < h; i++)
			{
				GameObject gameObject = new GameObject("NeutralCamp_post");
				gameObject.transform.position = world.BlockToWorldPos(new Vector2Int(f.x + x, f.y + i));
				SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = tile.sprite;
				spriteRenderer.sharedMaterial = world.defaultMat;
				spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f);
				spriteRenderer.sortingOrder = -705;
				c.Props.Add(gameObject);
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00008DBC File Offset: 0x00006FBC
		private static void NoDoors(GameObject root)
		{
			List<string> list = new List<string>();
			foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
			{
				if (transform && !(transform == root.transform))
				{
					string text = transform.name.ToLowerInvariant();
					if (transform.GetComponent<Joint2D>())
					{
						goto IL_0072;
					}
					Rigidbody2D component = transform.GetComponent<Rigidbody2D>();
					if (component != null && component.bodyType != 2)
					{
						goto IL_0072;
					}
					bool flag = transform.GetComponent<Animator>();
					IL_0073:
					bool flag2 = flag;
					list.Add(transform.name + "[" + string.Join(",", from k in transform.GetComponents<Component>()
						where k && !(k is Transform)
						select k.GetType().Name) + "]");
					if (text.Contains("door") || text.Contains("hatch") || flag2)
					{
						Plugin.Log.LogInfo("Neutral camp: the shuttle's " + transform.name + " taken off (a door, or it moves)");
						Object.Destroy(transform.gameObject);
						goto IL_0134;
					}
					goto IL_0134;
					IL_0072:
					flag = true;
					goto IL_0073;
				}
				IL_0134:;
			}
			Plugin.Log.LogInfo("Neutral camp: the shuttle's fittings: " + string.Join(" ", list));
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00008F2C File Offset: 0x0000712C
		private static void Shuttle(Camp c, Vector2Int f, bool fallen)
		{
			Camps.<>c__DisplayClass33_0 CS$<>8__locals1;
			CS$<>8__locals1.fallen = fallen;
			WorldGeneration world = WorldGeneration.world;
			GameObject gameObject = Resources.Load<GameObject>("Lifepod");
			GameObject gameObject2 = Resources.Load<GameObject>("LifepodStart");
			Tilemap tilemap = (gameObject ? gameObject.transform.GetChild(0).GetComponent<Tilemap>() : null);
			if (!tilemap)
			{
				Plugin.Log.LogWarning("Neutral camp: no lifepod to make a home of");
				return;
			}
			CS$<>8__locals1.o = (CS$<>8__locals1.fallen ? new Vector2Int(f.x - 22, f.y + 5) : new Vector2Int(f.x + 13, f.y + 7));
			Random random = new Random(f.x * 31 + f.y);
			BoundsInt cellBounds = tilemap.cellBounds;
			int num = 0;
			for (int i = cellBounds.xMin; i < cellBounds.xMax; i++)
			{
				for (int j = cellBounds.yMin; j < cellBounds.yMax; j++)
				{
					TileBase tile = tilemap.GetTile(new Vector3Int(i, j, 0));
					if (tile)
					{
						Vector2Int vector2Int = Camps.<Shuttle>g__At|33_0(i, j, ref CS$<>8__locals1);
						if (tile == world.specialNullTile)
						{
							world.SetBlock(vector2Int, 0);
						}
						else
						{
							int num2 = Array.IndexOf<TileBase>(world.tiles, tile);
							if (num2 >= 0)
							{
								bool flag = (CS$<>8__locals1.fallen ? (i == -6) : (j == -8));
								double num3 = random.NextDouble();
								if (!flag && num3 < 0.09)
								{
									world.SetBlock(vector2Int, 0);
									num++;
								}
								else
								{
									if (num2 == 10 && num3 < 0.33)
									{
										num2 = 3;
									}
									else if (num2 == 6 && num3 < 0.35)
									{
										num2 = ((num3 < 0.2) ? 3 : 11);
									}
									else if (num2 == 9 && num3 < 0.6)
									{
										num2 = 11;
									}
									else if (num2 == 8 && num3 < 0.45)
									{
										num2 = 24;
									}
									else if (num2 == 7 && num3 < 0.5 && !flag)
									{
										world.SetBlock(vector2Int, 0);
										goto IL_0220;
									}
									world.SetBlock(vector2Int, (ushort)num2);
								}
							}
						}
					}
					IL_0220:;
				}
			}
			if (CS$<>8__locals1.fallen)
			{
				for (int k = -13; k <= -8; k++)
				{
					for (int l = -5; l <= -1; l++)
					{
						world.SetBlock(Camps.<Shuttle>g__At|33_0(l, k, ref CS$<>8__locals1), 0);
					}
				}
			}
			else
			{
				for (int m = -7; m <= -3; m++)
				{
					world.SetBlock(Camps.<Shuttle>g__At|33_0(-6, m, ref CS$<>8__locals1), 0);
				}
			}
			float num4 = (CS$<>8__locals1.fallen ? 90f : 0f);
			Vector2 vector = world.BlockToWorldPos(CS$<>8__locals1.o);
			Quaternion quaternion = Quaternion.Euler(0f, 0f, num4);
			if (gameObject2 && gameObject2.transform.childCount > 0 && gameObject2.transform.GetChild(0).childCount > 0)
			{
				GameObject gameObject3 = Object.Instantiate<GameObject>(gameObject2.transform.GetChild(0).GetChild(0).gameObject, vector, quaternion);
				c.Props.Add(gameObject3);
				Camps.NoDoors(gameObject3);
			}
			foreach (object obj in gameObject.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.GetComponent<Item>() || transform.GetComponent<BuildingEntity>() || transform.gameObject.name == "DOSPAWN")
				{
					GameObject gameObject4 = Object.Instantiate<GameObject>(transform.gameObject, vector - Vector2.one * 0.5f + quaternion * transform.localPosition, quaternion * transform.localRotation);
					c.Props.Add(gameObject4);
					Camps.NoDoors(gameObject4);
				}
			}
			Camps.Prop(c, "crates", CS$<>8__locals1.fallen ? (-7.5f) : 4.6f, true);
			Plugin.Log.LogInfo(string.Format("Neutral camp: the trader's home is a wrecked shuttle {0} ({1} holes in its hull)", CS$<>8__locals1.fallen ? "on its side" : "standing", num));
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000093BC File Offset: 0x000075BC
		public static void Clear()
		{
			Camp current = Camps.Current;
			Camps.Current = null;
			if (current == null)
			{
				return;
			}
			if (current.Trader)
			{
				Object.Destroy(current.Trader.gameObject);
			}
			if (current.Fire)
			{
				Object.Destroy(current.Fire.gameObject);
			}
			if (current.Tent)
			{
				Object.Destroy(current.Tent);
			}
			foreach (GameObject gameObject in current.Props)
			{
				if (gameObject)
				{
					Object.Destroy(gameObject);
				}
			}
			if (current.Look)
			{
				Object.Destroy(current.Look);
			}
			if (current.Keeper)
			{
				Object.Destroy(current.Keeper.gameObject);
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000094B0 File Offset: 0x000076B0
		public static void Update(Body b, float dt)
		{
			Camp current = Camps.Current;
			if (current == null)
			{
				return;
			}
			Camps.Creatures(b, dt);
			Camps.Care(b, current, dt);
			if (current.Fire && !current.Betrayed && current.Fire.transform.parent == null && Vector2.Distance(current.Fire.transform.position, current.Center) < 4f)
			{
				current.Fire.condition = 1f;
			}
			if (current.Look)
			{
				if (!current.Trader)
				{
					Object.Destroy(current.Look);
					current.Look = null;
					return;
				}
				current.Look.transform.position = current.Trader.transform.position + current.LookOffset;
				if (!current.Dead)
				{
					if (b)
					{
						current.Look.transform.localScale = new Vector3((b.transform.position.x >= current.Look.transform.position.x) ? 1f : (-1f), 1f, 1f);
					}
					if ((current.Blink -= dt) <= 0f)
					{
						current.Blink = Random.Range(2.5f, 5f);
					}
					current.Rig.Blink(current.Blink < 0.12f);
				}
			}
			if (!current.Betrayed && current.Trader && current.Trader.hostile)
			{
				Camps.Betray(b, "the trader turned on her");
			}
			BuildingEntity build = current.Build;
			if (current.Keeper && current.Trader && !current.Dead)
			{
				Vector3 vector = current.Keeper.pos - current.LookOffset;
				current.Trader.transform.position = vector;
				Traverse.Create(current.Trader).Field("desiredPos").SetValue(vector);
				current.TraderFeet = current.Keeper.pos;
				if (build)
				{
					build.health = current.LastHealth;
				}
				return;
			}
			if (build && !current.Dead)
			{
				build.cantHit = false;
				current.HurtByOther = Mathf.Max(0f, current.HurtByOther - dt);
				if (build.health < current.LastHealth - 0.5f && current.HurtByOther <= 0f && b && Vector2.Distance(b.transform.position, current.Trader.transform.position) < 30f)
				{
					Camps.Betray(b, "she hit the trader");
				}
				current.LastHealth = build.health;
				if (build.health < 200f)
				{
					Camps.Died(current);
				}
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000097CD File Offset: 0x000079CD
		public static void GuardFired()
		{
			if (Camps.Current != null)
			{
				Camps.Current.HurtByOther = Mathf.Max(Camps.Current.HurtByOther, 0.6f);
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000097F4 File Offset: 0x000079F4
		public static bool Prey(Vector2 from, float range, out Vector2 at, out bool trader)
		{
			at = Vector2.zero;
			trader = false;
			Camp current = Camps.Current;
			if (current == null || current.Betrayed)
			{
				return false;
			}
			float num = range;
			bool flag = false;
			if (current.Guard && current.Guard.hit && current.Guard.hit.health > 0f && Vector2.Distance(from, current.Guard.Chest) < num)
			{
				num = Vector2.Distance(from, current.Guard.Chest);
				at = current.Guard.Chest;
				flag = true;
			}
			Vector2 vector;
			if (Camps.TraderTarget(out vector) && Vector2.Distance(from, vector) < num)
			{
				at = vector;
				trader = true;
				flag = true;
			}
			return flag;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000098B8 File Offset: 0x00007AB8
		public static void CreatureBite(bool trader, float dmg, string by)
		{
			Camp current = Camps.Current;
			if (current == null)
			{
				return;
			}
			if (trader)
			{
				Camps.HurtTrader(dmg);
			}
			else if (current.Guard && current.Guard.hit)
			{
				Scavenger.CreatureHit = true;
				try
				{
					current.Guard.hit.health -= dmg;
					current.Guard.hit.gameObject.SendMessage("AnimalHit", dmg, 1);
				}
				finally
				{
					Scavenger.CreatureHit = false;
				}
			}
			Plugin.Log.LogInfo(string.Format("Neutral camp: {0} bites the {1} ({2:0})", by, trader ? "trader" : "guard", dmg));
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000997C File Offset: 0x00007B7C
		private static void Creatures(Body b, float dt)
		{
			if ((Camps.creatureScan -= dt) > 0f)
			{
				return;
			}
			Camps.creatureScan = 0.35f;
			Camp current = Camps.Current;
			if (current == null || current.Betrayed)
			{
				return;
			}
			foreach (SpiderHandler spiderHandler in Object.FindObjectsOfType<SpiderHandler>())
			{
				if (spiderHandler && Vector2.Distance(spiderHandler.transform.position, current.Center) <= 50f)
				{
					Vector2 vector = spiderHandler.transform.position;
					Vector2 vector2;
					bool flag;
					if (Camps.Prey(vector, Mathf.Min(8f, spiderHandler.seeDistance), out vector2, out flag) && (!b || !b.alive || Vector2.Distance(vector, b.transform.position) >= Vector2.Distance(vector, vector2)))
					{
						spiderHandler.target = vector2;
						float num;
						if (Vector2.Distance(vector, vector2) <= 2.4f && (!Camps.biteReady.TryGetValue(spiderHandler, out num) || Time.time >= num))
						{
							Camps.biteReady[spiderHandler] = Time.time + Mathf.Max(1.5f, spiderHandler.biteCoolToSet);
							if (spiderHandler.attackSounds != null && spiderHandler.attackSounds.Count > 0)
							{
								Sound.Play(spiderHandler.attackSounds[Random.Range(0, spiderHandler.attackSounds.Count)], vector, false, true, null, 1f, 1f, false, false);
							}
							bool flag2 = spiderHandler.GetComponent<ElderThornbackBehaviour>();
							Camps.CreatureBite(flag, flag2 ? Random.Range(45f, 70f) : Mathf.Clamp((spiderHandler.biteSkinDamage + spiderHandler.biteMuscleDamage) * 0.6f, 10f, 35f), flag2 ? "an elder" : spiderHandler.name.Replace("(Clone)", ""));
						}
					}
				}
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00009B7C File Offset: 0x00007D7C
		private static void Care(Body b, Camp c, float dt)
		{
			if (!b || !b.alive || c.Betrayed || c.Dead || !c.Keeper || c.Keeper.mode == Scavenger.Mode.Dead)
			{
				return;
			}
			Vector2 vector = b.transform.position;
			bool flag = Vector2.Distance(vector, c.Center) < 9f && NudeMood.Covered(b) == NudeMood.Cover.Naked;
			c.NakedFor = (flag ? (c.NakedFor + dt) : 0f);
			if (c.NakedFor > 2f && Time.time > c.ClothesAt)
			{
				c.ClothesAt = Time.time + 300f;
				string text = ((Random.value < 0.65f) ? "tornshirt" : "hoodie");
				GameObject gameObject = null;
				try
				{
					gameObject = Utils.Create(text, vector + Vector2.up * 0.6f + Vector2.right * ((c.Keeper.pos.x > vector.x) ? 0.5f : (-0.5f)), 0f);
				}
				catch
				{
				}
				Item item = (gameObject ? gameObject.GetComponent<Item>() : null);
				if (item)
				{
					item.SetCondition(Random.Range(0.3f, 0.6f));
				}
				bool flag2 = NudeMood.Tier >= 2;
				c.Keeper.Bark(flag2 ? "camp_clothes_smirk" : "camp_clothes", true);
				LewdLink.Say(b, flag2 ? "camp_got_clothes_likes" : "camp_got_clothes");
				Plugin.Log.LogInfo(string.Format("Neutral camp: she is naked by the fire, the trader gives her a worn {0} ({1:P0})", text, item ? item.condition : 0f));
			}
			if ((b.limbs.Any<Limb>((Limb l) => l && !l.dismembered && l.bleedAmount > 0.6f) || b.internalBleeding > 4f) && Time.time > c.HealAt && !c.Keeper.Tending && Vector2.Distance(vector, c.Keeper.pos) < 8f)
			{
				c.HealAt = Time.time + 10f;
				c.Keeper.Bark("camp_heal", true);
				c.Keeper.Tend(3.5f, delegate
				{
					Camps.Dress(b, c);
				});
				Plugin.Log.LogInfo("Neutral camp: the trader sees her bleeding and comes to dress her wounds");
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00009EC0 File Offset: 0x000080C0
		private static void Dress(Body b, Camp c)
		{
			if (!b || !b.alive || c == null)
			{
				return;
			}
			Vector2 vector = b.transform.position;
			c.HealAt = Time.time + 60f;
			int num = 0;
			using (List<Limb>.Enumerator enumerator = b.limbs.Where<Limb>((Limb x) => x && !x.dismembered && x.bleedAmount > 0.6f).ToList<Limb>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Limb l = enumerator.Current;
					if (Camps.ArmsAndLegs.Any<string>((string p) => l.name.StartsWith(p)) && l.bleedAmount > 3f && !l.GetComponent<TourniquetScript>())
					{
						l.gameObject.AddComponent<TourniquetScript>().condition = 1f;
					}
					else
					{
						l.bandageSlowAmount += 10f;
						l.skinHealAmount += 4f;
						l.pain = Mathf.Max(0f, l.pain - 10f);
						l.bleedAmount *= 0.3f;
						l.CreateTemporarySprite(Resources.Load<Sprite>("Special/bandageWrap"), 0f, new Color?(new Color32(143, 126, 139, byte.MaxValue)), true, 1000f, null);
					}
					num++;
				}
			}
			float internalBleeding = b.internalBleeding;
			b.internalBleeding *= 0.1f;
			Sound.Play("splint", vector, false, true, null, 1f, 1f, false, false);
			LewdLink.Say(b, "camp_healed");
			Plugin.Log.LogInfo(string.Format("Neutral camp: the trader dresses her wounds for free ({0} dressed, internal bleeding {1:0.0} -> {2:0.0})", num, internalBleeding, b.internalBleeding));
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000A10C File Offset: 0x0000830C
		public static bool TraderTarget(out Vector2 chest)
		{
			Camp current = Camps.Current;
			chest = ((current != null && current.Keeper) ? current.Keeper.Chest : ((current != null && current.Trader) ? (current.TraderFeet + Vector2.up * 3f) : Vector2.zero));
			return current != null && current.Trader && !current.Dead && current.Build && current.Build.health >= 200f;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000A1B0 File Offset: 0x000083B0
		public static void HurtTrader(float dmg)
		{
			Camp current = Camps.Current;
			BuildingEntity buildingEntity = ((current != null) ? current.Build : null);
			if (buildingEntity == null || current.Dead)
			{
				return;
			}
			current.HurtByOther = 1.5f;
			if (buildingEntity.health - dmg < 200f)
			{
				Traverse traverse = Traverse.Create(current.Trader);
				if (!traverse.Field("didDeathMoodDebuff").GetValue<bool>())
				{
					traverse.Field("didDeathMoodDebuff").SetValue(true);
					try
					{
						traverse.Method("DropInventory", Array.Empty<object>()).GetValue();
					}
					catch (Exception ex)
					{
						Plugin.Log.LogWarning("Trader's goods: " + ex.Message);
					}
					buildingEntity.description = Locale.GetBuilding("traderdscdead");
					buildingEntity.fullName = Locale.GetBuilding("corpseseen");
					UsableObject component = current.Trader.GetComponent<UsableObject>();
					if (component)
					{
						Object.Destroy(component);
					}
				}
			}
			buildingEntity.health -= dmg;
			current.LastHealth = buildingEntity.health;
			current.Trader.AnimalHit();
			if (buildingEntity.health < 200f)
			{
				Camps.Died(current);
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000A2F0 File Offset: 0x000084F0
		public static void TraderSwallowed()
		{
			Camp current = Camps.Current;
			if (current == null || current.Dead)
			{
				return;
			}
			Camps.HurtTrader((current.Build ? current.Build.health : 0f) + 1f);
			if (!current.Dead)
			{
				Camps.Died(current);
			}
			if (current.Look)
			{
				Object.Destroy(current.Look);
				current.Look = null;
			}
			if (current.Trader)
			{
				current.Trader.gameObject.SetActive(false);
			}
			Plugin.Log.LogInfo("Neutral camp: the giant worm swallowed the trader");
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000A394 File Offset: 0x00008594
		public static void KeeperDied(bool byHer)
		{
			Camp current = Camps.Current;
			if (current == null || current.Dead)
			{
				return;
			}
			BuildingEntity build = current.Build;
			if (build)
			{
				Camps.HurtTrader(build.health);
			}
			else
			{
				Camps.Died(current);
			}
			if (byHer && !current.Betrayed)
			{
				current.Betrayed = true;
			}
			Plugin.Log.LogInfo("Neutral camp: the trader was killed (" + (byHer ? "by her" : "not by her") + ")");
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000A410 File Offset: 0x00008610
		private static void Died(Camp c)
		{
			if (c.Dead)
			{
				return;
			}
			c.Dead = true;
			if (c.Look)
			{
				c.Look.transform.rotation = Quaternion.Euler(0f, 0f, (c.Look.transform.localScale.x > 0f) ? 90f : (-90f));
				c.LookOffset += Vector3.up * 0.4f;
				c.Rig.Blink(true);
			}
			if (c.Guard && c.Guard.mode != Scavenger.Mode.Dead)
			{
				c.Guard.Bark("camp_trader_dead", true);
			}
			Plugin.Log.LogInfo("Neutral camp: the trader is dead (" + ((c.HurtByOther > 0f) ? "a creature" : "her doing") + ")");
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000A50C File Offset: 0x0000870C
		public static void Betray(Body b, string why)
		{
			Camp current = Camps.Current;
			if (current == null || current.Betrayed)
			{
				return;
			}
			current.Betrayed = true;
			if (current.Guard && current.Guard.mode != Scavenger.Mode.Dead)
			{
				current.Guard.Bark("camp_betrayed", true);
				if (b)
				{
					current.Guard.TurnOn(b);
				}
			}
			if (current.Keeper && current.Keeper.mode != Scavenger.Mode.Dead && b)
			{
				current.Keeper.TurnOn(b);
			}
			if (current.Trader && !current.Trader.hostile)
			{
				Traverse.Create(current.Trader).Field("hostility").SetValue(100f);
			}
			Plugin.Log.LogInfo("Neutral camp: the peace is over (" + why + ")");
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000A5FC File Offset: 0x000087FC
		public static bool Propose(Body b)
		{
			Camp current = Camps.Current;
			if (current == null || current.Betrayed || !b || !b.alive || !current.Guard || current.Guard.mode == Scavenger.Mode.Dead || !current.Trader)
			{
				return false;
			}
			if (Vector2.Distance(b.transform.position, current.Center) > 12f)
			{
				return false;
			}
			if (!LewdLink.Present || Capture.Active || Scenes.Busy)
			{
				return false;
			}
			Item item = Devices.BeltOn(b);
			if (item && Devices.IsLocked(item))
			{
				b.talker.Talk(Plugin.English ? "I've no money... but this belt..." : "Денег нет... а этот пояс...", null, false, false);
				current.Trader.talker.Talk(Plugin.English ? "Locked up tight? Bring a key, then we'll talk." : "Заперта на железо? Найди ключ — тогда поговорим.", null, false, false);
				return true;
			}
			int num = TraderScript.MAX_VALUE_GIVEN - current.Trader.totalValueGiven;
			b.talker.Talk(Plugin.English ? "I've no money... Will you take me instead?" : "Денег нет... Возьмёшь меня вместо них?", null, false, false);
			if (num <= 0)
			{
				current.Trader.talker.Talk(Plugin.English ? "No more trade today. You've given enough." : "Хватит на сегодня. Ты и так дала достаточно.", null, false, false);
				return true;
			}
			current.Trader.talker.Talk(Plugin.English ? "Deal. He'll see to you, I'll put it on your account." : "Договорились. Он тобой займётся, а я запишу на твой счёт.", null, false, false);
			Deal.ForCamp(b, current.Guard);
			Plugin.Log.LogInfo("Neutral camp: she pays the trader with herself, the guard takes her");
			return true;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x0000A790 File Offset: 0x00008990
		public static void Pay(Body b)
		{
			Camp current = Camps.Current;
			if (current == null || !current.Trader)
			{
				return;
			}
			int num = Mathf.Min(Mathf.Max(1, Plugin.CampPayValue.Value), TraderScript.MAX_VALUE_GIVEN - current.Trader.totalValueGiven);
			if (num <= 0)
			{
				return;
			}
			current.Trader.valueGiven = Mathf.Min(TraderScript.MAX_VALUE_GIVEN, current.Trader.valueGiven + num);
			current.Trader.totalValueGiven += num;
			current.Trader.reputation += 5f;
			current.Trader.talker.Talk(Plugin.English ? "Paid in full. Take what you need." : "Расплатилась сполна. Бери, что нужно.", null, false, false);
			if (PlayerCamera.main)
			{
				PlayerCamera.main.UpdateTradeTexts();
			}
			Plugin.Log.LogInfo(string.Format("Neutral camp: the trader counts {0} as given ({1} to spend)", num, current.Trader.valueGiven));
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000A9AC File Offset: 0x00008BAC
		[CompilerGenerated]
		internal static Vector2Int <Shuttle>g__At|33_0(int x, int y, ref Camps.<>c__DisplayClass33_0 A_2)
		{
			if (!A_2.fallen)
			{
				return new Vector2Int(A_2.o.x + x, A_2.o.y + y);
			}
			return new Vector2Int(A_2.o.x - y, A_2.o.y + x);
		}

		// Token: 0x040000AC RID: 172
		public static Camp Current;

		// Token: 0x040000AD RID: 173
		private static Sprite tentSprite;

		// Token: 0x040000AE RID: 174
		public const string Credit = "__camp";

		// Token: 0x040000AF RID: 175
		private const float KeeperReach = 6f;

		// Token: 0x040000B0 RID: 176
		public const float QuietRadius = 25f;

		// Token: 0x040000B1 RID: 177
		private static readonly int[] rejected = new int[6];

		// Token: 0x040000B2 RID: 178
		private static int wet;

		// Token: 0x040000B3 RID: 179
		private static readonly string[] GameHazards = new string[]
		{
			"beartrap", "barbedwirefence", "caveticks", "coil", "grabberplant", "landmine", "overgrowntick", "shadecrawler", "soundcannon", "spikestabber",
			"thornbackyoung", "wallbiter", "spentfuel"
		};

		// Token: 0x040000B4 RID: 180
		private static readonly Dictionary<string, Sprite> props = new Dictionary<string, Sprite>();

		// Token: 0x040000B5 RID: 181
		private static readonly string[] LookNames = new string[] { "plain", "woodpile", "wooden roof", "shuttle standing", "shuttle on its side" };

		// Token: 0x040000B6 RID: 182
		private const ushort Planks = 11;

		// Token: 0x040000B7 RID: 183
		private const ushort Logs = 24;

		// Token: 0x040000B8 RID: 184
		private const ushort Scrap = 3;

		// Token: 0x040000B9 RID: 185
		private const ushort Trash = 4;

		// Token: 0x040000BA RID: 186
		public static int ForceLook = -1;

		// Token: 0x040000BB RID: 187
		private static readonly Dictionary<SpiderHandler, float> biteReady = new Dictionary<SpiderHandler, float>();

		// Token: 0x040000BC RID: 188
		private static float creatureScan;

		// Token: 0x040000BD RID: 189
		private static readonly string[] ArmsAndLegs = new string[] { "UpArm", "DownArm", "Hand", "Thigh", "Crus", "Foot" };
	}
}
