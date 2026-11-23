using System;
using System.Collections.Generic;
using System.Linq;
using LewdStatus;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000014 RID: 20
	internal static class AmmoLines
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00004CC4 File Offset: 0x00002EC4
		private static bool Her(Body b)
		{
			return b && b.alive && b.conscious && b.talker && PlayerCamera.main && PlayerCamera.main.body == b && Plugin.GunFindLines != null && Plugin.GunFindLines.Value;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00004D27 File Offset: 0x00002F27
		private static void Say(Body b, string key, bool urgent = false)
		{
			if (!AmmoLines.Her(b) || Time.time - AmmoLines.lastLine < (urgent ? 1.5f : 5f))
			{
				return;
			}
			AmmoLines.lastLine = Time.time;
			Relief.Say(b, key);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00004D5F File Offset: 0x00002F5F
		public static void Swap(Body b)
		{
			if (Random.value < 0.5f)
			{
				AmmoLines.Say(b, "ammo_swap", false);
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004D79 File Offset: 0x00002F79
		public static void Loaded(Body b, GunScript g, bool more)
		{
			AmmoLines.clicked.Remove(g.GetInstanceID());
			AmmoLines.warned.Remove(g.GetInstanceID());
			if (!more)
			{
				AmmoLines.Say(b, "ammo_last", false);
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004DAC File Offset: 0x00002FAC
		public static void Nothing(Body b)
		{
			AmmoLines.Say(b, "ammo_none", true);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00004DBC File Offset: 0x00002FBC
		public static void Shot(Body b, GunScript g)
		{
			if (g.feedType != null || !g.hasMag || AmmoInfo.MagSize(g) < 10)
			{
				return;
			}
			if (g.roundsInMag > 3)
			{
				AmmoLines.warned.Remove(g.GetInstanceID());
				return;
			}
			if (g.roundsInMag > 0 && AmmoLines.warned.Add(g.GetInstanceID()))
			{
				AmmoLines.Say(b, "ammo_low", false);
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004E26 File Offset: 0x00003026
		public static void Click(Body b, GunScript g)
		{
			if (g.roundsInMag > 0)
			{
				return;
			}
			if (AmmoLines.clicked.Add(g.GetInstanceID()))
			{
				AmmoLines.Say(b, "ammo_empty", true);
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00004E50 File Offset: 0x00003050
		public static void Picked(Body b, Item item)
		{
			AmmoScript ammoScript;
			if (!AmmoLines.Her(b) || !item || !item.TryGetComponent<AmmoScript>(ref ammoScript) || !b.HoldingItem(b.handSlot))
			{
				return;
			}
			GunScript g = (b.GetItem(b.handSlot) ? b.GetItem(b.handSlot).GetComponent<GunScript>() : null);
			if (!g || ammoScript.ammoType != g.ammoType || ammoScript.rounds <= 0 || !item.transform.IsChildOf(b.transform))
			{
				return;
			}
			if (!b.GetAllItemsThorough().Any<Item>(delegate(Item i)
			{
				AmmoScript ammoScript2;
				return i && i != item && i.TryGetComponent<AmmoScript>(ref ammoScript2) && ammoScript2.ammoType == g.ammoType && ammoScript2.rounds > 0;
			}) && g.roundsInMag == 0)
			{
				AmmoLines.Say(b, "ammo_found", false);
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004F3E File Offset: 0x0000313E
		public static Body Holder(GunScript g)
		{
			if (!g)
			{
				return null;
			}
			return g.GetComponentInParent<Body>();
		}

		// Token: 0x04000033 RID: 51
		private static float lastLine = -99f;

		// Token: 0x04000034 RID: 52
		private static readonly HashSet<int> clicked = new HashSet<int>();

		// Token: 0x04000035 RID: 53
		private static readonly HashSet<int> warned = new HashSet<int>();
	}
}
