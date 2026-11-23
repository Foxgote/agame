using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000E2 RID: 226
	internal static class Plugs
	{
		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000520 RID: 1312 RVA: 0x0005605B File Offset: 0x0005425B
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x00056064 File Offset: 0x00054264
		public static ToyDef Worn(Body b)
		{
			Item item = (b ? b.GetWearableBySlotID("dd_plug") : null);
			if (!item)
			{
				return null;
			}
			return Toys.ById(item.id);
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x000560A0 File Offset: 0x000542A0
		public static float SessionBoost(Body b)
		{
			ToyDef toyDef = Plugs.Worn(b);
			if (toyDef == null)
			{
				return 1f;
			}
			return 1f + (toyDef.Boost - 1f) * 0.6f;
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x000560D8 File Offset: 0x000542D8
		public static void Update(Body b, float dt)
		{
			if (Plugs.Vibe(b) != null && b.conscious)
			{
				Plugs.S.arousal = Mathf.Min(100f, Plugs.S.arousal + (0.25f + Plugs.S.arousal * 0.003f) * Suppressant.Factor * dt);
				Plugs.S.lust = Mathf.Min(100f, Plugs.S.lust + 0.03f * dt);
			}
			ToyDef toyDef = Plugs.Worn(b);
			if (toyDef == null || !b.conscious)
			{
				return;
			}
			float num = Mathf.Clamp01(b.rb.velocity.magnitude / 8f);
			float num2 = (b.standing ? 0f : 0.5f);
			float num3 = (0.12f + 0.5f * num + num2) * (toyDef.Boost - 0.8f) * Suppressant.Factor;
			Plugs.S.arousal = Mathf.Min(100f, Plugs.S.arousal + num3 * dt);
			Plugs.S.lust = Mathf.Min(100f, Plugs.S.lust + num3 * 0.15f * dt);
			if (toyDef.Size == "l" && Plugs.S.arousal < 40f)
			{
				Mood.Change(b, -0.004f * dt);
			}
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x00056240 File Offset: 0x00054440
		public static bool IsPlug(Item item)
		{
			if (item)
			{
				ToyDef toyDef = Toys.ById(item.id);
				if (toyDef != null)
				{
					return toyDef.Plug || toyDef.Kind == "vibe";
				}
			}
			return false;
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x00056280 File Offset: 0x00054480
		public static ToyDef Vibe(Body b)
		{
			Item item = (b ? b.GetWearableBySlotID("dd_vibe") : null);
			if (!item)
			{
				return null;
			}
			return Toys.ById(item.id);
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x000562BC File Offset: 0x000544BC
		public static void Hide(Item item, bool hidden)
		{
			SpriteRenderer spriteRenderer;
			if (item && item.TryGetComponent<SpriteRenderer>(ref spriteRenderer))
			{
				spriteRenderer.enabled = !hidden;
			}
		}

		// Token: 0x04000612 RID: 1554
		public const string Slot = "dd_plug";

		// Token: 0x04000613 RID: 1555
		public const string VibeSlot = "dd_vibe";
	}
}
