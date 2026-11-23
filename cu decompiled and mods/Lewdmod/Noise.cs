using System;
using System.Collections.Generic;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000BF RID: 191
	internal static class Noise
	{
		// Token: 0x06000442 RID: 1090 RVA: 0x0004500C File Offset: 0x0004320C
		public static void Make(Vector2 at, float radius, string what, float every = 0.3f)
		{
			if (radius <= 0f || Worm.Beasts.Count == 0)
			{
				return;
			}
			float num;
			if (Noise.lastAt.TryGetValue(what, out num) && Time.time - num < every)
			{
				return;
			}
			Noise.lastAt[what] = Time.time;
			foreach (WormBeast wormBeast in Worm.Beasts.ToArray())
			{
				if (wormBeast)
				{
					wormBeast.OnNoise(at, radius, what);
				}
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000443 RID: 1091 RVA: 0x00045086 File Offset: 0x00043286
		private static Body Her
		{
			get
			{
				if (!PlayerCamera.main)
				{
					return null;
				}
				return PlayerCamera.main.body;
			}
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x000450A0 File Offset: 0x000432A0
		public static void OnSound(AudioClip clip, Vector2 pos, float volume)
		{
			if (!clip || Worm.Beasts.Count == 0)
			{
				return;
			}
			string name = clip.name;
			Body her = Noise.Her;
			if (name == "drop")
			{
				Noise.Make(pos, Plugin.WormNoiseDrop.Value, "drop", 0.15f);
				return;
			}
			if (name.StartsWith("bodyFall") || name.StartsWith("BoneBreak") || name.StartsWith("landbig"))
			{
				Noise.Make(pos, Plugin.WormNoiseFall.Value, "fall", 0.3f);
				return;
			}
			if (name.StartsWith("BSFootstep") && her && her.rb && Mathf.Abs(her.rb.velocity.x) > 6f && Vector2.Distance(pos, her.transform.position) < 3f)
			{
				Noise.Make(pos, Plugin.WormNoiseRun.Value, "run", 0.25f);
				return;
			}
			if (name.StartsWith("climax/"))
			{
				Noise.Make(pos, Plugin.WormNoiseClimax.Value, "climax", 1f);
				return;
			}
			if (name.StartsWith("moan/") || name.StartsWith("moan_soft/") || name.StartsWith("muffled/"))
			{
				Noise.Make(pos, Plugin.WormNoiseMoan.Value * Mathf.Clamp(volume, 0.3f, 1.2f), "moan", 0.8f);
			}
		}

		// Token: 0x040003DF RID: 991
		private static readonly Dictionary<string, float> lastAt = new Dictionary<string, float>();
	}
}
