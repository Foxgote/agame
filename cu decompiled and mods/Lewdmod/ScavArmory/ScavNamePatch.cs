using System;
using HarmonyLib;
using LewdStatus;

namespace ScavArmory
{
	// Token: 0x02000048 RID: 72
	[HarmonyPatch(typeof(Locale), "GetBuilding")]
	internal static class ScavNamePatch
	{
		// Token: 0x0600016C RID: 364 RVA: 0x00018EC8 File Offset: 0x000170C8
		private static void Postfix(string str, ref string __result)
		{
			if (str == "arm_scav")
			{
				__result = (Plugin.English ? Plugin.Zh("Scavenger") : "Мародёр");
				return;
			}
			if (str == "arm_slimehost")
			{
				__result = (Plugin.English ? Plugin.Zh("Slime host") : "Заражённый слизью");
				return;
			}
			if (str == "arm_slimehostdsc")
			{
				__result = (Plugin.English ? Plugin.Zh("A survivor swallowed by living slime. It wants to share.") : "Выживший, которого поглотила живая слизь. Хочет поделиться.");
				return;
			}
			if (str == "arm_scavdsc")
			{
				__result = (Plugin.English ? Plugin.Zh("Another survivor down here. Armed, and not friendly.") : "Ещё один выживший. Вооружён и совсем не дружелюбен.");
			}
		}
	}
}
