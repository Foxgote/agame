using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000127 RID: 295
	internal static class WallArt
	{
		// Token: 0x060006DB RID: 1755 RVA: 0x0006AD40 File Offset: 0x00068F40
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
				if (fileNameWithoutExtension.StartsWith("fleshwall_"))
				{
					WallArt.flesh.Add(texture2D);
				}
				else if (fileNameWithoutExtension.StartsWith("slimewall_"))
				{
					WallArt.slime.Add(texture2D);
				}
				else if (fileNameWithoutExtension.StartsWith("engulf_"))
				{
					WallArt.engulf.Add(texture2D);
				}
				else if (fileNameWithoutExtension.StartsWith("mawjaw_"))
				{
					WallArt.jaw[fileNameWithoutExtension] = texture2D;
				}
				else if (fileNameWithoutExtension.StartsWith("mawgut_"))
				{
					WallArt.gut.Add(texture2D);
				}
			}
		}

		// Token: 0x060006DC RID: 1756 RVA: 0x0006AE6C File Offset: 0x0006906C
		public static void Ensure(float pixelsPerUnit)
		{
			if (Mathf.Approximately(pixelsPerUnit, WallArt.ppu))
			{
				return;
			}
			WallArt.ppu = pixelsPerUnit;
			WallArt.Flesh = WallArt.flesh.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0f, 0.5f), WallArt.ppu)).ToArray<Sprite>();
			WallArt.Slime = WallArt.slime.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0f, 0.5f), WallArt.ppu)).ToArray<Sprite>();
			WallArt.Engulf = WallArt.engulf.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0.5f, 0.5f), WallArt.ppu)).ToArray<Sprite>();
			WallArt.Jaw = (from kv in WallArt.jaw
				group kv by kv.Key.Split(new char[] { '_' })[1] into g
				orderby int.Parse(g.Key)
				select g.Select<KeyValuePair<string, Texture2D>, Sprite>((KeyValuePair<string, Texture2D> kv) => Sprite.Create(kv.Value, new Rect(0f, 0f, (float)kv.Value.width, (float)kv.Value.height), new Vector2(0f, 1f), WallArt.ppu)).ToArray<Sprite>()).ToArray<Sprite[]>();
			WallArt.Gut = WallArt.gut.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0.5f, 1f), WallArt.ppu)).ToArray<Sprite>();
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060006DD RID: 1757 RVA: 0x0006AFD4 File Offset: 0x000691D4
		public static float Height
		{
			get
			{
				if (WallArt.Flesh.Length == 0)
				{
					return 3.5f;
				}
				return WallArt.Flesh[0].bounds.size.y;
			}
		}

		// Token: 0x060006DE RID: 1758 RVA: 0x0006B008 File Offset: 0x00069208
		public static Sprite Slice(Sprite whole, int i, int n, int bend = 0)
		{
			Texture2D texture = whole.texture;
			Sprite sprite;
			if (WallArt.slices.TryGetValue(new ValueTuple<Texture2D, int, int, int>(texture, i, n, bend), out sprite) && sprite && Mathf.Approximately(sprite.pixelsPerUnit, whole.pixelsPerUnit))
			{
				return sprite;
			}
			int height = texture.height;
			int num = height - Mathf.RoundToInt((float)((i + 1) * height) / (float)n);
			int num2 = height - Mathf.RoundToInt((float)(i * height) / (float)n);
			if (bend == 0)
			{
				sprite = Sprite.Create(texture, new Rect(0f, (float)num, (float)texture.width, (float)(num2 - num)), new Vector2(0f, 0.5f), whole.pixelsPerUnit);
			}
			else
			{
				int width = texture.width;
				int num3 = num2 - num;
				int num4 = Mathf.Abs(bend);
				Color[] pixels = texture.GetPixels(0, num, width, num3);
				for (int j = 0; j < num3; j++)
				{
					float num5 = ((num3 > 1) ? ((bend > 0) ? ((float)j / (float)(num3 - 1)) : ((float)(num3 - 1 - j) / (float)(num3 - 1))) : 0f);
					float num6 = ((float)(num4 - 1) + num5) / 2f;
					float num7 = Mathf.Lerp((float)width, 0f, num6);
					for (int k = 0; k < width; k++)
					{
						if ((float)k >= num7)
						{
							pixels[j * width + k] = Color.clear;
						}
					}
				}
				Texture2D texture2D = new Texture2D(width, num3, 4, false);
				texture2D.filterMode = 0;
				texture2D.wrapMode = 1;
				texture2D.SetPixels(pixels);
				texture2D.Apply(false);
				sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)width, (float)num3), new Vector2(0f, 0.5f), whole.pixelsPerUnit);
			}
			WallArt.slices[new ValueTuple<Texture2D, int, int, int>(texture, i, n, bend)] = sprite;
			return sprite;
		}

		// Token: 0x0400072B RID: 1835
		public static Sprite[] Flesh = new Sprite[0];

		// Token: 0x0400072C RID: 1836
		public static Sprite[] Slime = new Sprite[0];

		// Token: 0x0400072D RID: 1837
		public static Sprite[] Engulf = new Sprite[0];

		// Token: 0x0400072E RID: 1838
		public static Sprite[][] Jaw = new Sprite[0][];

		// Token: 0x0400072F RID: 1839
		public static Sprite[] Gut = new Sprite[0];

		// Token: 0x04000730 RID: 1840
		private static readonly List<Texture2D> gut = new List<Texture2D>();

		// Token: 0x04000731 RID: 1841
		private static readonly List<Texture2D> flesh = new List<Texture2D>();

		// Token: 0x04000732 RID: 1842
		private static readonly List<Texture2D> slime = new List<Texture2D>();

		// Token: 0x04000733 RID: 1843
		private static readonly List<Texture2D> engulf = new List<Texture2D>();

		// Token: 0x04000734 RID: 1844
		private static readonly SortedDictionary<string, Texture2D> jaw = new SortedDictionary<string, Texture2D>();

		// Token: 0x04000735 RID: 1845
		private static float ppu = -1f;

		// Token: 0x04000736 RID: 1846
		private static readonly Dictionary<ValueTuple<Texture2D, int, int, int>, Sprite> slices = new Dictionary<ValueTuple<Texture2D, int, int, int>, Sprite>();

		// Token: 0x04000737 RID: 1847
		public const int LapPixels = 6;
	}
}
