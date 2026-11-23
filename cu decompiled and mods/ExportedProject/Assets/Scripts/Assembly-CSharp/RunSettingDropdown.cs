public class RunSettingDropdown : RunSetting
{
	public string[] choices;

	public RunSettingDropdown(string name, string[] choices)
	{
		base.name = name;
		this.choices = choices;
	}
}
