using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000FB RID: 251
	[HarmonyPatch(typeof(Item), "SetupItems")]
	internal static class SewingKitRegisterPatch
	{
		// Token: 0x06000582 RID: 1410 RVA: 0x00059B9E File Offset: 0x00057D9E
		private static void Postfix()
		{
			SewingKit.RegisterItem();
		}
	}
}
