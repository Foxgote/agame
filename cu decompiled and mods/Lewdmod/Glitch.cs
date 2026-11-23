using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LewdStatus
{
	// Token: 0x0200009F RID: 159
	internal static class Glitch
	{
		// Token: 0x06000314 RID: 788 RVA: 0x00034C74 File Offset: 0x00032E74
		public static void Feather(Texture2D tex)
		{
			int width = tex.width;
			int height = tex.height;
			Color32[] pixels = tex.GetPixels32();
			Random random = new Random();
			int num = Mathf.Max(3, Mathf.Min(width, height) / 110);
			float num2 = (float)Mathf.Min(width, height) * 0.24f;
			int num3 = width / num + 1;
			int num4 = height / num + 1;
			float[] array = new float[num3 * num4];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (float)random.NextDouble();
			}
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < width; k++)
				{
					int num5 = k / num;
					int num6 = j / num;
					float num7 = (float)(num5 * num) + (float)num * 0.5f;
					float num8 = (float)(num6 * num) + (float)num * 0.5f;
					float num9 = Mathf.Min(Mathf.Min(num7, (float)width - num7), Mathf.Min(num8, (float)height - num8)) / num2;
					if (num9 < 1f)
					{
						float num10 = Mathf.SmoothStep(0f, 1f, num9);
						num10 = Mathf.Clamp01((num10 - array[num6 * num3 + num5] * 0.9f * (1f - num10)) / 0.8f);
						int num11 = j * width + k;
						Color32 color = pixels[num11];
						float num12 = 0.35f + 0.65f * num10;
						pixels[num11] = new Color32((byte)((float)color.r * num12), (byte)((float)color.g * num12), (byte)((float)color.b * num12), (byte)((float)color.a * num10));
					}
				}
			}
			tex.SetPixels32(pixels);
			tex.Apply();
		}

		// Token: 0x06000315 RID: 789 RVA: 0x00034E30 File Offset: 0x00033030
		public static void Damage(Texture2D tex)
		{
			int width = tex.width;
			int height = tex.height;
			Color32[] pixels = tex.GetPixels32();
			Color32[] array = (Color32[])pixels.Clone();
			Random random = new Random();
			int num = random.Next(9, 16);
			for (int i = 0; i < num; i++)
			{
				int num2 = random.Next(0, height);
				int num3 = random.Next(3, Mathf.Max(4, height / 28));
				int num4 = random.Next(-width / 14, width / 14);
				int num5 = ((random.NextDouble() < 0.5) ? random.Next(4, 12) : 0);
				for (int j = num2; j < Mathf.Min(height, num2 + num3); j++)
				{
					for (int k = 0; k < width; k++)
					{
						Color32 color = pixels[j * width + Glitch.Wrap(k - num4, width)];
						if (num5 > 0)
						{
							color.r = pixels[j * width + Glitch.Wrap(k - num4 - num5, width)].r;
							color.b = pixels[j * width + Glitch.Wrap(k - num4 + num5, width)].b;
						}
						array[j * width + k] = color;
					}
				}
			}
			int num6 = random.Next(4, 9);
			for (int l = 0; l < num6; l++)
			{
				int num7 = random.Next(width / 20, width / 6);
				int num8 = random.Next(height / 60, height / 18);
				int num9 = random.Next(0, width - num7);
				int num10 = random.Next(0, height - num8);
				int num11 = random.Next(0, width - num7);
				int num12 = random.Next(0, height - num8);
				bool flag = random.NextDouble() < 0.5;
				for (int m = 0; m < num8; m++)
				{
					for (int n = 0; n < num7; n++)
					{
						Color32 color2 = pixels[(num10 + m) * width + num9 + n];
						if (flag)
						{
							color2..ctor(color2.r / 2, color2.g, (byte)Mathf.Min(255, (int)(color2.b + 60)), byte.MaxValue);
						}
						array[(num12 + m) * width + num11 + n] = color2;
					}
				}
			}
			for (int num13 = 0; num13 < height; num13++)
			{
				float num14 = ((num13 % 3 == 0) ? 0.8f : 1f);
				float num15 = Mathf.Abs((float)num13 / (float)height - 0.5f) * 2f;
				for (int num16 = 0; num16 < width; num16++)
				{
					int num17 = num13 * width + num16;
					float num18 = Mathf.Abs((float)num16 / (float)width - 0.5f) * 2f;
					float num19 = 1f - 0.45f * Mathf.Clamp01(Mathf.Max(num18, num15) * 1.25f - 0.35f);
					float num20 = (float)(random.NextDouble() - 0.5) * 26f;
					Color32 color3 = array[num17];
					array[num17] = new Color32(Glitch.Byte(((float)color3.r * 0.92f + num20) * num14 * num19), Glitch.Byte(((float)color3.g * 0.96f + num20) * num14 * num19), Glitch.Byte(((float)color3.b * 1.04f + num20) * num14 * num19), byte.MaxValue);
				}
			}
			tex.SetPixels32(array);
			tex.Apply();
		}

		// Token: 0x06000316 RID: 790 RVA: 0x000351D1 File Offset: 0x000333D1
		private static int Wrap(int x, int w)
		{
			return (x % w + w) % w;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x000351DA File Offset: 0x000333DA
		private static byte Byte(float v)
		{
			return (byte)Mathf.Clamp(v, 0f, 255f);
		}

		// Token: 0x020001C0 RID: 448
		public class Jitter : MonoBehaviour
		{
			// Token: 0x060009A4 RID: 2468 RVA: 0x00079E7C File Offset: 0x0007807C
			public void Init(RawImage i)
			{
				this.img = i;
				this.rt = (RectTransform)i.transform;
				this.home = this.rt.anchoredPosition;
				this.next = Time.unscaledTime + Random.Range(2f, 4f);
			}

			// Token: 0x060009A5 RID: 2469 RVA: 0x00079ED0 File Offset: 0x000780D0
			private void Update()
			{
				if (!this.img)
				{
					return;
				}
				float unscaledTime = Time.unscaledTime;
				if (unscaledTime >= this.next)
				{
					this.until = unscaledTime + Random.Range(0.05f, 0.18f);
					this.next = unscaledTime + Random.Range(0.6f, 3.5f);
					this.rt.anchoredPosition = this.home + new Vector2(Random.Range(-14f, 14f), Random.Range(-3f, 3f));
					this.img.uvRect = new Rect(Random.Range(-0.02f, 0.02f), 0f, 1f, 1f);
					int num = Random.Range(0, 3);
					this.img.color = ((num == 0) ? new Color(1f, 0.75f, 0.8f) : ((num == 1) ? new Color(0.75f, 0.95f, 1f) : new Color(1f, 1f, 1f, 0.8f)));
					return;
				}
				if (unscaledTime >= this.until)
				{
					this.rt.anchoredPosition = this.home;
					this.img.uvRect = new Rect(0f, 0f, 1f, 1f);
					this.img.color = Color.white;
				}
			}

			// Token: 0x04000997 RID: 2455
			private RawImage img;

			// Token: 0x04000998 RID: 2456
			private RectTransform rt;

			// Token: 0x04000999 RID: 2457
			private Vector2 home;

			// Token: 0x0400099A RID: 2458
			private float next;

			// Token: 0x0400099B RID: 2459
			private float until;
		}

		// Token: 0x020001C1 RID: 449
		public class TextFlicker : MonoBehaviour
		{
			// Token: 0x060009A7 RID: 2471 RVA: 0x0007A048 File Offset: 0x00078248
			public void Init(TextMeshProUGUI t)
			{
				this.text = t;
				this.baseColor = t.color;
			}

			// Token: 0x060009A8 RID: 2472 RVA: 0x0007A060 File Offset: 0x00078260
			private void Update()
			{
				if (!this.text)
				{
					return;
				}
				float unscaledTime = Time.unscaledTime;
				if (unscaledTime >= this.next)
				{
					this.until = unscaledTime + Random.Range(0.04f, 0.12f);
					this.next = unscaledTime + Random.Range(0.4f, 2.5f);
					this.text.color = new Color(this.baseColor.r, this.baseColor.g, this.baseColor.b, Random.Range(0.25f, 0.7f));
					this.text.characterSpacing = Random.Range(-4f, 6f);
					return;
				}
				if (unscaledTime >= this.until)
				{
					this.text.color = this.baseColor;
					this.text.characterSpacing = 0f;
				}
			}

			// Token: 0x0400099C RID: 2460
			private TextMeshProUGUI text;

			// Token: 0x0400099D RID: 2461
			private float next;

			// Token: 0x0400099E RID: 2462
			private float until;

			// Token: 0x0400099F RID: 2463
			private Color baseColor;
		}
	}
}
