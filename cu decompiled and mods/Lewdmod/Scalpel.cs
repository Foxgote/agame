using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000061 RID: 97
	internal static class Scalpel
	{
		// Token: 0x060001EA RID: 490 RVA: 0x00028720 File Offset: 0x00026920
		public static void RegisterItem()
		{
			ItemInfo itemInfo;
			if (Item.GlobalItems == null || Item.GlobalItems.ContainsKey("lewd_scalpel") || !Item.GlobalItems.TryGetValue("flimsyknife", out itemInfo))
			{
				return;
			}
			ItemInfo itemInfo2 = (ItemInfo)AccessTools.Method(typeof(object), "MemberwiseClone", null, null).Invoke(itemInfo, null);
			itemInfo2.category = "medical";
			itemInfo2.weight = 0.1f;
			itemInfo2.value = 16;
			itemInfo2.qualities = new List<CraftingQuality>
			{
				new CraftingQuality("cutting", 30f)
			};
			itemInfo2.fullName = Plugin.T("scalpel_name");
			itemInfo2.description = Plugin.T("scalpel_desc");
			Item.GlobalItems["lewd_scalpel"] = itemInfo2;
			Counter.AddToPool("lewd_scalpel", new string[] { "medical" });
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00028804 File Offset: 0x00026A04
		public static GameObject Template()
		{
			if (Scalpel.template)
			{
				return Scalpel.template;
			}
			GameObject gameObject = Resources.Load("flimsyknife") as GameObject;
			if (!gameObject)
			{
				return null;
			}
			Scalpel.template = Counter.Clone(gameObject, "lewd_scalpel", ref Scalpel.holder);
			Scalpel.template.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			SpriteRenderer component = Scalpel.template.GetComponent<SpriteRenderer>();
			if (component)
			{
				component.color = new Color32(214, 230, 240, byte.MaxValue);
			}
			return Scalpel.template;
		}

		// Token: 0x040001ED RID: 493
		public const string ItemId = "lewd_scalpel";

		// Token: 0x040001EE RID: 494
		private static GameObject template;

		// Token: 0x040001EF RID: 495
		private static GameObject holder;
	}
}
