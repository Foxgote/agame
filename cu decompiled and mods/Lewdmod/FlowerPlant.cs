using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200008E RID: 142
	internal class FlowerPlant : MonoBehaviour
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060002B5 RID: 693 RVA: 0x00030241 File Offset: 0x0002E441
		public string FrameName
		{
			get
			{
				return string.Format("flower_{0}_{1}", this.anim, this.frame);
			}
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00030260 File Offset: 0x0002E460
		public Vector2 Point(int i)
		{
			Vector2[] array;
			if (!FlowerArt.Meta(this.FrameName, out array))
			{
				return base.transform.position;
			}
			return base.transform.position + FlowerArt.Offset(array[i], this.flip);
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060002B7 RID: 695 RVA: 0x000302B4 File Offset: 0x0002E4B4
		public Vector2 MouthFront
		{
			get
			{
				return (this.Point(2) + this.Point(3)) * 0.5f;
			}
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x000302D4 File Offset: 0x0002E4D4
		public void Set(string a, int f)
		{
			Sprite[] array;
			if (!FlowerArt.Anim.TryGetValue(a, out array) || array.Length == 0)
			{
				return;
			}
			this.anim = a;
			this.frame = Mathf.Clamp(f, 0, array.Length - 1);
			this.sr.sprite = array[this.frame];
			this.sr.flipX = this.flip;
			if (!this.box)
			{
				this.box = base.GetComponent<BoxCollider2D>();
			}
			if (this.box)
			{
				this.box.offset = new Vector2(this.flip ? (-this.boxX) : this.boxX, this.box.offset.y);
			}
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0003038F File Offset: 0x0002E58F
		private void Start()
		{
			this.seed = Random.value * 10f;
			this.wantFlip = this.flip;
			this.rootAt = base.transform.position;
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060002BA RID: 698 RVA: 0x000303C4 File Offset: 0x0002E5C4
		public bool Down
		{
			get
			{
				return this.burrow != 0;
			}
		}

		// Token: 0x060002BB RID: 699 RVA: 0x000303CF File Offset: 0x0002E5CF
		public void Face(bool left)
		{
			if (left != this.flip || this.turn > 0f)
			{
				this.wantFlip = left;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060002BC RID: 700 RVA: 0x000303EE File Offset: 0x0002E5EE
		public bool Turning
		{
			get
			{
				return this.turn > 0f || this.wantFlip != this.flip;
			}
		}

		// Token: 0x060002BD RID: 701 RVA: 0x00030410 File Offset: 0x0002E610
		private void Update()
		{
			if (this.sated > 0f)
			{
				this.sated -= Time.deltaTime;
			}
			float num = 1f;
			float num2 = 0f;
			if (this.Turning && !Flowers.Drives(this) && !this.Down)
			{
				this.turn += Time.deltaTime / 0.4f;
				if (this.turn >= 0.5f && this.flip != this.wantFlip)
				{
					this.flip = this.wantFlip;
					if (this.sr)
					{
						this.sr.flipX = this.flip;
					}
					if (this.box)
					{
						this.box.offset = new Vector2(this.flip ? (-this.boxX) : this.boxX, this.box.offset.y);
					}
				}
				if (this.turn >= 1f)
				{
					this.turn = 0f;
				}
				num = Mathf.Max(0.08f, Mathf.Abs(1f - 2f * this.turn));
			}
			if (this.burrow == 1 || this.burrow == 3)
			{
				string text = ((this.burrow == 1) ? "burrow" : "emerge");
				Sprite[] array;
				int num3 = (FlowerArt.Anim.TryGetValue(text, out array) ? array.Length : 1);
				this.sink = Mathf.Min(1f, this.sink + Time.deltaTime / ((this.burrow == 1) ? 1.2f : 1.6f));
				this.Set(text, Mathf.Min(num3 - 1, (int)(this.sink * (float)num3)));
				num2 = Mathf.Sin(Time.time * 55f) * 0.07f * (1f - 0.5f * this.sink);
				if ((this.shakeTimer -= Time.deltaTime) <= 0f)
				{
					this.shakeTimer = 0.15f;
					Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
					if (body && Vector2.Distance(body.transform.position, this.rootAt) < 22f)
					{
						PlayerCamera.main.shaker.Shake(18f);
					}
				}
				if (this.sink >= 1f)
				{
					if (this.burrow == 1)
					{
						Flowers.Buried(this);
					}
					else
					{
						this.burrow = 0;
						this.sink = 0f;
						this.sated = 0.5f;
						if (this.box)
						{
							this.box.enabled = true;
						}
					}
				}
			}
			base.transform.localScale = new Vector3(num, 1f, 1f);
			base.transform.position = new Vector3(this.rootAt.x + num2, this.rootAt.y, 0f);
			if (Flowers.Drives(this) || !this.sr || this.Down)
			{
				return;
			}
			float num4 = Time.time - this.swallowAt;
			if (num4 < 1.5f)
			{
				this.Set("swallow", 1 + (int)(num4 / 0.3f));
				return;
			}
			if (Pits.Holder == this.pit || this.sated > 0f)
			{
				this.Set("fed", (int)(Time.time * 3f + this.seed) % 4);
				return;
			}
			this.Set("idle", (int)(Time.time * 4f + this.seed) % 4);
		}

		// Token: 0x060002BE RID: 702 RVA: 0x000307C4 File Offset: 0x0002E9C4
		public void AnimalHit(float damage)
		{
			FleshBurn component = base.GetComponent<FleshBurn>();
			if (component)
			{
				Burning.Hit(component, damage);
				return;
			}
			Flowers.OnHit(this, damage);
		}

		// Token: 0x060002BF RID: 703 RVA: 0x000307F0 File Offset: 0x0002E9F0
		public void AnimalDeath()
		{
			BuildingEntity component = base.GetComponent<BuildingEntity>();
			if (component)
			{
				component.health = 100000f;
			}
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x00030817 File Offset: 0x0002EA17
		private void OnDestroy()
		{
			Flowers.Plants.Remove(this);
		}

		// Token: 0x04000289 RID: 649
		public Pit pit;

		// Token: 0x0400028A RID: 650
		public SpriteRenderer sr;

		// Token: 0x0400028B RID: 651
		public bool flip;

		// Token: 0x0400028C RID: 652
		public float sated;

		// Token: 0x0400028D RID: 653
		public float swallowAt = -100f;

		// Token: 0x0400028E RID: 654
		public bool dead;

		// Token: 0x0400028F RID: 655
		public string anim = "idle";

		// Token: 0x04000290 RID: 656
		public int frame;

		// Token: 0x04000291 RID: 657
		private float seed;

		// Token: 0x04000292 RID: 658
		private BoxCollider2D box;

		// Token: 0x04000293 RID: 659
		public float boxX;

		// Token: 0x04000294 RID: 660
		private Vector2 rootAt;

		// Token: 0x04000295 RID: 661
		private bool wantFlip;

		// Token: 0x04000296 RID: 662
		private float turn;

		// Token: 0x04000297 RID: 663
		public int hits;

		// Token: 0x04000298 RID: 664
		public float lastHit = -100f;

		// Token: 0x04000299 RID: 665
		public int burrow;

		// Token: 0x0400029A RID: 666
		public float sink;

		// Token: 0x0400029B RID: 667
		public float underTimer;

		// Token: 0x0400029C RID: 668
		private float shakeTimer;
	}
}
