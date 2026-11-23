using System;
using System.Collections.Generic;

[Serializable]
public class Language
{
	public string name;

	public string description;

	public string version;

	public Dictionary<string, string> main;

	public Dictionary<string, string> buildings;

	public Dictionary<string, string> moodles;

	public Dictionary<string, string> other;

	public Dictionary<string, List<string>>[] character;

	public List<(string text, string sprite, string font)>[] notes;

	public List<(string text, string sprite)> pdaNotes;

	public Dictionary<string, List<string>> pauseQuotes;

	public Language()
	{
		main = new Dictionary<string, string>();
		buildings = new Dictionary<string, string>();
		moodles = new Dictionary<string, string>();
		other = new Dictionary<string, string>();
		character = new Dictionary<string, List<string>>[1]
		{
			new Dictionary<string, List<string>>()
		};
		notes = new List<(string, string, string)>[0];
		pdaNotes = new List<(string, string)>();
		pauseQuotes = new Dictionary<string, List<string>>();
	}
}
