using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000059 RID: 89
	internal class FleshBurn : MonoBehaviour
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060001AC RID: 428 RVA: 0x0002568C File Offset: 0x0002388C
		public Vector2 Center
		{
			get
			{
				if (!this.box)
				{
					return base.transform.position;
				}
				return this.box.bounds.center;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060001AD RID: 429 RVA: 0x000256D0 File Offset: 0x000238D0
		public float Reach
		{
			get
			{
				if (!this.box)
				{
					return 1f;
				}
				return Mathf.Max(this.box.bounds.extents.x, this.box.bounds.extents.y);
			}
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00025725 File Offset: 0x00023925
		private void Start()
		{
			this.box = base.GetComponent<Collider2D>();
			if (this.look)
			{
				this.baseColor = this.look.color;
			}
			Burning.All.Add(this);
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0002575C File Offset: 0x0002395C
		private void OnDestroy()
		{
			Burning.All.Remove(this);
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0002576A File Offset: 0x0002396A
		public void Scorch()
		{
			this.glow = 1f;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00025778 File Offset: 0x00023978
		private void Update()
		{
			if (this.glow <= 0f || !this.look)
			{
				return;
			}
			this.glow = Mathf.Max(0f, this.glow - Time.deltaTime * 1.5f);
			this.look.color = Color.Lerp(this.baseColor, new Color(1f, 0.55f, 0.25f, this.baseColor.a), this.glow);
		}

		// Token: 0x040001B9 RID: 441
		public const float Toughness = 100000f;

		// Token: 0x040001BA RID: 442
		public float hp;

		// Token: 0x040001BB RID: 443
		public float max;

		// Token: 0x040001BC RID: 444
		public string what;

		// Token: 0x040001BD RID: 445
		public Action stun;

		// Token: 0x040001BE RID: 446
		public Action death;

		// Token: 0x040001BF RID: 447
		public SpriteRenderer look;

		// Token: 0x040001C0 RID: 448
		public bool dead;

		// Token: 0x040001C1 RID: 449
		public bool hitsHurt;

		// Token: 0x040001C2 RID: 450
		public float fireScale = 1f;

		// Token: 0x040001C3 RID: 451
		public bool douses;

		// Token: 0x040001C4 RID: 452
		private float glow;

		// Token: 0x040001C5 RID: 453
		private Color baseColor = Color.white;

		// Token: 0x040001C6 RID: 454
		private Collider2D box;
	}
}
