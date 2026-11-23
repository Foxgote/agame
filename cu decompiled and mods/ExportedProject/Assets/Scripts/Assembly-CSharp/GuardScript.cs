using System.Collections;
using UnityEngine;

public class GuardScript : MonoBehaviour
{
	public SpriteRenderer reticle;

	public Transform arms;

	public Transform head;

	public Transform barrel;

	public SpriteRenderer laserLine;

	public SpriteRenderer laserDot;

	public SpriteRenderer armSprite;

	public SpriteRenderer myBody;

	public Sprite armsNormal;

	public Sprite armsStun;

	public ParticleSystem tesla;

	public Transform gunSpawn;

	public Transform beltSpawn;

	private BuildingEntity bld;

	private Animator animator;

	private Vector2 reticlePos;

	private Vector2 reticleVel;

	public bool isAiming;

	private int scale = 1;

	public float shootTime;

	public static float MAX_ACTIVE_DISTANCE = 128f;

	private Collider2D col;

	private int ammo = 17;

	private float reloadtime;

	private bool didDie;

	public bool doingMelee;

	private Coroutine meleeCoroutine;

	private float meleeCooldown;

	private Vector2 hurtOffset;

	private Vector2 visualHurtOffset;

	private float stun;

	private float walkCycleProgress;

	private float moveSpeed = 8f;

	private float velocity;

	private float desiredPos;

	private float posChangeTime;

	public Sprite[] bodyMoveSprites;

	public Sprite bodyStaySprite;

	private RangeF moveBounds;

	public bool blockMovementUntilMelee;

	public static bool anyGuardKilled = false;

	private Body body => PlayerCamera.main.body;

	private bool stunned => stun > 1f;

	private void Start()
	{
		reticlePos = body.transform.position;
		reticleVel = Vector2.zero;
		shootTime = Random.Range(0.66f, 1f);
		col = GetComponent<Collider2D>();
		bld = GetComponent<BuildingEntity>();
		animator = base.transform.GetChild(0).GetComponent<Animator>();
		Vector2 vector = FindMovementBounds(base.transform.position);
		moveBounds = new RangeF(vector.x, vector.y);
		desiredPos = base.transform.position.x;
	}

	public void Die()
	{
		anyGuardKilled = true;
		didDie = true;
		col.enabled = false;
		animator.enabled = true;
		animator.Play("GuardFall");
		laserLine.enabled = false;
		laserDot.enabled = false;
		Sound.Play("bodyFall2", base.transform.position);
		GameObject obj = Utils.Create("pistol", gunSpawn.transform.position, gunSpawn.transform.eulerAngles.z);
		obj.GetComponent<GunScript>().hasMag = ammo >= 0;
		obj.GetComponent<GunScript>().roundsInMag = Mathf.Max(0, ammo - 1);
		obj.GetComponent<GunScript>().roundInChamber = ((ammo <= 0) ? GunScript.RoundInChamber.None : GunScript.RoundInChamber.Round);
		if (ammo <= 0)
		{
			Utils.Create("smallmagazine", beltSpawn.transform.position, Random.value * 360f);
		}
		if (Random.value < 0.25f)
		{
			Utils.Create(new string[9] { "sterilizedbandage", "bruisekit", "woundglue", "adhesivebandage", "bloodcoagulant", "combatpen", "boneweldingtool", "lrd", "riflemagazine" }.PickRandom(), beltSpawn.transform.position, Random.value * 360f);
		}
		if (meleeCoroutine != null)
		{
			doingMelee = false;
			StopCoroutine(meleeCoroutine);
		}
	}

