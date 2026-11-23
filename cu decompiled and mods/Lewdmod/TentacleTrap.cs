using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000118 RID: 280
	internal class TentacleTrap : MonoBehaviour
	{
		// Token: 0x06000681 RID: 1665 RVA: 0x0006729C File Offset: 0x0006549C
		public bool RockGone()
		{
			WorldGeneration w = WorldGeneration.world;
			if (!w || this.Holds.Count == 0)
			{
				return false;
			}
			if ((this.holdCheck -= Time.deltaTime) > 0f)
			{
				return false;
			}
			this.holdCheck = 0.5f;
			int num = this.Holds.Count<Vector2Int>((Vector2Int c) => w.GetBlock(c) == 0);
			return (this.Holds.Contains(this.RootHold) && w.GetBlock(this.RootHold) == 0) || num >= Mathf.Min(2, this.Holds.Count);
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x00067356 File Offset: 0x00065556
		public Vector2 Anchor(int i)
		{
			return base.transform.position + new Vector2(((float)i - 3f) * 0.32f, -0.25f);
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x00067388 File Offset: 0x00065588
		private void Update()
		{
			if (this.sated > 0f)
			{
				this.sated -= Time.deltaTime;
			}
			if ((this.frameTime -= Time.deltaTime) <= 0f && TentacleArt.Root.Length != 0)
			{
				this.frameTime = (Tentacles.HolderIs(this) ? 0.25f : 0.7f);
				this.frame = (this.frame + 1) % TentacleArt.Root.Length;
				this.root.sprite = TentacleArt.Root[this.frame];
			}
			if (!Tentacles.HolderIs(this))
			{
				Tentacles.DrawIdle(this);
			}
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x00067430 File Offset: 0x00065630
		public void AnimalHit(float damage)
		{
			FleshBurn component = base.GetComponent<FleshBurn>();
			if (component)
			{
				Burning.Hit(component, damage);
				return;
			}
			Tentacles.OnHit(this, damage);
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x0006745B File Offset: 0x0006565B
		public void AnimalDeath()
		{
			Tentacles.OnHit(this, 0f);
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x00067468 File Offset: 0x00065668
		private void OnDestroy()
		{
			foreach (SpriteRenderer[] array in this.chains)
			{
				foreach (SpriteRenderer spriteRenderer in array)
				{
					if (spriteRenderer)
					{
						Object.Destroy(spriteRenderer.gameObject);
					}
				}
			}
			foreach (SpriteRenderer spriteRenderer2 in this.tips)
			{
				if (spriteRenderer2)
				{
					Object.Destroy(spriteRenderer2.gameObject);
				}
			}
			Tentacles.Traps.Remove(this);
		}

		// Token: 0x040006ED RID: 1773
		public const int Count = 7;

		// Token: 0x040006EE RID: 1774
		public float sated;

		// Token: 0x040006EF RID: 1775
		public SpriteRenderer root;

		// Token: 0x040006F0 RID: 1776
		public readonly List<SpriteRenderer[]> chains = new List<SpriteRenderer[]>();

		// Token: 0x040006F1 RID: 1777
		public readonly List<SpriteRenderer> tips = new List<SpriteRenderer>();

		// Token: 0x040006F2 RID: 1778
		public float[] idleLen = new float[7];

		// Token: 0x040006F3 RID: 1779
		private float frameTime;

		// Token: 0x040006F4 RID: 1780
		private float holdCheck;

		// Token: 0x040006F5 RID: 1781
		private int frame;

		// Token: 0x040006F6 RID: 1782
		public readonly List<Vector2Int> Holds = new List<Vector2Int>();

		// Token: 0x040006F7 RID: 1783
		public Vector2Int RootHold = new Vector2Int(-1, -1);

		// Token: 0x040006F8 RID: 1784
		public const float Toughness = 100000f;
	}
}
