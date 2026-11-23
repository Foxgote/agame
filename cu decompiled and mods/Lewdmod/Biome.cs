using System;

namespace LewdStatus
{
	// Token: 0x02000083 RID: 131
	internal static class Biome
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000262 RID: 610 RVA: 0x0002CA4D File Offset: 0x0002AC4D
		private static WorldGeneration W
		{
			get
			{
				return WorldGeneration.world;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000263 RID: 611 RVA: 0x0002CA54 File Offset: 0x0002AC54
		private static bool Real
		{
			get
			{
				return Biome.W && Biome.W.biomeOverride == 0;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000264 RID: 612 RVA: 0x0002CA71 File Offset: 0x0002AC71
		public static bool Desert
		{
			get
			{
				return Biome.Real && Biome.W.biomeDepth == 2;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000265 RID: 613 RVA: 0x0002CA89 File Offset: 0x0002AC89
		public static bool Jungle
		{
			get
			{
				return Biome.Real && Biome.W.biomeDepth == 4;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000266 RID: 614 RVA: 0x0002CAA1 File Offset: 0x0002ACA1
		public static bool Ending
		{
			get
			{
				return Biome.Real && Biome.W.biomeDepth >= 10;
			}
		}
	}
}
