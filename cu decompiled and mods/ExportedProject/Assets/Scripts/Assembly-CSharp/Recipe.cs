using System.Collections.Generic;
using UnityEngine;

public class Recipe
{
	public bool specialKnown;

	public int INT;

	public List<RecipeItem> items;

	public RecipeResult result;

	public bool hasMadeBefore;

	public Recipes.RecipeCategory category;

	public bool isRepair;

	public int index;

	public (Sprite, Color) resultSprite
	{
		get
		{
			if (result.isLiquid)
			{
				return (Resources.Load<Sprite>("Sprites/droplet"), Liquids.Registry[result.id].color);
			}
			return (Resources.Load<GameObject>(result.id).GetComponent<SpriteRenderer>().sprite, Color.white);
		}
	}

	public string fullName
	{
		get
		{
			string text = (hasMadeBefore ? "" : "<sprite index=25>");
			if (isRepair)
			{
				text += Locale.GetOther("craftrepair");
			}
			if (result.isLiquid)
			{
				text += Locale.GetOther(result.id);
				text += $" ({(int)result.resultCondition}mL)";
			}
			else
			{
				text += Locale.GetItem(result.id);
				if (result.resultCondition != 1f)
				{
					text += $" ({Mathf.RoundToInt(result.resultCondition * 100f)}%)";
				}
				if (result.amount > 1)
				{
					text += $" (x{result.amount})";
				}
			}
			return text;
		}
	}

	public string simpleName
	{
		get
		{
			string text = "";
			if (result.isLiquid)
			{
				text += Locale.GetOther(result.id);
				text += $" ({(int)result.resultCondition}mL)";
			}
			else
			{
				text += Locale.GetItem(result.id);
				if (result.resultCondition != 1f)
				{
					text += $" ({Mathf.RoundToInt(result.resultCondition * 100f)}%)";
				}
				if (result.amount > 1)
				{
					text += $" (x{result.amount})";
				}
			}
			return text;
		}
	}

	public string description
	{
		get
		{
			if (result.isLiquid)
			{
				return Locale.GetOther(result.id + "dsc");
			}
			return Locale.GetItem(result.id + "dsc");
		}
	}

	public bool visible
	{
		get
		{
			if (!specialKnown)
			{
				return PlayerCamera.main.body.skills.INT - INT >= -3;
			}
			return true;
		}
	}

	public List<Item> GetItemsForRecipe()
	{
		List<Item> list = new List<Item>();
		List<Item> list2 = new List<Item>();
		List<Item> allItemsThorough = PlayerCamera.main.body.GetAllItemsThorough();
		Collider2D[] array = Physics2D.OverlapCircleAll(PlayerCamera.main.body.transform.position, 10f, LayerMask.GetMask("Item"));
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].TryGetComponent<Item>(out var component) && PlayerCamera.main.body.DoPickupCheck(component, noAlerts: true))
			{
				allItemsThorough.Add(component);
			}
		}
		foreach (RecipeItem item in items)
		{
			Item matchingItem = item.GetMatchingItem(allItemsThorough, list2);
			if (matchingItem != null)
			{
				list.Add(matchingItem);
				if (item.destroyItem && !item.isLiquid)
				{
					list2.Add(matchingItem);
				}
				continue;
			}
			return null;
		}
		return list;
	}

	public List<Item> GetItemsForRecipeThorough()
	{
		List<Item> list = new List<Item>();
		List<Item> list2 = new List<Item>();
		List<Item> allItemsThorough = PlayerCamera.main.body.GetAllItemsThorough();
		Collider2D[] array = Physics2D.OverlapCircleAll(PlayerCamera.main.body.transform.position, 10f, LayerMask.GetMask("Item"));
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].TryGetComponent<Item>(out var component) && PlayerCamera.main.body.DoPickupCheck(component, noAlerts: true))
			{
				allItemsThorough.Add(component);
			}
		}
		foreach (RecipeItem item in items)
		{
			Item matchingItem = item.GetMatchingItem(allItemsThorough, list2);
			if (matchingItem != null)
			{
				list.Add(matchingItem);
				if (item.destroyItem && !item.isLiquid)
				{
					list2.Add(matchingItem);
				}
			}
			else
			{
				list.Add(null);
			}
		}
		return list;
	}

	public void TryMake()
	{
		List<Item> itemsForRecipe = GetItemsForRecipe();
		if (itemsForRecipe != null)
		{
			for (int i = 0; i < itemsForRecipe.Count; i++)
			{
				items[i].UseItem(itemsForRecipe[i]);
			}
			result.SpawnResult(INT);
			Sound.Play("combine", PlayerCamera.main.body.transform.position);
			if (!hasMadeBefore)
			{
				hasMadeBefore = true;
				PlayerCamera.main.body.happiness++;
				PlayerCamera.main.body.skills.AddExp(2, 10f);
			}
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Click);
		}
		else
		{
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Deny);
		}
	}
}
