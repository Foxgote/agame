using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000126 RID: 294
	internal class LewdWall : MonoBehaviour
	{
		// Token: 0x060006D2 RID: 1746 RVA: 0x0006A802 File Offset: 0x00068A02
		public void Init(WallKind kind, Vector2 face, int facing, float height)
		{
			this.Kind = kind;
			this.Face = face;
			this.Facing = facing;
			this.Height = height;
			this.sr = base.GetComponent<SpriteRenderer>();
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x0006A830 File Offset: 0x00068A30
		public bool Near(Vector2 p, float maxOut, out float outward)
		{
			outward = (p.x - this.Face.x) * (float)this.Facing;
			return Mathf.Abs(p.y - this.Face.y) < this.Height * 0.5f + 0.3f && outward > -0.5f && outward < maxOut;
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x0006A894 File Offset: 0x00068A94
		private void Update()
		{
			if (this.sated > 0f)
			{
				this.sated -= Time.deltaTime;
			}
			Sprite[] array = ((this.Kind == WallKind.Flesh) ? WallArt.Flesh : WallArt.Slime);
			this.frameTime -= Time.deltaTime * ((FleshWall.Holder == this) ? 2.5f : 1f);
			if (this.frameTime <= 0f && array.Length != 0 && this.sr)
			{
				this.frameTime = ((this.Kind == WallKind.Flesh) ? 0.5f : 0.7f);
				this.frame = (this.frame + 1) % array.Length;
				this.sr.sprite = array[this.frame];
				for (int i = 0; i < this.Slices.Count; i++)
				{
					if (this.Slices[i])
					{
						this.Slices[i].sprite = WallArt.Slice(array[this.frame], this.SliceRow[i], this.Rows, this.SliceBend[i]);
					}
				}
			}
			for (int j = 0; j < this.Slices.Count; j++)
			{
				if (this.Slices[j])
				{
					this.Slices[j].color = this.sr.color;
				}
			}
			if ((this.holdCheck -= Time.deltaTime) <= 0f)
			{
				this.holdCheck = 0.5f;
				if (this.RockGone())
				{
					this.Fall();
					return;
				}
			}
			if (this.Kind != WallKind.Flesh)
			{
				return;
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && Vector2.Distance(body.transform.position, this.Face) < 10f && (this.gurgle -= Time.deltaTime) <= 0f)
			{
				this.gurgle = Random.Range(3f, 7f);
				Sound.Play("goo", this.Face, false, true, base.transform, 0.5f, 0.4f, false, false);
			}
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x0006AAF0 File Offset: 0x00068CF0
		private bool RockGone()
		{
			WorldGeneration w = WorldGeneration.world;
			return w && this.Holds.Count != 0 && this.Holds.Count<Vector2Int>((Vector2Int c) => w.GetBlock(c) == 0) >= Mathf.Max(2, this.Holds.Count / 3);
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x0006AB5C File Offset: 0x00068D5C
		private void Fall()
		{
			Plugin.Log.LogInfo(string.Format("The {0} wall at {1} lost its rock", this.Kind.ToString().ToLowerInvariant(), this.Face));
			FleshBurn component = base.GetComponent<FleshBurn>();
			if (component && !component.dead)
			{
				Burning.Damage(component, component.hp + 1f, "its rock is gone", false);
				return;
			}
			if (!component)
			{
				this.AnimalDeath();
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0006ABE8 File Offset: 0x00068DE8
		public void AnimalHit(float damage)
		{
			Sound.Play("goo", this.Face, false, true, base.transform, 1f, (this.Kind == WallKind.Flesh) ? 0.6f : 1.1f, false, false);
			FleshBurn component = base.GetComponent<FleshBurn>();
			if (component)
			{
				Burning.Hit(component, damage);
			}
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x0006AC40 File Offset: 0x00068E40
		public void AnimalDeath()
		{
			Sound.Play((this.Kind == WallKind.Flesh) ? "gore" : "goo", this.Face, false, true, null, 1f, 1f, false, false);
			if (this.Kind == WallKind.Flesh)
			{
				FleshWall.OnDestroyed(this);
			}
			else
			{
				SlimeFluid.Pour(this.Face + Vector2.right * (float)this.Facing * 0.8f, 6, null);
			}
			Plugin.Log.LogInfo(string.Format("A {0} wall at {1} was destroyed", this.Kind.ToString().ToLowerInvariant(), this.Face));
			Walls.All.Remove(this);
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x0006ACFB File Offset: 0x00068EFB
		private void OnDestroy()
		{
			Walls.All.Remove(this);
		}

		// Token: 0x0400071C RID: 1820
		public WallKind Kind;

		// Token: 0x0400071D RID: 1821
		public int Facing;

		// Token: 0x0400071E RID: 1822
		public Vector2 Face;

		// Token: 0x0400071F RID: 1823
		public float Height;

		// Token: 0x04000720 RID: 1824
		public float sated;

		// Token: 0x04000721 RID: 1825
		private SpriteRenderer sr;

		// Token: 0x04000722 RID: 1826
		private float frameTime;

		// Token: 0x04000723 RID: 1827
		private float gurgle;

		// Token: 0x04000724 RID: 1828
		private float holdCheck;

		// Token: 0x04000725 RID: 1829
		private int frame;

		// Token: 0x04000726 RID: 1830
		public readonly List<SpriteRenderer> Slices = new List<SpriteRenderer>();

		// Token: 0x04000727 RID: 1831
		public readonly List<int> SliceRow = new List<int>();

		// Token: 0x04000728 RID: 1832
		public readonly List<int> SliceBend = new List<int>();

		// Token: 0x04000729 RID: 1833
		public int Rows;

		// Token: 0x0400072A RID: 1834
		public readonly List<Vector2Int> Holds = new List<Vector2Int>();
	}
}
