using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using LewdStatus;
using PoseKit;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x0200001B RID: 27
	[BepInPlugin("jens.casualtiesunknown.armory", "LEWD Casualties UNKNOWN - Armory", "1.5.1")]
	public class Plugin : BaseUnityPlugin
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000052 RID: 82 RVA: 0x00005578 File Offset: 0x00003778
		internal static bool English
		{
			get
			{
				ConfigEntry<string> language = Plugin.Language;
				string text = (((language != null) ? language.Value : null) ?? "auto").Trim().ToLowerInvariant();
				return text == "en" || text == "zh" || (!(text == "ru") && !Plugin.Russian);
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000055DD File Offset: 0x000037DD
		internal static bool DebugOn
		{
			get
			{
				ConfigEntry<bool> debugKeys = Plugin.DebugKeys;
				if (debugKeys == null)
				{
					return Plugin.DebugKeys.Value;
				}
				return debugKeys.Value;
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000055F8 File Offset: 0x000037F8
		private void Awake()
		{
			Plugin.Log = base.Logger;
			Plugin.Language = base.Config.Bind<string>("General", "TextLanguage", "auto", "Item names, the raiders' shouts and her lines about guns: auto (as LEWD Casualties UNKNOWN: the game's language), ru or en.");
			Plugin.GunsEnabled = base.Config.Bind<bool>("Guns", "Enabled", true, "Our guns and their ammo turn up at all: by corpses, in crates, in the game's loot and in the raiders' hands. Off: none of them (the game's own guns stay).");
			Plugin.AmmoMatchShare = base.Config.Bind<float>("Spawns", "AmmoMatchShare", 0.7f, "Share of the boxes and magazines found that fit a gun she carries (the rest any calibre). 0 = always random.");
			Plugin.GunIntCut = base.Config.Bind<int>("Guns", "IntelligenceCut", 5, "How much less intelligence she needs to recognise and handle guns, magazines and rounds (the game asks 9-11).");
			Plugin.AmmoCounter = base.Config.Bind<bool>("Guns", "AmmoCounter", true, "While she holds a gun, a \"12/17 +1\" counter by the chamber icon (rounds in the magazine, +1 in the chamber). The tooltips always tell the rounds.");
			Plugin.GunFindLines = base.Config.Bind<bool>("Guns", "FindLines", true, "She says something (\"Holy shit!\") when she picks a gun up for the first time.");
			Plugin.CorpseGunChance = base.Config.Bind<float>("Spawns", "CorpseGunChance", 0.025f, "Chance that a dead body has one of our guns by it (a layer has some 80 bodies).");
			Plugin.CorpseAmmoChance = base.Config.Bind<float>("Spawns", "CorpseAmmoChance", 0.08f, "Chance that a dead body has a box of rounds or a .45 magazine by it.");
			Plugin.CrateGunChance = base.Config.Bind<float>("Spawns", "CrateGunChance", 0.05f, "Chance that a crate she breaks open holds one of our guns.");
			Plugin.CrateAmmoChance = base.Config.Bind<float>("Spawns", "CrateAmmoChance", 0.25f, "Chance that a crate she breaks open holds a box of rounds or a .45 magazine.");
			Plugin.GunMenuAtBottom = base.Config.Bind<bool>("Guns", "MenuAtBottom", true, "Move the gun menu (rack, magazine, safety, chamber) from the middle of the screen down to its bottom edge. Off: where the game puts it.");
			Plugin.GunMenuBottomOffset = base.Config.Bind<float>("Guns", "MenuBottomOffset", 24f, "How far above the bottom edge of the screen the moved gun menu sits (screen units of the game's interface).");
			Plugin.ReloadKey = base.Config.Bind<KeyCode>("Guns", "ReloadKey", 114, "Reload the gun in her hand: the fullest fitting magazine goes in (the old one into her inventory), or rounds go in one by one from her loose rounds and ammo boxes. If the game already uses this key (R opens the wound view by default), ReloadKeyFallback is used.");
			Plugin.ReloadKeyFallback = base.Config.Bind<KeyCode>("Guns", "ReloadKeyFallback", 118, "The reload key when ReloadKey is taken by one of the game's own keys.");
			if (Plugin.ReloadKeyFallback.Value == 101)
			{
				Plugin.ReloadKeyFallback.Value = 118;
			}
			Plugin.AmmoCraftInt = base.Config.Bind<int>("Guns", "AmmoCraftIntelligence", 6, "Intelligence the ammo box recipes need (2 scrap metal or 7 casings + powder + a tool: a box of 15 pistol or 5 rifle rounds).");
			Plugin.PodRifleChance = base.Config.Bind<float>("Spawns", "PodRifleChance", 0.25f, "Chance a new run starts with the three-line rifle and a box of its purple rounds fixed to the pod's wall.");
			Plugin.LoadedChance = base.Config.Bind<float>("Spawns", "LoadedChance", 0.3f, "Chance that a gun found by a corpse or in a crate is still loaded: a magazine in it (part full) and maybe a round in the chamber; a revolver or shotgun part loaded.");
			Plugin.ScavLoadedChance = base.Config.Bind<float>("Spawns", "ScavLoadedChance", 0.6f, "Chance that the gun a dead scavenger drops is still loaded.");
			Plugin.InGameLoot = base.Config.Bind<bool>("Spawns", "InGameLoot", true, "Our guns and ammo also join the game's own loot pools.");
			Plugin.LogShots = base.Config.Bind<bool>("Debug", "LogShots", false, "Write every shot of our guns to the BepInEx log.");
			Plugin.DebugKeys = base.Config.Bind<bool>("Debug", "DebugKeys", false, "Ctrl + numpad Enter: fill the gun in her hand (a full magazine or cylinder and a round in the chamber). Ctrl + numpad -: a raider near her. Ctrl + numpad +: a pack of slime hosts near her. (LEWD Casualties UNKNOWN's own Debug keys switch covers these too.)");
			Plugin.ScavEnabled = base.Config.Bind<bool>("Scavengers", "Enabled", true, "Hostile scavengers like her roam the layers, armed with melee weapons or guns.");
			Plugin.ScavPerLayer = base.Config.Bind<float>("Scavengers", "PerLayer", 3f, "Scavengers on every new layer (one more every second layer deeper, up to +3).");
			Plugin.ScavHealth = base.Config.Bind<float>("Scavengers", "Health", 320f, "How much a scavenger takes (the stock pistol does 90 a shot).");
			Plugin.ScavDamage = base.Config.Bind<float>("Scavengers", "DamageToHer", 1f, "How hard their shots and blows hurt her (multiplier).");
			Plugin.ScavSight = base.Config.Bind<float>("Scavengers", "SightRange", 22f, "How far they spot her (they look the way they face; they hear her close by).");
			Plugin.ScavGunShare = base.Config.Bind<float>("Scavengers", "GunShare", 0.45f, "Share of scavengers carrying a gun rather than a blade or a club.");
			Plugin.ScavCorpseSeconds = base.Config.Bind<float>("Scavengers", "CorpseSeconds", 180f, "How long a dead scavenger's body stays.");
			Plugin.GrabberShare = base.Config.Bind<float>("Capture", "GrabberShare", 0.4f, "Share of scavengers that try to grab her rather than kill her (gunmen do it once she is down).");
			Plugin.UltimatumEnabled = base.Config.Bind<bool>("Capture", "Ultimatum", true, "A raider with a gun holds his fire on her when she is badly hurt and calls on her to give up (Z). If she shoots or hits one of them after that, raiders on this layer take no more surrenders.");
			Plugin.TreatChance = base.Config.Bind<float>("Capture", "TreatChance", 0.7f, "She gave herself up bleeding: chance he dresses her worst wound first (a tourniquet on a limb bleeding hard, otherwise a bandage).");
			Plugin.MeleeGrabberShare = base.Config.Bind<float>("Capture", "MeleeGrabberShare", 0.65f, "Share of scavengers with a blade or a club that try to grab her rather than kill her.");
			Plugin.ThrownDamage = base.Config.Bind<float>("Scavengers", "ThrownDamage", 0.66f, "Damage of a thing thrown at a scavenger: speed x mass (up to 4) x this; a blade 1.5x.");
			Plugin.LecherShare = base.Config.Bind<float>("Capture", "LecherShare", 0.6f, "Of those, the share that also want to use her (needs LEWD Casualties UNKNOWN).");
			Plugin.CaptureUseChance = base.Config.Bind<float>("Capture", "UseChance", 0.7f, "Chance such a scavenger uses her once he has her.");
			Plugin.DealEnabled = base.Config.Bind<bool>("Deal", "Enabled", true, "A raider who wants her body may offer her something she needs for it instead of attacking. The surrender key says yes.");
			Plugin.DealChance = base.Config.Bind<float>("Deal", "OfferChance", 0.3f, "Chance such a raider offers a deal when he first sees her.");
			Plugin.DealChanceNoDeath = base.Config.Bind<float>("Deal", "OfferChanceNoDeadlyScenes", 0.9f, "With deadly scenes off: chance any raider (not only one who wants her body) offers a deal when he first sees her.");
			if (Mathf.Approximately(Plugin.DealChanceNoDeath.Value, 0.6f))
			{
				Plugin.DealChanceNoDeath.Value = 0.9f;
			}
			Plugin.DealKey = base.Config.Bind<KeyCode>("Deal", "OfferKey", 107, "Key for her to offer a raider who has seen her a deal herself (he may turn it down: likelier yes the more he wants her, and with deadly scenes off).");
			Plugin.CampEnabled = base.Config.Bind<bool>("Camp", "Enabled", true, "Neutral camps: a trader dressed as a raider and an armed guard by a tent and a campfire. They don't attack her, the guard shoots creatures and slime hosts near the camp.");
			Plugin.CampChance = base.Config.Bind<float>("Camp", "Chance", 1f, "Chance of one neutral camp on a map (at most one).");
			Plugin.CampPayValue = base.Config.Bind<int>("Camp", "PayValue", 30, "What the trader counts her paying with herself as (goods given; the game caps a trader at 60).");
			Plugin.DealSeconds = base.Config.Bind<float>("Deal", "AnswerSeconds", 15f, "Seconds he waits for her answer.");
			Plugin.DealAngerChance = base.Config.Bind<float>("Deal", "AngerChance", 0.35f, "Chance he attacks her when she ignores the offer or walks away (otherwise he walks off).");
			Plugin.NakedUseChance = base.Config.Bind<float>("Capture", "NakedUseChance", 0.6f, "She has nothing on her and nothing to take: chance a scavenger who is not after her body uses her anyway. Naked she is 1.4x likelier to be used (in underwear 1.2x).");
			Plugin.CaptureKillChance = base.Config.Bind<float>("Capture", "KillChance", 0.12f, "Chance he kills her at the end (+8% for every time she struggled, -8% if she surrendered).");
			Plugin.CaptureEscapeChance = base.Config.Bind<float>("Capture", "EscapeChance", 0.3f, "Chance per struggle to get free, at full strength (less the weaker she is, less while he uses her).");
			Plugin.CaptureUseSeconds = base.Config.Bind<float>("Capture", "UseSeconds", 30f, "About how long he uses her.");
			Plugin.HostsEnabled = base.Config.Bind<bool>("SlimeHosts", "Enabled", true, "Slime hosts (former survivors covered in living slime) roam the layers. Needs LEWD Casualties UNKNOWN.");
			Plugin.GunsEnabled.SettingChanged += delegate(object s, EventArgs e)
			{
				Armory.SyncLoot();
			};
			Plugin.InGameLoot.SettingChanged += delegate(object s, EventArgs e)
			{
				Armory.SyncLoot();
			};
			Plugin.ScavEnabled.SettingChanged += delegate(object s, EventArgs e)
			{
				if (!Plugin.ScavEnabled.Value)
				{
					Scavengers.Remove(false);
				}
			};
			Plugin.HostsEnabled.SettingChanged += delegate(object s, EventArgs e)
			{
				if (!Plugin.HostsEnabled.Value)
				{
					Scavengers.Remove(true);
				}
			};
			Plugin.HostPerLayer = base.Config.Bind<float>("SlimeHosts", "PerLayer", 1f, "Packs of slime hosts on every new layer (sometimes one more). They need LEWD Casualties UNKNOWN: the slime suit they give her is its.");
			Plugin.Feud = base.Config.Bind<bool>("SlimeHosts", "FeudWithRaiders", true, "Raiders and slime hosts fight each other to the death on sight.");
			Plugin.FeudHostDamage = base.Config.Bind<float>("SlimeHosts", "FeudDamage", 30f, "Damage a slime host does to a raider with every squeeze (about once a second).");
			Plugin.FeudRiseChance = base.Config.Bind<float>("SlimeHosts", "FeudRiseChance", 0.6f, "Chance that a raider killed by a slime host gets up as a slime host a few seconds later.");
			Plugin.HostGroupMin = base.Config.Bind<int>("SlimeHosts", "GroupMin", 2, "Fewest slime hosts in a pack.");
			Plugin.HostGroupMax = base.Config.Bind<int>("SlimeHosts", "GroupMax", 3, "Most slime hosts in a pack.");
			Plugin.HostInfectSeconds = base.Config.Bind<float>("SlimeHosts", "InfectSeconds", 6f, "Seconds between the doses of slime it gives her while it holds her (each adds a stage of its slime suit; stage 5 closes over her face and she is one of them).");
			Plugin.PosedScenes = base.Config.Bind<bool>("Capture", "PosedScenes", true, "Hold her in drawn poses (poses.txt) rather than as a ragdoll.");
			Plugin.StruggleKey = base.Config.Bind<KeyCode>("Capture", "StruggleKey", 106, "Key to struggle in a scavenger's grip (the same as the tear-off key of LEWD Casualties UNKNOWN).");
			Plugin.SurrenderKey = base.Config.Bind<KeyCode>("Capture", "SurrenderKey", 122, "Key to surrender to the scavengers that are after her, or to stop struggling once caught.");
			string text = Path.Combine(Path.GetDirectoryName(base.Info.Location), "armory");
			Armory.Load(Path.Combine(text, "sprites"));
			ScavArt.Load(text);
			Kit.Log = delegate(string m)
			{
				Plugin.Log.LogInfo(m);
			};
			Kit.Warn = delegate(string m)
			{
				Plugin.Log.LogWarning(m);
			};
			Poses.Load(Path.Combine(text, "poses.txt"));
			Plugin.Log.LogInfo(string.Format("Scav Armory: {0} items, {1} sprites, {2} scavenger skins, {3} overalls", new object[]
			{
				Armory.All.Length,
				Armory.Sprites.Count,
				ScavArt.Skins.Count,
				ScavArt.Overalls.Count
			}));
		}

		// Token: 0x04000038 RID: 56
		public const string Guid = "jens.casualtiesunknown.armory";

		// Token: 0x04000039 RID: 57
		internal static ManualLogSource Log;

		// Token: 0x0400003A RID: 58
		internal static ConfigEntry<string> Language;

		// Token: 0x0400003B RID: 59
		internal static ConfigEntry<float> CorpseGunChance;

		// Token: 0x0400003C RID: 60
		internal static ConfigEntry<float> CorpseAmmoChance;

		// Token: 0x0400003D RID: 61
		internal static ConfigEntry<float> CrateGunChance;

		// Token: 0x0400003E RID: 62
		internal static ConfigEntry<float> CrateAmmoChance;

		// Token: 0x0400003F RID: 63
		internal static ConfigEntry<float> LoadedChance;

		// Token: 0x04000040 RID: 64
		internal static ConfigEntry<float> ScavLoadedChance;

		// Token: 0x04000041 RID: 65
		internal static ConfigEntry<bool> InGameLoot;

		// Token: 0x04000042 RID: 66
		internal static ConfigEntry<bool> LogShots;

		// Token: 0x04000043 RID: 67
		internal static ConfigEntry<bool> DebugKeys;

		// Token: 0x04000044 RID: 68
		internal static ConfigEntry<bool> ScavEnabled;

		// Token: 0x04000045 RID: 69
		internal static ConfigEntry<bool> HostsEnabled;

		// Token: 0x04000046 RID: 70
		internal static ConfigEntry<bool> GunsEnabled;

		// Token: 0x04000047 RID: 71
		internal static ConfigEntry<float> AmmoMatchShare;

		// Token: 0x04000048 RID: 72
		internal static ConfigEntry<float> PodRifleChance;

		// Token: 0x04000049 RID: 73
		internal static ConfigEntry<int> AmmoCraftInt;

		// Token: 0x0400004A RID: 74
		internal static ConfigEntry<float> ScavPerLayer;

		// Token: 0x0400004B RID: 75
		internal static ConfigEntry<float> ScavHealth;

		// Token: 0x0400004C RID: 76
		internal static ConfigEntry<float> ScavDamage;

		// Token: 0x0400004D RID: 77
		internal static ConfigEntry<float> ScavSight;

		// Token: 0x0400004E RID: 78
		internal static ConfigEntry<float> ScavGunShare;

		// Token: 0x0400004F RID: 79
		internal static ConfigEntry<float> ScavCorpseSeconds;

		// Token: 0x04000050 RID: 80
		internal static ConfigEntry<float> NakedUseChance;

		// Token: 0x04000051 RID: 81
		internal static ConfigEntry<bool> DealEnabled;

		// Token: 0x04000052 RID: 82
		internal static ConfigEntry<bool> CampEnabled;

		// Token: 0x04000053 RID: 83
		internal static ConfigEntry<float> CampChance;

		// Token: 0x04000054 RID: 84
		internal static ConfigEntry<int> CampPayValue;

		// Token: 0x04000055 RID: 85
		internal static ConfigEntry<KeyCode> DealKey;

		// Token: 0x04000056 RID: 86
		internal static ConfigEntry<KeyCode> ReloadKey;

		// Token: 0x04000057 RID: 87
		internal static ConfigEntry<KeyCode> ReloadKeyFallback;

		// Token: 0x04000058 RID: 88
		internal static ConfigEntry<bool> GunMenuAtBottom;

		// Token: 0x04000059 RID: 89
		internal static ConfigEntry<float> GunMenuBottomOffset;

		// Token: 0x0400005A RID: 90
		internal static ConfigEntry<float> DealChance;

		// Token: 0x0400005B RID: 91
		internal static ConfigEntry<float> DealChanceNoDeath;

		// Token: 0x0400005C RID: 92
		internal static ConfigEntry<float> DealSeconds;

		// Token: 0x0400005D RID: 93
		internal static ConfigEntry<float> DealAngerChance;

		// Token: 0x0400005E RID: 94
		internal static ConfigEntry<float> MeleeGrabberShare;

		// Token: 0x0400005F RID: 95
		internal static ConfigEntry<float> ThrownDamage;

		// Token: 0x04000060 RID: 96
		internal static ConfigEntry<float> TreatChance;

		// Token: 0x04000061 RID: 97
		internal static ConfigEntry<bool> UltimatumEnabled;

		// Token: 0x04000062 RID: 98
		internal static ConfigEntry<float> GrabberShare;

		// Token: 0x04000063 RID: 99
		internal static ConfigEntry<float> LecherShare;

		// Token: 0x04000064 RID: 100
		internal static ConfigEntry<float> CaptureUseChance;

		// Token: 0x04000065 RID: 101
		internal static ConfigEntry<float> CaptureKillChance;

		// Token: 0x04000066 RID: 102
		internal static ConfigEntry<float> CaptureEscapeChance;

		// Token: 0x04000067 RID: 103
		internal static ConfigEntry<float> CaptureUseSeconds;

		// Token: 0x04000068 RID: 104
		internal static ConfigEntry<KeyCode> StruggleKey;

		// Token: 0x04000069 RID: 105
		internal static ConfigEntry<KeyCode> SurrenderKey;

		// Token: 0x0400006A RID: 106
		internal static ConfigEntry<float> HostPerLayer;

		// Token: 0x0400006B RID: 107
		internal static ConfigEntry<float> HostInfectSeconds;

		// Token: 0x0400006C RID: 108
		internal static ConfigEntry<int> HostGroupMin;

		// Token: 0x0400006D RID: 109
		internal static ConfigEntry<int> HostGroupMax;

		// Token: 0x0400006E RID: 110
		internal static ConfigEntry<bool> Feud;

		// Token: 0x0400006F RID: 111
		internal static ConfigEntry<float> FeudHostDamage;

		// Token: 0x04000070 RID: 112
		internal static ConfigEntry<float> FeudRiseChance;

		// Token: 0x04000071 RID: 113
		internal static ConfigEntry<bool> PosedScenes;

		// Token: 0x04000072 RID: 114
		internal static ConfigEntry<int> GunIntCut;

		// Token: 0x04000073 RID: 115
		internal static ConfigEntry<bool> GunFindLines;

		// Token: 0x04000074 RID: 116
		internal static ConfigEntry<bool> AmmoCounter;
	}
}
