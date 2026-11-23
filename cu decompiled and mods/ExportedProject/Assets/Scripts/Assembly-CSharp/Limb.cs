using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Limb : MonoBehaviour
{
	[Serializable]
	public struct ProstheticInfo
	{
		public bool hasProsthetic;

		public string itemId;

		public float forceChange;

		public float armorChange;

		public string destroyItem;

		public string appearancePath;

		public int maxHoles;

		public float r;

		public float g;

		public float b;
	}

	public static float muscleDeathThreshold = 5f;

	public static float dislocationHealSpeed = 0.07f;

	public static float boneHealSpeed = 0.043f;

	public Body body;

	public float baseMass;

	public Rigidbody2D rb;

	public Transform animLimb;

	public string fullName;

	public string shortName;

	[JsonProperty]
	public float skinHealth = 100f;

	[JsonProperty]
	public float muscleHealth = 100f;

	[JsonProperty]
	public bool dislocated;

	[JsonProperty]
	public bool broken;

	[JsonProperty]
	public bool splinted;

	[JsonProperty]
	public bool infected;

	[JsonProperty]
	public float infectionAmount;

	[JsonProperty]
	public float pain;

	public float bonusRot;

	[JsonProperty]
	public float bleedAmount;

	public bool isVital;

	public bool isHead;

	public bool isAbdomen;

	public HingeJoint2D joint;

	private Material mat;

	public Limb[] connectedLimbs;

	public bool generateHeat;

	[JsonProperty]
	public float dislocationTimer;

	[JsonProperty]
	public float boneHealTimer;

	public bool isLegLimb;

	private GameObject brokenBoneObj;

	public float brokenPainMultiplier;

	public float infectionSpeedMult = 1f;

	private ParticleSystem bleedPart;

	private ParticleSystem waterBleedPart;

	private float infectionCheckTime;

	public SpriteRenderer bonusRendererToAffect;

	public float starvationHealthLossMult = 1f;

	public float horizontalTemporaryScale = 1f;

	[JsonProperty]
	public bool blockedBleeding;

	public int distanceToHeart;

	public bool isArm;

	[JsonProperty]
	public int shrapnel;

	[JsonProperty]
	public float furBloodAmount;

	private ParticleSystem.EmissionModule dripEmmis;

	[JsonProperty]
	public float disinfectionTime;

	private float randDripMult;

	public float bandageMinigameSpeedMult = 1f;

	public float weightVisualScaleMult;

	private LiquidAffect affect;

	[JsonProperty]
	public float bandageSlowAmount;

	[JsonProperty]
	public float skinHealAmount;

	[JsonProperty]
	public bool dismembered;

	public bool strokeAffected;

	[JsonProperty]
	public ProstheticInfo prostheticInfo;

	public float forceMult = 1f;

	public float totalBleedAmount => bleedAmount * (blockedBleeding ? 0f : 1f);

	public float bleedSpeedMult => body.bleedingSpeedMultiplier;

	public bool hasShrapnel => shrapnel > 0;

	public float MuscleHealRate => Time.deltaTime * 0.08f * (body.sleeping ? 1.4f : 1f) * ((muscleHealth > 10f || isHead) ? 1f : 0.25f) * ((!hasShrapnel) ? 1f : ((!isHead || muscleHealth > 14.28571f) ? 0f : 1f)) * body.hungerLimbHealCurrent * WorldGeneration.GetRunSettingFloat("healingrate") * (prostheticInfo.hasProsthetic ? 0f : 1f);

	public float SkinHealRate => Time.deltaTime * 0.055f * (body.sleeping ? 1.4f : 1f) * ((skinHealth > 10f) ? 1f : 0.25f) * (hasShrapnel ? 0f : 1f) * ((bleedAmount > 1f) ? 0.2f : 1f) * body.hungerLimbHealCurrent * WorldGeneration.GetRunSettingFloat("healingrate") * (prostheticInfo.hasProsthetic ? 0f : 1f);

	public float injuryHealTime
	{
		get
		{
			float runSettingFloat = WorldGeneration.GetRunSettingFloat("healingrate");
			if (broken)
			{
				if ((bool)GetComponent<SplintLimb>())
				{
					return boneHealTimer / (boneHealSpeed * 2.5f * runSettingFloat);
				}
				return boneHealTimer / (boneHealSpeed * runSettingFloat);
			}
			if (dislocated)
			{
				if ((bool)GetComponent<SplintLimb>())
				{
					return dislocationTimer / (dislocationHealSpeed * 2f * runSettingFloat);
				}
				return dislocationTimer / (dislocationHealSpeed * runSettingFloat);
			}
			return 0f;
		}
	}

	public float totalForce => muscleHealth * 0.01f * ((broken || dislocated || splinted) ? 0f : 1f) * (dismembered ? 0f : 1f) * Mathf.Clamp01(1f - (Mathf.Max(pain, body.averagePain * 0.9f) - body.curAdrenaline * 0.5f) * 0.007f) * body.bloodOxygen * 0.01f * (strokeAffected ? Mathf.Clamp01(1f - body.strokeAmount / 50f) : 1f) * (1f + prostheticInfo.forceChange) * forceMult;

	public Color GetBaseColor()
	{
		return new Color(prostheticInfo.r, prostheticInfo.g, prostheticInfo.b, prostheticInfo.hasProsthetic ? 0.5f : 1f);
	}

	public void AttachProsthetic(ProstheticInfo prosthetic, bool isLoad = false)
	{
		if (!WorldGeneration.GetRunSettingBool("amputee"))
		{
			SteamManager.TryUnlockAchievement("TRANSHUMANIST");
		}
		if (!isLoad)
		{
			Reattach();
			Sound.Play("prostheticapply", Vector2.zero, twoDimensional: true);
		}
		foreach (Limb lowerLimb in TourniquetScript.GetLowerLimbs(this))
		{
			lowerLimb.ApplyProstheticValues(prosthetic);
		}
		if (body.limbs[3].prostheticInfo.hasProsthetic && body.limbs[3].prostheticInfo.itemId == "peo" && body.limbs[6].prostheticInfo.hasProsthetic && body.limbs[6].prostheticInfo.itemId == "peo" && body.limbs[9].prostheticInfo.hasProsthetic && body.limbs[9].prostheticInfo.itemId == "peo" && body.limbs[12].prostheticInfo.hasProsthetic && body.limbs[12].prostheticInfo.itemId == "peo")
		{
			SteamManager.TryUnlockAchievement("CERTAINTYOFSTEEL");
		}
	}

	private void ApplyProstheticValues(ProstheticInfo prosthetic)
	{
		prostheticInfo = prosthetic;
		mat.SetTexture("_OverlayTex", Resources.Load<Sprite>(prostheticInfo.appearancePath).texture);
		mat.SetFloat("_OverlayVis", 1f);
	}

	public void Dismember()
	{
		if (dismembered)
		{
			return;
		}
		if (isAbdomen || isHead || isVital)
		{
			ConsoleScript.instance.Alert("Can only amputate extremities!");
			return;
		}
		WoundView.view.limbLookingAt = 1;
		Sound.Play("gore", base.transform.position);
		if (!prostheticInfo.hasProsthetic)
		{
			body.DoGoreSound();
			body.traumaAmount += 30f;
		}
		else
		{
			Sound.Play("prostheticBreak", Vector2.zero, twoDimensional: true);
		}
		foreach (Limb lowerLimb in TourniquetScript.GetLowerLimbs(this))
		{
			lowerLimb.gameObject.SendMessage("OnDismember", null, SendMessageOptions.DontRequireReceiver);
			foreach (Item limbWearable in lowerLimb.GetLimbWearables())
			{
				body.DropWearable(limbWearable);
			}
			if (lowerLimb.prostheticInfo.hasProsthetic)
			{
				Utils.Create(lowerLimb.prostheticInfo.destroyItem, lowerLimb.transform.position, UnityEngine.Random.value * 360f);
			}
			InventorySlot componentInChildren = lowerLimb.GetComponentInChildren<InventorySlot>();
			if ((bool)componentInChildren)
			{
				body.DropItem(componentInChildren.slot);
			}
			if (lowerLimb != this)
			{
				lowerLimb.dismembered = true;
				lowerLimb.gameObject.SetActive(value: false);
				lowerLimb.infected = false;
				lowerLimb.infectionAmount = 0f;
				lowerLimb.muscleHealth = 0f;
				lowerLimb.skinHealth = 0f;
				lowerLimb.bleedAmount = 0f;
				lowerLimb.mat.SetFloat("_OverlayVis", 0f);
				lowerLimb.prostheticInfo = default;
			}
		}
		Limb[] array = connectedLimbs;
		foreach (Limb limb in array)
		{
			if (!limb.dismembered)
			{
				limb.skinHealth -= 100f * (prostheticInfo.hasProsthetic ? 0.2f : 1f);
				limb.muscleHealth -= 50f * (prostheticInfo.hasProsthetic ? 0.15f : 1f);
				limb.bleedAmount = 100f * (prostheticInfo.hasProsthetic ? 0.1f : 1f);
				limb.pain = 100f * (prostheticInfo.hasProsthetic ? 0.25f : 1f);
			}
		}
		dismembered = true;
		foreach (Item limbWearable2 in GetLimbWearables())
		{
			UnityEngine.Object.Destroy(limbWearable2.gameObject);
		}
		prostheticInfo = default;
		mat.SetFloat("_OverlayVis", 0f);
		base.gameObject.SetActive(value: false);
		infected = false;
		infectionAmount = 0f;
		muscleHealth = 0f;
		skinHealth = 0f;
		bleedAmount = 0f;
	}

	public void Reattach()
	{
		if (!dismembered)
		{
			return;
		}
		List<Limb> lowerLimbs = TourniquetScript.GetLowerLimbs(body.GetTopOfExtremity(this));
		foreach (Limb item in lowerLimbs)
		{
			if (item.dismembered)
			{
				item.dismembered = false;
				item.transform.localPosition = Vector3.zero;
				item.gameObject.SetActive(value: true);
				item.Heal();
			}
		}
		foreach (Limb item2 in lowerLimbs)
		{
			Limb[] limbs = body.limbs;
			for (int i = 0; i < limbs.Length; i++)
			{
				Physics2D.IgnoreCollision(limbs[i].GetComponent<Collider2D>(), item2.GetComponent<Collider2D>(), ignore: true);
			}
		}
	}

	public List<Item> GetLimbWearables()
	{
		List<Item> list = new List<Item>();
		foreach (Transform item in base.transform)
		{
			if (item.TryGetComponent<Item>(out var component) && component.Stats.wearable)
			{
				list.Add(component);
			}
			if (item.TryGetComponent<WearableExtension>(out var component2))
			{
				list.Add(component2.reference);
			}
		}
		return list;
	}

	public void Start()
	{
		if (broken && !brokenBoneObj)
		{
			MakeBoneSprite();
		}
		if (dismembered)
		{
			base.gameObject.SetActive(value: false);
		}
		if (prostheticInfo.hasProsthetic)
		{
			AttachProsthetic(prostheticInfo, isLoad: true);
		}
		brokenPainMultiplier *= WorldGeneration.GetRunSettingFloat("fracturepain");
	}

	public void Dislocate()
	{
		if (!dislocated)
		{
			dislocated = true;
			dislocationTimer = UnityEngine.Random.Range(80f, 100f);
			pain += UnityEngine.Random.Range(40f, 80f);
			body.adrenaline += 75f;
			Sound.Play("dislocation", base.transform.position);
			body.PromptTalk();
			body.DoGoreSound();
			PlayerCamera.main.shaker.Shake(50f);
		}
		else
		{
			dislocationTimer = 100f;
		}
	}

	public void UnDislocate()
	{
		if (dislocated)
		{
			dislocated = false;
			body.happiness += 3f;
		}
	}

	public void BreakBone()
	{
		if (!broken)
		{
			broken = true;
			if (!prostheticInfo.hasProsthetic)
			{
				skinHealth -= 33f;
			}
			bleedAmount += UnityEngine.Random.Range(0f, 20f);
			pain += UnityEngine.Random.Range(60f, 100f);
			body.adrenaline += 80f;
			if (!prostheticInfo.hasProsthetic)
			{
				MakeBoneSprite();
			}
			Sound.Play("BoneBreak" + UnityEngine.Random.Range(1, 4), base.transform.position);
			boneHealTimer = 100f;
			body.PromptTalk();
			body.DoGoreSound();
			body.internalBleeding += ((isVital || isHead) ? 34f : 10f);
			PlayerCamera.main.shaker.Shake(80f);
			if (isHead && UnityEngine.Random.Range(0f, 1f) < 0.2f)
			{
				body.Disfigure();
			}
		}
		else
		{
			boneHealTimer = 100f;
		}
	}

	private void MakeBoneSprite()
	{
		brokenBoneObj = new GameObject("BrokenBone", typeof(SpriteRenderer));
		brokenBoneObj.transform.SetParent(base.transform);
		brokenBoneObj.transform.localPosition = Vector3.zero;
		brokenBoneObj.transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		brokenBoneObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("brokenBone");
		brokenBoneObj.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
		brokenBoneObj.GetComponent<SpriteRenderer>().material = WorldGeneration.world.defaultMat;
	}

	public void MendBone()
	{
		if (broken)
		{
			if ((bool)brokenBoneObj)
			{
				UnityEngine.Object.Destroy(brokenBoneObj);
			}
			broken = false;
			body.happiness += 5f;
			SteamManager.TryUnlockAchievement("GOODASNEW");
		}
	}

	public void CreateTemporarySprite(Sprite spr, float angle = 0f, Color? clr = null, bool scaleLimb = false, float time = 1000f, WoundView.LimbCondition removeCondition = null)
	{
		GameObject gameObject = new GameObject("TempSprite", typeof(SpriteRenderer));
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localEulerAngles = new Vector3(0f, 0f, angle);
		gameObject.transform.localScale = (scaleLimb ? new Vector3(horizontalTemporaryScale, 1f, 1f) : Vector3.one);
		gameObject.GetComponent<SpriteRenderer>().sprite = spr;
		gameObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 2;
		gameObject.GetComponent<SpriteRenderer>().material = WorldGeneration.world.defaultMat;
		gameObject.GetComponent<SpriteRenderer>().color = clr ?? Color.white;
		if (removeCondition != null)
		{
			OnLimbWound onLimbWound = gameObject.AddComponent<OnLimbWound>();
			onLimbWound.cond = removeCondition;
			onLimbWound.limb = this;
		}
		else
		{
			UnityEngine.Object.Destroy(gameObject, time);
		}
	}

	public float GetArmorReduction()
	{
		float num = 1f + WorldGeneration.GetRunSettingFloat("bonuslimbarmor") + body.bonusLimbProtection + prostheticInfo.armorChange;
		foreach (Item limbWearable in GetLimbWearables())
		{
			num += limbWearable.Stats.wearableArmor;
		}
		if (num < 0.1f)
		{
			num = 0.1f;
		}
		return num;
	}

	public void DamageWearables(float conditionLoss)
	{
		foreach (Item limbWearable in GetLimbWearables())
		{
			if (limbWearable.Stats.wearableArmor > 0f)
			{
				limbWearable.condition -= conditionLoss * limbWearable.Stats.wearableHitDurabilityLossMultiplier;
			}
		}
	}

	public void ImpactDamage(float force, Vector2 dir)
	{
		if (!(force > body.jumpSpeed))
		{
			return;
		}
		float armorReduction = GetArmorReduction();
		DamageWearables(force * 0.0015f);
		if (body.fallShakeCooldown < 0f)
		{
			body.fallShakeCooldown = 0.2f;
			PlayerCamera.main.shaker.Shake(force);
		}
		if (force > body.jumpSpeed * 1.66f)
		{
			body.DoFurTuft();
		}
		body.dirtyness += 1.35f;
		muscleHealth -= force * 0.33f / armorReduction;
		if (muscleHealth < 0f)
		{
			muscleHealth = 0f;
		}
		if (force > UnityEngine.Random.Range(body.jumpSpeed * 1.55f, body.jumpSpeed * 4.4f) * ((muscleHealth < 50f) ? 0.632f : 1f))
		{
			skinHealth -= force * 0.33f / armorReduction;
			if (force > body.jumpSpeed * 2.6f)
			{
				bleedAmount = Mathf.Max(bleedAmount, UnityEngine.Random.Range(bleedAmount, (100f - skinHealth) * 0.4f));
				body.DoGoreSound();
			}
		}
		if (force > UnityEngine.Random.Range(body.jumpSpeed * 3.5f, body.jumpSpeed * 5f) * Mathf.Clamp01(muscleHealth * 0.01f + 0.5f))
		{
			Dislocate();
		}
		if (force > UnityEngine.Random.Range(body.jumpSpeed * 4f, body.jumpSpeed * 7f) * Mathf.Clamp01(muscleHealth * 0.01f + 0.5f))
		{
			BreakBone();
		}
		if ((isVital || isHead) && force > body.jumpSpeed * 2f && UnityEngine.Random.value < 0.66f)
		{
			body.internalBleeding += force * 0.22f / armorReduction;
		}
		if (isHead)
		{
			body.consciousness -= force * 1.6f / armorReduction;
			if (force > body.jumpSpeed * 1.8f)
			{
				body.brainHealth -= force * 0.04f / armorReduction;
				Sound.Play("headhit", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
			}
			if (force > body.jumpSpeed * 3.2f)
			{
				body.brainHealth -= force * 0.12f / armorReduction;
			}
			if (force > body.jumpSpeed * 0.5f && UnityEngine.Random.value < 0.25f)
			{
				body.brainHealth -= force * 0.025f / armorReduction;
			}
		}
		pain += force * 0.4f / armorReduction;
		body.adrenaline += force * 0.075f / armorReduction;
		body.shock += force * 0.05f / armorReduction;
		Vector2 pos = (Vector2)base.transform.position + dir;
		WorldGeneration.world.DamageBlock(pos, force, force > 7f && body.soundCooldown <= 0f);
		ushort block = WorldGeneration.world.GetBlock(pos);
		if ((float)(int)block > 0f)
		{
			Sound.Play(WorldGeneration.world.RandomStepSound(WorldGeneration.world.GetBlockInfo(block).stepsound), pos);
		}
		if (force > 3f && body.soundCooldown <= 0f)
		{
			body.soundCooldown = 0.25f;
			Sound.Play($"bodyFall{UnityEngine.Random.Range(1, 6)}", base.transform.position);
		}
	}

	private void Awake()
	{
		joint = GetComponent<HingeJoint2D>();
		rb = GetComponent<Rigidbody2D>();
		rb.mass = baseMass;
		animLimb.GetComponent<SpriteRenderer>().enabled = false;
		fullName = Locale.GetOther(base.gameObject.name);
		shortName = Locale.GetOther(base.gameObject.name + "Short");
		mat = new Material(GetComponent<SpriteRenderer>().material);
		GetComponent<SpriteRenderer>().sharedMaterial = mat;
		if ((bool)bonusRendererToAffect)
		{
			bonusRendererToAffect.sharedMaterial = mat;
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("BleedParticle"), base.transform);
		bleedPart = gameObject.GetComponent<ParticleSystem>();
		gameObject.GetComponent<ParticleSystemRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
		gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Special/UnderwaterBleedParticle"), base.transform);
		waterBleedPart = gameObject.GetComponent<ParticleSystem>();
		gameObject.GetComponent<ParticleSystemRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
		affect = base.gameObject.AddComponent<LiquidAffect>();
		gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Special/WaterDrip"), base.transform);
		dripEmmis = gameObject.GetComponent<ParticleSystem>().emission;
		randDripMult = UnityEngine.Random.Range(0.75f, 1.25f);
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		float force = Mathf.Abs(col.relativeVelocity.magnitude * Mathf.Abs(Vector3.Dot(col.relativeVelocity.normalized, col.GetContact(0).normal)));
		ImpactDamage(force, body.lastTimeStepVelocity.normalized);
	}

	public void FurBloodUpdate()
	{
		ParticleSystem.EmissionModule emission = bleedPart.emission;
		ParticleSystem.EmissionModule emission2 = waterBleedPart.emission;
		if (totalBleedAmount > 0f)
		{
			furBloodAmount += 0.2f * totalBleedAmount * 0.007f;
		}
		else
		{
			furBloodAmount -= 0.00066666666f;
		}
		if (furBloodAmount > 0.75f)
		{
			float num = (furBloodAmount - 0.75f) * 0.5f;
			bool flag = false;
			List<Limb> list = new List<Limb>();
			Limb[] array = connectedLimbs;
			foreach (Limb limb in array)
			{
				if (!limb.dismembered && limb.transform.position.y < base.transform.position.y && limb.furBloodAmount < 0.99f)
				{
					list.Add(limb);
					flag = true;
				}
			}
			foreach (Limb item in list)
			{
				item.furBloodAmount += num / (float)list.Count;
				furBloodAmount -= num / (float)list.Count;
			}
			if (furBloodAmount > 0.95f && !flag && !affect.wasWater)
			{
				emission.rateOverTime = 5f;
				furBloodAmount -= 0.015f;
			}
			else
			{
				emission.rateOverTime = 0f;
			}
		}
		else
		{
			emission.rateOverTime = 0f;
		}
		if (affect.wasWater && furBloodAmount > 0.05f)
		{
			furBloodAmount -= 0.05f;
			emission2.rateOverTime = 4f;
		}
		else
		{
			emission2.rateOverTime = 0f;
		}
		furBloodAmount = Mathf.Clamp(furBloodAmount, 0f, 10f);
		mat.SetFloat("_BloodOverlay", furBloodAmount);
		mat.SetFloat("_Wetness", (body.hasFur ? (body.wetness * 0.01f) : 0f) * (prostheticInfo.hasProsthetic ? 0f : 1f));
	}

	public void SetDisinfect(float num)
	{
		disinfectionTime = Mathf.Max(disinfectionTime, num);
	}

	private void Update()
	{
		if (prostheticInfo.hasProsthetic)
		{
			bleedAmount = 0f;
			pain = 0f;
			infected = false;
			infectionAmount = 0f;
			if ((shrapnel > 0 || broken) && body.moveDir.x != 0f)
			{
				muscleHealth -= Time.deltaTime * 0.66f;
			}
			if (skinHealth < 100f)
			{
				float num = 100f - skinHealth;
				skinHealth = 100f;
				muscleHealth -= num * 0.05f;
			}
			if (muscleHealth <= 0f)
			{
				Dismember();
			}
		}
		if (WorldGeneration.world.generatingWorld || WorldGeneration.world.cutsceneActive)
		{
			return;
		}
		body.bloodVolume -= totalBleedAmount * bleedSpeedMult * Time.deltaTime;
		mat.SetFloat("_SkinDamage", 100f - skinHealth);
		mat.SetFloat("_MuscleDamage", (100f - muscleHealth) * (prostheticInfo.hasProsthetic ? 10f : 1f));
		mat.SetFloat("_InfectionPercent", infectionAmount * 0.01f);
		mat.SetFloat("_SnowAmount", Mathf.Max(body.snowAmount, body.dirtyness * 0.01f));
		mat.SetFloat("_Dirtyness", Mathf.Clamp01(body.dirtyness * 0.02f));
		mat.SetFloat("_Pain", Mathf.Clamp01(pain * 0.01f - body.adrenaline * 0.005f));
		float num2 = 15f - skinHealth * 0.15f + infectionAmount * 0.1f;
		pain = Mathf.Clamp(Mathf.MoveTowards(pain, num2, (pain > num2) ? Time.deltaTime : (Time.deltaTime * 0.6f)), 0f, 100f);
		if (body.temperature < 32f)
		{
			pain -= Time.deltaTime * 5f;
		}
		dripEmmis.rateOverTime = body.wetness * 0.01f * (body.wetness * 0.01f) * randDripMult;
		dislocationTimer -= Time.deltaTime * dislocationHealSpeed * WorldGeneration.GetRunSettingFloat("healingrate") * (prostheticInfo.hasProsthetic ? 0f : 1f);
		boneHealTimer -= Time.deltaTime * boneHealSpeed * WorldGeneration.GetRunSettingFloat("healingrate") * (prostheticInfo.hasProsthetic ? 0f : 1f);
		if (dislocated && dislocationTimer <= 0f)
		{
			UnDislocate();
		}
		if (broken && boneHealTimer <= 0f)
		{
			MendBone();
		}
		bleedAmount = Mathf.Clamp(bleedAmount - Time.deltaTime * body.bleedClottingSpeed * (hasShrapnel ? 0f : 1f) * WorldGeneration.GetRunSettingFloat("healingrate") - ((bandageSlowAmount > 0f) ? (Time.deltaTime * 1.25f) : 0f), 0f, 100f - skinHealth);
		bandageSlowAmount = Mathf.Max(bandageSlowAmount - Time.deltaTime * 1.25f, 0f);
		skinHealAmount = Mathf.Max(skinHealAmount - Time.deltaTime * 0.5f, 0f);
		disinfectionTime = Mathf.Max(disinfectionTime - Time.deltaTime * WorldGeneration.GetRunSettingFloat("infectionspeed"), 0f);
		if (!infected && skinHealth < 80f)
		{
			infectionCheckTime -= Time.deltaTime;
			if (infectionCheckTime < 0f)
			{
				infectionCheckTime = 40f;
				float num3 = WorldGeneration.GetRunSettingFloat("infectionchance") * body.infectionChanceMultiplier;
				float num4 = Mathf.Lerp(skinHealth * 0.01f / 0.8f, 1f, 0.77f) - bleedAmount * 0.007f;
				float num5 = 1f - num4;
				num5 = ((!(num3 <= 1f)) ? (1f - (1f - num5) * Mathf.Pow(0.5f, num3 - 1f)) : (num5 * num3));
				infected = UnityEngine.Random.Range(0f, 1f) < num5;
			}
		}
		if (infected)
		{
			float infectionSpeed = GetInfectionSpeed();
			infectionAmount += infectionSpeed * Time.deltaTime / 60f * WorldGeneration.GetRunSettingFloat("infectionspeed");
			if (body.temperature < 40.5f)
			{
				body.temperature += Time.deltaTime * 0.02f;
			}
			if (infectionAmount > 100f)
			{
				infectionAmount = 100f;
			}
			if (infectionAmount < 0f)
			{
				infected = false;
				infectionAmount = 0f;
			}
		}
		else
		{
			infectionAmount = Mathf.Clamp(infectionAmount - Time.deltaTime * 0.5f, 0f, 100f);
		}
		if (infected && infectionAmount > 90f)
		{
			Limb[] array = connectedLimbs;
			foreach (Limb limb in array)
			{
				if (!limb.infected)
				{
					limb.infected = true;
				}
			}
		}
		if (infectionAmount > 75f)
		{
			muscleHealth = Mathf.Clamp(muscleHealth - Time.deltaTime * 0.2f, 0f, 100f);
		}
		else if ((!infected || isHead) && body.hunger > 0f)
		{
			muscleHealth = Mathf.Clamp(muscleHealth + MuscleHealRate, 0f, 100f);
			if (!infected)
			{
				skinHealth = Mathf.Clamp(skinHealth + SkinHealRate, 0f, 100f);
			}
		}
		if (muscleHealth <= muscleDeathThreshold)
		{
			if (isVital)
			{
				body.respiratoryRate = 0f;
			}
			if (isAbdomen)
			{
				body.internalBleeding += Time.deltaTime * 0.2f;
			}
		}
		if ((dislocated || broken) && muscleHealth > 50f)
		{
			muscleHealth = 50f;
		}
		muscleHealth = Mathf.Clamp(muscleHealth, 0f, 100f);
		skinHealth = Mathf.Clamp(skinHealth + ((skinHealAmount > 0f) ? (Time.deltaTime * 0.5f) : 0f), 0f, 100f);
		if ((body.bloodOxygen <= 5f || body.bloodPressure < 20f) && !prostheticInfo.hasProsthetic)
		{
			muscleHealth = Mathf.Clamp(muscleHealth - Time.deltaTime * 0.6f, 0f, 100f);
		}
	}

	public float GetInfectionSpeed()
	{
		float num = body.curImmunityMult;
		if (disinfectionTime > 0f)
		{
			num -= 1.7f;
		}
		return 7.2f * num * infectionSpeedMult;
	}

	public static float InCubic(float t)
	{
		return t * t * t;
	}

	public static float OutCubic(float t)
	{
		return 1f - InCubic(1f - t);
	}

	public void Heal()
	{
		muscleHealth = 100f;
		skinHealth = 100f;
		boneHealTimer = 0f;
		dislocationTimer = 0f;
		infectionAmount = 0f;
		bleedAmount = 0f;
		pain = 0f;
		shrapnel = 0;
		infected = false;
	}
}
