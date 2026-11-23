using System;
using BepInEx.Logging;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000F0 RID: 240
	[HarmonyPatch(typeof(Recipes), "SetUpRecipes")]
	internal static class RecipeFixupPatch
	{
		// Token: 0x0600054F RID: 1359 RVA: 0x0005795C File Offset: 0x00055B5C
		[HarmonyPriority(0)]
		private static void Postfix()
		{
			if (Recipes.recipes == null)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < Recipes.recipes.Count; i++)
			{
				Recipe recipe = Recipes.recipes[i];
				if (recipe != null && recipe.items != null)
				{
					recipe.index = i;
					foreach (RecipeItem recipeItem in recipe.items)
					{
						if (recipeItem != null)
						{
							if (!string.IsNullOrEmpty(recipeItem.specificId) && !recipeItem.specific)
							{
								recipeItem.specific = true;
								num++;
							}
							if (recipeItem.ignoredId == null)
							{
								recipeItem.ignoredId = ((recipe.isRepair || recipe.result == null) ? "" : recipe.result.id);
							}
							if (!recipeItem.specific && recipeItem.quality == null)
							{
								recipeItem.quality = new CraftingQuality("rippable");
								num++;
								ManualLogSource log = Plugin.Log;
								string text = "Recipe ";
								RecipeResult result = recipe.result;
								log.LogWarning(text + ((result != null) ? result.id : null) + ": an ingredient had neither an item nor a quality");
							}
						}
					}
				}
			}
			if (num > 0)
			{
				Plugin.Log.LogInfo(string.Format("Recipes: {0} ingredients of the mod's recipes marked as specific items", num));
			}
		}
	}
}
