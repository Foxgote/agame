using System;
using System.Reflection;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x0200007E RID: 126
	[HarmonyPatch]
	internal static class DeviceSellCoreLibPatch
	{
		// Token: 0x06000253 RID: 595 RVA: 0x0002C6C4 File Offset: 0x0002A8C4
		private static MethodBase Target()
		{
			return AccessTools.Method("CUCoreLib.Patches.TraderCustomItemPatches:GiveItem", null, null);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0002C6D2 File Offset: 0x0002A8D2
		private static bool Prepare()
		{
			return DeviceSellCoreLibPatch.Target() != null;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0002C6DF File Offset: 0x0002A8DF
		private static MethodBase TargetMethod()
		{
			return DeviceSellCoreLibPatch.Target();
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0002C6E6 File Offset: 0x0002A8E6
		private static bool Prefix(Item item, ref bool __result)
		{
			if (!DeviceSellPatch.Refused(item))
			{
				return true;
			}
			__result = false;
			return false;
		}
	}
}
