using System;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x0200001D RID: 29
	internal class ArmDef
	{
		// Token: 0x0400007A RID: 122
		public string Id;

		// Token: 0x0400007B RID: 123
		public string Base;

		// Token: 0x0400007C RID: 124
		public Kind Kind;

		// Token: 0x0400007D RID: 125
		public string NameRu;

		// Token: 0x0400007E RID: 126
		public string NameEn;

		// Token: 0x0400007F RID: 127
		public string DescRu;

		// Token: 0x04000080 RID: 128
		public string DescEn;

		// Token: 0x04000081 RID: 129
		public float Damage = 1f;

		// Token: 0x04000082 RID: 130
		public float Kick = 1f;

		// Token: 0x04000083 RID: 131
		public float Loud = 1f;

		// Token: 0x04000084 RID: 132
		public float Spread = 1f;

		// Token: 0x04000085 RID: 133
		public float Gas = 1f;

		// Token: 0x04000086 RID: 134
		public float Wear = 1f;

		// Token: 0x04000087 RID: 135
		public float Weight = -1f;

		// Token: 0x04000088 RID: 136
		public int Value = -1;

		// Token: 0x04000089 RID: 137
		public int Caliber = -1;

		// Token: 0x0400008A RID: 138
		public GunScript.FiringMode? Mode;

		// Token: 0x0400008B RID: 139
		public GunScript.FeedType? Feed;

		// Token: 0x0400008C RID: 140
		public int Capacity = -1;

		// Token: 0x0400008D RID: 141
		public int Rounds = -1;

		// Token: 0x0400008E RID: 142
		public float Rarity = 1f;

		// Token: 0x0400008F RID: 143
		public bool Variant;

		// Token: 0x04000090 RID: 144
		public GameObject Template;
	}
}
