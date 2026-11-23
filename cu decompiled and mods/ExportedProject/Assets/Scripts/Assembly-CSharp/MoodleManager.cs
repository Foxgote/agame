using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoodleManager : MonoBehaviour
{
	public Transform moodles;

	public static MoodleManager main;

	public Dictionary<string, Sprite> icons = new Dictionary<string, Sprite>();

	public Sprite[] backgroundIcons;

	public GameObject bonusMoodlePrefab;

	private float updateTime;

	private int moodleCount;

	private int mainCount;

	public bool sideMoodles;

	private Body body;

	private List<string> prevMoodles;

	public Sprite flashSprite;

	public static bool alwaysShowHidden;

	public static bool showSideMoodles
	{
		get
		{
			if (main.sideMoodles && !alwaysShowHidden && !(Input.mousePosition.y / PlayerCamera.uiScale < 100f))
			{
				return PlayerCamera.main.woundView.activeSelf;
			}
			return true;
		}
	}

	private void Awake()
	{
		main = this;
		icons = new Dictionary<string, Sprite>();
		Sprite[] array = Resources.LoadAll<Sprite>("Moodles");
		foreach (Sprite sprite in array)
		{
			icons.Add(sprite.name, sprite);
		}
	}

	private void Start()
	{
		body = PlayerCamera.main.body;
	}

	public void ClearMoodles()
	{
		foreach (Transform moodle in moodles)
		{
			UnityEngine.Object.Destroy(moodle.gameObject);
		}
		moodleCount = 0;
		mainCount = 0;
	}

	public void UpdatePrevMoodles()
	{
		prevMoodles = new List<string>();
		foreach (Transform moodle in moodles)
		{
			if ((bool)moodle.GetComponent<Moodle>())
			{
				prevMoodles.Add(moodle.GetComponent<Moodle>().type);
			}
		}
	}

	public void AddMoodle(int intensity, string icon, string name, string desc, bool critical = false, bool chippedOnly = false)
	{
		if (!chippedOnly || !WorldGeneration.unchipped)
		{
			GameObject gameObject = new GameObject("Moodle" + icon, typeof(Image));
			gameObject.transform.SetParent(moodles);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.5f);
			gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0.5f);
			gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(moodleCount * 70, (!critical) ? 0f : (Mathf.Sin(Time.unscaledTime * 6f) * 4f));
			gameObject.GetComponent<Image>().sprite = backgroundIcons[intensity];
			gameObject.GetComponent<Image>().SetNativeSize();
			UITooltip uITooltip = gameObject.AddComponent<UITooltip>();
			uITooltip.skipLocale = true;
			uITooltip.tipName = (critical ? "<color=\"red\">" : "") + name;
			uITooltip.tipDesc = desc;
			Moodle moodle = gameObject.AddComponent<Moodle>();
			moodle.type = icon + intensity;
			moodle.isSide = sideMoodles;
			GameObject gameObject2 = new GameObject("MoodleInside", typeof(Image));
			gameObject2.transform.SetParent(gameObject.transform);
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localScale = Vector3.one;
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.GetComponent<Image>().sprite = icons[icon];
			gameObject2.GetComponent<Image>().SetNativeSize();
			gameObject2.GetComponent<Image>().raycastTarget = false;
			if (critical && showSideMoodles)
			{
				GameObject gameObject3 = new GameObject("Flash", typeof(Image));
				gameObject3.transform.SetParent(gameObject.transform);
				gameObject3.transform.localScale = Vector3.one;
				gameObject3.transform.localPosition = Vector3.zero;
				gameObject3.GetComponent<Image>().sprite = flashSprite;
				gameObject3.GetComponent<Image>().SetNativeSize();
				gameObject3.GetComponent<Image>().raycastTarget = false;
				gameObject3.GetComponent<Image>().color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.unscaledTime * 2.5f, 1f));
				moodle.flash = gameObject3.GetComponent<Image>();
				moodle.doWarningFlash = true;
			}
			if (!prevMoodles.Contains(moodle.type))
			{
				gameObject.GetComponent<RectTransform>().anchoredPosition += Vector2.up * 75f;
				gameObject.GetComponent<RectTransform>().localScale = Vector2.one * 2.5f;
				gameObject.GetComponent<Moodle>().unTransparentTime = 0.5f;
			}
			if (!showSideMoodles)
			{
				gameObject.GetComponent<Image>().enabled = false;
				gameObject2.GetComponent<Image>().enabled = false;
			}
			if (!sideMoodles)
			{
				mainCount++;
			}
			moodleCount++;
		}
	}

	public void UpdateMoodles()
	{
		sideMoodles = false;
		updateTime = 0.5f;
		UpdatePrevMoodles();
		ClearMoodles();
		AddAllMoodles();
		if (body.overdoseIndex > 0)
		{
			body.overdoseIndex--;
		}
	}

	private void AddAllMoodles()
	{
		if (!body.alive || WorldGeneration.world.cutsceneActive)
		{
			return;
		}
		if (body.hasCargo)
		{
			AddMoodle(4, "cargo", Locale.GetMoodle("cargo"), Locale.GetMoodle("cargodsc"));
		}
		if (body.barotrauma > 100f)
		{
			AddMoodle(3, "barotrauma", Locale.GetMoodle("barotrauma4"), Locale.GetMoodle("barotrauma4dsc"), critical: true);
		}
		else if (body.barotrauma > 65f)
		{
			AddMoodle(2, "barotrauma", Locale.GetMoodle("barotrauma3"), Locale.GetMoodle("barotrauma3dsc"), critical: true);
		}
		else if (body.barotrauma > 30f)
		{
			AddMoodle(1, "barotrauma", Locale.GetMoodle("barotrauma2"), Locale.GetMoodle("barotrauma2dsc"), body.barotrauma > 50f, chippedOnly: true);
		}
		else if (body.barotrauma > 0f)
		{
			AddMoodle(0, "barotrauma", Locale.GetMoodle("barotrauma1"), Locale.GetMoodle("barotrauma1dsc"), critical: false, chippedOnly: true);
		}
		if (body.focusedLevel > 0f)
		{
			AddMoodle(8, "focused", Locale.GetMoodle("focused"), (WorldGeneration.world.biomeDepth == 11) ? Locale.GetMoodle("focuseddscalt") : Locale.GetMoodle("focuseddsc"), critical: true);
		}
		else if (body.horrifiedLevel > 0f)
		{
			if (body.horrifiedLevel <= 50f)
			{
				AddMoodle(1, "impendingdoom", Locale.GetMoodle("impendingdoom"), Locale.GetMoodle("impendingdoomdsc"));
			}
			else
			{
				AddMoodle(3, "horrified", Locale.GetMoodle("horrified"), Locale.GetMoodle("horrifieddsc"), critical: true);
			}
		}
		if (WorldGeneration.unchipped && !body.inCardiacArrest)
		{
			if (body.heartRate > 110f)
			{
				AddMoodle((body.heartRate > 160f) ? ((!(body.heartRate > 200f)) ? 1 : 2) : 0, "tachycardia", Locale.GetMoodle("tachycardia"), Locale.GetMoodle("tachycardiadsc"));
			}
			else if (body.heartRate < 60f)
			{
				AddMoodle((body.heartRate < 40f) ? 1 : 0, "bradycardia", Locale.GetMoodle("bradycardia"), Locale.GetMoodle("bradycardiadsc"));
			}
			if (body.fibrillationProgress > 50f || body.heartRate > 240f)
			{
				AddMoodle(3, "palpitations", Locale.GetMoodle("palpitations"), Locale.GetMoodle("palpitationsdsc"));
			}
		}
		if (body.lastStandTime > 0f)
		{
			AddMoodle(8, "lastleg", Locale.GetMoodle("laststand"), Locale.GetMoodle("laststanddsc"));
		}
		if (body.brainHealth < 30f)
		{
			AddMoodle(3, "braindamage", Locale.GetMoodle("braindamage4"), Locale.GetMoodle("braindamage4dsc"), critical: true, chippedOnly: true);
		}
		else if (body.brainHealth < 60f)
		{
			AddMoodle(2, "braindamage", Locale.GetMoodle("braindamage3"), Locale.GetMoodle("braindamage3dsc"), critical: false, chippedOnly: true);
		}
		else if (body.brainHealth < 80f)
		{
			AddMoodle(1, "braindamage", Locale.GetMoodle("braindamage2"), Locale.GetMoodle("braindamage2dsc"), critical: false, chippedOnly: true);
		}
		else if (body.brainHealth < 95f)
		{
			AddMoodle(0, "braindamage", Locale.GetMoodle("braindamage1"), Locale.GetMoodle("braindamage1dsc"), critical: false, chippedOnly: true);
		}
		if (body.strokeAmount > 70f)
		{
			AddMoodle(3, "stroke", Locale.GetMoodle("stroke"), Locale.GetMoodle("strokedsc"), critical: true, chippedOnly: true);
		}
		if (body.inCardiacArrest)
		{
			AddMoodle(3, "cardiacarrest", Locale.GetMoodle("cardiacarrest"), Locale.GetMoodle("cardiacarrestdsc"), critical: true);
		}
		else if (body.fibrillationProgress > 75f)
		{
			AddMoodle(3, "arrythmia", Locale.GetMoodle("fibrillation3"), Locale.GetMoodle("fibrillation3dsc"), critical: true, chippedOnly: true);
		}
		else if (body.fibrillationProgress > 50f)
		{
			AddMoodle(2, "arrythmia", Locale.GetMoodle("fibrillation2"), Locale.GetMoodle("fibrillation2dsc"), critical: true, chippedOnly: true);
		}
		else if (body.fibrillationProgress > 15f)
		{
			AddMoodle(1, "arrythmia", Locale.GetMoodle("fibrillation1"), Locale.GetMoodle("fibrillation1dsc"), critical: false, chippedOnly: true);
		}
		if (!body.inCardiacArrest)
		{
			if (body.bloodPressure < 60f)
			{
				AddMoodle(3, "hypotension", Locale.GetMoodle("hypotension4"), Locale.GetMoodle("hypotension4dsc"), critical: true, chippedOnly: true);
			}
			else if (body.bloodPressure < 83f)
			{
				AddMoodle(2, "hypotension", Locale.GetMoodle("hypotension3"), Locale.GetMoodle("hypotension3dsc"), body.bloodPressure < 70f, chippedOnly: true);
			}
			else if (body.bloodPressure < 96f)
			{
				AddMoodle(1, "hypotension", Locale.GetMoodle("hypotension2"), Locale.GetMoodle("hypotension2dsc"), critical: false, chippedOnly: true);
			}
			else if (body.bloodPressure < 110f)
			{
				AddMoodle(0, "hypotension", Locale.GetMoodle("hypotension1"), Locale.GetMoodle("hypotension1dsc"), critical: false, chippedOnly: true);
			}
			if (body.bloodPressure > 180f)
			{
				AddMoodle(3, "hypertension", Locale.GetMoodle("hypertension4"), Locale.GetMoodle("hypertension4dsc"), critical: true, chippedOnly: true);
			}
			else if (body.bloodPressure > 162f)
			{
				AddMoodle(2, "hypertension", Locale.GetMoodle("hypertension3"), Locale.GetMoodle("hypertension3dsc"), critical: false, chippedOnly: true);
			}
			else if (body.bloodPressure > 145f)
			{
				AddMoodle(1, "hypertension", Locale.GetMoodle("hypertension2"), Locale.GetMoodle("hypertension2dsc"), critical: false, chippedOnly: true);
			}
			else if (body.bloodPressure > 130f)
			{
				AddMoodle(0, "hypertension", Locale.GetMoodle("hypertension1"), Locale.GetMoodle("hypertension1dsc"), critical: false, chippedOnly: true);
			}
		}
		if (!body.breathing)
		{
			AddMoodle(3, "cantbreathe", Locale.GetMoodle("cantbreathe"), Locale.GetMoodle("cantbreathedsc"), critical: true);
		}
		else if (body.respiratoryRate < 50f)
		{
			AddMoodle(1, "hypoventilation", Locale.GetMoodle("hypoventilation2"), Locale.GetMoodle("hypoventilation2dsc"));
		}
		else if (body.respiratoryRate < 90f)
		{
			AddMoodle(0, "hypoventilation", Locale.GetMoodle("hypoventilation1"), Locale.GetMoodle("hypoventilation1dsc"));
		}
		if (body.limbs[1].muscleHealth < 4f)
		{
			AddMoodle(3, "thoraxdestroyed", Locale.GetMoodle("thoraxdestroyed"), Locale.GetMoodle("thoraxdestroyeddsc"), critical: true);
		}
		if (body.hemothorax > 40f)
		{
			AddMoodle(1, "hemothorax", Locale.GetMoodle("hemothorax"), Locale.GetMoodle("hemothoraxdsc"), body.hemothorax > 70f, chippedOnly: true);
		}
		if (body.hasPulmonaryEmbolism)
		{
			AddMoodle(3, "pulmonaryembolism", Locale.GetMoodle("pulmonaryembolism"), Locale.GetMoodle("pulmonaryembolismdsc"), critical: true, chippedOnly: true);
		}
		if (body.bloodOxygen < 45f)
		{
			AddMoodle(3, "oxygen", Locale.GetMoodle("oxygen4"), Locale.GetMoodle("oxygen4dsc"), critical: false, chippedOnly: true);
		}
		else if (body.bloodOxygen < 60f)
		{
			AddMoodle(2, "oxygen", Locale.GetMoodle("oxygen3"), Locale.GetMoodle("oxygen3dsc"), critical: false, chippedOnly: true);
		}
		else if (body.bloodOxygen < 75f)
		{
			AddMoodle(1, "oxygen", Locale.GetMoodle("oxygen2"), Locale.GetMoodle("oxygen2dsc"), critical: false, chippedOnly: true);
		}
		else if (body.bloodOxygen < 90f)
		{
			AddMoodle(0, "oxygen", Locale.GetMoodle("oxygen1"), Locale.GetMoodle("oxygen1dsc"), critical: false, chippedOnly: true);
		}
		if (body.radiationSickness > 80f)
		{
			AddMoodle(3, "irradiated", Locale.GetMoodle("irradiated4"), Locale.GetMoodle("irradiated4dsc"), critical: true, chippedOnly: true);
		}
		else if (body.radiationSickness > 50f)
		{
			AddMoodle(2, "irradiated", Locale.GetMoodle("irradiated3"), Locale.GetMoodle("irradiated3dsc"), critical: true, chippedOnly: true);
		}
		else if (body.radiationSickness > 30f)
		{
			AddMoodle(1, "irradiated", Locale.GetMoodle("irradiated2"), Locale.GetMoodle("irradiated2dsc"), critical: false, chippedOnly: true);
		}
		else if (body.radiationSickness > 10f)
		{
			AddMoodle(0, "irradiated", Locale.GetMoodle("irradiated1"), Locale.GetMoodle("irradiated1dsc"), critical: false, chippedOnly: true);
		}
		if (body.averagePain > 80f)
		{
			AddMoodle(3, "pain", Locale.GetMoodle("pain4"), Locale.GetMoodle("pain4dsc"), critical: true);
		}
		else if (body.averagePain > 55f)
		{
			AddMoodle(2, "pain", Locale.GetMoodle("pain3"), Locale.GetMoodle("pain3dsc"));
		}
		else if (body.averagePain > 30f)
		{
			AddMoodle(1, "pain", Locale.GetMoodle("pain2"), Locale.GetMoodle("pain2dsc"));
		}
		else if (body.averagePain > 10f)
		{
			AddMoodle(0, "pain", Locale.GetMoodle("pain1"), Locale.GetMoodle("pain1dsc"));
		}
		if (body.painShock > 0.3f)
		{
			AddMoodle(3, "shock", Locale.GetMoodle("shock"), Locale.GetMoodle("shockdsc"));
		}
		if (body.TryGetComponent<Painkillers>(out var component))
		{
			if (component.actualOpiateReception > 80f)
			{
				AddMoodle(3, "overdose", Locale.GetMoodle("overdose4"), Locale.GetMoodle("overdose4dsc"), critical: true, chippedOnly: true);
			}
			else if (component.actualOpiateReception > 50f)
			{
				AddMoodle(2, "overdose", Locale.GetMoodle("overdose3"), Locale.GetMoodle("overdose3dsc"), critical: false, chippedOnly: true);
			}
			else if (component.actualOpiateReception > 20f)
			{
				AddMoodle(1, "overdose", Locale.GetMoodle("overdose2"), Locale.GetMoodle("overdose2dsc"), critical: false, chippedOnly: true);
			}
			else if (component.actualOpiateReception > 5f)
			{
				AddMoodle(0, "overdose", Locale.GetMoodle("overdose1"), Locale.GetMoodle("overdose1dsc"), critical: false, chippedOnly: true);
			}
			if (component.actualOpiateReception < -34f)
			{
				AddMoodle(3, "withdrawal", Locale.GetMoodle("withdrawal4"), Locale.GetMoodle("withdrawal4dsc"), critical: true, chippedOnly: true);
			}
			else if (component.actualOpiateReception < -25f)
			{
				AddMoodle(2, "withdrawal", Locale.GetMoodle("withdrawal3"), Locale.GetMoodle("withdrawal3dsc"), critical: false, chippedOnly: true);
			}
			else if (component.actualOpiateReception < -15f)
			{
				AddMoodle(1, "withdrawal", Locale.GetMoodle("withdrawal2"), Locale.GetMoodle("withdrawal2dsc"), critical: false, chippedOnly: true);
			}
			else if (component.actualOpiateReception < -5f)
			{
				AddMoodle(0, "withdrawal", Locale.GetMoodle("withdrawal1"), Locale.GetMoodle("withdrawal1dsc"), critical: false, chippedOnly: true);
			}
		}
		if (body.onHardStimulants)
		{
			AddMoodle(6, "stimulants", Locale.GetMoodle("stimulated"), Locale.GetMoodle("stimulateddsc"), critical: false, chippedOnly: true);
		}
		if (body.limbs[0].muscleHealth < 14.4f)
		{
			AddMoodle(3, "concussion", Locale.GetMoodle("concussion"), Locale.GetMoodle("concussiondsc"), critical: true, chippedOnly: true);
		}
		else if (body.consciousness <= 20f)
		{
			if (!body.sleeping)
			{
				AddMoodle(3, "unconscious", Locale.GetMoodle("unconscious"), Locale.GetMoodle("unconsciousdsc"));
			}
			else
			{
				AddMoodle(4, "asleep", Locale.GetMoodle("asleep"), Locale.GetMoodle("asleepdsc"));
			}
		}
		else if (body.consciousness < 30f)
		{
			AddMoodle(3, "confused", Locale.GetMoodle("incapacitated"), Locale.GetMoodle("incapacitateddsc"));
		}
		else if (body.consciousness < 55f)
		{
			AddMoodle(2, "confused", Locale.GetMoodle("confused3"), Locale.GetMoodle("confused3dsc"));
		}
		else if (body.consciousness < 72f)
		{
			AddMoodle(1, "confused", Locale.GetMoodle("confused2"), Locale.GetMoodle("confused2dsc"));
		}
		else if (body.consciousness < 90f)
		{
			AddMoodle(0, "confused", Locale.GetMoodle("confused1"), Locale.GetMoodle("confused1dsc"));
		}
		if (body.overdoseIndex > 0)
		{
			AddMoodle(3, "drugoverdose", Locale.GetMoodle("drugoverdose"), Locale.GetMoodle("drugoverdosedsc"), critical: true, chippedOnly: true);
		}
		if (body.internalBleeding > 5f)
		{
			AddMoodle((int)((body.internalBleeding - 1f) / 25f), "internalBleed", Locale.GetMoodle("internalBleed"), Locale.GetMoodle("internalBleeddsc"), body.internalBleeding > 50f, chippedOnly: true);
		}
		if (body.totalBleedSpeed > 0.3f)
		{
			AddMoodle(3, "maxbleeding", Locale.GetMoodle("bleeding4"), Locale.GetMoodle("bleeding4dsc"), critical: true);
		}
		else if (body.totalBleedSpeed > 0.15f)
		{
			AddMoodle(2, "heavybleeding", Locale.GetMoodle("bleeding3"), Locale.GetMoodle("bleeding3dsc"), critical: true);
		}
		else if (body.totalBleedSpeed > 0.06f)
		{
			AddMoodle(1, "bleeding", Locale.GetMoodle("bleeding2"), Locale.GetMoodle("bleeding2dsc"));
		}
		else if (body.totalBleedSpeed > 0f)
		{
			AddMoodle(0, "smallbleeding", Locale.GetMoodle("bleeding1"), Locale.GetMoodle("bleeding1dsc"));
		}
		if (body.stamina <= 15f)
		{
			AddMoodle(3, "exertion", Locale.GetMoodle("exertion4"), Locale.GetMoodle("exertion4dsc").Replace("<>", Mathf.RoundToInt(body.stamina).ToString()), critical: true);
		}
		else if (body.stamina < 35f)
		{
			AddMoodle(2, "exertion", Locale.GetMoodle("exertion3"), Locale.GetMoodle("exertion3dsc").Replace("<>", Mathf.RoundToInt(body.stamina).ToString()));
		}
		else if (body.stamina < 50f)
		{
			AddMoodle(1, "exertion", Locale.GetMoodle("exertion2"), Locale.GetMoodle("exertion2dsc").Replace("<>", Mathf.RoundToInt(body.stamina).ToString()));
		}
		else if (body.stamina < 70f)
		{
			AddMoodle(0, "exertion", Locale.GetMoodle("exertion1"), Locale.GetMoodle("exertion1dsc").Replace("<>", Mathf.RoundToInt(body.stamina).ToString()));
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		int num4 = 0;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		Limb[] limbs = body.limbs;
		foreach (Limb limb in limbs)
		{
			if (limb.dismembered)
			{
				flag5 = true;
				continue;
			}
			if (limb.prostheticInfo.hasProsthetic)
			{
				flag6 = true;
				if (limb.broken)
				{
					flag2 = true;
				}
				if (limb.dislocated)
				{
					flag = true;
				}
				if (limb.broken || limb.hasShrapnel)
				{
					flag3 = true;
				}
			}
			else
			{
				if (limb.infectionAmount > num)
				{
					num = limb.infectionAmount;
				}
				if (limb.broken && limb.boneHealTimer > num2)
				{
					num2 = limb.boneHealTimer;
				}
				if (limb.dislocated && limb.dislocationTimer > num3)
				{
					num3 = limb.dislocationTimer;
				}
				if (PlayerCamera.main.showInfection[num4])
				{
					flag4 = true;
				}
			}
			num4++;
		}
		if (num2 > 0f)
		{
			AddMoodle(Mathf.RoundToInt(num2 * 0.03f), "brokenbone", Locale.GetMoodle("brokenbone"), Locale.GetMoodle("brokenbonedsc"));
		}
		if (num3 > 0f)
		{
			AddMoodle(Mathf.RoundToInt(num3 * 0.03f), "dislocation", Locale.GetMoodle("dislocation"), Locale.GetMoodle("dislocationdsc"));
		}
		if (body.limbs[0].broken)
		{
			AddMoodle(Mathf.RoundToInt(body.limbs[0].boneHealTimer * 0.03f), "brokenneck", Locale.GetMoodle("brokenneck"), Locale.GetMoodle("brokenneckdsc"), critical: false, chippedOnly: true);
		}
		if (body.limbs[1].broken)
		{
			AddMoodle(Mathf.RoundToInt(body.limbs[1].boneHealTimer * 0.03f), "brokenribs", Locale.GetMoodle("brokenribs"), Locale.GetMoodle("brokenribsdsc"), critical: false, chippedOnly: true);
		}
		if (body.limbs[0].dislocated)
		{
			AddMoodle(Mathf.RoundToInt(body.limbs[0].dislocationTimer * 0.03f), "dislocatedjaw", Locale.GetMoodle("dislocatedjaw"), Locale.GetMoodle("dislocatedjawdsc"));
		}
		if (body.limbs[1].dislocated)
		{
			AddMoodle(Mathf.RoundToInt(body.limbs[1].dislocationTimer * 0.03f), "dislocatedspine", Locale.GetMoodle("dislocatedspine"), Locale.GetMoodle("dislocatedspinedsc"), critical: false, chippedOnly: true);
		}
		if (num > 80f)
		{
			AddMoodle(3, "infected", Locale.GetMoodle("infected4"), Locale.GetMoodle("infected4dsc"));
		}
		else if (num > 60f)
		{
			AddMoodle(2, "infected", Locale.GetMoodle("infected3"), Locale.GetMoodle("infected3dsc"));
		}
		else if (num > 40f)
		{
			AddMoodle(1, "infected", Locale.GetMoodle("infected2"), Locale.GetMoodle("infected2dsc"));
		}
		else if ((num > 25f) | flag4)
		{
			AddMoodle(0, "infected", Locale.GetMoodle("infected1"), Locale.GetMoodle("infected1dsc"), critical: false, chippedOnly: true);
		}
		if (body.septicShock > 80f)
		{
			AddMoodle(3, "sepsis", Locale.GetMoodle("sepsis3"), Locale.GetMoodle("sepsis3dsc"), critical: true, chippedOnly: true);
		}
		else if (body.septicShock > 50f)
		{
			AddMoodle(2, "sepsis", Locale.GetMoodle("sepsis2"), Locale.GetMoodle("sepsis2dsc"), critical: false, chippedOnly: true);
		}
		else if (body.septicShock > 10f)
		{
			AddMoodle(1, "sepsis", Locale.GetMoodle("sepsis1"), Locale.GetMoodle("sepsis1dsc"), critical: false, chippedOnly: true);
		}
		float num5 = body.venomCurrent + body.venomFake;
		if (num5 > 90f)
		{
			AddMoodle(3, "venom", Locale.GetMoodle("venom4"), Locale.GetMoodle("venom4dsc"), critical: true);
		}
		else if (num5 > 55f)
		{
			AddMoodle(2, "venom", Locale.GetMoodle("venom3"), Locale.GetMoodle("venom3dsc"));
		}
		else if (num5 > 25f)
		{
			AddMoodle(1, "venom", Locale.GetMoodle("venom2"), Locale.GetMoodle("venom2dsc"));
		}
		else if (num5 > 2f)
		{
			AddMoodle(0, "venom", Locale.GetMoodle("venom1"), Locale.GetMoodle("venom1dsc"));
		}
		if (body.energy <= 7f)
		{
			AddMoodle(3, "tired", Locale.GetMoodle("tired4"), Locale.GetMoodle("tired4dsc"), critical: true);
		}
		else if (body.energy < 15f)
		{
			AddMoodle(2, "tired", Locale.GetMoodle("tired3"), Locale.GetMoodle("tired3dsc"));
		}
		else if (body.energy < 25f)
		{
			AddMoodle(1, "tired", Locale.GetMoodle("tired2"), Locale.GetMoodle("tired2dsc"));
		}
		else if (body.energy < 35f)
		{
			AddMoodle(0, "tired", Locale.GetMoodle("tired1"), Locale.GetMoodle("tired1dsc"));
		}
		float num6 = Mathf.Clamp01(1f + body.happiness * 0.005f);
		if (body.hunger <= 17.5f * num6)
		{
			AddMoodle(3, "hunger", Locale.GetMoodle("hunger4"), Locale.GetMoodle("hunger4dsc"), body.hunger <= 0f);
		}
		else if (body.hunger < 40f * num6)
		{
			AddMoodle(2, "hunger", Locale.GetMoodle("hunger3"), Locale.GetMoodle("hunger3dsc"));
		}
		else if (body.hunger < 60f * num6)
		{
			AddMoodle(1, "hunger", Locale.GetMoodle("hunger2"), Locale.GetMoodle("hunger2dsc"));
		}
		else if (body.hunger < 80f * num6)
		{
			AddMoodle(0, "hunger", Locale.GetMoodle("hunger1"), Locale.GetMoodle("hunger1dsc"));
		}
		if (body.hunger > 120f)
		{
			AddMoodle(5, "hunger", Locale.GetMoodle("hunger6"), Locale.GetMoodle("hunger6dsc"));
		}
		else if (body.hunger > 100f)
		{
			AddMoodle(4, "hunger", Locale.GetMoodle("hunger5"), Locale.GetMoodle("hunger5dsc"));
		}
		if (body.thirst <= 20f)
		{
			AddMoodle(3, "thirst", Locale.GetMoodle("thirst4"), Locale.GetMoodle("thirst4dsc"), body.thirst <= 0f);
		}
		else if (body.thirst < 40f)
		{
			AddMoodle(2, "thirst", Locale.GetMoodle("thirst3"), Locale.GetMoodle("thirst3dsc"));
		}
		else if (body.thirst < 60f)
		{
			AddMoodle(1, "thirst", Locale.GetMoodle("thirst2"), Locale.GetMoodle("thirst2dsc"));
		}
		else if (body.thirst < 75f)
		{
			AddMoodle(0, "thirst", Locale.GetMoodle("thirst1"), Locale.GetMoodle("thirst1dsc"));
		}
		if (body.thirst > 175f)
		{
			AddMoodle(3, "overhydrated", Locale.GetMoodle("overhydrated3"), Locale.GetMoodle("overhydrated3dsc"), critical: true);
		}
		else if (body.thirst > 125f)
		{
			AddMoodle(1, "overhydrated", Locale.GetMoodle("overhydrated2"), Locale.GetMoodle("overhydrated2dsc"));
		}
		else if (body.thirst > 100f)
		{
			AddMoodle(0, "overhydrated", Locale.GetMoodle("overhydrated1"), Locale.GetMoodle("overhydrated1dsc"));
		}
		if (body.sicknessAmount > 75f)
		{
			AddMoodle(3, "sick", Locale.GetMoodle("sick4"), Locale.GetMoodle("sick4dsc"), body.sicknessAmount > 95f);
		}
		else if (body.sicknessAmount > 50f)
		{
			AddMoodle(2, "sick", Locale.GetMoodle("sick3"), Locale.GetMoodle("sick3dsc"));
		}
		else if (body.sicknessAmount > 30f)
		{
			AddMoodle(1, "sick", Locale.GetMoodle("sick2"), Locale.GetMoodle("sick2dsc"));
		}
		else if (body.sicknessAmount > 10f)
		{
			AddMoodle(0, "sick", Locale.GetMoodle("sick1"), Locale.GetMoodle("sick1dsc"));
		}
		if (body.temperature > 41.5f)
		{
			AddMoodle(3, "hot", Locale.GetMoodle("hot4"), Locale.GetMoodle("hot4dsc"), critical: true);
		}
		else if (body.temperature > 40.25f)
		{
			AddMoodle(2, "hot", Locale.GetMoodle("hot3"), Locale.GetMoodle("hot3dsc"));
		}
		else if (body.temperature > 39f)
		{
			AddMoodle(1, "hot", Locale.GetMoodle("hot2"), Locale.GetMoodle("hot2dsc"));
		}
		else if (body.temperature > 38f)
		{
			AddMoodle(0, "hot", Locale.GetMoodle("hot1"), Locale.GetMoodle("hot1dsc"));
		}
		if (body.temperature < 28f)
		{
			AddMoodle(3, "cold", Locale.GetMoodle("cold4"), Locale.GetMoodle("cold4dsc"), critical: true);
		}
		else if (body.temperature < 32.5f)
		{
			AddMoodle(2, "cold", Locale.GetMoodle("cold3"), Locale.GetMoodle("cold3dsc"));
		}
		else if (body.temperature < 34f)
		{
			AddMoodle(1, "cold", Locale.GetMoodle("cold2"), Locale.GetMoodle("cold2dsc"));
		}
		else if (body.temperature < 35.5f)
		{
			AddMoodle(0, "cold", Locale.GetMoodle("cold1"), Locale.GetMoodle("cold1dsc"));
		}
		if (body.totalHappiness <= -75f)
		{
			AddMoodle(3, "miserable", Locale.GetMoodle("depression4"), Locale.GetMoodle("depression4dsc"), body.harmer.timeWasStill > 20f);
		}
		else if (body.totalHappiness < -50f)
		{
			AddMoodle(2, "depression", Locale.GetMoodle("depression3"), Locale.GetMoodle("depression3dsc"), body.harmer.timeWasStill > 20f);
		}
		else if (body.totalHappiness < -30f)
		{
			AddMoodle(1, "gloomy", Locale.GetMoodle("depression2"), Locale.GetMoodle("depression2dsc"));
		}
		else if (body.totalHappiness < -10f)
		{
			AddMoodle(0, "sad", Locale.GetMoodle("depression1"), Locale.GetMoodle("depression1dsc"));
		}
		if (body.totalHappiness >= 75f)
		{
			AddMoodle(7, "happy", Locale.GetMoodle("happy4"), Locale.GetMoodle("happy4dsc"));
		}
		else if (body.totalHappiness > 50f)
		{
			AddMoodle(6, "happy", Locale.GetMoodle("happy3"), Locale.GetMoodle("happy3dsc"));
		}
		else if (body.totalHappiness > 30f)
		{
			AddMoodle(5, "happy", Locale.GetMoodle("happy2"), Locale.GetMoodle("happy2dsc"));
		}
		else if (body.totalHappiness > 10f)
		{
			AddMoodle(4, "happy", Locale.GetMoodle("happy1"), Locale.GetMoodle("happy1dsc"));
		}
		if (body.clawHealth < 25f)
		{
			AddMoodle(1, "clawdamage", Locale.GetMoodle("clawdamage2"), Locale.GetMoodle("clawdamage2dsc"));
		}
		else if (body.clawHealth < 60f)
		{
			AddMoodle(0, "clawdamage", Locale.GetMoodle("clawdamage1"), Locale.GetMoodle("clawdamage1dsc"));
		}
		if (body.hearingLoss > 70f)
		{
			AddMoodle(2, "hearingloss", Locale.GetMoodle("deaf4"), Locale.GetMoodle("deaf4dsc"));
		}
		else if (body.hearingLoss > 40f)
		{
			AddMoodle(1, "hearingloss", Locale.GetMoodle("deaf3"), Locale.GetMoodle("deaf3dsc"));
		}
		else if (body.hearingLoss > 15f)
		{
			AddMoodle(0, "hearingloss", Locale.GetMoodle("deaf2"), Locale.GetMoodle("deaf2dsc"));
		}
		if (body.dirtyness > 80f)
		{
			AddMoodle(1, "dirty", Locale.GetMoodle("dirty2"), Locale.GetMoodle("dirty2dsc").Replace("<>", Mathf.RoundToInt(body.dirtyness).ToString()));
		}
		else if (body.dirtyness > 50f)
		{
			AddMoodle(0, "dirty", Locale.GetMoodle("dirty1"), Locale.GetMoodle("dirty1dsc").Replace("<>", Mathf.RoundToInt(body.dirtyness).ToString()));
		}
		if (body.overEncumberance > 0.85f)
		{
			AddMoodle(3, "encumbered", Locale.GetMoodle("encumbered4"), Locale.GetMoodle("encumbered4dsc").Replace("<>", $"{body.overEncumberance * 100f:F0}%"), critical: true);
		}
		else if (body.overEncumberance > 0.55f)
		{
			AddMoodle(2, "encumbered", Locale.GetMoodle("encumbered3"), Locale.GetMoodle("encumbered3dsc").Replace("<>", $"{body.overEncumberance * 100f:F0}%"));
		}
		else if (body.overEncumberance > 0.3f)
		{
			AddMoodle(1, "encumbered", Locale.GetMoodle("encumbered2"), Locale.GetMoodle("encumbered2dsc").Replace("<>", $"{body.overEncumberance * 100f:F0}%"));
		}
		else if (body.overEncumberance > 0f)
		{
			AddMoodle(0, "encumbered", Locale.GetMoodle("encumbered1"), Locale.GetMoodle("encumbered1dsc").Replace("<>", $"{body.overEncumberance * 100f:F0}%"));
		}
		if (body.wetness > 70f)
		{
			AddMoodle(3, "wet", Locale.GetMoodle("wet4"), Locale.GetMoodle("wet4dsc"));
		}
		else if (body.wetness > 45f)
		{
			AddMoodle(2, "wet", Locale.GetMoodle("wet3"), Locale.GetMoodle("wet3dsc"));
		}
		else if (body.wetness > 20f)
		{
			AddMoodle(1, "wet", Locale.GetMoodle("wet2"), Locale.GetMoodle("wet2dsc"));
		}
		else if (body.wetness > 5f)
		{
			AddMoodle(0, "wet", Locale.GetMoodle("wet1"), Locale.GetMoodle("wet1dsc"));
		}
		if (flag3)
		{
			AddMoodle(9, "loosescrews", Locale.GetMoodle("loosescrews"), Locale.GetMoodle("loosescrewsdsc"), critical: false, chippedOnly: true);
		}
		if (flag)
		{
			AddMoodle(9, "bentprosthesis", Locale.GetMoodle("bentprosthesis"), Locale.GetMoodle("bentprosthesisdsc"));
		}
		if (flag2)
		{
			AddMoodle(9, "severematerialloss", Locale.GetMoodle("materialloss"), Locale.GetMoodle("materiallossdsc"));
		}
		sideMoodles = true;
		if (flag6)
		{
			AddMoodle(9, "prosthetic", Locale.GetMoodle("prosthesis"), Locale.GetMoodle("prosthesisdsc"));
		}
		if (body.immunity > 150f)
		{
			AddMoodle(5, "highimmunity", Locale.GetMoodle("highimmunity"), Locale.GetMoodle("highimmunitydsc"), critical: false, chippedOnly: true);
		}
		else if (body.immunity < 55f)
		{
			AddMoodle(1, "lowimmunity", Locale.GetMoodle("lowimmunity"), Locale.GetMoodle("lowimmunitydsc"), critical: false, chippedOnly: true);
		}
		if (body.clawRegrowTime > 0f)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(body.clawRegrowTime);
			AddMoodle(5, "keratinbooster", Locale.GetMoodle("keratinbooster"), Locale.GetMoodle("keratinboosterdsc").Replace("<>", $"{timeSpan:h\\:m\\:ss}"), critical: false, chippedOnly: true);
		}
		if (body.weightOffset <= -50f)
		{
			AddMoodle(3, "underweight", Locale.GetMoodle("underweight4"), Locale.GetMoodle("underweight4dsc"));
		}
		else if (body.weightOffset < -40f)
		{
			AddMoodle(2, "underweight", Locale.GetMoodle("underweight3"), Locale.GetMoodle("underweight3dsc"));
		}
		else if (body.weightOffset < -30f)
		{
			AddMoodle(1, "underweight", Locale.GetMoodle("underweight2"), Locale.GetMoodle("underweight2dsc"), critical: false, chippedOnly: true);
		}
		else if (body.weightOffset < -15f)
		{
			AddMoodle(0, "underweight", Locale.GetMoodle("underweight1"), Locale.GetMoodle("underweight1dsc"), critical: false, chippedOnly: true);
		}
		if (body.weightOffset >= 50f)
		{
			AddMoodle(3, "overweight", Locale.GetMoodle("overweight4"), Locale.GetMoodle("overweight4dsc"));
		}
		else if (body.weightOffset > 40f)
		{
			AddMoodle(2, "overweight", Locale.GetMoodle("overweight3"), Locale.GetMoodle("overweight3dsc"));
		}
		else if (body.weightOffset > 30f)
		{
			AddMoodle(1, "overweight", Locale.GetMoodle("overweight2"), Locale.GetMoodle("overweight2dsc"), critical: false, chippedOnly: true);
		}
		else if (body.weightOffset > 15f)
		{
			AddMoodle(0, "overweight", Locale.GetMoodle("overweight1"), Locale.GetMoodle("overweight1dsc"), critical: false, chippedOnly: true);
		}
		if (body.traumaAmount > 80f)
		{
			AddMoodle(3, "trauma", Locale.GetMoodle("trauma4"), Locale.GetMoodle("trauma4dsc"));
		}
		else if (body.traumaAmount > 45f)
		{
			AddMoodle(2, "trauma", Locale.GetMoodle("trauma3"), Locale.GetMoodle("trauma3dsc"));
		}
		else if (body.traumaAmount > 25f)
		{
			AddMoodle(1, "trauma", Locale.GetMoodle("trauma2"), Locale.GetMoodle("trauma2dsc"));
		}
		if (body.caffeinated > 0f)
		{
			TimeSpan timeSpan2 = TimeSpan.FromSeconds(body.caffeinated);
			AddMoodle(5, "energized", Locale.GetMoodle("energized"), Locale.GetMoodle("energizeddsc").Replace("<>", $"{(int)timeSpan2.TotalHours:D2}:{timeSpan2.Minutes:D2}:{timeSpan2.Seconds:D2}"), critical: false, chippedOnly: true);
		}
		if (body.badSleepAmount > 0f)
		{
			AddMoodle(0, "badsleep", Locale.GetMoodle("badsleep"), Locale.GetMoodle("badsleepdsc"), critical: false, chippedOnly: true);
		}
		if (body.conscious && body.talker.impairedSpeech)
		{
			AddMoodle(0, "impairedspeech", Locale.GetMoodle("impairedspeech"), Locale.GetMoodle("impairedspeechdsc"));
		}
		if (body.brainGrowSickness > 0f)
		{
			TimeSpan timeSpan3 = TimeSpan.FromSeconds(body.brainGrowSickness);
			AddMoodle(0, "rnrs", Locale.GetMoodle("rnrs"), Locale.GetMoodle("rnrsdsc").Replace("<>", $"{(int)timeSpan3.TotalHours:D2}:{timeSpan3.Minutes:D2}:{timeSpan3.Seconds:D2}"), critical: false, chippedOnly: true);
		}
		if (body.disfigured)
		{
			AddMoodle(3, "rippedjaw", Locale.GetMoodle("rippedjaw"), Locale.GetMoodle("rippedjawdsc"));
		}
		if (flag5)
		{
			AddMoodle(3, "amputation", Locale.GetMoodle("amputation"), Locale.GetMoodle("amputationdsc"));
		}
		if (body.bothEyesGone)
		{
			AddMoodle(3, "rippedeye", Locale.GetMoodle("rippedeyes"), Locale.GetMoodle("rippedeyesdsc"));
		}
		else if (body.eyeGone)
		{
			AddMoodle(2, "rippedeye", Locale.GetMoodle("rippedeye"), Locale.GetMoodle("rippedeyedsc"));
		}
		if (body.curAdrenaline > 20f)
		{
			AddMoodle((body.curAdrenaline > 65f) ? 1 : 0, "fightorflight", Locale.GetMoodle("adrenaline"), Locale.GetMoodle("adrenalinedsc"), critical: false, chippedOnly: true);
		}
		if ((bool)body.mindWipe && body.mindWipe.active)
		{
			AddMoodle(0, "hollow", Locale.GetMoodle("hollow"), Locale.GetMoodle("hollowdsc"));
		}
		if (moodleCount - mainCount > 0)
		{
			GameObject obj = UnityEngine.Object.Instantiate(bonusMoodlePrefab, moodles);
			obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(mainCount * 70 - 10, 0f);
			obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + (moodleCount - mainCount);
		}
	}

	private void Update()
	{
		updateTime -= Time.unscaledDeltaTime;
		if (updateTime <= 0f)
		{
			UpdateMoodles();
		}
	}
}
