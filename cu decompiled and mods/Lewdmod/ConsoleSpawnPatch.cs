using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ScavArmory;

namespace LewdStatus
{
	// Token: 0x0200005F RID: 95
	[HarmonyPatch(typeof(ConsoleScript), "RegisterSpawnEntities")]
	internal static class ConsoleSpawnPatch
	{
		// Token: 0x060001DD RID: 477 RVA: 0x00027EA8 File Offset: 0x000260A8
		public static IEnumerable<string> ModItemIds()
		{
			foreach (OutfitDef outfitDef in Outfits.All)
			{
				yield return outfitDef.Id;
			}
			OutfitDef[] array = null;
			foreach (ToyDef toyDef in Toys.All)
			{
				yield return toyDef.Id;
			}
			List<ToyDef>.Enumerator enumerator = default(List<ToyDef>.Enumerator);
			yield return "lewd_suppressant_pills";
			yield return "lewd_scalpel";
			yield return "lewd_sewingkit";
			yield return "lewd_lustpotion";
			yield return "lewd_antivenom";
			foreach (ArmDef armDef in Armory.All)
			{
				yield return armDef.Id;
			}
			ArmDef[] array2 = null;
			yield break;
			yield break;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00027EB4 File Offset: 0x000260B4
		private static void Postfix()
		{
			Command command = ConsoleScript.SearchExact("spawn");
			if (command == null)
			{
				return;
			}
			if (command.argAutofill == null)
			{
				command.argAutofill = new Dictionary<int, List<string>>();
			}
			List<string> list;
			if (!command.argAutofill.TryGetValue(0, out list))
			{
				list = (command.argAutofill[0] = new List<string>());
			}
			int count = list.Count;
			foreach (string text in ConsoleSpawnPatch.ModItemIds().Distinct<string>())
			{
				if (!string.IsNullOrEmpty(text) && !list.Contains(text))
				{
					list.Add(text);
				}
			}
			if (list.Count > count)
			{
				Plugin.Log.LogInfo(string.Format("Console: {0} of the mod's items added to the spawn list", list.Count - count));
			}
		}
	}
}
