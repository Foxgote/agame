using System;
using System.Collections.Generic;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x020000BA RID: 186
	[HarmonyPatch(typeof(ConsoleScript), "RegisterPlayerDetails")]
	internal static class SetBodyFieldAutofillPatch
	{
		// Token: 0x06000434 RID: 1076 RVA: 0x00044404 File Offset: 0x00042604
		private static void Postfix()
		{
			Command command = ConsoleScript.SearchExact("setbodyfield");
			List<string> list;
			if (((command != null) ? command.argAutofill : null) == null || !command.argAutofill.TryGetValue(0, out list))
			{
				return;
			}
			foreach (string text in SetBodyFieldPatch.Stats)
			{
				if (!list.Contains(text))
				{
					list.Add(text);
				}
			}
		}
	}
}
