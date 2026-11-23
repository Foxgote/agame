using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000052 RID: 82
	[HarmonyPatch(typeof(Resources), "Load", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	internal static class AntivenomLoadPatch
	{
		// Token: 0x06000196 RID: 406 RVA: 0x0001ABA9 File Offset: 0x00018DA9
		private static bool Prefix(string path, ref Object __result)
		{
			if (path != "lewd_antivenom")
			{
				return true;
			}
			__result = Antivenom.Template();
			return __result == null;
		}
	}
}
