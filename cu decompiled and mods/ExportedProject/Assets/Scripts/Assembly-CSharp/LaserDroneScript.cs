using UnityEngine;

public class LaserDroneScript : MonoBehaviour
{
	public Sprite openSprite;

	public bool open;

	public float openAmount;

	public bool hidden;

	private Rigidbody2D rb;

	public Transform head;

	public Transform armR;

	public Transform armL;

	public Transform laser;

	public Transform barrel;

	public Vector2[] posClosed;

	public Vector2[] posOpen;

	public SpriteRenderer render;

	public SpriteRenderer overheatSprite;

	public SpriteRenderer[] jetpacks;

	public Vector2 shootTarget;

	private BuildingEntity bld;

	private float laserAngle;

	private float transformTime;

	public float heat;

	private bool overheat;

	public ParticleSystem smoke;

	private float fireTime = 1.5f;

	private Body body => PlayerCamera.main.body;

	public float buildHealthNormalized => bld.health / 1000f;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		bld = GetComponent<BuildingEntity>();
		base.transform.eulerAngles += Vector3.forward * Random.Range(-45f, 45f);
		rb.gravityScale = 0.5f;
		transformTime = Random.Range(4f, 10f);
		GetComponent<LookTarget>().enabled = false;
	}

	public void AddHeat(float amt)
	{
		heat += amt;
		if (heat > 100f)
		{
			heat = 100f;
			overheat = true;
			Sound.Play("droneOverheat", base.transform.position, twoDimensional: false, pitchShift: false, base.transform);
			smoke.Play();
		}
	}

	public void BuildingHit()
	{
		if (bld.health <= 0f)
		{
			Sound.Play("droneBreak", base.transform.position, twoDimensional: false, pitchShift: false);
		}
	}

	private void Update()
	{
		if (!hidden)
		{
			transformTime -= Time.deltaTime * (overheat ? 3f : 1f);
		}
		openAmount = Mathf.MoveTowards(openAmount, open ? 1f : 0f, Time.deltaTime * 2f);
		heat = Mathf.MoveTowards(heat, 0f, Time.deltaTime * 15f * (open ? 1f : 2f));
		if (heat <= 0f)
		{
			overheat = false;
			if (smoke.isPlaying)
			{
				smoke.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
			}
		}
		if (open)
		{
			PlayerCamera.main.currentThreatTheme = 19;
			if (WorldGeneration.world.biomeOverride == WorldGeneration.OverrideSceneType.Tutorial)
			{
				PlayerCamera.main.currentThreatTheme = 17;
			}
			PlayerCamera.main.threatMusicTime = 5f;
			shootTarget = body.transform.position;
			float target = Vector2.SignedAngle(Vector2.up, (shootTarget - (Vector2)laser.transform.position).normalized);
			laserAngle = Mathf.MoveTowardsAngle(laserAngle, target, Time.deltaTime * 75f);
			laser.eulerAngles = new Vector3(0f, 0f, laserAngle);
			if (transformTime <= 0f)
			{
				transformTime = Random.Range(1f, 2.5f);
				Close();
			}
			fireTime -= Time.deltaTime;
			if (fireTime < 0f)
			{
				FireShot();
				fireTime = Random.Range(0.01f, 2.3f);
			}
		}
		else if (!hidden && transformTime <= 0f && Vector2.Distance(base.transform.position, body.transform.position) < 64f && !overheat)
		{
			transformTime = Random.Range(4f, 10f);
			Open();
		}
	}

	public void FireShot()
	{
		if (overheat)
		{
			return;
		}
		AddHeat(30f);
		RaycastHit2D[] array = Physics2D.RaycastAll(barrel.position, barrel.up, 64f, LayerMask.GetMask("Ground", "Body", "Limb"));
		Vector2 vector = barrel.position + barrel.up * 64f;
		float num = Random.Range(0.85f, 1.15f);
		float num2 = num * 50f;
		float num3 = num * 15f;
		RaycastHit2D[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit2D raycastHit2D = array2[i];
			if (raycastHit2D.transform == base.transform)
			{
				continue;
			}
			if (raycastHit2D.collider.TryGetComponent<BuildingEntity>(out var component) && !component.cantHit)
			{
				component.health -= num2;
				if (component.animal)
				{
					component.gameObject.SendMessage("AnimalHit", num2);
					Sound.Play("gore" + Random.Range(1, 6), raycastHit2D.point);
				}
				WorldGeneration.CreateDamageNumber(raycastHit2D.point, (int)num2);
			}
			else if (raycastHit2D.collider.gameObject.layer == 6)
			{
				WorldGeneration.world.DamageBlock(raycastHit2D.point + (Vector2)barrel.up * 0.5f, num2);
				WorldGeneration.CreateDamageNumber(raycastHit2D.point, (int)num2);
				vector = raycastHit2D.point;
				break;
			}
			if ((bool)raycastHit2D.rigidbody)
			{
				raycastHit2D.rigidbody.AddForceAtPosition(barrel.up * num2, raycastHit2D.point, ForceMode2D.Impulse);
			}
			PlayerCamera.main.body.CreateCloudSmall(raycastHit2D.point, Vector2.zero);
			Body component2 = null;
			if (raycastHit2D.collider.TryGetComponent<Limb>(out var component3))
			{
				component2 = component3.body;
			}
			if ((bool)component2 || raycastHit2D.collider.TryGetComponent<Body>(out component2))
			{
				component2.DoGoreSound();
				Limb closestLimb = component2.GetClosestLimb(raycastHit2D.point);
				float armorReduction = closestLimb.GetArmorReduction();
				closestLimb.pain += num3 / armorReduction;
				closestLimb.body.adrenaline += 25f;
				closestLimb.skinHealth -= num3 / armorReduction;
				closestLimb.muscleHealth -= num3 / armorReduction * 0.25f;
				closestLimb.DamageWearables(0.015f);
				component2.stamina += 35f;
				component2.temperature -= 0.5f;
				vector = raycastHit2D.point;
				break;
			}
		}
		GameObject obj = (GameObject)Object.Instantiate(Resources.Load("Special/DroneCryoLine"), Vector2.zero, Quaternion.identity);
		obj.GetComponent<LineRenderer>().SetPosition(0, barrel.position);
		obj.GetComponent<LineRenderer>().SetPosition(1, vector);
		Sound.Play("droneFire1", vector, twoDimensional: false, pitchShift: false);
		obj.AddComponent<LaserDroneCryoLine>();
	}

	private void OnWillRenderObject()
	{
		if (hidden && Vector2.Distance(body.transform.position, base.transform.position) < 10f)
		{
			Open();
		}
		head.localPosition = Vector2.Lerp(posClosed[0], posOpen[0], openAmount);
		armR.localPosition = Vector2.Lerp(posClosed[1], posOpen[1], openAmount);
		armL.localPosition = Vector2.Lerp(posClosed[2], posOpen[2], openAmount);
		laser.localPosition = Vector2.Lerp(posClosed[3], posOpen[3], openAmount);
		if (open)
		{
			SpriteRenderer[] array = jetpacks;
			foreach (SpriteRenderer obj in array)
			{
				obj.color = obj.color.WithAlpha(Random.value);
			}
			armR.rotation = Quaternion.identity;
			armL.rotation = Quaternion.identity;
		}
		else
		{
			SpriteRenderer[] array = jetpacks;
			foreach (SpriteRenderer obj2 in array)
			{
				obj2.color = obj2.color.WithAlpha(0f);
			}
			armR.localRotation = Quaternion.identity;
			armL.localRotation = Quaternion.identity;
			laser.localRotation = Quaternion.identity;
		}
		overheatSprite.color = overheatSprite.color.WithAlpha((heat - 40f) / 60f);
	}

	private void FixedUpdate()
	{
		if (open)
		{
			Vector2 normalized = ((Vector2)(body.transform.position + Vector3.up * (11f + (1f - buildHealthNormalized) * 5f)) + Vector2.right * 8f * Mathf.Sin(Time.time * 1.6f) + Vector2.up * 3f * Mathf.Sin(Time.time * 1.2f) - (Vector2)base.transform.position).normalized;
			normalized.x *= 0.5f;
			rb.velocity += normalized * 75f * Time.fixedDeltaTime;
			rb.angularVelocity += (0f - rb.rotation) * Time.fixedDeltaTime * 12f;
			rb.angularVelocity *= 0.98f;
			rb.velocity *= 0.98f;
		}
	}

	public void Open()
	{
		if (!open)
		{
			if (hidden)
			{
				hidden = false;
				render.sprite = openSprite;
				bld.ignoreBodyOptimize = true;
				bld.fullName = Locale.GetBuilding("laserdrone2");
				bld.description = Locale.GetBuilding("laserdronedsc2");
				rb.bodyType = RigidbodyType2D.Dynamic;
				GetComponent<LookTarget>().enabled = true;
			}
			bld.health /= 2f;
			rb.velocity += Vector2.up * 6f;
			open = true;
			Sound.Play("droneOpen", base.transform.position, twoDimensional: false, pitchShift: false, base.transform);
		}
	}

	public void Close()
	{
		if (open)
		{
			bld.health *= 2f;
			open = false;
			Sound.Play("droneClose", base.transform.position, twoDimensional: false, pitchShift: false, base.transform);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!open && collision.transform == body.transform && collision.relativeVelocity.magnitude > 8f && base.transform.position.y > body.transform.position.y)
		{
			body.Ragdoll();
			Sound.Play("bodyFall4", base.transform.position);
		}
	}
}
