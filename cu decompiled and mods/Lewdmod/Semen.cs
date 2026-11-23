using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000F4 RID: 244
	internal static class Semen
	{
		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x0005859B File Offset: 0x0005679B
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x000585A4 File Offset: 0x000567A4
		public static void Load(string dir)
		{
			if (!Directory.Exists(dir))
			{
				return;
			}
			foreach (string text in from f in Directory.GetFiles(dir, "splat_*.png")
				orderby f
				select f)
			{
				Texture2D texture2D = new Texture2D(2, 2, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text));
				Semen.textures.Add(texture2D);
			}
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x0005864C File Offset: 0x0005684C
		public static void Add(Body b, float amount)
		{
			Semen.S.cum = Mathf.Min(100f, Semen.S.cum + amount);
			InvasionPopup.OnCumOutside(b, amount);
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x00058678 File Offset: 0x00056878
		public static void Update(Body b, float dt)
		{
			Semen.S.cum = Mathf.Max(0f, Semen.S.cum - 0.05f * dt);
			if (b.inWater)
			{
				Semen.S.cum = Mathf.Max(0f, Semen.S.cum - 6f * dt);
			}
			else if (b.wetness > 8f)
			{
				Semen.S.cum = Mathf.Max(0f, Semen.S.cum - 0.06f * b.wetness * dt);
			}
			if (Semen.S.cum > 5f)
			{
				float num = ((Semen.S.corruption > 60f) ? 0.01f : (-0.012f * (1f - Semen.S.corruption / 60f)));
				Mood.Change(b, num * dt);
				Semen.S.lust = Mathf.Min(100f, Semen.S.lust + 0.02f * (Semen.S.cum / 100f) * dt);
			}
			Semen.UpdateDecals(b, Semen.S.cum);
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x000587AC File Offset: 0x000569AC
		private static void UpdateDecals(Body b, float cum)
		{
			List<SpriteRenderer> list;
			if (!Semen.decalSets.TryGetValue(b, out list))
			{
				foreach (Body body in new List<Body>(Semen.decalSets.Keys))
				{
					if (!body)
					{
						Semen.decalSets.Remove(body);
					}
				}
				list = (Semen.decalSets[b] = new List<SpriteRenderer>());
			}
			list.RemoveAll((SpriteRenderer d) => !d);
			int num = ((cum <= 5f) ? 0 : Mathf.CeilToInt(cum / 100f * 14f));
			SpriteRenderer component = b.limbs[1].GetComponent<SpriteRenderer>();
			if (component && component.sprite && !Mathf.Approximately(component.sprite.pixelsPerUnit, Semen.ppu))
			{
				Semen.ppu = component.sprite.pixelsPerUnit;
				Semen.sprites = Semen.textures.Select<Texture2D, Sprite>((Texture2D t) => Sprite.Create(t, new Rect(0f, 0f, (float)t.width, (float)t.height), new Vector2(0.5f, 0.6f), Semen.ppu)).ToArray<Sprite>();
			}
			while (list.Count < num)
			{
				if (Semen.sprites.Length == 0)
				{
					break;
				}
				Limb limb = b.LimbByName(Semen.Spots[list.Count % Semen.Spots.Length]);
				if (!limb || limb.dismembered)
				{
					limb = b.limbs[1];
				}
				SpriteRenderer component2 = limb.GetComponent<SpriteRenderer>();
				GameObject gameObject = new GameObject("LewdSplat");
				gameObject.transform.SetParent(limb.transform, false);
				Vector3 vector = ((component2 && component2.sprite) ? component2.sprite.bounds.extents : (Vector3.one * 0.2f));
				Vector3 vector2 = ((component2 && component2.sprite) ? component2.sprite.bounds.center : Vector3.zero);
				gameObject.transform.localPosition = vector2 + new Vector3(Random.Range(-vector.x, vector.x) * 0.6f, Random.Range(-vector.y, vector.y) * 0.6f, 0f);
				SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = Semen.sprites[Random.Range(0, Semen.sprites.Length)];
				spriteRenderer.flipX = Random.value < 0.5f;
				if (component2)
				{
					spriteRenderer.sharedMaterial = component2.sharedMaterial;
					spriteRenderer.sortingLayerID = component2.sortingLayerID;
					spriteRenderer.sortingOrder = component2.sortingOrder + 12;
				}
				list.Add(spriteRenderer);
			}
			while (list.Count > num)
			{
				Object.Destroy(list[list.Count - 1].gameObject);
				list.RemoveAt(list.Count - 1);
			}
			float num2 = Mathf.Clamp01(cum / 30f) * 0.9f;
			foreach (SpriteRenderer spriteRenderer2 in list)
			{
				if (spriteRenderer2)
				{
					spriteRenderer2.color = new Color(1f, 1f, 1f, num2);
				}
			}
		}

		// Token: 0x04000630 RID: 1584
		private static readonly List<Texture2D> textures = new List<Texture2D>();

		// Token: 0x04000631 RID: 1585
		private static Sprite[] sprites = new Sprite[0];

		// Token: 0x04000632 RID: 1586
		private static float ppu = -1f;

		// Token: 0x04000633 RID: 1587
		private static readonly Dictionary<Body, List<SpriteRenderer>> decalSets = new Dictionary<Body, List<SpriteRenderer>>();

		// Token: 0x04000634 RID: 1588
		private static readonly string[] Spots = new string[]
		{
			"UpTorso", "UpTorso", "DownTorso", "DownTorso", "ThighF", "ThighB", "Head", "UpTorso", "ThighF", "DownArmF",
			"UpArmF", "CrusF"
		};
	}
}
