using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ScavArmory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LewdStatus
{
	// Token: 0x02000092 RID: 146
	internal static class GameOverPic
	{
		// Token: 0x060002EF RID: 751 RVA: 0x0003325D File Offset: 0x0003145D
		public static void Init(string nsfwDir)
		{
			GameOverPic.root = nsfwDir;
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x00033268 File Offset: 0x00031468
		public static string Fate
		{
			get
			{
				if (!GameOverPic.diedInScene || GameOverPic.lastCause == null || !Plugin.Text.ContainsKey("fate_" + GameOverPic.lastCause))
				{
					return null;
				}
				return Plugin.T("fate_" + GameOverPic.lastCause);
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x000332B4 File Offset: 0x000314B4
		// (set) Token: 0x060002F2 RID: 754 RVA: 0x000332BB File Offset: 0x000314BB
		public static int Subject { get; private set; }

		// Token: 0x060002F3 RID: 755 RVA: 0x000332C4 File Offset: 0x000314C4
		public static void Track(Body b)
		{
			if (!b)
			{
				return;
			}
			if (!b.alive)
			{
				if (GameOverPic.wasAlive)
				{
					GameOverPic.diedInScene = Time.time - GameOverPic.lastSceneTime < 3f || HostConversion.Done;
					GameOverPic.Subject = Random.Range(10000, 99999);
					Plugin.Log.LogInfo("She died " + (GameOverPic.diedInScene ? ("in a scene (" + GameOverPic.lastKind + ")") : "outside any scene"));
				}
				GameOverPic.wasAlive = false;
				return;
			}
			GameOverPic.wasAlive = true;
			GameOverPic.diedInScene = false;
			string text = (HostConversion.Active ? "hostslime" : (LatexTakeover.Active ? "latexsuit" : (LatexPond.Busy ? "latexpond" : (Latexes.Busy ? "latexblob" : (LatexMites.Busy ? "latexmite" : (FleshWall.Holding ? "fleshwall" : (Pits.InMaw ? (Pits.Ravishing ? "pitravish" : "pitmaw") : (Slits.Inside ? (Slits.Ravishing ? "pitravish" : "pitmaw") : (Slits.Holding ? "tentacle" : (Tentacles.Holding ? "tentacle" : (Flowers.Holding ? "flower" : (Worm.Inside ? "worm" : (Flesh.Holding ? "flesh" : (Webs.Caught ? "web" : (SlimeBog.Stuck ? "slime" : (External.ScavHolding ? "scav" : null))))))))))))))));
			if (text == null)
			{
				return;
			}
			GameOverPic.lastCause = text;
			GameOverPic.lastKind = ((text == "hostslime" || (text == "scav" && Capture.Holder && Capture.Holder.slimeHost) || text == "web") ? "none" : ((text == "worm" || text == "flesh" || text == "pitmaw" || text == "flower") ? "vore" : ((text == "fleshwall" || text == "tentacle") ? text : ((text == "pitravish") ? "tentacle" : ((text == "slime") ? "slime" : ((text == "latexsuit") ? "Latexsuit" : (text.StartsWith("latex") ? "latex" : ((text == "scav") ? "raider" : "any"))))))));
			GameOverPic.lastSceneTime = Time.time;
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060002F4 RID: 756 RVA: 0x000335B6 File Offset: 0x000317B6
		public static bool DiedInScene
		{
			get
			{
				return GameOverPic.diedInScene;
			}
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x000335C0 File Offset: 0x000317C0
		public static void Show(Image endScreen)
		{
			GameOverPic.Hide();
			if (!Plugin.GameOverPicEnabled.Value || !GameOverPic.DiedInScene || GameOverPic.root == null || !Directory.Exists(GameOverPic.root))
			{
				return;
			}
			if (GameOverPic.lastKind == "none")
			{
				return;
			}
			string text = Path.Combine(GameOverPic.root, GameOverPic.lastKind ?? "any");
			string[] array = (Directory.Exists(text) ? GameOverPic.Pictures(text) : new string[0]);
			if (array.Length == 0 && (GameOverPic.lastKind ?? "any") == "any")
			{
				IEnumerable<string> directories = Directory.GetDirectories(GameOverPic.root);
				Func<string, IEnumerable<string>> func;
				if ((func = GameOverPic.<>O.<0>__Pictures) == null)
				{
					func = (GameOverPic.<>O.<0>__Pictures = new Func<string, IEnumerable<string>>(GameOverPic.Pictures));
				}
				array = directories.SelectMany<string, string>(func).ToArray<string>();
			}
			if (array.Length == 0)
			{
				return;
			}
			string text2 = array[Random.Range(0, array.Length)];
			Texture2D texture2D = new Texture2D(2, 2, 4, false);
			ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text2));
			GameOverPic.tex = new Texture2D(texture2D.width, texture2D.height, 4, false)
			{
				filterMode = 1,
				wrapMode = 1
			};
			GameOverPic.tex.SetPixels32(texture2D.GetPixels32());
			GameOverPic.tex.Apply();
			Object.Destroy(texture2D);
			Glitch.Damage(GameOverPic.tex);
			Glitch.Feather(GameOverPic.tex);
			GameOverPic.shown = new GameObject("LewdGameOverPicture", new Type[]
			{
				typeof(RectTransform),
				typeof(CanvasGroup),
				typeof(RawImage)
			});
			GameOverPic.shown.transform.SetParent(endScreen.transform.parent, false);
			GameOverPic.shown.transform.SetSiblingIndex(endScreen.transform.GetSiblingIndex() + 1);
			RectTransform rectTransform = (RectTransform)GameOverPic.shown.transform;
			Vector2 vector;
			vector..ctor(0.5f, 0.5f);
			rectTransform.pivot = vector;
			rectTransform.anchorMin = (rectTransform.anchorMax = vector);
			RectTransform rectTransform2 = (RectTransform)endScreen.canvas.transform;
			float num = rectTransform2.rect.height * 0.86f;
			rectTransform.sizeDelta = new Vector2(num * (float)GameOverPic.tex.width / (float)GameOverPic.tex.height, num);
			rectTransform.anchoredPosition = new Vector2(-rectTransform2.rect.width * 0.18f, 0f);
			RawImage component = GameOverPic.shown.GetComponent<RawImage>();
			component.texture = GameOverPic.tex;
			component.raycastTarget = false;
			CanvasGroup component2 = GameOverPic.shown.GetComponent<CanvasGroup>();
			component2.alpha = 0f;
			component2.blocksRaycasts = false;
			GameOverPic.shown.AddComponent<GameOverPic.FadeIn>().Init(component2, 1.2f, 1.6f);
			GameOverPic.shown.AddComponent<Glitch.Jitter>().Init(component);
			GameOverPic.Caption(rectTransform, num);
			Plugin.Log.LogInfo("Game over in a scene (" + GameOverPic.lastKind + "): showing " + Path.GetFileName(text2));
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x000338C8 File Offset: 0x00031AC8
		private static void Caption(RectTransform pic, float h)
		{
			GameObject gameObject = new GameObject("LewdFate", new Type[] { typeof(RectTransform) });
			gameObject.transform.SetParent(pic, false);
			RectTransform rectTransform = (RectTransform)gameObject.transform;
			rectTransform.anchorMin = new Vector2(0f, 0f);
			rectTransform.anchorMax = new Vector2(1f, 0f);
			rectTransform.pivot = new Vector2(0.5f, 0f);
			rectTransform.anchoredPosition = new Vector2(0f, h * 0.02f);
			rectTransform.sizeDelta = new Vector2(-h * 0.04f, h * 0.12f);
			TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
			TextMeshProUGUI textMeshProUGUI2 = ((PlayerCamera.main && PlayerCamera.main.deathStats) ? PlayerCamera.main.deathStats.GetComponentInChildren<TextMeshProUGUI>(true) : null);
			if (textMeshProUGUI2)
			{
				textMeshProUGUI.font = textMeshProUGUI2.font;
			}
			textMeshProUGUI.fontSize = h * 0.032f;
			textMeshProUGUI.alignment = 1026;
			textMeshProUGUI.color = new Color32(byte.MaxValue, 70, 90, byte.MaxValue);
			textMeshProUGUI.raycastTarget = false;
			textMeshProUGUI.text = Plugin.T("fate_subject").Replace("<n>", GameOverPic.Subject.ToString()) + "\n" + GameOverPic.Fate;
			gameObject.AddComponent<Glitch.TextFlicker>().Init(textMeshProUGUI);
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00033A43 File Offset: 0x00031C43
		public static void Hide()
		{
			if (GameOverPic.shown)
			{
				Object.Destroy(GameOverPic.shown);
			}
			if (GameOverPic.tex)
			{
				Object.Destroy(GameOverPic.tex);
			}
			GameOverPic.shown = null;
			GameOverPic.tex = null;
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00033A7D File Offset: 0x00031C7D
		private static string[] Pictures(string dir)
		{
			return (from f in Directory.GetFiles(dir)
				where new string[] { ".png", ".jpg", ".jpeg" }.Contains(Path.GetExtension(f).ToLowerInvariant())
				select f).ToArray<string>();
		}

		// Token: 0x040002BC RID: 700
		private static string root;

		// Token: 0x040002BD RID: 701
		private static string lastKind;

		// Token: 0x040002BE RID: 702
		private static float lastSceneTime = -100f;

		// Token: 0x040002BF RID: 703
		private static GameObject shown;

		// Token: 0x040002C0 RID: 704
		private static Texture2D tex;

		// Token: 0x040002C1 RID: 705
		private static string lastCause;

		// Token: 0x040002C3 RID: 707
		private static bool wasAlive = true;

		// Token: 0x040002C4 RID: 708
		private static bool diedInScene;

		// Token: 0x020001B9 RID: 441
		private class FadeIn : MonoBehaviour
		{
			// Token: 0x06000996 RID: 2454 RVA: 0x00079D51 File Offset: 0x00077F51
			public void Init(CanvasGroup g, float wait, float length)
			{
				this.cg = g;
				this.delay = wait;
				this.time = length;
			}

			// Token: 0x06000997 RID: 2455 RVA: 0x00079D68 File Offset: 0x00077F68
			private void Update()
			{
				this.t += Time.unscaledDeltaTime;
				if (this.cg)
				{
					this.cg.alpha = Mathf.Clamp01((this.t - this.delay) / this.time);
				}
			}

			// Token: 0x0400098A RID: 2442
			private CanvasGroup cg;

			// Token: 0x0400098B RID: 2443
			private float delay;

			// Token: 0x0400098C RID: 2444
			private float time;

			// Token: 0x0400098D RID: 2445
			private float t;
		}

		// Token: 0x020001BA RID: 442
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400098E RID: 2446
			public static Func<string, IEnumerable<string>> <0>__Pictures;
		}
	}
}
