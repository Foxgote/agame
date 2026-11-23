using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200005C RID: 92
	internal static class Clothes
	{
		// Token: 0x060001CC RID: 460 RVA: 0x00027363 File Offset: 0x00025563
		public static IEnumerable<Item> Worn(Body b)
		{
			foreach (string text in Outfits.AllSlots.Concat<string>(Clothes.GameSlots).Distinct<string>())
			{
				Item wearableBySlotID = b.GetWearableBySlotID(text);
				if (wearableBySlotID && (Outfits.ById(wearableBySlotID.id) != null || Clothes.GameCloth.Contains(wearableBySlotID.id)))
				{
					yield return wearableBySlotID;
				}
			}
			IEnumerator<string> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060001CD RID: 461 RVA: 0x00027373 File Offset: 0x00025573
		private static bool LivingSuit
		{
			get
			{
				return Suit.Stage > 0 && !Suit.Drone && !Clothes.ClothesWelcome;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060001CE RID: 462 RVA: 0x0002738E File Offset: 0x0002558E
		private static bool ClothesWelcome
		{
			get
			{
				return Suit.Obeys && (!Suit.Symbiosis || Clothes.S.symbClothesOk);
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060001CF RID: 463 RVA: 0x000273AC File Offset: 0x000255AC
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x000273B4 File Offset: 0x000255B4
		private static void SymbioteRefused(Body b)
		{
			if (!Suit.Symbiosis || Clothes.S.symbClothesOk)
			{
				return;
			}
			State s = Clothes.S;
			int num = s.symbClothesNo + 1;
			s.symbClothesNo = num;
			if (num >= 3)
			{
				Clothes.S.symbClothesOk = true;
				SymbioteVoice.Say(b, "symv_clothes_ok", 1.5f);
				Relief.Say(b, "symr_clothes_ok");
				Plugin.Log.LogInfo("The symbiote has given in: she may wear clothes over it now");
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00027424 File Offset: 0x00025624
		private static void Welcome(Body b)
		{
			if (Clothes.saidOk || !Clothes.ClothesWelcome || !Clothes.Worn(b).Any<Item>())
			{
				return;
			}
			Clothes.saidOk = true;
			Relief.Say(b, Suit.Symbiote ? "symr_clothes_worn" : (Suit.HostSlime ? "hslime_clothes_ok" : (Suit.Slime ? "slime_clothes_ok" : "suit_clothes_ok")));
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00027488 File Offset: 0x00025688
		private static IEnumerable<Item> InTheWay(Body b)
		{
			if (!Clothes.LivingSuit || Suit.Stage < 3)
			{
				yield break;
			}
			HashSet<Item> mine = new HashSet<Item>(Clothes.Worn(b));
			foreach (Item item in b.GetAllWearables())
			{
				if (item && !mine.Contains(item) && Outfits.ById(item.id) == null)
				{
					string wearSlotId = item.Stats.wearSlotId;
					if (Clothes.CoveredAt3.Contains(wearSlotId) || (Suit.Stage >= 4 && Clothes.CoveredAt4.Contains(wearSlotId)))
					{
						yield return item;
					}
				}
			}
			List<Item>.Enumerator enumerator = default(List<Item>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x00027498 File Offset: 0x00025698
		private static bool Grabbed
		{
			get
			{
				return Flowers.Holding || Tentacles.Holding || Flesh.Holding || FleshWall.Holding || Pits.InMaw || Pits.InThroat || Slits.Holding || Slits.Inside || SlimeBog.Stuck || Worm.Inside || External.ScavHolding;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x000274F2 File Offset: 0x000256F2
		private static float TearMult
		{
			get
			{
				if (External.ScavHolding)
				{
					return 1.5f;
				}
				if (Pits.InMaw || Slits.Inside)
				{
					return 3f;
				}
				if (!SlimeBog.Stuck)
				{
					return 1f;
				}
				return 1.5f;
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00027528 File Offset: 0x00025728
		public static void Update(Body b, float dt)
		{
			if (!b.alive)
			{
				return;
			}
			float num = (Clothes.Grabbed ? (Plugin.ClothesTearPerMinute.Value / 60f * Clothes.TearMult) : 0f);
			float num2 = (Clothes.LivingSuit ? (Plugin.ClothesAssimilatePerMinute.Value / 60f * (0.6f + 0.2f * (float)Suit.Stage) * Clothes.DebugRate) : 0f);
			float num3 = num2 * 1.5f;
			if (num3 > 0f)
			{
				foreach (Item item in Clothes.InTheWay(b).ToArray<Item>())
				{
					float condition = item.condition;
					item.condition = Mathf.Max(0f, item.condition - num3 * dt);
					if (item.condition <= 0.02f)
					{
						Clothes.PushedOff(b, item);
					}
					else if (Mathf.Floor(condition * 3f) != Mathf.Floor(item.condition * 3f) && (Clothes.ripLine -= 1f) <= 0f)
					{
						Clothes.ripLine = 2f;
						Sound.Play("ripclothing", b.transform.position, false, true, b.transform, 0.5f, 1f, false, false);
						Relief.Say(b, Suit.Slime ? "suit_slime_rip" : "suit_rip");
					}
				}
			}
			if (num > 0f || num2 > 0f)
			{
				foreach (Item item2 in Clothes.Worn(b).ToArray<Item>())
				{
					float condition2 = item2.condition;
					item2.condition = Mathf.Max(0f, item2.condition - (num + num2) * dt);
					if (item2.condition <= 0.02f)
					{
						Clothes.Gone(b, item2, num2 > num);
					}
					else if (num > 0f && Mathf.Floor(condition2 * 4f) != Mathf.Floor(item2.condition * 4f) && (Clothes.ripLine -= 1f) <= 0f)
					{
						Clothes.ripLine = 2f;
						Sound.Play("ripclothing", b.transform.position, false, true, b.transform, 0.6f, 1f, false, false);
						Relief.Say(b, "clothes_rip");
					}
				}
				if (num2 > 0f && (Clothes.eatLine -= dt) <= 0f && Clothes.Worn(b).Any<Item>())
				{
					Clothes.eatLine = Random.Range(40f, 70f);
					if (Suit.Symbiosis && !Clothes.S.symbClothesOk)
					{
						SymbioteVoice.Say(b, "symv_clothes_no", 0f);
					}
					else
					{
						Relief.Say(b, Suit.Slime ? "clothes_dissolve" : "clothes_absorb");
					}
				}
			}
			if ((Clothes.scan -= dt) <= 0f)
			{
				Clothes.scan = 0.5f;
				foreach (Item item3 in Clothes.Worn(b))
				{
					Clothes.Look(b, item3);
				}
				Clothes.Welcome(b);
				if (Suit.Symbiosis && !Clothes.S.symbClothesOk && (Clothes.symbNoLine -= 0.5f) <= 0f && (Clothes.Worn(b).Any<Item>() || Clothes.InTheWay(b).Any<Item>()))
				{
					Clothes.symbNoLine = 60f;
					SymbioteVoice.Say(b, "symv_clothes_no", 0f);
				}
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x000278F4 File Offset: 0x00025AF4
		private static void Gone(Body b, Item i, bool absorbed)
		{
			b.DropWearable(i);
			PlayerCamera main = PlayerCamera.main;
			if (main != null)
			{
				main.UpdateWearables();
			}
			Clothes.SymbioteRefused(b);
			if (absorbed)
			{
				Sfx.PlayMix(b, 0.7f, new string[] { "slime_gloop", "wet_slap" });
				Relief.Say(b, Suit.Slime ? "clothes_dissolved" : "clothes_absorbed");
				Plugin.Log.LogInfo("The living suit took in her " + i.id);
				Object.Destroy(i.gameObject);
				return;
			}
			i.condition = 0.05f;
			Sound.Play("ripclothing", b.transform.position, false, true, b.transform, 0.9f, 1f, false, false);
			Relief.Say(b, "clothes_off");
			Plugin.Log.LogInfo("Her " + i.id + " was torn off her");
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x000279E8 File Offset: 0x00025BE8
		private static void PushedOff(Body b, Item i)
		{
			b.DropWearable(i);
			PlayerCamera main = PlayerCamera.main;
			if (main != null)
			{
				main.UpdateWearables();
			}
			i.condition = 0.05f;
			Clothes.SymbioteRefused(b);
			Sound.Play("ripclothing", b.transform.position, false, true, b.transform, 0.9f, 1f, false, false);
			if (!Suit.Drone)
			{
				Sfx.PlayMix(b, 0.6f, new string[] { "slime_squelch", "wet_slap" });
			}
			Relief.Say(b, Suit.Slime ? "suit_slime_ripoff" : "suit_ripoff");
			Plugin.Log.LogInfo("The living suit tore her " + i.id + " off her");
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x00027AAC File Offset: 0x00025CAC
		public static void Look(Body b, Item i)
		{
			OutfitDef d = Outfits.ById(i.id);
			if (d == null || d.NotWorn)
			{
				return;
			}
			int level = ((i.condition < 0.3f) ? 2 : ((i.condition < 0.6f) ? 1 : 0));
			TornMark tornMark = i.GetComponent<TornMark>() ?? i.gameObject.AddComponent<TornMark>();
			if (tornMark.level == level)
			{
				return;
			}
			tornMark.level = level;
			SpriteRenderer component = i.GetComponent<SpriteRenderer>();
			Sprite sprite = Outfits.TornSprite(d, d.MainTexture, level);
			if (component && sprite)
			{
				component.sprite = sprite;
			}
			Wearable w = i.GetComponent<Wearable>();
			if (w && w.secondaryLimbs != null)
			{
				w.secondaryLimbSprites = w.secondaryLimbs.Select<string, Sprite>((string l, int k) => Outfits.TornSprite(d, Outfits.TextureFor(l), level) ?? w.secondaryLimbSprites[k]).ToArray<Sprite>();
				if (i.transform.parent && i.transform.parent.GetComponent<Limb>())
				{
					w.CreateSprites(b, null);
				}
			}
			Plugin.Log.LogInfo(string.Format("Her {0} is {1} ({2:P0})", i.id, (level == 0) ? "whole" : ((level == 1) ? "ripped" : "in tatters"), i.condition));
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x00027C48 File Offset: 0x00025E48
		public static void AddRecipes()
		{
			if (Recipes.recipes == null || !Plugin.RepairRecipes.Value)
			{
				return;
			}
			OutfitDef[] all = Outfits.All;
			for (int i = 0; i < all.Length; i++)
			{
				OutfitDef d = all[i];
				if (!d.NotWorn && !Recipes.recipes.Any<Recipe>((Recipe r) => r.isRepair && r.result != null && r.result.id == d.Id))
				{
					Recipe recipe = new Recipe
					{
						INT = 2,
						result = new RecipeResult
						{
							id = d.Id
						},
						items = new List<RecipeItem>
						{
							new RecipeItem(0f)
							{
								specificId = d.Id,
								specific = true
							},
							new RecipeItem(0f)
							{
								quality = new CraftingQuality("rippable")
							}
						},
						category = 3,
						isRepair = true
					};
					recipe.index = Recipes.recipes.Count;
					recipe.items[1].ignoredId = "";
					Recipes.recipes.Add(recipe);
				}
			}
		}

		// Token: 0x040001DD RID: 477
		private static readonly HashSet<string> GameCloth = new HashSet<string> { "hoodie", "tornshirt", "armwarmers", "limbwraps", "latexgloves" };

		// Token: 0x040001DE RID: 478
		private static readonly string[] GameSlots = new string[] { "outertorso", "torso", "arms", "wraps", "hands" };

		// Token: 0x040001DF RID: 479
		private static float scan;

		// Token: 0x040001E0 RID: 480
		private static float ripLine;

		// Token: 0x040001E1 RID: 481
		private static float eatLine;

		// Token: 0x040001E2 RID: 482
		public static float DebugRate = 1f;

		// Token: 0x040001E3 RID: 483
		private static readonly string[] CoveredAt3 = new string[]
		{
			"outertorso", "torso", "torsofront", "arms", "wraps", "hands", "knees", "thigh", "thighback", "belt",
			"bandolier"
		};

		// Token: 0x040001E4 RID: 484
		private static readonly string[] CoveredAt4 = new string[] { "feet", "neck", "mouth", "balaclava" };

		// Token: 0x040001E5 RID: 485
		private static bool saidOk;

		// Token: 0x040001E6 RID: 486
		private static float symbNoLine;
	}
}
