using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200011A RID: 282
	internal static class TraderSuit
	{
		// Token: 0x060006A9 RID: 1705 RVA: 0x000695CC File Offset: 0x000677CC
		public static string Variant()
		{
			if (Suit.Stage == 0)
			{
				return null;
			}
			if (Suit.Latex)
			{
				if (Plugin.S.latexCorruption < 50f)
				{
					return "latex";
				}
				return "latex_deep";
			}
			else
			{
				if (Suit.LatexPartial)
				{
					return "latex";
				}
				if (Suit.HostSlime)
				{
					if (Suit.HostTamed)
					{
						return "host_tamed";
					}
					if (Plugin.S.hostStableLeft > 0f)
					{
						return "host_calm";
					}
					if (Suit.Stage < 3)
					{
						return "host_12";
					}
					return "host_34";
				}
				else if (Suit.Slime)
				{
					if (Suit.Stage >= 5)
					{
						return "slime_bond";
					}
					if (Suit.Stage < 3)
					{
						return "slime_12";
					}
					return "slime_34";
				}
				else if (Suit.Symbiote)
				{
					if (Suit.Stage >= 5)
					{
						return "symb_bond";
					}
					if (Suit.Stage < 3)
					{
						return "symb_12";
					}
					return "symb_34";
				}
				else
				{
					if (Puppet.Active)
					{
						return "cloth_puppet";
					}
					if (Suit.Tamed)
					{
						return "cloth_tamed";
					}
					return "cloth_s" + Mathf.Clamp(Suit.Stage, 1, 4).ToString();
				}
			}
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x000696DC File Offset: 0x000678DC
		private static float RepPenalty(string v)
		{
			if (v != null)
			{
				switch (v.Length)
				{
				case 5:
					if (!(v == "latex"))
					{
						goto IL_0118;
					}
					break;
				case 6:
				case 11:
					goto IL_0118;
				case 7:
				{
					char c = v[0];
					if (c != 'h')
					{
						if (c != 's')
						{
							goto IL_0118;
						}
						if (!(v == "symb_34"))
						{
							goto IL_0118;
						}
						goto IL_0112;
					}
					else
					{
						if (v == "host_34")
						{
							return 25f;
						}
						if (!(v == "host_12"))
						{
							goto IL_0118;
						}
					}
					break;
				}
				case 8:
				{
					char c = v[0];
					if (c != 'c')
					{
						if (c != 's')
						{
							goto IL_0118;
						}
						if (!(v == "slime_34"))
						{
							goto IL_0118;
						}
						goto IL_0112;
					}
					else
					{
						if (!(v == "cloth_s3") && !(v == "cloth_s4"))
						{
							goto IL_0118;
						}
						goto IL_0112;
					}
					break;
				}
				case 9:
					if (!(v == "host_calm"))
					{
						goto IL_0118;
					}
					break;
				case 10:
					if (!(v == "latex_deep"))
					{
						goto IL_0118;
					}
					break;
				case 12:
					if (!(v == "cloth_puppet"))
					{
						goto IL_0118;
					}
					goto IL_0112;
				default:
					goto IL_0118;
				}
				return 10f;
				IL_0112:
				return 10f;
			}
			IL_0118:
			return 0f;
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x00069808 File Offset: 0x00067A08
		public static void Meet(TraderScript t)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (!t || !body || !LewdSettings.ParasitesOn)
			{
				return;
			}
			string text = TraderSuit.Variant();
			if (text == null)
			{
				return;
			}
			float num = TraderSuit.RepPenalty(text);
			t.reputation -= num;
			Plugin.Log.LogInfo(string.Format("A trader sees her suit ({0}): reputation -{1:0} (now {2:0})", text, num, t.reputation));
			t.StartCoroutine(TraderSuit.Talk(t, body, text));
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x0006989B File Offset: 0x00067A9B
		private static IEnumerator Talk(TraderScript t, Body b, string v)
		{
			yield return new WaitForSeconds(2.5f);
			int i = 0;
			Func<bool> <>9__1;
			while (i < 3 && t && t.talker)
			{
				Func<bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = () => !t || !t.talker || t.talker.FinishedTalking());
				}
				yield return new WaitUntil(func);
				yield return new WaitForSeconds(0.6f);
				int num = i;
				i = num + 1;
			}
			if (!t || !t.talker || !b)
			{
				yield break;
			}
			string text = TraderSuit.Pick(string.Format("tsuit_{0}_t{1}", v, Mathf.Clamp(t.character, 0, 2)));
			if (text == null)
			{
				yield break;
			}
			t.talker.Talk(text, null, false, false);
			Plugin.Log.LogInfo(string.Format("Trader {0} about her suit: \"{1}\"", t.character, text));
			yield return new WaitUntil(() => !t || !t.talker || t.talker.FinishedTalking());
			yield return new WaitForSeconds(0.4f);
			if (!b || !b.alive)
			{
				yield break;
			}
			string text2 = "tsuit_" + v + "_her";
			if (Suit.Enjoying && TraderSuit.Has(text2 + "love"))
			{
				text2 += "love";
			}
			Relief.Say(b, text2);
			if (Suit.Latex && Random.value < 0.6f)
			{
				SymbioteVoice.Say(b, "latexv_trader", 2.2f);
			}
			yield break;
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x000698B8 File Offset: 0x00067AB8
		public static void Bind(ConfigFile c)
		{
			TraderSuit.RemovalPrice = c.Bind<int>("TraderSuit", "RemovalPrice", 30, "What a trader asks to take any living suit off her (the latex, the symbiote, a bonded slime, one she likes): it always works, the same price whatever he thinks of her. 0: traders do not offer it.");
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x000698D8 File Offset: 0x00067AD8
		public static void RegisterItem()
		{
			ItemInfo itemInfo;
			if (Item.GlobalItems == null || Item.GlobalItems.ContainsKey("lewd_suitoff") || !Item.GlobalItems.TryGetValue("flimsyknife", out itemInfo))
			{
				return;
			}
			ItemInfo itemInfo2 = (ItemInfo)AccessTools.Method(typeof(object), "MemberwiseClone", null, null).Invoke(itemInfo, null);
			itemInfo2.value = Mathf.Max(1, TraderSuit.RemovalPrice.Value);
			itemInfo2.weight = 0f;
			itemInfo2.fullName = Plugin.T("tsuitoff_name");
			itemInfo2.description = Plugin.T("tsuitoff_desc");
			Item.GlobalItems["lewd_suitoff"] = itemInfo2;
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060006AF RID: 1711 RVA: 0x00069985 File Offset: 0x00067B85
		private static bool Offered
		{
			get
			{
				return TraderSuit.RemovalPrice != null && TraderSuit.RemovalPrice.Value > 0 && LewdSettings.ParasitesOn;
			}
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x000699A2 File Offset: 0x00067BA2
		public static void Offer(TraderScript t)
		{
			if (!t || Item.GlobalItems == null || !Item.GlobalItems.ContainsKey("lewd_suitoff"))
			{
				return;
			}
			t.StartCoroutine(TraderSuit.OfferWhenReady(t));
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x000699D2 File Offset: 0x00067BD2
		private static IEnumerator OfferWhenReady(TraderScript t)
		{
			float w = 0f;
			while (t && t.items == null && w < 5f)
			{
				yield return null;
				w += Time.deltaTime;
			}
			if (!t)
			{
				yield break;
			}
			if (t.items == null)
			{
				t.items = new List<TraderItem>();
			}
			TraderSuit.Sync(t);
			yield break;
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x000699E4 File Offset: 0x00067BE4
		public static void Sync(TraderScript t)
		{
			if (!t || t.items == null || Item.GlobalItems == null || !Item.GlobalItems.ContainsKey("lewd_suitoff"))
			{
				return;
			}
			bool flag = TraderSuit.Offered && Suit.Stage > 0;
			bool flag2 = t.items.Any<TraderItem>((TraderItem x) => x.id == "lewd_suitoff");
			if (flag == flag2)
			{
				return;
			}
			if (!flag)
			{
				t.items.RemoveAll((TraderItem x) => x.id == "lewd_suitoff");
				return;
			}
			t.items.Insert(0, new TraderItem
			{
				id = "lewd_suitoff",
				preference = 0,
				value = TraderSuit.RemovalPrice.Value
			});
			t.items = t.items.OrderBy<TraderItem, int>((TraderItem x) => x.preference).ToList<TraderItem>();
			Plugin.Log.LogInfo(string.Format("A trader offers to take her suit off ({0})", TraderSuit.RemovalPrice.Value));
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x00069B18 File Offset: 0x00067D18
		public static bool Purchase(TraderScript t, TraderItem item)
		{
			BuildingEntity component = t.GetComponent<BuildingEntity>();
			if (component && component.health < 200f)
			{
				return true;
			}
			int num = Mathf.Max(0, TraderSuit.RemovalPrice.Value);
			if (t.valueGiven < num)
			{
				return false;
			}
			t.items.Remove(item);
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (!body || Suit.Stage == 0)
			{
				string text = TraderSuit.Pick(string.Format("tsuitoff_none_t{0}", Mathf.Clamp(t.character, 0, 2)));
				if (t.talker && text != null)
				{
					t.talker.Talk(text, null, false, false);
				}
				return true;
			}
			t.valueGiven -= num;
			t.reputation += 4f;
			PlayerCamera.main.PlayUISound(0, 1f);
			t.StartCoroutine(TraderSuit.TakeOff(t, body, Suit.Latex || Suit.LatexPartial, Suit.Key));
			return true;
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x00069C2B File Offset: 0x00067E2B
		private static IEnumerator TakeOff(TraderScript t, Body b, bool latex, string key)
		{
			int num = Mathf.Clamp(t.character, 0, 2);
			string text = TraderSuit.Pick(string.Format("tsuitoff_{0}_t{1}", latex ? "latex" : "any", num)) ?? TraderSuit.Pick(string.Format("tsuitoff_any_t{0}", num));
			if (t.talker && text != null)
			{
				t.talker.Talk(text, null, false, false);
			}
			if (latex)
			{
				SymbioteVoice.Say(b, "latexv_traderoff", 0.6f);
			}
			else if (key == "symb")
			{
				SymbioteVoice.Say(b, "symv_traderoff", 0.6f);
			}
			yield return new WaitForSeconds(1.6f);
			if (!b || Suit.Stage == 0)
			{
				yield break;
			}
			bool loved = Suit.RemoveByTrader(b);
			yield return new WaitForSeconds(0.8f);
			if (b && b.alive)
			{
				Relief.Say(b, loved ? "tsuitoff_herlove" : "tsuitoff_her");
			}
			yield break;
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x00069C50 File Offset: 0x00067E50
		public static void WatchRelief(Body b)
		{
			if (!b)
			{
				return;
			}
			float num = Plugin.ReliefTraderRange.Value + 2f;
			foreach (TraderScript traderScript in Object.FindObjectsOfType<TraderScript>())
			{
				if (traderScript && !traderScript.hostile && traderScript.talker && Vector2.Distance(traderScript.transform.position, b.transform.position) <= num)
				{
					BuildingEntity component = traderScript.GetComponent<BuildingEntity>();
					if (!component || component.health > 0f)
					{
						traderScript.StartCoroutine(TraderSuit.Watch(traderScript, b));
					}
				}
			}
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x00069D01 File Offset: 0x00067F01
		private static IEnumerator Watch(TraderScript t, Body b)
		{
			yield return new WaitForSeconds(Random.Range(2f, 4f));
			if (!t || !b || !Relief.Active)
			{
				yield break;
			}
			float num = Mathf.Max(0f, Plugin.TraderWatchRep.Value);
			float num2 = Random.Range(-num, num);
			t.reputation += num2;
			string text = TraderSuit.Pick(string.Format("trelief_{0}_t{1}", (num2 >= 0f) ? "good" : "bad", Mathf.Clamp(t.character, 0, 2)));
			if (text != null)
			{
				t.talker.Talk(text, null, false, false);
			}
			Plugin.Log.LogInfo(string.Format("Trader {0} watches her touch herself: reputation {1}{2:0} (now {3:0})", new object[]
			{
				t.character,
				(num2 >= 0f) ? "+" : "",
				num2,
				t.reputation
			}));
			yield break;
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x00069D17 File Offset: 0x00067F17
		private static bool Has(string key)
		{
			return Plugin.Text.ContainsKey(key) || Plugin.Text.ContainsKey(key + "1");
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x00069D40 File Offset: 0x00067F40
		private static string Pick(string key)
		{
			List<string> list = new List<string>();
			if (Plugin.Text.ContainsKey(key))
			{
				list.Add(key);
			}
			for (int i = 1; i < 10; i++)
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

		// Token: 0x0400070E RID: 1806
		internal const string ServiceId = "lewd_suitoff";

		// Token: 0x0400070F RID: 1807
		internal const string ServiceBase = "flimsyknife";

		// Token: 0x04000710 RID: 1808
		internal static ConfigEntry<int> RemovalPrice;
	}
}
