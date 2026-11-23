using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000ED RID: 237
	[HarmonyPatch(typeof(Body), "Jump")]
	internal static class PuppetJumpPatch
	{
		// Token: 0x0600054C RID: 1356 RVA: 0x000578C4 File Offset: 0x00055AC4
		private static bool Prefix(Body __instance)
		{
			return !PlayerCamera.main || __instance != PlayerCamera.main.body || (!Flesh.Holding && !SlimeBog.Stuck && !Tentacles.Holding && !Womb.InLabor && !Worm.Inside && !Slits.Holding && !Slits.Inside && (!Puppet.Active || Puppet.AllowJump));
		}
	}
}
