using UnityEngine;

public class SettingKeybind : Setting
{
	public KeyCode value;

	public override object GetValue()
	{
		return value;
	}

	public override void SetValue(object val)
	{
		value = (KeyCode)val;
	}

	public string GetDisplayString()
	{
		if (value != KeyCode.None)
		{
			return value.ToString();
		}
		return Locale.GetOther("keybindnotset");
	}

	public override void Apply()
	{
		KeyBinds.CacheBind(this);
		base.Apply();
	}
}
