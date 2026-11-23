using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000E5 RID: 229
	internal static class Potion
	{
		// Token: 0x0600052B RID: 1323 RVA: 0x00056560 File Offset: 0x00054760
		public static void RegisterLiquid()
		{
			if (Liquids.Registry == null || Liquids.Registry.ContainsKey("lewd_lust"))
			{
				return;
			}
			Dictionary<string, LiquidType> registry = Liquids.Registry;
			string text = "lewd_lust";
			LiquidType liquidType = new LiquidType();
			liquidType.localeName = "lewd_lust";
			liquidType.color = new Color32(214, 72, 196, byte.MaxValue);
			liquidType.valuePerLiter = 40f;
			liquidType.injectionSickness = 1f;
			LiquidType.OnDrink onDrink;
			if ((onDrink = Potion.<>O.<0>__Drink) == null)
			{
				onDrink = (Potion.<>O.<0>__Drink = new LiquidType.OnDrink(Potion.Drink));
			}
			liquidType.onDrink = onDrink;
			liquidType.qualities = new List<CraftingQuality>
			{
				new CraftingQuality("water", 0.3f)
			};
			registry[text] = liquidType;
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x00056620 File Offset: 0x00054820
		private static void Drink(float ml, Body body)
		{
			float num = ml * 0.001f;
			body.Drink(90f * num);
			Mood.Change(body, 10f * num);
			if (!LewdSettings.On || !PlayerCamera.main || PlayerCamera.main.body != body)
			{
				return;
			}
			State s = Plugin.S;
			float num2 = Plugin.PotionStrength.Value * (s.inHeat ? 1.5f : 1f) * Suppressant.Factor;
			s.lust = Mathf.Min(100f, s.lust + 110f * num * num2);
			s.arousal = Mathf.Min(100f, s.arousal + 80f * num * num2);
			s.corruption = Mathf.Min(100f, s.corruption + 3f * num * num2);
			if (Time.time - Potion.lastLine > 20f)
			{
				Potion.lastLine = Time.time;
				Relief.Say(body, "potion_drink");
			}
			Plugin.Log.LogInfo(string.Format("She drank {0:0} ml of the lust draught: lust {1:0}, arousal {2:0}", ml, s.lust, s.arousal));
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x00056758 File Offset: 0x00054958
		public static void RegisterItem()
		{
			if (Item.GlobalItems == null || Item.GlobalItems.ContainsKey("lewd_lustpotion"))
			{
				return;
			}
			ItemInfo itemInfo;
			if (!Item.GlobalItems.TryGetValue("waterbottle", out itemInfo) || !(itemInfo is LiquidItemInfo))
			{
				return;
			}
			LiquidItemInfo liquidItemInfo = (LiquidItemInfo)AccessTools.Method(typeof(object), "MemberwiseClone", null, null).Invoke(itemInfo, null);
			liquidItemInfo.defaultContents = new List<LiquidStack>
			{
				new LiquidStack("lewd_lust", liquidItemInfo.capacity)
			};
			liquidItemInfo.autoFill = false;
			liquidItemInfo.value = 12;
			liquidItemInfo.fullName = Plugin.T("potion_name");
			liquidItemInfo.description = Plugin.T("potion_desc");
			Item.GlobalItems["lewd_lustpotion"] = liquidItemInfo;
			if (Plugin.PotionInLoot.Value && ItemLootPool.pool != null)
			{
				foreach (string text in new string[] { "water", "drug" })
				{
					List<string> list;
					if (ItemLootPool.pool.TryGetValue(text, out list) && !list.Contains("lewd_lustpotion"))
					{
						list.Add("lewd_lustpotion");
					}
				}
			}
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x00056884 File Offset: 0x00054A84
		public static GameObject Template()
		{
			if (Potion.template)
			{
				return Potion.template;
			}
			GameObject gameObject = Resources.Load("waterbottle") as GameObject;
			if (!gameObject)
			{
				return null;
			}
			if (!Potion.holder)
			{
				Potion.holder = new GameObject("LewdPotionTemplate");
				Potion.holder.SetActive(false);
				Object.DontDestroyOnLoad(Potion.holder);
			}
			Potion.template = Object.Instantiate<GameObject>(gameObject, Potion.holder.transform);
			Potion.template.name = "lewd_lustpotion";
			Potion.template.GetComponent<Item>().id = "lewd_lustpotion";
			return Potion.template;
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0005692C File Offset: 0x00054B2C
		public static GameObject SpawnAt(Vector2 pos)
		{
			GameObject gameObject = Potion.Template();
			if (!gameObject)
			{
				return null;
			}
			return Object.Instantiate<GameObject>(gameObject, pos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x00056974 File Offset: 0x00054B74
		public static void SeedLayer(Body b)
		{
			if (Random.value >= Plugin.PotionLayerChance.Value)
			{
				return;
			}
			Vector2? vector = Spots.RandomFloor(25f, b.transform.position);
			if (vector == null)
			{
				return;
			}
			Potion.SpawnAt(vector.Value);
			Plugin.Log.LogInfo(string.Format("A lust draught lies at {0}", vector.Value));
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x000569E8 File Offset: 0x00054BE8
		public static void MaybeDrop(Vector2 at, float chance, string where)
		{
			if (Random.value >= chance)
			{
				return;
			}
			GameObject gameObject = Potion.SpawnAt(at + new Vector2(Random.Range(-1f, 1f), 1.5f));
			Rigidbody2D rigidbody2D;
			if (gameObject && gameObject.TryGetComponent<Rigidbody2D>(ref rigidbody2D))
			{
				rigidbody2D.velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 4f));
			}
			if (gameObject)
			{
				Plugin.Log.LogInfo(string.Format("A lust draught {0} at {1}", where, at));
			}
		}

		// Token: 0x04000615 RID: 1557
		public const string ItemId = "lewd_lustpotion";

		// Token: 0x04000616 RID: 1558
		public const string LiquidId = "lewd_lust";

		// Token: 0x04000617 RID: 1559
		private static GameObject template;

		// Token: 0x04000618 RID: 1560
		private static GameObject holder;

		// Token: 0x04000619 RID: 1561
		private static float lastLine = -99f;

		// Token: 0x0200020E RID: 526
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000A76 RID: 2678
			public static LiquidType.OnDrink <0>__Drink;
		}
	}
}
