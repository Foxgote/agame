using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000133 RID: 307
	internal class Larva : MonoBehaviour
	{
		// Token: 0x06000754 RID: 1876 RVA: 0x00070E50 File Offset: 0x0006F050
		public static void Spawn(Vector2 at, bool root)
		{
			if (Larva.frames.Length == 0)
			{
				Larva.frames = TentacleArt.Larva;
			}
			GameObject gameObject = new GameObject("LewdLarva");
			gameObject.transform.position = at;
			Larva larva = gameObject.AddComponent<Larva>();
			larva.sr = gameObject.AddComponent<SpriteRenderer>();
			larva.sr.sprite = ((Larva.frames.Length != 0) ? Larva.frames[0] : null);
			if (WorldGeneration.world)
			{
				larva.sr.sharedMaterial = WorldGeneration.world.defaultMat;
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			SpriteRenderer spriteRenderer = (body ? body.limbs[1].GetComponent<SpriteRenderer>() : null);
			if (spriteRenderer)
			{
				larva.sr.sortingLayerID = spriteRenderer.sortingLayerID;
				larva.sr.sortingOrder = spriteRenderer.sortingOrder + 20;
			}
			larva.rb = gameObject.AddComponent<Rigidbody2D>();
			larva.rb.freezeRotation = true;
			gameObject.AddComponent<BoxCollider2D>().size = new Vector2(0.4f, 0.15f);
			gameObject.layer = LayerMask.NameToLayer("Default");
			larva.dir = ((Random.value < 0.5f) ? (-1f) : 1f);
			larva.rb.velocity = new Vector2(larva.dir * Random.Range(1f, 3f), Random.Range(1f, 3f));
			larva.rooting = root;
			larva.sr.flipX = larva.dir < 0f;
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x00070FF0 File Offset: 0x0006F1F0
		private void Update()
		{
			if ((this.frameTime -= Time.deltaTime) <= 0f && Larva.frames.Length != 0)
			{
				this.frameTime = 0.18f;
				this.frame = (this.frame + 1) % Larva.frames.Length;
				this.sr.sprite = Larva.frames[this.frame];
			}
			this.rb.velocity = new Vector2(Mathf.MoveTowards(this.rb.velocity.x, this.dir * 1.6f, Time.deltaTime * 4f), this.rb.velocity.y);
			this.life -= Time.deltaTime;
			if (this.life < 1.5f)
			{
				this.sr.color = new Color(1f, 1f, 1f, Mathf.Clamp01(this.life / 1.5f));
			}
			if (this.life <= 0f)
			{
				if (this.rooting)
				{
					Tentacles.RootNear(base.transform.position);
				}
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0400077D RID: 1917
		private static Sprite[] frames = new Sprite[0];

		// Token: 0x0400077E RID: 1918
		private SpriteRenderer sr;

		// Token: 0x0400077F RID: 1919
		private Rigidbody2D rb;

		// Token: 0x04000780 RID: 1920
		private float life = 7f;

		// Token: 0x04000781 RID: 1921
		private float frameTime;

		// Token: 0x04000782 RID: 1922
		private float dir;

		// Token: 0x04000783 RID: 1923
		private int frame;

		// Token: 0x04000784 RID: 1924
		private bool rooting;
	}
}
