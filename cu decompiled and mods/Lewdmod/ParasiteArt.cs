using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000D4 RID: 212
	internal static class ParasiteArt
	{
		// Token: 0x06000498 RID: 1176 RVA: 0x0004A074 File Offset: 0x00048274
		public static Color LightColor(int c)
		{
			return (c == 3 || c == 8) ? new Color32(120, 105, 190, byte.MaxValue) : ((c == 7) ? new Color32(110, 210, 40, byte.MaxValue) : ParasiteArt.Colors[c]);
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0004A0CC File Offset: 0x000482CC
		public static void EnsureFrames(float ppu)
		{
			if (Mathf.Approximately(ppu, ParasiteArt.framesPpu) || ParasiteArt.frameTextures.Count == 0)
			{
				return;
			}
			ParasiteArt.framesPpu = ppu;
			ParasiteArt.Frames = ParasiteArt.frameTextures.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0.5f, 0.05f), ppu)).ToArray<Sprite>();
			ParasiteArt.MiteFrames = ParasiteArt.miteTextures.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0.5f, 0.05f), ppu)).ToArray<Sprite>();
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x0004A150 File Offset: 0x00048350
		public static void Load(string dir)
		{
			if (!Directory.Exists(dir))
			{
				return;
			}
			foreach (string text in from f in Directory.GetFiles(dir, "*.png")
				orderby f
				select f)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				Texture2D texture2D = new Texture2D(2, 2, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text));
				texture2D.name = fileNameWithoutExtension;
				if (fileNameWithoutExtension.StartsWith("parasite_"))
				{
					ParasiteArt.frameTextures.Add(texture2D);
				}
				else if (fileNameWithoutExtension.StartsWith("mite_"))
				{
					ParasiteArt.miteTextures.Add(texture2D);
				}
				else if (fileNameWithoutExtension.StartsWith("suit_"))
				{
					ParasiteArt.SuitTextures[fileNameWithoutExtension.Substring(5)] = texture2D;
				}
				else if (fileNameWithoutExtension.StartsWith("suitfull_"))
				{
					ParasiteArt.SuitFullTextures[fileNameWithoutExtension.Substring(9)] = texture2D;
				}
				else if (fileNameWithoutExtension.StartsWith("hsuit_"))
				{
					ParasiteArt.HostTextures[fileNameWithoutExtension.Substring(6)] = texture2D;
				}
				else if (fileNameWithoutExtension.StartsWith("hsuitfull_"))
				{
					ParasiteArt.HostFullTextures[fileNameWithoutExtension.Substring(10)] = texture2D;
				}
				else if (fileNameWithoutExtension.StartsWith("lens_"))
				{
					ParasiteArt.LensTextures[fileNameWithoutExtension.Substring(5)] = texture2D;
				}
			}
			ParasiteArt.EnsureFrames(100f);
		}

		// Token: 0x04000464 RID: 1124
		public static readonly Color[] Colors = new Color[]
		{
			new Color32(150, 70, 210, byte.MaxValue),
			new Color32(80, 200, 110, byte.MaxValue),
			new Color32(240, 110, 190, byte.MaxValue),
			new Color32(52, 52, 66, byte.MaxValue),
			new Color32(236, 238, 246, byte.MaxValue),
			new Color32(196, 30, 46, byte.MaxValue),
			new Color32(120, 235, 105, byte.MaxValue),
			new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
			new Color32(40, 36, 54, byte.MaxValue)
		};

		// Token: 0x04000465 RID: 1125
		public static readonly string[] ColorNames = new string[] { "purple", "green", "pink", "black", "dronewhite", "dronered", "slime", "slime", "latex" };

		// Token: 0x04000466 RID: 1126
		public static readonly Dictionary<string, Texture2D> LensTextures = new Dictionary<string, Texture2D>();

		// Token: 0x04000467 RID: 1127
		public static Sprite[] Frames = new Sprite[0];

		// Token: 0x04000468 RID: 1128
		private static readonly List<Texture2D> frameTextures = new List<Texture2D>();

		// Token: 0x04000469 RID: 1129
		public static Sprite[] MiteFrames = new Sprite[0];

		// Token: 0x0400046A RID: 1130
		private static readonly List<Texture2D> miteTextures = new List<Texture2D>();

		// Token: 0x0400046B RID: 1131
		private static float framesPpu = -1f;

		// Token: 0x0400046C RID: 1132
		public static readonly Dictionary<string, Texture2D> SuitTextures = new Dictionary<string, Texture2D>();

		// Token: 0x0400046D RID: 1133
		public static readonly Dictionary<string, Texture2D> SuitFullTextures = new Dictionary<string, Texture2D>();

		// Token: 0x0400046E RID: 1134
		public static readonly Dictionary<string, Texture2D> HostTextures = new Dictionary<string, Texture2D>();

		// Token: 0x0400046F RID: 1135
		public static readonly Dictionary<string, Texture2D> HostFullTextures = new Dictionary<string, Texture2D>();
	}
}
