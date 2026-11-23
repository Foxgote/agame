using System;
using System.Linq;

namespace LewdStatus
{
	// Token: 0x02000094 RID: 148
	internal static class Blanket
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060002FC RID: 764 RVA: 0x00033B41 File Offset: 0x00031D41
		public static bool Wrapped
		{
			get
			{
				return Blanket.wrapped;
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00033B48 File Offset: 0x00031D48
		public static void Update(Body b, float dt)
		{
			if ((Blanket.scan -= dt) <= 0f)
			{
				Blanket.scan = 1f;
				Blanket.has = b.GetAllItemsThorough().Any<Item>((Item i) => i && i.id == Outfits.Blanket.Id);
			}
			bool flag = Blanket.has && b.sleeping;
			if (flag && !Blanket.wrapped)
			{
				Relief.Say(b, "blanket_sleep");
				Plugin.Log.LogInfo("She wraps herself in the blanket to sleep");
			}
			Blanket.wrapped = flag;
		}

		// Token: 0x040002C5 RID: 709
		private static bool has;

		// Token: 0x040002C6 RID: 710
		private static bool wrapped;

		// Token: 0x040002C7 RID: 711
		private static float scan;
	}
}
