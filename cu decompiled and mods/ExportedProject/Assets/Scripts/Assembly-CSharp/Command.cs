using System.Collections.Generic;
using System.Linq;

public class Command
{
	public delegate void Action(string[] args);

	public string name;

	public string description;

	public (string shortDesc, string longDesc)[] argDescription;

	public Dictionary<int, List<string>> argAutofill;

	public Action action;

	public bool custom;

	public string GetShortDescription(bool full = false)
	{
		string text = name;
		if (argDescription.Length != 0)
		{
			text = text + " [" + string.Join("] [", argDescription.Select(((string shortDesc, string longDesc) c) => c.shortDesc).ToList()) + "]";
		}
		if (full)
		{
			text = text + " - " + description;
		}
		return text;
	}

	public Command(string name, string description, Action action, Dictionary<int, List<string>> argAutofill, params (string, string)[] argDescription)
	{
		this.name = name;
		this.description = description;
		this.argDescription = argDescription ?? new (string, string)[0];
		this.action = action;
		this.argAutofill = argAutofill;
		for (int i = 0; i < this.argDescription.Length; i++)
		{
			if (this.argDescription[i].shortDesc.StartsWith("bool"))
			{
				if (this.argAutofill == null)
				{
					this.argAutofill = new Dictionary<int, List<string>>();
				}
				this.argAutofill.Add(i, new List<string> { "true", "false" });
			}
			if (this.argDescription[i].shortDesc.StartsWith("position"))
			{
				if (this.argAutofill == null)
				{
					this.argAutofill = new Dictionary<int, List<string>>();
				}
				this.argAutofill.Add(i, new List<string> { "cursor", "player", "random", "#.#" });
			}
		}
	}
}
