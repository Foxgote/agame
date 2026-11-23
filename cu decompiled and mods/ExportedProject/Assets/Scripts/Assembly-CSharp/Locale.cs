using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Locale
{
	public static Language currentLang;

	public static string currentLangName;

	public static string GetItem(string str)
	{
		return GetString(str, 0);
	}

	public static string GetBuilding(string str)
	{
		return GetString(str, 1);
	}

	public static string GetMoodle(string str)
	{
		return GetString(str, 2);
	}

	public static string GetOther(string str)
	{
		return GetString(str, 3);
	}

	public static string GetTutorial(string str)
	{
		return GetString("tut" + str, 3);
	}

	public static (string text, string sprite, string font) GetNote(int index)
	{
		if (currentLang == null)
		{
			LoadLanguage();
		}
		(string, string, string) result = ("", "", "");
		if (index < 0 || index >= currentLang.notes.Length)
		{
			return result;
		}
		return currentLang.notes[index].PickRandom();
	}

	public static (string text, string sprite) GetPdaNote(int index)
	{
		if (currentLang == null)
		{
			LoadLanguage();
		}
		(string, string) result = ("", "");
		if (index < 0 || index >= currentLang.pdaNotes.Count)
		{
			return result;
		}
		return currentLang.pdaNotes[index];
	}

	public static (string text, string sprite) GetPdaNote()
	{
		if (currentLang == null)
		{
			LoadLanguage();
		}
		return GetPdaNote(UnityEngine.Random.Range(0, currentLang.pdaNotes.Count));
	}

	public static string GetString(string str, int type)
	{
		if (currentLang == null)
		{
			LoadLanguage();
		}
		string value = "";
		switch (type)
		{
		case 0:
			if (currentLang.main.TryGetValue(str, out value))
			{
				return value;
			}
			return str;
		case 1:
			if (currentLang.buildings.TryGetValue(str, out value))
			{
				return value;
			}
			return str;
		case 2:
			if (currentLang.moodles.TryGetValue(str, out value))
			{
				return value;
			}
			return str;
		case 3:
			if (currentLang.other.TryGetValue(str, out value))
			{
				return value;
			}
			return str;
		default:
			return str;
		}
	}

	public static List<string> GetCharacter(string str, int chr)
	{
		if (currentLang == null)
		{
			LoadLanguage();
		}
		if (currentLang.character[chr].TryGetValue(str, out var value))
		{
			return value;
		}
		return new List<string> { str };
	}

	public static List<string> GetCharacter(string str)
	{
		return GetCharacter(str, PlayerCamera.main.body.charType);
	}

	public static string GetPauseQuote(string type)
	{
		if (currentLang == null)
		{
			LoadLanguage();
		}
		if (currentLang.pauseQuotes.ContainsKey(type) && currentLang.pauseQuotes[type].Count > 0)
		{
			return currentLang.pauseQuotes[type].PickRandom();
		}
		return "You broke something, doofus.";
	}

	public static string GetCharacter(string str, int chr, int index)
	{
		if (index < 0)
		{
			return "";
		}
		if (currentLang == null)
		{
			LoadLanguage();
		}
		if (currentLang.character[chr].TryGetValue(str, out var value) && index < value.Count)
		{
			return value[index];
		}
		return "";
	}

	public static void ChangeLanguage(string str)
	{
		currentLangName = str;
		PlayerPrefs.SetString("locale", str);
		currentLang = null;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public static List<MiniLang> GetAllLanguages()
	{
		string version = Application.version;
		List<MiniLang> list = new List<MiniLang>();
		foreach (string item in Directory.EnumerateFiles(Application.dataPath + "/Lang/", "*.json"))
		{
			try
			{
				Language language = JsonConvert.DeserializeObject<Language>(File.ReadAllText(item));
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item);
				list.Add(new MiniLang
				{
					name = language.name + " (" + fileNameWithoutExtension + ")" + ((language.version == version) ? "" : ("<color=\"red\"> (Outdated! " + language.version + "->" + version + ")")),
					description = language.description,
					reference = fileNameWithoutExtension
				});
			}
			catch (Exception ex)
			{
				list.Add(new MiniLang
				{
					name = "<color=\"red\">Invalid!",
					description = "Something went wrong while loading the locale. Did you miss a comma?\n\n<color=\"red\">" + ex.Message,
					reference = "error"
				});
			}
		}
		return list;
	}

	public static void LoadLanguage()
	{
		currentLangName = PlayerPrefs.GetString("locale");
		if (string.IsNullOrEmpty(currentLangName))
		{
			currentLangName = "EN";
			if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
			{
				currentLangName = "zh-CN";
			}
		}
		if (File.Exists(Application.dataPath + "/Lang/" + currentLangName + ".json"))
		{
			try
			{
				currentLang = JsonConvert.DeserializeObject<Language>(File.ReadAllText(Application.dataPath + "/Lang/" + currentLangName + ".json"));
				if (currentLang.version != Application.version)
				{
					LanguageSelect.LoadLanguageOutdated();
				}
				return;
			}
			catch
			{
				currentLang = new Language();
				Debug.LogError("Language missing!");
				LanguageSelect.LoadLanguageError();
				return;
			}
		}
		currentLang = new Language();
		Debug.LogError("Language missing!");
		LanguageSelect.LoadLanguageError();
	}

	public static void GenerateTestLanguageBase()
	{
		Language language = new Language();
		language.name = "Test";
		language.notes = new List<(string, string, string)>[2]
		{
			new List<(string, string, string)> { ("test1", "test2", "test3") },
			new List<(string, string, string)> { ("test1", "test2", "test3") }
		};
		string contents = JsonConvert.SerializeObject(language, Formatting.Indented);
		File.WriteAllText(Application.dataPath + "/Lang/TEST.json", contents);
	}
}
