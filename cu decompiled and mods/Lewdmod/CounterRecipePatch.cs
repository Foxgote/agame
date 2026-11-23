using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000065 RID: 101
	[HarmonyPatch(typeof(Recipes), "SetUpRecipes")]
	internal static class CounterRecipePatch
	{
		// Token: 0x060001FB RID: 507 RVA: 0x000291A6 File Offset: 0x000273A6
		private static void Postfix()
		{
			Suppressant.RegisterLiquid();
			Suppressant.AddRecipe();
		}
	}
}
