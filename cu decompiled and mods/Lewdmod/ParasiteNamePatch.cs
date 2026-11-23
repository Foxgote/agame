using System;
using HarmonyLib;

namespace LewdStatus
{
	// Token: 0x02000112 RID: 274
	[HarmonyPatch(typeof(Locale), "GetBuilding")]
	internal static class ParasiteNamePatch
	{
		// Token: 0x06000667 RID: 1639 RVA: 0x000660C0 File Offset: 0x000642C0
		private static void Postfix(string str, ref string __result)
		{
			if (str == "lewd_parasite")
			{
				__result = Plugin.T("parasite_name");
				return;
			}
			if (str == "lewd_parasitedsc")
			{
				__result = Plugin.T("parasite_desc");
				return;
			}
			if (str == "lewd_flesh")
			{
				__result = Plugin.T("flesh_name");
				return;
			}
			if (str == "lewd_fleshdsc")
			{
				__result = Plugin.T("flesh_desc");
				return;
			}
			if (str == "lewd_symbiote")
			{
				__result = Plugin.T("symbiote_name");
				return;
			}
			if (str == "lewd_symbiotedsc")
			{
				__result = Plugin.T("symbiote_desc");
				return;
			}
			if (str == "lewd_slimemon")
			{
				__result = Plugin.T("slimemon_name");
				return;
			}
			if (str == "lewd_slimemondsc")
			{
				__result = Plugin.T("slimemon_desc");
				return;
			}
			if (str == "lewd_worm")
			{
				__result = Plugin.T("worm_name");
				return;
			}
			if (str == "lewd_wormdsc")
			{
				__result = Plugin.T("worm_desc");
				return;
			}
			if (str == "lewd_tentacles")
			{
				__result = Plugin.T("tentacles_name");
				return;
			}
			if (str == "lewd_tentaclesdsc")
			{
				__result = Plugin.T("tentacles_desc");
				return;
			}
			if (str == "lewd_flower")
			{
				__result = Plugin.T("flower_name");
				return;
			}
			if (str == "lewd_flowerdsc")
			{
				__result = Plugin.T("flower_desc");
				return;
			}
			if (str == "lewd_slit")
			{
				__result = Plugin.T("slit_name");
				return;
			}
			if (str == "lewd_slitdsc")
			{
				__result = Plugin.T("slit_desc");
				return;
			}
			if (str == "lewd_latex")
			{
				__result = Plugin.T("latex_name");
				return;
			}
			if (str == "lewd_latexdsc")
			{
				__result = Plugin.T("latex_desc");
				return;
			}
			if (str == "lewd_latexpond")
			{
				__result = Plugin.T("latexpond_name");
				return;
			}
			if (str == "lewd_latexponddsc")
			{
				__result = Plugin.T("latexpond_desc");
				return;
			}
			if (str == "lewd_gasvent")
			{
				__result = Plugin.T("gasvent_name");
				return;
			}
			if (str == "lewd_gasventdsc")
			{
				__result = Plugin.T("gasvent_desc");
				return;
			}
			if (str == "lewd_fleshwall")
			{
				__result = Plugin.T("fleshwall_name");
				return;
			}
			if (str == "lewd_fleshwalldsc")
			{
				__result = Plugin.T("fleshwall_desc");
				return;
			}
			if (str == "lewd_slimewall")
			{
				__result = Plugin.T("slimewall_name");
				return;
			}
			if (str == "lewd_slimewalldsc")
			{
				__result = Plugin.T("slimewall_desc");
			}
		}
	}
}
