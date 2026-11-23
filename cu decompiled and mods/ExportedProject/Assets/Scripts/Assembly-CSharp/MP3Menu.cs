using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class MP3Menu : MonoBehaviour
{
	public TMP_Dropdown dropdown;

	[SerializeField]
	public static List<AudioClip> clips;

	public static List<TMP_Dropdown.OptionData> dropdownList;

	private static bool loadingMusic;

	public void UpdateList()
	{
		if (dropdownList == null)
		{
			dropdownList = new List<TMP_Dropdown.OptionData>();
			clips = Resources.LoadAll<AudioClip>("Sounds/music").ToList();
			for (int i = 0; i < clips.Count; i++)
			{
				string postfix = GetPostfix(i % 8);
				dropdownList.Add(new TMP_Dropdown.OptionData(clips[i].name + postfix));
			}
			string text = Path.Combine(Application.dataPath, "custommusic");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			StartCoroutine(LoadAllMusic(text));
		}
	}

	private IEnumerator LoadAllMusic(string folderPath)
	{
		loadingMusic = true;
		List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();
		string[] files = Directory.GetFiles(folderPath, "*.*");
		string[] array = files;
		foreach (string file in array)
		{
			if (!file.EndsWith(".mp3") && !file.EndsWith(".wav") && !file.EndsWith(".ogg"))
			{
				continue;
			}
			string uri = "file://" + file;
			using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.UNKNOWN);
			yield return www.SendWebRequest();
			if (www.result == UnityWebRequest.Result.Success)
			{
				AudioClip content = DownloadHandlerAudioClip.GetContent(www);
				clips.Add(content);
				newOptions.Add(new TMP_Dropdown.OptionData(Path.GetFileName(file)));
			}
			else
			{
				Debug.LogWarning("Failed to load: " + file);
			}
		}
		dropdown.AddOptions(newOptions);
		dropdownList.AddRange(newOptions);
		loadingMusic = false;
	}

	private void Start()
	{
		dropdown.ClearOptions();
		UpdateList();
		dropdown.AddOptions(dropdownList);
	}

	public void Play()
	{
		if (!loadingMusic)
		{
			MusicManager.main.PlaySong(clips[dropdown.value]);
			Exit();
		}
	}

	public void Exit()
	{
		if (!loadingMusic)
		{
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Close);
			Object.Destroy(base.gameObject);
		}
	}

	public string GetPostfix(int index)
	{
		return index switch
		{
			0 => ".mp3", 
			1 => ".ogg", 
			2 => ".wav", 
			3 => ".flac", 
			4 => ".aac", 
			5 => ".webm", 
			6 => ".3gp", 
			7 => ".m4a", 
			_ => "", 
		};
	}
}
