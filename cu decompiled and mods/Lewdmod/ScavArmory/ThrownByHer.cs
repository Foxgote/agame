using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000043 RID: 67
	internal static class ThrownByHer
	{
		// Token: 0x06000156 RID: 342 RVA: 0x00015E82 File Offset: 0x00014082
		public static void Mark(Item it)
		{
			if (!it)
			{
				return;
			}
			if (ThrownByHer.at.Count > 30)
			{
				ThrownByHer.at.Clear();
			}
			ThrownByHer.at[it.GetInstanceID()] = Time.time;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00015EBC File Offset: 0x000140BC
		public static bool Recently(Item it)
		{
			float num;
			return it && ThrownByHer.at.TryGetValue(it.GetInstanceID(), out num) && Time.time - num < 3f;
		}

		// Token: 0x04000176 RID: 374
		private static readonly Dictionary<int, float> at = new Dictionary<int, float>();
	}
}
