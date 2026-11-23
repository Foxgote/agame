using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000B1 RID: 177
	internal static class LewdSettings
	{
		// Token: 0x06000406 RID: 1030 RVA: 0x00041D24 File Offset: 0x0003FF24
		public static void Bind(ConfigFile c, ConfigEntry<string> language)
		{
			LewdSettings.Language = language;
			LewdSettings.Enabled = c.Bind<bool>("General", "Enabled", true, "LEWD MOD: the base of the mod — her statuses (arousal, lust, heat, filled, naked...), touching herself, her thoughts aloud, and the clothes on the layers, by corpses and in crates. Creatures and traps have their own switches.");
			LewdSettings.Difficulty = c.Bind<string>("General", "Difficulty", "normal", "soft / normal / hard: traps and creatures per layer (x0.6 / x1 / x1.4), chances to break free (x1.5 / x1 / x0.7), how fast the deadly scenes kill (x0.6 / x1 / x1.3).");
			LewdSettings.DeadlyScenes = c.Bind<bool>("General", "DeadlyScenes", true, "Whether a scene can kill her. Off: whatever has her lets her go once her breath or her body is about to give out; raiders never shoot or hit her (they only grab her) and never kill her; the hosts' slime slides back off her face instead of turning her.");
			LewdSettings.Traps = c.Bind<bool>("Features", "Traps", true, "Living flesh traps and biomes, ceiling tentacles, webs, slime bogs.");
			LewdSettings.Worms = c.Bind<bool>("Features", "Worms", true, "The giant worm.");
			LewdSettings.SlimeCreatures = c.Bind<bool>("Features", "SlimeCreatures", true, "The slime hosts of Scav Armory (former survivors covered in living slime).");
			LewdSettings.ParasiteSuits = c.Bind<bool>("Features", "ParasiteSuits", true, "Living cloth parasites, symbiotes and the slime suit (from the slime bogs and the slime hosts).");
			LewdSettings.Enabled.SettingChanged += delegate(object s, EventArgs e)
			{
				if (!LewdSettings.Enabled.Value)
				{
					Plugin.BasicsOff();
				}
			};
			LewdSettings.Traps.SettingChanged += delegate(object s, EventArgs e)
			{
				if (!LewdSettings.Traps.Value)
				{
					LewdSettings.Clear(delegate(Body b)
					{
						Flesh.ClearAll(b);
						FleshZones.Clear();
						Tentacles.ClearAll(b);
						Webs.Clear(b);
						Walls.Clear(b);
						Pits.Clear(b);
						LustGas.Clear();
					});
				}
			};
			LewdSettings.Worms.SettingChanged += delegate(object s, EventArgs e)
			{
				if (!LewdSettings.Worms.Value)
				{
					Action<Body> action;
					if ((action = LewdSettings.<>O.<0>__ClearAll) == null)
					{
						action = (LewdSettings.<>O.<0>__ClearAll = new Action<Body>(Worm.ClearAll));
					}
					LewdSettings.Clear(action);
				}
			};
			LewdSettings.SlimeCreatures.SettingChanged += delegate(object s, EventArgs e)
			{
				LewdSettings.SetArmory("HostsEnabled", LewdSettings.SlimeCreatures.Value);
			};
			LewdSettings.ParasiteSuits.SettingChanged += delegate(object s, EventArgs e)
			{
				if (!LewdSettings.ParasiteSuits.Value)
				{
					LewdSettings.Clear(delegate(Body b)
					{
						Parasites.ClearAll();
						Suit.Strip(b);
					});
				}
			};
			LewdSettings.Language.SettingChanged += delegate(object s, EventArgs e)
			{
				Relief.ResetLines();
			};
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x00041EF0 File Offset: 0x000400F0
		private static void Clear(Action<Body> what)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body)
			{
				what(body);
			}
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00041F28 File Offset: 0x00040128
		private static ConfigEntry<bool> Armory(string field)
		{
			if (!Chainloader.PluginInfos.ContainsKey("jens.casualtiesunknown.armory"))
			{
				return null;
			}
			Type type = AccessTools.TypeByName("ScavArmory.Plugin");
			if (!(type == null))
			{
				FieldInfo fieldInfo = AccessTools.Field(type, field);
				return ((fieldInfo != null) ? fieldInfo.GetValue(null) : null) as ConfigEntry<bool>;
			}
			return null;
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x00041F78 File Offset: 0x00040178
		private static void SetArmory(string field, bool on)
		{
			ConfigEntry<bool> configEntry = LewdSettings.Armory(field);
			if (configEntry != null && configEntry.Value != on)
			{
				configEntry.Value = on;
			}
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00041F9F File Offset: 0x0004019F
		private static IEnumerable<ConfigEntry<bool>> Parts()
		{
			yield return LewdSettings.Traps;
			yield return LewdSettings.Worms;
			yield return LewdSettings.SlimeCreatures;
			yield return LewdSettings.ParasiteSuits;
			yield return LewdSettings.Armory("ScavEnabled");
			yield return LewdSettings.Armory("GunsEnabled");
			yield return Devices.Enabled;
			yield return InvasionPopup.Enabled;
			yield return LewdSettings.DeadlyScenes;
			yield return Plugin.HideCrouchGuide;
			yield return Plugin.DebugKeys;
			yield break;
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00041FA8 File Offset: 0x000401A8
		public static List<Setting> Entries()
		{
			List<Setting> list = new List<Setting>
			{
				new LewdSettings.AboutButton
				{
					name = "lewd_about",
					category = 5,
					value = false
				},
				new LewdSettings.MasterToggle
				{
					name = "lewd_enabled",
					category = 5,
					entry = LewdSettings.Enabled,
					value = LewdSettings.Enabled.Value
				},
				new LewdSettings.DiffPick
				{
					name = "lewd_difficulty",
					category = 5,
					choices = Diff.Choices,
					value = Diff.Level
				},
				new LewdSettings.Toggle
				{
					name = "lewd_deadly",
					category = 5,
					entry = LewdSettings.DeadlyScenes,
					value = LewdSettings.DeadlyScenes.Value
				},
				new LewdSettings.Toggle
				{
					name = "lewd_traps",
					category = 5,
					entry = LewdSettings.Traps,
					value = LewdSettings.Traps.Value
				},
				new LewdSettings.Toggle
				{
					name = "lewd_worms",
					category = 5,
					entry = LewdSettings.Worms,
					value = LewdSettings.Worms.Value
				},
				new LewdSettings.Toggle
				{
					name = "lewd_slime",
					category = 5,
					entry = LewdSettings.SlimeCreatures,
					value = LewdSettings.SlimeCreatures.Value
				},
				new LewdSettings.Toggle
				{
					name = "lewd_parasites",
					category = 5,
					entry = LewdSettings.ParasiteSuits,
					value = LewdSettings.ParasiteSuits.Value
				}
			};
			if (InvasionPopup.Enabled != null)
			{
				list.Add(new LewdSettings.Toggle
				{
					name = "lewd_invasion",
					category = 5,
					entry = InvasionPopup.Enabled,
					value = InvasionPopup.Enabled.Value
				});
			}
			if (Plugin.HideCrouchGuide != null)
			{
				list.Add(new LewdSettings.Toggle
				{
					name = "lewd_crouchguide",
					category = 5,
					entry = Plugin.HideCrouchGuide,
					value = Plugin.HideCrouchGuide.Value
				});
			}
			ConfigEntry<bool> configEntry = LewdSettings.Armory("ScavEnabled");
			if (configEntry != null)
			{
				list.Add(new LewdSettings.Toggle
				{
					name = "lewd_raiders",
					category = 5,
					entry = configEntry,
					value = configEntry.Value
				});
			}
			if (Devices.Enabled != null)
			{
				list.Add(new LewdSettings.Toggle
				{
					name = "lewd_devices",
					category = 5,
					entry = Devices.Enabled,
					value = Devices.Enabled.Value
				});
			}
			ConfigEntry<bool> configEntry2 = LewdSettings.Armory("GunsEnabled");
			if (configEntry2 != null)
			{
				list.Add(new LewdSettings.Toggle
				{
					name = "lewd_guns",
					category = 5,
					entry = configEntry2,
					value = configEntry2.Value
				});
			}
			ConfigEntry<bool> configEntry3 = LewdSettings.Armory("AmmoCounter");
			if (configEntry3 != null)
			{
				list.Add(new LewdSettings.Toggle
				{
					name = "lewd_ammocounter",
					category = 5,
					entry = configEntry3,
					value = configEntry3.Value
				});
			}
			if (Plugin.MoanVolume != null)
			{
				list.Add(new LewdSettings.Volume
				{
					name = "lewd_moanvolume",
					category = 5,
					entry = Plugin.MoanVolume,
					value = Plugin.MoanVolume.Value
				});
			}
			if (SkinVoice.VoiceVolume != null)
			{
				list.Add(new LewdSettings.Volume
				{
					name = "lewd_voicevolume",
					category = 5,
					entry = SkinVoice.VoiceVolume,
					value = SkinVoice.VoiceVolume.Value
				});
			}
			if (SkinVoice.EffortOriginalEntry != null)
			{
				list.Add(new LewdSettings.Toggle
				{
					name = "lewd_effortvoice",
					category = 5,
					entry = SkinVoice.EffortOriginalEntry,
					value = SkinVoice.EffortOriginalEntry.Value
				});
			}
			if (SkinVoice.StandardTail != null)
			{
				list.Add(new LewdSettings.Toggle
				{
					name = "lewd_standardtail",
					category = 5,
					entry = SkinVoice.StandardTail,
					value = SkinVoice.StandardTail.Value
				});
			}
			if (AboutMod.StartupNotice != null)
			{
				list.Add(new LewdSettings.Toggle
				{
					name = "lewd_notice",
					category = 5,
					entry = AboutMod.StartupNotice,
					value = AboutMod.StartupNotice.Value
				});
			}
			if (Plugin.DebugKeys != null)
			{
				list.Add(new LewdSettings.Toggle
				{
					name = "lewd_debug",
					category = 5,
					entry = Plugin.DebugKeys,
					value = Plugin.DebugKeys.Value
				});
			}
			return list;
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x00042454 File Offset: 0x00040654
		public static bool On
		{
			get
			{
				return LewdSettings.Enabled == null || LewdSettings.Enabled.Value;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x00042469 File Offset: 0x00040669
		public static bool TrapsOn
		{
			get
			{
				return LewdSettings.Traps == null || LewdSettings.Traps.Value;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x0004247E File Offset: 0x0004067E
		public static bool WormsOn
		{
			get
			{
				return LewdSettings.Worms == null || LewdSettings.Worms.Value;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x0600040F RID: 1039 RVA: 0x00042493 File Offset: 0x00040693
		public static bool ParasitesOn
		{
			get
			{
				return LewdSettings.ParasiteSuits == null || LewdSettings.ParasiteSuits.Value;
			}
		}

		// Token: 0x040003B8 RID: 952
		public const Setting.SettingCategory Tab = 5;

		// Token: 0x040003B9 RID: 953
		private const string ArmoryGuid = "jens.casualtiesunknown.armory";

		// Token: 0x040003BA RID: 954
		public static ConfigEntry<bool> Enabled;

		// Token: 0x040003BB RID: 955
		public static ConfigEntry<bool> Traps;

		// Token: 0x040003BC RID: 956
		public static ConfigEntry<bool> Worms;

		// Token: 0x040003BD RID: 957
		public static ConfigEntry<bool> SlimeCreatures;

		// Token: 0x040003BE RID: 958
		public static ConfigEntry<bool> ParasiteSuits;

		// Token: 0x040003BF RID: 959
		public static ConfigEntry<bool> DeadlyScenes;

		// Token: 0x040003C0 RID: 960
		public static ConfigEntry<string> Difficulty;

		// Token: 0x040003C1 RID: 961
		public static ConfigEntry<string> Language;

		// Token: 0x040003C2 RID: 962
		private static readonly string[] LangChoices = new string[] { "auto", "ru", "en", "zh" };

		// Token: 0x020001D3 RID: 467
		private class Toggle : SettingBool
		{
			// Token: 0x060009E4 RID: 2532 RVA: 0x0007A5F9 File Offset: 0x000787F9
			public override void Apply()
			{
				if (this.entry != null && this.entry.Value != this.value)
				{
					this.entry.Value = this.value;
				}
			}

			// Token: 0x040009F0 RID: 2544
			public ConfigEntry<bool> entry;
		}

		// Token: 0x020001D4 RID: 468
		private class MasterToggle : LewdSettings.Toggle
		{
			// Token: 0x060009E6 RID: 2534 RVA: 0x0007A630 File Offset: 0x00078830
			public override void Apply()
			{
				bool flag = this.entry != null && this.entry.Value != this.value;
				base.Apply();
				if (!flag)
				{
					return;
				}
				foreach (ConfigEntry<bool> configEntry in LewdSettings.Parts())
				{
					if (configEntry != null && configEntry.Value != this.value)
					{
						configEntry.Value = this.value;
					}
				}
				Plugin.Log.LogInfo("Lewd settings: the whole mod switched " + (this.value ? "on" : "off") + ", every part with it");
				SettingsMenu settingsMenu = Object.FindObjectOfType<SettingsMenu>();
				if (settingsMenu)
				{
					settingsMenu.SelectTab(5);
				}
			}
		}

		// Token: 0x020001D5 RID: 469
		private class Volume : SettingFloat
		{
			// Token: 0x060009E8 RID: 2536 RVA: 0x0007A708 File Offset: 0x00078908
			public Volume()
			{
				this.min = 0f;
				this.max = 2f;
				this.formatValue = (float v) => Mathf.RoundToInt(v * 100f).ToString() + "%";
			}

			// Token: 0x060009E9 RID: 2537 RVA: 0x0007A758 File Offset: 0x00078958
			public override void Apply()
			{
				if (this.entry != null && !Mathf.Approximately(this.entry.Value, this.value))
				{
					this.entry.Value = Mathf.Round(this.value * 20f) / 20f;
				}
			}

			// Token: 0x040009F1 RID: 2545
			public ConfigEntry<float> entry;
		}

		// Token: 0x020001D6 RID: 470
		private class AboutButton : SettingBool
		{
			// Token: 0x060009EA RID: 2538 RVA: 0x0007A7A8 File Offset: 0x000789A8
			public override void Apply()
			{
				if (!this.value)
				{
					return;
				}
				this.value = false;
				AboutMod.ShowWindow();
				SettingsMenu settingsMenu = Object.FindObjectOfType<SettingsMenu>();
				if (settingsMenu)
				{
					settingsMenu.SelectTab(5);
				}
			}
		}

		// Token: 0x020001D7 RID: 471
		private class LangPick : SettingDropdown
		{
			// Token: 0x060009EC RID: 2540 RVA: 0x0007A7E8 File Offset: 0x000789E8
			public override void Apply()
			{
				string text = LewdSettings.LangChoices[Mathf.Clamp(this.value, 0, LewdSettings.LangChoices.Length - 1)];
				if (LewdSettings.Language.Value != text)
				{
					LewdSettings.Language.Value = text;
				}
			}
		}

		// Token: 0x020001D8 RID: 472
		private class DiffPick : SettingDropdown
		{
			// Token: 0x060009EE RID: 2542 RVA: 0x0007A838 File Offset: 0x00078A38
			public override void Apply()
			{
				string text = Diff.Choices[Mathf.Clamp(this.value, 0, Diff.Choices.Length - 1)];
				if (LewdSettings.Difficulty.Value != text)
				{
					LewdSettings.Difficulty.Value = text;
				}
			}
		}

		// Token: 0x020001D9 RID: 473
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040009F2 RID: 2546
			public static Action<Body> <0>__ClearAll;
		}
	}
}
