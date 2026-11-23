using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CrystalEnemy : MonoBehaviour
{
	private Rigidbody2D rb;

	private Light2D light;

	private SpriteRenderer sprite;

	private AudioSource source;

	private BuildingEntity build;

	private float timeBeforeAttack;

	private bool didAttackWarning;

	private bool stuck;

	private LineRenderer rend;

	public static int enemyCount;

	private Body body => PlayerCamera.main.body;

	private bool close => Vector2.Distance(base.transform.position, body.transform.position) < 64f;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		light = GetComponent<Light2D>();
		sprite = GetComponent<SpriteRenderer>();
		source = GetComponent<AudioSource>();
		build = GetComponent<BuildingEntity>();
		rend = base.transform.GetChild(0).GetComponent<LineRenderer>();
		rend.startColor = Color.clear;
		rend.endColor = Color.clear;
		rb.gravityScale = 0f;
	}

	private void Update()
	{
		if (close)
		{
			PlayerCamera.main.currentThreatTheme = 18;
			if (WorldGeneration.world.biomeOverride == WorldGeneration.OverrideSceneType.Tutorial)
			{
				PlayerCamera.main.currentThreatTheme = 17;
			}
			PlayerCamera.main.threatMusicTime = 2f;
		}
		float num = 1f - build.health / 450f;
		float b = 1f + rb.velocity.magnitude * 0.005f + Mathf.Clamp01(0f - timeBeforeAttack) * (stuck ? 0f : 1f) + num;
		float b2 = Mathf.Clamp01(0.3f + rb.velocity.magnitude * 0.005f) + Mathf.Clamp01(0f - timeBeforeAttack) * (stuck ? 0f : 1f);
		source.pitch = Mathf.Lerp(source.pitch, b, Time.deltaTime * 8f);
		source.volume = Mathf.Lerp(source.volume, b2, Time.deltaTime * 8f);
		if (!stuck)
		{
			if (close || timeBeforeAttack < 0f)
			{
				timeBeforeAttack -= Time.deltaTime;
			}
			if (timeBeforeAttack < 0f)
			{
				if (!didAttackWarning)
				{
					Sound.Play("crystalenemywindup", base.transform.position, twoDimensional: true);
					didAttackWarning = true;
				}
				rend.SetPosition(0, base.transform.position);
				RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, base.transform.up, 999f, LayerMask.GetMask("Ground"));
				if ((bool)raycastHit2D)
				{
					rend.SetPosition(1, raycastHit2D.point);
				}
				else
				{
					rend.SetPosition(1, base.transform.position + base.transform.up * 999f);
				}
				rend.startColor = Color.Lerp(Color.clear, sprite.color, (0f - timeBeforeAttack) * 2f);
				rend.widthMultiplier = timeBeforeAttack * 2f;
			}
			else
			{
				rend.endColor = Color.clear;
				rend.startColor = Color.clear;
			}
			if (timeBeforeAttack < -0.5f)
			{
				Lunge();
			}
		}
		else
		{
			if (body.conscious)
			{
				timeBeforeAttack -= Time.deltaTime;
			}
			rend.endColor = Color.Lerp(sprite.color, Color.clear, 0f - timeBeforeAttack);
			if (timeBeforeAttack < -2.25f - (float)(enemyCount - 1))
			{
				stuck = false;
				rb.velocity -= (Vector2)base.transform.up * 12f;
				timeBeforeAttack = Random.Range(0.5f, 4f + (float)(enemyCount - 1));
				rb.constraints = RigidbodyConstraints2D.None;
			}
		}
	}

	public void AnimalHit()
	{
	}

	public void AnimalDeath()
	{
		Sound.Play("crystalenemydeath", base.transform.position, twoDimensional: false, pitchShift: false);
		Utils.Create("Special/CrystalDistort", base.transform.position, 0f).AddComponent<CrystalDeathAnimation>();
	}

	public void Lunge()
	{
		stuck = true;
		didAttackWarning = false;
		rend.endColor = rend.startColor;
		rend.startColor = Color.clear;
		RaycastHit2D[] array = Physics2D.RaycastAll(base.transform.position, base.transform.up);
		bool flag = false;
		RaycastHit2D[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit2D raycastHit2D = array2[i];
			if (Utils.GetBody(raycastHit2D.transform.gameObject, out var body) && !flag)
			{
				flag = true;
				Limb limb = body.limbs.Where((Limb x) => !x.dismembered).ToList().PickRandom();
				float armorReduction = limb.GetArmorReduction();
				limb.DamageWearables(0.4f);
				limb.muscleHealth -= 35f / armorReduction;
				limb.skinHealth -= 50f / armorReduction;
				limb.pain += 35f / armorReduction;
				limb.bleedAmount += 15f / armorReduction;
				body.adrenaline += 70f;
				body.stamina = 100f;
				body.eyePanicTime = 0.5f;
				body.Scream();
				body.Ragdoll();
				body.DoGoreSound();
				build.health -= 100f;
				Sound.Play("crystalenemylaugh", base.transform.position, twoDimensional: true);
			}
			if (raycastHit2D.transform.gameObject.layer == 6)
			{
				base.transform.position = raycastHit2D.point;
				rb.MovePosition(raycastHit2D.point);
				rb.velocity = Vector2.zero;
				rb.constraints = RigidbodyConstraints2D.FreezeAll;
				Sound.Play("crystalenemylodge", base.transform.position, twoDimensional: true);
				break;
			}
		}
	}

	private void FixedUpdate()
	{
		Vector2 vector = (Vector2)body.transform.position + Vector2.up * 12f - (Vector2)base.transform.position;
		rb.angularVelocity = 0f;
		if (stuck)
		{
			return;
		}
		if (timeBeforeAttack <= 0f)
		{
			rb.velocity *= 0.9f;
			if (timeBeforeAttack > -0.25f)
			{
				rb.velocity -= (Vector2)base.transform.up * Time.fixedDeltaTime * 70f;
			}
			else
			{
				rb.velocity += (Vector2)base.transform.up * Time.fixedDeltaTime * 350f;
			}
		}
		else
		{
			rb.velocity += vector.normalized * 90f * Time.fixedDeltaTime;
			rb.velocity *= 0.97f;
			rb.MoveRotation(Mathf.LerpAngle(rb.rotation, Vector2.SignedAngle(Vector2.up, (body.transform.position - base.transform.position).normalized), Time.fixedDeltaTime * 6f));
		}
	}

	private void Start()
	{
		timeBeforeAttack = Random.Range(2f, 5f);
		rb.mass = 20f;
		enemyCount++;
		if (enemyCount >= 6)
		{
			SteamManager.TryUnlockAchievement("CENTEROFATTENTION");
		}
	}

	private void OnDestroy()
	{
		enemyCount--;
	}

	public void SetColor(Color clr)
	{
		clr.r *= Random.Range(0.9f, 1f);
		clr.g *= Random.Range(0.9f, 1f);
		clr.b *= Random.Range(0.9f, 1f);
		sprite.color = clr;
		light.color = clr;
		light.intensity = Random.Range(0.5f, 1f);
	}
}