	private void Update()
	{
		if (bld.health <= 0f && !didDie)
		{
			Die();
		}
		hurtOffset = Vector2.Lerp(hurtOffset, didDie ? Vector2.zero : (Vector2.down * (450f - bld.health) * 0.003f), Time.deltaTime * 6f * ((stunned && !didDie) ? 0.15f : 1f));
		if (hurtOffset.x > 0.3f)
		{
			hurtOffset.x = 0.3f;
		}
		if (hurtOffset.x < -0.3f)
		{
			hurtOffset.x = -0.3f;
		}
		visualHurtOffset = Vector2.Lerp(visualHurtOffset, hurtOffset, Time.deltaTime * 8f);
		animator.transform.localPosition = new Vector2((0f - visualHurtOffset.x) * 3f, visualHurtOffset.y * 0.35f);
		animator.transform.localEulerAngles = new Vector3(0f, 0f, visualHurtOffset.x * 70f);
		if (didDie)
		{
			arms.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(arms.transform.localEulerAngles.z, 0f, Time.deltaTime * 8f));
			return;
		}
		float deltaTime = Time.deltaTime;
		Vector2 vector = ((ammo > 0) ? ((Vector2)body.transform.position) : ((Vector2)base.transform.position + Vector2.right * scale * 5f + Vector2.down * 8f));
		if (Vector2.Distance(base.transform.position, vector) > MAX_ACTIVE_DISTANCE)
		{
			return;
		}
		stun = Mathf.MoveTowards(stun, 0f, Time.deltaTime * 1.66f);
		if (!doingMelee)
		{
			laserDot.enabled = !stunned;
			laserLine.enabled = !stunned;
			armSprite.sprite = (stunned ? armsStun : armsNormal);
		}
		if (doingMelee || stunned)
		{
			arms.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(arms.transform.localEulerAngles.z, 0f, Time.deltaTime * 8f));
			velocity = 0f;
			return;
		}
		meleeCooldown -= Time.deltaTime;
		reticleVel += Vector2.ClampMagnitude(vector - reticlePos, 2.5f) * deltaTime * 60f;
		reticleVel *= Mathf.Pow(0.95f * ((ammo > 0) ? 1f : 0.9f), deltaTime / Time.fixedDeltaTime);
		reticlePos += reticleVel * deltaTime;
		if (reticlePos.x < base.transform.position.x && scale == 1)
		{
			scale = -1;
			base.transform.localScale = new Vector2(-1f, 1f);
		}
		if (reticlePos.x > base.transform.position.x && scale == -1)
		{
			scale = 1;
			base.transform.localScale = new Vector2(1f, 1f);
		}
		float num = Vector2.SignedAngle(Vector2.right * scale, reticlePos - (Vector2)arms.position);
		arms.rotation = Quaternion.Euler(0f, 0f, num);
		head.rotation = Quaternion.Euler(0f, 0f, num * 0.7f);
		float num2 = Vector2.Distance(laserLine.transform.position, reticlePos);
		bool flag = true;
		RaycastHit2D[] array = Physics2D.LinecastAll(laserLine.transform.position, reticlePos, LayerMask.GetMask("Ground"));
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit2D raycastHit2D = array[i];
			if (!(raycastHit2D.collider == col))
			{
				num2 = raycastHit2D.distance;
				flag = false;
				break;
			}
		}
		laserLine.transform.localScale = new Vector2(1f, num2);
		laserDot.transform.localPosition = (Vector2)laserLine.transform.localPosition + Vector2.right * num2;
		if (Mathf.Abs(base.transform.position.x - (body.transform.position.x + body.lastTimeStepVelocity.x * 0.2f)) < 4.5f && meleeCooldown < 0f && ((body.transform.position.x > base.transform.position.x && scale == 1) || (body.transform.position.x < base.transform.position.x && scale == -1)) && Mathf.Abs(base.transform.position.y - body.transform.position.y) < 8f && !stunned && body.conscious && !WorldGeneration.world.cutsceneActive)
		{
			meleeCoroutine = StartCoroutine(MeleeAttack());
			return;
		}
		if (flag && body.conscious && !stunned && ammo > 0 && !WorldGeneration.world.cutsceneActive)
		{
			shootTime -= Time.deltaTime;
			PlayerCamera.main.threatMusicTime = 5f;
			if (shootTime <= 0f)
			{
				Fire();
			}
		}
		if (ammo <= 0)
		{
			reloadtime += Time.deltaTime;
			if (reloadtime > 4f)
			{
				reloadtime = 0f;
				ammo = 17;
			}
		}
		posChangeTime -= Time.deltaTime;
		if (posChangeTime < 0f && !blockMovementUntilMelee)
		{
			posChangeTime = Random.Range(0.8f, 2.5f);
			desiredPos = moveBounds.RandomFromRange();
			if (!flag)
			{
				desiredPos = body.transform.position.x;
			}
		}
		float num3 = 0f;
		if (Mathf.Abs(base.transform.position.x - desiredPos) > 0.2f)
		{
			num3 = ((base.transform.position.x > desiredPos) ? (-1f) : 1f);
		}
		velocity = Mathf.MoveTowards(velocity, num3 * moveSpeed, Time.deltaTime * moveSpeed * 5f);
		if (base.transform.position.x + velocity * Time.deltaTime > moveBounds.max)
		{
			velocity = 0f;
		}
		if (base.transform.position.x + velocity * Time.deltaTime < moveBounds.min)
		{
			velocity = 0f;
		}
		walkCycleProgress += velocity / 8f * Time.deltaTime;
		if (walkCycleProgress >= 1f)
		{
			walkCycleProgress--;
		}
		if (walkCycleProgress < 0f)
		{
			walkCycleProgress++;
		}
		if (velocity != 0f)
		{
			myBody.sprite = bodyMoveSprites[Mathf.FloorToInt(walkCycleProgress * 4f)];
		}
		else
		{
			myBody.sprite = bodyStaySprite;
			walkCycleProgress = 0.35f;
		}
		base.transform.position += Vector3.right * velocity * Time.deltaTime;
	}

	private IEnumerator MeleeAttack()
	{
		StartCoroutine(MeleeAnim());
		blockMovementUntilMelee = false;
		PlayerCamera.main.threatMusicTime = 5f;
		doingMelee = true;
		laserLine.enabled = false;
		laserDot.enabled = false;
		animator.enabled = true;
		animator.Play("GuardSwing", -1, 0f);
		yield return new WaitForSeconds(0.46f);
		Collider2D collider2D = Physics2D.OverlapBox((Vector2)base.transform.position + Vector2.right * scale * 3f + Vector2.up * 1.5f, new Vector2(2f, 7f), 0f, LayerMask.GetMask("Body", "Limb"));
		if ((bool)collider2D && Utils.GetBody(collider2D.gameObject, out var body))
		{
			Limb closestLimb = body.GetClosestLimb(arms.transform.position);
			body.Ragdoll();
			body.SetVelocity(Vector2.right * scale * 25f);
			closestLimb.rb.velocity *= 1.25f;
			if (body.standing)
			{
				body.shock = 35f;
			}
			closestLimb.skinHealth -= 5f;
			closestLimb.muscleHealth -= 8f;
			closestLimb.pain += 35f;
			closestLimb.bleedAmount += 1.25f;
			GameObject obj = Utils.Create("Special/Tesla", closestLimb.transform);
			obj.transform.localPosition = Vector3.zero;
			Object.Destroy(obj, 1.5f);
			this.body.CreateCloudBig(closestLimb.transform.position, closestLimb.rb.velocity);
			Sound.Play("stunhit", Vector2.zero, twoDimensional: true);
		}
		yield return new WaitForSeconds(0.46f);
		animator.Update(1f);
		animator.enabled = false;
		laserLine.enabled = true;
		laserDot.enabled = true;
		doingMelee = false;
		meleeCooldown = Random.Range(-0.5f, 1.46f);
	}

	private IEnumerator MeleeAnim()
	{
		yield return new WaitForSeconds(0.225f);
		Sound.Play("guardswing", base.transform.position);
		tesla.Play();
		yield return new WaitForSeconds(0.5f);
		tesla.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
	}

	public bool ReticleOverlapsPlayer()
	{
		return Physics2D.OverlapPoint(reticlePos, LayerMask.GetMask("Limb", "Body"));
	}

	public void AnimalHit(float dmg)
	{
		ParticleSystem.EmissionModule emission = Utils.Create("HumanBlood", base.transform.position, 0f).GetComponent<ParticleSystem>().emission;
		ParticleSystem.Burst burst = emission.GetBurst(0);
		burst.count = dmg;
		emission.SetBurst(0, burst);
		Vector2 vector = new Vector2(Random.value * dmg * 0.01f, (0f - dmg) * 0.01f);
		hurtOffset += vector;
		PlayerCamera.main.threatMusicTime = 5f;
		stun += dmg * 0.01f;
		if (hurtOffset.y < -5f)
		{
			hurtOffset.y = -5f;
		}
	}

	public void Fire()
	{
		shootTime = ((Random.value < 0.25f) ? Random.Range(0.16f, 0.35f) : Random.Range(0.66f, 1.75f));
		TurretScript.Shoot(new FireInfo
		{
			pos = barrel.position,
			dir = barrel.right * scale,
			doTinnitus = false,
			ignoreBody = !ReticleOverlapsPlayer(),
			structureDamage = 90f,
			animalDamage = 90f,
			playerDamageMultiplier = 0.35f,
			ignoreTrans = base.transform
		});
		Sound.Play("pistolshot", base.transform.position);
		reticleVel += Vector2.up * 30f;
		ammo--;
	}

	public Vector2 FindMovementBounds(Vector2 spawnPosition)
	{
		int mask = LayerMask.GetMask("Ground");
		RaycastHit2D[] array = Physics2D.RaycastAll(spawnPosition, Vector2.down, 100f, mask);
		RaycastHit2D raycastHit2D = default;
		bool flag = false;
		RaycastHit2D[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit2D raycastHit2D2 = array2[i];
			if (raycastHit2D2.collider != null && raycastHit2D2.collider != col && !raycastHit2D2.transform.CompareTag("Guard"))
			{
				raycastHit2D = raycastHit2D2;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return new Vector2(spawnPosition.x, spawnPosition.x);
		}
		float y = raycastHit2D.point.y;
		float x = spawnPosition.x;
		float y2 = spawnPosition.x;
		float num = y;
		for (float num2 = spawnPosition.x - 1f; num2 >= spawnPosition.x - 100f; num2--)
		{
			RaycastHit2D[] array3 = Physics2D.RaycastAll(new Vector2(num2, y + 1f), Vector2.down, 100f, mask);
			RaycastHit2D raycastHit2D3 = default;
			bool flag2 = false;
			array2 = array3;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit2D raycastHit2D4 = array2[i];
				if (raycastHit2D4.collider != null && raycastHit2D4.collider != col && !raycastHit2D4.transform.CompareTag("Guard"))
				{
					raycastHit2D3 = raycastHit2D4;
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				break;
			}
			float y3 = raycastHit2D3.point.y;
			if (Mathf.Abs(y3 - num) > 0.3f)
			{
				break;
			}
			RaycastHit2D[] array4 = Physics2D.RaycastAll(new Vector2(num2 - 1f, y3 + 0.05f), Vector2.left, 0.05f, mask);
			bool flag3 = false;
			array2 = array4;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit2D raycastHit2D5 = array2[i];
				if (raycastHit2D5.collider != null && raycastHit2D5.collider != col && !raycastHit2D5.transform.CompareTag("Guard"))
				{
					flag3 = true;
					break;
				}
			}
			if (flag3)
			{
				break;
			}
			RaycastHit2D[] array5 = Physics2D.RaycastAll(new Vector2(num2 + 1f - 0.01f, y3 + 1f), Vector2.down, 100f, mask);
			RaycastHit2D raycastHit2D6 = default;
			bool flag4 = false;
			array2 = array5;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit2D raycastHit2D7 = array2[i];
				if (raycastHit2D7.collider != null && raycastHit2D7.collider != col && !raycastHit2D7.transform.CompareTag("Guard"))
				{
					raycastHit2D6 = raycastHit2D7;
					flag4 = true;
					break;
				}
			}
			if (!flag4 || Mathf.Abs(raycastHit2D6.point.y - y3) > 0.3f)
			{
				break;
			}
			x = num2;
			num = y3;
		}
		num = y;
		for (float num3 = spawnPosition.x + 1f; num3 <= spawnPosition.x + 100f; num3++)
		{
			RaycastHit2D[] array6 = Physics2D.RaycastAll(new Vector2(num3, y + 1f), Vector2.down, 100f, mask);
			RaycastHit2D raycastHit2D8 = default;
			bool flag5 = false;
			array2 = array6;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit2D raycastHit2D9 = array2[i];
				if (raycastHit2D9.collider != null && raycastHit2D9.collider != col && !raycastHit2D9.transform.CompareTag("Guard"))
				{
					raycastHit2D8 = raycastHit2D9;
					flag5 = true;
					break;
				}
			}
			if (!flag5)
			{
				break;
			}
			float y4 = raycastHit2D8.point.y;
			if (Mathf.Abs(y4 - num) > 0.3f)
			{
				break;
			}
			RaycastHit2D[] array7 = Physics2D.RaycastAll(new Vector2(num3 + 1f, y4 + 0.05f), Vector2.right, 0.05f, mask);
			bool flag6 = false;
			array2 = array7;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit2D raycastHit2D10 = array2[i];
				if (raycastHit2D10.collider != null && raycastHit2D10.collider != col && !raycastHit2D10.transform.CompareTag("Guard"))
				{
					flag6 = true;
					break;
				}
			}
			if (flag6)
			{
				break;
			}
			RaycastHit2D[] array8 = Physics2D.RaycastAll(new Vector2(num3 - 1f + 0.01f, y4 + 1f), Vector2.down, 100f, mask);
			RaycastHit2D raycastHit2D11 = default;
			bool flag7 = false;
			array2 = array8;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit2D raycastHit2D12 = array2[i];
				if (raycastHit2D12.collider != null && raycastHit2D12.collider != col && !raycastHit2D12.transform.CompareTag("Guard"))
				{
					raycastHit2D11 = raycastHit2D12;
					flag7 = true;
					break;
				}
			}
			if (!flag7 || Mathf.Abs(raycastHit2D11.point.y - y4) > 0.3f)
			{
				break;
			}
			y2 = num3;
			num = y4;
		}
		return new Vector2(x, y2);
	}
}
