using UnityEngine;
using UnityEngine.UI;

public class Moodle : MonoBehaviour
{
	public string type;

	public bool doWarningFlash;

	private Image img;

	private Image img2;

	public Image flash;

	public float unTransparentTime;

	public bool isSide;

	private void Start()
	{
		img = GetComponent<Image>();
		img2 = base.transform.GetChild(0).GetComponent<Image>();
		img.color = new Color(1f, 1f, 1f, ((PlayerCamera.main.blackAmount < PlayerCamera.GetUnconsciousBlack()) ? 1f : 0f) - unTransparentTime * 2f);
		img2.color = new Color(1f, 1f, 1f, 1f - unTransparentTime * 2f);
	}

	private void Update()
	{
		if (isSide)
		{
			img.enabled = MoodleManager.showSideMoodles;
			img2.enabled = MoodleManager.showSideMoodles;
			if ((bool)flash)
			{
				flash.enabled = MoodleManager.showSideMoodles;
			}
		}
		unTransparentTime -= Time.deltaTime;
		if (unTransparentTime < 0f)
		{
			unTransparentTime = 0f;
		}
		if (doWarningFlash)
		{
			flash.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.unscaledTime * 2.5f, 1f));
		}
		GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, Mathf.Lerp(GetComponent<RectTransform>().anchoredPosition.y, (!doWarningFlash) ? 0f : (Mathf.Sin(Time.unscaledTime * 6f) * 4f), Time.unscaledDeltaTime * 12f));
		GetComponent<RectTransform>().localScale = Vector2.Lerp(GetComponent<RectTransform>().localScale, Vector2.one, Time.unscaledDeltaTime * 12f);
		img.color = new Color(1f, 1f, 1f, ((PlayerCamera.main.blackAmount < PlayerCamera.GetUnconsciousBlack()) ? 1f : 0f) - unTransparentTime * 2f);
		img2.color = new Color(1f, 1f, 1f, 1f - unTransparentTime * 2f);
	}
}
