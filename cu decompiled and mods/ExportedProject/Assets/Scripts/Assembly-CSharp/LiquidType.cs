using System.Collections.Generic;
using UnityEngine;

public class LiquidType
{
	public delegate void OnDrink(float ml, Body body);

	public delegate void OnHealthUse(float ml, Limb limb);

	public string localeName;

	public Color color;

	public float valuePerLiter;

	public OnDrink onDrink;

	public OnHealthUse onHealthUse;

	public bool healthUsable;

	public bool injectable;

	public bool localeFromItem;

	public float injectionSickness = 1f;

	public List<CraftingQuality> qualities = new List<CraftingQuality>();

	public List<CraftingQuality> GetScaledQualities(float ml)
	{
		List<CraftingQuality> list = new List<CraftingQuality>();
		foreach (CraftingQuality quality in qualities)
		{
			list.Add(new CraftingQuality(quality.id, quality.amount * ml));
		}
		return list;
	}
}
