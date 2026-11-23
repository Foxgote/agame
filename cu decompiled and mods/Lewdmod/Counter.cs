using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000063 RID: 99
	internal static class Counter
	{
		// Token: 0x060001F4 RID: 500 RVA: 0x00028E58 File Offset: 0x00027058
		public static GameObject Clone(GameObject src, string id, ref GameObject holder)
		{
			if (!holder)
			{
				holder = new GameObject("LewdCounterTemplates");
				holder.SetActive(false);
				Object.DontDestroyOnLoad(holder);
			}
			GameObject gameObject = Object.Instantiate<GameObject>(src, holder.transform);
			gameObject.name = id;
			gameObject.GetComponent<Item>().id = id;
			return gameObject;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00028EAC File Offset: 0x000270AC
		public static void AddToPool(string id, params string[] pools)
		{
			if (ItemLootPool.pool == null || !Plugin.CounterInLoot.Value)
			{
				return;
			}
			foreach (string text in pools)
			{
				List<string> list;
				if (ItemLootPool.pool.TryGetValue(text, out list) && !list.Contains(id))
				{
					list.Add(id);
				}
			}
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00028F00 File Offset: 0x00027100
		public static void FillCrate(BuildingEntity e, string id)
		{
			float num = (id.Contains("medcrate") ? 2.5f : 1f);
			Vector2 vector = e.transform.position;
			if (Random.value < Plugin.SuppressCrateChance.Value * Mathf.Min(num, 2f))
			{
				Counter.Throw(Suppressant.Template(), vector, "suppressant pills");
			}
			if (Random.value < Plugin.ScalpelCrateChance.Value * num)
			{
				Counter.Throw(Scalpel.Template(), vector, "scalpel");
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00028F88 File Offset: 0x00027188
		private static void Throw(GameObject t, Vector2 at, string what)
		{
			if (!t)
			{
				return;
			}
			Rigidbody2D rigidbody2D;
			if (Object.Instantiate<GameObject>(t, at, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))).TryGetComponent<Rigidbody2D>(ref rigidbody2D))
			{
				rigidbody2D.velocity = new Vector2(Random.Range(-6f, 6f), Random.Range(2f, 6f));
			}
			Plugin.Log.LogInfo("Crate broke: " + what + " in it");
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00029014 File Offset: 0x00027214
		public static bool UseDisinfectant(Body b, float ml)
		{
			foreach (Item item in b.GetAllItemsThorough())
			{
				WaterContainerItem waterContainerItem;
				if (item && item.TryGetComponent<WaterContainerItem>(ref waterContainerItem) && waterContainerItem.stack != null)
				{
					foreach (LiquidStack liquidStack in waterContainerItem.stack.ToArray())
					{
						LiquidType liquidType;
						if (liquidStack.amount >= ml && Liquids.Registry.TryGetValue(liquidStack.liquidId, out liquidType) && liquidType.qualities != null)
						{
							if (liquidType.qualities.Any<CraftingQuality>((CraftingQuality q) => q.id == "disinfectant"))
							{
								waterContainerItem.Drain(waterContainerItem.CalculateDrainSingleLiquid(liquidStack.liquidId, ml));
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x00029124 File Offset: 0x00027324
		public static float MaskFactor(Body b, float dt)
		{
			Item wearableBySlotID = b.GetWearableBySlotID("mouth");
			if (!wearableBySlotID)
			{
				return 1f;
			}
			wearableBySlotID.condition = Mathf.Max(0f, wearableBySlotID.condition - Plugin.MaskWear.Value * dt);
			return Mathf.Clamp01(1f - Plugin.MaskProtection.Value * Mathf.Clamp01(wearableBySlotID.condition * 2f));
		}
	}
}
