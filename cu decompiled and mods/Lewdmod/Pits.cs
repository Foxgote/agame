using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000D8 RID: 216
	internal static class Pits
	{
		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060004A7 RID: 1191 RVA: 0x0004AE42 File Offset: 0x00049042
		// (set) Token: 0x060004A8 RID: 1192 RVA: 0x0004AE49 File Offset: 0x00049049
		public static Pit Holder { get; private set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060004A9 RID: 1193 RVA: 0x0004AE51 File Offset: 0x00049051
		public static bool InMaw
		{
			get
			{
				return Pits.Holder != null;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060004AA RID: 1194 RVA: 0x0004AE5B File Offset: 0x0004905B
		public static bool Hard
		{
			get
			{
				return Pits.Holder != null && (Pits.Holder.Ravish || Pits.timeIn > Plugin.PitDigestAfter.Value);
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060004AB RID: 1195 RVA: 0x0004AE85 File Offset: 0x00049085
		// (set) Token: 0x060004AC RID: 1196 RVA: 0x0004AE8C File Offset: 0x0004908C
		public static bool InThroat { get; private set; }

		// Token: 0x060004AD RID: 1197 RVA: 0x0004AE94 File Offset: 0x00049094
		public static bool InSlimePit(Body b)
		{
			return b && Pits.W && Pits.All.Any<Pit>((Pit p) => p.Open && p.Kind == PitKind.Slime && Pits.InPit(p, b.limbs[1].transform.position, 1.5f));
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060004AE RID: 1198 RVA: 0x0004AEDF File Offset: 0x000490DF
		public static bool Ravishing
		{
			get
			{
				return Pits.Holder != null && Pits.Holder.Ravish;
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060004AF RID: 1199 RVA: 0x0004AEF4 File Offset: 0x000490F4
		// (set) Token: 0x060004B0 RID: 1200 RVA: 0x0004AEFB File Offset: 0x000490FB
		public static int Orgasms { get; private set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060004B1 RID: 1201 RVA: 0x0004AF03 File Offset: 0x00049103
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x0004AF0A File Offset: 0x0004910A
		private static WorldGeneration W
		{
			get
			{
				return WorldGeneration.world;
			}
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x0004AF14 File Offset: 0x00049114
		public static void SeedLayer(Body b)
		{
			if (!Pits.W)
			{
				return;
			}
			int num = Diff.Count(Plugin.PitsPerLayer.Value);
			Pits.FindHeavy();
			int num2 = 0;
			int num3 = 0;
			while (num3 < 12000 && num2 < num)
			{
				Vector2Int vector2Int;
				vector2Int..ctor(Random.Range(10, (int)(Pits.W.width - 10U)), Random.Range(10, (int)(Pits.W.height - 10U)));
				if (Pits.W.GetBlock(vector2Int) == 0)
				{
					int num4 = 0;
					while (num4 < 40 && vector2Int.y > 2 && Pits.W.GetBlock(vector2Int) == 0)
					{
						vector2Int += Vector2Int.down;
						num4++;
					}
					if (Vector2.Distance(Pits.W.BlockToWorldPos(vector2Int), b.transform.position) >= 30f && Pits.Fits(vector2Int) && !Pits.TooClose(vector2Int))
					{
						Pits.Make(vector2Int, (Random.value < 0.5f) ? PitKind.Maw : PitKind.Slime, true);
						num2++;
					}
				}
				num3++;
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: dug {1} hidden pits ({2} spots skipped for crates nearby, {3} heavy things on the layer)", new object[]
			{
				Pits.W.biomeDepth,
				num2,
				Pits.crated,
				Pits.heavySpots.Count
			}));
			int num5 = (Biome.Jungle ? Diff.Count(Plugin.FlowersPerLayer.Value) : 0);
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			while (num10 < 12000 && num6 < num5)
			{
				Vector2Int vector2Int2;
				vector2Int2..ctor(Random.Range(10, (int)(Pits.W.width - 10U)), Random.Range(10, (int)(Pits.W.height - 10U)));
				if (Pits.W.GetBlock(vector2Int2) == 0)
				{
					int num11 = 0;
					while (num11 < 40 && vector2Int2.y > 2 && Pits.W.GetBlock(vector2Int2) == 0)
					{
						vector2Int2 += Vector2Int.down;
						num11++;
					}
					if (Vector2.Distance(Pits.W.BlockToWorldPos(vector2Int2), b.transform.position) < 30f || Pits.TooClose(vector2Int2))
					{
						num9++;
					}
					else if (!Pits.FitsRough(vector2Int2))
					{
						num7++;
					}
					else if (!Pits.RoomToGrow(vector2Int2))
					{
						num8++;
					}
					else
					{
						Pits.Level(vector2Int2, 4, 3);
						if (Pits.Make(vector2Int2, PitKind.Flower, true).Plant)
						{
							num6++;
						}
					}
				}
				num10++;
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: {1} of {2} carnivorous flowers", Pits.W.biomeDepth, num6, num5) + ((num6 < num5) ? string.Format(" (turned down: {0} no floor for its stomach, {1} no room to grow, {2} near her or a pit)", num7, num8, num9) : ""));
			Pits.crated = 0;
			Pits.heavySpots = null;
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x0004B228 File Offset: 0x00049428
		public static List<Vector2Int> Bowl(int half, int depth)
		{
			List<Vector2Int> list = new List<Vector2Int>();
			float num = (float)half + 0.5f;
			float num2 = (float)depth + 0.5f;
			for (int i = 1; i <= depth; i++)
			{
				for (int j = -half; j <= half; j++)
				{
					float num3 = (float)i / num2;
					if ((float)j / num * ((float)j / num) + num3 * num3 * num3 * num3 <= 1f)
					{
						list.Add(new Vector2Int(j, -i));
					}
				}
			}
			return list;
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x0004B2A0 File Offset: 0x000494A0
		private static IEnumerable<Vector2Int> Rim(List<Vector2Int> bowl, int half)
		{
			HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>(bowl);
			HashSet<Vector2Int> hashSet2 = new HashSet<Vector2Int>();
			foreach (Vector2Int vector2Int in bowl)
			{
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						Vector2Int vector2Int2;
						vector2Int2..ctor(vector2Int.x + i, vector2Int.y + j);
						if (vector2Int2.y < 0 && !hashSet.Contains(vector2Int2))
						{
							hashSet2.Add(vector2Int2);
						}
					}
				}
			}
			for (int k = half + 1; k <= half + 2; k++)
			{
				hashSet2.Add(new Vector2Int(k, 0));
				hashSet2.Add(new Vector2Int(-k, 0));
			}
			return hashSet2;
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x0004B380 File Offset: 0x00049580
		private static bool Fits(Vector2Int top)
		{
			if (top.y - 10 < 40 || (long)top.y > (long)((ulong)(Pits.W.height - 30U)) || top.x - 2 - 3 < 2 || (long)(top.x + 2 + 3) > (long)((ulong)(Pits.W.width - 2U)))
			{
				return false;
			}
			for (int i = -4; i <= 4; i++)
			{
				if (Pits.W.GetBlock(new Vector2Int(top.x + i, top.y)) == 0)
				{
					return false;
				}
				for (int j = 1; j <= 3; j++)
				{
					if (Pits.W.GetBlock(new Vector2Int(top.x + i, top.y + j)) != 0)
					{
						return false;
					}
				}
			}
			return Pits.GroundFits(top);
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x0004B448 File Offset: 0x00049648
		private static bool FitsRough(Vector2Int top)
		{
			return top.y - 10 >= 40 && (long)top.y <= (long)((ulong)(Pits.W.height - 30U)) && top.x - 2 - 3 >= 2 && (long)(top.x + 2 + 3) <= (long)((ulong)(Pits.W.width - 2U)) && Pits.Rough(top, 4, 2, 3) && Pits.GroundFits(top);
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x0004B4BC File Offset: 0x000496BC
		public static bool Rough(Vector2Int top, int half, int step, int clear)
		{
			for (int i = -half; i <= half; i++)
			{
				int num = top.x + i;
				int num2 = int.MinValue;
				int num3 = top.y + step;
				while (num3 >= top.y - step && num2 == -2147483648)
				{
					if (Pits.W.GetBlock(new Vector2Int(num, num3)) != 0 && Pits.W.GetBlock(new Vector2Int(num, num3 + 1)) == 0)
					{
						num2 = num3;
					}
					num3--;
				}
				if (num2 == -2147483648)
				{
					return false;
				}
				for (int j = top.y + step + 1; j <= top.y + clear; j++)
				{
					if (Pits.W.GetBlock(new Vector2Int(num, j)) != 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x0004B580 File Offset: 0x00049780
		public static void Level(Vector2Int top, int half, int clear)
		{
			for (int i = -half; i <= half; i++)
			{
				int num = top.x + i;
				for (int j = top.y; j >= top.y - 3; j--)
				{
					Vector2Int vector2Int;
					vector2Int..ctor(num, j);
					if (Pits.W.GetBlock(vector2Int) != 0)
					{
						break;
					}
					Pits.W.SetBlock(vector2Int, 16);
				}
				for (int k = 1; k <= clear; k++)
				{
					Vector2Int vector2Int2;
					vector2Int2..ctor(num, top.y + k);
					if (Pits.W.GetBlock(vector2Int2) != 0)
					{
						Pits.W.SetBlock(vector2Int2, 0);
					}
				}
			}
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x0004B624 File Offset: 0x00049824
		private static bool GroundFits(Vector2Int top)
		{
			FluidManager main = FluidManager.main;
			List<Vector2Int> list = Pits.Bowl(2, 10);
			int num = 0;
			int num2 = 0;
			foreach (Vector2Int vector2Int in list.Concat<Vector2Int>(Pits.Rim(list, 2)))
			{
				Vector2Int vector2Int2 = top + vector2Int;
				ushort block = Pits.W.GetBlock(vector2Int2);
				num2++;
				if (block == 0)
				{
					num++;
				}
				if (block != 0 && Pits.W.GetBlockInfo(block).health > 2000f)
				{
					return false;
				}
				if (main && main.GetLiquid(vector2Int2.x, vector2Int2.y) != 0)
				{
					return false;
				}
			}
			if (num > num2 / 3)
			{
				return false;
			}
			if (Pits.HeavyNear(Pits.W.BlockToWorldPos(top)))
			{
				Pits.crated++;
				return false;
			}
			if (main)
			{
				for (int i = -6; i <= 6; i++)
				{
					for (int j = 1; j <= 4; j++)
					{
						if (main.GetLiquid(top.x + i, top.y + j) != 0)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x0004B76C File Offset: 0x0004996C
		private static void FindHeavy()
		{
			Pits.heavySpots = (from r in Resources.FindObjectsOfTypeAll<Rigidbody2D>()
				where r && r.gameObject.scene.IsValid() && Pits.Heavy(r.gameObject)
				select r.transform.position).ToList<Vector2>();
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x0004B7D0 File Offset: 0x000499D0
		private static bool HeavyNear(Vector2 mouth)
		{
			if (Pits.heavySpots == null)
			{
				Pits.FindHeavy();
			}
			float hw = 8.5f;
			return Pits.heavySpots.Any<Vector2>((Vector2 h) => Mathf.Abs(h.x - mouth.x) < hw && h.y > mouth.y - 1.5f && h.y < mouth.y + 8f);
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x0004B818 File Offset: 0x00049A18
		internal static bool Heavy(GameObject go)
		{
			if (!go || go.GetComponentInParent<Body>())
			{
				return false;
			}
			if (go.GetComponentInParent<DamagingCrate>())
			{
				return true;
			}
			Rigidbody2D component = go.GetComponent<Rigidbody2D>();
			return (go.GetComponentInParent<Container>() && (!component || component.mass >= 3f)) || (component && component.bodyType == null && component.mass >= 8f);
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x0004B898 File Offset: 0x00049A98
		public static Pit Make(Vector2Int top, PitKind kind, bool withBones)
		{
			Pit pit = new Pit
			{
				Kind = kind,
				Top = top,
				Half = 2,
				Depth = 10,
				Ravish = (Random.value < Plugin.PitRavishChance.Value)
			};
			if (kind == PitKind.Flower)
			{
				pit.Ravish = false;
				pit.Milk = Random.value < Plugin.FlowerMilkChance.Value;
				if (!Flowers.Plant(pit))
				{
					return pit;
				}
			}
			Pits.All.Add(pit);
			if (withBones)
			{
				Pits.Bones(pit);
			}
			return pit;
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x0004B928 File Offset: 0x00049B28
		public static int RemoveNear(Vector2 at, float radius)
		{
			int num = 0;
			IEnumerable<Pit> all = Pits.All;
			Func<Pit, bool> <>9__0;
			Func<Pit, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (Pit x) => !x.Open && Pits.Holder != x && Vector2.Distance(x.Mouth, at) < radius);
			}
			foreach (Pit pit in all.Where<Pit>(func).ToArray<Pit>())
			{
				foreach (GameObject gameObject in pit.Props)
				{
					if (gameObject)
					{
						Object.Destroy(gameObject);
					}
				}
				if (pit.Plant)
				{
					Object.Destroy(pit.Plant.gameObject);
				}
				Pits.All.Remove(pit);
				num++;
			}
			return num;
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x0004BA18 File Offset: 0x00049C18
		private static bool RoomToGrow(Vector2Int top)
		{
			int num = 0;
			for (int i = -4; i <= 4; i++)
			{
				for (int j = 1; j <= 9; j++)
				{
					if (Pits.W.GetBlock(new Vector2Int(top.x + i, top.y + j)) != 0 && ++num > 10)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x0004BA70 File Offset: 0x00049C70
		private static void Bones(Pit pit)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			int num = Random.Range(1, 3);
			for (int i = 0; i < num; i++)
			{
				float num2 = (float)(((i % 2 == 0) ? 1 : (-1)) * ((Random.value < 0.5f) ? 1 : (-1))) * Random.Range((float)pit.Half + 1.4f, (float)pit.Half + 3f);
				float num3;
				if (Pits.Ground(pit, num2, out num3))
				{
					Vector2 vector;
					vector..ctor(pit.Mouth.x + num2, num3 + 0.02f);
					GameObject gameObject = new GameObject("LewdSkeleton");
					gameObject.transform.position = vector;
					SpriteRenderer spriteRenderer2 = gameObject.AddComponent<SpriteRenderer>();
					spriteRenderer2.sprite = Pits.BonesSprite((spriteRenderer && spriteRenderer.sprite) ? spriteRenderer.sprite.pixelsPerUnit : 16f);
					spriteRenderer2.flipX = Random.value < 0.5f;
					if (Pits.W)
					{
						spriteRenderer2.sharedMaterial = Pits.W.defaultMat;
					}
					if (spriteRenderer)
					{
						spriteRenderer2.sortingLayerID = spriteRenderer.sortingLayerID;
						spriteRenderer2.sortingOrder = spriteRenderer.sortingOrder - 12;
					}
					pit.Props.Add(gameObject);
				}
			}
			string[] array = new string[] { "utility", "medical", "food", "drug", "water", "tool" };
			int num4 = Random.Range(Plugin.PitLootMin.Value, Plugin.PitLootMax.Value + 1);
			int num5 = 0;
			int num6 = 0;
			while (num6 < num4 && num5++ < num4 * 6)
			{
				string text = array[Random.Range(0, array.Length)];
				if (ItemLootPool.pool != null && ItemLootPool.pool.ContainsKey(text) && ItemLootPool.pool[text].Count != 0)
				{
					string item = ItemLootPool.RandomFromPool(text).Item1;
					float num7 = ((Random.value < 0.5f) ? (-1f) : 1f) * Random.Range((float)pit.Half + 1.1f, (float)pit.Half + 3.5f);
					Vector2 vector2;
					float num8;
					vector2..ctor(pit.Mouth.x + num7, (Pits.Ground(pit, num7, out num8) ? num8 : (pit.Mouth.y + 0.5f)) + 0.4f);
					try
					{
						GameObject gameObject2 = Utils.Create(item, vector2, Random.Range(0f, 360f));
						if (Pits.Heavy(gameObject2))
						{
							Object.Destroy(gameObject2);
							num6--;
						}
					}
					catch
					{
					}
				}
				num6++;
			}
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x0004BD78 File Offset: 0x00049F78
		private static bool Ground(Pit pit, float dx, out float y)
		{
			Vector2Int vector2Int = Pits.W.WorldToBlockPos(pit.Mouth + new Vector2(dx, 0f));
			for (int i = 2; i >= -3; i--)
			{
				Vector2Int vector2Int2;
				vector2Int2..ctor(vector2Int.x, pit.Top.y + i);
				if (Pits.W.GetBlock(vector2Int2) != 0 && Pits.W.GetBlock(vector2Int2 + Vector2Int.up) == 0)
				{
					y = Pits.W.BlockToWorldPos(vector2Int2).y + 0.5f;
					return true;
				}
			}
			y = 0f;
			return false;
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0004BE14 File Offset: 0x0004A014
		private static Sprite BonesSprite(float ppu)
		{
			if (Pits.bones)
			{
				return Pits.bones;
			}
			Texture2D texture2D = new Texture2D(22, 7, 4, false)
			{
				filterMode = 0
			};
			Pits.<>c__DisplayClass56_0 CS$<>8__locals1;
			CS$<>8__locals1.px = new Color32[154];
			Color32 color;
			color..ctor(214, 206, 186, byte.MaxValue);
			Color32 color2;
			color2..ctor(120, 110, 96, byte.MaxValue);
			Color32 color3;
			color3..ctor(30, 26, 22, byte.MaxValue);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 1; j < 5; j++)
				{
					Pits.<BonesSprite>g__P|56_0(i, j, color, ref CS$<>8__locals1);
				}
			}
			Pits.<BonesSprite>g__P|56_0(1, 3, color3, ref CS$<>8__locals1);
			Pits.<BonesSprite>g__P|56_0(2, 3, color3, ref CS$<>8__locals1);
			Pits.<BonesSprite>g__P|56_0(1, 1, color2, ref CS$<>8__locals1);
			Pits.<BonesSprite>g__P|56_0(3, 1, color2, ref CS$<>8__locals1);
			for (int k = 4; k < 6; k++)
			{
				Pits.<BonesSprite>g__P|56_0(k, 2, color, ref CS$<>8__locals1);
			}
			for (int l = 6; l < 12; l++)
			{
				Pits.<BonesSprite>g__P|56_0(l, 2, color, ref CS$<>8__locals1);
				if (l % 2 == 0)
				{
					Pits.<BonesSprite>g__P|56_0(l, 3, color, ref CS$<>8__locals1);
					Pits.<BonesSprite>g__P|56_0(l, 4, color, ref CS$<>8__locals1);
					Pits.<BonesSprite>g__P|56_0(l, 1, color2, ref CS$<>8__locals1);
				}
			}
			for (int m = 12; m < 15; m++)
			{
				for (int n = 1; n < 4; n++)
				{
					Pits.<BonesSprite>g__P|56_0(m, n, (n == 2) ? color2 : color, ref CS$<>8__locals1);
				}
			}
			for (int num = 15; num < 22; num++)
			{
				Pits.<BonesSprite>g__P|56_0(num, 1, color, ref CS$<>8__locals1);
				if (num > 16)
				{
					Pits.<BonesSprite>g__P|56_0(num, 3, color2, ref CS$<>8__locals1);
				}
			}
			Pits.<BonesSprite>g__P|56_0(21, 0, color, ref CS$<>8__locals1);
			Pits.<BonesSprite>g__P|56_0(8, 5, color2, ref CS$<>8__locals1);
			Pits.<BonesSprite>g__P|56_0(9, 6, color, ref CS$<>8__locals1);
			Pits.<BonesSprite>g__P|56_0(10, 6, color, ref CS$<>8__locals1);
			texture2D.SetPixels32(CS$<>8__locals1.px);
			texture2D.Apply();
			Pits.bones = Sprite.Create(texture2D, new Rect(0f, 0f, 22f, 7f), new Vector2(0.5f, 0f), ppu);
			return Pits.bones;
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x0004C024 File Offset: 0x0004A224
		public static void Clear(Body b)
		{
			if (Pits.InMaw && b)
			{
				Pits.Release(b, false);
			}
			foreach (Pit pit in Pits.All)
			{
				foreach (SpriteRenderer spriteRenderer in new SpriteRenderer[] { pit.LipL, pit.LipR, pit.Gullet })
				{
					if (spriteRenderer)
					{
						Object.Destroy(spriteRenderer.gameObject);
					}
				}
				foreach (GameObject gameObject in pit.Props)
				{
					if (gameObject)
					{
						Object.Destroy(gameObject);
					}
				}
			}
			Pits.All.Clear();
			Flowers.Clear(b);
			Slits.Clear(b);
			Pits.ClearTentacles();
			Engulfed.Hide();
			Pits.InThroat = false;
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0004C148 File Offset: 0x0004A348
		public static void Update(Body b, float dt)
		{
			if (!Pits.W || !b)
			{
				return;
			}
			if (Pits.attemptCooldown > 0f)
			{
				Pits.attemptCooldown -= dt;
			}
			if (Pits.InMaw && !b.alive)
			{
				Pits.Release(b, false);
				return;
			}
			Pits.InThroat = false;
			if (Pits.InMaw)
			{
				Pits.Inside(b, dt);
				return;
			}
			if ((Pits.lastCheck -= dt) <= 0f)
			{
				Pits.lastCheck = 1f;
				IEnumerable<Pit> all = Pits.All;
				Func<Pit, bool> <>9__0;
				Func<Pit, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (Pit x) => x.Kind != PitKind.Flower && !x.Open && !x.Checked && !x.Dead && Vector2.Distance(x.Mouth, b.transform.position) < 35f);
				}
				foreach (Pit pit in all.Where<Pit>(func).ToArray<Pit>())
				{
					pit.Checked = true;
					foreach (Collider2D collider2D in Physics2D.OverlapBoxAll(pit.Mouth + Vector2.up * 3f, new Vector2(pit.Width + 13f, 9f), 0f))
					{
						if (collider2D && Pits.Heavy(collider2D.attachedRigidbody ? collider2D.attachedRigidbody.gameObject : collider2D.gameObject))
						{
							pit.Dead = true;
							Plugin.Log.LogInfo(string.Format("A hidden pit at {0} gave up: {1} lies by it", pit.Mouth, collider2D.name));
							break;
						}
					}
				}
			}
			Pits.FireAtLips(b, dt);
			using (List<Pit>.Enumerator enumerator = Pits.All.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Pit p = enumerator.Current;
					if (p.Kind == PitKind.Flower)
					{
						if (p.Spitting && !p.Dead)
						{
							Pits.SpitWatch(b, p);
						}
						else if (p.Open && Pits.Holder != p && !b.limbs.Any<Limb>((Limb l) => l && !l.dismembered && Pits.InPit(p, l.transform.position, 1f)))
						{
							Pits.Refill(p);
						}
					}
					else if (!p.Dead)
					{
						if (p.BurnCooldown > 0f)
						{
							p.BurnCooldown -= dt;
						}
						if (p.Sated > 0f)
						{
							p.Sated -= dt;
						}
						if (!p.Open)
						{
							Pits.Step(b, p);
						}
						else if (Pits.Broken(p))
						{
							Pits.Die(b, p);
						}
						else if (p.Kind == PitKind.Maw)
						{
							Pits.MawOpen(b, p, dt);
						}
						else
						{
							Pits.SlimeDown(b, p, dt);
						}
					}
				}
			}
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x0004C4DC File Offset: 0x0004A6DC
		private static bool Burn(Body b, Pit p, float amount, string why)
		{
			p.Burnt += amount;
			p.BurnCooldown = 1f;
			Sound.Play("Scald", p.Mouth, false, true, null, 1f, 0.7f, false, false);
			Sound.Play("megabite", p.Mouth, false, true, null, 0.6f, 1.4f, false, false);
			float num = Mathf.Max(1f, Plugin.PitFireHealth.Value);
			Plugin.Log.LogInfo(string.Format("The maw at {0} burns ({1}, {2:0}): {3:0}/{4:0} left", new object[]
			{
				p.Mouth,
				why,
				amount,
				Mathf.Max(0f, num - p.Burnt),
				num
			}));
			if (p.Burnt < num)
			{
				return false;
			}
			if (b && b.alive && Vector2.Distance(b.transform.position, p.Mouth) < 14f)
			{
				Relief.Say(b, "pit_burnt");
			}
			Plugin.Log.LogInfo(string.Format("The maw at {0} burnt to death", p.Mouth));
			Pits.Die(b, p);
			return true;
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x0004C620 File Offset: 0x0004A820
		private static void FireAtLips(Body b, float dt)
		{
			if ((Pits.fireScan -= dt) <= 0f)
			{
				Pits.fireScan = 2f;
				Pits.fires.Clear();
				if (Pits.All.Any<Pit>((Pit p) => p.Kind == PitKind.Maw && p.Open && !p.Dead && Vector2.Distance(p.Mouth, b.transform.position) < 40f))
				{
					foreach (Item item in Object.FindObjectsOfType<Item>())
					{
						if (item.id == "torch" || item.id == "campfire")
						{
							Pits.fires.Add(item);
						}
					}
				}
			}
			if ((Pits.fireTick -= dt) > 0f || Pits.fires.Count == 0)
			{
				return;
			}
			Pits.fireTick = 0.5f;
			foreach (Pit pit in Pits.All.Where<Pit>((Pit x) => x.Kind == PitKind.Maw && x.Open && !x.Dead && !x.Sealed).ToArray<Pit>())
			{
				foreach (Item item2 in Pits.fires)
				{
					if (item2 && item2.condition > 0f && !item2.ParentContainer())
					{
						Vector2 vector = item2.transform.position;
						if (Mathf.Abs(vector.x - pit.Mouth.x) <= (float)pit.Half + Plugin.FireReach.Value && vector.y <= pit.Mouth.y + Plugin.FireReach.Value && vector.y >= pit.Mouth.y - (float)pit.Depth - 1f && Pits.Burn(b, pit, Plugin.FireBurn.Value * 0.5f, (item2.id == "campfire") ? "a campfire" : "a torch"))
						{
							break;
						}
					}
				}
			}
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x0004C880 File Offset: 0x0004AA80
		private static bool Broken(Pit p)
		{
			if (p.RimCells.Count == 0)
			{
				return false;
			}
			int num = 0;
			foreach (Vector2Int vector2Int in p.RimCells)
			{
				if (Pits.W.GetBlock(vector2Int) == 0 && ++num > 2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x0004C8FC File Offset: 0x0004AAFC
		private static void Die(Body b, Pit p)
		{
			p.Dead = true;
			if (Pits.Holder == p)
			{
				Pits.Release(b, p.Kind == PitKind.Flower);
			}
			if (p.Kind == PitKind.Flower && p.Plant)
			{
				Flowers.Kill(p.Plant);
			}
			Engulfed.Hide();
			if (Pits.tentaclesOf == p)
			{
				Pits.ClearTentacles();
			}
			foreach (Component component in new Component[] { p.LipL, p.LipR, p.Gullet })
			{
				if (component)
				{
					Object.Destroy(component.gameObject);
				}
			}
			p.LipL = (p.LipR = (p.Gullet = null));
			Plugin.Log.LogInfo(string.Format("The {0} pit at {1} was broken open: it is dead", p.Kind, p.Mouth));
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0004C9E4 File Offset: 0x0004ABE4
		private static void Refill(Pit p)
		{
			if (p.Ground != null)
			{
				foreach (KeyValuePair<Vector2Int, ushort> keyValuePair in p.Ground)
				{
					if (Pits.W.GetBlock(keyValuePair.Key) != keyValuePair.Value)
					{
						Pits.W.SetBlock(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			p.Ground = null;
			p.Open = false;
			p.Sealed = false;
			p.RimCells.Clear();
			foreach (Component component in new Component[] { p.LipL, p.LipR, p.Gullet })
			{
				if (component)
				{
					Object.Destroy(component.gameObject);
				}
			}
			p.LipL = (p.LipR = (p.Gullet = null));
			Plugin.Log.LogInfo(string.Format("The flower's stomach at {0} closed up: the ground is whole again", p.Mouth));
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x0004CB10 File Offset: 0x0004AD10
		public static void FlowerDied(Body b, Pit p)
		{
			if (p.Dead)
			{
				return;
			}
			Pits.Die(b, p);
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x0004CB24 File Offset: 0x0004AD24
		public static void SwallowWhole(Body b, Pit p)
		{
			if (!p.Open)
			{
				p.Ground = new Dictionary<Vector2Int, ushort>();
				List<Vector2Int> list = Pits.Bowl(p.Half, p.Depth);
				foreach (Vector2Int vector2Int in list.Concat<Vector2Int>(Pits.Rim(list, p.Half)))
				{
					p.Ground[p.Top + vector2Int] = Pits.W.GetBlock(p.Top + vector2Int);
				}
				for (int i = -p.Half; i <= p.Half; i++)
				{
					p.Ground[new Vector2Int(p.Top.x + i, p.Top.y)] = Pits.W.GetBlock(new Vector2Int(p.Top.x + i, p.Top.y));
				}
				p.Open = true;
				p.Cover = 16;
				List<Vector2Int> list2 = Pits.Bowl(p.Half, p.Depth);
				p.RimCells.Clear();
				foreach (Vector2Int vector2Int2 in Pits.Rim(list2, p.Half))
				{
					Vector2Int vector2Int3 = p.Top + vector2Int2;
					p.RimCells.Add(vector2Int3);
					if (Pits.W.GetBlock(vector2Int3) == 0)
					{
						Pits.W.SetBlock(vector2Int3, 16);
						FluidManager main = FluidManager.main;
						if (main != null)
						{
							main.SetLiquid(vector2Int3.x, vector2Int3.y, 0);
						}
					}
				}
				foreach (Vector2Int vector2Int4 in list2)
				{
					if (Pits.W.GetBlock(p.Top + vector2Int4) != 0)
					{
						Pits.W.SetBlock(p.Top + vector2Int4, 0);
					}
				}
			}
			p.Sealed = false;
			Pits.Seal(p);
			p.Closed = 1f;
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			if (component && component.sprite)
			{
				FleshArt.Ensure(component.sprite.pixelsPerUnit);
				WallArt.Ensure(component.sprite.pixelsPerUnit);
			}
			Pits.overOrder = b.GetComponentsInChildren<SpriteRenderer>(true).Max<SpriteRenderer>((SpriteRenderer r) => r.sortingOrder);
			if (!p.Gullet)
			{
				p.Gullet = Pits.MakePart(component, "LewdPitGullet", (FleshArt.Floor.Length != 0) ? FleshArt.Floor[0] : null, -20);
				if (p.Gullet)
				{
					Vector2 vector = p.Gullet.sprite.bounds.size;
					p.Gullet.transform.localScale = new Vector3((p.Width + 0.4f) / vector.x, ((float)p.Depth + 0.4f) / vector.y, 1f);
					p.Gullet.transform.position = new Vector3(p.Mouth.x, p.Mouth.y - 0.6f - (float)p.Depth, 0f);
				}
			}
			if (p.Gullet)
			{
				p.Gullet.color = new Color(0.42f, 0.6f, 0.38f);
			}
			Sprite sprite = ((WallArt.Jaw.Length != 0 && WallArt.Jaw[0].Length != 0) ? WallArt.Jaw[0][0] : null);
			if (!p.LipL)
			{
				p.LipL = Pits.MakePart(component, "LewdPitJaw", sprite, 0);
			}
			if (!p.LipR)
			{
				p.LipR = Pits.MakePart(component, "LewdPitJaw", sprite, 0);
			}
			foreach (Limb limb in b.limbs)
			{
				if (limb && !limb.dismembered && limb.rb)
				{
					limb.rb.velocity = Vector2.zero;
				}
			}
			Pits.Shift(b, p.Mouth + Vector2.down * 6.4f - b.limbs[1].transform.position);
			Pits.Swallow(b, p);
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x0004D000 File Offset: 0x0004B200
		private static bool CoverBroken(Pit p)
		{
			for (int i = -p.Half; i <= p.Half; i++)
			{
				if (Pits.W.GetBlock(new Vector2Int(p.Top.x + i, p.Top.y)) == 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x0004D050 File Offset: 0x0004B250
		private static bool Disturbed(Body b, Pit p)
		{
			bool flag = b && b.limbs.Any<Limb>((Limb l) => l && !l.dismembered && Mathf.Abs(l.transform.position.x - p.Mouth.x) < (float)p.Half + 1f && Mathf.Abs(l.transform.position.y - p.Mouth.y) < 1.5f);
			if (Time.time - p.SealedAt >= 5f)
			{
				p.Sated = 0f;
				p.Closed = 0f;
				Pits.Unseal(p);
				Pits.PlaceLips(p);
				Sound.Play("megabite", p.Mouth, false, true, null, 0.8f, 0.7f, false, false);
				if (b && b.alive && Vector2.Distance(b.transform.position, p.Mouth) < 12f)
				{
					Relief.Say(b, "pit_maw");
				}
				Plugin.Log.LogInfo(string.Format("The resting maw at {0} was dug into: it wakes and opens", p.Mouth));
				return true;
			}
			if (flag)
			{
				return false;
			}
			for (int i = -p.Half; i <= p.Half; i++)
			{
				Vector2Int vector2Int;
				vector2Int..ctor(p.Top.x + i, p.Top.y);
				if (Pits.W.GetBlock(vector2Int) == 0)
				{
					Pits.W.SetBlock(vector2Int, 16);
				}
			}
			Plugin.Log.LogInfo(string.Format("The maw at {0} held under her: its lips shut again", p.Mouth));
			return false;
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0004D1FC File Offset: 0x0004B3FC
		private static void Regrow(Body b, Pit p, float dt)
		{
			bool flag = false;
			int num = -p.Half;
			while (num <= p.Half && !flag)
			{
				flag = Pits.W.GetBlock(new Vector2Int(p.Top.x + num, p.Top.y)) == 0;
				num++;
			}
			if (!flag)
			{
				p.RegrowTimer = 0f;
				return;
			}
			if (b && b.limbs.Any<Limb>((Limb l) => l && !l.dismembered && Mathf.Abs(l.transform.position.x - p.Mouth.x) < (float)p.Half + 1f && Mathf.Abs(l.transform.position.y - p.Mouth.y) < 1.5f))
			{
				return;
			}
			if ((p.RegrowTimer += dt) < 8f)
			{
				return;
			}
			p.RegrowTimer = 0f;
			for (int i = -p.Half; i <= p.Half; i++)
			{
				Vector2Int vector2Int;
				vector2Int..ctor(p.Top.x + i, p.Top.y);
				if (Pits.W.GetBlock(vector2Int) == 0)
				{
					Pits.W.SetBlock(vector2Int, 16);
				}
			}
			Sound.Play("goo", p.Mouth, false, true, null, 0.7f, 0.6f, false, false);
			Plugin.Log.LogInfo(string.Format("The maw at {0} grew its lips back", p.Mouth));
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0004D38C File Offset: 0x0004B58C
		private static void Step(Body b, Pit p)
		{
			if (!b.alive || Pits.Busy())
			{
				return;
			}
			Vector2 mouth = p.Mouth;
			float num = Scenes.FeetY(b);
			if (Mathf.Abs(b.transform.position.x - mouth.x) > (float)p.Half + 0.3f || num < mouth.y + 0.2f || num > mouth.y + 1.4f)
			{
				return;
			}
			Pits.Trigger(b, p);
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0004D408 File Offset: 0x0004B608
		private static bool Busy()
		{
			return Slits.Holding || Slits.Inside || Flowers.Holding || Flesh.Holding || FleshWall.Holding || Tentacles.Holding || Worm.Inside || Webs.Caught || SlimeBog.Stuck || External.ScavHolding || Womb.InLabor;
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0004D464 File Offset: 0x0004B664
		public static void Trigger(Body b, Pit p)
		{
			p.Open = true;
			ushort num = 16;
			p.Cover = 16;
			List<Vector2Int> list = Pits.Bowl(p.Half, p.Depth);
			int num2 = 0;
			p.RimCells.Clear();
			foreach (Vector2Int vector2Int in Pits.Rim(list, p.Half))
			{
				Vector2Int vector2Int2 = p.Top + vector2Int;
				p.RimCells.Add(vector2Int2);
				if (Pits.W.GetBlock(vector2Int2) == 0)
				{
					Pits.W.SetBlock(vector2Int2, num);
					FluidManager main = FluidManager.main;
					if (main != null)
					{
						main.SetLiquid(vector2Int2.x, vector2Int2.y, 0);
					}
					num2++;
				}
			}
			foreach (Vector2Int vector2Int3 in list)
			{
				if (Pits.W.GetBlock(p.Top + vector2Int3) != 0)
				{
					Pits.W.SetBlock(p.Top + vector2Int3, 0);
				}
			}
			for (int i = -p.Half; i <= p.Half; i++)
			{
				Vector2Int vector2Int4;
				vector2Int4..ctor(p.Top.x + i, p.Top.y);
				if (Pits.W.GetBlock(vector2Int4) != 0)
				{
					Pits.W.DamageBlock(vector2Int4, Pits.W.GetBlockInfo(Pits.W.GetBlock(vector2Int4)).health + 1f, true, false, true);
				}
			}
			if (num2 > 0)
			{
				Plugin.Log.LogInfo(string.Format("The pit's rim needed {0} blocks of rock to close it", num2));
			}
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			if (component && component.sprite)
			{
				FleshArt.Ensure(component.sprite.pixelsPerUnit);
			}
			if (p.Kind == PitKind.Maw)
			{
				p.Gullet = Pits.MakePart(component, "LewdPitGullet", (FleshArt.Floor.Length != 0) ? FleshArt.Floor[0] : null, -20);
				if (p.Gullet)
				{
					Vector2 vector = p.Gullet.sprite.bounds.size;
					p.Gullet.transform.localScale = new Vector3((p.Width + 0.4f) / vector.x, ((float)p.Depth + 0.4f) / vector.y, 1f);
					p.Gullet.color = new Color(0.7f, 0.5f, 0.55f);
					p.Gullet.transform.position = new Vector3(p.Mouth.x, p.Mouth.y - 0.6f - (float)p.Depth, 0f);
				}
				if (component && component.sprite)
				{
					WallArt.Ensure(component.sprite.pixelsPerUnit);
				}
				Sprite sprite = ((WallArt.Jaw.Length != 0 && WallArt.Jaw[0].Length != 0) ? WallArt.Jaw[0][0] : null);
				Pits.overOrder = b.GetComponentsInChildren<SpriteRenderer>(true).Max<SpriteRenderer>((SpriteRenderer r) => r.sortingOrder);
				p.LipL = Pits.MakePart(component, "LewdPitJaw", sprite, 0);
				p.LipR = Pits.MakePart(component, "LewdPitJaw", sprite, 0);
				foreach (SpriteRenderer spriteRenderer in new SpriteRenderer[] { p.LipL, p.LipR })
				{
					if (spriteRenderer)
					{
						spriteRenderer.sortingOrder = Pits.overOrder + 10;
					}
				}
				p.Closed = 0f;
				Pits.PlaceLips(p);
				Sound.Play("megabite", p.Mouth, false, true, null, 0.8f, 0.7f, false, false);
				Sound.Play("goo", p.Mouth, false, true, null, 1f, 0.5f, false, false);
				Relief.Say(b, "pit_maw");
			}
			else
			{
				int num3 = Pits.FillSlime(p);
				Sound.Play("goo", p.Mouth, false, true, null, 1f, 0.6f, false, false);
				Sfx.PlayMix(b, 0.8f, new string[] { "slime_gloop", "slime_bubble" });
				Relief.Say(b, "pit_slime");
				Plugin.Log.LogInfo(string.Format("A slime pit opened under her ({0} cells of slime)", num3));
			}
			b.eyePanicTime = 1f;
			b.adrenaline += 25f;
			PlayerCamera main2 = PlayerCamera.main;
			if (main2 != null)
			{
				main2.shaker.Shake(80f);
			}
			Plugin.Log.LogInfo(string.Format("A hidden {0} pit opened under her at {1}", p.Kind, p.Mouth));
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0004D98C File Offset: 0x0004BB8C
		private static int FillSlime(Pit p)
		{
			FluidManager main = FluidManager.main;
			if (!main)
			{
				return 0;
			}
			int num = 0;
			foreach (Vector2Int vector2Int in Pits.Bowl(p.Half, p.Depth))
			{
				if (vector2Int.y <= -1)
				{
					Vector2Int vector2Int2 = p.Top + vector2Int;
					if (main.Empty(vector2Int2))
					{
						main.fluid[vector2Int2.x, vector2Int2.y] = SlimeFluid.Id;
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0004DA3C File Offset: 0x0004BC3C
		private static bool InPit(Pit p, Vector2 at, float margin)
		{
			return Mathf.Abs(at.x - p.Mouth.x) < (float)p.Half + margin && at.y < p.Mouth.y + 0.5f && at.y > p.Mouth.y - (float)p.Depth - 1.5f;
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0004DAA8 File Offset: 0x0004BCA8
		private static SpriteRenderer MakePart(SpriteRenderer torso, string name, Sprite s, int order)
		{
			if (!s)
			{
				return null;
			}
			SpriteRenderer spriteRenderer = new GameObject(name).AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = s;
			if (Pits.W)
			{
				spriteRenderer.sharedMaterial = Pits.W.defaultMat;
			}
			if (torso)
			{
				spriteRenderer.sortingLayerID = torso.sortingLayerID;
				spriteRenderer.sortingOrder = torso.sortingOrder + order;
			}
			return spriteRenderer;
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x0004DB14 File Offset: 0x0004BD14
		private static void Seal(Pit p)
		{
			if (p.Sealed)
			{
				return;
			}
			p.Sealed = true;
			p.SealedAt = Time.time;
			for (int i = -p.Half; i <= p.Half; i++)
			{
				Vector2Int vector2Int;
				vector2Int..ctor(p.Top.x + i, p.Top.y);
				if (Pits.W.GetBlock(vector2Int) == 0)
				{
					Pits.W.SetBlock(vector2Int, 16);
				}
			}
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x0004DB8C File Offset: 0x0004BD8C
		private static void Unseal(Pit p)
		{
			if (!p.Sealed)
			{
				return;
			}
			p.Sealed = false;
			for (int i = -p.Half; i <= p.Half; i++)
			{
				Vector2Int vector2Int;
				vector2Int..ctor(p.Top.x + i, p.Top.y);
				if (Pits.W.GetBlock(vector2Int) != 0)
				{
					Pits.W.SetBlock(vector2Int, 0);
				}
			}
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x0004DBF8 File Offset: 0x0004BDF8
		private static void PlaceLips(Pit p)
		{
			if (!p.LipL || !p.LipR || WallArt.Jaw.Length == 0)
			{
				return;
			}
			if (p.Kind == PitKind.Flower)
			{
				bool flag = Pits.Holder == p;
				p.LipL.enabled = flag;
				p.LipR.enabled = false;
				if (p.Gullet)
				{
					p.Gullet.enabled = flag;
				}
				if (!flag)
				{
					return;
				}
			}
			if (Pits.Holder == p && WallArt.Gut.Length != 0)
			{
				Sprite sprite = WallArt.Gut[(int)(Time.time * 3f) % WallArt.Gut.Length];
				float num = 10.8f / sprite.bounds.size.y;
				p.LipL.sprite = sprite;
				p.LipL.flipX = false;
				p.LipL.transform.localScale = new Vector3(num, num, 1f);
				p.LipL.transform.position = new Vector3(p.Mouth.x, p.Mouth.y + 0.62f, 0f);
				p.LipL.sortingOrder = Pits.overOrder + 10;
				p.LipL.color = ((p.Kind == PitKind.Flower) ? new Color(0.6f, 0.85f, 0.55f) : Color.white);
				p.LipR.enabled = false;
				return;
			}
			p.LipR.enabled = true;
			int num2 = Mathf.Clamp(Mathf.RoundToInt(p.Closed * (float)(WallArt.Jaw.Length - 1)), 0, WallArt.Jaw.Length - 1);
			Sprite[] array = WallArt.Jaw[num2];
			Sprite sprite2 = array[(int)(Time.time * 4f + (float)p.Top.x) % array.Length];
			float num3 = 10.8f / sprite2.bounds.size.y;
			float num4 = p.Mouth.y + 0.62f;
			float num5 = p.Width * 0.5f + 0.1f;
			foreach (ValueTuple<SpriteRenderer, float> valueTuple in new ValueTuple<SpriteRenderer, float>[]
			{
				new ValueTuple<SpriteRenderer, float>(p.LipL, -1f),
				new ValueTuple<SpriteRenderer, float>(p.LipR, 1f)
			})
			{
				SpriteRenderer item = valueTuple.Item1;
				float item2 = valueTuple.Item2;
				item.sprite = sprite2;
				item.flipX = item2 > 0f;
				item.transform.localScale = new Vector3(num3, num3, 1f);
				item.transform.position = new Vector3(p.Mouth.x + item2 * num5, num4, 0f);
				item.sortingOrder = Pits.overOrder + 10;
			}
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x0004DEEC File Offset: 0x0004C0EC
		private static Limb[] Targets(Body b, string[] names)
		{
			return names.Select<string, Limb>((string n) => ((n.Length > 0) ? b.LimbByName(n) : null) ?? b.limbs[1]).Select<Limb, Limb>(delegate(Limb l)
			{
				if (!l || l.dismembered)
				{
					return b.limbs[1];
				}
				return l;
			}).ToArray<Limb>();
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x0004DF30 File Offset: 0x0004C130
		private static void TentacleSounds(Body b, float dt, bool inside)
		{
			if ((Pits.writheTimer -= dt) <= 0f)
			{
				Pits.writheTimer = Random.Range(1.6f, 3.2f);
				Sfx.PlayMix(b, inside ? 0.5f : 0.7f, new string[] { "tentacle_writhe" });
			}
			if ((Pits.suckTimer -= dt) <= 0f)
			{
				Pits.suckTimer = Random.Range(3f, 6f);
				Sfx.PlayMix(b, 0.6f, new string[] { "tentacle_suck" });
			}
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x0004DFCC File Offset: 0x0004C1CC
		private static Vector2 JawEdge(Pit p, float side, float depth)
		{
			float num = Mathf.Lerp(Pits.JawReachPx[0], Pits.JawReachPx[Pits.JawReachPx.Length - 1], p.Closed) * (1f - 0.25f * depth / 10.8f) * 10.8f / 150f;
			return new Vector2(p.Mouth.x + side * (p.Width * 0.5f + 0.1f - num + 0.15f), p.Mouth.y + 0.62f - depth);
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x0004E05C File Offset: 0x0004C25C
		private static void MawOpen(Body b, Pit p, float dt)
		{
			if (Pits.SpitWatch(b, p))
			{
				return;
			}
			if (p.Sealed && Pits.Holder != p && Pits.CoverBroken(p) && Pits.Disturbed(b, p))
			{
				return;
			}
			if (p.Closed >= 1f && p.Sated <= 0f)
			{
				Vector2 mouth = p.Mouth;
				float num = Scenes.FeetY(b);
				if (b.alive && !Pits.Busy() && Mathf.Abs(b.transform.position.x - mouth.x) <= (float)p.Half + 0.3f && num > mouth.y - 0.6f && num < mouth.y + 1.2f)
				{
					p.Closed = 0f;
					Pits.Unseal(p);
					Sound.Play("megabite", p.Mouth, false, true, null, 0.7f, 0.8f, false, false);
					Relief.Say(b, "pit_maw");
				}
				Pits.PlaceLips(p);
				return;
			}
			bool flag = b.alive && Pits.InPit(p, b.limbs[1].transform.position, 0.6f);
			bool flag2 = b.alive && b.limbs.Any<Limb>((Limb l) => l && !l.dismembered && Pits.InPit(p, l.transform.position, 0.3f));
			if (p.Closed >= 1f)
			{
				Pits.PlaceLips(p);
			}
			if (p.Closed < 1f && p.Sated > 0f && (flag || flag2))
			{
				p.Closed = Mathf.Max(0f, p.Closed - dt);
				Pits.PlaceLips(p);
				return;
			}
			if (p.Closed < 1f)
			{
				p.Closed = Mathf.Min(1f, p.Closed + dt / Mathf.Max(2f, Plugin.PitCloseSeconds.Value) * ((p.Sated > 0f) ? 2f : 1f));
				Pits.PlaceLips(p);
				if (flag && Burning.LitTorchInHand(b))
				{
					if (p.BurnCooldown <= 0f && Pits.Burn(b, p, Plugin.FireTorchHit.Value, "the torch in its throat"))
					{
						return;
					}
					p.Closed = 0f;
					p.Sated = Mathf.Max(p.Sated, 8f);
					if (Pits.tentaclesOf == p)
					{
						Pits.ClearTentacles();
					}
					Pits.PlaceLips(p);
					return;
				}
				else
				{
					if (flag)
					{
						Pits.InThroat = true;
						float num2 = p.Mouth.y - 1.6f - b.limbs[1].transform.position.y;
						if (num2 > 0f)
						{
							b.transform.position += Vector3.up * Mathf.Min(num2, 12f * dt);
							if (b.rb.velocity.y < 0f)
							{
								b.rb.velocity = new Vector2(b.rb.velocity.x, 0f);
							}
						}
						Pits.S.arousal = Mathf.Min(100f, Pits.S.arousal + 1.2f * dt);
						if ((Pits.lineTimer -= dt) <= 0f)
						{
							Pits.lineTimer = Random.Range(2.5f, 4f);
							Relief.Say(b, (Random.value < 0.5f) ? "pit_closing" : "pit_bound");
						}
						if (Pits.tentaclesOf != p || Pits.tentacles.Count != Pits.ThroatHold.Length)
						{
							Pits.ClearTentacles();
							Pits.tentaclesOf = p;
							SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
							for (int i = 0; i < Pits.ThroatHold.Length; i++)
							{
								Pits.tentacles.Add(Pits.MakeTentacle(component, null));
							}
							Sfx.PlayMix(b, 0.8f, new string[] { "tentacle_writhe" });
						}
						Vector2[] array = new Vector2[Pits.ThroatHold.Length];
						for (int j = 0; j < array.Length; j++)
						{
							array[j] = Pits.JawEdge(p, (j % 2 == 0) ? (-1f) : 1f, 0.5f + (float)j * 0.45f);
						}
						Pits.DrawTentacles(array, Pits.Targets(b, Pits.ThroatHold));
						Pits.TentacleSounds(b, dt, false);
					}
					else if (Pits.tentaclesOf == p && !Pits.InMaw)
					{
						Pits.ClearTentacles();
					}
					if (p.Closed >= 1f)
					{
						Sound.Play("eatFlesh", p.Mouth, false, true, null, 1f, 0.6f, false, false);
						if ((flag || flag2) && Pits.Busy())
						{
							p.Closed = 0.6f;
							p.Sated = Mathf.Max(p.Sated, 5f);
							Pits.PlaceLips(p);
							Plugin.Log.LogInfo("The maw would shut on her while something else has her: it opens again");
							return;
						}
						if (flag || flag2)
						{
							Pits.Swallow(b, p);
						}
						else
						{
							Plugin.Log.LogInfo("The maw shut on nothing");
							p.Sated = Mathf.Max(p.Sated, 20f);
							if (b.alive && Vector2.Distance(b.transform.position, p.Mouth) < 9f)
							{
								Relief.Say(b, "pit_sealed");
							}
						}
						Pits.Seal(p);
					}
				}
			}
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x0004E6C8 File Offset: 0x0004C8C8
		private static void Swallow(Body b, Pit p)
		{
			Pits.Shift(b, Vector2.down * 1.5f);
			foreach (Item item in Clothes.Worn(b).ToArray<Item>())
			{
				item.condition = Mathf.Max(0.03f, item.condition - 0.15f);
			}
			Pits.Holder = p;
			Pits.timeIn = 0f;
			Pits.pulse = 2f;
			Pits.lineTimer = 3f;
			Pits.attemptCooldown = 0.8f;
			Relief.ForceStop();
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			b.bodyAnimator.Play("ExperimentLayDown");
			b.armsAnimator.Play("ArmsLayDown");
			Pits.wetLoop = (Sfx.Has("loop_gut") ? Sfx.Loop(b, "loop_gut") : Sfx.Loop(b, "loop_wet"));
			Pits.ClearTentacles();
			Pits.tentaclesOf = p;
			Pits.Orgasms = 0;
			Pits.orgasmGap = 3f;
			Pits.spent = false;
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			bool flag = p.Kind == PitKind.Flower;
			for (int j = 0; j < Pits.GutHold.Length; j++)
			{
				Pits.tentacles.Add(Pits.MakeTentacle(component, flag ? new Color32?(new Color32(176, 56, 118, byte.MaxValue)) : null));
			}
			Sfx.PlayMix(b, 0.9f, new string[] { "tentacle_thrust" });
			Relief.Say(b, flag ? "flower_swallowed" : "pit_swallowed");
			Plugin.Log.LogInfo(flag ? ("The flower swallowed her (" + (p.Milk ? "it milks her" : "it digests") + ")") : ("The maw swallowed her (" + (p.Ravish ? "it ravishes" : "it digests") + ")"));
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0004E8C8 File Offset: 0x0004CAC8
		private static void Inside(Body b, float dt)
		{
			Pit holder = Pits.Holder;
			if (Pits.Broken(holder))
			{
				Pits.Die(b, holder);
				return;
			}
			if (holder.Kind == PitKind.Flower && Burning.LitTorchInHand(b) && holder.BurnCooldown <= 0f)
			{
				holder.BurnCooldown = 1f;
				FleshBurn fleshBurn = (holder.Plant ? holder.Plant.GetComponent<FleshBurn>() : null);
				if (fleshBurn)
				{
					Burning.Damage(fleshBurn, Plugin.FireTorchHit.Value, "the torch in its stomach", true);
				}
				if (Pits.Holder != holder)
				{
					return;
				}
				Relief.Say(b, "pit_torch_spit");
				Pits.Release(b, true);
				return;
			}
			else if (Burning.LitTorchInHand(b) && holder.BurnCooldown <= 0f)
			{
				if (Pits.Burn(b, holder, Plugin.FireTorchHit.Value, "the torch in its belly"))
				{
					return;
				}
				Relief.Say(b, "pit_torch_spit");
				Pits.Release(b, true);
				return;
			}
			else
			{
				if (!Pits.InPit(holder, b.limbs[1].transform.position, 2f))
				{
					Plugin.Log.LogInfo("She is out of the maw");
					Pits.Release(b, false);
					return;
				}
				Pits.timeIn += dt;
				Vector3 position = b.transform.position;
				position.x = Mathf.MoveTowards(position.x, holder.Mouth.x, 2f * dt);
				b.transform.position = position;
				b.rb.velocity = new Vector2(0f, Mathf.Min(b.rb.velocity.y, 0f));
				if (Keys.Down(Plugin.RemoveKey.Value))
				{
					Pits.Struggle(b);
				}
				Pits.PlaceLips(holder);
				Vector2 vector = holder.Mouth + Vector2.down * 7.95f;
				Vector2[] array = new Vector2[Pits.GutHold.Length];
				for (int i = 0; i < array.Length; i++)
				{
					float num = 3.1415927f * (0.15f + 0.7f * (float)i / ((float)array.Length - 1f)) * ((i % 2 == 0) ? 1f : (-1f)) + 1.5707964f;
					array[i] = vector + new Vector2(Mathf.Cos(num) * 1.9f, Mathf.Sin(num) * ((Mathf.Sin(num) > 0f) ? 2.6f : 2.1f));
				}
				Pits.DrawTentacles(array, Pits.Targets(b, Pits.GutHold));
				Pits.TentacleSounds(b, dt, true);
				Pits.S.arousal = Mathf.Min(100f, Pits.S.arousal + 2.2f * dt);
				Pits.S.lust = Mathf.Min(100f, Pits.S.lust + 0.4f * dt);
				if (Pits.wetLoop)
				{
					Pits.wetLoop.volume = Mathf.MoveTowards(Pits.wetLoop.volume, 0.45f, dt);
				}
				if (holder.Ravish)
				{
					Pits.S.arousal = Mathf.Min(100f, Pits.S.arousal + 1.6f * dt);
				}
				if ((Pits.pulse -= dt) <= 0f)
				{
					Pits.pulse = (holder.Ravish ? Random.Range(2.2f, 4f) : Random.Range(3.5f, 6f));
					Pits.S.arousal = Mathf.Min(100f, Pits.S.arousal + (holder.Ravish ? 11f : 7f));
					b.eatTime = Mathf.Max(b.eatTime, 0.6f);
					Sfx.Play(b, 0.75f, new string[] { "muffled", "moan" });
					Sfx.Play(b, 0.4f, new string[] { "drip" });
					Sfx.PlayMix(b, 0.7f, new string[] { "tentacle_thrust" });
					Semen.Add(b, 3f);
					Womb.Fill(b, 4f);
				}
				if (holder.Kind == PitKind.Flower)
				{
					if ((Pits.lineTimer -= dt) <= 0f)
					{
						Pits.lineTimer = Random.Range(6f, 10f);
						Relief.Say(b, holder.Milk ? ((Pits.S.exhaustion > 70f) ? "flower_drained" : "flower_inside_milk") : ((Pits.timeIn < Plugin.PitDigestAfter.Value) ? "flower_inside" : "flower_digest"));
					}
					if (holder.Milk)
					{
						Flowers.Milk(b, dt);
						return;
					}
				}
				else if ((Pits.lineTimer -= dt) <= 0f)
				{
					Pits.lineTimer = Random.Range(6f, 10f);
					Relief.Say(b, holder.Ravish ? ((Pits.Orgasms >= Plugin.PitOrgasmsToDeath.Value - 2) ? "pit_ravish_spent" : "pit_ravish") : ((Pits.timeIn < Plugin.PitDigestAfter.Value) ? "pit_inside" : "pit_digest"));
				}
				if (holder.Ravish && Pits.spent)
				{
					b.bloodOxygen = 0f;
				}
				if (holder.Ravish)
				{
					Pits.orgasmGap -= dt;
					if (Pits.S.arousal >= 99f && Pits.orgasmGap <= 0f && b.conscious)
					{
						Pits.orgasmGap = 4f;
						Plugin.Orgasm(b, false);
						Pits.Orgasms++;
						Pits.S.arousal = Mathf.Max(Pits.S.arousal, 45f + 6f * (float)Pits.Orgasms);
						b.bloodOxygen = Mathf.Max(0f, b.bloodOxygen - Plugin.FleshWallOrgasmAir.Value * Diff.Lethal);
						b.stamina = Mathf.Max(0f, b.stamina - 20f);
						Semen.Add(b, 8f);
						Womb.Fill(b, 10f);
						Relief.Say(b, "pit_ravish_come");
						Plugin.Log.LogInfo(string.Format("The maw forced orgasm {0} out of her (oxygen {1:0})", Pits.Orgasms, b.bloodOxygen));
						if (Pits.Orgasms >= Plugin.PitOrgasmsToDeath.Value && Diff.LethalScenes)
						{
							Relief.Say(b, "pit_ravish_end");
							b.bloodOxygen = 0f;
							Pits.spent = true;
							Plugin.Log.LogInfo("The maw used her to death");
						}
					}
					return;
				}
				if (Pits.timeIn > Plugin.PitDigestAfter.Value)
				{
					foreach (Limb limb in b.limbs)
					{
						if (!limb.dismembered)
						{
							float armorReduction = limb.GetArmorReduction();
							limb.skinHealth -= 0.9f * Diff.Lethal / armorReduction * dt;
							limb.muscleHealth -= 0.3f * Diff.Lethal / armorReduction * dt;
							limb.pain = Mathf.Min(100f, limb.pain + 1.2f / armorReduction * dt);
						}
					}
					b.bloodOxygen = Mathf.Max(0f, b.bloodOxygen - 1.1f * Diff.Lethal * dt);
				}
				return;
			}
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x0004F038 File Offset: 0x0004D238
		private static void Struggle(Body b)
		{
			if (Pits.attemptCooldown > 0f || !b.conscious)
			{
				return;
			}
			Pits.attemptCooldown = 1f;
			b.stamina = Mathf.Max(0f, b.stamina - 9f);
			float num = Plugin.PitEscape.Value * Diff.Escape * ((b.stamina < 15f) ? 0.5f : 1f) * (1f - Pits.S.submission * 0.003f) * ((Pits.S.orgasmTimer > 15f) ? 0.5f : 1f);
			bool flag = Random.value < num;
			Plugin.Log.LogInfo(string.Format("Struggling in the maw: chance {0:P0}, {1}", num, flag ? "free" : "held"));
			if (flag)
			{
				Relief.Say(b, "pit_escape");
				Pits.Release(b, true);
				return;
			}
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.8f, 0.6f, false, false);
			Relief.Say(b, "pit_fail");
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x0004F162 File Offset: 0x0004D362
		public static void LetGo(Body b)
		{
			if (Pits.InMaw)
			{
				Pits.Release(b, true);
			}
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x0004F174 File Offset: 0x0004D374
		private static void Release(Body b, bool toss)
		{
			Pit holder = Pits.Holder;
			Pits.Holder = null;
			Engulfed.Hide();
			Pits.ClearTentacles();
			if (Pits.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(Pits.wetLoop, 0.5f));
			}
			Pits.wetLoop = null;
			if (!b)
			{
				return;
			}
			Traverse.Create(b).Field("movingAllowed").SetValue(true);
			bool flag = toss && holder != null && b.alive;
			if (holder != null && holder.Plant)
			{
				holder.Plant.sated = 30f;
			}
			if (holder != null)
			{
				holder.Sated = 30f;
				holder.Closed = 0f;
				Pits.Unseal(holder);
				Pits.PlaceLips(holder);
			}
			if (flag)
			{
				float num = ((Pits.Landing(holder, 1) && (!Pits.Landing(holder, -1) || Random.value < 0.5f)) ? 1f : (Pits.Landing(holder, -1) ? (-1f) : ((Random.value < 0.5f) ? (-1f) : 1f)));
				float num2 = Mathf.Max(4f, holder.Mouth.y + 4f - Scenes.FeetY(b));
				float num3 = Mathf.Clamp(Mathf.Sqrt(2f * Mathf.Abs(Physics2D.gravity.y) * 3.5f * num2), 30f, 55f);
				b.Ragdoll();
				float num4 = Mathf.Clamp((float)holder.Half + 3f, 4f, 7f);
				foreach (Limb limb in b.limbs)
				{
					if (limb && !limb.dismembered && limb.rb)
					{
						limb.rb.velocity = new Vector2(num * num4, num3);
					}
				}
				b.rb.velocity = new Vector2(num * num4, num3);
				b.Scream();
				Sound.Play("jumppad", holder.Mouth, false, true, null, 1f, 1f, false, false);
				PlayerCamera main = PlayerCamera.main;
				if (main != null)
				{
					main.shaker.Shake(60f);
				}
				holder.SpatAt = Time.time;
				holder.Spitting = true;
				b.wetness = Mathf.Max(b.wetness, 40f);
				Semen.Add(b, 20f);
				Sound.Play("vomit", b.transform.position, false, true, b.transform, 0.8f, 1f, false, false);
				Plugin.Log.LogInfo(string.Format("The maw throws her out (up {0:0}, side {1:0})", num3, num));
			}
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x0004F43C File Offset: 0x0004D63C
		private static bool SpitWatch(Body b, Pit p)
		{
			if (!p.Spitting)
			{
				return false;
			}
			if (b.limbs.All<Limb>((Limb l) => !l || l.dismembered || !Pits.InPit(p, l.transform.position, 0.4f) || l.transform.position.y > p.Mouth.y + 0.7f) && Time.time - p.SpatAt > 0.15f)
			{
				p.Spitting = false;
				p.Closed = 1f;
				p.Sealed = false;
				Pits.Seal(p);
				Pits.PlaceLips(p);
				Sound.Play("eatFlesh", p.Mouth, false, true, null, 0.8f, 0.7f, false, false);
				Plugin.Log.LogInfo("She is out: the maw shuts under her");
				return true;
			}
			if (Time.time - p.SpatAt > 5f)
			{
				p.Spitting = false;
			}
			return true;
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x0004F534 File Offset: 0x0004D734
		private static bool Landing(Pit p, int side)
		{
			int num = p.Top.x + side * (p.Half + 2);
			if (Pits.W.GetBlock(new Vector2Int(num, p.Top.y)) == 0)
			{
				return false;
			}
			for (int i = 1; i <= 4; i++)
			{
				if (Pits.W.GetBlock(new Vector2Int(num, p.Top.y + i)) != 0)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x0004F5A8 File Offset: 0x0004D7A8
		private static void Shift(Body b, Vector2 d)
		{
			b.transform.position += d;
			foreach (Limb limb in b.limbs)
			{
				if (limb && !limb.dismembered && !limb.transform.IsChildOf(b.transform))
				{
					limb.transform.position += d;
				}
			}
			Physics2D.SyncTransforms();
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x0004F630 File Offset: 0x0004D830
		private static void SlimeDown(Body b, Pit p, float dt)
		{
			if (!SlimeBog.Stuck || !Pits.InPit(p, b.transform.position, 1f))
			{
				if (Pits.tentaclesOf == p && !Pits.InMaw)
				{
					Pits.ClearTentacles();
				}
				p.Closed = 0f;
				return;
			}
			p.Closed += dt;
			if (Pits.tentaclesOf != p || Pits.tentacles.Count != Pits.SlimeHold.Length)
			{
				Pits.ClearTentacles();
				Pits.tentaclesOf = p;
				SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
				for (int i = 0; i < Pits.SlimeHold.Length; i++)
				{
					Pits.tentacles.Add(Pits.MakeTentacle(component, new Color32?(new Color32(90, 190, 110, byte.MaxValue))));
				}
				Sfx.PlayMix(b, 0.8f, new string[] { "tentacle_writhe" });
			}
			Vector2[] array = new Vector2[Pits.SlimeHold.Length];
			for (int j = 0; j < array.Length; j++)
			{
				float num = ((float)j / ((float)array.Length - 1f) - 0.5f) * 2f;
				array[j] = p.Mouth + new Vector2(num * ((float)p.Half + 0.2f), (float)(-(float)p.Depth) + 0.3f + Mathf.Abs(num) * 5f);
			}
			Pits.DrawTentacles(array, Pits.Targets(b, Pits.SlimeHold));
			Pits.TentacleSounds(b, dt, false);
			b.rb.velocity += Vector2.down * Plugin.PitSlimePull.Value * dt;
			Pits.S.arousal = Mathf.Min(100f, Pits.S.arousal + 0.8f * dt);
			if (p.Closed <= Plugin.PitDigestAfter.Value && (Pits.lineTimer -= dt) <= 0f)
			{
				Pits.lineTimer = Random.Range(6f, 10f);
				Relief.Say(b, "pit_slimegrope");
			}
			if (p.Closed > Plugin.PitDigestAfter.Value)
			{
				foreach (Limb limb in b.limbs)
				{
					if (!limb.dismembered)
					{
						float armorReduction = limb.GetArmorReduction();
						limb.skinHealth -= 0.6f * Diff.Lethal / armorReduction * dt;
						limb.pain = Mathf.Min(100f, limb.pain + 0.8f / armorReduction * dt);
					}
				}
				if ((Pits.lineTimer -= dt) <= 0f)
				{
					Pits.lineTimer = Random.Range(6f, 10f);
					Relief.Say(b, "pit_slimedigest");
				}
			}
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x0004F904 File Offset: 0x0004DB04
		private static SpriteRenderer[] MakeTentacle(SpriteRenderer torso, Color32? tint = null)
		{
			SpriteRenderer[] array = new SpriteRenderer[9];
			for (int i = 0; i < array.Length; i++)
			{
				SpriteRenderer spriteRenderer = new GameObject("LewdTentacle").AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = FleshArt.Tendril;
				if (Pits.W)
				{
					spriteRenderer.sharedMaterial = Pits.W.defaultMat;
				}
				if (torso)
				{
					spriteRenderer.sortingLayerID = torso.sortingLayerID;
					spriteRenderer.sortingOrder = Mathf.Max(torso.sortingOrder + 25, Pits.overOrder + 5);
				}
				Color color = tint ?? new Color32(120, 30, 52, byte.MaxValue);
				Color color2 = ((tint != null) ? (tint.Value * 1.2f) : new Color32(196, 78, 104, byte.MaxValue));
				spriteRenderer.color = Color.Lerp(color, color2, (float)i / (float)array.Length);
				array[i] = spriteRenderer;
			}
			return array;
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x0004FA14 File Offset: 0x0004DC14
		private static void DrawTentacles(Vector2[] anchors, Limb[] targets)
		{
			if (!FleshArt.Tendril)
			{
				return;
			}
			float x = FleshArt.Tendril.bounds.size.x;
			int num = 0;
			while (num < Pits.tentacles.Count && num < targets.Length && num < anchors.Length)
			{
				Vector2 vector = anchors[num];
				Vector2 vector2 = targets[num].transform.position;
				Vector2 vector3 = vector2 - vector;
				Vector2 normalized = new Vector2(-vector3.y, vector3.x).normalized;
				Vector2 vector4 = vector;
				SpriteRenderer[] array = Pits.tentacles[num];
				int num2 = array.Length - 3;
				float num3 = Time.time * (2.2f + (float)num * 0.3f) + (float)num;
				for (int i = 0; i < array.Length; i++)
				{
					float num4 = Mathf.Min(1f, ((float)i + 1f) / (float)num2);
					Vector2 vector5 = ((i < num2) ? (vector + vector3 * num4 + normalized * Mathf.Sin(Time.time * 5f + (float)num * 1.9f + num4 * 4f) * 0.22f * Mathf.Sin(num4 * 3.1415927f)) : (vector2 + new Vector2(Mathf.Cos(num3 + (float)(i - num2) * 2.1f), Mathf.Sin(num3 + (float)(i - num2) * 2.1f)) * 0.2f));
					Vector2 vector6 = vector5 - vector4;
					SpriteRenderer spriteRenderer = array[i];
					if (spriteRenderer)
					{
						spriteRenderer.transform.position = vector4;
						spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector6.y, vector6.x) * 57.29578f);
						spriteRenderer.transform.localScale = new Vector3(vector6.magnitude / x + 0.15f, (i < num2) ? Mathf.Lerp(0.85f, 0.42f, num4) : 0.38f, 1f);
						vector4 = vector5;
					}
				}
				num++;
			}
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0004FC64 File Offset: 0x0004DE64
		private static void ClearTentacles()
		{
			foreach (SpriteRenderer[] array in Pits.tentacles)
			{
				foreach (SpriteRenderer spriteRenderer in array)
				{
					if (spriteRenderer)
					{
						Object.Destroy(spriteRenderer.gameObject);
					}
				}
			}
			Pits.tentacles.Clear();
			Pits.tentaclesOf = null;
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x0004FCE4 File Offset: 0x0004DEE4
		private static bool TooClose(Vector2Int top)
		{
			return Pits.All.Any<Pit>((Pit x) => Mathf.Abs(x.Top.x - top.x) < 16 && Mathf.Abs(x.Top.y - top.y) < 15);
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0004FD14 File Offset: 0x0004DF14
		public static Pit DebugPit(Body b, PitKind? want = null)
		{
			if (!Pits.W)
			{
				return null;
			}
			float num = Scenes.FeetY(b);
			Vector2 vector;
			vector..ctor(b.transform.position.x + (b.isRight ? 3f : (-3f)), num - 0.5f);
			Vector2Int vector2Int = Pits.W.WorldToBlockPos(vector);
			int num2 = 0;
			while (num2 < 4 && Pits.W.GetBlock(vector2Int) == 0)
			{
				vector2Int += Vector2Int.down;
				num2++;
			}
			Vector2Int vector2Int2 = vector2Int;
			bool flag = false;
			int num3 = 0;
			while (num3 <= 30 && !flag)
			{
				foreach (int num4 in new int[]
				{
					num3,
					-num3
				})
				{
					bool flag2 = false;
					int num5 = -4;
					while (num5 <= 4 && !flag2)
					{
						Vector2Int vector2Int3;
						vector2Int3..ctor(vector2Int.x + (b.isRight ? num4 : (-num4)), vector2Int.y + num5);
						if (Pits.Fits(vector2Int3) && !Pits.TooClose(vector2Int3))
						{
							vector2Int2 = vector2Int3;
							flag = (flag2 = true);
						}
						num5++;
					}
					if (flag2)
					{
						break;
					}
				}
				num3++;
			}
			if (!flag && Pits.TooClose(vector2Int))
			{
				Plugin.Log.LogWarning("Debug: another pit is right here, not digging one into it");
				return null;
			}
			if (!flag)
			{
				Plugin.Log.LogWarning("Debug: no proper spot for a pit nearby, digging it right there");
			}
			vector2Int = vector2Int2;
			PitKind pitKind = want ?? ((PitKind)(Pits.debugKind++ % 3));
			Pit pit = Pits.Make(vector2Int, pitKind, true);
			Pits.heavySpots = null;
			Plugin.Log.LogInfo(string.Format("Debug: a hidden {0} pit at {1}", pitKind, Pits.W.BlockToWorldPos(vector2Int)));
			return pit;
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x00050009 File Offset: 0x0004E209
		[CompilerGenerated]
		internal static void <BonesSprite>g__P|56_0(int x, int y, Color32 c, ref Pits.<>c__DisplayClass56_0 A_3)
		{
			if (x >= 0 && x < 22 && y >= 0 && y < 7)
			{
				A_3.px[y * 22 + x] = c;
			}
		}

		// Token: 0x04000496 RID: 1174
		public static readonly List<Pit> All = new List<Pit>();

		// Token: 0x0400049A RID: 1178
		private static float orgasmGap;

		// Token: 0x0400049B RID: 1179
		private static bool spent;

		// Token: 0x0400049C RID: 1180
		private static int overOrder = -1073741824;

		// Token: 0x0400049D RID: 1181
		private static float attemptCooldown;

		// Token: 0x0400049E RID: 1182
		private static float pulse;

		// Token: 0x0400049F RID: 1183
		private static float lineTimer;

		// Token: 0x040004A0 RID: 1184
		private static float timeIn;

		// Token: 0x040004A1 RID: 1185
		private static float lastCheck;

		// Token: 0x040004A2 RID: 1186
		private static AudioSource wetLoop;

		// Token: 0x040004A3 RID: 1187
		private static readonly List<SpriteRenderer[]> tentacles = new List<SpriteRenderer[]>();

		// Token: 0x040004A4 RID: 1188
		private static Pit tentaclesOf;

		// Token: 0x040004A5 RID: 1189
		private static Sprite bones;

		// Token: 0x040004A6 RID: 1190
		private const int HalfW = 2;

		// Token: 0x040004A7 RID: 1191
		private const int DepthB = 10;

		// Token: 0x040004A8 RID: 1192
		private static int crated;

		// Token: 0x040004A9 RID: 1193
		private static List<Vector2> heavySpots;

		// Token: 0x040004AA RID: 1194
		private static readonly List<Item> fires = new List<Item>();

		// Token: 0x040004AB RID: 1195
		private static float fireScan;

		// Token: 0x040004AC RID: 1196
		private static float fireTick;

		// Token: 0x040004AD RID: 1197
		private const ushort Soil = 16;

		// Token: 0x040004AE RID: 1198
		private const float JawHeight = 10.8f;

		// Token: 0x040004AF RID: 1199
		private static readonly float[] JawReachPx = new float[] { 10f, 17f, 25f, 34f, 48f };

		// Token: 0x040004B0 RID: 1200
		private static readonly string[] ThroatHold = new string[] { "HandF", "HandB", "Neck", "", "DownTorso", "ThighF", "ThighB", "CrusF" };

		// Token: 0x040004B1 RID: 1201
		private static readonly string[] GutHold = new string[] { "HandF", "HandB", "Neck", "", "DownTorso", "ThighF", "ThighB", "CrusF", "CrusB" };

		// Token: 0x040004B2 RID: 1202
		private static readonly string[] SlimeHold = new string[] { "DownTorso", "ThighF", "ThighB", "CrusF", "CrusB", "", "HandF" };

		// Token: 0x040004B3 RID: 1203
		private static float writheTimer;

		// Token: 0x040004B4 RID: 1204
		private static float suckTimer;

		// Token: 0x040004B5 RID: 1205
		private static int debugKind;
	}
}
