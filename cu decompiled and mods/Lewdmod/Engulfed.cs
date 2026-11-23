using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace LewdStatus
{
	// Token: 0x020000D9 RID: 217
	internal static class Engulfed
	{
		// Token: 0x060004ED RID: 1261 RVA: 0x00050030 File Offset: 0x0004E230
		public static void Tick(Body b)
		{
			bool flag = b && b.alive;
			if (flag && Pits.InMaw)
			{
				Engulfed.Show((Pits.Holder.Kind == PitKind.Flower) ? Engulfed.FlowerBeat : Engulfed.MawBeat, Pits.Hard, default(Vector2), default(Vector2));
			}
			else if (flag && Worm.Inside)
			{
				Engulfed.Show(Engulfed.WormBeat, Worm.Sealed || !Worm.Tentacled, default(Vector2), default(Vector2));
			}
			else if (flag && Slits.Inside)
			{
				Engulfed.Show(Engulfed.SlitBeat, Slits.Hard, Slits.ViewCenter, Slits.ViewHalf);
			}
			else if (flag && (Latexes.Inside || LatexPond.Inside))
			{
				Engulfed.Show(Engulfed.LatexBeat, true, default(Vector2), default(Vector2));
				Engulfed.Backdrop(b);
			}
			else if (Engulfed.cam)
			{
				Engulfed.Hide();
			}
			if (Engulfed.backdrop && (!flag || (!Latexes.Inside && !LatexPond.Inside)))
			{
				Engulfed.DropBackdrop();
			}
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0005015C File Offset: 0x0004E35C
		private static void Backdrop(Body b)
		{
			if (!b)
			{
				return;
			}
			List<SpriteRenderer> list = (from r in b.GetComponentsInChildren<SpriteRenderer>(false)
				where r && r.enabled && r.gameObject.name != "LewdBioWarning"
				select r).ToList<SpriteRenderer>();
			if (list.Count == 0)
			{
				return;
			}
			SpriteRenderer spriteRenderer = ((b.limbs.Length > 1) ? b.limbs[1].GetComponent<SpriteRenderer>() : list[0]);
			if (!Engulfed.backdrop)
			{
				Engulfed.backdropTex = new Texture2D(64, 64, 4, false)
				{
					filterMode = 0,
					wrapMode = 0
				};
				Color32[] array = new Color32[4096];
				Random random = new Random(8);
				for (int i = 0; i < 64; i++)
				{
					for (int j = 0; j < 64; j++)
					{
						float num = Mathf.Pow(Mathf.Max(0f, Mathf.Sin(((float)j * 0.11f + (float)i * 0.05f) * 3.1415927f)), 18f);
						float num2 = (float)random.NextDouble() * 0.04f;
						float num3 = 0.03f + num2 + num * 0.1f;
						array[i * 64 + j] = new Color(num3 * 0.8f, num3 * 0.6f, num3 * 1.4f, 0.97f);
					}
				}
				Engulfed.backdropTex.SetPixels32(array);
				Engulfed.backdropTex.Apply(false);
				Engulfed.backdrop = new GameObject("LewdLatexBackdrop", new Type[] { typeof(SpriteRenderer) }).GetComponent<SpriteRenderer>();
				Engulfed.backdrop.sprite = Sprite.Create(Engulfed.backdropTex, new Rect(0f, 0f, 64f, 64f), new Vector2(0.5f, 0.5f), 4f);
				if (WorldGeneration.world)
				{
					Engulfed.backdrop.sharedMaterial = WorldGeneration.world.defaultMat;
				}
			}
			Engulfed.backdrop.sortingLayerID = (spriteRenderer ? spriteRenderer.sortingLayerID : list[0].sortingLayerID);
			Engulfed.backdrop.sortingOrder = (from r in list
				where r.sortingLayerID == Engulfed.backdrop.sortingLayerID
				select r.sortingOrder).DefaultIfEmpty(0).Min() - 2;
			Vector2 vector = (spriteRenderer ? spriteRenderer.transform.position : b.transform.position);
			float num4 = Mathf.Sin(Time.time * 0.4f) * 0.35f;
			Engulfed.backdrop.transform.position = new Vector3(vector.x + num4, vector.y + num4 * 0.5f, b.transform.position.z + 0.5f);
			Engulfed.backdrop.transform.localScale = Vector3.one * 1.6f;
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x0005048F File Offset: 0x0004E68F
		private static void DropBackdrop()
		{
			if (Engulfed.backdrop)
			{
				Object.Destroy(Engulfed.backdrop.gameObject);
			}
			if (Engulfed.backdropTex)
			{
				Object.Destroy(Engulfed.backdropTex);
			}
			Engulfed.backdrop = null;
			Engulfed.backdropTex = null;
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060004F0 RID: 1264 RVA: 0x000504CE File Offset: 0x0004E6CE
		public static bool Active
		{
			get
			{
				return Engulfed.cam;
			}
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x000504DC File Offset: 0x0004E6DC
		private static void Show(Color beatColour, bool hard, Vector2 center = default(Vector2), Vector2 half = default(Vector2))
		{
			if (!Engulfed.cam && !Engulfed.Enter())
			{
				return;
			}
			Engulfed.Keep();
			if (!Engulfed.ring || Engulfed.ringW != Screen.width || Engulfed.ringH != Screen.height || Mathf.Abs(Engulfed.cam.orthographicSize - Engulfed.ringFor) > 0.001f || half != Engulfed.rectHalf)
			{
				Engulfed.rectHalf = half;
				Engulfed.MakeRing();
			}
			if (Engulfed.ring && Engulfed.rectHalf != Vector2.zero)
			{
				Engulfed.ring.transform.position = new Vector3(center.x, center.y, Engulfed.ring.transform.position.z);
			}
			if (Engulfed.ringSr)
			{
				float num = Mathf.Pow(Mathf.Max(0f, Mathf.Sin(Time.time * (hard ? 5.5f : 3.2f))), 3f) * (hard ? 0.9f : 0.35f);
				Engulfed.ringSr.color = Color.Lerp(Engulfed.Rest, beatColour, num);
			}
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x0005060C File Offset: 0x0004E80C
		public static void Hide()
		{
			if (Engulfed.hooked)
			{
				Action<ScriptableRenderContext, Camera> action;
				if ((action = Engulfed.<>O.<0>__BeforeRender) == null)
				{
					action = (Engulfed.<>O.<0>__BeforeRender = new Action<ScriptableRenderContext, Camera>(Engulfed.BeforeRender));
				}
				RenderPipelineManager.beginCameraRendering -= action;
			}
			Engulfed.hooked = false;
			if (Engulfed.cam && Mathf.Abs(Engulfed.cam.orthographicSize - Engulfed.ourSize) < 0.001f)
			{
				Engulfed.cam.orthographicSize = Engulfed.baseSize;
			}
			if (Engulfed.limiter && (Engulfed.limiter.localScale - Engulfed.ourLimit).sqrMagnitude < 1E-06f)
			{
				Engulfed.limiter.localScale = Engulfed.baseLimit;
			}
			if (Engulfed.ring)
			{
				Object.Destroy(Engulfed.ring);
			}
			if (Engulfed.ringTex)
			{
				Object.Destroy(Engulfed.ringTex);
			}
			Engulfed.ring = null;
			Engulfed.ringSr = null;
			Engulfed.ringTex = null;
			Engulfed.cam = null;
			Engulfed.limiter = null;
			Engulfed.rectHalf = Vector2.zero;
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x00050710 File Offset: 0x0004E910
		private static bool Enter()
		{
			Camera camera = (PlayerCamera.main ? PlayerCamera.main.GetComponent<Camera>() : Camera.main);
			if (!camera || !camera.orthographic)
			{
				return false;
			}
			Engulfed.cam = camera;
			Engulfed.baseSize = camera.orthographicSize;
			Engulfed.ourSize = Engulfed.baseSize * 0.6f;
			Engulfed.limiter = camera.transform.Find("SightLimiter");
			if (Engulfed.limiter)
			{
				Engulfed.baseLimit = Engulfed.limiter.localScale;
				Engulfed.ourLimit = new Vector3(Engulfed.baseLimit.x * 0.6f, Engulfed.baseLimit.y * 0.6f, Engulfed.baseLimit.z);
			}
			Action<ScriptableRenderContext, Camera> action;
			if ((action = Engulfed.<>O.<0>__BeforeRender) == null)
			{
				action = (Engulfed.<>O.<0>__BeforeRender = new Action<ScriptableRenderContext, Camera>(Engulfed.BeforeRender));
			}
			RenderPipelineManager.beginCameraRendering += action;
			Engulfed.hooked = true;
			Engulfed.Keep();
			Plugin.Log.LogInfo("Swallowed: the view closes in round her");
			return true;
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x0005080D File Offset: 0x0004EA0D
		private static void BeforeRender(ScriptableRenderContext ctx, Camera c)
		{
			if (c == Engulfed.cam)
			{
				Engulfed.Keep();
			}
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x00050821 File Offset: 0x0004EA21
		private static void Keep()
		{
			if (!Engulfed.cam)
			{
				return;
			}
			Engulfed.cam.orthographicSize = Engulfed.ourSize;
			if (Engulfed.limiter)
			{
				Engulfed.limiter.localScale = Engulfed.ourLimit;
			}
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x0005085C File Offset: 0x0004EA5C
		private static void MakeRing()
		{
			if (Engulfed.ring)
			{
				Object.Destroy(Engulfed.ring);
			}
			if (Engulfed.ringTex)
			{
				Object.Destroy(Engulfed.ringTex);
			}
			Engulfed.ringW = Screen.width;
			Engulfed.ringH = Screen.height;
			Engulfed.ringFor = Engulfed.cam.orthographicSize;
			bool flag = Engulfed.rectHalf != Vector2.zero;
			int num = (flag ? 320 : 160);
			int num2 = Mathf.Max(1, Mathf.RoundToInt((float)num * Engulfed.cam.aspect));
			float num3 = Engulfed.cam.orthographicSize * 2f * (flag ? 2f : 1f) / (float)num;
			Color32[] array = new Color32[num2 * num];
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					float num4 = ((float)j + 0.5f - (float)num2 * 0.5f) * num3;
					float num5 = ((float)i + 0.5f - (float)num * 0.5f) * num3;
					float num6 = (flag ? Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(-0.1f, 0.55f, Mathf.Max(Mathf.Abs(num4) - Engulfed.rectHalf.x, Mathf.Abs(num5) - Engulfed.rectHalf.y))) : Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(2.3f, 5.2f, Mathf.Sqrt(num4 * num4 + num5 * num5))));
					array[i * num2 + j] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Mathf.RoundToInt(num6 * 255f));
				}
			}
			Engulfed.ringTex = new Texture2D(num2, num, 4, false)
			{
				filterMode = 1,
				wrapMode = 1
			};
			Engulfed.ringTex.SetPixels32(array);
			Engulfed.ringTex.Apply(false, true);
			Engulfed.ring = new GameObject("LewdMawVignette");
			Engulfed.ring.transform.SetParent(Engulfed.cam.transform, false);
			Engulfed.ring.transform.localPosition = new Vector3(0f, 0f, 1f);
			Engulfed.ringSr = Engulfed.ring.AddComponent<SpriteRenderer>();
			Engulfed.ringSr.sprite = Sprite.Create(Engulfed.ringTex, new Rect(0f, 0f, (float)num2, (float)num), new Vector2(0.5f, 0.5f), 1f / num3);
			SortingLayer[] layers = SortingLayer.layers;
			if (layers.Length != 0)
			{
				Engulfed.ringSr.sortingLayerID = layers[layers.Length - 1].id;
			}
			Engulfed.ringSr.sortingOrder = 32759;
			Engulfed.ringSr.color = Engulfed.Rest;
		}

		// Token: 0x040004B6 RID: 1206
		private const float Zoom = 0.6f;

		// Token: 0x040004B7 RID: 1207
		private const float Clear = 2.3f;

		// Token: 0x040004B8 RID: 1208
		private const float Dark = 5.2f;

		// Token: 0x040004B9 RID: 1209
		private static readonly Color Rest = new Color(0.03f, 0.006f, 0.008f, 1f);

		// Token: 0x040004BA RID: 1210
		private static readonly Color MawBeat = new Color(0.38f, 0.03f, 0.04f, 1f);

		// Token: 0x040004BB RID: 1211
		private static readonly Color WormBeat = new Color(0.34f, 0.12f, 0.08f, 1f);

		// Token: 0x040004BC RID: 1212
		private static readonly Color FlowerBeat = new Color(0.1f, 0.26f, 0.08f, 1f);

		// Token: 0x040004BD RID: 1213
		private static readonly Color SlitBeat = new Color(0.42f, 0.08f, 0.2f, 1f);

		// Token: 0x040004BE RID: 1214
		private static readonly Color LatexBeat = new Color(0.22f, 0.06f, 0.38f, 1f);

		// Token: 0x040004BF RID: 1215
		private static SpriteRenderer backdrop;

		// Token: 0x040004C0 RID: 1216
		private static Texture2D backdropTex;

		// Token: 0x040004C1 RID: 1217
		private static Camera cam;

		// Token: 0x040004C2 RID: 1218
		private static Transform limiter;

		// Token: 0x040004C3 RID: 1219
		private static float baseSize;

		// Token: 0x040004C4 RID: 1220
		private static float ourSize;

		// Token: 0x040004C5 RID: 1221
		private static Vector3 baseLimit;

		// Token: 0x040004C6 RID: 1222
		private static Vector3 ourLimit;

		// Token: 0x040004C7 RID: 1223
		private static GameObject ring;

		// Token: 0x040004C8 RID: 1224
		private static SpriteRenderer ringSr;

		// Token: 0x040004C9 RID: 1225
		private static Texture2D ringTex;

		// Token: 0x040004CA RID: 1226
		private static float ringFor;

		// Token: 0x040004CB RID: 1227
		private static int ringW;

		// Token: 0x040004CC RID: 1228
		private static int ringH;

		// Token: 0x040004CD RID: 1229
		private static bool hooked;

		// Token: 0x040004CE RID: 1230
		private static Vector2 rectHalf;

		// Token: 0x02000207 RID: 519
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000A68 RID: 2664
			public static Action<ScriptableRenderContext, Camera> <0>__BeforeRender;
		}
	}
}
