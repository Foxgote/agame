using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using BepInEx.Logging;
using ScavArmory;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200006C RID: 108
	internal static class Devices
	{
		// Token: 0x0600020D RID: 525 RVA: 0x0002A1EC File Offset: 0x000283EC
		private static List<OutfitDef> Build()
		{
			List<OutfitDef> list = new List<OutfitDef>();
			foreach (string text in Devices.Colors)
			{
				foreach (KeyValuePair<Devices.Kind, ValueTuple<string, string, string, string, int, float>> keyValuePair in Devices.Spec)
				{
					ValueTuple<string, string, string, string, int, float> value = keyValuePair.Value;
					if (keyValuePair.Key != Devices.Kind.Jacket || Devices.JacketColors.Contains(text))
					{
						list.Add(new OutfitDef
						{
							Id = "lewd_dd_" + value.Item1 + "_" + text,
							TexturePrefix = value.Item1 + "_" + text + "_",
							NameKey = "dd_" + value.Item1,
							ColorKey = text,
							Folder = "devices/" + value.Item1 + "_" + text,
							Slot = value.Item2,
							MainLimb = value.Item3,
							MainTexture = value.Item4,
							VisualOffset = value.Item5,
							IsDevice = true,
							Locks = (keyValuePair.Key != Devices.Kind.Harness),
							Kind = keyValuePair.Key,
							Rarity = 0f,
							CorpseRarity = 0f,
							Weight = ((keyValuePair.Key == Devices.Kind.Suit || keyValuePair.Key == Devices.Kind.Jacket) ? 0.9f : 0.3f),
							Value = ((keyValuePair.Key == Devices.Kind.Suit) ? 40 : ((keyValuePair.Key == Devices.Kind.Jacket) ? 35 : 20)),
							Armor = ((keyValuePair.Key == Devices.Kind.Suit) ? 0.05f : ((keyValuePair.Key == Devices.Kind.Jacket) ? 0.1f : 0f)),
							Isolation = ((keyValuePair.Key == Devices.Kind.Suit || keyValuePair.Key == Devices.Kind.Jacket) ? 0.15f : 0f),
							IsLatex = (keyValuePair.Key == Devices.Kind.Suit)
						});
					}
				}
				list.Add(new OutfitDef
				{
					Id = "lewd_lstock_" + text,
					TexturePrefix = "lstock_" + text + "_",
					NameKey = "lstock",
					ColorKey = text,
					Folder = "devices/lstock_" + text,
					Slot = "legwear",
					MainLimb = "ThighF",
					MainTexture = "experimentThigh",
					VisualOffset = 2,
					Lingerie = true,
					Kind = Devices.Kind.Stockings,
					Isolation = 0.08f,
					Weight = 0.15f,
					Value = 14,
					Rarity = 0.12f,
					MinDepth = 1
				});
				list.Add(new OutfitDef
				{
					Id = "lewd_boots_" + text,
					TexturePrefix = "boots_" + text + "_",
					NameKey = "lboots",
					ColorKey = text,
					Folder = "devices/boots_" + text,
					Slot = "feet",
					MainLimb = "CrusF",
					MainTexture = "experimentCrus",
					VisualOffset = 4,
					Kind = Devices.Kind.Boots,
					Shoes = "heels_boot",
					Armor = 0.05f,
					Isolation = 0.1f,
					Weight = 0.5f,
					Value = 22,
					Rarity = 0.12f,
					MinDepth = 1
				});
			}
			list.Add(Devices.Key);
			return list;
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0002A5C8 File Offset: 0x000287C8
		public static void Bind(ConfigFile c)
		{
			Devices.Enabled = c.Bind<bool>("Devices", "Enabled", true, "Restraints (cuffs, ankle cuffs, gag, collar, chastity belt, a latex suit that locks): raiders may lock them on her, crates hold them now and then.");
			Devices.CaptureChance = c.Bind<float>("Devices", "CaptureChance", 0.45f, "Chance a raider who wants her also locks a device or two on her before letting her go (more if she surrendered, more the more submissive she is).");
			Devices.CrateChance = c.Bind<float>("Devices", "CrateChance", 0.05f, "Chance a broken crate holds a device (or latex stockings, boots).");
			Devices.KeyCrateChance = c.Bind<float>("Devices", "KeyCrateChance", 0.04f, "Chance a broken crate holds a key to them.");
			Devices.KeyDropChance = c.Bind<float>("Devices", "RaiderKeyChance", 0.7f, "Chance the raider who locked something on her carries its key (dropped when he dies).");
			Devices.SelfLockChance = c.Bind<float>("Devices", "SelfLockChance", 0.5f, "Chance the lock clicks shut when she puts a device on herself.");
			Devices.StruggleScale = c.Bind<float>("Devices", "StruggleScale", 1f, "Scales every chance to struggle out of a device (cuffs 6%, ankle cuffs 10%, gag 12%, collar 4%, belt and latex suit 3% a try).");
			Devices.TraderKeyValue = c.Bind<int>("Devices", "TraderKeyValue", 30, "What a trader asks for a restraint key (he has one for every lock on her, bought with money or in trade).");
			Devices.MinLust = c.Bind<float>("Devices", "MinLust", 45f, "Lust (or arousal) she needs before she will pick a device up or put one on herself.");
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600020F RID: 527 RVA: 0x0002A6E5 File Offset: 0x000288E5
		public static bool On
		{
			get
			{
				return Devices.Enabled == null || Devices.Enabled.Value;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000210 RID: 528 RVA: 0x0002A6FA File Offset: 0x000288FA
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0002A704 File Offset: 0x00028904
		public static OutfitDef ById(string id)
		{
			if (id == null || !id.StartsWith("lewd_"))
			{
				return null;
			}
			return Devices.Defs.FirstOrDefault<OutfitDef>((OutfitDef d) => d.Id == id);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0002A750 File Offset: 0x00028950
		private static bool IsLockable(Item i)
		{
			if (i)
			{
				OutfitDef outfitDef = Devices.ById(i.id);
				if (outfitDef != null)
				{
					return outfitDef.Locks;
				}
			}
			return false;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0002A77C File Offset: 0x0002897C
		public static Item Worn(Body b, Devices.Kind k)
		{
			ValueTuple<string, string, string, string, int, float> valueTuple;
			if (!b || !Devices.Spec.TryGetValue(k, out valueTuple))
			{
				return null;
			}
			Item wearableBySlotID = b.GetWearableBySlotID(valueTuple.Item2);
			if (wearableBySlotID)
			{
				OutfitDef outfitDef = Devices.ById(wearableBySlotID.id);
				if (outfitDef != null && outfitDef.Kind == k)
				{
					return wearableBySlotID;
				}
			}
			return null;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0002A7D2 File Offset: 0x000289D2
		public static bool IsLocked(Item i)
		{
			return i && Devices.S != null && Devices.S.locked.Contains(i.id);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0002A7FC File Offset: 0x000289FC
		public static bool OnHer(Body b, Item i)
		{
			if (!b || !i || !i.transform.parent || b.limbs == null)
			{
				return false;
			}
			Limb component = i.transform.parent.GetComponent<Limb>();
			return component && Array.IndexOf<Limb>(b.limbs, component) >= 0;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0002A862 File Offset: 0x00028A62
		public static bool HandsBound(Body b)
		{
			return Devices.On && (Devices.Worn(b, Devices.Kind.Wrists) || Devices.Worn(b, Devices.Kind.Jacket));
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0002A88A File Offset: 0x00028A8A
		public static bool Gagged(Body b)
		{
			return Devices.On && Devices.Worn(b, Devices.Kind.Gag);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0002A8A1 File Offset: 0x00028AA1
		public static bool Belted(Body b)
		{
			return Devices.On && Devices.Worn(b, Devices.Kind.Belt);
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0002A8B8 File Offset: 0x00028AB8
		public static bool Hobbled(Body b)
		{
			return Devices.On && Devices.Worn(b, Devices.Kind.Ankles);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0002A8CF File Offset: 0x00028ACF
		public static bool Covers(Devices.Kind k)
		{
			return k == Devices.Kind.Suit || k == Devices.Kind.Jacket;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0002A8DC File Offset: 0x00028ADC
		public static List<Item> LockedOn(Body b)
		{
			List<Item> list = (from k in new Devices.Kind[]
				{
					Devices.Kind.Jacket,
					Devices.Kind.Wrists,
					Devices.Kind.Gag,
					Devices.Kind.Belt,
					Devices.Kind.Ankles,
					Devices.Kind.Suit,
					Devices.Kind.Collar
				}
				select Devices.Worn(b, k) into i
				where i && Devices.IsLocked(i)
				select i).ToList<Item>();
			Item item = (b ? b.GetWearableBySlotID("dd_plug") : null);
			if (item && Devices.IsLocked(item))
			{
				list.Add(item);
			}
			return list;
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0002A97B File Offset: 0x00028B7B
		public static void Lock(Item i)
		{
			if (i && !Devices.S.locked.Contains(i.id))
			{
				Devices.S.locked.Add(i.id);
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600021D RID: 541 RVA: 0x0002A9B1 File Offset: 0x00028BB1
		// (set) Token: 0x0600021E RID: 542 RVA: 0x0002A9B8 File Offset: 0x00028BB8
		public static bool Releasing { get; private set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600021F RID: 543 RVA: 0x0002A9C0 File Offset: 0x00028BC0
		// (set) Token: 0x06000220 RID: 544 RVA: 0x0002A9C7 File Offset: 0x00028BC7
		public static bool Applying { get; private set; }

		// Token: 0x06000221 RID: 545 RVA: 0x0002A9D0 File Offset: 0x00028BD0
		public static void Release(Body b, Item i, bool drop = true)
		{
			if (!i)
			{
				return;
			}
			Devices.S.locked.Remove(i.id);
			if (!drop)
			{
				return;
			}
			Devices.Releasing = true;
			try
			{
				b.DropWearable(i);
			}
			finally
			{
				Devices.Releasing = false;
			}
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0002AA28 File Offset: 0x00028C28
		public static bool WantsToLock(bool surrendered, bool lewd)
		{
			if (!Devices.On || !LewdSettings.On || !lewd)
			{
				return false;
			}
			float num = Devices.CaptureChance.Value * (surrendered ? 1.3f : 1f) * (1f + Devices.S.submission * 0.005f);
			return Random.value < num;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0002AA84 File Offset: 0x00028C84
		public static bool LockOn(Body b, string by)
		{
			int num = ((Random.value < 0.35f) ? 2 : 1);
			List<Devices.Kind> list = new List<Devices.Kind>
			{
				Devices.Kind.Wrists,
				Devices.Kind.Gag,
				Devices.Kind.Collar,
				Devices.Kind.Belt,
				Devices.Kind.Ankles,
				Devices.Kind.Suit,
				Devices.Kind.Jacket,
				Devices.Kind.Harness
			};
			bool flag = false;
			string col = Devices.Colors[Random.Range(0, Devices.Colors.Length)];
			int num2 = 0;
			while (num2 < 8 && num > 0 && list.Count > 0)
			{
				Devices.Kind kind = list[Random.Range(0, list.Count)];
				list.Remove(kind);
				if (!Devices.Worn(b, kind) && ((kind != Devices.Kind.Suit && kind != Devices.Kind.Jacket) || Random.value >= 0.6f))
				{
					OutfitDef outfitDef = Devices.Defs.FirstOrDefault<OutfitDef>((OutfitDef d) => d.Kind == kind && d.ColorKey == col) ?? Devices.Defs.First<OutfitDef>((OutfitDef d) => d.Kind == kind);
					if (Devices.PutOn(b, outfitDef, outfitDef.Locks))
					{
						flag = true;
						num--;
					}
				}
				num2++;
			}
			if (flag)
			{
				ManualLogSource log = Plugin.Log;
				string[] array = new string[5];
				array[0] = "Devices: ";
				array[1] = by;
				array[2] = " locked ";
				array[3] = string.Join(", ", from i in Devices.LockedOn(b)
					select i.id);
				array[4] = " on her";
				log.LogInfo(string.Concat(array));
			}
			return flag;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0002AC48 File Offset: 0x00028E48
		public static bool PutOn(Body b, OutfitDef d, bool locked)
		{
			Item wearableBySlotID = b.GetWearableBySlotID(d.Slot);
			if (wearableBySlotID)
			{
				if (Devices.IsLocked(wearableBySlotID))
				{
					return false;
				}
				b.DropWearable(wearableBySlotID);
			}
			GameObject gameObject = Outfits.SpawnAt(d, b.transform.position);
			Item item;
			if (!gameObject || !gameObject.TryGetComponent<Item>(ref item))
			{
				return false;
			}
			Devices.Applying = true;
			try
			{
				b.WearWearable(item);
			}
			finally
			{
				Devices.Applying = false;
			}
			if (!b.GetWearableBySlotID(d.Slot) || b.GetWearableBySlotID(d.Slot) != item)
			{
				Object.Destroy(gameObject);
				return false;
			}
			if (locked)
			{
				Devices.Lock(item);
			}
			return true;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0002AD08 File Offset: 0x00028F08
		public static void Update(Body b, float dt)
		{
			if (!Devices.On || !b)
			{
				return;
			}
			if (Devices.struggleCooldown > 0f)
			{
				Devices.struggleCooldown -= dt;
			}
			if (Devices.Worn(b, Devices.Kind.Harness) && Devices.S.submission < 50f)
			{
				Devices.S.submission += 0.002f * dt;
			}
			if (Devices.Gagged(b) || Devices.Worn(b, Devices.Kind.Collar))
			{
				Devices.S.submission = Mathf.Min(70f, Devices.S.submission + (Devices.Gagged(b) ? 0.003f : 0f) * dt + (Devices.Worn(b, Devices.Kind.Collar) ? 0.004f : 0f) * dt);
			}
			if (Devices.Belted(b) && Devices.S.lust > 60f)
			{
				Devices.S.arousal = Mathf.Min(Devices.S.arousal, 92f);
				Mood.Change(b, -0.008f * dt);
			}
			if ((Devices.tick -= dt) <= 0f)
			{
				Devices.tick = 2f;
				IEnumerable<string> locked = Devices.S.locked;
				Func<string, bool> <>9__0;
				Func<string, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (string id) => !b.GetAllWearables().Any<Item>((Item w) => w && w.id == id));
				}
				foreach (string text in locked.Where<string>(func).ToList<string>())
				{
					Devices.S.locked.Remove(text);
					Plugin.Log.LogInfo("Devices: " + text + " is no longer on her; now " + Devices.Where(text));
				}
			}
			if (Keys.Down(Plugin.RemoveKey.Value) && Suit.Stage == 0 && !Scenes.Busy && !Capture.Active)
			{
				Devices.TryFree(b);
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0002AF48 File Offset: 0x00029148
		public static string Where(string id)
		{
			List<Item> list = (from i in Resources.FindObjectsOfTypeAll<Item>()
				where i && i.id == id && i.gameObject.scene.IsValid()
				select i).ToList<Item>();
			if (list.Count == 0)
			{
				return "gone (destroyed)";
			}
			return string.Join("; ", list.Select<Item, string>(delegate(Item i)
			{
				List<string> list2 = new List<string>();
				Transform transform = i.transform.parent;
				while (transform)
				{
					list2.Add(transform.name);
					transform = transform.parent;
				}
				return string.Format("{0} at {1} under [{2}]", i.gameObject.activeInHierarchy ? "active" : "inactive", i.transform.position, string.Join(" < ", list2));
			}));
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0002AFBC File Offset: 0x000291BC
		private static void TryFree(Body b)
		{
			List<Item> list = Devices.LockedOn(b);
			if (list.Count == 0 || Devices.struggleCooldown > 0f)
			{
				return;
			}
			Devices.struggleCooldown = 1.2f;
			Item item = list[0];
			Item item2 = b.GetAllItemsThorough().FirstOrDefault<Item>((Item i) => i && i.id == Devices.Key.Id);
			if (item2)
			{
				Devices.UseKey(b, item2);
				return;
			}
			Item item3 = Devices.HeldItem(b, (Item i) => i.id == "lockpickingkit");
			if (item3 && !Devices.HandsBound(b))
			{
				Lockpick.Start(b, item, item3);
				return;
			}
			Item item4 = Devices.Worn(b, Devices.Kind.Suit);
			if (item4 && Devices.IsLocked(item4))
			{
				if (Devices.HeldItem(b, delegate(Item i)
				{
					if (i.Stats.qualities != null)
					{
						return i.Stats.qualities.Any<CraftingQuality>((CraftingQuality q) => q.id == "cutting");
					}
					return false;
				}) && !Devices.HandsBound(b))
				{
					Devices.cutProgress += 0.2f;
					b.stamina = Mathf.Max(0f, b.stamina - 6f);
					Sound.Play("ripclothing", b.transform.position, false, true, b.transform, 0.6f, 1f, false, false);
					if (Devices.cutProgress >= 1f)
					{
						Devices.cutProgress = 0f;
						Devices.Release(b, item4, false);
						Devices.Releasing = true;
						try
						{
							b.DropWearable(item4);
						}
						finally
						{
							Devices.Releasing = false;
						}
						Object.Destroy(item4.gameObject);
						Relief.Say(b, "dd_cut_done");
						return;
					}
					Relief.Say(b, "dd_cut");
					return;
				}
			}
			if (b.stamina < 8f)
			{
				Relief.Say(b, "relief_tired");
				return;
			}
			b.stamina = Mathf.Max(0f, b.stamina - 10f);
			OutfitDef outfitDef = Devices.ById(item.id);
			float num = ((outfitDef != null) ? Devices.Spec[outfitDef.Kind].Item6 : 0.05f) * Devices.StruggleScale.Value * Diff.Escape * ((Devices.HandsBound(b) && outfitDef != null && outfitDef.Kind != Devices.Kind.Wrists && outfitDef.Kind != Devices.Kind.Jacket) ? 0.5f : 1f) * (1f - Devices.S.submission * 0.004f);
			if (Random.value < num)
			{
				item.SetCondition(Mathf.Max(0.05f, item.condition - 0.4f));
				Devices.Release(b, item, true);
				Relief.Say(b, "dd_struggle_free");
				Plugin.Log.LogInfo(string.Format("Devices: she struggled out of her {0} ({1:P0})", item.id, num));
				return;
			}
			Relief.Say(b, "dd_struggle");
			Sound.Play("ripclothing", b.transform.position, false, true, b.transform, 0.25f, 1f, false, false);
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0002B2E0 File Offset: 0x000294E0
		public static void UseKey(Body b, Item key)
		{
			if (!b || !key)
			{
				return;
			}
			List<Item> list = Devices.LockedOn(b);
			if (list.Count == 0)
			{
				Relief.Say(b, "dd_key_nothing");
				return;
			}
			Item item = list[0];
			if (Devices.HandsBound(b))
			{
				Item item2 = list.FirstOrDefault<Item>(delegate(Item i)
				{
					OutfitDef outfitDef = Devices.ById(i.id);
					return outfitDef != null && (outfitDef.Kind == Devices.Kind.Wrists || outfitDef.Kind == Devices.Kind.Jacket);
				});
				if (item2)
				{
					item = item2;
				}
				if (Random.value < 0.5f)
				{
					Relief.Say(b, "dd_key_fumble");
					return;
				}
			}
			Object.Destroy(key.gameObject);
			Devices.Release(b, item, true);
			Relief.Say(b, "dd_key_used");
			if (PlayerCamera.main)
			{
				PlayerCamera.main.PlayUISound(0, 1f);
			}
			Plugin.Log.LogInfo("Devices: a key opened her " + item.id);
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0002B3C5 File Offset: 0x000295C5
		public static Item BeltOn(Body b)
		{
			if (!Devices.On)
			{
				return null;
			}
			return Devices.Worn(b, Devices.Kind.Belt);
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0002B3D8 File Offset: 0x000295D8
		private static Item HeldItem(Body b, Func<Item, bool> what)
		{
			for (int i = 0; i < b.slots.Length; i++)
			{
				if (b.slots[i].isHand && b.HoldingItem(i))
				{
					Item item = b.GetItem(i);
					if (item != null && item && what(item))
					{
						return item;
					}
				}
			}
			return null;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0002B430 File Offset: 0x00029630
		public static void FillCrate(BuildingEntity e)
		{
			if (!Devices.On || !LewdSettings.On)
			{
				return;
			}
			Vector2 vector = e.transform.position;
			if (Random.value < Devices.CrateChance.Value)
			{
				List<OutfitDef> list = Devices.Defs.Where<OutfitDef>((OutfitDef d) => d != Devices.Key).ToList<OutfitDef>();
				Devices.Throw(list[Random.Range(0, list.Count)], vector);
			}
			if (Random.value < Devices.KeyCrateChance.Value)
			{
				Devices.Throw(Devices.Key, vector);
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0002B4D4 File Offset: 0x000296D4
		public static void Throw(OutfitDef d, Vector2 at)
		{
			GameObject gameObject = Outfits.SpawnAt(d, at);
			Rigidbody2D rigidbody2D;
			if (gameObject && gameObject.TryGetComponent<Rigidbody2D>(ref rigidbody2D))
			{
				rigidbody2D.velocity = new Vector2(Random.Range(-6f, 6f), Random.Range(2f, 6f));
			}
			if (gameObject)
			{
				Plugin.Log.LogInfo("Crate broke: a " + d.Id + " was in it");
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600022D RID: 557 RVA: 0x0002B54B File Offset: 0x0002974B
		private static bool Wanting
		{
			get
			{
				return Devices.S.lust >= Devices.MinLust.Value || Devices.S.arousal >= Devices.MinLust.Value;
			}
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0002B580 File Offset: 0x00029780
		public static bool AllowPickUp(Body b, Item item)
		{
			if (item && Devices.On && LewdSettings.On)
			{
				OutfitDef outfitDef = Devices.ById(item.id);
				if (outfitDef != null && outfitDef.IsDevice && outfitDef.Kind != Devices.Kind.Key)
				{
					if (!PlayerCamera.main || PlayerCamera.main.body != b || Time.timeSinceLevelLoad < 5f || Devices.accepted.Contains(item.GetInstanceID()))
					{
						return true;
					}
					if (!Devices.Wanting)
					{
						Devices.Refuse(b, "dd_refuse");
						return false;
					}
					Devices.accepted.Add(item.GetInstanceID());
					Relief.Say(b, "dd_accept");
					return true;
				}
			}
			return true;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0002B634 File Offset: 0x00029834
		private static void Refuse(Body b, string line)
		{
			if (Time.time - Devices.lastRefuse < 3f)
			{
				return;
			}
			Devices.lastRefuse = Time.time;
			Relief.Say(b, line);
			PlayerCamera.main.PlayUISound(6, 1f);
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0002B66C File Offset: 0x0002986C
		public static bool BeforeWear(Body b, Item item)
		{
			if (Devices.Applying || !Devices.On || !PlayerCamera.main || PlayerCamera.main.body != b || Time.timeSinceLevelLoad < 5f)
			{
				return true;
			}
			if (Devices.HandsBound(b))
			{
				Devices.Refuse(b, "dd_hands");
				return false;
			}
			if (!Devices.IsLockable(item))
			{
				return true;
			}
			if (!Devices.Wanting)
			{
				Devices.Refuse(b, "dd_nowear");
				return false;
			}
			return true;
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0002B6E8 File Offset: 0x000298E8
		public static void AfterWear(Body b, Item item)
		{
			if (Devices.Applying || !Devices.IsLockable(item) || !Devices.On || !PlayerCamera.main || PlayerCamera.main.body != b)
			{
				return;
			}
			if (b.GetWearableBySlotID(Devices.ById(item.id).Slot) != item)
			{
				return;
			}
			if (Random.value < Devices.SelfLockChance.Value)
			{
				Devices.Lock(item);
				Relief.Say(b, "dd_selflock");
				Devices.LastLine = Time.time;
				PlayerCamera.main.PlayUISound(0, 1f);
				Plugin.Log.LogInfo("Devices: she put on " + item.id + " and it locked");
				return;
			}
			Relief.Say(b, "dd_selfwear");
			Devices.LastLine = Time.time;
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0002B7BC File Offset: 0x000299BC
		public static bool AllowDrop(Body b, Item item)
		{
			if (Devices.Releasing || !PlayerCamera.main || PlayerCamera.main.body != b)
			{
				return true;
			}
			if (Devices.PlayerTaking && Devices.HandsBound(b) && !Devices.IsLocked(item))
			{
				OutfitDef outfitDef = Devices.ById(item.id);
				if (outfitDef == null || (outfitDef.Kind != Devices.Kind.Wrists && outfitDef.Kind != Devices.Kind.Jacket))
				{
					Devices.Refuse(b, "dd_hands");
					return false;
				}
			}
			if (!Devices.IsLocked(item))
			{
				return true;
			}
			Limb limb = (item.transform.parent ? item.transform.parent.GetComponent<Limb>() : null);
			if (limb && limb.dismembered)
			{
				return true;
			}
			Devices.Refuse(b, "dd_locked");
			return false;
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0002B883 File Offset: 0x00029A83
		public static void Reset()
		{
			Devices.cutProgress = 0f;
			Devices.struggleCooldown = 0f;
		}

		// Token: 0x0400022C RID: 556
		public static readonly string[] Colors = new string[] { "black", "red", "pink", "white", "purple" };

		// Token: 0x0400022D RID: 557
		[TupleElementNames(new string[] { "art", "slot", "limb", "tex", "offset", "escape" })]
		private static readonly Dictionary<Devices.Kind, ValueTuple<string, string, string, string, int, float>> Spec = new Dictionary<Devices.Kind, ValueTuple<string, string, string, string, int, float>>
		{
			{
				Devices.Kind.Wrists,
				new ValueTuple<string, string, string, string, int, float>("cuffs", "dd_wrists", "DownArmF", "experimentDownArm", 6, 0.06f)
			},
			{
				Devices.Kind.Ankles,
				new ValueTuple<string, string, string, string, int, float>("ankles", "dd_ankles", "CrusF", "experimentCrus", 6, 0.1f)
			},
			{
				Devices.Kind.Gag,
				new ValueTuple<string, string, string, string, int, float>("gag", "dd_mouth", "Head", "experimentHead", 6, 0.12f)
			},
			{
				Devices.Kind.Collar,
				new ValueTuple<string, string, string, string, int, float>("collar", "dd_neck", "UpTorso", "experimentUpTorso", 6, 0.04f)
			},
			{
				Devices.Kind.Belt,
				new ValueTuple<string, string, string, string, int, float>("belt", "dd_belt", "DownTorso", "experimentDownTorso", 5, 0.03f)
			},
			{
				Devices.Kind.Suit,
				new ValueTuple<string, string, string, string, int, float>("lsuit", "bodysuit", "UpTorso", "experimentUpTorso", 3, 0.03f)
			},
			{
				Devices.Kind.Jacket,
				new ValueTuple<string, string, string, string, int, float>("jacket", "bodysuit", "UpTorso", "experimentUpTorso", 3, 0.02f)
			},
			{
				Devices.Kind.Harness,
				new ValueTuple<string, string, string, string, int, float>("harness", "dd_harness", "UpTorso", "experimentUpTorso", 4, 0f)
			}
		};

		// Token: 0x0400022E RID: 558
		private static readonly string[] JacketColors = new string[] { "white", "black" };

		// Token: 0x0400022F RID: 559
		public static readonly OutfitDef Key = new OutfitDef
		{
			Id = "lewd_ddkey",
			TexturePrefix = "ddkey_",
			NameKey = "ddkey",
			Folder = "devices/key",
			MainTexture = "experimentHandF",
			NotWorn = true,
			Weight = 0.05f,
			Value = 15,
			Rarity = 0f,
			IsDevice = true,
			Kind = Devices.Kind.Key
		};

		// Token: 0x04000230 RID: 560
		public static readonly List<OutfitDef> Defs = Devices.Build();

		// Token: 0x04000231 RID: 561
		internal static ConfigEntry<bool> Enabled;

		// Token: 0x04000232 RID: 562
		internal static ConfigEntry<float> CaptureChance;

		// Token: 0x04000233 RID: 563
		internal static ConfigEntry<float> CrateChance;

		// Token: 0x04000234 RID: 564
		internal static ConfigEntry<float> KeyCrateChance;

		// Token: 0x04000235 RID: 565
		internal static ConfigEntry<float> SelfLockChance;

		// Token: 0x04000236 RID: 566
		internal static ConfigEntry<float> StruggleScale;

		// Token: 0x04000237 RID: 567
		internal static ConfigEntry<float> MinLust;

		// Token: 0x04000238 RID: 568
		internal static ConfigEntry<float> KeyDropChance;

		// Token: 0x04000239 RID: 569
		internal static ConfigEntry<int> TraderKeyValue;

		// Token: 0x0400023A RID: 570
		public static float LastLine = -99f;

		// Token: 0x0400023D RID: 573
		public static bool PlayerTaking;

		// Token: 0x0400023E RID: 574
		private static float struggleCooldown;

		// Token: 0x0400023F RID: 575
		private static float tick;

		// Token: 0x04000240 RID: 576
		private static float cutProgress;

		// Token: 0x04000241 RID: 577
		private static float lastRefuse = -99f;

		// Token: 0x04000242 RID: 578
		private static readonly HashSet<int> accepted = new HashSet<int>();

		// Token: 0x02000199 RID: 409
		public enum Kind
		{
			// Token: 0x0400092F RID: 2351
			Wrists,
			// Token: 0x04000930 RID: 2352
			Ankles,
			// Token: 0x04000931 RID: 2353
			Gag,
			// Token: 0x04000932 RID: 2354
			Collar,
			// Token: 0x04000933 RID: 2355
			Belt,
			// Token: 0x04000934 RID: 2356
			Suit,
			// Token: 0x04000935 RID: 2357
			Stockings,
			// Token: 0x04000936 RID: 2358
			Boots,
			// Token: 0x04000937 RID: 2359
			Key,
			// Token: 0x04000938 RID: 2360
			Jacket,
			// Token: 0x04000939 RID: 2361
			Harness
		}
	}
}
