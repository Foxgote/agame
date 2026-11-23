using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Recipes : MonoBehaviour
{
	public enum RecipeCategory
	{
		Materials = 0,
		Tools = 1,
		Medicine = 2,
		Utilities = 3,
		Food = 4
	}

	public static List<Recipe> recipes;

	public static Dictionary<CraftingQuality, string> QualityExamples = new Dictionary<CraftingQuality, string>
	{
		{
			new CraftingQuality("heatsource", 1f),
			"campfire"
		},
		{
			new CraftingQuality("firestarter", 1f),
			"firestarter"
		},
		{
			new CraftingQuality("produce", 1f),
			"geofruit"
		},
		{
			new CraftingQuality("flammable", 1f),
			"woodscraps"
		}
	};

	public static int RandomRecipeIndex()
	{
		int num;
		do
		{
			num = Random.Range(0, recipes.Count);
		}
		while (!(recipes[num].result.id != "mercscarf"));
		return num;
	}

	public static void SetUpRecipes()
	{
		recipes = new List<Recipe>();
		recipes.Add(new Recipe
		{
			INT = 1,
			result = new RecipeResult
			{
				id = "rope"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					quality = new CraftingQuality("foliage"),
					minimumCondition = 0f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("foliage"),
					minimumCondition = 0f
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "foliage"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "funguschunk"
				},
				new RecipeItem(0f)
				{
					specificId = "funguschunk"
				},
				new RecipeItem(1f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 5,
			result = new RecipeResult
			{
				id = "string",
				amount = 3
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "rope",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					specificId = "rope",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("cutting"),
					minimumCondition = 0f,
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 2,
			result = new RecipeResult
			{
				id = "canvas"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					quality = new CraftingQuality("foliage"),
					minimumCondition = 0f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("foliage"),
					minimumCondition = 0f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("foliage"),
					minimumCondition = 0f
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 3,
			result = new RecipeResult
			{
				id = "woodscraps"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					quality = new CraftingQuality("foliage"),
					minimumCondition = 0f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("foliage"),
					minimumCondition = 0f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("foliage"),
					minimumCondition = 0f
				}
			}
		});
		recipes.Add(new Recipe
		{
			INT = 0,
			result = new RecipeResult
			{
				id = "rippeddressing"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					quality = new CraftingQuality("rippable"),
					destroyItem = false,
					minimumCondition = 0f
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 1,
			result = new RecipeResult
			{
				id = "sterilizedbandage"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					quality = new CraftingQuality("dressing")
				},
				new RecipeItem
				{
					isLiquid = true,
					quality = new CraftingQuality("disinfectant", 20f)
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 4,
			result = new RecipeResult
			{
				id = "biochem",
				isLiquid = true,
				resultCondition = 10f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "glowplantfruit",
					minimumCondition = 0f
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "biochem",
				isLiquid = true,
				resultCondition = 25f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(250f)
				{
					specificId = "lumalgae",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("heatsource"),
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 7,
			result = new RecipeResult
			{
				id = "opium",
				isLiquid = true,
				resultCondition = 50f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "mushpear",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					specificId = "mushpear",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					isLiquid = true,
					specificId = "biochem",
					minimumCondition = 10f
				},
				new RecipeItem
				{
					isLiquid = true,
					quality = new CraftingQuality("water", 50f)
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "morphine",
				isLiquid = true,
				resultCondition = 50f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(75f)
				{
					specificId = "opium",
					isLiquid = true
				},
				new RecipeItem(10f)
				{
					isLiquid = true,
					specificId = "biochem"
				},
				new RecipeItem
				{
					isLiquid = true,
					quality = new CraftingQuality("water", 15f)
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 11,
			result = new RecipeResult
			{
				id = "fentanyl",
				isLiquid = true,
				resultCondition = 10f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(50f)
				{
					specificId = "morphine",
					isLiquid = true
				},
				new RecipeItem(25f)
				{
					isLiquid = true,
					specificId = "biochem"
				},
				new RecipeItem
				{
					isLiquid = true,
					quality = new CraftingQuality("water", 25f)
				},
				new RecipeItem(0f)
				{
					specificId = "flammablepowder"
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "painkillers",
				isLiquid = true,
				resultCondition = 200f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "reliefcrystalshard"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(20f)
				{
					isLiquid = true,
					specificId = "biochem"
				},
				new RecipeItem
				{
					isLiquid = true,
					quality = new CraftingQuality("water", 50f)
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 18,
			result = new RecipeResult
			{
				id = "neuralbooster"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(50f)
				{
					isLiquid = true,
					specificId = "biochem"
				},
				new RecipeItem(80f)
				{
					isLiquid = true,
					specificId = "braingrow"
				},
				new RecipeItem(30f)
				{
					isLiquid = true,
					specificId = "mindwipe"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 6,
			result = new RecipeResult
			{
				id = "canteen"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "plasticchunk",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					specificId = "plasticchunk",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					specificId = "rope",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					isLiquid = true,
					specificId = "biochem",
					minimumCondition = 10f
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 5,
			result = new RecipeResult
			{
				id = "foliagebag"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "rope",
					minimumCondition = 0f
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 7,
			result = new RecipeResult
			{
				id = "slingbag"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "foliagebag",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					specificId = "string",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					specificId = "rope",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					specificId = "rope",
					minimumCondition = 0f
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 6,
			result = new RecipeResult
			{
				id = "scrapcube"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "scrapmetal",
					minimumCondition = 0.9f
				},
				new RecipeItem
				{
					specificId = "biochem",
					isLiquid = true,
					minimumCondition = 10f
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 5,
			result = new RecipeResult
			{
				id = "scrappanel",
				amount = 5
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "scrapcube",
					minimumCondition = 0.9f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("cutting"),
					minimumCondition = 0f,
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 5,
			result = new RecipeResult
			{
				id = "scraptube",
				amount = 2
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "scrapcube",
					minimumCondition = 0.9f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("cutting"),
					minimumCondition = 0f,
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 5,
			result = new RecipeResult
			{
				id = "nails",
				amount = 2
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "scrapcube"
				},
				new RecipeItem
				{
					quality = new CraftingQuality("cutting", 1f),
					minimumCondition = 0f,
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 4,
			result = new RecipeResult
			{
				id = "woodcube"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "woodscraps"
				},
				new RecipeItem(0f)
				{
					specificId = "woodscraps"
				},
				new RecipeItem(0f)
				{
					specificId = "woodscraps"
				},
				new RecipeItem
				{
					specificId = "biochem",
					isLiquid = true,
					minimumCondition = 10f
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 4,
			result = new RecipeResult
			{
				id = "woodpanel",
				amount = 5
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "woodcube",
					minimumCondition = 0.9f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("cutting"),
					minimumCondition = 0f,
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 4,
			result = new RecipeResult
			{
				id = "stick",
				amount = 2
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "woodcube",
					minimumCondition = 0.9f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("cutting"),
					minimumCondition = 0f,
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 4,
			result = new RecipeResult
			{
				id = "flimsyknife"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "scrapmetal",
					minimumCondition = 0.9f
				},
				new RecipeItem
				{
					specificId = "rope",
					minimumCondition = 0f
				},
				new RecipeItem(0f)
				{
					specificId = "woodcube",
					minimumCondition = 0.9f
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "processedcopper"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "rawcopper",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					isLiquid = true,
					specificId = "biochem",
					minimumCondition = 40f
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "bundleofwires",
				amount = 6
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "processedcopper",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					specificId = "plasticchunk",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					specificId = "plasticchunk",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					specificId = "plasticchunk",
					minimumCondition = 0f
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 15,
			result = new RecipeResult
			{
				id = "titaniumslab"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "ilmenitechunk",
					minimumCondition = 0f
				},
				new RecipeItem
				{
					isLiquid = true,
					specificId = "biochem",
					minimumCondition = 50f
				},
				new RecipeItem
				{
					isLiquid = true,
					quality = new CraftingQuality("water", 100f)
				},
				new RecipeItem
				{
					quality = new CraftingQuality("hammering", 4f),
					destroyItem = false,
					minimumCondition = 0f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("cutting", 4f),
					destroyItem = false,
					minimumCondition = 0f
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 15,
			result = new RecipeResult
			{
				id = "titaniumsheet",
				amount = 5
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "titaniumslab"
				},
				new RecipeItem
				{
					isLiquid = true,
					specificId = "biochem",
					minimumCondition = 10f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("cutting", 4f),
					destroyItem = false,
					minimumCondition = 0f
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 15,
			result = new RecipeResult
			{
				id = "titaniumrod",
				amount = 2
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "titaniumslab"
				},
				new RecipeItem
				{
					isLiquid = true,
					specificId = "biochem",
					minimumCondition = 10f
				},
				new RecipeItem
				{
					quality = new CraftingQuality("cutting", 4f),
					destroyItem = false,
					minimumCondition = 0f
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 2,
			result = new RecipeResult
			{
				id = "alienblood",
				isLiquid = true,
				resultCondition = 50f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					minimumCondition = 0f,
					specificId = "bloodsac"
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "saline",
				isLiquid = true,
				resultCondition = 175f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(150f)
				{
					specificId = "water",
					isLiquid = true
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("blood", 15f),
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "antivenom",
				isLiquid = true,
				resultCondition = 50f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "venomgland",
					minimumCondition = 0f
				},
				new RecipeItem(5f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("blood", 25f),
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("disinfectant", 10f),
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "blood",
				isLiquid = true,
				resultCondition = 100f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(99.99f)
				{
					specificId = "alienblood",
					isLiquid = true
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("water", 40f),
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "makeshiftwrench",
				resultCondition = 1f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem
				{
					specificId = "biochem",
					isLiquid = true,
					minimumCondition = 10f
				},
				new RecipeItem
				{
					quality = "hammering",
					minimumCondition = 0f,
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "wrench",
				resultCondition = 1f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "makeshiftwrench",
					minimumCondition = 0f
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem
				{
					specificId = "biochem",
					isLiquid = true,
					minimumCondition = 10f
				},
				new RecipeItem
				{
					quality = "hammering",
					minimumCondition = 0f,
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "crudecleaver",
				resultCondition = 1f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "stick"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem
				{
					quality = "nails",
					minimumCondition = 0f,
					destroyItem = false
				},
				new RecipeItem
				{
					quality = "hammering",
					minimumCondition = 0f,
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "machete",
				resultCondition = 1f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "crudecleaver"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(30f)
				{
					isLiquid = true,
					specificId = "biochem"
				},
				new RecipeItem
				{
					quality = "hammering",
					minimumCondition = 0f,
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "flammablepowder"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "stonefruitopen"
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				},
				new RecipeItem(5f)
				{
					isLiquid = true,
					specificId = "biochem"
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "casing",
				amount = 5
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "9mmround",
				amount = 5
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "flammablepowder"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("hammering", 0.5f),
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "556round",
				amount = 4
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "flammablepowder"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("hammering", 0.5f),
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "12gauge",
				amount = 3
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "casing"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "flammablepowder"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("hammering", 0.5f),
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "magazinebase"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					quality = "nails",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "smallmagazine"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "magazinebase"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "riflemagazine"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "magazinebase"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "boxof12gauge"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "dynamite"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "flammablepowder"
				},
				new RecipeItem(0f)
				{
					specificId = "flammablepowder"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 4,
			result = new RecipeResult
			{
				id = "largecarcass"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "carcass"
				},
				new RecipeItem(0f)
				{
					specificId = "carcass"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "circuitboard"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "circuitboard",
					noMaxCondition = true
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				}
			},
			category = RecipeCategory.Materials,
			isRepair = true
		});
		recipes.Add(new Recipe
		{
			INT = 11,
			result = new RecipeResult
			{
				id = "smallbattery",
				amount = 2
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "processedcopper"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "mediumbattery"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "smallbattery"
				},
				new RecipeItem(0f)
				{
					specificId = "smallbattery"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 13,
			result = new RecipeResult
			{
				id = "largebattery"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "mediumbattery"
				},
				new RecipeItem(0f)
				{
					specificId = "processedcopper"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "flashlight"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "lightbulb"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem(0f)
				{
					quality = "nails",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "headlamp"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "flashlight"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					quality = "nails",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 11,
			result = new RecipeResult
			{
				id = "lcdscreen"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					quality = "nails",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "flexiglass"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 11,
			result = new RecipeResult
			{
				id = "lightbulb"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem
				{
					specificId = "scrapcube"
				},
				new RecipeItem(0f)
				{
					quality = "nails",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 6,
			result = new RecipeResult
			{
				id = "limbwraps"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					quality = "dressing"
				},
				new RecipeItem
				{
					quality = "dressing"
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 4,
			result = new RecipeResult
			{
				id = "makeshiftheadlamp"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "carcass"
				},
				new RecipeItem(0f)
				{
					specificId = "glowplantfruit"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 13,
			result = new RecipeResult
			{
				id = "bikehelmet"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "makeshifthelmet"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					quality = "nails",
					destroyItem = false
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 17,
			result = new RecipeResult
			{
				id = "riothelmet"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "bikehelmet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(40f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "makeshifthelmet"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "largecarcass"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 7,
			result = new RecipeResult
			{
				id = "makeshiftdiggingtool"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "flimsyknife"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "stick"
				},
				new RecipeItem(0f)
				{
					specificId = "stick"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "makeshiftrifle"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "magazinebase"
				},
				new RecipeItem(0f)
				{
					quality = "nails",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 16,
			result = new RecipeResult
			{
				id = "minilaserdrill"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem(0f)
				{
					specificId = "flashlight"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					quality = "nails",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 5,
			result = new RecipeResult
			{
				id = "bandage"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "rippeddressing"
				},
				new RecipeItem
				{
					specificId = "rippeddressing"
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "lantern"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "emissivecrystalshard"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					quality = "nails",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "terrainscanner"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "lcdscreen"
				},
				new RecipeItem(0f)
				{
					quality = "nails"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 11,
			result = new RecipeResult
			{
				id = "geigercounter"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem
				{
					specificId = "scrapcube"
				},
				new RecipeItem(0f)
				{
					specificId = "lcdscreen"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "lockpickingkit",
				resultCondition = 0.1f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("hammering", 0.2f),
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("cutting", 0.2f),
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "scubadivinggear"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "oxygencrystalshard"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "pickaxe"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "makeshiftdiggingtool"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					quality = "nails"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 14,
			result = new RecipeResult
			{
				id = "scaffoldingpack"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "titaniumslab"
				},
				new RecipeItem
				{
					specificId = "titaniumslab"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 13,
			result = new RecipeResult
			{
				id = "bigpack"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "woodsandals"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 16,
			result = new RecipeResult
			{
				id = "tacticalboots"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(30f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 15,
			result = new RecipeResult
			{
				id = "tacticalgloves"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 11,
			result = new RecipeResult
			{
				id = "duffelbag"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "brokenbag"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "materialpouch"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("nails"),
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "belt",
				resultCondition = 0.5f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 15,
			result = new RecipeResult
			{
				id = "traumarig",
				resultCondition = 1f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "belt"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(30f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 2,
			result = new RecipeResult
			{
				id = "bowlofcereal",
				dontDrainResultLiquid = true,
				amount = 3
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "cereal"
				},
				new RecipeItem(500f)
				{
					specificId = "milk",
					isLiquid = true
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "fat",
				isLiquid = true,
				resultCondition = 100f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0.2f)
				{
					quality = "meat"
				},
				new RecipeItem(0f)
				{
					quality = "heatsource",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("cutting", 0.5f),
					destroyItem = false
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "soap",
				isLiquid = true,
				resultCondition = 100f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					quality = new CraftingQuality("fat", 50f),
					isLiquid = true
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("water", 40f),
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "clottingmush"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("blood", 50f),
					isLiquid = true
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					specificId = "flammablepowder"
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "naltrexone",
				isLiquid = true,
				resultCondition = 200f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "digestioncrystalshard"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					isLiquid = true,
					quality = new CraftingQuality("opiate", 50f)
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "antidepressants",
				isLiquid = true,
				resultCondition = 200f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "soothingcrystalshard"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					specificId = "flammablepowder"
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "autoinjector"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Materials
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "chestdrain"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "autoinjector"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 14,
			result = new RecipeResult
			{
				id = "handcrank"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "processedcopper"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "antisepticmush"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "bulbskin"
				},
				new RecipeItem(0f)
				{
					specificId = "roselight"
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 14,
			result = new RecipeResult
			{
				id = "plasticbandage"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0.5f)
				{
					quality = "dressing"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "tourniquet",
				resultCondition = 0.5f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 14,
			result = new RecipeResult
			{
				id = "boneweldingtool"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(10f)
				{
					specificId = "lrdserum",
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "tweezers",
				resultCondition = 0.1f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 12,
			result = new RecipeResult
			{
				id = "bloodbag"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem(0f)
				{
					specificId = "autoinjector"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 11,
			result = new RecipeResult
			{
				id = "saline"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "flexiglass"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "disinfectant",
				isLiquid = true,
				resultCondition = 100f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(25f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(50f)
				{
					specificId = "soap",
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("blood", 25f),
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("water", 50f),
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "reliefcream",
				isLiquid = true,
				resultCondition = 100f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "flammablepowder"
				},
				new RecipeItem
				{
					quality = new CraftingQuality("water", 50f),
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("blood", 25f),
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("opiate", 50f),
					isLiquid = true
				},
				new RecipeItem(25f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "splint",
				resultCondition = 0.8f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					specificId = "woodpanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("nails"),
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("hammering"),
					destroyItem = false
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "bruisekit"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0.5f)
				{
					quality = "dressing"
				},
				new RecipeItem
				{
					quality = new CraftingQuality("opiate", 25f),
					isLiquid = true
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 6,
			result = new RecipeResult
			{
				id = "carcasssplint"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "largecarcass"
				},
				new RecipeItem(0f)
				{
					specificId = "carcass"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "makeshiftlrd"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					quality = "nails",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 15,
			result = new RecipeResult
			{
				id = "lrd"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "makeshiftlrd"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(25f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					specificId = "autoinjector"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 11,
			result = new RecipeResult
			{
				id = "lrdserum",
				isLiquid = true,
				resultCondition = 25f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					quality = new CraftingQuality("disinfectant", 25f),
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("water", 50f),
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("opiate", 25f),
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("blood", 25f),
					isLiquid = true
				},
				new RecipeItem(25f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 2,
			result = new RecipeResult
			{
				id = "producejuice",
				resultCondition = 250f,
				isLiquid = true
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0.5f)
				{
					quality = "produce"
				},
				new RecipeItem(0.5f)
				{
					quality = "produce"
				},
				new RecipeItem(0.5f)
				{
					quality = "produce"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("hammering", 0.1f),
					destroyItem = false
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 0,
			result = new RecipeResult
			{
				id = "milk",
				resultCondition = 500f,
				isLiquid = true
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("water", 470f),
					isLiquid = true
				},
				new RecipeItem(30f)
				{
					specificId = "powderedmilk",
					isLiquid = true
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 4,
			result = new RecipeResult
			{
				id = "refinedjuice",
				resultCondition = 200f,
				isLiquid = true
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(250f)
				{
					specificId = "producejuice",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("heatsource"),
					destroyItem = false
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "drillrepairkit"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					specificId = "scrappanel"
				},
				new RecipeItem(0f)
				{
					quality = "nails",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "bundleofwires"
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 14,
			result = new RecipeResult
			{
				id = "procoagulant",
				isLiquid = true,
				resultCondition = 100f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "clottingmush"
				},
				new RecipeItem
				{
					quality = new CraftingQuality("blood", 50f),
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					specificId = "bunchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "bunchunk"
				},
				new RecipeItem(25f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 7,
			result = new RecipeResult
			{
				id = "spraybottle"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "canteen"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "syringe"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "canteen"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 8,
			result = new RecipeResult
			{
				id = "firestarter"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "stick"
				},
				new RecipeItem(0f)
				{
					specificId = "stick"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("foliage")
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 4,
			result = new RecipeResult
			{
				id = "campfire"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0.01f)
				{
					quality = new CraftingQuality("firestarter"),
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("flammable")
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("flammable")
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("flammable")
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 1,
			result = new RecipeResult
			{
				id = "water",
				isLiquid = true,
				resultCondition = 50f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("heatsource", 0.1f),
					destroyItem = false
				},
				new RecipeItem(50f)
				{
					specificId = "groundwater",
					isLiquid = true
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 7,
			result = new RecipeResult
			{
				id = "charcoal",
				amount = 3
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					quality = "heatsource",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					specificId = "droppings"
				},
				new RecipeItem(10f)
				{
					specificId = "biochem",
					isLiquid = true
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 7,
			result = new RecipeResult
			{
				id = "bread"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					quality = "heatsource",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "flour"
				},
				new RecipeItem
				{
					quality = new CraftingQuality("water", 150f),
					isLiquid = true
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 5,
			result = new RecipeResult
			{
				id = "pancake"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					quality = "heatsource",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "flour"
				},
				new RecipeItem
				{
					quality = new CraftingQuality("water", 100f),
					isLiquid = true
				},
				new RecipeItem(100f)
				{
					specificId = "sap",
					isLiquid = true
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 6,
			result = new RecipeResult
			{
				id = "ryeflour"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "ryebulb"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("hammering", 0.5f),
					destroyItem = false
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 5,
			result = new RecipeResult
			{
				id = "torch",
				resultCondition = 0f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "stick"
				},
				new RecipeItem(0f)
				{
					specificId = "stick"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 5,
			result = new RecipeResult
			{
				id = "torch",
				resultCondition = 1f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "torch",
					noMaxCondition = true
				},
				new RecipeItem(0f)
				{
					quality = "heatsource",
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities,
			isRepair = true
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "nutrientbar"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					quality = "heatsource",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("flour", 1f)
				},
				new RecipeItem(0.5f)
				{
					quality = new CraftingQuality("produce")
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("fat", 25f),
					isLiquid = true
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 6,
			result = new RecipeResult
			{
				id = "pemmican"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					quality = "heatsource",
					destroyItem = false
				},
				new RecipeItem(0.2f)
				{
					quality = "meat"
				},
				new RecipeItem(0.5f)
				{
					quality = "produce"
				},
				new RecipeItem
				{
					quality = new CraftingQuality("fat", 75f),
					isLiquid = true
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 7,
			result = new RecipeResult
			{
				id = "foliagemeal"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					quality = "heatsource",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "foliage"
				},
				new RecipeItem(0f)
				{
					quality = "foliage"
				},
				new RecipeItem(0f)
				{
					quality = "foliage"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("produce")
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 6,
			result = new RecipeResult
			{
				id = "burger"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					quality = "heatsource",
					destroyItem = false
				},
				new RecipeItem(0.2f)
				{
					quality = "meat"
				},
				new RecipeItem(0.2f)
				{
					specificId = "bread"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("condiment", 25f),
					isLiquid = true
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 9,
			result = new RecipeResult
			{
				id = "soup",
				isLiquid = true,
				resultCondition = 300f
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					quality = "heatsource",
					destroyItem = false
				},
				new RecipeItem
				{
					quality = new CraftingQuality("water", 200f),
					isLiquid = true
				},
				new RecipeItem
				{
					quality = new CraftingQuality("fat", 25f),
					isLiquid = true
				},
				new RecipeItem(0.2f)
				{
					quality = "meat"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("cutting", 0.5f),
					destroyItem = false
				}
			},
			category = RecipeCategory.Food
		});
		recipes.Add(new Recipe
		{
			INT = 14,
			result = new RecipeResult
			{
				id = "icepack"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0.5f)
				{
					specificId = "frigiantfruit"
				},
				new RecipeItem(0.5f)
				{
					specificId = "frigiantfruit"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("water", 200f),
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 10,
			result = new RecipeResult
			{
				id = "scarf"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "canvas"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				},
				new RecipeItem(0f)
				{
					specificId = "string"
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 99,
			result = new RecipeResult
			{
				id = "mercscarf"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "scarf"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem(0f)
				{
					specificId = "regenerationchip"
				},
				new RecipeItem(0f)
				{
					specificId = "capacitorchip"
				},
				new RecipeItem(0f)
				{
					specificId = "swapchip"
				},
				new RecipeItem(0f)
				{
					specificId = "rejuvenationchip"
				},
				new RecipeItem(0f)
				{
					specificId = "aichip"
				},
				new RecipeItem(0f)
				{
					specificId = "kineticchip"
				}
			},
			category = RecipeCategory.Utilities
		});
		recipes.Add(new Recipe
		{
			INT = 16,
			result = new RecipeResult
			{
				id = "titaniumpickaxe"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "pickaxe"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumrod"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumrod"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 16,
			result = new RecipeResult
			{
				id = "titaniummachete"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "machete"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 16,
			result = new RecipeResult
			{
				id = "titaniummultitool"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumrod"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Tools
		});
		recipes.Add(new Recipe
		{
			INT = 7,
			result = new RecipeResult
			{
				id = "peglimb"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "woodcube",
					minimumCondition = 0.9f
				},
				new RecipeItem(0f)
				{
					specificId = "woodcube",
					minimumCondition = 0.9f
				},
				new RecipeItem(0f)
				{
					specificId = "stick"
				},
				new RecipeItem(0f)
				{
					specificId = "stick"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(1f)
				{
					quality = "nails"
				},
				new RecipeItem(20f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 11,
			result = new RecipeResult
			{
				id = "steelprosthesis"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "scrapcube"
				},
				new RecipeItem
				{
					specificId = "scrapcube"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "scraptube"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(1f)
				{
					quality = "nails"
				},
				new RecipeItem(30f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 16,
			result = new RecipeResult
			{
				id = "peo"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem
				{
					specificId = "titaniumslab"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "titaniumsheet"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem(0f)
				{
					specificId = "plasticchunk"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem
				{
					specificId = "circuitboard"
				},
				new RecipeItem
				{
					specificId = "bundleofwires"
				},
				new RecipeItem(1f)
				{
					quality = "nails"
				},
				new RecipeItem(30f)
				{
					specificId = "biochem",
					isLiquid = true
				},
				new RecipeItem(0f)
				{
					quality = "hammering",
					destroyItem = false
				},
				new RecipeItem(0f)
				{
					quality = "cutting",
					destroyItem = false
				}
			},
			category = RecipeCategory.Medicine
		});
		recipes.Add(new Recipe
		{
			INT = 7,
			result = new RecipeResult
			{
				id = "climbingrope"
			},
			items = new List<RecipeItem>
			{
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					specificId = "rope"
				},
				new RecipeItem(0f)
				{
					quality = new CraftingQuality("nails", 0.5f),
					destroyItem = false
				}
			},
			category = RecipeCategory.Utilities
		});
		for (int i = 0; i < recipes.Count; i++)
		{
			recipes[i].index = i;
			foreach (RecipeItem item in recipes[i].items)
			{
				if (!string.IsNullOrEmpty(item.specificId))
				{
					item.specific = true;
				}
				item.ignoredId = (recipes[i].isRepair ? "" : recipes[i].result.id);
			}
		}
	}

	public static List<Recipe> GetVisibleRecipes(Item filterItem = null)
	{
		if (!filterItem)
		{
			return recipes.Where((Recipe r) => r.visible).ToList();
		}
		return recipes.Where((Recipe r) =>
		{
			foreach (RecipeItem item in r.items)
			{
				if (item.DoesUseItemType(filterItem))
				{
					return r.visible;
				}
			}
			return false;
		}).ToList();
	}
}
