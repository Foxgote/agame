using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000CE RID: 206
	[HarmonyPatch(typeof(CorpseScript), "Start")]
	internal static class OutfitCorpsePatch
	{
		// Token: 0x06000480 RID: 1152 RVA: 0x000492F3 File Offset: 0x000474F3
		private static void Postfix(CorpseScript __instance)
		{
			if (LewdSettings.On)
			{
				Outfits.DressCorpse(__instance);
			}
		}
	}
}
