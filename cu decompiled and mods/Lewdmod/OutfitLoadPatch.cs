using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000D0 RID: 208
	[HarmonyPatch(typeof(Resources), "Load", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	internal static class OutfitLoadPatch
	{
		// Token: 0x06000482 RID: 1154 RVA: 0x0004930C File Offset: 0x0004750C
		private static bool Prefix(string path, ref Object __result)
		{
			OutfitDef outfitDef = Outfits.ById(path);
			if (outfitDef == null)
			{
				return true;
			}
			__result = Outfits.Template(outfitDef);
			return false;
		}
	}
}
