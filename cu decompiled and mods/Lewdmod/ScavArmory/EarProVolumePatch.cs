using System;
using HarmonyLib;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000033 RID: 51
	[HarmonyPatch(typeof(Sound), "Play", new Type[]
	{
		typeof(AudioClip),
		typeof(Vector2),
		typeof(bool),
		typeof(bool),
		typeof(Transform),
		typeof(float),
		typeof(float),
		typeof(bool),
		typeof(bool)
	})]
	internal static class EarProVolumePatch
	{
		// Token: 0x060000E0 RID: 224 RVA: 0x0000DC1E File Offset: 0x0000BE1E
		private static void Prefix(AudioClip clip, ref float volume)
		{
			if (EarPro.On && EarPro.IsShot(clip))
			{
				volume *= 0.4f;
			}
		}
	}
}
