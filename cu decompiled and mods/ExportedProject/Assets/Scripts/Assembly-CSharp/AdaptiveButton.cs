using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdaptiveButton : MonoBehaviour
{
	public enum MenuAction
	{
		Play = 0,
		Quit = 1,
		Settings = 2,
		Tutorial = 3,
		Credits = 4
	}

	public MenuAction action;

	public Image overlay;

	private RectTransform rect;

	public TextMeshProUGUI text;

	public bool overlayActive
	{
		get
		{
			if (!PreRunScript.instance.runSettingsScreen.activeSelf && !PreRunScript.instance.contentWarning.activeSelf && !PreRunScript.instance.tutorialWarning.activeSelf && !ScrollableText.textExists && !SettingsMenu.instance)
			{
				return PreRunScript.instance.creditsScreen.activeSelf;
			}
			return true;
		}
	}

	private void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		float num = GetDistanceToCursor();
		if (overlayActive)
		{
			num = 99999f;
		}
		float a = Mathf.Max(1f - num * 0.005f, 0.025f);
		float num2 = 1f;
		overlay.color = Color.Lerp(overlay.color, new Color(num2, num2, num2, a), Time.unscaledDeltaTime * 3f);
		a = ((num <= 0f) ? 1f : 0f);
		text.color = Color.Lerp(text.color, new Color(1f, 1f, 1f, a), Time.unscaledDeltaTime * 3f);
		if (Input.GetMouseButtonDown(0) && num <= 0f)
		{
			Clicked();
		}
	}

	private void Clicked()
	{
		switch (action)
		{
		case MenuAction.Play:
			PreRunScript.instance.runSettingsScreen.SetActive(value: true);
			PlayerCamera.PlayUISound("click");
			break;
		case MenuAction.Quit:
			PreRunScript.instance.Quit();
			break;
		case MenuAction.Tutorial:
			PreRunScript.instance.StartTutorial();
			PlayerCamera.PlayUISound("click");
			break;
		case MenuAction.Credits:
			PreRunScript.instance.StartCoroutine("SwitchToCredits");
			PlayerCamera.PlayUISound("click");
			break;
		case MenuAction.Settings:
			SettingsMenu.OpenMenu(PreRunScript.instance.mainCanvas.transform);
			break;
		}
	}

	public float GetDistanceToCursor()
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rect, Input.mousePosition, null, out var localPoint);
		Rect rect = this.rect.rect;
		if (rect.Contains(localPoint))
		{
			return 0f;
		}
		float x = Mathf.Clamp(localPoint.x, rect.xMin, rect.xMax);
		float y = Mathf.Clamp(localPoint.y, rect.yMin, rect.yMax);
		return Vector2.Distance(b: new Vector2(x, y), a: localPoint);
	}
}
