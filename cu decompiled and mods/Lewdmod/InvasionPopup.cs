using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;
using ScavArmory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LewdStatus
{
	// Token: 0x020000A4 RID: 164
	internal static class InvasionPopup
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000333 RID: 819 RVA: 0x00035CB0 File Offset: 0x00033EB0
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00035CB7 File Offset: 0x00033EB7
		public static void Bind(ConfigFile c)
		{
			InvasionPopup.Enabled = c.Bind<bool>("Body", "InvasionPopups", true, "During a monster scene or while a living suit takes her, a window of the EROS-SCAN monitor slides in on the left for a moment, showing what is done to her (yellow; violet once she likes the suit).");
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00035CD4 File Offset: 0x00033ED4
		public static void Load(string folder)
		{
			InvasionPopup.dir = folder;
		}

		// Token: 0x06000336 RID: 822 RVA: 0x00035CDC File Offset: 0x00033EDC
		private static string Loved(string name)
		{
			if (name.EndsWith("_love"))
			{
				return name;
			}
			string text = name + "_love";
			if ((!InvasionPopup.pics.ContainsKey(text) || !InvasionPopup.pics[text]) && !File.Exists(Path.Combine(InvasionPopup.dir ?? "", text + ".png")))
			{
				return name;
			}
			return text;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00035D4C File Offset: 0x00033F4C
		private static Texture2D Pic(string name)
		{
			Texture2D texture2D;
			if (InvasionPopup.pics.TryGetValue(name, out texture2D))
			{
				return texture2D;
			}
			string text = Path.Combine(InvasionPopup.dir ?? "", name + ".png");
			if (File.Exists(text))
			{
				texture2D = new Texture2D(2, 2, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text));
			}
			else if (InvasionPopup.missing.Add(name))
			{
				Plugin.Log.LogWarning("Invasion window: no " + name + ".png");
			}
			return InvasionPopup.pics[name] = texture2D;
		}

		// Token: 0x06000338 RID: 824 RVA: 0x00035DF0 File Offset: 0x00033FF0
		private static void Show(string anim, string part, bool love, bool once = false, string then = null, float seconds = 3.6f)
		{
			if (InvasionPopup.Enabled != null && !InvasionPopup.Enabled.Value)
			{
				return;
			}
			if (Suit.Latex)
			{
				love = true;
				anim = InvasionPopup.Loved(anim);
				if (then != null)
				{
					then = InvasionPopup.Loved(then);
				}
			}
			if (!InvasionPopup.Pic(anim) || (then != null && !InvasionPopup.Pic(then)))
			{
				return;
			}
			InvasionPopup.Req req = new InvasionPopup.Req
			{
				Anim = anim,
				Part = part,
				Love = love,
				Once = once,
				Then = then,
				Seconds = seconds
			};
			if (InvasionPopup.cur == null)
			{
				InvasionPopup.Start(req);
				return;
			}
			InvasionPopup.pending = req;
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00035E96 File Offset: 0x00034096
		private static void Start(InvasionPopup.Req r)
		{
			InvasionPopup.cur = r;
			InvasionPopup.pending = null;
			InvasionPopup.shownAt = Time.time;
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600033A RID: 826 RVA: 0x00035EAE File Offset: 0x000340AE
		private static int Bust
		{
			get
			{
				return Mathf.Clamp(InvasionPopup.S.bust, 0, 2);
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600033B RID: 827 RVA: 0x00035EC1 File Offset: 0x000340C1
		private static bool LivingSuit
		{
			get
			{
				return Suit.Stage > 0 && !Suit.Drone && (Suit.Slime || Suit.Symbiote || Suit.Latex || InvasionPopup.S.suitColor <= 2);
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600033C RID: 828 RVA: 0x00035EFB File Offset: 0x000340FB
		private static string Kind
		{
			get
			{
				if (Suit.Latex)
				{
					return "latex";
				}
				if (Suit.Slime)
				{
					return "slime";
				}
				if (!Suit.Symbiote)
				{
					return "cloth";
				}
				return "sym";
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600033D RID: 829 RVA: 0x00035F29 File Offset: 0x00034129
		private static string Col
		{
			get
			{
				return ParasiteArt.ColorNames[Mathf.Clamp(InvasionPopup.S.suitColor, 0, 2)];
			}
		}

		// Token: 0x0600033E RID: 830 RVA: 0x00035F44 File Offset: 0x00034144
		private static string SuitAnim(string part, bool milk = false)
		{
			bool flag = InvasionPopup.Kind == "cloth";
			string text;
			if (!(part == "breast"))
			{
				if (!(part == "womb"))
				{
					text = (flag ? ("cloth_" + part + "_" + InvasionPopup.Col) : (InvasionPopup.Kind + "_" + part));
				}
				else
				{
					text = (flag ? ("cloth_vagina_" + InvasionPopup.Col) : (InvasionPopup.Kind + "_womb"));
				}
			}
			else
			{
				text = ((milk && !Suit.Enjoying) ? (flag ? string.Format("cloth_breast_milk_{0}_{1}", InvasionPopup.Col, InvasionPopup.Bust) : string.Format("{0}_breast_milk_{1}", InvasionPopup.Kind, InvasionPopup.Bust)) : (flag ? string.Format("cloth_breast_{0}_{1}", InvasionPopup.Col, InvasionPopup.Bust) : string.Format("{0}_breast_{1}", InvasionPopup.Kind, InvasionPopup.Bust)));
			}
			if (!Suit.Enjoying)
			{
				return text;
			}
			return text + "_love";
		}

		// Token: 0x0600033F RID: 831 RVA: 0x00036068 File Offset: 0x00034268
		public static void SuitStage(Body b, int stage)
		{
			if (!InvasionPopup.LivingSuit || !b || !b.alive)
			{
				return;
			}
			if (stage > 1)
			{
				string text = ((stage == 3) ? "anal" : "vagina");
				InvasionPopup.Show(InvasionPopup.SuitAnim(text, false), text, Suit.Enjoying, false, null, 3.6f);
				return;
			}
			if (Suit.Enjoying)
			{
				InvasionPopup.Show(InvasionPopup.SuitAnim("breast", false), "breast", true, false, null, 3.6f);
				return;
			}
			InvasionPopup.Show(InvasionPopup.SuitAnim("breast", false), "breast", false, true, Suit.Milking ? InvasionPopup.SuitAnim("breast", true) : null, Suit.Milking ? 4.6f : 3f);
		}

		// Token: 0x06000340 RID: 832 RVA: 0x00036120 File Offset: 0x00034320
		public static void SuitPulse(Body b)
		{
			if (!InvasionPopup.LivingSuit || !b || !b.alive || InvasionPopup.cur != null)
			{
				return;
			}
			string text = ((Random.value < 0.55f) ? "vagina" : "anal");
			InvasionPopup.Show(InvasionPopup.SuitAnim(text, false), text, Suit.Enjoying, false, null, 3.6f);
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0003617E File Offset: 0x0003437E
		private static bool SlimeHost
		{
			get
			{
				return Capture.Holder && Capture.Holder.slimeHost;
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000342 RID: 834 RVA: 0x00036198 File Offset: 0x00034398
		private static bool SlimeTendrils
		{
			get
			{
				return InvasionPopup.SlimeHost || (SlimeBog.Stuck && PlayerCamera.main && Pits.InSlimePit(PlayerCamera.main.body));
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000343 RID: 835 RVA: 0x000361C7 File Offset: 0x000343C7
		private static bool SlimeScene
		{
			get
			{
				return InvasionPopup.SlimeTendrils || SlimeBog.Stuck;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000344 RID: 836 RVA: 0x000361D8 File Offset: 0x000343D8
		private static bool MonsterScene
		{
			get
			{
				return Flowers.Holding || Tentacles.Holding || Worm.Sex || Flesh.Holding || FleshWall.Holding || Pits.InMaw || Slits.Inside || SlimeBog.Stuck || InvasionPopup.SlimeHost || Latexes.Using || LatexPond.Inside;
			}
		}

		// Token: 0x06000345 RID: 837 RVA: 0x00036234 File Offset: 0x00034434
		private static void MonsterWindow(string part)
		{
			if (Latexes.Using || LatexPond.Inside)
			{
				InvasionPopup.Show((part == "breast") ? string.Format("latex_breast_{0}", InvasionPopup.Bust) : ("latex_" + part), part, false, false, null, 3.6f);
				return;
			}
			if (InvasionPopup.SlimeTendrils)
			{
				InvasionPopup.Show((part == "breast") ? string.Format("slimet_breast_{0}", InvasionPopup.Bust) : ("slimet_" + part), part, false, false, null, 3.6f);
				return;
			}
			if (!InvasionPopup.SlimeScene)
			{
				InvasionPopup.Show((part == "breast") ? string.Format("breast_{0}", InvasionPopup.Bust) : part, part, false, false, null, 3.6f);
				return;
			}
			if (part == "breast")
			{
				InvasionPopup.Show(string.Format("slime_breast_{0}", InvasionPopup.Bust), part, false, true, null, 3.6f);
				return;
			}
			InvasionPopup.Show("slime_" + part, part, false, false, null, 3.6f);
		}

		// Token: 0x06000346 RID: 838 RVA: 0x00036354 File Offset: 0x00034554
		private static string RandomPart()
		{
			float value = Random.value;
			if (Flowers.Holding || (Pits.InMaw && Pits.Holder.Kind == PitKind.Flower && Pits.Holder.Milk))
			{
				return "breast";
			}
			if (Tentacles.Holding)
			{
				if (value >= 0.7f)
				{
					return "breast";
				}
				if (!Tentacles.Anal)
				{
					return "vagina";
				}
				return "anal";
			}
			else
			{
				if (value < 0.42f)
				{
					return "vagina";
				}
				if (value >= 0.72f)
				{
					return "breast";
				}
				return "anal";
			}
		}

		// Token: 0x06000347 RID: 839 RVA: 0x000363E0 File Offset: 0x000345E0
		private static void WombWindow()
		{
			if (InvasionPopup.wombCooldown > 0f)
			{
				return;
			}
			if (Latexes.Using || LatexPond.Inside)
			{
				InvasionPopup.Show("latex_womb", "womb", false, false, null, 3.6f);
			}
			else if (InvasionPopup.SlimeTendrils)
			{
				InvasionPopup.Show("slimet_womb", "womb", false, false, null, 3.6f);
			}
			else if (InvasionPopup.SlimeScene)
			{
				InvasionPopup.Show("slime_womb", "womb", false, false, null, 3.6f);
			}
			else if (InvasionPopup.MonsterScene)
			{
				InvasionPopup.Show("womb", "womb", false, false, null, 3.6f);
			}
			else
			{
				if (!InvasionPopup.LivingSuit || !(InvasionPopup.Kind != "cloth"))
				{
					return;
				}
				InvasionPopup.Show(InvasionPopup.SuitAnim("womb", false), "womb", Suit.Enjoying, false, null, 3.6f);
			}
			InvasionPopup.wombCooldown = 6f;
			InvasionPopup.sceneTimer = Mathf.Max(InvasionPopup.sceneTimer, Random.Range(7f, 11f));
		}

		// Token: 0x06000348 RID: 840 RVA: 0x000364E8 File Offset: 0x000346E8
		public static void OnCumInside(Body b, float amount)
		{
			InvasionPopup.insideFrame = Time.frameCount;
			if (amount >= 8f && b && b.alive)
			{
				InvasionPopup.WombWindow();
			}
		}

		// Token: 0x06000349 RID: 841 RVA: 0x00036511 File Offset: 0x00034711
		public static void OnOrgasm(Body b, bool self)
		{
			if (b && b.alive)
			{
				InvasionPopup.WombWindow();
			}
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00036528 File Offset: 0x00034728
		public static void OnCumOutside(Body b, float amount)
		{
			if (InvasionPopup.Quiet || amount < 8f || !b || !b.alive)
			{
				return;
			}
			InvasionPopup.outsideFrame = Time.frameCount;
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00036554 File Offset: 0x00034754
		private static void Cumshot()
		{
			if (InvasionPopup.SlimeScene)
			{
				return;
			}
			if (!InvasionPopup.MonsterScene && !Capture.Active && !InvasionPopup.LivingSuit && !Relief.Active)
			{
				return;
			}
			float value = Random.value;
			string text = ((value < 0.4f) ? "breast" : ((value < 0.7f) ? "butt" : "vagina"));
			InvasionPopup.Show((text == "breast") ? string.Format("cum_breast_{0}", InvasionPopup.Bust) : ("cum_" + text), text, false, true, null, 3.4f);
			InvasionPopup.sceneTimer = Mathf.Max(InvasionPopup.sceneTimer, Random.Range(6f, 10f));
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0003660C File Offset: 0x0003480C
		private static void Triggers(Body b, float dt)
		{
			if (InvasionPopup.wombCooldown > 0f)
			{
				InvasionPopup.wombCooldown -= dt;
			}
			if (InvasionPopup.outsideFrame >= 0 && Time.frameCount > InvasionPopup.outsideFrame)
			{
				if (InvasionPopup.insideFrame != InvasionPopup.outsideFrame)
				{
					InvasionPopup.Cumshot();
				}
				InvasionPopup.outsideFrame = -1;
			}
			bool monsterScene = InvasionPopup.MonsterScene;
			if (monsterScene && !InvasionPopup.wasScene)
			{
				InvasionPopup.sceneTimer = 1.5f;
			}
			InvasionPopup.wasScene = monsterScene;
			if (monsterScene && (InvasionPopup.sceneTimer -= dt) <= 0f && InvasionPopup.cur == null)
			{
				InvasionPopup.sceneTimer = Random.Range(8f, 13f);
				InvasionPopup.MonsterWindow(InvasionPopup.RandomPart());
			}
			bool flag = InvasionPopup.LivingSuit && Suit.Milking;
			if (flag && !InvasionPopup.wasMilking)
			{
				InvasionPopup.Show(InvasionPopup.SuitAnim("breast", true), "breast", Suit.Enjoying, false, null, 4.2f);
			}
			InvasionPopup.wasMilking = flag;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x000366F8 File Offset: 0x000348F8
		public static void Update(Body b)
		{
			PlayerCamera main = PlayerCamera.main;
			if (!main || !b || InvasionPopup.S == null)
			{
				return;
			}
			if (!LewdSettings.On || (InvasionPopup.Enabled != null && !InvasionPopup.Enabled.Value) || !b.alive)
			{
				InvasionPopup.cur = (InvasionPopup.pending = null);
				if (InvasionPopup.root)
				{
					InvasionPopup.root.SetActive(false);
				}
				return;
			}
			InvasionPopup.Triggers(b, Time.deltaTime);
			if (InvasionPopup.cur == null)
			{
				if (InvasionPopup.root)
				{
					InvasionPopup.root.SetActive(false);
				}
				return;
			}
			if (!InvasionPopup.root && !InvasionPopup.Build(main))
			{
				return;
			}
			bool flag = main.woundView && main.woundView.activeSelf;
			InvasionPopup.root.SetActive(!flag);
			float num = Time.time - InvasionPopup.shownAt;
			if (num >= InvasionPopup.cur.Seconds)
			{
				InvasionPopup.Req req = InvasionPopup.pending;
				InvasionPopup.cur = null;
				if (req != null)
				{
					InvasionPopup.Start(req);
				}
				return;
			}
			if (flag)
			{
				return;
			}
			float num2 = Mathf.Clamp01(Mathf.Min(num / 0.25f, (InvasionPopup.cur.Seconds - num) / 0.25f));
			num2 = num2 * num2 * (3f - 2f * num2);
			float num3 = 60f * InvasionPopup.k;
			float num4 = ((RectTransform)InvasionPopup.root.transform.parent).rect.width * 0.012f;
			InvasionPopup.rt.anchoredPosition = new Vector2(-num3 + (num3 + num4) * num2, InvasionPopup.rt.anchoredPosition.y);
			float num5 = 1.3333334f;
			string text = ((InvasionPopup.cur.Then != null && num >= num5) ? InvasionPopup.cur.Then : InvasionPopup.cur.Anim);
			int num6 = (int)(num * 9f);
			num6 = ((InvasionPopup.cur.Once && text == InvasionPopup.cur.Anim) ? Mathf.Min(11, num6) : (num6 % 12));
			Texture2D texture2D = InvasionPopup.Pic(text);
			if (InvasionPopup.art.texture != texture2D)
			{
				InvasionPopup.art.texture = texture2D;
			}
			InvasionPopup.art.uvRect = new Rect((float)num6 / 12f, 0f, 0.083333336f, 1f);
			Texture2D texture2D2 = InvasionPopup.Pic(InvasionPopup.cur.Love ? "frame_love" : "frame");
			if (texture2D2 && InvasionPopup.frameImage.texture != texture2D2)
			{
				InvasionPopup.frameImage.texture = texture2D2;
			}
			string text2 = (InvasionPopup.cur.Love ? (InvasionPopup.hasHeart ? "♥" : "*") : "!");
			InvasionPopup.title.text = string.Concat(new string[]
			{
				text2,
				" ",
				Plugin.T("inv_" + InvasionPopup.cur.Part),
				" ",
				text2
			});
			InvasionPopup.title.color = (InvasionPopup.cur.Love ? new Color32(236, 120, byte.MaxValue, byte.MaxValue) : new Color32(byte.MaxValue, 208, 40, byte.MaxValue));
			InvasionPopup.title.enabled = InvasionPopup.cur.Love || (int)(num * 9f) % 6 < 4;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x00036A84 File Offset: 0x00034C84
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
			InvasionPopup.k = rectTransform.rect.height * 0.4f / 171f;
			InvasionPopup.root = new GameObject("LewdInvasionPopup", new Type[]
			{
				typeof(RectTransform),
				typeof(RawImage)
			});
			InvasionPopup.root.transform.SetParent(componentInParent.transform, false);
			InvasionPopup.rt = (RectTransform)InvasionPopup.root.transform;
			RectTransform rectTransform2 = InvasionPopup.rt;
			RectTransform rectTransform3 = InvasionPopup.rt;
			Vector2 vector;
			vector..ctor(0f, 0.5f);
			rectTransform3.anchorMax = vector;
			rectTransform2.anchorMin = vector;
			InvasionPopup.rt.pivot = new Vector2(0f, 0.5f);
			InvasionPopup.rt.sizeDelta = new Vector2(60f * InvasionPopup.k, 58f * InvasionPopup.k);
			InvasionPopup.rt.anchoredPosition = new Vector2(-60f * InvasionPopup.k, rectTransform.rect.height * 0.06f);
			InvasionPopup.frameImage = InvasionPopup.root.GetComponent<RawImage>();
			InvasionPopup.frameImage.raycastTarget = false;
			InvasionPopup.frameImage.texture = InvasionPopup.Pic("frame");
			GameObject gameObject = new GameObject("LewdInvasionArt", new Type[]
			{
				typeof(RectTransform),
				typeof(RawImage)
			});
			gameObject.transform.SetParent(InvasionPopup.rt, false);
			RectTransform rectTransform4 = (RectTransform)gameObject.transform;
			Vector2 vector2;
			vector2..ctor(0f, 1f);
			rectTransform4.pivot = vector2;
			vector = (rectTransform4.anchorMax = vector2);
			rectTransform4.anchorMin = vector;
			rectTransform4.anchoredPosition = new Vector2(4f * InvasionPopup.k, -13f * InvasionPopup.k);
			rectTransform4.sizeDelta = new Vector2(52f * InvasionPopup.k, 42f * InvasionPopup.k);
			InvasionPopup.art = gameObject.GetComponent<RawImage>();
			InvasionPopup.art.raycastTarget = false;
			GameObject gameObject2 = new GameObject("LewdInvasionTitle", new Type[] { typeof(RectTransform) });
			gameObject2.transform.SetParent(InvasionPopup.rt, false);
			RectTransform rectTransform5 = (RectTransform)gameObject2.transform;
			vector2..ctor(0f, 1f);
			rectTransform5.pivot = vector2;
			vector = (rectTransform5.anchorMax = vector2);
			rectTransform5.anchorMin = vector;
			rectTransform5.anchoredPosition = new Vector2(3f * InvasionPopup.k, -3f * InvasionPopup.k);
			rectTransform5.sizeDelta = new Vector2(54f * InvasionPopup.k, 8f * InvasionPopup.k);
			InvasionPopup.title = gameObject2.AddComponent<TextMeshProUGUI>();
			TMP_FontAsset tmp_FontAsset = (from x in pc.woundView.GetComponentsInChildren<TextMeshProUGUI>(true)
				select x.font).FirstOrDefault<TMP_FontAsset>((TMP_FontAsset f) => f);
			if (tmp_FontAsset)
			{
				InvasionPopup.title.font = tmp_FontAsset;
			}
			InvasionPopup.hasHeart = tmp_FontAsset && tmp_FontAsset.HasCharacter('♥', true, false);
			InvasionPopup.title.fontSize = 6.5f * InvasionPopup.k;
			InvasionPopup.title.alignment = 514;
			InvasionPopup.title.enableWordWrapping = false;
			InvasionPopup.title.overflowMode = 0;
			InvasionPopup.title.raycastTarget = false;
			InvasionPopup.root.transform.SetAsLastSibling();
			Plugin.Log.LogInfo(string.Format("Invasion window built ({0:0}x{1:0})", 60f * InvasionPopup.k, 58f * InvasionPopup.k));
			return true;
		}

		// Token: 0x0600034F RID: 847 RVA: 0x00036E83 File Offset: 0x00035083
		public static void DebugShow(string anim, string part, bool love)
		{
			InvasionPopup.Show(anim, part, love, false, null, 3.6f);
		}

		// Token: 0x040002E4 RID: 740
		private const int FW = 60;

		// Token: 0x040002E5 RID: 741
		private const int FH = 58;

		// Token: 0x040002E6 RID: 742
		private const int Title = 10;

		// Token: 0x040002E7 RID: 743
		private const int ArtW = 52;

		// Token: 0x040002E8 RID: 744
		private const int ArtH = 42;

		// Token: 0x040002E9 RID: 745
		private const int Frames = 12;

		// Token: 0x040002EA RID: 746
		private const float Fps = 9f;

		// Token: 0x040002EB RID: 747
		private const float Slide = 0.25f;

		// Token: 0x040002EC RID: 748
		private const float Seconds = 3.6f;

		// Token: 0x040002ED RID: 749
		public static ConfigEntry<bool> Enabled;

		// Token: 0x040002EE RID: 750
		private static string dir;

		// Token: 0x040002EF RID: 751
		private static readonly Dictionary<string, Texture2D> pics = new Dictionary<string, Texture2D>();

		// Token: 0x040002F0 RID: 752
		private static readonly HashSet<string> missing = new HashSet<string>();

		// Token: 0x040002F1 RID: 753
		private static GameObject root;

		// Token: 0x040002F2 RID: 754
		private static RectTransform rt;

		// Token: 0x040002F3 RID: 755
		private static RawImage frameImage;

		// Token: 0x040002F4 RID: 756
		private static RawImage art;

		// Token: 0x040002F5 RID: 757
		private static TextMeshProUGUI title;

		// Token: 0x040002F6 RID: 758
		private static float k;

		// Token: 0x040002F7 RID: 759
		private static bool hasHeart;

		// Token: 0x040002F8 RID: 760
		private static InvasionPopup.Req cur;

		// Token: 0x040002F9 RID: 761
		private static InvasionPopup.Req pending;

		// Token: 0x040002FA RID: 762
		private static float shownAt;

		// Token: 0x040002FB RID: 763
		private static bool wasScene;

		// Token: 0x040002FC RID: 764
		private static bool wasMilking;

		// Token: 0x040002FD RID: 765
		private static float sceneTimer;

		// Token: 0x040002FE RID: 766
		private static float wombCooldown;

		// Token: 0x040002FF RID: 767
		public static bool Quiet;

		// Token: 0x04000300 RID: 768
		private static int insideFrame = -1;

		// Token: 0x04000301 RID: 769
		private static int outsideFrame = -1;

		// Token: 0x020001C2 RID: 450
		private class Req
		{
			// Token: 0x040009A0 RID: 2464
			public string Anim;

			// Token: 0x040009A1 RID: 2465
			public string Then;

			// Token: 0x040009A2 RID: 2466
			public string Part;

			// Token: 0x040009A3 RID: 2467
			public bool Love;

			// Token: 0x040009A4 RID: 2468
			public bool Once;

			// Token: 0x040009A5 RID: 2469
			public float Seconds;
		}
	}
}
