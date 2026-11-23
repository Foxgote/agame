using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000C5 RID: 197
	internal static class Outfits
	{
		// Token: 0x0600044D RID: 1101 RVA: 0x000453B4 File Offset: 0x000435B4
		private static OutfitDef Tac(string camo)
		{
			return new OutfitDef
			{
				Id = "lewd_tac_" + camo,
				TexturePrefix = "tac_" + camo + "_",
				NameKey = "tacsuit",
				ColorKey = camo,
				Folder = "tacsuit",
				Armor = 0.12f,
				Isolation = 0.3f,
				Weight = 0.8f,
				Value = 45,
				Rarity = 0.15f,
				CorpseRarity = 0.05f,
				MinDepth = 1,
				Pouches = 4
			};
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x00045458 File Offset: 0x00043658
		private static OutfitDef Ears(string col)
		{
			return new OutfitDef
			{
				Id = "lewd_headphones_" + col,
				TexturePrefix = "hp_" + col + "_",
				NameKey = "headphones",
				ColorKey = col,
				Folder = "headphones",
				Slot = "hat",
				MainLimb = "Head",
				MainTexture = "experimentHead",
				VisualOffset = 9,
				Armor = 0f,
				Isolation = 0.04f,
				Weight = 0.25f,
				Value = 30,
				Rarity = 0.2f,
				CorpseRarity = 0.04f,
				EarPro = true
			};
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x00045520 File Offset: 0x00043720
		private static OutfitDef Heels(string col)
		{
			return new OutfitDef
			{
				Id = "lewd_thighheels_" + col,
				TexturePrefix = "heels_" + col + "_",
				NameKey = "thighheels",
				ColorKey = col,
				Folder = "thighheels",
				Slot = "feet",
				MainLimb = "CrusF",
				MainTexture = "experimentCrus",
				VisualOffset = 4,
				Shoes = "heels_stiletto",
				Armor = 0.03f,
				Isolation = 0.12f,
				Weight = 0.6f,
				Value = 26,
				Lingerie = true,
				Rarity = 0.16f,
				CorpseRarity = 0.04f,
				InLoot = true
			};
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x000455F8 File Offset: 0x000437F8
		public static OutfitDef StartSuit()
		{
			if (Outfits.suitRng.NextDouble() >= (double)Plugin.StartSuitColorChance.Value)
			{
				return Outfits.Spacesuit;
			}
			OutfitDef[] array = Outfits.Spacesuits.Where<OutfitDef>((OutfitDef d) => d != Outfits.Spacesuit && d.Textures.Count > 0).ToArray<OutfitDef>();
			if (array.Length == 0)
			{
				return Outfits.Spacesuit;
			}
			return array[Outfits.suitRng.Next(array.Length)];
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x0004566C File Offset: 0x0004386C
		public static OutfitDef ById(string id)
		{
			return Outfits.All.FirstOrDefault<OutfitDef>((OutfitDef d) => d.Id == id);
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x0004569C File Offset: 0x0004389C
		public static void Load(string root)
		{
			foreach (OutfitDef outfitDef in Outfits.All)
			{
				string text = Path.Combine(root, outfitDef.Folder ?? outfitDef.NameKey);
				if (Directory.Exists(text))
				{
					foreach (string text2 in Directory.GetFiles(text, outfitDef.TexturePrefix + "*.png"))
					{
						Texture2D texture2D = new Texture2D(2, 2, 4, false)
						{
							filterMode = 0,
							wrapMode = 1
						};
						ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text2));
						texture2D.name = Path.GetFileNameWithoutExtension(text2);
						outfitDef.Textures[texture2D.name.Substring(outfitDef.TexturePrefix.Length)] = texture2D;
					}
				}
			}
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x00045778 File Offset: 0x00043978
		public static void Register()
		{
			ItemInfo itemInfo;
			if (Item.GlobalItems == null || !Item.GlobalItems.TryGetValue("hoodie", out itemInfo))
			{
				return;
			}
			foreach (OutfitDef outfitDef in Outfits.All)
			{
				if (!Item.GlobalItems.ContainsKey(outfitDef.Id))
				{
					ItemInfo itemInfo2 = (ItemInfo)AccessTools.Method(typeof(object), "MemberwiseClone", null, null).Invoke(itemInfo, null);
					itemInfo2.desiredWearLimb = outfitDef.MainLimb;
					itemInfo2.wearSlotId = outfitDef.Slot;
					itemInfo2.wearableArmor = outfitDef.Armor;
					itemInfo2.wearableIsolation = outfitDef.Isolation;
					itemInfo2.wearableHitDurabilityLossMultiplier = 0.15f;
					itemInfo2.wearableVisualOffset = outfitDef.VisualOffset;
					itemInfo2.wearableCanBeHeld = outfitDef.CanBeHeld;
					itemInfo2.weight = outfitDef.Weight;
					itemInfo2.decayMinutes = 0f;
					itemInfo2.rotSpeed = 0f;
					itemInfo2.qualities = new List<CraftingQuality>();
					itemInfo2.value = outfitDef.Value;
					itemInfo2.fullName = outfitDef.Name;
					itemInfo2.description = outfitDef.Desc;
					if (outfitDef.NotWorn)
					{
						itemInfo2.wearable = false;
						itemInfo2.usable = false;
						itemInfo2.wearSlotId = null;
						itemInfo2.wearableIsolation = 0f;
						itemInfo2.wearableArmor = 0f;
					}
					if (outfitDef == Devices.Key)
					{
						itemInfo2.usable = true;
						ItemInfo itemInfo3 = itemInfo2;
						ItemInfo.Use use;
						if ((use = Outfits.<>O.<0>__UseKey) == null)
						{
							use = (Outfits.<>O.<0>__UseKey = new ItemInfo.Use(Devices.UseKey));
						}
						itemInfo3.useAction = use;
					}
					Item.GlobalItems[outfitDef.Id] = itemInfo2;
				}
			}
			Outfits.AddToLoot();
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0004592C File Offset: 0x00043B2C
		private static void AddToLoot()
		{
			if (ItemLootPool.pool == null || !Plugin.OutfitsInLoot.Value)
			{
				return;
			}
			List<string> list;
			if (!ItemLootPool.pool.TryGetValue("utility", out list))
			{
				list = (ItemLootPool.pool["utility"] = new List<string>());
			}
			foreach (OutfitDef outfitDef in Outfits.All)
			{
				if (outfitDef.InLoot && !list.Contains(outfitDef.Id))
				{
					list.Add(outfitDef.Id);
				}
			}
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x000459B0 File Offset: 0x00043BB0
		public static bool EarProOn(Body b)
		{
			if (b)
			{
				return b.GetAllWearables().Any<Item>(delegate(Item w)
				{
					if (w)
					{
						OutfitDef outfitDef = Outfits.ById(w.id);
						return outfitDef != null && outfitDef.EarPro;
					}
					return false;
				});
			}
			return false;
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x000459E8 File Offset: 0x00043BE8
		public static GameObject Template(OutfitDef d)
		{
			if (d.Template)
			{
				return d.Template;
			}
			GameObject gameObject = Resources.Load("hoodie") as GameObject;
			if (!gameObject || d.Textures.Count == 0)
			{
				return null;
			}
			if (!Outfits.holder)
			{
				Outfits.holder = new GameObject("LewdItemTemplates");
				Outfits.holder.SetActive(false);
				Object.DontDestroyOnLoad(Outfits.holder);
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, Outfits.holder.transform);
			gameObject2.name = d.Id;
			gameObject2.GetComponent<Item>().id = d.Id;
			Container[] componentsInChildren = gameObject2.GetComponentsInChildren<Container>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (i == 0 && d.Pouches > 0)
				{
					componentsInChildren[i].tagRestriction = new string[] { "lewd_magazine" };
					componentsInChildren[i].maxWeightPerItem = 0.6f;
					componentsInChildren[i].maxWeight = 0.6f * (float)d.Pouches;
				}
				else
				{
					Object.DestroyImmediate(componentsInChildren[i]);
				}
			}
			SpriteRenderer component = gameObject2.GetComponent<SpriteRenderer>();
			component.sprite = Outfits.MakeSprite(d, d.MainTexture) ?? component.sprite;
			if (d.NotWorn)
			{
				Wearable component2 = gameObject2.GetComponent<Wearable>();
				if (component2)
				{
					Object.DestroyImmediate(component2);
				}
				d.Template = gameObject2;
				Plugin.Log.LogInfo("Item template " + d.Id + " built");
				return gameObject2;
			}
			Body b = (PlayerCamera.main ? PlayerCamera.main.body : null);
			List<string> list = Outfits.Parts.Keys.Where<string>((string l) => l != d.MainLimb && (b == null || b.LimbByName(l) != null) && d.Textures.ContainsKey(Outfits.Parts[l])).ToList<string>();
			Wearable wearable = gameObject2.GetComponent<Wearable>() ?? gameObject2.AddComponent<Wearable>();
			wearable.secondaryLimbs = list.ToArray();
			wearable.secondaryLimbSprites = list.Select<string, Sprite>((string l) => Outfits.MakeSprite(d, Outfits.Parts[l])).ToArray<Sprite>();
			if (d.IsSpacesuit && Plugin.SuitLight.Value > 0f && component.sprite)
			{
				SuitLed.Add(gameObject2.transform, component.sprite, Outfits.SuitLightColor(d));
			}
			d.Template = gameObject2;
			Plugin.Log.LogInfo(string.Format("Outfit template {0} built: torso + {1} limbs", d.Id, list.Count));
			return gameObject2;
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00045CB0 File Offset: 0x00043EB0
		private static Color SuitLightColor(OutfitDef d)
		{
			string text = d.Id.Replace("lewd_spacesuit", "").TrimStart(new char[] { '_' });
			if (text != null)
			{
				switch (text.Length)
				{
				case 4:
				{
					char c = text[0];
					if (c != 'b')
					{
						if (c == 'p')
						{
							if (text == "pink")
							{
								return new Color32(byte.MaxValue, 120, 200, byte.MaxValue);
							}
						}
					}
					else if (text == "blue")
					{
						return new Color32(90, 150, byte.MaxValue, byte.MaxValue);
					}
					break;
				}
				case 5:
				{
					char c = text[0];
					if (c != 'b')
					{
						if (c == 'g')
						{
							if (text == "green")
							{
								return new Color32(130, byte.MaxValue, 120, byte.MaxValue);
							}
						}
					}
					else if (text == "black")
					{
						return new Color32(110, byte.MaxValue, 140, byte.MaxValue);
					}
					break;
				}
				case 6:
				{
					char c = text[0];
					if (c != 'o')
					{
						if (c == 'y')
						{
							if (text == "yellow")
							{
								return new Color32(byte.MaxValue, 228, 90, byte.MaxValue);
							}
						}
					}
					else if (text == "orange")
					{
						return new Color32(byte.MaxValue, 160, 64, byte.MaxValue);
					}
					break;
				}
				case 7:
					if (text == "redblue")
					{
						return new Color32(byte.MaxValue, 96, 96, byte.MaxValue);
					}
					break;
				}
			}
			return new Color32(200, 230, byte.MaxValue, byte.MaxValue);
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x00045EB8 File Offset: 0x000440B8
		private static float BodyPpu()
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer = ((body && body.limbs != null && body.limbs.Length > 1) ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer && spriteRenderer.sprite)
			{
				return spriteRenderer.sprite.pixelsPerUnit;
			}
			Sprite sprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault<Sprite>((Sprite s) => s && s.name.StartsWith("experiment") && s.rect.width < 64f);
			if (!sprite)
			{
				return 8f;
			}
			return sprite.pixelsPerUnit;
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x00045F6C File Offset: 0x0004416C
		private static Sprite MakeSprite(OutfitDef d, string textureName)
		{
			Texture2D t;
			if (!d.Textures.TryGetValue(textureName, out t))
			{
				return null;
			}
			List<Sprite> list = (from s in Resources.FindObjectsOfTypeAll<Sprite>()
				where s && s.name == textureName
				select s).ToList<Sprite>();
			Sprite sprite = list.FirstOrDefault<Sprite>((Sprite s) => Mathf.RoundToInt(s.rect.width) == t.width && Mathf.RoundToInt(s.rect.height) == t.height) ?? list.FirstOrDefault<Sprite>();
			Vector2 vector = (sprite ? new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height) : new Vector2(0.5f, 0.5f));
			if (sprite && Mathf.RoundToInt(sprite.rect.width) == t.width && t.height > Mathf.RoundToInt(sprite.rect.height))
			{
				vector..ctor(sprite.pivot.x / (float)t.width, sprite.pivot.y / (float)t.height);
			}
			float num = (sprite ? sprite.pixelsPerUnit : Outfits.BodyPpu());
			if (!sprite)
			{
				Plugin.Log.LogWarning(string.Format("Outfit {0}: no game sprite named {1}; its scale is taken from her body ({2:0.#} px a unit)", d.Id, textureName, num));
			}
			Sprite sprite2 = Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), vector, num);
			sprite2.name = d.TexturePrefix + textureName;
			return sprite2;
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x00046144 File Offset: 0x00044344
		public static GameObject SpawnAt(OutfitDef d, Vector2 pos)
		{
			GameObject gameObject = Resources.Load(d.Id) as GameObject;
			if (!gameObject)
			{
				return null;
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, pos, Quaternion.identity);
			Item item;
			if (d.MaxCondition < 1f && gameObject2.TryGetComponent<Item>(ref item))
			{
				item.SetCondition(Random.Range(d.MinCondition, d.MaxCondition));
			}
			return gameObject2;
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x000461B0 File Offset: 0x000443B0
		public static void SeedLayer(Body b, bool newRun)
		{
			WorldGeneration world = WorldGeneration.world;
			int depth = (world ? world.biomeDepth : 0);
			if (newRun && Plugin.StartWithBodysuit.Value)
			{
				OutfitDef outfitDef = Outfits.StartSuit();
				GameObject gameObject = Outfits.SpawnAt(outfitDef, b.transform.position + new Vector2(b.isRight ? 1.2f : (-1.2f), 0.8f));
				Plugin.Log.LogInfo(string.Format("Start spacesuit {0}: {1}, she is at {2}", outfitDef.Id, gameObject ? ("placed at " + gameObject.transform.position.ToString()) : "NOT made", b.transform.position));
			}
			List<OutfitDef> list = Outfits.All.Where<OutfitDef>((OutfitDef d) => d.MinDepth <= depth && d.Textures.Count > 0).ToList<OutfitDef>();
			float value = Plugin.OutfitsPerLayer.Value;
			int num = Mathf.FloorToInt(value) + ((Random.value < value - Mathf.Floor(value)) ? 1 : 0);
			List<string> list2 = new List<string>();
			for (int i = 0; i < num; i++)
			{
				OutfitDef outfitDef2 = Outfits.Pick(list, (OutfitDef x) => x.Rarity, list2);
				Vector2? vector = ((outfitDef2 != null) ? Spots.RandomFloor(25f, b.transform.position) : null);
				if (vector != null)
				{
					Outfits.SpawnAt(outfitDef2, vector.Value);
					list2.Add(outfitDef2.Id);
					Plugin.Log.LogInfo(string.Format("A {0} lies at {1}", outfitDef2.Id, vector.Value));
				}
			}
			ManualLogSource log = Plugin.Log;
			string text = "Layer {0}: {1} outfits lying around ({2}); {3} pieces in the game loot";
			object[] array = new object[4];
			array[0] = depth;
			array[1] = list2.Count;
			array[2] = string.Join(", ", list2);
			int num2 = 3;
			List<string> list3;
			int num3;
			if (ItemLootPool.pool == null || !ItemLootPool.pool.TryGetValue("utility", out list3))
			{
				num3 = 0;
			}
			else
			{
				num3 = list3.Count<string>((string id) => Outfits.ById(id) != null);
			}
			array[num2] = num3;
			log.LogInfo(string.Format(text, array));
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x00046428 File Offset: 0x00044628
		public static void DressCorpse(CorpseScript c)
		{
			if (!c.animalCorpse)
			{
				Potion.MaybeDrop(c.transform.position, Plugin.PotionCorpseChance.Value, "by a corpse");
			}
			if (c.animalCorpse || Random.value > Plugin.OutfitCorpseChance.Value)
			{
				return;
			}
			int depth = (WorldGeneration.world ? WorldGeneration.world.biomeDepth : 0);
			OutfitDef outfitDef = Outfits.Pick(Outfits.All.Where<OutfitDef>((OutfitDef x) => x.MinDepth <= depth && x.Textures.Count > 0).ToList<OutfitDef>(), (OutfitDef x) => x.CorpseRarity, null);
			GameObject gameObject = ((outfitDef != null) ? Outfits.SpawnAt(outfitDef, c.transform.position + new Vector2(Random.Range(-1.5f, 1.5f), 2f)) : null);
			Item item;
			if (!gameObject || !gameObject.TryGetComponent<Item>(ref item))
			{
				return;
			}
			item.SetCondition(Mathf.Min(item.condition, Random.Range(0.2f, 0.7f)));
			SpriteRenderer spriteRenderer;
			SpriteRenderer spriteRenderer2;
			if (gameObject.TryGetComponent<SpriteRenderer>(ref spriteRenderer) && c.TryGetComponent<SpriteRenderer>(ref spriteRenderer2))
			{
				spriteRenderer.sortingOrder = spriteRenderer2.sortingOrder + 1;
			}
			Plugin.Log.LogInfo(string.Format("A {0} lies by a corpse at {1}", outfitDef.Id, c.transform.position));
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x0004659D File Offset: 0x0004479D
		public static bool IsLootCrate(BuildingEntity e)
		{
			return e && !e.animal && e.guaranteedDropAmount > 0 && !e.GetComponent<CorpseScript>();
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x000465C8 File Offset: 0x000447C8
		public static void FillCrate(BuildingEntity e)
		{
			Potion.MaybeDrop(e.transform.position, Plugin.PotionCrateChance.Value, "in a crate");
			Toys.MaybeDrop(e);
			Devices.FillCrate(e);
			string text = (e.id ?? e.name).ToLowerInvariant();
			Counter.FillCrate(e, text);
			float num = ((text.Contains("containercrate") || text.Contains("cargo")) ? 1.75f : (text.Contains("medcrate") ? 0.6f : (text.Contains("food") ? 0.3f : 1f)));
			if (Random.value > Plugin.OutfitCrateChance.Value * num)
			{
				Plugin.Log.LogInfo("Crate " + text + " broke: nothing of ours in it");
				return;
			}
			int depth = (WorldGeneration.world ? WorldGeneration.world.biomeDepth : 0);
			OutfitDef outfitDef = Outfits.Pick(Outfits.All.Where<OutfitDef>((OutfitDef x) => x.MinDepth <= depth && x.Textures.Count > 0).ToList<OutfitDef>(), (OutfitDef x) => x.Rarity, null);
			GameObject gameObject = ((outfitDef != null) ? Outfits.SpawnAt(outfitDef, e.transform.position) : null);
			if (!gameObject)
			{
				return;
			}
			Rigidbody2D rigidbody2D;
			if (gameObject.TryGetComponent<Rigidbody2D>(ref rigidbody2D))
			{
				rigidbody2D.velocity = new Vector2(Random.Range(-7f, 7f), Random.Range(-7f, 7f));
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && Vector2.Distance(body.transform.position, e.transform.position) < 8f)
			{
				gameObject.AddComponent<FreshItemDrop>();
			}
			Plugin.Log.LogInfo(string.Concat(new string[] { "Crate ", text, " broke: a ", outfitDef.Id, " was in it" }));
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x000467F8 File Offset: 0x000449F8
		private static OutfitDef Pick(List<OutfitDef> from, Func<OutfitDef, float> weight, List<string> already)
		{
			Outfits.<>c__DisplayClass68_0 CS$<>8__locals1 = new Outfits.<>c__DisplayClass68_0();
			CS$<>8__locals1.weight = weight;
			CS$<>8__locals1.already = already;
			float num = from.Sum<OutfitDef>(new Func<OutfitDef, float>(CS$<>8__locals1.<Pick>g__W|0));
			if (num <= 0f)
			{
				return null;
			}
			float num2 = Random.value * num;
			foreach (OutfitDef outfitDef in from)
			{
				if ((num2 -= CS$<>8__locals1.<Pick>g__W|0(outfitDef)) <= 0f)
				{
					return outfitDef;
				}
			}
			return from.Last<OutfitDef>();
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0004689C File Offset: 0x00044A9C
		public static OutfitDef Worn(Body b)
		{
			return Outfits.WornIn(b, "bodysuit");
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x000468AC File Offset: 0x00044AAC
		public static OutfitDef WornIn(Body b, string slot)
		{
			if (!b)
			{
				return null;
			}
			Item wearableBySlotID = b.GetWearableBySlotID(slot);
			if (!wearableBySlotID)
			{
				return null;
			}
			return Outfits.ById(wearableBySlotID.id);
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x000468E0 File Offset: 0x00044AE0
		public static IEnumerable<string> AllSlots
		{
			get
			{
				return Outfits.Slots;
			}
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x000468E8 File Offset: 0x00044AE8
		public static string TextureFor(string limb)
		{
			string text;
			if (!Outfits.Parts.TryGetValue(limb, out text))
			{
				return null;
			}
			return text;
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00046908 File Offset: 0x00044B08
		public static Sprite TornSprite(OutfitDef d, string textureName, int level)
		{
			if (textureName == null)
			{
				return null;
			}
			string text = string.Concat(new string[]
			{
				d.Id,
				"|",
				textureName,
				"|",
				level.ToString()
			});
			Sprite sprite;
			if (Outfits.torn.TryGetValue(text, out sprite))
			{
				return sprite;
			}
			Sprite sprite2 = Outfits.MakeSprite(d, textureName);
			if (sprite2 && level > 0)
			{
				Texture2D texture2D = d.Textures[textureName];
				Texture2D texture2D2 = new Texture2D(texture2D.width, texture2D.height, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				Color32[] pixels = texture2D.GetPixels32();
				bool[] array = new bool[pixels.Length];
				float num = ((level == 1) ? 0.13f : 0.32f);
				int hashCode = (d.Id + textureName).GetHashCode();
				for (int i = 0; i < texture2D.height; i++)
				{
					for (int j = 0; j < texture2D.width; j++)
					{
						int num2 = i * texture2D.width + j;
						if (pixels[num2].a != 0)
						{
							int num3 = (j / 2 * 73856093) ^ (i / 2 * 19349663) ^ hashCode;
							int num4 = (num3 ^ (int)((uint)num3 >> 13)) * 1540483477;
							if ((num4 ^ (int)((uint)num4 >> 15)) % 1000 / 1000f < num)
							{
								array[num2] = true;
							}
						}
					}
				}
				for (int k = 0; k < texture2D.height; k++)
				{
					for (int l = 0; l < texture2D.width; l++)
					{
						int num5 = k * texture2D.width + l;
						if (array[num5])
						{
							pixels[num5] = new Color32(0, 0, 0, 0);
						}
						else if (pixels[num5].a != 0 && ((l > 0 && array[num5 - 1]) || (l < texture2D.width - 1 && array[num5 + 1]) || (k > 0 && array[num5 - texture2D.width]) || (k < texture2D.height - 1 && array[num5 + texture2D.width])))
						{
							pixels[num5] = new Color32((byte)((float)pixels[num5].r * 0.7f), (byte)((float)pixels[num5].g * 0.7f), (byte)((float)pixels[num5].b * 0.7f), pixels[num5].a);
						}
					}
				}
				texture2D2.SetPixels32(pixels);
				texture2D2.Apply();
				texture2D2.name = texture2D.name;
				sprite2 = Sprite.Create(texture2D2, sprite2.rect, new Vector2(sprite2.pivot.x / sprite2.rect.width, sprite2.pivot.y / sprite2.rect.height), sprite2.pixelsPerUnit);
				sprite2.name = d.TexturePrefix + textureName + "_torn" + level.ToString();
			}
			Outfits.torn[text] = sprite2;
			return sprite2;
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x00046C2B File Offset: 0x00044E2B
		public static IEnumerable<OutfitDef> WornAll(Body b)
		{
			foreach (string text in Outfits.Slots)
			{
				OutfitDef outfitDef = Outfits.WornIn(b, text);
				if (outfitDef != null)
				{
					yield return outfitDef;
				}
			}
			string[] array = null;
			yield break;
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x00046C3C File Offset: 0x00044E3C
		public static bool Covers(OutfitDef d, Limb l)
		{
			string text;
			return l.name == d.MainLimb || (Outfits.Parts.TryGetValue(l.name, out text) && d.Textures.ContainsKey(text));
		}

		// Token: 0x04000401 RID: 1025
		public const string Slot = "bodysuit";

		// Token: 0x04000402 RID: 1026
		public static readonly OutfitDef Spacesuit = new OutfitDef
		{
			Id = "lewd_spacesuit",
			TexturePrefix = "bodysuit_",
			NameKey = "bodysuit",
			IsSpacesuit = true,
			Armor = 0.3f,
			Isolation = 0.5f,
			Weight = 1.1f,
			Value = 40,
			Rarity = 0.5f
		};

		// Token: 0x04000403 RID: 1027
		public static readonly OutfitDef Spacesuit_redblue = new OutfitDef
		{
			Id = "lewd_spacesuit_redblue",
			TexturePrefix = "bodysuit_redblue_",
			NameKey = "bodysuit_redblue",
			IsSpacesuit = true,
			Armor = 0.3f,
			Isolation = 0.5f,
			Weight = 1.1f,
			Value = 40,
			Rarity = 0.06f
		};

		// Token: 0x04000404 RID: 1028
		public static readonly OutfitDef Spacesuit_orange = new OutfitDef
		{
			Id = "lewd_spacesuit_orange",
			TexturePrefix = "bodysuit_orange_",
			NameKey = "bodysuit_orange",
			IsSpacesuit = true,
			Armor = 0.3f,
			Isolation = 0.5f,
			Weight = 1.1f,
			Value = 40,
			Rarity = 0.06f
		};

		// Token: 0x04000405 RID: 1029
		public static readonly OutfitDef Spacesuit_black = new OutfitDef
		{
			Id = "lewd_spacesuit_black",
			TexturePrefix = "bodysuit_black_",
			NameKey = "bodysuit_black",
			IsSpacesuit = true,
			Armor = 0.3f,
			Isolation = 0.5f,
			Weight = 1.1f,
			Value = 40,
			Rarity = 0.06f
		};

		// Token: 0x04000406 RID: 1030
		public static readonly OutfitDef Spacesuit_blue = new OutfitDef
		{
			Id = "lewd_spacesuit_blue",
			TexturePrefix = "bodysuit_blue_",
			NameKey = "bodysuit_blue",
			IsSpacesuit = true,
			Armor = 0.3f,
			Isolation = 0.5f,
			Weight = 1.1f,
			Value = 40,
			Rarity = 0.06f
		};

		// Token: 0x04000407 RID: 1031
		public static readonly OutfitDef Spacesuit_pink = new OutfitDef
		{
			Id = "lewd_spacesuit_pink",
			TexturePrefix = "bodysuit_pink_",
			NameKey = "bodysuit_pink",
			IsSpacesuit = true,
			Armor = 0.3f,
			Isolation = 0.5f,
			Weight = 1.1f,
			Value = 40,
			Rarity = 0.06f
		};

		// Token: 0x04000408 RID: 1032
		public static readonly OutfitDef Spacesuit_green = new OutfitDef
		{
			Id = "lewd_spacesuit_green",
			TexturePrefix = "bodysuit_green_",
			NameKey = "bodysuit_green",
			IsSpacesuit = true,
			Armor = 0.3f,
			Isolation = 0.5f,
			Weight = 1.1f,
			Value = 40,
			Rarity = 0.06f
		};

		// Token: 0x04000409 RID: 1033
		public static readonly OutfitDef Spacesuit_yellow = new OutfitDef
		{
			Id = "lewd_spacesuit_yellow",
			TexturePrefix = "bodysuit_yellow_",
			NameKey = "bodysuit_yellow",
			IsSpacesuit = true,
			Armor = 0.3f,
			Isolation = 0.5f,
			Weight = 1.1f,
			Value = 40,
			Rarity = 0.06f
		};

		// Token: 0x0400040A RID: 1034
		public static readonly OutfitDef Latex = new OutfitDef
		{
			Id = "lewd_latexsuit",
			TexturePrefix = "latex_",
			NameKey = "latexsuit",
			Armor = 0.05f,
			Isolation = 0.2f,
			Weight = 0.8f,
			Value = 30,
			Rarity = 0.5f,
			MinDepth = 1,
			IsLatex = true
		};

		// Token: 0x0400040B RID: 1035
		public static readonly OutfitDef LatexRed = new OutfitDef
		{
			Id = "lewd_latexred",
			TexturePrefix = "latexred_",
			NameKey = "latexred",
			Armor = 0.05f,
			Isolation = 0.2f,
			Weight = 0.8f,
			Value = 34,
			Rarity = 0.3f,
			MinDepth = 2,
			IsLatex = true
		};

		// Token: 0x0400040C RID: 1036
		public static readonly OutfitDef BunnySuit = new OutfitDef
		{
			Id = "lewd_bunnysuit",
			TexturePrefix = "bunny_",
			NameKey = "bunnysuit",
			Shoes = "heels_stiletto",
			Armor = 0.02f,
			Isolation = 0.12f,
			Weight = 0.3f,
			Value = 22,
			Lingerie = true,
			Rarity = 0.35f,
			MinDepth = 1
		};

		// Token: 0x0400040D RID: 1037
		public static readonly OutfitDef BunnySuitPink = new OutfitDef
		{
			Id = "lewd_bunnysuitpink",
			TexturePrefix = "bunnypink_",
			NameKey = "bunnysuitpink",
			Shoes = "heels_stiletto",
			Armor = 0.02f,
			Isolation = 0.12f,
			Weight = 0.3f,
			Value = 22,
			Lingerie = true,
			Rarity = 0.25f,
			MinDepth = 1
		};

		// Token: 0x0400040E RID: 1038
		public static readonly OutfitDef TacWoodland = Outfits.Tac("woodland");

		// Token: 0x0400040F RID: 1039
		public static readonly OutfitDef TacDesert = Outfits.Tac("desert");

		// Token: 0x04000410 RID: 1040
		public static readonly OutfitDef TacUrban = Outfits.Tac("urban");

		// Token: 0x04000411 RID: 1041
		public static readonly OutfitDef TacWinter = Outfits.Tac("winter");

		// Token: 0x04000412 RID: 1042
		public static readonly OutfitDef HeadphonesBlack = Outfits.Ears("black");

		// Token: 0x04000413 RID: 1043
		public static readonly OutfitDef HeadphonesOlive = Outfits.Ears("olive");

		// Token: 0x04000414 RID: 1044
		public static readonly OutfitDef HeelsBlack = Outfits.Heels("black");

		// Token: 0x04000415 RID: 1045
		public static readonly OutfitDef HeelsRed = Outfits.Heels("red");

		// Token: 0x04000416 RID: 1046
		public static readonly OutfitDef HeelsWhite = Outfits.Heels("white");

		// Token: 0x04000417 RID: 1047
		public static readonly OutfitDef HeelsPink = Outfits.Heels("pink");

		// Token: 0x04000418 RID: 1048
		public static readonly OutfitDef HeelsPurple = Outfits.Heels("purple");

		// Token: 0x04000419 RID: 1049
		public static readonly OutfitDef BunnyEars = new OutfitDef
		{
			Id = "lewd_bunnyears",
			TexturePrefix = "bunnyears_",
			NameKey = "bunnyears",
			Slot = "hat",
			MainLimb = "Head",
			MainTexture = "experimentHead",
			VisualOffset = 9,
			Armor = 0f,
			Isolation = 0.02f,
			Weight = 0.05f,
			Value = 8,
			Lingerie = true,
			Rarity = 0.4f
		};

		// Token: 0x0400041A RID: 1050
		public static readonly OutfitDef BBSuit = new OutfitDef
		{
			Id = "lewd_bbsuit",
			TexturePrefix = "bbsuit_",
			NameKey = "bbsuit",
			Shoes = "heels_boot",
			Armor = 0.15f,
			Isolation = 0.35f,
			Weight = 0.9f,
			Value = 45,
			Rarity = 0.15f,
			CorpseRarity = 0.05f,
			MinDepth = 2,
			IsLatex = true
		};

		// Token: 0x0400041B RID: 1051
		public static readonly OutfitDef Wetsuit = new OutfitDef
		{
			Id = "lewd_wetsuit",
			TexturePrefix = "wetsuit_",
			NameKey = "wetsuit",
			Armor = 0.05f,
			Isolation = 0.5f,
			Weight = 0.8f,
			Value = 30,
			Rarity = 0.3f,
			CorpseRarity = 0.05f,
			MinDepth = 1
		};

		// Token: 0x0400041C RID: 1052
		public static readonly OutfitDef SneakingSuit = new OutfitDef
		{
			Id = "lewd_sneakingsuit",
			TexturePrefix = "sneaking_",
			NameKey = "sneakingsuit",
			Armor = 0.12f,
			Isolation = 0.25f,
			Weight = 0.6f,
			Value = 50,
			Rarity = 0.12f,
			CorpseRarity = 0.04f,
			MinDepth = 2
		};

		// Token: 0x0400041D RID: 1053
		public static readonly OutfitDef ResearchSuit = new OutfitDef
		{
			Id = "lewd_researchsuit",
			TexturePrefix = "research_",
			NameKey = "researchsuit",
			Shoes = "heels_boot",
			Armor = 0.3f,
			Isolation = 0.2f,
			Weight = 1.2f,
			Value = 55,
			Rarity = 0.1f,
			CorpseRarity = 0.04f,
			MinDepth = 2
		};

		// Token: 0x0400041E RID: 1054
		public static readonly OutfitDef LeotardWhite = new OutfitDef
		{
			Id = "lewd_leotardwhite",
			TexturePrefix = "leotardwhite_",
			NameKey = "leotardwhite",
			Armor = 0.02f,
			Isolation = 0.1f,
			Weight = 0.2f,
			Value = 16,
			Lingerie = true,
			Rarity = 0.4f,
			MinDepth = 1
		};

		// Token: 0x0400041F RID: 1055
		public static readonly OutfitDef Miner = new OutfitDef
		{
			Id = "lewd_minersuit",
			TexturePrefix = "miner_",
			NameKey = "minersuit",
			Armor = 0.12f,
			Isolation = 0.35f,
			Weight = 1.3f,
			Value = 12,
			MinCondition = 0.3f,
			MaxCondition = 0.6f,
			Rarity = 1.6f,
			CorpseRarity = 3f,
			InLoot = true
		};

		// Token: 0x04000420 RID: 1056
		public static readonly OutfitDef Thermo = new OutfitDef
		{
			Id = "lewd_thermosuit",
			TexturePrefix = "thermo_",
			NameKey = "thermosuit",
			Armor = 0.08f,
			Isolation = 2.5f,
			Weight = 2.4f,
			Value = 60,
			Thermal = true,
			Rarity = 0.45f,
			CorpseRarity = 0.6f,
			MinDepth = 1,
			MinCondition = 0.55f,
			MaxCondition = 0.95f
		};

		// Token: 0x04000421 RID: 1057
		public static readonly OutfitDef Stockings = new OutfitDef
		{
			Id = "lewd_stockings",
			TexturePrefix = "stockings_",
			NameKey = "stockings",
			Slot = "legwear",
			MainLimb = "DownTorso",
			MainTexture = "experimentDownTorso",
			VisualOffset = 2,
			Armor = 0f,
			Isolation = 0.03f,
			Weight = 0.05f,
			Value = 6,
			Lingerie = true,
			Rarity = 1f,
			CorpseRarity = 0.4f,
			InLoot = true
		};

		// Token: 0x04000422 RID: 1058
		public static readonly OutfitDef Tights = new OutfitDef
		{
			Id = "lewd_tights",
			TexturePrefix = "tights_",
			NameKey = "tights",
			Slot = "legwear",
			MainLimb = "DownTorso",
			MainTexture = "experimentDownTorso",
			VisualOffset = 2,
			Armor = 0f,
			Isolation = 0.05f,
			Weight = 0.05f,
			Value = 5,
			Lingerie = true,
			Rarity = 1.1f,
			CorpseRarity = 0.5f,
			InLoot = true
		};

		// Token: 0x04000423 RID: 1059
		public static readonly OutfitDef Bra = new OutfitDef
		{
			Id = "lewd_bra",
			TexturePrefix = "bra_",
			NameKey = "bra",
			Slot = "bra",
			VisualOffset = 2,
			Armor = 0f,
			Isolation = 0.02f,
			Weight = 0.05f,
			Value = 5,
			Lingerie = true,
			Rarity = 1.1f,
			CorpseRarity = 0.5f,
			InLoot = true
		};

		// Token: 0x04000424 RID: 1060
		public static readonly OutfitDef Fishnets = new OutfitDef
		{
			Id = "lewd_fishnets",
			TexturePrefix = "fishnet_",
			NameKey = "fishnets",
			Slot = "legwear",
			MainLimb = "DownTorso",
			MainTexture = "experimentDownTorso",
			VisualOffset = 2,
			Armor = 0f,
			Isolation = 0f,
			Weight = 0.04f,
			Value = 7,
			Lingerie = true,
			Rarity = 0.7f,
			CorpseRarity = 0.3f,
			MinDepth = 1,
			InLoot = true
		};

		// Token: 0x04000425 RID: 1061
		public static readonly OutfitDef Socks = new OutfitDef
		{
			Id = "lewd_socks",
			TexturePrefix = "socks_",
			NameKey = "socks",
			Slot = "legwear",
			MainLimb = "ThighF",
			MainTexture = "experimentThigh",
			VisualOffset = 2,
			Armor = 0f,
			Isolation = 0.08f,
			Weight = 0.06f,
			Value = 4,
			Lingerie = true,
			Rarity = 1.2f,
			CorpseRarity = 0.5f,
			InLoot = true
		};

		// Token: 0x04000426 RID: 1062
		public static readonly OutfitDef BlackBra = new OutfitDef
		{
			Id = "lewd_blackbra",
			TexturePrefix = "blackbra_",
			NameKey = "blackbra",
			Slot = "bra",
			VisualOffset = 2,
			Armor = 0f,
			Isolation = 0.02f,
			Weight = 0.05f,
			Value = 6,
			Lingerie = true,
			Rarity = 0.8f,
			CorpseRarity = 0.4f,
			MinDepth = 1,
			InLoot = true
		};

		// Token: 0x04000427 RID: 1063
		public static readonly OutfitDef Panties = new OutfitDef
		{
			Id = "lewd_panties",
			TexturePrefix = "panties_",
			NameKey = "panties",
			Slot = "panties",
			MainLimb = "DownTorso",
			MainTexture = "experimentDownTorso",
			VisualOffset = 1,
			Armor = 0f,
			Isolation = 0.01f,
			Weight = 0.02f,
			Value = 3,
			Lingerie = true,
			Rarity = 1.3f,
			CorpseRarity = 0.6f,
			InLoot = true
		};

		// Token: 0x04000428 RID: 1064
		public static readonly OutfitDef Leotard = new OutfitDef
		{
			Id = "lewd_leotard",
			TexturePrefix = "leotard_",
			NameKey = "leotard",
			Armor = 0.02f,
			Isolation = 0.1f,
			Weight = 0.2f,
			Value = 15,
			Lingerie = true,
			Rarity = 0.5f,
			MinDepth = 1
		};

		// Token: 0x04000429 RID: 1065
		public static readonly OutfitDef LabSuit = new OutfitDef
		{
			Id = "lewd_labsuit",
			TexturePrefix = "lab_",
			NameKey = "labsuit",
			Armor = 0.06f,
			Isolation = 0.3f,
			Weight = 0.9f,
			Value = 10,
			MinCondition = 0.35f,
			MaxCondition = 0.75f,
			Rarity = 1.3f,
			CorpseRarity = 2.5f,
			InLoot = true
		};

		// Token: 0x0400042A RID: 1066
		public static readonly OutfitDef Blanket = new OutfitDef
		{
			Id = "lewd_blanket",
			TexturePrefix = "blanket_",
			NameKey = "blanket",
			Slot = "lewd_none",
			NotWorn = true,
			Weight = 1.2f,
			Value = 15,
			Rarity = 0.5f,
			CorpseRarity = 0.3f,
			InLoot = true,
			MinCondition = 0.6f,
			MaxCondition = 1f
		};

		// Token: 0x0400042B RID: 1067
		public static readonly OutfitDef ThermoBlanket = new OutfitDef
		{
			Id = "lewd_thermoblanket",
			TexturePrefix = "thermoblanket_",
			NameKey = "thermoblanket",
			Slot = "lewd_wrap",
			VisualOffset = 10,
			Armor = 0f,
			Isolation = 1.6f,
			Weight = 0.2f,
			Value = 25,
			CanBeHeld = true,
			Rarity = 0.4f,
			CorpseRarity = 0.2f,
			InLoot = true
		};

		// Token: 0x0400042C RID: 1068
		public static readonly OutfitDef[] All = new OutfitDef[]
		{
			Outfits.Blanket,
			Outfits.ThermoBlanket,
			Outfits.Spacesuit,
			Outfits.Spacesuit_redblue,
			Outfits.Spacesuit_orange,
			Outfits.Spacesuit_black,
			Outfits.Spacesuit_blue,
			Outfits.Spacesuit_pink,
			Outfits.Spacesuit_green,
			Outfits.Spacesuit_yellow,
			Outfits.Latex,
			Outfits.LatexRed,
			Outfits.Miner,
			Outfits.Thermo,
			Outfits.Stockings,
			Outfits.Tights,
			Outfits.Bra,
			Outfits.Fishnets,
			Outfits.Socks,
			Outfits.BlackBra,
			Outfits.Panties,
			Outfits.Leotard,
			Outfits.LeotardWhite,
			Outfits.LabSuit,
			Outfits.BBSuit,
			Outfits.Wetsuit,
			Outfits.SneakingSuit,
			Outfits.ResearchSuit,
			Outfits.BunnySuit,
			Outfits.BunnySuitPink,
			Outfits.BunnyEars,
			Outfits.TacWoodland,
			Outfits.TacDesert,
			Outfits.TacUrban,
			Outfits.TacWinter,
			Outfits.HeadphonesBlack,
			Outfits.HeadphonesOlive,
			Outfits.HeelsBlack,
			Outfits.HeelsRed,
			Outfits.HeelsWhite,
			Outfits.HeelsPink,
			Outfits.HeelsPurple
		}.Concat<OutfitDef>(Devices.Defs).ToArray<OutfitDef>();

		// Token: 0x0400042D RID: 1069
		public static readonly OutfitDef[] Spacesuits = new OutfitDef[]
		{
			Outfits.Spacesuit,
			Outfits.Spacesuit_redblue,
			Outfits.Spacesuit_orange,
			Outfits.Spacesuit_black,
			Outfits.Spacesuit_blue,
			Outfits.Spacesuit_pink,
			Outfits.Spacesuit_green,
			Outfits.Spacesuit_yellow
		};

		// Token: 0x0400042E RID: 1070
		private static readonly Random suitRng = new Random(Guid.NewGuid().GetHashCode());

		// Token: 0x0400042F RID: 1071
		public static readonly OutfitDef[] Wardrobe = new OutfitDef[]
		{
			Outfits.ThermoBlanket,
			Outfits.BunnySuit,
			Outfits.BunnySuitPink,
			Outfits.BunnyEars,
			Outfits.Miner,
			Outfits.Thermo,
			Outfits.LabSuit,
			Outfits.Leotard,
			Outfits.LeotardWhite,
			Outfits.LatexRed,
			Outfits.BBSuit,
			Outfits.Wetsuit,
			Outfits.SneakingSuit,
			Outfits.ResearchSuit,
			Outfits.TacWoodland,
			Outfits.TacDesert,
			Outfits.TacUrban,
			Outfits.TacWinter,
			Outfits.HeadphonesBlack,
			Outfits.HeadphonesOlive,
			Outfits.HeelsBlack,
			Outfits.HeelsRed,
			Outfits.Stockings,
			Outfits.Tights,
			Outfits.Fishnets,
			Outfits.Socks,
			Outfits.Bra,
			Outfits.BlackBra,
			Outfits.Panties
		};

		// Token: 0x04000430 RID: 1072
		private static readonly Dictionary<string, string> Parts = new Dictionary<string, string>
		{
			{ "DownTorso", "experimentDownTorso" },
			{ "ThighF", "experimentThigh" },
			{ "ThighB", "experimentThigh" },
			{ "CrusF", "experimentCrus" },
			{ "CrusB", "experimentCrus" },
			{ "FootF", "experimentFoot" },
			{ "FootB", "experimentFoot" },
			{ "UpArmF", "experimentUpArm" },
			{ "UpArmB", "experimentUpArm" },
			{ "DownArmF", "experimentDownArm" },
			{ "DownArmB", "experimentDownArm" },
			{ "HandF", "experimentHandF" },
			{ "HandB", "experimentHandB" }
		};

		// Token: 0x04000431 RID: 1073
		private static GameObject holder;

		// Token: 0x04000432 RID: 1074
		public const string MagazineTag = "lewd_magazine";

		// Token: 0x04000433 RID: 1075
		private static readonly string[] Slots = Outfits.All.Select<OutfitDef, string>((OutfitDef d) => d.Slot).Distinct<string>().ToArray<string>();

		// Token: 0x04000434 RID: 1076
		private static readonly Dictionary<string, Sprite> torn = new Dictionary<string, Sprite>();

		// Token: 0x020001E9 RID: 489
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000A28 RID: 2600
			public static ItemInfo.Use <0>__UseKey;
		}
	}
}
