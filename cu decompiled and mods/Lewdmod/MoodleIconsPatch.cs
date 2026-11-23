using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000DD RID: 221
	[HarmonyPatch(typeof(MoodleManager), "Awake")]
	internal static class MoodleIconsPatch
	{
		// Token: 0x06000515 RID: 1301 RVA: 0x00054C88 File Offset: 0x00052E88
		private static void Postfix(MoodleManager __instance)
		{
			Sprite sprite = __instance.icons.Values.FirstOrDefault<Sprite>();
			float num = (sprite ? sprite.pixelsPerUnit : 100f);
			foreach (KeyValuePair<string, Sprite> keyValuePair in Plugin.Icons)
			{
				Sprite value = keyValuePair.Value;
				Sprite sprite2 = Sprite.Create(value.texture, value.rect, new Vector2(0.5f, 0.5f), num);
				sprite2.name = keyValuePair.Key;
				__instance.icons[keyValuePair.Key] = sprite2;
			}
		}
	}
}
