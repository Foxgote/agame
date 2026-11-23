using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000050 RID: 80
	internal static class Antivenom
	{
		// Token: 0x06000193 RID: 403 RVA: 0x0001AA24 File Offset: 0x00018C24
		public static void RegisterItem()
		{
			if (Item.GlobalItems == null || Item.GlobalItems.ContainsKey("lewd_antivenom"))
			{
				return;
			}
			if (Liquids.Registry == null || !Liquids.Registry.ContainsKey("antivenom"))
			{
				return;
			}
			ItemInfo itemInfo;
			if (!Item.GlobalItems.TryGetValue("antiserum", out itemInfo) || !(itemInfo is LiquidItemInfo))
			{
				return;
			}
			LiquidItemInfo liquidItemInfo = (LiquidItemInfo)AccessTools.Method(typeof(object), "MemberwiseClone", null, null).Invoke(itemInfo, null);
			liquidItemInfo.defaultContents = new List<LiquidStack>
			{
				new LiquidStack("antivenom", liquidItemInfo.capacity)
			};
			liquidItemInfo.autoFill = false;
			liquidItemInfo.fullName = Plugin.T("antivenom_name");
			liquidItemInfo.description = Plugin.T("antivenom_desc");
			Item.GlobalItems["lewd_antivenom"] = liquidItemInfo;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0001AAFC File Offset: 0x00018CFC
		public static GameObject Template()
		{
			if (Antivenom.template)
			{
				return Antivenom.template;
			}
			GameObject gameObject = Resources.Load("antiserum") as GameObject;
			if (!gameObject)
			{
				return null;
			}
			if (!Antivenom.holder)
			{
				Antivenom.holder = new GameObject("LewdAntivenomTemplate");
				Antivenom.holder.SetActive(false);
				Object.DontDestroyOnLoad(Antivenom.holder);
			}
			Antivenom.template = Object.Instantiate<GameObject>(gameObject, Antivenom.holder.transform);
			Antivenom.template.name = "lewd_antivenom";
			Antivenom.template.GetComponent<Item>().id = "lewd_antivenom";
			return Antivenom.template;
		}

		// Token: 0x04000193 RID: 403
		public const string ItemId = "lewd_antivenom";

		// Token: 0x04000194 RID: 404
		private static GameObject template;

		// Token: 0x04000195 RID: 405
		private static GameObject holder;
	}
}
