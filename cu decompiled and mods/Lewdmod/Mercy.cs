using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000082 RID: 130
	internal static class Mercy
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000260 RID: 608 RVA: 0x0002C8B0 File Offset: 0x0002AAB0
		private static bool InScene
		{
			get
			{
				return Slits.Holding || Slits.Inside || Flowers.Holding || Flesh.Holding || FleshWall.Holding || Pits.InMaw || Tentacles.Holding || Worm.Inside || Webs.Caught || SlimeBog.Stuck;
			}
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0002C904 File Offset: 0x0002AB04
		public static void Update(Body b, float dt)
		{
			if (Mercy.cooldown > 0f)
			{
				Mercy.cooldown -= dt;
			}
			if (Diff.LethalScenes || !b.alive || !Mercy.InScene || Mercy.cooldown > 0f)
			{
				return;
			}
			bool flag = b.bloodOxygen < 66f || b.fibrillationProgress > 40f;
			int num = 0;
			while (num < Mathf.Min(3, b.limbs.Length) && !flag)
			{
				Limb limb = b.limbs[num];
				if (limb && !limb.dismembered && (limb.skinHealth < 30f || limb.muscleHealth < 30f))
				{
					flag = true;
				}
				num++;
			}
			if (!flag)
			{
				return;
			}
			Mercy.cooldown = 8f;
			RunStats.Muted = true;
			Flesh.LetGo(b);
			FleshWall.LetGo(b);
			Pits.LetGo(b);
			Flowers.LetGo(b);
			Slits.LetGo(b);
			Tentacles.LetGo(b);
			Worm.LetGo(b);
			Webs.LetGo(b);
			SlimeBog.LetGo(b);
			RunStats.Muted = false;
			b.bloodOxygen = Mathf.Max(b.bloodOxygen, 70f);
			b.fibrillationProgress = 0f;
			b.fibrillationForced = false;
			Relief.Say(b, "mercy");
			Plugin.Log.LogInfo("Deadly scenes are off: it let her go before she died");
		}

		// Token: 0x04000252 RID: 594
		private static float cooldown;
	}
}
