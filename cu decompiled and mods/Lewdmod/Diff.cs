using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000081 RID: 129
	internal static class Diff
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000259 RID: 601 RVA: 0x0002C7B4 File Offset: 0x0002A9B4
		public static int Level
		{
			get
			{
				if (LewdSettings.Difficulty == null)
				{
					return 1;
				}
				int num = Array.IndexOf<string>(Diff.Choices, LewdSettings.Difficulty.Value.Trim().ToLowerInvariant());
				if (num >= 0)
				{
					return num;
				}
				return 1;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600025A RID: 602 RVA: 0x0002C7F0 File Offset: 0x0002A9F0
		public static float Spawn
		{
			get
			{
				return Diff.spawn[Diff.Level];
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600025B RID: 603 RVA: 0x0002C7FD File Offset: 0x0002A9FD
		public static float Escape
		{
			get
			{
				return Diff.escape[Diff.Level];
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600025C RID: 604 RVA: 0x0002C80A File Offset: 0x0002AA0A
		public static float Lethal
		{
			get
			{
				return Diff.lethal[Diff.Level];
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600025D RID: 605 RVA: 0x0002C817 File Offset: 0x0002AA17
		public static bool LethalScenes
		{
			get
			{
				return LewdSettings.DeadlyScenes == null || LewdSettings.DeadlyScenes.Value;
			}
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0002C82C File Offset: 0x0002AA2C
		public static int Count(float perLayer)
		{
			return Mathf.RoundToInt(perLayer * Diff.Spawn);
		}

		// Token: 0x0400024E RID: 590
		public static readonly string[] Choices = new string[] { "soft", "normal", "hard" };

		// Token: 0x0400024F RID: 591
		private static readonly float[] spawn = new float[] { 0.6f, 1f, 1.4f };

		// Token: 0x04000250 RID: 592
		private static readonly float[] escape = new float[] { 1.5f, 1f, 0.7f };

		// Token: 0x04000251 RID: 593
		private static readonly float[] lethal = new float[] { 0.6f, 1f, 1.3f };
	}
}
