using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000EC RID: 236
	[HarmonyPatch(typeof(PlayerCamera), "HandleInput")]
	internal static class PuppetInputPatch
	{
		// Token: 0x0600054B RID: 1355 RVA: 0x00057850 File Offset: 0x00055A50
		private static void Postfix(PlayerCamera __instance)
		{
			if (!Puppet.Active || PauseHandler.paused || !__instance.body)
			{
				return;
			}
			Body body = __instance.body;
			body.moveDir = Puppet.MoveDir;
			body.crouching = false;
			body.targetLookPos = body.transform.position + new Vector3((float)Puppet.Facing * 5f, 0.3f, 0f);
		}
	}
}
