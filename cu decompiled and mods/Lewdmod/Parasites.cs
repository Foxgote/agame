using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ScavArmory;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x020000D5 RID: 213
	internal static class Parasites
	{
		// Token: 0x0600049C RID: 1180 RVA: 0x0004A4F4 File Offset: 0x000486F4
		internal static void ClearWorld(Body b)
		{
			foreach (ParasiteAI parasiteAI in Parasites.Alive.ToArray())
			{
				if (parasiteAI)
				{
					Object.Destroy(parasiteAI.gameObject);
				}
			}
			Parasites.Alive.Clear();
			Flesh.ClearAll(b);
			Tentacles.ClearAll(b);
			Worm.ClearAll(b);
			FleshZones.Clear();
			Webs.Clear(b);
			Walls.Clear(b);
			Pits.Clear(b);
			LustGas.Clear();
			Latexes.Clear(b);
			LatexPond.Clear(b);
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x0004A578 File Offset: 0x00048778
		public static void Update(Body b)
		{
			WorldGeneration world = WorldGeneration.world;
			if (!world || !world.worldExists)
			{
				return;
			}
			if (world != Parasites.clearedFor || world.biomeDepth != Parasites.clearedDepth)
			{
				Parasites.clearedFor = world;
				Parasites.clearedDepth = world.biomeDepth;
				Parasites.ClearWorld(b);
				Plugin.Log.LogInfo(string.Format("A new world ({0}, depth {1}): the mod's traps and biomes from the last one are cleared", world.biomeOverride, world.biomeDepth));
			}
			if (world.biomeOverride == 1)
			{
				return;
			}
			if (world == Parasites.seededWorld && world.biomeDepth == Parasites.seededDepth)
			{
				return;
			}
			if (Parasites.pendingWorld != world || Parasites.pendingDepth != world.biomeDepth)
			{
				Parasites.pendingWorld = world;
				Parasites.pendingDepth = world.biomeDepth;
				Parasites.pendingSince = Time.time;
				return;
			}
			if (Time.time - Parasites.pendingSince < 3f || !Parasites.LayerFilled(world))
			{
				return;
			}
			if (b.transform.position.sqrMagnitude < 0.01f && Time.time - Parasites.pendingSince < 30f)
			{
				return;
			}
			Parasites.seededWorld = world;
			Parasites.seededDepth = world.biomeDepth;
			foreach (ParasiteAI parasiteAI in Parasites.Alive.ToArray())
			{
				if (parasiteAI)
				{
					Object.Destroy(parasiteAI.gameObject);
				}
			}
			Parasites.Alive.Clear();
			Flesh.ClearAll(b);
			Tentacles.ClearAll(b);
			Worm.ClearAll(b);
			FleshZones.Clear();
			Webs.Clear(b);
			Walls.Clear(b);
			Pits.Clear(b);
			LustGas.Clear();
			Latexes.Clear(b);
			LatexPond.Clear(b);
			if (Biome.Ending)
			{
				Plugin.Log.LogInfo(string.Format("Layer {0}: the run's end, nothing of the mod's is placed", world.biomeDepth));
				return;
			}
			int num = ((Random.value < Plugin.RareSpawnChance.Value) ? 1 : 0);
			int num2 = Diff.Count(Plugin.ParasitesPerLayer.Value) + Mathf.Min(3, world.biomeDepth / 2);
			int num3 = (LewdSettings.ParasitesOn ? (num2 + num) : 0);
			int num4 = 0;
			int num5 = 0;
			while (num5 < 400 && num4 < num3)
			{
				Vector2 vector;
				vector..ctor(Random.Range(-world.halfWidth, world.halfWidth), Random.Range(-world.halfHeight, world.halfHeight));
				if (Vector2.Distance(vector, b.transform.position) >= 25f && !Physics2D.OverlapPoint(vector, LayerMask.GetMask(new string[] { "Ground" })))
				{
					RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, Vector2.down, 20f, LayerMask.GetMask(new string[] { "Ground" }));
					if (raycastHit2D)
					{
						Parasites.Spawn(raycastHit2D.point + Vector2.up * 0.3f, (num4 < num3 - num) ? Random.Range(0, 3) : 3, false);
						num4++;
					}
				}
				num5++;
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: placed {1} parasites ({2} symbiotes)", world.biomeDepth, num4, Mathf.Min(num4, num)));
			if (LewdSettings.ParasitesOn)
			{
				LatexMites.SeedLayer(b, world);
			}
			if (LewdSettings.TrapsOn)
			{
				Flesh.SeedLayer(b);
				FleshZones.SeedLayer(b);
				Webs.SeedLayer(b);
				Walls.SeedLayer(b);
				Pits.SeedLayer(b);
				Slits.SeedLayer(b);
				Latexes.SeedLayer(b);
				LatexPond.SeedLayer(b);
				LustGas.SeedLayer(b);
				SlimeFluid.SeedLayer(b);
				Tentacles.SeedLayer(b);
			}
			if (LewdSettings.WormsOn)
			{
				Worm.SeedLayer(b);
			}
			if (LewdSettings.On)
			{
				Outfits.SeedLayer(b, Plugin.FreshRun);
			}
			if (Plugin.FreshRun)
			{
				PodRifle.MaybePlace(b);
			}
			if (LewdSettings.On)
			{
				Potion.SeedLayer(b);
			}
			Plugin.FreshRun = false;
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x0004A960 File Offset: 0x00048B60
		public static void ClearAll()
		{
			foreach (ParasiteAI parasiteAI in Parasites.Alive.ToArray())
			{
				if (parasiteAI)
				{
					Object.Destroy(parasiteAI.gameObject);
				}
			}
			Parasites.Alive.Clear();
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x0004A9A8 File Offset: 0x00048BA8
		private static bool LayerFilled(WorldGeneration w)
		{
			int num = 0;
			for (int i = 0; i < 200; i++)
			{
				if (w.GetBlock(new Vector2Int(Random.Range(0, (int)w.width), Random.Range(0, (int)w.height))) != 0)
				{
					num++;
				}
			}
			return num >= 20;
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0004A9F8 File Offset: 0x00048BF8
		private static Vector2 FreeSpot(Vector2 pos, Body b)
		{
			Parasites.<>c__DisplayClass12_0 CS$<>8__locals1;
			CS$<>8__locals1.w = WorldGeneration.world;
			if (!CS$<>8__locals1.w)
			{
				return pos;
			}
			if (Parasites.<FreeSpot>g__Free|12_0(pos, ref CS$<>8__locals1))
			{
				return pos;
			}
			for (int i = 1; i <= 3; i++)
			{
				if (Parasites.<FreeSpot>g__Free|12_0(pos + Vector2.up * (float)i, ref CS$<>8__locals1))
				{
					return pos + Vector2.up * (float)i;
				}
				if (Parasites.<FreeSpot>g__Free|12_0(pos + Vector2.down * (float)i, ref CS$<>8__locals1))
				{
					return pos + Vector2.down * (float)i;
				}
			}
			if (b)
			{
				return new Vector2(b.transform.position.x, Scenes.FeetY(b) + 0.3f);
			}
			return pos;
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x0004AAC0 File Offset: 0x00048CC0
		public static ParasiteAI Spawn(Vector2 pos, int color, bool weakened = false)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				ParasiteArt.EnsureFrames(spriteRenderer.sprite.pixelsPerUnit);
			}
			if (ParasiteArt.Frames.Length == 0)
			{
				return null;
			}
			GameObject gameObject = new GameObject("LewdParasite");
			gameObject.transform.position = Parasites.FreeSpot(pos, body);
			SpriteRenderer spriteRenderer2 = gameObject.AddComponent<SpriteRenderer>();
			bool flag = color == 8 && ParasiteArt.MiteFrames.Length != 0;
			spriteRenderer2.sprite = (flag ? ParasiteArt.MiteFrames[0] : ParasiteArt.Frames[0]);
			spriteRenderer2.color = (flag ? Color.white : ParasiteArt.Colors[color]);
			if (WorldGeneration.world)
			{
				spriteRenderer2.sharedMaterial = WorldGeneration.world.defaultMat;
			}
			if (spriteRenderer)
			{
				spriteRenderer2.sortingLayerID = spriteRenderer.sortingLayerID;
				spriteRenderer2.sortingOrder = spriteRenderer.sortingOrder + 20;
			}
			Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
			rigidbody2D.freezeRotation = true;
			rigidbody2D.mass = 2f;
			rigidbody2D.interpolation = 1;
			BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
			boxCollider2D.size = spriteRenderer2.sprite.bounds.size * new Vector2(0.9f, 0.8f);
			boxCollider2D.offset = spriteRenderer2.sprite.bounds.center;
			BuildingEntity buildingEntity = gameObject.AddComponent<BuildingEntity>();
			buildingEntity.animal = true;
			buildingEntity.health = (weakened ? 15f : 40f);
			buildingEntity.id = ((color == 3) ? "lewd_symbiote" : (flag ? "lewd_latexmite" : "lewd_parasite"));
			if (flag)
			{
				buildingEntity.health = (weakened ? 10f : 25f);
			}
			buildingEntity.ignoreBodyOptimize = true;
			buildingEntity.itemsDropOnDestroy = new ItemDrop[0];
			buildingEntity.alwaysDrop = new ItemDrop[0];
			buildingEntity.itemCategoriesToAdd = new string[0];
			buildingEntity.hitSound = Resources.Load<AudioClip>("Sounds/goo");
			Light2D light2D = gameObject.AddComponent<Light2D>();
			light2D.lightType = 3;
			light2D.color = ParasiteArt.LightColor(color);
			light2D.intensity = ((color == 3 || flag) ? 0.5f : 0.7f);
			light2D.pointLightOuterRadius = 2.8f;
			light2D.pointLightInnerRadius = 0.2f;
			ParasiteAI parasiteAI = gameObject.AddComponent<ParasiteAI>();
			parasiteAI.color = color;
			parasiteAI.weakened = weakened;
			Parasites.Alive.Add(parasiteAI);
			return parasiteAI;
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0004AD7C File Offset: 0x00048F7C
		[CompilerGenerated]
		internal static bool <FreeSpot>g__Free|12_0(Vector2 at, ref Parasites.<>c__DisplayClass12_0 A_1)
		{
			return A_1.w.GetBlock(A_1.w.WorldToBlockPos(at)) == 0 && A_1.w.GetBlock(A_1.w.WorldToBlockPos(at + Vector2.up * 0.6f)) == 0;
		}

		// Token: 0x04000470 RID: 1136
		public static readonly List<ParasiteAI> Alive = new List<ParasiteAI>();

		// Token: 0x04000471 RID: 1137
		private static WorldGeneration seededWorld;

		// Token: 0x04000472 RID: 1138
		private static WorldGeneration pendingWorld;

		// Token: 0x04000473 RID: 1139
		private static int pendingDepth = -1;

		// Token: 0x04000474 RID: 1140
		private static float pendingSince;

		// Token: 0x04000475 RID: 1141
		private static int seededDepth = -1;

		// Token: 0x04000476 RID: 1142
		private static WorldGeneration clearedFor;

		// Token: 0x04000477 RID: 1143
		private static int clearedDepth = -1;
	}
}
