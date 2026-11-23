using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000D1 RID: 209
	[HarmonyPatch(typeof(Locale), "GetItem")]
	internal static class OutfitNamePatch
	{
		// Token: 0x06000483 RID: 1155 RVA: 0x00049330 File Offset: 0x00047530
		private static void Postfix(string str, ref string __result)
		{
			foreach (OutfitDef outfitDef in Outfits.All)
			{
				if (str == outfitDef.Id)
				{
					__result = outfitDef.Name;
				}
				else if (str == outfitDef.Id + "dsc")
				{
					__result = outfitDef.Desc;
				}
			}
		}
	}
}
