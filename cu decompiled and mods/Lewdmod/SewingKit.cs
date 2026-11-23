using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000FA RID: 250
	internal static class SewingKit
	{
		// Token: 0x0600057D RID: 1405 RVA: 0x000595E4 File Offset: 0x000577E4
		public static void RegisterItem()
		{
			ItemInfo itemInfo;
			if (Item.GlobalItems == null || Item.GlobalItems.ContainsKey("lewd_sewingkit") || !Item.GlobalItems.TryGetValue("rag", out itemInfo))
			{
				return;
			}
			ItemInfo itemInfo2 = (ItemInfo)AccessTools.Method(typeof(object), "MemberwiseClone", null, null).Invoke(itemInfo, null);
			itemInfo2.category = "utility";
			itemInfo2.usable = true;
			itemInfo2.usableWithLMB = false;
			itemInfo2.usableOnLimb = false;
			itemInfo2.useLimbAction = null;
			itemInfo2.combineable = false;
			itemInfo2.destroyAtZeroCondition = true;
			itemInfo2.scaleWeightWithCondition = false;
			itemInfo2.weight = 0.3f;
			itemInfo2.value = 14;
			itemInfo2.qualities = new List<CraftingQuality>();
			itemInfo2.tags = "";
			itemInfo2.SetTags();
			ItemInfo itemInfo3 = itemInfo2;
			ItemInfo.Use use;
			if ((use = SewingKit.<>O.<0>__Use) == null)
			{
				use = (SewingKit.<>O.<0>__Use = new ItemInfo.Use(SewingKit.Use));
			}
			itemInfo3.useAction = use;
			itemInfo2.fullName = Plugin.T("sewkit_name");
			itemInfo2.description = Plugin.T("sewkit_desc");
			Item.GlobalItems["lewd_sewingkit"] = itemInfo2;
			Counter.AddToPool("lewd_sewingkit", new string[] { "utility" });
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x00059714 File Offset: 0x00057914
		public static GameObject Template()
		{
			if (SewingKit.template)
			{
				return SewingKit.template;
			}
			GameObject gameObject = Resources.Load("rag") as GameObject;
			if (!gameObject)
			{
				return null;
			}
			SewingKit.template = Counter.Clone(gameObject, "lewd_sewingkit", ref SewingKit.holder);
			SpriteRenderer component = SewingKit.template.GetComponent<SpriteRenderer>();
			Texture2D texture2D = SewingKit.LoadArt();
			if (component && texture2D)
			{
				float num = (component.sprite ? component.sprite.pixelsPerUnit : 16f);
				Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), num);
				sprite.name = "lewd_sewingkit";
				component.sprite = sprite;
				component.color = Color.white;
				foreach (Collider2D collider2D in SewingKit.template.GetComponents<Collider2D>())
				{
					if (!(collider2D is BoxCollider2D))
					{
						Object.DestroyImmediate(collider2D);
					}
				}
				BoxCollider2D boxCollider2D = SewingKit.template.GetComponent<BoxCollider2D>() ?? SewingKit.template.AddComponent<BoxCollider2D>();
				boxCollider2D.size = new Vector2((float)texture2D.width / num, (float)texture2D.height / num);
				boxCollider2D.offset = Vector2.zero;
			}
			else if (component)
			{
				component.color = new Color32(232, 196, 150, byte.MaxValue);
			}
			return SewingKit.template;
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x000598A8 File Offset: 0x00057AA8
		private static Texture2D LoadArt()
		{
			string text = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "items", "lewd_sewingkit.png");
			if (!File.Exists(text))
			{
				Plugin.Log.LogWarning("Sewing kit: no picture at " + text);
				return null;
			}
			Texture2D texture2D = new Texture2D(2, 2, 4, false)
			{
				filterMode = 0,
				wrapMode = 1
			};
			if (!ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text)))
			{
				return null;
			}
			return texture2D;
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x00059928 File Offset: 0x00057B28
		public static void AddRecipe()
		{
			if (Recipes.recipes != null)
			{
				if (!Recipes.recipes.Any<Recipe>((Recipe r) => r.result != null && r.result.id == "lewd_sewingkit"))
				{
					Recipe recipe = new Recipe
					{
						INT = 2,
						result = new RecipeResult
						{
							id = "lewd_sewingkit"
						},
						items = new List<RecipeItem>
						{
							new RecipeItem(0f)
							{
								quality = new CraftingQuality("rippable")
							},
							new RecipeItem(0f)
							{
								specificId = "string",
								specific = true
							}
						},
						category = 3
					};
					recipe.index = Recipes.recipes.Count;
					recipe.items[0].ignoredId = "";
					Recipes.recipes.Add(recipe);
					return;
				}
			}
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x00059A10 File Offset: 0x00057C10
		private static void Use(Body b, Item kit)
		{
			if (!b || !kit)
			{
				return;
			}
			IEnumerable<Item> enumerable = from i in Clothes.Worn(b)
				where i && Outfits.ById(i.id) != null
				select i;
			IEnumerable<Item> enumerable2 = from i in b.GetAllItemsThorough()
				where i && i != kit && Outfits.ById(i.id) != null
				select i;
			Item item = (from i in enumerable.Concat<Item>(enumerable2)
				where i.condition < 0.97f
				orderby i.condition
				select i).FirstOrDefault<Item>();
			if (!item)
			{
				Relief.Say(b, "sewkit_nothing");
				return;
			}
			item.SetCondition(1f);
			Clothes.Look(b, item);
			kit.condition -= 0.25f;
			Sound.Play("splint", b.transform.position, false, true, null, 1f, 1f, false, false);
			Relief.Say(b, "sewkit_done");
			Plugin.Log.LogInfo(string.Format("Sewing kit: mended her {0} (kit {1:P0} left)", item.id, Mathf.Max(0f, kit.condition)));
			if (kit.condition <= 0.01f)
			{
				Object.Destroy(kit.gameObject);
			}
		}

		// Token: 0x0400063C RID: 1596
		public const string ItemId = "lewd_sewingkit";

		// Token: 0x0400063D RID: 1597
		private static GameObject template;

		// Token: 0x0400063E RID: 1598
		private static GameObject holder;

		// Token: 0x02000214 RID: 532
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000A95 RID: 2709
			public static ItemInfo.Use <0>__Use;
		}
	}
}
