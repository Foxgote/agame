using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000056 RID: 86
	[HarmonyPatch(typeof(PreRunScript), "Update")]
	internal static class BridgeMenuPatch
	{
		// Token: 0x060001A8 RID: 424 RVA: 0x000253EC File Offset: 0x000235EC
		private static void Postfix(PreRunScript __instance)
		{
			if (!Plugin.DebugKeys.Value || (BridgeMenuPatch.poll -= Time.unscaledDeltaTime) > 0f)
			{
				return;
			}
			BridgeMenuPatch.poll = 0.5f;
			string text = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "bridge_cmd.txt");
			if (!File.Exists(text))
			{
				return;
			}
			string[] array;
			try
			{
				array = File.ReadAllLines(text);
			}
			catch
			{
				return;
			}
			if (!array.Any<string>((string l) => l.Trim() == "startrun"))
			{
				return;
			}
			try
			{
				File.Delete(text);
			}
			catch
			{
			}
			Plugin.Log.LogInfo("[Bridge] starting a run from the menu");
			__instance.StartRun();
		}

		// Token: 0x040001B5 RID: 437
		private static float poll;
	}
}
