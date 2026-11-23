using UnityEngine;

public class Units : MonoBehaviour
{
	public static float ConvertTemperature(float temp)
	{
		return Settings.Get<SettingDropdown>("units").value switch
		{
			0 => temp, 
			1 => temp * 1.8f + 32f, 
			2 => temp * 1.8f + 491.67f, 
			_ => temp, 
		};
	}

	public static string TemperatureUnit(bool lower = false)
	{
		string text = "";
		switch (Settings.Get<SettingDropdown>("units").value)
		{
		case 0:
			text = "'C";
			break;
		case 1:
			text = "'F";
			break;
		case 2:
			text = "'R";
			break;
		}
		if (lower)
		{
			text = text.ToLower();
		}
		return text;
	}

	public static float ConvertDistance(float distance)
	{
		return Settings.Get<SettingDropdown>("units").value switch
		{
			0 => distance, 
			1 => distance * 3.281f, 
			2 => distance / 0.0508f, 
			_ => distance, 
		};
	}

	public static string DistanceUnit(bool lower = false)
	{
		string text = "";
		switch (Settings.Get<SettingDropdown>("units").value)
		{
		case 0:
			text = "m";
			break;
		case 1:
			text = "'";
			break;
		case 2:
			text = "brg";
			break;
		}
		if (lower)
		{
			text = text.ToLower();
		}
		return text;
	}

	public static float ConvertWeight(float weight)
	{
		return Settings.Get<SettingDropdown>("units").value switch
		{
			0 => weight, 
			1 => weight * 2.2046f, 
			2 => weight / 0.15f, 
			_ => weight, 
		};
	}

	public static string WeightUnit(bool lower = false)
	{
		string text = "";
		switch (Settings.Get<SettingDropdown>("units").value)
		{
		case 0:
			text = "kg";
			break;
		case 1:
			text = "lb";
			break;
		case 2:
			text = "brg";
			break;
		}
		if (lower)
		{
			text = text.ToLower();
		}
		return text;
	}
}
