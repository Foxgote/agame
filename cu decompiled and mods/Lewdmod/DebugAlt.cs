using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using ScavArmory;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200006A RID: 106
	internal static class DebugAlt
	{
		// Token: 0x06000200 RID: 512 RVA: 0x000292C0 File Offset: 0x000274C0
		public static void Update(Body body)
		{
			Vector2 vector = body.transform.position + (body.isRight ? Vector2.right : Vector2.left) * 2.5f + Vector2.up;
			if (Devices.On)
			{
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.WristCuffs))
				{
					DebugAlt.Toggle(body, Devices.Kind.Wrists);
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.AnkleCuffs))
				{
					DebugAlt.Toggle(body, Devices.Kind.Ankles);
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.Gag))
				{
					DebugAlt.Toggle(body, Devices.Kind.Gag);
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.Collar))
				{
					DebugAlt.Toggle(body, Devices.Kind.Collar);
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.ChastityBelt))
				{
					DebugAlt.Toggle(body, Devices.Kind.Belt);
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.Straitjacket))
				{
					DebugAlt.Toggle(body, Devices.Kind.Jacket);
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.LockingLatex))
				{
					DebugAlt.Toggle(body, Devices.Kind.Suit);
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.Harness))
				{
					DebugAlt.Toggle(body, Devices.Kind.Harness);
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.AllRestraintsOff))
				{
					List<Item> list = (from k in Devices.Defs.Select<OutfitDef, Devices.Kind>((OutfitDef d) => d.Kind).Distinct<Devices.Kind>()
						select Devices.Worn(body, k) into i
						where i
						select i).ToList<Item>();
					foreach (Item item in list)
					{
						Devices.Release(body, item, true);
					}
					Plugin.Log.LogInfo(string.Format("Debug: took off {0} restraints", list.Count));
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.KeyAndLockpicks))
				{
					DebugAlt.Throw(Devices.Key.Id, vector);
					DebugAlt.Throw("lockpickingkit", vector + Vector2.right * 0.6f);
					Plugin.Log.LogInfo("Debug: a restraint key and lockpicks by her");
				}
			}
			if (LewdSettings.On)
			{
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.NextPlug))
				{
					DebugAlt.NextPlug(body);
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.BBSuit))
				{
					Plugin.Instance.DebugWear(body, Outfits.BBSuit);
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.SewingKit))
				{
					DebugAlt.Throw("lewd_sewingkit", vector);
					Plugin.Log.LogInfo("Debug: a sewing kit by her");
				}
				if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.SecondChance))
				{
					SecondChance.Take(body);
					Plugin.Log.LogInfo("Debug: her second chance");
				}
			}
			if (Plugin.ScavEnabled.Value && DebugAlt.<Update>g__Alt|2_0(DebugBinds.Camp))
			{
				Plugin.Log.LogInfo("Debug: " + Camps.DebugHere(body));
			}
			if (DebugAlt.<Update>g__Alt|2_0(DebugBinds.Trader))
			{
				DebugAlt.Trader(body);
			}
		}

		// Token: 0x06000201 RID: 513 RVA: 0x000295F4 File Offset: 0x000277F4
		private static void Toggle(Body b, Devices.Kind k)
		{
			Item item = Devices.Worn(b, k);
			if (item)
			{
				Devices.Release(b, item, true);
				Plugin.Log.LogInfo("Debug: took off " + item.id);
				return;
			}
			List<OutfitDef> list = Devices.Defs.Where<OutfitDef>((OutfitDef d) => d.Kind == k).ToList<OutfitDef>();
			if (list.Count == 0)
			{
				return;
			}
			OutfitDef outfitDef = list[Random.Range(0, list.Count)];
			bool flag = Devices.PutOn(b, outfitDef, k != Devices.Kind.Harness);
			Plugin.Log.LogInfo(string.Format("Debug: {0} on her{1}: {2}", outfitDef.Id, (k != Devices.Kind.Harness) ? " and locked" : "", flag));
		}

		// Token: 0x06000202 RID: 514 RVA: 0x000296CC File Offset: 0x000278CC
		private static void NextPlug(Body b)
		{
			foreach (Item item in (from w in b.GetAllWearables()
				where w && Plugs.IsPlug(w)
				select w).ToList<Item>())
			{
				if (Devices.IsLocked(item))
				{
					Devices.Release(b, item, true);
				}
				else
				{
					b.DropWearable(item);
				}
			}
			DebugAlt.plugIndex = (DebugAlt.plugIndex + 1) % (DebugAlt.PlugKinds.Length + 1);
			if (DebugAlt.plugIndex == DebugAlt.PlugKinds.Length)
			{
				Plugin.Log.LogInfo("Debug: no plug");
				return;
			}
			List<ToyDef> list = Toys.All.Where<ToyDef>((ToyDef t) => t.Kind == DebugAlt.PlugKinds[DebugAlt.plugIndex]).ToList<ToyDef>();
			if (list.Count == 0)
			{
				return;
			}
			ToyDef toyDef = list[Random.Range(0, list.Count)];
			GameObject gameObject = Toys.Template(toyDef);
			if (!gameObject)
			{
				return;
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, b.transform.position, Quaternion.identity);
			gameObject2.SetActive(true);
			Item component = gameObject2.GetComponent<Item>();
			if (component && component.Stats.wearable)
			{
				b.WearWearable(component);
			}
			Plugin.Log.LogInfo(string.Concat(new string[]
			{
				"Debug: ",
				toyDef.Id,
				" in her (",
				(component != null) ? component.Stats.wearSlotId : null,
				")"
			}));
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00029878 File Offset: 0x00027A78
		private static void Throw(string id, Vector2 at)
		{
			GameObject gameObject = Resources.Load(id) as GameObject;
			if (gameObject)
			{
				Object.Instantiate<GameObject>(gameObject, at, Quaternion.identity);
				return;
			}
			Plugin.Log.LogWarning("Debug: no item " + id);
		}

		// Token: 0x06000204 RID: 516 RVA: 0x000298C4 File Offset: 0x00027AC4
		private static void Trader(Body b)
		{
			GameObject gameObject = Resources.Load("trader" + Random.Range(1, 4).ToString()) as GameObject;
			if (!gameObject)
			{
				return;
			}
			TraderScript component = Object.Instantiate<GameObject>(gameObject, b.limbs[1].transform.position + new Vector2(b.isRight ? 3f : (-3f), 0f), Quaternion.identity).GetComponent<TraderScript>();
			if (component)
			{
				component.MeetPlayer();
			}
			Plugin.Log.LogInfo("Debug: a trader by her");
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0002999E File Offset: 0x00027B9E
		[CompilerGenerated]
		internal static bool <Update>g__Alt|2_0(ConfigEntry<KeyboardShortcut> k)
		{
			return DebugBinds.Down(k);
		}

		// Token: 0x040001F5 RID: 501
		private static int plugIndex = -1;

		// Token: 0x040001F6 RID: 502
		private static readonly string[] PlugKinds = new string[] { "plug", "tailplug", "inflplug", "vibe" };
	}
}
