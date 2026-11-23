using System;
using ScavArmory;

namespace LewdStatus
{
	// Token: 0x02000084 RID: 132
	internal static class Scenes
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000267 RID: 615 RVA: 0x0002CAC0 File Offset: 0x0002ACC0
		public static bool Held
		{
			get
			{
				return Slits.Holding || Slits.Inside || Latexes.Holding || Latexes.Inside || LatexPond.Busy || Flowers.Holding || Flesh.Holding || FleshWall.Holding || Pits.InMaw || Pits.InThroat || Tentacles.Holding || Worm.Inside || Webs.Caught || SlimeBog.Stuck || Capture.Active || LatexMites.Busy;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000268 RID: 616 RVA: 0x0002CB3D File Offset: 0x0002AD3D
		public static bool Busy
		{
			get
			{
				return Scenes.Held || Womb.InLabor || Heart.KnockedOut;
			}
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0002CB54 File Offset: 0x0002AD54
		public static float FeetY(Body b)
		{
			float num = float.MaxValue;
			foreach (Limb limb in b.limbs)
			{
				if (limb && !limb.dismembered && limb.transform.position.y < num)
				{
					num = limb.transform.position.y;
				}
			}
			if (num != 3.4028235E+38f)
			{
				return num;
			}
			return b.transform.position.y;
		}
	}
}
