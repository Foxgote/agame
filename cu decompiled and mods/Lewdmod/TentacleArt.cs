using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000117 RID: 279
	internal static class TentacleArt
	{
		// Token: 0x0600067D RID: 1661 RVA: 0x0006700C File Offset: 0x0006520C
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
				TentacleArt.tex[texture2D.name] = texture2D;
			}
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x000670C8 File Offset: 0x000652C8
		public static void Ensure(float pixelsPerUnit)
		{
			if (Mathf.Approximately(pixelsPerUnit, TentacleArt.ppu) || TentacleArt.tex.Count == 0)
			{
				return;
			}
			TentacleArt.ppu = pixelsPerUnit;
			TentacleArt.Root = (from k in TentacleArt.tex
				where k.Key.StartsWith("root_")
				orderby k.Key
				select TentacleArt.<Ensure>g__Make|7_0(k.Value, new Vector2(0.5f, 1f))).ToArray<Sprite>();
			TentacleArt.Larva = (from k in TentacleArt.tex
				where k.Key.StartsWith("larva_")
				orderby k.Key
				select TentacleArt.<Ensure>g__Make|7_0(k.Value, new Vector2(0.5f, 0f))).ToArray<Sprite>();
			Texture2D texture2D;
			if (TentacleArt.tex.TryGetValue("seg", out texture2D))
			{
				TentacleArt.Seg = TentacleArt.<Ensure>g__Make|7_0(texture2D, new Vector2(0f, 0.5f));
			}
			Texture2D texture2D2;
			if (TentacleArt.tex.TryGetValue("tip", out texture2D2))
			{
				TentacleArt.Tip = TentacleArt.<Ensure>g__Make|7_0(texture2D2, new Vector2(0f, 0.5f));
			}
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x00067271 File Offset: 0x00065471
		[CompilerGenerated]
		internal static Sprite <Ensure>g__Make|7_0(Texture2D t, Vector2 pivot)
		{
			return Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), pivot, TentacleArt.ppu);
		}

		// Token: 0x040006E7 RID: 1767
		public static Sprite[] Root = new Sprite[0];

		// Token: 0x040006E8 RID: 1768
		public static Sprite[] Larva = new Sprite[0];

		// Token: 0x040006E9 RID: 1769
		public static Sprite Seg;

		// Token: 0x040006EA RID: 1770
		public static Sprite Tip;

		// Token: 0x040006EB RID: 1771
		private static readonly Dictionary<string, Texture2D> tex = new Dictionary<string, Texture2D>();

		// Token: 0x040006EC RID: 1772
		private static float ppu = -1f;
	}
}
