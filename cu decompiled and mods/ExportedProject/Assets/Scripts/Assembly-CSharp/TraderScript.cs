using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TraderScript : MonoBehaviour
{
	public enum TraderItemPreference
	{
		WantsTrade = 0,
		Indifferent = 1,
		WantsKeep = 2
	}

	private BuildingEntity build;

	public Talker talker;

	public bool startedConvo;

	public int character;

	public int valueGiven;

	public int totalValueGiven;

	public static int MAX_VALUE_GIVEN = 60;

	public RangeF MoveRange;

	public float reputation;

	public List<TraderItem> items;

	public Transform head;

	public Transform torso;

	public Transform lArm;

	public Transform rArm;

	public Transform lThigh;

	public Transform rThigh;

	public Transform lFoot;

	public Transform rFoot;

	public SpriteRenderer eyes;

	public SpriteRenderer headRender;

	public Sprite[] eyeSprites;

	public Sprite[] headSprites;

	private float standAmount;

	private float standVelocity;

	public float standTarget;

	public float talkTime;

	private float sitTime;

	public float heightMult = 1f;

	public Transform[] lerptrans;

	public bool shiftface;

	private float lastAttackTime;

	public GameObject attackAnimation;

	public AudioClip[] painSounds;

	private Material mat;

	private float[] bonusArmRot;

	private bool didDeathMoodDebuff;

	public float hurtPitch = 1f;

	public float bonusRot;

	public Transform rotatePoint;

	public AnimationCurve valueMultiplierCurve;

	public const float WantsTradePriceMult = 0.7f;

	public const float WantsKeepPriceMult = 1.5f;

	public const float HostilityRepThreshold = 30f;

	public float haggleAmount;

	private bool freeDressing;

	private int freeAmount;

	public float minHugReputation = 100f;

	public float hostility;

	private bool didHug;

	private int hostileLineTime;

	private bool lastSlept;

	public List<TraderItemPreference> collapsedCategories;

	private Vector2 desiredPos;

	public bool farEnoughToMove;

	public bool didMove;

	private Body body => PlayerCamera.main.body;

	public float actualReputation => reputation - (100f - body.consciousness) * 0.5f;

	public bool hostile => hostility >= 100f;

	public static Color PrefToColor(TraderItemPreference pref)
	{
		return pref switch
		{
			TraderItemPreference.WantsTrade => Color.green, 
			TraderItemPreference.Indifferent => Color.yellow, 
			TraderItemPreference.WantsKeep => new Color32(byte.MaxValue, 132, 0, byte.MaxValue), 
			_ => Color.clear, 
		};
	}

	public float ValueMultiplier()
	{
		return valueMultiplierCurve.Evaluate(reputation * 0.01f);
	}

	public void OnUse()
	{
		PlayerCamera.main.ToggleTradeMenu();
	}

	public void PromptResponse(string type)
	{
		if (!(build.health <= 200f) && startedConvo && !hostile && !(reputation <= 30f) && !(Vector2.Distance(base.transform.position, body.transform.position) > 15f))
		{
			StartCoroutine(DelayedTalk(Locale.GetCharacter(type + "response", character).PickRandom()));
		}
	}

	public IEnumerator DelayedTalk(string text)
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(0.9f, 1.5f));
		talker.Talk(text);
	}

	public bool AskToMove()
	{
		if (reputation < 70f)
		{
			talker.Talk(Locale.GetCharacter("tradermovefail", character));
			reputation -= 3f;
			return false;
		}
		reputation--;
		talker.Talk(Locale.GetCharacter("tradermove", character));
		Vector3 position = base.transform.position;
		position.x = (MoveRange.min + MoveRange.max) * 0.5f;
		didMove = true;
		desiredPos = position;
		return true;
	}

	public void MeetPlayer()
	{
		eyes.sprite = eyeSprites[1];
		sitTime = Time.time;
		startedConvo = true;
		reputation = UnityEngine.Random.Range(75f, 135f);
		reputation += body.skills.INTFrom10 * 4f;
		reputation += WorldGeneration.GetRunSettingFloat("traderrepoffset");
		if (body.talker.impairedSpeech)
		{
			reputation -= 20f;
		}
		if (body.disfigured)
		{
			reputation -= 5f;
		}
		if (body.dirtyness > 50f)
		{
			reputation -= body.dirtyness * 0.25f;
		}
		if (body.HoldingItem(body.handSlot) && body.GetItem(body.handSlot).Stats.HasTag("gun"))
		{
			hostility += 50f;
			reputation -= 20f;
		}
		if ((bool)body.mindWipe)
		{
			reputation *= 0.7f;
		}
		reputation -= (100f - body.brainHealth) * 0.5f;
		reputation += body.happiness * 0.5f;
		if (body.hearingLoss > 50f)
		{
			reputation -= 20f;
		}
		if (body.totalBleedSpeed > 0.001f)
		{
			freeDressing = true;
			items.Add(new TraderItem
			{
				id = "bandage",
				preference = TraderItemPreference.Indifferent,
				value = Item.GetItem("bandage").value
			});
			items = items.OrderBy((TraderItem x) => (int)x.preference).ToList();
		}
		StartCoroutine(Convo());
	}

	private IEnumerator Convo()
	{
		body.talker.Talk(Locale.GetCharacter("tradeemeet"));
		yield return new WaitForSeconds(1.5f);
		if (body.HoldingItem(body.handSlot) && body.GetItem(body.handSlot).Stats.HasTag("gun"))
		{
			talker.Talk(Locale.GetCharacter("traderseegun", character));
			yield return new WaitUntil(() => talker.FinishedTalking());
		}
		if (!body.talker.impairedSpeech || (bool)body.mindWipe)
		{
			string text = ((reputation > 125f) ? "good" : ((!(reputation > 75f)) ? "bad" : "normal"));
			talker.Talk(Locale.GetCharacter("tradermeet" + text, character));
		}
		else
		{
			talker.Talk(Locale.GetCharacter("tradermeetimpairedspeech", character));
		}
		standTarget = UnityEngine.Random.Range(0.9f, 1f);
		yield return new WaitUntil(() => talker.FinishedTalking());
		if (freeDressing)
		{
			talker.Talk(Locale.GetCharacter("traderseehurt", character));
		}
	}

	public int ItemPrice(TraderItem item)
	{
		float num = (float)item.value * ValueMultiplier();
		if (item.preference == TraderItemPreference.WantsTrade)
		{
			num *= 0.7f;
		}
		if (item.preference == TraderItemPreference.WantsKeep)
		{
			num *= 1.5f;
		}
		if (freeDressing && item.info.HasTag("dressing"))
		{
			num = 0f;
		}
		if (freeAmount > 0)
		{
			num = 0f;
		}
		return Mathf.RoundToInt(num);
	}

	public void TryHaggle()
	{
		if (character != 2)
		{
			haggleAmount++;
			float num = UnityEngine.Random.Range(-25f, 25f) / haggleAmount;
			reputation += num;
			if (num > 0f && haggleAmount < 8f)
			{
				body.skills.AddExp(2, 2.5f);
			}
			talker.Talk(Locale.GetCharacter((!(haggleAmount < 8f)) ? "..." : ((num > 0f) ? "traderhagglesuccess" : "traderhagglefail"), character));
		}
		else if (totalValueGiven < MAX_VALUE_GIVEN)
		{
			talker.Talk(Locale.GetCharacter("traderbite", character));
			List<Limb> list = (from x in body.limbs.Skip(3)
				where !x.dismembered
				select x).ToList();
			if (list.Count == 0)
			{
				return;
			}
			Limb limb = list.PickRandom();
			limb.muscleHealth -= UnityEngine.Random.Range(25f, 40f);
			limb.skinHealth -= UnityEngine.Random.Range(12f, 20f);
			limb.bleedAmount += UnityEngine.Random.Range(6f, 12f);
			limb.pain += UnityEngine.Random.Range(37f, 55f);
			body.adrenaline += 40f;
			body.DoGoreSound();
			Sound.Play("eatFlesh", body.transform.position);
			float num2 = (limb.prostheticInfo.hasProsthetic ? 0.4f : 1f);
			float num3 = UnityEngine.Random.Range(20f, 28f) * num2;
			reputation += num3;
			int num4 = Mathf.RoundToInt((float)UnityEngine.Random.Range(6, 11) * num2);
			valueGiven += num4;
			totalValueGiven += num4;
			if (valueGiven > MAX_VALUE_GIVEN)
			{
				valueGiven = MAX_VALUE_GIVEN;
			}
			if (totalValueGiven > MAX_VALUE_GIVEN)
			{
				totalValueGiven = MAX_VALUE_GIVEN;
			}
		}
		else
		{
			talker.Talk("...");
		}
		PlayerCamera.main.UpdateTradeTexts();
		PlayerCamera.main.RefreshTraderInventories();
		PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.MiniClick);
	}

	private void OnWillRenderObject()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(base.transform.position + Vector3.up);
		FluidManager.main.fluid[vector2Int.x, vector2Int.y] = 0;
		if (Vector2.Distance(base.transform.position, body.transform.position) < 6f && build.health > 200f)
		{
			if (!startedConvo)
			{
				MeetPlayer();
			}
			if (!hostile && build.health > 200f)
			{
				PlayerCamera.main.currentTrader = this;
				PlayerCamera.main.lastTrader = this;
			}
		}
		head.localPosition = Vector2.LerpUnclamped(new Vector2(0f, 2.312f), new Vector2(0f, 4.9f), standAmount) + (shiftface ? (Vector2.right * 0.125f) : Vector2.zero);
		torso.localPosition = Vector2.LerpUnclamped(new Vector2(0f, 1.034f), new Vector2(0f, 3.44f), standAmount);
		lThigh.localPosition = Vector2.LerpUnclamped(new Vector2(-0.345f, 0.79f), new Vector2(-0.4f, 2.11f), standAmount);
		rThigh.localPosition = Vector2.LerpUnclamped(new Vector2(0.345f, 0.79f), new Vector2(0.4f, 2.11f), standAmount);
		lFoot.localPosition = Vector2.LerpUnclamped(new Vector2(-0.627f, 0.727f), new Vector2(-0.34f, 0.74f), standAmount);
		rFoot.localPosition = Vector2.LerpUnclamped(new Vector2(0.627f, 0.727f), new Vector2(0.34f, 0.74f), standAmount);
		lArm.localPosition = new Vector2(-0.3f, 0.422f);
		rArm.localPosition = new Vector2(0.3f, 0.422f);
		lThigh.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(-168f, 0f, standAmount));
		rThigh.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(168f, 0f, standAmount));
		lFoot.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(-2.64f, 0f, standAmount));
		rFoot.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(2.64f, 0f, standAmount));
		lArm.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(-13.97f, -10f, standAmount) + bonusArmRot[0]);
		rArm.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(41.995f, 10f, standAmount) + bonusArmRot[1]);
		head.rotation = Quaternion.identity;
		torso.rotation = Quaternion.identity;
		for (int i = 0; i < 2; i++)
		{
			bonusArmRot[i] = Mathf.Lerp(bonusArmRot[i], 0f, Time.deltaTime * 8f);
		}
		float b = 0f;
		if (Mathf.Abs(base.transform.position.x - body.transform.position.x) < 2f && build.health > 200f)
		{
			b = ((body.transform.position.x > base.transform.position.x) ? 8f : (-8f));
		}
		bonusRot = Mathf.Lerp(bonusRot, b, Time.deltaTime * 5f);
		Transform[] array = lerptrans;
		foreach (Transform transform in array)
		{
			transform.localPosition = new Vector2(transform.localPosition.x, Mathf.LerpUnclamped(transform.localPosition.y, 0f, 1f - heightMult));
		}
		head.RotateAround(rotatePoint.position, Vector3.forward, bonusRot);
		torso.RotateAround(rotatePoint.position, Vector3.forward, bonusRot);
		if (startedConvo && Time.time - sitTime > 60f && build.health > 200f)
		{
			standTarget = 0.1f;
		}
		if (character != 2)
		{
			eyes.flipX = body.transform.position.x < base.transform.position.x;
			if (shiftface)
			{
				eyes.transform.localPosition = (eyes.flipX ? new Vector2(-0.125f, 0f) : Vector2.zero);
			}
		}
		talkTime -= Time.unscaledDeltaTime;
		headRender.sprite = ((talkTime > 0f) ? headSprites[1] : headSprites[0]);
		if ((hostile || reputation < 30f) && build.health > 200f)
		{
			if (hostility > 30f && hostileLineTime < 1)
			{
				hostileLineTime = 1;
				sitTime = Time.time;
				talker.Talk(Locale.GetCharacter("traderwarning", character));
				standTarget = 1f;
			}
			if (hostility > 55f && hostileLineTime < 2)
			{
				hostileLineTime = 2;
				talker.Talk(Locale.GetCharacter("traderwarning", character));
			}
			if (hostility >= 100f && hostileLineTime < 3)
			{
				hostileLineTime = 3;
				talker.Talk(Locale.GetCharacter("tradercombat", character));
				head.GetComponent<LookTarget>().scaredFace = true;
				body.happiness -= 2f;
				eyes.sprite = eyeSprites[2];
			}
			if (Vector2.Distance(base.transform.position, body.transform.position) < 12f)
			{
				hostility = Mathf.MoveTowards(hostility, 100f, Time.deltaTime * 5f);
			}
			if (hostile)
			{
				build.cantHit = false;
				sitTime = Time.time;
				standTarget = Mathf.Clamp01(build.health / 400f);
				if (Time.time - lastAttackTime > 0.78f && Vector2.Distance(base.transform.position, body.transform.position) < 8f)
				{
					Swing();
				}
			}
		}
		mat.SetFloat("_SkinDamage", Mathf.Max(0f, (800f - build.health) * 0.125f));
		if (build.health < 200f)
		{
			sitTime = Time.time;
			standTarget = -0.1f;
			eyes.sprite = eyeSprites[0];
			if (!didDeathMoodDebuff)
			{
				DropInventory();
				didDeathMoodDebuff = true;
				body.skills.AddExp(1, 60f);
				if (body.charType != 2)
				{
					body.happiness -= 10f;
					body.traumaAmount += 30f;
					body.sicknessAmount += 30f * (body.mindWipe ? 0.5f : 1f);
					body.StartCoroutine(body.Cry());
				}
				body.talker.Talk(Locale.GetCharacter("murderregret"));
				build.description = Locale.GetBuilding("traderdscdead");
				build.fullName = Locale.GetBuilding("corpseseen");
				SteamManager.TryUnlockAchievement("MYSURVIVALOVERYOURS");
				UnityEngine.Object.Destroy(GetComponent<UsableObject>());
			}
		}
		if (lastSlept && !body.sleeping && body.energy > 45f)
		{
			DoWakeUpGreet();
			reputation += 8f / body.sleepCycleSpeed;
		}
		lastSlept = body.sleeping;
		if (reputation >= 199.5f)
		{
			SteamManager.TryUnlockAchievement("REALSUCKUP");
		}
		base.transform.position = Vector2.Lerp(base.transform.position, desiredPos, Time.deltaTime * 6f);
	}

	public void AnimalHit()
	{
		standAmount -= UnityEngine.Random.Range(0.1f, 0.2f);
		bonusRot += UnityEngine.Random.Range(-20f, 20f);
		if (UnityEngine.Random.Range(0f, 1f) < 0.75f && build.health > 200f)
		{
			Sound.Play(painSounds.PickRandom(), head.transform.position, twoDimensional: false, pitchShift: true, null, 1f, hurtPitch);
		}
		if (UnityEngine.Random.value < 0.2f && build.health > 200f)
		{
			body.talker.Talk(Locale.GetCharacter("swingresponse"));
		}
	}

	public void UpdateScreen()
	{
		PlayerCamera.main.UpdateTradeTexts();
		PlayerCamera.main.RefreshTraderInventories();
	}

	private void Update()
	{
		standVelocity += (standTarget + Mathf.Sin(Time.time * 1.7f) * 0.06f * ((build.health > 200f) ? 1f : 0f) - standAmount - 0.06f) * Time.deltaTime * 0.2f;
		standVelocity /= 1f + Time.deltaTime * 4f;
		if (Time.timeScale > 0f)
		{
			standAmount += standVelocity;
		}
	}

	public void LightBroken()
	{
		if (!(build.health < 200f))
		{
			reputation -= 40f;
			standTarget = 1f;
			talker.Talk(Locale.GetCharacter("traderlightbreak", character));
			UpdateScreen();
		}
	}

	public void TryHug()
	{
		if (body.charType == 1 && body.happiness > -50f)
		{
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Deny);
			return;
		}
		if (reputation < minHugReputation || body.dirtyness > 50f)
		{
			talker.Talk(Locale.GetCharacter("traderhugfail", character));
			if (!didHug)
			{
				reputation -= 8f;
				body.happiness -= 2.5f;
				body.SetVelocity((body.transform.position - torso.transform.position).normalized * 3f);
				body.Ragdoll();
				Sound.Play("BSSwing1", base.transform.position);
				UpdateScreen();
				didHug = true;
			}
		}
		else
		{
			talker.Talk(Locale.GetCharacter("traderhugsuccess", character));
			Sound.Play("combine", base.transform.position);
			if (!didHug && character != 2)
			{
				reputation += 5f;
				body.happiness += 2.5f;
				UpdateScreen();
				didHug = true;
			}
		}
		if (reputation < 30f)
		{
			hostility = 100f;
		}
		PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.MiniClick);
	}

	public void DoRest()
	{
		if (body.canTakeNap)
		{
			body.sleeping = true;
			body.consciousness = 30f;
			body.forcedSleepQuality = Body.SleepQuality.Good;
			PlayerCamera.main.ToggleTradeMenu();
		}
	}

	public void DoWakeUpGreet()
	{
		if (!(build.health < 200f) && startedConvo && reputation > 30f)
		{
			StartCoroutine(WakeUpGreet());
		}
	}

	private IEnumerator WakeUpGreet()
	{
		yield return new WaitForSecondsRealtime(1.8f);
		talker.Talk(Locale.GetCharacter("traderwakegreet", character));
	}

	public void Threaten()
	{
		reputation *= 0.3f;
		body.happiness -= 5f;
		body.traumaAmount += 5f;
		float num = UnityEngine.Random.value;
		if (body.HoldingItem(body.handSlot) && body.GetItem(body.handSlot).Stats.HasTag("gun"))
		{
			num = Mathf.Lerp(num, 1f, 0.25f);
		}
		if (character == 2)
		{
			num *= 0.5f;
		}
		if (body.charType == 2)
		{
			num *= 1.3f;
		}
		if (num > 0.6f && reputation > 30f)
		{
			talker.Talk(Locale.GetCharacter("traderthreatensuccess", character));
			freeAmount += UnityEngine.Random.Range(2, 4);
			UpdateScreen();
		}
		else if (num > 0.3f)
		{
			talker.Talk(Locale.GetCharacter("traderwarning", character));
			UpdateScreen();
		}
		else
		{
			hostility = 100f;
		}
	}

	private void Swing()
	{
		GameObject obj = UnityEngine.Object.Instantiate(attackAnimation);
		Vector2 to = (body.transform.position - torso.position).normalized;
		obj.transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector3.right, to));
		obj.transform.localScale = new Vector3(1f, (to.x > 0f) ? 1f : (-1f), 1f);
		obj.transform.position = torso.transform.position + torso.up * 0.8f;
		Sound.Play("BSSwing1", base.transform.position);
		UnityEngine.Object.Destroy(obj, 2f);
		lastAttackTime = Time.time + UnityEngine.Random.Range(-0.5f, 0.5f);
		bonusArmRot[(to.x > 0f) ? 1u : 0u] = ((to.x > 0f) ? 90f : (-90f));
		PlayerCamera.main.threatMusicTime = 10f;
		PlayerCamera.main.currentThreatTheme = 12;
		bonusRot += ((to.x > 0f) ? (-10f) : 10f);
		if (UnityEngine.Random.value < 0.1f)
		{
			talker.Talk(Locale.GetCharacter("tradercombat", character));
		}
		if (Vector2.Distance(torso.position, body.transform.position) < 6.5f)
		{
			Limb limb = body.limbs.PickRandom();
			float num = 1f;
			if (character == 1)
			{
				num = 0.9f;
			}
			if (character == 2)
			{
				num = 1.3f;
			}
			limb.muscleHealth -= 20f / limb.GetArmorReduction() * num;
			limb.skinHealth -= UnityEngine.Random.Range(24f, 40f) / limb.GetArmorReduction() * num;
			limb.bleedAmount += UnityEngine.Random.value * 12f / limb.GetArmorReduction() * num;
			limb.pain += 45f / limb.GetArmorReduction() * num;
			limb.DamageWearables(0.3f * num);
			limb.body.adrenaline = 100f;
			limb.body.DoGoreSound();
			if (character == 2 && UnityEngine.Random.value < 0.1f && Array.IndexOf(limb.body.limbs, limb) > 2)
			{
				limb.Dismember();
			}
		}
		Vector3 position = base.transform.position;
		position.x += to.x * UnityEngine.Random.Range(0.125f, 0.5f) * ((UnityEngine.Random.value < 0.25f) ? (-1.6f) : 1f) * ((UnityEngine.Random.value < 0.2f) ? 5f : 1f);
		position.x = Mathf.Clamp(position.x, MoveRange.min, MoveRange.max);
		desiredPos = position;
	}

	public void AnimalDeath()
	{
		Sound.Play("gore", torso.transform.position);
		UnityEngine.Object.Instantiate(Resources.Load("BloodExplosion"), torso.transform.position, Quaternion.identity);
	}

	public void GiveItem(Item item)
	{
		if (item.TryGetComponent<Container>(out var component) && component.GetHoldingWeight() > 0f)
		{
			PlayerCamera.main.DoAlert(Locale.GetOther("alertsellcontainer"));
		}
		else
		{
			if (item.Stats.GetValue(item) <= 0 || (bool)item.GetComponent<BoughtItem>())
			{
				return;
			}
			if (totalValueGiven == MAX_VALUE_GIVEN)
			{
				talker.Talk(Locale.GetCharacter("tradergiveitemrefuse", character));
				return;
			}
			talker.Talk(Locale.GetCharacter("tradergiveitem", character));
			int num = MAX_VALUE_GIVEN - totalValueGiven;
			int num2 = item.Stats.GetValue(item);
			if (num2 > num)
			{
				num2 = num;
			}
			valueGiven += num2;
			totalValueGiven += num2;
			if (valueGiven > MAX_VALUE_GIVEN)
			{
				valueGiven = MAX_VALUE_GIVEN;
			}
			if (totalValueGiven > MAX_VALUE_GIVEN)
			{
				totalValueGiven = MAX_VALUE_GIVEN;
			}
			PlayerCamera.main.UpdateTradeTexts();
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	public void GenerateInventory()
	{
		items = new List<TraderItem>();
		items.AddRange(GenerateSingleItemList(TraderItemPreference.WantsTrade));
		items.AddRange(GenerateSingleItemList(TraderItemPreference.Indifferent));
		items.AddRange(GenerateSingleItemList(TraderItemPreference.WantsKeep));
		if (character == 1)
		{
			string[] source = new string[3] { "smallmagazine", "riflemagazine", "boxof12gauge" };
			for (int i = 0; i < 2; i++)
			{
				string id = source.PickRandom();
				items.Add(new TraderItem
				{
					preference = (TraderItemPreference)UnityEngine.Random.Range(0, 3),
					id = id,
					value = Item.GetItem(id).value
				});
			}
			string id2 = new string[3] { "pistol", "rifle", "shotgun" }.PickRandom();
			items.Add(new TraderItem
			{
				preference = (TraderItemPreference)UnityEngine.Random.Range(0, 3),
				id = id2,
				value = Item.GetItem(id2).value
			});
			items = items.OrderBy((TraderItem x) => (int)x.preference).ToList();
		}
	}

	private List<TraderItem> GenerateSingleItemList(TraderItemPreference pref)
	{
		List<TraderItem> list = new List<TraderItem>();
		int num = Mathf.RoundToInt((float)UnityEngine.Random.Range(2, 9) * WorldGeneration.GetRunSettingFloat("traderitemamount"));
		if (character == 2)
		{
			num = Mathf.RoundToInt((float)num * 0.66f);
		}
		string[] source = new string[8] { "medical", "food", "water", "tool", "drug", "container", "utility", "custom" };
		for (int i = 0; i < num; i++)
		{
			(string, ItemInfo) tuple;
			do
			{
				tuple = ItemLootPool.RandomFromPool(source.PickRandom());
			}
			while (tuple.Item2.value <= 0 || tuple.Item2.untradeable);
			TraderItem item = new TraderItem
			{
				preference = pref,
				id = tuple.Item1,
				bought = false,
				value = tuple.Item2.DefaultValue()
			};
			list.Add(item);
		}
		return list;
	}

	private void Start()
	{
		standAmount = 0f;
		reputation = 100f;
		desiredPos = base.transform.position;
		GenerateInventory();
	}

	private void Awake()
	{
		build = GetComponent<BuildingEntity>();
		mat = new Material(head.GetComponent<SpriteRenderer>().sharedMaterial);
		head.GetComponent<SpriteRenderer>().sharedMaterial = mat;
		torso.GetComponent<SpriteRenderer>().sharedMaterial = mat;
		lArm.GetComponent<SpriteRenderer>().sharedMaterial = mat;
		lArm.GetComponent<SpriteRenderer>().sharedMaterial = mat;
		lThigh.GetComponent<SpriteRenderer>().sharedMaterial = mat;
		rThigh.GetComponent<SpriteRenderer>().sharedMaterial = mat;
		lFoot.GetComponent<SpriteRenderer>().sharedMaterial = mat;
		rFoot.GetComponent<SpriteRenderer>().sharedMaterial = mat;
		bonusArmRot = new float[2];
	}

	public void DropInventory()
	{
		foreach (TraderItem item in items)
		{
			if (UnityEngine.Random.value < 0.66f)
			{
				Item component = Utils.Create(item.id, base.transform.position, UnityEngine.Random.value * 360f).GetComponent<Item>();
				component.gameObject.AddComponent<FreshItemDrop>();
				if (component.TryGetComponent<AmmoScript>(out var component2) && component2.itemType == AmmoScript.AmmoItemType.Magazine)
				{
					component2.rounds = (int)Mathf.Lerp(0f, component2.maxRounds, UnityEngine.Random.value);
				}
			}
		}
		items.Clear();
	}

	public void TryPurchase(TraderItem item)
	{
		if (build.health < 200f)
		{
			return;
		}
		if (valueGiven >= ItemPrice(item))
		{
			valueGiven -= ItemPrice(item);
			if (ItemPrice(item) > 0)
			{
				if (item.preference == TraderItemPreference.WantsTrade)
				{
					reputation += 7f;
				}
				else if (item.preference == TraderItemPreference.Indifferent)
				{
					reputation += 4f;
				}
			}
			if (freeAmount > 0)
			{
				freeAmount--;
			}
			else
			{
				talker.Talk(Locale.GetCharacter("traderbuy", character));
				body.happiness += 0.75f;
				body.skills.AddExp(2, 1f);
			}
			freeDressing = false;
			items.Remove(item);
			GameObject gameObject = Utils.Create(item.id, base.transform.position, 0f);
			gameObject.AddComponent<BoughtItem>();
			if (gameObject.TryGetComponent<AmmoScript>(out var component) && component.itemType == AmmoScript.AmmoItemType.Magazine)
			{
				component.rounds = UnityEngine.Random.Range((int)((float)component.maxRounds * 0.5f), component.maxRounds);
			}
			PlayerCamera.main.body.AutoPickUpItem(gameObject.GetComponent<Item>());
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Click);
		}
		else
		{
			if (totalValueGiven != MAX_VALUE_GIVEN)
			{
				talker.Talk(Locale.GetCharacter("traderbuyfail", character));
			}
			else
			{
				talker.Talk(Locale.GetCharacter("traderbuyfailmaxvalue", character));
			}
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Deny);
			reputation -= 2f;
		}
		PlayerCamera.main.RefreshTraderInventories();
		PlayerCamera.main.UpdateTradeTexts();
	}
}
