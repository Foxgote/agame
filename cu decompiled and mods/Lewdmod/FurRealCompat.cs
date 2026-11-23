using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000091 RID: 145
	internal static class FurRealCompat
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x00032AE5 File Offset: 0x00030CE5
		public static bool Present
		{
			get
			{
				return Chainloader.PluginInfos.ContainsKey("com.nocturnal.furreal");
			}
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00032AF8 File Offset: 0x00030CF8
		public static void ResetTail()
		{
			if (!FurRealCompat.Present)
			{
				return;
			}
			try
			{
				Type type = AccessTools.TypeByName("FurReal.FurSpriteFactory");
				foreach (string text in new string[] { "SourcePixelCache", "Cache", "ParticlePalettes" })
				{
					FieldInfo fieldInfo = AccessTools.Field(type, text);
					IDictionary dictionary = ((fieldInfo != null) ? fieldInfo.GetValue(null) : null) as IDictionary;
					if (dictionary != null)
					{
						dictionary.Clear();
					}
				}
				Type type2 = AccessTools.TypeByName("FurReal.FurSpriteVisual");
				Type type3 = AccessTools.TypeByName("FurReal.TailFurVisual");
				if (!(type2 == null) && !(type3 == null))
				{
					FieldInfo fieldInfo2 = AccessTools.Field(type2, "rendererToAffect");
					FieldInfo fieldInfo3 = AccessTools.Field(type2, "source");
					FieldInfo fieldInfo4 = AccessTools.Field(type2, "shown");
					FieldInfo fieldInfo5 = AccessTools.Field(type2, "hasShownAppearance");
					int num = 0;
					foreach (Object @object in Object.FindObjectsOfType(type3))
					{
						SpriteRenderer spriteRenderer = ((fieldInfo2 != null) ? fieldInfo2.GetValue(@object) : null) as SpriteRenderer;
						Sprite sprite = ((fieldInfo3 != null) ? fieldInfo3.GetValue(@object) : null) as Sprite;
						if (spriteRenderer && sprite && spriteRenderer.sprite != sprite)
						{
							spriteRenderer.sprite = sprite;
						}
						if (fieldInfo4 != null)
						{
							fieldInfo4.SetValue(@object, null);
						}
						if (fieldInfo5 != null)
						{
							fieldInfo5.SetValue(@object, false);
						}
						num++;
					}
					Plugin.Log.LogInfo(string.Format("Fur Real: its tail coat rebuilt from the new picture ({0} tails)", num));
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Fur Real tail reset: " + ex.Message);
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00032CDC File Offset: 0x00030EDC
		public static void Bind(ConfigFile c)
		{
			FurRealCompat.ours = c.Bind<bool>("Compat", "FurRealSpritesOffByUs", false, "Set by older versions of the mod: Fur Real's body sprites were turned off for a living suit and must be turned back on. Leave it alone.");
			FurRealCompat.ignoredSlots = c.Bind<string>("Compat", "FurRealIgnoredSlots", "back,bandolier,belt,blindfold,eyes,mouth,neck,knees,thigh,thighback,hat,torsofront,bra,panties", "Clothing slots that do not hide Fur Real's fur on their limb (bags, belts, small pieces that cover only part of it). Comma-separated.");
			FurRealCompat.ignored = FurRealCompat.Parse(FurRealCompat.ignoredSlots.Value);
			FurRealCompat.ignoredSlots.SettingChanged += delegate(object s, EventArgs e)
			{
				FurRealCompat.ignored = FurRealCompat.Parse(FurRealCompat.ignoredSlots.Value);
				FurRealCompat.covered.Clear();
			};
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00032D60 File Offset: 0x00030F60
		private static HashSet<string> Parse(string v)
		{
			return new HashSet<string>(from x in (v ?? "").Split(new char[] { ',' })
				select x.Trim().ToLowerInvariant() into x
				where x.Length > 0
				select x);
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00032DD4 File Offset: 0x00030FD4
		public static void Update()
		{
			if (FurRealCompat.looked)
			{
				return;
			}
			FurRealCompat.looked = true;
			if (!Chainloader.PluginInfos.ContainsKey("com.nocturnal.furreal"))
			{
				return;
			}
			Type type = AccessTools.TypeByName("FurReal.FurVisual");
			Type type2 = AccessTools.TypeByName("FurReal.FurSpriteVisual");
			MethodInfo methodInfo = ((type == null) ? null : AccessTools.Method(type, "TryGetCoat", null, null));
			MethodInfo source = ((type2 == null) ? null : AccessTools.PropertyGetter(type2, "CurrentSource"));
			if (methodInfo == null)
			{
				Plugin.Log.LogWarning("Fur Real found, but not its FurVisual.TryGetCoat: its fur stays over clothes and living suits");
				return;
			}
			if (source != null)
			{
				FurRealCompat.currentSource = (object o) => source.Invoke(o, null) as Sprite;
			}
			new Harmony("jens.casualtiesunknown.lewdstatus.furreal").Patch(methodInfo, null, new HarmonyMethod(typeof(FurRealCompat), "CoatPostfix", null), null, null, null);
			Type type3 = AccessTools.TypeByName("FurReal.TailFurVisual");
			MethodInfo methodInfo2 = ((type3 == null) ? null : AccessTools.Method(type3, "TryGetCoat", null, null));
			if (methodInfo2 != null)
			{
				new Harmony("jens.casualtiesunknown.lewdstatus.furreal.tail").Patch(methodInfo2, null, new HarmonyMethod(typeof(FurRealCompat), "TailCoatPostfix", null), null, null, null);
			}
			Plugin.Log.LogInfo("Fur Real found: its fur is hidden on the limbs clothes or a living suit cover");
			Type type4 = AccessTools.TypeByName("FurReal.Cfg");
			ConfigEntry<bool> configEntry;
			if (!(type4 == null))
			{
				FieldInfo fieldInfo = AccessTools.Field(type4, "BodySprites");
				configEntry = ((fieldInfo != null) ? fieldInfo.GetValue(null) : null) as ConfigEntry<bool>;
			}
			else
			{
				configEntry = null;
			}
			ConfigEntry<bool> configEntry2 = configEntry;
			if (FurRealCompat.ours != null && FurRealCompat.ours.Value)
			{
				if (configEntry2 != null)
				{
					configEntry2.Value = true;
				}
				FurRealCompat.ours.Value = false;
				Plugin.Log.LogInfo("Fur Real: body fur back on (turned off by an older version)");
			}
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00032F95 File Offset: 0x00031195
		private static void TailCoatPostfix(ref bool __result)
		{
			if (__result && SkinVoice.StandardTail != null && !SkinVoice.StandardTail.Value)
			{
				__result = false;
			}
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00032FB4 File Offset: 0x000311B4
		private static void CoatPostfix(MonoBehaviour __instance, ref bool __result)
		{
			if (!__result || !__instance)
			{
				return;
			}
			try
			{
				if (SkinVoice.StandardTail != null && !SkinVoice.StandardTail.Value && __instance.name.IndexOf("Tail", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					__result = false;
				}
				else
				{
					Limb component = __instance.GetComponent<Limb>();
					if (component && FurRealCompat.Covered(component, __instance))
					{
						__result = false;
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x060002EB RID: 747 RVA: 0x0003302C File Offset: 0x0003122C
		private static bool Covered(Limb limb, MonoBehaviour visual)
		{
			int instanceID = limb.GetInstanceID();
			KeyValuePair<float, bool> keyValuePair;
			if (FurRealCompat.covered.TryGetValue(instanceID, out keyValuePair) && Time.time - keyValuePair.Key < 0.25f)
			{
				return keyValuePair.Value;
			}
			bool flag = FurRealCompat.ByClothes(limb) || FurRealCompat.BySuit(limb, visual);
			FurRealCompat.covered[instanceID] = new KeyValuePair<float, bool>(Time.time, flag);
			return flag;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00033098 File Offset: 0x00031298
		private static bool ByClothes(Limb limb)
		{
			Transform transform = limb.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.gameObject.activeInHierarchy)
				{
					Item item = child.GetComponent<Item>();
					if (!item)
					{
						WearableExtension component = child.GetComponent<WearableExtension>();
						item = (component ? component.reference : null);
					}
					if (item && item.Stats != null && item.Stats.wearable)
					{
						OutfitDef outfitDef = Outfits.ById(item.id);
						if (outfitDef == null || !outfitDef.IsDevice || Devices.Covers(outfitDef.Kind))
						{
							SpriteRenderer component2 = child.GetComponent<SpriteRenderer>();
							if ((!component2 || component2.enabled) && !FurRealCompat.ignored.Contains((item.Stats.wearSlotId ?? "").ToLowerInvariant()))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00033190 File Offset: 0x00031390
		private static bool BySuit(Limb limb, MonoBehaviour visual)
		{
			Body body = limb.body;
			if (body && (!PlayerCamera.main || !(body == PlayerCamera.main.body)))
			{
				return false;
			}
			Suit.Look mine = Suit.Mine;
			if (mine.stage <= 0 || mine.Drone)
			{
				return false;
			}
			Sprite sprite = ((FurRealCompat.currentSource != null) ? FurRealCompat.currentSource(visual) : null);
			if (!sprite)
			{
				SpriteRenderer component = limb.GetComponent<SpriteRenderer>();
				sprite = ((component != null) ? component.sprite : null);
			}
			return sprite && sprite.texture && Suit.Covers(sprite.texture.name, mine.stage, mine);
		}

		// Token: 0x040002B5 RID: 693
		private const string Guid = "com.nocturnal.furreal";

		// Token: 0x040002B6 RID: 694
		private static ConfigEntry<bool> ours;

		// Token: 0x040002B7 RID: 695
		private static ConfigEntry<string> ignoredSlots;

		// Token: 0x040002B8 RID: 696
		private static HashSet<string> ignored = new HashSet<string>();

		// Token: 0x040002B9 RID: 697
		private static bool looked;

		// Token: 0x040002BA RID: 698
		private static Func<object, Sprite> currentSource;

		// Token: 0x040002BB RID: 699
		private static readonly Dictionary<int, KeyValuePair<float, bool>> covered = new Dictionary<int, KeyValuePair<float, bool>>();
	}
}
