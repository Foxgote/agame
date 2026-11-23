using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using HarmonyLib;
using PoseKit;
using ScavArmory;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace LewdStatus
{
	// Token: 0x02000055 RID: 85
	internal static class Bridge
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0001B94D File Offset: 0x00019B4D
		public static bool SwimUp
		{
			get
			{
				return Bridge.swim > 0f;
			}
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0001B95C File Offset: 0x00019B5C
		public static void Tick(Body b)
		{
			SoundLog.Watch(b);
			if (Bridge.faint > 0f)
			{
				Bridge.faint -= Time.deltaTime;
				b.consciousness = 0f;
			}
			if (Bridge.walk > 0f)
			{
				Bridge.walk -= Time.deltaTime;
				if (b.conscious && !Pits.InMaw)
				{
					b.rb.velocity = new Vector2(3.5f, b.rb.velocity.y);
				}
			}
			if (Bridge.swim > 0f)
			{
				Bridge.swim -= Time.deltaTime;
				if (!SlimeBog.Stuck && Mathf.Abs(Bridge.SwimDir.x) > 0.1f)
				{
					b.rb.velocity = new Vector2(Mathf.Sign(Bridge.SwimDir.x) * 3.5f, b.rb.velocity.y);
				}
			}
			if ((Bridge.poll -= Time.unscaledDeltaTime) > 0f)
			{
				return;
			}
			Bridge.poll = 0.5f;
			if (!File.Exists(Bridge.File_))
			{
				return;
			}
			string[] array;
			try
			{
				array = File.ReadAllLines(Bridge.File_);
				File.Delete(Bridge.File_);
			}
			catch
			{
				return;
			}
			foreach (string text in from l in array
				select l.Trim() into l
				where l.Length > 0
				select l)
			{
				try
				{
					Bridge.Run(b, text.Split(new char[] { ' ' }));
				}
				catch (Exception ex)
				{
					Bridge.Say(string.Concat(new string[]
					{
						text,
						": ",
						ex.GetType().Name,
						" ",
						ex.Message
					}));
				}
			}
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0001BB90 File Offset: 0x00019D90
		private static void Say(string s)
		{
			Plugin.Log.LogInfo("[Bridge] " + s);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0001BBA8 File Offset: 0x00019DA8
		private static void Run(Body b, string[] a)
		{
			string text = a[0];
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 1805184399U)
			{
				if (num <= 924336389U)
				{
					if (num <= 444794139U)
					{
						if (num <= 260133711U)
						{
							if (num <= 83472466U)
							{
								if (num <= 50088013U)
								{
									if (num != 37661439U)
									{
										if (num != 50088013U)
										{
											goto IL_93D4;
										}
										if (!(text == "flesh"))
										{
											goto IL_93D4;
										}
										List<FleshBurn> list = (from f in Burning.All
											where f && !f.dead
											orderby Vector2.Distance(f.Center, b.transform.position)
											select f).ToList<FleshBurn>();
										if (a.Length > 1 && a[1] == "hit" && list.Count > 0)
										{
											list[Mathf.Clamp((a.Length > 2) ? int.Parse(a[2]) : 0, 0, list.Count - 1)].gameObject.SendMessage("AnimalHit", (a.Length > 3) ? float.Parse(a[3], CultureInfo.InvariantCulture) : 60f, 1);
										}
										Bridge.Say("flesh: " + string.Join("; ", list.Select<FleshBurn, string>((FleshBurn f) => string.Format("{0} {1:0}/{2:0} at {3:0} m", new object[]
										{
											f.what,
											f.hp,
											f.max,
											Vector2.Distance(f.Center, b.transform.position)
										}))));
										string text2 = "vents: ";
										int i2 = Object.FindObjectsOfType<GasVentHit>().Length;
										Bridge.Say(text2 + i2.ToString() + ", walls: " + string.Join(", ", Walls.All.Where<LewdWall>((LewdWall w) => w2).Select<LewdWall, string>(delegate(LewdWall w)
										{
											string text28 = w2.Kind.ToString();
											string text29 = " ";
											BuildingEntity component19 = w2.GetComponent<BuildingEntity>();
											return text28 + text29 + ((component19 != null) ? component19.health : (-1f)).ToString("0");
										})));
										return;
									}
									else
									{
										if (!(text == "antiserum"))
										{
											goto IL_93D4;
										}
										float num2 = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 50f);
										Antiserum.Ensure();
										Liquids.Registry["antiserum"].onHealthUse.Invoke(num2, b.limbs[1]);
										Bridge.Say(string.Format("antiserum {0:0} ml: host slime {1} stage {2} progress {3:0} calm {4} left {5:0}s doses {6} tamed {7} hostsWant {8}", new object[]
										{
											num2,
											Suit.HostSlime,
											Suit.Stage,
											Plugin.S.suitProgress,
											Suit.HostCalm,
											Plugin.S.hostStableLeft,
											Plugin.S.hostDoses,
											Plugin.S.hostTamed,
											Capture.HostWantsHer
										}));
										return;
									}
								}
								else if (num != 61828529U)
								{
									if (num != 66564749U)
									{
										if (num != 83472466U)
										{
											goto IL_93D4;
										}
										if (!(text == "struggle"))
										{
											goto IL_93D4;
										}
										if (Flowers.Holding)
										{
											Traverse.Create(typeof(Flowers)).Method("Struggle", new object[] { b }).GetValue();
											return;
										}
										if (Pits.InMaw)
										{
											Traverse.Create(typeof(Pits)).Method("Struggle", new object[] { b }).GetValue();
											return;
										}
										if (FleshWall.Holding)
										{
											Traverse.Create(typeof(FleshWall)).Method("Struggle", new object[] { b }).GetValue();
											return;
										}
										if (Worm.Inside)
										{
											Traverse.Create(typeof(Worm)).Method("Struggle", new object[] { b }).GetValue();
											return;
										}
										if (SlimeBog.Stuck)
										{
											Traverse.Create(typeof(SlimeBog)).Method("Struggle", new object[] { b }).GetValue();
											return;
										}
										Bridge.Say("nothing holds her");
										return;
									}
									else
									{
										if (!(text == "slitflood"))
										{
											goto IL_93D4;
										}
										Bridge.Say(Slits.DebugFlood());
										return;
									}
								}
								else
								{
									if (!(text == "trader"))
									{
										goto IL_93D4;
									}
									GameObject gameObject = Resources.Load("trader" + ((a.Length > 1) ? a[1] : "1")) as GameObject;
									if (!gameObject)
									{
										Bridge.Say("no trader prefab");
										return;
									}
									TraderScript component = Object.Instantiate<GameObject>(gameObject, b.limbs[1].transform.position + new Vector2(b.isRight ? 3f : (-3f), 0f), Quaternion.identity).GetComponent<TraderScript>();
									if (component)
									{
										component.MeetPlayer();
									}
									Bridge.Say(string.Format("trader {0} met her: suit {1}, reputation {2:0}", component ? component.character.ToString() : "?", TraderSuit.Variant() ?? "none", component ? component.reputation : 0f));
									return;
								}
							}
							else if (num <= 132037789U)
							{
								if (num != 119152511U)
								{
									if (num != 132037789U)
									{
										goto IL_93D4;
									}
									if (!(text == "wormin"))
									{
										goto IL_93D4;
									}
									Bridge.Say(Worm.InsideState(b));
									return;
								}
								else
								{
									if (!(text == "mitespread"))
									{
										goto IL_93D4;
									}
									Bridge.Say(LatexMites.DebugSpread(b, (a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 60f));
									return;
								}
							}
							else if (num != 180205224U)
							{
								if (num != 255436899U)
								{
									if (num != 260133711U)
									{
										goto IL_93D4;
									}
									if (!(text == "slitbomb"))
									{
										goto IL_93D4;
									}
									Bridge.Say(Slits.DebugBomb(b));
									return;
								}
								else
								{
									if (!(text == "tail"))
									{
										goto IL_93D4;
									}
									SkinVoice.StandardTail.Value = a.Length > 1 && a[1] == "std";
									Bridge.Say("standard tail " + SkinVoice.StandardTail.Value.ToString());
									return;
								}
							}
							else
							{
								if (!(text == "campinfo"))
								{
									goto IL_93D4;
								}
								Camp current = Camps.Current;
								Bridge.Say((current == null) ? "no camp" : (string.Format("camp {0}: trader {1}, ", current.Center, current.Trader ? string.Format("{0} hostile {1} given {2}/{3}", new object[]
								{
									current.Trader.transform.position,
									current.Trader.hostile,
									current.Trader.valueGiven,
									current.Trader.totalValueGiven
								}) : "none") + string.Format("guard {0}, keeper {1}, dead {2}, fire {3}, betrayed {4}, her temp {5:0.0}, dist {6:0.0}", new object[]
								{
									current.Guard ? string.Format("{0} at {1}", current.Guard.mode, current.Guard.pos) : "none",
									current.Keeper ? string.Format("{0} at {1} hp {2:0}", current.Keeper.mode, current.Keeper.pos, current.Keeper.hit ? current.Keeper.hit.health : 0f) : "none",
									current.Dead,
									current.Fire ? current.Fire.condition.ToString("0.00") : "none",
									current.Betrayed,
									b.temperature,
									Vector2.Distance(b.transform.position, current.Center)
								})));
								return;
							}
						}
						else if (num <= 341365879U)
						{
							if (num <= 294577280U)
							{
								if (num != 262456946U)
								{
									if (num != 294577280U)
									{
										goto IL_93D4;
									}
									if (!(text == "nap"))
									{
										goto IL_93D4;
									}
									b.energy = Mathf.Min(b.energy, (a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 40f);
									b.TakeANap();
									Bridge.Say(string.Format("nap (energy {0:0})", b.energy));
									return;
								}
								else
								{
									if (!(text == "ddlock"))
									{
										goto IL_93D4;
									}
									Bridge.Say("devices locked: " + Devices.LockOn(b, "debug").ToString() + " -> " + string.Join(", ", from i in Devices.LockedOn(b)
										select i.id));
									return;
								}
							}
							else if (num != 327705747U)
							{
								if (num != 330088921U)
								{
									if (num != 341365879U)
									{
										goto IL_93D4;
									}
									if (!(text == "flowerhit"))
									{
										goto IL_93D4;
									}
									FlowerPlant flowerPlant = (from x in Flowers.Plants
										where x && x.isActiveAndEnabled
										orderby Vector2.Distance(x.transform.position, b.transform.position)
										select x).FirstOrDefault<FlowerPlant>();
									if (!flowerPlant)
									{
										Bridge.Say("no flower up");
										return;
									}
									int num3 = ((a.Length > 1) ? int.Parse(a[1]) : 1);
									for (int j = 0; j < num3; j++)
									{
										flowerPlant.gameObject.SendMessage("AnimalHit", 30f, 1);
									}
									Bridge.Say(string.Format("flower hit {0} times: burrow {1} hits {2}", num3, flowerPlant.burrow, flowerPlant.hits));
									return;
								}
								else
								{
									if (!(text == "pitbreak"))
									{
										goto IL_93D4;
									}
									if (Bridge.lastPit == null)
									{
										Bridge.Say("no pit");
										return;
									}
									WorldGeneration world = WorldGeneration.world;
									int num4 = 0;
									if (a.Length > 1 && a[1] == "lips")
									{
										for (int k = -Bridge.lastPit.Half; k <= Bridge.lastPit.Half; k++)
										{
											Vector2Int vector2Int;
											vector2Int..ctor(Bridge.lastPit.Top.x + k, Bridge.lastPit.Top.y);
											if (world.GetBlock(vector2Int) != 0)
											{
												world.SetBlock(vector2Int, 0);
												num4++;
											}
										}
									}
									else
									{
										int bottom = Bridge.lastPit.RimCells.Min<Vector2Int>((Vector2Int c) => c.y);
										IEnumerable<Vector2Int> rimCells = Bridge.lastPit.RimCells;
										Func<Vector2Int, bool> func;
										Func<Vector2Int, bool> <>9__36;
										if ((func = <>9__36) == null)
										{
											func = (<>9__36 = (Vector2Int c) => c.y <= bottom + 1);
										}
										foreach (Vector2Int vector2Int2 in rimCells.Where<Vector2Int>(func))
										{
											if (world.GetBlock(vector2Int2) != 0)
											{
												world.SetBlock(vector2Int2, 0);
												num4++;
											}
										}
									}
									Bridge.Say(string.Format("broke {0} blocks of the pit ({1})", num4, (a.Length > 1) ? a[1] : "floor"));
									return;
								}
							}
							else
							{
								if (!(text == "equip"))
								{
									goto IL_93D4;
								}
								GameObject gameObject2 = Resources.Load(a[1]) as GameObject;
								if (!gameObject2)
								{
									Bridge.Say("no item " + a[1]);
									return;
								}
								Item component2 = Object.Instantiate<GameObject>(gameObject2, b.transform.position, Quaternion.identity).GetComponent<Item>();
								if (component2 && component2.Stats.wearable)
								{
									Item wearableBySlotID = b.GetWearableBySlotID(component2.Stats.wearSlotId);
									if (wearableBySlotID)
									{
										b.DropWearable(wearableBySlotID);
									}
									b.WearWearable(component2);
								}
								Bridge.Say("equip " + a[1] + ": slot " + (component2 ? component2.Stats.wearSlotId : "-"));
								return;
							}
						}
						else if (num <= 408537171U)
						{
							if (num != 369992131U)
							{
								if (num != 385449219U)
								{
									if (num != 408537171U)
									{
										goto IL_93D4;
									}
									if (!(text == "slitinfo"))
									{
										goto IL_93D4;
									}
									Bridge.Say(Slits.Info(b));
									return;
								}
								else
								{
									if (!(text == "darkpool"))
									{
										goto IL_93D4;
									}
									SlimeFluid.DebugPool(b, true);
									if (a.Length > 1 && a[1] == "hosts")
									{
										foreach (SlimeFluid.DarkPool darkPool in SlimeFluid.DarkPools)
										{
											darkPool.Hosts = false;
										}
										Scavengers.PlaceBogHosts();
									}
									Bridge.Say(string.Format("dark slime pool here; {0} dark pools", SlimeFluid.DarkPools.Count));
									return;
								}
							}
							else
							{
								if (!(text == "blocks"))
								{
									goto IL_93D4;
								}
								WorldGeneration world2 = WorldGeneration.world;
								StringBuilder stringBuilder = new StringBuilder();
								for (int l2 = 0; l2 < world2.tiles.Length; l2++)
								{
									BlockInfo blockInfo = null;
									try
									{
										blockInfo = world2.GetBlockInfo((ushort)l2);
									}
									catch
									{
									}
									stringBuilder.Append(string.Format("{0}:{1}({2:0}) ", l2, (blockInfo != null) ? blockInfo.name : "-", (blockInfo != null) ? blockInfo.health : 0f));
								}
								string text3 = "blocks ";
								StringBuilder stringBuilder2 = stringBuilder;
								Bridge.Say(text3 + ((stringBuilder2 != null) ? stringBuilder2.ToString() : null));
								return;
							}
						}
						else if (num != 425118420U)
						{
							if (num != 438634916U)
							{
								if (num != 444794139U)
								{
									goto IL_93D4;
								}
								if (!(text == "clothes"))
								{
									goto IL_93D4;
								}
								Bridge.Say("clothes: " + string.Join(", ", from i in Clothes.Worn(b)
									select string.Format("{0} {1:P0}", i.id, i.condition)));
								return;
							}
							else
							{
								if (!(text == "pouch"))
								{
									goto IL_93D4;
								}
								Item item = b.GetAllWearables().FirstOrDefault<Item>((Item w) => w2 && w2.id.StartsWith("lewd_tac_"));
								Container container = (item ? item.GetComponentInChildren<Container>() : null);
								if (!container)
								{
									Bridge.Say("no tactical suit on her");
									return;
								}
								using (IEnumerator<string> enumerator3 = a.Skip<string>(1).GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										string text4 = enumerator3.Current;
										GameObject gameObject3 = Utils.Create(text4, b.limbs[1].transform.position, 0f);
										Item item2 = (gameObject3 ? gameObject3.GetComponent<Item>() : null);
										if (!item2)
										{
											Bridge.Say("no item " + text4);
										}
										else
										{
											bool flag = container.CanHoldItem(item2);
											if (flag)
											{
												container.LoadItem(item2);
											}
											Bridge.Say(string.Format("pouch {0} ({1:0.00}): {2}, pouch {3:0.00}/{4:0.00}", new object[]
											{
												text4,
												item2.totalWeight,
												flag ? "in" : "refused",
												container.GetHoldingWeight(),
												container.maxWeight
											}));
										}
									}
									return;
								}
							}
						}
						else
						{
							if (!(text == "lights"))
							{
								goto IL_93D4;
							}
							Limb limb = b.LimbByName("Head");
							Limb limb2 = b.LimbByName("UpTorso");
							AnimatorClipInfo[] currentAnimatorClipInfo = b.bodyAnimator.GetCurrentAnimatorClipInfo(0);
							Bridge.Say(string.Format("clip {0} head {1} uptorso {2}", (currentAnimatorClipInfo.Length != 0) ? currentAnimatorClipInfo[0].clip.name : "-", limb ? limb.transform.position : Vector3.zero, limb2 ? limb2.transform.position : Vector3.zero));
							foreach (Light2D light2D in Object.FindObjectsOfType<Light2D>())
							{
								if (Vector2.Distance(light2D.transform.position, b.transform.position) < 4f)
								{
									Bridge.Say(string.Format("light {0} on {1} at {2} on {3} r {4:0.00} i {5:0.00}", new object[]
									{
										light2D.name,
										light2D.transform.parent ? light2D.transform.parent.name : "-",
										light2D.transform.position,
										light2D.enabled && light2D.gameObject.activeInHierarchy,
										light2D.pointLightOuterRadius,
										light2D.intensity
									}));
								}
							}
							return;
						}
					}
					else if (num <= 654774252U)
					{
						if (num <= 546079117U)
						{
							if (num <= 508974720U)
							{
								if (num != 473536235U)
								{
									if (num != 508974720U)
									{
										goto IL_93D4;
									}
									if (!(text == "crawl"))
									{
										goto IL_93D4;
									}
									Bridge.Say(WallCrawl.DebugClimb(b, (a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 4f));
									return;
								}
								else
								{
									if (!(text == "ddfree"))
									{
										goto IL_93D4;
									}
									Traverse.Create(typeof(Devices)).Method("TryFree", new object[] { b }).GetValue();
									Bridge.Say("free attempt -> " + string.Join(", ", from i in Devices.LockedOn(b)
										select i.id));
									return;
								}
							}
							else if (num != 511673672U)
							{
								if (num != 534583185U)
								{
									if (num != 546079117U)
									{
										goto IL_93D4;
									}
									if (!(text == "sleepstate"))
									{
										goto IL_93D4;
									}
									Bridge.Say(string.Format("sleeping {0} conscious {1} ({2:0}) energy {3:0} timescale {4:0.0} latexComfort {5:0.00} nudeComfort {6:0.00} suitWorn {7:0}", new object[]
									{
										b.sleeping,
										b.conscious,
										b.consciousness,
										b.energy,
										Time.timeScale,
										Plugin.S.latexComfort,
										Plugin.S.nudeComfort,
										Plugin.S.suitWorn
									}));
									return;
								}
								else
								{
									if (!(text == "latexgrab"))
									{
										goto IL_93D4;
									}
									Bridge.Say(Latexes.DebugGrab(b, (a.Length > 1) ? a[1] : "leg"));
									return;
								}
							}
							else
							{
								if (!(text == "ragdoll"))
								{
									goto IL_93D4;
								}
								b.Ragdoll();
								Bridge.Say(string.Format("ragdoll asked: standing {0}, posed {1}, captured {2}", b.standing, HerPuppet.Holds(b), Capture.Active));
								return;
							}
						}
						else if (num <= 588970055U)
						{
							if (num != 563128878U)
							{
								if (num != 571821808U)
								{
									if (num != 588970055U)
									{
										goto IL_93D4;
									}
									if (!(text == "treat"))
									{
										goto IL_93D4;
									}
									Scavenger scavenger = (from x in Scavengers.All
										where x
										orderby Vector2.Distance(x.pos, b.transform.position)
										select x).FirstOrDefault<Scavenger>();
									Ultimatum.Treat(b, scavenger);
									Bridge.Say("treated: " + string.Join(", ", from x in b.limbs
										where x && x.GetComponent<TourniquetScript>()
										select x.name + " tourniquet"));
									return;
								}
								else
								{
									if (!(text == "tentcut"))
									{
										goto IL_93D4;
									}
									WorldGeneration world3 = WorldGeneration.world;
									TentacleTrap tentacleTrap = (from x in Tentacles.Traps
										where x
										orderby Vector2.Distance(x.transform.position, b.transform.position)
										select x).FirstOrDefault<TentacleTrap>();
									if (!tentacleTrap)
									{
										Bridge.Say("no tentacles");
										return;
									}
									foreach (Vector2Int vector2Int3 in tentacleTrap.Holds)
									{
										world3.SetBlock(vector2Int3, 0);
									}
									Bridge.Say(string.Format("dug out {0} ceiling blocks over the tentacles at {1}", tentacleTrap.Holds.Count, tentacleTrap.transform.position));
									return;
								}
							}
							else
							{
								if (!(text == "slitburn"))
								{
									goto IL_93D4;
								}
								SlitMouth slitMouth = (from x in Slits.All
									where x
									orderby Vector2.Distance(x.Mouth, b.transform.position)
									select x).FirstOrDefault<SlitMouth>();
								FleshBurn fleshBurn = (slitMouth ? slitMouth.GetComponent<FleshBurn>() : null);
								if (fleshBurn)
								{
									Burning.Damage(fleshBurn, (a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 100f, "the bridge", true);
								}
								Bridge.Say(fleshBurn ? ("slit burnt: " + Slits.Info(b)) : "no womb slit");
								return;
							}
						}
						else if (num != 602051538U)
						{
							if (num != 626447106U)
							{
								if (num != 654774252U)
								{
									goto IL_93D4;
								}
								if (!(text == "miteinfo"))
								{
									goto IL_93D4;
								}
								Bridge.Say(LatexMites.Info(b));
								return;
							}
							else
							{
								if (!(text == "bustdbg"))
								{
									goto IL_93D4;
								}
								if (a.Length > 1)
								{
									Bust.DebugRed = a[1] == "red";
								}
								Bridge.Say(Bust.Debug(b));
								return;
							}
						}
						else
						{
							if (!(text == "pondinfo"))
							{
								goto IL_93D4;
							}
							Bridge.Say(LatexPond.Info(b));
							return;
						}
					}
					else if (num <= 775068458U)
					{
						if (num <= 727472617U)
						{
							if (num != 726832650U)
							{
								if (num != 727472617U)
								{
									goto IL_93D4;
								}
								if (!(text == "webinfo"))
								{
									goto IL_93D4;
								}
								SpriteRenderer component3 = b.limbs[1].GetComponent<SpriteRenderer>();
								SpriteRenderer[] array2 = (from r in Object.FindObjectsOfType<SpriteRenderer>()
									where r.name == "LewdCocoon"
									select r).ToArray<SpriteRenderer>();
								Bridge.Say(string.Format("torso order {0}; cocoons: ", component3.sortingOrder) + string.Join(", ", array2.Select<SpriteRenderer, string>((SpriteRenderer r) => string.Format("order {0} alpha {1:0.00}", r.sortingOrder, r.color.a))) + string.Format("; caught {0} cocoon {1}", Webs.Caught, Webs.InCocoon));
								return;
							}
							else
							{
								if (!(text == "fold"))
								{
									goto IL_93D4;
								}
								GameObject gameObject4 = GameObject.Find("LewdHealthHeader");
								if (gameObject4)
								{
									gameObject4.GetComponent<Button>().onClick.Invoke();
								}
								Bridge.Say(string.Format("header clicked: folded {0}", LewdHealthPanel.Collapsed.Value));
								return;
							}
						}
						else if (num != 772665558U)
						{
							if (num != 774867328U)
							{
								if (num != 775068458U)
								{
									goto IL_93D4;
								}
								if (!(text == "console"))
								{
									goto IL_93D4;
								}
								ConsoleScript consoleScript = Object.FindObjectOfType<ConsoleScript>();
								if (!consoleScript)
								{
									Bridge.Say("no console");
									return;
								}
								Traverse.Create(consoleScript).Method("TryExecuteCommand", new Type[]
								{
									typeof(string[]),
									typeof(bool)
								}, null).GetValue(new object[]
								{
									a.Skip<string>(1).ToArray<string>(),
									true
								});
								IList list2 = Traverse.Create(consoleScript).Field("executedCommands").GetValue() as IList;
								string text5 = "console ran \"{0}\": history {1}, arousal {2:0}";
								object obj = string.Join(" ", a.Skip<string>(1));
								object obj2;
								if (list2 == null)
								{
									obj2 = "?";
								}
								else
								{
									int i2 = list2.Count;
									obj2 = i2.ToString();
								}
								Bridge.Say(string.Format(text5, obj, obj2, Plugin.S.arousal));
								return;
							}
							else
							{
								if (!(text == "suit"))
								{
									goto IL_93D4;
								}
								Suit.ForceNextStage(b);
								Bridge.Say(string.Format("suit stage {0} colour {1} enjoying {2}", Suit.Stage, Plugin.S.suitColor, Suit.Enjoying));
								return;
							}
						}
						else
						{
							if (!(text == "gunstate"))
							{
								goto IL_93D4;
							}
							GunScript gunScript = (b.HoldingItem(b.handSlot) ? b.GetItem(b.handSlot).GetComponent<GunScript>() : null);
							string text6;
							if (!gunScript)
							{
								text6 = "no gun";
							}
							else
							{
								text6 = string.Format("gun {0}: mag {1} rounds {2} chamber {3}; carried mags: ", new object[] { gunScript.name, gunScript.hasMag, gunScript.roundsInMag, gunScript.roundInChamber }) + string.Join(", ", (from i in b.GetAllItemsThorough()
									where i && i.GetComponent<AmmoScript>()
									select i).Select<Item, string>(delegate(Item i)
								{
									Container container3;
									return string.Format("{0}({1})", i.id, i.GetComponent<AmmoScript>().rounds) + (i.TryGetParentContainer(ref container3) ? ("@" + (container3.GetComponent<Item>() ? container3.GetComponent<Item>().id : container3.name)) : "");
								}));
							}
							Bridge.Say(text6);
							return;
						}
					}
					else if (num <= 862393864U)
					{
						if (num != 799090015U)
						{
							if (num != 804546073U)
							{
								if (num != 862393864U)
								{
									goto IL_93D4;
								}
								if (!(text == "slithit"))
								{
									goto IL_93D4;
								}
								SlitMouth slitMouth2 = (from x in Slits.All
									where x
									orderby Vector2.Distance(x.Mouth, b.transform.position)
									select x).FirstOrDefault<SlitMouth>();
								int num5 = ((a.Length > 1) ? int.Parse(a[1]) : 1);
								int num6 = 0;
								while (slitMouth2 && num6 < num5)
								{
									slitMouth2.gameObject.SendMessage("AnimalHit", 30f, 1);
									num6++;
								}
								Bridge.Say(slitMouth2 ? string.Format("slit hit x{0}: {1}", num5, Slits.Info(b)) : "no womb slit");
								return;
							}
							else
							{
								if (!(text == "res"))
								{
									goto IL_93D4;
								}
								CultureInfo invariantCulture = CultureInfo.InvariantCulture;
								GameObject gameObject5 = Resources.Load(a[1]) as GameObject;
								Vector2 vector = b.transform.position + new Vector2((a.Length > 2) ? float.Parse(a[2], invariantCulture) : 6f, (a.Length > 3) ? float.Parse(a[3], invariantCulture) : 1f);
								if (gameObject5)
								{
									Object.Instantiate<GameObject>(gameObject5, vector, Quaternion.identity);
								}
								Bridge.Say(gameObject5 ? string.Format("{0} at {1}", a[1], vector) : "no such resource");
								return;
							}
						}
						else
						{
							if (!(text == "latexinfo"))
							{
								goto IL_93D4;
							}
							Bridge.Say(Latexes.Info(b));
							return;
						}
					}
					else if (num != 877783941U)
					{
						if (num != 908888153U)
						{
							if (num != 924336389U)
							{
								goto IL_93D4;
							}
							if (!(text == "spawnid"))
							{
								goto IL_93D4;
							}
							float num7 = ((a.Length > 2) ? float.Parse(a[2], CultureInfo.InvariantCulture) : 2f);
							Bridge.Say(Utils.Create(a[1], b.limbs[1].transform.position + new Vector2(num7, 0f), 0f) ? ("spawned " + a[1]) : "nothing");
							return;
						}
						else
						{
							if (!(text == "npcs"))
							{
								goto IL_93D4;
							}
							WorldGeneration world4 = WorldGeneration.world;
							using (IEnumerator<Scavenger> enumerator5 = Scavengers.All.Where<Scavenger>((Scavenger x) => x).GetEnumerator())
							{
								while (enumerator5.MoveNext())
								{
									Scavenger scavenger2 = enumerator5.Current;
									Vector2Int vector2Int4 = world4.WorldToBlockPos(scavenger2.pos + Vector2.down * 0.3f);
									bool flag2 = Physics2D.OverlapBox(world4.BlockToWorldPos(vector2Int4), new Vector2(0.8f, 0.8f), 0f, LayerMask.GetMask(new string[] { "Ground" }));
									Bridge.Say(string.Format("npc {0} {1} at {2} vel {3} grounded {4} block under {5} collider {6} dist {7:0}", new object[]
									{
										scavenger2.slimeHost ? "host" : "scav",
										scavenger2.mode,
										scavenger2.pos,
										scavenger2.vel,
										scavenger2.grounded,
										world4.GetBlock(vector2Int4),
										flag2,
										Vector2.Distance(scavenger2.pos, b.transform.position)
									}));
								}
								return;
							}
							goto IL_635E;
						}
					}
					else
					{
						if (!(text == "clothesfast"))
						{
							goto IL_93D4;
						}
						Clothes.DebugRate = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 20f);
						Bridge.Say(string.Format("living suit takes/tears clothes x{0:0.#}", Clothes.DebugRate));
						return;
					}
				}
				else if (num <= 1414647269U)
				{
					if (num <= 1176627656U)
					{
						if (num <= 1056631982U)
						{
							if (num <= 979982427U)
							{
								if (num != 958856746U)
								{
									if (num != 979982427U)
									{
										goto IL_93D4;
									}
									if (!(text == "heal"))
									{
										goto IL_93D4;
									}
									foreach (Limb limb3 in b.limbs)
									{
										limb3.skinHealth = 100f;
										limb3.muscleHealth = 100f;
										limb3.pain = 0f;
									}
									b.bloodOxygen = 100f;
									b.stamina = 100f;
									b.painShock = 0f;
									Bridge.Say("healed");
									return;
								}
								else
								{
									if (!(text == "litbomb"))
									{
										goto IL_93D4;
									}
									float num8 = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 8f);
									float num9 = ((a.Length > 2) ? float.Parse(a[2], CultureInfo.InvariantCulture) : 1f);
									GameObject gameObject6 = Object.Instantiate<GameObject>(Resources.Load("dynamite") as GameObject, b.transform.position + new Vector2(num8, num9), Quaternion.identity);
									Item component4 = gameObject6.GetComponent<Item>();
									component4.Stats.useAction.Invoke(b, component4);
									Bridge.Say(string.Format("a lit stick of dynamite at {0}", gameObject6.transform.position));
									return;
								}
							}
							else if (num != 986211098U)
							{
								if (num != 1017561621U)
								{
									if (num != 1056631982U)
									{
										goto IL_93D4;
									}
									if (!(text == "wormdmg"))
									{
										goto IL_93D4;
									}
									using (List<WormBeast>.Enumerator enumerator6 = Worm.Beasts.GetEnumerator())
									{
										while (enumerator6.MoveNext())
										{
											WormBeast wormBeast = enumerator6.Current;
											if (wormBeast)
											{
												wormBeast.TakeHit(float.Parse(a[1], CultureInfo.InvariantCulture), "bridge");
												Bridge.Say("worm " + wormBeast.State(b));
											}
										}
										return;
									}
									goto IL_4386;
								}
								else
								{
									if (!(text == "ddsell"))
									{
										goto IL_93D4;
									}
									TraderScript traderScript = (from t in Object.FindObjectsOfType<TraderScript>()
										orderby Vector2.Distance(t.transform.position, b.transform.position)
										select t).FirstOrDefault<TraderScript>();
									if (!traderScript)
									{
										Bridge.Say("no trader");
										return;
									}
									List<Item> list3 = Devices.LockedOn(b);
									Patches patchInfo = Harmony.GetPatchInfo(AccessTools.Method(typeof(TraderScript), "GiveItem", null, null));
									string text7 = "GiveItem patches: ";
									string text8;
									if (patchInfo != null)
									{
										text8 = string.Join(", ", patchInfo.Prefixes.Select<Patch, string>((Patch p) => string.Concat(new string[]
										{
											"prefix ",
											p.owner,
											"/",
											p.PatchMethod.DeclaringType.Name,
											".",
											p.PatchMethod.Name
										}))) + "; " + string.Join(", ", from p in patchInfo.Postfixes.Concat<Patch>(patchInfo.Transpilers)
											select p.owner + "/" + p.PatchMethod.Name);
									}
									else
									{
										text8 = "none";
									}
									Bridge.Say(text7 + text8);
									int value = Traverse.Create(traderScript).Field("valueGiven").GetValue<int>();
									foreach (Item item3 in list3)
									{
										traderScript.GiveItem(item3);
									}
									Bridge.Say(string.Format("ddsell: valueGiven {0} -> {1}; tried {2}; still locked on her: ", value, Traverse.Create(traderScript).Field("valueGiven").GetValue<int>(), list3.Count) + string.Join(", ", from i in Devices.LockedOn(b)
										select i.id));
									return;
								}
							}
							else
							{
								if (!(text == "wormbeat"))
								{
									goto IL_93D4;
								}
								using (List<WormBeast>.Enumerator enumerator6 = Worm.Beasts.GetEnumerator())
								{
									while (enumerator6.MoveNext())
									{
										WormBeast wormBeast2 = enumerator6.Current;
										if (wormBeast2)
										{
											wormBeast2.DebugBeat();
											Bridge.Say("worm beaten: " + wormBeast2.State(b));
										}
									}
									return;
								}
								goto IL_5ACA;
							}
						}
						else if (num <= 1146800830U)
						{
							if (num != 1076498297U)
							{
								if (num != 1146800830U)
								{
									goto IL_93D4;
								}
								if (!(text == "quake"))
								{
									goto IL_93D4;
								}
								if (WorldGeneration.world)
								{
									WorldGeneration.world.earthquakeTime = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 5f);
								}
								string text9 = "earthquake {0:0.0}s";
								WorldGeneration world5 = WorldGeneration.world;
								Bridge.Say(string.Format(text9, (world5 != null) ? new float?(world5.earthquakeTime) : null));
								return;
							}
							else
							{
								if (!(text == "hitvent"))
								{
									goto IL_93D4;
								}
								GasVentHit gasVentHit = (from x in Object.FindObjectsOfType<GasVentHit>()
									orderby Vector2.Distance(x.transform.position, b.transform.position)
									select x).FirstOrDefault<GasVentHit>();
								if (gasVentHit)
								{
									BuildingEntity component5 = gasVentHit.GetComponent<BuildingEntity>();
									if (component5)
									{
										component5.health -= 45f;
									}
									gasVentHit.SendMessage("AnimalHit", 45f);
								}
								string text10;
								if (!gasVentHit)
								{
									text10 = "no vent";
								}
								else
								{
									string text11 = "hit a vent: health {0:0}";
									BuildingEntity component6 = gasVentHit.GetComponent<BuildingEntity>();
									text10 = string.Format(text11, (component6 != null) ? new float?(component6.health) : null);
								}
								Bridge.Say(text10);
								return;
							}
						}
						else if (num != 1154488495U)
						{
							if (num != 1162665365U)
							{
								if (num != 1176627656U)
								{
									goto IL_93D4;
								}
								if (!(text == "latexin"))
								{
									goto IL_93D4;
								}
								Bridge.Say(Latexes.DebugSwallow(b));
								return;
							}
							else
							{
								if (!(text == "liq"))
								{
									goto IL_93D4;
								}
								WorldGeneration world6 = WorldGeneration.world;
								Vector2Int vector2Int5 = world6.WorldToBlockPos(b.transform.position);
								for (int m = 3; m >= -7; m--)
								{
									StringBuilder stringBuilder3 = new StringBuilder();
									for (int n2 = -5; n2 <= 5; n2++)
									{
										Vector2Int vector2Int6;
										vector2Int6..ctor(vector2Int5.x + n2, vector2Int5.y + m);
										int liquid = (int)FluidManager.main.GetLiquid(vector2Int6.x, vector2Int6.y);
										stringBuilder3.Append((n2 == 0 && m == 0) ? '@' : ((world6.GetBlock(vector2Int6) != 0) ? '#' : ((liquid != 0) ? ((char)(48 + liquid % 10)) : '.')));
									}
									Bridge.Say(string.Format("liq {0}: {1}", vector2Int5.y + m, stringBuilder3));
								}
								return;
							}
						}
						else
						{
							if (!(text == "boxfill"))
							{
								goto IL_93D4;
							}
							GameObject gameObject7 = Utils.Create(a[1], b.limbs[1].transform.position, 0f);
							Item item4 = ((gameObject7 != null) ? gameObject7.GetComponent<Item>() : null);
							GameObject gameObject8 = Utils.Create(a[2], b.limbs[1].transform.position, 0f);
							Item item5 = ((gameObject8 != null) ? gameObject8.GetComponent<Item>() : null);
							if (!item4 || !item5)
							{
								Bridge.Say("no such items");
								return;
							}
							AmmoScript component7 = item4.GetComponent<AmmoScript>();
							AmmoScript component8 = item5.GetComponent<AmmoScript>();
							component7.rounds = ((a.Length > 3) ? int.Parse(a[3]) : 0);
							b.CombineItems(item4, item5);
							Bridge.Say(string.Format("boxfill: {0} {1}/{2}, {3} {4} left", new object[] { item4.id, component7.rounds, component7.maxRounds, item5.id, component8.rounds }));
							Object.Destroy(item4.gameObject);
							Object.Destroy(item5.gameObject);
							return;
						}
					}
					else if (num <= 1254773041U)
					{
						if (num <= 1205575210U)
						{
							if (num != 1185048109U)
							{
								if (num != 1205575210U)
								{
									goto IL_93D4;
								}
								if (!(text == "mitepiece"))
								{
									goto IL_93D4;
								}
								Bridge.Say(LatexMites.DebugPiece(b, (a.Length > 1) ? int.Parse(a[1]) : 1));
								return;
							}
							else
							{
								if (!(text == "surrender"))
								{
									goto IL_93D4;
								}
								Capture.Surrender(b);
								Bridge.Say(string.Format("surrendered {0}", Capture.Surrendered));
								return;
							}
						}
						else if (num != 1227036488U)
						{
							if (num != 1229280041U)
							{
								if (num != 1254773041U)
								{
									goto IL_93D4;
								}
								if (!(text == "ddlist"))
								{
									goto IL_93D4;
								}
								Bridge.Say("worn: " + string.Join(", ", from w in b.GetAllWearables()
									where w2
									select w2.id + (Devices.IsLocked(w2) ? "(locked)" : "")) + string.Format("; bound {0} gagged {1} belted {2} hobbled {3} speed {4:0.00}", new object[]
								{
									Devices.HandsBound(b),
									Devices.Gagged(b),
									Devices.Belted(b),
									Devices.Hobbled(b),
									b.actualMaxSpeed
								}));
								return;
							}
							else
							{
								if (!(text == "tp"))
								{
									goto IL_93D4;
								}
								Bridge.Move(b, new Vector2(float.Parse(a[1], CultureInfo.InvariantCulture), float.Parse(a[2], CultureInfo.InvariantCulture)));
								Bridge.Dump(b);
								return;
							}
						}
						else
						{
							if (!(text == "swimup"))
							{
								goto IL_93D4;
							}
							Bridge.swim = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 4f);
							Bridge.SwimDir = ((a.Length > 3) ? new Vector2(float.Parse(a[2], CultureInfo.InvariantCulture), float.Parse(a[3], CultureInfo.InvariantCulture)) : Vector2.up);
							Bridge.Say(string.Format("swimming up for {0:0}s", Bridge.swim));
							return;
						}
					}
					else if (num <= 1362809445U)
					{
						if (num != 1263673922U)
						{
							if (num != 1275707616U)
							{
								if (num != 1362809445U)
								{
									goto IL_93D4;
								}
								if (!(text == "hp"))
								{
									goto IL_93D4;
								}
								Bridge.Say(string.Format("limbs skin {0:0.0} muscle {1:0.0} held {2}", b.limbs.Sum<Limb>(delegate(Limb l)
								{
									if (!l)
									{
										return 0f;
									}
									return l.skinHealth;
								}), b.limbs.Sum<Limb>(delegate(Limb l)
								{
									if (!l)
									{
										return 0f;
									}
									return l.muscleHealth;
								}), Scenes.Held));
								return;
							}
							else
							{
								if (!(text == "spawnlist"))
								{
									goto IL_93D4;
								}
								Command command = ConsoleScript.SearchExact("spawn");
								List<string> list5;
								List<string> list4 = ((((command != null) ? command.argAutofill : null) != null && command.argAutofill.TryGetValue(0, out list5)) ? list5 : null);
								Bridge.Say((list4 == null) ? "spawn has no autofill yet" : (string.Format("spawn autofill {0}: ", list4.Count) + string.Join(", ", list4.Where<string>((string x) => x.IndexOf(a[1], StringComparison.OrdinalIgnoreCase) >= 0))));
								return;
							}
						}
						else
						{
							if (!(text == "st"))
							{
								goto IL_93D4;
							}
							Traverse traverse = Traverse.Create(Plugin.S).Field(a[1]);
							if (!traverse.FieldExists())
							{
								Bridge.Say("no state field " + a[1]);
								return;
							}
							object obj3 = ((traverse.GetValueType() == typeof(bool)) ? (a[2] == "1" || a[2] == "true") : ((traverse.GetValueType() == typeof(int)) ? int.Parse(a[2]) : float.Parse(a[2], CultureInfo.InvariantCulture)));
							traverse.SetValue(obj3);
							Bridge.Say(string.Format("state {0} = {1}", a[1], obj3));
							return;
						}
					}
					else if (num != 1410115415U)
					{
						if (num != 1412654217U)
						{
							if (num != 1414647269U)
							{
								goto IL_93D4;
							}
							if (!(text == "latexcorr"))
							{
								goto IL_93D4;
							}
							if (a.Length > 1)
							{
								Plugin.S.latexCorruption = Mathf.Clamp(float.Parse(a[1], CultureInfo.InvariantCulture), 0f, 100f);
							}
							Bridge.Say(string.Format("latex corruption {0:0.0}, suit latex {1}, taking her {2}", Plugin.S.latexCorruption, Suit.Latex, LatexTakeover.Active));
							return;
						}
						else
						{
							if (!(text == "pos"))
							{
								goto IL_93D4;
							}
							Bridge.Dump(b);
							return;
						}
					}
					else
					{
						if (!(text == "get"))
						{
							goto IL_93D4;
						}
						Traverse traverse2 = Traverse.Create(Plugin.S).Field(a[1]);
						Bridge.Say(traverse2.FieldExists() ? string.Format("state {0} = {1}", a[1], traverse2.GetValue()) : ("no state field " + a[1]));
						return;
					}
				}
				else if (num <= 1587342749U)
				{
					if (num <= 1469170932U)
					{
						if (num <= 1437070040U)
						{
							if (num != 1425233679U)
							{
								if (num != 1437070040U)
								{
									goto IL_93D4;
								}
								if (!(text == "scavwith"))
								{
									goto IL_93D4;
								}
								foreach (Scavenger scavenger3 in Scavengers.All.ToArray())
								{
									if (scavenger3 && Vector2.Distance(scavenger3.pos, b.transform.position) < 40f)
									{
										Scavengers.All.Remove(scavenger3);
										Object.Destroy(scavenger3.gameObject);
									}
								}
								WeaponDef weaponDef = Scavengers.Weapons.FirstOrDefault<WeaponDef>((WeaponDef x) => x.Id == a[1]);
								if (weaponDef == null)
								{
									Bridge.Say("no weapon " + a[1]);
									return;
								}
								float num10 = ((a.Length > 2) ? float.Parse(a[2], CultureInfo.InvariantCulture) : 5f);
								Scavenger scavenger4 = Scavenger.Make(b.transform.position + new Vector2(num10, 0f), weaponDef, Random.value < 0.5f, null, false, null);
								Bridge.lastScav = scavenger4;
								Bridge.Say(string.Format("raider with {0} at {1}", weaponDef.Id, scavenger4.pos));
								return;
							}
							else
							{
								if (!(text == "near"))
								{
									goto IL_93D4;
								}
								Limb limb4 = b.LimbByName("Head");
								Vector2 vector2 = (limb4 ? limb4.transform.position : b.transform.position);
								foreach (Renderer renderer in Object.FindObjectsOfType<Renderer>())
								{
									if (renderer.enabled && renderer.gameObject.activeInHierarchy)
									{
										Bounds bounds = renderer.bounds;
										if (bounds.size.x <= 30f && bounds.Intersects(new Bounds(vector2, new Vector3(1.2f, 1.2f, 100f))))
										{
											SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
											string text12 = ((spriteRenderer != null && spriteRenderer.sprite) ? spriteRenderer.sprite.name : renderer.GetType().Name);
											Bridge.Say(string.Format("near {0} <- {1} sprite {2} size {3:0.00}x{4:0.00} at {5} layer {6} {7} mat {8}", new object[]
											{
												renderer.name,
												renderer.transform.parent ? renderer.transform.parent.name : "-",
												text12,
												bounds.size.x,
												bounds.size.y,
												bounds.center,
												renderer.sortingLayerName,
												renderer.sortingOrder,
												renderer.sharedMaterial ? renderer.sharedMaterial.name : "-"
											}));
										}
									}
								}
								return;
							}
						}
						else if (num != 1438993425U)
						{
							if (num != 1462688907U)
							{
								if (num != 1469170932U)
								{
									goto IL_93D4;
								}
								if (!(text == "use"))
								{
									goto IL_93D4;
								}
								Item item6 = b.GetAllItemsThorough().FirstOrDefault<Item>((Item i) => i && i.id == a[1]);
								if (!item6)
								{
									Bridge.Say("she has no " + a[1]);
									return;
								}
								b.UseItem(item6);
								Bridge.Say(string.Format("used {0} (usable {1})", a[1], item6.Stats.usable));
								return;
							}
							else
							{
								if (!(text == "setlang"))
								{
									goto IL_93D4;
								}
								Bridge.Say("game language -> " + a[1]);
								Locale.ChangeLanguage(a[1]);
								return;
							}
						}
						else
						{
							if (!(text == "screen"))
							{
								goto IL_93D4;
							}
							Camera main = Camera.main;
							Vector3 vector3 = main.WorldToScreenPoint(b.limbs[1].transform.position);
							float num11 = main.WorldToScreenPoint(b.limbs[1].transform.position + Vector3.right).x - vector3.x;
							Bridge.Say(string.Format("screen chest {0:0} {1:0} unit {2:0.0} size {3}x{4} facing {5}", new object[]
							{
								vector3.x,
								(float)Screen.height - vector3.y,
								num11,
								Screen.width,
								Screen.height,
								b.isRight ? "right" : "left"
							}));
							return;
						}
					}
					else if (num <= 1536888239U)
					{
						if (num != 1480246043U)
						{
							if (num != 1497424882U)
							{
								if (num != 1536888239U)
								{
									goto IL_93D4;
								}
								if (!(text == "ammo"))
								{
									goto IL_93D4;
								}
								Item item7 = (b.HoldingItem(b.handSlot) ? b.GetItem(b.handSlot) : null);
								GunScript gunScript2;
								GunScript gunScript3;
								GunScript gunScript4;
								if (a[1] == "hold")
								{
									GameObject gameObject9 = Utils.Create(a[2], b.limbs[1].transform.position, 0f);
									Item item8;
									if (gameObject9 && gameObject9.TryGetComponent<Item>(ref item8))
									{
										b.PickUpItem(item8, b.handSlot, false);
									}
									Bridge.Say("holding " + (b.HoldingItem(b.handSlot) ? b.GetItem(b.handSlot).id : "nothing"));
								}
								else if (a[1] == "tip")
								{
									Item item9;
									if (a.Length > 2)
									{
										GameObject gameObject10 = Utils.Create(a[2], b.limbs[1].transform.position + Vector2.right * 2f, 0f);
										if (gameObject10 != null)
										{
											item9 = gameObject10.GetComponent<Item>();
											goto IL_7393;
										}
									}
									item9 = item7;
									IL_7393:
									Item item10 = item9;
									Bridge.Say("tip: " + PlayerCamera.ItemHoverDescription(item10).Item2.Replace("\n", " | "));
								}
								else if (a[1] == "drop" && item7)
								{
									b.DropItem(item7);
								}
								else if (a[1] == "fill" && item7 && item7.TryGetComponent<GunScript>(ref gunScript2))
								{
									if (gunScript2.feedType == null)
									{
										gunScript2.hasMag = true;
										gunScript2.roundsInMag = 10;
									}
									else
									{
										gunScript2.roundsInMag = Mathf.Max(0, gunScript2.magCapacity - 2);
									}
									gunScript2.roundInChamber = 0;
								}
								else if (a[1] == "unload" && item7 && item7.TryGetComponent<GunScript>(ref gunScript3))
								{
									gunScript3.UnloadMag();
								}
								else if (a[1] == "shoot" && item7 && item7.TryGetComponent<GunScript>(ref gunScript4))
								{
									gunScript4.safe = false;
									gunScript4.Fire(false);
								}
								TextMeshProUGUI textMeshProUGUI;
								if (!PlayerCamera.main.gunMenu)
								{
									textMeshProUGUI = null;
								}
								else
								{
									textMeshProUGUI = PlayerCamera.main.gunMenu.GetComponentsInChildren<TextMeshProUGUI>(true).FirstOrDefault<TextMeshProUGUI>((TextMeshProUGUI t) => t.name == "ArmoryAmmoCounter");
								}
								TextMeshProUGUI textMeshProUGUI2 = textMeshProUGUI;
								Bridge.Say("counter: " + (textMeshProUGUI2 ? (textMeshProUGUI2.enabled ? textMeshProUGUI2.text : "(hidden)") : "none"));
								return;
							}
							else
							{
								if (!(text == "pit"))
								{
									goto IL_93D4;
								}
								Bridge.lastPit = Pits.DebugPit(b, new PitKind?((a.Length > 1 && a[1] == "slime") ? PitKind.Slime : ((a.Length > 1 && a[1] == "flower") ? PitKind.Flower : PitKind.Maw)));
								Bridge.Say((Bridge.lastPit != null) ? string.Format("pit {0} top {1} mouth {2}", Bridge.lastPit.Kind, Bridge.lastPit.Top, Bridge.lastPit.Mouth) : "no pit");
								return;
							}
						}
						else
						{
							if (!(text == "latex"))
							{
								goto IL_93D4;
							}
							Bridge.Say(Latexes.DebugMake(b, (a.Length > 1) ? a[1] : "crawler"));
							return;
						}
					}
					else if (num != 1547907707U)
					{
						if (num != 1548049279U)
						{
							if (num != 1587342749U)
							{
								goto IL_93D4;
							}
							if (!(text == "biowarn"))
							{
								goto IL_93D4;
							}
							BioWarning.Show();
							Bridge.Say("biological warning shown");
							return;
						}
						else
						{
							if (!(text == "latexloose"))
							{
								goto IL_93D4;
							}
							Suit.TakeAntiserum(b, 50f);
							Bridge.Say(string.Format("loose for {0:0}s, tear-off {1:P0}", Plugin.S.latexLooseLeft, Suit.RemoveChance[5]));
							return;
						}
					}
					else
					{
						if (!(text == "ui"))
						{
							goto IL_93D4;
						}
						if (a.Length > 1 && a[1] == "wound")
						{
							PlayerCamera.main.ToggleWoundView(false);
						}
						Bridge.Say(string.Format("mouse {0} uiScale {1:0.00} showSide {2} woundView {3} ", new object[]
						{
							Input.mousePosition,
							PlayerCamera.uiScale,
							MoodleManager.showSideMoodles,
							PlayerCamera.main.woundView.activeSelf
						}) + "moodles " + string.Join(" ", MoodleManager.main.moodles.Cast<Transform>().Select<Transform, string>(delegate(Transform t)
						{
							Moodle component20 = t.GetComponent<Moodle>();
							if (!component20)
							{
								return string.Format("[marker@{0:0}]", ((RectTransform)t).anchoredPosition.x);
							}
							return string.Format("{0}{1}{2}@{3:0}", new object[]
							{
								component20.type,
								component20.isSide ? "*" : "",
								component20.doWarningFlash ? "!" : "",
								((RectTransform)t).anchoredPosition.x
							});
						})));
						return;
					}
				}
				else if (num <= 1668658321U)
				{
					if (num <= 1620630079U)
					{
						if (num != 1611551368U)
						{
							if (num != 1620630079U)
							{
								goto IL_93D4;
							}
							if (!(text == "worn"))
							{
								goto IL_93D4;
							}
							Bridge.Say(string.Format("suit stage {0} colour {1}; worn: ", Suit.Stage, Plugin.S.suitColor) + string.Join(", ", from i in Clothes.Worn(b)
								select string.Format("{0} {1:0.000}", i.id, i.condition)) + "; all: " + string.Join(", ", from i in b.GetAllWearables()
								select i.id + "/" + i.Stats.wearSlotId));
							return;
						}
						else
						{
							if (!(text == "paras"))
							{
								goto IL_93D4;
							}
							WorldGeneration w2 = WorldGeneration.world;
							Bridge.Say(string.Format("suit stage {0}; her feet {1:0.00}; parasites: ", Suit.Stage, Scenes.FeetY(b)) + string.Join("; ", Parasites.Alive.Where<ParasiteAI>((ParasiteAI x) => x).Select<ParasiteAI, string>(delegate(ParasiteAI x)
							{
								SpriteRenderer component21 = x.GetComponent<SpriteRenderer>();
								Vector2 vector11 = (component21 ? component21.bounds.center : x.transform.position);
								string text30 = "at {0} inRock {1} vel {2}";
								object obj6 = vector11;
								object obj7 = w2.GetBlock(w2.WorldToBlockPos(vector11)) > 0;
								Rigidbody2D component22 = x.GetComponent<Rigidbody2D>();
								return string.Format(text30, obj6, obj7, (component22 != null) ? new Vector2?(component22.velocity) : null);
							})));
							return;
						}
					}
					else if (num != 1622640710U)
					{
						if (num != 1625305005U)
						{
							if (num != 1668658321U)
							{
								goto IL_93D4;
							}
							if (!(text == "wormboom"))
							{
								goto IL_93D4;
							}
							WormBeast wormBeast3 = (from x in Worm.Beasts
								where x
								orderby Vector2.Distance(x.HeadPos, b.transform.position)
								select x).FirstOrDefault<WormBeast>();
							if (!wormBeast3)
							{
								Bridge.Say("no worm");
								return;
							}
							WorldGeneration.CreateExplosion(new ExplosionParams
							{
								position = wormBeast3.Maw,
								range = 6f,
								muscleDamage = new RangeF(0f, 0f),
								skinDamage = new RangeF(0f, 0f),
								structuralDamage = 0f,
								boneBreakChance = 0f,
								dislocationChance = 0f,
								disfigureChance = 0f,
								bleedChance = 0f,
								shrapnelChance = 0f,
								velocity = 2f
							});
							Bridge.Say(string.Format("a blast in the worm's mouth: leaving {0}", wormBeast3.leaving));
							return;
						}
						else
						{
							if (!(text == "hide"))
							{
								goto IL_93D4;
							}
							foreach (Renderer renderer2 in Object.FindObjectsOfType<Renderer>())
							{
								if (renderer2.name == a[1] && Vector2.Distance(renderer2.transform.position, b.transform.position) < 4f)
								{
									renderer2.enabled = !renderer2.enabled;
									Bridge.Say(renderer2.name + " " + (renderer2.enabled ? "shown" : "hidden"));
								}
							}
							return;
						}
					}
					else
					{
						if (!(text == "soundlog"))
						{
							goto IL_93D4;
						}
						SoundLog.Until = Time.time + ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 20f);
						Bridge.Say("logging sounds");
						return;
					}
				}
				else if (num <= 1778087159U)
				{
					if (num != 1723874488U)
					{
						if (num != 1756456866U)
						{
							if (num != 1778087159U)
							{
								goto IL_93D4;
							}
							if (!(text == "bust"))
							{
								goto IL_93D4;
							}
							if (a[1] == "random")
							{
								Bust.Setting.Value = "random";
								Bridge.Say("bust size: random again");
								return;
							}
							Plugin.S.bust = int.Parse(a[1]);
							Bust.Setting.Value = Bust.Name(Plugin.S.bust);
							Bridge.Say("bust " + Bust.Name(Plugin.S.bust));
							return;
						}
						else
						{
							if (!(text == "symtalk"))
							{
								goto IL_93D4;
							}
							SymbioteVoice.DebugTalk(b);
							Bridge.Say("the symbiote talks");
							return;
						}
					}
					else
					{
						if (!(text == "wound"))
						{
							goto IL_93D4;
						}
						float num12 = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 4f);
						Limb limb5 = b.LimbByName((a.Length > 2) ? a[2] : "ThighF") ?? b.limbs[1];
						limb5.bleedAmount += num12;
						limb5.pain = Mathf.Min(100f, limb5.pain + 40f);
						limb5.skinHealth -= 20f;
						Bridge.Say(string.Format("wounded: {0} bleeds {1:0.0}, total bleed {2:0.00}, blood {3:0}, pain {4:0}, wounded {5}", new object[]
						{
							limb5.name,
							limb5.bleedAmount,
							b.totalBleedSpeed,
							b.bloodVolume,
							b.averagePain,
							Ultimatum.Wounded(b)
						}));
						return;
					}
				}
				else if (num != 1788017107U)
				{
					if (num != 1788363406U)
					{
						if (num != 1805184399U)
						{
							goto IL_93D4;
						}
						if (!(text == "health"))
						{
							goto IL_93D4;
						}
						PlayerCamera.main.ToggleWoundView(false);
						Bridge.Say("health panel " + (PlayerCamera.main.woundView.activeSelf ? "open" : "closed"));
						return;
					}
					else
					{
						if (!(text == "ddwear"))
						{
							goto IL_93D4;
						}
						OutfitDef outfitDef = Devices.ById(a[1]);
						Bridge.Say((outfitDef == null) ? ("no device " + a[1]) : string.Format("put on {0}: {1}", a[1], Devices.PutOn(b, outfitDef, a.Length > 2 && a[2] == "lock")));
						return;
					}
				}
				else if (!(text == "earshot"))
				{
					goto IL_93D4;
				}
				float hearingLoss = b.hearingLoss;
				TurretScript.Shoot(new FireInfo
				{
					pos = b.transform.position + Vector2.right * 3f,
					dir = Vector2.up,
					doTinnitus = true,
					playerDamageMultiplier = 1f,
					animalDamage = 0f,
					structureDamage = 0f
				});
				Bridge.Say(string.Format("earshot: hearing loss {0:0.0} -> {1:0.0}, ear protection {2}", hearingLoss, b.hearingLoss, EarPro.On));
				return;
			}
			if (num > 3341224086U)
			{
				if (num <= 3722811363U)
				{
					if (num <= 3433691610U)
					{
						if (num <= 3377879410U)
						{
							if (num <= 3348403114U)
							{
								if (num != 3344365753U)
								{
									if (num != 3348403114U)
									{
										goto IL_93D4;
									}
									if (!(text == "boom"))
									{
										goto IL_93D4;
									}
									float num13 = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 5f);
									WorldGeneration.CreateExplosion(new ExplosionParams
									{
										position = b.limbs[1].transform.position + new Vector2(num13, 0f),
										range = 8f,
										muscleDamage = new RangeF(0f, 0f),
										skinDamage = new RangeF(0f, 0f),
										structuralDamage = 0f,
										boneBreakChance = 0f,
										dislocationChance = 0f,
										disfigureChance = 0f,
										bleedChance = 0f,
										shrapnelChance = 0f,
										velocity = 5f
									});
									Bridge.Say(string.Format("a blast {0:0} away; held {1}", num13, Scenes.Held));
									return;
								}
								else
								{
									if (!(text == "approach"))
									{
										goto IL_93D4;
									}
									if (Bridge.lastPit == null)
									{
										Bridge.Say("no pit");
										return;
									}
									float num14 = Scenes.FeetY(b);
									Bridge.Move(b, Bridge.lastPit.Mouth + new Vector2(-((float)Bridge.lastPit.Half + 1.3f), 0.7f) - new Vector2(b.transform.position.x, num14));
									Bridge.walk = 1.2f;
									Bridge.Say("beside the pit, walking onto it");
									return;
								}
							}
							else if (num != 3365988096U)
							{
								if (num != 3372870890U)
								{
									if (num != 3377879410U)
									{
										goto IL_93D4;
									}
									if (!(text == "podmap"))
									{
										goto IL_93D4;
									}
									WorldGeneration world7 = WorldGeneration.world;
									GameObject gameObject11 = Resources.Load<GameObject>((a.Length > 1) ? a[1] : "Lifepod");
									Tilemap component9 = gameObject11.transform.GetChild(0).GetComponent<Tilemap>();
									BoundsInt cellBounds = component9.cellBounds;
									Bridge.Say(string.Format("podmap {0} bounds x {1}..{2} y {3}..{4}", new object[] { gameObject11.name, cellBounds.xMin, cellBounds.xMax, cellBounds.yMin, cellBounds.yMax }));
									for (int num15 = cellBounds.yMax - 1; num15 >= cellBounds.yMin; num15--)
									{
										StringBuilder stringBuilder4 = new StringBuilder();
										for (int num16 = cellBounds.xMin; num16 < cellBounds.xMax; num16++)
										{
											TileBase tile = component9.GetTile(new Vector3Int(num16, num15, 0));
											int num17 = (tile ? Array.IndexOf<TileBase>(world7.tiles, tile) : (-2));
											stringBuilder4.Append((tile == null) ? " ." : ((num17 < 0) ? " #" : num17.ToString().PadLeft(2)));
											stringBuilder4.Append(' ');
										}
										Bridge.Say(string.Format("podrow {0,3}: {1}", num15, stringBuilder4));
									}
									using (IEnumerator enumerator8 = gameObject11.transform.GetEnumerator())
									{
										while (enumerator8.MoveNext())
										{
											object obj4 = enumerator8.Current;
											Transform transform = (Transform)obj4;
											Bridge.Say(string.Format("podpart {0} at {1} {2}", transform.name, transform.localPosition, transform.GetComponent<Item>() ? ("item " + transform.GetComponent<Item>().id) : (transform.GetComponent<BuildingEntity>() ? ("building " + transform.GetComponent<BuildingEntity>().id) : "")));
										}
										return;
									}
									goto IL_870E;
								}
								else
								{
									if (!(text == "entities"))
									{
										goto IL_93D4;
									}
									Dictionary<string, string> dictionary = new Dictionary<string, string>();
									foreach (BuildingEntity buildingEntity in Object.FindObjectsOfType<BuildingEntity>())
									{
										string text13 = buildingEntity.id ?? buildingEntity.name;
										if (!dictionary.ContainsKey(text13))
										{
											Dictionary<string, string> dictionary2 = dictionary;
											string text14 = text13;
											string text15 = "guaranteed {0} cats [{1}] drops [{2}] always {3} animal {4}";
											object[] array6 = new object[5];
											array6[0] = buildingEntity.guaranteedDropAmount;
											array6[1] = string.Join(",", buildingEntity.itemCategoriesToAdd ?? new string[0]);
											array6[2] = string.Join(",", (buildingEntity.itemsDropOnDestroy ?? new ItemDrop[0]).Select<ItemDrop, string>((ItemDrop d) => d.id));
											int num18 = 3;
											ItemDrop[] alwaysDrop = buildingEntity.alwaysDrop;
											array6[num18] = ((alwaysDrop != null) ? alwaysDrop.Length : 0);
											array6[4] = buildingEntity.animal;
											dictionary2[text14] = string.Format(text15, array6);
										}
									}
									using (Dictionary<string, string>.Enumerator enumerator9 = dictionary.GetEnumerator())
									{
										while (enumerator9.MoveNext())
										{
											KeyValuePair<string, string> keyValuePair = enumerator9.Current;
											Bridge.Say("entity " + keyValuePair.Key + ": " + keyValuePair.Value);
										}
										return;
									}
								}
							}
							else
							{
								if (!(text == "relief"))
								{
									goto IL_93D4;
								}
								Traverse.Create(typeof(Relief)).Method("TryStart", new object[] { b, false }).GetValue();
								Bridge.Say(string.Format("relief active {0}", Relief.Active));
								return;
							}
						}
						else if (num <= 3398140368U)
						{
							if (num != 3394807423U)
							{
								if (num != 3396934729U)
								{
									if (num != 3398140368U)
									{
										goto IL_93D4;
									}
									if (!(text == "latexturn"))
									{
										goto IL_93D4;
									}
									Bridge.Say(Latexes.DebugTransform(b));
									return;
								}
								else
								{
									if (!(text == "faint"))
									{
										goto IL_93D4;
									}
									Bridge.faint = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 10f);
									Bridge.Say(string.Format("faint for {0:0}s", Bridge.faint));
									return;
								}
							}
							else
							{
								if (!(text == "flowerburn"))
								{
									goto IL_93D4;
								}
								FlowerPlant flowerPlant2 = (from x in Flowers.Plants
									where x
									orderby Vector2.Distance(x.transform.position, b.transform.position)
									select x).FirstOrDefault<FlowerPlant>();
								FleshBurn fleshBurn2 = (flowerPlant2 ? flowerPlant2.GetComponent<FleshBurn>() : null);
								if (fleshBurn2)
								{
									Burning.Damage(fleshBurn2, (a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 80f, "the bridge", true);
								}
								Bridge.Say(fleshBurn2 ? string.Format("flower burnt: {0:0} left", fleshBurn2.hp) : "no flower");
								return;
							}
						}
						else if (num != 3399231628U)
						{
							if (num != 3430978581U)
							{
								if (num != 3433691610U)
								{
									goto IL_93D4;
								}
								if (!(text == "scavgrab"))
								{
									goto IL_93D4;
								}
								Scavenger scavenger5 = (from x in Scavengers.All
									where x && x.mode != Scavenger.Mode.Dead
									orderby Vector2.Distance(x.pos, b.transform.position)
									select x).FirstOrDefault<Scavenger>();
								if (scavenger5)
								{
									Capture.SurrenderTo(b, scavenger5);
									Capture.Grab(b, scavenger5);
								}
								Bridge.Say(scavenger5 ? "grabbed by the nearest scavenger" : "no scavenger");
								return;
							}
							else
							{
								if (!(text == "shootat"))
								{
									goto IL_93D4;
								}
								goto IL_2262;
							}
						}
						else
						{
							if (!(text == "bodypng"))
							{
								goto IL_93D4;
							}
							GameObject gameObject12 = new GameObject("LewdBodyCam");
							Camera camera = gameObject12.AddComponent<Camera>();
							Camera main2 = Camera.main;
							camera.CopyFrom(main2);
							camera.orthographic = true;
							float num19;
							camera.orthographicSize = ((a.Length > 2 && float.TryParse(a[2], NumberStyles.Float, CultureInfo.InvariantCulture, out num19)) ? num19 : 4.2f);
							SpriteRenderer component10 = b.limbs[1].GetComponent<SpriteRenderer>();
							Vector2 vector4 = ((a.Length > 2 && a[2] != "scav" && (a.Length <= 3 || !(a[3] == "wall")) && component10) ? (component10.bounds.center + Vector2.up * 0.6f * camera.orthographicSize / 4.2f) : b.limbs[1].transform.position);
							if (a.Length > 3 && a[3] == "wall")
							{
								LewdWall lewdWall = Walls.All.LastOrDefault<LewdWall>((LewdWall x) => x);
								if (lewdWall)
								{
									vector4 = lewdWall.Face + Vector2.right * (float)lewdWall.Facing * -1f + Vector2.up * 0.6f * camera.orthographicSize / 4.2f;
								}
							}
							if (a.Length > 2 && a[2] == "scav")
							{
								Scavenger scavenger6;
								if (!Bridge.lastScav)
								{
									scavenger6 = (from x in Scavengers.All
										where x
										orderby Vector2.Distance(x.pos, b.transform.position)
										select x).FirstOrDefault<Scavenger>();
								}
								else
								{
									scavenger6 = Bridge.lastScav;
								}
								Scavenger scavenger7 = scavenger6;
								if (scavenger7)
								{
									vector4 = scavenger7.pos + Vector2.up * 2.6f;
								}
							}
							camera.transform.position = new Vector3(vector4.x, vector4.y - 0.6f * camera.orthographicSize / 4.2f, main2.transform.position.z);
							camera.clearFlags = 2;
							camera.backgroundColor = new Color(0.35f, 0.36f, 0.4f, 1f);
							RenderTexture renderTexture = new RenderTexture(480, 480, 24);
							camera.targetTexture = renderTexture;
							camera.Render();
							RenderTexture.active = renderTexture;
							Texture2D texture2D = new Texture2D(480, 480, 3, false);
							texture2D.ReadPixels(new Rect(0f, 0f, 480f, 480f), 0, 0);
							texture2D.Apply();
							RenderTexture.active = null;
							camera.targetTexture = null;
							string text16 = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "bodypng_" + ((a.Length > 1) ? a[1] : "now") + ".png");
							File.WriteAllBytes(text16, ImageConversion.EncodeToPNG(texture2D));
							Object.Destroy(gameObject12);
							Object.Destroy(renderTexture);
							Object.Destroy(texture2D);
							Bridge.Say("body drawn to " + text16);
							return;
						}
					}
					else if (num <= 3566434650U)
					{
						if (num <= 3527465418U)
						{
							if (num != 3492168515U)
							{
								if (num != 3527465418U)
								{
									goto IL_93D4;
								}
								if (!(text == "stepon"))
								{
									goto IL_93D4;
								}
								if (Bridge.lastPit == null)
								{
									Bridge.Say("no pit");
									return;
								}
								Vector2 vector5;
								vector5..ctor(b.transform.position.x, Scenes.FeetY(b));
								Bridge.Move(b, Bridge.lastPit.Mouth + Vector2.up * 0.9f - vector5);
								Bridge.Say("on the pit");
								return;
							}
							else
							{
								if (!(text == "wormhit"))
								{
									goto IL_93D4;
								}
								goto IL_5ACA;
							}
						}
						else if (num != 3531726399U)
						{
							if (num != 3558494521U)
							{
								if (num != 3566434650U)
								{
									goto IL_93D4;
								}
								if (!(text == "debris"))
								{
									goto IL_93D4;
								}
								Plugin.WebDebrisSeconds.Value = float.Parse(a[1], CultureInfo.InvariantCulture);
								Bridge.Say(string.Format("web debris fades after {0:0} s", Plugin.WebDebrisSeconds.Value));
								return;
							}
							else if (!(text == "wormspawn"))
							{
								goto IL_93D4;
							}
						}
						else
						{
							if (!(text == "weappng"))
							{
								goto IL_93D4;
							}
							string[] array7 = new string[] { "machete", "crudecleaver", "flimsyknife", "wrench", "pickaxe", "shovel", "scraptube", "sledgehammer" };
							List<ValueTuple<string, Color[], int, int, int, int>> list6 = new List<ValueTuple<string, Color[], int, int, int, int>>();
							foreach (string text17 in array7)
							{
								GameObject gameObject13 = Resources.Load(text17) as GameObject;
								SpriteRenderer spriteRenderer2 = (gameObject13 ? gameObject13.GetComponent<SpriteRenderer>() : null);
								if (spriteRenderer2 && spriteRenderer2.sprite)
								{
									Sprite sprite = spriteRenderer2.sprite;
									Texture2D texture2D2 = Bust.Readable(sprite.texture);
									Rect textureRect = sprite.textureRect;
									int num20 = (int)textureRect.width;
									int num21 = (int)textureRect.height;
									list6.Add(new ValueTuple<string, Color[], int, int, int, int>(text17, texture2D2.GetPixels((int)textureRect.x, (int)textureRect.y, num20, num21), num20, num21, (int)sprite.pivot.x, (int)sprite.pivot.y));
									Bridge.Say(string.Format("weapon {0}: {1}x{2} px, pivot {3}, ppu {4}, prefab rot {5:0}", new object[]
									{
										text17,
										num20,
										num21,
										sprite.pivot,
										sprite.pixelsPerUnit,
										gameObject13.transform.eulerAngles.z
									}));
								}
							}
							int num22;
							if (list6.Count != 0)
							{
								num22 = list6.Max<ValueTuple<string, Color[], int, int, int, int>>(([TupleElementNames(new string[] { "id", "px", "w", "h", "pxp", "pyp" })] ValueTuple<string, Color[], int, int, int, int> t) => Mathf.Max(t.Item3, t.Item4)) + 4;
							}
							else
							{
								num22 = 8;
							}
							int num23 = num22;
							int num24 = 4;
							Texture2D texture2D3 = new Texture2D(num23 * list6.Count * num24, num23 * num24, 4, false);
							Color[] array9 = new Color[texture2D3.width * texture2D3.height];
							for (int num25 = 0; num25 < array9.Length; num25++)
							{
								array9[num25] = new Color(0.3f, 0.3f, 0.35f, 1f);
							}
							texture2D3.SetPixels(array9);
							for (int num26 = 0; num26 < list6.Count; num26++)
							{
								ValueTuple<string, Color[], int, int, int, int> valueTuple = list6[num26];
								for (int num27 = 0; num27 < valueTuple.Item4; num27++)
								{
									for (int num28 = 0; num28 < valueTuple.Item3; num28++)
									{
										Color color = valueTuple.Item2[num27 * valueTuple.Item3 + num28];
										if (num28 == valueTuple.Item5 && num27 == valueTuple.Item6)
										{
											color = Color.red;
										}
										if (color.a >= 0.1f)
										{
											for (int num29 = 0; num29 < num24; num29++)
											{
												for (int num30 = 0; num30 < num24; num30++)
												{
													texture2D3.SetPixel((num26 * num23 + 2 + num28) * num24 + num30, (2 + num27) * num24 + num29, color);
												}
											}
										}
									}
								}
							}
							texture2D3.Apply();
							string text18 = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "weappng.png");
							File.WriteAllBytes(text18, ImageConversion.EncodeToPNG(texture2D3));
							Bridge.Say("weapons drawn to " + text18);
							return;
						}
					}
					else if (num <= 3630339793U)
					{
						if (num != 3601144614U)
						{
							if (num != 3614680169U)
							{
								if (num != 3630339793U)
								{
									goto IL_93D4;
								}
								if (!(text == "die"))
								{
									goto IL_93D4;
								}
								b.brainHealth = 0f;
								Bridge.Say("she dies");
								return;
							}
							else
							{
								if (!(text == "chestpng"))
								{
									goto IL_93D4;
								}
								goto IL_635E;
							}
						}
						else
						{
							if (!(text == "slitswallow"))
							{
								goto IL_93D4;
							}
							Bridge.Say(Slits.DebugGrab(b, true));
							return;
						}
					}
					else if (num != 3685382517U)
					{
						if (num != 3697379050U)
						{
							if (num != 3722811363U)
							{
								goto IL_93D4;
							}
							if (!(text == "npcflags"))
							{
								goto IL_93D4;
							}
							IEnumerable<Scavenger> all = Scavengers.All;
							Func<Scavenger, bool> <>9__42;
							Func<Scavenger, bool> func2;
							if ((func2 = <>9__42) == null)
							{
								func2 = (<>9__42 = (Scavenger x) => x && Vector2.Distance(x.pos, b.transform.position) < 30f);
							}
							using (IEnumerator<Scavenger> enumerator5 = all.Where<Scavenger>(func2).GetEnumerator())
							{
								while (enumerator5.MoveNext())
								{
									Scavenger scavenger8 = enumerator5.Current;
									Traverse traverse3 = Traverse.Create(scavenger8);
									Bridge.Say(string.Format("npc {0} {1} grabber {2} canGrab {3} busy {4} stagger {5} stunned {6} sated {7} comingFor {8} spent {9} holding {10}", new object[]
									{
										scavenger8.slimeHost ? "host" : "scav",
										scavenger8.mode,
										scavenger8.grabber,
										Capture.CanGrab(b),
										Scenes.Busy,
										traverse3.Field("stagger").GetValue(),
										traverse3.Field("stunned").GetValue(),
										traverse3.Field("sated").GetValue(),
										traverse3.Field("comingFor").GetValue(),
										scavenger8.spent,
										scavenger8.Holding
									}));
								}
								return;
							}
							goto IL_474D;
						}
						else
						{
							if (!(text == "pitinfo"))
							{
								goto IL_93D4;
							}
							if (Bridge.lastPit == null)
							{
								Bridge.Say("no pit");
								return;
							}
							WorldGeneration w = WorldGeneration.world;
							string text19 = string.Join(" ", from x in Enumerable.Range(-Bridge.lastPit.Half, Bridge.lastPit.Half * 2 + 1)
								select w.GetBlock(new Vector2Int(Bridge.lastPit.Top.x + x, Bridge.lastPit.Top.y)).ToString());
							int num31 = Bridge.lastPit.RimCells.Count<Vector2Int>((Vector2Int c) => w.GetBlock(c) == 0);
							string text20 = string.Format("pit {0} open {1} sealed {2} closed {3:0.00} dead {4} sated {5:0} cover row [{6}] rim {7} gone {8} ", new object[]
							{
								Bridge.lastPit.Kind,
								Bridge.lastPit.Open,
								Bridge.lastPit.Sealed,
								Bridge.lastPit.Closed,
								Bridge.lastPit.Dead,
								Bridge.lastPit.Sated,
								text19,
								Bridge.lastPit.RimCells.Count,
								num31
							});
							string text21 = "inMaw {0} her {1} broken bones {2} slime {3} under {4}";
							object[] array10 = new object[5];
							array10[0] = Pits.InMaw;
							array10[1] = b.transform.position;
							array10[2] = b.limbs.Count<Limb>((Limb l) => l && l.broken);
							array10[3] = SlimeBog.Stuck;
							array10[4] = SlimeBog.Under;
							Bridge.Say(text20 + string.Format(text21, array10));
							return;
						}
					}
					else
					{
						if (!(text == "body"))
						{
							goto IL_93D4;
						}
						if (a.Length > 2)
						{
							Traverse traverse4 = Traverse.Create(b).Field(a[1]);
							if (traverse4.FieldExists())
							{
								traverse4.SetValue(float.Parse(a[2], CultureInfo.InvariantCulture));
							}
						}
						Bridge.Say(string.Format("hunger {0:0} thirst {1:0} dirt {2:0} energy {3:0} sick {4:0} pain {5:0} wet {6:0.0} slip {7:0.00} temp {8:0.0} sleeping {9} ", new object[] { b.hunger, b.thirst, b.dirtyness, b.energy, b.sicknessAmount, b.averagePain, b.wetness, b.liquidSlipTime, b.temperature, b.sleeping }) + string.Format("discomfort {0:0.00} arousal {1:0.0} lust {2:0.0} stamina {3:0.0} breathing {4} oxygen {5:0} ", new object[]
						{
							Plugin.Discomfort(b),
							Plugin.S.arousal,
							Plugin.S.lust,
							b.stamina,
							b.breathing,
							b.bloodOxygen
						}) + "pant " + string.Join("/", from x in b.GetComponents<AudioSource>()
							where x.clip && x.loop
							select x.clip.name + " " + x.volume.ToString("0.00") + (x.isPlaying ? "" : " stopped")));
						return;
					}
					float num32 = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 35f);
					WormBeast wormBeast4 = Worm.Spawn(b.transform.position + new Vector2(num32, 0f));
					Bridge.Say(wormBeast4 ? ("worm " + wormBeast4.State(b)) : "no worm");
					return;
				}
				if (num <= 4012524298U)
				{
					if (num <= 3911001148U)
					{
						if (num <= 3837534366U)
						{
							if (num != 3784070831U)
							{
								if (num != 3837534366U)
								{
									goto IL_93D4;
								}
								if (!(text == "wear"))
								{
									goto IL_93D4;
								}
								OutfitDef outfitDef2 = ((a.Length > 1) ? Outfits.ById(a[1]) : Outfits.Spacesuit);
								Plugin.Instance.DebugWear(b, outfitDef2 ?? Outfits.Spacesuit);
								Bridge.Say((outfitDef2 ?? Outfits.Spacesuit).Id + " on");
								return;
							}
							else if (!(text == "make"))
							{
								goto IL_93D4;
							}
						}
						else if (num != 3847676929U)
						{
							if (num != 3861910447U)
							{
								if (num != 3911001148U)
								{
									goto IL_93D4;
								}
								if (!(text == "fillmags"))
								{
									goto IL_93D4;
								}
								int num33 = 0;
								foreach (AmmoScript ammoScript in from i in b.GetAllItemsThorough()
									where i
									select i.GetComponent<AmmoScript>() into x
									where x && x.itemType == 1 && !Armory.IsBox(x.GetComponent<Item>())
									select x)
								{
									ammoScript.rounds = ammoScript.maxRounds;
									num33++;
								}
								Bridge.Say(string.Format("{0} magazines filled", num33));
								return;
							}
							else
							{
								if (!(text == "puppet"))
								{
									goto IL_93D4;
								}
								if (a.Length > 1 && a[1] == "free")
								{
									Puppet.Release(b, false);
								}
								Bridge.Say(string.Format("puppet active {0} stage {1} escapes {2} retry {3} cooldown {4:0} tamed {5} armor {6}", new object[]
								{
									Puppet.Active,
									Suit.Stage,
									Plugin.S.takeoverEscapes,
									Plugin.S.takeoverRetry,
									Plugin.S.takeoverCooldown,
									Plugin.S.suitTamed,
									Suit.Armor[Mathf.Min(Suit.Stage, 5)]
								}));
								return;
							}
						}
						else
						{
							if (!(text == "ddbag"))
							{
								goto IL_93D4;
							}
							Container container2 = b.GetAllItemsThorough().Select<Item, Container>(delegate(Item i)
							{
								if (!i)
								{
									return null;
								}
								return i.GetComponent<Container>();
							}).FirstOrDefault<Container>((Container c) => c);
							if (!container2)
							{
								Bridge.Say("no bag on her");
								return;
							}
							List<Item> list7 = Devices.LockedOn(b);
							foreach (Item item11 in list7)
							{
								container2.LoadItem(item11);
							}
							Bridge.Say(string.Format("ddbag: tried {0}; still locked on her: ", list7.Count) + string.Join(", ", from i in Devices.LockedOn(b)
								select i.id));
							return;
						}
					}
					else if (num <= 3981844376U)
					{
						if (num != 3963997134U)
						{
							if (num != 3966689298U)
							{
								if (num != 3981844376U)
								{
									goto IL_93D4;
								}
								if (!(text == "cum"))
								{
									goto IL_93D4;
								}
								goto IL_5B29;
							}
							else
							{
								if (!(text == "mode"))
								{
									goto IL_93D4;
								}
								if (Bridge.lastPit != null)
								{
									Bridge.lastPit.Ravish = a.Length > 1 && a[1] == "ravish";
								}
								Bridge.Say("pit mode: " + ((Bridge.lastPit != null && Bridge.lastPit.Ravish) ? "ravish" : "digest"));
								return;
							}
						}
						else
						{
							if (!(text == "moodles"))
							{
								goto IL_93D4;
							}
							MoodleManager main3 = MoodleManager.main;
							if (!main3 || !main3.moodles)
							{
								Bridge.Say("no moodle row");
								return;
							}
							if (a.Length > 1)
							{
								MoodleManager.alwaysShowHidden = a[1] == "show";
							}
							List<string> list8 = new List<string>();
							foreach (object obj5 in main3.moodles)
							{
								Transform transform2 = (Transform)obj5;
								Moodle component11 = transform2.GetComponent<Moodle>();
								RectTransform component12 = transform2.GetComponent<RectTransform>();
								list8.Add(string.Format("{0}{1}{2}@{3:0}", new object[]
								{
									component11 ? component11.type : transform2.name,
									(component11 && component11.isSide) ? "(side)" : "",
									transform2.gameObject.activeSelf ? "" : "(off)",
									component12.anchoredPosition.x
								}));
							}
							Bridge.Say(string.Format("moodles ({0}, show hidden {1}): ", list8.Count, MoodleManager.alwaysShowHidden) + string.Join(", ", list8));
							return;
						}
					}
					else if (num != 3984383372U)
					{
						if (num != 3988098555U)
						{
							if (num != 4012524298U)
							{
								goto IL_93D4;
							}
							if (!(text == "latexsuit"))
							{
								goto IL_93D4;
							}
							Suit.AttachLatex(b);
							Bridge.Say(string.Format("suit stage {0} colour {1}", Suit.Stage, Plugin.S.suitColor));
							return;
						}
						else
						{
							if (!(text == "ammotest"))
							{
								goto IL_93D4;
							}
							int num34 = ((a.Length > 1) ? int.Parse(a[1]) : 200);
							Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
							for (int num35 = 0; num35 < num34; num35++)
							{
								ArmDef armDef = Armory.Pick(false);
								int num36;
								dictionary3[armDef.Id] = (dictionary3.TryGetValue(armDef.Id, out num36) ? (num36 + 1) : 1);
							}
							IEnumerable<string> enumerable = from g in b.GetComponentsInChildren<GunScript>(true)
								select g.name + ":" + g.ammoType.ToString();
							Bridge.Say(string.Format("ammo picks ({0}) with guns [{1}]: ", num34, string.Join(", ", enumerable)) + string.Join(", ", from kv in dictionary3
								orderby kv.Value descending
								select kv.Key + " " + kv.Value.ToString()));
							return;
						}
					}
					else
					{
						if (!(text == "reload"))
						{
							goto IL_93D4;
						}
						Bridge.Say(GunControls.Start(PlayerCamera.main));
						return;
					}
				}
				else if (num <= 4110721160U)
				{
					if (num <= 4035825995U)
					{
						if (num != 4013831322U)
						{
							if (num != 4035825995U)
							{
								goto IL_93D4;
							}
							if (!(text == "con"))
							{
								goto IL_93D4;
							}
							string[] array11 = a.Skip<string>(1).ToArray<string>();
							if (ConsoleScript.instance)
							{
								Traverse.Create(ConsoleScript.instance).Method("TryExecuteCommand", new Type[]
								{
									typeof(string[]),
									typeof(bool)
								}, new object[] { array11, true }).GetValue();
							}
							Bridge.Say(string.Format("console: {0} -> corruption {1:0} arousal {2:0} lust {3:0} submission {4:0} exhaustion {5:0}", new object[]
							{
								string.Join(" ", array11),
								Plugin.S.corruption,
								Plugin.S.arousal,
								Plugin.S.lust,
								Plugin.S.submission,
								Plugin.S.exhaustion
							}));
							return;
						}
						else
						{
							if (!(text == "prof"))
							{
								goto IL_93D4;
							}
							Prof.Start((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 30f);
							Bridge.Say("profiling on");
							return;
						}
					}
					else if (num != 4076964457U)
					{
						if (num != 4084072156U)
						{
							if (num != 4110721160U)
							{
								goto IL_93D4;
							}
							if (!(text == "capstruggle"))
							{
								goto IL_93D4;
							}
							if (Capture.Active)
							{
								Traverse.Create(typeof(Capture)).Method("Struggle", new object[] { b }).GetValue();
							}
							Bridge.Say(string.Format("captured {0}", Capture.Active));
							return;
						}
						else
						{
							if (!(text == "lightoff"))
							{
								goto IL_93D4;
							}
							foreach (Light2D light2D2 in Object.FindObjectsOfType<Light2D>())
							{
								if (light2D2.name == a[1] && Vector2.Distance(light2D2.transform.position, b.transform.position) < 4f)
								{
									light2D2.enabled = !light2D2.enabled;
									Bridge.Say(light2D2.name + " " + (light2D2.enabled ? "on" : "off"));
								}
							}
							return;
						}
					}
					else
					{
						if (!(text == "semenpool"))
						{
							goto IL_93D4;
						}
						Bridge.Say(string.Format("semen poured round her: {0} cells (byte {1})", SemenFluid.DebugPool(b), SemenFluid.Id));
						return;
					}
				}
				else if (num <= 4218709923U)
				{
					if (num != 4186016599U)
					{
						if (num != 4210035195U)
						{
							if (num != 4218709923U)
							{
								goto IL_93D4;
							}
							if (!(text == "dropitem"))
							{
								goto IL_93D4;
							}
							float num37 = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 8f);
							GameObject gameObject14 = (Resources.Load("rock") ?? Resources.Load("rag")) as GameObject;
							GameObject gameObject15 = Object.Instantiate<GameObject>(gameObject14, b.transform.position + new Vector2(num37, 3f), Quaternion.identity);
							Rigidbody2D rigidbody2D;
							if (gameObject15.TryGetComponent<Rigidbody2D>(ref rigidbody2D))
							{
								rigidbody2D.velocity = new Vector2(0f, -12f);
							}
							Bridge.Say(string.Format("{0} dropped at {1}", gameObject14.name, gameObject15.transform.position));
							return;
						}
						else
						{
							if (!(text == "wallhere"))
							{
								goto IL_93D4;
							}
							WorldGeneration world8 = WorldGeneration.world;
							float num38 = Scenes.FeetY(b);
							Vector2Int vector2Int7 = world8.WorldToBlockPos(new Vector2(b.transform.position.x, num38 - 0.5f));
							ushort num39 = world8.GetBlock(vector2Int7);
							if (num39 == 0)
							{
								num39 = 1;
							}
							int num40 = (b.isRight ? 1 : (-1));
							for (int num41 = 0; num41 <= 9; num41++)
							{
								for (int num42 = 1; num42 <= 2; num42++)
								{
									world8.SetBlock(new Vector2Int(vector2Int7.x + num40 * num42, vector2Int7.y + num41), (num41 == 0) ? num39 : 0);
								}
								for (int num43 = 3; num43 <= 5; num43++)
								{
									world8.SetBlock(new Vector2Int(vector2Int7.x + num40 * num43, vector2Int7.y + num41), num39);
								}
							}
							Walls.DebugWall(b, WallKind.Flesh);
							return;
						}
					}
					else
					{
						if (!(text == "wormstate"))
						{
							goto IL_93D4;
						}
						using (List<WormBeast>.Enumerator enumerator6 = Worm.Beasts.GetEnumerator())
						{
							while (enumerator6.MoveNext())
							{
								WormBeast wormBeast5 = enumerator6.Current;
								if (wormBeast5)
								{
									Bridge.Say("worm " + wormBeast5.State(b));
								}
							}
							return;
						}
						goto IL_4427;
					}
				}
				else if (num != 4243373641U)
				{
					if (num != 4246411473U)
					{
						if (num != 4277529766U)
						{
							goto IL_93D4;
						}
						if (!(text == "hsuit"))
						{
							goto IL_93D4;
						}
						if (Suit.Stage == 0)
						{
							Suit.Attach(b, 7);
						}
						while (Suit.Stage < ((a.Length > 1) ? int.Parse(a[1]) : 1) && Suit.Stage < 4)
						{
							Suit.ForceNextStage(b);
						}
						Bridge.Say(string.Format("host slime suit stage {0}", Suit.Stage));
						return;
					}
					else
					{
						if (!(text == "strip"))
						{
							goto IL_93D4;
						}
						Suit.Strip(b);
						Bridge.Say(string.Format("suit stripped, stage {0}", Suit.Stage));
						return;
					}
				}
				else
				{
					if (!(text == "crates"))
					{
						goto IL_93D4;
					}
					Rigidbody2D[] array12 = (from r in Object.FindObjectsOfType<Rigidbody2D>()
						where Pits.Heavy(r.gameObject)
						select r).ToArray<Rigidbody2D>();
					Bridge.Say(string.Format("heavy things on the layer: {0}; e.g. ", array12.Length) + string.Join(", ", from r in array12.Take<Rigidbody2D>(8)
						select string.Format("{0} {1:0.#}kg", r.name, r.mass)));
					return;
				}
				IL_474D:
				string text22 = ((a.Length > 1) ? a[1] : "flesh");
				if (text22 == "flesh")
				{
					RaycastHit2D raycastHit2D = Physics2D.Raycast(b.transform.position + (b.isRight ? Vector2.right : Vector2.left) * 5f + Vector2.up, Vector2.down, 8f, LayerMask.GetMask(new string[] { "Ground" }));
					if (raycastHit2D)
					{
						Flesh.Spawn(raycastHit2D.point);
					}
				}
				else if (text22 == "wall")
				{
					Walls.DebugWall(b, WallKind.Flesh);
				}
				else if (text22 == "slimewall")
				{
					Walls.DebugWall(b, WallKind.Slime);
				}
				else if (text22 == "vent")
				{
					LustGas.DebugVent(b);
				}
				else if (text22 == "tent")
				{
					Tentacles.DebugSpawn(b);
				}
				else if (text22 == "pit")
				{
					Pits.DebugPit(b, null);
				}
				else if (text22 == "worm")
				{
					Worm.DebugSpawn(b);
				}
				Bridge.Say("made " + text22);
				return;
			}
			if (num <= 2481552184U)
			{
				if (num <= 2165035846U)
				{
					if (num <= 1909948760U)
					{
						if (num <= 1836553703U)
						{
							if (num != 1821323511U)
							{
								if (num != 1836553703U)
								{
									goto IL_93D4;
								}
								if (!(text == "wormdrop"))
								{
									goto IL_93D4;
								}
								goto IL_4386;
							}
							else
							{
								if (!(text == "slit"))
								{
									goto IL_93D4;
								}
								SlitMouth slitMouth3 = Slits.DebugMake(b);
								Bridge.Say(slitMouth3 ? string.Format("womb slit at {0} facing {1}, {2:0.0} from her", slitMouth3.Mouth, (slitMouth3.Dir > 0) ? "right" : "left", Vector2.Distance(slitMouth3.Mouth, b.transform.position)) : "no slope's face for a womb slit near her");
								return;
							}
						}
						else if (num != 1894398805U)
						{
							if (num != 1896895332U)
							{
								if (num != 1909948760U)
								{
									goto IL_93D4;
								}
								if (!(text == "letgo"))
								{
									goto IL_93D4;
								}
								if (Pits.InMaw)
								{
									Pits.LetGo(b);
									Bridge.Say("the maw spits her out (as a won struggle)");
									return;
								}
								if (Slits.Holding || Slits.Inside)
								{
									Slits.LetGo(b);
									Bridge.Say("the womb slit lets her go");
									return;
								}
								SlimeBog.LetGo(b);
								Bridge.Say("slime lets go (hauled out)");
								return;
							}
							else
							{
								if (!(text == "cocoon"))
								{
									goto IL_93D4;
								}
								goto IL_4427;
							}
						}
						else
						{
							if (!(text == "lang"))
							{
								goto IL_93D4;
							}
							Bridge.Say(string.Format("game {0}, mod {1} (russian {2}, chinese {3}), armory english {4}: ", new object[]
							{
								Locale.currentLangName,
								Plugin.Lang,
								Plugin.Russian,
								Plugin.Chinese,
								Plugin.English
							}) + string.Format("{0} / {1} / {2} / {3} ({4} + {5} zh)", new object[]
							{
								Armory.Name(Armory.All[0]),
								Plugin.T("inv_vagina"),
								Plugin.T("slit_name"),
								Plugin.Zh("Got you!"),
								Plugin.TextZh.Count,
								Plugin.PhrasesZh.Count
							}));
							return;
						}
					}
					else if (num <= 2077956170U)
					{
						if (num != 2057535312U)
						{
							if (num != 2077956170U)
							{
								goto IL_93D4;
							}
							if (!(text == "pond"))
							{
								goto IL_93D4;
							}
							Bridge.Say(LatexPond.DebugMake(b));
							return;
						}
						else
						{
							if (!(text == "mite"))
							{
								goto IL_93D4;
							}
							Bridge.Say(LatexMites.DebugSpawn(b, (a.Length > 1) ? int.Parse(a[1]) : 1));
							return;
						}
					}
					else if (num != 2122543653U)
					{
						if (num != 2128299726U)
						{
							if (num != 2165035846U)
							{
								goto IL_93D4;
							}
							if (!(text == "ddpickwin"))
							{
								goto IL_93D4;
							}
							BuildingEntity value2 = Traverse.Create(typeof(Lockpick)).Field("dummy").GetValue<BuildingEntity>();
							if (value2)
							{
								value2.health = 0f;
							}
							Bridge.Say(value2 ? "lock picked" : "no lock being picked");
							return;
						}
						else
						{
							if (!(text == "paraspawn"))
							{
								goto IL_93D4;
							}
							float num44 = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 0f);
							float num45 = ((a.Length > 2) ? float.Parse(a[2], CultureInfo.InvariantCulture) : 0f);
							Vector2 vector6;
							vector6..ctor(b.transform.position.x + num44, Scenes.FeetY(b) + num45);
							ParasiteAI parasiteAI = Parasites.Spawn(vector6, (a.Length > 3) ? int.Parse(a[3]) : 0, a.Length > 4);
							Bridge.Say(parasiteAI ? string.Format("parasite asked at {0}, put at {1}", vector6, parasiteAI.transform.position) : "no parasite");
							return;
						}
					}
					else
					{
						if (!(text == "slitgrab"))
						{
							goto IL_93D4;
						}
						Bridge.Say(Slits.DebugGrab(b, false));
						return;
					}
				}
				else if (num <= 2388087250U)
				{
					if (num <= 2341128772U)
					{
						if (num != 2232908586U)
						{
							if (num != 2341128772U)
							{
								goto IL_93D4;
							}
							if (!(text == "orgasm"))
							{
								goto IL_93D4;
							}
							Plugin.Orgasm(b, false);
							Bridge.Say("orgasm");
							return;
						}
						else
						{
							if (!(text == "hostspawn"))
							{
								goto IL_93D4;
							}
							Vector2 vector7 = b.transform.position + new Vector2((b.isRight ? 1f : (-1f)) * ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 6f), 0f);
							Scavengers.HostPack(Scavengers.GroundNear(vector7).GetValueOrDefault(vector7));
							Bridge.Say(string.Format("slime host at {0}, wants her {1}", vector7, Capture.HostWantsHer));
							return;
						}
					}
					else if (num != 2366795564U)
					{
						if (num != 2386521466U)
						{
							if (num != 2388087250U)
							{
								goto IL_93D4;
							}
							if (!(text == "campgo"))
							{
								goto IL_93D4;
							}
							Camp current2 = Camps.Current;
							if (current2 == null)
							{
								Bridge.Say("no camp");
								return;
							}
							Bridge.Move(b, current2.Center + new Vector2((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 1.5f, 2.7f) - b.transform.position);
							Bridge.Say(string.Format("she is by the camp at {0}", current2.Center));
							return;
						}
						else
						{
							if (!(text == "breaknear"))
							{
								goto IL_93D4;
							}
							BuildingEntity buildingEntity2 = null;
							foreach (BuildingEntity buildingEntity3 in Object.FindObjectsOfType<BuildingEntity>())
							{
								if ((buildingEntity3.id ?? buildingEntity3.name) == a[1] && (!buildingEntity2 || Vector2.Distance(buildingEntity3.transform.position, b.transform.position) < Vector2.Distance(buildingEntity2.transform.position, b.transform.position)))
								{
									buildingEntity2 = buildingEntity3;
								}
							}
							if (!buildingEntity2)
							{
								Bridge.Say("no " + a[1]);
								return;
							}
							buildingEntity2.health = 0.1f;
							Bridge.Say(string.Format("broke {0} at {1}, loot crate {2}", a[1], buildingEntity2.transform.position, Outfits.IsLootCrate(buildingEntity2)));
							return;
						}
					}
					else
					{
						if (!(text == "witness"))
						{
							goto IL_93D4;
						}
						Bridge.Say("witness: " + (Witness.Why(b, false) ?? "nobody") + string.Format(" (hurt recently {0})", Witness.RecentlyHurt));
						return;
					}
				}
				else if (num <= 2448968664U)
				{
					if (num != 2411172191U)
					{
						if (num != 2443948391U)
						{
							if (num != 2448968664U)
							{
								goto IL_93D4;
							}
							if (!(text == "minigame"))
							{
								goto IL_93D4;
							}
							Bridge.Say("minigame: " + ((MinigameBase.main && MinigameBase.main.currentMinigame) ? MinigameBase.main.currentMinigame.GetType().Name : "none"));
							return;
						}
						else
						{
							if (!(text == "hitscav"))
							{
								goto IL_93D4;
							}
							Scavenger scavenger9 = (from x in Scavengers.All
								where x && !x.slimeHost && x.mode != Scavenger.Mode.Dead
								orderby Vector2.Distance(x.pos, b.transform.position)
								select x).FirstOrDefault<Scavenger>();
							if (scavenger9)
							{
								scavenger9.OnHit(5f);
							}
							Bridge.Say(scavenger9 ? string.Format("hit a raider: now {0}, sated {1}", scavenger9.mode, Traverse.Create(scavenger9).Field("sated").GetValue()) : "no raider");
							return;
						}
					}
					else
					{
						if (!(text == "layers"))
						{
							goto IL_93D4;
						}
						int d = LayerMask.NameToLayer("Descriptor");
						Bridge.Say(string.Format("queriesHitTriggers {0}; Descriptor ignores: ", Physics2D.queriesHitTriggers) + string.Join(", ", from n in new string[] { "Default", "Ground", "Body", "Limb", "Descriptor", "Item" }
							where LayerMask.NameToLayer(n) >= 0
							select string.Format("{0}={1}", n, Physics2D.GetIgnoreLayerCollision(d, LayerMask.NameToLayer(n)))));
						return;
					}
				}
				else if (num != 2452564170U)
				{
					if (num != 2459084481U)
					{
						if (num != 2481552184U)
						{
							goto IL_93D4;
						}
						if (!(text == "inv"))
						{
							goto IL_93D4;
						}
						InvasionPopup.DebugShow(a[1], (a.Length > 2) ? a[2] : "vagina", a.Length > 3 && a[3] == "love");
						Bridge.Say("invasion window " + a[1]);
						return;
					}
					else
					{
						if (!(text == "chance"))
						{
							goto IL_93D4;
						}
						SecondChance.Take(b);
						Bridge.Say(string.Format("second chance: corruption {0:0} exhaustion {1:0} suit {2} puppet {3}", new object[]
						{
							Plugin.S.corruption,
							Plugin.S.exhaustion,
							Suit.Stage,
							Puppet.Active
						}));
						return;
					}
				}
				else if (!(text == "rayat"))
				{
					goto IL_93D4;
				}
			}
			else if (num <= 2853391968U)
			{
				if (num <= 2686129127U)
				{
					if (num <= 2554875734U)
					{
						if (num != 2495097014U)
						{
							if (num != 2554875734U)
							{
								goto IL_93D4;
							}
							if (!(text == "chest"))
							{
								goto IL_93D4;
							}
							foreach (SpriteRenderer spriteRenderer3 in b.limbs[1].GetComponentsInChildren<SpriteRenderer>(true))
							{
								Bridge.Say(string.Format("chest: {0} on {1} layer {2} order {3} sprite {4}", new object[]
								{
									spriteRenderer3.name,
									spriteRenderer3.enabled,
									spriteRenderer3.sortingLayerName,
									spriteRenderer3.sortingOrder,
									spriteRenderer3.sprite ? string.Concat(new string[]
									{
										spriteRenderer3.sprite.name,
										" ",
										spriteRenderer3.sprite.rect.ToString(),
										" ppu ",
										spriteRenderer3.sprite.pixelsPerUnit.ToString(),
										" bounds ",
										spriteRenderer3.sprite.bounds.ToString()
									}) : "none"
								}));
							}
							return;
						}
						else
						{
							if (!(text == "flowerinfo"))
							{
								goto IL_93D4;
							}
							FlowerPlant flowerPlant3 = (from x in Flowers.Plants
								where x
								orderby Vector2.Distance(x.transform.position, b.transform.position)
								select x).FirstOrDefault<FlowerPlant>();
							if (!flowerPlant3)
							{
								Bridge.Say("no flower");
								return;
							}
							FleshBurn component13 = flowerPlant3.GetComponent<FleshBurn>();
							string text23 = string.Format("flower at {0} flip {1} anim {2} sated {3:0.0} hp {4:0} holding {5} blind {6:0.00} ", new object[]
							{
								flowerPlant3.transform.position,
								flowerPlant3.flip,
								flowerPlant3.FrameName,
								flowerPlant3.sated,
								component13 ? component13.hp : (-1f),
								Flowers.Holding,
								Flowers.Blind
							});
							string text24 = "throat {0} front {1} her {2} inMaw {3} pit open {4} milk {5} ";
							object[] array14 = new object[6];
							array14[0] = flowerPlant3.Point(1);
							array14[1] = flowerPlant3.MouthFront;
							array14[2] = b.limbs[1].transform.position;
							array14[3] = Pits.InMaw;
							int num46 = 4;
							Pit pit = flowerPlant3.pit;
							array14[num46] = ((pit != null) ? new bool?(pit.Open) : null);
							int num47 = 5;
							Pit pit2 = flowerPlant3.pit;
							array14[num47] = ((pit2 != null) ? new bool?(pit2.Milk) : null);
							Bridge.Say(text23 + string.Format(text24, array14) + string.Format("exhaustion {0:0} arousal {1:0} milkTimer {2:0.0} ppu {3:0.0} size {4}", new object[]
							{
								Plugin.S.exhaustion,
								Plugin.S.arousal,
								Plugin.S.milkTimer,
								FlowerArt.Ppu,
								flowerPlant3.sr.bounds.size
							}));
							return;
						}
					}
					else if (num != 2562668600U)
					{
						if (num != 2571906332U)
						{
							if (num != 2686129127U)
							{
								goto IL_93D4;
							}
							if (!(text == "nude"))
							{
								goto IL_93D4;
							}
							foreach (Item item12 in b.GetAllWearables().ToList<Item>())
							{
								b.DropWearable(item12);
							}
							Bridge.Say("all she wore dropped");
							return;
						}
						else
						{
							if (!(text == "give"))
							{
								goto IL_93D4;
							}
							GameObject gameObject16 = Resources.Load(a[1]) as GameObject;
							if (!gameObject16)
							{
								Bridge.Say("no item " + a[1]);
								return;
							}
							GameObject gameObject17 = Object.Instantiate<GameObject>(gameObject16, b.transform.position, Quaternion.identity);
							Item component14 = gameObject17.GetComponent<Item>();
							int num48 = -1;
							for (int num49 = 0; num49 < b.slots.Length; num49++)
							{
								if (!b.HoldingItem(num49))
								{
									num48 = num49;
									break;
								}
							}
							if (component14 && num48 >= 0)
							{
								b.PickUpItem(component14, num48, true);
								Bridge.Say(string.Format("gave {0} into slot {1}", a[1], num48));
								return;
							}
							gameObject17.transform.SetParent(b.transform, true);
							gameObject17.SetActive(false);
							Bridge.Say("gave " + a[1] + " (held out of sight)");
							return;
						}
					}
					else
					{
						if (!(text == "wormeat"))
						{
							goto IL_93D4;
						}
						Worm.DebugForceTentacles = ((a.Length > 1) ? new bool?(a[1] == "tent") : null);
						WormBeast wormBeast6 = Worm.Beasts.Find((WormBeast x) => x) ?? Worm.Spawn(b.transform.position + new Vector2(8f, 0f));
						if (wormBeast6)
						{
							Worm.Swallow(b, wormBeast6);
						}
						Bridge.Say(string.Format("swallowed: tentacles {0}", Worm.Tentacled));
						return;
					}
				}
				else if (num <= 2734574112U)
				{
					if (num != 2695511749U)
					{
						if (num != 2714341963U)
						{
							if (num != 2734574112U)
							{
								goto IL_93D4;
							}
							if (!(text == "wallcorner"))
							{
								goto IL_93D4;
							}
							WorldGeneration world9 = WorldGeneration.world;
							int num50 = ((a.Length > 1) ? int.Parse(a[1]) : 4);
							Bridge.cornerAt = world9.WorldToBlockPos(new Vector2(b.transform.position.x, Scenes.FeetY(b) - 0.5f));
							Bridge.cornerDir = (b.isRight ? 1 : (-1));
							ushort num51 = world9.GetBlock(Bridge.cornerAt);
							if (num51 == 0)
							{
								num51 = 1;
							}
							for (int num52 = 0; num52 <= 12; num52++)
							{
								for (int num53 = 1; num53 <= 2; num53++)
								{
									world9.SetBlock(new Vector2Int(Bridge.cornerAt.x + Bridge.cornerDir * num53, Bridge.cornerAt.y + num52), (num52 == 0) ? num51 : 0);
								}
								for (int num54 = 3; num54 <= 6; num54++)
								{
									world9.SetBlock(new Vector2Int(Bridge.cornerAt.x + Bridge.cornerDir * num54, Bridge.cornerAt.y + num52), (num52 <= num50) ? num51 : 0);
								}
							}
							Bridge.Say(string.Format("rock {0} blocks tall carved in front of her (the colliders catch up next frame: then \"wall\" or \"wall slime\")", num50));
							return;
						}
						else
						{
							if (!(text == "guide"))
							{
								goto IL_93D4;
							}
							Bridge.Say(string.Format("crouch guide colour {0}, crouching {1}", PlayerCamera.main.crouchGuide.color, b.crouching));
							return;
						}
					}
					else
					{
						if (!(text == "camphurt"))
						{
							goto IL_93D4;
						}
						Camp current3 = Camps.Current;
						if (current3 == null || !current3.Build)
						{
							Bridge.Say("no trader");
							return;
						}
						float num55 = float.Parse(a[1], CultureInfo.InvariantCulture);
						if (a.Length > 2 && a[2] == "her")
						{
							current3.Build.health -= num55;
						}
						else
						{
							Camps.HurtTrader(num55);
						}
						Bridge.Say(string.Format("trader health {0:0}, dead {1}, betrayed {2}", current3.Build.health, current3.Dead, current3.Betrayed));
						return;
					}
				}
				else if (num != 2804296981U)
				{
					if (num != 2851026516U)
					{
						if (num != 2853391968U)
						{
							goto IL_93D4;
						}
						if (!(text == "tent"))
						{
							goto IL_93D4;
						}
						Tentacles.DebugSpawn(b);
						Bridge.Say("tentacles above her");
						return;
					}
					else
					{
						if (!(text == "camp"))
						{
							goto IL_93D4;
						}
						goto IL_870E;
					}
				}
				else
				{
					if (!(text == "wall"))
					{
						goto IL_93D4;
					}
					Walls.DebugWall(b, (a.Length > 1 && a[1] == "slime") ? WallKind.Slime : WallKind.Flesh);
					return;
				}
			}
			else if (num <= 3020151336U)
			{
				if (num <= 2984927816U)
				{
					if (num != 2917080719U)
					{
						if (num != 2984927816U)
						{
							goto IL_93D4;
						}
						if (!(text == "fill"))
						{
							goto IL_93D4;
						}
						Womb.Fill(b, (a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 20f);
						Bridge.Say("cum in her");
						return;
					}
					else
					{
						if (!(text == "deal"))
						{
							goto IL_93D4;
						}
						if (a.Length > 1 && a[1] == "pick")
						{
							string text25;
							Bridge.Say(string.Concat(new string[]
							{
								"deal pick: ",
								Deal.Pick(b, out text25),
								" (",
								text25,
								")"
							}));
							return;
						}
						if (a.Length > 1 && a[1] == "yes")
						{
							Bridge.Say(string.Format("deal accepted {0}", Deal.Accept(b)));
							return;
						}
						if (a.Length > 1 && a[1] == "propose")
						{
							Bridge.Say(string.Format("her offer made {0}, captured {1}", Deal.Propose(b), Capture.Active));
							return;
						}
						Scavenger scavenger10 = (from x in Scavengers.All
							where x && !x.slimeHost && x.mode != Scavenger.Mode.Dead
							orderby Vector2.Distance(x.pos, b.transform.position)
							select x).FirstOrDefault<Scavenger>();
						if (!scavenger10)
						{
							Bridge.Say("no raider");
							return;
						}
						scavenger10.lecher = true;
						scavenger10.offered = false;
						Bridge.Say(string.Format("deal offered {0}", Deal.TryOffer(scavenger10, b, true)));
						return;
					}
				}
				else if (num != 3005452862U)
				{
					if (num != 3019887885U)
					{
						if (num != 3020151336U)
						{
							goto IL_93D4;
						}
						if (!(text == "suppdose"))
						{
							goto IL_93D4;
						}
						float corruption = Plugin.S.corruption;
						Traverse.Create(typeof(Suppressant)).Method("Dose", new object[] { 5f, b }).GetValue();
						Recipe recipe = Recipes.recipes.FirstOrDefault<Recipe>((Recipe r) => r.result != null && r.result.id == "lewd_suppressant");
						Bridge.Say(string.Format("suppressant dose: timer {0:0}s corruption {1:0.0}->{2:0.0} peak {3:0} recipe INT {4}", new object[]
						{
							Plugin.S.suppressTimer,
							corruption,
							Plugin.S.corruption,
							Plugin.S.corruptionPeak,
							(recipe != null) ? recipe.INT : (-1)
						}));
						return;
					}
					else
					{
						if (!(text == "slitgo"))
						{
							goto IL_93D4;
						}
						SlitMouth slitMouth4 = (from x in Slits.All
							where x
							orderby Vector2.Distance(x.Mouth, b.transform.position)
							select x).FirstOrDefault<SlitMouth>();
						if (!slitMouth4)
						{
							Bridge.Say("no womb slit");
							return;
						}
						float num56 = ((a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 10f);
						float num57 = slitMouth4.FaceX + (float)slitMouth4.Dir * num56;
						RaycastHit2D raycastHit2D2 = Physics2D.Raycast(new Vector2(num57, slitMouth4.Mouth.y + 3f), Vector2.down, 12f, LayerMask.GetMask(new string[] { "Ground" }));
						if (!raycastHit2D2)
						{
							Bridge.Say("no floor there");
							return;
						}
						Bridge.Move(b, new Vector2(num57, raycastHit2D2.point.y + 0.2f) - new Vector2(b.transform.position.x, Scenes.FeetY(b)));
						Bridge.Say(string.Format("in front of the slit at {0}, {1:0.#} away", slitMouth4.Mouth, num56));
						return;
					}
				}
				else
				{
					if (!(text == "about"))
					{
						goto IL_93D4;
					}
					if (a.Length > 1 && a[1] == "notice")
					{
						AboutMod.ShowNotice();
					}
					else
					{
						AboutMod.ShowWindow();
					}
					Bridge.Say("about shown");
					return;
				}
			}
			else if (num <= 3190225515U)
			{
				if (num != 3023610511U)
				{
					if (num != 3186656602U)
					{
						if (num != 3190225515U)
						{
							goto IL_93D4;
						}
						if (!(text == "sel"))
						{
							goto IL_93D4;
						}
						Bridge.lastPit = Pits.All[int.Parse(a[1])];
						Bridge.Say(string.Format("selected {0} at {1}", Bridge.lastPit.Kind, Bridge.lastPit.Mouth));
						return;
					}
					else
					{
						if (!(text == "find"))
						{
							goto IL_93D4;
						}
						Bridge.Say(a[1] + ": " + Devices.Where(a[1]));
						return;
					}
				}
				else
				{
					if (!(text == "scavspawn"))
					{
						goto IL_93D4;
					}
					Scavengers.DebugSpawn(b);
					Bridge.Say("scavenger spawned");
					return;
				}
			}
			else if (num != 3219261619U)
			{
				if (num != 3325034847U)
				{
					if (num != 3341224086U)
					{
						goto IL_93D4;
					}
					if (!(text == "sew"))
					{
						goto IL_93D4;
					}
					foreach (Item item13 in from i in Clothes.Worn(b)
						where Outfits.ById(i.id) != null
						select i)
					{
						item13.condition = 0.4f;
						Clothes.Look(b, item13);
					}
					Item component15 = Object.Instantiate<GameObject>(Resources.Load("lewd_sewingkit") as GameObject, b.transform.position, Quaternion.identity).GetComponent<Item>();
					component15.Stats.useAction.Invoke(b, component15);
					bool flag3 = Recipes.recipes.Any<Recipe>((Recipe r) => r.result != null && r.result.id == "lewd_sewingkit");
					int num58 = Recipes.recipes.Count<Recipe>((Recipe r) => r.isRepair && r.result != null && Outfits.ById(r.result.id) != null);
					Bridge.Say(string.Format("sew: kit {0:P0} name '{1}' recipe {2} outfit repair recipes {3}; clothes: ", new object[]
					{
						component15.condition,
						Locale.GetItem("lewd_sewingkit"),
						flag3,
						num58
					}) + string.Join(", ", from i in Clothes.Worn(b)
						select string.Format("{0} {1:P0}", i.id, i.condition)));
					return;
				}
				else
				{
					if (!(text == "sit"))
					{
						goto IL_93D4;
					}
					b.bodyAnimator.Play("ExperimentSit");
					b.armsAnimator.Play("ArmsSit");
					Bridge.Say("sit");
					return;
				}
			}
			else
			{
				if (!(text == "wallcut"))
				{
					goto IL_93D4;
				}
				WorldGeneration world10 = WorldGeneration.world;
				int num59 = ((a.Length > 1) ? int.Parse(a[1]) : 2);
				for (int num60 = 1; num60 <= num59; num60++)
				{
					world10.SetBlock(new Vector2Int(Bridge.cornerAt.x + Bridge.cornerDir * 3, Bridge.cornerAt.y + num60), 0);
				}
				Bridge.Say(string.Format("dug {0} blocks out of the rock face; walls left: {1}", num59, Walls.All.Count<LewdWall>((LewdWall x) => x)));
				return;
			}
			IL_2262:
			Transform transform3 = null;
			if (a.Length > 1 && a[1] == "worm")
			{
				WormBeast wormBeast7 = Worm.Beasts.FirstOrDefault<WormBeast>((WormBeast x) => x);
				if (wormBeast7)
				{
					CircleCollider2D componentInChildren = wormBeast7.transform.GetComponentInChildren<CircleCollider2D>();
					transform3 = ((componentInChildren != null) ? componentInChildren.transform : null);
				}
			}
			else if (a.Length > 1 && a[1] == "latex")
			{
				LatexBlob latexBlob = (from x in Latexes.All
					where x && !x.dead
					orderby Vector2.Distance(x.pos, b.transform.position)
					select x).FirstOrDefault<LatexBlob>();
				transform3 = ((latexBlob != null) ? latexBlob.transform : null);
			}
			else
			{
				TentacleTrap tentacleTrap2 = (from x in Tentacles.Traps
					where x
					orderby Vector2.Distance(x.transform.position, b.transform.position)
					select x).FirstOrDefault<TentacleTrap>();
				transform3 = ((tentacleTrap2 != null) ? tentacleTrap2.transform : null);
			}
			if (!transform3)
			{
				Bridge.Say("no target");
				return;
			}
			Collider2D component16 = transform3.GetComponent<Collider2D>();
			Vector2 vector8 = b.limbs[1].transform.position;
			Vector2 vector9 = (component16 ? component16.bounds.center : transform3.position);
			BuildingEntity component17 = transform3.GetComponent<BuildingEntity>();
			float num61 = (component17 ? component17.health : (-1f));
			if (a[0] == "shootat")
			{
				TurretScript.Shoot(new FireInfo
				{
					pos = vector8,
					dir = (vector9 - vector8).normalized,
					ignoreBody = true,
					animalDamage = 60f,
					structureDamage = 0f,
					playerDamageMultiplier = 0f
				});
			}
			RaycastHit2D[] array15 = Physics2D.RaycastAll(vector8, (vector9 - vector8).normalized, Vector2.Distance(vector8, vector9) + 1f);
			Bridge.Say(string.Format("{0} {1} dist {2:0.0} layer {3} health {4:0} -> {5:0}; ray: ", new object[]
			{
				a[0],
				a[1],
				Vector2.Distance(vector8, vector9),
				LayerMask.LayerToName(transform3.gameObject.layer),
				num61,
				component17 ? component17.health : (-1f)
			}) + string.Join(", ", array15.Select<RaycastHit2D, string>((RaycastHit2D h) => h.transform.name + (h.transform.CompareTag("BlockGround") ? "(ground)" : ""))));
			return;
			IL_4427:
			Webs.DebugCocoon(b);
			Bridge.Say(string.Format("web caught {0} cocoon {1}", Webs.Caught, Webs.InCocoon));
			return;
			IL_870E:
			Camps.ForceLook = ((a.Length > 1) ? int.Parse(a[1]) : (-1));
			Bridge.Say(Camps.DebugHere(b));
			Camps.ForceLook = -1;
			return;
			IL_4386:
			string text26 = "drop bomb: ";
			object value3 = Traverse.Create(typeof(Worm)).Method("DropBomb", new object[] { b }).GetValue();
			Bridge.Say(text26 + ((value3 != null) ? value3.ToString() : null));
			return;
			IL_5ACA:
			using (List<WormBeast>.Enumerator enumerator6 = Worm.Beasts.GetEnumerator())
			{
				while (enumerator6.MoveNext())
				{
					WormBeast wormBeast8 = enumerator6.Current;
					if (wormBeast8)
					{
						wormBeast8.OnOutsideHit();
						Bridge.Say("worm hit: " + wormBeast8.State(b));
					}
				}
				return;
			}
			IL_5B29:
			Semen.Add(b, (a.Length > 1) ? float.Parse(a[1], CultureInfo.InvariantCulture) : 20f);
			Bridge.Say("cum on her");
			return;
			IL_635E:
			Limb limb6 = b.limbs[1];
			SpriteRenderer[] array16 = (from r in limb6.GetComponentsInChildren<SpriteRenderer>()
				where r.enabled && r.sprite
				orderby r.sortingOrder
				select r).ToArray<SpriteRenderer>();
			SpriteRenderer component18 = limb6.GetComponent<SpriteRenderer>();
			float pixelsPerUnit = component18.sprite.pixelsPerUnit;
			int num62 = 16;
			int num63 = 12;
			int num64 = 2;
			int num65 = 1;
			Color[] array17 = new Color[num62 * num63];
			foreach (SpriteRenderer spriteRenderer4 in array16)
			{
				Sprite sprite2 = spriteRenderer4.sprite;
				Rect textureRect2 = sprite2.textureRect;
				Vector3 vector10 = spriteRenderer4.transform.localPosition;
				if (spriteRenderer4.transform == limb6.transform)
				{
					vector10 = Vector3.zero;
				}
				int num66 = num64 + Mathf.RoundToInt((sprite2.bounds.min.x + (spriteRenderer4.flipX ? (-vector10.x) : vector10.x) - component18.sprite.bounds.min.x) * pixelsPerUnit);
				int num67 = num65 + Mathf.RoundToInt((component18.sprite.bounds.max.y - (sprite2.bounds.max.y + vector10.y)) * pixelsPerUnit);
				int num68 = 0;
				while ((float)num68 < textureRect2.height)
				{
					int num69 = 0;
					while ((float)num69 < textureRect2.width)
					{
						Color color2;
						try
						{
							color2 = Bust.Readable(sprite2.texture).GetPixel((int)textureRect2.x + num69, (int)textureRect2.y + (int)textureRect2.height - 1 - num68) * spriteRenderer4.color;
						}
						catch
						{
							goto IL_657E;
						}
						goto IL_6536;
						IL_657E:
						num69++;
						continue;
						IL_6536:
						int num70 = num66 + num69;
						int num71 = num67 + num68;
						if (color2.a >= 0.5f && num70 >= 0 && num71 >= 0 && num70 < num62 && num71 < num63)
						{
							array17[(num63 - 1 - num71) * num62 + num70] = color2;
							goto IL_657E;
						}
						goto IL_657E;
					}
					num68++;
				}
			}
			Texture2D texture2D4 = new Texture2D(num62 * 8, num63 * 8, 4, false);
			for (int num72 = 0; num72 < num63 * 8; num72++)
			{
				for (int num73 = 0; num73 < num62 * 8; num73++)
				{
					texture2D4.SetPixel(num73, num72, (array17[num72 / 8 * num62 + num73 / 8].a > 0f) ? array17[num72 / 8 * num62 + num73 / 8] : new Color(0.15f, 0.15f, 0.2f));
				}
			}
			string text27 = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "chest_" + ((a.Length > 1) ? a[1] : "now") + ".png");
			File.WriteAllBytes(text27, ImageConversion.EncodeToPNG(texture2D4));
			Bridge.Say("chest drawn to " + text27 + ": " + string.Join(", ", array16.Select<SpriteRenderer, string>((SpriteRenderer r) => string.Format("{0}({1})", r.name, r.sortingOrder))));
			return;
			IL_93D4:
			Bridge.Say("unknown: " + string.Join(" ", a));
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0002518C File Offset: 0x0002338C
		private static void Move(Body b, Vector2 d)
		{
			b.transform.position += d;
			foreach (Limb limb in b.limbs)
			{
				if (limb && !limb.dismembered)
				{
					if (!limb.transform.IsChildOf(b.transform))
					{
						limb.transform.position += d;
					}
					if (limb.rb)
					{
						limb.rb.velocity = Vector2.zero;
					}
				}
			}
			b.rb.velocity = Vector2.zero;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0002523C File Offset: 0x0002343C
		private static void Dump(Body b)
		{
			float[] array = (from l in b.limbs
				where l && !l.dismembered
				select l.transform.position.y).ToArray<float>();
			string[] array2 = new string[5];
			array2[0] = string.Format("pos {0} vel {1} alive {2} conscious {3} ({4:0}) ", new object[]
			{
				b.transform.position,
				b.rb.velocity,
				b.alive,
				b.conscious,
				b.consciousness
			});
			array2[1] = string.Format("limbs y {0:0.00}..{1:0.00} inMaw {2} wall {3} slime {4} ", new object[]
			{
				array.Min(),
				array.Max(),
				Pits.InMaw,
				FleshWall.Holding,
				SlimeBog.Stuck
			});
			array2[2] = "pits [";
			array2[3] = string.Join("; ", Pits.All.Select<Pit, string>((Pit p) => string.Format("{0} open {1} closed {2:0.00}", p.Kind, p.Open, p.Closed)));
			array2[4] = "]";
			Bridge.Say(string.Concat(array2));
		}

		// Token: 0x040001AB RID: 427
		private static readonly string File_ = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "bridge_cmd.txt");

		// Token: 0x040001AC RID: 428
		private static float poll;

		// Token: 0x040001AD RID: 429
		private static float faint;

		// Token: 0x040001AE RID: 430
		private static float walk;

		// Token: 0x040001AF RID: 431
		private static float swim;

		// Token: 0x040001B0 RID: 432
		public static Vector2 SwimDir = Vector2.up;

		// Token: 0x040001B1 RID: 433
		private static Pit lastPit;

		// Token: 0x040001B2 RID: 434
		private static Scavenger lastScav;

		// Token: 0x040001B3 RID: 435
		private static Vector2Int cornerAt;

		// Token: 0x040001B4 RID: 436
		private static int cornerDir = 1;
	}
}
