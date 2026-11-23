using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200004E RID: 78
	[HarmonyPatch(typeof(WoundView), "SetCharDetails")]
	internal static class AdultAgePatch
	{
		// Token: 0x06000190 RID: 400 RVA: 0x0001A8A4 File Offset: 0x00018AA4
		private static void Prefix(ref int age)
		{
			if (age >= 21 && age <= 25)
			{
				return;
			}
			int num = age;
			age = Mathf.Clamp(21 + Mathf.RoundToInt((float)((Mathf.Clamp(age, 10, 25) - 10) * 4) / 15f), 21, 25);
			Plugin.Log.LogInfo(string.Format("Her age on the health screen: {0} -> {1} (always {2}-{3} with this mod)", new object[] { num, age, 21, 25 }));
		}

		// Token: 0x04000190 RID: 400
		public const int Min = 21;

		// Token: 0x04000191 RID: 401
		public const int Max = 25;
	}
}
