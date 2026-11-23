using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000C2 RID: 194
	[HarmonyPatch(typeof(WorldGeneration), "CreateExplosion")]
	internal static class NoiseBlastPatch
	{
		// Token: 0x06000448 RID: 1096 RVA: 0x0004528F File Offset: 0x0004348F
		private static void Postfix(ExplosionParams param)
		{
			Noise.Make(param.position, Plugin.WormNoiseBlast.Value, "blast", 0.05f);
		}
	}
}
