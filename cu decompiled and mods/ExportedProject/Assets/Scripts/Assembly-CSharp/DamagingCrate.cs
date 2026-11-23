using UnityEngine;

public class DamagingCrate : MonoBehaviour
{
	private Rigidbody2D rb;

	private Vector2 prevFrameSpeed;

	public int type;

	public string stalactiteHitSound = "stalactitehit";

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		base.gameObject.AddComponent<LiquidAffect>();
	}

	private void Update()
	{
		prevFrameSpeed = rb.velocity;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!(prevFrameSpeed.magnitude > 6f))
		{
			return;
		}
		if (type == -1)
		{
			if (collision.gameObject.TryGetComponent<Body>(out var component))
			{
				Sound.Play("bodyFall4", base.transform.position);
				component.liquidSlipTime += 0.7f;
			}
			return;
		}
		if (type == 0)
		{
			if (base.transform.position.y > collision.transform.position.y && collision.gameObject.TryGetComponent<Body>(out var component2))
			{
				Sound.Play("bodyFall4", base.transform.position);
				component2.consciousness = 0f;
			}
			return;
		}
		Limb limb = Body.LimbFromObject(collision.gameObject, collision.GetContact(0).point);
		if (!limb || !(collision.relativeVelocity.magnitude > 6f))
		{
			return;
		}
		if (type == 1)
		{
			if (limb.isHead)
			{
				limb.body.consciousness -= 80f / limb.GetArmorReduction();
			}
			limb.muscleHealth -= prevFrameSpeed.magnitude * 0.75f / limb.GetArmorReduction();
			limb.skinHealth -= prevFrameSpeed.magnitude / limb.GetArmorReduction();
			limb.bleedAmount += prevFrameSpeed.magnitude * 0.1f / limb.GetArmorReduction();
			limb.DamageWearables(0.1f);
		}
		else
		{
			if (limb.isHead)
			{
				limb.body.consciousness -= 100f / limb.GetArmorReduction();
			}
			limb.muscleHealth -= prevFrameSpeed.magnitude * 1.25f / limb.GetArmorReduction();
			limb.skinHealth -= prevFrameSpeed.magnitude * 0.6f / limb.GetArmorReduction();
			limb.bleedAmount += prevFrameSpeed.magnitude * 0.05f / limb.GetArmorReduction();
			if (Random.value < 0.5f && prevFrameSpeed.magnitude > 25f)
			{
				limb.Dislocate();
			}
			if (Random.value < 0.5f && prevFrameSpeed.magnitude > 45f)
			{
				limb.BreakBone();
			}
			limb.DamageWearables(0.25f);
		}
		Sound.Play(stalactiteHitSound, base.transform.position, twoDimensional: true);
		limb.body.Ragdoll();
		Object.Destroy(this);
	}
}
