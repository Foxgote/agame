using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000088 RID: 136
	internal static class FleshArt
	{
		// Token: 0x0600027D RID: 637 RVA: 0x0002D5B4 File Offset: 0x0002B7B4
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
				if (fileNameWithoutExtension.StartsWith("flesh_"))
				{
					FleshArt.floorTex.Add(texture2D);
				}
				else if (fileNameWithoutExtension.StartsWith("mound_"))
				{
					FleshArt.moundTex.Add(texture2D);
				}
				else if (fileNameWithoutExtension.StartsWith("carpet_"))
				{
					FleshArt.carpetTex.Add(texture2D);
				}
				else if (fileNameWithoutExtension.StartsWith("hang_"))
				{
					FleshArt.hangTex.Add(texture2D);
				}
				else if (fileNameWithoutExtension.StartsWith("vein_"))
				{
					FleshArt.veinTex.Add(texture2D);
				}
				else if (fileNameWithoutExtension == "tendril")
				{
					FleshArt.tendrilTex = texture2D;
				}
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0002D6FC File Offset: 0x0002B8FC
		public static void Ensure(float pixelsPerUnit)
		{
			if (Mathf.Approximately(pixelsPerUnit, FleshArt.ppu))
			{
				return;
			}
			FleshArt.ppu = pixelsPerUnit;
			FleshArt.Floor = FleshArt.floorTex.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0.5f, 0f), FleshArt.ppu)).ToArray<Sprite>();
			FleshArt.Mound = FleshArt.moundTex.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0.5f, 0f), FleshArt.ppu)).ToArray<Sprite>();
			FleshArt.Carpet = FleshArt.carpetTex.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0.5f, 0f), FleshArt.ppu)).ToArray<Sprite>();
			FleshArt.Hang = FleshArt.hangTex.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0.5f, 1f), FleshArt.ppu)).ToArray<Sprite>();
			FleshArt.Vein = FleshArt.veinTex.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0.5f, 0.5f), FleshArt.ppu)).ToArray<Sprite>();
			if (FleshArt.tendrilTex)
			{
				FleshArt.Tendril = Sprite.Create(FleshArt.tendrilTex, new Rect(0f, 0f, (float)FleshArt.tendrilTex.width, (float)FleshArt.tendrilTex.height), new Vector2(0f, 0.5f), FleshArt.ppu);
			}
		}

		// Token: 0x04000260 RID: 608
		public static Sprite[] Floor = new Sprite[0];

		// Token: 0x04000261 RID: 609
		public static Sprite[] Mound = new Sprite[0];

		// Token: 0x04000262 RID: 610
		public static Sprite[] Carpet = new Sprite[0];

		// Token: 0x04000263 RID: 611
		public static Sprite[] Hang = new Sprite[0];

		// Token: 0x04000264 RID: 612
		public static Sprite[] Vein = new Sprite[0];

		// Token: 0x04000265 RID: 613
		public static Sprite Tendril;

		// Token: 0x04000266 RID: 614
		private static readonly List<Texture2D> floorTex = new List<Texture2D>();

		// Token: 0x04000267 RID: 615
		private static readonly List<Texture2D> moundTex = new List<Texture2D>();

		// Token: 0x04000268 RID: 616
		private static readonly List<Texture2D> carpetTex = new List<Texture2D>();

		// Token: 0x04000269 RID: 617
		private static readonly List<Texture2D> hangTex = new List<Texture2D>();

		// Token: 0x0400026A RID: 618
		private static readonly List<Texture2D> veinTex = new List<Texture2D>();

		// Token: 0x0400026B RID: 619
		private static Texture2D tendrilTex;

		// Token: 0x0400026C RID: 620
		private static float ppu = -1f;
	}
}
