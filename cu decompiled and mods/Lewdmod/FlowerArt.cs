using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200008D RID: 141
	internal static class FlowerArt
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060002AE RID: 686 RVA: 0x0002FE0F File Offset: 0x0002E00F
		public static bool Ready
		{
			get
			{
				return FlowerArt.Anim.ContainsKey("idle") && FlowerArt.Seg;
			}
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0002FE30 File Offset: 0x0002E030
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
				FlowerArt.tex[texture2D.name] = texture2D;
			}
			string text2 = Path.Combine(dir, "flower_meta.txt");
			if (!File.Exists(text2))
			{
				return;
			}
			foreach (string text3 in File.ReadAllLines(text2))
			{
				FlowerArt.<>c__DisplayClass9_0 CS$<>8__locals1;
				CS$<>8__locals1.a = text3.Split(new char[] { ' ' });
				if (CS$<>8__locals1.a.Length >= 3)
				{
					if (CS$<>8__locals1.a[0] == "root")
					{
						FlowerArt.rootPx = new Vector2(FlowerArt.<Load>g__F|9_0(1, ref CS$<>8__locals1), FlowerArt.<Load>g__F|9_0(2, ref CS$<>8__locals1));
					}
					else if (CS$<>8__locals1.a.Length >= 9)
					{
						FlowerArt.meta[CS$<>8__locals1.a[0]] = new Vector2[]
						{
							new Vector2(FlowerArt.<Load>g__F|9_0(1, ref CS$<>8__locals1), FlowerArt.<Load>g__F|9_0(2, ref CS$<>8__locals1)),
							new Vector2(FlowerArt.<Load>g__F|9_0(3, ref CS$<>8__locals1), FlowerArt.<Load>g__F|9_0(4, ref CS$<>8__locals1)),
							new Vector2(FlowerArt.<Load>g__F|9_0(5, ref CS$<>8__locals1), FlowerArt.<Load>g__F|9_0(6, ref CS$<>8__locals1)),
							new Vector2(FlowerArt.<Load>g__F|9_0(7, ref CS$<>8__locals1), FlowerArt.<Load>g__F|9_0(8, ref CS$<>8__locals1))
						};
					}
				}
			}
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0002FFD8 File Offset: 0x0002E1D8
		public static void Ensure(float ppu)
		{
			if (Mathf.Approximately(ppu, FlowerArt.Ppu) || FlowerArt.tex.Count == 0)
			{
				return;
			}
			FlowerArt.Ppu = ppu;
			FlowerArt.Anim.Clear();
			Func<KeyValuePair<string, Texture2D>, Sprite> <>9__3;
			foreach (IGrouping<string, KeyValuePair<string, Texture2D>> grouping in from k in FlowerArt.tex
				where k.Key.StartsWith("flower_")
				group k by k.Key.Substring(7, k.Key.LastIndexOf('_') - 7))
			{
				Dictionary<string, Sprite[]> anim = FlowerArt.Anim;
				string key = grouping.Key;
				IEnumerable<KeyValuePair<string, Texture2D>> enumerable = grouping.OrderBy<KeyValuePair<string, Texture2D>, int>((KeyValuePair<string, Texture2D> k) => int.Parse(k.Key.Substring(k.Key.LastIndexOf('_') + 1)));
				Func<KeyValuePair<string, Texture2D>, Sprite> func;
				if ((func = <>9__3) == null)
				{
					func = (<>9__3 = (KeyValuePair<string, Texture2D> k) => Sprite.Create(k.Value, new Rect(0f, 0f, (float)k.Value.width, (float)k.Value.height), new Vector2(FlowerArt.rootPx.x / (float)k.Value.width, 1f - FlowerArt.rootPx.y / (float)k.Value.height), ppu));
				}
				anim[key] = enumerable.Select<KeyValuePair<string, Texture2D>, Sprite>(func).ToArray<Sprite>();
			}
			Texture2D texture2D;
			if (FlowerArt.tex.TryGetValue("tendril", out texture2D))
			{
				FlowerArt.Seg = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0.5f), ppu);
			}
			Texture2D texture2D2;
			if (FlowerArt.tex.TryGetValue("sucker", out texture2D2))
			{
				FlowerArt.Sucker = Sprite.Create(texture2D2, new Rect(0f, 0f, (float)texture2D2.width, (float)texture2D2.height), new Vector2(0.3f, 0.5f), ppu);
			}
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x000301AC File Offset: 0x0002E3AC
		public static Vector2 Offset(Vector2 px, bool flip)
		{
			float num = (px.x - FlowerArt.rootPx.x) / FlowerArt.Ppu;
			return new Vector2(flip ? (-num) : num, (FlowerArt.rootPx.y - px.y) / FlowerArt.Ppu);
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x000301F5 File Offset: 0x0002E3F5
		public static bool Meta(string frame, out Vector2[] m)
		{
			return FlowerArt.meta.TryGetValue(frame, out m);
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0003022D File Offset: 0x0002E42D
		[CompilerGenerated]
		internal static float <Load>g__F|9_0(int i, ref FlowerArt.<>c__DisplayClass9_0 A_1)
		{
			return float.Parse(A_1.a[i], CultureInfo.InvariantCulture);
		}

		// Token: 0x04000282 RID: 642
		public static readonly Dictionary<string, Sprite[]> Anim = new Dictionary<string, Sprite[]>();

		// Token: 0x04000283 RID: 643
		public static Sprite Seg;

		// Token: 0x04000284 RID: 644
		public static Sprite Sucker;

		// Token: 0x04000285 RID: 645
		public static float Ppu = -1f;

		// Token: 0x04000286 RID: 646
		private static readonly Dictionary<string, Texture2D> tex = new Dictionary<string, Texture2D>();

		// Token: 0x04000287 RID: 647
		private static readonly Dictionary<string, Vector2[]> meta = new Dictionary<string, Vector2[]>();

		// Token: 0x04000288 RID: 648
		private static Vector2 rootPx;
	}
}
