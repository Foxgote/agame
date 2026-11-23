using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LewdStatus;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000046 RID: 70
	internal static class Scavengers
	{
		// Token: 0x0600015D RID: 349 RVA: 0x00015F80 File Offset: 0x00014180
		public static WeaponDef PickWeapon(int depth)
		{
			bool gun = Random.value < Plugin.ScavGunShare.Value;
			bool ours = Plugin.GunsEnabled.Value;
			List<WeaponDef> list = Scavengers.Weapons.Where<WeaponDef>((WeaponDef w) => w.Gun == gun && w.MinDepth <= depth && (ours || !w.Id.StartsWith("arm_")) && Resources.Load(w.Id) != null).ToList<WeaponDef>();
			if (list.Count == 0)
			{
				list = Scavengers.Weapons.Where<WeaponDef>((WeaponDef w) => w.MinDepth <= depth && (ours || !w.Id.StartsWith("arm_"))).ToList<WeaponDef>();
			}
			float num = Random.value * list.Sum<WeaponDef>((WeaponDef w) => w.Rarity);
			foreach (WeaponDef weaponDef in list)
			{
				if ((num -= weaponDef.Rarity) <= 0f)
				{
					return weaponDef;
				}
			}
			return list.Last<WeaponDef>();
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0001608C File Offset: 0x0001428C
		public static void Update(Body b)
		{
			WorldGeneration world = WorldGeneration.world;
			if (!world || !world.worldExists || world.biomeOverride == 1 || !b)
			{
				return;
			}
			if (world == Scavengers.seededWorld && world.biomeDepth == Scavengers.seededDepth)
			{
				return;
			}
			if (Scavengers.pendingSince <= 0f)
			{
				Scavengers.pendingSince = Time.time;
			}
			if (Time.time - Scavengers.pendingSince < 4f)
			{
				return;
			}
			if (b.transform.position.sqrMagnitude < 0.01f && Time.time - Scavengers.pendingSince < 30f)
			{
				return;
			}
			Scavengers.seededWorld = world;
			Scavengers.seededDepth = world.biomeDepth;
			Scavengers.pendingSince = 0f;
			foreach (Scavenger scavenger in Scavengers.All.ToArray())
			{
				if (scavenger)
				{
					Object.Destroy(scavenger.gameObject);
				}
			}
			Scavengers.All.Clear();
			if (Biome.Ending)
			{
				return;
			}
			Scavengers.Seed(b, world);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x000161A0 File Offset: 0x000143A0
		private static void Seed(Body b, WorldGeneration w)
		{
			int num = (Plugin.ScavEnabled.Value ? (Diff.Count(Plugin.ScavPerLayer.Value) + Mathf.Min(3, w.biomeDepth / 2)) : 0);
			int num2 = 0;
			int num3 = 0;
			while (num3 < 60 && num2 < num)
			{
				Vector2? vector = Scavengers.Floor(b.transform.position, 40f);
				if (vector != null)
				{
					int num4 = ((Random.value < 0.35f) ? 2 : 1);
					int num5 = 0;
					while (num5 < num4 && num2 < num)
					{
						Vector2? vector2 = ((num5 == 0) ? vector : Scavengers.GroundNear(vector.Value + Vector2.right * 2.5f * ((Random.value < 0.5f) ? 1f : (-1f))));
						if (vector2 != null)
						{
							Scavenger.Make(vector2.Value, Scavengers.PickWeapon(w.biomeDepth), Random.value < 0.5f, null, false, null);
							num2++;
						}
						num5++;
					}
				}
				num3++;
			}
			int num6 = ((Plugin.HostsEnabled.Value && LewdLink.Present && LewdLink.CanInfect) ? Mathf.RoundToInt(Plugin.HostPerLayer.Value * Diff.Spawn + ((Random.value < 0.4f) ? 1f : 0f)) : 0);
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			while (num8 < num6 && num9 < 30)
			{
				Vector2? vector3 = Scavengers.Floor(b.transform.position, 40f);
				if (vector3 != null)
				{
					num7 += Scavengers.HostPack(vector3.Value);
					num8++;
				}
				num9++;
			}
			Plugin.Log.LogInfo(string.Format("Layer {0}: {1} packs of slime hosts ({2} hosts)", w.biomeDepth, num6, num7));
			foreach (SlimeFluid.DarkPool darkPool in SlimeFluid.DarkPools)
			{
				darkPool.Hosts = false;
			}
			Scavengers.PlaceBogHosts();
			Camps.Place(b);
			Plugin.Log.LogInfo(string.Format("Layer {0}: {1} scavengers ({2})", w.biomeDepth, num2, string.Join(", ", Scavengers.All.Select<Scavenger, string>((Scavenger s) => (s.female ? "f " : "m ") + s.skin + " " + s.weapon.Id))));
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0001643C File Offset: 0x0001463C
		public static int HostPack(Vector2 at)
		{
			int num = Random.Range(Plugin.HostGroupMin.Value, Plugin.HostGroupMax.Value + 1);
			Scavenger scavenger = Scavenger.MakeHost(at, false, null);
			scavenger.pack = scavenger;
			int num2 = 1;
			for (int i = 1; i < num; i++)
			{
				Vector2? vector = Scavengers.GroundNear(at + Vector2.right * (1.8f + 1.4f * (float)i) * ((i % 2 == 0) ? 1f : (-1f)));
				Vector2? vector2 = ((vector != null) ? vector : Scavengers.GroundNear(at));
				if (vector2 != null)
				{
					Scavenger.MakeHost(vector2.Value, false, scavenger);
					num2++;
				}
			}
			return num2;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x000164F0 File Offset: 0x000146F0
		public static void PlaceBogHosts()
		{
			WorldGeneration world = WorldGeneration.world;
			if (!world || Scavengers.seededWorld != world || Scavengers.seededDepth != world.biomeDepth)
			{
				return;
			}
			if (!Plugin.HostsEnabled.Value || !LewdLink.Present || !LewdLink.CanInfect)
			{
				return;
			}
			foreach (SlimeFluid.DarkPool darkPool in SlimeFluid.DarkPools)
			{
				if (!darkPool.Hosts && !(darkPool.World != world) && darkPool.Depth == world.biomeDepth)
				{
					darkPool.Hosts = true;
					bool flag = Random.value < 0.5f;
					Vector2? vector = Scavengers.GroundNear(flag ? darkPool.Left : darkPool.Right);
					Vector2? vector2 = ((vector != null) ? vector : Scavengers.GroundNear(flag ? darkPool.Right : darkPool.Left));
					int num = ((vector2 != null) ? Scavengers.HostPack(vector2.Value) : 0);
					if (Random.value < 0.65f)
					{
						vector = Scavengers.GroundNear(darkPool.Bottom);
						if (vector != null)
						{
							Vector2 valueOrDefault = vector.GetValueOrDefault();
							Scavenger scavenger = Scavenger.MakeHost(valueOrDefault, false, null);
							scavenger.pack = scavenger;
							scavenger.Lurk();
							num++;
						}
					}
					Plugin.Log.LogInfo(string.Format("Dark slime bog at {0}: {1} slime hosts by it", darkPool.Bottom, num));
				}
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00016698 File Offset: 0x00014898
		public static void HostsStayNear(Vector2 at)
		{
			foreach (Scavenger scavenger in Scavengers.All)
			{
				if (scavenger && scavenger.slimeHost && scavenger.mode != Scavenger.Mode.Dead && Vector2.Distance(scavenger.pos, at) <= 30f)
				{
					scavenger.StayNear(at);
				}
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00016718 File Offset: 0x00014918
		public static void Remove(bool hosts)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			int num = 0;
			foreach (Scavenger scavenger in Scavengers.All.ToArray())
			{
				if (scavenger && scavenger.slimeHost == hosts)
				{
					if (scavenger.Holding && body)
					{
						Capture.Release(body, false, false);
					}
					Scavengers.All.Remove(scavenger);
					Object.Destroy(scavenger.gameObject);
					num++;
				}
			}
			if (!hosts)
			{
				Camps.Clear();
			}
			Plugin.Log.LogInfo(string.Format("Switched off: removed {0} {1}", num, hosts ? "slime hosts" : "scavengers"));
		}

		// Token: 0x06000164 RID: 356 RVA: 0x000167DC File Offset: 0x000149DC
		public static Vector2? GroundNear(Vector2 at)
		{
			WorldGeneration world = WorldGeneration.world;
			if (!world)
			{
				return new Vector2?(at);
			}
			Vector2Int vector2Int = world.WorldToBlockPos(at + Vector2.up * 0.2f);
			int num = 0;
			while (num < 3 && world.GetBlock(vector2Int) != 0)
			{
				vector2Int += Vector2Int.up;
				num++;
			}
			int i = 0;
			while (i < 6)
			{
				if (world.GetBlock(vector2Int) != 0)
				{
					return null;
				}
				if (world.GetBlock(vector2Int + Vector2Int.down) != 0)
				{
					for (int j = 0; j < 5; j++)
					{
						if (world.GetBlock(vector2Int + Vector2Int.up * j) != 0)
						{
							return null;
						}
					}
					return new Vector2?(world.BlockToWorldPos(vector2Int) + Vector2.down * 0.5f);
				}
				i++;
				vector2Int += Vector2Int.down;
			}
			return null;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000168E0 File Offset: 0x00014AE0
		public static Vector2? Floor(Vector2 from, float minDistance)
		{
			WorldGeneration world = WorldGeneration.world;
			for (int i = 0; i < 400; i++)
			{
				int num = Random.Range(10, (int)(world.width - 10U));
				int num2 = Random.Range(10, (int)(world.height - 10U));
				if (world.GetBlock(new Vector2Int(num, num2 - 1)) != 0)
				{
					bool flag = true;
					int num3 = 0;
					while (num3 <= 1 && flag)
					{
						int num4 = 0;
						while (num4 < 6 && flag)
						{
							flag = world.GetBlock(new Vector2Int(num + num3, num2 + num4)) == 0;
							num4++;
						}
						num3++;
					}
					if (flag && (!FluidManager.main || FluidManager.main.GetLiquid(num, num2) == 0) && !Camps.InQuiet(world.BlockToWorldPos(new Vector2Int(num, num2))))
					{
						Vector2 vector = world.BlockToWorldPos(new Vector2Int(num, num2)) + Vector2.down * 0.5f;
						if (Vector2.Distance(vector, from) >= minDistance)
						{
							return new Vector2?(vector);
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x000169FC File Offset: 0x00014BFC
		public static void DebugSpawn(Body b)
		{
			Vector2 vector = b.transform.position + new Vector2((b.isRight ? 1f : (-1f)) * 9f, -2.5f);
			Scavenger scavenger = Scavenger.Make(vector, Scavengers.PickWeapon(WorldGeneration.world ? WorldGeneration.world.biomeDepth : 0), Random.value < 0.5f, null, false, null);
			Plugin.Log.LogInfo(string.Format("Debug: scavenger ({0}, {1}, {2}, {3}) at {4}", new object[]
			{
				scavenger.female ? "female" : "male",
				scavenger.skin,
				scavenger.overall ?? "no overalls",
				scavenger.weapon.Id,
				vector
			}));
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00016ADC File Offset: 0x00014CDC
		public static void DropLoot(Scavenger s)
		{
			Vector2 vector = s.pos + Vector2.up * 1.5f;
			GameObject gameObject = Scavengers.Throw(s.weapon.Id, vector, Random.Range(0.25f, 0.8f));
			if (s.weapon.Gun)
			{
				Armory.MaybeLoad(gameObject, Plugin.ScavLoadedChance.Value);
			}
			if (s.weapon.Gun)
			{
				GameObject gameObject2 = Resources.Load(s.weapon.Id) as GameObject;
				GunScript gunScript;
				if (gameObject2 != null && gameObject2.TryGetComponent<GunScript>(ref gunScript))
				{
					string text = AmmoScript.AmmoTypeToMagazine(gunScript.ammoType);
					string text2 = AmmoScript.AmmoTypeToItem(gunScript.ammoType);
					if (gunScript.feedType == null && Random.value < 0.6f)
					{
						Scavengers.Throw(text, vector, 1f);
					}
					for (int i = Random.Range(0, 5); i > 0; i--)
					{
						Scavengers.Throw(text2, vector, 1f);
					}
				}
			}
			if (s.hasKey)
			{
				Scavengers.Throw(Devices.Key.Id, vector, 1f);
			}
			string[] array = new string[] { "food", "medical", "utility", "drug" };
			for (int j = Random.Range(0, 3); j > 0; j--)
			{
				string text3 = array[Random.Range(0, array.Length)];
				List<string> list;
				if (ItemLootPool.pool != null && ItemLootPool.pool.TryGetValue(text3, out list) && list.Count > 0)
				{
					Scavengers.Throw(list[Random.Range(0, list.Count)], vector, Random.Range(0.3f, 1f));
				}
			}
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00016C8C File Offset: 0x00014E8C
		internal static GameObject Throw(string id, Vector2 at, float condition)
		{
			GameObject gameObject = Resources.Load(id) as GameObject;
			if (!gameObject)
			{
				return null;
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, at, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
			Item item;
			if (gameObject2.TryGetComponent<Item>(ref item))
			{
				item.SetCondition(condition);
			}
			Rigidbody2D rigidbody2D;
			if (gameObject2.TryGetComponent<Rigidbody2D>(ref rigidbody2D))
			{
				rigidbody2D.velocity = new Vector2(Random.Range(-4f, 4f), Random.Range(2f, 6f));
			}
			return gameObject2;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00016D20 File Offset: 0x00014F20
		public static string Line(string key, bool female)
		{
			ValueTuple<string[], string[]> valueTuple;
			if (!Scavengers.Lines.TryGetValue(key, out valueTuple))
			{
				return null;
			}
			ValueTuple<string[], string[]> valueTuple2;
			if (female && Scavengers.Lines.TryGetValue(key + "_f", out valueTuple2) && Random.value < 0.7f)
			{
				valueTuple = valueTuple2;
			}
			string[] array = (Plugin.English ? valueTuple.Item2 : valueTuple.Item1);
			return Plugin.Zh(array[Random.Range(0, array.Length)]);
		}

		// Token: 0x04000178 RID: 376
		public static readonly List<Scavenger> All = new List<Scavenger>();

		// Token: 0x04000179 RID: 377
		public static readonly WeaponDef[] Weapons = new WeaponDef[]
		{
			new WeaponDef
			{
				Id = "pistol",
				Gun = true,
				Damage = 90f,
				Cooldown = 0.9f,
				Spread = 4f,
				PlayerMult = 0.4f,
				Mag = 10,
				Rarity = 1.4f
			},
			new WeaponDef
			{
				Id = "arm_compact",
				Gun = true,
				Damage = 77f,
				Cooldown = 0.8f,
				Spread = 4.5f,
				PlayerMult = 0.35f,
				Mag = 10,
				Rarity = 0.8f
			},
			new WeaponDef
			{
				Id = "arm_m45",
				Gun = true,
				Damage = 120f,
				Cooldown = 1.1f,
				Spread = 4f,
				PlayerMult = 0.55f,
				Mag = 7,
				Rarity = 0.6f,
				MinDepth = 1
			},
			new WeaponDef
			{
				Id = "arm_revolver",
				Gun = true,
				Damage = 140f,
				Cooldown = 1.3f,
				Spread = 3.5f,
				PlayerMult = 0.6f,
				Mag = 6,
				Rarity = 0.4f,
				MinDepth = 1
			},
			new WeaponDef
			{
				Id = "arm_mp9",
				Gun = true,
				TwoHanded = true,
				Damage = 70f,
				Cooldown = 1.4f,
				Spread = 7f,
				PlayerMult = 0.3f,
				Mag = 15,
				Burst = 3,
				Rarity = 0.3f,
				MinDepth = 2
			},
			new WeaponDef
			{
				Id = "rifle",
				Gun = true,
				TwoHanded = true,
				Damage = 120f,
				Cooldown = 1.6f,
				Spread = 3f,
				PlayerMult = 0.5f,
				Mag = 20,
				Burst = 3,
				Rarity = 0.4f,
				MinDepth = 2
			},
			new WeaponDef
			{
				Id = "shotgun",
				Gun = true,
				TwoHanded = true,
				Damage = 40f,
				Cooldown = 1.8f,
				Spread = 9f,
				PlayerMult = 0.25f,
				Mag = 5,
				Pellets = 6,
				Rarity = 0.5f,
				MinDepth = 1
			},
			new WeaponDef
			{
				Id = "makeshiftrifle",
				Gun = true,
				TwoHanded = true,
				Damage = 100f,
				Cooldown = 2f,
				Spread = 6f,
				PlayerMult = 0.45f,
				Mag = 10,
				Rarity = 0.6f
			},
			new WeaponDef
			{
				Id = "machete",
				Cutting = true,
				Damage = 34f,
				Cooldown = 1.1f,
				Range = 2.8f,
				Rarity = 1.2f
			},
			new WeaponDef
			{
				Id = "crudecleaver",
				Cutting = true,
				Damage = 26f,
				Cooldown = 0.9f,
				Range = 2.45f,
				Rarity = 1f
			},
			new WeaponDef
			{
				Id = "flimsyknife",
				Cutting = true,
				Damage = 18f,
				Cooldown = 0.55f,
				Range = 2.15f,
				Rarity = 1f
			},
			new WeaponDef
			{
				Id = "wrench",
				Damage = 30f,
				Cooldown = 1f,
				Range = 2.45f,
				Rarity = 1f
			},
			new WeaponDef
			{
				Id = "pickaxe",
				Damage = 36f,
				Cooldown = 1.4f,
				Range = 2.95f,
				Rarity = 0.7f
			},
			new WeaponDef
			{
				Id = "shovel",
				Damage = 28f,
				Cooldown = 1.3f,
				Range = 3.1f,
				Rarity = 0.7f
			},
			new WeaponDef
			{
				Id = "scraptube",
				Damage = 18f,
				Cooldown = 0.8f,
				Range = 2.55f,
				Rarity = 1.2f
			},
			new WeaponDef
			{
				Id = "sledgehammer",
				Damage = 50f,
				Cooldown = 2.2f,
				Range = 3.1f,
				Rarity = 0.3f,
				MinDepth = 2
			}
		};

		// Token: 0x0400017A RID: 378
		private static WorldGeneration seededWorld;

		// Token: 0x0400017B RID: 379
		private static int seededDepth = -1;

		// Token: 0x0400017C RID: 380
		private static float pendingSince;

		// Token: 0x0400017D RID: 381
		[TupleElementNames(new string[] { "ru", "en" })]
		private static readonly Dictionary<string, ValueTuple<string[], string[]>> Lines = new Dictionary<string, ValueTuple<string[], string[]>>
		{
			{
				"scav_spot",
				new ValueTuple<string[], string[]>(new string[] { "Эй! Ты!", "Стоять!", "Опа, свежее мясо.", "Ещё одна...", "Не дёргайся!", "Это моё место!" }, new string[] { "Hey! You!", "Stop right there!", "Well, fresh meat.", "Another one...", "Don't move!", "This is my spot!" })
			},
			{
				"scav_lost",
				new ValueTuple<string[], string[]>(new string[] { "Куда делась?..", "Показалось.", "Ладно, живи пока." }, new string[] { "Where'd she go?..", "Must've been nothing.", "Fine, live for now." })
			},
			{
				"scav_hurt",
				new ValueTuple<string[], string[]>(new string[] { "Ах ты дрянь!", "Сука!", "Кх... больно!", "Ну всё, тебе конец!" }, new string[] { "You little-!", "Bitch!", "Agh... that hurt!", "That's it, you're dead!" })
			},
			{
				"scav_flee",
				new ValueTuple<string[], string[]>(new string[] { "Не надо! Я ухожу!", "Хватит, хватит!", "Да пошла ты!" }, new string[] { "Stop! I'm leaving!", "Enough, enough!", "Screw you!" })
			},
			{
				"scav_reload",
				new ValueTuple<string[], string[]>(new string[] { "Перезаряжаю!", "Чёрт, пусто!" }, new string[] { "Reloading!", "Damn, empty!" })
			},
			{
				"scav_grab",
				new ValueTuple<string[], string[]>(new string[] { "Попалась!", "Иди-ка сюда!", "Лежать!", "Куда собралась?" }, new string[] { "Got you!", "Come here!", "Down!", "Where d'you think you're going?" })
			},
			{
				"scav_grab_miss",
				new ValueTuple<string[], string[]>(new string[] { "Вертлявая...", "Стой, дрянь!" }, new string[] { "Slippery one...", "Hold still!" })
			},
			{
				"scav_surrender_seen",
				new ValueTuple<string[], string[]>(new string[] { "Вот и умница.", "Руки чтобы я видел!", "Не двигайся." }, new string[] { "Good girl.", "Hands where I can see them!", "Don't move." })
			},
			{
				"scav_take_surrendered",
				new ValueTuple<string[], string[]>(new string[] { "Умная девочка.", "Вот так, тихо.", "Не дёргайся — и будешь жить." }, new string[] { "Smart girl.", "That's it, nice and quiet.", "Don't twitch and you'll live." })
			},
			{
				"scav_rob",
				new ValueTuple<string[], string[]>(new string[] { "Посмотрим, что у тебя есть...", "Это теперь моё.", "Карманы выворачивай!" }, new string[] { "Let's see what you've got...", "That's mine now.", "Empty your pockets!" })
			},
			{
				"scav_lock",
				new ValueTuple<string[], string[]>(new string[] { "Подарочек тебе. Носи и помни меня.", "Вот так. Теперь не убежишь.", "Ключ? Какой ключ?" }, new string[] { "A little gift. Wear it and think of me.", "There. Not running off now.", "Key? What key?" })
			},
			{
				"scav_strip",
				new ValueTuple<string[], string[]>(new string[] { "Снимай это.", "Шмотки тоже мои.", "Раздевайся, живо." }, new string[] { "Take that off.", "The clothes are mine too.", "Strip. Now." })
			},
			{
				"deal_offer_unlock",
				new ValueTuple<string[], string[]>(new string[] { "Эй, а ты, я смотрю, в железках. Могу снять — если будешь со мной поласковее.", "Кто ж тебя так упаковал? Освобожу. Но не бесплатно, сама понимаешь." }, new string[] { "Hey, you're all locked up, I see. I can get that off you, if you're nice to me.", "Who wrapped you up like that? I'll set you free. Not for nothing, mind." })
			},
			{
				"deal_taken_unlock",
				new ValueTuple<string[], string[]>(new string[] { "Ладно. Потом сниму с тебя эти железки.", "По рукам. Освобожу, как закончим." }, new string[] { "Fine. I'll take those things off you after.", "Deal. You'll be free when we're done." })
			},
			{
				"deal_paid_unlock",
				new ValueTuple<string[], string[]>(new string[] { "Вот, снял. Свободна... пока.", "Щёлк-щёлк. Держи свою свободу." }, new string[] { "There, off. You're free... for now.", "Click, click. There's your freedom." })
			},
			{
				"deal_offer",
				new ValueTuple<string[], string[]>(new string[] { "Эй, красотка! Есть <item>. Отдам, если будешь со мной поласковее.", "Не стреляю. Мне нужна ты, тебе — <item>. Договоримся?", "Тихо, тихо... Есть <item>. Твоё — если сделаешь мне хорошо." }, new string[] { "Hey, gorgeous! Got <item>. It's yours if you're nice to me.", "I'm not shooting. I want you, you want <item>. Deal?", "Easy, easy... Got <item>. Yours if you make me feel good." })
			},
			{
				"deal_taken",
				new ValueTuple<string[], string[]>(new string[] { "Хм... Ладно, по рукам. Получишь <item>.", "Сама напросилась. Иди сюда — и получишь <item>.", "Договорились. Будешь хорошей — <item> твой." }, new string[] { "Hm... Fine, deal. You'll get <item>.", "You asked for it. Come here and you get <item>.", "Deal. Be good and <item>'s yours." })
			},
			{
				"deal_no",
				new ValueTuple<string[], string[]>(new string[] { "Не нужна ты мне.", "Обойдусь. Пошла вон!", "Думаешь, купишь меня этим? Нет." }, new string[] { "I don't need you.", "I'll pass. Get out!", "Think that buys you off? No." })
			},
			{
				"scav_shoo",
				new ValueTuple<string[], string[]>(new string[] { "Всё, проваливай!", "Свободна. Вали отсюда.", "Пошла вон, пока я добрый." }, new string[] { "That's it, get lost!", "You're done. Beat it.", "Get out while I'm feeling nice." })
			},
			{
				"scav_shoo_threat",
				new ValueTuple<string[], string[]>(new string[] { "Я сказал — вали отсюда!", "Оглохла? Последний раз говорю: пошла!", "Ещё шаг ко мне — пожалеешь." }, new string[] { "I said get out of here!", "Deaf? Last time: go!", "One more step and you'll regret it." })
			},
			{
				"scav_shoo_beat",
				new ValueTuple<string[], string[]>(new string[] { "Ну всё, сама напросилась!", "Не понимаешь по-хорошему?", "Сейчас я тебе объясню!" }, new string[] { "Right, you asked for it!", "Won't take a hint?", "Let me spell it out for you!" })
			},
			{
				"scav_shoo_done",
				new ValueTuple<string[], string[]>(new string[] { "И чтобы я тебя больше не видел.", "Вот так-то лучше.", "Тьфу." }, new string[] { "And don't let me see you again.", "That's better.", "Tch." })
			},
			{
				"deal_accepted",
				new ValueTuple<string[], string[]>(new string[] { "Умница. Иди сюда.", "Вот и договорились.", "Не пожалеешь." }, new string[] { "Good girl. Come here.", "That's a deal.", "You won't regret it." })
			},
			{
				"scav_belted",
				new ValueTuple<string[], string[]>(new string[] { "Железные трусы? Тьфу. Ну и ходи в них.", "Заперта? Кто-то тебя уже застолбил.", "Пояс... без ключа не открыть. Повезло тебе." }, new string[] { "Iron panties? Pah. Keep them, then.", "Locked up? Someone's already claimed you.", "A belt... can't open it without a key. Lucky you." })
			},
			{
				"scav_belt_off",
				new ValueTuple<string[], string[]>(new string[] { "Это ещё что? Долой.", "Не запертый пояс? Сама виновата." }, new string[] { "What's this? Off it comes.", "An unlocked belt? Your own fault." })
			},
			{
				"scav_belt_unlock",
				new ValueTuple<string[], string[]>(new string[] { "Ключики есть у меня. Щёлк.", "Не бойся, пояс открою. Мне же нужно." }, new string[] { "I've got keys. Click.", "Don't worry, I'll open the belt. I need to, after all." })
			},
			{
				"scav_warn",
				new ValueTuple<string[], string[]>(new string[] { "Стоять!", "Ни с места!", "Вот она! Стоять, стреляю!", "Эй! Замри!" }, new string[] { "Stop!", "Don't move!", "There she is! Stop or I shoot!", "Hey! Freeze!" })
			},
			{
				"scav_mutter",
				new ValueTuple<string[], string[]>(new string[] { "Эй, красотка... где же ты прячешься...", "Скучно тут одному. Вот бы компанию...", "Есть что предложить, если что... хм.", "Девочка, выходи, не обижу..." }, new string[] { "Hey, pretty... where are you hiding...", "Lonely down here. Could use some company...", "Got something to offer, if anyone's around... hm.", "Come out, girl, I won't hurt you..." })
			},
			{
				"scav_dontmove",
				new ValueTuple<string[], string[]>(new string[] { "Не двигайся! Сдавайся!", "Стой где стоишь! Руки!", "Замри — и будешь жить!" }, new string[] { "Don't move! Give up!", "Stay where you are! Hands!", "Freeze and you live!" })
			},
			{
				"scav_ultimatum",
				new ValueTuple<string[], string[]>(new string[] { "Сдайся! Брось оружие!", "Хватит! Сдавайся — и останешься жива!", "Ты ранена. Сдайся, пока не поздно!", "Не дури! Руки вверх — и я не стреляю!" }, new string[] { "Give up! Drop your weapon!", "Enough! Give up and you'll live!", "You're hurt. Give up while you can!", "Don't be stupid! Hands up and I won't shoot!" })
			},
			{
				"scav_nomercy",
				new ValueTuple<string[], string[]>(new string[] { "Ах так?! Больше никаких поблажек!", "Не хочешь по-хорошему? Сдохни!", "Всё, девка. Разговор окончен." }, new string[] { "Like that?! No more mercy!", "Won't do it the easy way? Then die!", "That's it, girl. Talk's over." })
			},
			{
				"scav_too_late",
				new ValueTuple<string[], string[]>(new string[] { "Поздно! Надо было раньше сдаваться!", "Сдаёшься? Теперь? Нет уж.", "После того, что ты сделала? Ни за что." }, new string[] { "Too late! You should've given up before!", "Giving up? Now? No way.", "After what you did? Never." })
			},
			{
				"scav_treat",
				new ValueTuple<string[], string[]>(new string[] { "Не дёргайся. Перевяжу, а то истечёшь.", "Тихо... дай посмотрю рану.", "Подохнешь мне раньше времени — какой с тебя толк. Держи." }, new string[] { "Hold still. I'll bind that, or you'll bleed out.", "Easy... let me see that wound.", "No use to me if you bleed out. Here." })
			},
			{
				"camp_greet",
				new ValueTuple<string[], string[]>(new string[] { "Грейся, пока можно. Тут спокойно.", "Свои? Проходи к огню.", "Торговать — к нему. Я тут смотрю, чтобы никто не мешал." }, new string[] { "Warm up while you can. It's quiet here.", "Friendly? Come to the fire.", "Trading's with him. I make sure nobody bothers you." })
			},
			{
				"camp_foe",
				new ValueTuple<string[], string[]>(new string[] { "Гости! Держись за мной!", "А ну назад, тварь!", "К огню не подпущу!" }, new string[] { "Company! Stay behind me!", "Get back, you thing!", "Not near the fire, you don't!" })
			},
			{
				"camp_trader_dead",
				new ValueTuple<string[], string[]>(new string[] { "Нет! Его достали!", "Сволочи... Он был нормальным мужиком." }, new string[] { "No! They got him!", "Bastards... He was a good man." })
			},
			{
				"camp_clothes",
				new ValueTuple<string[], string[]>(new string[] { "Эй, ты же голая совсем. На, накинь хоть это.", "Замёрзнешь так. Держи, старое, но целое... почти.", "Прикройся, а? Вот, бери." }, new string[] { "Hey, you've got nothing on. Here, put this on at least.", "You'll freeze like that. Here, it's old but whole... nearly.", "Cover up, will you? Here, take it." })
			},
			{
				"camp_clothes_smirk",
				new ValueTuple<string[], string[]>(new string[] { "Нравится так ходить, да? Ну, всё равно держи — пригодится.", "Красиво, не спорю. Но на, накинь, а то охранник отвлекается." }, new string[] { "Like walking around like that, huh? Take this anyway, it'll come in handy.", "Pretty, no argument. But put this on, you're distracting the guard." })
			},
			{
				"camp_heal",
				new ValueTuple<string[], string[]>(new string[] { "Ты ж кровью истекаешь! Сядь, перевяжу. Бесплатно.", "Стой, дай гляну... Так, держись, сейчас остановим.", "Денег не надо. Не помирай только у моего костра." }, new string[] { "You're bleeding out! Sit down, I'll bind it. No charge.", "Hold on, let me look... Right, hang on, we'll stop it.", "Keep your money. Just don't die by my fire." })
			},
			{
				"camp_betrayed",
				new ValueTuple<string[], string[]>(new string[] { "Вот как ты платишь за тепло?!", "Ну всё, гостья. Сама напросилась." }, new string[] { "That's how you pay for the warmth?!", "That's it, guest. You asked for it." })
			},
			{
				"deal_paid",
				new ValueTuple<string[], string[]>(new string[] { "Держи, заслужила.", "Уговор есть уговор. Бери.", "Это твоё. Заходи ещё." }, new string[] { "Here, you earned it.", "A deal's a deal. Take it.", "That's yours. Come again." })
			},
			{
				"deal_refused",
				new ValueTuple<string[], string[]>(new string[] { "Ну и ладно. Передумаешь — свистни.", "Гордая, значит... Ну-ну." }, new string[] { "Fine. Change your mind, whistle.", "Proud, huh... We'll see." })
			},
			{
				"deal_angry",
				new ValueTuple<string[], string[]>(new string[] { "Ах так? Тогда возьму сам!", "Не хочешь по-хорошему? Будет по-плохому!" }, new string[] { "Oh, like that? Then I'll just take it!", "Won't do it the nice way? Then the hard way!" })
			},
			{
				"scav_naked_tempted",
				new ValueTuple<string[], string[]>(new string[] { "Взять с тебя нечего... ну, почти.", "Голая и пустая? Тогда расплатишься иначе.", "Карманы пусты. Зато ты — нет." }, new string[] { "Nothing to take off you... well, almost.", "Naked and empty-handed? Then you pay another way.", "Your pockets are empty. You aren't." })
			},
			{
				"scav_use",
				new ValueTuple<string[], string[]>(new string[] { "А теперь развлечёмся.", "Давно я такую не трахал.", "Задницу выше, сучка.", "Расслабься, будет не больно. Почти.", "Ну-ка, покажи, что у тебя там.", "На колени. Живо.", "Вот это я понимаю — добыча.", "Стой смирно, и всё быстро кончится." }, new string[] { "Now let's have some fun.", "Haven't had one like you in ages.", "Ass up, bitch.", "Relax, it won't hurt. Much.", "Let's see what you've got down there.", "On your knees. Now.", "Now that's what I call loot.", "Hold still and it'll be over quick." })
			},
			{
				"scav_use_talk",
				new ValueTuple<string[], string[]>(new string[]
				{
					"Вот так... хорошая...", "Тебе ведь нравится, а?", "Тесная какая...", "Не дёргайся.", "Тише, тише...", "Ох, вот это да...", "Терпи, сладкая.", "Сама виновата — нечего тут шляться.", "Ммх... давно бы так.", "Глубже берёшь, чем кажется.",
					"Смотри-ка, а ты мокрая.", "Ещё немного...", "Молчи и держись.", "Такую грех не взять."
				}, new string[]
				{
					"That's it... good girl...", "You like it, huh?", "So tight...", "Stop squirming.", "Easy, easy...", "Oh, now that's something...", "Take it, sweetheart.", "Your own fault for wandering down here.", "Mmh... should've done this sooner.", "You take more than you look like.",
					"Look at that, you're wet.", "Just a bit more...", "Shut up and hold on.", "Would be a sin not to."
				})
			},
			{
				"scav_use_talk_f",
				new ValueTuple<string[], string[]>(new string[] { "Тише, девочка... я знаю, где тебе хорошо.", "Не зажимайся. Дай мне.", "Какая ты чувствительная...", "Смотри на меня, а не в пол.", "Мм... ты вся дрожишь.", "Нравятся тебе мои пальцы, да?" }, new string[] { "Easy, girl... I know where you like it.", "Don't tense up. Let me in.", "So sensitive...", "Look at me, not the floor.", "Mm... you're shaking all over.", "You like my fingers, don't you?" })
			},
			{
				"scav_use_f",
				new ValueTuple<string[], string[]>(new string[] { "Иди сюда, красавица. Поиграем.", "Раздвинь ножки. Не бойся.", "Давно мне никто не попадался такой сладкий." }, new string[] { "Come here, pretty. Let's play.", "Spread those legs. Don't be scared.", "Haven't had anything this sweet in a while." })
			},
			{
				"scav_laugh",
				new ValueTuple<string[], string[]>(new string[] { "Ха! Кончила, шлюшка.", "Нравится, да?", "Хе-хе...", "А говорила — не надо.", "Смотри, как тебя трясёт.", "Ещё одна готова.", "Вот это я понимаю — благодарность.", "Ха, да ты сама этого хотела." }, new string[] { "Ha! You came, slut.", "Like it, huh?", "Heh-heh...", "And you said no.", "Look at you shaking.", "Another one done.", "Now that's gratitude.", "Ha, you wanted it all along." })
			},
			{
				"scav_laugh_f",
				new ValueTuple<string[], string[]>(new string[] { "Умница. Ещё разок?", "Вот так, кончай для меня.", "Какая отзывчивая." }, new string[] { "Good girl. Once more?", "That's it, come for me.", "So responsive." })
			},
			{
				"scav_climax",
				new ValueTuple<string[], string[]>(new string[] { "Ммх... держи.", "Всё в тебя.", "Ах... хорошо.", "Ххх... до капли.", "Вот тебе подарок на память.", "Ох... теперь ты моя.", "Не вздумай выплюнуть... в смысле, вытечь.", "Ааах... то, что надо." }, new string[] { "Mmh... take it.", "All of it, inside.", "Ahh... good.", "Hhh... every drop.", "Here's something to remember me by.", "Oh... now you're mine.", "Don't you dare let it drip out.", "Aaah... just what I needed." })
			},
			{
				"scav_climax_f",
				new ValueTuple<string[], string[]>(new string[] { "Ммм... я тоже... ах...", "Хорошо было... для нас обеих.", "Ах... вот теперь можно и отдохнуть." }, new string[] { "Mmm... me too... ah...", "That was good... for both of us.", "Ah... now I can rest." })
			},
			{
				"deal_use_talk",
				new ValueTuple<string[], string[]>(new string[] { "Отрабатывай, отрабатывай.", "Хочешь свою вещь — старайся.", "Вот это я понимаю — честная сделка.", "Не халтурь, а то передумаю.", "Хорошо... почти заслужила.", "Ещё чуть-чуть — и твоё.", "Видишь, без стрельбы можно договориться.", "Умная девочка. Выгодно торгуешься." }, new string[] { "Earn it, earn it.", "Want your thing? Try harder.", "Now that's an honest deal.", "Don't slack or I'll change my mind.", "Good... you've almost earned it.", "A bit more and it's yours.", "See, no need for shooting.", "Smart girl. You drive a good bargain." })
			},
			{
				"deal_use_talk_f",
				new ValueTuple<string[], string[]>(new string[] { "Старайся, и получишь, что обещано.", "Мне нравится с тобой торговаться.", "Вот так... честно и приятно." }, new string[] { "Try hard and you'll get what I promised.", "I like trading with you.", "That's it... fair and sweet." })
			},
			{
				"scav_mock",
				new ValueTuple<string[], string[]>(new string[] { "Да ты же сама насаживаешься!", "Смотри-ка, просит ещё.", "А строила из себя недотрогу.", "Слышишь, как стонешь? Шлюшка.", "Тебе это нужнее, чем мне.", "Ну и мокрая... понравилось, да?", "Вот она, правда о тебе.", "Ещё попросишь — подумаю." }, new string[] { "You're riding it yourself!", "Look at that, she wants more.", "And you played hard to get.", "Hear yourself moan? Slut.", "You need this more than I do.", "So wet... you like it, huh?", "There's the truth about you.", "Ask nicely and I'll think about it." })
			},
			{
				"scav_mock_f",
				new ValueTuple<string[], string[]>(new string[] { "Ты так сладко стонешь...", "Признайся, тебе нравится.", "Хорошая девочка, не прячь это." }, new string[] { "You moan so sweetly...", "Admit it, you like it.", "Good girl, don't hide it." })
			},
			{
				"scav_kill",
				new ValueTuple<string[], string[]>(new string[] { "Ничего личного.", "Свидетели мне не нужны.", "Спи." }, new string[] { "Nothing personal.", "I don't need witnesses.", "Sleep." })
			},
			{
				"scav_release",
				new ValueTuple<string[], string[]>(new string[] { "Проваливай.", "Свободна. Пока.", "Ещё увидимся.", "Беги, пока я добрый." }, new string[] { "Get lost.", "You're free. For now.", "See you around.", "Run, while I'm feeling kind." })
			},
			{
				"scav_foe",
				new ValueTuple<string[], string[]>(new string[] { "Заражённый! Огонь!", "Слизняк! Вали его!", "Не подпускай его!", "Эта дрянь опять здесь!", "Жги эту слизь!" }, new string[] { "Slimer! Open fire!", "Slime freak! Drop it!", "Don't let it close!", "That filth again!", "Burn the slime!" })
			},
			{
				"host_foe",
				new ValueTuple<string[], string[]>(new string[] { "Ммргх... живой...", "Иди... к нам...", "Ещё... один...", "Станешь... нами..." }, new string[] { "Mmrgh... alive...", "Come... to us...", "One... more...", "You'll be... us..." })
			},
			{
				"host_release",
				new ValueTuple<string[], string[]>(new string[] { "Скоро... вернёшься...", "Иди... мы подождём...", "Ты... уже наша..." }, new string[] { "You'll... be back...", "Go... we'll wait...", "You're... ours already..." })
			},
			{
				"host_escaped",
				new ValueTuple<string[], string[]>(new string[] { "Ммргх!..", "Не... уходи...", "Вернись... к нам..." }, new string[] { "Mmrgh!..", "Don't... leave...", "Come back... to us..." })
			},
			{
				"host_hit",
				new ValueTuple<string[], string[]>(new string[] { "Тише... тише...", "Не бейся...", "Бульк..." }, new string[] { "Hush... hush...", "Don't fight...", "Blub..." })
			},
			{
				"host_wait",
				new ValueTuple<string[], string[]>(new string[] { "...ждём...", "...скоро...", "...она наша..." }, new string[] { "...we wait...", "...soon...", "...she's ours..." })
			},
			{
				"host_lost",
				new ValueTuple<string[], string[]>(new string[] { "...где?..", "...ушла...", "...найдём..." }, new string[] { "...where?..", "...gone...", "...we'll find her..." })
			},
			{
				"host_hurt",
				new ValueTuple<string[], string[]>(new string[] { "Гхрр...", "Бульк!..", "...больно... нам..." }, new string[] { "Ghrr...", "Blub!..", "...hurts... us..." })
			},
			{
				"host_miss",
				new ValueTuple<string[], string[]>(new string[] { "...скользкая...", "Ммм..." }, new string[] { "...slippery...", "Mmm..." })
			},
			{
				"host_risen",
				new ValueTuple<string[], string[]>(new string[] { "...мы...", "Ммм... тепло...", "...нас больше..." }, new string[] { "...we...", "Mmm... warm...", "...more of us..." })
			},
			{
				"host_spot",
				new ValueTuple<string[], string[]>(new string[] { "Ммргх...", "Тёплая... живая...", "Иди... к нам..." }, new string[] { "Mmrgh...", "Warm... alive...", "Come... to us..." })
			},
			{
				"host_grab",
				new ValueTuple<string[], string[]>(new string[] { "Не бойся... это не больно...", "Будь... одной из нас...", "Прими... её..." }, new string[] { "Don't be afraid... it doesn't hurt...", "Be... one of us...", "Take... it..." })
			},
			{
				"host_talk",
				new ValueTuple<string[], string[]>(new string[] { "Глубже... в тебя...", "Мягко... тепло...", "Скоро... ты поймёшь...", "Бульк... ммм..." }, new string[] { "Deeper... into you...", "Soft... warm...", "Soon... you'll understand...", "Blub... mmm..." })
			},
			{
				"host_done",
				new ValueTuple<string[], string[]>(new string[] { "Ты... наша...", "Теперь... ты как мы..." }, new string[] { "You're... ours...", "Now... you're like us..." })
			},
			{
				"host_blocked",
				new ValueTuple<string[], string[]>(new string[] { "Занята... другим...", "Ммргх... нет места..." }, new string[] { "Taken... by another...", "Mmrgh... no room..." })
			},
			{
				"host_melt",
				new ValueTuple<string[], string[]>(new string[] { "Ах...", "Бульк..." }, new string[] { "Ahh...", "Blub..." })
			},
			{
				"scav_wait",
				new ValueTuple<string[], string[]>(new string[] { "Я следующий.", "Давай быстрее, я тоже хочу.", "Оставь и мне.", "Хе, хорошо попалась." }, new string[] { "I'm next.", "Hurry up, I want a turn.", "Save some for me.", "Heh, nice catch." })
			},
			{
				"scav_escaped",
				new ValueTuple<string[], string[]>(new string[] { "Ах ты!..", "Сбежала, дрянь!" }, new string[] { "Why you-!", "She got away!" })
			},
			{
				"scav_hit_her",
				new ValueTuple<string[], string[]>(new string[] { "Лежать, сказал!", "Ещё раз дёрнешься — убью.", "Тихо!" }, new string[] { "I said stay down!", "Twitch again and you're dead.", "Quiet!" })
			}
		};
	}
}
