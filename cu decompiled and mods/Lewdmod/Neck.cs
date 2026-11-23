using System;
using System.Collections.Generic;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000BE RID: 190
	internal static class Neck
	{
		// Token: 0x0600043D RID: 1085 RVA: 0x0004485C File Offset: 0x00042A5C
		private static Neck.State For(Body b)
		{
			Neck.State state;
			if (!Neck.states.TryGetValue(b, out state))
			{
				foreach (Body body in new List<Body>(Neck.states.Keys))
				{
					if (!body)
					{
						if (Neck.states[body].neckTex)
						{
							Object.Destroy(Neck.states[body].neckTex);
						}
						Neck.states.Remove(body);
					}
				}
				state = (Neck.states[b] = new Neck.State());
			}
			return state;
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x00044918 File Offset: 0x00042B18
		public static void Update(Body b)
		{
			Neck.State state = Neck.For(b);
			Limb limb = ((b.limbs.Length > 2) ? b.LimbByName("Head") : null);
			Limb limb2 = ((b.limbs.Length > 1) ? b.limbs[1] : null);
			bool flag = limb && limb2 && !limb.dismembered && !limb2.dismembered;
			if (flag && limb.joint && Neck.raised.Add(limb.joint.GetInstanceID()))
			{
				HingeJoint2D joint = limb.joint;
				Vector2 connectedAnchor = joint.connectedAnchor;
				joint.autoConfigureConnectedAnchor = false;
				joint.connectedAnchor = connectedAnchor + new Vector2(0f, Neck.Rise(limb2));
				if (PlayerCamera.main && b == PlayerCamera.main.body)
				{
					Plugin.Log.LogInfo(string.Format("Her neck: head raised {0} px", 1));
				}
			}
			Neck.DrawNeck(state, limb2, limb, flag);
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x00044A1C File Offset: 0x00042C1C
		private static float Rise(Limb torso)
		{
			SpriteRenderer component = torso.GetComponent<SpriteRenderer>();
			float num = ((component && component.sprite) ? component.sprite.pixelsPerUnit : 8f);
			return 1f / Mathf.Max(num, 8f);
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00044A6C File Offset: 0x00042C6C
		private static void DrawNeck(Neck.State o, Limb torso, Limb head, bool show)
		{
			SpriteRenderer spriteRenderer = (show ? torso.GetComponent<SpriteRenderer>() : null);
			SpriteRenderer spriteRenderer2 = (show ? head.GetComponent<SpriteRenderer>() : null);
			if (!spriteRenderer || !spriteRenderer.enabled || !spriteRenderer.sprite || !spriteRenderer2 || !spriteRenderer2.enabled || !head.joint)
			{
				if (o.neckSr)
				{
					o.neckSr.enabled = false;
				}
				return;
			}
			if (o.neck && o.neck.transform.parent == head.transform && o.neckSr.enabled && spriteRenderer.sprite == o.lastSprite && Time.unscaledTime < o.nextLook)
			{
				return;
			}
			o.lastSprite = spriteRenderer.sprite;
			o.nextLook = Time.unscaledTime + 0.5f;
			Sprite sprite = spriteRenderer.sprite;
			float pixelsPerUnit = sprite.pixelsPerUnit;
			int num = Mathf.RoundToInt(sprite.rect.width);
			int num2 = Mathf.RoundToInt(sprite.rect.height);
			if (!o.neck || o.neck.transform.parent != head.transform)
			{
				if (o.neck)
				{
					Object.Destroy(o.neck);
				}
				if (o.neckTex)
				{
					Object.Destroy(o.neckTex);
				}
				o.neck = new GameObject("LewdNeck");
				o.neck.transform.SetParent(head.transform, false);
				o.neckSr = o.neck.AddComponent<SpriteRenderer>();
				o.neckTex = null;
			}
			if (!o.neckTex || o.neckTex.width != num)
			{
				if (o.neckTex)
				{
					Object.Destroy(o.neckTex);
				}
				o.neckTex = new Texture2D(num, 4, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				o.neckSr.sprite = null;
			}
			o.neckSr.sharedMaterial = spriteRenderer.sharedMaterial;
			int num3 = Mathf.Max(0, (num2 - 10) / 2);
			Color32[] array = new Color32[num];
			for (int i = 0; i < num; i++)
			{
				Color color;
				if (Bust.TryPixel(sprite, i, num3, out color) && color.a > 0.5f)
				{
					color *= spriteRenderer.color;
					color.a = 1f;
					array[i] = color;
				}
			}
			for (int j = num - 2; j >= 0; j--)
			{
				if (array[j].a > 0)
				{
					array[j + 1] = array[j];
					break;
				}
			}
			Color32[] array2 = new Color32[num * 4];
			for (int k = 0; k < 4; k++)
			{
				Array.Copy(array, 0, array2, k * num, num);
			}
			o.neckTex.SetPixels32(array2);
			o.neckTex.Apply(false);
			if (!o.neckSr.sprite || !Mathf.Approximately(o.neckSr.sprite.pixelsPerUnit, pixelsPerUnit))
			{
				o.neckSr.sprite = Sprite.Create(o.neckTex, new Rect(0f, 0f, (float)num, 4f), new Vector2(0f, 1f), pixelsPerUnit);
			}
			bool flipX = spriteRenderer.flipX;
			float x = sprite.bounds.min.x;
			float num4 = sprite.bounds.min.y + (float)(num2 - num3 + 1) / pixelsPerUnit;
			Vector2 vector = new Vector2(flipX ? (-x) : x, num4) - head.joint.connectedAnchor + head.joint.anchor;
			o.neck.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
			o.neck.transform.localRotation = Quaternion.identity;
			o.neck.transform.localScale = Vector3.one;
			o.neckSr.flipX = flipX;
			o.neckSr.sortingLayerID = spriteRenderer.sortingLayerID;
			o.neckSr.sortingOrder = spriteRenderer.sortingOrder;
			Vector3 position = o.neck.transform.position;
			o.neck.transform.position = new Vector3(position.x, position.y, torso.transform.position.z - 0.0005f);
			Suit.Look look = ((PlayerCamera.main && torso.body == PlayerCamera.main.body) ? Suit.Mine : default(Suit.Look));
			bool flag = look.stage > 0 && Suit.Covers("experimentUpTorso", look.stage, look);
			Color color2 = (flag ? ParasiteArt.Colors[Mathf.Clamp(look.color, 0, ParasiteArt.Colors.Length - 1)] : Color.white);
			o.neckSr.color = ((flag && look.HostSlime) ? new Color(0.2f, 0.22f, 0.18f) : color2);
			o.neckSr.enabled = true;
		}

		// Token: 0x040003D7 RID: 983
		private const int Rows = 1;

		// Token: 0x040003D8 RID: 984
		private const int Under = 1;

		// Token: 0x040003D9 RID: 985
		private const int Into = 2;

		// Token: 0x040003DA RID: 986
		private const int Tall = 4;

		// Token: 0x040003DB RID: 987
		private const int ChestW = 8;

		// Token: 0x040003DC RID: 988
		private const int ChestH = 10;

		// Token: 0x040003DD RID: 989
		private static readonly HashSet<int> raised = new HashSet<int>();

		// Token: 0x040003DE RID: 990
		private static readonly Dictionary<Body, Neck.State> states = new Dictionary<Body, Neck.State>();

		// Token: 0x020001E8 RID: 488
		private sealed class State
		{
			// Token: 0x04000A23 RID: 2595
			public GameObject neck;

			// Token: 0x04000A24 RID: 2596
			public SpriteRenderer neckSr;

			// Token: 0x04000A25 RID: 2597
			public Texture2D neckTex;

			// Token: 0x04000A26 RID: 2598
			public Sprite lastSprite;

			// Token: 0x04000A27 RID: 2599
			public float nextLook;
		}
	}
}
