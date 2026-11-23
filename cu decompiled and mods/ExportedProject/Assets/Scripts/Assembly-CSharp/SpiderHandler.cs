using System.Collections.Generic;
using UnityEngine;

public class SpiderHandler : MonoBehaviour
{
	public IKHandle[] legs;

	public Vector2[] targets;

	public float legSpeed = 8f;

	public float timeOffset;

	private Rigidbody2D rb;

	public float stunTime;

	public Vector2 target;

	public float moveForce;

	public float moveTime;

	public RangeF moveWaitTime;

	public float moveRange;

	protected float biteCooldown;

	public List<AudioClip> attackSounds;

	private float maxHealth;

	public float infectionChance;

	public float biteSkinDamage;

	public float biteMuscleDamage;

	public float bitePain;

	public float bleedDamage;

	public float venomAmount;

	public float stunMult = 0.05f;

	public float aggro = 0.15f;

	public float retreatMoveTime = 5f;

	public float biteCoolToSet = 5f;

	public float seeDistance = 8f;

	public bool clawAnim;

	public bool doDeathExplode = true;

	public bool doFallDamage = true;

	public float happinessLoss = 0.75f;

	public int droneToPlay;

	private BuildingEntity bld;

	private ParticleSystem.EmissionModule emmis;

	public bool burrowWalls;

	public float burrowWallDamage;

	public float lookForceMult = 1f;

	public bool hitConnected;

	public bool breakBone;

	public string biteSound = "spiderbite";

	public float minVectorDotToBite = -1f;

	public float retreatHealth;

	public static float strengthMult;

	private bool threatWorkaround;

