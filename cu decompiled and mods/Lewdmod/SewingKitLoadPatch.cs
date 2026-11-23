using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000FD RID: 253
	[HarmonyPatch(typeof(Resources), "Load", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	internal static class SewingKitLoadPatch
	{
		// Token: 0x06000584 RID: 1412 RVA: 0x00059BAC File Offset: 0x00057DAC
		private static bool Prefix(string path, ref Object __result)
		{
			if (path != "lewd_sewingkit")
			{
				return true;
			}
			__result = SewingKit.Template();
			return __result == null;
		}
	}
}
