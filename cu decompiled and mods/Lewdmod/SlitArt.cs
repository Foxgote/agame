using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200010B RID: 267
	internal static class SlitArt
	{
		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060005D2 RID: 1490 RVA: 0x0005D53D File Offset: 0x0005B73D
		public static bool Ready
		{
			get
			{
				return SlitArt.Lips.Length != 0 && SlitArt.Stomach && SlitArt.Seg;
			}
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x0005D560 File Offset: 0x0005B760
		public static void Load(string dir)
		{
			if (!Directory.Exists(dir))
			{
				return;
			}
			foreach (string text in Directory.GetFiles(dir, "*.png"))
			{
				Texture2D texture2D = new Texture2D(2, 2, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text));
				texture2D.name = Path.GetFileNameWithoutExtension(text);
				SlitArt.tex[texture2D.name] = texture2D;
			}
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x0005D5D8 File Offset: 0x0005B7D8
		public static void Ensure(float ppu)
		{
			if (Mathf.Approximately(ppu, SlitArt.Ppu) || SlitArt.tex.Count == 0)
			{
				return;
			}
			SlitArt.Ppu = ppu;
			SlitArt.Lips = (from k in SlitArt.tex
				where k.Key.StartsWith("slit_lips_")
				orderby k.Key
				select Sprite.Create(k.Value, new Rect(0f, 0f, (float)k.Value.width, (float)k.Value.height), new Vector2(9f / (float)k.Value.width, 0.5f), ppu)).ToArray<Sprite>();
			Texture2D texture2D;
			if (SlitArt.tex.TryGetValue("stomach", out texture2D))
			{
				SlitArt.Stomach = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(1f, 0.5f), ppu);
			}
			Texture2D texture2D2;
			if (SlitArt.tex.TryGetValue("tentacle_seg", out texture2D2))
			{
				SlitArt.Seg = Sprite.Create(texture2D2, new Rect(0f, 0f, (float)texture2D2.width, (float)texture2D2.height), new Vector2(0f, 0.5f), ppu);
			}
			Texture2D texture2D3;
			if (SlitArt.tex.TryGetValue("tentacle_tip", out texture2D3))
			{
				SlitArt.Tip = Sprite.Create(texture2D3, new Rect(0f, 0f, (float)texture2D3.width, (float)texture2D3.height), new Vector2(0.05f, 0.55f), ppu);
			}
		}

		// Token: 0x0400066B RID: 1643
		public static Sprite[] Lips = new Sprite[0];

		// Token: 0x0400066C RID: 1644
		public static Sprite Stomach;

		// Token: 0x0400066D RID: 1645
		public static Sprite Seg;

		// Token: 0x0400066E RID: 1646
		public static Sprite Tip;

		// Token: 0x0400066F RID: 1647
		public static float Ppu = -1f;

		// Token: 0x04000670 RID: 1648
		private static readonly Dictionary<string, Texture2D> tex = new Dictionary<string, Texture2D>();
	}
}
