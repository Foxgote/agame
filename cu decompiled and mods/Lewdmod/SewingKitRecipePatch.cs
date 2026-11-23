using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000FC RID: 252
	[HarmonyPatch(typeof(Recipes), "SetUpRecipes")]
	internal static class SewingKitRecipePatch
	{
		// Token: 0x06000583 RID: 1411 RVA: 0x00059BA5 File Offset: 0x00057DA5
		private static void Postfix()
		{
			SewingKit.AddRecipe();
		}
	}
}
