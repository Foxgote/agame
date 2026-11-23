using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LewdStatus;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x0200002E RID: 46
	internal static class Deal
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x0000CAAC File Offset: 0x0000ACAC
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x0000CAB3 File Offset: 0x0000ACB3
		public static Scavenger Offerer { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x0000CABB File Offset: 0x0000ACBB
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x0000CAC2 File Offset: 0x0000ACC2
		public static bool Accepted { get; private set; }

		// Token: 0x060000CA RID: 202 RVA: 0x0000CACA File Offset: 0x0000ACCA
		public static bool IsFor(Scavenger s)
		{
			return s && Deal.Offerer == s && Deal.Accepted;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000CB RID: 203 RVA: 0x0000CAE8 File Offset: 0x0000ACE8
		public static bool Unlocks
		{
			get
			{
				return Deal.itemId == "__unlock";
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000CAFC File Offset: 0x0000ACFC
		private static string DealLine(string key, Scavenger s)
		{
			if (!(Deal.itemId == "__unlock"))
			{
				return Scavengers.Line(key, s.female).Replace("<item>", Deal.ItemName(Deal.itemId));
			}
			return Scavengers.Line(key + "_unlock", s.female);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x0000CB54 File Offset: 0x0000AD54
		public static bool TryOffer(Scavenger s, Body b, bool force = false)
		{
			bool flag = !Diff.LethalScenes;
			if (!Plugin.DealEnabled.Value || s.slimeHost || (!s.lecher && !flag) || s.offered || Deal.Offerer || !b || !b.alive)
			{
				return false;
			}
			if (Capture.Active || Scenes.Busy || !LewdLink.Present || (Time.time < Deal.cooldownUntil && !force))
			{
				return false;
			}
			s.offered = true;
			float num = (Diff.LethalScenes ? Plugin.DealChance.Value : Plugin.DealChanceNoDeath.Value);
			if (!force && Random.value >= num)
			{
				return false;
			}
			string text;
			Deal.itemId = Deal.Pick(b, out text);
			Deal.Offerer = s;
			Deal.Accepted = (Deal.Earned = false);
			Deal.timer = Mathf.Max(5f, Plugin.DealSeconds.Value);
			s.BarkText(Deal.DealLine("deal_offer", s) + "  [" + Plugin.SurrenderKey.Value.ToString() + "]", Deal.timer);
			Plugin.Log.LogInfo(string.Format("A raider offers her a deal: {0} ({1}), she has {2:0} s to say yes", Deal.itemId, text, Deal.timer));
			return true;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000CCA4 File Offset: 0x0000AEA4
		public static void Update(Body b, float dt)
		{
			if (!Deal.Offerer)
			{
				Deal.Clear();
				return;
			}
			if (Deal.Offerer.mode == Scavenger.Mode.Dead || !b || !b.alive)
			{
				Deal.Clear();
				return;
			}
			if (Deal.Accepted)
			{
				if (!Capture.Active && !Capture.Surrendered)
				{
					Plugin.Log.LogInfo("The deal fell through (he never got to her)");
					Deal.Clear();
				}
				return;
			}
			Deal.timer -= dt;
			bool flag = Vector2.Distance(Deal.Offerer.pos, b.transform.position) > 28f;
			if (Deal.timer > 0f && !flag)
			{
				return;
			}
			Scavenger offerer = Deal.Offerer;
			Deal.Clear();
			Deal.cooldownUntil = Time.time + 30f;
			if (Random.value < Plugin.DealAngerChance.Value)
			{
				offerer.Bark("deal_angry", true);
				offerer.TurnOn(b);
				Plugin.Log.LogInfo("She said nothing to the deal: the raider takes it badly");
				return;
			}
			offerer.Bark("deal_refused", true);
			offerer.LetGo(false);
			Plugin.Log.LogInfo("She said nothing to the deal: the raider walks off");
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000CDC4 File Offset: 0x0000AFC4
		public static bool Accept(Body b)
		{
			if (!Deal.Offerer || Deal.Accepted || !b || !b.alive)
			{
				return false;
			}
			if (Vector2.Distance(Deal.Offerer.pos, b.transform.position) > 30f)
			{
				return false;
			}
			Deal.Accepted = true;
			Deal.Offerer.BarkText(null, 0f);
			Deal.Offerer.Bark("deal_accepted", true);
			b.talker.Talk(Plugin.English ? "Fine... deal. But you keep your word." : "Ладно... договорились. Только не обмани.", null, false, false);
			Capture.SurrenderTo(b, Deal.Offerer);
			Plugin.Log.LogInfo("She took the deal (" + Deal.itemId + ")");
			return true;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000CE90 File Offset: 0x0000B090
		public static bool Propose(Body b)
		{
			if (!Plugin.DealEnabled.Value || !b || !b.alive || Deal.Offerer || Capture.Active || Scenes.Busy || !LewdLink.Present)
			{
				return false;
			}
			if (Camps.Propose(b))
			{
				return true;
			}
			Vector2 at = b.limbs[1].transform.position;
			Scavenger scavenger = (from x in Scavengers.All
				where x && !x.slimeHost && !Camps.IsCampSide(x) && x.mode != Scavenger.Mode.Dead && !x.Holding && Vector2.Distance(x.pos, at) < ((x.mode == Scavenger.Mode.Fight || x.mode == Scavenger.Mode.Alert) ? 25f : 15f)
				orderby Vector2.Distance(x.pos, at)
				select x).FirstOrDefault<Scavenger>();
			if (!scavenger)
			{
				Plugin.Log.LogInfo("Her offer: no raider near enough to hear her");
				return false;
			}
			b.talker.Talk(Plugin.English ? "Wait! Don't... Let's make a deal. You can have me." : "Подожди! Не надо... Давай договоримся. Можешь взять меня.", null, false, false);
			if (Time.time < scavenger.refusedUntil)
			{
				scavenger.Bark("deal_no", true);
				return true;
			}
			float num = (scavenger.lecher ? 0.8f : (scavenger.grabber ? 0.5f : 0.3f)) * (Diff.LethalScenes ? 1f : 1.5f);
			if (Random.value >= Mathf.Min(0.95f, num))
			{
				scavenger.refusedUntil = Time.time + 20f;
				scavenger.Bark("deal_no", true);
				Plugin.Log.LogInfo(string.Format("Her offer: the raider turns it down (chance {0:P0})", Mathf.Min(0.95f, num)));
				return true;
			}
			string text;
			Deal.itemId = Deal.Pick(b, out text);
			Deal.Offerer = scavenger;
			scavenger.offered = true;
			Deal.Accepted = true;
			Deal.Earned = false;
			scavenger.BarkText(Deal.DealLine("deal_taken", scavenger), 4f);
			Capture.SurrenderTo(b, scavenger);
			Plugin.Log.LogInfo(string.Format("Her offer: the raider takes it, he will give {0} ({1}; chance {2:P0})", Deal.itemId, text, Mathf.Min(0.95f, num)));
			return true;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000D086 File Offset: 0x0000B286
		public static void ForCamp(Body b, Scavenger guard)
		{
			Deal.itemId = "__camp";
			Deal.Offerer = guard;
			guard.offered = true;
			Deal.Accepted = true;
			Deal.Earned = false;
			guard.Bark("deal_accepted", true);
			Capture.SurrenderTo(b, guard);
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000D0BE File Offset: 0x0000B2BE
		public static void Cancel(Scavenger s)
		{
			if (s != Deal.Offerer)
			{
				return;
			}
			Plugin.Log.LogInfo("She attacked the raider: the deal is off");
			s.BarkText(null, 0f);
			Deal.Clear();
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000D0F0 File Offset: 0x0000B2F0
		public static void Settle(Body b, Scavenger s, bool escaped)
		{
			if (!Deal.IsFor(s))
			{
				return;
			}
			bool flag = Deal.Earned && !escaped && b && b.alive;
			if (flag && Deal.itemId == "__camp")
			{
				Camps.Pay(b);
			}
			else if (flag && Deal.itemId == "__unlock")
			{
				List<Item> list = Devices.LockedOn(b);
				foreach (Item item in list)
				{
					Devices.Release(b, item, true);
				}
				s.BarkText(Scavengers.Line("deal_paid_unlock", s.female), 4f);
				Plugin.Log.LogInfo(string.Format("The raider kept his word: took {0} restraints off her", list.Count));
			}
			else if (flag)
			{
				Vector2 vector = b.limbs[1].transform.position + Vector2.up * 0.8f;
				GameObject gameObject = Scavengers.Throw(Deal.itemId, vector, Random.Range(0.8f, 1f));
				s.Bark("deal_paid", true);
				Plugin.Log.LogInfo(gameObject ? ("The raider kept his word: " + Deal.itemId + " by her") : ("The raider meant to pay with " + Deal.itemId + ", but there is no such item"));
			}
			else
			{
				Plugin.Log.LogInfo("The deal ended without pay");
			}
			Deal.Clear();
			Deal.cooldownUntil = Time.time + 90f;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x0000D2A0 File Offset: 0x0000B4A0
		private static void Clear()
		{
			Deal.Offerer = null;
			Deal.Accepted = (Deal.Earned = false);
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000D2B4 File Offset: 0x0000B4B4
		private static string ItemName(string id)
		{
			string item = Locale.GetItem(id);
			if (!string.IsNullOrEmpty(item) && !(item == id))
			{
				return item.ToLowerInvariant();
			}
			return id;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000D2E4 File Offset: 0x0000B4E4
		public static string Pick(Body b, out string why)
		{
			if (Devices.On && Devices.LockedOn(b).Count > 0)
			{
				why = "locked in restraints";
				return "__unlock";
			}
			List<ValueTuple<float, string, string>> list = new List<ValueTuple<float, string, string>>();
			float num = b.limbs.Where<Limb>((Limb l) => l && !l.dismembered).Sum<Limb>((Limb l) => l.bleedAmount);
			if (num > 2.5f)
			{
				list.Add(new ValueTuple<float, string, string>(100f, "tourniquet", "bleeding hard"));
			}
			else if (num > 0.4f)
			{
				list.Add(new ValueTuple<float, string, string>(45f, "bandage", "bleeding"));
			}
			if (b.limbs.Any<Limb>((Limb l) => l && !l.dismembered && (l.infected || l.infectionAmount > 10f)))
			{
				list.Add(new ValueTuple<float, string, string>(80f, "antibiotics", "infection"));
			}
			if (b.thirst < 35f)
			{
				list.Add(new ValueTuple<float, string, string>(70f, "waterbottle", "very thirsty"));
			}
			else if (b.thirst < 55f)
			{
				list.Add(new ValueTuple<float, string, string>(40f, "waterbottle", "thirsty"));
			}
			if (b.hunger < 35f)
			{
				list.Add(new ValueTuple<float, string, string>(65f, Deal.Any(new string[] { "pemmican", "nutrientbar", "chocolatebar" }), "starving"));
			}
			else if (b.hunger < 50f)
			{
				list.Add(new ValueTuple<float, string, string>(35f, Deal.Any(new string[] { "nutrientbar", "chocolatebar", "cereal" }), "hungry"));
			}
			bool flag = NudeMood.Covered(b) == NudeMood.Cover.Naked;
			if (b.temperature < 35.5f)
			{
				list.Add(new ValueTuple<float, string, string>(60f, flag ? "hoodie" : "sleepingbag", "cold"));
			}
			else if (flag)
			{
				list.Add(new ValueTuple<float, string, string>(28f, "hoodie", "naked"));
			}
			if (b.averagePain > 30f)
			{
				list.Add(new ValueTuple<float, string, string>(50f, "painkillers", "in pain"));
			}
			List<Item> list2 = (from i in b.GetAllItems()
				where i
				select i).ToList<Item>();
			GunScript gun = list2.Select<Item, GunScript>((Item i) => i.GetComponent<GunScript>()).FirstOrDefault<GunScript>((GunScript g) => g);
			if (gun && !list2.Any<Item>(delegate(Item i)
			{
				AmmoScript ammoScript;
				return i.TryGetComponent<AmmoScript>(ref ammoScript) && ammoScript.ammoType == gun.ammoType;
			}))
			{
				list.Add(new ValueTuple<float, string, string>(30f, Deal.AmmoFor(gun.ammoType), "a gun with no ammo"));
			}
			string[] lights = new string[] { "flashlight", "emergencylight", "headlamp", "makeshiftheadlamp", "lantern", "torch" };
			if (!list2.Any<Item>((Item i) => lights.Contains(i.id)) && !b.GetAllWearables().Any<Item>((Item w) => w && lights.Contains(w.id)))
			{
				list.Add(new ValueTuple<float, string, string>(25f, "flashlight", "no light"));
			}
			string[] blades = new string[] { "flimsyknife", "machete", "titaniummachete", "crudecleaver" };
			if (!gun && !list2.Any<Item>((Item i) => blades.Contains(i.id)))
			{
				list.Add(new ValueTuple<float, string, string>(20f, Deal.Any(new string[] { "flimsyknife", "machete" }), "unarmed"));
			}
			list = list.Where<ValueTuple<float, string, string>>(([TupleElementNames(new string[] { "score", "id", "why" })] ValueTuple<float, string, string> w) => Resources.Load(w.Item2) is GameObject).ToList<ValueTuple<float, string, string>>();
			if (list.Count == 0)
			{
				why = "nothing pressing";
				return Deal.Any(new string[] { "medkit", "energydrink", "bandage", "nutrientbar", "waterbottle", "painkillers", "smallbattery", "antibiotics" });
			}
			ValueTuple<float, string, string> valueTuple = list.OrderByDescending<ValueTuple<float, string, string>, float>(([TupleElementNames(new string[] { "score", "id", "why" })] ValueTuple<float, string, string> w) => w.Item1 + Random.value * 5f).First<ValueTuple<float, string, string>>();
			why = valueTuple.Item3;
			return valueTuple.Item2;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000D7E3 File Offset: 0x0000B9E3
		private static string Any(params string[] ids)
		{
			return ids[Random.Range(0, ids.Length)];
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000D7F0 File Offset: 0x0000B9F0
		private static string AmmoFor(GunScript.AmmoType t)
		{
			switch (t)
			{
			case 0:
				return "arm_box9mm";
			case 1:
				return "arm_box556";
			case 2:
				return "boxof12gauge";
			default:
				if (t != 3)
				{
					return AmmoScript.AmmoTypeToMagazine(t);
				}
				return "arm_box45";
			}
		}

		// Token: 0x040000E2 RID: 226
		public static bool Earned;

		// Token: 0x040000E3 RID: 227
		private static string itemId;

		// Token: 0x040000E4 RID: 228
		private static float timer;

		// Token: 0x040000E5 RID: 229
		private static float cooldownUntil;

		// Token: 0x040000E6 RID: 230
		public const string Unlock = "__unlock";
	}
}
