using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Settings
{
	public static List<Setting> settings;

	private static bool initialized;

	public static List<Setting> GetAllSettings()
	{
		EnsureLoaded();
		return settings;
	}

	public static List<Setting> DefaultSettings()
	{
		List<Setting> list = new List<Setting>();
		list.Add(new SettingInt
		{
			name = "reswidth",
			value = Screen.resolutions[Screen.resolutions.Length - 1].width,
			min = 800,
			max = 7680,
			apply = () =>
			{
				SettingInt setting = GetSetting<SettingInt>("reswidth");
				SettingInt setting2 = GetSetting<SettingInt>("resheight");
				Screen.SetResolution(fullscreenMode: GetSetting<SettingBool>("fullscreen").value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, width: setting.value, height: setting2.value);
			},
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingInt
		{
			name = "resheight",
			value = Screen.resolutions[Screen.resolutions.Length - 1].height,
			min = 600,
			max = 2160,
			apply = () =>
			{
				SettingInt setting = GetSetting<SettingInt>("reswidth");
				SettingInt setting2 = GetSetting<SettingInt>("resheight");
				Screen.SetResolution(fullscreenMode: GetSetting<SettingBool>("fullscreen").value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, width: setting.value, height: setting2.value);
			},
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingBool
		{
			name = "fullscreen",
			value = true,
			apply = () =>
			{
				SettingInt setting = GetSetting<SettingInt>("reswidth");
				SettingInt setting2 = GetSetting<SettingInt>("resheight");
				Screen.SetResolution(fullscreenMode: GetSetting<SettingBool>("fullscreen").value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, width: setting.value, height: setting2.value);
			},
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingBool
		{
			name = "vsync",
			value = true,
			apply = () =>
			{
				QualitySettings.vSyncCount = (GetSetting<SettingBool>("vsync").value ? 1 : 0);
			},
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingInt
		{
			name = "framerate",
			value = 0,
			min = 0,
			max = 240,
			apply = () =>
			{
				SettingInt setting = GetSetting<SettingInt>("framerate");
				Application.targetFrameRate = ((setting.value == 0) ? (-1) : Math.Max(10, setting.value));
			},
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingBool
		{
			name = "pixelate",
			value = true,
			apply = () =>
			{
				SettingBool setting = GetSetting<SettingBool>("pixelate");
				if ((bool)PlayerCamera.main)
				{
					PlayerCamera.main.GetComponent<PixelPerfectCamera>().enabled = setting.value;
				}
			},
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingBool
		{
			name = "darkerbackground",
			value = false,
			apply = () =>
			{
				if ((bool)WorldGeneration.world)
				{
					WorldGeneration.world.UpdateAllBackgroundColors();
				}
			},
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingBool
		{
			name = "fastcamera",
			value = false,
			apply = () =>
			{
				PlayerCamera.fastCam = GetSetting<SettingBool>("fastcamera").value;
			},
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingBool
		{
			name = "bloom",
			value = true,
			apply = () =>
			{
				if ((bool)WorldGeneration.world)
				{
					WorldGeneration.world.UpdateBiomePostProcess();
				}
			},
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingFloat
		{
			name = "filmgrain",
			value = 1f,
			max = 1f,
			min = 0f,
			apply = () =>
			{
				if ((bool)WorldGeneration.world)
				{
					WorldGeneration.world.UpdateBiomePostProcess();
				}
			},
			formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingFloat
		{
			name = "brightness",
			value = 0f,
			max = 1f,
			min = 0f,
			apply = () =>
			{
				PlayerCamera.brightness = GetSetting<SettingFloat>("brightness").value;
			},
			formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingInt
		{
			name = "lineofsightcount",
			value = 512,
			min = 128,
			max = 1024,
			apply = () =>
			{
				PlayerCamera.lineOfSightRes = GetSetting<SettingInt>("lineofsightcount").value;
			},
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingFloat
		{
			name = "screenshake",
			value = 1f,
			max = 1f,
			min = 0f,
			apply = () =>
			{
				PlayerCamera.screenShakeMult = GetSetting<SettingFloat>("screenshake").value;
			},
			formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
			category = Setting.SettingCategory.Video
		});
		list.Add(new SettingBool
		{
			name = "censorpain",
			value = false,
			apply = () =>
			{
				Body.censorPain = GetSetting<SettingBool>("censorpain").value;
			},
			category = Setting.SettingCategory.Game
		});
		list.Add(new SettingBool
		{
			name = "censormood",
			value = false,
			apply = () =>
			{
				Body.censorMood = GetSetting<SettingBool>("censormood").value;
			},
			category = Setting.SettingCategory.Game
		});
		list.Add(new SettingBool
		{
			name = "alwaysexpand",
			value = false,
			apply = () =>
			{
				PlayerCamera.alwaysExpandDescriptions = GetSetting<SettingBool>("alwaysexpand").value;
			},
			category = Setting.SettingCategory.Game
		});
		list.Add(new SettingBool
		{
			name = "showhiddenmoodles",
			value = false,
			apply = () =>
			{
				MoodleManager.alwaysShowHidden = GetSetting<SettingBool>("showhiddenmoodles").value;
			},
			category = Setting.SettingCategory.Game
		});
		list.Add(new SettingBool
		{
			name = "itemfloating",
			value = true,
			apply = () =>
			{
				Item.itemFloating = GetSetting<SettingBool>("itemfloating").value;
			},
			category = Setting.SettingCategory.Game
		});
		list.Add(new SettingBool
		{
			name = "clearerrecipes",
			value = false,
			apply = () =>
			{
				PlayerCamera.clearerRecipes = GetSetting<SettingBool>("clearerrecipes").value;
			},
			category = Setting.SettingCategory.Game
		});
		list.Add(new SettingFloat
		{
			name = "inventoryuseleniency",
			value = 0.95f,
			max = 0.99f,
			min = 0.1f,
			apply = () =>
			{
				PlayerCamera.inventoryUseLeniency = GetSetting<SettingFloat>("inventoryuseleniency").value;
			},
			formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
			category = Setting.SettingCategory.Game
		});
		list.Add(new SettingBool
		{
			name = "skipintro",
			value = false,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Game
		});
		list.Add(new SettingBool
		{
			name = "rpc",
			value = true,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Game
		});
		list.Add(new SettingBool
		{
			name = "hidemenuprogression",
			value = false,
			apply = () =>
			{
				if ((bool)PreRunScript.instance)
				{
					PreRunScript.instance.UpdateEndImages();
				}
			},
			category = Setting.SettingCategory.Game
		});
		list.Add(new SettingDropdown
		{
			name = "units",
			value = 0,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Game,
			choices = new string[3] { "metric", "imperial", "burger" }
		});
		list.Add(new SettingFloat
		{
			name = "mastervolume",
			value = 1f,
			max = 1f,
			min = 0f,
			apply = () =>
			{
				AudioListener.volume = GetSetting<SettingFloat>("mastervolume").value;
			},
			formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
			category = Setting.SettingCategory.Audio
		});
		list.Add(new SettingFloat
		{
			name = "musicvolume",
			value = 0.45f,
			max = 1f,
			min = 0f,
			apply = () =>
			{
				SettingFloat setting = GetSetting<SettingFloat>("musicvolume");
				if ((bool)MusicManager.main)
				{
					MusicManager.main.origVolume = setting.value;
				}
				if ((bool)PreRunScript.instance)
				{
					PreRunScript.instance.menuSource.volume = Get<SettingFloat>("musicvolume").value * 0.6666f;
				}
			},
			formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
			category = Setting.SettingCategory.Audio
		});
		list.Add(new SettingFloat
		{
			name = "dronevolume",
			value = 1f,
			max = 1f,
			min = 0f,
			apply = () =>
			{
				SettingFloat setting = GetSetting<SettingFloat>("dronevolume");
				if ((bool)MusicManager.main)
				{
					MusicManager.main.droneVolume = setting.value;
				}
			},
			formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
			category = Setting.SettingCategory.Audio
		});
		list.Add(new SettingFloat
		{
			name = "trackinterval",
			value = 170f,
			max = 1200f,
			min = 10f,
			apply = () =>
			{
				SettingFloat setting = GetSetting<SettingFloat>("trackinterval");
				if ((bool)MusicManager.main)
				{
					MusicManager.main.trackInterval = setting.value;
				}
			},
			formatValue = (float v) => Mathf.RoundToInt(v) + "s",
			category = Setting.SettingCategory.Audio
		});
		list.Add(new SettingBool
		{
			name = "sadnessdistortion",
			value = true,
			apply = () =>
			{
				SettingBool setting = GetSetting<SettingBool>("sadnessdistortion");
				if ((bool)MusicManager.main)
				{
					MusicManager.main.sadnessDistortion = setting.value;
				}
			},
			category = Setting.SettingCategory.Audio
		});
		list.Add(new SettingBool
		{
			name = "autojump",
			value = false,
			apply = () =>
			{
				PlayerCamera.autoJump = GetSetting<SettingBool>("autojump").value;
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingBool
		{
			name = "togglelabels",
			value = false,
			apply = () =>
			{
				AltHoverScript.toggleActivated = GetSetting<SettingBool>("togglelabels").value;
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "jump",
			value = KeyCode.Space,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "left",
			value = KeyCode.A,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "right",
			value = KeyCode.D,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "down",
			value = KeyCode.S,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "up",
			value = KeyCode.W,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "attack",
			value = KeyCode.Mouse0,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "iteminteract",
			value = KeyCode.Mouse1,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "throw",
			value = KeyCode.T,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "switchhands",
			value = KeyCode.Q,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "ragdoll",
			value = KeyCode.X,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "woundview",
			value = KeyCode.R,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "craft",
			value = KeyCode.C,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "favourite",
			value = KeyCode.F,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "customizeitem",
			value = KeyCode.N,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "toggleinventory",
			value = KeyCode.Tab,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "speed1",
			value = KeyCode.Alpha1,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "speed2",
			value = KeyCode.Alpha2,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "speed3",
			value = KeyCode.Alpha3,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "expanddesc",
			value = KeyCode.LeftShift,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "altview",
			value = KeyCode.LeftControl,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "bark",
			value = KeyCode.B,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "console",
			value = KeyCode.BackQuote,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "pause",
			value = KeyCode.Escape,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "marksmanrack",
			value = KeyCode.E,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		list.Add(new SettingKeybind
		{
			name = "marksmanunload",
			value = KeyCode.G,
			apply = () =>
			{
			},
			category = Setting.SettingCategory.Input
		});
		return list;
	}

	public static void EnsureLoaded()
	{
		if (!initialized)
		{
			LoadSettings();
		}
	}

	private static T GetSetting<T>(string name) where T : Setting
	{
		return settings.Find((Setting s) => s.name == name) as T;
	}

	public static T Get<T>(string name) where T : Setting
	{
		EnsureLoaded();
		Setting setting = settings.Find((Setting s) => s.name == name);
		if (setting == null)
		{
			Debug.LogError("Setting '" + name + "' not found.");
			return null;
		}
		if (!(setting is T result))
		{
			Debug.LogError("Setting '" + name + "' is of type " + setting.GetType().Name + ", not " + typeof(T).Name);
			return null;
		}
		return result;
	}

	public static void LoadSettings()
	{
		string path = Application.persistentDataPath + "/settings.json";
		settings = DefaultSettings();
		if (!File.Exists(path))
		{
			SaveSettings();
			return;
		}
		try
		{
			foreach (SettingSaveData saved in JsonConvert.DeserializeObject<List<SettingSaveData>>(File.ReadAllText(path)))
			{
				Setting setting = settings.Find((Setting s) => s.name == saved.name);
				if (setting != null)
				{
					if (setting is SettingFloat)
					{
						setting.SetValue(Convert.ToSingle(saved.value));
					}
					else if (setting is SettingInt)
					{
						setting.SetValue(Convert.ToInt32(saved.value));
					}
					else if (setting is SettingDropdown)
					{
						setting.SetValue(Convert.ToInt32(saved.value));
					}
					else if (setting is SettingBool)
					{
						setting.SetValue(Convert.ToBoolean(saved.value));
					}
					else if (setting is SettingKeybind)
					{
						setting.SetValue(Convert.ToInt32(saved.value));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to load settings! " + ex);
		}
		initialized = true;
		ApplyAll();
	}

	public static void ApplyAll()
	{
		foreach (Setting setting in settings)
		{
			setting.Apply();
		}
	}

	public static void SaveSettings()
	{
		List<SettingSaveData> list = new List<SettingSaveData>();
		foreach (Setting setting in settings)
		{
			list.Add(new SettingSaveData
			{
				name = setting.name,
				value = setting.GetValue()
			});
		}
		string contents = JsonConvert.SerializeObject(list, Formatting.Indented);
		File.WriteAllText(Application.persistentDataPath + "/settings.json", contents);
	}
}
