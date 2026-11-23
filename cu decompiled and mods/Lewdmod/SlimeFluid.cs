using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using ScavArmory;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000104 RID: 260
	internal static class SlimeFluid
	{
		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060005AA RID: 1450 RVA: 0x0005B129 File Offset: 0x00059329
		// (set) Token: 0x060005AB RID: 1451 RVA: 0x0005B130 File Offset: 0x00059330
		public static byte Id { get; private set; } = 7;

		// Token: 0x060005AC RID: 1452 RVA: 0x0005B138 File Offset: 0x00059338
		private static void ChooseId()
		{
			if (SlimeFluid.idChosen)
			{
				return;
			}
			SlimeFluid.idChosen = true;
			try
			{
				Type type = AccessTools.TypeByName("CUCoreLib.Registries.LiquidTileRegistry");
				Type type2 = AccessTools.TypeByName("CUCoreLib.Data.CustomLiquidTileInfo");
				if (!(type == null) && !(type2 == null))
				{
					object obj = Activator.CreateInstance(type2);
					Traverse traverse = Traverse.Create(obj);
					traverse.Field("LiquidId").SetValue("lewd_slimefluid");
					traverse.Field("FillLiquidId").SetValue("dirtywater");
					traverse.Field("Buoyancy").SetValue(0.25f);
					traverse.Field("Drag").SetValue(0.82f);
					traverse.Field("WetnessPerSecond").SetValue(0f);
					traverse.Field("ExistingVisualLiquidByte").SetValue(2);
					traverse.Field("Tint").SetValue(SlimeFluid.Color);
					traverse.Field("SpawnAmount").SetValue(0f);
					AccessTools.Method(type, "Register", new Type[]
					{
						typeof(string),
						type2
					}, null).Invoke(null, new object[] { "lewd_slime", obj });
					object[] array = new object[] { "lewd_slime", 0 };
					if ((bool)AccessTools.Method(type, "TryGetWorldByte", null, null).Invoke(null, array))
					{
						SlimeFluid.Id = (byte)array[1];
						SlimeFluid.viaCuCore = true;
					}
					Plugin.Log.LogInfo(string.Format("Slime fluid: registered with CUCoreLib, byte {0}", SlimeFluid.Id));
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning(string.Format("Slime fluid: CUCoreLib registration failed ({0}), byte {1}", ex.Message, SlimeFluid.Id));
			}
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x0005B340 File Offset: 0x00059540
		public static bool Dark(Vector2Int c)
		{
			using (List<SlimeFluid.DarkPool>.Enumerator enumerator = SlimeFluid.DarkPools.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Box.Contains(c))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x0005B3A0 File Offset: 0x000595A0
		public static bool DarkAt(Vector2 world)
		{
			return WorldGeneration.world && SlimeFluid.At(world) && SlimeFluid.Dark(WorldGeneration.world.WorldToBlockPos(world));
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x0005B3C8 File Offset: 0x000595C8
		public static SlimeFluid.DarkPool MarkDark(List<Vector2Int> cells)
		{
			WorldGeneration world = WorldGeneration.world;
			if (cells.Count == 0 || !world)
			{
				return null;
			}
			int x0 = cells.Min<Vector2Int>((Vector2Int c) => c.x);
			int x1 = cells.Max<Vector2Int>((Vector2Int c) => c.x);
			int y0 = cells.Min<Vector2Int>((Vector2Int c) => c.y);
			int y1 = cells.Max<Vector2Int>((Vector2Int c) => c.y);
			List<Vector2Int> list = cells.Where<Vector2Int>((Vector2Int c) => c.y == y1).ToList<Vector2Int>();
			Vector2Int vector2Int = (from c in cells
				where c.y == y0
				orderby Mathf.Abs(c.x - (x0 + x1) / 2)
				select c).First<Vector2Int>();
			SlimeFluid.DarkPool darkPool = new SlimeFluid.DarkPool();
			darkPool.Box = new RectInt(x0 - 2, y0 - 2, x1 - x0 + 5, y1 - y0 + 6);
			darkPool.Left = world.BlockToWorldPos(new Vector2Int(list.Min<Vector2Int>((Vector2Int c) => c.x) - 2, y1 + 1));
			darkPool.Right = world.BlockToWorldPos(new Vector2Int(list.Max<Vector2Int>((Vector2Int c) => c.x) + 2, y1 + 1));
			darkPool.Bottom = world.BlockToWorldPos(vector2Int);
			darkPool.World = world;
			darkPool.Depth = world.biomeDepth;
			SlimeFluid.DarkPool darkPool2 = darkPool;
			SlimeFluid.DarkPools.Add(darkPool2);
			Plugin.Log.LogInfo(string.Format("Slime bog of the hosts' slime: {0} cells at {1} (banks {2}, {3})", new object[] { cells.Count, darkPool2.Bottom, darkPool2.Left, darkPool2.Right }));
			return darkPool2;
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x0005B623 File Offset: 0x00059823
		public static bool At(Vector2Int p)
		{
			return FluidManager.main && FluidManager.main.GetLiquid(p.x, p.y) == SlimeFluid.Id;
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x0005B652 File Offset: 0x00059852
		public static bool At(Vector2 world)
		{
			return WorldGeneration.world && SlimeFluid.At(WorldGeneration.world.WorldToBlockPos(world));
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x0005B674 File Offset: 0x00059874
		public static void Install(FluidManager fm)
		{
			SlimeFluid.ChooseId();
			List<ParticleSystem> value = Traverse.Create(fm).Field("liquidParticles").GetValue<List<ParticleSystem>>();
			if (value == null || value.Count == 0)
			{
				return;
			}
			if (!SlimeFluid.viaCuCore && value.Count >= (int)SlimeFluid.Id)
			{
				SlimeFluid.Id = (byte)(value.Count + 1);
			}
			while (value.Count < (int)(SlimeFluid.Id - 1))
			{
				value.Add(value[0]);
			}
			ParticleSystem particleSystem = value[Mathf.Min(1, value.Count - 1)];
			SlimeFluid.particles = Object.Instantiate<GameObject>(particleSystem.gameObject, fm.transform).GetComponent<ParticleSystem>();
			SlimeFluid.particles.name = "LewdSlimeParticles";
			if (value.Count >= (int)SlimeFluid.Id)
			{
				if (value[(int)(SlimeFluid.Id - 1)])
				{
					Object.Destroy(value[(int)(SlimeFluid.Id - 1)].gameObject);
				}
				value[(int)(SlimeFluid.Id - 1)] = SlimeFluid.particles;
				if (fm.LiquidParticlePrefabs.Count >= (int)SlimeFluid.Id)
				{
					fm.LiquidParticlePrefabs[(int)(SlimeFluid.Id - 1)] = particleSystem.gameObject;
				}
			}
			else
			{
				value.Add(SlimeFluid.particles);
			}
			while (fm.LiquidParticlePrefabs.Count < value.Count)
			{
				fm.LiquidParticlePrefabs.Add(particleSystem.gameObject);
			}
			if (fm.liquidColors.Length <= (int)SlimeFluid.Id)
			{
				List<Color> list = fm.liquidColors.ToList<Color>();
				while (list.Count <= (int)SlimeFluid.Id)
				{
					list.Add(SlimeFluid.Color);
				}
				fm.liquidColors = list.ToArray();
			}
			else
			{
				fm.liquidColors[(int)SlimeFluid.Id] = SlimeFluid.Color;
			}
			FluidManager.WorldFluidToLiquidID[(int)SlimeFluid.Id] = "dirtywater";
			Plugin.Log.LogInfo(string.Format("Slime fluid installed (id {0}, {1} liquid systems)", SlimeFluid.Id, value.Count));
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x0005B864 File Offset: 0x00059A64
		public static void Tint()
		{
			if (!SlimeFluid.particles)
			{
				return;
			}
			int particleCount = SlimeFluid.particles.particleCount;
			if (particleCount == 0)
			{
				return;
			}
			if (SlimeFluid.buffer.Length < particleCount)
			{
				SlimeFluid.buffer = new ParticleSystem.Particle[particleCount * 2];
			}
			particleCount = SlimeFluid.particles.GetParticles(SlimeFluid.buffer);
			Color32 color = new Color(SlimeFluid.Color.r, SlimeFluid.Color.g, SlimeFluid.Color.b, 1f);
			Color32 color2 = new Color(SlimeFluid.DarkColor.r, SlimeFluid.DarkColor.g, SlimeFluid.DarkColor.b, 1f);
			Color32 color3 = SlimeFluid.AcidColor;
			WorldGeneration world = WorldGeneration.world;
			bool flag = SlimeFluid.DarkPools.Count > 0 && world;
			bool flag2 = SlimeFluid.particles.main.simulationSpace == 0;
			Transform transform = SlimeFluid.particles.transform;
			for (int i = 0; i < particleCount; i++)
			{
				Color32 color4 = color;
				if (flag)
				{
					Vector3 vector = (flag2 ? transform.TransformPoint(SlimeFluid.buffer[i].position) : SlimeFluid.buffer[i].position);
					Vector2Int vector2Int = world.WorldToBlockPos(vector);
					if (SlimeFluid.Dark(vector2Int))
					{
						color4 = ((((vector2Int.x * 7 + vector2Int.y * 13) % 11 + 11) % 11 == 0) ? color3 : color2);
					}
				}
				SlimeFluid.buffer[i].startColor = color4;
			}
			SlimeFluid.particles.SetParticles(SlimeFluid.buffer, particleCount);
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x0005BA10 File Offset: 0x00059C10
		public static void SeedLayer(Body b)
		{
			FluidManager main = FluidManager.main;
			WorldGeneration w = WorldGeneration.world;
			SlimeFluid.DarkPools.Clear();
			if (!main || main.fluid == null || !w)
			{
				return;
			}
			int num = Diff.Count(Plugin.SlimeBogsPerLayer.Value);
			if (num <= 0)
			{
				return;
			}
			int width = (int)w.width;
			int height = (int)w.height;
			bool[,] array = new bool[width, height];
			List<List<Vector2Int>> list = new List<List<Vector2Int>>();
			for (int i = 1; i < width - 1; i++)
			{
				for (int j = 1; j < height - 1; j++)
				{
					byte b2 = main.fluid[i, j];
					if (!array[i, j] && (b2 == 1 || b2 == 5))
					{
						List<Vector2Int> list2 = SlimeFluid.Flood(main, array, i, j, width, height);
						if (list2.Count >= 10)
						{
							list.Add(list2);
						}
					}
				}
			}
			Vector2 me = b.transform.position;
			int num2 = 0;
			Func<Vector2Int, bool> <>9__1;
			foreach (List<Vector2Int> list3 in list.OrderBy<List<Vector2Int>, float>((List<Vector2Int> _) => Random.value))
			{
				if (num2 >= num)
				{
					break;
				}
				if (Random.value <= Plugin.SlimePoolChance.Value)
				{
					IEnumerable<Vector2Int> enumerable = list3;
					Func<Vector2Int, bool> func;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = (Vector2Int c) => Vector2.Distance(w.BlockToWorldPos(c), me) < 20f);
					}
					if (!enumerable.Any<Vector2Int>(func))
					{
						foreach (Vector2Int vector2Int in list3)
						{
							main.fluid[vector2Int.x, vector2Int.y] = SlimeFluid.Id;
						}
						if (Random.value < Plugin.DarkBogChance.Value)
						{
							SlimeFluid.MarkDark(list3);
						}
						num2++;
						Plugin.Log.LogInfo(string.Format("Slime bog: a pool of {0} at {1} turned to slime", list3.Count, w.BlockToWorldPos(list3[0])));
					}
				}
			}
			int num3 = 0;
			while (num3 < 600 && num2 < num)
			{
				Vector2? vector = Spots.RandomFloor(25f, me);
				if (vector != null && SlimeFluid.Hollow(w, w.WorldToBlockPos(vector.Value)))
				{
					List<Vector2Int> list4 = new List<Vector2Int>();
					if (SlimeFluid.Pour(vector.Value, 110, list4) >= 14)
					{
						if (Random.value < Plugin.DarkBogChance.Value)
						{
							SlimeFluid.MarkDark(list4);
						}
						num2++;
						Plugin.Log.LogInfo(string.Format("Slime bog poured into a hollow at {0}", vector.Value));
					}
				}
				num3++;
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: {1} slime bogs, {2} of the hosts' slime ({3} water pools found, {4} spots tried)", new object[]
			{
				w.biomeDepth,
				num2,
				SlimeFluid.DarkPools.Count,
				list.Count,
				num3
			}));
			Scavengers.PlaceBogHosts();
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x0005BDB8 File Offset: 0x00059FB8
		private static List<Vector2Int> Flood(FluidManager fm, bool[,] seen, int sx, int sy, int width, int height)
		{
			List<Vector2Int> list = new List<Vector2Int>();
			Stack<Vector2Int> stack = new Stack<Vector2Int>();
			stack.Push(new Vector2Int(sx, sy));
			seen[sx, sy] = true;
			while (stack.Count > 0 && list.Count < 4000)
			{
				Vector2Int vector2Int = stack.Pop();
				list.Add(vector2Int);
				foreach (Vector2Int vector2Int2 in new Vector2Int[]
				{
					Vector2Int.left,
					Vector2Int.right,
					Vector2Int.up,
					Vector2Int.down
				})
				{
					Vector2Int vector2Int3 = vector2Int + vector2Int2;
					if (vector2Int3.x >= 1 && vector2Int3.y >= 1 && vector2Int3.x < width - 1 && vector2Int3.y < height - 1 && !seen[vector2Int3.x, vector2Int3.y])
					{
						byte b = fm.fluid[vector2Int3.x, vector2Int3.y];
						if (b == 1 || b == 5)
						{
							seen[vector2Int3.x, vector2Int3.y] = true;
							stack.Push(vector2Int3);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x0005BF00 File Offset: 0x0005A100
		private static bool Hollow(WorldGeneration w, Vector2Int p)
		{
			SlimeFluid.<>c__DisplayClass23_0 CS$<>8__locals1;
			CS$<>8__locals1.w = w;
			CS$<>8__locals1.p = p;
			return SlimeFluid.<Hollow>g__Wall|23_0(-1, ref CS$<>8__locals1) && SlimeFluid.<Hollow>g__Wall|23_0(1, ref CS$<>8__locals1);
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x0005BF34 File Offset: 0x0005A134
		public static int Pour(Vector2 world, int max, List<Vector2Int> filled = null)
		{
			FluidManager main = FluidManager.main;
			WorldGeneration world2 = WorldGeneration.world;
			if (!main || !world2)
			{
				return 0;
			}
			Vector2Int vector2Int = world2.WorldToBlockPos(world);
			List<Vector2Int> list = new List<Vector2Int>();
			HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>();
			Queue<Vector2Int> queue = new Queue<Vector2Int>();
			queue.Enqueue(vector2Int);
			hashSet.Add(vector2Int);
			while (queue.Count > 0 && list.Count < max)
			{
				Vector2Int vector2Int2 = queue.Dequeue();
				if (vector2Int2.x >= 1 && vector2Int2.y >= 1 && (long)vector2Int2.x < (long)((ulong)(world2.width - 1U)) && (long)vector2Int2.y < (long)((ulong)(world2.height - 1U)) && main.Empty(vector2Int2))
				{
					list.Add(vector2Int2);
					foreach (Vector2Int vector2Int3 in new Vector2Int[]
					{
						Vector2Int.down,
						Vector2Int.left,
						Vector2Int.right
					})
					{
						if (hashSet.Add(vector2Int2 + vector2Int3))
						{
							queue.Enqueue(vector2Int2 + vector2Int3);
						}
					}
				}
			}
			if (list.Count >= max)
			{
				return 0;
			}
			foreach (Vector2Int vector2Int4 in list)
			{
				main.fluid[vector2Int4.x, vector2Int4.y] = SlimeFluid.Id;
			}
			if (filled != null)
			{
				filled.AddRange(list);
			}
			return list.Count;
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x0005C0E8 File Offset: 0x0005A2E8
		public static void DebugPool(Body b, bool dark = false)
		{
			FluidManager main = FluidManager.main;
			Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(b.transform.position + Vector2.down * 1.2f);
			List<Vector2Int> list = new List<Vector2Int>();
			for (int i = -4; i <= 4; i++)
			{
				for (int j = 0; j <= 4; j++)
				{
					Vector2Int vector2Int2;
					vector2Int2..ctor(vector2Int.x + i, vector2Int.y + j);
					if (main.Empty(vector2Int2))
					{
						main.fluid[vector2Int2.x, vector2Int2.y] = SlimeFluid.Id;
						list.Add(vector2Int2);
					}
				}
			}
			if (dark)
			{
				SlimeFluid.MarkDark(list);
			}
			Plugin.Log.LogInfo(string.Format("Debug: poured {0} {1}slime cells around her", list.Count, dark ? "dark " : ""));
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x0005C254 File Offset: 0x0005A454
		[CompilerGenerated]
		internal static bool <Hollow>g__Wall|23_0(int dir, ref SlimeFluid.<>c__DisplayClass23_0 A_1)
		{
			for (int i = 1; i <= 8; i++)
			{
				for (int j = 0; j <= 2; j++)
				{
					if (A_1.w.GetBlock(new Vector2Int(A_1.p.x + dir * i, A_1.p.y + j)) != 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04000655 RID: 1621
		private static bool idChosen;

		// Token: 0x04000656 RID: 1622
		private static bool viaCuCore;

		// Token: 0x04000657 RID: 1623
		public static readonly Color Color = new Color(0.42f, 0.92f, 0.38f, 0.72f);

		// Token: 0x04000658 RID: 1624
		private static ParticleSystem particles;

		// Token: 0x04000659 RID: 1625
		private static ParticleSystem.Particle[] buffer = new ParticleSystem.Particle[0];

		// Token: 0x0400065A RID: 1626
		public static readonly List<SlimeFluid.DarkPool> DarkPools = new List<SlimeFluid.DarkPool>();

		// Token: 0x0400065B RID: 1627
		public static readonly Color DarkColor = new Color(0.04f, 0.085f, 0.05f, 0.9f);

		// Token: 0x0400065C RID: 1628
		private static readonly Color AcidColor = new Color(0.55f, 1f, 0.25f, 1f);

		// Token: 0x0200021E RID: 542
		internal class DarkPool
		{
			// Token: 0x04000AB8 RID: 2744
			public RectInt Box;

			// Token: 0x04000AB9 RID: 2745
			public Vector2 Bottom;

			// Token: 0x04000ABA RID: 2746
			public Vector2 Left;

			// Token: 0x04000ABB RID: 2747
			public Vector2 Right;

			// Token: 0x04000ABC RID: 2748
			public WorldGeneration World;

			// Token: 0x04000ABD RID: 2749
			public int Depth;

			// Token: 0x04000ABE RID: 2750
			public bool Hosts;
		}
	}
}
