using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
	public static SteamManager instance;

	private bool initialized;

	private readonly HashSet<string> unlockedAchievements = new HashSet<string>();

	public static bool disableAchievements;

	public static void CheckInstance()
	{
		if (!instance)
		{
			new GameObject("SteamManager", typeof(SteamManager));
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		instance = this;
		if (Application.isEditor)
		{
			return;
		}
		initialized = SteamAPI.Init();
		if (!initialized)
		{
			Debug.LogError("Failed to initialize Steam!");
			return;
		}
		if (HasAchievement("BIOPOD"))
		{
			PlayerPrefs.SetInt("didbiopod", 1);
		}
		if (HasAchievement("COMPANYSWORST"))
		{
			PlayerPrefs.SetInt("didescape", 1);
		}
		if (HasAchievement("TOBECONTINUED"))
		{
			PlayerPrefs.SetInt("didmerc", 1);
		}
	}

	private void Update()
	{
		if (initialized)
		{
			SteamAPI.RunCallbacks();
		}
	}

	public static void TryUnlockAchievement(string type, bool force = false)
	{
		if ((bool)instance && (force || !WorldGeneration.world || WorldGeneration.world.biomeOverride != WorldGeneration.OverrideSceneType.Tutorial) && !disableAchievements)
		{
			instance.UnlockAchievement(type);
		}
	}

	private void UnlockAchievement(string type)
	{
		if (!unlockedAchievements.Contains(type))
		{
			if (!initialized)
			{
				Debug.Log("Tried to unlock achievement " + type + ", but Steam is not initialized.");
			}
			else if (SteamUserStats.SetAchievement(type))
			{
				SteamUserStats.StoreStats();
				unlockedAchievements.Add(type);
			}
		}
	}

	public bool HasAchievement(string name)
	{
		if (!SteamUserStats.GetAchievement(name, out var pbAchieved))
		{
			return false;
		}
		return pbAchieved;
	}

	private void OnDestroy()
	{
		if (initialized)
		{
			SteamAPI.Shutdown();
		}
	}
}
