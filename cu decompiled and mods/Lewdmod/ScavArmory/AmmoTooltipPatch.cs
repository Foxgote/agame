using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000012 RID: 18
	[HarmonyPatch(typeof(PlayerCamera), "ItemHoverDescription")]
	internal static class AmmoTooltipPatch
	{
		// Token: 0x06000039 RID: 57 RVA: 0x00004C74 File Offset: 0x00002E74
		private static void Postfix(Item item, ref ValueTuple<string, string> __result)
		{
			try
			{
				AmmoInfo.Describe(item, ref __result.Item2);
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Ammo tooltip: " + ex.Message);
			}
		}
	}
}
