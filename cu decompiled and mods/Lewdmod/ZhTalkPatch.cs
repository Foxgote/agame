using System;
using System.Collections.Generic;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200013C RID: 316
	[HarmonyPatch(typeof(Talker), "Talk", new Type[]
	{
		typeof(List<string>),
		typeof(Limb),
		typeof(bool),
		typeof(bool)
	})]
	internal static class ZhTalkPatch
	{
		// Token: 0x060007AE RID: 1966 RVA: 0x000762E8 File Offset: 0x000744E8
		private static void Prefix(ref List<string> lines)
		{
			if (!Plugin.Chinese || lines == null || Plugin.PhrasesZh.Count == 0)
			{
				return;
			}
			List<string> list = new List<string>(lines.Count);
			foreach (string text in lines)
			{
				list.Add(Plugin.Zh(text));
			}
			lines = list;
		}
	}
}
