using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000029 RID: 41
	internal class Camp
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00007A5D File Offset: 0x00005C5D
		public BuildingEntity Build
		{
			get
			{
				if (!this.Trader)
				{
					return null;
				}
				return this.Trader.GetComponent<BuildingEntity>();
			}
		}

		// Token: 0x04000097 RID: 151
		public Vector2 Center;

		// Token: 0x04000098 RID: 152
		public Vector2 TraderFeet;

		// Token: 0x04000099 RID: 153
		public TraderScript Trader;

		// Token: 0x0400009A RID: 154
		public Scavenger Guard;

		// Token: 0x0400009B RID: 155
		public Scavenger Keeper;

		// Token: 0x0400009C RID: 156
		public Item Fire;

		// Token: 0x0400009D RID: 157
		public GameObject Tent;

		// Token: 0x0400009E RID: 158
		public GameObject Look;

		// Token: 0x0400009F RID: 159
		public ScavRig Rig;

		// Token: 0x040000A0 RID: 160
		public Vector3 LookOffset;

		// Token: 0x040000A1 RID: 161
		public bool Betrayed;

		// Token: 0x040000A2 RID: 162
		public bool Dead;

		// Token: 0x040000A3 RID: 163
		public float Blink;

		// Token: 0x040000A4 RID: 164
		public float NextLine;

		// Token: 0x040000A5 RID: 165
		public float LastHealth;

		// Token: 0x040000A6 RID: 166
		public float HurtByOther;

		// Token: 0x040000A7 RID: 167
		public int LookKind;

		// Token: 0x040000A8 RID: 168
		public readonly List<GameObject> Props = new List<GameObject>();

		// Token: 0x040000A9 RID: 169
		public float ClothesAt;

		// Token: 0x040000AA RID: 170
		public float NakedFor;

		// Token: 0x040000AB RID: 171
		public float HealAt;
	}
}
