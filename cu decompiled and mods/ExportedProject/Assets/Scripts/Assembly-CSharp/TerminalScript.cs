using UnityEngine;

public class TerminalScript : MonoBehaviour
{
	public void OnUse()
	{
		if (!ConsoleScript.instance.active)
		{
			ConsoleScript.instance.ToggleActiveState();
			ConsoleScript.instance.ExecuteCommand("log Welcome to the console! You can safely use any command in the sandbox. Press (~) to exit.");
		}
	}
}
