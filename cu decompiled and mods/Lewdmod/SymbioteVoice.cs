using System;
using System.Collections.Generic;
using ScavArmory;
using TMPro;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000116 RID: 278
	internal static class SymbioteVoice
	{
		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x00066559 File Offset: 0x00064759
		private static bool Hive
		{
			get
			{
				return Suit.HostSlime;
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x0600066D RID: 1645 RVA: 0x00066560 File Offset: 0x00064760
		private static bool Latex
		{
			get
			{
				return Suit.Latex;
			}
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x0600066E RID: 1646 RVA: 0x00066567 File Offset: 0x00064767
		private static Color Col
		{
			get
			{
				if (!SymbioteVoice.Hive)
				{
					return SymbioteVoice.Violet;
				}
				return SymbioteVoice.Acid;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x0600066F RID: 1647 RVA: 0x0006657B File Offset: 0x0006477B
		private static string V
		{
			get
			{
				if (SymbioteVoice.Latex)
				{
					return "latexv";
				}
				if (!SymbioteVoice.Hive)
				{
					return "symv";
				}
				return "hivev";
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000670 RID: 1648 RVA: 0x0006659C File Offset: 0x0006479C
		private static string R
		{
			get
			{
				if (SymbioteVoice.Latex)
				{
					return "latexr";
				}
				if (!SymbioteVoice.Hive)
				{
					return "symr";
				}
				return "hiver";
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000671 RID: 1649 RVA: 0x000665BD File Offset: 0x000647BD
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x000665C4 File Offset: 0x000647C4
		public static void Say(Body b, string key, float delay = 0f)
		{
			if (!b)
			{
				return;
			}
			if (delay > 0f)
			{
				SymbioteVoice.pendingKey = key;
				SymbioteVoice.pendingAt = Time.time + delay;
				return;
			}
			string text = SymbioteVoice.Pick(key);
			if (text == null || !SymbioteVoice.Ensure(b))
			{
				return;
			}
			SymbioteVoice.line = text;
			SymbioteVoice.typed = 0f;
			SymbioteVoice.text.text = "";
			SymbioteVoice.text.color = SymbioteVoice.Col;
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x00066638 File Offset: 0x00064838
		private static string Pick(string key)
		{
			List<string> list = new List<string>();
			if (Plugin.Text.ContainsKey(key))
			{
				list.Add(key);
			}
			for (int i = 1; i < 20; i++)
			{
				if (Plugin.Text.ContainsKey(key + i.ToString()))
				{
					list.Add(key + i.ToString());
				}
			}
			if (list.Count != 0)
			{
				return Plugin.T(list[Random.Range(0, list.Count)]);
			}
			return null;
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x000666BC File Offset: 0x000648BC
		private static bool Ensure(Body b)
		{
			if (SymbioteVoice.text && SymbioteVoice.owner == b)
			{
				return true;
			}
			if (SymbioteVoice.text)
			{
				Object.Destroy(SymbioteVoice.text.gameObject);
			}
			SymbioteVoice.owner = b;
			SymbioteVoice.hers = (b.talker ? b.talker.text : null);
			if (!SymbioteVoice.hers)
			{
				return false;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(SymbioteVoice.hers.gameObject, SymbioteVoice.hers.transform.parent);
			gameObject.name = "LewdSymbioteVoice";
			foreach (MonoBehaviour monoBehaviour in gameObject.GetComponentsInChildren<MonoBehaviour>(true))
			{
				if (!(monoBehaviour is TMP_Text) && !(monoBehaviour is TMP_SubMesh))
				{
					Object.Destroy(monoBehaviour);
				}
			}
			SymbioteVoice.text = gameObject.GetComponent<TextMeshPro>();
			SymbioteVoice.lineH = SymbioteVoice.hers.GetPreferredValues("Яy").y * SymbioteVoice.hers.transform.localScale.y;
			SymbioteVoice.Place();
			SymbioteVoice.text.color = SymbioteVoice.Violet;
			SymbioteVoice.text.fontStyle |= 2;
			SymbioteVoice.text.text = "";
			return true;
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x000667FC File Offset: 0x000649FC
		private static void Place()
		{
			if (!SymbioteVoice.text || !SymbioteVoice.hers)
			{
				return;
			}
			float num = (string.IsNullOrEmpty(SymbioteVoice.hers.text) ? 0f : (SymbioteVoice.hers.GetPreferredValues(SymbioteVoice.hers.text).y * SymbioteVoice.hers.transform.localScale.y + SymbioteVoice.lineH * 0.25f));
			SymbioteVoice.text.transform.localPosition = SymbioteVoice.hers.transform.localPosition + Vector3.up * num;
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x000668A4 File Offset: 0x00064AA4
		private static string Topic(Body b)
		{
			if (SymbioteVoice.situationCooldown <= 0f)
			{
				if (Scenes.Held)
				{
					return "held";
				}
				if (b.averagePain > 25f || b.limbs[1].bleedAmount > 1f)
				{
					return "hurt";
				}
				if (SymbioteVoice.S.arousal > 70f)
				{
					return "lewd";
				}
				if (SymbioteVoice.Hive && SymbioteVoice.HostsNear(b))
				{
					return "hosts";
				}
			}
			int stage = Suit.Stage;
			if (SymbioteVoice.Latex)
			{
				if (Random.value >= 0.6f)
				{
					return "t2";
				}
				return "t3";
			}
			else if (SymbioteVoice.Hive)
			{
				if (stage < 4)
				{
					if (stage != 3)
					{
						return "t1";
					}
					return "t2";
				}
				else
				{
					if (Random.value >= 0.8f)
					{
						return "t2";
					}
					return "t3";
				}
			}
			else if (stage >= 5)
			{
				if (Random.value >= 0.75f)
				{
					return "t2";
				}
				return "t3";
			}
			else
			{
				if (stage != 4)
				{
					return "t1";
				}
				return "t2";
			}
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x000669A0 File Offset: 0x00064BA0
		private static bool HostsNear(Body b)
		{
			foreach (Scavenger scavenger in Scavengers.All)
			{
				if (scavenger && scavenger.slimeHost && scavenger.mode != Scavenger.Mode.Dead && Vector2.Distance(scavenger.pos, b.transform.position) < 18f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x00066A30 File Offset: 0x00064C30
		private static List<string> Letters(string topic)
		{
			string text = SymbioteVoice.V + "_" + topic;
			List<string> list;
			if (SymbioteVoice.letters.TryGetValue(text, out list))
			{
				return list;
			}
			list = new List<string>();
			for (char c = 'a'; c <= 'z'; c += '\u0001')
			{
				if (Plugin.Text.ContainsKey(string.Format("{0}_{1}", text, c)))
				{
					list.Add(c.ToString());
				}
			}
			return SymbioteVoice.letters[text] = list;
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x00066AB0 File Offset: 0x00064CB0
		private static void Converse(Body b)
		{
			string text = SymbioteVoice.Topic(b);
			List<string> list = SymbioteVoice.Letters(text);
			if (list.Count == 0)
			{
				return;
			}
			string text2 = list[Random.Range(0, list.Count)];
			if (text + text2 == SymbioteVoice.lastTopic && list.Count > 1)
			{
				text2 = list[(list.IndexOf(text2) + 1) % list.Count];
			}
			SymbioteVoice.lastTopic = text + text2;
			SymbioteVoice.Say(b, string.Concat(new string[]
			{
				SymbioteVoice.V,
				"_",
				text,
				"_",
				text2
			}), 0f);
			if (Plugin.Text.ContainsKey(string.Concat(new string[]
			{
				SymbioteVoice.R,
				"_",
				text,
				"_",
				text2
			})))
			{
				SymbioteVoice.replyKey = string.Concat(new string[]
				{
					SymbioteVoice.R,
					"_",
					text,
					"_",
					text2
				});
				SymbioteVoice.replyAt = Time.time + (float)SymbioteVoice.line.Length / 16f + 1.3f;
			}
			if (text == "held" || text == "hurt" || text == "lewd" || text == "hosts")
			{
				SymbioteVoice.situationCooldown = 45f;
			}
			Plugin.Log.LogInfo(string.Concat(new string[]
			{
				"The ",
				SymbioteVoice.Latex ? "living latex" : (SymbioteVoice.Hive ? "hive in the hosts' slime" : "symbiote"),
				" talks to her (",
				text,
				" ",
				text2,
				")"
			}));
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x00066C84 File Offset: 0x00064E84
		public static void Update(Body b, float dt)
		{
			if (SymbioteVoice.text)
			{
				if (SymbioteVoice.line.Length > 0)
				{
					SymbioteVoice.typed += dt * 16f;
					int num = Mathf.Clamp((int)SymbioteVoice.typed, 0, SymbioteVoice.line.Length);
					SymbioteVoice.text.text = SymbioteVoice.line.Substring(0, num);
					SymbioteVoice.Place();
					float num2 = SymbioteVoice.typed / 16f - (float)SymbioteVoice.line.Length / 16f;
					Color col = SymbioteVoice.Col;
					col.a = ((num2 > 3.2f) ? Mathf.Clamp01(1f - (num2 - 3.2f) / 0.6f) : 1f);
					SymbioteVoice.text.color = col;
					if (num2 > 3.8000002f)
					{
						SymbioteVoice.line = "";
						SymbioteVoice.text.text = "";
					}
				}
				if (!b || !b.alive || !b.conscious)
				{
					SymbioteVoice.line = "";
					SymbioteVoice.text.text = "";
				}
			}
			if (SymbioteVoice.pendingKey != null && Time.time >= SymbioteVoice.pendingAt)
			{
				string text = SymbioteVoice.pendingKey;
				SymbioteVoice.pendingKey = null;
				SymbioteVoice.Say(b, text, 0f);
			}
			if (SymbioteVoice.replyKey != null && Time.time >= SymbioteVoice.replyAt)
			{
				Relief.Say(b, SymbioteVoice.replyKey);
				SymbioteVoice.replyKey = null;
			}
			if (((!Suit.Symbiote || Suit.Stage < 3) && (!SymbioteVoice.Hive || Suit.Stage < 1 || HostConversion.Active || Suit.HostCalm) && !SymbioteVoice.Latex) || !b || !b.alive || !b.conscious)
			{
				return;
			}
			if (SymbioteVoice.situationCooldown > 0f)
			{
				SymbioteVoice.situationCooldown -= dt;
			}
			SymbioteVoice.talkTimer -= dt;
			if (SymbioteVoice.situationCooldown <= 0f && (Scenes.Held || b.averagePain > 25f) && SymbioteVoice.talkTimer > 8f)
			{
				SymbioteVoice.talkTimer = 8f;
			}
			if (SymbioteVoice.talkTimer > 0f || SymbioteVoice.line.Length > 0 || SymbioteVoice.replyKey != null || (b.talker && !b.talker.FinishedTalking()))
			{
				return;
			}
			if (SymbioteVoice.Latex)
			{
				SymbioteVoice.talkTimer = Random.Range(35f, 70f);
			}
			else if (SymbioteVoice.Hive)
			{
				SymbioteVoice.talkTimer = ((Suit.Stage >= 4) ? Random.Range(30f, 55f) : ((Suit.Stage == 3) ? Random.Range(55f, 95f) : Random.Range(80f, 140f)));
			}
			else
			{
				SymbioteVoice.talkTimer = ((Suit.Stage >= 5) ? Random.Range(70f, 130f) : ((Suit.Stage == 4) ? Random.Range(90f, 160f) : Random.Range(120f, 200f)));
			}
			SymbioteVoice.Converse(b);
		}

		// Token: 0x0600067B RID: 1659 RVA: 0x00066F91 File Offset: 0x00065191
		public static void DebugTalk(Body b)
		{
			SymbioteVoice.Converse(b);
		}

		// Token: 0x040006D5 RID: 1749
		private const float CharsPerSecond = 16f;

		// Token: 0x040006D6 RID: 1750
		private const float Hold = 3.2f;

		// Token: 0x040006D7 RID: 1751
		private static readonly Color Violet = new Color32(196, 140, byte.MaxValue, byte.MaxValue);

		// Token: 0x040006D8 RID: 1752
		private static readonly Color Acid = new Color32(150, byte.MaxValue, 70, byte.MaxValue);

		// Token: 0x040006D9 RID: 1753
		private static TextMeshPro text;

		// Token: 0x040006DA RID: 1754
		private static TextMeshPro hers;

		// Token: 0x040006DB RID: 1755
		private static Body owner;

		// Token: 0x040006DC RID: 1756
		private static float lineH;

		// Token: 0x040006DD RID: 1757
		private static string line = "";

		// Token: 0x040006DE RID: 1758
		private static float typed;

		// Token: 0x040006DF RID: 1759
		private static float pendingAt;

		// Token: 0x040006E0 RID: 1760
		private static string pendingKey;

		// Token: 0x040006E1 RID: 1761
		private static float talkTimer = 90f;

		// Token: 0x040006E2 RID: 1762
		private static float situationCooldown;

		// Token: 0x040006E3 RID: 1763
		private static string replyKey;

		// Token: 0x040006E4 RID: 1764
		private static float replyAt;

		// Token: 0x040006E5 RID: 1765
		private static readonly Dictionary<string, List<string>> letters = new Dictionary<string, List<string>>();

		// Token: 0x040006E6 RID: 1766
		private static string lastTopic;
	}
}
