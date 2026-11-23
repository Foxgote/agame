using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class ConsoleSettings
{
	public float consoleHeight;

	[JsonIgnore]
	public Color backgroundColor;

	[JsonIgnore]
	public Color textColor;

	public string hexBackgroundColor;

	public string hexTextColor;

	public List<string> customCommands;

	public Dictionary<string, string> customBinds;

	public static ConsoleSettings defaultSettings => new ConsoleSettings
	{
		consoleHeight = 300f,
		backgroundColor = Color.black,
		textColor = Color.white,
		customCommands = new List<string>(),
		customBinds = new Dictionary<string, string>()
	};
}
