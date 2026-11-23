using System;
using System.Collections.Generic;
using LewdStatus;
using TMPro;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000011 RID: 17
	internal static class AmmoInfo
	{
		// Token: 0x06000032 RID: 50 RVA: 0x000046B8 File Offset: 0x000028B8
		internal static int MagSize(GunScript g)
		{
			if (g.feedType == 1)
			{
				return g.magCapacity;
			}
			int num;
			if (!AmmoInfo.magSize.TryGetValue(g.ammoType, out num))
			{
				GameObject gameObject = Resources.Load(AmmoScript.AmmoTypeToMagazine(g.ammoType)) as GameObject;
				AmmoScript ammoScript;
				num = ((gameObject && gameObject.TryGetComponent<AmmoScript>(ref ammoScript) && ammoScript.maxRounds > 0) ? ammoScript.maxRounds : 0);
				if (num > 0)
				{
					AmmoInfo.magSize[g.ammoType] = num;
				}
			}
			if (num <= 0)
			{
				return g.magCapacity;
			}
			return num;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004748 File Offset: 0x00002948
		private static string Chamber(GunScript g)
		{
			bool english = Plugin.English;
			GunScript.RoundInChamber roundInChamber = g.roundInChamber;
			if (roundInChamber != null)
			{
				if (roundInChamber != 1)
				{
					if (!english)
					{
						return "пусто";
					}
					return Plugin.Zh("empty");
				}
				else
				{
					if (!english)
					{
						return "гильза";
					}
					return Plugin.Zh("a spent casing");
				}
			}
			else
			{
				if (!english)
				{
					return "патрон";
				}
				return Plugin.Zh("a round");
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000047A8 File Offset: 0x000029A8
		public static string GunLine(GunScript g)
		{
			bool english = Plugin.English;
			string text;
			if (g.feedType == 1)
			{
				text = (english ? Plugin.Zh("Loaded: ") : "Заряжено: ") + g.roundsInMag.ToString() + "/" + g.magCapacity.ToString();
			}
			else if (g.hasMag)
			{
				text = (english ? Plugin.Zh("Magazine: ") : "Магазин: ") + g.roundsInMag.ToString() + "/" + AmmoInfo.MagSize(g).ToString();
			}
			else
			{
				text = (english ? Plugin.Zh("No magazine") : "Магазин не вставлен");
			}
			return string.Concat(new string[]
			{
				"<color=#ff8fb0><sprite index=8 tint=1>",
				text,
				english ? Plugin.Zh(" · chamber: ") : " · в патроннике: ",
				AmmoInfo.Chamber(g),
				"\n"
			});
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00004894 File Offset: 0x00002A94
		public static void Describe(Item item, ref string description)
		{
			if (!item || !item.Stats.rec.recognizable || description == null)
			{
				return;
			}
			string text = null;
			GunScript gunScript;
			AmmoScript ammoScript;
			if (item.TryGetComponent<GunScript>(ref gunScript))
			{
				text = AmmoInfo.GunLine(gunScript);
			}
			else if (item.TryGetComponent<AmmoScript>(ref ammoScript) && ammoScript.itemType == 1 && !description.Contains("<color=#ff8fb0><sprite index=8 tint=1>"))
			{
				text = string.Concat(new string[]
				{
					"<color=#ff8fb0><sprite index=8 tint=1>",
					ammoScript.rounds.ToString(),
					"/",
					ammoScript.maxRounds.ToString(),
					" ",
					Locale.GetOther("magazinerounds"),
					"\n"
				});
			}
			if (text == null)
			{
				return;
			}
			int num = description.IndexOf("<color=#ffffff><sprite index=0 tint=1>");
			description = ((num >= 0) ? description.Insert(num, text) : (description + "\n" + text));
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000497C File Offset: 0x00002B7C
		public static void Hud(PlayerCamera pc)
		{
			Body body = pc.body;
			GunScript gunScript = ((body && body.HoldingItem(body.handSlot)) ? body.GetItem(body.handSlot).GetComponent<GunScript>() : null);
			if (!Plugin.AmmoCounter.Value || !gunScript || !pc.gunMenu || !pc.gunMenu.activeInHierarchy || WorldGeneration.unchipped)
			{
				if (AmmoInfo.counter)
				{
					AmmoInfo.counter.enabled = false;
				}
				return;
			}
			if (!AmmoInfo.counter && !AmmoInfo.Make(pc))
			{
				return;
			}
			bool flag = gunScript.feedType == 1 || gunScript.hasMag;
			string text = (flag ? (gunScript.roundsInMag.ToString() + "/" + ((gunScript.feedType == 1) ? gunScript.magCapacity : AmmoInfo.MagSize(gunScript)).ToString()) : "--");
			if (gunScript.roundInChamber == null)
			{
				text += " +1";
			}
			AmmoInfo.counter.text = text;
			AmmoInfo.counter.color = (((flag && gunScript.roundsInMag > 0) || gunScript.roundInChamber == null) ? new Color(1f, 0.56f, 0.69f) : new Color(1f, 0.35f, 0.35f));
			AmmoInfo.counter.enabled = true;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00004AE8 File Offset: 0x00002CE8
		private static bool Make(PlayerCamera pc)
		{
			if (!pc.gunBulletImage)
			{
				return false;
			}
			RectTransform rectTransform = (RectTransform)pc.gunBulletImage.transform;
			GameObject gameObject = new GameObject("ArmoryAmmoCounter", new Type[] { typeof(RectTransform) });
			gameObject.transform.SetParent(rectTransform, false);
			RectTransform rectTransform2 = (RectTransform)gameObject.transform;
			Vector2 vector;
			vector..ctor(1f, 0.5f);
			rectTransform2.anchorMax = vector;
			rectTransform2.anchorMin = vector;
			rectTransform2.pivot = new Vector2(0f, 0.5f);
			float num = Mathf.Max(8f, rectTransform.rect.height);
			rectTransform2.anchoredPosition = new Vector2(num * 0.25f, 0f);
			rectTransform2.sizeDelta = new Vector2(num * 4f, num);
			AmmoInfo.counter = gameObject.AddComponent<TextMeshProUGUI>();
			if (pc.holdingText)
			{
				AmmoInfo.counter.font = pc.holdingText.font;
			}
			AmmoInfo.counter.fontSize = num * 0.7f;
			AmmoInfo.counter.alignment = 4097;
			AmmoInfo.counter.enableWordWrapping = false;
			AmmoInfo.counter.overflowMode = 0;
			AmmoInfo.counter.raycastTarget = false;
			Plugin.Log.LogInfo(string.Format("Ammo counter: up by the chamber icon ({0:0}x{1:0})", rectTransform.rect.width, rectTransform.rect.height));
			return true;
		}

		// Token: 0x0400002F RID: 47
		private const string Pink = "<color=#ff8fb0><sprite index=8 tint=1>";

		// Token: 0x04000030 RID: 48
		private const string WeightLine = "<color=#ffffff><sprite index=0 tint=1>";

		// Token: 0x04000031 RID: 49
		private static readonly Dictionary<GunScript.AmmoType, int> magSize = new Dictionary<GunScript.AmmoType, int>();

		// Token: 0x04000032 RID: 50
		private static TextMeshProUGUI counter;
	}
}
