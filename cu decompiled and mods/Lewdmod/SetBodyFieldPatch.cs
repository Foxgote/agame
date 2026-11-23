using System;
using System.Globalization;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000B9 RID: 185
	[HarmonyPatch(typeof(ConsoleScript), "TryExecuteCommand")]
	internal static class SetBodyFieldPatch
	{
		// Token: 0x06000432 RID: 1074 RVA: 0x00044200 File Offset: 0x00042400
		private static bool Prefix(ConsoleScript __instance, string[] args, bool addToLog)
		{
			if (args == null || args.Length < 3 || !string.Equals(args[0], "setbodyfield", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			Traverse traverse = Traverse.Create(Plugin.S).Field(args[1]);
			if (!traverse.FieldExists() || typeof(Body).GetField(args[1]) != null)
			{
				return true;
			}
			Type valueType = traverse.GetValueType();
			object obj;
			if (valueType == typeof(float))
			{
				obj = Mathf.Clamp(float.Parse(args[2], CultureInfo.InvariantCulture), 0f, 100f);
			}
			else if (valueType == typeof(int))
			{
				obj = int.Parse(args[2]);
			}
			else
			{
				if (!(valueType == typeof(bool)))
				{
					return true;
				}
				obj = args[2] == "1" || args[2].ToLowerInvariant() == "true";
			}
			traverse.SetValue(obj);
			if (args[1] == "corruption")
			{
				Plugin.S.corruptionPeak = Mathf.Max(Plugin.S.corruptionPeak, (float)obj);
			}
			Traverse.Create(__instance).Method("LogToConsole", new object[] { string.Concat(new string[]
			{
				"Set her status \"",
				args[1],
				"\" to \"",
				(obj != null) ? obj.ToString() : null,
				"\" (LEWD Casualties UNKNOWN)."
			}) }).GetValue();
			if (addToLog)
			{
				Traverse.Create(__instance).Method("AddCommandToLogAndClearInput", Array.Empty<object>()).GetValue();
			}
			return false;
		}

		// Token: 0x040003D4 RID: 980
		public static readonly string[] Stats = new string[] { "arousal", "lust", "corruption", "submission", "exhaustion", "filled", "cum", "willpower" };
	}
}
