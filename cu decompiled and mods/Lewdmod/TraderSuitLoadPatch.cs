using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200011D RID: 285
	[HarmonyPatch(typeof(Resources), "Load", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	internal static class TraderSuitLoadPatch
	{
		// Token: 0x060006BB RID: 1723 RVA: 0x00069E3F File Offset: 0x0006803F
		private static void Prefix(ref string path)
		{
			if (path == "lewd_suitoff")
			{
				path = "flimsyknife";
			}
		}
	}
}
