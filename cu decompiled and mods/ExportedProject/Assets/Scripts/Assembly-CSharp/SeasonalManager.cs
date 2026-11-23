using System;
using UnityEngine;

public class SeasonalManager : MonoBehaviour
{
	public enum SeasonalEventType
	{
		None = 0,
		Halloween = 1,
		Holidays = 2
	}

	public static SeasonalEventType GetSeasonalEvent()
	{
		DateTime now = DateTime.Now;
		if ((now.Month == 10 && now.Day >= 10) || (now.Month == 11 && now.Day <= 1))
		{
			return SeasonalEventType.Halloween;
		}
		if (now.Month == 12 && now.Day >= 15)
		{
			return SeasonalEventType.Holidays;
		}
		return SeasonalEventType.None;
	}
}
