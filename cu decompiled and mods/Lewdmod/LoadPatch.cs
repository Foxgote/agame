using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000E0 RID: 224
	[HarmonyPatch(typeof(SaveSystem), "TryLoadGame")]
	internal static class LoadPatch
	{
		// Token: 0x0600051E RID: 1310 RVA: 0x00055ECD File Offset: 0x000540CD
		private static void Postfix()
		{
			Plugin.Load();
		}
	}
}
