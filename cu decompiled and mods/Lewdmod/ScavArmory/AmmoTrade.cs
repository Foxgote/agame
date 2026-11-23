using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000017 RID: 23
	internal static class AmmoTrade
	{
		// Token: 0x06000047 RID: 71 RVA: 0x00004FDC File Offset: 0x000031DC
		public static int CraftedRounds(string id)
		{
			return AmmoTrade.Boxes.FirstOrDefault<ValueTuple<string, int>>(([TupleElementNames(new string[] { "box", "rounds" })] ValueTuple<string, int> x) => x.Item1 == id).Item2;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00005014 File Offset: 0x00003214
		public static void AddRecipes()
		{
			if (Recipes.recipes == null || !Plugin.GunsEnabled.Value)
			{
				return;
			}
			if (Recipes.recipes.Any<Recipe>((Recipe r) => r.result != null && r.result.id == "arm_box9mm"))
			{
				return;
			}
			ValueTuple<string, int>[] boxes = AmmoTrade.Boxes;
			for (int i = 0; i < boxes.Length; i++)
			{
				string item = boxes[i].Item1;
				ValueTuple<string, int>[] array = new ValueTuple<string, int>[]
				{
					new ValueTuple<string, int>("scrapmetal", 2),
					new ValueTuple<string, int>("casing", 7)
				};
				for (int j = 0; j < array.Length; j++)
				{
					ValueTuple<string, int> valueTuple = array[j];
					string metal = valueTuple.Item1;
					int item2 = valueTuple.Item2;
					List<RecipeItem> list = (from _ in Enumerable.Range(0, item2)
						select new RecipeItem(0f)
						{
							specificId = metal,
							specific = true
						}).ToList<RecipeItem>();
					list.Add(new RecipeItem(0f)
					{
						specificId = "flammablepowder",
						specific = true
					});
					list.Add(new RecipeItem(0f)
					{
						quality = new CraftingQuality("hammering", 0.5f),
						destroyItem = false
					});
					AmmoTrade.Add(new Recipe
					{
						INT = Plugin.AmmoCraftInt.Value,
						result = new RecipeResult
						{
							id = item
						},
						items = list,
						category = 3
					});
				}
			}
			Plugin.Log.LogInfo("Ammo recipes: boxes of 9 mm / .45 (15 rounds) and 5.56 / rifle rounds (5), from 2 scrap metal or 7 casings + powder + a tool");
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000051AE File Offset: 0x000033AE
		private static void Add(Recipe r)
		{
			r.index = Recipes.recipes.Count;
			Recipes.recipes.Add(r);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000051CB File Offset: 0x000033CB
		private static bool Purple(string id)
		{
			return id == "arm_box762" || id == "arm_762round";
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000051E7 File Offset: 0x000033E7
		private static float StockWeight(string id)
		{
			if (!AmmoTrade.Purple(id))
			{
				return 1f;
			}
			return 0.35f;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000051FC File Offset: 0x000033FC
		public static void Stock_(TraderScript t)
		{
			if (!t || t.items == null || !Plugin.GunsEnabled.Value || Item.GlobalItems == null)
			{
				return;
			}
			List<string> list = (from id in AmmoTrade.Stock
				where Item.GlobalItems.ContainsKey(id) && t.items.All<TraderItem>((TraderItem x) => x.id != id)
				orderby Mathf.Pow(Random.value, 1f / AmmoTrade.StockWeight(id)) descending
				select id).ToList<string>();
			bool purple = false;
			list = list.Where<string>((string id) => !AmmoTrade.Purple(id) || (!purple && (purple = true))).Take<string>(Random.Range(2, 4)).ToList<string>();
			foreach (string text in list)
			{
				t.items.Add(new TraderItem
				{
					preference = Random.Range(0, 3),
					id = text,
					value = Item.GetItem(text).value
				});
			}
			t.items = t.items.OrderBy<TraderItem, int>((TraderItem x) => x.preference).ToList<TraderItem>();
			if (list.Count > 0)
			{
				Plugin.Log.LogInfo("A trader has ammo for sale: " + string.Join(", ", list));
			}
		}

		// Token: 0x04000036 RID: 54
		[TupleElementNames(new string[] { "box", "rounds" })]
		private static readonly ValueTuple<string, int>[] Boxes = new ValueTuple<string, int>[]
		{
			new ValueTuple<string, int>("arm_box9mm", 15),
			new ValueTuple<string, int>("arm_box45", 15),
			new ValueTuple<string, int>("arm_box556", 5),
			new ValueTuple<string, int>("arm_box762", 5)
		};

		// Token: 0x04000037 RID: 55
		private static readonly string[] Stock = new string[] { "arm_box9mm", "arm_box556", "arm_box45", "arm_45round", "arm_45mag", "boxof12gauge", "arm_box762", "arm_762round" };
	}
}
