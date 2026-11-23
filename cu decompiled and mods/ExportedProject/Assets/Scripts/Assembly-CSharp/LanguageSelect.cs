using TMPro;
using UnityEngine;

public class LanguageSelect : MonoBehaviour
{
	public static void LoadLanguageError()
	{
		GameObject gameObject = GameObject.Find("LANGWARNING");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
		}
	}

	public static void LoadLanguageOutdated()
	{
		GameObject gameObject = GameObject.Find("LANGOUTDATED");
		if ((bool)gameObject)
		{
			gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
			if (Locale.GetOther("outdatedwarning") != "outdatedwarning")
			{
				gameObject.GetComponent<TextMeshProUGUI>().text = Locale.GetOther("outdatedwarning");
			}
		}
	}
}
