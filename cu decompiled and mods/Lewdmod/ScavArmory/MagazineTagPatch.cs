using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x0200002F RID: 47
	[HarmonyPatch(typeof(Item), "SetupItems")]
	internal static class MagazineTagPatch
	{
		// Token: 0x060000D9 RID: 217 RVA: 0x0000D828 File Offset: 0x0000BA28
		private static void Postfix()
		{
			if (Item.GlobalItems == null)
			{
				return;
			}
			List<string> list = new List<string>();
			list.Add("smallmagazine");
			list.Add("riflemagazine");
			list.AddRange(from d in Armory.All
				where d.Kind == Kind.Magazine
				select d.Id);
			int num = 0;
			foreach (string text in list)
			{
				ItemInfo itemInfo;
				if (Item.GlobalItems.TryGetValue(text, out itemInfo) && !(itemInfo.tags ?? "").Split(new char[] { ',' }).Contains("lewd_magazine"))
				{
					itemInfo.tags = (string.IsNullOrEmpty(itemInfo.tags) ? "lewd_magazine" : (itemInfo.tags + ",lewd_magazine"));
					itemInfo.SetTags();
					num++;
				}
			}
			Plugin.Log.LogInfo(string.Format("Magazines tagged for the pouches: {0}", num));
		}
	}
}
