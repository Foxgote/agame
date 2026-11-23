using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurvivorNote : MonoBehaviour
{
	private string loreString;

	private Sprite loreSprite;

	private TMP_FontAsset fontAsset;

	public TMP_FontAsset[] fonts;

	private void Start()
	{
		(string, string, string) note = Locale.GetNote((!(Random.value > 0.5f)) ? (WorldGeneration.world.biomeDepth + 1) : 0);
		loreString = "\"" + note.Item1 + "\"";
		if (!string.IsNullOrEmpty(note.Item2))
		{
			loreSprite = Resources.Load<Sprite>(note.Item2);
		}
		switch (note.Item3)
		{
		case "experiment":
			fontAsset = fonts[0];
			break;
		case "milky":
			fontAsset = fonts[1];
			break;
		case "dune":
			fontAsset = fonts[2];
			break;
		}
	}

	public void OnUse()
	{
		Clicked();
	}

	public void Clicked()
	{
		if (Vector2.Distance(base.transform.position, PlayerCamera.main.body.transform.position) < 6f && PlayerCamera.main.body.canRead)
		{
			PlayerCamera.main.SetTimeScale(PlayerCamera.SpeedType.Slowmo, switchSound: false);
			ScrollableText.CreateText(loreString, cover: false, loreSprite ? new List<Sprite> { loreSprite } : null, fontAsset);
			StartCoroutine(ChangeTimeScaleWhenFinishedReading());
		}
	}

	public static IEnumerator ChangeTimeScaleWhenFinishedReading(string alertwhenend = "")
	{
		while (ScrollableText.textExists)
		{
			yield return null;
		}
		PlayerCamera.main.SetTimeScale(PlayerCamera.SpeedType.Normal, switchSound: false);
		if (!string.IsNullOrEmpty(alertwhenend))
		{
			PlayerCamera.main.DoAlert(Locale.GetOther(alertwhenend));
		}
	}
}
