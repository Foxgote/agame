using System;
using HarmonyLib;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000020 RID: 32
	[HarmonyPatch(typeof(Resources), "Load", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	internal static class LoadPatch
	{
		// Token: 0x06000067 RID: 103 RVA: 0x0000760C File Offset: 0x0000580C
		private static bool Prefix(string path, ref Object __result)
		{
			ArmDef armDef = Armory.ById(path);
			if (armDef == null)
			{
				return true;
			}
			__result = Armory.Template(armDef);
			return false;
		}
	}
}
