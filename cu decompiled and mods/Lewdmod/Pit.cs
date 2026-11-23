using System;
using System.Collections.Generic;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000D7 RID: 215
	internal class Pit
	{
		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060004A4 RID: 1188 RVA: 0x0004ADD2 File Offset: 0x00048FD2
		public Vector2 Mouth
		{
			get
			{
				return WorldGeneration.world.BlockToWorldPos(this.Top);
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060004A5 RID: 1189 RVA: 0x0004ADE4 File Offset: 0x00048FE4
		public float Width
		{
			get
			{
				return (float)(this.Half * 2 + 1);
			}
		}

		// Token: 0x0400047C RID: 1148
		public PitKind Kind;

		// Token: 0x0400047D RID: 1149
		public Vector2Int Top;

		// Token: 0x0400047E RID: 1150
		public int Half = 2;

		// Token: 0x0400047F RID: 1151
		public int Depth = 10;

		// Token: 0x04000480 RID: 1152
		public bool Open;

		// Token: 0x04000481 RID: 1153
		public bool Sealed;

		// Token: 0x04000482 RID: 1154
		public float SealedAt = -100f;

		// Token: 0x04000483 RID: 1155
		public bool Checked;

		// Token: 0x04000484 RID: 1156
		public bool Dead;

		// Token: 0x04000485 RID: 1157
		public ushort Cover;

		// Token: 0x04000486 RID: 1158
		public float Closed;

		// Token: 0x04000487 RID: 1159
		public float Sated;

		// Token: 0x04000488 RID: 1160
		public readonly List<GameObject> Props = new List<GameObject>();

		// Token: 0x04000489 RID: 1161
		public SpriteRenderer LipL;

		// Token: 0x0400048A RID: 1162
		public SpriteRenderer LipR;

		// Token: 0x0400048B RID: 1163
		public SpriteRenderer Gullet;

		// Token: 0x0400048C RID: 1164
		public bool Ravish;

		// Token: 0x0400048D RID: 1165
		public readonly List<Vector2Int> RimCells = new List<Vector2Int>();

		// Token: 0x0400048E RID: 1166
		public float RegrowTimer;

		// Token: 0x0400048F RID: 1167
		public float SpatAt = -100f;

		// Token: 0x04000490 RID: 1168
		public bool Spitting;

		// Token: 0x04000491 RID: 1169
		public float Burnt;

		// Token: 0x04000492 RID: 1170
		public float BurnCooldown;

		// Token: 0x04000493 RID: 1171
		public FlowerPlant Plant;

		// Token: 0x04000494 RID: 1172
		public bool Milk;

		// Token: 0x04000495 RID: 1173
		public Dictionary<Vector2Int, ushort> Ground;
	}
}
