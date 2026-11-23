using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LewdStatus;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000036 RID: 54
	internal static class GunControls
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x0000DDD8 File Offset: 0x0000BFD8
		public static KeyCode Key
		{
			get
			{
				KeyCode value = Plugin.ReloadKey.Value;
				KeyCode keyCode = (GunControls.Taken(value) ? Plugin.ReloadKeyFallback.Value : value);
				if (GunControls.Taken(keyCode))
				{
					keyCode = 0;
				}
				if (keyCode != GunControls.announced)
				{
					GunControls.announced = keyCode;
					Plugin.Log.LogInfo((keyCode == value) ? string.Format("Reload key: {0}", keyCode) : string.Format("Reload key: {0} ({1} is used by the game)", keyCode, value));
				}
				return keyCode;
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000DE58 File Offset: 0x0000C058
		private static bool Taken(KeyCode k)
		{
			return k == null || GunControls.GameBinds.Any<string>((string n) => KeyBinds.GetBind(n) == k);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000DE94 File Offset: 0x0000C094
		private static void PlaceMenu(PlayerCamera pc)
		{
			if (!pc.gunMenu || !Plugin.GunMenuAtBottom.Value)
			{
				return;
			}
			RectTransform rectTransform = pc.gunMenu.transform as RectTransform;
			if (!rectTransform)
			{
				return;
			}
			float value = Plugin.GunMenuBottomOffset.Value;
			if (rectTransform == GunControls.movedMenu && Mathf.Approximately(value, GunControls.lastBottom))
			{
				return;
			}
			Vector2 size = rectTransform.rect.size;
			RectTransform rectTransform2 = rectTransform;
			RectTransform rectTransform3 = rectTransform;
			Vector2 vector;
			vector..ctor(0.5f, 0f);
			rectTransform3.anchorMax = vector;
			rectTransform2.anchorMin = vector;
			rectTransform.pivot = new Vector2(0.5f, 0f);
			rectTransform.sizeDelta = size;
			rectTransform.anchoredPosition = new Vector2(0f, value);
			GunControls.movedMenu = rectTransform;
			GunControls.lastBottom = value;
			Plugin.Log.LogInfo(string.Format("Gun menu moved to the bottom of the screen ({0:0}x{1:0}, {2:0} up)", size.x, size.y, value));
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000DF94 File Offset: 0x0000C194
		public static void Update(PlayerCamera pc)
		{
			GunControls.PlaceMenu(pc);
			Body body = pc.body;
			if (!body || !body.alive || !body.conscious || !body.HoldingItem(body.handSlot))
			{
				return;
			}
			GunScript component = body.GetItem(body.handSlot).GetComponent<GunScript>();
			if (!component || GunControls.running != null)
			{
				return;
			}
			if (Keys.Down(GunControls.Key))
			{
				GunControls.running = pc.StartCoroutine(GunControls.Reload(body, component));
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x0000E018 File Offset: 0x0000C218
		public static string Start(PlayerCamera pc)
		{
			Body body = (pc ? pc.body : null);
			if (!body || !body.HoldingItem(body.handSlot))
			{
				return "nothing in her hand";
			}
			GunScript component = body.GetItem(body.handSlot).GetComponent<GunScript>();
			if (!component)
			{
				return "not a gun";
			}
			if (GunControls.running != null)
			{
				return "already reloading";
			}
			GunControls.running = pc.StartCoroutine(GunControls.Reload(body, component));
			return string.Format("reload started ({0}, mag {1} {2}, animated {3})", new object[]
			{
				component.feedType,
				component.hasMag,
				component.roundsInMag,
				ReloadAnim.CanPlay(body)
			});
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000E0DA File Offset: 0x0000C2DA
		private static IEnumerator Reload(Body b, GunScript g)
		{
			try
			{
				if (g.feedType == null && ReloadAnim.CanPlay(b))
				{
					AmmoScript best = GunControls.Best(b, g);
					if (best == null || (g.hasMag && best.rounds <= g.roundsInMag))
					{
						if (best == null)
						{
							AmmoLines.Nothing(b);
						}
						GunControls.Deny();
						yield break;
					}
					AmmoLines.Swap(b);
					ReloadAnim.Begin(b, g);
					yield return new WaitForSeconds(0.25f);
					if (!GunControls.Still(b, g))
					{
						yield break;
					}
					if (g.hasMag)
					{
						g.UnloadMag();
						ReloadAnim.Hold(g, new bool?(true));
					}
					yield return new WaitForSeconds(0.3f);
					if (!GunControls.Still(b, g))
					{
						yield break;
					}
					best = GunControls.Best(b, g);
					if (best == null)
					{
						AmmoLines.Nothing(b);
						GunControls.Deny();
						yield break;
					}
					ReloadAnim.Hold(g, new bool?(false));
					yield return new WaitForSeconds(0.3f);
					if (!GunControls.Still(b, g) || !best)
					{
						yield break;
					}
					GunControls.Take(best);
					g.LoadMag(best);
					AmmoLines.Loaded(b, g, GunControls.Mags(b, g).Any<AmmoScript>());
					ReloadAnim.Hold(g, null);
					yield return new WaitForSeconds(0.14999998f);
					if (GunControls.Still(b, g) && g.roundInChamber == 2)
					{
						g.TryRack();
					}
					yield return new WaitForSeconds(0.25f);
					best = null;
				}
				else if (g.feedType == null)
				{
					AmmoScript ammoScript = GunControls.Best(b, g);
					if (g.hasMag)
					{
						if (ammoScript == null || ammoScript.rounds <= g.roundsInMag)
						{
							if (ammoScript == null)
							{
								AmmoLines.Nothing(b);
							}
							GunControls.Deny();
							yield break;
						}
						AmmoLines.Swap(b);
						g.UnloadMag();
						yield return new WaitForSeconds(0.45f);
						if (!GunControls.Still(b, g))
						{
							yield break;
						}
						ammoScript = GunControls.Best(b, g);
					}
					if (ammoScript == null)
					{
						AmmoLines.Nothing(b);
						GunControls.Deny();
						yield break;
					}
					GunControls.Take(ammoScript);
					g.LoadMag(ammoScript);
					AmmoLines.Loaded(b, g, GunControls.Mags(b, g).Any<AmmoScript>());
					yield return new WaitForSeconds(0.3f);
					if (GunControls.Still(b, g) && g.roundInChamber == 2)
					{
						g.TryRack();
					}
				}
				else
				{
					bool any = false;
					while (GunControls.Still(b, g) && (g.roundsInMag < g.magCapacity || (g.racked && g.roundInChamber == 2)) && !Input.GetKey(KeyBinds.GetBind("attack")) && GunControls.OneRound(b, g))
					{
						any = true;
						yield return new WaitForSeconds(0.35f);
					}
					if (!any)
					{
						AmmoLines.Nothing(b);
						GunControls.Deny();
						yield break;
					}
					AmmoLines.Loaded(b, g, true);
					if (GunControls.Still(b, g) && g.roundInChamber == 2 && g.roundsInMag > 0)
					{
						g.TryRack();
					}
				}
			}
			finally
			{
				GunControls.running = null;
				ReloadAnim.Stop();
			}
			yield break;
			yield break;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000E0F0 File Offset: 0x0000C2F0
		private static bool Still(Body b, GunScript g)
		{
			return g && b && b.alive && b.HoldingItem(b.handSlot) && b.GetItem(b.handSlot) && b.GetItem(b.handSlot).gameObject == g.gameObject;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000E154 File Offset: 0x0000C354
		private static AmmoScript Best(Body b, GunScript g)
		{
			List<AmmoScript> list = GunControls.Mags(b, g).ToList<AmmoScript>();
			Func<AmmoScript, bool> func;
			if ((func = GunControls.<>O.<0>__InPouch) == null)
			{
				func = (GunControls.<>O.<0>__InPouch = new Func<AmmoScript, bool>(GunControls.InPouch));
			}
			AmmoScript ammoScript = (from a in list.Where<AmmoScript>(func)
				orderby a.rounds descending
				select a).FirstOrDefault<AmmoScript>();
			AmmoScript ammoScript2 = list.OrderByDescending<AmmoScript, int>((AmmoScript a) => a.rounds).FirstOrDefault<AmmoScript>();
			if (!ammoScript || (g.hasMag && ammoScript.rounds <= g.roundsInMag))
			{
				return ammoScript2;
			}
			return ammoScript;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000E204 File Offset: 0x0000C404
		private static bool InPouch(AmmoScript a)
		{
			Item item;
			Container container;
			return a && a.TryGetComponent<Item>(ref item) && item.TryGetParentContainer(ref container) && container.GetComponent<Item>() && container.GetComponent<Item>().id.StartsWith("lewd_tac_");
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000E254 File Offset: 0x0000C454
		private static IEnumerable<AmmoScript> Mags(Body b, GunScript g)
		{
			return from i in b.GetAllItemsThorough()
				where i && !Armory.IsBox(i)
				select i.GetComponent<AmmoScript>() into a
				where a && a.ammoType == g.ammoType && a.itemType == 1 && a.rounds > 0
				select a;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000E2D0 File Offset: 0x0000C4D0
		private static bool OneRound(Body b, GunScript g)
		{
			AmmoScript ammoScript = (from i in b.GetAllItemsThorough()
				where i && !Armory.IsBox(i) && i.id != "boxof12gauge"
				select i.GetComponent<AmmoScript>()).FirstOrDefault<AmmoScript>((AmmoScript a) => a && a.ammoType == g.ammoType && a.itemType == 0);
			if (!ammoScript)
			{
				return false;
			}
			GunControls.Take(ammoScript);
			g.LoadMag(ammoScript);
			return true;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000E368 File Offset: 0x0000C568
		private static void Take(AmmoScript a)
		{
			Item item;
			Container container;
			if (a && a.TryGetComponent<Item>(ref item) && item.TryGetParentContainer(ref container))
			{
				container.UnloadItem(item, null);
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000E399 File Offset: 0x0000C599
		private static void Deny()
		{
			if (PlayerCamera.main)
			{
				PlayerCamera.main.PlayUISound(6, 1f);
			}
		}

		// Token: 0x040000EB RID: 235
		private static readonly string[] GameBinds = new string[]
		{
			"jump", "left", "right", "down", "up", "attack", "iteminteract", "throw", "switchhands", "ragdoll",
			"woundview", "craft", "favourite", "toggleinventory", "speed1", "speed2", "speed3", "expanddesc", "altview", "bark",
			"console", "pause"
		};

		// Token: 0x040000EC RID: 236
		private static Coroutine running;

		// Token: 0x040000ED RID: 237
		private static KeyCode announced = 0;

		// Token: 0x040000EE RID: 238
		private static RectTransform movedMenu;

		// Token: 0x040000EF RID: 239
		private static float lastBottom = -1f;

		// Token: 0x02000161 RID: 353
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000852 RID: 2130
			public static Func<AmmoScript, bool> <0>__InPouch;
		}
	}
}
