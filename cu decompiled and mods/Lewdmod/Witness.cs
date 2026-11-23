using System;
using System.Runtime.CompilerServices;
using ScavArmory;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000130 RID: 304
	internal static class Witness
	{
		// Token: 0x06000738 RID: 1848 RVA: 0x0006FB91 File Offset: 0x0006DD91
		public static void Update(Body b)
		{
			if (!b)
			{
				return;
			}
			float averagePain = b.averagePain;
			if (averagePain - Witness.lastPain > 2f)
			{
				Witness.lastHurt = Time.time;
			}
			Witness.lastPain = averagePain;
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000739 RID: 1849 RVA: 0x0006FBBF File Offset: 0x0006DDBF
		public static bool Shameless
		{
			get
			{
				return Plugin.S.corruption >= Plugin.ShamelessCorruption.Value || Plugin.S.lust >= Plugin.ShamelessLust.Value;
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x0600073A RID: 1850 RVA: 0x0006FBF2 File Offset: 0x0006DDF2
		public static bool RecentlyHurt
		{
			get
			{
				return Time.time - Witness.lastHurt < Plugin.ReliefHurtSeconds.Value;
			}
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x0006FC0C File Offset: 0x0006DE0C
		public static string Why(Body b, bool compelled)
		{
			if (Time.time - Witness.scanAt < 0.5f)
			{
				return Witness.scanned;
			}
			Witness.scanAt = Time.time;
			try
			{
				Witness.scanned = Witness.Scan(b, compelled);
			}
			catch (Exception ex)
			{
				Witness.scanned = null;
				Plugin.Log.LogWarning("Witness scan failed: " + ex.Message);
			}
			return Witness.scanned;
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x0006FC84 File Offset: 0x0006DE84
		private static string Scan(Body b, bool compelled)
		{
			if (Witness.RecentlyHurt)
			{
				return "relief_attacked";
			}
			Witness.<>c__DisplayClass10_0 CS$<>8__locals1;
			CS$<>8__locals1.at = b.limbs[1].transform.position;
			float value = Plugin.ReliefEnemyRange.Value;
			float num = value * value;
			foreach (Scavenger scavenger in Scavengers.All)
			{
				if (scavenger && scavenger.mode != Scavenger.Mode.Dead && Witness.<Scan>g__Near|10_0(scavenger.pos, num, ref CS$<>8__locals1))
				{
					return "relief_enemy";
				}
			}
			foreach (ParasiteAI parasiteAI in Parasites.Alive)
			{
				if (parasiteAI && Witness.<Scan>g__Near|10_0(parasiteAI.transform.position, num, ref CS$<>8__locals1))
				{
					return "relief_enemy";
				}
			}
			if (Worm.Hunting)
			{
				return "relief_enemy";
			}
			if (Witness.Hostile<SpiderHandler>(CS$<>8__locals1.at, num) || Witness.Hostile<CrystalEnemy>(CS$<>8__locals1.at, num) || Witness.Hostile<DroneScript>(CS$<>8__locals1.at, num) || Witness.Hostile<TurretScript>(CS$<>8__locals1.at, num))
			{
				return "relief_enemy";
			}
			float value2 = Plugin.ReliefTraderRange.Value;
			foreach (TraderScript traderScript in Object.FindObjectsOfType<TraderScript>())
			{
				if (Witness.Living(traderScript) && Witness.<Scan>g__Near|10_0(traderScript.transform.position, traderScript.hostile ? num : (value2 * value2), ref CS$<>8__locals1))
				{
					if (traderScript.hostile)
					{
						return "relief_enemy";
					}
					if (!compelled && !Witness.Shameless)
					{
						return "relief_trader";
					}
				}
			}
			return null;
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x0006FE70 File Offset: 0x0006E070
		private static bool Living(Component c)
		{
			BuildingEntity component = c.GetComponent<BuildingEntity>();
			return !component || component.health > 0f;
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x0006FE9C File Offset: 0x0006E09C
		private static bool Hostile<T>(Vector2 at, float sq) where T : Component
		{
			foreach (T t in Object.FindObjectsOfType<T>())
			{
				if (Witness.Living(t) && (t.transform.position - at).sqrMagnitude < sq)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x0006FF14 File Offset: 0x0006E114
		[CompilerGenerated]
		internal static bool <Scan>g__Near|10_0(Vector2 p, float sq, ref Witness.<>c__DisplayClass10_0 A_2)
		{
			return (p - A_2.at).sqrMagnitude < sq;
		}

		// Token: 0x04000771 RID: 1905
		private static float lastHurt = -100f;

		// Token: 0x04000772 RID: 1906
		private static float lastPain;

		// Token: 0x04000773 RID: 1907
		private static float scanAt = -100f;

		// Token: 0x04000774 RID: 1908
		private static string scanned;
	}
}
