using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000E7 RID: 231
	[HarmonyPatch(typeof(Resources), "Load", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	internal static class PotionLoadPatch
	{
		// Token: 0x06000534 RID: 1332 RVA: 0x00056A99 File Offset: 0x00054C99
		private static bool Prefix(string path, ref Object __result)
		{
			if (path != "lewd_lustpotion")
			{
				return true;
			}
			__result = Potion.Template();
			return __result == null;
		}
	}
}
