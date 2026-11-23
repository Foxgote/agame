using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x02000128 RID: 296
	internal static class Walls
	{
		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x0006B25E File Offset: 0x0006945E
		private static int Ground
		{
			get
			{
				return LayerMask.GetMask(new string[] { "Ground" });
			}
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x0006B274 File Offset: 0x00069474
		public static void SeedLayer(Body b)
		{
			WorldGeneration world = WorldGeneration.world;
			if (!world)
			{
				return;
			}
			Walls.Prepare(b);
			int num = Walls.Count(Plugin.FleshWallsPerLayer.Value * Diff.Spawn);
			int num2 = Walls.Count(Plugin.SlimeWallsPerLayer.Value * Diff.Spawn);
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			while (num5 < 900 && (num3 < num || num4 < num2))
			{
				Vector2 vector;
				vector..ctor(Random.Range(-world.halfWidth, world.halfWidth), Random.Range(-world.halfHeight, world.halfHeight));
				int num6;
				Vector2 face;
				if (Vector2.Distance(vector, b.transform.position) >= 25f && !Physics2D.OverlapPoint(vector, Walls.Ground) && Walls.FindFace(vector, (Random.value < 0.5f) ? 1 : (-1), 12f, out face, out num6) && !Walls.All.Any<LewdWall>((LewdWall x) => Vector2.Distance(x.Face, face) < 8f))
				{
					bool flag = num3 < num && (num4 >= num2 || Random.value < 0.5f);
					if (!(Walls.Spawn(flag ? WallKind.Flesh : WallKind.Slime, face, num6) == null))
					{
						if (flag)
						{
							num3++;
						}
						else
						{
							num4++;
						}
					}
				}
				num5++;
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: placed {1} flesh walls, {2} slime walls", world.biomeDepth, num3, num4));
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x0006B415 File Offset: 0x00069615
		private static int Count(float avg)
		{
			return Mathf.FloorToInt(avg) + ((Random.value < avg - Mathf.Floor(avg)) ? 1 : 0);
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x0006B430 File Offset: 0x00069630
		private static void Prepare(Body b)
		{
			SpriteRenderer spriteRenderer = ((b && b.limbs.Length > 1) ? b.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				WallArt.Ensure(spriteRenderer.sprite.pixelsPerUnit);
			}
		}

		// Token: 0x060006E4 RID: 1764 RVA: 0x0006B488 File Offset: 0x00069688
		public static bool FindFace(Vector2 from, int dir, float range, out Vector2 face, out int facing)
		{
			face = default(Vector2);
			facing = -dir;
			RaycastHit2D raycastHit2D = Physics2D.Raycast(from, Vector2.right * (float)dir, range, Walls.Ground);
			if (!raycastHit2D || Mathf.Abs(raycastHit2D.normal.x) < 0.95f)
			{
				return false;
			}
			float num = WallArt.Height * 0.7f;
			int num2 = 0;
			int num3 = 0;
			float num4 = -num * 0.5f;
			while (num4 <= num * 0.5f + 0.01f)
			{
				RaycastHit2D raycastHit2D2 = Physics2D.Raycast(new Vector2(raycastHit2D.point.x - (float)dir * 1.5f, raycastHit2D.point.y + num4), Vector2.right * (float)dir, 2.5f, Walls.Ground);
				if (!raycastHit2D2 || raycastHit2D2.distance < 0.01f || Mathf.Abs(raycastHit2D2.point.x - raycastHit2D.point.x) > 0.6f)
				{
					num2++;
				}
				num4 += 0.5f;
				num3++;
			}
			if ((float)(num3 - num2) * 0.5f < 2.5f)
			{
				return false;
			}
			if (Physics2D.OverlapBox(new Vector2(raycastHit2D.point.x - (float)dir * 1.2f, raycastHit2D.point.y), new Vector2(1.2f, 2.4f), 0f, Walls.Ground))
			{
				return false;
			}
			face = new Vector2(raycastHit2D.point.x, raycastHit2D.point.y);
			return true;
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x0006B628 File Offset: 0x00069828
		public static LewdWall Spawn(WallKind kind, Vector2 face, int facing)
		{
			Walls.<>c__DisplayClass7_0 CS$<>8__locals1 = new Walls.<>c__DisplayClass7_0();
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			Walls.Prepare(body);
			Sprite[] array = ((kind == WallKind.Flesh) ? WallArt.Flesh : WallArt.Slime);
			if (array.Length == 0)
			{
				return null;
			}
			CS$<>8__locals1.w = WorldGeneration.world;
			if (!CS$<>8__locals1.w)
			{
				return null;
			}
			float y = array[0].bounds.size.y;
			float x = array[0].bounds.size.x;
			List<ValueTuple<int, Vector2, float, Vector2Int, int>> list;
			int num;
			int num2;
			float[] array2;
			if (!Walls.Lay(CS$<>8__locals1.w, face, facing, 6, out list, out num, out num2, out array2, out CS$<>8__locals1.top))
			{
				return null;
			}
			int num3 = 6;
			int num4 = (num + num2) / 2;
			Vector2 vector;
			vector..ctor(array2[num4], (CS$<>8__locals1.<Spawn>g__yc|0(num) + CS$<>8__locals1.<Spawn>g__yc|0(num2)) * 0.5f);
			CS$<>8__locals1.go = new GameObject((kind == WallKind.Flesh) ? "LewdFleshWall" : "LewdSlimeWall");
			CS$<>8__locals1.go.transform.position = vector + Vector2.right * (float)facing * -0.05f;
			SpriteRenderer spriteRenderer = CS$<>8__locals1.go.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = array[0];
			spriteRenderer.enabled = false;
			if (WorldGeneration.world)
			{
				spriteRenderer.sharedMaterial = WorldGeneration.world.defaultMat;
			}
			SpriteRenderer spriteRenderer2 = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			CS$<>8__locals1.wall = CS$<>8__locals1.go.AddComponent<LewdWall>();
			CS$<>8__locals1.wall.Init(kind, vector, facing, (float)(num2 - num + 1));
			CS$<>8__locals1.wall.Rows = num3;
			float num5 = (float)num3 / y;
			foreach (ValueTuple<int, Vector2, float, Vector2Int, int> valueTuple in list)
			{
				int item = valueTuple.Item1;
				Vector2 item2 = valueTuple.Item2;
				float item3 = valueTuple.Item3;
				Vector2Int item4 = valueTuple.Item4;
				int item5 = valueTuple.Item5;
				SpriteRenderer spriteRenderer3 = new GameObject("LewdWallSlice").AddComponent<SpriteRenderer>();
				spriteRenderer3.transform.SetParent(CS$<>8__locals1.go.transform, true);
				spriteRenderer3.transform.position = new Vector3(item2.x - (float)facing * 0.05f * ((item3 == 0f) ? 1f : 0f), item2.y - ((item3 == 0f) ? 0f : (Mathf.Sign(item3) * (float)facing * 0.05f)), CS$<>8__locals1.go.transform.position.z);
				spriteRenderer3.transform.rotation = Quaternion.Euler(0f, 0f, item3);
				spriteRenderer3.transform.localScale = new Vector3(1f, num5, 1f);
				spriteRenderer3.sprite = WallArt.Slice(array[0], item, num3, item5);
				spriteRenderer3.flipX = facing < 0;
				spriteRenderer3.sharedMaterial = spriteRenderer.sharedMaterial;
				if (spriteRenderer2)
				{
					spriteRenderer3.sortingLayerID = spriteRenderer2.sortingLayerID;
					spriteRenderer3.sortingOrder = spriteRenderer2.sortingOrder - 15;
				}
				CS$<>8__locals1.wall.Slices.Add(spriteRenderer3);
				CS$<>8__locals1.wall.SliceRow.Add(item);
				CS$<>8__locals1.wall.SliceBend.Add(item5);
				CS$<>8__locals1.wall.Holds.Add(item4);
			}
			BoxCollider2D boxCollider2D = CS$<>8__locals1.go.AddComponent<BoxCollider2D>();
			boxCollider2D.isTrigger = false;
			boxCollider2D.size = new Vector2(x, CS$<>8__locals1.wall.Height);
			boxCollider2D.offset = new Vector2((float)facing * x * 0.5f, 0f);
			CS$<>8__locals1.go.layer = LayerMask.NameToLayer("Descriptor");
			BuildingEntity buildingEntity = CS$<>8__locals1.go.AddComponent<BuildingEntity>();
			buildingEntity.animal = true;
			buildingEntity.health = 100000f;
			buildingEntity.id = ((kind == WallKind.Flesh) ? "lewd_fleshwall" : "lewd_slimewall");
			buildingEntity.ignoreBodyOptimize = true;
			buildingEntity.itemsDropOnDestroy = new ItemDrop[0];
			buildingEntity.alwaysDrop = new ItemDrop[0];
			buildingEntity.itemCategoriesToAdd = new string[0];
			buildingEntity.hitSound = Resources.Load<AudioClip>("Sounds/goo");
			if (kind == WallKind.Slime)
			{
				FleshBurn fleshBurn = Burning.Make(CS$<>8__locals1.go, "slime wall", Plugin.SlimeWallHealth.Value, spriteRenderer, null, delegate
				{
					CS$<>8__locals1.wall.AnimalDeath();
					Object.Destroy(CS$<>8__locals1.go);
				});
				fleshBurn.hitsHurt = true;
				fleshBurn.douses = true;
				fleshBurn.fireScale = Mathf.Clamp01(Plugin.SlimeWallFireScale.Value);
			}
			if (kind == WallKind.Flesh)
			{
				Burning.Make(CS$<>8__locals1.go, "flesh wall", Plugin.FleshWallHealth.Value, spriteRenderer, delegate
				{
					FleshWall.OnHit(CS$<>8__locals1.wall);
					CS$<>8__locals1.wall.sated = Mathf.Max(CS$<>8__locals1.wall.sated, Plugin.FleshStunSeconds.Value);
				}, delegate
				{
					CS$<>8__locals1.wall.AnimalDeath();
					Object.Destroy(CS$<>8__locals1.go);
				});
				Light2D light2D = CS$<>8__locals1.go.AddComponent<Light2D>();
				light2D.lightType = 3;
				light2D.color = new Color32(220, 60, 80, byte.MaxValue);
				light2D.intensity = 0.5f;
				light2D.pointLightOuterRadius = 3.5f;
			}
			Walls.All.Add(CS$<>8__locals1.wall);
			return CS$<>8__locals1.wall;
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x0006BB78 File Offset: 0x00069D78
		private static bool Lay(WorldGeneration w, Vector2 face, int facing, int rows, [TupleElementNames(new string[] { "i", "pos", "angle", "hold", "bend" })] out List<ValueTuple<int, Vector2, float, Vector2Int, int>> layout, out int runA, out int runB, out float[] faceX, out int top)
		{
			Walls.<>c__DisplayClass10_0 CS$<>8__locals1;
			CS$<>8__locals1.w = w;
			CS$<>8__locals1.facing = facing;
			layout = new List<ValueTuple<int, Vector2, float, Vector2Int, int>>();
			runA = 0;
			runB = rows - 1;
			faceX = new float[rows];
			top = 0;
			CS$<>8__locals1.x = Mathf.Round(face.x);
			int y = CS$<>8__locals1.w.WorldToBlockPos(face).y;
			int num = int.MinValue;
			foreach (int num2 in new int[] { 0, 1, -1, 2, -2, 3, -3 })
			{
				int num3 = y + rows / 2 + num2;
				bool flag = true;
				int num4 = 0;
				while (num4 < rows && flag)
				{
					flag = Walls.<Lay>g__FaceAt|10_2(num3 - num4, ref CS$<>8__locals1);
					num4++;
				}
				if (flag)
				{
					num = num3;
					break;
				}
			}
			if (num == -2147483648)
			{
				return false;
			}
			top = num;
			for (int j = 0; j < rows; j++)
			{
				faceX[j] = CS$<>8__locals1.x;
				layout.Add(new ValueTuple<int, Vector2, float, Vector2Int, int>(j, new Vector2(CS$<>8__locals1.x, Walls.<Lay>g__Y|10_1(top - j, ref CS$<>8__locals1)), 0f, CS$<>8__locals1.w.WorldToBlockPos(new Vector2(CS$<>8__locals1.x - (float)CS$<>8__locals1.facing * 0.5f, Walls.<Lay>g__Y|10_1(top - j, ref CS$<>8__locals1))), 0));
			}
			float num5 = 6f / Mathf.Max(1f, (WallArt.Flesh.Length != 0) ? WallArt.Flesh[0].pixelsPerUnit : 16f);
			foreach (int num6 in new int[] { 1, -1 })
			{
				int num7 = ((num6 > 0) ? top : (top - rows + 1));
				if (!Walls.<Lay>g__Solid|10_0(CS$<>8__locals1.x - (float)CS$<>8__locals1.facing * 0.5f, Walls.<Lay>g__Y|10_1(num7 + num6, ref CS$<>8__locals1), ref CS$<>8__locals1))
				{
					for (int k = 1; k <= 2; k++)
					{
						float num8 = CS$<>8__locals1.x - (float)CS$<>8__locals1.facing * ((float)k - 0.5f);
						float num9 = Walls.<Lay>g__Y|10_1(num7, ref CS$<>8__locals1);
						if (!Walls.<Lay>g__Solid|10_0(num8, num9, ref CS$<>8__locals1) || Walls.<Lay>g__Solid|10_0(num8, num9 + (float)num6, ref CS$<>8__locals1))
						{
							break;
						}
						layout.Add(new ValueTuple<int, Vector2, float, Vector2Int, int>((num6 > 0) ? 0 : (rows - 1), new Vector2(num8 + (float)CS$<>8__locals1.facing * num5, num9 + 0.5f * (float)num6), 90f * (float)CS$<>8__locals1.facing * (float)num6, CS$<>8__locals1.w.WorldToBlockPos(new Vector2(num8, num9)), k * num6));
					}
				}
			}
			return true;
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x0006BE24 File Offset: 0x0006A024
		public static void Clear(Body b)
		{
			FleshWall.ReleaseIfHeld(b);
			foreach (LewdWall lewdWall in Walls.All.ToArray())
			{
				if (lewdWall)
				{
					Object.Destroy(lewdWall.gameObject);
				}
			}
			Walls.All.Clear();
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x0006BE74 File Offset: 0x0006A074
		public static void DebugWall(Body b, WallKind kind)
		{
			Walls.Prepare(b);
			Vector2 vector = b.limbs[1].transform.position;
			int num = (b.isRight ? 1 : (-1));
			foreach (float num2 in new float[] { 0f, 1f, -1f, 2f, -2f, 3f })
			{
				foreach (int num3 in new int[]
				{
					num,
					-num
				})
				{
					Vector2 vector2;
					int num4;
					if (Walls.FindFace(vector + Vector2.up * num2, num3, 12f, out vector2, out num4))
					{
						LewdWall lewdWall = Walls.Spawn(kind, vector2, num4);
						if (lewdWall != null)
						{
							Plugin.Log.LogInfo(string.Format("Debug: a {0} wall at {1} facing {2}, upright {3:0}, {4} blocks bent round a corner", new object[]
							{
								kind,
								lewdWall.Face,
								num4,
								lewdWall.Height,
								lewdWall.Slices.Count - Mathf.RoundToInt(lewdWall.Height)
							}));
							return;
						}
					}
				}
			}
			Plugin.Log.LogInfo("Debug: no sheer rock face beside her for a wall");
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x0006BFC4 File Offset: 0x0006A1C4
		[CompilerGenerated]
		internal static bool <Lay>g__Solid|10_0(float px, float py, ref Walls.<>c__DisplayClass10_0 A_2)
		{
			return A_2.w.GetBlock(A_2.w.WorldToBlockPos(new Vector2(px, py))) > 0;
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x0006BFE6 File Offset: 0x0006A1E6
		[CompilerGenerated]
		internal static float <Lay>g__Y|10_1(int row, ref Walls.<>c__DisplayClass10_0 A_1)
		{
			return A_1.w.BlockToWorldPos(new Vector2Int(0, row)).y;
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x0006C000 File Offset: 0x0006A200
		[CompilerGenerated]
		internal static bool <Lay>g__FaceAt|10_2(int row, ref Walls.<>c__DisplayClass10_0 A_1)
		{
			return Walls.<Lay>g__Solid|10_0(A_1.x - (float)A_1.facing * 0.5f, Walls.<Lay>g__Y|10_1(row, ref A_1), ref A_1) && !Walls.<Lay>g__Solid|10_0(A_1.x + (float)A_1.facing * 0.5f, Walls.<Lay>g__Y|10_1(row, ref A_1), ref A_1);
		}

		// Token: 0x04000738 RID: 1848
		public static readonly List<LewdWall> All = new List<LewdWall>();

		// Token: 0x04000739 RID: 1849
		public const int Blocks = 6;

		// Token: 0x0400073A RID: 1850
		public const int Wrap = 2;
	}
}
