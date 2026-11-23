using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000EE RID: 238
	[HarmonyPatch(typeof(PlayerCamera), "HandleAttacks")]
	internal static class PuppetAttackPatch
	{
		// Token: 0x0600054D RID: 1357 RVA: 0x00057932 File Offset: 0x00055B32
		private static bool Prefix()
		{
			return !Puppet.Active && !Flesh.Swallowed;
		}
	}
}
