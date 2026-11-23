using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000F5 RID: 245
	internal static class SemenFluid
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x00058BF9 File Offset: 0x00056DF9
		// (set) Token: 0x0600056F RID: 1391 RVA: 0x00058C00 File Offset: 0x00056E00
		public static byte Id { get; private set; }

		// Token: 0x06000570 RID: 1392 RVA: 0x00058C08 File Offset: 0x00056E08
		private static void ChooseId()
		{
			if (SemenFluid.idChosen)
			{
				return;
			}
			SemenFluid.idChosen = true;
			try
			{
				Type type = AccessTools.TypeByName("CUCoreLib.Registries.LiquidTileRegistry");
				Type type2 = AccessTools.TypeByName("CUCoreLib.Data.CustomLiquidTileInfo");
				if (!(type == null) && !(type2 == null))
				{
					object obj = Activator.CreateInstance(type2);
					Traverse traverse = Traverse.Create(obj);
					traverse.Field("LiquidId").SetValue("lewd_semenfluid");
					traverse.Field("FillLiquidId").SetValue("dirtywater");
					traverse.Field("Buoyancy").SetValue(0.35f);
					traverse.Field("Drag").SetValue(0.7f);
					traverse.Field("WetnessPerSecond").SetValue(0f);
					traverse.Field("ExistingVisualLiquidByte").SetValue(4);
					traverse.Field("Tint").SetValue(SemenFluid.Color);
					traverse.Field("SpawnAmount").SetValue(0f);
					AccessTools.Method(type, "Register", new Type[]
					{
						typeof(string),
						type2
					}, null).Invoke(null, new object[] { "lewd_semen", obj });
					object[] array = new object[] { "lewd_semen", 0 };
					if ((bool)AccessTools.Method(type, "TryGetWorldByte", null, null).Invoke(null, array))
					{
						SemenFluid.Id = (byte)array[1];
						SemenFluid.viaCuCore = true;
					}
					Plugin.Log.LogInfo(string.Format("Semen fluid: registered with CUCoreLib, byte {0}", SemenFluid.Id));
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Semen fluid: CUCoreLib registration failed (" + ex.Message + ")");
			}
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x00058E08 File Offset: 0x00057008
		public static bool At(Vector2Int p)
		{
			return SemenFluid.Id != 0 && FluidManager.main && FluidManager.main.GetLiquid(p.x, p.y) == SemenFluid.Id;
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x00058E3E File Offset: 0x0005703E
		public static bool At(Vector2 world)
		{
			return WorldGeneration.world && SemenFluid.At(WorldGeneration.world.WorldToBlockPos(world));
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x00058E60 File Offset: 0x00057060
		public static void Install(FluidManager fm)
		{
			SemenFluid.ChooseId();
			List<ParticleSystem> value = Traverse.Create(fm).Field("liquidParticles").GetValue<List<ParticleSystem>>();
			if (value == null || value.Count == 0)
			{
				return;
			}
			if (!SemenFluid.viaCuCore || SemenFluid.Id == 0)
			{
				SemenFluid.Id = (byte)Mathf.Max(value.Count + 1, (int)(SlimeFluid.Id + 1));
			}
			while (value.Count < (int)(SemenFluid.Id - 1))
			{
				value.Add(value[0]);
			}
			for (int i = 0; i < Mathf.Min(6, value.Count); i++)
			{
				ParticleSystemRenderer particleSystemRenderer = (value[i] ? value[i].GetComponent<ParticleSystemRenderer>() : null);
				Plugin.Log.LogInfo(string.Format("Liquid system {0}: {1}", i + 1, (particleSystemRenderer && particleSystemRenderer.sharedMaterial) ? (particleSystemRenderer.sharedMaterial.name + " / " + particleSystemRenderer.sharedMaterial.shader.name) : "-"));
			}
			ParticleSystem particleSystem = value[Mathf.Min(3, value.Count - 1)];
			SemenFluid.particles = Object.Instantiate<GameObject>(particleSystem.gameObject, fm.transform).GetComponent<ParticleSystem>();
			SemenFluid.particles.name = "LewdSemenParticles";
			SemenFluid.Whiten(SemenFluid.particles);
			if (value.Count >= (int)SemenFluid.Id)
			{
				if (value[(int)(SemenFluid.Id - 1)] && value[(int)(SemenFluid.Id - 1)] != value[0])
				{
					Object.Destroy(value[(int)(SemenFluid.Id - 1)].gameObject);
				}
				value[(int)(SemenFluid.Id - 1)] = SemenFluid.particles;
				if (fm.LiquidParticlePrefabs.Count >= (int)SemenFluid.Id)
				{
					fm.LiquidParticlePrefabs[(int)(SemenFluid.Id - 1)] = particleSystem.gameObject;
				}
			}
			else
			{
				value.Add(SemenFluid.particles);
			}
			while (fm.LiquidParticlePrefabs.Count < value.Count)
			{
				fm.LiquidParticlePrefabs.Add(particleSystem.gameObject);
			}
			if (fm.liquidColors.Length <= (int)SemenFluid.Id)
			{
				List<Color> list = fm.liquidColors.ToList<Color>();
				while (list.Count <= (int)SemenFluid.Id)
				{
					list.Add(SemenFluid.Color);
				}
				fm.liquidColors = list.ToArray();
			}
			else
			{
				fm.liquidColors[(int)SemenFluid.Id] = SemenFluid.Color;
			}
			FluidManager.WorldFluidToLiquidID[(int)SemenFluid.Id] = "dirtywater";
			Plugin.Log.LogInfo(string.Format("Semen fluid installed (id {0}, {1} liquid systems)", SemenFluid.Id, value.Count));
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x00059118 File Offset: 0x00057318
		private static void Whiten(ParticleSystem ps)
		{
			ParticleSystemRenderer component = ps.GetComponent<ParticleSystemRenderer>();
			Shader shader = Shader.Find("Sprites/Default");
			if (!component || !shader)
			{
				Plugin.Log.LogWarning("Semen fluid: no sprite shader, drawn as the sap");
				return;
			}
			Texture2D texture2D = new Texture2D(20, 20, 4, false)
			{
				filterMode = 0,
				wrapMode = 1
			};
			Color32[] array = new Color32[400];
			Random random = new Random(5);
			for (int i = 0; i < 20; i++)
			{
				for (int j = 0; j < 20; j++)
				{
					float num = Mathf.Max(0f, Mathf.Abs((float)j + 0.5f - 10f) - 7f);
					float num2 = Mathf.Max(0f, Mathf.Abs((float)i + 0.5f - 10f) - 7f);
					float num3 = Mathf.Clamp01(1.2f - Mathf.Sqrt(num * num + num2 * num2) / 3f);
					int num4 = ((j + i * 2) / 3 + (i * 7 + j) % 5) % 6;
					byte b = (byte)((num4 == 0) ? 255 : ((num4 == 3) ? 226 : (240 + random.Next(0, 8))));
					array[i * 20 + j] = new Color32(b, b, b, (byte)(num3 * 255f));
				}
			}
			texture2D.SetPixels32(array);
			texture2D.Apply(false, true);
			Material material = new Material(shader)
			{
				mainTexture = texture2D,
				color = Color.white,
				name = "LewdSemen"
			};
			material.renderQueue = (component.sharedMaterial ? component.sharedMaterial.renderQueue : material.renderQueue);
			component.sharedMaterial = material;
			Plugin.Log.LogInfo("Semen fluid: drawn with its own cream drops (sprite shader)");
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x000592F8 File Offset: 0x000574F8
		public static void Tint()
		{
			if (!SemenFluid.particles)
			{
				return;
			}
			int particleCount = SemenFluid.particles.particleCount;
			if (particleCount == 0)
			{
				return;
			}
			if (SemenFluid.buffer.Length < particleCount)
			{
				SemenFluid.buffer = new ParticleSystem.Particle[particleCount * 2];
			}
			particleCount = SemenFluid.particles.GetParticles(SemenFluid.buffer);
			Color32 color = new Color(SemenFluid.Color.r, SemenFluid.Color.g, SemenFluid.Color.b, 1f);
			for (int i = 0; i < particleCount; i++)
			{
				SemenFluid.buffer[i].startColor = color;
			}
			SemenFluid.particles.SetParticles(SemenFluid.buffer, particleCount);
		}

		// Token: 0x06000576 RID: 1398 RVA: 0x000593A4 File Offset: 0x000575A4
		public static void Tick(Body b, float dt)
		{
			if (!b || !b.alive || SemenFluid.Id == 0 || (SemenFluid.coatTimer -= dt) > 0f)
			{
				return;
			}
			SemenFluid.coatTimer = 0.5f;
			int num = b.limbs.Count<Limb>((Limb l) => l && !l.dismembered && SemenFluid.At(l.transform.position));
			if (num > 0)
			{
				Semen.Add(b, 0.5f * (float)num * 0.5f);
			}
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x0005942C File Offset: 0x0005762C
		public static int DebugPool(Body b)
		{
			FluidManager main = FluidManager.main;
			WorldGeneration world = WorldGeneration.world;
			if (!main || !world || SemenFluid.Id == 0)
			{
				return 0;
			}
			Vector2Int vector2Int = world.WorldToBlockPos(b.transform.position + Vector2.down * 1.2f);
			int num = 0;
			for (int i = -3; i <= 3; i++)
			{
				for (int j = 0; j <= 3; j++)
				{
					Vector2Int vector2Int2;
					vector2Int2..ctor(vector2Int.x + i, vector2Int.y + j);
					if (main.Empty(vector2Int2))
					{
						main.fluid[vector2Int2.x, vector2Int2.y] = SemenFluid.Id;
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x04000636 RID: 1590
		private static bool idChosen;

		// Token: 0x04000637 RID: 1591
		private static bool viaCuCore;

		// Token: 0x04000638 RID: 1592
		public static readonly Color Color = new Color(0.93f, 0.91f, 0.84f, 0.86f);

		// Token: 0x04000639 RID: 1593
		private static ParticleSystem particles;

		// Token: 0x0400063A RID: 1594
		private static ParticleSystem.Particle[] buffer = new ParticleSystem.Particle[0];

		// Token: 0x0400063B RID: 1595
		private static float coatTimer;
	}
}
