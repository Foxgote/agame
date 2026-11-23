using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class Talker : MonoBehaviour
{
	private struct DelayTalkInfo
	{
		public float delay;

		public string text;

		public Limb limb;

		public bool force;

		public bool resetTalkTimer;

		public int layerRestrict;
	}

	public TextMeshPro text;

	public GameObject textPrefab;

	public Body body;

	private float timeSinceTalked;

	private string currentString;

	private float newTalkTime;

	private string lastLineType;

	private bool wasConscious;

	public static char[] fullMouthChars;

	public static char[] underwaterChars;

	public static bool updatedChars;

	public int lastStringProgress;

	public AudioClip talkSoundCustom;

	public TraderScript trader;

	public float talkTimeMult;

	public bool isSoundGlobal;

	public bool impairedSpeech
	{
		get
		{
			if (!body.HoldingItem(2) && !body.limbs[0].dislocated)
			{
				return body.disfigured;
			}
			return true;
		}
	}

	public bool brainDamaged
	{
		get
		{
			if (!(body.brainHealth < 82.5f))
			{
				return body.strokeAmount > 20f;
			}
			return true;
		}
	}

	public float clampedUnscaledDeltaTime => Mathf.Clamp(Time.unscaledDeltaTime, 0f, 0.2f);

	private void Start()
	{
		if (!updatedChars)
		{
			updatedChars = true;
			fullMouthChars = Locale.GetOther("fullmouthchars").ToCharArray();
			underwaterChars = Locale.GetOther("underwaterchars").ToCharArray();
		}
		body = GetComponent<Body>();
		currentString = "";
		if (!text)
		{
			text = UnityEngine.Object.Instantiate(textPrefab, base.transform.position, Quaternion.identity).GetComponent<TextMeshPro>();
		}
		text.text = "";
		lastLineType = "";
		wasConscious = true;
		lastStringProgress = -1;
	}

	public void EatGood()
	{
		if (body.hunger >= 125f)
		{
			Talk(Locale.GetCharacter("full"));
		}
		else if (body.hunger < 100f)
		{
			Talk(Locale.GetCharacter("eatGood"));
		}
	}

	public void EatMediocre()
	{
		if (body.hunger >= 125f)
		{
			Talk(Locale.GetCharacter("full"));
		}
		else
		{
			Talk(Locale.GetCharacter("eatMediocre"));
		}
	}

	public void TalkGlass()
	{
		Talk(Locale.GetCharacter("steponglass"));
	}

	public void PromptTalk()
	{
		timeSinceTalked = 999f;
	}

	public void EatBad()
	{
		Talk(Locale.GetCharacter("eatBad"));
	}

	public void Vomit()
	{
		Talk(Locale.GetCharacter("vomit"));
		PromptTraderResponse("vomit");
	}

	public void VomitBlood()
	{
		Talk(Locale.GetCharacter("vomitblood"));
		PromptTraderResponse("vomitblood");
	}

	public void RefuseEat()
	{
		Talk(Locale.GetCharacter("refuseEat"));
	}

	public void PromptTraderResponse(string textType)
	{
		if ((bool)body && !(body.totalHappiness < -75f) && !(body.traumaAmount > 80f) && !body.mindWipe && (bool)PlayerCamera.main.lastTrader)
		{
			PlayerCamera.main.lastTrader.PromptResponse(textType);
		}
	}

	private void Update()
	{
		if (WorldGeneration.world.generatingWorld || WorldGeneration.world.cutsceneActive)
		{
			return;
		}
		if ((bool)body)
		{
			float num = Mathf.Clamp01(1f + body.happiness * 0.009f);
			if (newTalkTime > 120f)
			{
				lastLineType = "";
			}
			Limb limb = null;
			Limb limb2 = null;
			Limb limb3 = null;
			Limb[] limbs = body.limbs;
			foreach (Limb limb4 in limbs)
			{
				if (!limb4.dismembered)
				{
					if (limb4.muscleHealth < 50f)
					{
						limb = limb4;
					}
					if (limb4.skinHealth < 50f)
					{
						limb2 = limb4;
					}
					if (limb4.infectionAmount > 5f)
					{
						limb3 = limb4;
					}
				}
			}
			if (body.conscious && !body.forceNoMovement)
			{
				newTalkTime += Time.deltaTime;
			}
			if (body.averagePain > 80f && newTalkTime > 5f && !Body.censorPain)
			{
				lastLineType = "";
				Talk(Locale.GetCharacter("bigpain"), null, force: false, resetTalkTimer: true);
			}
			else if (!body.breathing && newTalkTime > 8f)
			{
				lastLineType = "";
				Talk(Locale.GetCharacter("cantBreathe"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("dying");
			}
			else if (body.totalHappiness < -62f && newTalkTime > 30f && lastLineType != "miserable" && !Body.censorMood)
			{
				lastLineType = "miserable";
				Talk(Locale.GetCharacter("miserable"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("depressed");
			}
			else if (body.isCriticallyDying && newTalkTime > 12f && !Body.censorPain)
			{
				lastLineType = "lastmoments";
				Talk(Locale.GetCharacter("lastmoments"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("dying");
			}
			else if ((bool)body.GetComponent<Painkillers>() && body.GetComponent<Painkillers>().actualOpiateReception < -15f && newTalkTime > 20f && lastLineType != "opiatewithdrawal")
			{
				lastLineType = "opiatewithdrawal";
				Talk(Locale.GetCharacter("opiatewithdrawal"), null, force: false, resetTalkTimer: true);
			}
			else if (body.hunger < 30f * num && newTalkTime > 25f && lastLineType != "starving")
			{
				lastLineType = "starving";
				Talk(Locale.GetCharacter("starving"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("hungry");
			}
			else if (body.thirst < 30f && newTalkTime > 25f && lastLineType != "dehydrated")
			{
				lastLineType = "dehydrated";
				Talk(Locale.GetCharacter("dehydrated"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("thirsty");
			}
			else if (body.stamina < 25f && newTalkTime > 10f && lastLineType != "exhausted")
			{
				lastLineType = "exhausted";
				Talk(Locale.GetCharacter("exhausted"), null, force: false, resetTalkTimer: true);
			}
			else if (body.temperature < 31f && newTalkTime > 22f && lastLineType != "freezing")
			{
				lastLineType = "freezing";
				Talk(Locale.GetCharacter("freezing"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("cold");
			}
			else if (body.temperature > 40f && newTalkTime > 22f && lastLineType != "hot")
			{
				lastLineType = "hot";
				Talk(Locale.GetCharacter("hot"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("hot");
			}
			else if (body.brainHealth < 95f && newTalkTime > 16f && lastLineType != "braindamage")
			{
				lastLineType = "braindamage";
				Talk(Locale.GetCharacter("braindamaged"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("braindamaged");
			}
			else if (body.totalBleedSpeed > 0.03f && newTalkTime > 35f && lastLineType != "bleeding")
			{
				lastLineType = "bleeding";
				Talk(Locale.GetCharacter((body.totalBleedSpeed > 0.142f) ? "bleedingheavy" : "bleeding"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("bleeding");
			}
			else if (body.sicknessAmount > 70f && newTalkTime > 35f && lastLineType != "verysick")
			{
				lastLineType = "verysick";
				Talk(Locale.GetCharacter("verysick"), null, force: false, resetTalkTimer: true);
			}
			else if (body.totalHappiness < -46f && newTalkTime > 36f && lastLineType != "depressed" && !Body.censorMood)
			{
				lastLineType = "depressed";
				Talk(Locale.GetCharacter("depressed"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("depressed");
			}
			else if (body.stamina < 55f && newTalkTime > 15f && lastLineType != "exerted")
			{
				lastLineType = "exerted";
				Talk(Locale.GetCharacter("exerted"), null, force: false, resetTalkTimer: true);
			}
			else if (body.averagePain > 35f && newTalkTime > 11f && lastLineType != "pain")
			{
				limbs = body.limbs;
				foreach (Limb limb5 in limbs)
				{
					if (limb5.pain > 35f)
					{
						lastLineType = "pain";
						Talk(Locale.GetCharacter("pain"), limb5, force: false, resetTalkTimer: true);
						PromptTraderResponse("pain");
						break;
					}
				}
			}
			else if (body.energy < 15f && newTalkTime > 20f && lastLineType != "verytired")
			{
				lastLineType = "verytired";
				Talk(Locale.GetCharacter("verytired"), null, force: false, resetTalkTimer: true);
			}
			else if ((bool)limb3 && newTalkTime > 60f && lastLineType != "limbinfected")
			{
				lastLineType = "limbinfected";
				Talk(Locale.GetCharacter("limbinfected"), limb3, force: false, resetTalkTimer: true);
			}
			else if ((bool)limb2 && newTalkTime > 60f && lastLineType != "limbskin")
			{
				lastLineType = "limbskin";
				Talk(Locale.GetCharacter("limbskin"), limb2, force: false, resetTalkTimer: true);
			}
			else if ((bool)limb && newTalkTime > 70f && lastLineType != "limbmuscle")
			{
				lastLineType = "limbmuscle";
				Talk(Locale.GetCharacter("limbmuscle"), limb, force: false, resetTalkTimer: true);
			}
			else if (body.totalHappiness < -30f && newTalkTime > 45f && lastLineType != "gloomy")
			{
				lastLineType = "gloomy";
				Talk(Locale.GetCharacter("gloomy"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("sad");
			}
			else if (body.consciousness < 70f && newTalkTime > 30f && lastLineType != "confused")
			{
				lastLineType = "confused";
				Talk(Locale.GetCharacter("confused"), null, force: false, resetTalkTimer: true);
			}
			else if (body.energy < 35f && newTalkTime > 50f && lastLineType != "tired")
			{
				lastLineType = "tired";
				Talk(Locale.GetCharacter("tired"), null, force: false, resetTalkTimer: true);
			}
			else if (body.hunger < 70f * num && newTalkTime > 100f && lastLineType != "hungry")
			{
				lastLineType = "hungry";
				Talk(Locale.GetCharacter("hungry"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("hungry");
			}
			else if (body.thirst < 70f && newTalkTime > 100f && lastLineType != "thirsty")
			{
				lastLineType = "thirsty";
				Talk(Locale.GetCharacter("thirsty"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("thirsty");
			}
			else if (body.dirtyness > 70f && newTalkTime > 85f && lastLineType != "dirty")
			{
				lastLineType = "dirty";
				Talk(Locale.GetCharacter("dirty"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("dirty");
			}
			else if (body.overEncumberance > 0f && newTalkTime > 85f && lastLineType != "encumbered")
			{
				lastLineType = "encumbered";
				Talk(Locale.GetCharacter("encumbered"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("encumbered");
			}
			else if (body.wetness > 70f && newTalkTime > 60f && lastLineType != "wet" && body.charType != 2)
			{
				lastLineType = "wet";
				Talk(Locale.GetCharacter("wet"), null, force: false, resetTalkTimer: true);
			}
			else if (body.temperature < 35f && newTalkTime > 50f && lastLineType != "cold")
			{
				lastLineType = "cold";
				Talk(Locale.GetCharacter("cold"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("cold");
			}
			else if (body.temperature > 38f && newTalkTime > 50f && lastLineType != "warm")
			{
				lastLineType = "warm";
				Talk(Locale.GetCharacter("warm"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("hot");
			}
			else if (body.totalHappiness < -13f && newTalkTime > 55f && lastLineType != "sad")
			{
				lastLineType = "sad";
				Talk(Locale.GetCharacter("sad"), null, force: false, resetTalkTimer: true);
				PromptTraderResponse("sad");
			}
			else if (body.happiness > -30f && (bool)body.GetComponent<Painkillers>() && body.GetComponent<Painkillers>().actualOpiateReception > 20f && newTalkTime > 30f && lastLineType != "opiated")
			{
				lastLineType = "opiated";
				Talk(Locale.GetCharacter("opiated"), null, force: false, resetTalkTimer: true);
			}
			else if (body.happiness <= -30f && (bool)body.GetComponent<Painkillers>() && body.GetComponent<Painkillers>().actualOpiateReception > 20f && newTalkTime > 30f && lastLineType != "opiatedsad")
			{
				lastLineType = "opiatedsad";
				Talk(Locale.GetCharacter("opiatedsad"), null, force: false, resetTalkTimer: true);
			}
			else if (body.sicknessAmount > 20f && newTalkTime > 60f && lastLineType != "sick")
			{
				lastLineType = "sick";
				Talk(Locale.GetCharacter("sick"), null, force: false, resetTalkTimer: true);
			}
			timeSinceTalked += clampedUnscaledDeltaTime * body.consciousness * 0.01f * (1f + talkTimeMult);
			if (!body.conscious && text.text != "")
			{
				currentString = "";
				text.text = "";
			}
		}
		else
		{
			timeSinceTalked += clampedUnscaledDeltaTime * (1f + talkTimeMult);
		}
		if (currentString != "")
		{
			int num2 = Math.Clamp((int)(timeSinceTalked * 18f), 0, currentString.Length);
			text.text = currentString.Substring(0, num2);
			if (num2 != lastStringProgress && char.IsLetterOrDigit(currentString[Math.Clamp(num2, 0, currentString.Length - 1)]) && currentString[0] != '(' && currentString[0] != '*')
			{
				if ((bool)body)
				{
					switch (body.charType)
					{
					case 0:
						if (!impairedSpeech)
						{
							Sound.Play("speech", base.transform.position);
						}
						else
						{
							Sound.Play("speechbad", base.transform.position);
						}
						break;
					case 1:
						if (!impairedSpeech)
						{
							Sound.Play("milspeech", base.transform.position);
						}
						else
						{
							Sound.Play("milspeechbad", base.transform.position);
						}
						break;
					case 2:
						if (!impairedSpeech)
						{
							Sound.Play("speechdune", base.transform.position);
						}
						else
						{
							Sound.Play("speechdunebad", base.transform.position);
						}
						break;
					}
					body.eatTime = UnityEngine.Random.Range(0.05f, 0.1f) * (char.IsUpper(currentString[Math.Clamp(num2, 0, currentString.Length - 1)]) ? 2.5f : 1f);
				}
				else
				{
					if ((bool)trader)
					{
						trader.talkTime = UnityEngine.Random.Range(0.05f, 0.1f);
					}
					Sound.Play(talkSoundCustom, base.transform.position, isSoundGlobal, !isSoundGlobal);
				}
			}
			lastStringProgress = num2;
			if (timeSinceTalked > (float)currentString.Length / 18f + 3f)
			{
				currentString = "";
				text.text = "";
				lastStringProgress = -1;
			}
		}
		if ((bool)body)
		{
			if (body.conscious && !wasConscious && body.averagePain < 21f)
			{
				StartCoroutine(SayWakeUp());
			}
			wasConscious = body.conscious;
		}
	}

	public bool FinishedTalking()
	{
		return timeSinceTalked > (float)currentString.Length / 18f + 3f;
	}

	public IEnumerator SayWakeUp()
	{
		yield return new WaitForSeconds(1f);
		Talk(Locale.GetCharacter("wakeup"));
	}

	private void OnDestroy()
	{
		text.text = "";
	}

	private void LateUpdate()
	{
		text.rectTransform.position = (body ? (body.limbs[0].transform.position + Vector3.up * 4f + (Vector3)UnityEngine.Random.insideUnitCircle * body.averagePain * 0.0025f + Vector3.back * 2.6f) : (base.transform.position + Vector3.up * 3f + Vector3.back * 2.6f));
	}

	public void Talk(string text, Limb limb = null, bool force = false, bool resetTalkTimer = false)
	{
		Talk(new List<string> { text }, limb, force, resetTalkTimer);
	}

	private IEnumerator DoTalkDelayed(DelayTalkInfo info)
	{
		yield return new WaitForSeconds(info.delay);
		if (WorldGeneration.world.biomeDepth == info.layerRestrict || info.layerRestrict == -1)
		{
			Talk(info.text, info.limb, info.force, info.resetTalkTimer);
		}
	}

	public void TalkDelayed(float delay, string text, Limb limb = null, bool force = false, bool resetTalkTimer = false, int layerRestrict = -1)
	{
		StartCoroutine(DoTalkDelayed(new DelayTalkInfo
		{
			delay = delay,
			text = text,
			limb = limb,
			force = force,
			resetTalkTimer = resetTalkTimer,
			layerRestrict = layerRestrict
		}));
	}

	public void TalkDelayed(float delay, List<string> lines, Limb limb = null, bool force = false, bool resetTalkTimer = false, int layerRestrict = -1)
	{
		StartCoroutine(DoTalkDelayed(new DelayTalkInfo
		{
			delay = delay,
			text = lines.PickRandom(),
			limb = limb,
			force = force,
			resetTalkTimer = resetTalkTimer,
			layerRestrict = layerRestrict
		}));
	}

	public void Skip()
	{
		timeSinceTalked = 999f;
	}

	public void Talk(List<string> lines, Limb limb = null, bool force = false, bool resetTalkTimer = false)
	{
		if (!body)
		{
			currentString = lines[UnityEngine.Random.Range(0, lines.Count)];
			if ((bool)limb)
			{
				currentString = currentString.Replace("<limb>", limb.shortName.ToLower());
			}
			text.text = "";
			timeSinceTalked = 0f;
		}
		else
		{
			if (!body.conscious || !(body.brainHealth > 50f) || (bool)body.mindWipe || ((body.totalHappiness < -75f || body.traumaAmount > 80f) && !force))
			{
				return;
			}
			currentString = lines[UnityEngine.Random.Range(0, lines.Count)];
			if ((bool)EndLabScript.instance && EndLabScript.instance.doingEscape && body.hasCargo)
			{
				currentString = ((PlayerCamera.main.threatMusicTime > 0f) ? Locale.GetCharacter("labcommentcombat").PickRandom() : Locale.GetCharacter("labcommentcalm").PickRandom());
			}
			StringBuilder stringBuilder = new StringBuilder(currentString);
			if ((bool)limb)
			{
				currentString = currentString.Replace("<limb>", limb.shortName.ToLower());
				stringBuilder = new StringBuilder(currentString);
			}
			if (currentString.Length > 0 && currentString[0] != '(' && currentString[0] != '*')
			{
				if (impairedSpeech)
				{
					for (int i = 0; (float)i < (float)currentString.Length * 0.25f; i++)
					{
						int index = UnityEngine.Random.Range(0, stringBuilder.Length - 1);
						if (char.IsLetter(stringBuilder[index]))
						{
							if (char.IsLower(stringBuilder[index]))
							{
								stringBuilder[index] = fullMouthChars.PickRandom();
							}
							else
							{
								stringBuilder[index] = char.ToUpper(fullMouthChars.PickRandom());
							}
						}
					}
				}
				if (brainDamaged)
				{
					for (int j = 0; j < currentString.Length; j++)
					{
						if (char.IsLetter(stringBuilder[j]))
						{
							if (UnityEngine.Random.value < 0.12f)
							{
								char value = stringBuilder[j];
								for (int k = 0; k < UnityEngine.Random.Range(1, 4); k++)
								{
									stringBuilder.Insert(j, value);
								}
							}
							if (UnityEngine.Random.value < 0.12f)
							{
								stringBuilder.Insert(j, "-" + stringBuilder[j]);
							}
						}
						if (char.IsWhiteSpace(stringBuilder[j]) && UnityEngine.Random.value < 0.18f)
						{
							stringBuilder.Insert(j, "....");
						}
					}
				}
				if (body.inWater && !body.hasScubaGear)
				{
					for (int l = 0; l < currentString.Length; l++)
					{
						if (char.IsLetter(stringBuilder[l]))
						{
							stringBuilder[l] = underwaterChars.PickRandom();
						}
					}
				}
			}
			currentString = stringBuilder.ToString();
			text.text = "";
			timeSinceTalked = 0f;
			if (resetTalkTimer)
			{
				newTalkTime = 0f;
			}
		}
	}
}
