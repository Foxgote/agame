using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000021 RID: 33
	[HarmonyPatch(typeof(Locale), "GetItem")]
	internal static class NamePatch
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00007630 File Offset: 0x00005830
		private static void Postfix(string str, ref string __result)
		{
			if (str == null || !str.StartsWith("arm_"))
			{
				return;
			}
			ArmDef armDef = Armory.ById(str.EndsWith("dsc") ? str.Substring(0, str.Length - 3) : str);
			if (armDef != null)
			{
				__result = (str.EndsWith("dsc") ? Armory.Desc(armDef) : Armory.Name(armDef));
			}
		}
	}
}
