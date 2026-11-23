using System;

public abstract class Setting
{
	public enum SettingCategory
	{
		Video = 0,
		Audio = 1,
		Game = 2,
		Input = 3,
		Language = 4
	}

	public string name;

	public Action apply;

	public SettingCategory category;

	public abstract object GetValue();

	public abstract void SetValue(object val);

	public virtual void Apply()
	{
		apply?.Invoke();
	}
}
