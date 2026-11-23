using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollableText : MonoBehaviour
{
	public enum TextCloseType
	{
		Instant = 0,
		OnFade = 1
	}

	[TextArea]
	public string startString;

	private List<int> positions;

	private string desiredText;

	private int progress;

	private int posIndex;

	public TextMeshProUGUI text;

	private float lastCheckTime;

	private AudioSource src;

	private static GameObject textObj;

	private List<Sprite> sprites;

	public Image[] imgs;

	public static TextCloseType closeType;

	private bool toClose;

	public static bool textExists => textObj != null;

	public bool _onLastPanel => posIndex == positions.Count - 1;

	public static bool onLastPanel
	{
		get
		{
			if ((bool)textObj)
			{
				return textObj.GetComponent<ScrollableText>()._onLastPanel;
			}
			return false;
		}
	}

	public static void CreateText(string text, bool cover = false, List<Sprite> spr = null, TMP_FontAsset font = null)
	{
		if (!textObj)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Special/ClickableText"), PlayerCamera.main ? PlayerCamera.main.mainCanvas.transform : GlobalDark.main.canvas.transform) as GameObject;
			gameObject.GetComponent<ScrollableText>().UpdateText(text);
			if (cover)
			{
				gameObject.transform.GetChild(0).gameObject.SetActive(value: true);
			}
			gameObject.GetComponent<ScrollableText>().sprites = spr;
			textObj = gameObject;
			if ((bool)font)
			{
				gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().font = font;
			}
		}
	}

	public static void ForceClose()
	{
		if ((bool)textObj)
		{
			Object.Destroy(textObj);
			textObj = null;
		}
	}

	private void Start()
	{
		src = GetComponent<AudioSource>();
		if (!string.IsNullOrEmpty(startString))
		{
			UpdateText(Locale.GetOther(startString));
		}
	}

	public void Skip()
	{
		if (progress != positions[posIndex])
		{
			progress = positions[posIndex];
		}
		else if (posIndex == positions.Count - 1)
		{
			if (closeType == TextCloseType.Instant)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			toClose = true;
			GlobalDark.main.Darken();
		}
		else
		{
			posIndex++;
		}
	}

	public void ScrollDown()
	{
		float num = 200f;
		text.rectTransform.sizeDelta = text.rectTransform.sizeDelta + Vector2.up * num;
		text.rectTransform.position = text.rectTransform.position + Vector3.up * num * 0.5f;
	}

	private void Update()
	{
		if (Time.unscaledTime - lastCheckTime > 0.025f)
		{
			lastCheckTime = Time.unscaledTime;
			progress++;
			if (progress > positions[posIndex])
			{
				progress = positions[posIndex];
			}
			else
			{
				src.Stop();
				src.Play();
			}
			text.text = desiredText.Substring(0, progress) + ((progress == positions[posIndex]) ? ("\n<alpha=#44><align=\"center\">" + ((!WorldGeneration.world || !WorldGeneration.world.cutsceneActive) ? Locale.GetOther("clicktocontinue") : Locale.GetOther("clicktocontinuealt"))) : "");
			text.ForceMeshUpdate();
			if (text.isTextOverflowing)
			{
				ScrollDown();
			}
		}
		if (sprites != null && sprites.Count > 0)
		{
			imgs[posIndex % 2].sprite = sprites[posIndex];
			imgs[posIndex % 2].color = Color.Lerp(imgs[posIndex % 2].color, Color.white, Time.unscaledDeltaTime);
			imgs[(posIndex % 2 == 0) ? 1u : 0u].color = Color.Lerp(imgs[(posIndex % 2 == 0) ? 1u : 0u].color, Color.clear, Time.unscaledDeltaTime * 4f);
		}
		if (Input.GetKeyDown(KeyBinds.GetBind("pause")) && (!WorldGeneration.world || !WorldGeneration.world.cutsceneActive))
		{
			ForceClose();
		}
		if (toClose && !GlobalDark.main.IsDarkening())
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void UpdateText(string str)
	{
		positions = new List<int>();
		foreach (Match item in Regex.Matches(str, "<>"))
		{
			positions.Add(item.Index);
		}
		positions.Add(str.Length);
		desiredText = str;
		desiredText = desiredText.Replace("<>", "  ");
	}
}
