using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LewdStatus
{
	// Token: 0x02000054 RID: 84
	internal static class BioWarning
	{
		// Token: 0x06000198 RID: 408 RVA: 0x0001ABFE File Offset: 0x00018DFE
		public static void Show()
		{
			BioWarning.shownAt = Time.time;
			Plugin.Log.LogInfo("BIOLOGICAL WARNING");
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0001AC19 File Offset: 0x00018E19
		public static void Bar(float value, float left)
		{
			BioWarning.barWanted = true;
			BioWarning.barValue = Mathf.Clamp01(value);
			BioWarning.barLeft = Mathf.Clamp01(left);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0001AC37 File Offset: 0x00018E37
		public static void HideBar()
		{
			BioWarning.barWanted = false;
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0001AC40 File Offset: 0x00018E40
		public static void Update(Body b)
		{
			PlayerCamera main = PlayerCamera.main;
			if (!main || !b)
			{
				return;
			}
			float num = Time.time - BioWarning.shownAt;
			bool flag = num < 4.5f && LewdSettings.On && b.alive;
			bool flag2 = BioWarning.barWanted && b.alive;
			if (!flag && !flag2)
			{
				if (BioWarning.root)
				{
					BioWarning.root.SetActive(false);
				}
				if (BioWarning.barRoot)
				{
					BioWarning.barRoot.SetActive(false);
				}
				return;
			}
			if (!BioWarning.root && !BioWarning.Build(main))
			{
				return;
			}
			BioWarning.root.SetActive(flag);
			BioWarning.barRoot.SetActive(flag2);
			if (flag)
			{
				float num2 = Mathf.Clamp01(Mathf.Min(num / 0.25f, (4.5f - num) / 0.25f));
				num2 = num2 * num2 * (3f - 2f * num2);
				float num3 = 98f * BioWarning.k;
				float num4 = ((RectTransform)BioWarning.root.transform.parent).rect.width * 0.012f;
				BioWarning.rt.anchoredPosition = new Vector2(-num3 + (num3 + num4) * num2, BioWarning.rt.anchoredPosition.y);
				BioWarning.img.texture = (((int)(num / 0.3f) % 2 == 0) ? BioWarning.texOn : BioWarning.texOff);
			}
			if (flag2)
			{
				BioWarning.barFill.sizeDelta = new Vector2(BioWarning.barW * BioWarning.barValue, BioWarning.barFill.sizeDelta.y);
				BioWarning.barTime.sizeDelta = new Vector2(BioWarning.barW * BioWarning.barLeft, BioWarning.barTime.sizeDelta.y);
				BioWarning.barKey.text = string.Format("[{0}]", Plugin.RemoveKey.Value);
				BioWarning.barKey.enabled = (int)(Time.time / 0.25f) % 2 == 0;
			}
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0001AE50 File Offset: 0x00019050
		private static bool Build(PlayerCamera pc)
		{
			if (!pc.woundView)
			{
				return false;
			}
			Canvas componentInParent = pc.woundView.GetComponentInParent<Canvas>();
			if (!componentInParent)
			{
				return false;
			}
			RectTransform rectTransform = (RectTransform)componentInParent.transform;
			BioWarning.k = rectTransform.rect.height * 0.034f / 21f;
			BioWarning.texOn = BioWarning.Draw(true);
			BioWarning.texOff = BioWarning.Draw(false);
			BioWarning.root = new GameObject("LewdBioWarning", new Type[]
			{
				typeof(RectTransform),
				typeof(RawImage)
			});
			BioWarning.root.transform.SetParent(componentInParent.transform, false);
			BioWarning.rt = (RectTransform)BioWarning.root.transform;
			RectTransform rectTransform2 = BioWarning.rt;
			RectTransform rectTransform3 = BioWarning.rt;
			RectTransform rectTransform4 = BioWarning.rt;
			Vector2 vector;
			vector..ctor(0f, 1f);
			rectTransform4.pivot = vector;
			rectTransform2.anchorMin = (rectTransform3.anchorMax = vector);
			BioWarning.rt.sizeDelta = new Vector2(98f * BioWarning.k, 21f * BioWarning.k);
			BioWarning.rt.anchoredPosition = new Vector2(-98f * BioWarning.k, -rectTransform.rect.height * 0.12f);
			BioWarning.img = BioWarning.root.GetComponent<RawImage>();
			BioWarning.img.raycastTarget = false;
			BioWarning.img.texture = BioWarning.texOn;
			BioWarning.root.transform.SetAsLastSibling();
			float num = rectTransform.rect.height * 0.006f;
			BioWarning.barW = 60f * num;
			BioWarning.barRoot = new GameObject("LewdMiteBar", new Type[]
			{
				typeof(RectTransform),
				typeof(Image)
			});
			BioWarning.barRoot.transform.SetParent(componentInParent.transform, false);
			RectTransform rectTransform5 = (RectTransform)BioWarning.barRoot.transform;
			RectTransform rectTransform6 = rectTransform5;
			RectTransform rectTransform7 = rectTransform5;
			RectTransform rectTransform8 = rectTransform5;
			vector..ctor(0.5f, 0f);
			rectTransform8.pivot = vector;
			rectTransform6.anchorMin = (rectTransform7.anchorMax = vector);
			rectTransform5.sizeDelta = new Vector2(BioWarning.barW + 2f * num, 5f * num);
			rectTransform5.anchoredPosition = new Vector2(0f, rectTransform.rect.height * 0.2f);
			Image component = BioWarning.barRoot.GetComponent<Image>();
			component.color = new Color32(14, 12, 6, 220);
			component.raycastTarget = false;
			BioWarning.barFill = BioWarning.Rect(rectTransform5, "LewdMiteBarFill", new Color32(250, 206, 30, byte.MaxValue), new Vector2(num, 1.5f * num), 3f * num);
			BioWarning.barTime = BioWarning.Rect(rectTransform5, "LewdMiteBarTime", new Color32(230, 60, 40, byte.MaxValue), new Vector2(num, 0.5f * num), 0.6f * num);
			GameObject gameObject = new GameObject("LewdMiteBarKey", new Type[] { typeof(RectTransform) });
			gameObject.transform.SetParent(rectTransform5, false);
			RectTransform rectTransform9 = (RectTransform)gameObject.transform;
			vector..ctor(1f, 0.5f);
			rectTransform9.pivot = vector;
			rectTransform9.anchorMin = (rectTransform9.anchorMax = vector);
			rectTransform9.anchoredPosition = new Vector2(-BioWarning.barW - 4f * num, 0f);
			rectTransform9.sizeDelta = new Vector2(20f * num, 6f * num);
			BioWarning.barKey = gameObject.AddComponent<TextMeshProUGUI>();
			TMP_FontAsset tmp_FontAsset = (from x in pc.woundView.GetComponentsInChildren<TextMeshProUGUI>(true)
				select x.font).FirstOrDefault<TMP_FontAsset>((TMP_FontAsset f) => f);
			if (tmp_FontAsset)
			{
				BioWarning.barKey.font = tmp_FontAsset;
			}
			BioWarning.barKey.fontSize = 5f * num;
			BioWarning.barKey.alignment = 516;
			BioWarning.barKey.enableWordWrapping = false;
			BioWarning.barKey.color = new Color32(250, 206, 30, byte.MaxValue);
			BioWarning.barKey.raycastTarget = false;
			BioWarning.barRoot.transform.SetAsLastSibling();
			Plugin.Log.LogInfo(string.Format("Biological warning built ({0:0}x{1:0})", 98f * BioWarning.k, 21f * BioWarning.k));
			return true;
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0001B31C File Offset: 0x0001951C
		private static RectTransform Rect(RectTransform parent, string name, Color c, Vector2 at, float height)
		{
			GameObject gameObject = new GameObject(name, new Type[]
			{
				typeof(RectTransform),
				typeof(Image)
			});
			gameObject.transform.SetParent(parent, false);
			RectTransform rectTransform = (RectTransform)gameObject.transform;
			Vector2 vector;
			vector..ctor(0f, 0f);
			rectTransform.pivot = vector;
			rectTransform.anchorMin = (rectTransform.anchorMax = vector);
			rectTransform.anchoredPosition = at;
			rectTransform.sizeDelta = new Vector2(0f, height);
			Image component = gameObject.GetComponent<Image>();
			component.color = c;
			component.raycastTarget = false;
			return rectTransform;
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0001B3BC File Offset: 0x000195BC
		private static Texture2D Draw(bool on)
		{
			BioWarning.<>c__DisplayClass27_0 CS$<>8__locals1 = new BioWarning.<>c__DisplayClass27_0();
			CS$<>8__locals1.px = new Color32[2058];
			Color32 color;
			color..ctor(14, 12, 6, 235);
			Color32 color2;
			color2..ctor(240, 196, 20, byte.MaxValue);
			Color32 color3 = (on ? new Color32(250, 206, 30, byte.MaxValue) : new Color32(120, 96, 16, byte.MaxValue));
			Color32 color4 = (on ? new Color32(220, 214, 190, byte.MaxValue) : new Color32(130, 126, 110, byte.MaxValue));
			for (int i = 0; i < CS$<>8__locals1.px.Length; i++)
			{
				CS$<>8__locals1.px[i] = color;
			}
			for (int j = 0; j < 98; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					if ((j + k) / 2 % 2 == 0)
					{
						CS$<>8__locals1.<Draw>g__Put|0(j, k, color2);
						CS$<>8__locals1.<Draw>g__Put|0(j, 20 - k, color2);
					}
				}
			}
			for (int l = 5; l <= 15; l++)
			{
				float num = (float)(l - 5) * 0.5f;
				for (int m = Mathf.RoundToInt(7f - num); m <= Mathf.RoundToInt(7f + num); m++)
				{
					CS$<>8__locals1.<Draw>g__Put|0(m, l, color3);
				}
			}
			for (int n = 8; n <= 11; n++)
			{
				CS$<>8__locals1.<Draw>g__Put|0(7, n, color);
			}
			CS$<>8__locals1.<Draw>g__Put|0(7, 13, color);
			BioWarning.Text("BIOLOGICAL WARNING", 15, 5, color3, new Action<int, int, Color32>(CS$<>8__locals1.<Draw>g__Put|0));
			BioWarning.Text("BIO PARASITE CONTACT", 15, 12, color4, new Action<int, int, Color32>(CS$<>8__locals1.<Draw>g__Put|0));
			Texture2D texture2D = new Texture2D(98, 21, 4, false);
			texture2D.filterMode = 0;
			texture2D.wrapMode = 1;
			texture2D.SetPixels32(CS$<>8__locals1.px);
			texture2D.Apply(false);
			return texture2D;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0001B5AC File Offset: 0x000197AC
		private static void Text(string s, int x, int y, Color32 c, Action<int, int, Color32> put)
		{
			foreach (char c2 in s)
			{
				string[] array;
				if (BioWarning.Font.TryGetValue(c2, out array))
				{
					for (int j = 0; j < array.Length; j++)
					{
						for (int k = 0; k < 3; k++)
						{
							if (array[j][k] == '1')
							{
								put(x + k, y + j, c);
							}
						}
					}
				}
				x += 4;
			}
		}

		// Token: 0x04000196 RID: 406
		private const int W = 98;

		// Token: 0x04000197 RID: 407
		private const int H = 21;

		// Token: 0x04000198 RID: 408
		private const float Seconds = 4.5f;

		// Token: 0x04000199 RID: 409
		private const float Slide = 0.25f;

		// Token: 0x0400019A RID: 410
		private const float Blink = 0.3f;

		// Token: 0x0400019B RID: 411
		private static GameObject root;

		// Token: 0x0400019C RID: 412
		private static GameObject barRoot;

		// Token: 0x0400019D RID: 413
		private static RectTransform rt;

		// Token: 0x0400019E RID: 414
		private static RectTransform barFill;

		// Token: 0x0400019F RID: 415
		private static RectTransform barTime;

		// Token: 0x040001A0 RID: 416
		private static RawImage img;

		// Token: 0x040001A1 RID: 417
		private static Texture2D texOn;

		// Token: 0x040001A2 RID: 418
		private static Texture2D texOff;

		// Token: 0x040001A3 RID: 419
		private static TextMeshProUGUI barKey;

		// Token: 0x040001A4 RID: 420
		private static float shownAt = -99f;

		// Token: 0x040001A5 RID: 421
		private static float k = 1f;

		// Token: 0x040001A6 RID: 422
		private static float barW;

		// Token: 0x040001A7 RID: 423
		private static bool barWanted;

		// Token: 0x040001A8 RID: 424
		private static float barValue;

		// Token: 0x040001A9 RID: 425
		private static float barLeft;

		// Token: 0x040001AA RID: 426
		private static readonly Dictionary<char, string[]> Font = new Dictionary<char, string[]>
		{
			{
				'A',
				new string[] { "111", "101", "111", "101", "101" }
			},
			{
				'B',
				new string[] { "110", "101", "110", "101", "110" }
			},
			{
				'C',
				new string[] { "111", "100", "100", "100", "111" }
			},
			{
				'E',
				new string[] { "111", "100", "110", "100", "111" }
			},
			{
				'G',
				new string[] { "111", "100", "101", "101", "111" }
			},
			{
				'I',
				new string[] { "111", "010", "010", "010", "111" }
			},
			{
				'L',
				new string[] { "100", "100", "100", "100", "111" }
			},
			{
				'N',
				new string[] { "101", "111", "111", "111", "101" }
			},
			{
				'O',
				new string[] { "111", "101", "101", "101", "111" }
			},
			{
				'P',
				new string[] { "111", "101", "111", "100", "100" }
			},
			{
				'R',
				new string[] { "110", "101", "110", "101", "101" }
			},
			{
				'S',
				new string[] { "111", "100", "111", "001", "111" }
			},
			{
				'T',
				new string[] { "111", "010", "010", "010", "010" }
			},
			{
				'W',
				new string[] { "101", "101", "111", "111", "101" }
			}
		};
	}
}