	public void Start()
	{
		strengthMult = WorldGeneration.GetRunSettingFloat("enemystrength");
		rb = GetComponent<Rigidbody2D>();
		threatWorkaround = true;
		targets = new Vector2[legs.Length];
		target = base.transform.position;
		float num = Random.Range(0.5f, 1.5f);
		for (int i = 0; i < legs.Length; i++)
		{
			targets[i] = legs[i].transform.position;
			foreach (IKSegment segment in legs[i].segments)
			{
				segment.length *= num;
			}
		}
		bld = GetComponent<BuildingEntity>();
		maxHealth = bld.health;
		ParticleSystem component = Object.Instantiate(Resources.Load<GameObject>("BleedParticle"), base.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
		component.transform.SetParent(base.transform);
		emmis = component.emission;
	}

	private void Update()
	{
		if (!WorldGeneration.world.GetClosestChunkRenderer(WorldGeneration.world.WorldToBlockPos(base.transform.position)).enabled && !bld.ignoreBodyOptimize)
		{
			return;
		}
		timeOffset -= Time.deltaTime;
		biteCooldown -= Time.deltaTime;
		stunTime = Mathf.Max(stunTime - Time.deltaTime, 0f);
		if (stunTime > 0f)
		{
			rb.gravityScale = 1f;
		}
		else
		{
			rb.gravityScale = 0f;
		}
		for (int i = 0; i < legs.Length; i++)
		{
			float num = Vector2.Distance(legs[i].transform.position, targets[i]);
			if ((timeOffset <= 0f && num > legs[i].maxLength) || num > legs[i].maxLength * 1.4f)
			{
				targets[i] = ((Vector2)legs[i].transform.position - targets[i]).normalized * legs[i].maxLength + (Vector2)legs[i].transform.position;
				timeOffset = 0.17f;
			}
			legs[i].rootPos = legs[i].transform.position;
			legs[i].targetPos = Vector2.Lerp(legs[i].targetPos, targets[i], Time.deltaTime * legSpeed);
		}
		if (rb.bodyType == RigidbodyType2D.Static)
		{
			return;
		}
		moveTime -= Time.deltaTime;
		if (!(moveTime <= 0f))
		{
			return;
		}
		if (Random.value > Mathf.Lerp(bld.health, maxHealth, 0.2f) / maxHealth)
		{
			stunTime += 0.3f;
		}
		Collider2D collider2D = Physics2D.OverlapCircle(base.transform.position, seeDistance, LayerMask.GetMask("Body", "Limb"));
		if ((bool)collider2D)
		{
			if (Random.value > aggro || bld.health <= retreatHealth)
			{
				target = ((Vector2)base.transform.position - (Vector2)collider2D.transform.position).normalized * 15f + Random.insideUnitCircle * 5f + (Vector2)base.transform.position;
			}
			else
			{
				target = collider2D.transform.position;
				RaycastHit2D[] array = Physics2D.LinecastAll(base.transform.position, target, LayerMask.GetMask("Ground"));
				bool flag = false;
				RaycastHit2D[] array2 = array;
				foreach (RaycastHit2D raycastHit2D in array2)
				{
					if (raycastHit2D.transform != base.transform)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					PlayThreatMusic();
					PlayerCamera.main.SetTimeScale(PlayerCamera.SpeedType.Normal, switchSound: false);
					Sound.Play(attackSounds.PickRandom(), base.transform.position);
				}
			}
		}
		else
		{
			target = (Vector2)base.transform.position + Random.insideUnitCircle * moveRange;
		}
		moveTime = moveWaitTime.RandomFromRange();
	}

	private void OnWillRenderObject()
	{
		emmis.rateOverTime = (1f - bld.health / maxHealth) * 4f;
		if (FluidManager.main.GetLiquid(base.transform.position) == 6)
		{
			bld.health -= Time.deltaTime * 100f;
		}
	}

	private void FixedUpdate()
	{
		if (rb.bodyType != RigidbodyType2D.Static)
		{
			if (Vector2.Distance(base.transform.position, target) > 1.5f && stunTime <= 0f)
			{
				Vector2 vector = (target - (Vector2)base.transform.position).normalized * moveForce;
				rb.AddForce(vector * Time.deltaTime * (Mathf.Lerp(bld.health, maxHealth, 0.25f) / maxHealth) * 50f * ((timeOffset < 0.1f) ? 1f : 0.25f));
				float num = Vector2.SignedAngle(base.transform.up, (target - (Vector2)base.transform.position).normalized);
				rb.AddTorque(num * rb.mass * lookForceMult);
				rb.angularVelocity *= 0.92f;
			}
			else if (stunTime <= 0f)
			{
				rb.angularVelocity *= 0.95f;
			}
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (!(biteCooldown > 0f) && !(stunTime > 0f))
		{
			CheckForBlockDamage(collision);
			CheckForLimbDamage(collision);
		}
	}

	public virtual void DamageLimb(Limb limb)
	{
		if (Random.value < infectionChance * WorldGeneration.GetRunSettingFloat("infectionchance"))
		{
			limb.infected = true;
		}
		float armorReduction = limb.GetArmorReduction();
		limb.DamageWearables(biteMuscleDamage / 100f);
		limb.infectionAmount++;
		if (!limb.prostheticInfo.hasProsthetic)
		{
			limb.body.venomTotal += venomAmount * strengthMult;
		}
		limb.skinHealth -= biteSkinDamage * strengthMult / armorReduction;
		limb.muscleHealth -= biteMuscleDamage * strengthMult / armorReduction;
		limb.bleedAmount += bleedDamage * strengthMult / armorReduction;
		limb.body.adrenaline += 75f;
		limb.pain = bitePain * strengthMult / armorReduction;
		limb.body.stamina += biteMuscleDamage * 0.75f;
		if (breakBone)
		{
			limb.BreakBone();
		}
	}

	public void PlayThreatMusic()
	{
		PlayerCamera.main.currentThreatTheme = droneToPlay;
		if (WorldGeneration.world.biomeOverride == WorldGeneration.OverrideSceneType.Tutorial)
		{
			PlayerCamera.main.currentThreatTheme = 17;
		}
		PlayerCamera.main.threatMusicTime = 5f;
	}

	public void CheckForLimbDamage(Collision2D collision)
	{
		Limb limb = Body.LimbFromObject(collision.gameObject, base.transform.position);
		if (!limb || !(Vector2.Dot(((Vector2)collision.transform.position - collision.GetContact(0).point).normalized, base.transform.up) > minVectorDotToBite))
		{
			return;
		}
		biteCooldown = biteCoolToSet;
		Sound.Play(biteSound, base.transform.position);
		limb.body.eyeScareTime = 5f;
		limb.body.talker.Talk(Locale.GetCharacter("hitbycreature"), null, force: false, resetTalkTimer: true);
		limb.body.happiness -= happinessLoss;
		PlayThreatMusic();
		target = (base.transform.position - collision.transform.position).normalized * 15f + base.transform.position;
		moveTime = retreatMoveTime;
		DamageLimb(limb);
		if (hitConnected)
		{
			Limb[] connectedLimbs = limb.connectedLimbs;
			foreach (Limb limb2 in connectedLimbs)
			{
				DamageLimb(limb2);
			}
		}
		if (clawAnim)
		{
			GameObject obj = Object.Instantiate(Resources.Load<GameObject>("ClawAnim"));
			obj.transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.right, -collision.GetContact(0).normal));
			obj.transform.position = base.transform.position;
			obj.transform.SetParent(base.transform);
			Object.Destroy(obj, 5f);
		}
	}

