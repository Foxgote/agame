using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Saveable]
[JsonObject(MemberSerialization.OptIn)]
public class EPdaScript : MonoBehaviour
{
	public Sprite onSprite;

	public Sprite offSprite;

	public SpriteRenderer light;

	private SpriteRenderer mySprite;

	private Item item;

	[JsonProperty]
	public int savedIndex = -1;

	[JsonProperty]
	public bool hasBeenRead;

	private void Start()
	{
		item = GetComponent<Item>();
		mySprite = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		if (item.condition <= 0f)
		{
			light.enabled = false;
			mySprite.sprite = offSprite;
		}
		else
		{
			light.enabled = true;
			mySprite.sprite = onSprite;
		}
	}

	public string GetText()
	{
		if (savedIndex >= 0)
		{
			return Locale.GetPdaNote(savedIndex).text;
		}
		if (PlayerPrefs.GetInt("pdaSeed") == 0)
		{
			PlayerPrefs.SetInt("pdaSeed", Random.Range(1, 100000));
		}
		List<(string, string)> list = new List<(string, string)>(Locale.currentLang.pdaNotes);
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(i);
		}
		list2.Shuffle(PlayerPrefs.GetInt("pdaSeed"));
		int num = PlayerPrefs.GetInt("pdaIndex", 0);
		if (num >= list2.Count)
		{
			PlayerPrefs.SetInt("pdaSeed", Random.Range(1, 100000));
			PlayerPrefs.SetInt("pdaIndex", 0);
			PlayerPrefs.Save();
			return GetText();
		}
		savedIndex = list2[num];
		PlayerPrefs.SetInt("pdaIndex", num + 1);
		PlayerPrefs.Save();
		return Locale.GetPdaNote(savedIndex).text;
	}

	public void Use()
	{
		if (PlayerCamera.main.body.canRead)
		{
			bool flag = hasBeenRead;
			if (!hasBeenRead)
			{
				hasBeenRead = true;
				PlayerCamera.main.body.skills.AddExp(2, 35f);
			}
			string text = GetText();
			PlayerCamera.main.SetTimeScale(PlayerCamera.SpeedType.Slowmo, switchSound: false);
			ScrollableText.CreateText(text);
			SteamManager.TryUnlockAchievement("READINGCOMPREHENSION");
			StartCoroutine(SurvivorNote.ChangeTimeScaleWhenFinishedReading(flag ? "" : "epdalearn"));
		}
	}
}
