using System;
using UnityEngine;

public class FanTrap : MonoBehaviour
{
	public SpriteRenderer fan1;

	public SpriteRenderer fan2;

	private bool[] front;

	private float fanProgress;

	private float spinSpeed;

	private float desiredSpinSpeed;

	private AudioSource src;

	private SpriteRenderer spr;

	private float speedMult = 1f;

	private bool dislodged;

	private BuildingEntity bld;

	private Rigidbody2D rb;

	private void Start()
	{
		spinSpeed = UnityEngine.Random.Range(0.2f, 5f);
		desiredSpinSpeed = spinSpeed;
		src = GetComponent<AudioSource>();
		spr = GetComponent<SpriteRenderer>();
		front = new bool[2] { true, true };
		bld = GetComponent<BuildingEntity>();
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		fanProgress = (fanProgress + spinSpeed * Time.deltaTime) % 1f;
		spinSpeed = Mathf.MoveTowards(spinSpeed, desiredSpinSpeed, Time.deltaTime);
	}

	public float SinForSpin(float progress)
	{
		return Mathf.Sin(progress * 2f * MathF.PI);
	}

	public float DepthForSpin(float progress)
	{
		return Mathf.Cos(progress * 2f * MathF.PI);
	}

	public void BreakFan(int index)
	{
		SpriteRenderer spriteRenderer = ((index == 1) ? fan2 : fan1);
		BuildingEntity.CreateBreakParticle(spriteRenderer);
		GameObject obj = Utils.Create("scrapmetal", spriteRenderer.transform.position, UnityEngine.Random.value * 360f);
		obj.GetComponent<Item>().condition = UnityEngine.Random.value * 0.2f;
		obj.GetComponent<Rigidbody2D>().velocity = new Vector2(UnityEngine.Random.Range(-4f, 4f), UnityEngine.Random.value * 10f);
		spriteRenderer.gameObject.SetActive(value: false);
		Sound.Play("fansnap", base.transform.position);
		speedMult -= 0.5f;
		if (speedMult <= 0f)
		{
			GetComponent<BuildingEntity>().description = Locale.GetBuilding("fandsc2");
		}
	}

	private void FixedUpdate()
	{
		if (dislodged)
		{
			rb.AddForce(base.transform.up * speedMult * spinSpeed * 750f);
		}
	}

