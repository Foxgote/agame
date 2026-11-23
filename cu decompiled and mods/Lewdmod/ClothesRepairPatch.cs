using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200005E RID: 94
	[HarmonyPatch(typeof(Recipes), "SetUpRecipes")]
	internal static class ClothesRepairPatch
	{
		// Token: 0x060001DC RID: 476 RVA: 0x00027EA1 File Offset: 0x000260A1
		private static void Postfix()
		{
			Clothes.AddRecipes();
		}
	}
}
