using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000132 RID: 306
	internal static class Belly
	{
		// Token: 0x0600074F RID: 1871 RVA: 0x000709FC File Offset: 0x0006EBFC
		public static void Load(string dir)
		{
			if (!Directory.Exists(dir))
			{
				return;
			}
			foreach (string text in Directory.GetFiles(dir, "*.png"))
			{
				Texture2D texture2D = new Texture2D(2, 2, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text));
				texture2D.name = Path.GetFileNameWithoutExtension(text);
				Belly.textures[texture2D.name] = texture2D;
			}
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x00070A72 File Offset: 0x0006EC72
		public static void Update(Body b)
		{
			Belly.Update(b, Womb.BellyLevel);
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x00070A80 File Offset: 0x0006EC80
		public static void Update(Body b, int level)
		{
			Limb limb = b.LimbByName("DownTorso");
			SpriteRenderer spriteRenderer = ((limb && !limb.dismembered) ? limb.GetComponent<SpriteRenderer>() : null);
			Belly.Shown shown;
			if (!Belly.shown.TryGetValue(b, out shown))
			{
				foreach (Body body in new List<Body>(Belly.shown.Keys))
				{
					if (!body)
					{
						Belly.shown.Remove(body);
					}
				}
				shown = (Belly.shown[b] = new Belly.Shown());
			}
			if (!shown.sr || shown.sr.transform.parent != (spriteRenderer ? spriteRenderer.transform : null))
			{
				if (shown.sr)
				{
					Object.Destroy(shown.sr.gameObject);
				}
				shown.sr = null;
				if (!spriteRenderer)
				{
					return;
				}
				GameObject gameObject = new GameObject("LewdBelly");
				gameObject.transform.SetParent(spriteRenderer.transform, false);
				shown.sr = gameObject.AddComponent<SpriteRenderer>();
				shown.sr.sharedMaterial = spriteRenderer.sharedMaterial;
			}
			if (!spriteRenderer || level == 0 || !spriteRenderer.enabled || !spriteRenderer.sprite)
			{
				shown.level = 0;
				shown.sr.enabled = false;
				return;
			}
			Sprite sprite = Belly.SpriteFor(spriteRenderer.sprite, level);
			shown.sr.enabled = sprite;
			if (level != shown.level)
			{
				shown.level = level;
				if (PlayerCamera.main && b == PlayerCamera.main.body)
				{
					Plugin.Log.LogInfo(string.Format("Belly level {0} ({1}{2})", level, SkinVoice.SkinName, sprite ? string.Format(", {0}x{1} px", sprite.rect.width, sprite.rect.height) : ", no sprite"));
				}
			}
			if (!sprite)
			{
				return;
			}
			shown.sr.sprite = sprite;
			shown.sr.flipX = spriteRenderer.flipX;
			shown.sr.flipY = spriteRenderer.flipY;
			shown.sr.color = spriteRenderer.color;
			shown.sr.sortingLayerID = spriteRenderer.sortingLayerID;
			shown.sr.sortingOrder = spriteRenderer.sortingOrder + 4;
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x00070D24 File Offset: 0x0006EF24
		private static Sprite SpriteFor(Sprite parent, int level)
		{
			Sprite sprite;
			if (Belly.sprites.TryGetValue(new ValueTuple<Sprite, int>(parent, level), out sprite))
			{
				return sprite;
			}
			string skinName = SkinVoice.SkinName;
			Texture2D texture2D;
			if (!Belly.textures.TryGetValue(skinName + "_" + level.ToString(), out texture2D) && !Belly.textures.TryGetValue("original_" + level.ToString(), out texture2D))
			{
				return null;
			}
			Rect rect = parent.rect;
			sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, Mathf.Min((float)texture2D.height, rect.height)), new Vector2(parent.pivot.x / (float)texture2D.width, parent.pivot.y / Mathf.Min((float)texture2D.height, rect.height)), parent.pixelsPerUnit);
			sprite.name = "belly_" + level.ToString();
			Belly.sprites[new ValueTuple<Sprite, int>(parent, level)] = sprite;
			return sprite;
		}

		// Token: 0x0400077A RID: 1914
		private static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

		// Token: 0x0400077B RID: 1915
		private static readonly Dictionary<ValueTuple<Sprite, int>, Sprite> sprites = new Dictionary<ValueTuple<Sprite, int>, Sprite>();

		// Token: 0x0400077C RID: 1916
		private static readonly Dictionary<Body, Belly.Shown> shown = new Dictionary<Body, Belly.Shown>();

		// Token: 0x02000255 RID: 597
		private sealed class Shown
		{
			// Token: 0x04000B6B RID: 2923
			public SpriteRenderer sr;

			// Token: 0x04000B6C RID: 2924
			public int level;
		}
	}
}
