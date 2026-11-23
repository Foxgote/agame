using System;
using System.Collections.Generic;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000A9 RID: 169
	internal class LatexBlob : MonoBehaviour
	{
		// Token: 0x0600035B RID: 859 RVA: 0x000371E8 File Offset: 0x000353E8
		public void AnimalHit(float damage)
		{
			FleshBurn component = base.GetComponent<FleshBurn>();
			if (component)
			{
				Burning.Hit(component, damage);
				return;
			}
			Latexes.OnHit(this);
		}

		// Token: 0x0600035C RID: 860 RVA: 0x00037214 File Offset: 0x00035414
		public void AnimalDeath()
		{
			BuildingEntity component = base.GetComponent<BuildingEntity>();
			if (component)
			{
				component.health = 100000f;
			}
		}

		// Token: 0x0600035D RID: 861 RVA: 0x0003723C File Offset: 0x0003543C
		private void OnDestroy()
		{
			Latexes.All.Remove(this);
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
			if (this.tips != null)
			{
				foreach (SpriteRenderer spriteRenderer2 in this.tips)
				{
					if (spriteRenderer2)
					{
						Object.Destroy(spriteRenderer2.gameObject);
					}
				}
			}
			foreach (SpriteRenderer spriteRenderer3 in this.splash)
			{
				if (spriteRenderer3)
				{
					Object.Destroy(spriteRenderer3.gameObject);
				}
			}
		}

		// Token: 0x0400030C RID: 780
		public LatexKind kind;

		// Token: 0x0400030D RID: 781
		public Vector2 pos;

		// Token: 0x0400030E RID: 782
		public int facing = 1;

		// Token: 0x0400030F RID: 783
		public SpriteRenderer sr;

		// Token: 0x04000310 RID: 784
		public SpriteRenderer[] segs;

		// Token: 0x04000311 RID: 785
		public SpriteRenderer[] tips;

		// Token: 0x04000312 RID: 786
		public float sated;

		// Token: 0x04000313 RID: 787
		public float frame;

		// Token: 0x04000314 RID: 788
		public float attackCooldown;

		// Token: 0x04000315 RID: 789
		public float flinch;

		// Token: 0x04000316 RID: 790
		public float lastHit = -100f;

		// Token: 0x04000317 RID: 791
		public int hits;

		// Token: 0x04000318 RID: 792
		public bool dead;

		// Token: 0x04000319 RID: 793
		public float vy;

		// Token: 0x0400031A RID: 794
		public LatexBlob.Shot shot;

		// Token: 0x0400031B RID: 795
		public float shotTime;

		// Token: 0x0400031C RID: 796
		public readonly List<SpriteRenderer> splash = new List<SpriteRenderer>();

		// Token: 0x0400031D RID: 797
		public readonly List<Vector2> splashVel = new List<Vector2>();

		// Token: 0x0400031E RID: 798
		public Vector2[] reachTo = new Vector2[2];

		// Token: 0x020001C4 RID: 452
		public enum Shot
		{
			// Token: 0x040009AA RID: 2474
			None,
			// Token: 0x040009AB RID: 2475
			Falling,
			// Token: 0x040009AC RID: 2476
			Scattering,
			// Token: 0x040009AD RID: 2477
			Gone,
			// Token: 0x040009AE RID: 2478
			Reforming
		}
	}
}
