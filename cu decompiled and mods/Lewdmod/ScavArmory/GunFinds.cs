using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000039 RID: 57
	internal static class GunFinds
	{
		// Token: 0x060000FA RID: 250 RVA: 0x0000E830 File Offset: 0x0000CA30
		public static void LowerIntelligence()
		{
			if (Item.GlobalItems == null)
			{
				return;
			}
			int num = Mathf.Max(0, Plugin.GunIntCut.Value);
			int num2 = 0;
			foreach (string text in GunFinds.GameIds.Concat<string>(Armory.All.Select<ArmDef, string>((ArmDef d) => d.Id)))
			{
				ItemInfo itemInfo;
				if (Item.GlobalItems.TryGetValue(text, out itemInfo) && itemInfo.rec != null)
				{
					int min = itemInfo.rec.min;
					int num3;
					if (!GunFinds.origMin.TryGetValue(text, out num3))
					{
						num3 = (GunFinds.origMin[text] = min);
					}
					itemInfo.rec = new Recognition(Mathf.Max(0, num3 - num));
					num2++;
				}
			}
			Plugin.Log.LogInfo(string.Format("Guns and ammo: {0} items need {1} less intelligence", num2, num));
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000E940 File Offset: 0x0000CB40
		public static void Picked(Body b, Item item)
		{
			if (!Plugin.GunFindLines.Value || !item || !b || !b.talker || !PlayerCamera.main || PlayerCamera.main.body != b)
			{
				return;
			}
			if (!item.GetComponent<GunScript>() || Time.timeSinceLevelLoad < 5f)
			{
				return;
			}
			if (!item.transform.IsChildOf(b.transform))
			{
				return;
			}
			if (Time.time - GunFinds.lastLine < 8f || !GunFinds.seen.Add(item.GetInstanceID()))
			{
				return;
			}
			GunFinds.lastLine = Time.time;
			string[] array = (Plugin.English ? GunFinds.En : GunFinds.Ru);
			b.talker.Talk(array[Random.Range(0, array.Length)], null, false, false);
		}

		// Token: 0x040000F9 RID: 249
		private static readonly string[] GameIds = new string[]
		{
			"pistol", "rifle", "makeshiftrifle", "shotgun", "smallmagazine", "riflemagazine", "magazinebase", "9mmround", "556round", "12gauge",
			"boxof12gauge", "casing"
		};

		// Token: 0x040000FA RID: 250
		private static readonly string[] Ru = new string[] { "Нихуя себе!", "Ни хрена себе... настоящая пушка.", "Ого, ствол!", "Опа... вот это я понимаю находка.", "Нихуя себе, пушка! Ну теперь поговорим.", "Так-так... кому тут жить надоело?" };

		// Token: 0x040000FB RID: 251
		private static readonly string[] En = new string[] { "Holy shit!", "Hell yeah... a real gun.", "Whoa, a gun!", "Oh... now that's what I call a find.", "Holy shit, a gun! Now we talk.", "Well, well... who here is tired of living?" };

		// Token: 0x040000FC RID: 252
		private static readonly HashSet<int> seen = new HashSet<int>();

		// Token: 0x040000FD RID: 253
		private static float lastLine = -99f;

		// Token: 0x040000FE RID: 254
		private static readonly Dictionary<string, int> origMin = new Dictionary<string, int>();
	}
}
