using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000A6 RID: 166
	internal static class LatexArt
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000354 RID: 852 RVA: 0x00036F5F File Offset: 0x0003515F
		public static bool Ready
		{
			get
			{
				return LatexArt.tex.ContainsKey("idle_0");
			}
		}

		// Token: 0x06000355 RID: 853 RVA: 0x00036F70 File Offset: 0x00035170
		public static void Load(string dir)
		{
			if (!Directory.Exists(dir))
			{
				return;
			}
			foreach (string text in Directory.GetFiles(dir, "latexblob_*.png"))
			{
				Texture2D texture2D = new Texture2D(2, 2, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text));
				string text2 = Path.GetFileNameWithoutExtension(text).Substring("latexblob_".Length);
				texture2D.name = "latexblob_" + text2;
				LatexArt.tex[text2] = texture2D;
			}
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00037000 File Offset: 0x00035200
		public static Sprite Get(string name)
		{
			Sprite sprite;
			if (LatexArt.sprites.TryGetValue(name, out sprite))
			{
				return sprite;
			}
			Texture2D texture2D;
			if (!LatexArt.tex.TryGetValue(name, out texture2D))
			{
				return null;
			}
			Vector2 vector = ((name.StartsWith("hang") || name.StartsWith("pond_")) ? new Vector2(0.5f, 1f) : ((name.StartsWith("belly") || name == "drop") ? new Vector2(0.5f, 0.5f) : ((name.EndsWith("seg") || name.EndsWith("tip")) ? new Vector2(0f, 0.5f) : new Vector2(0.5f, 0f))));
			return LatexArt.sprites[name] = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), vector, (LatexArt.ppu > 0f) ? LatexArt.ppu : 8f);
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0003710A File Offset: 0x0003530A
		public static void Ensure(float scale)
		{
			if (Mathf.Approximately(scale, LatexArt.ppu))
			{
				return;
			}
			LatexArt.ppu = scale;
			LatexArt.sprites.Clear();
		}

		// Token: 0x04000302 RID: 770
		private static readonly Dictionary<string, Texture2D> tex = new Dictionary<string, Texture2D>();

		// Token: 0x04000303 RID: 771
		private static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

		// Token: 0x04000304 RID: 772
		private static float ppu = -1f;
	}
}
