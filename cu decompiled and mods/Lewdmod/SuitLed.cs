using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x020000CA RID: 202
	internal class SuitLed : MonoBehaviour
	{
		// Token: 0x06000473 RID: 1139 RVA: 0x00048668 File Offset: 0x00046868
		public static void Add(Transform item, Sprite torso, Color c)
		{
			float pixelsPerUnit = torso.pixelsPerUnit;
			Vector2 pivot = torso.pivot;
			float height = torso.rect.height;
			Vector2 vector = new Vector2(5.5f - pivot.x, height - 1f - 7f + 0.5f - pivot.y) / pixelsPerUnit;
			if (!SuitLed.dot)
			{
				Texture2D texture2D = new Texture2D(3, 1, 4, false)
				{
					filterMode = 0
				};
				for (int i = 0; i < 3; i++)
				{
					texture2D.SetPixel(i, 0, Color.white);
				}
				texture2D.Apply();
				SuitLed.dot = Sprite.Create(texture2D, new Rect(0f, 0f, 3f, 1f), new Vector2(0.5f, 0.5f), pixelsPerUnit);
				Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") ?? Shader.Find("Sprites/Default");
				if (shader)
				{
					SuitLed.unlit = new Material(shader);
				}
				Texture2D texture2D2 = new Texture2D(16, 16, 4, false)
				{
					filterMode = 1,
					wrapMode = 1
				};
				for (int j = 0; j < 16; j++)
				{
					for (int k = 0; k < 16; k++)
					{
						float num = Mathf.Clamp01(Vector2.Distance(new Vector2((float)k + 0.5f, (float)j + 0.5f), new Vector2(8f, 8f)) / 8f);
						texture2D2.SetPixel(k, j, new Color(1f, 1f, 1f, (1f - num) * (1f - num)));
					}
				}
				texture2D2.Apply();
				SuitLed.halo = Sprite.Create(texture2D2, new Rect(0f, 0f, 16f, 16f), new Vector2(0.5f, 0.5f), pixelsPerUnit * 16f / 7f);
			}
			GameObject gameObject = new GameObject("LewdSuitLed");
			gameObject.transform.SetParent(item, false);
			gameObject.transform.localPosition = vector;
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = SuitLed.dot;
			if (SuitLed.unlit)
			{
				spriteRenderer.sharedMaterial = SuitLed.unlit;
			}
			GameObject gameObject2 = new GameObject("LewdSuitLedGlow");
			gameObject2.transform.SetParent(gameObject.transform, false);
			SpriteRenderer spriteRenderer2 = gameObject2.AddComponent<SpriteRenderer>();
			spriteRenderer2.sprite = SuitLed.halo;
			if (SuitLed.unlit)
			{
				spriteRenderer2.sharedMaterial = SuitLed.unlit;
			}
			Light2D light2D = gameObject.AddComponent<Light2D>();
			light2D.lightType = 3;
			light2D.color = c;
			light2D.intensity = Plugin.SuitLight.Value * 3f;
			light2D.pointLightInnerRadius = 0f;
			light2D.pointLightOuterRadius = 0.35f;
			light2D.shadowIntensity = 0f;
			light2D.volumeIntensity = 0f;
			gameObject.AddComponent<SuitLed>().color = c;
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00048964 File Offset: 0x00046B64
		private void Awake()
		{
			this.led = base.GetComponent<SpriteRenderer>();
			this.light = base.GetComponent<Light2D>();
			this.body = (base.transform.parent ? base.transform.parent.GetComponent<SpriteRenderer>() : null);
			Transform transform = base.transform.Find("LewdSuitLedGlow");
			this.glow = (transform ? transform.GetComponent<SpriteRenderer>() : null);
			this.phase = Random.Range(0f, 6f);
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x000489F4 File Offset: 0x00046BF4
		private void LateUpdate()
		{
			if (!this.led)
			{
				return;
			}
			if (this.body)
			{
				this.led.sortingLayerID = this.body.sortingLayerID;
				this.led.sortingOrder = this.body.sortingOrder + 2;
				this.led.enabled = this.body.enabled;
				if (this.glow)
				{
					this.glow.sortingLayerID = this.body.sortingLayerID;
					this.glow.sortingOrder = this.body.sortingOrder + 1;
					this.glow.enabled = this.body.enabled;
				}
			}
			float num = 0.65f + 0.35f * Mathf.Sin(Time.time * 2.2f + this.phase);
			this.led.color = Color.Lerp(this.color * 0.6f, this.color, num) + new Color(0f, 0f, 0f, 1f);
			if (this.glow)
			{
				this.glow.color = new Color(this.color.r, this.color.g, this.color.b, Mathf.Clamp01(Plugin.SuitLight.Value * 1.6f) * num);
			}
			if (this.light)
			{
				this.light.intensity = Plugin.SuitLight.Value * 3f * (0.7f + 0.3f * num);
			}
		}

		// Token: 0x0400043A RID: 1082
		private const int PixelX = 4;

		// Token: 0x0400043B RID: 1083
		private const int PixelWidth = 3;

		// Token: 0x0400043C RID: 1084
		private const int PixelRowFromTop = 7;

		// Token: 0x0400043D RID: 1085
		private const float HaloPixels = 7f;

		// Token: 0x0400043E RID: 1086
		private static Sprite dot;

		// Token: 0x0400043F RID: 1087
		private static Sprite halo;

		// Token: 0x04000440 RID: 1088
		private static Material unlit;

		// Token: 0x04000441 RID: 1089
		private SpriteRenderer led;

		// Token: 0x04000442 RID: 1090
		private SpriteRenderer body;

		// Token: 0x04000443 RID: 1091
		private SpriteRenderer glow;

		// Token: 0x04000444 RID: 1092
		private Light2D light;

		// Token: 0x04000445 RID: 1093
		public Color color;

		// Token: 0x04000446 RID: 1094
		private float phase;
	}
}
