using System;
using HarmonyLib;

namespace ScavArmory
{
	// Token: 0x02000024 RID: 36
	[HarmonyPatch(typeof(GunScript), "LoadMag")]
	internal static class BoxNotMagPatch
	{
		// Token: 0x0600006B RID: 107 RVA: 0x000076CD File Offset: 0x000058CD
		private static bool Prefix(AmmoScript ammo)
		{
			return !Armory.IsBox(ammo ? ammo.GetComponent<Item>() : null);
		}
	}
}
