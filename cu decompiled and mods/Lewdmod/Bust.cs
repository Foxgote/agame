using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200005B RID: 91
	internal static class Bust
	{
		// Token: 0x060001BD RID: 445 RVA: 0x00025F14 File Offset: 0x00024114
		public static void Bind(ConfigFile c)
		{
			Bust.Setting = c.Bind<string>("Body", "BustSize", "random", "Her bust: random (rolled for every run: small, medium or large), small, medium or large.");
			Bust.Setting.SettingChanged += delegate(object s, EventArgs e)
			{
				Bust.applied = -1;
			};
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00025F69 File Offset: 0x00024169
		public static void RegisterChest(Texture2D tex, byte[] medium, byte[] small)
		{
			Bust.chest = tex;
			Bust.chestMedium = medium;
			Bust.chestSmall = small;
			Bust.applied = -1;
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00025F84 File Offset: 0x00024184
		public static int Roll()
		{
			string[] names = Bust.Names;
			ConfigEntry<string> setting = Bust.Setting;
			int num = Array.IndexOf<string>(names, (((setting != null) ? setting.Value : null) ?? "random").Trim().ToLowerInvariant());
			if (num < 0)
			{
				return Random.Range(0, 3);
			}
			return num;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00025FCD File Offset: 0x000241CD
		public static string Name(int size)
		{
			return Bust.Names[Mathf.Clamp(size, 0, 2)];
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00025FE0 File Offset: 0x000241E0
		public static void Update(Body b)
		{
			State s = Plugin.S;
			string[] names = Bust.Names;
			ConfigEntry<string> setting = Bust.Setting;
			int num = Array.IndexOf<string>(names, (((setting != null) ? setting.Value : null) ?? "random").Trim().ToLowerInvariant());
			if (num >= 0)
			{
				s.bust = num;
			}
			else if (s.bust < 0 || s.bust > 2)
			{
				s.bust = Bust.Roll();
			}
			if (s.bust != Bust.applied)
			{
				Bust.applied = s.bust;
				if (Bust.chest && Bust.chestMedium != null)
				{
					ImageConversion.LoadImage(Bust.chest, Bust.chestSmall ?? Bust.chestMedium, false);
					Bust.Forget(Bust.chest);
					Bust.chest.filterMode = 0;
				}
				Plugin.Log.LogInfo("Her bust: " + Bust.Name(Bust.applied));
			}
			Bust.UpdateBreast(b, Bust.applied);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x000260D0 File Offset: 0x000242D0
		private static Bust.Overlay For(Body b)
		{
			Bust.Overlay overlay;
			if (!Bust.overlays.TryGetValue(b, out overlay))
			{
				foreach (Body body in Bust.overlays.Keys.Where<Body>((Body k) => !k).ToList<Body>())
				{
					if (Bust.overlays[body].tex)
					{
						Object.Destroy(Bust.overlays[body].tex);
					}
					Bust.overlays.Remove(body);
				}
				overlay = (Bust.overlays[b] = new Bust.Overlay());
			}
			return overlay;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x000261A8 File Offset: 0x000243A8
		private static void UpdateBreast(Body b, int size)
		{
			Bust.Overlay o = Bust.For(b);
			Limb limb = ((b.limbs.Length > 1) ? b.limbs[1] : null);
			SpriteRenderer torso = (limb ? limb.GetComponent<SpriteRenderer>() : null);
			if (size < 0 || size > 2 || !torso || !torso.enabled || !torso.sprite || limb.dismembered)
			{
				if (o.sr)
				{
					o.sr.enabled = false;
				}
				return;
			}
			if (o.go && o.go.transform.parent == limb.transform && o.sr.enabled && torso.sprite == o.lastSprite && torso.flipX == o.lastFlip && size == o.lastSize && Time.unscaledTime < o.nextLook)
			{
				return;
			}
			o.lastSprite = torso.sprite;
			o.lastFlip = torso.flipX;
			o.lastSize = size;
			o.nextLook = Time.unscaledTime + 0.3f;
			if (!o.go || o.go.transform.parent != limb.transform)
			{
				if (o.go)
				{
					Object.Destroy(o.go);
				}
				if (o.tex)
				{
					Object.Destroy(o.tex);
				}
				o.go = new GameObject("LewdBust");
				o.go.transform.SetParent(limb.transform, false);
				o.sr = o.go.AddComponent<SpriteRenderer>();
				o.sr.sharedMaterial = torso.sharedMaterial;
				o.tex = new Texture2D(7, 10, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
			}
			Sprite sprite = torso.sprite;
			float pixelsPerUnit = sprite.pixelsPerUnit;
			int num = Mathf.RoundToInt(sprite.rect.height);
			int num2 = Mathf.Max(0, (Mathf.RoundToInt(sprite.rect.width) - 8) / 2);
			int num3 = Mathf.Max(0, (num - 10) / 2);
			List<SpriteRenderer> list = (from r in limb.GetComponentsInChildren<SpriteRenderer>()
				where r != o.sr && r.enabled && r.sprite && r.sortingLayerID == torso.sortingLayerID && r.sortingOrder >= torso.sortingOrder && r.name != "LewdSplat" && r.name != "LewdBelly" && r.name != "LewdBlush"
				orderby r.sortingOrder
				select r).ToList<SpriteRenderer>();
			if (!list.Contains(torso))
			{
				list.Insert(0, torso);
			}
			int num4 = torso.sortingOrder;
			Color color = default(Color);
			Color color2 = default(Color);
			bool flag = false;
			int num5 = list.Count - 1;
			while (num5 >= 0 && !flag)
			{
				SpriteRenderer spriteRenderer = list[num5];
				Color color3;
				if (Bust.At(spriteRenderer, sprite, 6 + num2, 3 + num3, out color3) && color3.a >= 0.5f)
				{
					List<Color> list2 = new List<Color>();
					for (int i = 1; i <= 5; i++)
					{
						for (int j = 1; j <= 6; j++)
						{
							Color color4;
							if (Bust.At(spriteRenderer, sprite, j + num2, i + num3, out color4) && color4.a > 0.5f)
							{
								list2.Add(color4);
							}
						}
					}
					if (list2.Count != 0)
					{
						list2.Sort((Color p, Color q) => Bust.Lum(p).CompareTo(Bust.Lum(q)));
						color = list2[list2.Count / 2] * spriteRenderer.color;
						color2 = list2[0] * spriteRenderer.color;
						if (Bust.Lum(color2) > Bust.Lum(color) * 0.85f)
						{
							color2 = color * 0.75f;
						}
						color.a = (color2.a = 1f);
						num4 = Mathf.Max(num4, spriteRenderer.sortingOrder);
						flag = true;
					}
				}
				num5--;
			}
			Color32[] array = new Color32[70];
			if (flag)
			{
				ValueTuple<int, int>[] array2 = Bust.Shapes[size];
				Color color5 = Color.Lerp(color, Color.white, 0.18f);
				Color color6 = color * 0.85f;
				color5.a = (color6.a = 1f);
				for (int k = 0; k < array2.Length; k++)
				{
					ValueTuple<int, int> valueTuple = array2[k];
					int item = valueTuple.Item1;
					int item2 = valueTuple.Item2;
					for (int l = 4; l <= item2; l++)
					{
						Color color7 = ((l == item2) ? color2 : color);
						if (l == item2 - 1 && k == 1)
						{
							color7 = color5;
						}
						if (l == item2 - 1 && k == array2.Length - 1)
						{
							color7 = color6;
						}
						array[(9 - item) * 7 + (l - 4)] = color7;
					}
				}
				int tip = Bust.TipRow[size];
				if (tip >= 0)
				{
					int num6 = array2.First<ValueTuple<int, int>>(([TupleElementNames(new string[] { "y", "edge" })] ValueTuple<int, int> t) => t.Item1 == tip).Item2 + 1;
					Color color8 = color2 * 0.85f;
					color8.a = 1f;
					if (num6 - 4 < 7)
					{
						array[(9 - tip) * 7 + (num6 - 4)] = color8;
					}
				}
			}
			if (Bust.DebugRed)
			{
				for (int m = 0; m < array.Length; m++)
				{
					array[m] = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
				}
			}
			o.tex.SetPixels32(array);
			o.tex.Apply(false);
			if (!o.sr.sprite || !Mathf.Approximately(o.sr.sprite.pixelsPerUnit, pixelsPerUnit))
			{
				o.sr.sprite = Sprite.Create(o.tex, new Rect(0f, 0f, 7f, 10f), new Vector2(0f, 1f), pixelsPerUnit);
			}
			bool flipX = torso.flipX;
			float num7 = sprite.bounds.min.x + (float)(4 + num2) / pixelsPerUnit;
			float num8 = sprite.bounds.min.y + (float)(num - num3) / pixelsPerUnit;
			o.go.transform.localPosition = new Vector3(flipX ? (-num7) : num7, num8, -0.001f);
			o.go.transform.localRotation = Quaternion.identity;
			o.go.transform.localScale = Vector3.one;
			o.sr.flipX = flipX;
			o.sr.flipY = torso.flipY;
			o.sr.sortingLayerID = torso.sortingLayerID;
			Limb limb2 = b.LimbByName("UpArmF");
			SpriteRenderer spriteRenderer2 = ((limb2 && !limb2.dismembered) ? limb2.GetComponent<SpriteRenderer>() : null);
			o.sr.sortingOrder = Mathf.Max(num4, (spriteRenderer2 && spriteRenderer2.sortingLayerID == torso.sortingLayerID) ? spriteRenderer2.sortingOrder : num4) + 1;
			o.sr.enabled = true;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00026A28 File Offset: 0x00024C28
		public static string Debug(Body b)
		{
			Bust.Overlay o = Bust.For(b);
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			string text = string.Join(", ", from r in b.GetComponentsInChildren<SpriteRenderer>()
				where r.enabled && r.sprite && o.sr && r.bounds.Intersects(o.sr.bounds) && r != o.sr
				select string.Format("{0}:{1}", r.name, r.sortingOrder));
			int num;
			if (!o.tex)
			{
				num = -1;
			}
			else
			{
				num = o.tex.GetPixels32().Count<Color32>((Color32 c) => c.a > 0);
			}
			int num2 = num;
			string text2;
			try
			{
				Color color;
				text2 = string.Format("readable {0} front {1} px(front,4) {2} rect {3}", new object[]
				{
					component.sprite.texture.isReadable,
					Bust.FrontEdge(component.sprite, 4),
					Bust.TryPixel(component.sprite, Bust.FrontEdge(component.sprite, 4), 4, out color) ? color.ToString() : "fail",
					component.sprite.textureRect
				});
			}
			catch (Exception ex)
			{
				text2 = "read error " + ex.Message;
			}
			string[] array = new string[7];
			array[0] = text2;
			array[1] = string.Format("; o.go active {0} visible {1} z {2}; torso bounds {3}-{4} layer {5}; o.go bounds {6} layer {7} sprite {8} painted {9} color {10} mat {11} / torso mat {12}; ", new object[]
			{
				o.go && o.go.activeInHierarchy,
				o.sr && o.sr.isVisible,
				o.go ? o.go.transform.position.z : 0f,
				component.bounds.min,
				component.bounds.max,
				component.sortingLayerName,
				o.sr ? (o.sr.bounds.min.ToString() + "-" + o.sr.bounds.max.ToString()) : "-",
				o.sr ? o.sr.sortingLayerName : "-",
				(o.sr && o.sr.sprite) ? "yes" : "no",
				num2,
				o.sr ? o.sr.color.ToString() : "-",
				(o.sr && o.sr.sharedMaterial) ? o.sr.sharedMaterial.name : "-",
				component.sharedMaterial ? component.sharedMaterial.name : "-"
			});
			int num3 = 2;
			string text3 = "applied {0} torso {1}/{2} order {3} flip {4} ";
			object[] array2 = new object[5];
			array2[0] = Bust.Name(Bust.applied);
			int num4 = 1;
			Sprite sprite = component.sprite;
			array2[num4] = ((sprite != null) ? sprite.name : null);
			int num5 = 2;
			Sprite sprite2 = component.sprite;
			object obj;
			if (sprite2 == null)
			{
				obj = null;
			}
			else
			{
				Texture2D texture = sprite2.texture;
				obj = ((texture != null) ? texture.name : null);
			}
			array2[num5] = obj;
			array2[3] = component.sortingOrder;
			array2[4] = component.flipX;
			array[num3] = string.Format(text3, array2);
			array[3] = "o.go ";
			array[4] = (o.sr ? string.Format("{0} order {1} at {2}", o.sr.enabled, o.sr.sortingOrder, o.go.transform.position) : "none");
			array[5] = "; over it: ";
			array[6] = text;
			return string.Concat(array);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00026F00 File Offset: 0x00025100
		private static float Lum(Color c)
		{
			return c.r * 0.3f + c.g * 0.59f + c.b * 0.11f;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00026F28 File Offset: 0x00025128
		private static bool At(SpriteRenderer r, Sprite chest, int x, int yFromTop, out Color c)
		{
			Sprite sprite = r.sprite;
			float pixelsPerUnit = chest.pixelsPerUnit;
			int num = Mathf.RoundToInt((chest.bounds.min.x - sprite.bounds.min.x) * pixelsPerUnit);
			int num2 = Mathf.RoundToInt((sprite.bounds.max.y - chest.bounds.max.y) * pixelsPerUnit);
			return Bust.TryPixel(sprite, x + num, yFromTop + num2, out c);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00026FB4 File Offset: 0x000251B4
		private static int FrontEdge(Sprite s, int rowFromTop)
		{
			int num = Mathf.RoundToInt(s.rect.width);
			for (int i = num - 1; i >= 0; i--)
			{
				Color color;
				if (Bust.TryPixel(s, i, rowFromTop, out color) && color.a > 0.5f)
				{
					return i;
				}
			}
			return num - 1;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00027004 File Offset: 0x00025204
		internal static Texture2D Readable(Texture2D t)
		{
			if (!t || t.isReadable)
			{
				return t;
			}
			Texture2D texture2D;
			if (Bust.readable.TryGetValue(t, out texture2D) && texture2D)
			{
				return texture2D;
			}
			RenderTexture temporary = RenderTexture.GetTemporary(t.width, t.height, 0, 0, 2);
			RenderTexture active = RenderTexture.active;
			Graphics.Blit(t, temporary);
			RenderTexture.active = temporary;
			texture2D = new Texture2D(t.width, t.height, 4, false)
			{
				filterMode = 0
			};
			texture2D.ReadPixels(new Rect(0f, 0f, (float)t.width, (float)t.height), 0, 0);
			texture2D.Apply(false);
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
			Bust.readable[t] = texture2D;
			return texture2D;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x000270C4 File Offset: 0x000252C4
		private static void Forget(Texture2D t)
		{
			foreach (Texture2D texture2D in Bust.readable.Values)
			{
				if (texture2D)
				{
					Object.Destroy(texture2D);
				}
			}
			Bust.readable.Clear();
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0002712C File Offset: 0x0002532C
		internal static bool TryPixel(Sprite s, int x, int rowFromTop, out Color c)
		{
			c = default(Color);
			bool flag;
			try
			{
				Rect textureRect = s.textureRect;
				int num = Mathf.RoundToInt(textureRect.height);
				if (x < 0 || (float)x >= textureRect.width || rowFromTop < 0 || rowFromTop >= num)
				{
					flag = false;
				}
				else
				{
					c = Bust.Readable(s.texture).GetPixel(Mathf.RoundToInt(textureRect.x) + x, Mathf.RoundToInt(textureRect.y) + num - 1 - rowFromTop);
					flag = true;
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x040001CB RID: 459
		public const int Small = 0;

		// Token: 0x040001CC RID: 460
		public const int Medium = 1;

		// Token: 0x040001CD RID: 461
		public const int Large = 2;

		// Token: 0x040001CE RID: 462
		public static ConfigEntry<string> Setting;

		// Token: 0x040001CF RID: 463
		private static readonly string[] Names = new string[] { "small", "medium", "large" };

		// Token: 0x040001D0 RID: 464
		private static Texture2D chest;

		// Token: 0x040001D1 RID: 465
		private static byte[] chestMedium;

		// Token: 0x040001D2 RID: 466
		private static byte[] chestSmall;

		// Token: 0x040001D3 RID: 467
		private static int applied = -1;

		// Token: 0x040001D4 RID: 468
		[TupleElementNames(new string[] { "y", "edge" })]
		private static readonly ValueTuple<int, int>[][] Shapes = new ValueTuple<int, int>[][]
		{
			new ValueTuple<int, int>[]
			{
				new ValueTuple<int, int>(1, 6),
				new ValueTuple<int, int>(2, 7),
				new ValueTuple<int, int>(3, 8),
				new ValueTuple<int, int>(4, 8),
				new ValueTuple<int, int>(5, 7),
				new ValueTuple<int, int>(6, 6)
			},
			new ValueTuple<int, int>[]
			{
				new ValueTuple<int, int>(1, 6),
				new ValueTuple<int, int>(2, 7),
				new ValueTuple<int, int>(3, 8),
				new ValueTuple<int, int>(4, 9),
				new ValueTuple<int, int>(5, 9),
				new ValueTuple<int, int>(6, 8),
				new ValueTuple<int, int>(7, 7)
			},
			new ValueTuple<int, int>[]
			{
				new ValueTuple<int, int>(1, 6),
				new ValueTuple<int, int>(2, 7),
				new ValueTuple<int, int>(3, 8),
				new ValueTuple<int, int>(4, 9),
				new ValueTuple<int, int>(5, 9),
				new ValueTuple<int, int>(6, 9),
				new ValueTuple<int, int>(7, 8)
			}
		};

		// Token: 0x040001D5 RID: 469
		private static readonly int[] TipRow = new int[] { -1, -1, -1 };

		// Token: 0x040001D6 RID: 470
		private const int Left = 4;

		// Token: 0x040001D7 RID: 471
		private const int Wide = 7;

		// Token: 0x040001D8 RID: 472
		private const int ChestW = 8;

		// Token: 0x040001D9 RID: 473
		private const int ChestH = 10;

		// Token: 0x040001DA RID: 474
		private static readonly Dictionary<Body, Bust.Overlay> overlays = new Dictionary<Body, Bust.Overlay>();

		// Token: 0x040001DB RID: 475
		public static bool DebugRed;

		// Token: 0x040001DC RID: 476
		private static readonly Dictionary<Texture2D, Texture2D> readable = new Dictionary<Texture2D, Texture2D>();

		// Token: 0x02000185 RID: 389
		private sealed class Overlay
		{
			// Token: 0x040008F1 RID: 2289
			public GameObject go;

			// Token: 0x040008F2 RID: 2290
			public SpriteRenderer sr;

			// Token: 0x040008F3 RID: 2291
			public Texture2D tex;

			// Token: 0x040008F4 RID: 2292
			public Sprite lastSprite;

			// Token: 0x040008F5 RID: 2293
			public bool lastFlip;

			// Token: 0x040008F6 RID: 2294
			public int lastSize = -1;

			// Token: 0x040008F7 RID: 2295
			public float nextLook;
		}
	}
}
