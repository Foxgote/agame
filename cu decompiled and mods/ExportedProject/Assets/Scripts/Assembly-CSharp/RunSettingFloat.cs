public class RunSettingFloat : RunSetting
{
	public RangeF limits;

	public bool wholeNum;

	public string postfix;

	public RunSettingFloat(string name)
	{
		base.name = name;
	}
}
