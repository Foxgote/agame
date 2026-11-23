using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000060 RID: 96
	internal static class Suppressant
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060001DF RID: 479 RVA: 0x00027F90 File Offset: 0x00026190
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x00027F97 File Offset: 0x00026197
		public static bool Active
		{
			get
			{
				return Suppressant.S != null && Suppressant.S.suppressTimer > 0f;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x00027FB3 File Offset: 0x000261B3
		public static float Factor
		{
			get
			{
				if (!Suppressant.Active)
				{
					return 1f;
				}
				return Mathf.Clamp01(Plugin.SuppressLeak.Value);
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00027FD4 File Offset: 0x000261D4
		public static void Update(float dt)
		{
			if (Suppressant.S.suppressTimer <= 0f)
			{
				return;
			}
			Suppressant.S.suppressTimer = Mathf.Max(0f, Suppressant.S.suppressTimer - dt);
			float num = Suppressant.S.corruptionPeak * Mathf.Clamp01(Plugin.CorruptionFloorShare.Value);
			if (Suppressant.S.corruption > num)
			{
				Suppressant.S.corruption = Mathf.Max(num, Suppressant.S.corruption - Plugin.SuppressCorruptionCure.Value / 60f * dt);
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00028068 File Offset: 0x00026268
		public static void RegisterLiquid()
		{
			if (Liquids.Registry == null || Liquids.Registry.ContainsKey("lewd_suppressant"))
			{
				return;
			}
			Dictionary<string, LiquidType> registry = Liquids.Registry;
			string text = "lewd_suppressant";
			LiquidType liquidType = new LiquidType();
			liquidType.localeName = "lewd_suppressant";
			liquidType.color = new Color32(130, 176, 228, byte.MaxValue);
			liquidType.valuePerLiter = 300f;
			liquidType.injectionSickness = 1f;
			LiquidType.OnDrink onDrink;
			if ((onDrink = Suppressant.<>O.<0>__Dose) == null)
			{
				onDrink = (Suppressant.<>O.<0>__Dose = new LiquidType.OnDrink(Suppressant.Dose));
			}
			liquidType.onDrink = onDrink;
			liquidType.qualities = new List<CraftingQuality>();
			registry[text] = liquidType;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00028114 File Offset: 0x00026314
		private static void Dose(float ml, Body body)
		{
			if (!PlayerCamera.main || PlayerCamera.main.body != body || !LewdSettings.On)
			{
				return;
			}
			float num = ml / 5f;
			bool flag = Suppressant.S.suppressTimer > Plugin.SuppressMinutes.Value * 60f * 1.5f;
			Suppressant.S.lust = Mathf.Max(0f, Suppressant.S.lust - 45f * num);
			Suppressant.S.arousal = Mathf.Max(0f, Suppressant.S.arousal - 55f * num);
			Suppressant.S.suppressTimer = Mathf.Min(Plugin.SuppressMinutes.Value * 60f * 4f, Suppressant.S.suppressTimer + Plugin.SuppressMinutes.Value * 60f * num);
			Mood.Change(body, -6f * num);
			body.energy = Mathf.Max(0f, body.energy - 8f * num);
			if (flag)
			{
				Mood.Change(body, -12f * num);
				body.stamina = Mathf.Max(0f, body.stamina - 25f);
			}
			if (Suppressant.EndBrood(body, ml))
			{
				Suppressant.lastLine = Time.time;
			}
			else if (Time.time - Suppressant.lastLine > 15f)
			{
				Suppressant.lastLine = Time.time;
				Relief.Say(body, flag ? "supp_over" : "supp_take");
			}
			Plugin.Log.LogInfo(string.Format("Suppressant: {0:0} ml, lust {1:0}, arousal {2:0}, {3:0.0} min{4}", new object[]
			{
				ml,
				Suppressant.S.lust,
				Suppressant.S.arousal,
				Suppressant.S.suppressTimer / 60f,
				flag ? " (too much)" : ""
			}));
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00028310 File Offset: 0x00026510
		private static bool EndBrood(Body body, float ml)
		{
			if (!Suppressant.S.pregnant || ml < 4f)
			{
				return false;
			}
			if (Suppressant.S.pregProgress >= Plugin.SuppressEndBroodBefore.Value)
			{
				Relief.Say(body, "supp_brood_late");
				Plugin.Log.LogInfo(string.Format("Suppressant: the brood is too far along to end ({0:0}%)", Suppressant.S.pregProgress));
				return true;
			}
			Suppressant.S.pregnant = false;
			Suppressant.S.pregProgress = 0f;
			Suppressant.S.brood = 0;
			Limb limb = body.LimbByName("DownTorso");
			if (limb)
			{
				limb.pain = Mathf.Min(100f, limb.pain + 45f);
				limb.bleedAmount += 1.5f;
			}
			body.stamina = Mathf.Max(0f, body.stamina - 40f);
			Mood.Change(body, -10f);
			Sfx.Play(body, 0.8f, new string[] { "moan" });
			Relief.Say(body, "supp_brood_end");
			Plugin.Log.LogInfo("Suppressant: the brood is ended (cramps, a little blood)");
			return true;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0002843C File Offset: 0x0002663C
		public static void RegisterItem()
		{
			if (Item.GlobalItems == null || Item.GlobalItems.ContainsKey("lewd_suppressant_pills"))
			{
				return;
			}
			ItemInfo itemInfo;
			if (!Item.GlobalItems.TryGetValue("sleepingpills", out itemInfo) || !(itemInfo is LiquidItemInfo))
			{
				return;
			}
			LiquidItemInfo liquidItemInfo = (LiquidItemInfo)AccessTools.Method(typeof(object), "MemberwiseClone", null, null).Invoke(itemInfo, null);
			liquidItemInfo.defaultContents = new List<LiquidStack>
			{
				new LiquidStack("lewd_suppressant", 25f)
			};
			liquidItemInfo.value = 24;
			liquidItemInfo.fullName = Plugin.T("supp_name");
			liquidItemInfo.description = Plugin.T("supp_desc");
			Item.GlobalItems["lewd_suppressant_pills"] = liquidItemInfo;
			if (Plugin.SuppressInGameLoot.Value)
			{
				Counter.AddToPool("lewd_suppressant_pills", new string[] { "drug", "medical" });
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00028528 File Offset: 0x00026728
		public static GameObject Template()
		{
			if (Suppressant.template)
			{
				return Suppressant.template;
			}
			GameObject gameObject = Resources.Load("sleepingpills") as GameObject;
			if (!gameObject)
			{
				return null;
			}
			Suppressant.template = Counter.Clone(gameObject, "lewd_suppressant_pills", ref Suppressant.holder);
			SpriteRenderer component = Suppressant.template.GetComponent<SpriteRenderer>();
			if (component)
			{
				component.color = new Color32(196, 222, byte.MaxValue, byte.MaxValue);
			}
			return Suppressant.template;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x000285B4 File Offset: 0x000267B4
		public static void AddRecipe()
		{
			if (Recipes.recipes != null)
			{
				if (!Recipes.recipes.Any<Recipe>((Recipe r) => r.result != null && r.result.id == "lewd_suppressant"))
				{
					Recipe recipe = new Recipe
					{
						INT = Plugin.SuppressRecipeInt.Value,
						result = new RecipeResult
						{
							id = "lewd_suppressant",
							isLiquid = true,
							resultCondition = 5f
						},
						items = new List<RecipeItem>
						{
							new RecipeItem(0f)
							{
								quality = new CraftingQuality("foliage")
							},
							new RecipeItem(0f)
							{
								quality = new CraftingQuality("foliage")
							},
							new RecipeItem(0.9f)
							{
								isLiquid = true,
								quality = new CraftingQuality("water", 50f)
							}
						},
						category = 2
					};
					recipe.index = Recipes.recipes.Count;
					foreach (RecipeItem recipeItem in recipe.items)
					{
						recipeItem.ignoredId = "lewd_suppressant";
					}
					Recipes.recipes.Add(recipe);
					return;
				}
			}
		}

		// Token: 0x040001E8 RID: 488
		public const string LiquidId = "lewd_suppressant";

		// Token: 0x040001E9 RID: 489
		public const string ItemId = "lewd_suppressant_pills";

		// Token: 0x040001EA RID: 490
		private static GameObject template;

		// Token: 0x040001EB RID: 491
		private static GameObject holder;

		// Token: 0x040001EC RID: 492
		private static float lastLine = -99f;

		// Token: 0x0200018F RID: 399
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400091B RID: 2331
			public static LiquidType.OnDrink <0>__Dose;
		}
	}
}
