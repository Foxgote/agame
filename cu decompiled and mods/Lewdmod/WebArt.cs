using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200012C RID: 300
	internal static class WebArt
	{
		// Token: 0x0600070A RID: 1802 RVA: 0x0006D690 File Offset: 0x0006B890
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
				texture2D.name = "lewd_web_" + Path.GetFileNameWithoutExtension(text);
				WebArt.tex[Path.GetFileNameWithoutExtension(text)] = texture2D;
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x0600070B RID: 1803 RVA: 0x0006D754 File Offset: 0x0006B954
		public static bool Ready
		{
			get
			{
				return WebArt.Sheet.Length != 0;
			}
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x0006D760 File Offset: 0x0006B960
		public static void Ensure(float pixelsPerUnit)
		{
			if (Mathf.Approximately(pixelsPerUnit, WebArt.ppu) || WebArt.tex.Count == 0)
			{
				return;
			}
			WebArt.ppu = pixelsPerUnit;
			WebArt.Sheet = (from s in WebArt.<Ensure>g__All|13_1("sheet_", new Vector2(0.5f, 0.5f))
				where !s.texture.name.EndsWith("torn")
				select s).ToArray<Sprite>();
			WebArt.Hang = WebArt.<Ensure>g__All|13_1("hang_", new Vector2(0.5f, 1f));
			WebArt.Silk = WebArt.<Ensure>g__All|13_1("silk_", new Vector2(0.5f, 0f));
			WebArt.Old = WebArt.<Ensure>g__All|13_1("old_", new Vector2(0.5f, 1f));
			Texture2D texture2D;
			if (WebArt.tex.TryGetValue("sheet_torn", out texture2D))
			{
				WebArt.SheetTorn = WebArt.<Ensure>g__Make|13_0(texture2D, new Vector2(0.5f, 0.5f));
			}
			Texture2D texture2D2;
			if (WebArt.tex.TryGetValue("corner", out texture2D2))
			{
				WebArt.Corner = WebArt.<Ensure>g__Make|13_0(texture2D2, new Vector2(0f, 1f));
			}
			Texture2D texture2D3;
			if (WebArt.tex.TryGetValue("cocoon", out texture2D3))
			{
				WebArt.Cocoon = WebArt.<Ensure>g__Make|13_0(texture2D3, new Vector2(0.5f, 0.5f));
			}
			Texture2D texture2D4;
			if (WebArt.tex.TryGetValue("cocoon_torn", out texture2D4))
			{
				WebArt.CocoonTorn = WebArt.<Ensure>g__Make|13_0(texture2D4, new Vector2(0.5f, 0.5f));
			}
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x0006D931 File Offset: 0x0006BB31
		[CompilerGenerated]
		internal static Sprite <Ensure>g__Make|13_0(Texture2D t, Vector2 pivot)
		{
			return Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), pivot, WebArt.ppu);
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x0006D95C File Offset: 0x0006BB5C
		[CompilerGenerated]
		internal static Sprite[] <Ensure>g__All|13_1(string prefix, Vector2 pivot)
		{
			return (from kv in WebArt.tex
				where kv.Key.StartsWith(prefix)
				orderby kv.Key
				select WebArt.<Ensure>g__Make|13_0(kv.Value, pivot)).ToArray<Sprite>();
		}

		// Token: 0x04000753 RID: 1875
		public static Sprite[] Sheet = new Sprite[0];

		// Token: 0x04000754 RID: 1876
		public static Sprite[] Hang = new Sprite[0];

		// Token: 0x04000755 RID: 1877
		public static Sprite[] Silk = new Sprite[0];

		// Token: 0x04000756 RID: 1878
		public static Sprite[] Old = new Sprite[0];

		// Token: 0x04000757 RID: 1879
		public static Sprite SheetTorn;

		// Token: 0x04000758 RID: 1880
		public static Sprite Corner;

		// Token: 0x04000759 RID: 1881
		public static Sprite Cocoon;

		// Token: 0x0400075A RID: 1882
		public static Sprite CocoonTorn;

		// Token: 0x0400075B RID: 1883
		private static readonly Dictionary<string, Texture2D> tex = new Dictionary<string, Texture2D>();

		// Token: 0x0400075C RID: 1884
		private static float ppu = -1f;
	}
}
