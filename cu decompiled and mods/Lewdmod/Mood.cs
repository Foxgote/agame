using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000B8 RID: 184
	internal static class Mood
	{
		// Token: 0x06000430 RID: 1072 RVA: 0x00044168 File Offset: 0x00042368
		public static void Change(Body b, float delta)
		{
			if (!b || delta == 0f)
			{
				return;
			}
			delta *= ((delta > 0f) ? Plugin.MoodGainScale.Value : Plugin.MoodLossScale.Value);
			b.happiness = Mathf.Clamp(b.happiness + delta, -100f, 100f);
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x000441C5 File Offset: 0x000423C5
		public static void Consumed(Body b, float loss)
		{
			if (!b || loss <= 0f)
			{
				return;
			}
			b.happiness = Mathf.Clamp(b.happiness - loss * Plugin.ConsumedMoodScale.Value, -100f, 100f);
		}
	}
}
