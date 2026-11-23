public class SettingInt : Setting
{
	public int value;

	public int min;

	public int max;

	public override object GetValue()
	{
		return value;
	}

	public override void SetValue(object val)
	{
		value = (int)val;
	}
}
