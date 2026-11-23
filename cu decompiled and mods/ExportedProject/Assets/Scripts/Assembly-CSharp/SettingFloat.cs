using System;

public class SettingFloat : Setting
{
	public float value;

	public float min;

	public float max;

	public Func<float, string> formatValue;

	public override object GetValue()
	{
		return value;
	}

	public override void SetValue(object val)
	{
		value = (float)val;
	}

	public string GetDisplayString()
	{
		if (formatValue != null)
		{
			return formatValue(value);
		}
		return value.ToString("0.##");
	}
}
