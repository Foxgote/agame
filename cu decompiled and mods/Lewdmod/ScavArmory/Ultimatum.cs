using System;
using System.Linq;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x0200004A RID: 74
	internal static class Ultimatum
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000171 RID: 369 RVA: 0x000196A4 File Offset: 0x000178A4
		public static bool NoMercy
		{
			get
			{
				WorldGeneration world = WorldGeneration.world;
				return world && Ultimatum.noMercyWorld == world && Ultimatum.noMercyDepth == world.biomeDepth;
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000196DC File Offset: 0x000178DC
		public static bool Wounded(Body b)
		{
			return b && b.alive && (b.totalBleedSpeed > 0.06f || b.bloodVolume < 75f || b.averagePain > 45f || b.consciousness < 60f || !b.standing);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0001973C File Offset: 0x0001793C
		public static void Tick(Body b)
		{
			if (!b)
			{
				return;
			}
			if (b.totalBleedSpeed > Ultimatum.lastBleed + 0.01f)
			{
				Ultimatum.woundedAt = Time.time;
			}
			Ultimatum.lastBleed = Mathf.Lerp(Ultimatum.lastBleed, b.totalBleedSpeed, 0.2f);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0001978C File Offset: 0x0001798C
		public static bool HoldFire(Scavenger s, Body b)
		{
			if (!Plugin.UltimatumEnabled.Value || Ultimatum.NoMercy || s.slimeHost || Capture.Active)
			{
				return false;
			}
			if (Time.time - Ultimatum.woundedAt < 3f && b && b.rb && b.rb.velocity.magnitude < 1.5f && !Ultimatum.Wounded(b))
			{
				if (Time.time - Ultimatum.pauseCalled > 3f)
				{
					Ultimatum.pauseCalled = Time.time;
					s.Bark("scav_dontmove", true);
					Plugin.Log.LogInfo("She was hit and keeps still: the raider holds his fire a moment");
				}
				return true;
			}
			if (!Ultimatum.Wounded(b))
			{
				return false;
			}
			if (Time.time - Ultimatum.lastCall > 5f)
			{
				Ultimatum.lastCall = Time.time;
				if (Time.time - Ultimatum.calledAt > 30f)
				{
					Plugin.Log.LogInfo("She is badly hurt: the raider holds his fire and calls on her to give up");
				}
				Ultimatum.calledAt = Time.time;
				s.Bark("scav_ultimatum", true);
			}
			return true;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x000198A0 File Offset: 0x00017AA0
		public static void HerAttack(Body b)
		{
			if (Ultimatum.NoMercy || Time.time - Ultimatum.calledAt > 60f)
			{
				return;
			}
			WorldGeneration world = WorldGeneration.world;
			Ultimatum.noMercyWorld = world;
			Ultimatum.noMercyDepth = (world ? world.biomeDepth : (-1));
			Vector2 at = (b ? b.transform.position : Vector2.zero);
			Scavenger scavenger = (from x in Scavengers.All
				where x && !x.slimeHost && x.mode != Scavenger.Mode.Dead
				orderby Vector2.Distance(x.pos, at)
				select x).FirstOrDefault<Scavenger>();
			if (scavenger)
			{
				scavenger.Bark("scav_nomercy", true);
			}
			Plugin.Log.LogInfo("She fought on after the call to give up: no more surrender on this layer");
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00019978 File Offset: 0x00017B78
		public static void Refuse(Body b)
		{
			Vector2 at = b.transform.position;
			Scavenger scavenger = (from x in Scavengers.All
				where x && !x.slimeHost && x.mode != Scavenger.Mode.Dead && Vector2.Distance(x.pos, at) < 30f
				orderby Vector2.Distance(x.pos, at)
				select x).FirstOrDefault<Scavenger>();
			if (scavenger)
			{
				scavenger.Bark("scav_too_late", true);
			}
			b.talker.Talk(Plugin.English ? "I give up! I give up!.." : "Сдаюсь! Я сдаюсь!..", null, false, false);
			Plugin.Log.LogInfo("Surrender refused: she fought on after the call to give up");
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00019A14 File Offset: 0x00017C14
		private static bool IsArmOrLeg(Limb l)
		{
			return l && Ultimatum.ArmsAndLegs.Any<string>((string p) => l.name.StartsWith(p));
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00019A53 File Offset: 0x00017C53
		public static bool Bleeding(Body b)
		{
			if (b)
			{
				return b.limbs.Any<Limb>((Limb l) => l && !l.dismembered && l.bleedAmount > 0.6f);
			}
			return false;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00019A8C File Offset: 0x00017C8C
		public static void Treat(Body b, Scavenger s)
		{
			int num = 0;
			foreach (Limb limb in (from x in b.limbs
				where x && !x.dismembered && x.bleedAmount > 0.6f
				orderby x.bleedAmount descending
				select x).Take<Limb>(2).ToList<Limb>())
			{
				if (Ultimatum.IsArmOrLeg(limb) && !limb.GetComponent<TourniquetScript>() && limb.bleedAmount > 3f)
				{
					limb.gameObject.AddComponent<TourniquetScript>().condition = 1f;
					Plugin.Log.LogInfo("The raider puts a tourniquet on her " + limb.name);
				}
				else
				{
					limb.bandageSlowAmount += 10f;
					limb.skinHealAmount += 4f;
					limb.pain = Mathf.Max(0f, limb.pain - 10f);
					limb.bleedAmount *= 0.4f;
					limb.CreateTemporarySprite(Resources.Load<Sprite>("Special/bandageWrap"), 0f, new Color?(new Color32(143, 126, 139, byte.MaxValue)), true, 1000f, null);
					Plugin.Log.LogInfo("The raider bandages her " + limb.name);
				}
				num++;
			}
			if (num > 0)
			{
				Sound.Play("splint", b.transform.position, false, true, null, 1f, 1f, false, false);
				LewdLink.Say(b, "scav_treated");
			}
		}

		// Token: 0x04000182 RID: 386
		private static float calledAt = -99f;

		// Token: 0x04000183 RID: 387
		private static float lastCall = -99f;

		// Token: 0x04000184 RID: 388
		private static WorldGeneration noMercyWorld;

		// Token: 0x04000185 RID: 389
		private static int noMercyDepth = -1;

		// Token: 0x04000186 RID: 390
		private static float lastBleed;

		// Token: 0x04000187 RID: 391
		private static float woundedAt = -99f;

		// Token: 0x04000188 RID: 392
		private static float pauseCalled = -99f;

		// Token: 0x04000189 RID: 393
		private static readonly string[] ArmsAndLegs = new string[] { "UpArm", "DownArm", "Hand", "Thigh", "Crus", "Foot" };
	}
}
