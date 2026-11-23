using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using LewdStatus;
using PoseKit;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x0200002C RID: 44
	internal static class Capture
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x0000AED3 File Offset: 0x000090D3
		// (set) Token: 0x060000AA RID: 170 RVA: 0x0000AEDA File Offset: 0x000090DA
		public static Scavenger Holder { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000AB RID: 171 RVA: 0x0000AEE2 File Offset: 0x000090E2
		public static bool Active
		{
			get
			{
				return Capture.Holder;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000AEEE File Offset: 0x000090EE
		// (set) Token: 0x060000AD RID: 173 RVA: 0x0000AEF5 File Offset: 0x000090F5
		public static bool Surrendered { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000AE RID: 174 RVA: 0x0000AEFD File Offset: 0x000090FD
		// (set) Token: 0x060000AF RID: 175 RVA: 0x0000AF04 File Offset: 0x00009104
		[TupleElementNames(new string[] { "a", "b", "t" })]
		public static ValueTuple<ScenePose, ScenePose, float> Scene
		{
			[return: TupleElementNames(new string[] { "a", "b", "t" })]
			get;
			[param: TupleElementNames(new string[] { "a", "b", "t" })]
			private set;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000AF0C File Offset: 0x0000910C
		public static bool CanGrab(Body b)
		{
			return !Capture.Active && b && b.alive && !Capture.LewdBusy() && Time.time > Capture.nextAllowed;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x0000AF3C File Offset: 0x0000913C
		public static bool HostWantsHer
		{
			get
			{
				return LewdSettings.ParasitesOn && !HostConversion.Active && Time.time > HostConversion.CalmUntil && !Suit.HostTamed && (Suit.Stage == 0 || (Suit.Slime && (!Suit.HostSlime || Suit.Stage < 5)));
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000AF8F File Offset: 0x0000918F
		private static bool LewdBusy()
		{
			return Scenes.Busy;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000AF98 File Offset: 0x00009198
		public static void Surrender(Body b)
		{
			if (Capture.Active || !b || !b.alive)
			{
				return;
			}
			if (Ultimatum.NoMercy)
			{
				Ultimatum.Refuse(b);
				return;
			}
			Vector2 at = b.transform.position;
			Scavenger scavenger = (from x in Scavengers.All
				where x && x.mode != Scavenger.Mode.Dead && !x.slimeHost && Vector2.Distance(x.pos, at) < 30f
				orderby Vector2.Distance(x.pos, at)
				select x).FirstOrDefault<Scavenger>() ?? (from x in Scavengers.All
				where x && x.mode != Scavenger.Mode.Dead && x.slimeHost && (x.mode == Scavenger.Mode.Fight || x.mode == Scavenger.Mode.Alert) && Vector2.Distance(x.pos, at) < 25f
				orderby Vector2.Distance(x.pos, at)
				select x).FirstOrDefault<Scavenger>();
			if (!scavenger)
			{
				if (Time.time - Capture.nobodyLine > 4f)
				{
					Capture.nobodyLine = Time.time;
					b.talker.Talk(Plugin.English ? "There's nobody to give up to..." : "Никого нет... некому сдаваться.", null, false, false);
				}
				Plugin.Log.LogInfo("Surrender: nobody close by");
				return;
			}
			scavenger.Bark("scav_surrender_seen");
			Capture.SurrenderTo(b, scavenger);
			b.talker.Talk(Plugin.English ? "Okay! Okay, I give up!" : "Всё! Всё, я сдаюсь!", null, false, false);
			Plugin.Log.LogInfo("She surrenders to a scavenger (" + scavenger.weapon.Id + (scavenger.slimeHost ? ", a slime host" : "") + ")");
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000B104 File Offset: 0x00009304
		public static void SurrenderTo(Body b, Scavenger s)
		{
			s.ComeFor(b);
			Capture.surrenderGrace = 30f;
			Capture.nextAllowed = 0f;
			Capture.Surrendered = true;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000B127 File Offset: 0x00009327
		public static void BreakSurrender()
		{
			if (Capture.Active || !Capture.Surrendered)
			{
				return;
			}
			Capture.Surrendered = false;
			Capture.surrenderGrace = 0f;
			Plugin.Log.LogInfo("She attacked them: her surrender is off");
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000B158 File Offset: 0x00009358
		public static void Grab(Body b, Scavenger s)
		{
			Capture.Holder = s;
			Capture.plan.Clear();
			Capture.act = Capture.Act.Pin;
			Capture.actTimer = Random.Range(2.5f, 4f);
			Capture.anger = 0;
			Capture.orgasms = 0;
			Capture.sceneTime = 0f;
			Capture.struggleCooldown = (Capture.Surrendered ? 99f : 0.8f);
			Capture.posed = Plugin.PosedScenes.Value && Poses.All.Count > 0 && HerPuppet.Begin(b);
			if (!Capture.posed)
			{
				b.Ragdoll();
				Traverse.Create(b).Field("movingAllowed").SetValue(false);
			}
			b.eyePanicTime = 1f;
			b.adrenaline += 25f;
			Sound.Play("BSSwing" + Random.Range(1, 5).ToString(), b.transform.position, false, true, null, 1f, 1f, false, false);
			s.Bark(Capture.Surrendered ? "scav_take_surrendered" : "scav_grab");
			if (!Capture.Surrendered && !s.slimeHost)
			{
				LewdLink.Say(b, "scav_grabbed");
			}
			Plugin.Log.LogInfo(string.Concat(new string[]
			{
				"A scavenger (",
				s.skin,
				", ",
				s.weapon.Id,
				") has her",
				Capture.Surrendered ? " (she surrendered)" : ""
			}));
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000B2F8 File Offset: 0x000094F8
		private static bool BeltOff(Body b, Scavenger s, bool hasKey)
		{
			Item item = Devices.BeltOn(b);
			if (!item)
			{
				return true;
			}
			if (!Devices.IsLocked(item) || hasKey)
			{
				Devices.Release(b, item, true);
				s.Bark(hasKey ? "scav_belt_unlock" : "scav_belt_off", true);
				Plugin.Log.LogInfo("The scavenger takes her " + (hasKey ? "locked" : "open") + " chastity belt off");
				return true;
			}
			s.Bark("scav_belted", true);
			Plugin.Log.LogInfo("Her chastity belt is locked: the scavenger cannot use her");
			return false;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x0000B388 File Offset: 0x00009588
		private static void Decide(Body b)
		{
			Scavenger holder = Capture.Holder;
			if (holder.slimeHost)
			{
				Capture.plan.Enqueue(Capture.Act.Infect);
				Capture.plan.Enqueue(Capture.Act.Release);
				Plugin.Log.LogInfo("The slime host holds her to give her its slime");
				return;
			}
			if (!Deal.IsFor(holder))
			{
				bool flag = LewdLink.Present && holder.lecher;
				List<Capture.Act> list = new List<Capture.Act>();
				bool flag2 = b.GetAllItems().Any<Item>((Item i) => i);
				bool flag3 = b.GetAllWearables().Any<Item>((Item w) => w);
				NudeMood.Cover cover = NudeMood.Covered(b);
				if (!flag && LewdLink.Present && !flag2 && !flag3 && Random.value < Plugin.NakedUseChance.Value)
				{
					flag = true;
					holder.Bark("scav_naked_tempted");
				}
				float num = ((cover == NudeMood.Cover.Naked) ? 1.4f : ((cover == NudeMood.Cover.Underwear) ? 1.2f : 1f));
				float value = Random.value;
				if (flag2 && (value < 0.75f || !flag))
				{
					list.Add(Capture.Act.Rob);
				}
				if (flag3 && Random.value < (flag ? 0.6f : 0.35f))
				{
					list.Add(Capture.Act.Strip);
				}
				if (flag && Random.value < Plugin.CaptureUseChance.Value * num * (Capture.Surrendered ? 1.1f : 1f))
				{
					list.Add(Capture.Act.Use);
				}
				if (list.Count == 0)
				{
					list.Add(flag2 ? Capture.Act.Rob : (flag ? Capture.Act.Use : Capture.Act.Strip));
				}
				if (list.Contains(Capture.Act.Use) && !Capture.BeltOff(b, holder, false))
				{
					list.Remove(Capture.Act.Use);
					if (list.Count == 0)
					{
						list.Add(flag2 ? Capture.Act.Rob : Capture.Act.Strip);
					}
				}
				if (Capture.Surrendered && Ultimatum.Bleeding(b) && Random.value < Plugin.TreatChance.Value)
				{
					Capture.plan.Enqueue(Capture.Act.Treat);
				}
				foreach (Capture.Act act in list)
				{
					Capture.plan.Enqueue(act);
				}
				if (Devices.WantsToLock(Capture.Surrendered, flag))
				{
					Capture.plan.Enqueue(Capture.Act.Lock);
				}
				float num2 = Plugin.CaptureKillChance.Value + (float)Capture.anger * 0.08f;
				num2 = ((Diff.LethalScenes && !Capture.Surrendered) ? (num2 * Diff.Lethal) : 0f);
				Capture.plan.Enqueue((Random.value < Mathf.Clamp01(num2)) ? Capture.Act.Kill : Capture.Act.Release);
				Plugin.Log.LogInfo(string.Format("The scavenger's plan: {0} (kill chance {1:P0}, anger {2}, she is {3}, items {4}, lecher {5})", new object[]
				{
					string.Join<Capture.Act>(" > ", Capture.plan),
					Mathf.Clamp01(num2),
					Capture.anger,
					cover,
					flag2,
					holder.lecher
				}));
				return;
			}
			if (!Capture.BeltOff(b, holder, Deal.Unlocks))
			{
				Capture.plan.Enqueue(Capture.Act.Release);
				Plugin.Log.LogInfo("The scavenger's plan: Release (a deal, but her belt is locked)");
				return;
			}
			Capture.plan.Enqueue(Capture.Act.Use);
			Capture.plan.Enqueue(Capture.Act.Release);
			Plugin.Log.LogInfo("The scavenger's plan: Use > Release (a deal)");
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000B6E8 File Offset: 0x000098E8
		public static void Update(Body b, float dt)
		{
			Capture.surrenderGrace = Mathf.Max(0f, Capture.surrenderGrace - dt);
			if (!Capture.Active)
			{
				if (Capture.surrenderGrace <= 0f)
				{
					Capture.Surrendered = false;
				}
				return;
			}
			Scavenger holder = Capture.Holder;
			if (!holder || holder.mode == Scavenger.Mode.Dead || !b)
			{
				Capture.Release(b, false, false);
				return;
			}
			if (!b.alive)
			{
				Capture.Release(b, true, false);
				return;
			}
			if (!Capture.posed)
			{
				b.shock = Mathf.Max(b.shock, 25f);
			}
			Capture.struggleCooldown -= dt;
			Capture.actTimer -= dt;
			Capture.sceneTime += dt;
			if (Keys.Down(Plugin.StruggleKey.Value))
			{
				Capture.Struggle(b);
			}
			if (!Capture.Active)
			{
				return;
			}
			if (Keys.Down(Plugin.SurrenderKey.Value) && !Capture.Surrendered)
			{
				Capture.Surrendered = true;
				Capture.struggleCooldown = 99f;
				holder.Bark("scav_take_surrendered");
				b.talker.Talk(Plugin.English ? "Fine... just don't hurt me." : "Ладно... только не бей.", null, false, false);
			}
			if (Capture.posed)
			{
				Capture.PoseScene(b, holder, dt);
			}
			else
			{
				Capture.Pose(b, holder, dt);
			}
			switch (Capture.act)
			{
			case Capture.Act.Pin:
				if (Capture.actTimer <= 0f)
				{
					Capture.Decide(b);
					Capture.Next(b);
					return;
				}
				break;
			case Capture.Act.Rob:
			case Capture.Act.Strip:
				if (Capture.actTimer <= 0f)
				{
					Capture.Take(b, holder, Capture.act == Capture.Act.Strip);
					Capture.Next(b);
					return;
				}
				break;
			case Capture.Act.Use:
				Capture.UpdateUse(b, holder, dt);
				return;
			case Capture.Act.Infect:
				Capture.UpdateInfect(b, holder, dt);
				return;
			case Capture.Act.Lock:
				if (Capture.actTimer <= 0f)
				{
					if (Devices.LockOn(b, holder.skin))
					{
						holder.hasKey = Random.value < Devices.KeyDropChance.Value;
						LewdLink.Say(b, "dd_raider_locked");
						Devices.LastLine = Time.time;
					}
					Capture.Next(b);
					return;
				}
				break;
			case Capture.Act.Kill:
				if (Capture.actTimer <= 0f)
				{
					if (Capture.Surrendered)
					{
						Capture.Release(b, false, false);
						return;
					}
					Capture.Kill(b, holder);
					return;
				}
				break;
			case Capture.Act.Release:
				if (Capture.actTimer <= 0f)
				{
					Capture.Release(b, false, false);
					return;
				}
				break;
			case Capture.Act.Treat:
				if (Capture.actTimer <= 0f)
				{
					Ultimatum.Treat(b, holder);
					Capture.Next(b);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000B954 File Offset: 0x00009B54
		private static void Next(Body b)
		{
			Capture.act = ((Capture.plan.Count > 0) ? Capture.plan.Dequeue() : Capture.Act.Release);
			if (Capture.act == Capture.Act.Kill && Capture.Surrendered)
			{
				Capture.act = Capture.Act.Release;
			}
			Scavenger holder = Capture.Holder;
			switch (Capture.act)
			{
			case Capture.Act.Rob:
				Capture.actTimer = 2.2f;
				holder.Bark("scav_rob");
				return;
			case Capture.Act.Strip:
				Capture.actTimer = 2.6f;
				holder.Bark("scav_strip");
				LewdLink.Say(b, "scav_stripped");
				return;
			case Capture.Act.Use:
				Capture.usedHer = true;
				Capture.actTimer = Random.Range(Plugin.CaptureUseSeconds.Value * 0.7f, Plugin.CaptureUseSeconds.Value * 1.2f);
				Capture.climaxAt = Capture.actTimer * Random.Range(0.25f, 0.4f);
				Capture.thrust = 0f;
				Capture.lineTimer = 2f;
				holder.Bark("scav_use");
				LewdLink.Say(b, Deal.IsFor(holder) ? "scav_use_deal" : (Capture.Surrendered ? "scav_use_gave_up" : "scav_use_start"));
				return;
			case Capture.Act.Infect:
				Capture.actTimer = 1.5f;
				Capture.lineTimer = 3f;
				holder.Bark("host_grab");
				LewdLink.Say(b, "host_grabbed");
				return;
			case Capture.Act.Lock:
				Capture.actTimer = 2.4f;
				holder.Bark("scav_lock");
				return;
			case Capture.Act.Kill:
				Capture.actTimer = 1.4f;
				holder.Bark("scav_kill");
				return;
			case Capture.Act.Release:
				Capture.actTimer = 1.2f;
				holder.Bark("scav_release");
				return;
			case Capture.Act.Treat:
				Capture.actTimer = 3f;
				holder.Bark("scav_treat");
				return;
			default:
				return;
			}
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000BB18 File Offset: 0x00009D18
		private static void PoseScene(Body b, Scavenger s, float dt)
		{
			string text = (s.slimeHost ? "held" : ((Capture.act == Capture.Act.Use) ? "bent" : "pinned"));
			float num = ((Capture.act == Capture.Act.Use) ? Mathf.Lerp(0.8f, 1.5f, Mathf.Clamp01(Capture.sceneTime / 25f)) : 1f);
			Capture.clock += dt * num;
			ValueTuple<ScenePose, ScenePose, float> valueTuple = Poses.At(text, Capture.clock, 1f);
			if (valueTuple.Item1 == null)
			{
				valueTuple = Poses.At("pinned", Capture.clock, 1f);
			}
			Capture.Scene = valueTuple;
			HerPuppet.Show(valueTuple.Item1, valueTuple.Item2, valueTuple.Item3, s.transform.position, s.facing);
			s.Holding = true;
			s.Thrust = ((Capture.act == Capture.Act.Use) ? valueTuple.Item3 : 0f);
			Capture.thrust = valueTuple.Item3 * 3.1415927f;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000BC18 File Offset: 0x00009E18
		private static void Pose(Body b, Scavenger s, float dt)
		{
			int facing = s.facing;
			Vector2 pos = s.pos;
			if (Capture.act == Capture.Act.Use)
			{
				float num = Mathf.Sin(Capture.thrust);
				Vector2 vector = pos + new Vector2((float)facing * (1.05f + num * 0.18f), 1.9f);
				Capture.Pull(b, "DownTorso", vector, 7f, dt);
				Capture.Pull(b, "UpTorso", pos + new Vector2((float)facing * 2.3f, 1.35f), 4f, dt);
				Capture.Pull(b, "Head", pos + new Vector2((float)facing * 3.2f, 1.25f + num * 0.08f), 3f, dt);
				Capture.Pull(b, "HandF", pos + new Vector2((float)facing * 3.1f, 0.25f), 4f, dt);
				Capture.Pull(b, "HandB", pos + new Vector2((float)facing * 2.8f, 0.25f), 4f, dt);
				Capture.Pull(b, "CrusF", pos + new Vector2((float)facing * 0.9f, 0.35f), 5f, dt);
				Capture.Pull(b, "CrusB", pos + new Vector2((float)facing * 1.3f, 0.35f), 5f, dt);
				s.Thrust = Mathf.Max(0f, num);
			}
			else
			{
				Vector2 vector2 = pos + new Vector2((float)facing * 1.8f, 0.5f);
				Capture.Pull(b, "UpTorso", vector2 + new Vector2((float)facing * 0.6f, 0.1f), 5f, dt);
				Capture.Pull(b, "DownTorso", vector2 + new Vector2((float)(-(float)facing) * 0.4f, 0f), 5f, dt);
				Capture.Pull(b, "HandF", pos + new Vector2((float)facing * 0.9f, 1.9f), (Capture.act == Capture.Act.Kill) ? 2f : 6f, dt);
				Capture.Pull(b, "HandB", pos + new Vector2((float)facing * 0.7f, 1.7f), 6f, dt);
				s.Thrust = 0f;
			}
			s.Holding = true;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000BE74 File Offset: 0x0000A074
		private static void Pull(Body b, string limb, Vector2 target, float stiffness, float dt)
		{
			Limb limb2 = b.LimbByName(limb);
			if (!limb2 || limb2.dismembered || !limb2.rb)
			{
				return;
			}
			Vector2 vector = (target - limb2.transform.position) * stiffness;
			limb2.rb.velocity = Vector2.Lerp(limb2.rb.velocity, Vector2.ClampMagnitude(vector, 14f), Mathf.Clamp01(dt * 12f));
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000BEF8 File Offset: 0x0000A0F8
		private static void Take(Body b, Scavenger s, bool clothes)
		{
			Item item;
			if (clothes)
			{
				item = (from w in b.GetAllWearables()
					where w && !Devices.IsLocked(w)
					select w).ToList<Item>().OrderBy<Item, int>(delegate(Item w)
				{
					if (w.Stats == null)
					{
						return 0;
					}
					return -w.Stats.wearableVisualOffset;
				}).FirstOrDefault<Item>();
				if (item)
				{
					b.DropWearable(item);
				}
			}
			else
			{
				item = (from i in b.GetAllItems()
					orderby (float)((i.GetComponent<GunScript>() ? 1000 : 0) + ((i.Stats != null) ? i.Stats.value : 0)) + Random.value * 15f descending
					select i).FirstOrDefault<Item>();
				if (item)
				{
					b.DropItem(item);
				}
			}
			if (!item)
			{
				Plugin.Log.LogInfo("Nothing to take (" + (clothes ? "clothes" : "items") + ")");
				return;
			}
			s.Stash(item);
			Sound.Play(clothes ? "ripclothing" : "gunloadmag", b.transform.position, false, true, b.transform, clothes ? 0.8f : 0.5f, 1f, false, false);
			LewdLink.Say(b, clothes ? "scav_stripped" : "scav_robbed");
			Plugin.Log.LogInfo("The scavenger took her " + item.id + (clothes ? " (worn)" : ""));
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000C06C File Offset: 0x0000A26C
		private static void UpdateUse(Body b, Scavenger s, float dt)
		{
			float num = Mathf.Lerp(2.4f, 4.4f, Mathf.Clamp01(Capture.sceneTime / 25f));
			float num2 = Mathf.Sin(Capture.thrust);
			if (!Capture.posed)
			{
				Capture.thrust += num * dt;
			}
			bool flag = (Capture.posed ? (Capture.lastT < 0.9f && Capture.Scene.Item3 >= 0.9f) : (num2 < 0.9f && Mathf.Sin(Capture.thrust) >= 0.9f));
			Capture.lastT = Capture.Scene.Item3;
			if (flag)
			{
				Sound.Play("goo", b.transform.position, false, true, b.transform, 0.35f, Random.Range(0.9f, 1.15f), false, false);
				LewdLink.Sound(b, 0.45f, new string[] { "wet_slap", "slime_squelch" });
				if (Random.value < 0.6f)
				{
					LewdLink.Sound(b, 0.6f, new string[]
					{
						(LewdLink.Arousal > 60f) ? "moan" : "moan_soft",
						"moan"
					});
				}
				b.stamina = Mathf.Max(0f, b.stamina - 1.5f);
			}
			LewdLink.Arousal += (Capture.Surrendered ? 2.6f : 1.8f) * dt;
			if (LewdLink.Arousal >= 99f && LewdLink.OrgasmTimer <= 0f && b.conscious)
			{
				LewdLink.Orgasm(b);
				Capture.orgasms++;
				LewdLink.Say(b, "scav_orgasm");
				s.Bark("scav_laugh");
			}
			if ((Capture.lineTimer -= dt) <= 0f)
			{
				Capture.lineTimer = Random.Range(6f, 10f);
				bool flag2 = Deal.IsFor(s);
				bool flag3 = LewdLink.Arousal > 70f || Plugin.S.corruption >= 50f;
				if (Random.value < 0.5f)
				{
					s.Bark(flag2 ? "deal_use_talk" : ((flag3 && Random.value < 0.5f) ? "scav_mock" : "scav_use_talk"));
				}
				else
				{
					LewdLink.Say(b, (flag3 && Random.value < 0.6f) ? "scav_use_enjoy" : (flag2 ? "scav_use_deal" : (Capture.Surrendered ? "scav_use_gave_up" : "scav_use")));
				}
			}
			if (Capture.climaxAt > 0f && Capture.actTimer <= Capture.climaxAt)
			{
				Capture.climaxAt = 0f;
				LewdLink.Fill(b, 30f);
				LewdLink.Semen(b, 15f);
				LewdLink.Sound(b, 0.7f, new string[] { "drip" });
				LewdLink.Say(b, "scav_climax");
				s.Bark("scav_climax");
				Plugin.Log.LogInfo(string.Format("The scavenger came in her ({0} orgasms of hers)", Capture.orgasms));
			}
			if (Capture.actTimer <= 0f)
			{
				s.Thrust = 0f;
				if (Deal.IsFor(s))
				{
					Deal.Earned = true;
				}
				Capture.Next(b);
			}
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x0000C3B0 File Offset: 0x0000A5B0
		private static void UpdateInfect(Body b, Scavenger s, float dt)
		{
			Capture.thrust += dt * 1.6f;
			s.Thrust = Mathf.Max(0f, Mathf.Sin(Capture.thrust)) * 0.4f;
			LewdLink.Arousal += 0.6f * dt;
			if (Random.value < dt * 0.8f)
			{
				LewdLink.Sound(b, 0.45f, new string[] { "slime_squelch", "slime_slurp", "slime_bubble" });
			}
			if ((Capture.lineTimer -= dt) <= 0f)
			{
				Capture.lineTimer = Random.Range(5f, 9f);
				if (Random.value < 0.5f)
				{
					s.Bark("host_talk");
				}
				else
				{
					LewdLink.Say(b, "host_held");
				}
			}
			if (Capture.actTimer > 0f)
			{
				return;
			}
			Capture.actTimer = Plugin.HostInfectSeconds.Value * Random.Range(0.8f, 1.2f);
			LewdLink.Infection infection = LewdLink.Infect(b);
			LewdLink.Sound(b, 0.8f, new string[] { "slime_gloop", "wet_slap" });
			Plugin.Log.LogInfo(string.Format("The slime host gives her its slime: {0} (suit stage {1})", infection, LewdLink.SuitStage));
			if (infection == LewdLink.Infection.Attached)
			{
				LewdLink.Say(b, "host_infected");
			}
			if (infection == LewdLink.Infection.Full || infection == LewdLink.Infection.Blocked)
			{
				s.Bark((infection == LewdLink.Infection.Full) ? "host_done" : "host_blocked");
				Capture.Next(b);
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0000C530 File Offset: 0x0000A730
		private static void Kill(Body b, Scavenger s)
		{
			if (Capture.posed)
			{
				HerPuppet.End();
				Capture.posed = false;
			}
			Limb limb = b.LimbByName("Head");
			if (s.weapon.Gun)
			{
				TurretScript.Shoot(new FireInfo
				{
					pos = b.transform.position + Vector3.up * 2f,
					dir = Vector2.down,
					ignoreTrans = (s.hit ? s.hit.transform : null),
					ignoreBody = true,
					forceHead = true,
					playerDamageMultiplier = 1.5f,
					structureDamage = 1f,
					animalDamage = 1f
				});
				Sound.Play("gunshot", b.transform.position, false, true, null, 1f, 1f, false, false);
			}
			if (limb)
			{
				limb.skinHealth -= 60f;
				limb.muscleHealth -= 60f;
				limb.bleedAmount += 20f;
			}
			b.brainHealth -= 250f;
			Sound.Play("gore" + Random.Range(1, 6).ToString(), b.transform.position, false, true, null, 1f, 1f, false, false);
			Plugin.Log.LogInfo("The scavenger killed her");
			Capture.Release(b, true, false);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000C6CC File Offset: 0x0000A8CC
		private static void Struggle(Body b)
		{
			if (Capture.struggleCooldown > 0f || !b.conscious)
			{
				return;
			}
			Capture.struggleCooldown = 1.1f;
			Scavenger holder = Capture.Holder;
			float num = Mathf.Clamp(b.stamina / 100f, 0.1f, 1f) * Mathf.Clamp01(b.consciousness / 100f + 0.2f);
			float num2 = Plugin.CaptureEscapeChance.Value * Diff.Escape * num * ((Capture.act == Capture.Act.Use) ? 0.6f : 1f) * ((LewdLink.OrgasmTimer > 0f) ? 0.3f : 1f);
			num2 = Mathf.Max(num2, 0.02f);
			b.stamina = Mathf.Max(0f, b.stamina - 12f);
			Capture.anger++;
			bool flag = Random.value < num2;
			Plugin.Log.LogInfo(string.Format("Struggling with the scavenger: chance {0:P0}, {1}", num2, flag ? "free" : "held"));
			if (flag)
			{
				holder.Bark("scav_escaped");
				RunStats.Escaped();
				holder.Stun(1.8f);
				Capture.Release(b, false, true);
				return;
			}
			Sound.Play("BSSwing" + Random.Range(1, 5).ToString(), b.transform.position, false, true, null, 0.6f, 1f, false, false);
			if (Random.value < 0.4f && Diff.LethalScenes)
			{
				Limb limb = b.LimbByName((Random.value < 0.5f) ? "Head" : "UpTorso");
				if (limb)
				{
					limb.muscleHealth -= 6f;
					limb.pain += 15f;
				}
				Sound.Play("gore" + Random.Range(1, 6).ToString(), b.transform.position, false, true, null, 1f, 1f, false, false);
				holder.Bark("scav_hit_her");
				return;
			}
			LewdLink.Say(b, "scav_struggle");
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000C900 File Offset: 0x0000AB00
		public static void Release(Body b, bool quiet, bool escaped = false)
		{
			Scavenger holder = Capture.Holder;
			Deal.Settle(b, holder, escaped);
			Capture.Holder = null;
			Capture.plan.Clear();
			Capture.Surrendered = false;
			Capture.nextAllowed = Time.time + (escaped ? 4f : 20f);
			if (holder)
			{
				holder.Holding = false;
				holder.Thrust = 0f;
				holder.LetGo(escaped);
				if (Capture.usedHer && !escaped && !holder.slimeHost && !Camps.IsCampSide(holder) && b && b.alive)
				{
					holder.Shoo();
				}
			}
			Capture.usedHer = false;
			if (Capture.posed)
			{
				HerPuppet.End();
				Capture.posed = false;
			}
			Capture.Scene = default(ValueTuple<ScenePose, ScenePose, float>);
			if (b)
			{
				Traverse.Create(b).Field("movingAllowed").SetValue(true);
			}
		}

		// Token: 0x040000CE RID: 206
		private static readonly Queue<Capture.Act> plan = new Queue<Capture.Act>();

		// Token: 0x040000CF RID: 207
		private static Capture.Act act;

		// Token: 0x040000D0 RID: 208
		private static float actTimer;

		// Token: 0x040000D1 RID: 209
		private static float struggleCooldown;

		// Token: 0x040000D2 RID: 210
		private static float thrust;

		// Token: 0x040000D3 RID: 211
		private static float lineTimer;

		// Token: 0x040000D4 RID: 212
		private static float sceneTime;

		// Token: 0x040000D5 RID: 213
		private static float climaxAt;

		// Token: 0x040000D6 RID: 214
		private static int anger;

		// Token: 0x040000D7 RID: 215
		private static int orgasms;

		// Token: 0x040000D8 RID: 216
		private static float surrenderGrace;

		// Token: 0x040000D9 RID: 217
		private static bool posed;

		// Token: 0x040000DA RID: 218
		private static float lastT;

		// Token: 0x040000DB RID: 219
		private static float clock;

		// Token: 0x040000DD RID: 221
		private static float nextAllowed;

		// Token: 0x040000DE RID: 222
		private static bool usedHer;

		// Token: 0x040000DF RID: 223
		private static float nobodyLine = -10f;

		// Token: 0x02000159 RID: 345
		private enum Act
		{
			// Token: 0x0400082F RID: 2095
			Pin,
			// Token: 0x04000830 RID: 2096
			Rob,
			// Token: 0x04000831 RID: 2097
			Strip,
			// Token: 0x04000832 RID: 2098
			Use,
			// Token: 0x04000833 RID: 2099
			Infect,
			// Token: 0x04000834 RID: 2100
			Lock,
			// Token: 0x04000835 RID: 2101
			Kill,
			// Token: 0x04000836 RID: 2102
			Release,
			// Token: 0x04000837 RID: 2103
			Treat
		}
	}
}
