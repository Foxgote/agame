using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LewdStatus
{
	// Token: 0x020000B0 RID: 176
	internal static class LewdHealthPanel
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060003F3 RID: 1011 RVA: 0x000408A7 File Offset: 0x0003EAA7
		private static bool Violet
		{
			get
			{
				return Suit.Latex;
			}
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x000408B0 File Offset: 0x0003EAB0
		private static Texture2D Tint(Texture2D t)
		{
			if (!t || !LewdHealthPanel.Violet)
			{
				return t;
			}
			Texture2D texture2D;
			if (LewdHealthPanel.violet.TryGetValue(t, out texture2D) && texture2D)
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
				Color color = pixels[i];
				float num;
				float num2;
				float num3;
				Color.RGBToHSV(color, ref num, ref num2, ref num3);
				if (num2 > 0.15f)
				{
					num = (num + 0.3472222f) % 1f;
				}
				Color color2 = Color.HSVToRGB(num, num2, num3);
				color2.a = color.a;
				pixels[i] = color2;
			}
			texture2D.SetPixels32(pixels);
			texture2D.Apply(false);
			return LewdHealthPanel.violet[t] = texture2D;
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x000409A8 File Offset: 0x0003EBA8
		private static void Recolour()
		{
			LewdHealthPanel.shownViolet = LewdHealthPanel.Violet;
			if (LewdHealthPanel.image && LewdHealthPanel.baseTex)
			{
				LewdHealthPanel.image.texture = LewdHealthPanel.Tint(LewdHealthPanel.baseTex);
			}
			for (int i = 0; i < 4; i++)
			{
				LewdHealthPanel.shownPic[i] = null;
			}
			foreach (TextMeshProUGUI textMeshProUGUI in LewdHealthPanel.titles)
			{
				if (textMeshProUGUI)
				{
					textMeshProUGUI.color = (LewdHealthPanel.shownViolet ? LewdHealthPanel.VioletText : LewdHealthPanel.Green);
				}
			}
			foreach (TextMeshProUGUI textMeshProUGUI2 in LewdHealthPanel.lines)
			{
				if (textMeshProUGUI2)
				{
					textMeshProUGUI2.color = (LewdHealthPanel.shownViolet ? LewdHealthPanel.VioletSoft : LewdHealthPanel.Soft);
				}
			}
			if (LewdHealthPanel.arrow)
			{
				LewdHealthPanel.arrow.color = (LewdHealthPanel.shownViolet ? LewdHealthPanel.VioletText : LewdHealthPanel.Green);
			}
			if (LewdHealthPanel.plugBase && LewdHealthPanel.plugBase.texture)
			{
				LewdHealthPanel.plugBase.texture = LewdHealthPanel.Tint(LewdHealthPanel.Pic("plug_base"));
			}
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00040AD3 File Offset: 0x0003ECD3
		public static void Bind(ConfigFile c)
		{
			LewdHealthPanel.Collapsed = c.Bind<bool>("Body", "HealthBlockCollapsed", false, "The EROS-SCAN block on the health screen is folded up to its title bar (click the title to open or fold it).");
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00040AF0 File Offset: 0x0003ECF0
		private static void Layout()
		{
			if (!LewdHealthPanel.root)
			{
				return;
			}
			bool flag = LewdHealthPanel.Collapsed != null && LewdHealthPanel.Collapsed.Value;
			RectTransform rectTransform = (RectTransform)LewdHealthPanel.root.transform;
			float num = (float)(flag ? 12 : 171);
			rectTransform.sizeDelta = new Vector2(130f * LewdHealthPanel.scale, num * LewdHealthPanel.scale);
			LewdHealthPanel.image.uvRect = new Rect(0f, 1f - num / 171f, 1f, num / 171f);
			for (int i = 0; i < 4; i++)
			{
				if (LewdHealthPanel.titles[i])
				{
					LewdHealthPanel.titles[i].gameObject.SetActive(!flag);
				}
				if (LewdHealthPanel.lines[i])
				{
					LewdHealthPanel.lines[i].gameObject.SetActive(!flag);
				}
				if (LewdHealthPanel.windows[i])
				{
					LewdHealthPanel.windows[i].gameObject.SetActive(!flag);
				}
			}
			if (LewdHealthPanel.arrow)
			{
				LewdHealthPanel.arrow.text = (flag ? "+" : "-");
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060003F8 RID: 1016 RVA: 0x00040C1F File Offset: 0x0003EE1F
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00040C26 File Offset: 0x0003EE26
		public static void Load(string folder)
		{
			LewdHealthPanel.dir = folder;
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00040C30 File Offset: 0x0003EE30
		private static Texture2D Pic(string name)
		{
			Texture2D texture2D;
			if (LewdHealthPanel.pics.TryGetValue(name, out texture2D))
			{
				return texture2D;
			}
			string text = Path.Combine(LewdHealthPanel.dir ?? "", name + ".png");
			if (File.Exists(text))
			{
				texture2D = new Texture2D(2, 2, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text));
			}
			return LewdHealthPanel.pics[name] = texture2D;
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00040CAC File Offset: 0x0003EEAC
		private static Texture2D Tex(int bust)
		{
			Texture2D texture2D = LewdHealthPanel.Pic("block_empty");
			LewdHealthPanel.layered = texture2D;
			if (texture2D)
			{
				return texture2D;
			}
			bust = Mathf.Clamp(bust, 0, 2);
			if (LewdHealthPanel.texs[bust])
			{
				return LewdHealthPanel.texs[bust];
			}
			string text = Path.Combine(LewdHealthPanel.dir ?? "", string.Format("block_{0}.png", bust));
			if (!File.Exists(text))
			{
				text = Path.Combine(LewdHealthPanel.dir ?? "", "block.png");
			}
			if (!File.Exists(text))
			{
				return null;
			}
			Texture2D texture2D2 = new Texture2D(2, 2, 4, false)
			{
				filterMode = 0,
				wrapMode = 1
			};
			ImageConversion.LoadImage(texture2D2, File.ReadAllBytes(text));
			return LewdHealthPanel.texs[bust] = texture2D2;
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00040D78 File Offset: 0x0003EF78
		public static void Update(Body b)
		{
			PlayerCamera main = PlayerCamera.main;
			if (!main || !main.woundView || !b)
			{
				return;
			}
			if (!main.woundView.activeSelf || !LewdSettings.On || LewdHealthPanel.S == null)
			{
				if (LewdHealthPanel.root)
				{
					LewdHealthPanel.root.SetActive(false);
				}
				return;
			}
			if (!LewdHealthPanel.root && !LewdHealthPanel.Build(main))
			{
				return;
			}
			if (!LewdHealthPanel.root.activeSelf)
			{
				LewdHealthPanel.root.SetActive(true);
				LewdHealthPanel.root.transform.SetAsLastSibling();
				LewdHealthPanel.refresh = 0f;
			}
			if (!LewdHealthPanel.layered && Mathf.Clamp(LewdHealthPanel.S.bust, 0, 2) != LewdHealthPanel.shownBust && LewdHealthPanel.Tex(LewdHealthPanel.S.bust))
			{
				LewdHealthPanel.shownBust = Mathf.Clamp(LewdHealthPanel.S.bust, 0, 2);
				LewdHealthPanel.baseTex = LewdHealthPanel.Tex(LewdHealthPanel.shownBust);
				LewdHealthPanel.image.texture = LewdHealthPanel.Tint(LewdHealthPanel.baseTex);
			}
			if (LewdHealthPanel.Violet != LewdHealthPanel.shownViolet)
			{
				LewdHealthPanel.Recolour();
			}
			LewdHealthPanel.Animate();
			if ((LewdHealthPanel.refresh -= Time.unscaledDeltaTime) > 0f)
			{
				return;
			}
			LewdHealthPanel.refresh = 0.25f;
			LewdHealthPanel.Fill(b);
			if (LewdHealthPanel.layered)
			{
				LewdHealthPanel.PickPictures();
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060003FD RID: 1021 RVA: 0x00040EE0 File Offset: 0x0003F0E0
		private static bool Used
		{
			get
			{
				return Flowers.Holding || Tentacles.Holding || Worm.Sex || Flesh.Holding || FleshWall.Holding || Pits.InMaw || Slits.Inside || External.ScavHolding;
			}
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x00040F1C File Offset: 0x0003F11C
		private static void PickPictures()
		{
			int num = Mathf.Clamp(LewdHealthPanel.S.bust, 0, 2);
			LewdHealthPanel.Show(0, Suit.Milking ? string.Format("anim/breast_milk_{0}", num) : string.Format("breast_{0}", num));
			LewdHealthPanel.Show(1, (LewdHealthPanel.S.filled > 3f) ? string.Format("anim/womb_fill_{0}", Mathf.Clamp(Mathf.CeilToInt(LewdHealthPanel.S.filled / 20f), 1, 5)) : "womb");
			LewdHealthPanel.Show(2, LewdHealthPanel.S.inHeat ? "anim/vulva_heat" : ((LewdHealthPanel.S.arousal >= 50f) ? "anim/vulva_wet" : "vulva"));
			LewdHealthPanel.Show(3, LewdHealthPanel.Used ? "anim/butt_jiggle" : ((LewdHealthPanel.S.cum > 35f) ? "anim/butt_leak" : ((LewdHealthPanel.S.arousal >= 30f) ? "anim/butt_idle" : "butt")));
			ToyDef toyDef = (PlayerCamera.main ? Plugs.Worn(PlayerCamera.main.body) : null);
			if (LewdHealthPanel.plugBase)
			{
				LewdHealthPanel.plugBase.enabled = toyDef != null && LewdHealthPanel.plugBase.texture;
				if (toyDef != null)
				{
					LewdHealthPanel.plugBase.color = toyDef.Tint;
					Texture2D texture2D = LewdHealthPanel.Pic((toyDef.Kind == "tailplug") ? "plug_tail" : "plug_base");
					if (texture2D && LewdHealthPanel.plugBase.texture != LewdHealthPanel.Tint(texture2D))
					{
						LewdHealthPanel.plugBase.texture = LewdHealthPanel.Tint(texture2D);
					}
				}
			}
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x000410EC File Offset: 0x0003F2EC
		private static void Show(int i, string name)
		{
			if (!LewdHealthPanel.windows[i] || LewdHealthPanel.shownPic[i] == name)
			{
				return;
			}
			Texture2D texture2D = LewdHealthPanel.Pic(name);
			if (!texture2D && name.StartsWith("anim/"))
			{
				name = (new string[]
				{
					string.Format("breast_{0}", Mathf.Clamp(LewdHealthPanel.S.bust, 0, 2)),
					"womb",
					"vulva",
					"butt"
				})[i];
				texture2D = LewdHealthPanel.Pic(name);
			}
			LewdHealthPanel.shownPic[i] = name;
			LewdHealthPanel.animated[i] = texture2D && texture2D.width >= 624;
			LewdHealthPanel.windows[i].texture = LewdHealthPanel.Tint(texture2D);
			LewdHealthPanel.windows[i].enabled = texture2D;
			LewdHealthPanel.windows[i].uvRect = new Rect(0f, 0f, LewdHealthPanel.animated[i] ? 0.083333336f : 1f, 1f);
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x00041204 File Offset: 0x0003F404
		private static void Animate()
		{
			int num = (int)(Time.unscaledTime * 9f) % 12;
			float num2 = Mathf.Lerp(0.5f, 2f, Mathf.InverseLerp(30f, 100f, LewdHealthPanel.S.arousal));
			LewdHealthPanel.buttPhase = (LewdHealthPanel.buttPhase + Time.unscaledDeltaTime * 9f * num2) % 12f;
			for (int i = 0; i < 4; i++)
			{
				if (LewdHealthPanel.animated[i] && LewdHealthPanel.windows[i])
				{
					int num3 = ((i == 3 && LewdHealthPanel.shownPic[3] == "anim/butt_idle") ? ((int)LewdHealthPanel.buttPhase) : num);
					LewdHealthPanel.windows[i].uvRect = new Rect((float)num3 / 12f, 0f, 0.083333336f, 1f);
				}
			}
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x000412D4 File Offset: 0x0003F4D4
		private static bool Build(PlayerCamera pc)
		{
			Texture2D texture2D = LewdHealthPanel.Tex(LewdHealthPanel.S.bust);
			if (!texture2D)
			{
				return false;
			}
			Canvas componentInParent = pc.woundView.GetComponentInParent<Canvas>();
			if (!componentInParent)
			{
				return false;
			}
			LewdHealthPanel.shownBust = Mathf.Clamp(LewdHealthPanel.S.bust, 0, 2);
			RectTransform rectTransform = (RectTransform)componentInParent.transform;
			LewdHealthPanel.root = new GameObject("LewdHealthPanel", new Type[]
			{
				typeof(RectTransform),
				typeof(RawImage)
			});
			LewdHealthPanel.root.transform.SetParent(componentInParent.transform, false);
			RectTransform rectTransform2 = (RectTransform)LewdHealthPanel.root.transform;
			RectTransform rectTransform3 = rectTransform2;
			RectTransform rectTransform4 = rectTransform2;
			Vector2 vector;
			vector..ctor(1f, 1f);
			rectTransform4.anchorMax = vector;
			rectTransform3.anchorMin = vector;
			rectTransform2.pivot = new Vector2(0.5f, 1f);
			float num = rectTransform.rect.height * 0.4f / 171f;
			LewdHealthPanel.scale = num;
			rectTransform2.sizeDelta = new Vector2(130f * num, 171f * num);
			rectTransform2.anchoredPosition = new Vector2(-rectTransform.rect.width * 0.1133f, -rectTransform.rect.height * 0.272f);
			LewdHealthPanel.image = LewdHealthPanel.root.GetComponent<RawImage>();
			LewdHealthPanel.baseTex = texture2D;
			LewdHealthPanel.image.texture = LewdHealthPanel.Tint(texture2D);
			LewdHealthPanel.shownViolet = !LewdHealthPanel.Violet;
			LewdHealthPanel.image.raycastTarget = false;
			GameObject gameObject = new GameObject("LewdHealthHeader", new Type[]
			{
				typeof(RectTransform),
				typeof(Image),
				typeof(Button)
			});
			gameObject.transform.SetParent(rectTransform2, false);
			RectTransform rectTransform5 = (RectTransform)gameObject.transform;
			Vector2 vector2;
			vector2..ctor(0f, 1f);
			rectTransform5.pivot = vector2;
			vector = (rectTransform5.anchorMax = vector2);
			rectTransform5.anchorMin = vector;
			rectTransform5.anchoredPosition = Vector2.zero;
			rectTransform5.sizeDelta = new Vector2(130f * num, 13f * num);
			gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				LewdHealthPanel.Collapsed.Value = !LewdHealthPanel.Collapsed.Value;
				LewdHealthPanel.Layout();
			});
			TMP_FontAsset tmp_FontAsset;
			if ((tmp_FontAsset = (from t in pc.woundView.GetComponentsInChildren<TextMeshProUGUI>(true)
				select t.font).FirstOrDefault<TMP_FontAsset>((TMP_FontAsset f) => f)) == null)
			{
				if (!pc.deathStats)
				{
					tmp_FontAsset = null;
				}
				else
				{
					TextMeshProUGUI componentInChildren = pc.deathStats.GetComponentInChildren<TextMeshProUGUI>(true);
					tmp_FontAsset = ((componentInChildren != null) ? componentInChildren.font : null);
				}
			}
			TMP_FontAsset tmp_FontAsset2 = tmp_FontAsset;
			for (int i = 0; i < 4; i++)
			{
				int num2 = 3 + i % 2 * 64;
				int num3 = 13 + i / 2 * 79;
				if (LewdHealthPanel.layered)
				{
					GameObject gameObject2 = new GameObject("LewdHealthWindow", new Type[]
					{
						typeof(RectTransform),
						typeof(RawImage)
					});
					gameObject2.transform.SetParent(rectTransform2, false);
					RectTransform rectTransform6 = (RectTransform)gameObject2.transform;
					vector2..ctor(0f, 1f);
					rectTransform6.pivot = vector2;
					vector = (rectTransform6.anchorMax = vector2);
					rectTransform6.anchorMin = vector;
					rectTransform6.anchoredPosition = new Vector2((float)(num2 + 4) * num, (float)(-(float)(num3 + 11)) * num);
					rectTransform6.sizeDelta = new Vector2(52f * num, 42f * num);
					LewdHealthPanel.windows[i] = gameObject2.GetComponent<RawImage>();
					LewdHealthPanel.windows[i].raycastTarget = false;
					LewdHealthPanel.shownPic[i] = null;
					if (i == 3)
					{
						GameObject gameObject3 = new GameObject("LewdPlugBase", new Type[]
						{
							typeof(RectTransform),
							typeof(RawImage)
						});
						gameObject3.transform.SetParent(gameObject2.transform, false);
						RectTransform rectTransform7 = (RectTransform)gameObject3.transform;
						rectTransform7.anchorMin = Vector2.zero;
						rectTransform7.anchorMax = Vector2.one;
						vector = (rectTransform7.offsetMax = Vector2.zero);
						rectTransform7.offsetMin = vector;
						LewdHealthPanel.plugBase = gameObject3.GetComponent<RawImage>();
						LewdHealthPanel.plugBase.raycastTarget = false;
						LewdHealthPanel.plugBase.texture = LewdHealthPanel.Pic("plug_base");
						LewdHealthPanel.plugBase.enabled = false;
					}
				}
				LewdHealthPanel.titles[i] = LewdHealthPanel.Text(rectTransform2, tmp_FontAsset2, num, num2 + 1, num3 + 1, 58, 8, 6.5f, 514, LewdHealthPanel.Green);
				LewdHealthPanel.lines[i] = LewdHealthPanel.Text(rectTransform2, tmp_FontAsset2, num, num2 + 3, num3 + 10 + 42 + 3, 54, 19, 5.2f, 4097, LewdHealthPanel.Soft);
			}
			LewdHealthPanel.arrow = LewdHealthPanel.Text(rectTransform2, tmp_FontAsset2, num, 118, 2, 8, 9, 7f, 514, LewdHealthPanel.Green);
			if (LewdHealthPanel.layered)
			{
				LewdHealthPanel.PickPictures();
			}
			gameObject.transform.SetAsLastSibling();
			LewdHealthPanel.root.transform.SetAsLastSibling();
			LewdHealthPanel.Layout();
			Plugin.Log.LogInfo(string.Format("Health screen: the lewd block is up ({0:0}x{1:0} on a {2:0}x{3:0} canvas)", new object[]
			{
				130f * num,
				171f * num,
				rectTransform.rect.width,
				rectTransform.rect.height
			}));
			return true;
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x000418BC File Offset: 0x0003FABC
		private static TextMeshProUGUI Text(RectTransform parent, TMP_FontAsset font, float k, int x, int y, int w, int h, float size, TextAlignmentOptions align, Color color)
		{
			GameObject gameObject = new GameObject("LewdHealthText", new Type[] { typeof(RectTransform) });
			gameObject.transform.SetParent(parent, false);
			RectTransform rectTransform = (RectTransform)gameObject.transform;
			Vector2 vector;
			vector..ctor(0f, 1f);
			rectTransform.pivot = vector;
			rectTransform.anchorMin = (rectTransform.anchorMax = vector);
			rectTransform.anchoredPosition = new Vector2((float)x * k, (float)(-(float)y) * k);
			rectTransform.sizeDelta = new Vector2((float)w * k, (float)h * k);
			TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
			if (font)
			{
				textMeshProUGUI.font = font;
			}
			textMeshProUGUI.fontSize = size * k;
			textMeshProUGUI.alignment = align;
			textMeshProUGUI.color = color;
			textMeshProUGUI.enableWordWrapping = false;
			textMeshProUGUI.overflowMode = 0;
			textMeshProUGUI.lineSpacing = -10f;
			textMeshProUGUI.raycastTarget = false;
			return textMeshProUGUI;
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x000419A0 File Offset: 0x0003FBA0
		private static string Pct(float v)
		{
			return Mathf.RoundToInt(Mathf.Clamp(v, 0f, 100f)).ToString();
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x000419CC File Offset: 0x0003FBCC
		private static void Fill(Body b)
		{
			string[] array = new string[] { "lh_breast", "lh_womb", "lh_vulva", "lh_butt" };
			for (int i = 0; i < 4; i++)
			{
				if (LewdHealthPanel.titles[i])
				{
					LewdHealthPanel.titles[i].text = Plugin.T(array[i]);
				}
			}
			string text = Plugin.T("lh_size" + Mathf.Clamp(LewdHealthPanel.S.bust, 0, 2).ToString());
			string text2 = (Suit.Milking ? Plugin.T("lh_milk") : ((LewdHealthPanel.S.arousal > 55f) ? Plugin.T("lh_nip_hard") : Plugin.T("lh_nip_soft")));
			LewdHealthPanel.lines[0].text = Plugin.T("lh_size").Replace("<v>", text) + "\n" + text2;
			string text3 = (LewdHealthPanel.S.pregnant ? Plugin.T("lh_preg").Replace("<v>", Plugin.T("lh_preg" + Womb.PregStage.ToString())) : Plugin.T("lh_notpreg"));
			LewdHealthPanel.lines[1].text = Plugin.T("lh_filled").Replace("<v>", LewdHealthPanel.Pct(LewdHealthPanel.S.filled)) + "\n" + text3;
			string text4 = (LewdHealthPanel.S.inHeat ? Plugin.T("lh_heat") : Plugin.T("lh_orgasms").Replace("<v>", LewdHealthPanel.S.orgasms.ToString()));
			LewdHealthPanel.lines[2].text = Plugin.T("lh_arousal").Replace("<v>", LewdHealthPanel.Pct(LewdHealthPanel.S.arousal)) + "\n" + text4;
			LewdHealthPanel.lines[3].text = Plugin.T("lh_butt_fill").Replace("<v>", LewdHealthPanel.Pct(LewdHealthPanel.S.cum));
			if (Suit.Latex)
			{
				TextMeshProUGUI textMeshProUGUI = LewdHealthPanel.lines[3];
				textMeshProUGUI.text = textMeshProUGUI.text + "\n" + Plugin.T("lh_latex_corruption").Replace("<v>", LewdHealthPanel.Pct(LewdHealthPanel.S.latexCorruption));
			}
		}

		// Token: 0x04000395 RID: 917
		private const int BW = 130;

		// Token: 0x04000396 RID: 918
		private const int BH = 171;

		// Token: 0x04000397 RID: 919
		private const int PanelW = 60;

		// Token: 0x04000398 RID: 920
		private const int PanelH = 76;

		// Token: 0x04000399 RID: 921
		private const int Top = 13;

		// Token: 0x0400039A RID: 922
		private const int ArtW = 52;

		// Token: 0x0400039B RID: 923
		private const int ArtH = 42;

		// Token: 0x0400039C RID: 924
		private const int Frames = 12;

		// Token: 0x0400039D RID: 925
		private const float Fps = 9f;

		// Token: 0x0400039E RID: 926
		private static readonly Color Green = new Color32(47, 224, 129, byte.MaxValue);

		// Token: 0x0400039F RID: 927
		private static readonly Color Soft = new Color32(86, 221, 132, byte.MaxValue);

		// Token: 0x040003A0 RID: 928
		private static readonly Color VioletText = new Color32(190, 120, byte.MaxValue, byte.MaxValue);

		// Token: 0x040003A1 RID: 929
		private static readonly Color VioletSoft = new Color32(204, 156, byte.MaxValue, byte.MaxValue);

		// Token: 0x040003A2 RID: 930
		private static readonly Dictionary<Texture2D, Texture2D> violet = new Dictionary<Texture2D, Texture2D>();

		// Token: 0x040003A3 RID: 931
		private static Texture2D baseTex;

		// Token: 0x040003A4 RID: 932
		private static bool shownViolet;

		// Token: 0x040003A5 RID: 933
		private static string dir;

		// Token: 0x040003A6 RID: 934
		private static readonly Texture2D[] texs = new Texture2D[3];

		// Token: 0x040003A7 RID: 935
		private static int shownBust = -1;

		// Token: 0x040003A8 RID: 936
		private static GameObject root;

		// Token: 0x040003A9 RID: 937
		private static readonly TextMeshProUGUI[] titles = new TextMeshProUGUI[4];

		// Token: 0x040003AA RID: 938
		private static readonly TextMeshProUGUI[] lines = new TextMeshProUGUI[4];

		// Token: 0x040003AB RID: 939
		private static float refresh;

		// Token: 0x040003AC RID: 940
		private static float scale;

		// Token: 0x040003AD RID: 941
		private static RawImage image;

		// Token: 0x040003AE RID: 942
		private static readonly RawImage[] windows = new RawImage[4];

		// Token: 0x040003AF RID: 943
		private static RawImage plugBase;

		// Token: 0x040003B0 RID: 944
		private static readonly string[] shownPic = new string[4];

		// Token: 0x040003B1 RID: 945
		private static readonly bool[] animated = new bool[4];

		// Token: 0x040003B2 RID: 946
		private static readonly Dictionary<string, Texture2D> pics = new Dictionary<string, Texture2D>();

		// Token: 0x040003B3 RID: 947
		private static bool layered;

		// Token: 0x040003B4 RID: 948
		private static TextMeshProUGUI arrow;

		// Token: 0x040003B5 RID: 949
		public static ConfigEntry<bool> Collapsed;

		// Token: 0x040003B6 RID: 950
		private const float ButtBreathFrom = 30f;

		// Token: 0x040003B7 RID: 951
		private static float buttPhase;
	}
}
