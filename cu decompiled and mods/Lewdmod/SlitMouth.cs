using System;
using System.Collections.Generic;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200010C RID: 268
	internal class SlitMouth : MonoBehaviour
	{
		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060005D6 RID: 1494 RVA: 0x0005D795 File Offset: 0x0005B995
		private static WorldGeneration W
		{
			get
			{
				return WorldGeneration.world;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060005D7 RID: 1495 RVA: 0x0005D79C File Offset: 0x0005B99C
		public float FaceX
		{
			get
			{
				return SlitMouth.W.BlockToWorldPos(this.Face).x - (float)this.Dir * 0.5f;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060005D8 RID: 1496 RVA: 0x0005D7C1 File Offset: 0x0005B9C1
		public Vector2 Mouth
		{
			get
			{
				return new Vector2(this.FaceX, SlitMouth.W.BlockToWorldPos(this.Face).y + 0.5f);
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060005D9 RID: 1497 RVA: 0x0005D7E9 File Offset: 0x0005B9E9
		public Vector2 Throat
		{
			get
			{
				return this.Mouth - new Vector2((float)this.Dir * 0.5f, 0f);
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060005DA RID: 1498 RVA: 0x0005D80D File Offset: 0x0005BA0D
		public Vector2 Belly
		{
			get
			{
				return this.Mouth - new Vector2((float)(this.Dir * 6) * 0.5f, 0f);
			}
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x0005D833 File Offset: 0x0005BA33
		public IEnumerable<Vector2Int> Cells()
		{
			int num;
			for (int i = 1; i <= 6; i = num + 1)
			{
				for (int y = 0; y < 2; y = num + 1)
				{
					yield return new Vector2Int(this.Face.x - this.Dir * i, this.Face.y + y);
					num = y;
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x0005D843 File Offset: 0x0005BA43
		public IEnumerable<Vector2Int> Walls()
		{
			int num;
			for (int i = 1; i <= 7; i = num + 1)
			{
				yield return new Vector2Int(this.Face.x - this.Dir * i, this.Face.y - 1);
				yield return new Vector2Int(this.Face.x - this.Dir * i, this.Face.y + 2);
				num = i;
			}
			yield return new Vector2Int(this.Face.x - this.Dir * 7, this.Face.y);
			yield return new Vector2Int(this.Face.x - this.Dir * 7, this.Face.y + 1);
			yield break;
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x0005D854 File Offset: 0x0005BA54
		public bool InStomach(Vector2 at, float margin)
		{
			float num = (this.FaceX - at.x) * (float)this.Dir;
			return num > -margin && num < 6f + margin && Mathf.Abs(at.y - this.Mouth.y) < 1f + margin;
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x0005D8A8 File Offset: 0x0005BAA8
		public void AnimalHit(float damage)
		{
			FleshBurn component = base.GetComponent<FleshBurn>();
			if (component)
			{
				Burning.Hit(component, damage);
				return;
			}
			Slits.OnHit(this, damage);
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x0005D8D4 File Offset: 0x0005BAD4
		public void AnimalDeath()
		{
			BuildingEntity component = base.GetComponent<BuildingEntity>();
			if (component)
			{
				component.health = 100000f;
			}
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x0005D8FC File Offset: 0x0005BAFC
		private void OnDestroy()
		{
			Slits.All.Remove(this);
			if (this.segs != null)
			{
				foreach (SpriteRenderer spriteRenderer in this.segs)
				{
					if (spriteRenderer)
					{
						Object.Destroy(spriteRenderer.gameObject);
					}
				}
			}
			if (this.tip)
			{
				Object.Destroy(this.tip.gameObject);
			}
			if (this.stomach)
			{
				Object.Destroy(this.stomach.gameObject);
			}
		}

		// Token: 0x04000671 RID: 1649
		public Vector2Int Face;

		// Token: 0x04000672 RID: 1650
		public int Dir = 1;

		// Token: 0x04000673 RID: 1651
		public SpriteRenderer lips;

		// Token: 0x04000674 RID: 1652
		public SpriteRenderer stomach;

		// Token: 0x04000675 RID: 1653
		public SpriteRenderer[] segs;

		// Token: 0x04000676 RID: 1654
		public SpriteRenderer tip;

		// Token: 0x04000677 RID: 1655
		public CircleCollider2D[] cols;

		// Token: 0x04000678 RID: 1656
		public float sated;

		// Token: 0x04000679 RID: 1657
		public float flinch;

		// Token: 0x0400067A RID: 1658
		public int hits;

		// Token: 0x0400067B RID: 1659
		public float lastHit = -100f;

		// Token: 0x0400067C RID: 1660
		public bool dead;

		// Token: 0x0400067D RID: 1661
		public bool open;

		// Token: 0x0400067E RID: 1662
		public Dictionary<Vector2Int, ushort> ground;

		// Token: 0x0400067F RID: 1663
		public readonly List<Vector2Int> rim = new List<Vector2Int>();

		// Token: 0x04000680 RID: 1664
		public Vector2 tipAt;

		// Token: 0x04000681 RID: 1665
		public float seed;

		// Token: 0x04000682 RID: 1666
		public bool flood;

		// Token: 0x04000683 RID: 1667
		public bool mouthShut;

		// Token: 0x04000684 RID: 1668
		public const int Len = 6;
	}
}
