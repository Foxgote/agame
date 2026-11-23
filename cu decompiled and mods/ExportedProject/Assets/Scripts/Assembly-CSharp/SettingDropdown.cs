public class SettingDropdown : Setting
{
	public int value;

	public string[] choices;

	public override object GetValue()
	{
		return value;
	}

	public override void SetValue(object val)
	{
		value = (int)val;
	}
}
