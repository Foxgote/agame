using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PoseKit;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200010D RID: 269
	internal static class Slits
	{
		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060005E2 RID: 1506 RVA: 0x0005D9A8 File Offset: 0x0005BBA8
		// (set) Token: 0x060005E3 RID: 1507 RVA: 0x0005D9AF File Offset: 0x0005BBAF
		public static SlitMouth Holder { get; private set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060005E4 RID: 1508 RVA: 0x0005D9B7 File Offset: 0x0005BBB7
		// (set) Token: 0x060005E5 RID: 1509 RVA: 0x0005D9BE File Offset: 0x0005BBBE
		public static SlitMouth Eater { get; private set; }

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060005E6 RID: 1510 RVA: 0x0005D9C6 File Offset: 0x0005BBC6
		public static bool Holding
		{
			get
			{
				return Slits.Holder;
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060005E7 RID: 1511 RVA: 0x0005D9D2 File Offset: 0x0005BBD2
		public static bool Inside
		{
			get
			{
				return Slits.Eater;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060005E8 RID: 1512 RVA: 0x0005D9DE File Offset: 0x0005BBDE
		public static bool Busy
		{
			get
			{
				return Slits.Holding || Slits.Inside || Slits.striking;
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060005E9 RID: 1513 RVA: 0x0005D9FA File Offset: 0x0005BBFA
		public static bool Hard
		{
			get
			{
				return Slits.Eater && (Slits.ravish || Slits.timeIn > Plugin.PitDigestAfter.Value);
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060005EA RID: 1514 RVA: 0x0005DA24 File Offset: 0x0005BC24
		public static bool Ravishing
		{
			get
			{
				return Slits.Eater && Slits.ravish;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060005EB RID: 1515 RVA: 0x0005DA39 File Offset: 0x0005BC39
		public static int Orgasms
		{
			get
			{
				return Slits.orgasms;
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060005EC RID: 1516 RVA: 0x0005DA40 File Offset: 0x0005BC40
		public static Vector2 ViewCenter
		{
			get
			{
				if (!Slits.Eater)
				{
					return Vector2.zero;
				}
				return Slits.Eater.Belly;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x0005DA5E File Offset: 0x0005BC5E
		public static Vector2 ViewHalf
		{
			get
			{
				return new Vector2(3.15f, 1.15f);
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060005EE RID: 1518 RVA: 0x0005DA6F File Offset: 0x0005BC6F
		public static bool Drowning
		{
			get
			{
				return Slits.Eater && Slits.flooding && SemenFluid.At(PlayerCamera.main.body.limbs[0].transform.position);
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060005EF RID: 1519 RVA: 0x0005DAAB File Offset: 0x0005BCAB
		public static bool Flooding
		{
			get
			{
				return Slits.Eater && Slits.flooding;
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060005F0 RID: 1520 RVA: 0x0005DAC0 File Offset: 0x0005BCC0
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060005F1 RID: 1521 RVA: 0x0005DAC7 File Offset: 0x0005BCC7
		private static WorldGeneration W
		{
			get
			{
				return WorldGeneration.world;
			}
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x0005DAD0 File Offset: 0x0005BCD0
		public static void SeedLayer(Body b)
		{
			if (!Slits.W || Slits.W.biomeOverride != null)
			{
				return;
			}
			int biomeDepth = Slits.W.biomeDepth;
			int num = Diff.Count((biomeDepth == 2) ? Plugin.SlitsSand.Value : ((biomeDepth < 2) ? Plugin.SlitsEarly.Value : 0f));
			if (num <= 0)
			{
				Plugin.Log.LogInfo(string.Format("Layer {0}: no womb slits", biomeDepth));
				return;
			}
			Slits.Art(b);
			if (!SlitArt.Ready)
			{
				Plugin.Log.LogWarning("Womb slits: no art");
				return;
			}
			int num2 = 0;
			int num3 = 0;
			while (num3 < 20000 && num2 < num)
			{
				Vector2Int vector2Int = new Vector2Int(Random.Range(10, (int)(Slits.W.width - 10U)), Random.Range(30, (int)(Slits.W.height - 20U)));
				int num4 = ((Random.value < 0.5f) ? 1 : (-1));
				Vector2Int vector2Int2;
				if (Slits.Fits(vector2Int, num4, out vector2Int2) && Vector2.Distance(Slits.W.BlockToWorldPos(vector2Int2), b.transform.position) >= 30f && !Slits.TooClose(vector2Int2) && Slits.Make(vector2Int2, num4))
				{
					num2++;
				}
				num3++;
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: {1} of {2} womb slits ({3} spots looked at)", new object[] { biomeDepth, num2, num, num3 }));
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x0005DC54 File Offset: 0x0005BE54
		private static void Art(Body b)
		{
			SpriteRenderer spriteRenderer = (b ? b.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				SlitArt.Ensure(spriteRenderer.sprite.pixelsPerUnit / Mathf.Max(0.01f, Mathf.Abs(spriteRenderer.transform.lossyScale.y)));
			}
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x0005DCBF File Offset: 0x0005BEBF
		private static bool Solid(Vector2Int c)
		{
			return Slits.W.GetBlock(c) > 0;
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x0005DCD0 File Offset: 0x0005BED0
		public static bool Fits(Vector2Int c, int dir, out Vector2Int face)
		{
			face = c;
			if (Slits.Solid(c))
			{
				return false;
			}
			int num = 0;
			while (num < 30 && c.y > 4 && !Slits.Solid(c + Vector2Int.down))
			{
				c += Vector2Int.down;
				num++;
			}
			face = c + Vector2Int.up;
			if (face.x - dir * 9 < 3 || (long)(face.x - dir * 9) > (long)((ulong)(Slits.W.width - 3U)) || (long)face.y > (long)((ulong)(Slits.W.height - 10U)))
			{
				return false;
			}
			if (Slits.Solid(face) || Slits.Solid(face + Vector2Int.up))
			{
				return false;
			}
			Vector2Int vector2Int;
			vector2Int..ctor(face.x - dir, face.y);
			for (int i = 0; i < 4; i++)
			{
				if (!Slits.Solid(vector2Int + Vector2Int.up * i))
				{
					return false;
				}
			}
			FluidManager main = FluidManager.main;
			for (int j = 1; j <= 7; j++)
			{
				for (int k = -1; k <= 2; k++)
				{
					Vector2Int vector2Int2;
					vector2Int2..ctor(face.x - dir * j, face.y + k);
					ushort block = Slits.W.GetBlock(vector2Int2);
					if (block == 0)
					{
						return false;
					}
					if (Slits.W.GetBlockInfo(block).health > 2000f)
					{
						return false;
					}
					if (main && main.GetLiquid(vector2Int2.x, vector2Int2.y) != 0)
					{
						return false;
					}
				}
			}
			int num2 = 0;
			for (int l = 0; l <= 5; l++)
			{
				for (int m = 0; m <= 3; m++)
				{
					if (Slits.Solid(new Vector2Int(face.x + dir * l, face.y + m)) && ++num2 > 4)
					{
						return false;
					}
				}
			}
			if (main)
			{
				for (int n = 0; n <= 6; n++)
				{
					for (int num3 = -1; num3 <= 2; num3++)
					{
						if (main.GetLiquid(face.x + dir * n, face.y + num3) != 0)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x0005DF04 File Offset: 0x0005C104
		private static bool TooClose(Vector2Int face)
		{
			return Slits.All.Any<SlitMouth>((SlitMouth s) => Mathf.Abs(s.Face.x - face.x) < 20 && Mathf.Abs(s.Face.y - face.y) < 12) || Pits.All.Any<Pit>((Pit p) => Mathf.Abs(p.Top.x - face.x) < 16 && Mathf.Abs(p.Top.y - face.y) < 16);
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x0005DF50 File Offset: 0x0005C150
		public static SlitMouth Make(Vector2Int face, int dir)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			Slits.Art(body);
			if (!SlitArt.Ready)
			{
				return null;
			}
			SpriteRenderer spriteRenderer = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			GameObject gameObject = new GameObject("LewdSlit");
			SlitMouth s = gameObject.AddComponent<SlitMouth>();
			s.Face = face;
			s.Dir = dir;
			s.seed = Random.value * 10f;
			s.flood = Random.value < Plugin.SlitFloodChance.Value;
			gameObject.transform.position = s.Mouth;
			s.lips = gameObject.AddComponent<SpriteRenderer>();
			s.lips.sprite = SlitArt.Lips[0];
			s.lips.flipX = dir < 0;
			if (Slits.W)
			{
				s.lips.sharedMaterial = Slits.W.defaultMat;
			}
			if (spriteRenderer)
			{
				s.lips.sortingLayerID = spriteRenderer.sortingLayerID;
				s.lips.sortingOrder = spriteRenderer.sortingOrder - 14;
			}
			s.segs = new SpriteRenderer[16];
			for (int i = 0; i < s.segs.Length; i++)
			{
				s.segs[i] = Slits.Part(spriteRenderer, "LewdSlitTentacle", SlitArt.Seg, -13);
			}
			s.tip = Slits.Part(spriteRenderer, "LewdSlitTentacleTip", SlitArt.Tip ? SlitArt.Tip : SlitArt.Seg, -12);
			s.stomach = Slits.Part(spriteRenderer, "LewdSlitStomach", SlitArt.Stomach, -16);
			s.stomach.enabled = false;
			s.stomach.flipX = dir < 0;
			s.stomach.transform.position = s.Mouth;
			s.tipAt = Slits.Rest(s);
			gameObject.layer = LayerMask.NameToLayer("Descriptor");
			BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
			boxCollider2D.size = new Vector2(1.4f, 2.6f);
			boxCollider2D.offset = new Vector2((float)dir * 0.3f, 0f);
			s.cols = new CircleCollider2D[5];
			for (int j = 0; j < s.cols.Length; j++)
			{
				s.cols[j] = gameObject.AddComponent<CircleCollider2D>();
				s.cols[j].radius = 0.35f;
			}
			BuildingEntity buildingEntity = gameObject.AddComponent<BuildingEntity>();
			buildingEntity.animal = true;
			buildingEntity.health = 100000f;
			buildingEntity.id = "lewd_slit";
			buildingEntity.ignoreBodyOptimize = true;
			buildingEntity.itemsDropOnDestroy = new ItemDrop[0];
			buildingEntity.alwaysDrop = new ItemDrop[0];
			buildingEntity.itemCategoriesToAdd = new string[0];
			buildingEntity.hitSound = Resources.Load<AudioClip>("Sounds/goo");
			Burning.Make(gameObject, "womb slit", Plugin.SlitHealth.Value, s.lips, delegate
			{
				Slits.OnHit(s, 0f);
			}, delegate
			{
				Slits.Kill(s);
			});
			Slits.All.Add(s);
			Slits.DrawAll();
			Plugin.Log.LogInfo(string.Format("A womb slit at {0} facing {1}", s.Mouth, (dir > 0) ? "right" : "left"));
			return s;
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x0005E338 File Offset: 0x0005C538
		private static SpriteRenderer Part(SpriteRenderer torso, string name, Sprite sp, int order)
		{
			SpriteRenderer spriteRenderer = new GameObject(name).AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sp;
			if (Slits.W)
			{
				spriteRenderer.sharedMaterial = Slits.W.defaultMat;
			}
			if (torso)
			{
				spriteRenderer.sortingLayerID = torso.sortingLayerID;
				spriteRenderer.sortingOrder = torso.sortingOrder + order;
			}
			return spriteRenderer;
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x0005E398 File Offset: 0x0005C598
		public static int RemoveNear(Vector2 at, float radius)
		{
			int num = 0;
			IEnumerable<SlitMouth> all = Slits.All;
			Func<SlitMouth, bool> <>9__0;
			Func<SlitMouth, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (SlitMouth x) => x && x != Slits.Holder && x != Slits.Eater && x != Slits.striking && Vector2.Distance(x.Mouth, at) < radius);
			}
			foreach (SlitMouth slitMouth in all.Where<SlitMouth>(func).ToArray<SlitMouth>())
			{
				if (slitMouth.open)
				{
					Slits.Refill(slitMouth);
				}
				Object.Destroy(slitMouth.gameObject);
				num++;
			}
			return num;
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x0005E420 File Offset: 0x0005C620
		public static void Clear(Body b)
		{
			if (b && (Slits.Holding || Slits.Inside))
			{
				Slits.Release(b, false);
			}
			Slits.striking = null;
			foreach (SlitMouth slitMouth in Slits.All.ToArray())
			{
				if (slitMouth)
				{
					Object.Destroy(slitMouth.gameObject);
				}
			}
			Slits.All.Clear();
			Slits.ClearGut();
			Slits.spat = null;
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x0005E498 File Offset: 0x0005C698
		public static void Update(Body b, float dt)
		{
			Slits.All.RemoveAll((SlitMouth x) => !x);
			if (!Slits.W || !b)
			{
				return;
			}
			SemenFluid.Tick(b, dt);
			if (Slits.attemptCooldown > 0f)
			{
				Slits.attemptCooldown -= dt;
			}
			if ((Slits.Holding || Slits.Inside) && !b.alive)
			{
				Slits.Release(b, false);
				return;
			}
			if (Slits.spat)
			{
				Slits.SpitWatch(b, Slits.spat, dt);
			}
			foreach (SlitMouth slitMouth in Slits.All)
			{
				if (slitMouth.sated > 0f)
				{
					slitMouth.sated -= dt;
				}
				if (slitMouth.flinch > 0f)
				{
					slitMouth.flinch -= dt;
				}
			}
			if (Slits.bombSlit)
			{
				Slits.Bomb(b, dt);
			}
			else if (!Slits.Inside)
			{
				Slits.Bait(b, dt);
			}
			if (Slits.Inside)
			{
				Slits.InStomach(b, dt);
				Slits.DrawAll();
				return;
			}
			if (Slits.Holding)
			{
				Slits.Haul(b, dt);
				Slits.DrawAll();
				return;
			}
			if (Slits.striking)
			{
				Slits.Strike(b, dt);
				Slits.DrawAll();
				return;
			}
			if (b.alive && !Scenes.Busy && !HerPuppet.Active)
			{
				Vector2 vector = b.limbs[1].transform.position;
				foreach (SlitMouth slitMouth2 in Slits.All)
				{
					if (!slitMouth2.dead && !slitMouth2.open && slitMouth2.sated <= 0f && slitMouth2.flinch <= 0f)
					{
						float num = (vector.x - slitMouth2.FaceX) * (float)slitMouth2.Dir;
						if (num >= 0.4f && num <= Plugin.SlitReach.Value && Mathf.Abs(vector.y - slitMouth2.Mouth.y) <= 4f && !Physics2D.Linecast(slitMouth2.Mouth + Vector2.right * (float)slitMouth2.Dir * 0.6f, vector, LayerMask.GetMask(new string[] { "Ground" })))
						{
							Slits.striking = slitMouth2;
							Slits.strikeTime = 0f;
							Slits.held = Slits.Pick(b);
							Sound.Play("goo", slitMouth2.Mouth, false, true, null, 0.9f, 0.7f, false, false);
							Sound.Play("megabite", slitMouth2.Mouth, false, true, null, 0.5f, 1.3f, false, false);
							break;
						}
					}
				}
			}
			Slits.DrawAll();
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x0005E7D4 File Offset: 0x0005C9D4
		private static Limb Pick(Body b)
		{
			string[] array;
			if (Random.value >= 0.75f)
			{
				(array = new string[1])[0] = "DownTorso";
			}
			else
			{
				string[] array2 = new string[3];
				array2[0] = "FootF";
				array2[1] = "FootB";
				array = array2;
				array2[2] = "CrusF";
			}
			foreach (string text in array)
			{
				Limb limb = b.LimbByName(text);
				if (limb && !limb.dismembered && limb.rb)
				{
					return limb;
				}
			}
			return b.limbs[1];
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x0005E860 File Offset: 0x0005CA60
		private static void Strike(Body b, float dt)
		{
			SlitMouth slitMouth = Slits.striking;
			if (!slitMouth || slitMouth.dead || !Slits.held)
			{
				Slits.striking = null;
				return;
			}
			Slits.strikeTime += dt;
			float num = Mathf.Clamp01(Slits.strikeTime / 0.45f);
			slitMouth.tipAt = Vector2.Lerp(Slits.Rest(slitMouth), Slits.held.transform.position, num * num);
			slitMouth.lips.sprite = SlitArt.Lips[Mathf.Min(1, SlitArt.Lips.Length - 1)];
			if (num < 1f)
			{
				return;
			}
			Slits.striking = null;
			float num2 = (Slits.held.transform.position.x - slitMouth.FaceX) * (float)slitMouth.Dir;
			if (b.alive && !Scenes.Busy && !HerPuppet.Active && num2 > 0f && num2 < Plugin.SlitReach.Value + 1.5f)
			{
				Slits.Grab(b, slitMouth);
				return;
			}
			slitMouth.sated = 3f;
			Plugin.Log.LogInfo("The womb slit's tentacle missed her");
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x0005E984 File Offset: 0x0005CB84
		private static void Grab(Body b, SlitMouth s)
		{
			Slits.Holder = s;
			Slits.holdTime = 0f;
			Slits.haul = Slits.held.transform.position;
			Slits.attemptCooldown = 0.6f;
			Slits.lineTimer = 2.5f;
			Relief.ForceStop();
			b.Ragdoll();
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			b.eyePanicTime = 1f;
			b.adrenaline += 30f;
			b.Scream();
			Sound.Play("goo", Slits.held.transform.position, false, true, null, 1f, 0.5f, false, false);
			Sfx.PlayMix(b, 0.8f, new string[] { "tentacle_writhe" });
			Relief.Say(b, "slit_grab");
			Plugin.Log.LogInfo("The womb slit's tentacle has her by the " + Slits.held.name);
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x0005EA88 File Offset: 0x0005CC88
		private static void Haul(Body b, float dt)
		{
			SlitMouth holder = Slits.Holder;
			if (!holder || holder.dead || !Slits.held || Slits.held.dismembered)
			{
				Slits.Release(b, false);
				return;
			}
			Slits.holdTime += dt;
			b.shock = Mathf.Max(b.shock, 20f);
			b.eyePanicTime = Mathf.Max(b.eyePanicTime, 0.3f);
			if (Keys.Down(Plugin.RemoveKey.Value))
			{
				Slits.Struggle(b);
			}
			if (!Slits.Holder)
			{
				return;
			}
			if (Vector2.Distance(Slits.held.transform.position, holder.Mouth) > Plugin.SlitReach.Value + 4f)
			{
				Plugin.Log.LogInfo("The womb slit's tentacle lost her: she is out of its reach");
				Slits.Release(b, false);
				return;
			}
			Vector2 vector = holder.Mouth + new Vector2((float)holder.Dir * 0.3f, -0.3f);
			float num = Mathf.Max(0.5f, Plugin.SlitPullStrength.Value);
			if (Vector2.Distance(Slits.haul, Slits.held.transform.position) < 1.2f)
			{
				Slits.haul = Vector2.MoveTowards(Slits.haul, vector, Plugin.SlitDragSpeed.Value * dt);
			}
			Vector2 vector2 = -Physics2D.gravity * dt;
			Vector2 vector3 = Slits.held.transform.position;
			foreach (Limb limb in b.limbs)
			{
				if (limb && !limb.dismembered && limb.rb)
				{
					Vector2 vector4 = limb.transform.position;
					if (limb == Slits.held)
					{
						Vector2 vector5 = Vector2.ClampMagnitude((Slits.haul - vector4) * 8f * num, 6f + 3f * num);
						limb.rb.velocity = Vector2.Lerp(limb.rb.velocity, vector5, Mathf.Clamp01(dt * 14f));
					}
					else
					{
						limb.rb.velocity += vector2 * limb.rb.gravityScale * Mathf.Clamp01(num / 3f) * 0.85f;
						float num2 = Vector2.Distance(vector4, vector3) - 2.4f;
						if (num2 > 0f)
						{
							limb.rb.velocity = Vector2.Lerp(limb.rb.velocity, (vector3 - vector4).normalized * Mathf.Min(10f, num2 * 6f * num), Mathf.Clamp01(dt * 8f));
						}
					}
				}
			}
			holder.tipAt = Slits.held.transform.position;
			holder.lips.sprite = SlitArt.Lips[Mathf.Min(1 + (int)(Time.time * 2f) % 2, SlitArt.Lips.Length - 1)];
			if ((Slits.writhe -= dt) <= 0f)
			{
				Slits.writhe = Random.Range(1.4f, 2.6f);
				Sfx.PlayMix(b, 0.6f, new string[] { "tentacle_writhe" });
			}
			if ((Slits.lineTimer -= dt) <= 0f)
			{
				Slits.lineTimer = Random.Range(4f, 7f);
				Relief.Say(b, (Vector2.Distance(Slits.held.transform.position, holder.Mouth) < 2.5f) ? "slit_close" : "slit_drag");
			}
			if (Vector2.Distance(Slits.held.transform.position, holder.Mouth) < 1.3f)
			{
				Slits.Swallow(b, holder);
			}
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x0005EEAC File Offset: 0x0005D0AC
		private static void Spring(Limb l, Vector2 target, float stiffness, float dt)
		{
			if (!l || l.dismembered || !l.rb)
			{
				return;
			}
			Vector2 vector = (target - l.transform.position) * stiffness;
			l.rb.velocity = Vector2.Lerp(l.rb.velocity, Vector2.ClampMagnitude(vector, 12f), Mathf.Clamp01(dt * 12f));
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x0005EF26 File Offset: 0x0005D126
		private static void Spring(Body b, string name, Vector2 target, float stiffness, float dt)
		{
			Slits.Spring(b.LimbByName(name), target, stiffness, dt);
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x0005EF38 File Offset: 0x0005D138
		private static void Struggle(Body b)
		{
			if (Slits.attemptCooldown > 0f || !b.conscious)
			{
				return;
			}
			Slits.attemptCooldown = 1f;
			float num = Mathf.Clamp(b.stamina / 100f, 0.1f, 1f) * (1f - Slits.S.exhaustion / 100f);
			float num2 = Mathf.Max(0.01f, Plugin.SlitEscape.Value * Diff.Escape * num * (1f - Slits.S.submission * 0.004f));
			b.stamina = Mathf.Max(0f, b.stamina - 10f);
			foreach (string text in new string[] { "FootF", "FootB", "HandF", "HandB" })
			{
				Limb limb = b.LimbByName(text);
				if (limb && !limb.dismembered && limb.rb)
				{
					limb.rb.velocity += Random.insideUnitCircle * 6f;
				}
			}
			bool flag = Random.value < num2;
			Plugin.Log.LogInfo(string.Format("Struggling in the womb slit's tentacle: chance {0:P0}, {1}", num2, flag ? "free" : "held"));
			if (flag)
			{
				SlitMouth holder = Slits.Holder;
				holder.sated = 20f;
				Relief.Say(b, "slit_escape");
				Slits.Release(b, false);
				foreach (Limb limb2 in b.limbs)
				{
					if (limb2 && !limb2.dismembered && limb2.rb)
					{
						limb2.rb.velocity = new Vector2((float)holder.Dir * 4f, 2f);
					}
				}
				return;
			}
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.7f, 0.6f, false, false);
			Relief.Say(b, "slit_fail");
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x0005F174 File Offset: 0x0005D374
		private static void Swallow(Body b, SlitMouth s)
		{
			Slits.Holder = null;
			Slits.Open(s);
			Slits.Eater = s;
			Slits.timeIn = 0f;
			Slits.pulse = 2f;
			Slits.lineTimer = 3f;
			Slits.attemptCooldown = 0.8f;
			Slits.ravish = Random.value < Plugin.PitRavishChance.Value;
			Slits.flooding = false;
			Slits.floodTimer = 0f;
			Slits.floodStep = 0;
			Slits.wasUnder = false;
			Slits.spent = false;
			Slits.orgasms = 0;
			Slits.orgasmGap = 3f;
			Slits.Shift(b, s.Belly - b.limbs[1].transform.position);
			foreach (Limb limb in b.limbs)
			{
				if (limb && !limb.dismembered && limb.rb)
				{
					limb.rb.velocity = Vector2.zero;
				}
			}
			foreach (Item item in Clothes.Worn(b).ToArray<Item>())
			{
				item.condition = Mathf.Max(0.03f, item.condition - 0.12f);
			}
			Slits.overOrder = b.GetComponentsInChildren<SpriteRenderer>(true).Max<SpriteRenderer>((SpriteRenderer r) => r.sortingOrder);
			s.lips.sortingOrder = Slits.overOrder + 10;
			s.stomach.enabled = true;
			s.stomach.transform.position = s.Mouth;
			Slits.ClearGut();
			SpriteRenderer torso = b.limbs[1].GetComponent<SpriteRenderer>();
			Func<int, SpriteRenderer> <>9__1;
			for (int j = 0; j < Slits.GutHold.Length; j++)
			{
				List<SpriteRenderer[]> list = Slits.gut;
				IEnumerable<int> enumerable = Enumerable.Range(0, 6);
				Func<int, SpriteRenderer> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (int k) => Slits.Part(torso, "LewdSlitGut", SlitArt.Seg, 22));
				}
				list.Add(enumerable.Select<int, SpriteRenderer>(func).ToArray<SpriteRenderer>());
			}
			Slits.wetLoop = (Sfx.Has("loop_gut") ? Sfx.Loop(b, "loop_gut") : Sfx.Loop(b, "loop_wet"));
			Sound.Play("eatFlesh", s.Mouth, false, true, null, 1f, 0.6f, false, false);
			Sfx.PlayMix(b, 0.9f, new string[] { "tentacle_thrust" });
			PlayerCamera main = PlayerCamera.main;
			if (main != null)
			{
				main.shaker.Shake(40f);
			}
			Relief.Say(b, "slit_swallowed");
			Plugin.Log.LogInfo("The womb slit hauled her into its stomach (" + (Slits.ravish ? "it uses her" : "it digests") + ")");
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x0005F440 File Offset: 0x0005D640
		private static void Open(SlitMouth s)
		{
			if (s.open)
			{
				return;
			}
			s.ground = new Dictionary<Vector2Int, ushort>();
			foreach (Vector2Int vector2Int in s.Cells().Concat<Vector2Int>(s.Walls()))
			{
				s.ground[vector2Int] = Slits.W.GetBlock(vector2Int);
			}
			s.rim.Clear();
			foreach (Vector2Int vector2Int2 in s.Walls())
			{
				s.rim.Add(vector2Int2);
				if (Slits.W.GetBlock(vector2Int2) == 0)
				{
					Slits.W.SetBlock(vector2Int2, 16);
				}
				FluidManager main = FluidManager.main;
				if (main != null)
				{
					main.SetLiquid(vector2Int2.x, vector2Int2.y, 0);
				}
			}
			foreach (Vector2Int vector2Int3 in s.Cells())
			{
				if (Slits.W.GetBlock(vector2Int3) != 0)
				{
					Slits.W.SetBlock(vector2Int3, 0);
				}
				FluidManager main2 = FluidManager.main;
				if (main2 != null)
				{
					main2.SetLiquid(vector2Int3.x, vector2Int3.y, 0);
				}
			}
			s.open = true;
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x0005F5B8 File Offset: 0x0005D7B8
		private static void Refill(SlitMouth s)
		{
			FluidManager main = FluidManager.main;
			if (main)
			{
				foreach (Vector2Int vector2Int in s.Cells())
				{
					if (main.GetLiquid(vector2Int.x, vector2Int.y) != 0)
					{
						main.SetLiquid(vector2Int.x, vector2Int.y, 0);
					}
				}
			}
			s.mouthShut = false;
			if (s.ground != null)
			{
				foreach (KeyValuePair<Vector2Int, ushort> keyValuePair in s.ground)
				{
					if (Slits.W.GetBlock(keyValuePair.Key) != keyValuePair.Value)
					{
						Slits.W.SetBlock(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			s.ground = null;
			s.open = false;
			s.rim.Clear();
			if (s.stomach)
			{
				s.stomach.enabled = false;
			}
			Plugin.Log.LogInfo(string.Format("The womb slit's stomach at {0} closed up", s.Mouth));
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x0005F708 File Offset: 0x0005D908
		private static bool Broken(SlitMouth s)
		{
			int num = 0;
			foreach (Vector2Int vector2Int in s.rim)
			{
				if (Slits.W.GetBlock(vector2Int) == 0 && ++num > 2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x0005F774 File Offset: 0x0005D974
		private static void InStomach(Body b, float dt)
		{
			SlitMouth eater = Slits.Eater;
			if (!eater)
			{
				Slits.Release(b, false);
				return;
			}
			if (Slits.Broken(eater))
			{
				Plugin.Log.LogInfo("The womb slit's stomach was broken open");
				Slits.Kill(eater);
				return;
			}
			if (Burning.LitTorchInHand(b) && Slits.attemptCooldown <= 0f)
			{
				Slits.attemptCooldown = 1f;
				FleshBurn component = eater.GetComponent<FleshBurn>();
				if (component)
				{
					Burning.Damage(component, Plugin.FireTorchHit.Value, "the torch in its stomach", true);
				}
				if (Slits.Eater != eater)
				{
					return;
				}
				Relief.Say(b, "pit_torch_spit");
				Slits.Release(b, true);
				return;
			}
			else
			{
				if (!eater.InStomach(b.limbs[1].transform.position, 2.5f))
				{
					Plugin.Log.LogInfo("She is out of the womb slit");
					Slits.Release(b, false);
					return;
				}
				Slits.timeIn += dt;
				b.shock = Mathf.Max(b.shock, 12f);
				if (b.rb)
				{
					b.rb.velocity = Vector2.zero;
				}
				if (Keys.Down(Plugin.RemoveKey.Value))
				{
					Slits.Struggle2(b);
				}
				if (!Slits.Eater)
				{
					return;
				}
				if (eater.flood)
				{
					Slits.Flood(b, eater, dt);
				}
				Vector2 vector;
				vector..ctor((float)eater.Dir, 0f);
				Vector2 vector2 = eater.Belly - (eater.mouthShut ? (vector * 0.5f) : Vector2.zero);
				float num = Mathf.Sin(Time.time * (Slits.Hard ? 4.5f : 2.6f)) * 0.06f;
				Slits.Spring(b, "Head", vector2 + vector * 1.9f + Vector2.up * num, 5f, dt);
				Slits.Spring(b, "UpTorso", vector2 + vector * 1f, 5f, dt);
				Slits.Spring(b, "DownTorso", vector2 + vector * 0.1f, 5f, dt);
				Slits.Spring(b, "ThighF", vector2 - vector * 0.7f, 4f, dt);
				Slits.Spring(b, "ThighB", vector2 - vector * 0.7f + Vector2.down * 0.1f, 4f, dt);
				Slits.Spring(b, "FootF", vector2 - vector * 2f, 4f, dt);
				Slits.Spring(b, "FootB", vector2 - vector * 2f + Vector2.down * 0.15f, 4f, dt);
				Slits.Gut(b, eater);
				eater.lips.sprite = SlitArt.Lips[0];
				Slits.S.arousal = Mathf.Min(100f, Slits.S.arousal + 2f * dt);
				Slits.S.lust = Mathf.Min(100f, Slits.S.lust + 0.4f * dt);
				if (Slits.wetLoop)
				{
					Slits.wetLoop.volume = Mathf.MoveTowards(Slits.wetLoop.volume, 0.45f, dt);
				}
				if (Slits.ravish)
				{
					Slits.S.arousal = Mathf.Min(100f, Slits.S.arousal + 1.6f * dt);
				}
				if ((Slits.pulse -= dt) <= 0f)
				{
					Slits.pulse = (Slits.ravish ? Random.Range(2.2f, 4f) : Random.Range(3.5f, 6f));
					Slits.S.arousal = Mathf.Min(100f, Slits.S.arousal + (Slits.ravish ? 11f : 7f));
					b.eatTime = Mathf.Max(b.eatTime, 0.6f);
					Sfx.Play(b, 0.75f, new string[] { "muffled", "moan" });
					Sfx.Play(b, 0.4f, new string[] { "drip" });
					Sfx.PlayMix(b, 0.7f, new string[] { "tentacle_thrust" });
					Semen.Add(b, 3f);
					Womb.Fill(b, 4f);
				}
				if ((Slits.lineTimer -= dt) <= 0f)
				{
					Slits.lineTimer = Random.Range(6f, 10f);
					bool flag = SemenFluid.At(b.limbs[0].transform.position);
					Relief.Say(b, Slits.flooding ? (flag ? "slit_flood_under" : "slit_flood_up") : (Slits.ravish ? ((Slits.orgasms >= Plugin.PitOrgasmsToDeath.Value - 2) ? "pit_ravish_spent" : "pit_ravish") : ((Slits.timeIn < Plugin.PitDigestAfter.Value) ? "slit_inside" : "slit_digest")));
				}
				if (Slits.ravish)
				{
					if (Slits.spent)
					{
						b.bloodOxygen = 0f;
					}
					Slits.orgasmGap -= dt;
					if (Slits.S.arousal >= 99f && Slits.orgasmGap <= 0f && b.conscious)
					{
						Slits.orgasmGap = 4f;
						Plugin.Orgasm(b, false);
						Slits.orgasms++;
						Slits.S.arousal = Mathf.Max(Slits.S.arousal, 45f + 6f * (float)Slits.orgasms);
						b.bloodOxygen = Mathf.Max(0f, b.bloodOxygen - Plugin.FleshWallOrgasmAir.Value * Diff.Lethal);
						b.stamina = Mathf.Max(0f, b.stamina - 20f);
						Semen.Add(b, 8f);
						Womb.Fill(b, 10f);
						Relief.Say(b, "pit_ravish_come");
						Plugin.Log.LogInfo(string.Format("The womb slit forced orgasm {0} out of her (oxygen {1:0})", Slits.orgasms, b.bloodOxygen));
						if (Slits.orgasms >= Plugin.PitOrgasmsToDeath.Value && Diff.LethalScenes)
						{
							Relief.Say(b, "pit_ravish_end");
							b.bloodOxygen = 0f;
							Slits.spent = true;
							Plugin.Log.LogInfo("The womb slit used her to death");
						}
					}
					return;
				}
				if (Slits.timeIn > Plugin.PitDigestAfter.Value)
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

		// Token: 0x06000608 RID: 1544 RVA: 0x0005FEEC File Offset: 0x0005E0EC
		private static void Struggle2(Body b)
		{
			if (Slits.attemptCooldown > 0f || !b.conscious)
			{
				return;
			}
			Slits.attemptCooldown = 1f;
			b.stamina = Mathf.Max(0f, b.stamina - 9f);
			float num = Plugin.PitEscape.Value * Diff.Escape * ((b.stamina < 15f) ? 0.5f : 1f) * (1f - Slits.S.submission * 0.003f) * ((Slits.S.orgasmTimer > 15f) ? 0.5f : 1f);
			bool flag = Random.value < num;
			Plugin.Log.LogInfo(string.Format("Struggling in the womb slit's stomach: chance {0:P0}, {1}", num, flag ? "free" : "held"));
			if (flag)
			{
				Relief.Say(b, "pit_escape");
				Slits.Release(b, true);
				return;
			}
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.8f, 0.6f, false, false);
			Relief.Say(b, "pit_fail");
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x00060018 File Offset: 0x0005E218
		private static void Gut(Body b, SlitMouth s)
		{
			Vector2 belly = s.Belly;
			int num = 0;
			while (num < Slits.gut.Count && num < Slits.GutHold.Length)
			{
				Limb limb = b.LimbByName(Slits.GutHold[num]);
				if (!limb || limb.dismembered)
				{
					limb = b.limbs[1];
				}
				float num2 = (float)(num - 2) * 0.9f * (float)s.Dir;
				Vector2 vector = belly + new Vector2(num2, (num % 2 == 0) ? 0.85f : (-0.85f));
				Slits.Chain(Slits.gut[num], vector, limb.transform.position, 0.45f, 0.6f, true, (float)num);
				num++;
			}
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x000600DC File Offset: 0x0005E2DC
		private static void ClearGut()
		{
			foreach (SpriteRenderer[] array in Slits.gut)
			{
				foreach (SpriteRenderer spriteRenderer in array)
				{
					if (spriteRenderer)
					{
						Object.Destroy(spriteRenderer.gameObject);
					}
				}
			}
			Slits.gut.Clear();
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00060158 File Offset: 0x0005E358
		public static void LetGo(Body b)
		{
			if (Slits.Holding)
			{
				Slits.Holder.sated = 20f;
				Slits.Release(b, false);
				return;
			}
			if (Slits.Inside)
			{
				Slits.Release(b, true);
			}
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x00060188 File Offset: 0x0005E388
		private static void Release(Body b, bool spit)
		{
			SlitMouth s = (Slits.Holder ? Slits.Holder : Slits.Eater);
			bool flag = Slits.Eater;
			Slits.Holder = null;
			Slits.Eater = null;
			Slits.held = null;
			Slits.ClearGut();
			Engulfed.Hide();
			if (Slits.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(Slits.wetLoop, 0.5f));
			}
			Slits.wetLoop = null;
			if (s)
			{
				s.sated = Mathf.Max(s.sated, 25f);
				s.lips.sortingOrder = (b ? b.limbs[1].GetComponent<SpriteRenderer>().sortingOrder : s.lips.sortingOrder) - 14;
				s.tipAt = Slits.Rest(s);
			}
			if (!b)
			{
				return;
			}
			Traverse.Create(b).Field("movingAllowed").SetValue(true);
			if (!flag || !s)
			{
				return;
			}
			if (s.mouthShut)
			{
				s.mouthShut = false;
				IEnumerable<Vector2Int> enumerable = s.Cells();
				Func<Vector2Int, bool> <>9__0;
				Func<Vector2Int, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (Vector2Int c) => c.x == s.Face.x - s.Dir);
				}
				foreach (Vector2Int vector2Int in enumerable.Where<Vector2Int>(func))
				{
					if (Slits.W.GetBlock(vector2Int) != 0)
					{
						Slits.W.SetBlock(vector2Int, 0);
					}
				}
			}
			if (spit && b.alive)
			{
				Slits.Shift(b, s.Mouth + new Vector2((float)s.Dir * 0.2f, 0f) - b.limbs[1].transform.position);
				b.Ragdoll();
				foreach (Limb limb in b.limbs)
				{
					if (limb && !limb.dismembered && limb.rb)
					{
						limb.rb.velocity = new Vector2((float)s.Dir * 9f, 3f);
					}
				}
				b.Scream();
				Sound.Play("vomit", s.Mouth, false, true, null, 0.9f, 1f, false, false);
				Sound.Play("goo", s.Mouth, false, true, null, 1f, 0.6f, false, false);
				PlayerCamera main = PlayerCamera.main;
				if (main != null)
				{
					main.shaker.Shake(40f);
				}
				b.wetness = Mathf.Max(b.wetness, 40f);
				Semen.Add(b, 15f);
				Plugin.Log.LogInfo("The womb slit spat her out");
			}
			Slits.spat = s;
			Slits.spitTime = 0f;
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x000604E0 File Offset: 0x0005E6E0
		private static void SpitWatch(Body b, SlitMouth s, float dt)
		{
			if (Slits.Eater == s)
			{
				Slits.spat = null;
				return;
			}
			Slits.spitTime += dt;
			bool flag = !b || b.limbs.All<Limb>((Limb l) => !l || l.dismembered || !s.InStomach(l.transform.position, 0.3f));
			FluidManager fm = FluidManager.main;
			bool flag2 = fm && s.Cells().Any<Vector2Int>((Vector2Int c) => fm.GetLiquid(c.x, c.y) > 0);
			if (flag && Slits.spitTime > 0.3f && (!flag2 || Slits.spitTime > 6f))
			{
				if (s.open)
				{
					Slits.Refill(s);
				}
				Slits.spat = null;
			}
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x000605B4 File Offset: 0x0005E7B4
		public static void OnHit(SlitMouth s, float damage)
		{
			if (!s || s.dead)
			{
				return;
			}
			BuildingEntity component = s.GetComponent<BuildingEntity>();
			if (component)
			{
				component.health = 100000f;
			}
			if (Slits.Eater == s)
			{
				return;
			}
			s.hits = ((Time.time - s.lastHit < 6f) ? (s.hits + 1) : 1);
			s.lastHit = Time.time;
			Sound.Play("goo", s.Mouth, false, true, null, 0.9f, Random.Range(1.1f, 1.4f), false, false);
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (Slits.striking == s)
			{
				Slits.striking = null;
				s.sated = 2f;
			}
			if (!(Slits.Holder == s))
			{
				s.flinch = Mathf.Max(s.flinch, ((float)s.hits >= Plugin.SlitLetGoHits.Value) ? 15f : 3f);
				return;
			}
			if ((float)s.hits < Plugin.SlitLetGoHits.Value)
			{
				return;
			}
			Plugin.Log.LogInfo("The womb slit was shot enough: it lets go of her");
			s.flinch = 15f;
			s.hits = 0;
			if (body)
			{
				Relief.Say(body, "slit_shot_free");
				Slits.Release(body, false);
			}
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x00060718 File Offset: 0x0005E918
		public static void Kill(SlitMouth s)
		{
			if (!s || s.dead)
			{
				return;
			}
			s.dead = true;
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (Slits.Holder == s || Slits.Eater == s)
			{
				Slits.Release(body, true);
			}
			if (Slits.striking == s)
			{
				Slits.striking = null;
			}
			Sound.Play("gore", s.Mouth, false, true, null, 1f, 1f, false, false);
			Sound.Play("goo", s.Mouth, false, true, null, 1f, 0.5f, false, false);
			if (body && body.alive && Vector2.Distance(body.transform.position, s.Mouth) < 16f)
			{
				Relief.Say(body, "slit_killed");
			}
			Plugin.Log.LogInfo(string.Format("The womb slit at {0} is dead", s.Mouth));
			if (Slits.bombSlit == s)
			{
				Slits.DropBomb();
			}
			if (s.open && Slits.spat != s)
			{
				Slits.Refill(s);
			}
			Slits.All.Remove(s);
			Object.Destroy(s.gameObject);
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x0006086C File Offset: 0x0005EA6C
		private static Vector2 Rest(SlitMouth s)
		{
			float num = Time.time * 0.7f + s.seed;
			float num2 = Plugin.SlitReach.Value * ((s.flinch > 0f) ? 0.2f : 0.65f) + Mathf.Sin(num) * 0.6f;
			float num3 = s.FaceX + (float)s.Dir * num2;
			RaycastHit2D raycastHit2D = Physics2D.Raycast(new Vector2(num3, s.Mouth.y + 1.5f), Vector2.down, 8f, LayerMask.GetMask(new string[] { "Ground" }));
			return new Vector2(num3, raycastHit2D ? (raycastHit2D.point.y + 0.2f) : (s.Mouth.y - 1f));
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x0006093C File Offset: 0x0005EB3C
		private static void DrawAll()
		{
			foreach (SlitMouth slitMouth in Slits.All)
			{
				if (slitMouth && !slitMouth.dead)
				{
					if (slitMouth == Slits.bombSlit)
					{
						Slits.DrawBomb(slitMouth);
					}
					else
					{
						bool flag = slitMouth == Slits.Holder || slitMouth == Slits.striking;
						if (!flag && slitMouth != Slits.Eater)
						{
							slitMouth.tipAt = Vector2.Lerp(slitMouth.tipAt, Slits.Rest(slitMouth), Time.deltaTime * 2f);
						}
						if (!flag && slitMouth != Slits.Eater && slitMouth.lips)
						{
							slitMouth.lips.sprite = SlitArt.Lips[0];
						}
						bool flag2 = slitMouth != Slits.Eater;
						Vector2 vector = slitMouth.Mouth + Vector2.right * (float)slitMouth.Dir * 0.15f;
						if (flag2)
						{
							Slits.Chain(slitMouth.segs, vector, slitMouth.tipAt, 1f, (slitMouth == Slits.Holder) ? 0.4f : 1f, slitMouth == Slits.Holder, slitMouth.seed);
						}
						else
						{
							foreach (SpriteRenderer spriteRenderer in slitMouth.segs)
							{
								if (spriteRenderer)
								{
									spriteRenderer.enabled = false;
								}
							}
						}
						if (slitMouth.tip)
						{
							slitMouth.tip.enabled = flag2 && slitMouth != Slits.Holder;
							Vector2 vector2 = (slitMouth.segs[slitMouth.segs.Length - 1] ? slitMouth.segs[slitMouth.segs.Length - 1].transform.position : vector);
							Vector2 vector3 = slitMouth.tipAt - vector2;
							slitMouth.tip.transform.position = slitMouth.tipAt;
							slitMouth.tip.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector3.y, vector3.x) * 57.29578f);
						}
						for (int j = 0; j < slitMouth.cols.Length; j++)
						{
							SpriteRenderer spriteRenderer2 = slitMouth.segs[Mathf.Min(slitMouth.segs.Length - 1, 2 + j * 3)];
							if (slitMouth.cols[j] && spriteRenderer2)
							{
								slitMouth.cols[j].offset = spriteRenderer2.transform.position - slitMouth.transform.position;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x00060C3C File Offset: 0x0005EE3C
		private static void Chain(SpriteRenderer[] segs, Vector2 from, Vector2 to, float thick, float wiggle, bool coil, float seed)
		{
			if (segs == null || !SlitArt.Seg)
			{
				return;
			}
			float x = SlitArt.Seg.bounds.size.x;
			int num = (coil ? (segs.Length - 3) : segs.Length);
			Vector2 vector = to - from;
			Vector2 normalized = new Vector2(-vector.y, vector.x).normalized;
			Vector2 vector2 = from;
			float num2 = Time.time * 2.6f + seed;
			float num3 = Mathf.Clamp(vector.magnitude / 2.5f, 1f, 4f);
			for (int i = 0; i < segs.Length; i++)
			{
				SpriteRenderer spriteRenderer = segs[i];
				if (spriteRenderer)
				{
					float num4 = Mathf.Min(1f, ((float)i + 1f) / (float)num);
					Vector2 vector3 = ((i < num) ? (from + vector * num4 + normalized * Mathf.Sin(Time.time * 3.2f + seed + num4 * num3 * 3.1415927f) * 0.35f * wiggle * Mathf.Sin(num4 * 3.1415927f)) : (to + new Vector2(Mathf.Cos(num2 + (float)(i - num) * 2.1f) * 0.28f, Mathf.Sin(num2 + (float)(i - num) * 2.1f) * 0.14f)));
					Vector2 vector4 = vector3 - vector2;
					spriteRenderer.enabled = vector4.sqrMagnitude > 1E-06f;
					spriteRenderer.transform.position = vector2;
					spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector4.y, vector4.x) * 57.29578f);
					spriteRenderer.transform.localScale = new Vector3(vector4.magnitude / x + 0.1f, thick * Mathf.Lerp(1.1f, 0.55f, num4), 1f);
					vector2 = vector3;
				}
			}
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x00060E58 File Offset: 0x0005F058
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

		// Token: 0x06000614 RID: 1556 RVA: 0x00060EE0 File Offset: 0x0005F0E0
		public static SlitMouth DebugMake(Body b)
		{
			if (!Slits.W)
			{
				return null;
			}
			Vector2Int vector2Int = Slits.W.WorldToBlockPos(b.transform.position);
			SlitMouth slitMouth = null;
			int num = 4;
			while (num <= 60 && !slitMouth)
			{
				int num2 = 0;
				while (num2 < 40 && !slitMouth)
				{
					Vector2Int vector2Int2 = vector2Int + new Vector2Int(Random.Range(-num, num + 1), Random.Range(-num / 2, num / 2 + 1));
					foreach (int num3 in new int[] { 1, -1 })
					{
						Vector2Int vector2Int3;
						if (Slits.Fits(vector2Int2, num3, out vector2Int3) && !Slits.TooClose(vector2Int3) && Vector2.Distance(Slits.W.BlockToWorldPos(vector2Int3), b.transform.position) > 3f)
						{
							slitMouth = Slits.Make(vector2Int3, num3);
							break;
						}
					}
					num2++;
				}
				num += 2;
			}
			return slitMouth;
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x00060FE8 File Offset: 0x0005F1E8
		private static bool Lit(Item it, out CustomItemBehaviour c)
		{
			c = (it ? it.GetComponent<CustomItemBehaviour>() : null);
			bool flag;
			bool flag2;
			if (c && it.id == "dynamite" && c.data != null && c.data.Length != 0)
			{
				object obj = c.data[0];
				if (obj is bool)
				{
					flag = (bool)obj;
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
			}
			else
			{
				flag2 = false;
			}
			return flag2 && flag;
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00061058 File Offset: 0x0005F258
		private static void Bait(Body b, float dt)
		{
			if ((Slits.baitScan -= dt) > 0f || Slits.All.Count == 0)
			{
				return;
			}
			Slits.baitScan = 0.25f;
			foreach (CustomItemBehaviour customItemBehaviour in Object.FindObjectsOfType<CustomItemBehaviour>())
			{
				Item item = (customItemBehaviour ? customItemBehaviour.GetComponent<Item>() : null);
				CustomItemBehaviour customItemBehaviour2;
				if (Slits.Lit(item, out customItemBehaviour2) && !item.ParentContainer() && (!b || !item.transform.IsChildOf(b.transform)))
				{
					Vector2 vector = item.transform.position;
					foreach (SlitMouth slitMouth in Slits.All)
					{
						if (slitMouth && !slitMouth.dead && !(slitMouth == Slits.Eater))
						{
							float num = (vector.x - slitMouth.FaceX) * (float)slitMouth.Dir;
							if (num >= -0.5f && num <= 6f && Mathf.Abs(vector.y - slitMouth.Mouth.y) <= 4f)
							{
								if (Slits.Holder == slitMouth && b)
								{
									Slits.Release(b, false);
								}
								if (Slits.striking == slitMouth)
								{
									Slits.striking = null;
								}
								Slits.bombSlit = slitMouth;
								Slits.bombItem = item;
								Slits.bombPhase = 0;
								Slits.bombTime = 0f;
								slitMouth.flinch = 0f;
								customItemBehaviour.CancelInvoke("DynamiteExplode");
								Rigidbody2D rigidbody2D;
								if (item.TryGetComponent<Rigidbody2D>(ref rigidbody2D))
								{
									rigidbody2D.simulated = false;
								}
								Sound.Play("goo", slitMouth.Mouth, false, true, null, 1f, 0.6f, false, false);
								Sound.Play("megabite", slitMouth.Mouth, false, true, null, 0.6f, 1.2f, false, false);
								Plugin.Log.LogInfo(string.Format("The womb slit's tentacle goes for a lit stick of dynamite at {0}", vector));
								return;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x000612B4 File Offset: 0x0005F4B4
		private static void Bomb(Body b, float dt)
		{
			SlitMouth slitMouth = Slits.bombSlit;
			if (!slitMouth || slitMouth.dead)
			{
				Slits.DropBomb();
				return;
			}
			Slits.bombTime += dt;
			if (Slits.bombPhase == 0)
			{
				if (!Slits.bombItem)
				{
					Slits.bombSlit = null;
					return;
				}
				Vector2 vector = Slits.bombItem.transform.position;
				slitMouth.tipAt = Vector2.Lerp(slitMouth.tipAt, vector, Mathf.Clamp01(Slits.bombTime / 0.35f));
				if (Slits.bombTime < 0.35f)
				{
					return;
				}
				Slits.bombPhase = 1;
				Slits.bombTime = 0f;
				Slits.bombCaught = vector;
				Sound.Play("goo", vector, false, true, null, 0.9f, 0.8f, false, false);
				if (b && b.alive && Vector2.Distance(b.transform.position, slitMouth.Mouth) < 25f)
				{
					Relief.Say(b, "slit_bomb_caught");
				}
				Plugin.Log.LogInfo("The womb slit caught the dynamite: it draws it in");
				return;
			}
			else if (Slits.bombPhase == 1)
			{
				float num = Mathf.SmoothStep(0f, 1f, Slits.bombTime / 1.5f);
				slitMouth.tipAt = Vector2.Lerp(Slits.bombCaught, slitMouth.Mouth, num) + Random.insideUnitCircle * 0.04f * (1f - num);
				if (Slits.bombItem)
				{
					Slits.bombItem.transform.position = slitMouth.tipAt;
				}
				if (Slits.bombTime < 1.5f)
				{
					return;
				}
				Slits.bombPhase = 2;
				Slits.bombTime = 0f;
				if (Slits.bombItem)
				{
					Object.Destroy(Slits.bombItem.gameObject);
				}
				Slits.bombItem = null;
				Sound.Play("eatFlesh", slitMouth.Mouth, false, true, null, 1f, 0.5f, false, false);
				Plugin.Log.LogInfo("The womb slit swallowed the dynamite");
				return;
			}
			else
			{
				if ((Slits.writhe -= dt) <= 0f)
				{
					Slits.writhe = 0.4f;
					Sound.Play("goo", slitMouth.Mouth, false, true, null, 0.7f, Random.Range(0.4f, 0.6f), false, false);
					PlayerCamera main = PlayerCamera.main;
					if (main != null)
					{
						main.shaker.Shake(6f);
					}
				}
				if (Slits.bombTime < 1.6f)
				{
					return;
				}
				Slits.Blast(b, slitMouth);
				return;
			}
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x00061530 File Offset: 0x0005F730
		private static void DropBomb()
		{
			if (Slits.bombItem)
			{
				Rigidbody2D rigidbody2D;
				if (Slits.bombItem.TryGetComponent<Rigidbody2D>(ref rigidbody2D))
				{
					rigidbody2D.simulated = true;
				}
				CustomItemBehaviour customItemBehaviour;
				if (Slits.Lit(Slits.bombItem, out customItemBehaviour))
				{
					customItemBehaviour.Invoke("DynamiteExplode", 1.5f);
				}
			}
			Slits.bombItem = null;
			Slits.bombSlit = null;
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00061588 File Offset: 0x0005F788
		private static void Blast(Body b, SlitMouth s)
		{
			Vector2 belly = s.Belly;
			Slits.bombSlit = null;
			Plugin.Log.LogInfo(string.Format("The dynamite went off in the womb slit's stomach at {0}", belly));
			GameObject gameObject = Resources.Load("BloodExplosion") as GameObject;
			GameObject gameObject2 = Resources.Load("animalflesh") as GameObject;
			for (int i = 0; i < 4; i++)
			{
				Vector2 vector = Vector2.Lerp(s.Mouth, belly - new Vector2((float)(s.Dir * 6) * 0.4f, 0f), (float)i / 3f);
				if (gameObject)
				{
					Object.Destroy(Object.Instantiate<GameObject>(gameObject, vector, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))), 6f);
				}
				Rigidbody2D rigidbody2D;
				if (gameObject2 && Object.Instantiate<GameObject>(gameObject2, s.Mouth + new Vector2((float)s.Dir * 0.6f, 0f), Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))).TryGetComponent<Rigidbody2D>(ref rigidbody2D))
				{
					rigidbody2D.velocity = new Vector2((float)s.Dir * Random.Range(5f, 12f), Random.Range(3f, 9f));
					rigidbody2D.angularVelocity = Random.Range(-600f, 600f);
				}
			}
			Sound.Play("gore" + Random.Range(1, 6).ToString(), belly, false, true, null, 1f, 0.7f, false, false);
			s.ground = null;
			s.open = false;
			if (b && b.alive && Vector2.Distance(b.transform.position, belly) < 30f)
			{
				Relief.Say(b, "slit_bomb_boom");
			}
			Slits.Kill(s);
			WorldGeneration.CreateExplosion(new ExplosionParams
			{
				position = belly,
				range = Plugin.SlitBlastRange.Value,
				structuralDamage = 2000f
			});
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x000617B0 File Offset: 0x0005F9B0
		private static void DrawBomb(SlitMouth s)
		{
			Vector2 vector = s.Mouth + Vector2.right * (float)s.Dir * 0.15f;
			if (Slits.bombPhase == 2)
			{
				foreach (SpriteRenderer spriteRenderer in s.segs)
				{
					if (spriteRenderer)
					{
						spriteRenderer.enabled = false;
					}
				}
				if (s.tip)
				{
					s.tip.enabled = false;
				}
				s.lips.sprite = SlitArt.Lips[0];
				s.lips.transform.position = s.Mouth + Random.insideUnitCircle * 0.05f;
				return;
			}
			s.lips.sprite = SlitArt.Lips[SlitArt.Lips.Length - 1];
			s.lips.transform.position = s.Mouth;
			float num = ((Slits.bombPhase == 1) ? Mathf.Clamp01(Slits.bombTime / 1.5f) : 0f);
			int num2 = Mathf.Max(1, Mathf.RoundToInt((float)s.segs.Length * (1f - num)));
			Slits.Chain(s.segs.Take<SpriteRenderer>(num2).ToArray<SpriteRenderer>(), vector, s.tipAt, 1f - 0.3f * num, 0.5f, true, s.seed);
			for (int j = num2; j < s.segs.Length; j++)
			{
				if (s.segs[j])
				{
					s.segs[j].enabled = false;
				}
			}
			if (s.tip)
			{
				s.tip.enabled = false;
			}
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x00061974 File Offset: 0x0005FB74
		public static string DebugBomb(Body b)
		{
			SlitMouth slitMouth = (from x in Slits.All
				where x && !x.dead
				orderby Vector2.Distance(x.Mouth, b.transform.position)
				select x).FirstOrDefault<SlitMouth>();
			if (!slitMouth)
			{
				return "no womb slit";
			}
			Vector2 vector = slitMouth.Mouth + new Vector2((float)slitMouth.Dir * 3.5f, 1.5f);
			GameObject gameObject = Utils.Create("dynamite", vector, 0f);
			Item item = (gameObject ? gameObject.GetComponent<Item>() : null);
			if (!item || !gameObject.GetComponent<CustomItemBehaviour>())
			{
				return "no dynamite";
			}
			item.Stats.useAction.Invoke(b, item);
			return string.Format("a lit stick dropped in front of the slit at {0}", slitMouth.Mouth);
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x00061A6D File Offset: 0x0005FC6D
		public static string DebugFlood()
		{
			if (!Slits.Eater)
			{
				return "not in a womb slit";
			}
			Slits.Eater.flood = true;
			Slits.timeIn = Mathf.Max(Slits.timeIn, Plugin.SlitFloodAfter.Value);
			return "it floods now";
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x00061AAC File Offset: 0x0005FCAC
		public static string DebugGrab(Body b, bool swallow)
		{
			SlitMouth slitMouth = (from x in Slits.All
				where x && !x.dead
				orderby Vector2.Distance(x.Mouth, b.transform.position)
				select x).FirstOrDefault<SlitMouth>();
			if (!slitMouth)
			{
				return "no womb slit";
			}
			if (Slits.Holding || Slits.Inside)
			{
				return "already held";
			}
			Slits.held = Slits.Pick(b);
			Slits.Grab(b, slitMouth);
			if (swallow)
			{
				Slits.Swallow(b, slitMouth);
			}
			return string.Format("{0} by the slit at {1} ({2:0.0} away)", swallow ? "swallowed" : "grabbed", slitMouth.Mouth, Vector2.Distance(slitMouth.Mouth, b.transform.position));
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x00061B9C File Offset: 0x0005FD9C
		private static void Flood(Body b, SlitMouth s, float dt)
		{
			FluidManager main = FluidManager.main;
			if (!main || SemenFluid.Id == 0)
			{
				return;
			}
			if (!Slits.flooding)
			{
				if (Slits.timeIn < Plugin.SlitFloodAfter.Value)
				{
					return;
				}
				Slits.flooding = true;
				Slits.floodStep = 0;
				Slits.floodTimer = 0f;
				s.mouthShut = true;
				IEnumerable<Vector2Int> enumerable = s.Cells();
				Func<Vector2Int, bool> <>9__3;
				Func<Vector2Int, bool> func;
				if ((func = <>9__3) == null)
				{
					func = (<>9__3 = (Vector2Int c) => c.x == s.Face.x - s.Dir);
				}
				foreach (Vector2Int vector2Int in enumerable.Where<Vector2Int>(func))
				{
					main.SetLiquid(vector2Int.x, vector2Int.y, 0);
					Slits.W.SetBlock(vector2Int, 16);
				}
				Sound.Play("goo", s.Mouth, false, true, null, 1f, 0.45f, false, false);
				Sfx.Play(b, 0.8f, new string[] { "drip" });
				Relief.Say(b, "slit_flood");
				Plugin.Log.LogInfo("The womb slit floods its stomach with semen");
			}
			List<Vector2Int> list = (from c in s.Cells()
				where c.x != s.Face.x - s.Dir
				orderby c.y, (s.Face.x - c.x) * s.Dir descending
				select c).ToList<Vector2Int>();
			float num = Mathf.Max(5f, Plugin.SlitFloodSeconds.Value) / (float)Mathf.Max(1, list.Count);
			if (Slits.floodStep < list.Count && (Slits.floodTimer += dt) >= num)
			{
				Slits.floodTimer = 0f;
				Vector2Int vector2Int2 = list[Slits.floodStep++];
				if (main.Empty(vector2Int2))
				{
					main.fluid[vector2Int2.x, vector2Int2.y] = SemenFluid.Id;
				}
				Sfx.Play(b, 0.5f, new string[] { "drip" });
			}
			int num2 = 0;
			while (num2 < Slits.floodStep && num2 < list.Count)
			{
				if (main.Empty(list[num2]))
				{
					main.fluid[list[num2].x, list[num2].y] = SemenFluid.Id;
				}
				num2++;
			}
			bool flag = SemenFluid.At(b.limbs[0].transform.position);
			if (flag && !Slits.wasUnder)
			{
				Relief.Say(b, "slit_flood_under");
				Sfx.Play(b, 0.8f, new string[] { "muffled" });
				Plugin.Log.LogInfo("The semen closes over her head");
			}
			Slits.wasUnder = flag;
			Semen.Add(b, (flag ? 3f : 1.5f) * dt);
			Slits.S.arousal = Mathf.Min(100f, Slits.S.arousal + 0.8f * dt);
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00061EE8 File Offset: 0x000600E8
		public static string Info(Body b)
		{
			return string.Format("{0} slits; holding {1} inside {2} striking {3} timeIn {4:0} ravish {5} flooding {6} step {7} under {8} oxygen {9:0}; ", new object[]
			{
				Slits.All.Count,
				Slits.Holding,
				Slits.Inside,
				Slits.striking,
				Slits.timeIn,
				Slits.ravish,
				Slits.flooding,
				Slits.floodStep,
				Slits.Inside && SemenFluid.At(b.limbs[0].transform.position),
				b.bloodOxygen
			}) + string.Join(", ", from x in (from x in Slits.All
					where x
					orderby Vector2.Distance(x.Mouth, b.transform.position)
					select x).Take<SlitMouth>(4)
				select string.Format("{0} dir {1} dist {2:0} open {3} flood {4} sated {5:0} flinch {6:0}", new object[]
				{
					x.Mouth,
					x.Dir,
					Vector2.Distance(x.Mouth, b.transform.position),
					x.open,
					x.flood,
					x.sated,
					x.flinch
				}));
		}

		// Token: 0x04000685 RID: 1669
		public static readonly List<SlitMouth> All = new List<SlitMouth>();

		// Token: 0x04000688 RID: 1672
		private static SlitMouth striking;

		// Token: 0x04000689 RID: 1673
		private static float strikeTime;

		// Token: 0x0400068A RID: 1674
		private static float holdTime;

		// Token: 0x0400068B RID: 1675
		private static float attemptCooldown;

		// Token: 0x0400068C RID: 1676
		private static float lineTimer;

		// Token: 0x0400068D RID: 1677
		private static float timeIn;

		// Token: 0x0400068E RID: 1678
		private static float pulse;

		// Token: 0x0400068F RID: 1679
		private static float orgasmGap;

		// Token: 0x04000690 RID: 1680
		private static float spitTime;

		// Token: 0x04000691 RID: 1681
		private static float writhe;

		// Token: 0x04000692 RID: 1682
		private static bool ravish;

		// Token: 0x04000693 RID: 1683
		private static bool spent;

		// Token: 0x04000694 RID: 1684
		private static bool flooding;

		// Token: 0x04000695 RID: 1685
		private static float floodTimer;

		// Token: 0x04000696 RID: 1686
		private static int floodStep;

		// Token: 0x04000697 RID: 1687
		private static bool wasUnder;

		// Token: 0x04000698 RID: 1688
		private static int orgasms;

		// Token: 0x04000699 RID: 1689
		private static Limb held;

		// Token: 0x0400069A RID: 1690
		private static Vector2 haul;

		// Token: 0x0400069B RID: 1691
		private static AudioSource wetLoop;

		// Token: 0x0400069C RID: 1692
		private static SlitMouth spat;

		// Token: 0x0400069D RID: 1693
		private static readonly List<SpriteRenderer[]> gut = new List<SpriteRenderer[]>();

		// Token: 0x0400069E RID: 1694
		private static int overOrder;

		// Token: 0x0400069F RID: 1695
		private const ushort Soil = 16;

		// Token: 0x040006A0 RID: 1696
		private const float StrikeSeconds = 0.45f;

		// Token: 0x040006A1 RID: 1697
		private static readonly string[] GutHold = new string[] { "HandF", "HandB", "DownTorso", "ThighF", "UpTorso" };

		// Token: 0x040006A2 RID: 1698
		private static SlitMouth bombSlit;

		// Token: 0x040006A3 RID: 1699
		private static Item bombItem;

		// Token: 0x040006A4 RID: 1700
		private static int bombPhase;

		// Token: 0x040006A5 RID: 1701
		private static float bombTime;

		// Token: 0x040006A6 RID: 1702
		private static float baitScan;

		// Token: 0x040006A7 RID: 1703
		private static Vector2 bombCaught;

		// Token: 0x040006A8 RID: 1704
		private const float SnatchSeconds = 0.35f;

		// Token: 0x040006A9 RID: 1705
		private const float DrawInSeconds = 1.5f;

		// Token: 0x040006AA RID: 1706
		private const float RumbleSeconds = 1.6f;
	}
}
