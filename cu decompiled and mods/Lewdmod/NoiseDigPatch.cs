using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000C3 RID: 195
	[HarmonyPatch(typeof(WorldGeneration), "DamageBlock", new Type[]
	{
		typeof(Vector2Int),
		typeof(float),
		typeof(bool),
		typeof(bool),
		typeof(bool)
	})]
	internal static class NoiseDigPatch
	{
		// Token: 0x06000449 RID: 1097 RVA: 0x000452B0 File Offset: 0x000434B0
		private static void Postfix(WorldGeneration __instance, Vector2Int pos, bool hitSound)
		{
			if (hitSound)
			{
				Noise.Make(__instance.BlockToWorldPos(pos), Plugin.WormNoiseDig.Value, "dig", 0.4f);
			}
		}
	}
}