	public void CheckForBlockDamage(Collision2D collision)
	{
		if (burrowWalls)
		{
			biteCooldown = biteCoolToSet * 0.3f;
			WorldGeneration.world.DamageBlock(WorldGeneration.world.WorldToBlockPos(collision.GetContact(0).point - collision.GetContact(0).normal * 0.5f), burrowWallDamage * strengthMult, hitSound: true, bonusMetal: false, ignoreLoot: true);
			if (collision.gameObject.TryGetComponent<Item>(out var component) && !component.Stats.secret)
			{
				Object.Destroy(component.gameObject);
			}
			if (collision.gameObject.TryGetComponent<BuildingEntity>(out var component2) && !component2.animal)
			{
				component2.health -= burrowWallDamage * 0.5f * strengthMult;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!(biteCooldown > 0f) && !(stunTime > 0f))
		{
			CheckForLimbDamage(collision);
			float magnitude = collision.relativeVelocity.magnitude;
			if (doFallDamage && collision.gameObject.layer == 6 && magnitude > 14f)
			{
				bld.health -= magnitude;
				Sound.Play("gore3", base.transform.position);
			}
			if (collision.gameObject.TryGetComponent<Item>(out var component) && magnitude > 2f && Vector2.Distance(base.transform.position, PlayerCamera.main.body.transform.position) < 50f)
			{
				float num = magnitude * Mathf.Clamp(component.rb.mass, 0f, 4f);
				Sound.Play("gore3", base.transform.position);
				Sound.Play("boneHit", base.transform.position);
				rb.velocity = Vector2.Lerp(rb.velocity, component.rb.velocity, 1f / rb.mass * 10f);
				component.rb.velocity *= -1f;
				bld.health -= num * 0.66f;
				threatWorkaround = false;
				AnimalHit(num * 1.5f);
				threatWorkaround = true;
			}
		}
	}

	public void AnimalHit(float damage)
	{
		stunTime += damage * stunMult;
		if (threatWorkaround)
		{
			PlayThreatMusic();
		}
		PlayerCamera.main.SetTimeScale(PlayerCamera.SpeedType.Normal, switchSound: false);
		moveTime *= 0.8f;
		if (stunTime > 0.1f)
		{
			for (int i = 0; i < targets.Length; i++)
			{
				targets[i] += Random.insideUnitCircle;
			}
		}
	}

	public void AnimalDeath()
	{
		Sound.Play("gore", base.transform.position);
		if (doDeathExplode)
		{
			Object.Instantiate(Resources.Load("BloodExplosion"), base.transform.position, Quaternion.identity);
		}
		if (Vector2.Distance(base.transform.position, PlayerCamera.main.body.transform.position) < 32f)
		{
			PlayerCamera.main.body.skills.AddExp(1, maxHealth / 100f);
		}
	}
}