	private void OnWillRenderObject()
	{
		if (speedMult <= 0f)
		{
			src.Stop();
			return;
		}
		if (!dislodged && WorldGeneration.world.GetBlock(bld.blockPlacedOn) == 0)
		{
			dislodged = true;
			rb.bodyType = RigidbodyType2D.Dynamic;
			rb.mass = 20f;
		}
		SpriteRenderer[] array = new SpriteRenderer[2] { fan1, fan2 };
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].gameObject.activeSelf)
			{
				continue;
			}
			float num = SinForSpin(fanProgress + (float)i / (float)array.Length);
			float num2 = DepthForSpin(fanProgress + (float)i / (float)array.Length);
			array[i].transform.localPosition = Vector3.right * 3.375f * num;
			array[i].transform.localScale = new Vector3((Mathf.Abs(num) <= 0.07f) ? (Mathf.Sign(num) * 0.07f) : num, 1f + num2 * 0.25f);
			array[i].sortingOrder = ((num2 > 0f) ? 500 : (-500));
			float num3 = num2.Remap(-1f, 1f, 0.4f, 1f);
			array[i].color = new Color(num3, num3, num3);
			array[i].flipY = num < 0f;
			bool num4 = front[i];
			front[i] = num2 > 0f;
			if (num4 == front[i])
			{
				continue;
			}
			Vector2 vector = array[i].transform.right.normalized * array[i].transform.localScale.x;
			if (vector == Vector2.zero)
			{
				vector = Vector2.right;
			}
			RaycastHit2D[] array2 = Physics2D.RaycastAll(base.transform.position, vector, 6.5f, LayerMask.GetMask("Ground", "Item", "Body", "Limb"));
			bool flag = false;
			float num5 = spinSpeed;
			RaycastHit2D[] array3 = array2;
			for (int j = 0; j < array3.Length; j++)
			{
				RaycastHit2D raycastHit2D = array3[j];
				if (raycastHit2D.collider.TryGetComponent<BuildingEntity>(out var component) && component.gameObject != base.gameObject)
				{
					component.health -= 80f * num5;
					if (spinSpeed > 3f)
					{
						flag = true;
					}
					if ((bool)raycastHit2D.rigidbody)
					{
						raycastHit2D.rigidbody.AddForceAtPosition(vector * 80f * spinSpeed, vector, ForceMode2D.Impulse);
					}
					Sound.Play(component.hitSound, raycastHit2D.point);
					if (dislodged)
					{
						rb.velocity -= vector * spinSpeed * 2f;
					}
					spinSpeed *= 0.15f;
					break;
				}
				if (raycastHit2D.collider.TryGetComponent<Item>(out var component2))
				{
					component2.SetCondition(component2.condition * 0.75f - 0.25f * spinSpeed);
					component2.rb.velocity = vector * spinSpeed * 10f;
					if (dislodged)
					{
						rb.velocity -= vector * spinSpeed * 0.5f;
					}
					Sound.Play("drop", raycastHit2D.point);
					spinSpeed *= 0.88f;
					break;
				}
				if (WorldGeneration.world.GetBlock(raycastHit2D.point - raycastHit2D.normal * 0.5f) > 0)
				{
					WorldGeneration.world.DamageBlock(raycastHit2D.point - raycastHit2D.normal * 0.5f, spinSpeed * 10f);
					if (dislodged)
					{
						rb.velocity -= vector * spinSpeed * 5f;
					}
					spinSpeed *= 0.8f;
					break;
				}
				if (!Utils.GetBody(raycastHit2D.collider.gameObject, out var body))
				{
					continue;
				}
				Limb closestLimb = body.GetClosestLimb(raycastHit2D.point);
				body.DoGoreSound();
				if (spinSpeed >= 2f)
				{
					body.Ragdoll();
					flag = true;
				}
				else
				{
					body.crouchAmount += 0.3f;
					body.SetVelocity(vector * spinSpeed * 7.5f);
					body.liquidSlipTime += 0.35f;
				}
				closestLimb.rb.velocity = vector * spinSpeed * 5f;
				Limb[] connectedLimbs = closestLimb.connectedLimbs;
				for (int k = 0; k < connectedLimbs.Length; k++)
				{
					connectedLimbs[k].rb.velocity = vector * spinSpeed * 5f;
				}
				connectedLimbs = body.limbs;
				for (int k = 0; k < connectedLimbs.Length; k++)
				{
					_ = connectedLimbs[k];
					closestLimb.rb.velocity += vector * spinSpeed * 2.5f;
				}
				closestLimb.skinHealth -= 9f * spinSpeed;
				closestLimb.muscleHealth -= 7.5f * spinSpeed;
				closestLimb.bleedAmount += 4f * spinSpeed;
				closestLimb.pain += 7f * spinSpeed;
				body.adrenaline += 10f * spinSpeed;
				body.eyeScareTime = 1f;
				if (spinSpeed > 4.15f)
				{
					if (closestLimb.broken)
					{
						closestLimb.Dismember();
					}
					else
					{
						closestLimb.BreakBone();
					}
				}
				if (spinSpeed > 2.25f)
				{
					Sound.Play("loudstab", raycastHit2D.point);
				}
				if (dislodged)
				{
					rb.velocity -= vector * spinSpeed * 2f;
				}
				spinSpeed *= 0.35f;
				break;
			}
			if (flag)
			{
				BreakFan(i);
			}
		}
		src.pitch = spinSpeed * 0.62f * speedMult;
		src.volume = Mathf.Clamp01(spinSpeed * 0.25f) * 0.66f;
	}
}
