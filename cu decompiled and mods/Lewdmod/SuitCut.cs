using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000062 RID: 98
	internal static class SuitCut
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060001EC RID: 492 RVA: 0x000288B1 File Offset: 0x00026AB1
		// (set) Token: 0x060001ED RID: 493 RVA: 0x000288B8 File Offset: 0x00026AB8
		public static bool Active { get; private set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060001EE RID: 494 RVA: 0x000288C0 File Offset: 0x00026AC0
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x060001EF RID: 495 RVA: 0x000288C8 File Offset: 0x00026AC8
		private static float Blade(Body b, out Item blade)
		{
			blade = null;
			float num = 0f;
			for (int i = 0; i < b.slots.Length; i++)
			{
				if (b.slots[i].isHand && b.HoldingItem(i))
				{
					Item item = b.GetItem(i);
					float num2;
					if (!item || item.Stats.qualities == null)
					{
						num2 = 0f;
					}
					else
					{
						num2 = (from c in item.Stats.qualities
							where c.id == "cutting"
							select c.amount).DefaultIfEmpty(0f).Max();
					}
					float num3 = num2;
					if (num3 > num)
					{
						num = num3;
						blade = item;
					}
				}
			}
			return num;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x000289A8 File Offset: 0x00026BA8
		public static bool TryStart(Body b)
		{
			if (SuitCut.Active)
			{
				SuitCut.Stop(b, "cut_stop");
				return true;
			}
			if (Suit.Latex)
			{
				return false;
			}
			Item item;
			float num = SuitCut.Blade(b, out item);
			if (num < 5f || item == null)
			{
				return false;
			}
			if (!b.conscious || !b.standing)
			{
				Relief.Say(b, "suit_nohands");
				return true;
			}
			SuitCut.Active = true;
			SuitCut.progress = 0f;
			SuitCut.pulse = 0f;
			SuitCut.clean = item.id == "lewd_scalpel";
			SuitCut.duration = Mathf.Clamp(Plugin.CutSeconds.Value * 30f / Mathf.Max(10f, num) * (1f + 0.15f * (float)Suit.Stage), 4f, 25f);
			Relief.ForceStop();
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			Relief.Say(b, "cut_start");
			Plugin.Log.LogInfo(string.Format("She starts cutting the suit off with {0} (cutting {1:0}): {2:0.0} s", item.id, num, SuitCut.duration));
			return true;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00028AD4 File Offset: 0x00026CD4
		public static void Update(Body b, float dt)
		{
			if (!SuitCut.Active)
			{
				return;
			}
			if (Suit.Stage == 0)
			{
				SuitCut.Stop(b, null);
				return;
			}
			if (!b.alive || !b.conscious || Scenes.Held)
			{
				SuitCut.Stop(b, null);
				return;
			}
			Item item;
			if (SuitCut.Blade(b, out item) < 5f)
			{
				SuitCut.Stop(b, "cut_stop");
				return;
			}
			SuitCut.progress += dt;
			Limb limb = b.limbs[1];
			Limb limb2 = b.LimbByName("DownTorso");
			float num = (SuitCut.clean ? 0.12f : 0.3f) * (1f + 0.2f * (float)Suit.Stage);
			foreach (Limb limb3 in new Limb[] { limb, limb2 })
			{
				if (limb3 && !limb3.dismembered)
				{
					limb3.pain = Mathf.Min(100f, limb3.pain + 4f * dt);
					limb3.skinHealth -= (SuitCut.clean ? 0.6f : 1.2f) * dt;
					limb3.bleedAmount += num * dt;
				}
			}
			if ((SuitCut.pulse -= dt) <= 0f)
			{
				SuitCut.pulse = Random.Range(1.2f, 2.2f);
				Sfx.PlayMix(b, 0.6f, new string[] { "wet_slap", "slime_squelch" });
				Sound.Play("ripclothing", b.transform.position, false, true, b.transform, 0.5f, 1f, false, false);
				b.eyePanicTime = Mathf.Max(b.eyePanicTime, 0.3f);
			}
			if (SuitCut.progress >= SuitCut.duration)
			{
				SuitCut.Finish(b);
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00028CAC File Offset: 0x00026EAC
		private static void Finish(Body b)
		{
			SuitCut.Active = false;
			Traverse.Create(b).Field("movingAllowed").SetValue(true);
			Suit.Strip(b);
			SuitCut.S.removeCooldown = 8f;
			Limb limb = b.limbs[1];
			Limb limb2 = b.LimbByName("DownTorso");
			if (Counter.UseDisinfectant(b, 15f))
			{
				foreach (Limb limb3 in new Limb[] { limb, limb2 })
				{
					if (limb3)
					{
						limb3.disinfectionTime = Mathf.Max(limb3.disinfectionTime, 600f);
					}
				}
				Relief.Say(b, "cut_clean");
				Plugin.Log.LogInfo("The suit is cut off; the cuts are disinfected");
			}
			else
			{
				foreach (Limb limb4 in new Limb[] { limb, limb2 })
				{
					if (limb4)
					{
						limb4.infectionAmount += (SuitCut.clean ? 8f : 18f);
					}
				}
				Relief.Say(b, "cut_dirty");
				Plugin.Log.LogInfo("The suit is cut off; nothing to disinfect the cuts with");
			}
			Sfx.PlayMix(b, 0.9f, new string[] { "wet_slap", "slime_gloop" });
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00028DF8 File Offset: 0x00026FF8
		private static void Stop(Body b, string line)
		{
			if (!SuitCut.Active)
			{
				return;
			}
			SuitCut.Active = false;
			if (b)
			{
				Traverse.Create(b).Field("movingAllowed").SetValue(true);
			}
			if (line != null && b)
			{
				Relief.Say(b, line);
			}
			Plugin.Log.LogInfo("She stopped cutting the suit");
		}

		// Token: 0x040001F1 RID: 497
		private static float progress;

		// Token: 0x040001F2 RID: 498
		private static float duration;

		// Token: 0x040001F3 RID: 499
		private static float pulse;

		// Token: 0x040001F4 RID: 500
		private static bool clean;
	}
}
