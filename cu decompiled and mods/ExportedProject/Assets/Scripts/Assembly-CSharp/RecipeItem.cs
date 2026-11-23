using System.Collections.Generic;
using UnityEngine;

public class RecipeItem
{
	public bool specific;

	public string specificId;

	public bool isLiquid;

	public CraftingQuality quality;

	public float minimumCondition = 0.9f;

	public bool destroyItem = true;

	public string ignoredId;

	public bool noMaxCondition;

	public RecipeItem(float minimumCondition = 0.9f)
	{
		this.minimumCondition = minimumCondition;
	}

	public Item GetMatchingItem(List<Item> items, List<Item> exclusions)
	{
		foreach (Item item in items)
		{
			if (item.favourited || (item.id == ignoredId && !isLiquid) || exclusions.Contains(item))
			{
				continue;
			}
			if (isLiquid)
			{
				if (!item.TryGetComponent<WaterContainerItem>(out var component))
				{
					continue;
				}
				if (specific)
				{
					if (component.AmountOf(specificId) >= minimumCondition)
					{
						return item;
					}
					continue;
				}
				foreach (LiquidStack item2 in component.stack)
				{
					Liquids.Registry.TryGetValue(item2.liquidId, out var value);
					if (Item.GetQualityThatMeetsCriteria(quality, value.GetScaledQualities(item2.amount)) != null)
					{
						return item;
					}
				}
			}
			else
			{
				if (item.TryGetComponent<Container>(out var component2) && component2.itemCount > 0)
				{
					continue;
				}
				if (specific)
				{
					if (item.id == specificId && item.condition >= minimumCondition && (!noMaxCondition || item.condition < 0.991f) && (!item.Stats.destroyAtZeroCondition || item.condition > 0f))
					{
						return item;
					}
				}
				else if (item.condition >= minimumCondition && (!noMaxCondition || item.condition < 0.991f) && Item.GetQualityThatMeetsCriteria(quality, item.Stats.qualities) != null && (!item.Stats.destroyAtZeroCondition || item.condition > 0f))
				{
					return item;
				}
			}
		}
		return null;
	}

	public bool DoesUseItemType(Item item)
	{
		if (isLiquid)
		{
			if (item.TryGetComponent<WaterContainerItem>(out var component))
			{
				if (specific)
				{
					if (component.HasLiquid(specificId))
					{
						return true;
					}
				}
				else
				{
					foreach (LiquidStack item2 in component.stack)
					{
						Liquids.Registry.TryGetValue(item2.liquidId, out var value);
						if (Item.HasCommonQuality(quality, value.qualities) != null)
						{
							return true;
						}
					}
				}
			}
		}
		else if (specific)
		{
			if (item.id == specificId)
			{
				return true;
			}
		}
		else if (Item.HasCommonQuality(quality, item.Stats.qualities) != null)
		{
			return true;
		}
		return false;
	}

	public void UseItem(Item item)
	{
		if (!item)
		{
			return;
		}
		if (isLiquid)
		{
			if (!item.TryGetComponent<WaterContainerItem>(out var component))
			{
				return;
			}
			if (!specific)
			{
				string id = "";
				float max = 0f;
				foreach (LiquidStack item2 in component.stack)
				{
					Liquids.Registry.TryGetValue(item2.liquidId, out var value);
					CraftingQuality qualityThatMeetsCriteria = Item.GetQualityThatMeetsCriteria(quality, value.GetScaledQualities(item2.amount));
					if (qualityThatMeetsCriteria != null)
					{
						id = item2.liquidId;
						max = Mathf.Lerp(0f, item2.amount, quality.amount / qualityThatMeetsCriteria.amount);
						break;
					}
				}
				component.Drain(component.CalculateDrainSingleLiquid(id, max));
			}
			else
			{
				component.Drain(component.CalculateDrainSingleLiquid(specificId, minimumCondition));
			}
			return;
		}
		if ((bool)item.battery && destroyItem)
		{
			item.battery.UnloadBattery();
		}
		if (specific)
		{
			item.condition -= minimumCondition;
		}
		else
		{
			CraftingQuality qualityThatMeetsCriteria2 = Item.GetQualityThatMeetsCriteria(quality, item.Stats.qualities);
			float num = quality.amount / qualityThatMeetsCriteria2.amount;
			item.condition -= num;
		}
		if (destroyItem)
		{
			if (item.TryGetComponent<Container>(out var component2))
			{
				component2.UnloadAllItems();
			}
			PlayerCamera.main.body.DropItem(item);
			Object.Destroy(item.gameObject);
		}
	}
}
