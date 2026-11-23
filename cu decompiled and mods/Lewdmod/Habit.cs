using System;

namespace LewdStatus
{
	// Token: 0x020000C7 RID: 199
	internal static class Habit
	{
		// Token: 0x06000469 RID: 1129 RVA: 0x00048122 File Offset: 0x00046322
		public static bool Paused(Body b)
		{
			return !b || b.sleeping || !b.conscious;
		}
	}
}
