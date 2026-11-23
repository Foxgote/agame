using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using LewdStatus;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x0200001E RID: 30
	internal static class Armory
	{
		// Token: 0x06000057 RID: 87 RVA: 0x0000605C File Offset: 0x0000425C
		public static ArmDef ById(string id)
		{
			if (id != null)
			{
				return Armory.All.FirstOrDefault<ArmDef>((ArmDef d) => d.Id == id);
			}
			return null;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00006096 File Offset: 0x00004296
		public static bool IsBox(Item it)
		{
			if (it)
			{
				ArmDef armDef = Armory.ById(it.id);
				return armDef != null && armDef.Kind == Kind.Box;
			}
			return false;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000060BC File Offset: 0x000042BC
		public static void Load(string dir)
		{
			if (!Directory.Exists(dir))
			{
				return;
			}
			foreach (string text in Directory.GetFiles(dir, "*.png"))
			{
				Texture2D texture2D = new Texture2D(2, 2, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text));
				texture2D.name = Path.GetFileNameWithoutExtension(text);
				Armory.Sprites[texture2D.name] = texture2D;
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00006134 File Offset: 0x00004334
		public static void Register()
		{
			if (Item.GlobalItems == null)
			{
				return;
			}
			foreach (ArmDef armDef in Armory.All)
			{
				ItemInfo itemInfo;
				if (!Item.GlobalItems.ContainsKey(armDef.Id) && Item.GlobalItems.TryGetValue(armDef.Base, out itemInfo))
				{
					ItemInfo itemInfo2 = (ItemInfo)AccessTools.Method(typeof(object), "MemberwiseClone", null, null).Invoke(itemInfo, null);
					if (armDef.Weight > 0f)
					{
						itemInfo2.weight = armDef.Weight;
					}
					if (armDef.Value >= 0)
					{
						itemInfo2.value = armDef.Value;
					}
					itemInfo2.fullName = Armory.Name(armDef);
					itemInfo2.description = Armory.Desc(armDef);
					Item.GlobalItems[armDef.Id] = itemInfo2;
				}
			}
			Armory.SyncLoot();
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00006218 File Offset: 0x00004418
		public static void SyncLoot()
		{
			if (ItemLootPool.pool == null || Item.GlobalItems == null)
			{
				return;
			}
			bool flag = Plugin.GunsEnabled.Value && Plugin.InGameLoot.Value;
			ArmDef[] all = Armory.All;
			for (int i = 0; i < all.Length; i++)
			{
				ArmDef d = all[i];
				ItemInfo itemInfo;
				if (Item.GlobalItems.TryGetValue(d.Id, out itemInfo))
				{
					List<string> list;
					if (!ItemLootPool.pool.TryGetValue(itemInfo.category, out list))
					{
						if (!flag)
						{
							goto IL_00F0;
						}
						list = (ItemLootPool.pool[itemInfo.category] = new List<string>());
					}
					bool flag2 = flag && d.Rarity > 0f;
					if (flag2 && !list.Contains(d.Id))
					{
						list.Add(d.Id);
					}
					else if (!flag2)
					{
						list.RemoveAll((string id) => id == d.Id);
					}
				}
				IL_00F0:;
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00006322 File Offset: 0x00004522
		public static string Name(ArmDef d)
		{
			if (!Plugin.English)
			{
				return d.NameRu;
			}
			return Plugin.Zh(d.NameEn);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000633D File Offset: 0x0000453D
		public static string Desc(ArmDef d)
		{
			if (!Plugin.English)
			{
				return d.DescRu;
			}
			return Plugin.Zh(d.DescEn);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00006358 File Offset: 0x00004558
		public static GameObject Template(ArmDef d)
		{
			if (d.Template)
			{
				return d.Template;
			}
			GameObject gameObject = Resources.Load(d.Base) as GameObject;
			if (!gameObject || !Armory.Sprites.ContainsKey(d.Id))
			{
				return null;
			}
			if (!Armory.holder)
			{
				Armory.holder = new GameObject("ArmoryTemplates");
				Armory.holder.SetActive(false);
				Object.DontDestroyOnLoad(Armory.holder);
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, Armory.holder.transform);
			gameObject2.name = d.Id;
			gameObject2.GetComponent<Item>().id = d.Id;
			SpriteRenderer component = gameObject2.GetComponent<SpriteRenderer>();
			Sprite sprite = (component ? component.sprite : null);
			if (component)
			{
				component.sprite = Armory.MakeSprite(d.Id, sprite) ?? component.sprite;
			}
			GunScript gunScript;
			if (gameObject2.TryGetComponent<GunScript>(ref gunScript))
			{
				Sprite sprite2 = Armory.MakeSprite(d.Id, gunScript.normalSprite) ?? gunScript.normalSprite;
				gunScript.normalSprite = sprite2;
				gunScript.rackedSprite = Armory.MakeSprite(d.Id + "racked", gunScript.rackedSprite) ?? gunScript.rackedSprite;
				gunScript.normalSpriteNoMag = Armory.MakeSprite(d.Id + "nomag", gunScript.normalSpriteNoMag) ?? (d.Variant ? gunScript.normalSpriteNoMag : sprite2);
				gunScript.rackedSpriteNoMag = Armory.MakeSprite(d.Id + "rackednomag", gunScript.rackedSpriteNoMag) ?? (d.Variant ? gunScript.rackedSpriteNoMag : gunScript.rackedSprite);
				if (!d.Variant)
				{
					if (d.Caliber >= 0)
					{
						gunScript.ammoType = d.Caliber;
					}
					gunScript.animalDamage *= d.Damage;
					gunScript.structureDamage *= d.Damage;
					gunScript.knockBack *= d.Kick;
					gunScript.loudness *= d.Loud;
					gunScript.verticalSpread *= d.Spread;
					gunScript.desiredGasTime *= d.Gas;
					gunScript.conditionLossPerShot *= d.Wear;
					if (d.Mode != null)
					{
						gunScript.firingMode = d.Mode.Value;
					}
					if (d.Feed != null)
					{
						gunScript.feedType = d.Feed.Value;
						if (gunScript.feedType == 1)
						{
							gunScript.hasMag = false;
							gunScript.roundsInMag = 0;
						}
					}
					if (d.Capacity > 0)
					{
						gunScript.magCapacity = d.Capacity;
					}
					Texture2D texture2D;
					if (gunScript.barrel && sprite && Armory.Sprites.TryGetValue(d.Id, out texture2D))
					{
						gunScript.barrel.localPosition = new Vector3(gunScript.barrel.localPosition.x * (float)texture2D.width / sprite.rect.width, gunScript.barrel.localPosition.y, gunScript.barrel.localPosition.z);
					}
				}
				Plugin.Log.LogInfo(string.Format("{0}: {1} {2} {3}, damage {4:0}/{5:0}, kick {6:0.00}, cap {7}", new object[] { d.Id, gunScript.ammoType, gunScript.feedType, gunScript.firingMode, gunScript.animalDamage, gunScript.structureDamage, gunScript.knockBack, gunScript.magCapacity }));
			}
			AmmoScript ammoScript;
			if (gameObject2.TryGetComponent<AmmoScript>(ref ammoScript))
			{
				if (d.Caliber >= 0)
				{
					ammoScript.ammoType = d.Caliber;
				}
				if (d.Kind == Kind.Box)
				{
					ammoScript.itemType = 1;
				}
				if (d.Rounds > 0)
				{
					ammoScript.maxRounds = d.Rounds;
					ammoScript.rounds = d.Rounds;
				}
			}
			d.Template = gameObject2;
			return gameObject2;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000067A8 File Offset: 0x000049A8
		private static Sprite MakeSprite(string name, Sprite like)
		{
			Texture2D texture2D;
			if (!Armory.Sprites.TryGetValue(name, out texture2D))
			{
				return null;
			}
			Vector2 vector = (like ? new Vector2(like.pivot.x / like.rect.width, like.pivot.y / like.rect.height) : new Vector2(0.5f, 0.5f));
			float num = (like ? like.pixelsPerUnit : 16f);
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), vector, num);
			sprite.name = name;
			return sprite;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x0000685C File Offset: 0x00004A5C
		public static GameObject Spawn(ArmDef d, Vector2 pos, float minCond = 0.5f, float maxCond = 1f)
		{
			GameObject gameObject = Resources.Load(d.Id) as GameObject;
			if (!gameObject)
			{
				return null;
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, pos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
			Item item;
			if (gameObject2.TryGetComponent<Item>(ref item))
			{
				item.SetCondition(Random.Range(minCond, maxCond));
			}
			AmmoScript ammoScript;
			if (gameObject2.TryGetComponent<AmmoScript>(ref ammoScript) && d.Rounds > 0)
			{
				ammoScript.rounds = Random.Range(Mathf.Max(1, d.Rounds / 3), d.Rounds + 1);
			}
			Armory.MaybeLoad(gameObject2, Plugin.LoadedChance.Value);
			return gameObject2;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000690C File Offset: 0x00004B0C
		public static void MaybeLoad(GameObject made, float chance)
		{
			GunScript gunScript;
			if (!made || !made.TryGetComponent<GunScript>(ref gunScript) || Random.value >= chance)
			{
				return;
			}
			if (gunScript.feedType == null)
			{
				GameObject gameObject = Resources.Load(AmmoScript.AmmoTypeToMagazine(gunScript.ammoType)) as GameObject;
				AmmoScript ammoScript;
				int num = ((gameObject && gameObject.TryGetComponent<AmmoScript>(ref ammoScript)) ? ammoScript.maxRounds : 10);
				gunScript.hasMag = true;
				gunScript.roundsInMag = Random.Range(Mathf.Max(1, num / 3), num + 1);
			}
			else
			{
				gunScript.roundsInMag = Random.Range(1, Mathf.Max(1, gunScript.magCapacity) + 1);
			}
			if (Random.value < 0.6f)
			{
				gunScript.roundInChamber = 0;
				gunScript.racked = false;
			}
			gunScript.safe = true;
			Item item;
			Plugin.Log.LogInfo(string.Format("{0} found loaded: {1}{2} rounds{3}", new object[]
			{
				made.TryGetComponent<Item>(ref item) ? item.id : made.name,
				gunScript.hasMag ? "mag in, " : "",
				gunScript.roundsInMag,
				(gunScript.roundInChamber == null) ? " + 1 chambered" : ""
			}));
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00006A3C File Offset: 0x00004C3C
		private static HashSet<int> CarriedCalibres()
		{
			HashSet<int> hashSet = new HashSet<int>();
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body)
			{
				foreach (GunScript gunScript in body.GetComponentsInChildren<GunScript>(true))
				{
					hashSet.Add(gunScript.ammoType);
				}
			}
			return hashSet;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00006A9C File Offset: 0x00004C9C
		public static ArmDef Pick(bool gun)
		{
			List<ArmDef> list = Armory.All.Where<ArmDef>((ArmDef d) => d.Kind == Kind.Gun == gun && d.Kind != Kind.Round).ToList<ArmDef>();
			if (!gun && Random.value < Plugin.AmmoMatchShare.Value)
			{
				HashSet<int> have = Armory.CarriedCalibres();
				List<ArmDef> list2 = list.Where<ArmDef>((ArmDef d) => have.Contains(d.Caliber)).ToList<ArmDef>();
				if (list2.Count > 0)
				{
					if (Random.value < Mathf.Clamp01(list2.Max<ArmDef>((ArmDef d) => d.Rarity)))
					{
						list = list2;
					}
				}
			}
			float num = Random.value * list.Sum<ArmDef>((ArmDef d) => d.Rarity);
			foreach (ArmDef armDef in list)
			{
				if ((num -= armDef.Rarity) <= 0f)
				{
					return armDef;
				}
			}
			return list.Last<ArmDef>();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00006BE0 File Offset: 0x00004DE0
		public static void Drop(Vector2 pos, float gunChance, float ammoChance, string where, bool fling)
		{
			if (!Plugin.GunsEnabled.Value)
			{
				return;
			}
			bool[] array = new bool[2];
			array[0] = true;
			foreach (bool flag in array)
			{
				if (Random.value <= (flag ? gunChance : ammoChance))
				{
					ArmDef armDef = Armory.Pick(flag);
					GameObject gameObject = Armory.Spawn(armDef, pos + new Vector2(Random.Range(-1f, 1f), fling ? 0f : 1.5f), flag ? 0.35f : 0.8f, flag ? 0.9f : 1f);
					if (gameObject)
					{
						Rigidbody2D rigidbody2D;
						if (fling && gameObject.TryGetComponent<Rigidbody2D>(ref rigidbody2D))
						{
							rigidbody2D.velocity = new Vector2(Random.Range(-7f, 7f), Random.Range(-7f, 7f));
						}
						Plugin.Log.LogInfo(string.Format("{0} {1} at {2}", armDef.Id, where, pos));
					}
				}
			}
		}

		// Token: 0x04000091 RID: 145
		public const GunScript.AmmoType Cal45 = 3;

		// Token: 0x04000092 RID: 146
		public const GunScript.AmmoType Cal762 = 4;

		// Token: 0x04000093 RID: 147
		public static readonly ArmDef[] All = new ArmDef[]
		{
			new ArmDef
			{
				Id = "arm_compact",
				Base = "pistol",
				Kind = Kind.Gun,
				Damage = 0.85f,
				Kick = 0.75f,
				Loud = 0.9f,
				Weight = 0.55f,
				Value = 26,
				Rarity = 1.4f,
				NameRu = "Компактный пистолет 9 мм",
				NameEn = "Compact 9mm pistol",
				DescRu = "Короткий пистолет с полимерной рамкой под 9 мм. Лёгкий, слабее отдача, чуть меньше урон. Берёт обычные пистолетные магазины.",
				DescEn = "A short polymer-framed 9mm. Light, soft recoil, a little less punch. Takes the standard pistol magazines."
			},
			new ArmDef
			{
				Id = "arm_m45",
				Base = "pistol",
				Kind = Kind.Gun,
				Caliber = 3,
				Damage = 1.35f,
				Kick = 1.35f,
				Loud = 1.15f,
				Weight = 1.05f,
				Value = 45,
				Rarity = 1f,
				NameRu = "Служебный пистолет .45",
				NameEn = ".45 service pistol",
				DescRu = "Старый стальной пистолет с ореховыми накладками под .45. Тяжёлый, бьёт сильно и ощутимо подбрасывает. Нужны магазины .45.",
				DescEn = "An old all-steel .45 with walnut grips. Heavy, hits hard, kicks hard. Needs .45 magazines."
			},
			new ArmDef
			{
				Id = "arm_revolver",
				Base = "pistol",
				Kind = Kind.Gun,
				Caliber = 3,
				Damage = 1.6f,
				Kick = 1.6f,
				Loud = 1.25f,
				Weight = 1.15f,
				Value = 50,
				Rarity = 0.8f,
				Feed = new GunScript.FeedType?(1),
				Mode = new GunScript.FiringMode?(1),
				Capacity = 6,
				Wear = 0.5f,
				NameRu = "Револьвер .45",
				NameEn = ".45 revolver",
				DescRu = "Шестизарядный револьвер под .45. Патроны заряжаются по одному прямо в барабан. Почти не клинит, бьёт сильнее всех пистолетов.",
				DescEn = "A six-shot .45 revolver. Rounds go into the cylinder one at a time. Hardly ever jams and hits harder than any pistol."
			},
			new ArmDef
			{
				Id = "arm_mp9",
				Base = "pistol",
				Kind = Kind.Gun,
				Damage = 0.8f,
				Kick = 0.7f,
				Loud = 1.1f,
				Spread = 2.4f,
				Gas = 0.55f,
				Wear = 1.3f,
				Weight = 0.95f,
				Value = 55,
				Rarity = 0.6f,
				Mode = new GunScript.FiringMode?(2),
				NameRu = "Пистолет-пулемёт 9 мм",
				NameEn = "9mm machine pistol",
				DescRu = "Автоматический пистолет под 9 мм. Высыпает магазин за секунду, разброс большой. Берёт обычные пистолетные магазины.",
				DescEn = "A full-auto 9mm. Empties a magazine in a second and sprays wide. Takes the standard pistol magazines."
			},
			new ArmDef
			{
				Id = "arm_pistol_fde",
				Base = "pistol",
				Kind = Kind.Gun,
				Variant = true,
				Rarity = 1f,
				NameRu = "Пистолет (песочный)",
				NameEn = "Pistol (desert tan)",
				DescRu = "Обычный пистолет в песочной расцветке.",
				DescEn = "The standard pistol in desert tan."
			},
			new ArmDef
			{
				Id = "arm_pistol_chrome",
				Base = "pistol",
				Kind = Kind.Gun,
				Variant = true,
				Rarity = 0.5f,
				Value = 40,
				NameRu = "Пистолет (хром)",
				NameEn = "Pistol (chrome)",
				DescRu = "Обычный пистолет, отполированный до блеска. Кто-то им очень гордился.",
				DescEn = "The standard pistol, polished to a mirror. Someone was proud of it."
			},
			new ArmDef
			{
				Id = "arm_rifle_desert",
				Base = "rifle",
				Kind = Kind.Gun,
				Variant = true,
				Rarity = 0.7f,
				NameRu = "Винтовка (песочная)",
				NameEn = "Rifle (desert tan)",
				DescRu = "Обычная винтовка в песочной расцветке.",
				DescEn = "The standard rifle in desert tan."
			},
			new ArmDef
			{
				Id = "arm_rifle_olive",
				Base = "rifle",
				Kind = Kind.Gun,
				Variant = true,
				Rarity = 0.7f,
				NameRu = "Винтовка (олива)",
				NameEn = "Rifle (olive drab)",
				DescRu = "Обычная винтовка в оливковой расцветке.",
				DescEn = "The standard rifle in olive drab."
			},
			new ArmDef
			{
				Id = "arm_shotgun_walnut",
				Base = "shotgun",
				Kind = Kind.Gun,
				Variant = true,
				Rarity = 0.7f,
				NameRu = "Дробовик (орех)",
				NameEn = "Shotgun (walnut)",
				DescRu = "Обычный дробовик с ореховым прикладом и цевьём.",
				DescEn = "The standard shotgun with a walnut stock and forend."
			},
			new ArmDef
			{
				Id = "arm_shotgun_tactical",
				Base = "shotgun",
				Kind = Kind.Gun,
				Variant = true,
				Rarity = 0.7f,
				NameRu = "Дробовик (чёрный)",
				NameEn = "Shotgun (tactical black)",
				DescRu = "Обычный дробовик, весь в чёрном.",
				DescEn = "The standard shotgun, all in black."
			},
			new ArmDef
			{
				Id = "arm_45round",
				Base = "9mmround",
				Kind = Kind.Round,
				Caliber = 3,
				Weight = 0.04f,
				Rarity = 1.2f,
				NameRu = "Патрон .45",
				NameEn = ".45 round",
				DescRu = "Толстый пистолетный патрон .45.",
				DescEn = "A fat .45 pistol round."
			},
			new ArmDef
			{
				Id = "arm_45mag",
				Base = "smallmagazine",
				Kind = Kind.Magazine,
				Caliber = 3,
				Rounds = 7,
				Value = 24,
				Rarity = 1f,
				NameRu = "Магазин .45",
				NameEn = ".45 magazine",
				DescRu = "Магазин на 7 патронов .45 для служебного пистолета.",
				DescEn = "A 7-round .45 magazine for the service pistol."
			},
			new ArmDef
			{
				Id = "arm_box9mm",
				Base = "boxof12gauge",
				Kind = Kind.Box,
				Caliber = 0,
				Rounds = 20,
				Value = 20,
				Rarity = 1.5f,
				NameRu = "Коробка патронов 9 мм",
				NameEn = "Box of 9mm",
				DescRu = "Картонная коробка на 20 патронов 9 мм. Используй, чтобы достать патрон.",
				DescEn = "A cardboard box of 20 9mm rounds. Use it to take a round out."
			},
			new ArmDef
			{
				Id = "arm_box556",
				Base = "boxof12gauge",
				Kind = Kind.Box,
				Caliber = 1,
				Rounds = 20,
				Value = 28,
				Rarity = 1f,
				NameRu = "Коробка патронов 5.56",
				NameEn = "Box of 5.56",
				DescRu = "Картонная коробка на 20 винтовочных патронов 5.56.",
				DescEn = "A cardboard box of 20 5.56 rifle rounds."
			},
			new ArmDef
			{
				Id = "arm_box45",
				Base = "boxof12gauge",
				Kind = Kind.Box,
				Caliber = 3,
				Rounds = 20,
				Value = 24,
				Rarity = 1f,
				NameRu = "Коробка патронов .45",
				NameEn = "Box of .45",
				DescRu = "Картонная коробка на 20 патронов .45.",
				DescEn = "A cardboard box of 20 .45 rounds."
			},
			new ArmDef
			{
				Id = "arm_mosin",
				Base = "rifle",
				Kind = Kind.Gun,
				Caliber = 4,
				Damage = 2.1f,
				Kick = 1.7f,
				Loud = 1.5f,
				Spread = 0.35f,
				Gas = 3f,
				Wear = 0.4f,
				Weight = 4f,
				Value = 70,
				Rarity = 0f,
				Feed = new GunScript.FeedType?(1),
				Mode = new GunScript.FiringMode?(1),
				Capacity = 5,
				NameRu = "Трёхлинейка",
				NameEn = "Three-line rifle",
				DescRu = "Длинная винтовка со скользящим затвором, старше самой капсулы. Пять винтовочных патронов заряжаются по одному, после каждого выстрела — затвор. Медленная, тяжёлая, бьёт насквозь. Кто-то повесил её на стену капсулы — на удачу.",
				DescEn = "A long bolt-action rifle, older than the pod itself. Five rifle rounds go in one at a time, the bolt after every shot. Slow, heavy, it hits right through. Someone hung it on the pod's wall, for luck."
			},
			new ArmDef
			{
				Id = "arm_762round",
				Base = "556round",
				Kind = Kind.Round,
				Caliber = 4,
				Weight = 0.03f,
				Rarity = 0.6f,
				NameRu = "Винтовочный патрон",
				NameEn = "Rifle round",
				DescRu = "Длинный винтовочный патрон в фиолетовом лаке. Только для трёхлинейки.",
				DescEn = "A long rifle round in purple lacquer. For the three-line rifle only."
			},
			new ArmDef
			{
				Id = "arm_box762",
				Base = "boxof12gauge",
				Kind = Kind.Box,
				Caliber = 4,
				Rounds = 20,
				Value = 32,
				Rarity = 0.4f,
				NameRu = "Коробка винтовочных патронов",
				NameEn = "Box of rifle rounds",
				DescRu = "Фиолетовая коробка на 20 винтовочных патронов для трёхлинейки. Используй, чтобы достать патрон.",
				DescEn = "A purple box of 20 rifle rounds for the three-line rifle. Use it to take a round out."
			}
		};

		// Token: 0x04000094 RID: 148
		public static readonly Dictionary<string, Texture2D> Sprites = new Dictionary<string, Texture2D>();

		// Token: 0x04000095 RID: 149
		private static GameObject holder;
	}
}
