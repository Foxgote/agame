using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class SaveSystem
{
	public static bool loadedRun;

	public static float savedRunTime;

	private static Body body => PlayerCamera.main.body;

	public static bool help(string s)
	{
		Debug.Log(s);
		return true;
	}

	public static void SaveGame()
	{
		List<SavedItem> list = new List<SavedItem>();
		List<Dictionary<Type, Component>> list2 = new List<Dictionary<Type, Component>>();
		Component[] array2;
		for (int i = 0; i < body.slots.Length; i++)
		{
			if (!body.HoldingItem(i))
			{
				continue;
			}
			Item item = body.GetItem(i);
			Component[] array = (from x in item.gameObject.GetComponents<Component>()
				where (from y in Attribute.GetCustomAttributes(x.GetType())
					where y is Saveable
					select y).Count() > 0
				select x).ToArray();
			Dictionary<Type, Component> dictionary = new Dictionary<Type, Component>();
			array2 = array;
			foreach (Component component in array2)
			{
				dictionary.Add(component.GetType(), component);
			}
			list2.Add(dictionary);
			list.Add(new SavedItem
			{
				id = item.id,
				condition = item.condition,
				slot = i,
				favourited = item.favourited,
				label = item.label,
				swatch = item.colorSwatch
			});
			if (!item.GetComponent<Container>())
			{
				continue;
			}
			foreach (Transform item2 in item.transform)
			{
				if (!item2.TryGetComponent<Item>(out var component2))
				{
					continue;
				}
				Component[] array3 = (from x in component2.gameObject.GetComponents<Component>()
					where (from y in Attribute.GetCustomAttributes(x.GetType())
						where y is Saveable
						select y).Count() > 0
					select x).ToArray();
				Dictionary<Type, Component> dictionary2 = new Dictionary<Type, Component>();
				array2 = array3;
				foreach (Component component3 in array2)
				{
					dictionary2.Add(component3.GetType(), component3);
				}
				list2.Add(dictionary2);
				list.Add(new SavedItem
				{
					id = component2.id,
					condition = component2.condition,
					slot = i,
					favourited = component2.favourited,
					label = component2.label,
					swatch = component2.colorSwatch
				});
			}
		}
		foreach (Item allWearable in body.GetAllWearables())
		{
			Component[] array4 = (from x in allWearable.gameObject.GetComponents<Component>()
				where (from y in Attribute.GetCustomAttributes(x.GetType())
					where y is Saveable
					select y).Count() > 0
				select x).ToArray();
			Dictionary<Type, Component> dictionary3 = new Dictionary<Type, Component>();
			array2 = array4;
			foreach (Component component4 in array2)
			{
				dictionary3.Add(component4.GetType(), component4);
			}
			list2.Add(dictionary3);
			list.Add(new SavedItem
			{
				id = allWearable.id,
				condition = allWearable.condition,
				slot = -1,
				wearSlot = allWearable.Stats.wearSlotId,
				favourited = allWearable.favourited,
				label = allWearable.label,
				swatch = allWearable.colorSwatch
			});
			if (!allWearable.GetComponent<Container>())
			{
				continue;
			}
			foreach (Transform item3 in allWearable.transform)
			{
				if (!item3.TryGetComponent<Item>(out var component5))
				{
					continue;
				}
				Component[] array5 = (from x in component5.gameObject.GetComponents<Component>()
					where (from y in Attribute.GetCustomAttributes(x.GetType())
						where y is Saveable
						select y).Count() > 0
					select x).ToArray();
				Dictionary<Type, Component> dictionary4 = new Dictionary<Type, Component>();
				array2 = array5;
				foreach (Component component6 in array2)
				{
					dictionary4.Add(component6.GetType(), component6);
				}
				list2.Add(dictionary4);
				list.Add(new SavedItem
				{
					id = component5.id,
					condition = component5.condition,
					slot = -1,
					wearSlot = allWearable.Stats.wearSlotId,
					favourited = component5.favourited,
					label = component5.label,
					swatch = component5.colorSwatch
				});
			}
		}
		Component[] array6 = (from x in body.gameObject.GetComponents<Component>()
			where (from y in Attribute.GetCustomAttributes(x.GetType())
				where y is Saveable
				select y).Count() > 0
			select x).ToArray();
		Dictionary<Type, Component> dictionary5 = new Dictionary<Type, Component>();
		array2 = array6;
		foreach (Component component7 in array2)
		{
			dictionary5.Add(component7.GetType(), component7);
		}
		Dictionary<Type, Component>[] array7 = new Dictionary<Type, Component>[body.limbs.Length];
		for (int num2 = 0; num2 < array7.Length; num2++)
		{
			array7[num2] = new Dictionary<Type, Component>();
		}
		for (int num3 = 0; num3 < body.limbs.Length; num3++)
		{
			array2 = (from x in body.limbs[num3].gameObject.GetComponents<Component>()
				where (from y in Attribute.GetCustomAttributes(x.GetType())
					where y is Saveable
					select y).Count() > 0
				select x).ToArray();
			foreach (Component component8 in array2)
			{
				array7[num3].Add(component8.GetType(), component8);
			}
		}
		SavedRecipeData savedRecipeData = new SavedRecipeData();
		savedRecipeData.saved = new (bool, int)[Recipes.recipes.Count];
		for (int num4 = 0; num4 < savedRecipeData.saved.Length; num4++)
		{
			savedRecipeData.saved[num4].Item1 = Recipes.recipes[num4].hasMadeBefore;
			savedRecipeData.saved[num4].Item2 = Recipes.recipes[num4].INT;
		}
		SaveInfo saveInfo = new SaveInfo();
		saveInfo.body = body;
		saveInfo.limbs = body.limbs;
		saveInfo.items = list.ToArray();
		saveInfo.itemComponents = list2.ToArray();
		saveInfo.bodyComponents = dictionary5;
		saveInfo.limbComponents = array7;
		saveInfo.runTime = savedRunTime + WorldGeneration.world.realTimeElapsed;
		saveInfo.biome = WorldGeneration.world.biomeDepth + 1;
		if (saveInfo.biome >= WorldGeneration.world.amountOfLayers)
		{
			saveInfo.biome = 1;
		}
		saveInfo.totalTraveled = WorldGeneration.world.totalTraveled;
		saveInfo.cHeight = WoundView.view.cInfo[0];
		saveInfo.cAge = WoundView.view.cInfo[1];
		saveInfo.cId = WoundView.view.cInfo[2];
		saveInfo.cVer = WoundView.view.cInfo[3];
		saveInfo.savedRecipeData = savedRecipeData;
		saveInfo.lastHappiness = body.lastHappiness;
		saveInfo.trapRarity = WorldGeneration.world.trapRarityMultiplier;
		saveInfo.lootRarity = WorldGeneration.world.lootRarityMultiplier;
		saveInfo.caloriesConsumed = PlayerCamera.main.caloriesConsumed;
		saveInfo.character = PlayerCamera.main.body.charType;
		saveInfo.endlessLoops = WorldGeneration.world.endlessLoops;
		saveInfo.runSettings = DicToTupleList(WorldGeneration.runSettings);
		saveInfo.isMercRun = WorldGeneration.world.isMercRun;
		saveInfo.cheatedSave = SteamManager.disableAchievements;
		string uncompressed = JsonConvert.SerializeObject(saveInfo, Formatting.None, new JsonSerializerSettings
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore
		});
		File.WriteAllBytes(Application.persistentDataPath + "\\save.sv", Zip(uncompressed));
	}

	public static bool HasSave()
	{
		return File.Exists(Application.persistentDataPath + "\\save.sv");
	}

	public static void TryLoadGame()
	{
		if (loadedRun)
		{
			if (!HasSave())
			{
				loadedRun = false;
				return;
			}
			JObject jObject;
			try
			{
				jObject = JObject.Parse(Unzip(File.ReadAllBytes(Application.persistentDataPath + "\\save.sv")));
			}
			catch (Exception ex)
			{
				loadedRun = false;
				ConsoleScript.instance.Alert("A save file exists at \"" + Application.persistentDataPath + "\\save.sv\", but it could not be parsed. The save is invalid or corrupted.\n" + ex.Message + "\n" + ex.StackTrace);
				return;
			}
			foreach (JProperty item2 in jObject["body"].Children<JProperty>())
			{
				FieldInfo field = typeof(Body).GetField(item2.Name);
				if (field == null)
				{
					ConsoleScript.instance.Alert("Trying to load body field \"" + item2.Name + "\", but such field does not exist. Loading will continue. Make a bug report?");
					continue;
				}
				try
				{
					Type fieldType = field.FieldType;
					field.SetValue(value: (!(fieldType == typeof(Skills))) ? Convert.ChangeType(item2.Value, fieldType) : item2.Value.ToObject<Skills>(), obj: body);
				}
				catch (Exception ex2)
				{
					ConsoleScript.instance.Alert("Error occured during setting body field \"" + field.Name + "\".\n" + ex2.Message + "\n" + ex2.StackTrace + "\nLoading will continue. Make a bug report?");
				}
			}
			JObject[] array = jObject["limbs"].Children<JObject>().ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				foreach (JProperty item3 in array[i].Children<JProperty>())
				{
					FieldInfo field2 = typeof(Limb).GetField(item3.Name);
					if (field2 == null)
					{
						ConsoleScript.instance.Alert($"Trying to load limb field \"{item3.Name}\" on limb \"{body.limbs[i]}\", but such field does not exist. Loading will continue. Make a bug report?");
						continue;
					}
					try
					{
						Type fieldType2 = field2.FieldType;
						field2.SetValue(value: (!(fieldType2 == typeof(Limb.ProstheticInfo))) ? Convert.ChangeType(item3.Value, fieldType2) : ((object)item3.Value.ToObject<Limb.ProstheticInfo>()), obj: body.limbs[i]);
					}
					catch (Exception ex3)
					{
						ConsoleScript.instance.Alert($"Error occured during setting \"{body.limbs[i]}\" field \"{field2.Name}\".\n{ex3.Message}\n{ex3.StackTrace}\nLoading will continue. Make a bug report?");
					}
				}
			}
			SavedItem[] array2 = null;
			try
			{
				array2 = jObject["items"].ToObject<SavedItem[]>();
			}
			catch (Exception ex4)
			{
				ConsoleScript.instance.Alert("Error occured during parsing global item data.\n" + ex4.Message + "\n" + ex4.StackTrace + "\nLoading has been cancelled. Make a bug report?");
				return;
			}
			int num = 0;
			SavedItem[] array3 = array2;
			foreach (SavedItem savedItem in array3)
			{
				Item item = null;
				GameObject gameObject = null;
				try
				{
					gameObject = UnityEngine.Object.Instantiate(Resources.Load(savedItem.id), body.transform.position, Quaternion.identity) as GameObject;
					item = gameObject.GetComponent<Item>();
					item.condition = savedItem.condition;
					item.favourited = savedItem.favourited;
					item.label = savedItem.label;
					item.colorSwatch = savedItem.swatch;
				}
				catch (Exception ex5)
				{
					ConsoleScript.instance.Alert("Error occured during creating item \"" + savedItem.id + "\".\n" + ex5.Message + "\n" + ex5.StackTrace + "\nLoading will continue. Make a bug report?");
					continue;
				}
				try
				{
					if (savedItem.slot >= 0)
					{
						if (body.HoldingItem(savedItem.slot))
						{
							body.GetItem(savedItem.slot).GetComponent<Container>().LoadItem(item);
						}
						else
						{
							body.PickUpItem(item, savedItem.slot, force: true);
						}
					}
					else if ((bool)body.GetWearableBySlotID(savedItem.wearSlot))
					{
						body.GetWearableBySlotID(savedItem.wearSlot).GetComponent<Container>().LoadItem(item);
					}
					else
					{
						body.WearWearable(item);
					}
				}
				catch (Exception ex6)
				{
					ConsoleScript.instance.Alert($"Error occured during picking up item \"{item}\".\n{ex6.Message}\n{ex6.StackTrace}\nLoading will continue. Make a bug report?");
					continue;
				}
				foreach (JProperty item4 in jObject["itemComponents"][num].Children<JProperty>())
				{
					Type type = Type.GetType(item4.Name);
					if (type == null)
					{
						ConsoleScript.instance.Alert($"Error occured during loading item \"{item}\". \"{item4.Name}\" is not a valid type. Loading will continue. Make a bug report?");
						continue;
					}
					Component component = gameObject.GetComponent(type);
					if (component == null)
					{
						ConsoleScript.instance.Alert($"Error occured during loading item \"{item}\". Component for \"{type.Name}\" doesn't exist on object. Loading will continue. Make a bug report?");
						continue;
					}
					foreach (JProperty item5 in item4.Value.Children<JProperty>())
					{
						FieldInfo field3 = type.GetField(item5.Name);
						if (field3 == null)
						{
							ConsoleScript.instance.Alert($"Error occured during loading item \"{item}\". Field for \"{item5.Name}\" doesn't exist. Loading will continue. Make a bug report?");
							continue;
						}
						try
						{
							Type fieldType3 = field3.FieldType;
							object value = ((!(fieldType3 == typeof(List<LiquidStack>))) ? item5.Value.ToObject(fieldType3) : item5.Value.ToObject<List<LiquidStack>>());
							field3.SetValue(component, value);
						}
						catch (Exception ex7)
						{
							ConsoleScript.instance.Alert($"Error occured during loading item \"{item}\".\n{ex7.Message}\n{ex7.StackTrace}\nLoading will continue. Make a bug report?");
						}
					}
				}
				num++;
			}
			try
			{
				foreach (JProperty item6 in jObject["bodyComponents"].Children<JProperty>())
				{
					Type type2 = Type.GetType(item6.Name);
					Component obj = body.gameObject.AddComponent(type2);
					foreach (JProperty item7 in item6.Value.Children<JProperty>())
					{
						FieldInfo field4 = type2.GetField(item7.Name);
						object value2 = Convert.ChangeType(conversionType: field4.FieldType, value: item7.Value);
						field4.SetValue(obj, value2);
					}
				}
				for (int k = 0; k < body.limbs.Length; k++)
				{
					foreach (JProperty item8 in jObject["limbComponents"][k].Children<JProperty>())
					{
						Type type3 = Type.GetType(item8.Name);
						Component obj2 = body.limbs[k].gameObject.AddComponent(type3);
						foreach (JProperty item9 in item8.Value.Children<JProperty>())
						{
							FieldInfo field5 = type3.GetField(item9.Name);
							object value3 = Convert.ChangeType(conversionType: field5.FieldType, value: item9.Value);
							field5.SetValue(obj2, value3);
						}
					}
				}
			}
			catch (Exception ex8)
			{
				ConsoleScript.instance.Alert("Error occured during applying player components.\n" + ex8.Message + "\n" + ex8.StackTrace + "\nLoading will continue. Make a bug report?");
			}
			try
			{
				WorldGeneration.world.biomeDepth = (int)jObject["biome"];
				WorldGeneration.world.totalTraveled = (int)jObject["totalTraveled"];
				WorldGeneration.world.lootRarityMultiplier = (float)jObject["lootRarity"];
				WorldGeneration.world.trapRarityMultiplier = (float)jObject["trapRarity"];
				WorldGeneration.runSettings = TupleListToDic(jObject["runSettings"].ToObject<List<(string, string)>>());
				PlayerCamera.main.caloriesConsumed = (int)jObject["caloriesConsumed"];
				savedRunTime = (float)jObject["runTime"];
				WoundView.view.SetCharDetails((int)jObject["cHeight"], (int)jObject["cAge"], (int)jObject["cId"], (int)jObject["cVer"]);
				body.lastHappiness = (from jv in jObject["lastHappiness"].ToArray()
					select (float)jv).ToArray();
				SavedRecipeData savedRecipeData = jObject["savedRecipeData"].ToObject<SavedRecipeData>();
				for (int num2 = 0; num2 < savedRecipeData.saved.Length; num2++)
				{
					Recipes.recipes[num2].hasMadeBefore = savedRecipeData.saved[num2].Item1;
					Recipes.recipes[num2].INT = savedRecipeData.saved[num2].Item2;
				}
				WorldGeneration.world.endlessLoops = (int)jObject["endlessLoops"];
				WorldGeneration.world.isMercRun = (bool)jObject["isMercRun"];
				if (WorldGeneration.world.isMercRun)
				{
					Recipes.recipes.First((Recipe x) => x.result.id == "mercscarf").specialKnown = true;
				}
				if ((bool)jObject["cheatedSave"])
				{
					SteamManager.disableAchievements = true;
					ConsoleScript.instance.Alert("<color=#ff006a>WARNING: This save was previously cheated. Achievements are disabled until you restart the game.</color>");
				}
			}
			catch (Exception ex9)
			{
				ConsoleScript.instance.Alert("Error occured during applying world state.\n" + ex9.Message + "\n" + ex9.StackTrace + "\nMake a bug report?");
			}
			File.Delete(Application.persistentDataPath + "\\save.sv");
		}
		else
		{
			savedRunTime = 0f;
		}
	}

	public static byte[] Zip(string uncompressed)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (GZipStream stream = new GZipStream(memoryStream, System.IO.Compression.CompressionLevel.Optimal))
		{
			using StreamWriter streamWriter = new StreamWriter(stream, Encoding.ASCII);
			streamWriter.Write(uncompressed);
		}
		return memoryStream.ToArray();
	}

	public static string Unzip(byte[] compressed)
	{
		string text = null;
		using MemoryStream stream = new MemoryStream(compressed);
		using GZipStream stream2 = new GZipStream(stream, CompressionMode.Decompress);
		using StreamReader streamReader = new StreamReader(stream2, Encoding.ASCII);
		return streamReader.ReadToEnd();
	}

	public static List<(string, object)> DicToTupleList(Dictionary<string, object> dictionary)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		return dictionary.Select((KeyValuePair<string, object> kvp) => (Key: kvp.Key, Value: kvp.Value)).ToList();
	}

	public static Dictionary<string, object> TupleListToDic(List<(string, string)> tupleList)
	{
		if (tupleList == null)
		{
			throw new ArgumentNullException("tupleList");
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (var tuple in tupleList)
		{
			if (RunSettings.GetSetting(tuple.Item1) is RunSettingFloat)
			{
				dictionary.Add(tuple.Item1, float.Parse(tuple.Item2, CultureInfo.InvariantCulture));
			}
			else if (RunSettings.GetSetting(tuple.Item1) is RunSettingBool)
			{
				dictionary.Add(tuple.Item1, bool.Parse(tuple.Item2));
			}
			else if (RunSettings.GetSetting(tuple.Item1) is RunSettingDropdown)
			{
				dictionary.Add(tuple.Item1, int.Parse(tuple.Item2));
			}
		}
		return dictionary;
	}
}
