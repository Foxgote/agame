using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using PoseKit;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000DA RID: 218
	[BepInPlugin("jens.casualtiesunknown.lewdstatus", "LEWD Casualties UNKNOWN", "3.0.3")]
	public class Plugin : BaseUnityPlugin
	{
		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060004F8 RID: 1272 RVA: 0x00050BFC File Offset: 0x0004EDFC
		internal static string Lang
		{
			get
			{
				ConfigEntry<string> language = LewdSettings.Language;
				string text = (((language != null) ? language.Value : null) ?? "auto").Trim().ToLowerInvariant();
				if (text == "ru" || text == "en" || text == "zh")
				{
					return text;
				}
				string text2 = Locale.currentLangName ?? PlayerPrefs.GetString("locale");
				if (string.IsNullOrEmpty(text2))
				{
					return "en";
				}
				if (text2.StartsWith("ru", StringComparison.OrdinalIgnoreCase))
				{
					return "ru";
				}
				if (text2.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
				{
					return "zh";
				}
				return "en";
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x00050CA6 File Offset: 0x0004EEA6
		internal static bool Russian
		{
			get
			{
				return Plugin.Lang == "ru";
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060004FA RID: 1274 RVA: 0x00050CB7 File Offset: 0x0004EEB7
		internal static bool Chinese
		{
			get
			{
				return Plugin.Lang == "zh";
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060004FB RID: 1275 RVA: 0x00050CC8 File Offset: 0x0004EEC8
		internal static Dictionary<string, string> Text
		{
			get
			{
				if (Plugin.Russian)
				{
					return Plugin.TextRu;
				}
				if (!Plugin.Chinese)
				{
					return Plugin.TextEn;
				}
				return Plugin.TextZh;
			}
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x00050CEC File Offset: 0x0004EEEC
		internal static string Zh(string en)
		{
			string text;
			if (!Plugin.Chinese || en == null || !Plugin.PhrasesZh.TryGetValue(en, out text))
			{
				return en;
			}
			return text;
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x00050D18 File Offset: 0x0004EF18
		private void Awake()
		{
			Plugin.<>c__DisplayClass271_0 CS$<>8__locals1 = new Plugin.<>c__DisplayClass271_0();
			CS$<>8__locals1.<>4__this = this;
			Plugin.Instance = this;
			Plugin.Log = base.Logger;
			CS$<>8__locals1.root = Path.GetDirectoryName(base.Info.Location);
			ConfigEntry<string> configEntry = base.Config.Bind<string>("General", "Language", "auto", "Language of the mod's texts: auto follows the game's language (Russian when the game is in Russian, e.g. with the Russian Locale mod; Chinese when it is in Chinese; English otherwise). ru, en or zh force one.");
			LewdSettings.Bind(base.Config, configEntry);
			AboutMod.Bind(base.Config);
			Bust.Bind(base.Config);
			FurRealCompat.Bind(base.Config);
			Devices.Bind(base.Config);
			LatexMites.Bind(base.Config);
			TraderSuit.Bind(base.Config);
			Plugin.LustRate = base.Config.Bind<float>("Balance", "LustRate", 1f, "Multiplier for how fast lust builds up.");
			Plugin.TentacleHurtSeconds = base.Config.Bind<float>("Tentacles", "HurtSeconds", 30f, "Shot or struck before they have her, the ceiling tentacles cannot be killed, but they draw back and grab nothing for this many seconds (once they hold her, blows do nothing: only struggling frees her).");
			Plugin.WebDebrisSeconds = base.Config.Bind<float>("Webs", "DebrisSeconds", 240f, "Torn webs and empty cocoons fade away this many seconds after they are torn (0 = they stay).");
			Plugin.SubmissionDecay = base.Config.Bind<float>("Balance", "SubmissionDecay", 0.01f, "Submission lost per second when nothing feeds it (0.01 = about 36 an hour).");
			Plugin.ExhaustionRecovery = base.Config.Bind<float>("Balance", "ExhaustionRecovery", 0.15f, "Exhaustion she sheds per second while nothing holds her.");
			Plugin.SatisfiedSeconds = base.Config.Bind<float>("Balance", "SatisfiedSeconds", 180f, "How long she stays satisfied after bringing herself off (half that in heat): her lust does not grow meanwhile.");
			Plugin.CorruptionFromLust = base.Config.Bind<float>("Balance", "CorruptionFromLust", 0.02f, "Corruption gained per second while her lust is above CorruptionLustThreshold.");
			Plugin.CorruptionLustThreshold = base.Config.Bind<float>("Balance", "CorruptionLustThreshold", 80f, "Lust above which corruption creeps up.");
			Plugin.MoodGainScale = base.Config.Bind<float>("Balance", "MoodGainScale", 0.2f, "Scales every mood the mod gives her (orgasms, latex or nakedness she likes, a bonded suit, heat...). 1 = as before, 0.2 = a fifth.");
			Plugin.MoodLossScale = base.Config.Bind<float>("Balance", "MoodLossScale", 1f, "Scales every mood the mod takes from her (shame, frustration, the suppressant...).");
			Plugin.ConsumedMoodScale = base.Config.Bind<float>("Balance", "ConsumedMoodScale", 0.5f, "Scales the mood high corruption takes from her through its cravings (the Consumed status). 1 = as before.");
			Plugin.CorruptionFloorShare = base.Config.Bind<float>("Balance", "CorruptionFloorShare", 0.5f, "Corruption never fades below this share of the highest it has been (0.5 = half of the peak, 0 = it can fade all the way to nothing, 1 = it never fades).");
			Plugin.DiscomfortDamping = base.Config.Bind<float>("Balance", "DiscomfortDamping", 1f, "How hard hunger, thirst, dirt, tiredness and sickness put her off: they lower the arousal she settles at, drain it, and slow her lust (at the game's own moodle thresholds, more the worse it gets). 0 switches it off, 2 doubles it.");
			Plugin.PainDamping = base.Config.Bind<float>("Balance", "PainDamping", 1f, "How hard pain kills her arousal (unless she has come to like it, submission 50+). 0 switches it off, 2 doubles it.");
			Plugin.IdleArousalShare = base.Config.Bind<float>("Balance", "IdleArousalShare", 0.7f, "With nothing touching her, arousal settles at this share of her lust (1 = arousal simply follows lust, as before).");
			Plugin.IdleOrgasm = base.Config.Bind<bool>("Balance", "IdleOrgasm", false, "Whether she can cum from lust alone, with nothing touching her (as before). Off: only touching herself, the scenes and the suits bring her over.");
			Plugin.CorruptionDecayPerHour = base.Config.Bind<float>("Balance", "CorruptionDecayPerHour", 10f, "How much corruption fades per hour of play while her lust is low (under 40). It never falls below half of the highest it has been.");
			Plugin.HeatOffMinutes = base.Config.Bind<float>("Balance", "HeatOffMinutes", 15f, "Real minutes between heats.");
			Plugin.HeatOnMinutes = base.Config.Bind<float>("Balance", "HeatOnMinutes", 5f, "Real minutes a heat lasts.");
			Plugin.CorruptionPerOrgasm = base.Config.Bind<float>("Balance", "CorruptionPerOrgasm", 4f, "Corruption gained per orgasm.");
			Plugin.ReliefKey = base.Config.Bind<KeyCode>("Relief", "Key", 104, "Key to start/stop relieving herself.");
			Plugin.MoanVolume = base.Config.Bind<float>("Sound", "MoanVolume", 1f, "Loudness of her moans, sighs, breathing and climaxes in the mod's scenes, 0-2.");
			Plugin.ReliefEnemyRange = base.Config.Bind<float>("Relief", "EnemyRange", 15f, "She won't touch herself with something hostile this close (raiders, slime hosts, parasites, crawlers, crystals, drones, turrets), and stops if one comes this close.");
			Plugin.ReliefTraderRange = base.Config.Bind<float>("Relief", "TraderRange", 10f, "She won't do it in front of a trader this close (unless lust makes her).");
			Plugin.ReliefHurtSeconds = base.Config.Bind<float>("Relief", "HurtSeconds", 8f, "Nor for this many seconds after she was hit.");
			Plugin.Compulsion = base.Config.Bind<bool>("Relief", "Compulsion", true, "At 80+ corruption and 90+ arousal she may start on her own.");
			Plugin.RemoveKey = base.Config.Bind<KeyCode>("Parasite", "TearOffKey", 106, "Key to try to tear the living suit off.");
			Plugin.SuitEnjoyMinutes = base.Config.Bind<float>("Parasite", "EnjoyMinutes", 6f, "Minutes of wearing a living suit (from stage 2) before she starts to like it and says so, openly (sooner with corruption and submission; at once in the bonded slime suit).");
			Plugin.SuitGrowth = base.Config.Bind<float>("Parasite", "SuitGrowth", 1f, "Multiplier for how fast the suit grows to the next stage.");
			Plugin.ParasitesPerLayer = base.Config.Bind<float>("Parasite", "ParasitesPerLayer", 5f, "Living-cloth parasites placed on every new layer (one more every second layer deeper, up to +3). Symbiotes and drones are rolled separately.");
			Plugin.RareSpawnChance = base.Config.Bind<float>("Spawns", "SymbioteAndDroneChance", 0.35f, "Chance per new layer that a symbiote appears.");
			Plugin.TakeoverChance = base.Config.Bind<float>("Parasite", "TakeoverChance", 0.04f, "At stage 4: chance per minute that the suit takes over her body (raised by submission and corruption, x3 on orgasm).");
			Plugin.TakeoverMinutes = base.Config.Bind<float>("Parasite", "TakeoverMinutes", 5f, "How long a takeover lasts before the sated suit lets go.");
			Plugin.TameEscapes = base.Config.Bind<int>("Parasite", "TameEscapes", 4, "Living cloth: takeovers she has to break out of (tear-off key) before the suit gives in and obeys her: no more takeovers, stage-5 armour, faster healing. After each escape it tries again once it can.");
			Plugin.TouchScale = base.Config.Bind<float>("Parasite", "TouchScale", 0.4f, "How often the living suits do something to her (pulses, milking, freezing her in place, using her during a takeover): 1 = as before, 0.4 = 2.5 times rarer.");
			Plugin.StabilizeMinutes = base.Config.Bind<float>("SlimeHosts", "StabilizeMinutes", 15f, "A dose of antiserum (50 ml) keeps the hosts' slime quiet this long: it stops creeping to her face and gives the bonded slime's boons. Then it tries again.");
			Plugin.StabilizeDoses = base.Config.Bind<int>("SlimeHosts", "StabilizeDoses", 2, "Doses of antiserum (in all, or at once) that make the hosts' slime stable for good: it never turns her, gives the living cloth's boons, and the hosts leave her alone.");
			Plugin.FleshWallsPerLayer = base.Config.Bind<float>("FleshWall", "PerLayer", 1.5f, "Living flesh walls (on sheer rock faces) on every new layer.");
			Plugin.FleshWallAbsorbSeconds = base.Config.Bind<float>("FleshWall", "AbsorbSeconds", 45f, "Seconds the wall takes to grow over all of her (it closes over her head at 75%).");
			Plugin.RepairRecipes = base.Config.Bind<bool>("Clothes", "RepairRecipes", false, "A repair recipe for every outfit of the mod (outfit + rag), as before. Off: one sewing kit (a rag + string) mends them instead, and the crafting menu stays short.");
			Plugin.ShamelessCorruption = base.Config.Bind<float>("Relief", "ShamelessCorruption", 60f, "From this corruption she touches herself in front of traders too.");
			Plugin.ShamelessLust = base.Config.Bind<float>("Relief", "ShamelessLust", 85f, "From this lust, too.");
			Plugin.TraderWatchRep = base.Config.Bind<float>("Relief", "TraderWatchReputation", 15f, "A trader who watches her do it likes her up to this much more, or less (random, like a hug).");
			Plugin.SecondChanceCorruption = base.Config.Bind<float>("Balance", "SecondChanceCorruption", 20f, "The game's second chance (\"let's not give up yet\") frees her from whatever holds her, clears her exhaustion and takes this much corruption off (never below CorruptionFloorShare).");
			Plugin.SecondChanceSubmission = base.Config.Bind<float>("Balance", "SecondChanceSubmission", 30f, "And this much submission.");
			Plugin.FleshWallHealth = base.Config.Bind<float>("FleshWall", "Health", 180f, "How much fire the flesh wall takes before it burns away (bullets and blows only stun it; a blast tears it apart).");
			Plugin.SlimeWallHealth = base.Config.Bind<float>("FleshWall", "SlimeWallHealth", 120f, "How much the slime wall takes (hits, bullets, a little from fire, blasts): destroyed, its slime runs down off the rock.");
			Plugin.SlimeWallFireScale = base.Config.Bind<float>("FleshWall", "SlimeWallFireScale", 0.35f, "How much fire does to the slime wall (it is wet through): 0.35 = about a third of what it does to flesh.");
			Plugin.GasVentHealth = base.Config.Bind<float>("LustGas", "GeyserHealth", 80f, "How much hitting a lust gas geyser takes before it cracks shut. Digging out the rock under it or a blast close by does it at once.");
			Plugin.TentacleHealth = base.Config.Bind<float>("Tentacles", "Health", 220f, "How much fire the ceiling tentacles take before they die (bullets and blows only stun them; dead, they drop her).");
			Plugin.FleshHealth = base.Config.Bind<float>("Flesh", "Health", 150f, "How much fire a patch of living flesh takes before it burns away (bullets and blows only stun it).");
			Plugin.FleshStunSeconds = base.Config.Bind<float>("Flesh", "StunSeconds", 8f, "Seconds living flesh and the flesh wall grab nothing after a bullet or a blow.");
			Plugin.FireTorchHit = base.Config.Bind<float>("Fire", "TorchHit", 70f, "Burn from striking the mod's flesh (tentacles, living flesh, flesh wall) with a lit torch in her hand.");
			Plugin.FireBurn = base.Config.Bind<float>("Fire", "BurnPerSecond", 30f, "Burn per second from a lit torch or a campfire left by it.");
			Plugin.FireBlast = base.Config.Bind<float>("Fire", "Blast", 500f, "What a blast (dynamite) does to the mod's flesh right next to it (less further off).");
			Plugin.FireReach = base.Config.Bind<float>("Fire", "Reach", 1.6f, "How close a torch or campfire must be to burn it.");
			Plugin.WormHealth = base.Config.Bind<float>("GiantWorm", "Health", 2500f, "How much the giant worm takes from bullets, blows and blasts before it dies (it is hit anywhere along its length). Dynamite in its maw or its gut kills it outright.");
			if (Mathf.Approximately(Plugin.WormHealth.Value, 900f))
			{
				Plugin.WormHealth.Value = 2500f;
			}
			Plugin.WormNoiseGun = base.Config.Bind<float>("GiantWorm", "NoiseGunshot", 40f, "How far the giant worm hears a gunshot (metres).");
			Plugin.WormNoiseBlast = base.Config.Bind<float>("GiantWorm", "NoiseBlast", 60f, "How far it hears a blast.");
			Plugin.WormNoiseDig = base.Config.Bind<float>("GiantWorm", "NoiseDigging", 25f, "How far it hears digging (a pickaxe, a shovel, blows on rock).");
			Plugin.WormNoiseFall = base.Config.Bind<float>("GiantWorm", "NoiseFall", 18f, "How far it hears a body hitting the ground hard.");
			Plugin.WormNoiseDrop = base.Config.Bind<float>("GiantWorm", "NoiseThrown", 14f, "How far it hears a thing landing (thrown or dropped): a lure.");
			Plugin.WormNoiseMoan = base.Config.Bind<float>("GiantWorm", "NoiseMoan", 15f, "How far it hears her moans (scaled by how loud).");
			Plugin.WormNoiseClimax = base.Config.Bind<float>("GiantWorm", "NoiseClimax", 25f, "How far it hears her climax.");
			Plugin.WormNoiseRun = base.Config.Bind<float>("GiantWorm", "NoiseRunning", 6f, "How far it hears her running feet (walking and crouching are silent).");
			Plugin.WormNoiseFuse = base.Config.Bind<float>("GiantWorm", "NoiseFuse", 20f, "How far it hears a lit stick of dynamite hiss: it goes to it and swallows it, and that kills it.");
			Plugin.WormNoiseMemory = base.Config.Bind<float>("GiantWorm", "NoiseMemory", 10f, "Seconds it searches where it last heard something before it gives up.");
			Plugin.WormRetreatDamage = base.Config.Bind<float>("GiantWorm", "RetreatDamage", 300f, "Damage within 6 seconds that makes it back off for a while (long shooting drives it away).");
			Plugin.WormFireScare = base.Config.Bind<float>("GiantWorm", "FireScareRange", 3.5f, "A lit torch or a campfire this close to its maw while it comes to swallow her makes it recoil and back off (fire against its body does nothing; 0 = fire never scares it).");
			Plugin.WormBlastDamage = base.Config.Bind<float>("GiantWorm", "BlastDamage", 1500f, "Damage of a blast right next to its body (less further off); in its maw a blast kills it.");
			Plugin.WormDropKey = base.Config.Bind<KeyCode>("GiantWorm", "DropDynamiteKey", 117, "Swallowed, with dynamite in her hand: this key lets it fall deeper into the worm. It retches her out and dies a moment later.");
			if (Plugin.WormDropKey.Value == 103)
			{
				Plugin.WormDropKey.Value = 117;
			}
			Plugin.FleshWallEscape = base.Config.Bind<float>("FleshWall", "EscapeChance", 0.3f, "Chance per struggle to tear free at the start (less the deeper she is in it, the more tired, the more submissive).");
			Plugin.FleshWallArousal = base.Config.Bind<float>("FleshWall", "ArousalRate", 1f, "How fast it arouses her inside.");
			Plugin.FleshWallOrgasmAir = base.Config.Bind<float>("FleshWall", "OrgasmAirLoss", 12f, "Blood oxygen every forced orgasm takes from her.");
			Plugin.FleshWallOrgasmsToDeath = base.Config.Bind<int>("FleshWall", "OrgasmsToDeath", 6, "After this many forced orgasms she is spent: her breath gives out.");
			Plugin.FleshWallSuffocation = base.Config.Bind<float>("FleshWall", "Suffocation", 2f, "Blood oxygen per second she loses once it has closed over her head.");
			Plugin.SlimeWallsPerLayer = base.Config.Bind<float>("SlimeWall", "PerLayer", 2.5f, "Slime-coated walls on every new layer: she cannot slow her slide down them or jump off them.");
			Plugin.FleshPerLayer = base.Config.Bind<float>("LivingFlesh", "PatchesPerLayer", 2f, "Living flesh traps placed on every new layer.");
			Plugin.FleshSwallowSeconds = base.Config.Bind<float>("LivingFlesh", "SwallowSeconds", 6f, "Seconds in the tentacles' grip before it swallows her.");
			Plugin.FleshSceneSeconds = base.Config.Bind<float>("LivingFlesh", "SceneSeconds", 30f, "How long the tentacle scene inside lasts before it spits her out.");
			Plugin.FleshEscapeGrab = base.Config.Bind<float>("LivingFlesh", "EscapeChanceGrabbed", 0.35f, "Chance per struggle (tear-off key) to break the tentacles' grip.");
			Plugin.FleshEscapeInside = base.Config.Bind<float>("LivingFlesh", "EscapeChanceInside", 0.1f, "Chance per struggle to tear her way out once swallowed.");
			Plugin.FleshZonesPerLayer = base.Config.Bind<float>("FleshBiome", "BiomesPerLayer", 1f, "Areas overgrown with living flesh on every new layer.");
			Plugin.FleshZoneRadius = base.Config.Bind<float>("FleshBiome", "Radius", 14f, "Radius of a flesh biome.");
			Plugin.FleshZoneTraps = base.Config.Bind<float>("FleshBiome", "TrapsPerBiome", 4f, "Living flesh traps hidden inside each biome.");
			Plugin.FleshZoneSlow = base.Config.Bind<float>("FleshBiome", "SpeedOnFlesh", 0.6f, "Movement speed multiplier while walking on the flesh carpet.");
			Plugin.StartWithBodysuit = base.Config.Bind<bool>("Bodysuit", "StartWithBodysuit", true, "Put a space tactical bodysuit next to her at the start of a new run.");
			Plugin.BlanketWarmth = base.Config.Bind<float>("Blanket", "SleepWarmth", 0.8f, "Insulation the wool blanket adds while she sleeps with it on her (a hoodie gives about 0.6, a sleeping bag 0.5).");
			Plugin.ToyCrateChance = base.Config.Bind<float>("Toys", "CrateChance", 0.1f, "Chance a broken crate holds a dildo or an anal plug.");
			Plugin.ToyMinLust = base.Config.Bind<float>("Toys", "MinLust", 30f, "Lust (or arousal) she needs before she will pick a toy up or use it; below it she calls you a pervert.");
			Plugin.SuppressMinutes = base.Config.Bind<float>("Suppressant", "MinutesPerDose", 7f, "Minutes one dose (a pill, 5 ml) of the lust suppressant keeps her lust down.");
			Plugin.SuppressLeak = base.Config.Bind<float>("Suppressant", "Leak", 0.25f, "While it works, the share of lust and arousal that still gets through (lust building up, the gas, the draught, the flesh wall).");
			Plugin.SuppressRecipeInt = base.Config.Bind<int>("Suppressant", "RecipeIntelligence", 8, "Intelligence the brew recipe (2 foliage + 50 ml water) needs.");
			Plugin.SuppressCrateChance = base.Config.Bind<float>("Suppressant", "CrateChance", 0.01f, "Chance a broken crate holds a bottle of suppressant pills (medical crates 2x).");
			Plugin.SuppressInGameLoot = base.Config.Bind<bool>("Suppressant", "PillsInGameLoot", false, "Suppressant pills also turn up in the game's own drug and medical loot (on top of the crates above).");
			Plugin.SuppressEndBroodBefore = base.Config.Bind<float>("Suppressant", "EndBroodBefore", 50f, "A pill (5 ml) ends a brood that has not grown past this many percent (cramps, a little blood, she is drained). Later it is too late.");
			Plugin.SuppressCorruptionCure = base.Config.Bind<float>("Suppressant", "CorruptionCure", 1f, "Corruption it takes away per minute while it works (about 10 a dose), never below the floor of CorruptionFloorShare.");
			if (Plugin.SuppressRecipeInt.Value == 5)
			{
				Plugin.SuppressRecipeInt.Value = 8;
			}
			if (Mathf.Approximately(Plugin.SuppressCrateChance.Value, 0.06f) || Mathf.Approximately(Plugin.SuppressCrateChance.Value, 0.02f))
			{
				Plugin.SuppressCrateChance.Value = 0.01f;
			}
			Plugin.ScalpelCrateChance = base.Config.Bind<float>("Scalpel", "CrateChance", 0.04f, "Chance a broken crate holds a scalpel (medical crates 2.5x).");
			Plugin.CutSeconds = base.Config.Bind<float>("Scalpel", "CutSeconds", 8f, "About how long cutting a living suit off takes with the scalpel (a worse blade longer, a later suit stage longer).");
			Plugin.CounterInLoot = base.Config.Bind<bool>("Suppressant", "InGameLoot", true, "The scalpel (and the pills, if PillsInGameLoot) also turn up in the game's own loot.");
			Plugin.MaskProtection = base.Config.Bind<float>("LustGas", "MaskProtection", 0.75f, "How much of the lust gas a mask over her mouth (a dust mask) keeps out.");
			Plugin.MaskWear = base.Config.Bind<float>("LustGas", "MaskWear", 0.0006f, "Condition the mask loses per second in the gas (the filter clogs; a worn-out mask lets more through).");
			Plugin.ClothesTearPerMinute = base.Config.Bind<float>("Clothes", "TearPerMinute", 0.3f, "Condition her clothes lose per minute while something has hold of her (tentacles, flesh, a flesh wall, the worm; raiders 1.5x). At 0 the piece slips off her.");
			Plugin.ClothesAssimilatePerMinute = base.Config.Bind<float>("Clothes", "AssimilatePerMinute", 0.25f, "Condition per minute a living suit on her takes from what she wears over it (faster at later stages). At 0 the piece is gone into the suit.");
			Plugin.SuitLight = base.Config.Bind<float>("Bodysuit", "IndicatorLight", 0.35f, "Brightness of the little status light on the spacesuits (0 = none). It lights only what is right next to her.");
			Plugin.HideCrouchGuide = base.Config.Bind<bool>("Visual", "HideCrouchGuide", true, "Hide the game's white crouch-height line: crouching, it runs right across her head like a streak of light.");
			Plugin.WormShake = base.Config.Bind<float>("Worm", "Shake", 60f, "How hard the ground shakes as the giant worm comes near (0 = no shaking).");
			Plugin.WormShakeRange = base.Config.Bind<float>("Worm", "ShakeRange", 45f, "From how far away the shaking starts.");
			Plugin.WormHearRange = base.Config.Bind<float>("Worm", "HearRange", 70f, "From how far away she hears it (a low rumble, now and then a growl, louder as it comes; 0 = silent).");
			Plugin.WormDigMaxHealth = base.Config.Bind<float>("Worm", "DigMaxBlockHealth", 300f, "The worm breaks the blocks it bores through up to this hardness (gravel, dirt, stone; 0 = it breaks nothing).");
			Plugin.PitsPerLayer = base.Config.Bind<float>("Pits", "PerLayer", 1.5f, "Hidden pits on every new layer (a fleshy maw or a slime pit under ordinary-looking floor, bones and loot beside them).");
			Plugin.PitCloseSeconds = base.Config.Bind<float>("Pits", "MawCloseSeconds", 7f, "Seconds the maw takes to shut once it opens: the time she has to jump out.");
			Plugin.PitDigestAfter = base.Config.Bind<float>("Pits", "DigestAfter", 25f, "Seconds in the maw (or in the slime pit's tentacles) before it starts to digest her.");
			Plugin.PitEscape = base.Config.Bind<float>("Pits", "EscapeChance", 0.3f, "Chance per struggle to tear the shut maw open.");
			Plugin.PitSlimePull = base.Config.Bind<float>("Pits", "SlimePull", 4f, "How hard the slime pit's tentacles drag her down.");
			Plugin.PitRavishChance = base.Config.Bind<float>("Pits", "RavishChance", 0.5f, "Share of maws that do not digest what they swallow but use it: orgasm after orgasm until she is spent.");
			Plugin.PitOrgasmsToDeath = base.Config.Bind<int>("Pits", "OrgasmsToDeath", 6, "In a ravishing maw: after this many forced orgasms her breath gives out.");
			Plugin.PitLootMin = base.Config.Bind<int>("Pits", "LootMin", 3, "Least loot lying by the bones at a pit.");
			Plugin.PitLootMax = base.Config.Bind<int>("Pits", "LootMax", 5, "Most loot lying by the bones at a pit.");
			Plugin.FlowersPerLayer = base.Config.Bind<float>("Flower", "PerLayer", 1f, "Carnivorous flowers on every new jungle layer, and only there (rooted over a hidden stomach in the floor; Ctrl + numpad 5 cycles maw, slime pit and flower).");
			Plugin.FlowerHealth = base.Config.Bind<float>("Flower", "Health", 200f, "How much fire (or blast) the flower takes before it dies. Bullets and blows only make it flinch; dead, it lets her go, out of its stomach too.");
			Plugin.FlowerHoldSeconds = base.Config.Bind<float>("Flower", "HoldSeconds", 25f, "Seconds it holds her before its mouth, milking her, before it swallows her: her time to break free (tear-off key).");
			Plugin.FlowerEscape = base.Config.Bind<float>("Flower", "EscapeChance", 0.25f, "Chance per struggle out of its tongue at full strength; scaled down by lost stamina and exhaustion.");
			Plugin.FlowerBurrowHits = base.Config.Bind<float>("Flower", "BurrowHits", 5f, "Bullets or blows within 6 seconds that make it burrow under the ground (letting her go if it holds her before its mouth).");
			Plugin.FlowerBurrowSeconds = base.Config.Bind<float>("Flower", "BurrowSeconds", 40f, "Seconds it stays under the ground before it grows up again where it was (not while she stands on that spot).");
			Plugin.FlowerMilkChance = base.Config.Bind<float>("Flower", "MilkChance", 0.5f, "Share of flowers that go on milking her in their stomach until her heart gives out; the rest digest her (as the maw does).");
			Plugin.FlowerMilkDrain = base.Config.Bind<float>("Flower", "MilkExhaustion", 0.6f, "Exhaustion its milking adds per second (100 = her heart gives out); forced orgasms add [Tentacles] ExhaustionPerOrgasm on top.");
			Plugin.SlitsSand = base.Config.Bind<float>("Slit", "PerSandLayer", 3f, "Womb slits on every sand layer: a toothed slit in a slope's face, a narrow stomach in the hill behind it and a tentacle that drags her in.");
			Plugin.SlitsEarly = base.Config.Bind<float>("Slit", "PerEarlyLayer", 1.5f, "Womb slits on each of the first two layers (none deeper than the sand, none in the jungle).");
			Plugin.SlitHealth = base.Config.Bind<float>("Slit", "Health", 220f, "How much fire (or blast) a womb slit takes before it dies. Bullets and blows only make its tentacle flinch.");
			Plugin.SlitReach = base.Config.Bind<float>("Slit", "Reach", 8f, "How far (world units) its tentacle reaches out of the slit.");
			Plugin.SlitDragSpeed = base.Config.Bind<float>("Slit", "DragSpeed", 0.8f, "How fast (units a second) the tentacle drags her to the slit once it has her.");
			Plugin.SlitEscape = base.Config.Bind<float>("Slit", "EscapeChance", 0.22f, "Chance per struggle (tear-off key) to tear free of its tentacle at full strength.");
			Plugin.SlitFloodChance = base.Config.Bind<float>("Slit", "FloodChance", 0.4f, "Share of womb slits that flood their stomach with semen once they have her: it rises over her and she slowly drowns in it (the game's own drowning) unless she gets out.");
			Plugin.SlitFloodAfter = base.Config.Bind<float>("Slit", "FloodAfter", 10f, "Seconds in its stomach before the semen starts to rise.");
			Plugin.SlitFloodSeconds = base.Config.Bind<float>("Slit", "FloodSeconds", 50f, "Seconds for the semen to fill its stomach (her head goes under last).");
			Plugin.SlitPullStrength = base.Config.Bind<float>("Slit", "PullStrength", 3f, "How hard its tentacle hauls her: the grip on the limb it has, and how much of her weight it carries (3 and up: all of it, she is dragged along limp and lifted up a slope).");
			Plugin.SlitBlastRange = base.Config.Bind<float>("Slit", "BlastRange", 6f, "The blast of a stick of dynamite the slit has swallowed (the game's own explosion; a stick in the open is 18): it tears its stomach and the hill round it open.");
			Plugin.LatexPerLayer = base.Config.Bind<float>("Latex", "PerLayer", 1.5f, "Liquid latex on every layer from FirstLayer on: a big living slug of black latex. Some crawl, some hang from a cave's ceiling, some lie flat as a puddle.");
			Plugin.LatexFirstLayer = base.Config.Bind<float>("Latex", "FirstLayer", 1f, "The first layer with liquid latex (0 is the first layer of a run, 1 the second).");
			Plugin.LatexHealth = base.Config.Bind<float>("Latex", "Health", 180f, "How much fire (or blast) a liquid latex takes before it dies. Bullets and blows only make it flinch and, enough of them, let go of her.");
			Plugin.LatexSight = base.Config.Bind<float>("Latex", "Sight", 12f, "How far (world units) a crawling one notices her and comes for her.");
			Plugin.LatexSpeed = base.Config.Bind<float>("Latex", "CrawlSpeed", 1.1f, "How fast (units a second) it crawls after her.");
			Plugin.LatexReach = base.Config.Bind<float>("Latex", "Reach", 6f, "How far its two pseudopods shoot out to catch her round the waist and a wrist (closer than 2.8 it snatches a leg).");
			Plugin.LatexDragSpeed = base.Config.Bind<float>("Latex", "DragSpeed", 0.9f, "How fast (units a second) it reels her in once its pseudopods have her.");
			Plugin.LatexEscape = base.Config.Bind<float>("Latex", "EscapeChance", 0.18f, "Chance per struggle (tear-off key) to tear free while it reels her in or holds her (easier while she is still sinking into the puddle, harder once it has fallen over all of her).");
			Plugin.LatexEscapeInside = base.Config.Bind<float>("Latex", "EscapeInsideChance", 0.1f, "Chance per struggle to tear out of its belly once it has swallowed her (a lit torch in her hand makes it spit her out at once).");
			Plugin.LatexLetGoHits = base.Config.Bind<float>("Latex", "LetGoHits", 5f, "Bullets or blows on it within 6 seconds that make it let go of her.");
			Plugin.LatexSwallowAfter = base.Config.Bind<float>("Latex", "SwallowAfter", 25f, "Seconds it holds and uses her before it swallows her.");
			Plugin.LatexTransformAfter = base.Config.Bind<float>("Latex", "CreepSeconds", 12f, "Seconds its latex takes to close over her in its belly once her peak there has left her spent (no more sex then); after that it is her suit, the living latex.");
			Plugin.LatexRemoveLoose = base.Config.Bind<float>("Latex", "TearOffLoosened", 0.25f, "Chance per try to tear the living latex suit off while antiserum has loosened it (otherwise 1%).");
			Plugin.LatexLooseMinutes = base.Config.Bind<float>("Latex", "LooseMinutes", 6f, "Minutes a dose of antiserum keeps the living latex loose.");
			Plugin.LatexSeal = base.Config.Bind<float>("Latex", "SealsWounds", 0.08f, "Bleeding the living latex clots per second per wound (plus 1.2% of it).");
			Plugin.LatexHeal = base.Config.Bind<float>("Latex", "Mends", 2.5f, "How fast the living latex mends her skin and muscle (the living cloth at stage 3 is 1).");
			Plugin.LatexMind = base.Config.Bind<float>("Latex", "MindChange", 0.004f, "Submission the living latex adds per second (half as much corruption): it changes how she thinks.");
			Plugin.LatexWarmth = base.Config.Bind<float>("Latex", "Warmth", 0.15f, "Insulation against the cold the living latex adds per stage (it covers all of her: five stages' worth).");
			Plugin.LatexGlow = base.Config.Bind<float>("Latex", "EyeGlow", 1f, "How bright the living latex's violet eyes glow (0: no light).");
			Plugin.LatexTakeoverMinutes = base.Config.Bind<float>("Latex", "TakeoverMinutes", 10f, "Minutes in the living latex suit until its corruption of her reaches 100 and it takes her will: the run ends (deadly scenes on), or she is out cold and its hold eases back to 60 (off). Shown on the EROS-SCAN monitor while she wears it; off her it ebbs at half the speed.");
			Plugin.PondPerLayer = base.Config.Bind<float>("LatexPond", "PerLayer", 0.7f, "Latex ponds (a pool of living black latex in a dip of the floor) on every new layer from FirstLayer down (scaled by the difficulty).");
			Plugin.PondFirstLayer = base.Config.Bind<float>("LatexPond", "FirstLayer", 2f, "The first layer with latex ponds.");
			Plugin.PondPull = base.Config.Bind<float>("LatexPond", "Pull", 3f, "How hard it drags her down once she steps in.");
			Plugin.PondEscape = base.Config.Bind<float>("LatexPond", "EscapeChance", 0.55f, "Chance per struggle (the tear-off key) to tear free while it drags her down (less the deeper she is).");
			Plugin.PondEscapeUnder = base.Config.Bind<float>("LatexPond", "EscapeUnderChance", 0.3f, "Chance per struggle once she is under (less the longer she has been under).");
			if (Mathf.Approximately(Plugin.PondEscape.Value, 0.35f))
			{
				Plugin.PondEscape.Value = 0.55f;
			}
			if (Mathf.Approximately(Plugin.PondEscapeUnder.Value, 0.18f))
			{
				Plugin.PondEscapeUnder.Value = 0.3f;
			}
			Plugin.PondUnderSeconds = base.Config.Bind<float>("LatexPond", "UnderSeconds", 15f, "Seconds under it before it is over: she drowns in it (deadly scenes on) or it spits her out (off).");
			Plugin.PondLetGoHits = base.Config.Bind<float>("LatexPond", "LetGoHits", 4f, "Bullets or blows on it within 6 seconds that make it let go of her while it drags her down.");
			Plugin.PondSatedSeconds = base.Config.Bind<float>("LatexPond", "SatedSeconds", 0f, "After it spat her out, it takes no one for this long (0: it takes her again at once; a second and a half always, for her to land clear).");
			if (Mathf.Approximately(Plugin.PondSatedSeconds.Value, 90f))
			{
				Plugin.PondSatedSeconds.Value = 0f;
			}
			Plugin.LatexReformSeconds = base.Config.Bind<float>("Latex", "ReformSeconds", 20f, "Shot apart (any bullet or blow on one that does not have her), a liquid latex runs out into a puddle that sinks into the floor; this many seconds later it forms again there (not while she stands on the spot). Fire and blasts still kill it for good.");
			Plugin.LatexNightVision = base.Config.Bind<float>("Latex", "NightVision", 0.45f, "The living latex lets her see in the dark: the world's light, violet, is never dimmer than this while she wears it (0: off; the game's darkest setting is 0, its brightest 0.4).");
			Plugin.SlitLetGoHits = base.Config.Bind<float>("Slit", "LetGoHits", 4f, "Bullets or blows on it within 6 seconds that make it let go of her and pull its tentacle in for a while.");
			Plugin.PitFireHealth = base.Config.Bind<float>("Pits", "MawFireHealth", 150f, "How much fire the maw takes before it dies. Swallowing her with a lit torch in her hand burns it ([Fire] TorchHit) and it spits her out at once; a lit torch or a campfire at its lips burns it ([Fire] BurnPerSecond).");
			Plugin.SexWarmthRate = base.Config.Bind<float>("Balance", "SexWarmth", 0.15f, "Degrees per second sex warms her at full arousal (only from arousal 40, up to 37 degrees).");
			Plugin.StartSuitColorChance = base.Config.Bind<float>("Bodysuit", "StartColorChance2", 0.75f, "Chance the spacesuit by her pod at the start of a run is not the white one but another colour (red and blue, orange, black, blue, pink, olive, yellow).");
			Plugin.SuitSteps = base.Config.Bind<bool>("Bodysuit", "ShoeSounds", true, "Her steps click like stilettos in the latex outfit and clack like hard boots in the spacesuit.");
			Plugin.SuitStepVolume = base.Config.Bind<float>("Bodysuit", "ShoeVolume", 0.55f, "Loudness of those shoe sounds.");
			Plugin.NudeHabitRate = base.Config.Bind<float>("Bodysuit", "NudeHabitRate", 0.06f, "How fast she gets used to going naked (comfort points a second; 30 ends the shame, 70 with corruption she likes it).");
			Plugin.GasGeysersPerLayer = base.Config.Bind<float>("LustGas", "GeysersPerLayer", 2f, "Lust gas geysers scattered on every new layer (besides those in a gas biome).");
			Plugin.GasBiomeChance = base.Config.Bind<float>("LustGas", "BiomeChance", 0.5f, "Chance a new layer has a lust gas biome (a stretch of cave the gas hangs in).");
			Plugin.GasBiomeRadius = base.Config.Bind<float>("LustGas", "BiomeRadius", 13f, "Radius of a gas biome.");
			Plugin.GasGeysersPerBiome = base.Config.Bind<float>("LustGas", "GeysersPerBiome", 3f, "Geysers inside a gas biome.");
			Plugin.GasTriggerRange = base.Config.Bind<float>("LustGas", "TriggerRange", 3f, "How close she must come before a geyser rumbles and spouts.");
			Plugin.GasGeyserCooldown = base.Config.Bind<float>("LustGas", "GeyserCooldown", 20f, "Seconds before a geyser can spout again.");
			Plugin.GasStrength = base.Config.Bind<float>("LustGas", "Strength", 1f, "How strongly breathing the gas raises her lust and arousal.");
			Plugin.GasFog = base.Config.Bind<float>("LustGas", "BiomeFog", 0.45f, "How thick the pink fog over the screen gets inside a gas biome (0 = none).");
			Plugin.PotionLayerChance = base.Config.Bind<float>("LustDraught", "LayerChance", 0.3f, "Chance a new layer has a bottle of the lust draught lying somewhere.");
			Plugin.PotionCorpseChance = base.Config.Bind<float>("LustDraught", "CorpseChance", 0.01f, "Chance a dead body has a bottle by it (a layer has some 80 bodies).");
			Plugin.PotionCrateChance = base.Config.Bind<float>("LustDraught", "CrateChance", 0.05f, "Chance a crate she breaks open holds a bottle.");
			Plugin.PotionStrength = base.Config.Bind<float>("LustDraught", "Strength", 1f, "How strongly a drink of it raises her lust and arousal (a full bottle is about +55 lust, +40 arousal; half again in heat).");
			Plugin.PotionInLoot = base.Config.Bind<bool>("LustDraught", "InGameLoot", true, "The bottle also joins the game's own drink and drug loot pools.");
			Plugin.OutfitsPerLayer = base.Config.Bind<float>("Wardrobe", "PerLayer", 1.6f, "Pieces of clothing lying around on every new layer, on average (picked by rarity; latex, leotard, fishnets, black bra and the thermal suit only from the second layer on).");
			Plugin.OutfitCorpseChance = base.Config.Bind<float>("Wardrobe", "CorpseChance", 0.05f, "Chance that a dead body has a piece of clothing by it, mostly miner and lab overalls (a layer has some 80 bodies).");
			Plugin.OutfitCrateChance = base.Config.Bind<float>("Wardrobe", "CrateChance", 0.2f, "Chance that a crate or container she breaks open holds a piece of clothing (cargo crates x1.75, med crates x0.6, food boxes x0.3).");
			Plugin.OutfitsInLoot = base.Config.Bind<bool>("Wardrobe", "InGameLoot", true, "Everyday pieces also join the game's own loot (crates, corpses, broken containers).");
			Plugin.LatexComfortRate = base.Config.Bind<float>("Bodysuit", "LatexComfortRate", 0.15f, "How fast she gets used to (and starts to like) the latex outfit while wearing it.");
			Plugin.SlimeBogsPerLayer = base.Config.Bind<float>("SlimeBog", "BogsPerLayer", 2f, "Most slime bogs on a new layer (water pools turned to slime, or slime poured into hollows).");
			Plugin.DarkBogChance = base.Config.Bind<float>("SlimeBog", "DarkChance", 0.35f, "Chance for each slime bog to be the hosts' slime: nearly black, its suit is the hosts' one (stage 5 makes her one of them), and slime hosts stand by it and in it.");
			Plugin.SlimePoolChance = base.Config.Bind<float>("SlimeBog", "PoolChance", 0.35f, "Chance for each water pool on a layer to be slime instead.");
			Plugin.SlimeSuitChance = base.Config.Bind<float>("SlimeBog", "SuitChance", 0.12f, "Chance every 4 seconds in a bog that the slime crawls onto her as a sticky suit.");
			Plugin.ParasiteWarmth = base.Config.Bind<float>("Parasite", "SuitWarmth", 0.1f, "Insulation against the cold the living cloth and the symbiote add per stage, up to stage 4 (a hoodie gives about 0.6; nothing against heat).");
			Plugin.ParasiteProtectStage = base.Config.Bind<int>("Parasite", "ProtectFromStage", 3, "From this stage the living cloth looks after its host: it slowly clots her bleeding and slows infections (the symbiote one stage earlier).");
			Plugin.ParasiteSeal = base.Config.Bind<float>("Parasite", "SuitSealsWounds", 0.04f, "Bleeding the living cloth clots per second per wound (plus 1.2% of it); the symbiote half again as much.");
			Plugin.ParasiteInfectionMult = base.Config.Bind<float>("Parasite", "InfectionSpeed", 0.6f, "How fast an infection grows in her once the living suit protects her (1 = as usual; the symbiote squares it).");
			Plugin.SlimeWarmth = base.Config.Bind<float>("SlimeBog", "SuitWarmth", 0.1f, "Insulation against the cold the slime suit adds per stage (a hoodie gives about 0.6; nothing against heat).");
			Plugin.SlimeSeal = base.Config.Bind<float>("SlimeBog", "SuitSealsWounds", 0.05f, "At stage 4 the slime suit slowly clots her bleeding: bleeding taken off per second per wound (plus 1.2% of it). 0 turns it off.");
			Plugin.HostTakeoverMinutes = base.Config.Bind<float>("SlimeHosts", "TakeoverMinutes", 4f, "Minutes the hosts' slime suit at stage 4 takes to crawl over her face (stage 5): then she is one of them and the run ends. Faster when she is aroused or submissive; tear it off before that.");
			Plugin.HostConversionSeconds = base.Config.Bind<float>("SlimeHosts", "ConversionSeconds", 20f, "Seconds from the mask closing over her face to the end of the run: she fights it, fades, goes still; then a new host stands where she was.");
			Plugin.HostSparedKnockoutSeconds = base.Config.Bind<float>("SlimeHosts", "SparedKnockoutSeconds", 30f, "Deadly scenes off: seconds she lies out cold after the mask let her go (the slime slides back to stage 3; the hosts leave her be 3 minutes more).");
			Plugin.SlimeBondMinutes = base.Config.Bind<float>("SlimeBog", "BondMinutes", 10f, "Minutes of wearing the whole slime suit (stage 4) before it bonds with her (stage 5): she likes it, won't tear it off, it keeps infections away. Faster with corruption and submission.");
			Plugin.SlimeInfectionMult = base.Config.Bind<float>("SlimeBog", "InfectionSpeedAtStage4", 0.5f, "How fast an infection grows in her under the whole slime suit (1 = as usual). At stage 5 none takes hold.");
			Plugin.SlimeEscapeChance = base.Config.Bind<float>("SlimeBog", "EscapeChance", 0.3f, "Chance per struggle (tear-off key) to haul herself out of the slime.");
			Plugin.SlimeSwimSpeed = base.Config.Bind<float>("SlimeBog", "SwimSpeed", 2f, "How fast she can swim in slime (WASD, Space up). Slower than water; less when tired or submissive.");
			Plugin.SlimeSuffocation = base.Config.Bind<float>("SlimeBog", "Suffocation", 1.2f, "Extra blood oxygen lost per second with her head under the slime (on top of normal drowning).");
			Plugin.FillDrain = base.Config.Bind<float>("Womb", "FillDrain", 0.35f, "How fast 'filled' seeps out again (points per second; 100 = full).");
			Plugin.PregnancyChance = base.Config.Bind<float>("Womb", "PregnancyChance", 0.15f, "Chance per tentacle climax inside her to seed a brood (x3 in heat, up to x2 the fuller she is).");
			Plugin.PregnancyMinutes = base.Config.Bind<float>("Womb", "PregnancyMinutes", 25f, "Real minutes from seeding to birth (faster while she sleeps).");
			Plugin.BroodRootChance = base.Config.Bind<float>("Womb", "BroodRootChance", 0.5f, "Chance that one of the larvae she bears takes root as a new ceiling tentacle mass nearby.");
			Plugin.TentaclesPerLayer = base.Config.Bind<float>("Tentacles", "PerLayer", 2f, "Ceiling tentacle masses placed on every new layer.");
			Plugin.TentacleInsatiable = base.Config.Bind<float>("Tentacles", "InsatiableChance", 0.15f, "Chance that a mass never lets her go (raised by corruption and submission). She dies of exhaustion unless she breaks free.");
			Plugin.TentacleEscapeChance = base.Config.Bind<float>("Tentacles", "EscapeChance", 0.3f, "Chance per struggle (tear-off key) at full strength; scaled down by lost stamina and exhaustion.");
			Plugin.TentacleSceneSeconds = base.Config.Bind<float>("Tentacles", "SceneSeconds", 70f, "Longest a sated mass holds her (it also lets go after 3 climaxes).");
			Plugin.ExhaustionPerOrgasm = base.Config.Bind<float>("Tentacles", "ExhaustionPerOrgasm", 14f, "Exhaustion added by each orgasm the tentacles force on her (100 = her heart gives out).");
			Plugin.ExhaustionDeathRate = base.Config.Bind<float>("Tentacles", "ExhaustionDeathRate", 2f, "Held at full exhaustion (tentacles, the worm), her heart fibrillates: this much fibrillation (of 100, cardiac arrest) is added per second on top of the game's own rise. Break it (amiodarone, an autopump) to live: she passes out for ReviveKnockoutSeconds and is let go.");
			Plugin.ReviveKnockoutSeconds = base.Config.Bind<float>("Tentacles", "ReviveKnockoutSeconds", 120f, "Brought back from a heart giving out in a scene, she lies unconscious this long; nothing takes her meanwhile.");
			Plugin.WormChance = base.Config.Bind<float>("GiantWorm", "LayerChance", 0.3f, "Chance that a giant worm roams a new desert (sand) layer; it lives nowhere else.");
			Plugin.WormSenseRange = base.Config.Bind<float>("GiantWorm", "SenseRange", 22f, "It is blind: within this distance it knows where she is for a few seconds after she makes a noise (closer than 5 m it feels her anyway)");
			Plugin.WormSpeed = base.Config.Bind<float>("GiantWorm", "Speed", 9f, "Hunting speed (it lunges 1.6x faster). She runs at about 6-7.");
			Plugin.WormTentacleChance = base.Config.Bind<float>("GiantWorm", "TentacleChance", 0.4f, "Chance its gut holds tentacles that use her instead of digesting her (raised by corruption).");
			Plugin.WormInnerHealth = base.Config.Bind<float>("GiantWorm", "InnerHealth", 240f, "Damage her attacks must deal inside before it spits her out (claws hit for 30).");
			Plugin.WormEscapeChance = base.Config.Bind<float>("GiantWorm", "EscapeChance", 0.03f, "Chance per struggle (tear-off key) to squirm back out at full strength.");
			Plugin.WormAcid = base.Config.Bind<float>("GiantWorm", "Acid", 1f, "How fast its gut digests her (skin, muscle, clothes).");
			Plugin.WormAirLoss = base.Config.Bind<float>("GiantWorm", "AirLoss", 0.9f, "Blood oxygen lost per second while being digested.");
			Plugin.WormSceneSeconds = base.Config.Bind<float>("GiantWorm", "SceneSeconds", 90f, "How long the tentacles inside use her before it spits her out (if its walls have not sealed by then).");
			Plugin.WormCloseSeconds = base.Config.Bind<float>("GiantWorm", "CloseSeconds", 50f, "Seconds its walls take to close on her (faster on Hard, slower on Soft). Once they have sealed she cannot get out: struggling and blows no longer help, and it digests or smothers her.");
			Plugin.TentaclePosed = base.Config.Bind<bool>("Tentacles", "PosedScene", true, "Hold her in drawn poses (poses.txt) in the tentacles' grip rather than as a ragdoll.");
			Plugin.GameOverPicEnabled = base.Config.Bind<bool>("GameOver", "ScenePicture", true, "When she dies while a creature is using her, the game-over screen shows one random picture of it (nsfw/<kind>/).");
			Plugin.WebZonesPerLayer = base.Config.Bind<float>("Webs", "BiomesPerLayer", 1f, "Stretches of cave strung with webs on every new layer (deeper layers sometimes get one more).");
			Plugin.WebZoneRadius = base.Config.Bind<float>("Webs", "Radius", 11f, "Radius of a web biome.");
			Plugin.WebSheetsPerBiome = base.Config.Bind<float>("Webs", "WebsPerBiome", 6f, "Orb webs stretched across the passages of each biome; walking into one, she sticks to it.");
			Plugin.WebSheetSize = base.Config.Bind<float>("Webs", "WebSize", 3f, "Width of an orb web.");
			Plugin.WebSlow = base.Config.Bind<float>("Webs", "SpeedOnSilk", 0.7f, "Movement speed multiplier on the sticky silk over the floor.");
			Plugin.WebEscapeChance = base.Config.Bind<float>("Webs", "EscapeChance", 0.35f, "Chance of one tug (the struggle key) to tear her free; lower when tired, hurt or badly tangled.");
			Plugin.WebTangleSpeed = base.Config.Bind<float>("Webs", "TangleSpeed", 1f, "How much each failed tug winds the silk tighter round her (at 100% she is wrapped into a cocoon).");
			Plugin.WebLoosen = base.Config.Bind<float>("Webs", "LoosenPerSecond", 1.5f, "How fast the tangle eases while she keeps still (no tugging for 2 s).");
			Plugin.CocoonEscapeChance = base.Config.Bind<float>("Webs", "CocoonEscapeChance", 0.08f, "Chance of one tug to break out of the cocoon.");
			Plugin.CocoonWear = base.Config.Bind<float>("Webs", "CocoonWearPerTug", 0.015f, "Each failed tug tears the cocoon a little: this much is added to the chance of the next one.");
			Plugin.CocoonAirSeconds = base.Config.Bind<float>("Webs", "CocoonAirSeconds", 40f, "Seconds of air inside the cocoon (less when she is out of breath).");
			Plugin.CocoonSuffocation = base.Config.Bind<float>("Webs", "CocoonSuffocation", 1.2f, "Blood oxygen lost per second once the air in the cocoon is gone.");
			Plugin.MoodlesCollapse = base.Config.Bind<bool>("Moodles", "Collapse", true, "The mod's calmer statuses (arousal, lust, heat, filled, milk, nude, corruption, submission, latex...) fold away with the game's minor ones and show when the mouse is at the bottom of the screen or the health view is open. Traps and creatures holding her, a suit that threatens her (living cloth at stage 4-5, the hosts' slime not calmed), and anything flashing red, always stay in the main row.");
			Plugin.MoodlesSuitsMain = base.Config.Bind<bool>("Moodles", "SuitsAlwaysInMainRow", false, "Every living suit's status stays in the main row, even a quiet one (symbiote, bog slime, a broken-in or stable suit).");
			Plugin.MoodlesHidden = base.Config.Bind<string>("Moodles", "Hidden", "", "Statuses never to show, comma separated. The mod's: arousal, lust, heat, filled, milk, pregnant, exhausted, satisfied, orgasm, relief, nude, corruption, submission, pleasure, frustrated, suit (every living suit), suit_latex, tentacle, worm, web, flesh, slime, gas. The game's own work too, by their icon name (e.g. wet, tachycardia, fightorflight, highimmunity, lowimmunity). Example: Hidden = nude, corruption, wet");
			Plugin.MoodlesHidden.SettingChanged += delegate(object s, EventArgs e)
			{
				MoodleFilter.Reload();
			};
			Plugin.DebugKeys = base.Config.Bind<bool>("Debug", "DebugKeys", false, "Off by default; switch it in Settings -> Lewd -> Debug keys. Every key can be rebound in [DebugKeys] (a key, or a key with modifiers like \"Keypad1 + LeftControl\"; None switches one off). The defaults are on the numpad (Num Lock on); none of them work while the console or a text field is open, and each only while its part of the mod is on. Numpad alone - her: 1 +25 arousal, 2 heat on/off, 3 +20 submission, 4 +20 corruption, 5 reset her stats, 6 next bust size, 7 +40 filled, 8 start a brood / +25%; suits: 0 symbiote (onto her, or nearby), 9 next suit stage (no suit on: living cloth of a random colour), / slime suit, . a parasite nearby; outfits: * spacesuit + 40 semen, Enter latex, + next wardrobe piece, - take the suit off. Ctrl + numpad - spawning: 1 flesh biome, 2 living flesh, 3 ceiling tentacles, 4 slime bog, 5 hidden pit (maw, slime pit, carnivorous flower in turn), 6 flesh wall, 7 slime wall, 8 web biome, 9 web (again: cocoon), 0 a giant worm, . lust gas vent, / lust gas biome, * toys and items, - a raider, + a pack of slime hosts, Enter a loaded pistol in her hand (a gun in it already: filled). Alt + numpad - restraints and the rest (each pressed again takes it off): 1 wrist cuffs, 2 ankle cuffs, 3 gag, 4 collar, 5 chastity belt, 6 straitjacket, - locking latex suit, + harness (locked, random colour), 7 every restraint off, 8 a restraint key and lockpicks, 9 next plug (plug, tail plug, inflatable, vibrator, none), 0 a neutral camp nearby, . a trader nearby, / the BB Corps suit, * a sewing kit, Enter her second chance.");
			this.LoadIcons(Path.Combine(CS$<>8__locals1.root, "icons"));
			ParasiteArt.Load(Path.Combine(CS$<>8__locals1.root, "parasite"));
			FleshArt.Load(Path.Combine(CS$<>8__locals1.root, "flesh"));
			WallArt.Load(Path.Combine(CS$<>8__locals1.root, "walls"));
			LewdHealthPanel.Load(Path.Combine(CS$<>8__locals1.root, "health"));
			LewdHealthPanel.Bind(base.Config);
			InvasionPopup.Load(Path.Combine(CS$<>8__locals1.root, "health", "invasion"));
			InvasionPopup.Bind(base.Config);
			WebArt.Load(Path.Combine(CS$<>8__locals1.root, "web"));
			Outfits.Load(CS$<>8__locals1.root);
			Semen.Load(Path.Combine(CS$<>8__locals1.root, "splat"));
			TentacleArt.Load(Path.Combine(CS$<>8__locals1.root, "tentacle"));
			FlowerArt.Load(Path.Combine(CS$<>8__locals1.root, "flower"));
			SlitArt.Load(Path.Combine(CS$<>8__locals1.root, "slit"));
			LatexArt.Load(Path.Combine(CS$<>8__locals1.root, "latexblob"));
			WormArt.Load(Path.Combine(CS$<>8__locals1.root, "worm"));
			GameOverPic.Init(Path.Combine(CS$<>8__locals1.root, "nsfw"));
			Kit.Log = delegate(string m)
			{
				CS$<>8__locals1.<>4__this.Logger.LogInfo(m);
			};
			Kit.Warn = delegate(string m)
			{
				CS$<>8__locals1.<>4__this.Logger.LogWarning(m);
			};
			Poses.Load(Path.Combine(CS$<>8__locals1.root, "poses.txt"));
			Belly.Load(Path.Combine(CS$<>8__locals1.root, "belly"));
			Plugin.TextRu = CS$<>8__locals1.<Awake>g__ReadLang|3("ru");
			Plugin.TextEn = CS$<>8__locals1.<Awake>g__ReadLang|3("en");
			Plugin.TextZh = CS$<>8__locals1.<Awake>g__ReadLang|3("zh");
			Plugin.PhrasesZh = CS$<>8__locals1.<Awake>g__ReadLang|3("zh_phrases");
			Sfx.Load(Path.Combine(CS$<>8__locals1.root, "sounds"));
			base.Logger.LogInfo(string.Format("LEWD Casualties UNKNOWN: {0} icons, {1} strings, {2} sounds ({3})", new object[]
			{
				Plugin.Icons.Count,
				Plugin.Text.Count,
				Sfx.Count,
				Sfx.Summary
			}));
			SkinVoice.Init(base.Config, CS$<>8__locals1.root);
			DebugBinds.Init(base.Config);
			new Harmony("jens.casualtiesunknown.lewdstatus").PatchAll(typeof(Plugin).Assembly);
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x00053430 File Offset: 0x00051630
		private void LoadIcons(string dir)
		{
			if (!Directory.Exists(dir))
			{
				return;
			}
			foreach (string text in Directory.GetFiles(dir, "*.png"))
			{
				Texture2D texture2D = new Texture2D(2, 2, 4, false)
				{
					filterMode = 0,
					wrapMode = 1
				};
				ImageConversion.LoadImage(texture2D, File.ReadAllBytes(text));
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				texture2D.name = fileNameWithoutExtension;
				Plugin.Icons[fileNameWithoutExtension] = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), 100f);
				Plugin.Icons[fileNameWithoutExtension].name = fileNameWithoutExtension;
			}
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x000534F8 File Offset: 0x000516F8
		internal static void BasicsOff()
		{
			try
			{
				Relief.ForceStop();
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Switching off: " + ex.Message);
			}
			Plugin.Log.LogInfo("LEWD MOD off: statuses and clothes paused; creatures and traps follow their own switches");
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x00053548 File Offset: 0x00051748
		internal static string T(string key)
		{
			string text;
			if (!Plugin.Text.TryGetValue(key, out text) && !Plugin.TextEn.TryGetValue(key, out text) && !Plugin.TextRu.TryGetValue(key, out text))
			{
				return key;
			}
			return text;
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x00053588 File Offset: 0x00051788
		internal void Tick(Body body)
		{
			if (body != this.trackedBody)
			{
				this.trackedBody = body;
				if (!SaveSystem.loadedRun)
				{
					Plugin.S = new State();
					Plugin.FreshRun = true;
					HostConversion.Reset();
					LatexTakeover.Reset();
					Heart.Reset();
					Devices.Reset();
					base.Logger.LogInfo("New run: lewd status reset");
				}
			}
			Prof.Begin();
			LewdHealthPanel.Update(body);
			LatexHud.Update(body);
			InvasionPopup.Update(body);
			BioWarning.Update(body);
			if (Plugin.DebugKeys.Value)
			{
				this.DebugInput(body);
				Bridge.Tick(body);
			}
			bool on = LewdSettings.On;
			if (body.alive && Time.deltaTime > 0f)
			{
				float num = Mathf.Min(Time.deltaTime, 0.25f);
				if (on)
				{
					Plugin.Simulate(body, num);
					Relief.Update(body, num);
					Devices.Update(body, num);
					Plugs.Update(body, num);
					Lockpick.Update();
					LatexMood.Update(body, num);
					NudeMood.Update(body, num);
					WardrobeEffects.Update(body, num);
					Clothes.Update(body, num);
					this.HornyThoughts(body, num);
				}
				SkinVoice.Tick(num);
				Bust.Update(body);
				Neck.Update(body);
				FurRealCompat.Update();
				SlimeBog.Update(body, num);
				Worm.Update(body, num);
				Tentacles.Update(body, num);
				Womb.Update(body, num);
				FleshZones.Update(body, num);
				Webs.Update(body, num);
				if (LewdSettings.TrapsOn)
				{
					LustGas.Update(body, num);
				}
				Flesh.Update(body, num);
				Burning.Tick(num);
				Mercy.Update(body, num);
				if (LewdSettings.TrapsOn)
				{
					FleshWall.Update(body, num);
				}
				if (LewdSettings.TrapsOn)
				{
					Pits.Update(body, num);
				}
				if (LewdSettings.TrapsOn || Flowers.Holding)
				{
					Flowers.Update(body, num);
				}
				if (LewdSettings.TrapsOn || Slits.Busy)
				{
					Slits.Update(body, num);
				}
				if (LewdSettings.TrapsOn || Latexes.Busy)
				{
					Latexes.Update(body, num);
				}
				if (LewdSettings.TrapsOn || LatexPond.Busy)
				{
					LatexPond.Update(body, num);
				}
				LatexVoice.Update(body);
				Engulfed.Tick(body);
				Semen.Update(body, num);
				Blanket.Update(body, num);
				SuitCut.Update(body, num);
				Suit.Update(body, num);
				HostConversion.Update(body, num);
				LatexTakeover.Tick(body, num);
				LatexMites.Update(body, num);
				Heart.Update(body, num);
				WallCrawl.Update(body, num);
				SymbioteVoice.Update(body, num);
				Puppet.Update(body, num);
				Parasites.Update(body);
				GameOverPic.Track(body);
				RunStats.Track(body);
			}
			else
			{
				if (Tentacles.Holding)
				{
					Tentacles.Update(body, 0f);
				}
				if (Worm.Inside)
				{
					Worm.Update(body, 0f);
				}
				if (Webs.Caught)
				{
					Webs.Update(body, 0f);
				}
				if (FleshWall.Holding)
				{
					FleshWall.Update(body, 0f);
				}
				if (Pits.InMaw)
				{
					Pits.Update(body, 0f);
				}
				if (Flowers.Holding)
				{
					Flowers.Update(body, 0f);
				}
				if (Slits.Holding || Slits.Inside)
				{
					Slits.Update(body, 0f);
				}
				if (Latexes.Busy)
				{
					Latexes.Update(body, 0f);
				}
				if (LatexPond.Busy)
				{
					LatexPond.Update(body, 0f);
				}
				if (LatexMites.Busy)
				{
					LatexMites.LetGo(body);
				}
				if (SlimeBog.Stuck)
				{
					SlimeBog.Update(body, 0f);
				}
				Engulfed.Tick(body);
				HostConversion.Update(body, 0f);
				LatexTakeover.Tick(body, Time.deltaTime);
				GameOverPic.Track(body);
				RunStats.Track(body);
			}
			Relief.SyncFlip();
			this.saveCheck -= Time.unscaledDeltaTime;
			if (this.saveCheck < 0f)
			{
				this.saveCheck = 5f;
				if (!SaveSystem.HasSave() && File.Exists(Plugin.SavePath))
				{
					File.Delete(Plugin.SavePath);
				}
			}
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x00053914 File Offset: 0x00051B14
		private void HornyThoughts(Body body, float dt)
		{
			if (Scenes.Busy || Relief.Active || Plugin.S.arousal < 60f)
			{
				return;
			}
			if ((this.hornyTimer -= dt * (Plugin.S.inHeat ? 1.6f : 1f)) > 0f)
			{
				return;
			}
			this.hornyTimer = Random.Range(70f, 140f);
			Relief.Say(body, (Plugin.S.filled > 50f) ? "horny_filled" : ((Plugin.S.corruption > 50f) ? "horny_corrupt" : "horny_idle"));
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x000539C8 File Offset: 0x00051BC8
		internal void DebugWear(Body body, OutfitDef d)
		{
			if (Outfits.WornIn(body, d.Slot) == d)
			{
				return;
			}
			Item wearableBySlotID = body.GetWearableBySlotID(d.Slot);
			if (wearableBySlotID)
			{
				body.DropWearable(wearableBySlotID);
			}
			GameObject gameObject = Outfits.SpawnAt(d, body.transform.position);
			Item item = (gameObject ? gameObject.GetComponent<Item>() : null);
			if (item)
			{
				body.WearWearable(item);
			}
			base.Logger.LogInfo(string.Format("Debug wear {0}: worn {1}", d.Id, Outfits.WornIn(body, d.Slot) == d));
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x00053A68 File Offset: 0x00051C68
		private void DebugInput(Body body)
		{
			Vector2 vector = body.transform.position + (body.isRight ? Vector2.right : Vector2.left) * 2.5f + Vector2.up;
			if (LewdSettings.On)
			{
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.SpacesuitAndCum))
				{
					Semen.Add(body, 40f);
					this.DebugWear(body, Outfits.Spacesuit);
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.LatexOutfit))
				{
					this.DebugWear(body, Outfits.Latex);
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.NextOutfit))
				{
					this.wardrobeIndex = (this.wardrobeIndex + 1) % Outfits.Wardrobe.Length;
					this.DebugWear(body, Outfits.Wardrobe[this.wardrobeIndex]);
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.ToysNearby))
				{
					Toys.DebugSpawn(vector);
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.TakeOffOutfit))
				{
					Item wearableBySlotID = body.GetWearableBySlotID("bodysuit");
					if (wearableBySlotID)
					{
						body.DropWearable(wearableBySlotID);
					}
					base.Logger.LogInfo("Debug: took off " + (wearableBySlotID ? wearableBySlotID.id : "nothing"));
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.Fill))
				{
					Womb.Fill(body, 40f);
					base.Logger.LogInfo(string.Format("Debug: filled {0:0}", Plugin.S.filled));
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.Pregnancy))
				{
					Womb.DebugAdvance(body);
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.Arousal))
				{
					Plugin.S.arousal = Mathf.Min(100f, Plugin.S.arousal + 25f);
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.Heat))
				{
					Plugin.S.inHeat = !Plugin.S.inHeat;
					Plugin.S.heatTimer = 0f;
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.Submission))
				{
					Plugin.S.submission = Mathf.Min(100f, Plugin.S.submission + 20f);
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.Corruption))
				{
					Plugin.S.corruption = Mathf.Min(100f, Plugin.S.corruption + 20f);
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.ResetStats))
				{
					Plugin.S = new State();
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.NextBust))
				{
					Plugin.S.bust = (Mathf.Clamp(Plugin.S.bust, 0, 2) + 1) % 3;
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.Arousal) || Plugin.<DebugInput>g__Key|282_0(DebugBinds.Heat) || Plugin.<DebugInput>g__Key|282_0(DebugBinds.Submission) || Plugin.<DebugInput>g__Key|282_0(DebugBinds.Corruption) || Plugin.<DebugInput>g__Key|282_0(DebugBinds.ResetStats))
				{
					base.Logger.LogInfo(string.Format("Debug: arousal {0:0} lust {1:0} heat {2} submission {3:0} corruption {4:0}", new object[]
					{
						Plugin.S.arousal,
						Plugin.S.lust,
						Plugin.S.inHeat,
						Plugin.S.submission,
						Plugin.S.corruption
					}));
				}
			}
			if (LewdSettings.TrapsOn)
			{
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.FleshBiome))
				{
					FleshZones.Build(body.transform.position, Plugin.FleshZoneRadius.Value, body.transform.position);
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.LivingFlesh))
				{
					RaycastHit2D raycastHit2D = Physics2D.Raycast(body.transform.position + (body.isRight ? Vector2.right : Vector2.left) * 5f + Vector2.up, Vector2.down, 8f, LayerMask.GetMask(new string[] { "Ground" }));
					if (raycastHit2D)
					{
						Flesh.Spawn(raycastHit2D.point);
					}
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.Tentacles))
				{
					Tentacles.DebugSpawn(body);
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.SlimeBog))
				{
					SlimeFluid.DebugPool(body, false);
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.WebBiome))
				{
					Webs.DebugBiome(body);
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.WebCocoon))
				{
					Webs.DebugCocoon(body);
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.FleshWall))
				{
					Walls.DebugWall(body, WallKind.Flesh);
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.HiddenPit))
				{
					Pits.DebugPit(body, null);
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.SlimeWall))
				{
					Walls.DebugWall(body, WallKind.Slime);
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.GasGeyser))
				{
					LustGas.DebugVent(body);
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.GasBiome))
				{
					LustGas.DebugBiome(body);
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.LiquidLatex))
				{
					this.latexKind = (this.latexKind + 1) % Plugin.LatexKinds.Length;
					base.Logger.LogInfo("Debug: " + Latexes.DebugMake(body, Plugin.LatexKinds[this.latexKind]));
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.LatexPond))
				{
					base.Logger.LogInfo("Debug: " + LatexPond.DebugMake(body));
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.LatexMite))
				{
					base.Logger.LogInfo("Debug: " + LatexMites.DebugSpawn(body, 1));
				}
				if (Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.LatexMites))
				{
					base.Logger.LogInfo("Debug: " + LatexMites.DebugSpawn(body, 2));
				}
			}
			if (LewdSettings.WormsOn && Plugin.<DebugInput>g__Spawn|282_1(DebugBinds.GiantWorm))
			{
				Worm.DebugSpawn(body);
			}
			if (LewdSettings.ParasitesOn)
			{
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.Symbiote))
				{
					if (Suit.Stage == 0)
					{
						Suit.Attach(body, 3);
					}
					else
					{
						Parasites.Spawn(vector, 3, false);
					}
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.NextSuitStage))
				{
					Suit.ForceNextStage(body);
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.SlimeSuit) && Suit.Stage == 0)
				{
					Suit.Attach(body, 6);
				}
				if (Plugin.<DebugInput>g__Key|282_0(DebugBinds.ParasiteNearby))
				{
					Parasites.Spawn(vector, Random.Range(0, 4), false);
				}
			}
			DebugAlt.Update(body);
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x0005405C File Offset: 0x0005225C
		internal static float Discomfort(Body b)
		{
			if (!b)
			{
				return 0f;
			}
			float num = Mathf.Clamp01((50f - b.hunger) / 35f);
			float num2 = Mathf.Clamp01((55f - b.thirst) / 35f);
			float num3 = Mathf.Clamp01((b.dirtyness - 50f) / 50f) * 0.7f * (1f - Plugin.S.corruption / 200f);
			float num4 = Mathf.Clamp01((25f - b.energy) / 18f) * 0.6f;
			float num5 = Mathf.Clamp01((b.sicknessAmount - 20f) / 60f);
			float num6 = (1f - num) * (1f - num2) * (1f - num3) * (1f - num4) * (1f - num5);
			return Mathf.Clamp01((1f - num6) * Plugin.DiscomfortDamping.Value);
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x00054158 File Offset: 0x00052358
		private static void Simulate(Body b, float dt)
		{
			Plugin.S.heatTimer += dt;
			float num = (Plugin.S.inHeat ? Plugin.HeatOnMinutes.Value : Plugin.HeatOffMinutes.Value) * 60f;
			if (Plugin.S.heatTimer >= num)
			{
				Plugin.S.inHeat = !Plugin.S.inHeat;
				Plugin.S.heatTimer = 0f;
			}
			float averagePain = b.averagePain;
			bool flag = Plugin.S.submission >= 50f;
			float num2 = 0.05f * Plugin.LustRate.Value * (Plugin.S.inHeat ? 4f : 1f) * (1f + Plugin.S.corruption * 0.01f);
			if (b.sleeping)
			{
				num2 *= 0.3f;
			}
			if (Plugin.S.satisfiedTimer > 0f)
			{
				Plugin.S.satisfiedTimer -= dt;
				num2 = 0f;
			}
			Suppressant.Update(dt);
			float num3 = Plugin.Discomfort(b);
			num2 *= 1f - 0.6f * num3;
			Plugin.S.lust += num2 * Suppressant.Factor * dt;
			if (b.wetness > 50f)
			{
				Plugin.S.lust -= 0.08f * dt;
			}
			Plugin.S.lust = Mathf.Clamp(Plugin.S.lust, Plugin.LustFloor, 100f);
			if (Plugin.S.frustrationTimer > 0f)
			{
				Plugin.S.frustrationTimer -= dt;
				Mood.Change(b, -0.02f * dt);
			}
			Plugin.SexWarmth(b, dt);
			if (Relief.Active)
			{
				Plugin.UpdateOrgasmTimer(b, dt);
				return;
			}
			float num4 = Plugin.S.lust * Mathf.Clamp01(Plugin.IdleArousalShare.Value);
			float num5 = (flag ? 0f : (Mathf.Clamp01((averagePain - 10f) / 60f) * Mathf.Clamp01(Plugin.PainDamping.Value)));
			num4 *= (1f - 0.7f * num3) * (1f - 0.8f * num5);
			float num6 = ((num4 > Plugin.S.arousal) ? 0.4f : 0.3f);
			Plugin.S.arousal = Mathf.MoveTowards(Plugin.S.arousal, num4, num6 * dt);
			if (averagePain > 10f)
			{
				Plugin.S.arousal += (flag ? (averagePain * 0.004f * (Plugin.S.submission * 0.01f)) : (-averagePain * 0.025f * Plugin.PainDamping.Value)) * dt;
			}
			if (num3 > 0f)
			{
				Plugin.S.arousal -= num3 * 0.5f * (Scenes.Busy ? 0.3f : 1f) * dt;
			}
			if (b.sleeping)
			{
				Plugin.S.arousal -= 1f * dt;
			}
			if (b.wetness > 50f)
			{
				Plugin.S.arousal -= 0.15f * dt;
			}
			Plugin.S.arousal = Mathf.Clamp(Plugin.S.arousal, 0f, 100f);
			if (averagePain > 20f && Plugin.S.arousal > 40f)
			{
				Plugin.S.submission += 0.1f * dt * (Plugin.S.inHeat ? 1.5f : 1f);
			}
			else
			{
				Plugin.S.submission -= Plugin.SubmissionDecay.Value * dt;
			}
			Plugin.S.submission = Mathf.Clamp(Plugin.S.submission, 0f, 100f);
			if (Plugin.S.lust > Plugin.CorruptionLustThreshold.Value)
			{
				Plugin.S.corruption = Mathf.Min(100f, Plugin.S.corruption + Plugin.CorruptionFromLust.Value * dt);
			}
			Plugin.S.corruptionPeak = Mathf.Max(Plugin.S.corruptionPeak, Plugin.S.corruption);
			if (Plugin.S.lust < 40f && !Relief.Active)
			{
				Plugin.S.corruption = Mathf.Max(Plugin.S.corruptionPeak * Mathf.Clamp01(Plugin.CorruptionFloorShare.Value), Plugin.S.corruption - Plugin.CorruptionDecayPerHour.Value / 3600f * dt);
			}
			if (Plugin.S.arousal > 75f && Plugin.S.orgasmTimer <= 0f)
			{
				Mood.Change(b, -0.02f * dt);
				b.stamina = Mathf.Max(0f, b.stamina - 0.3f * dt);
			}
			if (Plugin.S.inHeat)
			{
				Mood.Change(b, 0.01f * dt);
			}
			if (Plugin.S.corruption > 60f && Plugin.S.arousal < 20f)
			{
				Mood.Consumed(b, 0.02f * dt * (Plugin.S.corruption - 60f) * 0.05f);
			}
			if (Plugin.S.orgasmTimer > 0f)
			{
				Plugin.UpdateOrgasmTimer(b, dt);
				return;
			}
			if (Plugin.S.arousal >= 99f && b.conscious && (Plugin.IdleOrgasm.Value || Plugin.Stimulated))
			{
				Plugin.Orgasm(b, false);
			}
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x00054708 File Offset: 0x00052908
		private static void SexWarmth(Body b, float dt)
		{
			if ((!Relief.Active && !Flowers.Holding && !Tentacles.Holding && !Worm.Sex && !Flesh.Holding && !FleshWall.Holding && !Pits.InMaw && !Slits.Inside && !External.ScavHolding) || Plugin.S.arousal < 40f || b.temperature >= 37f)
			{
				return;
			}
			b.temperature = Mathf.Min(37f, b.temperature + Plugin.SexWarmthRate.Value * (Plugin.S.arousal / 100f) * dt);
			Plugin.warmSaid -= dt;
			if (b.temperature < 35.5f && Plugin.warmSaid <= 0f)
			{
				Plugin.warmSaid = 40f;
				Relief.Say(b, "sex_warm");
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000508 RID: 1288 RVA: 0x000547E4 File Offset: 0x000529E4
		private static bool Stimulated
		{
			get
			{
				return Flowers.Holding || Tentacles.Holding || Worm.Sex || Flesh.Holding || FleshWall.Holding || Pits.InMaw || Slits.Inside || Latexes.Using || LatexPond.Inside || SlimeBog.Stuck || Suit.Stage > 0 || Webs.Caught || External.ScavHolding;
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000509 RID: 1289 RVA: 0x0005484D File Offset: 0x00052A4D
		private static float LustFloor
		{
			get
			{
				return Mathf.Max(Plugin.S.corruption * 0.4f, Plugin.S.inHeat ? 30f : 0f);
			}
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x0005487C File Offset: 0x00052A7C
		private static void UpdateOrgasmTimer(Body b, float dt)
		{
			if (Plugin.S.orgasmTimer <= 0f)
			{
				return;
			}
			Plugin.S.orgasmTimer -= dt;
			if (Plugin.S.orgasmTimer > 17f)
			{
				b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 0.2f);
			}
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x000548D4 File Offset: 0x00052AD4
		internal static void Orgasm(Body b, bool self)
		{
			InvasionPopup.OnOrgasm(b, self);
			Plugin.S.orgasmTimer = 20f;
			Plugin.S.orgasms++;
			Plugin.S.arousal = 15f;
			Plugin.S.lust = Mathf.Max(Plugin.LustFloor, Plugin.S.lust - (self ? 60f : 50f));
			Plugin.S.corruption = Mathf.Min(100f, Plugin.S.corruption + Plugin.CorruptionPerOrgasm.Value * (self ? 0.5f : 1f));
			if (Plugin.S.inHeat)
			{
				Plugin.S.submission = Mathf.Min(100f, Plugin.S.submission + 2f);
			}
			if (self)
			{
				Plugin.S.selfOrgasms++;
				Plugin.S.satisfiedTimer = Plugin.SatisfiedSeconds.Value * (Plugin.S.inHeat ? 0.5f : 1f);
				Plugin.S.frustrationTimer = 0f;
			}
			Mood.Change(b, self ? 15f : 20f);
			b.stamina = Mathf.Min(b.stamina, 25f);
			b.eyeCloseTime = 3f;
			b.eatTime = Mathf.Max(b.eatTime, 1.5f);
			Sfx.Play(b, 1f, new string[] { "climax", "moan" });
			if (!self)
			{
				Sfx.Play(b, 0.5f, new string[] { "heartbeat" });
			}
			Suit.OnOrgasm(b);
			Plugin.Log.LogInfo(string.Format("Orgasm #{0} (self {1}), corruption {2:0}", Plugin.S.orgasms, self, Plugin.S.corruption));
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x00054AC7 File Offset: 0x00052CC7
		internal static string SavePath
		{
			get
			{
				return Path.Combine(Application.persistentDataPath, "lewdstatus.json");
			}
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x00054AD8 File Offset: 0x00052CD8
		internal static void Save()
		{
			try
			{
				File.WriteAllText(Plugin.SavePath, JsonConvert.SerializeObject(Plugin.S));
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError("Save failed: " + ex.Message);
			}
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x00054B28 File Offset: 0x00052D28
		internal static void Load()
		{
			try
			{
				if (File.Exists(Plugin.SavePath))
				{
					Plugin.S = JsonConvert.DeserializeObject<State>(File.ReadAllText(Plugin.SavePath)) ?? new State();
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError("Load failed: " + ex.Message);
			}
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x00054C20 File Offset: 0x00052E20
		[CompilerGenerated]
		internal static bool <DebugInput>g__Key|282_0(ConfigEntry<KeyboardShortcut> k)
		{
			return DebugBinds.Down(k);
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x00054C28 File Offset: 0x00052E28
		[CompilerGenerated]
		internal static bool <DebugInput>g__Spawn|282_1(ConfigEntry<KeyboardShortcut> k)
		{
			return DebugBinds.Down(k);
		}

		// Token: 0x040004CF RID: 1231
		public const string Guid = "jens.casualtiesunknown.lewdstatus";

		// Token: 0x040004D0 RID: 1232
		internal static Plugin Instance;

		// Token: 0x040004D1 RID: 1233
		internal static ManualLogSource Log;

		// Token: 0x040004D2 RID: 1234
		internal static readonly Dictionary<string, Sprite> Icons = new Dictionary<string, Sprite>();

		// Token: 0x040004D3 RID: 1235
		internal static Dictionary<string, string> TextRu = new Dictionary<string, string>();

		// Token: 0x040004D4 RID: 1236
		internal static Dictionary<string, string> TextEn = new Dictionary<string, string>();

		// Token: 0x040004D5 RID: 1237
		internal static Dictionary<string, string> TextZh = new Dictionary<string, string>();

		// Token: 0x040004D6 RID: 1238
		internal static Dictionary<string, string> PhrasesZh = new Dictionary<string, string>();

		// Token: 0x040004D7 RID: 1239
		internal static ConfigEntry<float> LustRate;

		// Token: 0x040004D8 RID: 1240
		internal static ConfigEntry<float> HeatOffMinutes;

		// Token: 0x040004D9 RID: 1241
		internal static ConfigEntry<float> HeatOnMinutes;

		// Token: 0x040004DA RID: 1242
		internal static ConfigEntry<float> CorruptionPerOrgasm;

		// Token: 0x040004DB RID: 1243
		internal static ConfigEntry<float> IdleArousalShare;

		// Token: 0x040004DC RID: 1244
		internal static ConfigEntry<float> CorruptionDecayPerHour;

		// Token: 0x040004DD RID: 1245
		internal static ConfigEntry<float> DiscomfortDamping;

		// Token: 0x040004DE RID: 1246
		internal static ConfigEntry<float> PainDamping;

		// Token: 0x040004DF RID: 1247
		internal static ConfigEntry<float> CorruptionFloorShare;

		// Token: 0x040004E0 RID: 1248
		internal static ConfigEntry<float> TentacleHurtSeconds;

		// Token: 0x040004E1 RID: 1249
		internal static ConfigEntry<float> ReviveKnockoutSeconds;

		// Token: 0x040004E2 RID: 1250
		internal static ConfigEntry<float> SubmissionDecay;

		// Token: 0x040004E3 RID: 1251
		internal static ConfigEntry<float> ExhaustionRecovery;

		// Token: 0x040004E4 RID: 1252
		internal static ConfigEntry<float> SatisfiedSeconds;

		// Token: 0x040004E5 RID: 1253
		internal static ConfigEntry<float> CorruptionFromLust;

		// Token: 0x040004E6 RID: 1254
		internal static ConfigEntry<float> CorruptionLustThreshold;

		// Token: 0x040004E7 RID: 1255
		internal static ConfigEntry<float> MoodGainScale;

		// Token: 0x040004E8 RID: 1256
		internal static ConfigEntry<float> MoodLossScale;

		// Token: 0x040004E9 RID: 1257
		internal static ConfigEntry<float> ConsumedMoodScale;

		// Token: 0x040004EA RID: 1258
		internal static ConfigEntry<float> WebDebrisSeconds;

		// Token: 0x040004EB RID: 1259
		internal static ConfigEntry<bool> IdleOrgasm;

		// Token: 0x040004EC RID: 1260
		internal static ConfigEntry<float> SuppressLeak;

		// Token: 0x040004ED RID: 1261
		internal static ConfigEntry<float> SuppressMinutes;

		// Token: 0x040004EE RID: 1262
		internal static ConfigEntry<float> SuppressCrateChance;

		// Token: 0x040004EF RID: 1263
		internal static ConfigEntry<float> ScalpelCrateChance;

		// Token: 0x040004F0 RID: 1264
		internal static ConfigEntry<float> CutSeconds;

		// Token: 0x040004F1 RID: 1265
		internal static ConfigEntry<float> MaskProtection;

		// Token: 0x040004F2 RID: 1266
		internal static ConfigEntry<float> MaskWear;

		// Token: 0x040004F3 RID: 1267
		internal static ConfigEntry<int> SuppressRecipeInt;

		// Token: 0x040004F4 RID: 1268
		internal static ConfigEntry<bool> CounterInLoot;

		// Token: 0x040004F5 RID: 1269
		internal static ConfigEntry<bool> SuppressInGameLoot;

		// Token: 0x040004F6 RID: 1270
		internal static ConfigEntry<float> SuppressCorruptionCure;

		// Token: 0x040004F7 RID: 1271
		internal static ConfigEntry<float> SuppressEndBroodBefore;

		// Token: 0x040004F8 RID: 1272
		internal static ConfigEntry<float> ClothesTearPerMinute;

		// Token: 0x040004F9 RID: 1273
		internal static ConfigEntry<float> ClothesAssimilatePerMinute;

		// Token: 0x040004FA RID: 1274
		internal static ConfigEntry<float> SuitLight;

		// Token: 0x040004FB RID: 1275
		internal static ConfigEntry<bool> HideCrouchGuide;

		// Token: 0x040004FC RID: 1276
		internal static ConfigEntry<float> WormShake;

		// Token: 0x040004FD RID: 1277
		internal static ConfigEntry<float> WormShakeRange;

		// Token: 0x040004FE RID: 1278
		internal static ConfigEntry<float> WormDigMaxHealth;

		// Token: 0x040004FF RID: 1279
		internal static ConfigEntry<float> WormHearRange;

		// Token: 0x04000500 RID: 1280
		internal static ConfigEntry<float> PitsPerLayer;

		// Token: 0x04000501 RID: 1281
		internal static ConfigEntry<float> PitCloseSeconds;

		// Token: 0x04000502 RID: 1282
		internal static ConfigEntry<float> PitDigestAfter;

		// Token: 0x04000503 RID: 1283
		internal static ConfigEntry<float> PitEscape;

		// Token: 0x04000504 RID: 1284
		internal static ConfigEntry<float> PitSlimePull;

		// Token: 0x04000505 RID: 1285
		internal static ConfigEntry<float> PitRavishChance;

		// Token: 0x04000506 RID: 1286
		internal static ConfigEntry<int> PitLootMin;

		// Token: 0x04000507 RID: 1287
		internal static ConfigEntry<int> PitLootMax;

		// Token: 0x04000508 RID: 1288
		internal static ConfigEntry<int> PitOrgasmsToDeath;

		// Token: 0x04000509 RID: 1289
		internal static ConfigEntry<float> PitFireHealth;

		// Token: 0x0400050A RID: 1290
		internal static ConfigEntry<float> FlowersPerLayer;

		// Token: 0x0400050B RID: 1291
		internal static ConfigEntry<float> FlowerHealth;

		// Token: 0x0400050C RID: 1292
		internal static ConfigEntry<float> FlowerHoldSeconds;

		// Token: 0x0400050D RID: 1293
		internal static ConfigEntry<float> FlowerEscape;

		// Token: 0x0400050E RID: 1294
		internal static ConfigEntry<float> FlowerMilkChance;

		// Token: 0x0400050F RID: 1295
		internal static ConfigEntry<float> FlowerMilkDrain;

		// Token: 0x04000510 RID: 1296
		internal static ConfigEntry<float> FlowerBurrowHits;

		// Token: 0x04000511 RID: 1297
		internal static ConfigEntry<float> FlowerBurrowSeconds;

		// Token: 0x04000512 RID: 1298
		internal static ConfigEntry<float> LatexPerLayer;

		// Token: 0x04000513 RID: 1299
		internal static ConfigEntry<float> LatexFirstLayer;

		// Token: 0x04000514 RID: 1300
		internal static ConfigEntry<float> LatexHealth;

		// Token: 0x04000515 RID: 1301
		internal static ConfigEntry<float> LatexSight;

		// Token: 0x04000516 RID: 1302
		internal static ConfigEntry<float> LatexSpeed;

		// Token: 0x04000517 RID: 1303
		internal static ConfigEntry<float> LatexReach;

		// Token: 0x04000518 RID: 1304
		internal static ConfigEntry<float> LatexDragSpeed;

		// Token: 0x04000519 RID: 1305
		internal static ConfigEntry<float> LatexEscape;

		// Token: 0x0400051A RID: 1306
		internal static ConfigEntry<float> LatexEscapeInside;

		// Token: 0x0400051B RID: 1307
		internal static ConfigEntry<float> LatexLetGoHits;

		// Token: 0x0400051C RID: 1308
		internal static ConfigEntry<float> LatexSwallowAfter;

		// Token: 0x0400051D RID: 1309
		internal static ConfigEntry<float> LatexTransformAfter;

		// Token: 0x0400051E RID: 1310
		internal static ConfigEntry<float> LatexRemoveLoose;

		// Token: 0x0400051F RID: 1311
		internal static ConfigEntry<float> LatexLooseMinutes;

		// Token: 0x04000520 RID: 1312
		internal static ConfigEntry<float> LatexSeal;

		// Token: 0x04000521 RID: 1313
		internal static ConfigEntry<float> LatexHeal;

		// Token: 0x04000522 RID: 1314
		internal static ConfigEntry<float> LatexMind;

		// Token: 0x04000523 RID: 1315
		internal static ConfigEntry<float> LatexWarmth;

		// Token: 0x04000524 RID: 1316
		internal static ConfigEntry<float> LatexGlow;

		// Token: 0x04000525 RID: 1317
		internal static ConfigEntry<float> LatexNightVision;

		// Token: 0x04000526 RID: 1318
		internal static ConfigEntry<float> LatexReformSeconds;

		// Token: 0x04000527 RID: 1319
		internal static ConfigEntry<float> LatexTakeoverMinutes;

		// Token: 0x04000528 RID: 1320
		internal static ConfigEntry<float> PondPerLayer;

		// Token: 0x04000529 RID: 1321
		internal static ConfigEntry<float> PondFirstLayer;

		// Token: 0x0400052A RID: 1322
		internal static ConfigEntry<float> PondPull;

		// Token: 0x0400052B RID: 1323
		internal static ConfigEntry<float> PondEscape;

		// Token: 0x0400052C RID: 1324
		internal static ConfigEntry<float> PondEscapeUnder;

		// Token: 0x0400052D RID: 1325
		internal static ConfigEntry<float> PondUnderSeconds;

		// Token: 0x0400052E RID: 1326
		internal static ConfigEntry<float> PondLetGoHits;

		// Token: 0x0400052F RID: 1327
		internal static ConfigEntry<float> PondSatedSeconds;

		// Token: 0x04000530 RID: 1328
		internal static ConfigEntry<float> SlitsSand;

		// Token: 0x04000531 RID: 1329
		internal static ConfigEntry<float> SlitsEarly;

		// Token: 0x04000532 RID: 1330
		internal static ConfigEntry<float> SlitHealth;

		// Token: 0x04000533 RID: 1331
		internal static ConfigEntry<float> SlitReach;

		// Token: 0x04000534 RID: 1332
		internal static ConfigEntry<float> SlitDragSpeed;

		// Token: 0x04000535 RID: 1333
		internal static ConfigEntry<float> SlitEscape;

		// Token: 0x04000536 RID: 1334
		internal static ConfigEntry<float> SlitLetGoHits;

		// Token: 0x04000537 RID: 1335
		internal static ConfigEntry<float> SlitFloodChance;

		// Token: 0x04000538 RID: 1336
		internal static ConfigEntry<float> SlitFloodAfter;

		// Token: 0x04000539 RID: 1337
		internal static ConfigEntry<float> SlitFloodSeconds;

		// Token: 0x0400053A RID: 1338
		internal static ConfigEntry<float> SlitPullStrength;

		// Token: 0x0400053B RID: 1339
		internal static ConfigEntry<float> SlitBlastRange;

		// Token: 0x0400053C RID: 1340
		internal static ConfigEntry<float> SexWarmthRate;

		// Token: 0x0400053D RID: 1341
		internal static ConfigEntry<float> BlanketWarmth;

		// Token: 0x0400053E RID: 1342
		internal static ConfigEntry<float> ToyCrateChance;

		// Token: 0x0400053F RID: 1343
		internal static ConfigEntry<float> ToyMinLust;

		// Token: 0x04000540 RID: 1344
		internal static ConfigEntry<float> StartSuitColorChance;

		// Token: 0x04000541 RID: 1345
		internal static ConfigEntry<float> PotionLayerChance;

		// Token: 0x04000542 RID: 1346
		internal static ConfigEntry<float> PotionCorpseChance;

		// Token: 0x04000543 RID: 1347
		internal static ConfigEntry<float> PotionCrateChance;

		// Token: 0x04000544 RID: 1348
		internal static ConfigEntry<float> PotionStrength;

		// Token: 0x04000545 RID: 1349
		internal static ConfigEntry<bool> PotionInLoot;

		// Token: 0x04000546 RID: 1350
		internal static ConfigEntry<float> GasGeysersPerLayer;

		// Token: 0x04000547 RID: 1351
		internal static ConfigEntry<float> GasBiomeChance;

		// Token: 0x04000548 RID: 1352
		internal static ConfigEntry<float> GasBiomeRadius;

		// Token: 0x04000549 RID: 1353
		internal static ConfigEntry<float> GasGeysersPerBiome;

		// Token: 0x0400054A RID: 1354
		internal static ConfigEntry<float> GasTriggerRange;

		// Token: 0x0400054B RID: 1355
		internal static ConfigEntry<float> GasGeyserCooldown;

		// Token: 0x0400054C RID: 1356
		internal static ConfigEntry<float> GasStrength;

		// Token: 0x0400054D RID: 1357
		internal static ConfigEntry<float> GasFog;

		// Token: 0x0400054E RID: 1358
		internal static ConfigEntry<bool> DebugKeys;

		// Token: 0x0400054F RID: 1359
		internal static ConfigEntry<bool> Compulsion;

		// Token: 0x04000550 RID: 1360
		internal static ConfigEntry<KeyCode> ReliefKey;

		// Token: 0x04000551 RID: 1361
		internal static ConfigEntry<KeyCode> RemoveKey;

		// Token: 0x04000552 RID: 1362
		internal static ConfigEntry<float> ReliefEnemyRange;

		// Token: 0x04000553 RID: 1363
		internal static ConfigEntry<float> ReliefTraderRange;

		// Token: 0x04000554 RID: 1364
		internal static ConfigEntry<float> ReliefHurtSeconds;

		// Token: 0x04000555 RID: 1365
		internal static ConfigEntry<float> MoanVolume;

		// Token: 0x04000556 RID: 1366
		internal static ConfigEntry<float> SuitEnjoyMinutes;

		// Token: 0x04000557 RID: 1367
		internal static ConfigEntry<float> SuitGrowth;

		// Token: 0x04000558 RID: 1368
		internal static ConfigEntry<float> ParasitesPerLayer;

		// Token: 0x04000559 RID: 1369
		internal static ConfigEntry<float> RareSpawnChance;

		// Token: 0x0400055A RID: 1370
		internal static ConfigEntry<float> TakeoverChance;

		// Token: 0x0400055B RID: 1371
		internal static ConfigEntry<float> TakeoverMinutes;

		// Token: 0x0400055C RID: 1372
		internal static ConfigEntry<float> LatexComfortRate;

		// Token: 0x0400055D RID: 1373
		internal static ConfigEntry<float> FleshPerLayer;

		// Token: 0x0400055E RID: 1374
		internal static ConfigEntry<float> FleshSwallowSeconds;

		// Token: 0x0400055F RID: 1375
		internal static ConfigEntry<float> FleshSceneSeconds;

		// Token: 0x04000560 RID: 1376
		internal static ConfigEntry<float> FleshEscapeGrab;

		// Token: 0x04000561 RID: 1377
		internal static ConfigEntry<float> FleshEscapeInside;

		// Token: 0x04000562 RID: 1378
		internal static ConfigEntry<float> FleshWallHealth;

		// Token: 0x04000563 RID: 1379
		internal static ConfigEntry<float> SlimeWallHealth;

		// Token: 0x04000564 RID: 1380
		internal static ConfigEntry<float> GasVentHealth;

		// Token: 0x04000565 RID: 1381
		internal static ConfigEntry<float> TentacleHealth;

		// Token: 0x04000566 RID: 1382
		internal static ConfigEntry<float> WormHealth;

		// Token: 0x04000567 RID: 1383
		internal static ConfigEntry<float> WormNoiseGun;

		// Token: 0x04000568 RID: 1384
		internal static ConfigEntry<float> WormNoiseBlast;

		// Token: 0x04000569 RID: 1385
		internal static ConfigEntry<float> WormNoiseDig;

		// Token: 0x0400056A RID: 1386
		internal static ConfigEntry<float> WormNoiseFall;

		// Token: 0x0400056B RID: 1387
		internal static ConfigEntry<float> WormNoiseDrop;

		// Token: 0x0400056C RID: 1388
		internal static ConfigEntry<float> WormNoiseMoan;

		// Token: 0x0400056D RID: 1389
		internal static ConfigEntry<float> WormNoiseClimax;

		// Token: 0x0400056E RID: 1390
		internal static ConfigEntry<float> WormNoiseRun;

		// Token: 0x0400056F RID: 1391
		internal static ConfigEntry<float> WormNoiseFuse;

		// Token: 0x04000570 RID: 1392
		internal static ConfigEntry<float> WormNoiseMemory;

		// Token: 0x04000571 RID: 1393
		internal static ConfigEntry<float> WormRetreatDamage;

		// Token: 0x04000572 RID: 1394
		internal static ConfigEntry<float> WormFireScare;

		// Token: 0x04000573 RID: 1395
		internal static ConfigEntry<float> WormBlastDamage;

		// Token: 0x04000574 RID: 1396
		internal static ConfigEntry<KeyCode> WormDropKey;

		// Token: 0x04000575 RID: 1397
		internal static ConfigEntry<bool> RepairRecipes;

		// Token: 0x04000576 RID: 1398
		internal static ConfigEntry<float> SlimeWallFireScale;

		// Token: 0x04000577 RID: 1399
		internal static ConfigEntry<float> SecondChanceCorruption;

		// Token: 0x04000578 RID: 1400
		internal static ConfigEntry<float> SecondChanceSubmission;

		// Token: 0x04000579 RID: 1401
		internal static ConfigEntry<float> ShamelessCorruption;

		// Token: 0x0400057A RID: 1402
		internal static ConfigEntry<float> ShamelessLust;

		// Token: 0x0400057B RID: 1403
		internal static ConfigEntry<float> TraderWatchRep;

		// Token: 0x0400057C RID: 1404
		internal static ConfigEntry<float> FleshHealth;

		// Token: 0x0400057D RID: 1405
		internal static ConfigEntry<float> FleshStunSeconds;

		// Token: 0x0400057E RID: 1406
		internal static ConfigEntry<float> FireTorchHit;

		// Token: 0x0400057F RID: 1407
		internal static ConfigEntry<float> FireBurn;

		// Token: 0x04000580 RID: 1408
		internal static ConfigEntry<float> FireBlast;

		// Token: 0x04000581 RID: 1409
		internal static ConfigEntry<float> FireReach;

		// Token: 0x04000582 RID: 1410
		internal static ConfigEntry<float> FleshWallsPerLayer;

		// Token: 0x04000583 RID: 1411
		internal static ConfigEntry<float> SlimeWallsPerLayer;

		// Token: 0x04000584 RID: 1412
		internal static ConfigEntry<float> FleshWallAbsorbSeconds;

		// Token: 0x04000585 RID: 1413
		internal static ConfigEntry<float> FleshWallEscape;

		// Token: 0x04000586 RID: 1414
		internal static ConfigEntry<float> FleshWallArousal;

		// Token: 0x04000587 RID: 1415
		internal static ConfigEntry<float> FleshWallOrgasmAir;

		// Token: 0x04000588 RID: 1416
		internal static ConfigEntry<float> FleshWallSuffocation;

		// Token: 0x04000589 RID: 1417
		internal static ConfigEntry<int> FleshWallOrgasmsToDeath;

		// Token: 0x0400058A RID: 1418
		internal static ConfigEntry<float> FleshZonesPerLayer;

		// Token: 0x0400058B RID: 1419
		internal static ConfigEntry<float> FleshZoneRadius;

		// Token: 0x0400058C RID: 1420
		internal static ConfigEntry<float> FleshZoneTraps;

		// Token: 0x0400058D RID: 1421
		internal static ConfigEntry<float> FleshZoneSlow;

		// Token: 0x0400058E RID: 1422
		internal static ConfigEntry<bool> StartWithBodysuit;

		// Token: 0x0400058F RID: 1423
		internal static ConfigEntry<bool> SuitSteps;

		// Token: 0x04000590 RID: 1424
		internal static ConfigEntry<float> SuitStepVolume;

		// Token: 0x04000591 RID: 1425
		internal static ConfigEntry<float> NudeHabitRate;

		// Token: 0x04000592 RID: 1426
		internal static ConfigEntry<float> OutfitsPerLayer;

		// Token: 0x04000593 RID: 1427
		internal static ConfigEntry<float> OutfitCorpseChance;

		// Token: 0x04000594 RID: 1428
		internal static ConfigEntry<float> OutfitCrateChance;

		// Token: 0x04000595 RID: 1429
		internal static ConfigEntry<bool> OutfitsInLoot;

		// Token: 0x04000596 RID: 1430
		internal static ConfigEntry<float> FillDrain;

		// Token: 0x04000597 RID: 1431
		internal static ConfigEntry<float> PregnancyChance;

		// Token: 0x04000598 RID: 1432
		internal static ConfigEntry<float> PregnancyMinutes;

		// Token: 0x04000599 RID: 1433
		internal static ConfigEntry<float> BroodRootChance;

		// Token: 0x0400059A RID: 1434
		internal static ConfigEntry<float> TentaclesPerLayer;

		// Token: 0x0400059B RID: 1435
		internal static ConfigEntry<float> TentacleInsatiable;

		// Token: 0x0400059C RID: 1436
		internal static ConfigEntry<float> TentacleEscapeChance;

		// Token: 0x0400059D RID: 1437
		internal static ConfigEntry<float> TentacleSceneSeconds;

		// Token: 0x0400059E RID: 1438
		internal static ConfigEntry<float> ExhaustionPerOrgasm;

		// Token: 0x0400059F RID: 1439
		internal static ConfigEntry<float> ExhaustionDeathRate;

		// Token: 0x040005A0 RID: 1440
		internal static ConfigEntry<float> WormChance;

		// Token: 0x040005A1 RID: 1441
		internal static ConfigEntry<float> WormSenseRange;

		// Token: 0x040005A2 RID: 1442
		internal static ConfigEntry<float> WormSpeed;

		// Token: 0x040005A3 RID: 1443
		internal static ConfigEntry<float> WormTentacleChance;

		// Token: 0x040005A4 RID: 1444
		internal static ConfigEntry<float> WormInnerHealth;

		// Token: 0x040005A5 RID: 1445
		internal static ConfigEntry<float> WormEscapeChance;

		// Token: 0x040005A6 RID: 1446
		internal static ConfigEntry<float> WormAcid;

		// Token: 0x040005A7 RID: 1447
		internal static ConfigEntry<float> WormAirLoss;

		// Token: 0x040005A8 RID: 1448
		internal static ConfigEntry<float> WormSceneSeconds;

		// Token: 0x040005A9 RID: 1449
		internal static ConfigEntry<float> WormCloseSeconds;

		// Token: 0x040005AA RID: 1450
		internal static ConfigEntry<bool> GameOverPicEnabled;

		// Token: 0x040005AB RID: 1451
		internal static ConfigEntry<bool> TentaclePosed;

		// Token: 0x040005AC RID: 1452
		internal static ConfigEntry<float> SlimeWarmth;

		// Token: 0x040005AD RID: 1453
		internal static ConfigEntry<float> SlimeSeal;

		// Token: 0x040005AE RID: 1454
		internal static ConfigEntry<float> SlimeBondMinutes;

		// Token: 0x040005AF RID: 1455
		internal static ConfigEntry<float> SlimeInfectionMult;

		// Token: 0x040005B0 RID: 1456
		internal static ConfigEntry<float> HostTakeoverMinutes;

		// Token: 0x040005B1 RID: 1457
		internal static ConfigEntry<float> HostConversionSeconds;

		// Token: 0x040005B2 RID: 1458
		internal static ConfigEntry<float> HostSparedKnockoutSeconds;

		// Token: 0x040005B3 RID: 1459
		internal static ConfigEntry<float> ParasiteWarmth;

		// Token: 0x040005B4 RID: 1460
		internal static ConfigEntry<float> ParasiteSeal;

		// Token: 0x040005B5 RID: 1461
		internal static ConfigEntry<float> ParasiteInfectionMult;

		// Token: 0x040005B6 RID: 1462
		internal static ConfigEntry<float> TouchScale;

		// Token: 0x040005B7 RID: 1463
		internal static ConfigEntry<float> StabilizeMinutes;

		// Token: 0x040005B8 RID: 1464
		internal static ConfigEntry<int> TameEscapes;

		// Token: 0x040005B9 RID: 1465
		internal static ConfigEntry<int> StabilizeDoses;

		// Token: 0x040005BA RID: 1466
		internal static ConfigEntry<int> ParasiteProtectStage;

		// Token: 0x040005BB RID: 1467
		internal static ConfigEntry<float> WebZonesPerLayer;

		// Token: 0x040005BC RID: 1468
		internal static ConfigEntry<float> WebZoneRadius;

		// Token: 0x040005BD RID: 1469
		internal static ConfigEntry<float> WebSheetsPerBiome;

		// Token: 0x040005BE RID: 1470
		internal static ConfigEntry<float> WebSheetSize;

		// Token: 0x040005BF RID: 1471
		internal static ConfigEntry<float> WebSlow;

		// Token: 0x040005C0 RID: 1472
		internal static ConfigEntry<float> WebEscapeChance;

		// Token: 0x040005C1 RID: 1473
		internal static ConfigEntry<float> WebTangleSpeed;

		// Token: 0x040005C2 RID: 1474
		internal static ConfigEntry<float> WebLoosen;

		// Token: 0x040005C3 RID: 1475
		internal static ConfigEntry<float> CocoonEscapeChance;

		// Token: 0x040005C4 RID: 1476
		internal static ConfigEntry<float> CocoonWear;

		// Token: 0x040005C5 RID: 1477
		internal static ConfigEntry<float> CocoonAirSeconds;

		// Token: 0x040005C6 RID: 1478
		internal static ConfigEntry<float> CocoonSuffocation;

		// Token: 0x040005C7 RID: 1479
		internal static ConfigEntry<float> DarkBogChance;

		// Token: 0x040005C8 RID: 1480
		internal static ConfigEntry<bool> MoodlesCollapse;

		// Token: 0x040005C9 RID: 1481
		internal static ConfigEntry<bool> MoodlesSuitsMain;

		// Token: 0x040005CA RID: 1482
		internal static ConfigEntry<string> MoodlesHidden;

		// Token: 0x040005CB RID: 1483
		internal static ConfigEntry<float> SlimeBogsPerLayer;

		// Token: 0x040005CC RID: 1484
		internal static ConfigEntry<float> SlimePoolChance;

		// Token: 0x040005CD RID: 1485
		internal static ConfigEntry<float> SlimeSuitChance;

		// Token: 0x040005CE RID: 1486
		internal static ConfigEntry<float> SlimeEscapeChance;

		// Token: 0x040005CF RID: 1487
		internal static ConfigEntry<float> SlimeSuffocation;

		// Token: 0x040005D0 RID: 1488
		internal static ConfigEntry<float> SlimeSwimSpeed;

		// Token: 0x040005D1 RID: 1489
		internal static bool FreshRun;

		// Token: 0x040005D2 RID: 1490
		internal static State S = new State();

		// Token: 0x040005D3 RID: 1491
		private Body trackedBody;

		// Token: 0x040005D4 RID: 1492
		private float saveCheck;

		// Token: 0x040005D5 RID: 1493
		private float hornyTimer = 90f;

		// Token: 0x040005D6 RID: 1494
		private int wardrobeIndex = -1;

		// Token: 0x040005D7 RID: 1495
		private int latexKind = -1;

		// Token: 0x040005D8 RID: 1496
		private static readonly string[] LatexKinds = new string[] { "crawler", "ceiling", "puddle" };

		// Token: 0x040005D9 RID: 1497
		private static float warmSaid;
	}
}
