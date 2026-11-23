using System.Collections.Generic;
using System.Linq;

public class RunSettings
{
	public static List<RunSetting> settingTypes = new List<RunSetting>
	{
		new RunSettingBool("unchipped")
		{
			divider = "runwide"
		},
		new RunSettingBool("endless"),
		new RunSettingFloat("baselootdensity")
		{
			limits = new RangeF(0f, 2f),
			postfix = "x",
			divider = "layer"
		},
		new RunSettingFloat("lootmultiplier")
		{
			limits = new RangeF(0f, 1f),
			postfix = "x"
		},
		new RunSettingFloat("basetrapdensity")
		{
			limits = new RangeF(0f, 3f),
			postfix = "x"
		},
		new RunSettingFloat("trapincrease")
		{
			limits = new RangeF(0f, 1f),
			postfix = "x"
		},
		new RunSettingDropdown("ambientlight", new string[3] { "none", "enough", "bright" }),
		new RunSettingFloat("timelimit")
		{
			limits = new RangeF(5f, 300f),
			wholeNum = true,
			postfix = "m"
		},
		new RunSettingFloat("oreamount")
		{
			limits = new RangeF(0f, 3f),
			postfix = "x"
		},
		new RunSettingFloat("layermodifierchance")
		{
			limits = new RangeF(0f, 100f),
			postfix = "%"
		},
		new RunSettingFloat("timebetweenearthquakes")
		{
			limits = new RangeF(0.1f, 5f),
			postfix = "x"
		},
		new RunSettingFloat("temperatureoffset")
		{
			limits = new RangeF(-80f, 50f),
			postfix = "'C",
			wholeNum = true
		},
		new RunSettingDropdown("startingsupplies", new string[3] { "none", "light", "full" })
		{
			divider = "player"
		},
		new RunSettingFloat("xpgain")
		{
			limits = new RangeF(0f, 5f),
			postfix = "x"
		},
		new RunSettingFloat("metabolismrate")
		{
			limits = new RangeF(0.1f, 2f),
			postfix = "x"
		},
		new RunSettingFloat("healingrate")
		{
			limits = new RangeF(0.1f, 3f),
			postfix = "x"
		},
		new RunSettingFloat("fracturepain")
		{
			limits = new RangeF(0f, 1.5f),
			postfix = "x"
		},
		new RunSettingFloat("bleedrate")
		{
			limits = new RangeF(0.2f, 2f),
			postfix = "x"
		},
		new RunSettingFloat("infectionspeed")
		{
			limits = new RangeF(0.1f, 3f),
			postfix = "x"
		},
		new RunSettingFloat("infectionchance")
		{
			limits = new RangeF(0f, 3f),
			postfix = "x"
		},
		new RunSettingFloat("fibrillationrate")
		{
			limits = new RangeF(0.1f, 3f),
			postfix = "x"
		},
		new RunSettingFloat("moodnormalizationrate")
		{
			limits = new RangeF(0f, 3f),
			postfix = "x"
		},
		new RunSettingFloat("bonuslimbarmor")
		{
			limits = new RangeF(0f, 4f)
		},
		new RunSettingFloat("staminaregen")
		{
			limits = new RangeF(0.5f, 2f),
			postfix = "x"
		},
		new RunSettingFloat("attackdamage")
		{
			limits = new RangeF(0.2f, 3f),
			postfix = "x"
		},
		new RunSettingFloat("minigamehandshake")
		{
			limits = new RangeF(0f, 2f),
			postfix = "x"
		},
		new RunSettingFloat("sleepcyclespeed")
		{
			limits = new RangeF(0.1f, 2f),
			postfix = "x"
		},
		new RunSettingFloat("encumbrancecap")
		{
			limits = new RangeF(0.5f, 3f),
			postfix = "x"
		},
		new RunSettingFloat("insulationmultiplier")
		{
			limits = new RangeF(0.5f, 2f),
			postfix = "x"
		},
		new RunSettingBool("strokes"),
		new RunSettingBool("braindamagefx"),
		new RunSettingBool("forcesleep"),
		new RunSettingBool("lowmoodevents"),
		new RunSettingBool("liquidpushing"),
		new RunSettingBool("disfigurement"),
		new RunSettingBool("crushdepth"),
		new RunSettingBool("nosleeprestrictions"),
		new RunSettingBool("infinitelaststand"),
		new RunSettingFloat("traderchance")
		{
			limits = new RangeF(0f, 100f),
			postfix = "%",
			divider = "trader"
		},
		new RunSettingFloat("traderitemamount")
		{
			limits = new RangeF(0.2f, 3f),
			postfix = "x"
		},
		new RunSettingFloat("traderrepoffset")
		{
			limits = new RangeF(-100f, 100f)
		},
		new RunSettingFloat("enemystrength")
		{
			limits = new RangeF(0.2f, 2f),
			postfix = "x",
			divider = "other"
		},
		new RunSettingFloat("itemdecayrate")
		{
			limits = new RangeF(0f, 3f),
			postfix = "x"
		},
		new RunSettingFloat("lockpickprecision")
		{
			limits = new RangeF(0.5f, 3f),
			postfix = "x"
		},
		new RunSettingBool("permanentaddiction")
		{
			divider = "challenges"
		},
		new RunSettingBool("mindwiped"),
		new RunSettingBool("zeroskills"),
		new RunSettingBool("blind"),
		new RunSettingBool("amputee"),
		new RunSettingBool("immortalsnail")
	};

