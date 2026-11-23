using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using HarmonyLib;
using PoseKit;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000AA RID: 170
	internal static class Latexes
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600035F RID: 863 RVA: 0x00037350 File Offset: 0x00035550
		// (set) Token: 0x06000360 RID: 864 RVA: 0x00037357 File Offset: 0x00035557
		public static LatexBlob Holder { get; private set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000361 RID: 865 RVA: 0x0003735F File Offset: 0x0003555F
		// (set) Token: 0x06000362 RID: 866 RVA: 0x00037366 File Offset: 0x00035566
		public static Latexes.Phase Now { get; private set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000363 RID: 867 RVA: 0x0003736E File Offset: 0x0003556E
		public static bool Holding
		{
			get
			{
				return Latexes.Holder && (Latexes.Now == Latexes.Phase.Dragging || Latexes.Now == Latexes.Phase.Sinking || Latexes.Now == Latexes.Phase.Using || Latexes.Now == Latexes.Phase.Dropping);
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000364 RID: 868 RVA: 0x000373A0 File Offset: 0x000355A0
		public static bool Inside
		{
			get
			{
				return Latexes.Holder && Latexes.Now == Latexes.Phase.Inside;
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000365 RID: 869 RVA: 0x000373B8 File Offset: 0x000355B8
		public static bool Busy
		{
			get
			{
				return Latexes.Now > Latexes.Phase.None;
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000366 RID: 870 RVA: 0x000373C2 File Offset: 0x000355C2
		public static bool Using
		{
			get
			{
				return Latexes.Holder && (Latexes.Now == Latexes.Phase.Using || Latexes.Now == Latexes.Phase.Inside);
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000367 RID: 871 RVA: 0x000373E4 File Offset: 0x000355E4
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000368 RID: 872 RVA: 0x000373EB File Offset: 0x000355EB
		private static WorldGeneration W
		{
			get
			{
				return WorldGeneration.world;
			}
		}

		// Token: 0x06000369 RID: 873 RVA: 0x000373F4 File Offset: 0x000355F4
		public static void SeedLayer(Body b)
		{
			if (!Latexes.W || Latexes.W.biomeOverride != null || !LatexArt.Ready)
			{
				return;
			}
			int biomeDepth = Latexes.W.biomeDepth;
			if (biomeDepth < Mathf.RoundToInt(Plugin.LatexFirstLayer.Value))
			{
				Plugin.Log.LogInfo(string.Format("Layer {0}: no liquid latex (too shallow)", biomeDepth));
				return;
			}
			int num = Diff.Count(Plugin.LatexPerLayer.Value);
			int num2 = 0;
			int num3 = 0;
			while (num3 < 60 && num2 < num)
			{
				Vector2? vector = Spots.RandomFloor(30f, b.transform.position);
				if (vector != null)
				{
					float value = Random.value;
					LatexKind latexKind = ((value < 0.4f) ? LatexKind.Crawler : ((value < 0.7f) ? LatexKind.Ceiling : LatexKind.Puddle));
					Vector2 vector2 = vector.Value + Vector2.down * 0.5f;
					if (latexKind == LatexKind.Ceiling)
					{
						RaycastHit2D raycastHit2D = Physics2D.Raycast(vector.Value + Vector2.up * 0.6f, Vector2.up, 10f, LayerMask.GetMask(new string[] { "Ground" }));
						if (!raycastHit2D || raycastHit2D.distance < 3.5f)
						{
							latexKind = LatexKind.Puddle;
						}
						else
						{
							vector2 = raycastHit2D.point;
						}
					}
					if (Latexes.Make(vector2, latexKind, (Random.value < 0.5f) ? 1 : (-1)))
					{
						num2++;
					}
				}
				num3++;
			}
			ManualLogSource log = Plugin.Log;
			string text = "Layer {0}: {1} of {2} liquid latex ({3})";
			object[] array = new object[4];
			array[0] = biomeDepth;
			array[1] = num2;
			array[2] = num;
			array[3] = string.Join<LatexKind>(", ", Latexes.All.Select<LatexBlob, LatexKind>((LatexBlob x) => x.kind));
			log.LogInfo(string.Format(text, array));
		}

		// Token: 0x0600036A RID: 874 RVA: 0x000375E4 File Offset: 0x000357E4
		public static LatexBlob Make(Vector2 at, LatexKind kind, int facing)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				LatexArt.Ensure(spriteRenderer.sprite.pixelsPerUnit);
			}
			if (!LatexArt.Ready)
			{
				return null;
			}
			GameObject gameObject = new GameObject("LewdLatexBlob");
			LatexBlob s = gameObject.AddComponent<LatexBlob>();
			s.kind = kind;
			s.pos = at;
			s.facing = facing;
			gameObject.transform.position = at;
			s.sr = gameObject.AddComponent<SpriteRenderer>();
			if (Latexes.W)
			{
				s.sr.sharedMaterial = Latexes.W.defaultMat;
			}
			if (spriteRenderer)
			{
				s.sr.sortingLayerID = spriteRenderer.sortingLayerID;
				s.sr.sortingOrder = spriteRenderer.sortingOrder - 15;
			}
			s.segs = new SpriteRenderer[20];
			for (int i = 0; i < s.segs.Length; i++)
			{
				s.segs[i] = Latexes.Part(spriteRenderer, "LewdLatexArm", LatexArt.Get("seg"), -14);
			}
			s.tips = new SpriteRenderer[]
			{
				Latexes.Part(spriteRenderer, "LewdLatexArmTip", LatexArt.Get("tip"), -13),
				Latexes.Part(spriteRenderer, "LewdLatexArmTip", LatexArt.Get("tip"), -13)
			};
			gameObject.layer = LayerMask.NameToLayer("Descriptor");
			BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
			boxCollider2D.size = new Vector2(6f, 3.5f);
			boxCollider2D.offset = ((kind == LatexKind.Ceiling) ? new Vector2(0f, -1.5f) : new Vector2(0f, 1.8f));
			BuildingEntity buildingEntity = gameObject.AddComponent<BuildingEntity>();
			buildingEntity.animal = true;
			buildingEntity.health = 100000f;
			buildingEntity.id = "lewd_latex";
			buildingEntity.ignoreBodyOptimize = true;
			buildingEntity.itemsDropOnDestroy = new ItemDrop[0];
			buildingEntity.alwaysDrop = new ItemDrop[0];
			buildingEntity.itemCategoriesToAdd = new string[0];
			buildingEntity.hitSound = Resources.Load<AudioClip>("Sounds/goo");
			Burning.Make(gameObject, "liquid latex", Plugin.LatexHealth.Value, s.sr, delegate
			{
				Latexes.OnHit(s);
			}, delegate
			{
				Latexes.Kill(s);
			});
			Latexes.All.Add(s);
			Latexes.Draw(s, null);
			Plugin.Log.LogInfo(string.Format("Liquid latex ({0}) at {1}", kind, at));
			return s;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x000378D8 File Offset: 0x00035AD8
		private static SpriteRenderer Part(SpriteRenderer torso, string name, Sprite sp, int order)
		{
			SpriteRenderer spriteRenderer = new GameObject(name).AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sp;
			if (Latexes.W)
			{
				spriteRenderer.sharedMaterial = Latexes.W.defaultMat;
			}
			if (torso)
			{
				spriteRenderer.sortingLayerID = torso.sortingLayerID;
				spriteRenderer.sortingOrder = torso.sortingOrder + order;
			}
			spriteRenderer.enabled = false;
			return spriteRenderer;
		}

		// Token: 0x0600036C RID: 876 RVA: 0x00037940 File Offset: 0x00035B40
		public static void Clear(Body b)
		{
			if (b && Latexes.Busy)
			{
				Latexes.Release(b, false);
			}
			foreach (LatexBlob latexBlob in Latexes.All.ToArray())
			{
				if (latexBlob)
				{
					Object.Destroy(latexBlob.gameObject);
				}
			}
			Latexes.All.Clear();
		}

		// Token: 0x0600036D RID: 877 RVA: 0x000379A0 File Offset: 0x00035BA0
		public static void Update(Body b, float dt)
		{
			Latexes.All.RemoveAll((LatexBlob x) => !x);
			if (!Latexes.W || !b)
			{
				return;
			}
			if (Latexes.attemptCooldown > 0f)
			{
				Latexes.attemptCooldown -= dt;
			}
			if (Latexes.Busy && !b.alive)
			{
				Latexes.Release(b, false);
				return;
			}
			foreach (LatexBlob latexBlob in Latexes.All)
			{
				if (latexBlob.sated > 0f)
				{
					latexBlob.sated -= dt;
				}
				if (latexBlob.attackCooldown > 0f)
				{
					latexBlob.attackCooldown -= dt;
				}
				if (latexBlob.flinch > 0f)
				{
					latexBlob.flinch -= dt;
				}
				latexBlob.frame += dt;
				if (latexBlob.shot != LatexBlob.Shot.None)
				{
					Latexes.ShotApart(latexBlob, b, dt);
				}
			}
			if (Latexes.Busy)
			{
				Latexes.phaseTime += dt;
				switch (Latexes.Now)
				{
				case Latexes.Phase.Reaching:
					Latexes.Reach(b, dt);
					break;
				case Latexes.Phase.Dragging:
					Latexes.Drag(b, dt);
					break;
				case Latexes.Phase.Dropping:
					Latexes.Drop(b, dt);
					break;
				case Latexes.Phase.Sinking:
					Latexes.Sink(b, dt);
					break;
				case Latexes.Phase.Using:
					Latexes.Use(b, dt);
					break;
				case Latexes.Phase.Inside:
					Latexes.InBelly(b, dt);
					break;
				case Latexes.Phase.Transforming:
					Latexes.Transform(b, dt);
					break;
				}
			}
			else if (b.alive && !Scenes.Busy && !HerPuppet.Active && !Slits.Busy)
			{
				Latexes.Watch(b, dt);
			}
			foreach (LatexBlob latexBlob2 in Latexes.All)
			{
				Latexes.Draw(latexBlob2, b);
			}
		}

		// Token: 0x0600036E RID: 878 RVA: 0x00037BAC File Offset: 0x00035DAC
		private static void Watch(Body b, float dt)
		{
			Vector2 vector = b.limbs[1].transform.position;
			foreach (LatexBlob latexBlob in Latexes.All)
			{
				if (!latexBlob.dead && latexBlob.sated <= 0f && latexBlob.flinch <= 0f && latexBlob.shot == LatexBlob.Shot.None)
				{
					LatexKind kind = latexBlob.kind;
					if (kind != LatexKind.Ceiling)
					{
						if (kind != LatexKind.Puddle)
						{
							float num = Vector2.Distance(vector, latexBlob.pos + Vector2.up * 1.5f);
							if (num <= Plugin.LatexSight.Value && Latexes.Clear(latexBlob.pos + Vector2.up * 2f, vector))
							{
								latexBlob.facing = ((vector.x >= latexBlob.pos.x) ? 1 : (-1));
								if (num < 2.8f)
								{
									Latexes.Begin(b, latexBlob, 1);
									break;
								}
								if (num < Plugin.LatexReach.Value && latexBlob.attackCooldown <= 0f)
								{
									Latexes.Begin(b, latexBlob, 4);
									break;
								}
								Latexes.Crawl(latexBlob, vector.x, dt);
							}
						}
						else
						{
							foreach (string text in new string[] { "FootF", "FootB" })
							{
								Limb limb = b.LimbByName(text);
								if (limb && Mathf.Abs(limb.transform.position.x - latexBlob.pos.x) < 1.8f && Mathf.Abs(limb.transform.position.y - latexBlob.pos.y) < 0.7f)
								{
									Latexes.Begin(b, latexBlob, 3);
									return;
								}
							}
						}
					}
					else if (Mathf.Abs(vector.x - latexBlob.pos.x) < 1.3f && vector.y < latexBlob.pos.y && latexBlob.pos.y - vector.y < 11f && Latexes.Clear(latexBlob.pos + Vector2.down * 1.5f, vector))
					{
						Latexes.Begin(b, latexBlob, 2);
						break;
					}
				}
			}
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00037E58 File Offset: 0x00036058
		private static bool Clear(Vector2 from, Vector2 to)
		{
			return !Physics2D.Linecast(from, to, LayerMask.GetMask(new string[] { "Ground" }));
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00037E7C File Offset: 0x0003607C
		private static void Crawl(LatexBlob s, float x, float dt)
		{
			float num = (float)s.facing * Plugin.LatexSpeed.Value * dt;
			Vector2 vector = s.pos + new Vector2(num, 0f);
			RaycastHit2D raycastHit2D = Physics2D.Raycast(vector + Vector2.up * 1.2f, Vector2.down, 2.6f, LayerMask.GetMask(new string[] { "Ground" }));
			if (!raycastHit2D || Physics2D.OverlapPoint(vector + Vector2.up * 1.2f, LayerMask.GetMask(new string[] { "Ground" })))
			{
				return;
			}
			s.pos = new Vector2(vector.x, raycastHit2D.point.y);
		}

		// Token: 0x06000371 RID: 881 RVA: 0x00037F48 File Offset: 0x00036148
		private static void Begin(Body b, LatexBlob s, int how)
		{
			Latexes.Holder = s;
			Latexes.variant = how;
			Latexes.phaseTime = 0f;
			Latexes.covered = false;
			Latexes.lookStage = 0f;
			Latexes.attemptCooldown = 0.6f;
			Latexes.lineTimer = 2f;
			Relief.ForceStop();
			switch (how)
			{
			case 1:
				Latexes.held = new Limb[] { Latexes.Pick(b, new string[] { "FootB", "FootF", "CrusB" }) };
				Latexes.Now = Latexes.Phase.Reaching;
				Sound.Play("goo", s.pos, false, true, null, 0.9f, 0.6f, false, false);
				goto IL_0175;
			case 2:
				Latexes.Now = Latexes.Phase.Dropping;
				s.vy = 0f;
				Sound.Play("goo", s.pos, false, true, null, 0.8f, 0.5f, false, false);
				goto IL_0175;
			case 4:
				Latexes.held = new Limb[]
				{
					Latexes.Pick(b, new string[] { "DownTorso" }),
					Latexes.Pick(b, new string[] { "HandF", "HandB", "DownArmF" })
				};
				Latexes.Now = Latexes.Phase.Reaching;
				Sound.Play("goo", s.pos, false, true, null, 1f, 0.5f, false, false);
				goto IL_0175;
			}
			Latexes.Now = Latexes.Phase.Sinking;
			Latexes.Grip(b, s);
			Relief.Say(b, "latex_grab_step");
			IL_0175:
			Plugin.Log.LogInfo(string.Format("Liquid latex ({0}) goes for her: {1}", s.kind, (how == 1) ? "a leg" : ((how == 2) ? "drops from the ceiling" : ((how == 3) ? "she stepped in it" : "its pseudopods"))));
		}

		// Token: 0x06000372 RID: 882 RVA: 0x00038110 File Offset: 0x00036310
		private static Limb Pick(Body b, params string[] names)
		{
			foreach (string text in names)
			{
				Limb limb = b.LimbByName(text);
				if (limb && !limb.dismembered && limb.rb)
				{
					return limb;
				}
			}
			return b.limbs[1];
		}

		// Token: 0x06000373 RID: 883 RVA: 0x00038160 File Offset: 0x00036360
		private static void Reach(Body b, float dt)
		{
			LatexBlob s = Latexes.Holder;
			if (s && !s.dead)
			{
				if (!Latexes.held.Any<Limb>((Limb l) => !l))
				{
					float num = Mathf.Clamp01(Latexes.phaseTime / 0.4f);
					for (int i = 0; i < Latexes.held.Length; i++)
					{
						s.reachTo[i] = Vector2.Lerp(Latexes.Root(s, i), Latexes.held[i].transform.position, num * num);
					}
					if (num < 1f)
					{
						return;
					}
					float num2 = Latexes.held.Min<Limb>((Limb l) => Vector2.Distance(l.transform.position, s.pos));
					if (!b.alive || Scenes.Busy || num2 > Plugin.LatexReach.Value + 1.5f)
					{
						s.sated = 3f;
						s.attackCooldown = 6f;
						Latexes.Release(b, false);
						Plugin.Log.LogInfo("The liquid latex missed her");
						return;
					}
					Latexes.Grip(b, s);
					Latexes.Now = Latexes.Phase.Dragging;
					Latexes.phaseTime = 0f;
					Latexes.haul = Latexes.held[0].transform.position;
					Relief.Say(b, (Latexes.variant == 1) ? "latex_grab_leg" : "latex_grab_tentacle");
					return;
				}
			}
			Latexes.Release(b, false);
		}

		// Token: 0x06000374 RID: 884 RVA: 0x000382F4 File Offset: 0x000364F4
		private static void Grip(Body b, LatexBlob s)
		{
			b.Ragdoll();
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			b.eyePanicTime = 1f;
			b.adrenaline += 30f;
			b.Scream();
			Sfx.PlayMix(b, 0.8f, new string[] { "slime_gloop", "wet_slap" });
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00038368 File Offset: 0x00036568
		private static void Drag(Body b, float dt)
		{
			LatexBlob holder = Latexes.Holder;
			if (holder && !holder.dead)
			{
				if (!Latexes.held.Any<Limb>((Limb l) => !l || l.dismembered))
				{
					Latexes.Hold(b);
					if (Keys.Down(Plugin.RemoveKey.Value))
					{
						Latexes.Struggle(b, 1.2f);
					}
					if (!Latexes.Holder)
					{
						return;
					}
					Vector2 vector = holder.pos + new Vector2((float)holder.facing * 2.2f, 1.6f);
					if (Vector2.Distance(Latexes.held[0].transform.position, vector) > Plugin.LatexReach.Value + 4f)
					{
						Plugin.Log.LogInfo("The liquid latex lost her");
						Latexes.Release(b, false);
						return;
					}
					if (Vector2.Distance(Latexes.haul, Latexes.held[0].transform.position) < 1.2f)
					{
						Latexes.haul = Vector2.MoveTowards(Latexes.haul, vector, Plugin.LatexDragSpeed.Value * dt);
					}
					Vector2 vector2 = -Physics2D.gravity * dt;
					foreach (Limb limb in b.limbs)
					{
						if (limb && !limb.dismembered && limb.rb)
						{
							int num = Array.IndexOf<Limb>(Latexes.held, limb);
							if (num >= 0)
							{
								Vector2 vector3 = ((num == 0) ? Latexes.haul : (Latexes.haul + Vector2.up * 0.8f));
								limb.rb.velocity = Vector2.Lerp(limb.rb.velocity, Vector2.ClampMagnitude((vector3 - limb.transform.position) * 8f, 9f), Mathf.Clamp01(dt * 14f));
							}
							else
							{
								limb.rb.velocity += vector2 * limb.rb.gravityScale * 0.8f;
								float num2 = Vector2.Distance(limb.transform.position, Latexes.held[0].transform.position) - 2.4f;
								if (num2 > 0f)
								{
									limb.rb.velocity = Vector2.Lerp(limb.rb.velocity, (Latexes.held[0].transform.position - limb.transform.position).normalized * Mathf.Min(10f, num2 * 6f), Mathf.Clamp01(dt * 8f));
								}
							}
						}
					}
					for (int j = 0; j < Latexes.held.Length; j++)
					{
						holder.reachTo[j] = Latexes.held[j].transform.position;
					}
					Latexes.Lines(b, dt, "latex_drag");
					if (Vector2.Distance(Latexes.held[0].transform.position, vector) < 1.4f)
					{
						Latexes.StartUsing(b, holder);
					}
					return;
				}
			}
			Latexes.Release(b, false);
		}

		// Token: 0x06000376 RID: 886 RVA: 0x000386E4 File Offset: 0x000368E4
		private static void Drop(Body b, float dt)
		{
			LatexBlob holder = Latexes.Holder;
			if (!holder || holder.dead)
			{
				Latexes.Release(b, false);
				return;
			}
			holder.vy += 26f * dt;
			holder.pos += Vector2.down * holder.vy * dt;
			Vector2 vector = b.limbs[1].transform.position;
			if (holder.pos.y - 1.6f > vector.y + 1.2f && Latexes.phaseTime < 3f)
			{
				return;
			}
			RaycastHit2D raycastHit2D = Physics2D.Raycast(vector + Vector2.up * 0.5f, Vector2.down, 6f, LayerMask.GetMask(new string[] { "Ground" }));
			holder.pos = (raycastHit2D ? raycastHit2D.point : (vector + Vector2.down * 1.2f));
			holder.kind = LatexKind.Crawler;
			Latexes.covered = true;
			Latexes.Grip(b, holder);
			Sound.Play("goo", holder.pos, false, true, null, 1f, 0.4f, false, false);
			Sfx.PlayMix(b, 1f, new string[] { "wet_slap", "slime_gloop" });
			PlayerCamera main = PlayerCamera.main;
			if (main != null)
			{
				main.shaker.Shake(30f);
			}
			Relief.Say(b, "latex_grab_drop");
			Latexes.StartUsing(b, holder);
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00038874 File Offset: 0x00036A74
		private static void Sink(Body b, float dt)
		{
			LatexBlob holder = Latexes.Holder;
			if (!holder || holder.dead)
			{
				Latexes.Release(b, false);
				return;
			}
			Latexes.Hold(b);
			if (Keys.Down(Plugin.RemoveKey.Value))
			{
				Latexes.Struggle(b, 1.6f);
			}
			if (!Latexes.Holder)
			{
				return;
			}
			Vector2 pos = holder.pos;
			Latexes.Spring(b, "FootF", pos + new Vector2(0.3f, 0.2f), 6f, dt);
			Latexes.Spring(b, "FootB", pos + new Vector2(-0.3f, 0.2f), 6f, dt);
			Latexes.Spring(b, "DownTorso", pos + Vector2.up * (1.8f - Latexes.phaseTime * 0.15f), 3f, dt);
			Latexes.Spring(b, "Head", pos + Vector2.up * (3.4f - Latexes.phaseTime * 0.15f), 2f, dt);
			Latexes.Lines(b, dt, "latex_grab_step");
			if (Latexes.phaseTime > 3.5f)
			{
				holder.kind = LatexKind.Crawler;
				Latexes.StartUsing(b, holder);
			}
		}

		// Token: 0x06000378 RID: 888 RVA: 0x000389AC File Offset: 0x00036BAC
		private static void StartUsing(Body b, LatexBlob s)
		{
			Latexes.Now = Latexes.Phase.Using;
			Latexes.phaseTime = 0f;
			Latexes.pulse = 1.5f;
			Latexes.lineTimer = 2.5f;
			Latexes.lookStage = 1f;
			if (!Latexes.wetLoop)
			{
				Latexes.wetLoop = (Sfx.Has("loop_gut") ? Sfx.Loop(b, "loop_gut") : Sfx.Loop(b, "loop_wet"));
			}
			Latexes.overOrder = b.GetComponentsInChildren<SpriteRenderer>(true).Max<SpriteRenderer>((SpriteRenderer r) => r.sortingOrder);
			Latexes.underOrder = b.limbs.Where<Limb>((Limb l) => l && l.GetComponent<SpriteRenderer>()).Min<Limb>((Limb l) => l.GetComponent<SpriteRenderer>().sortingOrder);
			Latexes.posed = Poses.All.ContainsKey("latex_held") && HerPuppet.Begin(b);
			Latexes.thrustPhase = 0f;
			Relief.Say(b, "latex_use");
			Plugin.Log.LogInfo(string.Concat(new string[]
			{
				"The liquid latex has her",
				Latexes.covered ? ", over all of her" : ", her arms and legs in it",
				" (",
				Latexes.posed ? "posed" : "ragdoll",
				")"
			}));
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00038B2C File Offset: 0x00036D2C
		private static void Use(Body b, float dt)
		{
			LatexBlob holder = Latexes.Holder;
			if (!holder || holder.dead)
			{
				Latexes.Release(b, false);
				return;
			}
			Latexes.Hold(b);
			if (Keys.Down(Plugin.RemoveKey.Value))
			{
				Latexes.Struggle(b, Latexes.covered ? 0.6f : 1f);
			}
			if (!Latexes.Holder)
			{
				return;
			}
			Latexes.thrustPhase += dt * 3.1415927f * 2f / 0.9f;
			if (Latexes.posed)
			{
				ValueTuple<ScenePose, ScenePose, float> valueTuple = Poses.At("latex_held", 0f, 1f);
				ScenePose item = valueTuple.Item1;
				ScenePose item2 = valueTuple.Item2;
				HerPuppet.Show(item, item2, (Mathf.Sin(Latexes.thrustPhase) + 1f) * 0.5f, holder.pos, holder.facing);
				Latexes.Ravish(b, dt, 1f);
				Latexes.Lines(b, dt, "latex_use");
				if (Latexes.phaseTime > Plugin.LatexSwallowAfter.Value)
				{
					Latexes.Swallow(b, holder);
				}
				return;
			}
			Vector2 vector = holder.pos + Vector2.up * (Latexes.covered ? 1.6f : 2.3f);
			float num = Mathf.Sin(Time.time * 4f) * 0.08f;
			Latexes.Spring(b, "DownTorso", vector + Vector2.up * num, 5f, dt);
			Latexes.Spring(b, "UpTorso", vector + Vector2.up * (1f + num), 5f, dt);
			Latexes.Spring(b, "Head", vector + new Vector2((float)holder.facing * 0.2f, 1.9f + num), 4f, dt);
			Latexes.Spring(b, "HandF", vector + new Vector2((float)holder.facing * 1f, -0.8f), 6f, dt);
			Latexes.Spring(b, "HandB", vector + new Vector2((float)(-(float)holder.facing) * 1f, -0.6f), 6f, dt);
			Latexes.Spring(b, "FootF", vector + new Vector2((float)holder.facing * 0.7f, -1.9f), 6f, dt);
			Latexes.Spring(b, "FootB", vector + new Vector2((float)(-(float)holder.facing) * 0.7f, -1.9f), 6f, dt);
			Latexes.Ravish(b, dt, 1f);
			Latexes.Lines(b, dt, "latex_use");
			if (Latexes.phaseTime > Plugin.LatexSwallowAfter.Value)
			{
				Latexes.Swallow(b, holder);
			}
		}

		// Token: 0x0600037A RID: 890 RVA: 0x00038DDC File Offset: 0x00036FDC
		private static void Swallow(Body b, LatexBlob s)
		{
			Latexes.Now = Latexes.Phase.Inside;
			Latexes.phaseTime = 0f;
			Latexes.cycleTime = 0f;
			Latexes.firstCycle = true;
			Latexes.spent = (Latexes.climaxed = false);
			Latexes.spentTime = (Latexes.creepShown = 0f);
			Latexes.lookStage = 0f;
			Latexes.covered = true;
			Latexes.orgasmGap = 3f;
			Sound.Play("eatFlesh", s.pos, false, true, null, 1f, 0.5f, false, false);
			Sfx.PlayMix(b, 1f, new string[] { "slime_slurp", "slime_gloop" });
			PlayerCamera main = PlayerCamera.main;
			if (main != null)
			{
				main.shaker.Shake(30f);
			}
			Latexes.<>c__DisplayClass62_0 CS$<>8__locals1;
			CS$<>8__locals1.torso = b.limbs[1].GetComponent<SpriteRenderer>();
			Latexes.Lift(b, true);
			Latexes.belly = Latexes.Part(CS$<>8__locals1.torso, "LewdLatexBelly", LatexArt.Get("belly_0"), 0);
			Latexes.belly.sortingOrder = Latexes.underOrder - 3;
			Latexes.belly.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
			if (CS$<>8__locals1.torso && CS$<>8__locals1.torso.sprite)
			{
				SlitArt.Ensure(CS$<>8__locals1.torso.sprite.pixelsPerUnit);
			}
			CS$<>8__locals1.seg = LatexArt.Get("fleshseg") ?? LatexArt.Get("seg");
			CS$<>8__locals1.tip = LatexArt.Get("fleshtip") ?? LatexArt.Get("tip");
			Latexes.holders = new SpriteRenderer[4][];
			Latexes.holderTips = new SpriteRenderer[4];
			Latexes.teasers = new SpriteRenderer[4][];
			Latexes.teaserTips = new SpriteRenderer[4];
			for (int i = 0; i < 4; i++)
			{
				Latexes.holders[i] = Latexes.<Swallow>g__Links|62_0(7, Latexes.underOrder - 1, 0.7f, ref CS$<>8__locals1);
				Latexes.holderTips[i] = Latexes.<Swallow>g__Tip|62_1(Latexes.overOrder + 31, 0.7f, ref CS$<>8__locals1);
				Latexes.teasers[i] = Latexes.<Swallow>g__Links|62_0(6, Latexes.underOrder - 1, 0.45f, ref CS$<>8__locals1);
				Latexes.teaserTips[i] = Latexes.<Swallow>g__Tip|62_1(Latexes.overOrder + 33, 0.45f, ref CS$<>8__locals1);
			}
			Latexes.gut = Latexes.<Swallow>g__Links|62_0(8, Latexes.underOrder - 1, 1f, ref CS$<>8__locals1);
			Latexes.gutTentacle = Latexes.<Swallow>g__Tip|62_1(Latexes.underOrder - 1, 1.25f, ref CS$<>8__locals1);
			Latexes.glow = Latexes.Part(CS$<>8__locals1.torso, "LewdLatexBellyGlow", Latexes.GlowSprite(), 0);
			Latexes.glow.sortingOrder = Latexes.underOrder - 2;
			if (Latexes.GlowMaterial)
			{
				Latexes.glow.sharedMaterial = Latexes.GlowMaterial;
			}
			Relief.Say(b, "latex_swallowed");
			Plugin.Log.LogInfo("The liquid latex swallowed her");
		}

		// Token: 0x0600037B RID: 891 RVA: 0x000390B4 File Offset: 0x000372B4
		private static void Lift(Body b, bool up)
		{
			if (!up)
			{
				foreach (KeyValuePair<SpriteRenderer, ValueTuple<int, int>> keyValuePair in Latexes.lifted)
				{
					if (keyValuePair.Key)
					{
						keyValuePair.Key.sortingLayerID = keyValuePair.Value.Item1;
						keyValuePair.Key.sortingOrder = keyValuePair.Value.Item2;
					}
				}
				if (Latexes.lifted.Count > 0)
				{
					Latexes.underOrder -= 10000;
					Latexes.overOrder -= 10000;
				}
				Latexes.lifted.Clear();
				return;
			}
			if (Latexes.lifted.Count > 0 || !b)
			{
				return;
			}
			SortingLayer[] layers = SortingLayer.layers;
			int num = ((layers.Length != 0) ? layers[layers.Length - 1].id : 0);
			foreach (SpriteRenderer spriteRenderer in b.GetComponentsInChildren<SpriteRenderer>(true))
			{
				if (spriteRenderer && !spriteRenderer.name.StartsWith("Lewd"))
				{
					Latexes.lifted[spriteRenderer] = new ValueTuple<int, int>(spriteRenderer.sortingLayerID, spriteRenderer.sortingOrder);
					spriteRenderer.sortingLayerID = num;
					spriteRenderer.sortingOrder += 10000;
				}
			}
			Latexes.underOrder += 10000;
			Latexes.overOrder += 10000;
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600037C RID: 892 RVA: 0x00039244 File Offset: 0x00037444
		private static float SpentS
		{
			get
			{
				return Mathf.Max(4f, Plugin.LatexTransformAfter.Value);
			}
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0003925C File Offset: 0x0003745C
		private static void Exhausted(Body b)
		{
			b.stamina = Mathf.Min(b.stamina, 10f);
			Latexes.S.exhaustion = Mathf.Min(100f, Latexes.S.exhaustion + 25f);
			Relief.Say(b, "latex_creep");
			Plugin.Log.LogInfo("Spent after her peak inside the liquid latex: it starts closing over her");
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600037E RID: 894 RVA: 0x000392C0 File Offset: 0x000374C0
		private static Material GlowMaterial
		{
			get
			{
				if (!Latexes.glowMaterial)
				{
					Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") ?? Shader.Find("Sprites/Default");
					if (shader)
					{
						Latexes.glowMaterial = new Material(shader);
					}
				}
				return Latexes.glowMaterial;
			}
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0003930C File Offset: 0x0003750C
		private static Sprite GlowSprite()
		{
			if (Latexes.glowSprite)
			{
				return Latexes.glowSprite;
			}
			Texture2D texture2D = new Texture2D(64, 64, 4, false)
			{
				filterMode = 1,
				wrapMode = 1
			};
			for (int i = 0; i < 64; i++)
			{
				for (int j = 0; j < 64; j++)
				{
					float num = Mathf.Clamp01(Vector2.Distance(new Vector2((float)j + 0.5f, (float)i + 0.5f), new Vector2(32f, 32f)) / 32f);
					texture2D.SetPixel(j, i, new Color(1f, 1f, 1f, (1f - num) * (1f - num)));
				}
			}
			texture2D.Apply();
			return Latexes.glowSprite = Sprite.Create(texture2D, new Rect(0f, 0f, 64f, 64f), new Vector2(0.5f, 0.5f), 7.111111f);
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00039400 File Offset: 0x00037600
		private static void InBelly(Body b, float dt)
		{
			LatexBlob holder = Latexes.Holder;
			if (!holder || holder.dead)
			{
				Latexes.Release(b, true);
				return;
			}
			Latexes.Hold(b);
			if (Burning.LitTorchInHand(b) && Latexes.attemptCooldown <= 0f)
			{
				Latexes.attemptCooldown = 1f;
				FleshBurn component = holder.GetComponent<FleshBurn>();
				if (component)
				{
					Burning.Damage(component, Plugin.FireTorchHit.Value, "the torch inside it", true);
				}
				if (Latexes.Holder != holder)
				{
					return;
				}
				Relief.Say(b, "latex_spit");
				Latexes.Release(b, true);
				return;
			}
			else
			{
				if (Keys.Down(Plugin.RemoveKey.Value))
				{
					Latexes.Struggle(b, 0f);
				}
				if (!Latexes.Holder)
				{
					return;
				}
				Vector2 vector = holder.pos + Vector2.up * 2.5f;
				Latexes.cycleTime += dt;
				float num = Latexes.cycleTime;
				string text;
				float num2;
				if (Latexes.spent)
				{
					Latexes.spentTime += dt;
					text = "spent";
					num2 = Mathf.Clamp01(Latexes.spentTime / Latexes.SpentS);
				}
				else if (Latexes.firstCycle && num < 2f)
				{
					text = "capture";
					num2 = num / 2f;
				}
				else
				{
					if (Latexes.firstCycle)
					{
						Latexes.firstCycle = false;
						num = (Latexes.cycleTime = 0f);
					}
					if (num < 7f)
					{
						text = "tease";
						num2 = num / 7f;
					}
					else if ((num -= 7f) < 3f)
					{
						text = "enter";
						num2 = num / 3f;
					}
					else if ((num -= 3f) < 11f)
					{
						text = "thrust";
						num2 = num / 11f;
					}
					else if ((num -= 11f) < 3f)
					{
						text = "climax";
						num2 = num / 3f;
					}
					else
					{
						Latexes.spent = true;
						Latexes.spentTime = 0f;
						text = "spent";
						num2 = 0f;
						Latexes.Exhausted(b);
					}
				}
				float num3 = ((text == "capture") ? num2 : 1f);
				float num4 = 0f;
				float num5 = 0f;
				Latexes.swell = Mathf.MoveTowards(Latexes.swell, 0f, dt * 2f);
				Latexes.glowAmount = Mathf.MoveTowards(Latexes.glowAmount, 0f, dt * 0.6f);
				string text2;
				string text3;
				float num6;
				float num7;
				if (!(text == "capture"))
				{
					if (!(text == "tease"))
					{
						if (!(text == "enter"))
						{
							if (!(text == "thrust"))
							{
								if (!(text == "climax"))
								{
									text2 = "latex_v_peak";
									text3 = "latex_v_limp";
									num6 = Mathf.Clamp01(Latexes.spentTime * 1.5f);
									num4 = Mathf.Clamp01(1f - Latexes.spentTime * 0.8f);
									Latexes.depth = Mathf.MoveTowards(Latexes.depth, 0f, dt * 0.8f);
									num7 = 0f;
									Latexes.lookStage = Mathf.Lerp(1f, 5.99f, num2);
									b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 0.2f);
								}
								else
								{
									float num8 = (Mathf.Sin(Latexes.cycleTime * 9f) + 1f) * 0.5f;
									text2 = "latex_v_lift";
									text3 = "latex_v_peak";
									num6 = 0.7f + 0.3f * num8;
									num4 = 1f;
									num5 = 1f;
									Latexes.depth = 1f;
									Latexes.swell = num8;
									num7 = 2f;
									Latexes.glowAmount = Mathf.Max(Latexes.glowAmount, (num2 < 0.4f) ? (num2 / 0.4f) : (1f - (num2 - 0.4f) / 0.6f));
									b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 0.3f);
									if (!Latexes.climaxed)
									{
										Latexes.climaxed = true;
										Latexes.S.arousal = 100f;
										Latexes.orgasmGap = 0f;
										Sfx.PlayMix(b, 0.9f, new string[] { "slime_slurp", "slime_gloop" });
										Womb.Fill(b, 10f);
									}
								}
							}
							else
							{
								float num9 = Mathf.Lerp(1f, 3f, num2);
								Latexes.thrustPhase += dt * 3.1415927f * 2f * num9;
								float num10 = (Mathf.Sin(Latexes.thrustPhase) + 1f) * 0.5f;
								text2 = "latex_v_arch";
								text3 = "latex_v_lift";
								num6 = num10;
								num4 = 1f;
								num5 = 0.4f + 0.6f * num10;
								Latexes.depth = 0.55f + 0.45f * num10;
								if (num10 < 0.3f)
								{
									Latexes.swell = Mathf.Max(Latexes.swell, 0.6f);
								}
								num7 = 1.4f + 1.2f * num2;
								if (num10 > 0.95f && Time.time - Latexes.lastThrust > 0.2f)
								{
									Latexes.lastThrust = Time.time;
									Sfx.PlayMix(b, 0.55f + 0.2f * num2, new string[] { "tentacle_thrust", "slime_squelch" });
									b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 0.15f);
								}
							}
						}
						else
						{
							text2 = "latex_v_arch";
							text3 = "latex_v_lift";
							num6 = num2;
							num4 = 1f;
							num7 = 1f;
							Latexes.depth = Mathf.SmoothStep(0f, 0.8f, num2);
							Latexes.swell = Mathf.Max(Latexes.swell, 1f - num2);
						}
					}
					else
					{
						text2 = "latex_v_hang";
						text3 = "latex_v_arch";
						num6 = 0.5f + 0.5f * Mathf.Sin(Latexes.cycleTime * 1.6f);
						num4 = Mathf.Clamp01(num2 * 4f);
						Latexes.depth = 0f;
						num7 = 0.7f;
					}
				}
				else
				{
					text2 = "latex_v_open";
					text3 = "latex_v_hang";
					num6 = num2;
					num5 = num2;
					Latexes.depth = 0f;
					num7 = 0.3f;
				}
				ScenePose scenePose;
				ScenePose scenePose2;
				if (Latexes.posed && Poses.All.TryGetValue(text2, out scenePose) && Poses.All.TryGetValue(text3, out scenePose2))
				{
					HerPuppet.Show(scenePose, scenePose2, num6, holder.pos, holder.facing);
				}
				if (!Latexes.spent)
				{
					Latexes.Ravish(b, dt, num7);
				}
				Latexes.Creep(b);
				Latexes.Lines(b, dt, Latexes.spent ? "latex_creep" : "latex_inside");
				if (Latexes.belly)
				{
					Latexes.belly.enabled = true;
					Latexes.belly.sprite = LatexArt.Get("belly_" + ((int)(Time.time * 1.5f) % 3).ToString()) ?? Latexes.belly.sprite;
					Latexes.belly.transform.position = vector;
				}
				if (Latexes.glow)
				{
					float num11 = Mathf.Max(Latexes.glowAmount, 0.3f);
					Limb limb = b.LimbByName("UpTorso");
					Latexes.glow.enabled = true;
					Latexes.glow.transform.position = Vector3.Lerp(vector, limb ? limb.transform.position : vector, 1f - Latexes.glowAmount);
					Latexes.glow.transform.localScale = Vector3.one * (0.55f + 0.85f * Latexes.glowAmount);
					Latexes.glow.color = new Color(0.62f, 0.25f, 1f, 0.6f * num11);
				}
				Latexes.DrawTendrils(b, holder, vector, num3, num4, num5);
				if (Latexes.spent && Latexes.spentTime >= Latexes.SpentS)
				{
					Latexes.BeginTransform(b, holder);
				}
				return;
			}
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00039C0C File Offset: 0x00037E0C
		private static void Creep(Body b)
		{
			float num = Latexes.creepShown;
			Latexes.creepShown = Latexes.lookStage;
			if (num < 1f && Latexes.lookStage >= 1f)
			{
				Latexes.Strip(b);
				Relief.Say(b, "latex_creep");
				Plugin.Log.LogInfo("The liquid latex starts closing over her inside it");
				return;
			}
			if (Mathf.FloorToInt(num) != Mathf.FloorToInt(Latexes.lookStage) && Latexes.lookStage >= 2f)
			{
				Sfx.PlayMix(b, 0.7f, new string[] { "slime_slurp", "slime_squelch" });
				b.eyePanicTime = Mathf.Max(b.eyePanicTime, 0.4f);
			}
		}

		// Token: 0x06000382 RID: 898 RVA: 0x00039CB4 File Offset: 0x00037EB4
		private static void DrawTendrils(Body b, LatexBlob s, Vector2 c, float bind, float tease, float squeeze)
		{
			string[] array = new string[] { "HandF", "HandB", "FootF", "FootB" };
			Vector2[] array2 = new Vector2[]
			{
				new Vector2(3.2f, 3.1f),
				new Vector2(-3.4f, 3f),
				new Vector2(2.9f, -3.2f),
				new Vector2(-3.1f, -3.3f)
			};
			int num = 0;
			while (num < 4 && num < Latexes.holders.Length)
			{
				Limb limb = b.LimbByName(array[num]);
				if (limb)
				{
					Vector2 vector = c + new Vector2(array2[num].x * (float)s.facing, array2[num].y);
					Vector2 vector2 = Vector2.Lerp(vector, limb.transform.position, bind);
					Latexes.Chain(Latexes.holders[num], Latexes.holderTips[num], vector, vector2, 0.12f);
					float num2 = 0.7f + 0.15f * squeeze;
					foreach (SpriteRenderer spriteRenderer in Latexes.holders[num])
					{
						if (spriteRenderer)
						{
							spriteRenderer.transform.localScale = new Vector3(spriteRenderer.transform.localScale.x, num2, 1f);
						}
					}
				}
				num++;
			}
			string[] array4 = new string[] { "UpTorso", "DownTorso", "ThighF", "ThighB" };
			int num3 = 0;
			while (num3 < 4 && num3 < Latexes.teasers.Length)
			{
				Limb limb2 = b.LimbByName(array4[num3]);
				bool flag = tease > 0.02f && limb2;
				foreach (SpriteRenderer spriteRenderer2 in Latexes.teasers[num3])
				{
					if (spriteRenderer2 && !flag)
					{
						spriteRenderer2.enabled = false;
					}
				}
				if (Latexes.teaserTips[num3])
				{
					Latexes.teaserTips[num3].enabled = flag;
				}
				if (flag)
				{
					float num4 = ((num3 % 2 == 0) ? 1f : (-1f));
					Vector2 vector3 = c + new Vector2(num4 * (float)s.facing * 4.2f, 1.2f - (float)num3 * 0.8f);
					Vector2 vector4 = limb2.transform.position + new Vector2(Mathf.Sin(Time.time * 2.3f + (float)num3 * 1.7f) * 0.25f, Mathf.Cos(Time.time * 1.9f + (float)num3) * 0.2f);
					Latexes.Chain(Latexes.teasers[num3], Latexes.teaserTips[num3], vector3, Vector2.Lerp(vector3, vector4, tease), 0.3f);
				}
				num3++;
			}
			Limb limb3 = b.LimbByName("DownTorso");
			if (limb3 && Latexes.gut.Length != 0)
			{
				Vector2 vector5 = limb3.transform.position + new Vector2(0f, -0.35f);
				Vector2 vector6 = vector5 + new Vector2((float)(-(float)s.facing) * 2.4f, -2.6f);
				Vector2 normalized = (vector5 - vector6).normalized;
				float num5 = Vector2.Distance(vector6, vector5);
				float num6 = Mathf.Lerp(num5 * 0.25f, num5, Mathf.Max(0.05f, Latexes.depth));
				Vector2 vector7 = vector6 + normalized * Mathf.Min(num6, num5);
				Latexes.Chain(Latexes.gut, Latexes.gutTentacle, vector6, vector7, 0.08f);
				if (Latexes.depth <= 0.05f)
				{
					foreach (SpriteRenderer spriteRenderer3 in Latexes.gut)
					{
						if (spriteRenderer3)
						{
							spriteRenderer3.enabled = false;
						}
					}
				}
				if (Latexes.gutTentacle)
				{
					Latexes.gutTentacle.enabled = Latexes.depth > 0.05f && Latexes.depth < 0.92f;
					Latexes.gutTentacle.transform.localScale = Vector3.one * (1.15f + 0.35f * Latexes.swell);
				}
			}
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0003A148 File Offset: 0x00038348
		private static void Ravish(Body b, float dt, float hard)
		{
			Latexes.S.arousal = Mathf.Min(100f, Latexes.S.arousal + 2.2f * hard * dt);
			Latexes.S.lust = Mathf.Min(100f, Latexes.S.lust + 0.4f * hard * dt);
			Latexes.S.submission = Mathf.Min(100f, Latexes.S.submission + 0.05f * hard * dt);
			if (Latexes.wetLoop)
			{
				Latexes.wetLoop.volume = Mathf.MoveTowards(Latexes.wetLoop.volume, 0.45f, dt);
			}
			if ((Latexes.pulse -= dt) <= 0f)
			{
				Latexes.pulse = Random.Range(2.2f, 4f) / hard;
				Latexes.S.arousal = Mathf.Min(100f, Latexes.S.arousal + 8f * hard);
				b.eatTime = Mathf.Max(b.eatTime, 0.6f);
				Sfx.Play(b, 0.75f, new string[]
				{
					Latexes.covered ? "muffled" : "moan",
					"moan"
				});
				Sfx.PlayMix(b, 0.6f, new string[] { "slime_squelch", "slime_slurp" });
				Womb.Fill(b, 3f);
			}
			if ((Latexes.writhe -= dt) <= 0f)
			{
				Latexes.writhe = Random.Range(1.6f, 2.8f);
				Sfx.PlayMix(b, 0.5f, new string[] { "slime_bubble", "slime_squelch" });
			}
			Latexes.orgasmGap -= dt;
			if (Latexes.S.arousal >= 99f && Latexes.orgasmGap <= 0f && b.conscious)
			{
				Latexes.orgasmGap = 5f;
				Plugin.Orgasm(b, false);
				Latexes.S.arousal = Mathf.Max(Latexes.S.arousal, 55f);
				b.stamina = Mathf.Max(0f, b.stamina - 15f);
				Womb.Fill(b, 8f);
				Relief.Say(b, "latex_come");
			}
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0003A398 File Offset: 0x00038598
		private static void BeginTransform(Body b, LatexBlob s)
		{
			Latexes.Now = Latexes.Phase.Transforming;
			Latexes.phaseTime = 0f;
			Latexes.HideGut();
			Engulfed.Hide();
			Latexes.lookStage = 5.99f;
			Latexes.Strip(b);
			Sfx.PlayMix(b, 1f, new string[] { "slime_slurp", "slime_gloop" });
			Sound.Play("goo", s.pos, false, true, null, 1f, 0.4f, false, false);
			Relief.Say(b, "latex_transform");
			Plugin.Log.LogInfo("The liquid latex closes over her: she wears it now");
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0003A42C File Offset: 0x0003862C
		private static void Transform(Body b, float dt)
		{
			LatexBlob holder = Latexes.Holder;
			float num = Mathf.Clamp01(Latexes.phaseTime / 2.6f);
			Latexes.Hold(b);
			ScenePose scenePose;
			if (Latexes.posed && holder && Poses.All.TryGetValue("latex_stand", out scenePose))
			{
				HerPuppet.Show(scenePose, scenePose, 0f, holder.pos, holder.facing);
			}
			Latexes.lookStage = 5.99f;
			if (holder)
			{
				holder.sr.color = new Color(1f, 1f, 1f, 1f - num);
				holder.transform.localScale = new Vector3(1f - 0.5f * num, 1f - 0.7f * num, 1f);
			}
			if (num < 1f)
			{
				return;
			}
			if (holder)
			{
				Object.Destroy(holder.gameObject);
			}
			Latexes.Holder = null;
			Latexes.Now = Latexes.Phase.None;
			Latexes.held = new Limb[0];
			if (Latexes.posed)
			{
				HerPuppet.End();
				Latexes.posed = false;
			}
			Traverse.Create(b).Field("movingAllowed").SetValue(true);
			if (Latexes.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(Latexes.wetLoop, 0.5f));
			}
			Latexes.wetLoop = null;
			Suit.AttachLatex(b);
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0003A590 File Offset: 0x00038790
		private static void Strip(Body b)
		{
			int num = 0;
			foreach (Item item in b.GetAllWearables().ToList<Item>())
			{
				if (item)
				{
					item.condition = Mathf.Max(0.05f, item.condition - 0.2f);
					b.DropWearable(item);
					if (!b.GetAllWearables().Contains(item))
					{
						num++;
					}
				}
			}
			if (num > 0)
			{
				Sound.Play("ripclothing", b.transform.position, false, true, b.transform, 1f, 1f, false, false);
				Plugin.Log.LogInfo(string.Format("The liquid latex pushed {0} pieces off her", num));
			}
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0003A670 File Offset: 0x00038870
		private static void Struggle(Body b, float how)
		{
			if (Latexes.attemptCooldown > 0f || !b.conscious)
			{
				return;
			}
			Latexes.attemptCooldown = 1f;
			float num = Mathf.Clamp(b.stamina / 100f, 0.1f, 1f) * (1f - Latexes.S.exhaustion / 100f);
			float num2 = ((how > 0f) ? (Plugin.LatexEscape.Value * how) : Plugin.LatexEscapeInside.Value) * Diff.Escape * num * (1f - Latexes.S.submission * 0.004f) * ((Latexes.S.orgasmTimer > 15f) ? 0.5f : 1f);
			num2 = Mathf.Max(0.01f, num2);
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
			Plugin.Log.LogInfo(string.Format("Struggling in the liquid latex ({0}): chance {1:P0}, {2}", Latexes.Now, num2, flag ? "free" : "held"));
			if (flag)
			{
				Relief.Say(b, "latex_escape");
				Latexes.Release(b, Latexes.Now == Latexes.Phase.Inside);
				return;
			}
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.7f, 0.6f, false, false);
			Relief.Say(b, "latex_fail");
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0003A878 File Offset: 0x00038A78
		public static void OnHit(LatexBlob s)
		{
			if (!s || s.dead)
			{
				return;
			}
			if (Time.time - s.lastHit > 6f)
			{
				s.hits = 0;
			}
			s.lastHit = Time.time;
			s.hits++;
			s.flinch = 1.5f;
			if (s.shot != LatexBlob.Shot.None)
			{
				return;
			}
			if (Latexes.Holder != s)
			{
				Latexes.ShootApart(s);
				return;
			}
			if ((Latexes.Now == Latexes.Phase.Reaching || Latexes.Now == Latexes.Phase.Dragging || Latexes.Now == Latexes.Phase.Using || Latexes.Now == Latexes.Phase.Sinking) && (float)s.hits >= Plugin.LatexLetGoHits.Value)
			{
				Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
				s.hits = 0;
				Plugin.Log.LogInfo("Shot at, the liquid latex lets go of her");
				if (body)
				{
					Relief.Say(body, "latex_escape");
					Latexes.Release(body, false);
				}
				Latexes.ShootApart(s);
			}
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0003A974 File Offset: 0x00038B74
		private static void ShootApart(LatexBlob s)
		{
			s.shotTime = 0f;
			s.sated = 0f;
			s.reachTo[0] = (s.reachTo[1] = Vector2.zero);
			if (s.kind == LatexKind.Ceiling)
			{
				s.shot = LatexBlob.Shot.Falling;
				s.vy = 0f;
				s.pos += Vector2.down * 1.2f;
			}
			else
			{
				Latexes.Scatter(s);
			}
			Sound.Play("goo", s.pos, false, true, null, 1f, 0.5f, false, false);
			Plugin.Log.LogInfo(string.Format("The liquid latex at {0} was shot apart", s.pos));
		}

		// Token: 0x0600038A RID: 906 RVA: 0x0003AA3C File Offset: 0x00038C3C
		private static void Scatter(LatexBlob s)
		{
			s.shot = LatexBlob.Shot.Scattering;
			s.shotTime = 0f;
			s.kind = LatexKind.Crawler;
			Sound.Play("goo", s.pos, false, true, null, 0.9f, 0.4f, false, false);
			foreach (SpriteRenderer spriteRenderer in s.splash)
			{
				if (spriteRenderer)
				{
					Object.Destroy(spriteRenderer.gameObject);
				}
			}
			s.splash.Clear();
			s.splashVel.Clear();
			Sprite sprite = LatexArt.Get("drop");
			for (int i = 0; i < 7; i++)
			{
				SpriteRenderer spriteRenderer2 = new GameObject("LewdLatexDrop").AddComponent<SpriteRenderer>();
				spriteRenderer2.sprite = sprite;
				if (Latexes.W)
				{
					spriteRenderer2.sharedMaterial = Latexes.W.defaultMat;
				}
				spriteRenderer2.sortingLayerID = s.sr.sortingLayerID;
				spriteRenderer2.sortingOrder = s.sr.sortingOrder + 1;
				float num = Random.Range(0.12f, 0.2f);
				spriteRenderer2.transform.localScale = new Vector3(num, num, 1f);
				spriteRenderer2.transform.position = s.pos + new Vector2(Random.Range(-1.2f, 1.2f), Random.Range(1.2f, 2.4f));
				s.splash.Add(spriteRenderer2);
				s.splashVel.Add(new Vector2(Random.Range(-4.5f, 4.5f), Random.Range(3f, 6.5f)));
			}
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0003AC08 File Offset: 0x00038E08
		private static void ShotApart(LatexBlob s, Body b, float dt)
		{
			s.shotTime += dt;
			BoxCollider2D component = s.GetComponent<BoxCollider2D>();
			bool flag = s.shot == LatexBlob.Shot.None;
			if (component && component.enabled != flag)
			{
				component.enabled = flag;
			}
			for (int i = 0; i < s.splash.Count; i++)
			{
				SpriteRenderer spriteRenderer = s.splash[i];
				if (spriteRenderer)
				{
					Vector2 vector = s.splashVel[i] + Vector2.down * 22f * dt;
					Vector2 vector2 = spriteRenderer.transform.position + vector * dt;
					if (vector2.y < s.pos.y + 0.05f)
					{
						vector2.y = s.pos.y + 0.05f;
						vector = Vector2.zero;
					}
					s.splashVel[i] = vector;
					spriteRenderer.transform.position = vector2;
					spriteRenderer.color = new Color(1f, 1f, 1f, Mathf.Clamp01(1.6f - s.shotTime));
				}
			}
			switch (s.shot)
			{
			case LatexBlob.Shot.Falling:
			{
				s.vy += 26f * dt;
				Vector2 vector3 = s.pos + Vector2.down * s.vy * dt;
				RaycastHit2D raycastHit2D = Physics2D.Raycast(s.pos, Vector2.down, s.vy * dt + 0.6f, LayerMask.GetMask(new string[] { "Ground" }));
				if (raycastHit2D || s.shotTime > 4f)
				{
					s.pos = (raycastHit2D ? raycastHit2D.point : vector3);
					PlayerCamera main = PlayerCamera.main;
					if (main != null)
					{
						main.shaker.Shake(10f);
					}
					Latexes.Scatter(s);
					return;
				}
				s.pos = vector3;
				return;
			}
			case LatexBlob.Shot.Scattering:
				if (s.shotTime >= 1.5999999f)
				{
					s.shot = LatexBlob.Shot.Gone;
					s.shotTime = 0f;
					foreach (SpriteRenderer spriteRenderer2 in s.splash)
					{
						if (spriteRenderer2)
						{
							Object.Destroy(spriteRenderer2.gameObject);
						}
					}
					s.splash.Clear();
					s.splashVel.Clear();
					return;
				}
				break;
			case LatexBlob.Shot.Gone:
				if (s.shotTime >= Plugin.LatexReformSeconds.Value && (!b || Vector2.Distance(b.limbs[1].transform.position, s.pos + Vector2.up * 1.5f) > 2.5f))
				{
					s.shot = LatexBlob.Shot.Reforming;
					s.shotTime = 0f;
					Sound.Play("goo", s.pos, false, true, null, 0.7f, 0.5f, false, false);
					return;
				}
				break;
			case LatexBlob.Shot.Reforming:
				if (s.shotTime >= 1.6f)
				{
					s.shot = LatexBlob.Shot.None;
					s.sated = 2f;
					s.hits = 0;
					Plugin.Log.LogInfo(string.Format("The liquid latex at {0} formed again", s.pos));
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0003AF94 File Offset: 0x00039194
		private static bool DrawShot(LatexBlob s)
		{
			if (s.shot == LatexBlob.Shot.None)
			{
				return false;
			}
			float shotTime = s.shotTime;
			Sprite sprite = LatexArt.Get("idle_0");
			Sprite sprite2 = LatexArt.Get("puddle_0");
			Sprite sprite3 = LatexArt.Get("drop");
			Vector3 one = Vector3.one;
			float num = 1f;
			Sprite sprite4 = sprite;
			Vector2 pos = s.pos;
			switch (s.shot)
			{
			case LatexBlob.Shot.Falling:
				sprite4 = sprite3;
				break;
			case LatexBlob.Shot.Scattering:
				if (shotTime < 0.1f)
				{
					one..ctor(0.95f, 1.08f, 1f);
				}
				else if (shotTime < 0.4f)
				{
					float num2 = (shotTime - 0.1f) / 0.3f;
					one..ctor(Mathf.Lerp(1f, 1.6f, num2), Mathf.Lerp(1f, 0.22f, num2), 1f);
				}
				else
				{
					float num3 = Mathf.Clamp01((shotTime - 0.4f) / 1.2f);
					sprite4 = sprite2;
					one..ctor(Mathf.Lerp(1.4f, 0.35f, num3), Mathf.Lerp(1.6f, 0.6f, num3), 1f);
					num = 1f - num3;
				}
				break;
			case LatexBlob.Shot.Gone:
				s.sr.enabled = false;
				return true;
			case LatexBlob.Shot.Reforming:
				if (shotTime < 0.6f)
				{
					float num4 = shotTime / 0.6f;
					sprite4 = sprite2;
					one..ctor(Mathf.Lerp(0.2f, 1.1f, num4), Mathf.Lerp(0.6f, 1.6f, num4), 1f);
					num = num4;
				}
				else
				{
					float num5 = (shotTime - 0.6f) / 1f;
					float num6 = ((num5 < 0.6f) ? Mathf.Lerp(0.2f, 1.1f, num5 / 0.6f) : (1f + 0.1f * Mathf.Cos((num5 - 0.6f) / 0.4f * 3.1415927f * 2.5f) * (1f - num5)));
					one..ctor(Mathf.Lerp(1.5f, 1f, Mathf.Clamp01(num5 / 0.6f)), num6, 1f);
				}
				break;
			}
			s.sr.enabled = true;
			if (sprite4)
			{
				s.sr.sprite = sprite4;
			}
			s.sr.flipX = s.facing < 0;
			s.sr.color = new Color(1f, 1f, 1f, num);
			s.transform.position = new Vector3(pos.x, pos.y, 0f);
			s.transform.localScale = one;
			foreach (SpriteRenderer spriteRenderer in s.segs)
			{
				if (spriteRenderer)
				{
					spriteRenderer.enabled = false;
				}
			}
			foreach (SpriteRenderer spriteRenderer2 in s.tips)
			{
				if (spriteRenderer2)
				{
					spriteRenderer2.enabled = false;
				}
			}
			return true;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0003B2B0 File Offset: 0x000394B0
		public static void Kill(LatexBlob s)
		{
			if (!s || s.dead)
			{
				return;
			}
			s.dead = true;
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (Latexes.Holder == s && body)
			{
				Latexes.Release(body, Latexes.Now == Latexes.Phase.Inside);
			}
			Sound.Play("goo", s.pos, false, true, null, 1f, 0.3f, false, false);
			Plugin.Log.LogInfo(string.Format("The liquid latex at {0} died", s.pos));
			Object.Destroy(s.gameObject);
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0003B35C File Offset: 0x0003955C
		public static void LetGo(Body b)
		{
			if (Latexes.Busy)
			{
				Latexes.Release(b, Latexes.Now == Latexes.Phase.Inside);
			}
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0003B374 File Offset: 0x00039574
		private static void Release(Body b, bool spit)
		{
			LatexBlob holder = Latexes.Holder;
			bool flag = Latexes.Now == Latexes.Phase.Inside;
			Latexes.Holder = null;
			Latexes.Now = Latexes.Phase.None;
			Latexes.held = new Limb[0];
			Latexes.covered = false;
			Latexes.lookStage = 0f;
			Latexes.HideGut();
			if (flag)
			{
				Engulfed.Hide();
			}
			if (Latexes.posed)
			{
				HerPuppet.End();
				Latexes.posed = false;
			}
			if (Latexes.wetLoop && b)
			{
				b.StartCoroutine(Sfx.FadeOut(Latexes.wetLoop, 0.5f));
			}
			Latexes.wetLoop = null;
			if (holder)
			{
				holder.sated = Mathf.Max(holder.sated, 25f);
				holder.reachTo[0] = (holder.reachTo[1] = Vector2.zero);
				holder.sr.color = Color.white;
				holder.transform.localScale = Vector3.one;
				if (holder.kind == LatexKind.Puddle || Latexes.variant == 3)
				{
					holder.kind = LatexKind.Crawler;
				}
			}
			if (!b)
			{
				return;
			}
			Traverse.Create(b).Field("movingAllowed").SetValue(true);
			if ((spit || flag) && b.alive && holder)
			{
				b.Ragdoll();
				foreach (Limb limb in b.limbs)
				{
					if (limb && !limb.dismembered && limb.rb)
					{
						limb.rb.velocity = new Vector2((float)holder.facing * 8f, 4f);
					}
				}
				b.Scream();
				Sound.Play("vomit", holder.pos, false, true, null, 0.9f, 1f, false, false);
				Sound.Play("goo", holder.pos, false, true, null, 1f, 0.6f, false, false);
				b.wetness = Mathf.Max(b.wetness, 30f);
				Plugin.Log.LogInfo("The liquid latex spat her out");
			}
		}

		// Token: 0x06000390 RID: 912 RVA: 0x0003B594 File Offset: 0x00039794
		private static void HideGut()
		{
			Latexes.Lift(PlayerCamera.main ? PlayerCamera.main.body : null, false);
			if (Latexes.belly)
			{
				Object.Destroy(Latexes.belly.gameObject);
			}
			Latexes.belly = null;
			if (Latexes.glow)
			{
				Object.Destroy(Latexes.glow.gameObject);
			}
			Latexes.glow = null;
			using (IEnumerator<SpriteRenderer[]> enumerator = Latexes.holders.Concat<SpriteRenderer[]>(Latexes.teasers).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					foreach (SpriteRenderer spriteRenderer in enumerator.Current)
					{
						if (spriteRenderer)
						{
							Object.Destroy(spriteRenderer.gameObject);
						}
					}
				}
			}
			foreach (SpriteRenderer spriteRenderer2 in Latexes.holderTips.Concat<SpriteRenderer>(Latexes.teaserTips))
			{
				if (spriteRenderer2)
				{
					Object.Destroy(spriteRenderer2.gameObject);
				}
			}
			Latexes.holders = new SpriteRenderer[0][];
			Latexes.teasers = new SpriteRenderer[0][];
			Latexes.holderTips = new SpriteRenderer[0];
			Latexes.teaserTips = new SpriteRenderer[0];
			foreach (SpriteRenderer spriteRenderer3 in Latexes.gut)
			{
				if (spriteRenderer3)
				{
					Object.Destroy(spriteRenderer3.gameObject);
				}
			}
			Latexes.gut = new SpriteRenderer[0];
			if (Latexes.gutTentacle)
			{
				Object.Destroy(Latexes.gutTentacle.gameObject);
			}
			Latexes.gutTentacle = null;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0003B748 File Offset: 0x00039948
		private static void Hold(Body b)
		{
			b.shock = Mathf.Max(b.shock, 15f);
			b.eyePanicTime = Mathf.Max(b.eyePanicTime, (Latexes.Now == Latexes.Phase.Inside) ? 0f : 0.3f);
			if (b.rb)
			{
				b.rb.velocity = Vector2.zero;
			}
		}

		// Token: 0x06000392 RID: 914 RVA: 0x0003B7AD File Offset: 0x000399AD
		private static void Lines(Body b, float dt, string key)
		{
			if ((Latexes.lineTimer -= dt) > 0f)
			{
				return;
			}
			Latexes.lineTimer = Random.Range(5f, 9f);
			Relief.Say(b, key);
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0003B7E0 File Offset: 0x000399E0
		private static void Spring(Body b, string name, Vector2 target, float stiffness, float dt)
		{
			Limb limb = b.LimbByName(name);
			if (!limb || limb.dismembered || !limb.rb)
			{
				return;
			}
			Vector2 vector = (target - limb.transform.position) * stiffness;
			limb.rb.velocity = Vector2.Lerp(limb.rb.velocity, Vector2.ClampMagnitude(vector, 12f), Mathf.Clamp01(dt * 12f));
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000394 RID: 916 RVA: 0x0003B863 File Offset: 0x00039A63
		public static int CreepStage
		{
			get
			{
				if (!Latexes.Holder || (Latexes.Now != Latexes.Phase.Inside && Latexes.Now != Latexes.Phase.Transforming))
				{
					return 0;
				}
				return Mathf.Clamp(Mathf.FloorToInt(Latexes.lookStage), 0, 5);
			}
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0003B894 File Offset: 0x00039A94
		private static Vector2 Root(LatexBlob s, int i)
		{
			return s.pos + new Vector2((float)s.facing * 2.2f, (i == 0) ? 1.6f : 2.4f);
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0003B8C4 File Offset: 0x00039AC4
		private static void Draw(LatexBlob s, Body b)
		{
			if (!s || !s.sr)
			{
				return;
			}
			if (Latexes.DrawShot(s))
			{
				return;
			}
			if (s.sr.color.a < 1f)
			{
				s.sr.color = Color.white;
			}
			bool flag = Latexes.Holder == s;
			string text;
			if (flag && Latexes.Now == Latexes.Phase.Dropping)
			{
				text = "drop";
			}
			else if (flag && Latexes.covered)
			{
				text = "mound_" + ((int)(s.frame * 2f) % 2).ToString();
			}
			else if (flag && (Latexes.Now == Latexes.Phase.Reaching || Latexes.Now == Latexes.Phase.Dragging))
			{
				text = "reach_" + ((int)(s.frame * 3f) % 2).ToString();
			}
			else if (flag && Latexes.Now == Latexes.Phase.Sinking)
			{
				text = "idle_0";
			}
			else if (s.kind == LatexKind.Ceiling)
			{
				text = "hang_" + ((int)(s.frame * 1.2f) % 2).ToString();
			}
			else if (s.kind == LatexKind.Puddle)
			{
				text = "puddle_" + ((int)(s.frame * 0.8f) % 2).ToString();
			}
			else
			{
				text = "idle_" + ((int)(s.frame * 2f) % 3).ToString();
			}
			Sprite sprite = LatexArt.Get(text);
			if (sprite)
			{
				s.sr.sprite = sprite;
			}
			bool flag2 = flag && Latexes.Now == Latexes.Phase.Inside;
			s.sr.enabled = !flag2;
			BoxCollider2D component = s.GetComponent<BoxCollider2D>();
			if (component && component.enabled == flag2)
			{
				component.enabled = !flag2;
			}
			s.sr.flipX = s.facing < 0;
			s.transform.position = new Vector3(s.pos.x, s.pos.y, 0f);
			if (flag && Latexes.Now == Latexes.Phase.Sinking)
			{
				s.transform.localScale = new Vector3(1f, Mathf.Lerp(0.25f, 1f, Mathf.Clamp01(Latexes.phaseTime / 3.5f)), 1f);
			}
			else if (!flag || Latexes.Now != Latexes.Phase.Transforming)
			{
				s.transform.localScale = Vector3.one;
			}
			SpriteRenderer spriteRenderer = (b ? b.limbs[1].GetComponent<SpriteRenderer>() : null);
			bool flag3 = flag && Latexes.Now != Latexes.Phase.Inside && (Latexes.covered || Latexes.Now == Latexes.Phase.Using || Latexes.Now == Latexes.Phase.Sinking || Latexes.Now == Latexes.Phase.Transforming);
			if (spriteRenderer)
			{
				s.sr.sortingOrder = (flag3 ? (Latexes.overOrder + 6) : ((flag && Latexes.Now == Latexes.Phase.Inside) ? (Latexes.underOrder - 4) : (spriteRenderer.sortingOrder - 15)));
			}
			for (int i = 0; i < 2; i++)
			{
				bool flag4 = flag && (Latexes.Now == Latexes.Phase.Reaching || Latexes.Now == Latexes.Phase.Dragging) && i < Latexes.held.Length && s.reachTo[i] != Vector2.zero;
				SpriteRenderer[] array = s.segs.Skip<SpriteRenderer>(i * 10).Take<SpriteRenderer>(10).ToArray<SpriteRenderer>();
				if (!flag4)
				{
					foreach (SpriteRenderer spriteRenderer2 in array)
					{
						if (spriteRenderer2)
						{
							spriteRenderer2.enabled = false;
						}
					}
					if (s.tips[i])
					{
						s.tips[i].enabled = false;
					}
				}
				else
				{
					Latexes.Chain(array, s.tips[i], Latexes.Root(s, i), s.reachTo[i], 0.25f);
				}
			}
		}

		// Token: 0x06000397 RID: 919 RVA: 0x0003BCAC File Offset: 0x00039EAC
		internal static void Chain(SpriteRenderer[] segs, SpriteRenderer tip, Vector2 from, Vector2 to, float wiggle)
		{
			Vector2 vector = to - from;
			float magnitude = vector.magnitude;
			Vector2 vector2 = ((magnitude > 0.001f) ? (new Vector2(-vector.y, vector.x) / magnitude) : Vector2.up);
			int num = Mathf.Clamp(Mathf.CeilToInt(magnitude / 0.9f), 1, segs.Length);
			for (int i = 0; i < segs.Length; i++)
			{
				if (segs[i])
				{
					segs[i].enabled = i < num;
					if (i < num)
					{
						float num2 = (float)i / (float)num;
						float num3 = (float)(i + 1) / (float)num;
						Vector2 vector3 = from + vector * num2 + vector2 * Mathf.Sin(num2 * 6f + Time.time * 5f) * wiggle * Mathf.Sin(num2 * 3.1415927f);
						Vector2 vector4 = from + vector * num3 + vector2 * Mathf.Sin(num3 * 6f + Time.time * 5f) * wiggle * Mathf.Sin(num3 * 3.1415927f);
						segs[i].transform.position = vector3;
						segs[i].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector4.y - vector3.y, vector4.x - vector3.x) * 57.29578f);
						float num4 = (segs[i].sprite ? (segs[i].sprite.rect.width / segs[i].sprite.pixelsPerUnit) : 1.1f);
						float num5 = Vector2.Distance(vector3, vector4) * 1.2f / Mathf.Max(0.1f, num4);
						float num6 = ((segs[i].transform.localScale.y > 0f) ? segs[i].transform.localScale.y : 1f);
						segs[i].transform.localScale = new Vector3(Mathf.Max(0.3f, num5), (segs[i].name == "LewdLatexGut") ? num6 : (1f - 0.4f * num2), 1f);
					}
				}
			}
			if (tip)
			{
				tip.enabled = true;
				tip.transform.position = to - vector.normalized * 0.4f;
				tip.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector.y, vector.x) * 57.29578f);
			}
		}

		// Token: 0x06000398 RID: 920 RVA: 0x0003BF90 File Offset: 0x0003A190
		public static string DebugMake(Body b, string kind)
		{
			Vector2 vector = b.transform.position + new Vector2(b.isRight ? 6f : (-6f), 0f);
			RaycastHit2D raycastHit2D = Physics2D.Raycast(vector + Vector2.up * 2f, Vector2.down, 8f, LayerMask.GetMask(new string[] { "Ground" }));
			if (raycastHit2D)
			{
				vector = raycastHit2D.point;
			}
			LatexKind latexKind = ((kind == "ceiling") ? LatexKind.Ceiling : ((kind == "puddle") ? LatexKind.Puddle : LatexKind.Crawler));
			if (latexKind == LatexKind.Ceiling)
			{
				Vector2 vector2 = b.transform.position;
				RaycastHit2D raycastHit2D2 = Physics2D.Raycast(vector2 + Vector2.up * 1f, Vector2.up, 12f, LayerMask.GetMask(new string[] { "Ground" }));
				vector = (raycastHit2D2 ? raycastHit2D2.point : (vector2 + Vector2.up * 6f));
			}
			if (!Latexes.Make(vector, latexKind, b.isRight ? (-1) : 1))
			{
				return "no art";
			}
			return string.Format("liquid latex ({0}) at {1}", latexKind, vector);
		}

		// Token: 0x06000399 RID: 921 RVA: 0x0003C0E8 File Offset: 0x0003A2E8
		public static string DebugGrab(Body b, string how)
		{
			LatexBlob latexBlob = (from x in Latexes.All
				where x && !x.dead && x.shot == LatexBlob.Shot.None
				orderby Vector2.Distance(x.pos, b.transform.position)
				select x).FirstOrDefault<LatexBlob>();
			if (!latexBlob)
			{
				latexBlob = Latexes.Make(b.transform.position + new Vector2(b.isRight ? (-2.5f) : 2.5f, -0.5f), LatexKind.Crawler, b.isRight ? 1 : (-1));
			}
			if (!latexBlob)
			{
				return "no art";
			}
			if (Latexes.Busy)
			{
				Latexes.Release(b, false);
			}
			int num = ((how == "drop") ? 2 : ((how == "step") ? 3 : ((how == "arms") ? 4 : 1)));
			if (num == 2)
			{
				latexBlob.kind = LatexKind.Ceiling;
				latexBlob.pos = b.limbs[1].transform.position + Vector2.up * 6f;
			}
			if (num == 3)
			{
				latexBlob.kind = LatexKind.Puddle;
				latexBlob.pos = b.transform.position + Vector2.down * 0.5f;
			}
			Latexes.Begin(b, latexBlob, num);
			return "liquid latex goes for her (" + how + ")";
		}

		// Token: 0x0600039A RID: 922 RVA: 0x0003C28C File Offset: 0x0003A48C
		public static string DebugSwallow(Body b)
		{
			if (!Latexes.Holder)
			{
				Latexes.DebugGrab(b, "leg");
			}
			if (Latexes.Holder)
			{
				if (Latexes.Now != Latexes.Phase.Using)
				{
					Latexes.StartUsing(b, Latexes.Holder);
				}
				Latexes.Swallow(b, Latexes.Holder);
			}
			return string.Format("phase {0}", Latexes.Now);
		}

		// Token: 0x0600039B RID: 923 RVA: 0x0003C2EF File Offset: 0x0003A4EF
		public static string DebugTransform(Body b)
		{
			if (!Latexes.Holder)
			{
				Latexes.DebugSwallow(b);
			}
			if (Latexes.Holder)
			{
				Latexes.BeginTransform(b, Latexes.Holder);
			}
			return string.Format("phase {0}", Latexes.Now);
		}

		// Token: 0x0600039C RID: 924 RVA: 0x0003C330 File Offset: 0x0003A530
		public static string Info(Body b)
		{
			return string.Format("latex {0}: ", Latexes.All.Count) + string.Join("; ", Latexes.All.Select<LatexBlob, string>((LatexBlob x) => string.Format("{0} at {1} sated {2:0}", x.kind, x.pos, x.sated))) + string.Format("; phase {0} for {1:0.0}s, creep {2}, variant {3}", new object[]
			{
				Latexes.Now,
				Latexes.phaseTime,
				Latexes.CreepStage,
				Latexes.variant
			}) + (Latexes.belly ? string.Format("; belly {0} order {1} at {2} sprite {3}", new object[]
			{
				Latexes.belly.enabled,
				Latexes.belly.sortingOrder,
				Latexes.belly.transform.position,
				Latexes.belly.sprite ? Latexes.belly.sprite.name : "none"
			}) : "");
		}

		// Token: 0x0600039E RID: 926 RVA: 0x0003C4E4 File Offset: 0x0003A6E4
		[CompilerGenerated]
		internal static SpriteRenderer[] <Swallow>g__Links|62_0(int n, int order, float thick, ref Latexes.<>c__DisplayClass62_0 A_3)
		{
			SpriteRenderer[] array = new SpriteRenderer[n];
			for (int i = 0; i < n; i++)
			{
				array[i] = Latexes.Part(A_3.torso, "LewdLatexGut", A_3.seg, 0);
				array[i].sortingOrder = order;
				array[i].color = Latexes.Flesh;
				array[i].transform.localScale = new Vector3(1f, thick, 1f);
			}
			return array;
		}

		// Token: 0x0600039F RID: 927 RVA: 0x0003C554 File Offset: 0x0003A754
		[CompilerGenerated]
		internal static SpriteRenderer <Swallow>g__Tip|62_1(int order, float size, ref Latexes.<>c__DisplayClass62_0 A_2)
		{
			SpriteRenderer spriteRenderer = Latexes.Part(A_2.torso, "LewdLatexGutTip", A_2.tip, 0);
			spriteRenderer.sortingOrder = order;
			spriteRenderer.color = Latexes.Flesh;
			spriteRenderer.transform.localScale = new Vector3(size, size, 1f);
			return spriteRenderer;
		}

		// Token: 0x0400031F RID: 799
		public static readonly List<LatexBlob> All = new List<LatexBlob>();

		// Token: 0x04000322 RID: 802
		private static readonly string[] Thrusts = new string[] { "vagina", "anal" };

		// Token: 0x04000323 RID: 803
		private const float ReachSeconds = 0.4f;

		// Token: 0x04000324 RID: 804
		private const float DropGravity = 26f;

		// Token: 0x04000325 RID: 805
		private const float TransformSeconds = 2.6f;

		// Token: 0x04000326 RID: 806
		private static Limb[] held = new Limb[0];

		// Token: 0x04000327 RID: 807
		private static Vector2 haul;

		// Token: 0x04000328 RID: 808
		private static float phaseTime;

		// Token: 0x04000329 RID: 809
		private static float attemptCooldown;

		// Token: 0x0400032A RID: 810
		private static float lineTimer;

		// Token: 0x0400032B RID: 811
		private static float pulse;

		// Token: 0x0400032C RID: 812
		private static float orgasmGap;

		// Token: 0x0400032D RID: 813
		private static float writhe;

		// Token: 0x0400032E RID: 814
		private static float lookStage;

		// Token: 0x0400032F RID: 815
		private static bool covered;

		// Token: 0x04000330 RID: 816
		private static int variant;

		// Token: 0x04000331 RID: 817
		private static SpriteRenderer belly;

		// Token: 0x04000332 RID: 818
		private static SpriteRenderer gutTentacle;

		// Token: 0x04000333 RID: 819
		private static SpriteRenderer[] gut = new SpriteRenderer[0];

		// Token: 0x04000334 RID: 820
		private static AudioSource wetLoop;

		// Token: 0x04000335 RID: 821
		private static int overOrder;

		// Token: 0x04000336 RID: 822
		private static int underOrder;

		// Token: 0x04000337 RID: 823
		private static bool posed;

		// Token: 0x04000338 RID: 824
		private static float thrustPhase;

		// Token: 0x04000339 RID: 825
		private const int LiftBy = 10000;

		// Token: 0x0400033A RID: 826
		[TupleElementNames(new string[] { "layer", "order" })]
		private static readonly Dictionary<SpriteRenderer, ValueTuple<int, int>> lifted = new Dictionary<SpriteRenderer, ValueTuple<int, int>>();

		// Token: 0x0400033B RID: 827
		private const float CaptureS = 2f;

		// Token: 0x0400033C RID: 828
		private const float TeaseS = 7f;

		// Token: 0x0400033D RID: 829
		private const float EnterS = 3f;

		// Token: 0x0400033E RID: 830
		private const float ThrustS = 11f;

		// Token: 0x0400033F RID: 831
		private const float ClimaxS = 3f;

		// Token: 0x04000340 RID: 832
		private const float RestS = 3f;

		// Token: 0x04000341 RID: 833
		private static readonly Color Flesh = Color.white;

		// Token: 0x04000342 RID: 834
		private static float cycleTime;

		// Token: 0x04000343 RID: 835
		private static float depth;

		// Token: 0x04000344 RID: 836
		private static float swell;

		// Token: 0x04000345 RID: 837
		private static float glowAmount;

		// Token: 0x04000346 RID: 838
		private static float lastThrust;

		// Token: 0x04000347 RID: 839
		private static float spentTime;

		// Token: 0x04000348 RID: 840
		private static float creepShown;

		// Token: 0x04000349 RID: 841
		private static bool firstCycle;

		// Token: 0x0400034A RID: 842
		private static bool climaxed;

		// Token: 0x0400034B RID: 843
		private static bool spent;

		// Token: 0x0400034C RID: 844
		private static SpriteRenderer[][] holders = new SpriteRenderer[0][];

		// Token: 0x0400034D RID: 845
		private static SpriteRenderer[][] teasers = new SpriteRenderer[0][];

		// Token: 0x0400034E RID: 846
		private static SpriteRenderer[] holderTips = new SpriteRenderer[0];

		// Token: 0x0400034F RID: 847
		private static SpriteRenderer[] teaserTips = new SpriteRenderer[0];

		// Token: 0x04000350 RID: 848
		private static SpriteRenderer glow;

		// Token: 0x04000351 RID: 849
		private static Sprite glowSprite;

		// Token: 0x04000352 RID: 850
		private static Material glowMaterial;

		// Token: 0x04000353 RID: 851
		private const float ScatterS = 0.9f;

		// Token: 0x04000354 RID: 852
		private const float ReformS = 1.6f;

		// Token: 0x020001C5 RID: 453
		internal enum Phase
		{
			// Token: 0x040009B0 RID: 2480
			None,
			// Token: 0x040009B1 RID: 2481
			Reaching,
			// Token: 0x040009B2 RID: 2482
			Dragging,
			// Token: 0x040009B3 RID: 2483
			Dropping,
			// Token: 0x040009B4 RID: 2484
			Sinking,
			// Token: 0x040009B5 RID: 2485
			Using,
			// Token: 0x040009B6 RID: 2486
			Inside,
			// Token: 0x040009B7 RID: 2487
			Transforming
		}
	}
}
