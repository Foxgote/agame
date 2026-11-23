using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConsoleScript : MonoBehaviour
{
	private struct RepeatCommandArgs
	{
		public int times;

		public float repeatTime;

		public string commands;
	}

	public static List<Command> Commands = new List<Command>();

	public TMP_InputField input;

	public bool active;

	public GameObject consoleCanvas;

	public GameObject descriptionBack;

	public TextMeshProUGUI descriptionText;

	public TextMeshProUGUI logText;

	public RectTransform consoleRect;

	public List<string> logs;

	public List<string> executedCommands;

	public Dictionary<KeyCode, string> customBinds;

	public static ConsoleScript instance;

	public int selectedCommandIndex;

	public static bool logErrors = true;

	private bool echo = true;

	private string lastErr = "";

	private bool playerDetailsRegistered;

	public ConsoleSettings settings;

	public RectTransform logRect;

	public bool noClip;

	public Vector2 noClipPos;

	public bool fullBright;

	public Vector2 freecamPos;

	private bool registeredSpawnEntities;

	public static string configPath => Application.persistentDataPath + "\\console.json";

	private void Awake()
	{
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		ConsoleSettings consoleSettings = ReadConsoleSettings();
		if (consoleSettings != null)
		{
			settings = consoleSettings;
		}
		else
		{
			settings = ConsoleSettings.defaultSettings;
		}
		customBinds = new Dictionary<KeyCode, string>();
		ApplySettings();
	}

	private void ApplySettings()
	{
		logRect.sizeDelta = new Vector2(1f, settings.consoleHeight - 32f);
		logRect.GetComponent<Image>().color = new Color(settings.backgroundColor.r, settings.backgroundColor.g, settings.backgroundColor.b, 226f / 255f);
		input.GetComponent<Image>().color = new Color(settings.backgroundColor.r, settings.backgroundColor.g, settings.backgroundColor.b, 1f);
		descriptionBack.GetComponent<Image>().color = new Color(settings.backgroundColor.r, settings.backgroundColor.g, settings.backgroundColor.b, 0.772549f);
		input.textComponent.color = settings.textColor;
		logText.color = settings.textColor;
		descriptionText.color = settings.textColor;
	}

	private void WriteConsoleSettings()
	{
		settings.hexTextColor = "#" + ColorUtility.ToHtmlStringRGB(settings.textColor);
		settings.hexBackgroundColor = "#" + ColorUtility.ToHtmlStringRGB(settings.backgroundColor);
		File.WriteAllText(configPath, JsonConvert.SerializeObject(settings, Formatting.Indented, new JsonSerializerSettings
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Error
		}));
		LogToConsole("Saved console config to disk.");
	}

	private ConsoleSettings ReadConsoleSettings()
	{
		if (File.Exists(configPath))
		{
			ConsoleSettings? consoleSettings = JsonConvert.DeserializeObject<ConsoleSettings>(File.ReadAllText(configPath));
			ColorUtility.TryParseHtmlString(consoleSettings.hexBackgroundColor, out var color);
			consoleSettings.backgroundColor = color;
			ColorUtility.TryParseHtmlString(consoleSettings.hexTextColor, out color);
			consoleSettings.textColor = color;
			return consoleSettings;
		}
		return null;
	}

	public void RegisterSpawnEntities()
	{
		if (!registeredSpawnEntities)
		{
			Command command = SearchExact("spawn");
			List<GameObject> list = new List<GameObject>();
			list.AddRange(Resources.LoadAll<GameObject>(""));
			list = list.Where((GameObject x) => (bool)x.GetComponent<Item>() || (bool)x.GetComponent<BuildingEntity>()).ToList();
			command.argAutofill.Add(0, list.Select((GameObject x) => x.name).ToList());
			registeredSpawnEntities = true;
		}
	}

	private void Start()
	{
		active = false;
		logs = new List<string>();
		executedCommands = new List<string>();
		if (Commands.Count == 0)
		{
			RegisterAllCommands();
		}
		foreach (string customCommand in settings.customCommands)
		{
			AddCustomCommand(customCommand.Split(' '));
		}
		foreach (KeyValuePair<string, string> customBind in settings.customBinds)
		{
			customBinds.Add(ParseKeycode(customBind.Key), customBind.Value);
		}
		SceneManager.activeSceneChanged += SceneChanged;
		HandleConsoleActiveState();
		Time.maximumDeltaTime = 0.5f;
	}

	public void ToggleActiveState()
	{
		active = !active;
		if (active)
		{
			input.Select();
			RegisterSpawnEntities();
			UpdateLogScreen();
		}
	}

	private void HandleConsoleActiveState()
	{
		if (Input.GetKeyDown(KeyBinds.GetBind("console")))
		{
			ToggleActiveState();
		}
		bool flag = consoleRect.anchoredPosition.y < 150f;
		if (consoleCanvas.activeSelf != flag)
		{
			consoleCanvas.SetActive(flag);
		}
		float b = (active ? (0f - settings.consoleHeight) : 200f);
		consoleRect.anchoredPosition = new Vector2(0f, Mathf.Lerp(consoleRect.anchoredPosition.y, b, Time.unscaledDeltaTime * 8f));
	}

	private void AddCommandToLogAndClearInput()
	{
		executedCommands.Add(input.text);
		input.text = "";
		selectedCommandIndex = 9999;
	}

	private void TryExecuteCommand(string[] args, bool addToLog = true)
	{
		if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
		{
			return;
		}
		Command command = SearchExact(args[0]);
		if (command == null)
		{
			LogToConsole("<color=orange>Invalid command \"" + args[0] + "\"</color>");
			if (addToLog)
			{
				AddCommandToLogAndClearInput();
			}
			return;
		}
		try
		{
			command.action(args);
		}
		catch (Exception ex)
		{
			LogToConsole("<color=orange>Command \"" + args[0] + "\" failed: " + ex.Message + "</color>");
		}
		if (addToLog)
		{
			AddCommandToLogAndClearInput();
		}
	}

	private void LogToConsole(string text)
	{
		if (echo)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
			logs.Add("[<alpha=#55>" + timeSpan.ToString("mm\\:ss") + "<alpha=#FF>] " + text);
			if (logs.Count > 100)
			{
				logs.RemoveAt(0);
			}
			if (active)
			{
				UpdateLogScreen();
			}
		}
	}

	private void UpdateLogScreen()
	{
		logText.text = string.Join("\n", logs);
	}

	private void ApplicationLogCallback(string condition, string stackTrace, LogType type)
	{
		if (logErrors && !(lastErr == condition))
		{
			lastErr = condition;
			switch (type)
			{
			case LogType.Error:
				LogToConsole("<color=red>" + condition + "; " + stackTrace + "</color>");
				break;
			case LogType.Exception:
				LogToConsole("<color=red>" + condition + "; " + stackTrace + "</color>");
				break;
			case LogType.Assert:
				LogToConsole("<color=red>" + condition + "; " + stackTrace + "</color>");
				break;
			case LogType.Warning:
				LogToConsole("<color=yellow>" + condition + "; " + stackTrace + "</color>");
				break;
			case LogType.Log:
				LogToConsole(condition);
				break;
			}
		}
	}

	public void HandleBinds()
	{
		if (active)
		{
			return;
		}
		foreach (KeyValuePair<KeyCode, string> customBind in customBinds)
		{
			if (Input.GetKeyDown(customBind.Key))
			{
				RunCommandString(customBind.Value);
			}
		}
	}

	public void SceneChanged(Scene scene, Scene scene2)
	{
		noClip = false;
		noClipPos = Vector2.zero;
	}

	public void Update()
	{
		HandleConsoleActiveState();
		HandleBinds();
		if (noClip && (bool)PlayerCamera.main)
		{
			noClipPos += PlayerCamera.main.body.moveDir * Time.deltaTime * 20f * (Input.GetKey(KeyCode.LeftShift) ? 4f : 1f);
			PlayerCamera.main.body.transform.position = noClipPos;
		}
		if ((bool)PlayerCamera.main && PlayerCamera.main.isFreecam)
		{
			Vector2 zero = Vector2.zero;
			zero.y += (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f);
			zero.y -= (Input.GetKey(KeyCode.DownArrow) ? 1f : 0f);
			zero.x += (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f);
			zero.x -= (Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
			zero *= (Input.GetKey(KeyCode.LeftShift) ? 60f : 10f);
			freecamPos += zero * Time.unscaledDeltaTime;
		}
		if (active)
		{
			string[] args = input.text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			string[] args2 = input.text.Split(' ');
			HandleDescriptionText(args2);
			descriptionBack.SetActive(!string.IsNullOrEmpty(descriptionText.text));
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				TryFinishCommandPart(args2);
			}
			if (Input.GetKeyDown(KeyCode.Return))
			{
				TryExecuteCommand(args);
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				GoToCommandHistory(-1);
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				GoToCommandHistory(1);
			}
		}
	}

	public void GoToCommandHistory(int change)
	{
		if (executedCommands.Count != 0)
		{
			selectedCommandIndex += change;
			if (selectedCommandIndex < 0)
			{
				selectedCommandIndex = 0;
			}
			if (selectedCommandIndex >= executedCommands.Count)
			{
				selectedCommandIndex = executedCommands.Count - 1;
			}
			input.text = executedCommands[selectedCommandIndex];
			SetCaretToEnd();
		}
	}

	public void TryFinishCommandPart(string[] args)
	{
		if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
		{
			return;
		}
		if (args.Length == 1)
		{
			Command command = SearchFirst(args[0]);
			if (command != null)
			{
				input.text = command.name + " ";
			}
		}
		else
		{
			Command command2 = SearchExact(args[0]);
			if (command2 != null && command2.argAutofill != null && command2.argAutofill.TryGetValue(args.Length - 2, out var value))
			{
				string text = SearchArgumentAutofillFirst(args[^1], value);
				if (!string.IsNullOrEmpty(text))
				{
					input.text = string.Join(" ", args.Take(args.Length - 1)) + " " + text;
				}
			}
			char c = input.text.Substring(input.text.Length - 1, 1)[0];
			if (c == '.' || c == ',')
			{
				input.text += "0";
			}
			input.text += " ";
		}
		SetCaretToEnd();
	}

	public void SetCaretToEnd()
	{
		input.caretPosition = input.text.Length;
	}

	public void HandleDescriptionText(string[] args)
	{
		descriptionText.text = "";
		Vector3 bottomLeft = input.textComponent.textInfo.characterInfo[input.caretPosition].bottomLeft;
		Vector3 vector = input.transform.TransformPoint(bottomLeft);
		descriptionText.transform.position = vector + Vector3.down * 35f;
		if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
		{
			return;
		}
		string text = "";
		if (args.Length == 1)
		{
			List<Command> list = Search(args[0]);
			if (list.Count == 0)
			{
				text = "<color=orange>Invalid command \"" + args[0] + "\"";
				descriptionText.text = text;
				return;
			}
			bool flag = true;
			foreach (Command item in list)
			{
				if (flag)
				{
					text += "<color=yellow>";
				}
				text = text + item.GetShortDescription(flag) + "\n";
				if (flag)
				{
					text += "</color>";
				}
				flag = false;
			}
		}
		else
		{
			Command command = SearchExact(args[0]);
			if (command == null)
			{
				text = "<color=orange>Invalid command \"" + args[0] + "\"";
				descriptionText.text = text;
				return;
			}
			text = command.name + " ";
			int num = args.Length - 2;
			for (int i = 0; i < command.argDescription.Length && i < command.argDescription.Length; i++)
			{
				if (i == num)
				{
					text += "<color=yellow>";
					text = text + "[" + command.argDescription[i].shortDesc + " - " + command.argDescription[i].longDesc + "] ";
					text += "</color>";
				}
				else
				{
					text = text + "[" + command.argDescription[i].shortDesc + "] ";
				}
			}
			if (command.argAutofill != null && command.argAutofill.TryGetValue(num, out var value))
			{
				value = SearchArgumentAutofill(args[^1], value);
				text += "\n";
				for (int j = 0; j < value.Count; j++)
				{
					if (j == 0)
					{
						text += "<color=yellow>";
						text = text + value[j] + "\n";
						text += "</color>";
					}
					else
					{
						text = text + value[j] + "\n";
					}
				}
			}
		}
		descriptionText.text = text;
	}

	public static List<string> SearchArgumentAutofill(string input, List<string> fills)
	{
		return fills.Where((string c) => c.Contains(input)).ToList();
	}

	public static string SearchArgumentAutofillFirst(string input, List<string> fills)
	{
		return fills.Where((string c) => c.Contains(input)).FirstOrDefault();
	}

	public static List<Command> Search(string input)
	{
		return Commands.Where((Command c) => c.name.Contains(input, StringComparison.OrdinalIgnoreCase)).ToList();
	}

	public static Command SearchFirst(string input)
	{
		return Commands.Where((Command c) => c.name.Contains(input, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
	}

	public static Command SearchExact(string input)
	{
		return Commands.Where((Command c) => c.name.Equals(input, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
	}

	public void RegisterAllCommands()
	{
		Application.logMessageReceived += ApplicationLogCallback;
		Commands.Add(new Command("help", "Shows the command list.", (string[] args) =>
		{
			List<string> list = new List<string>();
			foreach (Command command2 in Commands)
			{
				list.Add(command2.GetShortDescription() + " - <alpha=#48>" + command2.description + "<alpha=#FF>");
			}
			list.Add("Scroll to see all commands.");
			LogToConsole(string.Join("\n", list));
		}, null));
		Commands.Add(new Command("heal", "Fully heals the player character.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			bool ignoreDismembered = false;
			if (args.Length >= 2)
			{
				ignoreDismembered = ParseBool(args[1]);
			}
			PlayerCamera.main.body.Heal(ignoreDismembered);
			LogToConsole("Healed the player.");
		}, null, ("bool ignoreDismember", "If true, will not reattach dismembered limbs.")));
		Commands.Add(new Command("coagulate", "Stops all bleeding on the player character.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			Limb[] limbs = PlayerCamera.main.body.limbs;
			for (int i = 0; i < limbs.Length; i++)
			{
				limbs[i].bleedAmount = 0f;
			}
			PlayerCamera.main.body.internalBleeding = 0f;
			LogToConsole("Stopped bleeding on player.");
		}, null));
		Commands.Add(new Command("kill", "Instantly kills the player character by setting their brain health to 0.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			PlayerCamera.main.body.brainHealth = 0f;
			LogToConsole("Killed the player.");
		}, null));
		Commands.Add(new Command("spawn", "Spawns an item/building at a given position.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			CheckArgumentCount(args, 1);
			string text = args[1];
			float num = 1f;
			int num2 = 1;
			Vector2 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (args.Length > 2)
			{
				vector = ParsePosition(args[2]);
			}
			if (args.Length > 3)
			{
				num = ParseFloat(args[3]);
			}
			if (args.Length > 4)
			{
				num2 = ParseInt(args[4]);
			}
			for (int i = 0; i < num2; i++)
			{
				GameObject gameObject = Utils.Create(text, vector, 0f);
				if (!gameObject)
				{
					throw new Exception("Could not find entity \"" + text + "\".");
				}
				if (gameObject.TryGetComponent<Item>(out var component))
				{
					component.SetCondition(num);
					if (gameObject.TryGetComponent<AmmoScript>(out var component2) && component2.itemType == AmmoScript.AmmoItemType.Magazine)
					{
						component2.rounds = Mathf.RoundToInt((float)component2.maxRounds * num);
					}
					if (Vector2.Distance(gameObject.transform.position, PlayerCamera.main.body.transform.position) < 2f)
					{
						PlayerCamera.main.body.AutoPickUpItem(component);
					}
				}
				if (gameObject.TryGetComponent<BuildingEntity>(out var component3))
				{
					component3.blockPlacedOn = WorldGeneration.world.WorldToBlockPos(vector + Vector2.down);
				}
			}
			LogToConsole(string.Format("Created \"{0}\" {1}at {2}.", text, (num2 > 1) ? $"(x{num2}) " : "", vector));
		}, null, ("string id", "The ID of the entity to spawn"), ("position", "Where to spawn the item"), ("float condition", "Optional item condition, ranging 0-1, if applicable"), ("int amount", "Optional amount of the entity to spawn")));
		Commands.Add(new Command("spawncategory", "Spawns all items from a given loot pool with zero gravity.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			CheckArgumentCount(args, 2);
			string arg = args[1];
			Vector2 vector = ParsePosition(args[2]);
			foreach (var item2 in ItemLootPool.AllItemsFromPool(args[1]) ?? throw new Exception("Invalid item category \"" + args[1] + "\"."))
			{
				Utils.Create(item2.Item1, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + UnityEngine.Random.insideUnitCircle * 3f, 0f).GetComponent<Rigidbody2D>().gravityScale = 0f;
			}
			LogToConsole($"Spawned all items from category \"{arg}\" at {vector}.");
		}, null, ("string id", "The ID of the category to spawn from"), ("position", "Where to spawn the item")));
		Commands.Add(new Command("tp", "Teleports the player to a given position. Defaults to cursor if empty.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			Vector2 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (args.Length > 1)
			{
				vector = ParsePosition(args[1]);
			}
			PlayerCamera.main.body.transform.position = vector;
			PlayerCamera.main.transform.position = vector;
			LogToConsole($"Teleported player to {vector}.");
		}, null, ("position", "Where to teleport to")));
		Commands.Add(new Command("skiplayer", "Instantly skips to the next layer if not provided with a layer index, or goes directly to a provided layer index.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			int num = WorldGeneration.world.biomeDepth + 1;
			if (args.Length > 1)
			{
				num = ParseInt(args[1]);
			}
			WorldGeneration.world.biomeDepth = num - 1;
			WorldGeneration.world.StartCoroutine(WorldGeneration.world.RegenerateWorld());
			LogToConsole("Going to layer " + num + " (" + WorldGeneration.world.biomeTitles[num] + ").");
		}, null, ("int index", "The layer index to go to.")));
		Commands.Add(new Command("log", "Adds text to the console history.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);
			string text = string.Join(" ", args.Skip(1));
			LogToConsole(text);
		}, null, ("string text", "The text to log.")));
		Commands.Add(new Command("talk", "Makes the player character say something.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);
			CheckForWorld();
			string text = string.Join(" ", args.Skip(1));
			PlayerCamera.main.body.talker.Talk(text);
			LogToConsole("Made player say \"" + text + "\"");
		}, null, ("string text", "The text make the character speak")));
		Commands.Add(new Command("alert", "Displays an in-game alert.", (string[] args) =>
		{
			CheckArgumentCount(args, 2);
			CheckForWorld();
			string text = string.Join(" ", args.Skip(2));
			bool important = ParseBool(args[1]);
			PlayerCamera.main.DoAlert(text, important);
			LogToConsole("Displayed alert \"" + text + "\"");
		}, null, ("bool important", "If true, displays the alert in the center of the screen."), ("string text", "The text make the character speak.")));
		Commands.Add(new Command("saveandquit", "Instantly saves the game and exits to the main menu. Loading the save will put you at the beginning of the current layer.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			if (WorldGeneration.world.biomeDepth >= 9)
			{
				throw new Exception("Cannot save the game at WHERE WE MUST REACH and beyond!");
			}
			WorldGeneration.world.biomeDepth--;
			SaveSystem.SaveGame();
			PlayerCamera.main.ToMainMenu();
			LogToConsole("Saved the game and quit to menu.");
		}, null));
		Commands.Add(new Command("resetskills", "Sets all the character skills to zero.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			PlayerCamera.main.body.skills.STR = 0;
			PlayerCamera.main.body.skills.RES = 0;
			PlayerCamera.main.body.skills.INT = 0;
			PlayerCamera.main.body.skills.expSTR = 0f;
			PlayerCamera.main.body.skills.expRES = 0f;
			PlayerCamera.main.body.skills.expINT = 0f;
			PlayerCamera.main.body.skills.UpdateExpBoundaries();
			LogToConsole("Reset all player skills.");
		}, null));
		Commands.Add(new Command("timescale", "Sets the timescale to a desired value. Note that you cannot set the timescale to 0.", (string[] args) =>
		{
			CheckForCheats();
			CheckArgumentCount(args, 1);
			CheckForWorld();
			Time.timeScale = ParseFloat(args[1]);
			LogToConsole("Set timescale to " + args[1] + ".");
		}, null, ("float", "The speed to set the timescale to.")));
		Commands.Add(new Command("setconsoleheight", "Sets the console height to a desired value.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);
			float num = ParseFloat(args[1]);
			if (num < 100f)
			{
				num = 100f;
			}
			if (num > (float)Screen.height)
			{
				num = Screen.height;
			}
			settings.consoleHeight = num;
			ApplySettings();
			LogToConsole("Console height set to " + args[1] + ".");
			WriteConsoleSettings();
		}, null, ("float height", "(Default: 300) the height of the console log window")));
		Commands.Add(new Command("setconsolecolor", "Sets the color of one of the console's elements.", (string[] args) =>
		{
			CheckArgumentCount(args, 2);
			Color color = ParseColor(args[2]);
			string text = args[1];
			if (!(text == "text"))
			{
				if (!(text == "background"))
				{
					throw new Exception("\"" + args[1] + "\" is not a valid console element!");
				}
				settings.backgroundColor = color;
				LogToConsole("Console background color set to " + args[2] + ".");
				WriteConsoleSettings();
			}
			else
			{
				settings.textColor = color;
				LogToConsole("Console text color set to " + args[2] + ".");
				WriteConsoleSettings();
			}
			ApplySettings();
		}, new Dictionary<int, List<string>> { 
		{
			0,
			new List<string> { "text", "background" }
		} }, ("element", "Which part of the console to color"), ("color", "Hex color code")));
		Commands.Add(new Command("copylog", "Copies the entire console log into the clipboard.", (string[] args) =>
		{
			GUIUtility.systemCopyBuffer = new Regex("<[^>]*>").Replace(string.Join("\n", logs), string.Empty);
			LogToConsole("Copied console log to clipboard");
		}, null));
		Commands.Add(new Command("clear", "Clears the console log.", (string[] args) =>
		{
			logs = new List<string>();
			logText.text = "";
		}, null));
		Commands.Add(new Command("addxp", "Give the character experience in a selected skill.", (string[] args) =>
		{
			CheckForCheats();
			CheckArgumentCount(args, 2);
			CheckForWorld();
			float num = Mathf.Max(ParseFloat(args[2]), 0f);
			switch (args[1])
			{
			case "str":
				PlayerCamera.main.body.skills.AddExp(0, num);
				LogToConsole($"Gave the player {Mathf.RoundToInt(num)} strength experience.");
				break;
			case "res":
				PlayerCamera.main.body.skills.AddExp(1, num);
				LogToConsole($"Gave the player {Mathf.RoundToInt(num)} resilience experience.");
				break;
			case "int":
				PlayerCamera.main.body.skills.AddExp(2, num);
				LogToConsole($"Gave the player {Mathf.RoundToInt(num)} intelligence experience.");
				break;
			default:
				throw new Exception("\"" + args[1] + "\" is not a valid skill type!");
			}
		}, new Dictionary<int, List<string>> { 
		{
			0,
			new List<string> { "str", "res", "int" }
		} }, ("type", "The skill to affect"), ("float experience", "How much experience to give")));
		Commands.Add(new Command("applylayermodifier", "Applies a specific modifier for the layer.", (string[] args) =>
		{
			CheckForCheats();
			CheckArgumentCount(args, 1);
			CheckForWorld();
			int num = ParseInt(args[1]);
			WorldGeneration.world.ApplySpecificModifier(num);
			LogToConsole("Applied layer modifier \"" + LayerModifier.availableModifiers[num].ToString() + "\".");
		}, null, ("int index", "Index of the modifier to apply")));
		Commands.Add(new Command("loglocale", "Gets a locale string and logs it to the console.", (string[] args) =>
		{
			CheckArgumentCount(args, 2);
			switch (args[1])
			{
			case "item":
				LogToConsole("\"" + Locale.GetItem(args[2]) + "\"");
				break;
			case "building":
				LogToConsole("\"" + Locale.GetBuilding(args[2]) + "\"");
				break;
			case "moodle":
				LogToConsole("\"" + Locale.GetMoodle(args[2]) + "\"");
				break;
			case "other":
				LogToConsole("\"" + Locale.GetOther(args[2]) + "\"");
				break;
			case "character":
				LogToConsole("\"" + Locale.GetCharacter(args[2]).PickRandom() + "\"");
				break;
			case "note":
			{
				(string, string, string) note = Locale.GetNote(ParseInt(args[2]));
				LogToConsole("text: \"" + note.Item1 + "\"\nsprite: \"" + note.Item2 + "\"\n font: \"" + note.Item3 + "\"");
				break;
			}
			default:
				throw new Exception("\"" + args[1] + "\" is not a valid locale section!");
			}
		}, new Dictionary<int, List<string>> { 
		{
			0,
			new List<string> { "item", "building", "moodle", "other", "character", "note" }
		} }, ("string type", "The locale file section to look in"), ("string key", "The locale key to look for")));
		Commands.Add(new Command("nukeplayerprefs", "Deletes PlayerPrefs settings. This will delete all progress!", (string[] args) =>
		{
			CheckForCheats();
			if (args.Length > 1)
			{
				PlayerPrefs.DeleteKey(args[1]);
				LogToConsole("Deleted PlayerPrefs key \"" + args[1] + "\".");
			}
			else
			{
				PlayerPrefs.DeleteAll();
				LogToConsole("Deleted all PlayerPrefs keys.");
			}
		}, null, ("string key", "If provided a key, only delete that key. Otherwise delete all PlayerPrefs.")));
		Commands.Add(new Command("openfolder", "Opens one of the game's data folders.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);
			switch (args[1])
			{
			case "saves":
				PreRunScript.OpenInFileBrowser(Application.persistentDataPath);
				LogToConsole("Opened saves folder.");
				break;
			case "locale":
				PreRunScript.OpenInFileBrowser(Application.dataPath + "\\Lang");
				LogToConsole("Opened locale folder.");
				break;
			case "music":
				PreRunScript.OpenInFileBrowser(Application.dataPath + "\\custommusic");
				LogToConsole("Opened custom music folder.");
				break;
			default:
				throw new Exception("\"" + args[1] + "\" is not a valid folder!");
			}
		}, new Dictionary<int, List<string>> { 
		{
			0,
			new List<string> { "saves", "locale", "music" }
		} }, ("string type", "Which folder type to go to")));
		Commands.Add(new Command("setbodyfield", "Sets a variable in the player body.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			CheckArgumentCount(args, 2);
			string text = args[1];
			FieldInfo field = typeof(Body).GetField(text);
			object obj = TypeDescriptor.GetConverter(field.FieldType).ConvertFromInvariantString(args[2]);
			field.SetValue(PlayerCamera.main.body, obj);
			LogToConsole("Set player body field \"" + text + "\" to \"" + obj.ToString() + "\".");
		}, null, ("string field", "The field you want to set"), ("object value", "Value to set the field to")));
		Commands.Add(new Command("setlimbfield", "Sets a variable in either of the player's limbs.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			CheckArgumentCount(args, 3);
			Limb limb = PlayerCamera.main.body.LimbByName(args[1]);
			if (!limb)
			{
				throw new Exception("\"" + args[1] + "\" is not a valid limb!");
			}
			string text = args[2];
			FieldInfo field = typeof(Limb).GetField(text);
			object obj = TypeDescriptor.GetConverter(field.FieldType).ConvertFromInvariantString(args[3]);
			field.SetValue(limb, obj);
			LogToConsole("Set \"" + limb.fullName + "\" field \"" + text + "\" to \"" + obj.ToString() + "\".");
		}, null, ("string limb", "Name of your desired limb"), ("string field", "The field you want to set"), ("object value", "Value to set the field to")));
		Commands.Add(new Command("amputate", "Instantly dismembers a limb.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			CheckArgumentCount(args, 1);
			Limb limb = PlayerCamera.main.body.LimbByName(args[1]);
			if (!limb)
			{
				throw new Exception("\"" + args[1] + "\" is not a valid limb!");
			}
			if (limb.isAbdomen || limb.isHead || limb.isVital)
			{
				throw new Exception("\"" + args[1] + "\" is not an extremity and cannot be amputated!");
			}
			if (limb.dismembered)
			{
				throw new Exception(args[1] + " is already dismembered!");
			}
			limb.Dismember();
			LogToConsole("Dismembered \"" + limb.fullName + "\".");
		}, null, ("string limb", "Name of your desired limb")));
		Commands.Add(new Command("reattach", "Reattaches a dismembered limb.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			CheckArgumentCount(args, 1);
			Limb limb = PlayerCamera.main.body.LimbByName(args[1]);
			if (!limb)
			{
				throw new Exception("\"" + args[1] + "\" is not a valid limb!");
			}
			if (!limb.dismembered)
			{
				throw new Exception(args[1] + " is not dismembered!");
			}
			limb.Reattach();
			LogToConsole("Reattached \"" + limb.fullName + "\".");
		}, null, ("string limb", "Name of your desired limb")));
		Commands.Add(new Command("unchipped", "Sets unchipped mode on/off.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			CheckArgumentCount(args, 1);
			bool unchipped = ParseBool(args[1]);
			WorldGeneration.world.SetUnchipped(unchipped);
			LogToConsole("Set unchipped to " + unchipped + ".");
		}, null, ("bool", "Enable unchipped?")));
		Commands.Add(new Command("addcustomcommand", "Adds a custom command to the list.", (string[] args) =>
		{
			CheckArgumentCount(args, 3);
			if (SearchExact(args[1]) != null)
			{
				throw new Exception("Command named \"" + args[1] + "\" already exists!");
			}
			AddCustomCommand(args.Skip(1).ToArray());
			settings.customCommands.Add(string.Join(' ', args.Skip(1).ToArray()));
			LogToConsole("Added command \"" + args[1] + "\"");
			WriteConsoleSettings();
		}, null, ("string name", "Name of the command, no spaces"), ("string description", "Description of the command, spaces replaced by _"), ("string action", "Commands to run, separated by ; and spaces replaced by _")));
		Commands.Add(new Command("addliquid", "Adds a specified amount of liquid to the item held in your main hand.", (string[] args) =>
		{
			CheckForCheats();
			CheckArgumentCount(args, 2);
			CheckForWorld();
			Item item = PlayerCamera.main.body.GetItem(0);
			if (!item)
			{
				throw new Exception("Not holding an item in the main hand!");
			}
			WaterContainerItem component = item.GetComponent<WaterContainerItem>();
			if (!component)
			{
				throw new Exception("\"" + item.fullName + "\" is not a liquid container!");
			}
			string text = args[1];
			float num = ParseFloat(args[2]);
			if (!Liquids.LiquidExists(text))
			{
				throw new Exception("\"" + text + "\" is not a valid liquid id!");
			}
			component.AddLiquid(text, num);
			LogToConsole($"Added {num}mL of {text} to {item.fullName} ({component.CurrentTotal}mL/{component.Capacity}mL).");
		}, new Dictionary<int, List<string>> { 
		{
			0,
			Liquids.Registry.Select((KeyValuePair<string, LiquidType> kvp) => kvp.Key).ToList()
		} }, ("string id", "ID of the liquid to add"), ("float amount", "mL amount of the liquid to add")));
		Commands.Add(new Command("locate", "Searches for any objects with a specified name and teleports you to one.", (string[] args) =>
		{
			CheckForCheats();
			CheckArgumentCount(args, 1);
			CheckForWorld();
			string text = args[1];
			int val = ((args.Length > 2) ? ParseInt(args[2]) : 0);
			List<GameObject> list = FindObjectsWithName(text);
			if (list.Count == 0)
			{
				LogToConsole("Couldn't find any objects named \"" + text + "\"");
			}
			else
			{
				GameObject gameObject = list[Math.Min(val, list.Count - 1)];
				PlayerCamera.main.body.transform.position = gameObject.transform.position + Vector3.up * 3f;
				PlayerCamera.main.transform.position = gameObject.transform.position;
				noClipPos = gameObject.transform.position;
				LogToConsole(string.Format("Found {0} object{1} named \"{2}\". Teleported to \"{3}\", index {4}.", list.Count, (list.Count > 1) ? "s" : "", text, gameObject.name, Math.Min(val, list.Count - 1)));
			}
		}, null, ("string id", "Name of object to search for. Object name has to contain this string to be found"), ("int index", "Index of the object to teleport to")));
		Commands.Add(new Command("removecustomcommand", "Removes an added custom command from the list.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);
			Command command = SearchExact(args[1]);
			if (command == null)
			{
				throw new Exception("Couldn't find command \"" + args[1] + "\"!");
			}
			if (!command.custom)
			{
				throw new Exception("Command \"" + args[1] + "\" is not a custom command!");
			}
			string commName = command.name;
			Commands.Remove(command);
			int index = settings.customCommands.FindIndex((string x) => x.Split(' ')[0] == commName);
			settings.customCommands.RemoveAt(index);
			LogToConsole("Removed command \"" + commName + "\"");
			WriteConsoleSettings();
		}, null, ("string name", "Name of the command to remove")));
		Commands.Add(new Command("music", "Manages background music. Can play a new track, skip time in the current track or open the MP3 menu.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);
			CheckForWorld();
			switch (args[1])
			{
			case "play":
				MusicManager.main.timeSinceFinishedPlaying = 9999f;
				LogToConsole("Playing new biome track.");
				break;
			case "skiptime":
			{
				CheckArgumentCount(args, 2);
				float num = ParseFloat(args[2]);
				MusicManager.main.SkipTime(num);
				LogToConsole($"Skipped playing track by {num} seconds.");
				break;
			}
			case "menu":
				Utils.Create("Special/MP3SongSelect", PlayerCamera.main.mainCanvas.transform);
				LogToConsole("Opened music menu.");
				break;
			default:
				throw new Exception("\"" + args[1] + "\" is not a valid music action!");
			}
		}, new Dictionary<int, List<string>> { 
		{
			0,
			new List<string> { "play", "skiptime", "menu" }
		} }, ("string action", "play to play a new track, skiptime to skip time in the current track, menu to open the MP3 menu"), ("float time", "(if skipping time) How much time to skip by")));
		Commands.Add(new Command("bind", "Manages custom command binds, by either adding/removing/listing them.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);
			switch (args[1])
			{
			case "add":
			{
				CheckArgumentCount(args, 3);
				KeyValuePair<KeyCode, string> keyValuePair = new KeyValuePair<KeyCode, string>(ParseKeycode(args[2]), args[3]);
				customBinds.Add(keyValuePair.Key, keyValuePair.Value);
				settings.customBinds.Add(keyValuePair.Key.ToString(), keyValuePair.Value);
				LogToConsole("Bound \"" + args[2] + "\" to \"" + args[3] + "\".");
				WriteConsoleSettings();
				break;
			}
			case "clear":
				customBinds.Clear();
				settings.customBinds.Clear();
				LogToConsole("Cleared all custom binds.");
				WriteConsoleSettings();
				break;
			case "list":
				if (customBinds.Count == 0)
				{
					LogToConsole("No command binds active.");
				}
				else
				{
					List<string> list = new List<string>();
					foreach (KeyValuePair<KeyCode, string> customBind in customBinds)
					{
						list.Add($"\"{customBind.Key}\" bound to \"{customBind.Value}\"");
					}
					LogToConsole(string.Join("\n", list));
				}
				break;
			default:
				throw new Exception("\"" + args[1] + "\" is not a valid bind action!");
			}
		}, new Dictionary<int, List<string>>
		{
			{
				0,
				new List<string> { "add", "clear", "list" }
			},
			{
				1,
				Enum.GetNames(typeof(KeyCode)).ToList()
			}
		}, ("string action", "add to add new binds, clear to clear all binds and list to list all binds"), ("keycode", "(if adding) Keyboard button to bind"), ("string action", "(if adding) Commands to run, separated by ; and spaces replaced by _")));
		Commands.Add(new Command("repeat", "Runs a specified series of commands a specified amount of times with a delay.", (string[] args) =>
		{
			CheckArgumentCount(args, 3);
			StartCoroutine("RepeatCommand", new RepeatCommandArgs
			{
				times = ParseInt(args[1]),
				repeatTime = ParseFloat(args[2]),
				commands = args[3]
			});
		}, null, ("int times", "How many times to repeat the command"), ("float delay", "How long to wait between each repeat, in seconds"), ("string command", "Commands to run, separated by ; and spaces replaced by _")));
		Commands.Add(new Command("explode", "Explodes at a given position with optional parameters.", (string[] args) =>
		{
			CheckForCheats();
			CheckArgumentCount(args, 1);
			CheckForWorld();
			ExplosionParams explosionParams = new ExplosionParams
			{
				position = ParsePosition(args[1])
			};
			if (args.Length > 2)
			{
				explosionParams.muscleDamage = ParseRangeF(args[2]);
			}
			if (args.Length > 3)
			{
				explosionParams.skinDamage = ParseRangeF(args[3]);
			}
			if (args.Length > 4)
			{
				explosionParams.skinDamageChance = ParseFloat(args[4]);
			}
			if (args.Length > 5)
			{
				explosionParams.boneBreakChance = ParseFloat(args[5]);
			}
			if (args.Length > 6)
			{
				explosionParams.dislocationChance = ParseFloat(args[6]);
			}
			if (args.Length > 7)
			{
				explosionParams.disfigureChance = ParseFloat(args[7]);
			}
			if (args.Length > 8)
			{
				explosionParams.bleedChance = ParseFloat(args[8]);
			}
			if (args.Length > 9)
			{
				explosionParams.bleedAmount = ParseRangeF(args[9]);
			}
			if (args.Length > 10)
			{
				explosionParams.structuralDamage = ParseFloat(args[10]);
			}
			if (args.Length > 11)
			{
				explosionParams.range = ParseFloat(args[11]);
			}
			if (args.Length > 12)
			{
				explosionParams.velocity = ParseFloat(args[12]);
			}
			if (args.Length > 13)
			{
				explosionParams.shrapnelChance = ParseFloat(args[13]);
			}
			if (args.Length > 14)
			{
				explosionParams.sound = args[14];
			}
			WorldGeneration.CreateExplosion(explosionParams);
			LogToConsole($"Created explosion at {explosionParams.position}.");
		}, null, ("position", "Where to make the explosion"), ("range muscledmg", "Limb muscle damage range (0-60)"), ("range skindmg", "Limb skin damage range (0-75)"), ("float skinchance", "Chance to do skin damage (0.2)"), ("float breakchance", "Chance to fracture (0.06)"), ("float dislchance", "Chance to dislocate (0.135)"), ("float disfchance", "Chance to disfigure (0.34)"), ("float bleedchance", "Chance to bleed (0.15)"), ("range bleed", "Bleeding range (4-30)"), ("float structuredmg", "Damage to deal to structures (500)"), ("float range", "Explosion range (12)"), ("float velocity", "How hard to send things flying away (60)"), ("float shrapnelchance", "Chance to inflict shrapnel (0.5)"), ("string sound", "Sound to play (explosion)")));
		Commands.Add(new Command("floodfill", "Fills a specified position with a specified liquid. Won't work if you try to fill more than 2048 tiles at once.", (string[] args) =>
		{
			CheckForCheats();
			CheckArgumentCount(args, 2);
			CheckForWorld();
			if ((byte)ParseInt(args[2]) >= FluidManager.main.liquidColors.Length)
			{
				throw new Exception(args[2] + " is not a valid liquid type!");
			}
			FluidManager.main.StartFill(WorldGeneration.world.WorldToBlockPos(ParsePosition(args[1])), (byte)ParseInt(args[2]), 2048);
			LogToConsole($"Floodfilled {args[2]} at {ParsePosition(args[1])}.");
		}, null, ("position", "Where to floodfill"), ("byte liquidtype", "What type of liquid to fill the position with")));
		Commands.Add(new Command("spawncharacter", "Spawns a chosen character.", (string[] args) =>
		{
			CheckForCheats();
			CheckArgumentCount(args, 2);
			CheckForWorld();
			Vector2 vector = ParsePosition(args[2]);
			GameObject gameObject = PlayerCamera.main.SpawnCharacter(ParseInt(args[1]));
			gameObject.transform.position = vector;
			LogToConsole($"Created \"{gameObject.name}\" at {vector}.");
		}, null, ("int character", "Index of the character"), ("position", "Where to spawn the character")));
		Commands.Add(new Command("echo", "Toggles all new command logs on/off.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);
			bool flag = ParseBool(args[1]);
			echo = flag;
			if (echo)
			{
				LogToConsole("Echo enabled.");
			}
		}, null, ("bool", "Enable logging?")));
		Commands.Add(new Command("ui", "Toggles the main GUI.", (string[] args) =>
		{
			CheckForWorld();
			PlayerCamera.main.mainCanvas.gameObject.SetActive(!PlayerCamera.main.mainCanvas.gameObject.activeSelf);
			LogToConsole("Toggled UI.");
		}, null));
		Commands.Add(new Command("freecam", "Makes the camera controlled separately via arrow keys.", (string[] args) =>
		{
			CheckForWorld();
			PlayerCamera.main.isFreecam = !PlayerCamera.main.isFreecam;
			if (PlayerCamera.main.isFreecam)
			{
				freecamPos = PlayerCamera.main.transform.position;
			}
			LogToConsole("Toggled freecam. Remember to use the arrow keys!");
		}, null));
		Commands.Add(new Command("starterkit", "Spawns in a semi-randomized kit with basic containers, tools and medicine. Gives XP depending on the current layer.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("primitivediggingtool", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("flashlight", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("duffelbag", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("trashbag", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("medkit", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("legpouch", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("bikehelmet", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("materialpouch", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("bandage", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("sterilizedbandage", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("morphine", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("manualdefibrillator", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("lrd", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("tourniquet", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("tweezers", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("disinfectant", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("limbwraps", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("smallbattery", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create("mediumbattery", PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
			PlayerCamera.main.body.skills.AddExp(0, (float)WorldGeneration.world.biomeDepth * 140f);
			PlayerCamera.main.body.skills.AddExp(1, (float)WorldGeneration.world.biomeDepth * 150f);
			PlayerCamera.main.body.skills.AddExp(2, (float)WorldGeneration.world.biomeDepth * 110f);
			for (int i = 0; i < 3; i++)
			{
				SpawnBodyItem("drug");
			}
			for (int j = 0; j < 2; j++)
			{
				SpawnBodyItem("food");
			}
			for (int k = 0; k < 2; k++)
			{
				SpawnBodyItem("water");
			}
			SpawnBodyItem("tool");
			SpawnBodyItem("utility");
		}, null));
		Commands.Add(new Command("noclip", "Toggles no-clip (enables flight and disables player collision).", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			noClip = !noClip;
			noClipPos = PlayerCamera.main.body.transform.position;
			PlayerCamera.main.body.rb.simulated = !noClip;
			LogToConsole((noClip ? "Enabled" : "Disabled") + " noclip.");
		}, null));
		Commands.Add(new Command("playsound", "Plays a sound with a provided ID.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);
			Sound.Play(args[1], Vector2.zero, twoDimensional: true, pitchShift: false);
			LogToConsole("Playing sound \"" + args[1] + "\"");
		}, null, ("string id", "ID of the sound to play")));
		Commands.Add(new Command("fullbright", "Toggles debug lighting.", (string[] args) =>
		{
			CheckForCheats();
			fullBright = !fullBright;
			LogToConsole(fullBright ? "Enabled fullbright." : "Disabled fullbright.");
		}, null));
		Commands.Add(new Command("plushies", "Creates every type of in-game plushie at the player, in a line.", (string[] args) =>
		{
			CheckForCheats();
			CheckForWorld();
			int num = (Resources.Load("plushie") as GameObject).GetComponent<PlushScript>().possibleSprites.Length;
			for (int i = 0; i < num; i++)
			{
				Utils.Create("plushie", PlayerCamera.main.body.transform.position + Vector3.right * i * 3.5f - Vector3.right * ((float)num * 1.75f), 0f).GetComponent<PlushScript>().index = i;
			}
			LogToConsole($"Instantiated {num} different plushies at player.");
		}, null));
		Commands.Add(new Command("dunemode", "we are duned (This is permanent until you close the game!)", (string[] args) =>
		{
			CheckForCheats();
			if ((bool)DuneMode.dune)
			{
				LogToConsole("The dunepocalypse is already ongoing.");
			}
			else
			{
				new GameObject("Dune", typeof(DuneMode));
				LogToConsole("The dunepocalypse has begun. This is irreversible until the program is restarted.");
			}
		}, null));
		Commands.Add(new Command("errorlogging", "Toggles logging application errors on/off.", (string[] args) =>
		{
			logErrors = !logErrors;
			LogToConsole(logErrors ? "Enabled error logging." : "Disabled error logging.");
		}, null));
		if (Application.isEditor)
		{
			Commands.Add(new Command("setachievement", "Unlocks achievement.", (string[] args) =>
			{
				SteamManager.TryUnlockAchievement(args[1]);
				LogToConsole("Tried to unlock achievement \"" + args[1] + "\"");
			}, null, ("achievement", "achievement ID")));
		}
		static void SpawnBodyItem(string cat)
		{
			PlayerCamera.main.body.AutoPickUpItem(Utils.Create(ItemLootPool.pool[cat].PickRandom(), PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>());
		}
	}

	private void AddCustomCommand(string[] args)
	{
		string commandString = args[2];
		Commands.Add(new Command(args[0].Replace(' ', '.'), args[1].Replace('_', ' '), (string[] array) =>
		{
			RunCommandString(commandString);
		}, null)
		{
			custom = true
		});
	}

	private void RunCommandString(string commandstr)
	{
		commandstr = commandstr.Replace('_', ' ');
		string[] array = commandstr.Split(';');
		foreach (string text in array)
		{
			TryExecuteCommand(text.Split(' ', StringSplitOptions.RemoveEmptyEntries), addToLog: false);
		}
	}

	public void RegisterPlayerDetails()
	{
		if (!playerDetailsRegistered)
		{
			playerDetailsRegistered = true;
			Command command = SearchExact("setbodyfield");
			FieldInfo[] fields = typeof(Body).GetFields(BindingFlags.Instance | BindingFlags.Public);
			fields = fields.Where((FieldInfo f) => f.FieldType.IsPrimitive).ToArray();
			command.argAutofill = new Dictionary<int, List<string>>();
			command.argAutofill.Add(0, fields.Select((FieldInfo f) => f.Name).ToList());
			Command command2 = SearchExact("setlimbfield");
			fields = typeof(Limb).GetFields(BindingFlags.Instance | BindingFlags.Public);
			fields = fields.Where((FieldInfo f) => f.FieldType.IsPrimitive).ToArray();
			command2.argAutofill = new Dictionary<int, List<string>>();
			command2.argAutofill.Add(1, fields.Select((FieldInfo f) => f.Name).ToList());
			command2.argAutofill.Add(0, PlayerCamera.main.body.limbs.Select((Limb l) => l.name).ToList());
			SearchExact("spawncategory").argAutofill.Add(0, ItemLootPool.pool.Select((KeyValuePair<string, List<string>> x) => x.Key).ToList());
			SearchExact("playsound").argAutofill = new Dictionary<int, List<string>> { 
			{
				0,
				(from x in Resources.LoadAll<AudioClip>("Sounds")
					select x.name).ToList()
			} };
			Command command3 = SearchExact("amputate");
			command3.argAutofill = new Dictionary<int, List<string>>();
			command3.argAutofill.Add(0, PlayerCamera.main.body.limbs.Select((Limb l) => l.name).ToList());
			Command command4 = SearchExact("reattach");
			command4.argAutofill = new Dictionary<int, List<string>>();
			command4.argAutofill.Add(0, PlayerCamera.main.body.limbs.Select((Limb l) => l.name).ToList());
		}
	}

	private void CheckArgumentCount(string[] args, int desired)
	{
		if (args.Length <= desired)
		{
			throw new Exception(string.Format("Expected at least {0} argument{1}, but got {2}.", desired, (desired > 1) ? "s" : "", args.Length - 1));
		}
	}

	private void CheckForWorld()
	{
		if (!PlayerCamera.main)
		{
			throw new Exception("No world is loaded. Try starting a game?");
		}
	}

	private void CheckForCheats()
	{
		if ((!WorldGeneration.world || WorldGeneration.world.biomeOverride != WorldGeneration.OverrideSceneType.Tutorial) && !SteamManager.disableAchievements)
		{
			SteamManager.disableAchievements = true;
			Alert("<color=#ff006a>WARNING: You used a cheat command. Achievements are disabled until you restart the game.</color>");
		}
	}

	private int ParseInt(string s)
	{
		if (!int.TryParse(s, out var result))
		{
			throw new Exception("\"" + s + "\" is not a valid integer value! (0, 1, 5, etc)");
		}
		return result;
	}

	private float ParseFloat(string s)
	{
		if (!float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
		{
			throw new Exception("\"" + s + "\" is not a valid float value! (2, 0.7, 14.1, etc)");
		}
		return result;
	}

	private bool ParseBool(string s)
	{
		if (!bool.TryParse(s, out var result))
		{
			throw new Exception("\"" + s + "\" is not a valid boolean value! (true/false)");
		}
		return result;
	}

	private Color ParseColor(string s)
	{
		if (!ColorUtility.TryParseHtmlString(s, out var color))
		{
			throw new Exception("\"" + s + "\" is not a valid color value! (#FFFFFF, etc)");
		}
		return color;
	}

	private KeyCode ParseKeycode(string s)
	{
		if (!Enum.TryParse<KeyCode>(s, out var result))
		{
			throw new Exception("\"" + s + "\" is not a valid keycode!");
		}
		return result;
	}

	private Vector2 ParsePosition(string s)
	{
		switch (s)
		{
		case "cursor":
			return Camera.main.ScreenToWorldPoint(Input.mousePosition);
		case "player":
			return PlayerCamera.main.body.transform.position;
		case "random":
			return new Vector2(UnityEngine.Random.Range(-1f, 1f) * (float)WorldGeneration.world.halfWidth, UnityEngine.Random.Range(-1f, 1f) * (float)WorldGeneration.world.halfHeight);
		default:
		{
			string[] array = s.Split(',');
			if (array.Length == 2)
			{
				return new Vector2(ParseFloat(array[0]), ParseFloat(array[1]));
			}
			throw new Exception("\"" + s + "\" is not a valid position! (cursor/player/random/#,#)");
		}
		}
	}

	private RangeF ParseRangeF(string s)
	{
		string[] array = s.Split('-');
		if (array.Length == 2)
		{
			return new RangeF(ParseFloat(array[0]), ParseFloat(array[1]));
		}
		throw new Exception("\"" + s + "\" is not a valid range! (#-#)");
	}

	public static List<GameObject> FindObjectsWithName(string namePart)
	{
		List<GameObject> list = new List<GameObject>();
		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>(includeInactive: true);
		foreach (GameObject gameObject in array)
		{
			if (gameObject.name.Contains(namePart))
			{
				list.Add(gameObject);
			}
		}
		return list;
	}

	public void Alert(string message)
	{
		if (!active)
		{
			ToggleActiveState();
		}
		ExecuteCommand("log " + message);
	}

	public void ExecuteCommand(string command)
	{
		TryExecuteCommand(command.Split(' ', StringSplitOptions.RemoveEmptyEntries));
	}

	public static void CheckForConsole()
	{
		if (!instance)
		{
			UnityEngine.Object.Instantiate(Resources.Load("Special/Console"), Vector2.zero, Quaternion.identity);
		}
	}

	private IEnumerator RepeatCommand(RepeatCommandArgs args)
	{
		for (int i = 0; i < args.times; i++)
		{
			RunCommandString(args.commands);
			yield return new WaitForSecondsRealtime(args.repeatTime);
		}
	}
}
