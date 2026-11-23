using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusMoodleShowScript : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<Image>().enabled = !MoodleManager.showSideMoodles;
		base.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = !MoodleManager.showSideMoodles;
	}

	private void Update()
	{
		GetComponent<Image>().enabled = !MoodleManager.showSideMoodles;
		base.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = !MoodleManager.showSideMoodles;
	}
}
