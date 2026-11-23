using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemLootPool : MonoBehaviour
{
	public static Dictionary<string, List<string>> pool;

	public static void InitializePool()
	{
		pool = new Dictionary<string, List<string>>();
		foreach (KeyValuePair<string, ItemInfo> globalItem in Item.GlobalItems)
		{
			if (!pool.ContainsKey(globalItem.Value.category))
			{
				pool.Add(globalItem.Value.category, new List<string>());
			}
			pool[globalItem.Value.category].Add(globalItem.Key);
		}
	}

	public static List<(string, ItemInfo)> AllItemsFromPool(string category)
	{
		if (pool.TryGetValue(category, out var value))
		{
			return value.Select((string id) => (id: id, Item.GlobalItems[id])).ToList();
		}
		return null;
	}

	public static (string, ItemInfo) RandomFromPool(string category)
	{
		string text = pool[category].PickRandom();
		return (text, Item.GetItem(text));
	}
}