	public static List<RunSettingsPreset> presets = new List<RunSettingsPreset>
	{
		new RunSettingsPreset
		{
			presetName = "normal",
			presetValues = new Dictionary<string, object>
			{
				{ "unchipped", false },
				{ "endless", false },
				{ "baselootdensity", 1f },
				{ "lootmultiplier", 1f },
				{ "basetrapdensity", 1f },
				{ "trapincrease", 0f },
				{ "ambientlight", 0 },
				{ "timelimit", 60f },
				{ "startingsupplies", 1 },
				{ "enemystrength", 1f },
				{ "xpgain", 1f },
				{ "metabolismrate", 1f },
				{ "healingrate", 1f },
				{ "fracturepain", 1f },
				{ "bleedrate", 1f },
				{ "infectionspeed", 1f },
				{ "infectionchance", 1f },
				{ "fibrillationrate", 1f },
				{ "moodnormalizationrate", 1f },
				{ "bonuslimbarmor", 0f },
				{ "staminaregen", 1f },
				{ "attackdamage", 1f },
				{ "minigamehandshake", 1f },
				{ "sleepcyclespeed", 1f },
				{ "encumbrancecap", 1f },
				{ "insulationmultiplier", 1f },
				{ "strokes", true },
				{ "braindamagefx", true },
				{ "forcesleep", true },
				{ "lowmoodevents", true },
				{ "liquidpushing", true },
				{ "disfigurement", true },
				{ "nosleeprestrictions", false },
				{ "crushdepth", true },
				{ "infinitelaststand", false },
				{ "traderchance", 33f },
				{ "traderitemamount", 1f },
				{ "traderrepoffset", 0f },
				{ "itemdecayrate", 1f },
				{ "lockpickprecision", 1f },
				{ "layermodifierchance", 40f },
				{ "timebetweenearthquakes", 1f },
				{ "temperatureoffset", 0f },
				{ "oreamount", 1f },
				{ "permanentaddiction", false },
				{ "mindwiped", false },
				{ "zeroskills", false },
				{ "blind", false },
				{ "amputee", false },
				{ "immortalsnail", false }
			}
		},
		new RunSettingsPreset
		{
			presetName = "relaxed",
			presetValues = new Dictionary<string, object>
			{
				{ "baselootdensity", 1.15f },
				{ "basetrapdensity", 0.8f },
				{ "ambientlight", 1 },
				{ "timelimit", 120f },
				{ "startingsupplies", 2 },
				{ "enemystrength", 0.75f },
				{ "xpgain", 1.15f },
				{ "metabolismrate", 0.7f },
				{ "healingrate", 1.2f },
				{ "fracturepain", 0.67f },
				{ "bleedrate", 0.75f },
				{ "infectionspeed", 0.7f },
				{ "infectionchance", 0.75f },
				{ "minigamehandshake", 0.66f },
				{ "fibrillationrate", 0.8f },
				{ "moodnormalizationrate", 1.25f },
				{ "staminaregen", 1.1f },
				{ "insulationmultiplier", 1.15f },
				{ "forcesleep", false },
				{ "crushdepth", false },
				{ "traderrepoffset", 15f },
				{ "layermodifierchance", 20f },
				{ "itemdecayrate", 0.75f },
				{ "lockpickprecision", 1.5f },
				{ "timebetweenearthquakes", 1.5f }
			}
		},
		new RunSettingsPreset
		{
			presetName = "paradise",
			presetValues = new Dictionary<string, object>
			{
				{ "baselootdensity", 1.5f },
				{ "basetrapdensity", 0.35f },
				{ "ambientlight", 2 },
				{ "timelimit", 180f },
				{ "startingsupplies", 2 },
				{ "enemystrength", 0.5f },
				{ "xpgain", 1.25f },
				{ "metabolismrate", 0.7f },
				{ "healingrate", 1.5f },
				{ "fracturepain", 0.4f },
				{ "bleedrate", 0.7f },
				{ "infectionspeed", 0.5f },
				{ "infectionchance", 0.75f },
				{ "fibrillationrate", 0.6f },
				{ "moodnormalizationrate", 1.25f },
				{ "staminaregen", 1.2f },
				{ "minigamehandshake", 0.2f },
				{ "insulationmultiplier", 1.3f },
				{ "forcesleep", false },
				{ "liquidpushing", false },
				{ "crushdepth", false },
				{ "infinitelaststand", true },
				{ "attackdamage", 1.5f },
				{ "encumbrancecap", 1.5f },
				{ "traderrepoffset", 25f },
				{ "traderchance", 50f },
				{ "traderitemamount", 1.25f },
				{ "itemdecayrate", 0.5f },
				{ "layermodifierchance", 10f },
				{ "lockpickprecision", 2.5f },
				{ "bonuslimbarmor", 0.25f },
				{ "timebetweenearthquakes", 2f }
			}
		},
		new RunSettingsPreset
		{
			presetName = "desolate",
			presetValues = new Dictionary<string, object>
			{
				{ "baselootdensity", 0.4f },
				{ "basetrapdensity", 1.5f },
				{ "startingsupplies", 0 },
				{ "fracturepain", 1.25f },
				{ "moodnormalizationrate", 0.5f },
				{ "traderrepoffset", -10f },
				{ "traderchance", 10f },
				{ "minigamehandshake", 2f },
				{ "traderitemamount", 3f },
				{ "layermodifierchance", 100f },
				{ "temperatureoffset", -15f },
				{ "oreamount", 0.5f },
				{ "timebetweenearthquakes", 0.8f }
			}
		},
		new RunSettingsPreset
		{
			presetName = "unchipped",
			presetValues = new Dictionary<string, object>
			{
				{ "unchipped", true },
				{ "timelimit", 300f },
				{ "startingsupplies", 0 }
			}
		},
		new RunSettingsPreset
		{
			presetName = "endless",
			presetValues = new Dictionary<string, object>
			{
				{ "endless", true },
				{ "lootmultiplier", 0.95f },
				{ "trapincrease", 0.06f }
			}
		}
	};

	public static RunSetting GetSetting(string name)
	{
		return settingTypes.First((RunSetting x) => x.name == name);
	}

	public static RunSettingsPreset GetPreset(string presetName)
	{
		return presets.First((RunSettingsPreset x) => x.presetName == presetName);
	}
}
