using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TurretScript : MonoBehaviour
{
	public bool didBeep;

	private float beepTime;

	private bool didShoot;

	public Light2D fireLight;

	public Transform barrel;

	public Sprite normSprite;

	public Sprite fireSprite;

	public SpriteRenderer lightSprite;

	private SpriteRenderer spr;

	private BuildingEntity build;

	private float explodeCount;

	private bool didcountsound;

	public string shotSoundOverride;

	public float shotPowerMultiplier = 1f;

	public float timeSinceFired { get; private set; }

	private void Start()
	{
		timeSinceFired = 999f;
		build = GetComponent<BuildingEntity>();
		spr = GetComponent<SpriteRenderer>();
		GetComponentInChildren<ParticleSystem>().transform.localScale = new Vector3(base.transform.localScale.x, 1f, 1f);
	}

	private void Update()
	{
		spr.sprite = ((timeSinceFired < 2f) ? fireSprite : normSprite);
		timeSinceFired += Time.deltaTime;
		fireLight.intensity = Mathf.Max(0f, 80f - timeSinceFired * 300f);
		lightSprite.enabled = !didShoot || Mathf.Sin(Time.time * 10f) > 0f;
		if (timeSinceFired > 15f && didShoot)
		{
			didBeep = false;
			didShoot = false;
			beepTime = 0f;
			Sound.Play("reload", base.transform.position, twoDimensional: false, pitchShift: false);
		}
		if (didBeep)
		{
			beepTime += Time.deltaTime;
			if (beepTime >= 0.5f && !didShoot)
			{
				didShoot = true;
				timeSinceFired = 0f;
				GetComponentInChildren<ParticleSystem>().Play();
				Sound.Play("rifleshot", barrel.position, twoDimensional: true, pitchShift: false);
				Shoot(new FireInfo
				{
					pos = barrel.position,
					dir = base.transform.right * base.transform.localScale.x,
					ignoreTrans = base.transform,
					playerDamageMultiplier = shotPowerMultiplier
				});
			}
		}
		else if (Vector2.SqrMagnitude(base.transform.position - PlayerCamera.main.body.transform.position) < 10000f)
		{
			RaycastHit2D[] array = Physics2D.RaycastAll(barrel.position, base.transform.right * base.transform.localScale.x, 200f, LayerMask.GetMask("Ground", "Body", "Limb"));
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit2D raycastHit2D = array[i];
				if (raycastHit2D.transform == base.transform)
				{
					continue;
				}
				if (raycastHit2D.transform.gameObject.layer == 6)
				{
					break;
				}
				if (Utils.GetBody(raycastHit2D.transform.gameObject, out var body))
				{
					if (!(body.temperature <= 32f))
					{
						Sound.Play("turretsee", base.transform.position, twoDimensional: true, pitchShift: false);
						PlayerCamera.main.body.talker.Talk("..!");
						beepTime = (spr.isVisible ? 0f : (-0.5f));
						didBeep = true;
						break;
					}
					SteamManager.TryUnlockAchievement("THERMALCAMOUFLAGE");
				}
			}
		}
		if (build.health < 350f)
		{
			if (!didcountsound)
			{
				didcountsound = true;
				Sound.Play("turretcount", base.transform.position, twoDimensional: false, pitchShift: false);
			}
			explodeCount += Time.deltaTime;
			if (explodeCount > 2f)
			{
				explodeCount = 0f;
				build.health = 0f;
				GetComponent<Collider2D>().enabled = false;
				WorldGeneration.CreateExplosion(new ExplosionParams
				{
					position = base.transform.position,
					range = 9f,
					structuralDamage = 200f,
					boneBreakChance = 0f,
					dislocationChance = 0.05f,
					muscleDamage = new RangeF(0f, 35f),
					velocity = 15f,
					disfigureChance = 0.2f
				});
			}
		}
	}

	public static void Shoot(FireInfo info)
	{
		RaycastHit2D[] array = Physics2D.RaycastAll(info.pos, info.dir, 200f, LayerMask.GetMask("Ground", "Body", "Limb", "Descriptor"));
		Vector2 vector = info.pos + info.dir * 200f;
		if (info.doTinnitus && Vector2.Distance(info.pos, PlayerCamera.main.body.transform.position) < 20f && !PlayerCamera.main.body.HasHearingProtection())
		{
			Sound.Play("tinnitus", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
			PlayerCamera.main.body.hearingLoss += Random.Range(10f, 15f) * info.playerDamageMultiplier;
			PlayerCamera.main.body.eyePanicTime = 0.6f;
			PlayerCamera.main.body.eyeCloseTime = 4f;
			PlayerCamera.main.body.eyeScareTime = 8f;
			PlayerCamera.main.body.talker.Talk(Locale.GetCharacter("loud"));
			PlayerCamera.main.shaker.Shake();
		}
		float num = Random.Range(0.85f, 1.15f);
		List<Body> list = new List<Body>();
		RaycastHit2D[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit2D raycastHit2D = array2[i];
			if (raycastHit2D.transform == info.ignoreTrans)
			{
				continue;
			}
			if ((bool)raycastHit2D.rigidbody)
			{
				raycastHit2D.rigidbody.AddForceAtPosition(info.dir * info.structureDamage, raycastHit2D.point, ForceMode2D.Impulse);
			}
			PlayerCamera.main.body.CreateCloudSmall(raycastHit2D.point, Vector2.zero);
			if (raycastHit2D.collider.TryGetComponent<BuildingEntity>(out var component) && !component.cantHit)
			{
				component.health -= (component.animal ? (info.animalDamage * num) : (info.structureDamage * num));
				if (component.animal)
				{
					component.gameObject.SendMessage("AnimalHit", info.animalDamage * num, SendMessageOptions.DontRequireReceiver);
					Sound.Play("gore" + Random.Range(1, 6), raycastHit2D.point);
					WorldGeneration.CreateDamageNumber(raycastHit2D.point, (int)(info.animalDamage * num));
				}
				else
				{
					WorldGeneration.CreateDamageNumber(raycastHit2D.point, (int)(info.structureDamage * num));
				}
				if (raycastHit2D.collider.gameObject.layer == 6)
				{
					vector = raycastHit2D.point;
					break;
				}
			}
			else if (raycastHit2D.collider.gameObject.layer == 6)
			{
				WorldGeneration.world.DamageBlock(raycastHit2D.point + info.dir * 0.5f, info.structureDamage * num);
				WorldGeneration.CreateDamageNumber(raycastHit2D.point, (int)(info.structureDamage * num));
				vector = raycastHit2D.point;
				break;
			}
			if (raycastHit2D.transform.gameObject.CompareTag("Scientist") && (bool)EndLabScript.instance)
			{
				EndLabScript.instance.BeginEscapeSequence();
			}
			if (info.ignoreBody)
			{
				continue;
			}
			Body component2 = null;
			if (raycastHit2D.collider.TryGetComponent<Limb>(out var component3))
			{
				component2 = component3.body;
			}
			if ((!component2 && !raycastHit2D.collider.TryGetComponent<Body>(out component2)) || list.Contains(component2))
			{
				continue;
			}
			list.Add(component2);
			component2.DoGoreSound();
			component2.rb.velocity += info.dir * 15f * info.playerDamageMultiplier;
			component2.lastTimeStepVelocity += info.dir * 15f * info.playerDamageMultiplier;
			if (info.playerDamageMultiplier > 0.4f)
			{
				component2.Ragdoll();
			}
			else
			{
				component2.shock -= 30f;
			}
			Limb limb = (info.forceHead ? component2.limbs[0] : component2.GetClosestLimb(raycastHit2D.point));
			limb.pain += Random.Range(80f, 100f) * info.playerDamageMultiplier / limb.GetArmorReduction();
			limb.body.adrenaline = 100f;
			limb.skinHealth -= Random.Range(70f, 100f) * info.playerDamageMultiplier / limb.GetArmorReduction();
			limb.muscleHealth -= Random.Range(70f, 100f) * info.playerDamageMultiplier / limb.GetArmorReduction();
			limb.bleedAmount += Random.Range(30f, 40f) * info.playerDamageMultiplier / limb.GetArmorReduction();
			limb.shrapnel = Mathf.Clamp(Mathf.RoundToInt(info.playerDamageMultiplier * 5f), 0, 5);
			limb.DamageWearables(1f * info.playerDamageMultiplier);
			if (limb.isVital || limb.isHead)
			{
				component2.internalBleeding += Random.Range(18f, 22f) * info.playerDamageMultiplier / limb.GetArmorReduction();
			}
			if (Random.Range(0f, 1f) < 0.3f * (info.playerDamageMultiplier * info.playerDamageMultiplier))
			{
				if (info.playerDamageMultiplier > 0.4f)
				{
					limb.BreakBone();
				}
				else
				{
					limb.Dislocate();
				}
			}
			if (limb.isHead && info.playerDamageMultiplier > 0.4f)
			{
				component2.consciousness = 0f;
				component2.brainHealth -= Random.Range(10f, 85f) * info.playerDamageMultiplier / limb.GetArmorReduction();
				if (Random.value < 0.5f * info.playerDamageMultiplier)
				{
					component2.Disfigure();
				}
				else
				{
					component2.RemoveEye();
				}
				PlayerCamera.main.DoAchievementDelayed(600f, "AINTTHATAKICKTOTHEHEAD");
			}
		}
		GameObject obj = (GameObject)Object.Instantiate(Resources.Load("Special/TurretLine"), Vector2.zero, Quaternion.identity);
		obj.GetComponent<LineRenderer>().SetPosition(0, info.pos);
		obj.GetComponent<LineRenderer>().SetPosition(1, vector);
		Object.Destroy(obj, 0.05f);
	}
}
