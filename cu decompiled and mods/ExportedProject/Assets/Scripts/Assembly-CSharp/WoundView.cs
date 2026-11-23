using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WoundView : MonoBehaviour
{
	public delegate bool LimbCondition(Limb limb);

	public Transform[] limbTransforms;

	public Vector2[] origPositions;

	public Transform mainStatsTrans;

	public Body body;

	public int limbLookingAt;

	public TextMeshProUGUI bigLimbTitle;

	public Image[] limbImages;

	public UITooltip[] limbTooltips;

	public float[] limbImageFlash;

	public float[] limbImageLerp;

	public static WoundView view;

	public Button napbutton;

	public Sprite[] icons;

	public float checkTime;

	private List<GameObject> iconObj;

	public TextMeshProUGUI brainHealthText;

	public TextMeshProUGUI conscText;

	public TextMeshProUGUI painText;

	public TextMeshProUGUI bloodText;

	public TextMeshProUGUI bleedText;

	public TextMeshProUGUI oxyText;

	public TextMeshProUGUI tempText;

	public TextMeshProUGUI sickText;

	public TextMeshProUGUI happyText;

	public TextMeshProUGUI hungerText;

	public TextMeshProUGUI thirstText;

	public TextMeshProUGUI weightText;

	public TextMeshProUGUI respiratoryText;

	public TextMeshProUGUI energyText;

	public TextMeshProUGUI nameText;

	public TextMeshProUGUI limbNameText;

	public TextMeshProUGUI skinText;

	public TextMeshProUGUI muscleText;

	public TextMeshProUGUI infecText;

	public TextMeshProUGUI limbPainText;

	public TextMeshProUGUI limbBleedText;

	public TextMeshProUGUI radText;

	public TextMeshProUGUI immunityText;

	public TextMeshProUGUI bpmText;

	public TextMeshProUGUI versionText;

	public TextMeshProUGUI limbForceText;

	public TextMeshProUGUI limbHealTimeText;

	public TextMeshProUGUI opiumText;

	public TextMeshProUGUI toxinText;

	public Image brainFill;

	public Image bloodFill;

	public Image tempFill;

	public Image foodFill;

	public Image thirstFill;

	public Image warningSymbol;

	public Image skinFill;

	public Image muscleFill;

	public Image bleedImage;

	public Image happinessIcon;

	public Image disfiguredIcon;

	public Image eyeIcon;

	public Image overfeedFill;

	public Image overhydrateFill;

	public Image limbFractureImage;

	public Image limbBleedOuterImage;

	public Image opiumBack;

	public Image toxinBack;

	public Sprite flaskGood;

	public Sprite flaskBroken;

	public Sprite noBleed;

	public Sprite yesBleed;

	public Sprite[] moodIcons;

	public Image disfiguredImage;

	public Image eyeGoneImage;

	public Image bothEyesGoneImage;

	public Image cargoImage;

	public List<GameObject> disableWhenUnchipped;

	public RectTransform heartGradient;

	public Image heartMask;

	public Sprite heartNormal;

	public Sprite heartStopped;

	public GameObject specialUseButton;

	public TextMeshProUGUI specialUseText;

	public TextMeshProUGUI bodyStatText;

	public UITooltip specialUseTip;

	public Color normalTextColor;

	public Color critTextColor;

	private bool didThump;

	private float heartProg;

	public float timeOpen;

	public Gradient bleedGradient;

	public int[] cInfo = new int[4];

	public static int specimenId;

	public static string firmwareVer;

	public bool armorMode;

	public Gradient limbArmorGradient;

	public Image modeToggleImage;

	public Sprite modeToggleHealth;

	public Sprite modeToggleArmor;

	private AudioSource audio;

	public GameObject criticalPanel;

	private TextMeshProUGUI criticalText;

	public UITooltip sleepTip;

	public Image sleepImage;

	public FilledImagePP lungFill1;

	public FilledImagePP lungFill2;

	public FilledImagePP limbForceFill;

	public FilledImagePP limbBleedFill;

	public FilledImagePP hemothoraxFill;

	public FilledImagePP radFill;

	public FilledImagePP toxinFill;

	public FilledImagePP opiumFill;

	public FilledImagePP[] skillBars;

	public TextMeshProUGUI[] skillTexts;

	public GameObject workoutList;

	public TextMeshProUGUI miseryText;

	public TextMeshProUGUI timeTextBar;

	public TextMeshProUGUI timeTextBottom;

	public Sprite limbFracturedSprite;

	public Sprite limbDislocatedSprite;

	public TextMeshProUGUI speciesText;

	public Sprite[] headCharSprites;

	public void ToggleMode()
	{
		armorMode = !armorMode;
		modeToggleImage.sprite = (armorMode ? modeToggleArmor : modeToggleHealth);
		PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.MiniClick);
		if (!armorMode)
		{
			UITooltip[] array = limbTooltips;
			foreach (UITooltip obj in array)
			{
				obj.tipName = "";
				obj.tipDesc = "";
			}
		}
	}

	private void Awake()
	{
		view = this;
		origPositions = new Vector2[limbImages.Length];
		limbImageFlash = new float[limbImages.Length];
		limbImageLerp = new float[limbImages.Length];
		for (int i = 0; i < limbImages.Length; i++)
		{
			origPositions[i] = limbImages[i].rectTransform.anchoredPosition;
		}
		criticalText = criticalPanel.GetComponentInChildren<TextMeshProUGUI>();
		iconObj = new List<GameObject>();
	}

	private void Start()
	{
		limbTooltips = new UITooltip[limbImages.Length];
		for (int i = 0; i < limbImages.Length; i++)
		{
			limbTooltips[i] = limbImages[i].gameObject.AddComponent<UITooltip>();
			limbTooltips[i].skipLocale = true;
		}
		audio = GetComponent<AudioSource>();
		switch (body.charType)
		{
		case 0:
			speciesText.text = Locale.GetOther("experiment").ToUpper();
			break;
		case 1:
			speciesText.text = Locale.GetOther("milky").ToUpper();
			break;
		case 2:
			speciesText.text = Locale.GetOther("dune").ToUpper();
			break;
		}
		if (body.charType == 1)
		{
			limbImages[0].sprite = headCharSprites[2];
			limbImages[0].SetNativeSize();
			limbImages[0].transform.GetChild(0).GetComponent<Image>().sprite = headCharSprites[3];
			limbImages[0].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
		}
		if (body.charType == 2)
		{
			limbImages[0].sprite = headCharSprites[4];
			limbImages[0].SetNativeSize();
			limbImages[0].transform.GetChild(0).GetComponent<Image>().sprite = headCharSprites[5];
			limbImages[0].transform.GetChild(0).GetComponent<Image>().SetNativeSize();
		}
	}

	public void SetCharDetails(int height, int age, int id, int ver)
	{
		bodyStatText.text = height + "cm | " + age + "y | #" + id;
		versionText.text = "UNI-HEALTH v2.0" + ver;
		cInfo = new int[4] { height, age, id, ver };
		firmwareVer = "v2.0" + ver;
		specimenId = id;
	}

	public Image AddImageToLimb(Limb limb, Sprite sprite, bool flip, LimbCondition removeCondition = null)
	{
		RectTransform rectTransform = limbImages[Array.IndexOf(body.limbs, limb)].rectTransform;
		GameObject gameObject = new GameObject(sprite.name, typeof(Image));
		gameObject.transform.SetParent(rectTransform);
		gameObject.transform.localScale = new Vector3(flip ? (-1f) : 1f, 1f, 1f);
		gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		gameObject.GetComponent<Image>().sprite = sprite;
		gameObject.GetComponent<Image>().SetNativeSize();
		if (removeCondition == null)
		{
			UnityEngine.Object.Destroy(gameObject, 300f);
		}
		else
		{
			OnLimbWound onLimbWound = gameObject.AddComponent<OnLimbWound>();
			onLimbWound.cond = removeCondition;
			onLimbWound.limb = limb;
		}
		return gameObject.GetComponent<Image>();
	}

	public void TakeANap()
	{
		PlayerCamera.main.body.TakeANap();
	}

	public void FlashText(TextMeshProUGUI text, bool flash)
	{
		Color color = (flash ? critTextColor : normalTextColor);
		color.a = ((!flash || Mathf.Sin(Time.unscaledTime * 20f) > 0f) ? 1f : 0f);
		text.color = color;
	}

	public Image GenerateIcon(Vector2 pos, Sprite image)
	{
		GameObject obj = new GameObject("icon", typeof(Image));
		Image component = obj.GetComponent<Image>();
		component.raycastTarget = false;
		obj.transform.SetParent(base.transform);
		obj.GetComponent<RectTransform>().position = pos;
		obj.transform.localScale = Vector3.one;
		obj.transform.localRotation = Quaternion.identity;
		component.sprite = image;
		component.SetNativeSize();
		return component;
	}

	public void UpdateView()
	{
		UpdateSkillUI();
		if (body.isCriticallyDying && !WorldGeneration.unchipped)
		{
			criticalPanel.SetActive(value: true);
			criticalText.enabled = Mathf.Sin(Time.unscaledTime * 20f) > 0f;
			criticalText.text = Locale.GetOther("healthchipcriticalalert" + ((int)Time.unscaledTime % 3 + 1));
		}
		else if (RadiationLine.line.active)
		{
			criticalPanel.SetActive(value: true);
			criticalText.enabled = Mathf.Sin(Time.unscaledTime * 20f) > 0f;
			criticalText.text = Locale.GetOther("healthchipradalert" + ((int)Time.unscaledTime % 3 + 1));
		}
		else
		{
			criticalPanel.SetActive(value: false);
		}
		LayerTimerDisplay();
		for (int i = 0; i < limbImageLerp.Length; i++)
		{
			limbImageLerp[i] = Mathf.Lerp(limbImageLerp[i], (limbLookingAt == i) ? 1f : 0f, Time.deltaTime * 7f);
		}
		timeOpen += Time.unscaledDeltaTime;
		for (int j = 0; j < limbImageFlash.Length; j++)
		{
			limbImageFlash[j] = Mathf.Lerp(limbImageFlash[j], 0f, Time.unscaledDeltaTime * 4f);
		}
		napbutton.interactable = body.canTakeNap;
		Limb limb = body.limbs[limbLookingAt];
		PlayerCamera.main.selectedLimb = limb;
		limbNameText.text = limb.fullName;
		skinText.text = $"{Mathf.RoundToInt(limb.skinHealth)}";
		muscleText.text = $"{Mathf.RoundToInt(limb.muscleHealth)}";
		FlashText(skinText, limb.skinHealth < 10f);
		FlashText(muscleText, limb.muscleHealth < 10f);
		limbBleedText.text = ((limb.bleedAmount > 0.1f) ? $"{body.bloodToLiters(limb.bleedAmount * limb.bleedSpeedMult) * 60f:0.00}\nl/m" : "");
		FlashText(limbBleedText, limb.bleedAmount > 8f);
		limbPainText.text = string.Format("{0}", (limb.pain > 0f) ? ((object)Mathf.RoundToInt(limb.pain)) : "-");
		infecText.text = ((limb.infectionAmount >= 25f || PlayerCamera.main.showInfection[limbLookingAt]) ? (Mathf.RoundToInt(limb.infectionAmount).ToString() ?? "") : "-");
		FlashText(infecText, limb.infectionAmount >= 25f);
		FlashText(limbPainText, limb.pain >= 50f);
		limbForceText.text = Mathf.RoundToInt(limb.totalForce * 100f).ToString();
		FlashText(limbForceText, limb.totalForce < 0.25f);
		limbForceFill.fillAmount = limb.totalForce;
		limbBleedFill.fillAmount = limb.bleedAmount / 50f;
		limbBleedOuterImage.enabled = limb.bleedAmount > 0.1f;
		if (limb.broken)
		{
			limbFractureImage.enabled = true;
			limbFractureImage.sprite = limbFracturedSprite;
		}
		else if (limb.dislocated)
		{
			limbFractureImage.enabled = true;
			limbFractureImage.sprite = limbDislocatedSprite;
		}
		else
		{
			limbFractureImage.enabled = false;
		}
		float injuryHealTime = limb.injuryHealTime;
		if (injuryHealTime > 0f)
		{
			if (limb.prostheticInfo.hasProsthetic)
			{
				limbHealTimeText.text = $"{Mathf.RoundToInt(limb.broken ? limb.boneHealTimer : limb.dislocationTimer)}";
			}
			else
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(injuryHealTime);
				limbHealTimeText.text = $"{Mathf.FloorToInt((float)timeSpan.TotalMinutes)}m\n{timeSpan.Seconds}s";
			}
		}
		else
		{
			limbHealTimeText.text = "";
		}
		if (body.totalHappiness > 50f)
		{
			happinessIcon.sprite = moodIcons[0];
		}
		else if (body.totalHappiness > 10f)
		{
			happinessIcon.sprite = moodIcons[1];
		}
		else if (body.totalHappiness > -10f)
		{
			happinessIcon.sprite = moodIcons[2];
		}
		else if (body.totalHappiness > -40f)
		{
			happinessIcon.sprite = moodIcons[3];
		}
		else if (body.totalHappiness > -75f)
		{
			happinessIcon.sprite = moodIcons[4];
		}
		else
		{
			happinessIcon.sprite = moodIcons[5];
		}
		brainFill.GetComponent<FilledImagePP>().fillAmount = body.brainHealth * 0.01f;
		bloodFill.GetComponent<FilledImagePP>().fillAmount = body.bloodVolumePercentage;
		tempFill.GetComponent<FilledImagePP>().fillAmount = (body.temperature - 28f) * 0.08f;
		foodFill.GetComponent<FilledImagePP>().fillAmount = body.hunger * 0.01f;
		overfeedFill.GetComponent<FilledImagePP>().fillAmount = (body.hunger - 100f) * 0.01f;
		overhydrateFill.GetComponent<FilledImagePP>().fillAmount = (body.thirst - 100f) * 0.01f;
		thirstFill.GetComponent<FilledImagePP>().fillAmount = body.thirst * 0.01f;
		muscleFill.GetComponent<FilledImagePP>().fillAmount = limb.muscleHealth * 0.01f;
		skinFill.GetComponent<FilledImagePP>().fillAmount = limb.skinHealth * 0.01f;
		if (limb.prostheticInfo.hasProsthetic)
		{
			skinFill.fillAmount = 0f;
			skinText.text = "--";
		}
		bpmText.text = ((body.defibShockedFrames == 0) ? ((object)Mathf.RoundToInt(body.heartRate)) : "???")?.ToString() + " | " + body.bloodPressureReadout;
		FlashText(bpmText, body.inCardiacArrest || body.fibrillationRising || body.bloodPressure > 160f || body.bloodPressure < 75f);
		brainHealthText.text = $"{Mathf.RoundToInt(body.brainHealth)}";
		FlashText(brainHealthText, body.brainHealth < 50f);
		conscText.text = $"{Mathf.RoundToInt(body.consciousness)}%";
		FlashText(conscText, body.consciousness < 40f);
		painText.text = $"{Mathf.RoundToInt(body.averagePain)}%";
		FlashText(painText, body.averagePain > 75f);
		bloodText.text = $"{body.bloodToLitersBody(body.bloodVolume) * body.bloodScale:0.00}l";
		FlashText(bloodText, body.bloodVolume < 25f);
		bleedText.text = ((body.totalBleedSpeed > 0.0001f) ? $"{body.bloodToLiters(body.totalBleedSpeed * body.bloodScale) * 60f:0.00}l/m" : "");
		FlashText(bleedText, body.totalBleedSpeed > 0.06f);
		oxyText.text = $"{Mathf.RoundToInt(body.bloodOxygen)}%";
		FlashText(oxyText, body.bloodOxygen < 70f);
		immunityText.text = $"{Mathf.RoundToInt(body.immunity)}%";
		FlashText(immunityText, body.immunity < 50f);
		hungerText.text = $"{Mathf.RoundToInt(body.hunger)}";
		FlashText(hungerText, body.hunger < 10f);
		thirstText.text = $"{Mathf.RoundToInt(body.thirst)}";
		FlashText(thirstText, body.thirst < 10f);
		sickText.text = $"{Mathf.RoundToInt(body.sicknessAmount)}%";
		FlashText(sickText, body.sicknessAmount > 70f);
		lungFill2.fillAmount = (body.stamina - 50f) / 50f;
		lungFill1.fillAmount = body.stamina / 50f;
		respiratoryText.text = body.respiratoryRateReadout;
		hemothoraxFill.fillAmount = body.hemothorax * 0.01f;
		FlashText(respiratoryText, body.respiratoryRate < 0.5f);
		energyText.text = $"{Mathf.RoundToInt(body.energy)}%";
		FlashText(energyText, body.energy < 5f);
		happyText.text = $"{body.totalHappiness:0.0}";
		FlashText(happyText, body.totalHappiness <= -75f);
		float num = body.radiationSickness * 0.3f;
		radFill.fillAmount = body.radiationSickness * 0.01f;
		if (num >= 10f)
		{
			radText.text = $"{Mathf.RoundToInt(num)}gy";
		}
		else
		{
			radText.text = $"{num:0.0}gy";
		}
		FlashText(radText, body.radiationSickness > 25f);
		float num2 = Units.ConvertWeight(body.weightOffset * 0.34f + 50f + body.weightVisualOffset);
		weightText.text = $"{num2:0.#}{Units.WeightUnit()}";
		FlashText(weightText, Mathf.Abs(body.weightOffset) > 55f);
		float num3 = Units.ConvertTemperature(body.temperature);
		tempText.text = num3.ToString((num3 < 100f) ? "0.#" : "0") + Units.TemperatureUnit(lower: true);
		FlashText(tempText, body.temperature < 30f || body.temperature > 41f);
		if (body.TryGetComponent<Painkillers>(out var component) && component.actualOpiateReception > 0f)
		{
			opiumBack.enabled = true;
			opiumFill.fillAmount = component.actualOpiateReception / 80f;
			opiumText.text = Mathf.RoundToInt(component.actualOpiateReception) + "op";
			FlashText(opiumText, component.actualOpiateReception >= 80f);
		}
		else
		{
			opiumBack.enabled = false;
			opiumFill.fillAmount = 0f;
			opiumText.text = "";
		}
		if (body.venomCurrent > 0f)
		{
			toxinBack.enabled = true;
			toxinFill.fillAmount = body.venomCurrent * 0.01f;
			toxinText.text = ((body.venomCurrent >= 100f) ? (Mathf.RoundToInt(body.venomCurrent * 0.1f) + "ppm") : $"{body.venomCurrent * 0.1f:0.0}ppm");
			FlashText(toxinText, body.venomCurrent > 30f);
		}
		else
		{
			toxinBack.enabled = false;
			toxinFill.fillAmount = 0f;
			toxinText.text = "";
		}
		if (!body.aboveMedicalCutoff)
		{
			miseryText.gameObject.SetActive(value: true);
			miseryText.text = $"{body.totalHappiness:0.0}";
			miseryText.color = new Color(1f, 0f, 0f, UnityEngine.Random.Range(0.8f, 1f));
		}
		else
		{
			miseryText.gameObject.SetActive(value: false);
		}
		UITooltip uITooltip = sleepTip;
		string other = Locale.GetOther("napinfo");
		int curSleep = (int)body.curSleep;
		uITooltip.tipName = other.Replace("<>", Locale.GetOther("sleepquality" + curSleep));
		switch (body.curSleep)
		{
		case Body.SleepQuality.Good:
			sleepImage.color = Color.green;
			break;
		case Body.SleepQuality.Okay:
			sleepImage.color = Color.white;
			break;
		case Body.SleepQuality.Mediocre:
			sleepImage.color = new Color(1f, 0.5f, 0f);
			break;
		case Body.SleepQuality.Bad:
			sleepImage.color = Color.red;
			break;
		}
		if (!body.canTakeNap)
		{
			sleepImage.color = Color.gray;
		}
		for (int k = 0; k < body.limbs.Length; k++)
		{
			float num4 = limbImageFlash[k] * 0.6f;
			float value = ((timeOpen + 0.15f) * 12f - limbImages[k].transform.position.y / (float)Screen.height * 1080f * 0.01f) * 0.25f;
			value = Mathf.Clamp01(value);
			Color baseColor = body.limbs[k].GetBaseColor();
			if (body.limbs[k].dismembered)
			{
				value = 0f;
			}
			value *= baseColor.a;
			float num5 = (body.limbs[k].prostheticInfo.hasProsthetic ? body.limbs[k].muscleHealth : body.limbs[k].skinHealth);
			limbImages[k].color = new Color(1f - num4, num5 * 0.01f - num4, num5 * 0.01f - num4, value);
			limbImages[k].rectTransform.GetChild(0).GetComponent<Image>().color = new Color(Mathf.Lerp(baseColor.r, 1f, WorldGeneration.unchipped ? 0f : (1f - body.limbs[k].muscleHealth * 0.01f)), baseColor.g, baseColor.b + (WorldGeneration.unchipped ? (body.limbs[k].infectionAmount * 0.01f - 0.2f) : 0f), value);
			if (WorldGeneration.unchipped)
			{
				if (body.limbs[k].pain > 80f)
				{
					limbImages[k].rectTransform.GetChild(0).GetComponent<Image>().color = new Color(Mathf.PingPong(Time.unscaledTime * 4f, 1f), 0f, 0f, value);
				}
			}
			else if (body.limbs[k].muscleHealth < 10f)
			{
				limbImages[k].rectTransform.GetChild(0).GetComponent<Image>().color = new Color(Mathf.PingPong(Time.unscaledTime * 4f, 1f), 0f, 0f, value);
			}
			limbImages[k].rectTransform.anchoredPosition = origPositions[k] + new Vector2(UnityEngine.Random.Range((0f - body.limbs[k].pain) * 0.02f, body.limbs[k].pain * 0.02f), UnityEngine.Random.Range((0f - body.limbs[k].pain) * 0.02f, body.limbs[k].pain * 0.02f));
			if (armorMode)
			{
				Color color = limbArmorGradient.Evaluate(1f / body.limbs[k].GetArmorReduction());
				if (body.limbs[k].dismembered)
				{
					color.a = 0f;
				}
				limbImages[k].color = (body.limbs[k].dismembered ? Color.clear : Color.white);
				limbImages[k].rectTransform.GetChild(0).GetComponent<Image>().color = color;
				limbTooltips[k].tipName = body.limbs[k].fullName;
				limbTooltips[k].tipDesc = "<sprite index=13 tint=1>" + Locale.GetOther("damagereduction") + Mathf.RoundToInt((1f - 1f / body.limbs[k].GetArmorReduction()) * 100f) + "%\n<sprite index=14 tint=1>" + Locale.GetOther("bodyinsulation") + Mathf.RoundToInt(body.GetTotalInsulation() * 100f) + "%";
			}
		}
		if (checkTime > 0.1f)
		{
			checkTime = 0f;
			foreach (GameObject item in iconObj)
			{
				UnityEngine.Object.Destroy(item);
			}
			iconObj.Clear();
			disfiguredImage.enabled = body.disfigured;
			eyeGoneImage.enabled = body.eyeGone;
			bothEyesGoneImage.enabled = body.bothEyesGone;
			cargoImage.enabled = body.hasCargo;
			for (int l = 0; l < body.limbs.Length; l++)
			{
				if (body.limbs[l].dismembered)
				{
					continue;
				}
				if (armorMode)
				{
					GameObject gameObject = Utils.Create("Special/LimbArmorNum", limbImages[l].transform.parent);
					gameObject.transform.position = limbImages[l].transform.position;
					gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt((1f - 1f / body.limbs[l].GetArmorReduction()) * 100f) + "%";
					iconObj.Add(gameObject);
					continue;
				}
				List<int> list = new List<int>();
				if (body.limbs[l].totalBleedAmount > 1f)
				{
					list.Add(0);
				}
				if (body.limbs[l].infectionAmount >= 25f || PlayerCamera.main.showInfection[l])
				{
					list.Add(1);
				}
				if (body.limbs[l].broken)
				{
					list.Add(2);
				}
				if (body.limbs[l].dislocated)
				{
					list.Add(3);
				}
				if (body.limbs[l].disinfectionTime > 0f && !WorldGeneration.unchipped)
				{
					list.Add(4);
				}
				if (body.limbs[l].hasShrapnel)
				{
					list.Add(5);
				}
				if (body.limbs[l].TryGetComponent<ChilledLimb>(out var component2))
				{
					list.Add(6);
				}
				for (int m = 0; m < list.Count; m++)
				{
					float num6 = ((float)m * 50f - (float)(list.Count - 1) * 25f) * (limbImageLerp[l] * 0.8f + 0.2f);
					Vector2 vector = new Vector2(num6, num6 * 0.2f);
					if (list[m] == 0)
					{
						(Sprite, Color) tuple = BleedImageFromLimb(body.limbs[l]);
						Image image = GenerateIcon(vector + (Vector2)limbImages[l].transform.position, tuple.Item1);
						image.rectTransform.SetSiblingIndex(limbImages[0].transform.parent.GetSiblingIndex() + 1);
						image.color = tuple.Item2;
						iconObj.Add(image.gameObject);
					}
					if (list[m] == 1)
					{
						Image image2 = GenerateIcon(vector + (Vector2)limbImages[l].transform.position, icons[(body.limbs[l].GetInfectionSpeed() > 0f) ? 11 : 10]);
						image2.rectTransform.SetSiblingIndex(limbImages[0].transform.parent.GetSiblingIndex() + 1);
						image2.color = Color32.Lerp(Color.white, new Color32(163, 0, 182, byte.MaxValue), body.limbs[l].infectionAmount * 0.01f);
						iconObj.Add(image2.gameObject);
					}
					if (list[m] == 2)
					{
						Image image3 = GenerateIcon(vector + (Vector2)limbImages[l].transform.position, body.limbs[l].prostheticInfo.hasProsthetic ? icons[17] : icons[2]);
						image3.rectTransform.SetSiblingIndex(limbImages[0].transform.parent.GetSiblingIndex() + 1);
						image3.color = new Color(1f, 1f - body.limbs[l].boneHealTimer * 0.01f, 1f - body.limbs[l].boneHealTimer * 0.01f);
						iconObj.Add(image3.gameObject);
					}
					if (list[m] == 3)
					{
						Image image4 = GenerateIcon(vector + (Vector2)limbImages[l].transform.position, body.limbs[l].prostheticInfo.hasProsthetic ? icons[16] : icons[1]);
						image4.rectTransform.SetSiblingIndex(limbImages[0].transform.parent.GetSiblingIndex() + 1);
						image4.color = new Color(1f, 1f - body.limbs[l].dislocationTimer * 0.01f, 1f - body.limbs[l].dislocationTimer * 0.01f);
						iconObj.Add(image4.gameObject);
					}
					if (list[m] == 4)
					{
						Image image5 = GenerateIcon(vector + (Vector2)limbImages[l].transform.position, icons[4]);
						image5.rectTransform.SetSiblingIndex(limbImages[0].transform.parent.GetSiblingIndex() + 1);
						float num7 = 0.1f + body.limbs[l].disinfectionTime / 220f;
						image5.color = new Color(num7, num7, num7);
						iconObj.Add(image5.gameObject);
					}
					if (list[m] == 5)
					{
						Image image6 = GenerateIcon(vector + (Vector2)limbImages[l].transform.position, icons[5]);
						image6.rectTransform.SetSiblingIndex(limbImages[0].transform.parent.GetSiblingIndex() + 1);
						image6.color = new Color32(173, 106, 54, byte.MaxValue);
						iconObj.Add(image6.gameObject);
					}
					if (list[m] == 6)
					{
						Image image7 = GenerateIcon(vector + (Vector2)limbImages[l].transform.position, icons[6]);
						image7.rectTransform.SetSiblingIndex(limbImages[0].transform.parent.GetSiblingIndex() + 1);
						image7.color = Color.Lerp(Color.gray, new Color32(54, 137, byte.MaxValue, byte.MaxValue), component2.timeLeft / component2.maxTime);
						iconObj.Add(image7.gameObject);
					}
				}
			}
		}
		specialUseButton.transform.position = Vector3.Lerp(specialUseButton.transform.position, limbImages[limbLookingAt].transform.position + Vector3.up * 60f, Time.unscaledDeltaTime * 4f);
		if ((bool)limb.GetComponent<TourniquetScript>())
		{
			specialUseButton.SetActive(value: true);
			specialUseText.text = Locale.GetOther("removetourniquet");
			specialUseTip.tipName = "";
		}
		else if (limb.hasShrapnel)
		{
			specialUseText.text = Locale.GetOther("removeshrapnel");
			specialUseButton.SetActive(value: true);
			specialUseTip.tipDesc = Locale.GetOther("removeshrapneldsc");
		}
		else if ((bool)limb.GetComponent<SplintLimb>())
		{
			specialUseText.text = Locale.GetOther("removesplint");
			specialUseButton.SetActive(value: true);
			specialUseTip.tipDesc = Locale.GetOther("removesplintdsc");
		}
		else if (limb.dislocated && !limb.prostheticInfo.hasProsthetic)
		{
			specialUseText.text = Locale.GetOther("fixdislocation");
			specialUseButton.SetActive(value: true);
			specialUseTip.tipDesc = Locale.GetOther("fixdislocationdsc");
		}
		else
		{
			specialUseButton.SetActive(value: false);
		}
		specialUseTip.tipName = specialUseText.text;
	}

	private void LayerTimerDisplay()
	{
		string text = "XXXXXXXXXXXXXXXXX";
		float num = Mathf.Max(WorldGeneration.world.maxTimePerLayer - WorldGeneration.world.layerTimeSpent, 0f);
		float num2 = Mathf.Clamp01(num / WorldGeneration.world.maxTimePerLayer);
		if (body.hasCargo)
		{
			timeTextBar.text = "   (RETURN--BASE)";
			timeTextBottom.text = "CARGOACQ-MSN-CARGO";
		}
		else if (WorldGeneration.world.biomeDepth == 9)
		{
			timeTextBar.text = "   (ALMOST THERE)";
			timeTextBottom.text = "XX:XX:XX | MSN-CARGO";
		}
		else if (WorldGeneration.world.biomeOverride == WorldGeneration.OverrideSceneType.Tutorial)
		{
			timeTextBar.text = "      (DISABLED)";
			timeTextBottom.text = "XX:XX:XX | TRAINING";
		}
		else if (num2 > 0f)
		{
			text = text.Substring(0, Mathf.CeilToInt(num2 * (float)text.Length));
			if ((int)(Time.time * 2f) % 2 == 0)
			{
				text = text.Substring(0, text.Length - 1);
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			timeTextBar.text = text;
			timeTextBottom.text = timeSpan.ToString("hh\\:mm\\:ss") + " | MSN-CARGO";
		}
		else
		{
			if ((int)(Time.time * 2f) % 2 == 0)
			{
				text = "";
			}
			timeTextBar.text = text;
			timeTextBottom.text = "00:00:00 | " + Locale.GetOther("missionhurry");
		}
	}

	private void UpdateSkillUI()
	{
		skillBars[0].fillAmount = body.skills.ToNextNormalized(body.skills.expSTR, body.skills.minSTR, body.skills.maxSTR);
		skillBars[1].fillAmount = body.skills.ToNextNormalized(body.skills.expRES, body.skills.minRES, body.skills.maxRES);
		skillBars[2].fillAmount = body.skills.ToNextNormalized(body.skills.expINT, body.skills.minINT, body.skills.maxINT);
		skillTexts[0].text = body.skills.STR.ToString() ?? "";
		skillTexts[1].text = body.skills.RES.ToString() ?? "";
		skillTexts[2].text = body.skills.INT.ToString() ?? "";
		skillTexts[3].text = body.skills.minSTR.ToString() ?? "";
		skillTexts[4].text = body.skills.minRES.ToString() ?? "";
		skillTexts[5].text = body.skills.minINT.ToString() ?? "";
		skillTexts[6].text = Mathf.RoundToInt(body.skills.expSTR).ToString() ?? "";
		skillTexts[7].text = Mathf.RoundToInt(body.skills.expRES).ToString() ?? "";
		skillTexts[8].text = Mathf.RoundToInt(body.skills.expINT).ToString() ?? "";
		skillTexts[9].text = body.skills.maxSTR.ToString() ?? "";
		skillTexts[10].text = body.skills.maxRES.ToString() ?? "";
		skillTexts[11].text = body.skills.maxINT.ToString() ?? "";
	}

	public (Sprite, Color) BleedImageFromLimb(Limb limb)
	{
		if (!(limb.bandageSlowAmount <= 0.5f))
		{
			return ((limb.totalBleedAmount > 20f) ? icons[15] : ((limb.totalBleedAmount > 8f) ? icons[14] : ((limb.totalBleedAmount > 3.75f) ? icons[13] : icons[12])), bleedGradient.Evaluate(limb.totalBleedAmount * 0.01f));
		}
		return ((limb.totalBleedAmount > 20f) ? icons[8] : ((limb.totalBleedAmount > 8f) ? icons[0] : ((limb.totalBleedAmount > 3.75f) ? icons[9] : icons[7])), bleedGradient.Evaluate(limb.totalBleedAmount * 0.01f));
	}

	private void Update()
	{
		checkTime += Time.unscaledDeltaTime;
		if (base.gameObject.activeInHierarchy)
		{
			UpdateView();
		}
		bool flag = !WorldGeneration.unchipped && body.isCriticallyDying;
		if (flag && !audio.isPlaying)
		{
			audio.Play();
		}
		else if (!flag && audio.isPlaying)
		{
			audio.Stop();
		}
	}
}
