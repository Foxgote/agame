using System;
using PoseKit;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000038 RID: 56
	internal static class ReloadAnim
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x0000E4A6 File Offset: 0x0000C6A6
		public static bool Active
		{
			get
			{
				return ReloadAnim.gun && ReloadAnim.body && Time.time - ReloadAnim.start < 1.45f;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x0000E4D4 File Offset: 0x0000C6D4
		public static GunScript Gun
		{
			get
			{
				return ReloadAnim.gun;
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000E4DB File Offset: 0x0000C6DB
		public static bool CanPlay(Body b)
		{
			return b && b.standing && !HerPuppet.Holds(b) && !b.HoldingItem(1 - b.handSlot);
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000E508 File Offset: 0x0000C708
		public static void Begin(Body b, GunScript g)
		{
			ReloadAnim.body = b;
			ReloadAnim.gun = g;
			ReloadAnim.start = Time.time;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000E520 File Offset: 0x0000C720
		public static void Hold(GunScript g, bool? old)
		{
			if (ReloadAnim.mag)
			{
				Object.Destroy(ReloadAnim.mag);
			}
			ReloadAnim.mag = null;
			if (old == null || !g)
			{
				return;
			}
			GameObject gameObject = Resources.Load(AmmoScript.AmmoTypeToMagazine(g.ammoType)) as GameObject;
			SpriteRenderer spriteRenderer = (gameObject ? gameObject.GetComponentInChildren<SpriteRenderer>(true) : null);
			if (!spriteRenderer || !spriteRenderer.sprite)
			{
				return;
			}
			ReloadAnim.mag = new GameObject("LewdReloadMag");
			SpriteRenderer spriteRenderer2 = ReloadAnim.mag.AddComponent<SpriteRenderer>();
			spriteRenderer2.sprite = spriteRenderer.sprite;
			spriteRenderer2.sharedMaterial = spriteRenderer.sharedMaterial;
			ReloadAnim.mag.transform.localScale = gameObject.transform.localScale;
			SpriteRenderer componentInChildren = g.GetComponentInChildren<SpriteRenderer>();
			if (componentInChildren)
			{
				spriteRenderer2.sortingLayerID = componentInChildren.sortingLayerID;
				spriteRenderer2.sortingOrder = componentInChildren.sortingOrder + 2;
			}
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000E60F File Offset: 0x0000C80F
		public static void Stop()
		{
			if (ReloadAnim.mag)
			{
				Object.Destroy(ReloadAnim.mag);
			}
			ReloadAnim.mag = null;
			ReloadAnim.gun = null;
			ReloadAnim.body = null;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000E63C File Offset: 0x0000C83C
		public static bool Target(Vector2 grip, Vector2 muzzle, Vector2 hip, Transform hand, out Vector2 target, out float weight)
		{
			target = grip;
			weight = 0f;
			if (!ReloadAnim.Active)
			{
				return false;
			}
			float num = Time.time - ReloadAnim.start;
			Vector2 vector = Vector2.down * 0.4f;
			bool flag = Vector2.Distance(grip, muzzle) < 1.2f;
			Vector2 vector2 = (flag ? grip : Vector2.Lerp(grip, muzzle, 0.2f)) + vector;
			Vector2 vector3 = Vector2.Lerp(grip, muzzle, flag ? 0.3f : 0.15f) + Vector2.up * 0.25f;
			if (num < 0.25f)
			{
				target = vector2;
				weight = num / 0.25f;
			}
			else if (num < 0.55f)
			{
				target = Vector2.Lerp(vector2, hip, Mathf.SmoothStep(0f, 1f, (num - 0.25f) / 0.3f));
				weight = 1f;
			}
			else if (num < 0.85f)
			{
				target = Vector2.Lerp(hip, vector2, Mathf.SmoothStep(0f, 1f, (num - 0.55f) / 0.3f));
				weight = 1f;
			}
			else if (num < 1f)
			{
				target = Vector2.Lerp(vector2, vector3, (num - 0.85f) / 0.14999998f);
				weight = 1f;
			}
			else
			{
				target = vector3;
				weight = Mathf.Clamp01(1f - (num - 1f) / 0.25f);
			}
			if (ReloadAnim.mag && hand)
			{
				ReloadAnim.mag.transform.position = new Vector3(hand.position.x, hand.position.y, hand.position.z - 0.01f);
				ReloadAnim.mag.transform.rotation = hand.rotation;
			}
			return true;
		}

		// Token: 0x040000F0 RID: 240
		public const float Out = 0.25f;

		// Token: 0x040000F1 RID: 241
		public const float Hip = 0.55f;

		// Token: 0x040000F2 RID: 242
		public const float In = 0.85f;

		// Token: 0x040000F3 RID: 243
		public const float Rack = 1f;

		// Token: 0x040000F4 RID: 244
		public const float End = 1.25f;

		// Token: 0x040000F5 RID: 245
		private static Body body;

		// Token: 0x040000F6 RID: 246
		private static GunScript gun;

		// Token: 0x040000F7 RID: 247
		private static float start = -99f;

		// Token: 0x040000F8 RID: 248
		private static GameObject mag;
	}
}
