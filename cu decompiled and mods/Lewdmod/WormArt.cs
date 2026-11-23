using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000136 RID: 310
	internal static class WormArt
	{
		// Token: 0x0600075A RID: 1882 RVA: 0x000711E8 File Offset: 0x0006F3E8
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
				Texture2D texture2D = new Texture2D(2, 2, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text));
				texture2D.name = Path.GetFileNameWithoutExtension(text);
				WormArt.tex[texture2D.name] = texture2D;
			}
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x000712A4 File Offset: 0x0006F4A4
		public static void Ensure(float pixelsPerUnit)
		{
			if (Mathf.Approximately(pixelsPerUnit, WormArt.ppu) || WormArt.tex.Count == 0)
			{
				return;
			}
			WormArt.ppu = pixelsPerUnit;
			WormArt.Head = new Sprite[]
			{
				WormArt.<Ensure>g__Make|10_0("head_0", new Vector2(0.4f, 0.5f)),
				WormArt.<Ensure>g__Make|10_0("head_1", new Vector2(0.4f, 0.5f))
			}.Where<Sprite>((Sprite s) => s).ToArray<Sprite>();
			WormArt.Seg = WormArt.<Ensure>g__Make|10_0("seg", new Vector2(0.5f, 0.5f));
			WormArt.Tail = WormArt.<Ensure>g__Make|10_0("tail", new Vector2(0.2f, 0.5f));
			WormArt.Gut = WormArt.<Ensure>g__Make|10_0("gut", new Vector2(0.5f, 0.5f));
			Texture2D texture2D;
			if (WormArt.tex.TryGetValue("wall", out texture2D))
			{
				WormArt.WallInset = new int[texture2D.height];
				for (int i = 0; i < texture2D.height; i++)
				{
					int num = -1;
					int num2 = texture2D.width - 1;
					while (num2 >= 0 && num < 0)
					{
						if (texture2D.GetPixel(num2, i).a > 0.1f)
						{
							num = num2;
						}
						num2--;
					}
					WormArt.WallInset[i] = ((num < 0) ? texture2D.width : (texture2D.width - 1 - num));
				}
				int num3 = int.MaxValue;
				for (int j = 0; j < WormArt.WallInset.Length; j++)
				{
					int num4 = WormArt.WallInset[j] * 1000 + Mathf.Abs(j - WormArt.WallInset.Length / 2);
					if (num4 < num3)
					{
						num3 = num4;
						WormArt.WallWaist = j;
					}
				}
			}
			WormArt.Wall = WormArt.<Ensure>g__Make|10_0("wall", new Vector2(1f, 0.5f));
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x000714B8 File Offset: 0x0006F6B8
		[CompilerGenerated]
		internal static Sprite <Ensure>g__Make|10_0(string n, Vector2 pivot)
		{
			Texture2D texture2D;
			if (!WormArt.tex.TryGetValue(n, out texture2D))
			{
				return null;
			}
			return Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), pivot, WormArt.ppu);
		}

		// Token: 0x04000785 RID: 1925
		public static Sprite[] Head = new Sprite[0];

		// Token: 0x04000786 RID: 1926
		public static Sprite Seg;

		// Token: 0x04000787 RID: 1927
		public static Sprite Tail;

		// Token: 0x04000788 RID: 1928
		public static Sprite Gut;

		// Token: 0x04000789 RID: 1929
		public static Sprite Wall;

		// Token: 0x0400078A RID: 1930
		public static int[] WallInset = new int[0];

		// Token: 0x0400078B RID: 1931
		public static int WallWaist;

		// Token: 0x0400078C RID: 1932
		private static readonly Dictionary<string, Texture2D> tex = new Dictionary<string, Texture2D>();

		// Token: 0x0400078D RID: 1933
		private static float ppu = -1f;
	}
}
