using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalDark : MonoBehaviour
{
	public static GlobalDark main;

	public Canvas canvas;

	private float speedMult = 1f;

	public Image image;

	private float darkProgress;

	private bool darkening;

	public (string name, string desc) tooltipText;

	public TextMeshProUGUI tooltipNameObj;

	public TextMeshProUGUI tooltipDescObj;

	public RectTransform hoverNameBackground;

	public RectTransform hoverDescriptionBackground;

	public RectTransform tooltipRect;

	public string distortionChars = "!@#$%&";

	private bool stayDark;

	public float uiScale => (float)Screen.height / 1080f;

	public bool forceDisable
	{
		get
		{
			if ((!MinigameBase.main || !MinigameBase.main.currentMinigame) && (!WorldGeneration.world || !WorldGeneration.world.cutsceneActive))
			{
				if ((bool)PlayerCamera.main)
				{
					return !PlayerCamera.main.body.alive;
				}
				return false;
			}
			return true;
		}
	}

	public bool showTooltip
	{
		get
		{
			if (!string.IsNullOrEmpty(tooltipText.name))
			{
				return !forceDisable;
			}
			return false;
		}
	}

	public float distortionAmount
	{
		get
		{
			if (!PlayerCamera.main || !PlayerCamera.main.body.alive || (bool)SettingsMenu.instance)
			{
				return 0f;
			}
			return 0.95f - PlayerCamera.main.body.brainHealth * 0.01f;
		}
	}

	private void Awake()
	{
		main = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		RPCManager.CheckInstance();
		SteamManager.CheckInstance();
	}

	private void Update()
	{
		darkProgress = Mathf.MoveTowards(darkProgress, darkening ? 1f : 0f, Mathf.Min(Time.unscaledDeltaTime, 0.1f) * 1.5f * speedMult);
		if (darkProgress >= 1f && !stayDark)
		{
			darkening = false;
		}
		image.color = new Color(0f, 0f, 0f, darkProgress);
		HandleTooltip();
		tooltipText = (name: "", desc: "");
	}

	public void SetTooltip((string, string) newText)
	{
		tooltipText = newText;
	}

	public static float GetDeterministicRandom(int seed)
	{
		return (float)new System.Random(seed).NextDouble();
	}

	public static char GetDeterministicRandomChar(string input, int seed)
	{
		int index = new System.Random(seed).Next(0, input.Length);
		return input[index];
	}

	public static bool IsCharOutsideAngleBrackets(string s, int index)
	{
		bool flag = false;
		for (int i = 0; i <= index; i++)
		{
			switch (s[i])
			{
			case '<':
				flag = true;
				break;
			case '>':
				if (i == index)
				{
					return false;
				}
				flag = false;
				break;
			}
		}
		return !flag;
	}

	public string DistortStr(string str)
	{
		StringBuilder stringBuilder = new StringBuilder(str);
		for (int i = 0; i < str.Length; i++)
		{
			if (GetDeterministicRandom(i) < distortionAmount && IsCharOutsideAngleBrackets(str, i))
			{
				stringBuilder[i] = GetDeterministicRandomChar(distortionChars, i + 128);
			}
		}
		return stringBuilder.ToString();
	}

	public void HandleTooltip()
	{
		tooltipRect.anchoredPosition = Input.mousePosition;
		(string, string) uITooltip = UIUtil.GetUITooltip(UIUtil.GetEventSystemRaycastResults());
		if (!string.IsNullOrEmpty(uITooltip.Item1))
		{
			tooltipText = uITooltip;
		}
		tooltipRect.gameObject.SetActive(showTooltip);
		if (showTooltip)
		{
			tooltipRect.localScale = Vector3.Lerp(tooltipRect.localScale, Vector3.one * Screen.height / 1080f, Time.unscaledDeltaTime * 24f);
			if (distortionAmount > 0f)
			{
				tooltipText.name = DistortStr(tooltipText.name);
				tooltipText.desc = DistortStr(tooltipText.desc);
			}
			tooltipNameObj.text = tooltipText.name;
			tooltipDescObj.text = tooltipText.desc;
			hoverNameBackground.sizeDelta = new Vector2(tooltipNameObj.preferredWidth, hoverNameBackground.sizeDelta.y);
			hoverDescriptionBackground.sizeDelta = new Vector2(Mathf.Min(tooltipDescObj.preferredWidth, tooltipDescObj.rectTransform.sizeDelta.x), tooltipDescObj.preferredHeight);
			Vector2 vector = new Vector2(Mathf.Max(hoverNameBackground.position.x + hoverNameBackground.sizeDelta.x * uiScale, hoverDescriptionBackground.position.x + hoverDescriptionBackground.sizeDelta.x * uiScale), Mathf.Min(hoverDescriptionBackground.position.y, hoverNameBackground.position.y - hoverDescriptionBackground.sizeDelta.y * uiScale));
			Vector2 vector2 = new Vector2(Mathf.Clamp(vector.x, 30f * uiScale, (float)Screen.width - 30f * uiScale), Mathf.Clamp(vector.y, 30f * uiScale, (float)Screen.height - 30f * uiScale));
			if (vector != vector2)
			{
				Vector2 vector3 = vector2 - vector;
				tooltipRect.position += (Vector3)vector3;
			}
		}
		else
		{
			tooltipRect.localScale = new Vector3(0.1f, 0.1f, 1f);
		}
	}

	public void Darken()
	{
		darkening = true;
	}

	public bool IsDarkening()
	{
		return darkening;
	}

	public void SetDarkAmount(float amt)
	{
		darkProgress = amt;
		image.color = new Color(0f, 0f, 0f, darkProgress);
	}

	public void SetStayDark(bool stay)
	{
		stayDark = stay;
	}

	public void SetSpeedMultiplier(float amt)
	{
		speedMult = amt;
	}

	public static void CheckForDark()
	{
		if (!main)
		{
			UnityEngine.Object.Instantiate(Resources.Load("Special/GlobalDark"), Vector2.zero, Quaternion.identity);
		}
	}
}
