using System;

namespace ScavArmory
{
	// Token: 0x02000040 RID: 64
	internal class WeaponDef
	{
		// Token: 0x04000105 RID: 261
		public string Id;

		// Token: 0x04000106 RID: 262
		public bool Gun;

		// Token: 0x04000107 RID: 263
		public bool TwoHanded;

		// Token: 0x04000108 RID: 264
		public bool Cutting;

		// Token: 0x04000109 RID: 265
		public float Damage;

		// Token: 0x0400010A RID: 266
		public float Cooldown;

		// Token: 0x0400010B RID: 267
		public float Spread = 3f;

		// Token: 0x0400010C RID: 268
		public float PlayerMult = 0.45f;

		// Token: 0x0400010D RID: 269
		public float Range = 1.8f;

		// Token: 0x0400010E RID: 270
		public int Mag = 10;

		// Token: 0x0400010F RID: 271
		public int Pellets = 1;

		// Token: 0x04000110 RID: 272
		public int Burst = 1;

		// Token: 0x04000111 RID: 273
		public float Rarity = 1f;

		// Token: 0x04000112 RID: 274
		public int MinDepth;
	}
}
