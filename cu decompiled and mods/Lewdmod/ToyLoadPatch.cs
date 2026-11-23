using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000099 RID: 153
	[HarmonyPatch(typeof(Resources), "Load", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	internal static class ToyLoadPatch
	{
		// Token: 0x0600030E RID: 782 RVA: 0x00034B60 File Offset: 0x00032D60
		private static bool Prefix(string path, ref Object __result)
		{
			ToyDef toyDef = Toys.ById(path);
			if (toyDef == null)
			{
				return true;
			}
			__result = Toys.Template(toyDef);
			return __result == null;
		}
	}
}
