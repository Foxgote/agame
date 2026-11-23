using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000019 RID: 25
	[HarmonyPatch(typeof(Recipes), "SetUpRecipes")]
	internal static class AmmoRecipePatch
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00005566 File Offset: 0x00003766
		private static void Postfix()
		{
			AmmoTrade.AddRecipes();
		}
	}
}
