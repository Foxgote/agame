using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x0200003F RID: 63
	internal static class ScavArt
	{
		// Token: 0x06000105 RID: 261 RVA: 0x0000EDBC File Offset: 0x0000CFBC
		public static void Load(string pluginDir)
		{
			string text = Path.Combine(pluginDir, "skins");
			if (Directory.Exists(text))
			{
				foreach (string text2 in Directory.GetDirectories(text))
				{
					Dictionary<string, Sprite> dictionary = new Dictionary<string, Sprite>();
					bool flag = Path.GetFileName(text2).StartsWith("slimehost");
					foreach (string text3 in Directory.GetFiles(text2, "experiment*.png"))
					{
						Sprite sprite = ScavArt.MakeSprite(text3);
						if (flag)
						{
							sprite.texture.name = "slimehost_" + sprite.texture.name;
						}
						dictionary[Path.GetFileNameWithoutExtension(text3).Substring("experiment".Length)] = sprite;
					}
					ScavArt.Skins[Path.GetFileName(text2)] = dictionary;
				}
			}
			string directoryName = Path.GetDirectoryName(pluginDir);
			List<ValueTuple<string, string>> list = new List<ValueTuple<string, string>>
			{
				new ValueTuple<string, string>("minersuit", "miner_"),
				new ValueTuple<string, string>("labsuit", "lab_"),
				new ValueTuple<string, string>("thermosuit", "thermo_")
			};
			foreach (string text4 in Directory.Exists(directoryName) ? Directory.GetDirectories(directoryName, "bodysuit*") : new string[0])
			{
				list.Add(new ValueTuple<string, string>(Path.GetFileName(text4), Path.GetFileName(text4) + "_"));
			}
			foreach (ValueTuple<string, string> valueTuple in list)
			{
				string item = valueTuple.Item1;
				string item2 = valueTuple.Item2;
				string text5 = Path.Combine(directoryName, item);
				if (Directory.Exists(text5))
				{
					Dictionary<string, Sprite> dictionary2 = new Dictionary<string, Sprite>();
					foreach (string text6 in Directory.GetFiles(text5, item2 + "experiment*.png"))
					{
						dictionary2[Path.GetFileNameWithoutExtension(text6).Substring(item2.Length + "experiment".Length)] = ScavArt.MakeSprite(text6);
					}
					ScavArt.Overalls[item] = dictionary2;
				}
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000F01C File Offset: 0x0000D21C
		private static Sprite MakeSprite(string file)
		{
			Texture2D texture2D = new Texture2D(2, 2, 4, false)
			{
				filterMode = 0,
				wrapMode = 1
			};
			ImageConversion.LoadImage(texture2D, File.ReadAllBytes(file));
			texture2D.name = Path.GetFileNameWithoutExtension(file);
			return Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), 8f);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000F094 File Offset: 0x0000D294
		public static string RandomSpacesuit()
		{
			List<string> list = ScavArt.Overalls.Keys.Where<string>((string k) => k.StartsWith("bodysuit")).ToList<string>();
			if (list.Count <= 0)
			{
				return null;
			}
			return list[Random.Range(0, list.Count)];
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000F0F4 File Offset: 0x0000D2F4
		public static string RandomSkin(bool female)
		{
			List<string> list = ScavArt.Skins.Keys.Where<string>((string k) => k == "argonian" || k.StartsWith(female ? "female_" : "male_")).ToList<string>();
			if (list.Count <= 0)
			{
				return ScavArt.Skins.Keys.FirstOrDefault<string>();
			}
			return list[Random.Range(0, list.Count)];
		}

		// Token: 0x04000102 RID: 258
		public static readonly Dictionary<string, Dictionary<string, Sprite>> Skins = new Dictionary<string, Dictionary<string, Sprite>>();

		// Token: 0x04000103 RID: 259
		public static readonly Dictionary<string, Dictionary<string, Sprite>> Overalls = new Dictionary<string, Dictionary<string, Sprite>>();

		// Token: 0x04000104 RID: 260
		public static readonly Color[] RaiderDyes = new Color[]
		{
			new Color(1f, 1f, 1f),
			new Color(0.62f, 0.72f, 1f),
			new Color(0.66f, 0.95f, 0.62f),
			new Color(1f, 0.6f, 0.58f),
			new Color(0.55f, 0.55f, 0.6f),
			new Color(0.92f, 0.86f, 0.6f),
			new Color(0.8f, 0.62f, 0.95f)
		};
	}
}
