using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000129 RID: 297
	internal static class SlimeWall
	{
		// Token: 0x060006ED RID: 1773 RVA: 0x0006C058 File Offset: 0x0006A258
		public static bool Touching(Body b)
		{
			Vector2 vector = b.transform.position;
			foreach (LewdWall lewdWall in Walls.All)
			{
				float num;
				if (lewdWall && lewdWall.Kind == WallKind.Slime && lewdWall.Near(vector, 1.1f, out num))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x0006C0E0 File Offset: 0x0006A2E0
		public unsafe static void Apply(Body b)
		{
			if (!LewdSettings.TrapsOn || !PlayerCamera.main || PlayerCamera.main.body != b || !SlimeWall.Touching(b))
			{
				return;
			}
			bool flag = *SlimeWall.slidL.Invoke(b) || *SlimeWall.slidR.Invoke(b);
			*SlimeWall.slidL.Invoke(b) = false;
			*SlimeWall.slidR.Invoke(b) = false;
			*SlimeWall.sinceL.Invoke(b) = 1f;
			*SlimeWall.sinceR.Invoke(b) = 1f;
			b.wetness = Mathf.Min(100f, b.wetness + 2f * Time.deltaTime);
			Burning.Douse(b);
			if (flag && !b.grounded && Time.time - SlimeWall.lastLine > 15f)
			{
				SlimeWall.lastLine = Time.time;
				Relief.Say(b, "swall_slip");
				Sound.Play("goo", b.transform.position, false, true, b.transform, 0.5f, 0.8f, false, false);
			}
		}

		// Token: 0x0400073B RID: 1851
		private static readonly AccessTools.FieldRef<Body, bool> slidL = AccessTools.FieldRefAccess<Body, bool>("slidingLeft");

		// Token: 0x0400073C RID: 1852
		private static readonly AccessTools.FieldRef<Body, bool> slidR = AccessTools.FieldRefAccess<Body, bool>("slidingRight");

		// Token: 0x0400073D RID: 1853
		private static readonly AccessTools.FieldRef<Body, float> sinceL = AccessTools.FieldRefAccess<Body, float>("timeSinceSlidLeft");

		// Token: 0x0400073E RID: 1854
		private static readonly AccessTools.FieldRef<Body, float> sinceR = AccessTools.FieldRefAccess<Body, float>("timeSinceSlidRight");

		// Token: 0x0400073F RID: 1855
		private static float lastLine = -99f;
	}
}
