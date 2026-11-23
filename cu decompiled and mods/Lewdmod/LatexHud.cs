using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace LewdStatus
{
	// Token: 0x020000AB RID: 171
	internal static class LatexHud
	{
		// Token: 0x060003A0 RID: 928 RVA: 0x0003C5A4 File Offset: 0x0003A7A4
		public static void Update(Body b)
		{
			PlayerCamera main = PlayerCamera.main;
			if (!main || !main.woundView)
			{
				return;
			}
			if (!Suit.Latex || Suit.Stage < 5)
			{
				if (LatexHud.done.Count > 0)
				{
					LatexHud.Restore();
				}
				return;
			}
			if (!main.woundView.activeInHierarchy || Time.unscaledTime < LatexHud.next)
			{
				return;
			}
			LatexHud.next = Time.unscaledTime + 0.4f;
			foreach (Graphic graphic in main.woundView.GetComponentsInChildren<Graphic>(true))
			{
				if (graphic && !LatexHud.Ours(graphic))
				{
					LatexHud.Recolour(graphic);
				}
			}
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0003C658 File Offset: 0x0003A858
		private static bool Ours(Graphic g)
		{
			Transform transform = g.transform;
			while (transform)
			{
				if (transform.name.StartsWith("Lewd"))
				{
					return true;
				}
				transform = transform.parent;
			}
			return false;
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0003C694 File Offset: 0x0003A894
		private static void Recolour(Graphic g)
		{
			ValueTuple<Color, Color, Sprite, Sprite, Texture, Texture> valueTuple;
			LatexHud.done.TryGetValue(g, out valueTuple);
			bool flag = LatexHud.done.ContainsKey(g);
			Color color = ((flag && g.color == valueTuple.Item2) ? valueTuple.Item1 : g.color);
			Sprite sprite = null;
			Sprite sprite2 = null;
			Texture texture = null;
			Texture texture2 = null;
			Image image = g as Image;
			if (image != null && image.sprite)
			{
				sprite = ((flag && image.sprite == valueTuple.Item4) ? valueTuple.Item3 : image.sprite);
				sprite2 = LatexHud.Turn(sprite);
				if (image.sprite != sprite2)
				{
					image.sprite = sprite2;
				}
			}
			else
			{
				RawImage rawImage = g as RawImage;
				if (rawImage != null)
				{
					Texture2D texture2D = rawImage.texture as Texture2D;
					if (texture2D != null)
					{
						texture = ((flag && rawImage.texture == valueTuple.Item6) ? valueTuple.Item5 : texture2D);
						texture2 = LatexHud.Turn(texture as Texture2D);
						if (rawImage.texture != texture2)
						{
							rawImage.texture = texture2;
						}
					}
				}
			}
			Color color2 = LatexHud.Turn(color);
			if (g.color != color2)
			{
				g.color = color2;
			}
			LatexHud.done[g] = new ValueTuple<Color, Color, Sprite, Sprite, Texture, Texture>(color, color2, sprite, sprite2, texture, texture2);
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x0003C7F0 File Offset: 0x0003A9F0
		private static void Restore()
		{
			foreach (KeyValuePair<Graphic, ValueTuple<Color, Color, Sprite, Sprite, Texture, Texture>> keyValuePair in LatexHud.done)
			{
				Graphic key = keyValuePair.Key;
				if (key)
				{
					ValueTuple<Color, Color, Sprite, Sprite, Texture, Texture> value = keyValuePair.Value;
					if (key.color == value.Item2)
					{
						key.color = value.Item1;
					}
					Image image = key as Image;
					if (image != null && value.Item3 && image.sprite == value.Item4)
					{
						image.sprite = value.Item3;
					}
					RawImage rawImage = key as RawImage;
					if (rawImage != null && value.Item5 && rawImage.texture == value.Item6)
					{
						rawImage.texture = value.Item5;
					}
				}
			}
			LatexHud.done.Clear();
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0003C8FC File Offset: 0x0003AAFC
		private static Color Turn(Color c)
		{
			float num;
			float num2;
			float num3;
			Color.RGBToHSV(c, ref num, ref num2, ref num3);
			if (num2 < 0.15f)
			{
				return c;
			}
			Color color = Color.HSVToRGB((num + 0.3472222f) % 1f, num2, num3);
			color.a = c.a;
			return color;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x0003C944 File Offset: 0x0003AB44
		private static Texture2D Turn(Texture2D t)
		{
			if (!t)
			{
				return t;
			}
			Texture2D texture2D;
			if (LatexHud.textures.TryGetValue(t, out texture2D) && texture2D)
			{
				return texture2D;
			}
			Texture2D texture2D2 = Bust.Readable(t);
			texture2D = new Texture2D(t.width, t.height, 4, false)
			{
				filterMode = t.filterMode,
				wrapMode = t.wrapMode
			};
			Color32[] pixels = texture2D2.GetPixels32();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = LatexHud.Turn(pixels[i]);
			}
			texture2D.SetPixels32(pixels);
			texture2D.Apply(false);
			return LatexHud.textures[t] = texture2D;
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x0003C9F4 File Offset: 0x0003ABF4
		private static Sprite Turn(Sprite s)
		{
			if (!s || !s.texture)
			{
				return s;
			}
			Sprite sprite;
			if (LatexHud.sprites.TryGetValue(s, out sprite) && sprite)
			{
				return sprite;
			}
			Texture2D texture2D = LatexHud.Turn(s.texture);
			Rect rect = s.rect;
			sprite = Sprite.Create(texture2D, rect, new Vector2(s.pivot.x / rect.width, s.pivot.y / rect.height), s.pixelsPerUnit, 0U, 0, s.border);
			sprite.name = s.name + "_violet";
			return LatexHud.sprites[s] = sprite;
		}

		// Token: 0x04000355 RID: 853
		private const float HueTurn = 0.3472222f;

		// Token: 0x04000356 RID: 854
		private static readonly Dictionary<Texture2D, Texture2D> textures = new Dictionary<Texture2D, Texture2D>();

		// Token: 0x04000357 RID: 855
		private static readonly Dictionary<Sprite, Sprite> sprites = new Dictionary<Sprite, Sprite>();

		// Token: 0x04000358 RID: 856
		[TupleElementNames(new string[] { "own", "ours", "ownSprite", "ourSprite", "ownTex", "ourTex" })]
		private static readonly Dictionary<Graphic, ValueTuple<Color, Color, Sprite, Sprite, Texture, Texture>> done = new Dictionary<Graphic, ValueTuple<Color, Color, Sprite, Sprite, Texture, Texture>>();

		// Token: 0x04000359 RID: 857
		private static float next;
	}
}
