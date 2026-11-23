using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000F3 RID: 243
	internal static class SecondChance
	{
		// Token: 0x06000567 RID: 1383 RVA: 0x000584B8 File Offset: 0x000566B8
		public static void Take(Body b)
		{
			State s = Plugin.S;
			Heart.ReleaseAll(b, "her second chance");
			if (Puppet.Active)
			{
				Puppet.Release(b, false);
			}
			HostConversion.Abort(b);
			LatexTakeover.Spare(b);
			s.exhaustion = 0f;
			float num = s.corruptionPeak * Mathf.Clamp01(Plugin.CorruptionFloorShare.Value);
			s.corruption = Mathf.Max(num, s.corruption - Plugin.SecondChanceCorruption.Value);
			s.submission = Mathf.Max(0f, s.submission - Plugin.SecondChanceSubmission.Value);
			s.arousal = Mathf.Min(s.arousal, 30f);
			Relief.Say(b, "laststand_free");
			Plugin.Log.LogInfo(string.Format("Her second chance: let go of, exhaustion 0, corruption {0:0}, submission {1:0}", s.corruption, s.submission));
		}
	}
}
