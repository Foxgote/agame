using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200009A RID: 154
	[HarmonyPatch(typeof(Locale), "GetItem")]
	internal static class ToyNamePatch
	{
		// Token: 0x0600030F RID: 783 RVA: 0x00034B8C File Offset: 0x00032D8C
		private static void Postfix(string str, ref string __result)
		{
			if (str == null || !str.StartsWith("lewd_"))
			{
				return;
			}
			bool flag = str.EndsWith("dsc");
			ToyDef toyDef = Toys.ById(flag ? str.Substring(0, str.Length - 3) : str);
			if (toyDef == null)
			{
				return;
			}
			__result = (flag ? Plugin.T("toy_" + toyDef.Kind + "_desc") : Toys.Name(toyDef));
		}
	}
}
