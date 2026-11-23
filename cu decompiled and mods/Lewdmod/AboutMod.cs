using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using ScavArmory;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LewdStatus
{
	// Token: 0x0200004C RID: 76
	internal static class AboutMod
	{
		// Token: 0x0600017C RID: 380 RVA: 0x00019D50 File Offset: 0x00017F50
		private static string KeysBlock()
		{
			KeyCode key = GunControls.Key;
			return string.Concat(new string[]
			{
				"<size=120%><b>",
				Plugin.T("about_keys_title"),
				"</b></size>\n",
				AboutMod.<KeysBlock>g__Row|6_0(Plugin.ReliefKey.Value, "about_key_relief"),
				AboutMod.<KeysBlock>g__Row|6_0(Plugin.RemoveKey.Value, "about_key_free"),
				AboutMod.<KeysBlock>g__Row|6_0(Plugin.SurrenderKey.Value, "about_key_surrender"),
				AboutMod.<KeysBlock>g__Row|6_0(Plugin.DealKey.Value, "about_key_deal"),
				AboutMod.<KeysBlock>g__Row|6_0(key, "about_key_reload"),
				AboutMod.<KeysBlock>g__Row|6_0(Plugin.WormDropKey.Value, "about_key_dynamite"),
				Plugin.T("about_key_debug")
			});
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00019E1B File Offset: 0x0001801B
		public static string Version
		{
			get
			{
				if (!Plugin.Instance)
				{
					return "?";
				}
				return Plugin.Instance.Info.Metadata.Version.ToString();
			}
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00019E48 File Offset: 0x00018048
		public static void Bind(ConfigFile c)
		{
			AboutMod.StartupNotice = c.Bind<bool>("General", "StartupNotice", true, "A small notice in the game's menu the first time it comes up after launch: the mod's name, version and where to read about it.");
			SceneManager.sceneLoaded += delegate(Scene scene, LoadSceneMode mode)
			{
				if (scene.name != "PreGen" || AboutMod.noticeShown || !AboutMod.StartupNotice.Value || !Plugin.Instance)
				{
					return;
				}
				AboutMod.noticeShown = true;
				Plugin.Instance.StartCoroutine(AboutMod.NoticeSoon());
			};
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00019E94 File Offset: 0x00018094
		private static IEnumerator NoticeSoon()
		{
			yield return new WaitForSecondsRealtime(1.2f);
			AboutMod.ShowNotice();
			yield break;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00019E9C File Offset: 0x0001809C
		private static Transform Canvas()
		{
			if (AboutMod.canvasGo)
			{
				return AboutMod.canvasGo.transform;
			}
			AboutMod.canvasGo = new GameObject("LewdAboutCanvas", new Type[]
			{
				typeof(RectTransform),
				typeof(Canvas),
				typeof(CanvasScaler),
				typeof(GraphicRaycaster)
			});
			Object.DontDestroyOnLoad(AboutMod.canvasGo);
			Canvas component = AboutMod.canvasGo.GetComponent<Canvas>();
			component.renderMode = 0;
			component.sortingOrder = 32000;
			CanvasScaler component2 = AboutMod.canvasGo.GetComponent<CanvasScaler>();
			component2.uiScaleMode = 1;
			component2.referenceResolution = new Vector2(1920f, 1080f);
			component2.matchWidthOrHeight = 0.5f;
			AboutMod.canvasGo.AddComponent<AboutKeys>();
			return AboutMod.canvasGo.transform;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00019F73 File Offset: 0x00018173
		private static void EnsureEventSystem()
		{
			if (EventSystem.current)
			{
				return;
			}
			Object.DontDestroyOnLoad(new GameObject("LewdAboutEvents", new Type[]
			{
				typeof(EventSystem),
				typeof(StandaloneInputModule)
			}));
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00019FB4 File Offset: 0x000181B4
		private static TMP_FontAsset Font()
		{
			if (AboutMod.font)
			{
				return AboutMod.font;
			}
			TextMeshProUGUI textMeshProUGUI = Object.FindObjectsOfType<TextMeshProUGUI>().FirstOrDefault<TextMeshProUGUI>((TextMeshProUGUI t) => t && t.font);
			AboutMod.font = (textMeshProUGUI ? textMeshProUGUI.font : TMP_Settings.defaultFontAsset);
			return AboutMod.font;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0001A01C File Offset: 0x0001821C
		private static RectTransform Box(Transform parent, string name, Color colour)
		{
			GameObject gameObject = new GameObject(name, new Type[]
			{
				typeof(RectTransform),
				typeof(Image)
			});
			gameObject.transform.SetParent(parent, false);
			gameObject.GetComponent<Image>().color = colour;
			return (RectTransform)gameObject.transform;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0001A074 File Offset: 0x00018274
		private static TextMeshProUGUI Text(Transform parent, string text, float size, TextAlignmentOptions align)
		{
			GameObject gameObject = new GameObject("Text", new Type[]
			{
				typeof(RectTransform),
				typeof(TextMeshProUGUI)
			});
			gameObject.transform.SetParent(parent, false);
			TextMeshProUGUI component = gameObject.GetComponent<TextMeshProUGUI>();
			component.font = AboutMod.Font();
			component.fontSize = size;
			component.color = new Color(0.93f, 0.9f, 0.92f);
			component.alignment = align;
			component.enableWordWrapping = true;
			component.richText = true;
			component.raycastTarget = false;
			component.text = text;
			return component;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0001A10C File Offset: 0x0001830C
		private static Button ButtonAt(Transform parent, string label, Vector2 anchor, Vector2 pos, Vector2 size, Action click)
		{
			RectTransform rectTransform = AboutMod.Box(parent, "Button", new Color(0.36f, 0.1f, 0.2f, 1f));
			RectTransform rectTransform2 = rectTransform;
			rectTransform.anchorMax = anchor;
			rectTransform2.anchorMin = anchor;
			rectTransform.pivot = anchor;
			rectTransform.anchoredPosition = pos;
			rectTransform.sizeDelta = size;
			Button button = rectTransform.gameObject.AddComponent<Button>();
			ColorBlock colors = button.colors;
			colors.highlightedColor = new Color(1.35f, 1.35f, 1.35f);
			colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
			button.colors = colors;
			button.onClick.AddListener(delegate
			{
				click();
			});
			AboutMod.Stretch((RectTransform)AboutMod.Text(rectTransform, label, 22f, 514).transform, 4f);
			return button;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0001A1FD File Offset: 0x000183FD
		private static void Stretch(RectTransform rt, float inset)
		{
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.offsetMin = new Vector2(inset, inset);
			rt.offsetMax = new Vector2(-inset, -inset);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0001A234 File Offset: 0x00018434
		public static void ShowNotice()
		{
			if (AboutMod.notice)
			{
				Object.Destroy(AboutMod.notice);
			}
			TextMeshProUGUI textMeshProUGUI = AboutMod.Text(AboutMod.Canvas(), string.Format(Plugin.T("about_notice"), AboutMod.Version), 20f, 1025);
			AboutMod.notice = textMeshProUGUI.gameObject;
			RectTransform rectTransform = (RectTransform)textMeshProUGUI.transform;
			Vector2 vector;
			vector..ctor(0f, 0f);
			rectTransform.pivot = vector;
			rectTransform.anchorMin = (rectTransform.anchorMax = vector);
			rectTransform.anchoredPosition = new Vector2(16f, 12f);
			rectTransform.sizeDelta = new Vector2(900f, 30f);
			textMeshProUGUI.enableWordWrapping = false;
			textMeshProUGUI.color = new Color(0.86f, 0.62f, 0.76f, 1f);
			textMeshProUGUI.outlineWidth = 0.2f;
			textMeshProUGUI.outlineColor = new Color32(0, 0, 0, 200);
			Plugin.Instance.StartCoroutine(AboutMod.FadeAfter(AboutMod.notice, 8f));
			Plugin.Log.LogInfo("Startup notice: LEWD Casualties UNKNOWN v" + AboutMod.Version);
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0001A35C File Offset: 0x0001855C
		private static IEnumerator FadeAfter(GameObject which, float seconds)
		{
			yield return new WaitForSecondsRealtime(seconds);
			TextMeshProUGUI t = (which ? which.GetComponent<TextMeshProUGUI>() : null);
			float a = 1f;
			while (t && a > 0f)
			{
				t.alpha = a;
				yield return null;
				a -= Time.unscaledDeltaTime / 1.5f;
			}
			if (which && which == AboutMod.notice)
			{
				AboutMod.HideNotice();
			}
			yield break;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0001A372 File Offset: 0x00018572
		public static void HideNotice()
		{
			if (AboutMod.notice)
			{
				Object.Destroy(AboutMod.notice);
			}
			AboutMod.notice = null;
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600018A RID: 394 RVA: 0x0001A390 File Offset: 0x00018590
		public static bool WindowOpen
		{
			get
			{
				return AboutMod.window;
			}
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0001A39C File Offset: 0x0001859C
		public static void ShowWindow()
		{
			if (AboutMod.window)
			{
				return;
			}
			AboutMod.EnsureEventSystem();
			RectTransform rectTransform = AboutMod.Box(AboutMod.Canvas(), "LewdAbout", new Color(0f, 0f, 0f, 0.6f));
			AboutMod.window = rectTransform.gameObject;
			AboutMod.Stretch(rectTransform, 0f);
			RectTransform rectTransform2 = AboutMod.Box(rectTransform, "Panel", new Color(0.08f, 0.05f, 0.07f, 0.98f));
			RectTransform rectTransform3 = rectTransform2;
			RectTransform rectTransform4 = rectTransform2;
			RectTransform rectTransform5 = rectTransform2;
			Vector2 vector;
			vector..ctor(0.5f, 0.5f);
			rectTransform5.pivot = vector;
			rectTransform3.anchorMin = (rectTransform4.anchorMax = vector);
			rectTransform2.sizeDelta = new Vector2(1100f, 820f);
			RectTransform rectTransform6 = AboutMod.Box(rectTransform2, "Head", new Color(0.36f, 0.1f, 0.2f, 1f));
			rectTransform6.anchorMin = new Vector2(0f, 1f);
			rectTransform6.anchorMax = new Vector2(1f, 1f);
			rectTransform6.pivot = new Vector2(0.5f, 1f);
			rectTransform6.sizeDelta = new Vector2(0f, 64f);
			TextMeshProUGUI textMeshProUGUI = AboutMod.Text(rectTransform6, string.Format(Plugin.T("about_title"), AboutMod.Version), 30f, 4097);
			AboutMod.Stretch((RectTransform)textMeshProUGUI.transform, 0f);
			((RectTransform)textMeshProUGUI.transform).offsetMin = new Vector2(24f, 0f);
			RectTransform rectTransform7 = AboutMod.Box(rectTransform2, "View", new Color(0f, 0f, 0f, 0.01f));
			rectTransform7.anchorMin = Vector2.zero;
			rectTransform7.anchorMax = Vector2.one;
			rectTransform7.offsetMin = new Vector2(24f, 76f);
			rectTransform7.offsetMax = new Vector2(-24f, -76f);
			rectTransform7.gameObject.AddComponent<RectMask2D>();
			RectTransform component = new GameObject("Content", new Type[] { typeof(RectTransform) }).GetComponent<RectTransform>();
			component.SetParent(rectTransform7, false);
			component.anchorMin = new Vector2(0f, 1f);
			component.anchorMax = new Vector2(1f, 1f);
			component.pivot = new Vector2(0.5f, 1f);
			TextMeshProUGUI textMeshProUGUI2 = AboutMod.Text(component, AboutMod.KeysBlock() + "\n\n" + Plugin.T("about_body"), 21f, 257);
			RectTransform rectTransform8 = (RectTransform)textMeshProUGUI2.transform;
			rectTransform8.anchorMin = new Vector2(0f, 1f);
			rectTransform8.anchorMax = new Vector2(1f, 1f);
			rectTransform8.pivot = new Vector2(0.5f, 1f);
			rectTransform8.offsetMin = new Vector2(0f, 0f);
			rectTransform8.offsetMax = new Vector2(-16f, 0f);
			textMeshProUGUI2.ForceMeshUpdate(false, false);
			float num = textMeshProUGUI2.GetPreferredValues(1036f, 0f).y + 20f;
			rectTransform8.sizeDelta = new Vector2(rectTransform8.sizeDelta.x, num);
			component.sizeDelta = new Vector2(0f, num);
			ScrollRect scrollRect = rectTransform7.gameObject.AddComponent<ScrollRect>();
			scrollRect.content = component;
			scrollRect.viewport = rectTransform7;
			scrollRect.horizontal = false;
			scrollRect.movementType = 2;
			scrollRect.scrollSensitivity = 40f;
			Transform transform = rectTransform2;
			string text = Plugin.T("about_close");
			Vector2 vector2 = new Vector2(1f, 0f);
			Vector2 vector3 = new Vector2(-24f, 18f);
			Vector2 vector4 = new Vector2(180f, 44f);
			Action action;
			if ((action = AboutMod.<>O.<0>__HideWindow) == null)
			{
				action = (AboutMod.<>O.<0>__HideWindow = new Action(AboutMod.HideWindow));
			}
			AboutMod.ButtonAt(transform, text, vector2, vector3, vector4, action);
			TextMeshProUGUI textMeshProUGUI3 = AboutMod.Text(rectTransform2, Plugin.T("about_hint"), 18f, 4097);
			RectTransform rectTransform9 = (RectTransform)textMeshProUGUI3.transform;
			rectTransform9.anchorMin = new Vector2(0f, 0f);
			rectTransform9.anchorMax = new Vector2(1f, 0f);
			rectTransform9.pivot = new Vector2(0f, 0f);
			rectTransform9.offsetMin = new Vector2(24f, 18f);
			rectTransform9.offsetMax = new Vector2(-220f, 62f);
			textMeshProUGUI3.color = new Color(0.7f, 0.66f, 0.7f);
			Plugin.Log.LogInfo(string.Format("About window open ({0:0} px of text)", num));
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0001A83E File Offset: 0x00018A3E
		public static void HideWindow()
		{
			if (AboutMod.window)
			{
				Object.Destroy(AboutMod.window);
			}
			AboutMod.window = null;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0001A85C File Offset: 0x00018A5C
		[CompilerGenerated]
		internal static string <KeysBlock>g__Row|6_0(KeyCode k, string what)
		{
			if (k != null)
			{
				return string.Format("<color=#E07AA8><b>{0}</b></color> - {1}\n", k, Plugin.T(what));
			}
			return "";
		}

		// Token: 0x0400018A RID: 394
		public static ConfigEntry<bool> StartupNotice;

		// Token: 0x0400018B RID: 395
		private static bool noticeShown;

		// Token: 0x0400018C RID: 396
		private static GameObject canvasGo;

		// Token: 0x0400018D RID: 397
		private static GameObject notice;

		// Token: 0x0400018E RID: 398
		private static GameObject window;

		// Token: 0x0400018F RID: 399
		private static TMP_FontAsset font;

		// Token: 0x02000174 RID: 372
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000882 RID: 2178
			public static Action <0>__HideWindow;
		}
	}
}
