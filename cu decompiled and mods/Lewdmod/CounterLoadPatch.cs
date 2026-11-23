using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000066 RID: 102
	[HarmonyPatch(typeof(Resources), "Load", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	internal static class CounterLoadPatch
	{
		// Token: 0x060001FC RID: 508 RVA: 0x000291B2 File Offset: 0x000273B2
		private static bool Prefix(string path, ref Object __result)
		{
			if (path == "lewd_suppressant_pills")
			{
				__result = Suppressant.Template();
			}
			else
			{
				if (!(path == "lewd_scalpel"))
				{
					return true;
				}
				__result = Scalpel.Template();
			}
			return __result == null;
		}
	}
}
