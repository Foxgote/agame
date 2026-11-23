using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000087 RID: 135
	internal class FleshPatch : MonoBehaviour
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000275 RID: 629 RVA: 0x0002D214 File Offset: 0x0002B414
		public float HalfWidth
		{
			get
			{
				if (!this.sr)
				{
					return 2f;
				}
				return this.sr.bounds.extents.x;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000276 RID: 630 RVA: 0x0002D24C File Offset: 0x0002B44C
		public Vector2 Mouth
		{
			get
			{
				if (!this.sr)
				{
					return base.transform.position;
				}
				return new Vector2(this.sr.bounds.center.x, this.sr.bounds.min.y + this.sr.bounds.size.y * 0.55f);
			}
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0002D2CB File Offset: 0x0002B4CB
		private void Awake()
		{
			this.sr = base.GetComponent<SpriteRenderer>();
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0002D2DC File Offset: 0x0002B4DC
		private void Update()
		{
			if (this.sated > 0f)
			{
				this.sated -= Time.deltaTime;
			}
			this.frameTime -= Time.deltaTime * ((Flesh.Holder == this) ? 2.5f : 1f);
			if (this.frameTime <= 0f && FleshArt.Floor.Length != 0)
			{
				this.frameTime = 0.45f;
				this.frame = (this.frame + 1) % FleshArt.Floor.Length;
				this.sr.sprite = FleshArt.Floor[this.frame];
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && Vector2.Distance(body.transform.position, base.transform.position) < 12f && (this.gurgle -= Time.deltaTime) <= 0f)
			{
				this.gurgle = Random.Range(3f, 7f);
				Sound.Play("goo", base.transform.position, false, true, base.transform, 0.6f, 0.45f, false, false);
			}
			foreach (ParasiteAI parasiteAI in Parasites.Alive.ToArray())
			{
				if (parasiteAI)
				{
					Vector2 vector = parasiteAI.transform.position - base.transform.position;
					if (Mathf.Abs(vector.x) < this.HalfWidth * 0.8f && Mathf.Abs(vector.y) < 1f)
					{
						Sound.Play("eatFlesh", parasiteAI.transform.position, false, true, null, 0.8f, 1f, false, false);
						Parasites.Alive.Remove(parasiteAI);
						Object.Destroy(parasiteAI.gameObject);
					}
				}
			}
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0002D4F8 File Offset: 0x0002B6F8
		public void AnimalHit(float damage)
		{
			Sound.Play("goo", base.transform.position, false, true, base.transform, 1f, 0.6f, false, false);
			FleshBurn component = base.GetComponent<FleshBurn>();
			if (component)
			{
				Burning.Hit(component, damage);
				return;
			}
			Flesh.OnPatchHit(this);
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0002D554 File Offset: 0x0002B754
		public void AnimalDeath()
		{
			Sound.Play("gore", base.transform.position, false, true, null, 1f, 1f, false, false);
			Flesh.OnPatchDestroyed(this);
			Flesh.Patches.Remove(this);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0002D59D File Offset: 0x0002B79D
		private void OnDestroy()
		{
			Flesh.Patches.Remove(this);
		}

		// Token: 0x0400025B RID: 603
		public float sated;

		// Token: 0x0400025C RID: 604
		private SpriteRenderer sr;

		// Token: 0x0400025D RID: 605
		private float frameTime;

		// Token: 0x0400025E RID: 606
		private float gurgle;

		// Token: 0x0400025F RID: 607
		private int frame;
	}
}
