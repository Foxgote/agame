public class SettingBool : Setting
{
	public bool value;

	public override object GetValue()
	{
		return value;
	}

	public override void SetValue(object val)
	{
		value = (bool)val;
	}
}
