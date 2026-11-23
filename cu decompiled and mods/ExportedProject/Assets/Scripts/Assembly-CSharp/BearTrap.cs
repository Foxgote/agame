using UnityEngine;

public class BearTrap : MonoBehaviour
{
	public Sprite closeSprite;

	private Sprite origSprite;

	public Limb caughtLimb;

	private bool activated;

	private Vector2 origPos;

	private void Start()
	{
		origSprite = GetComponent<SpriteRenderer>().sprite;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (activated)
		{
			return;
		}
		if (other.TryGetComponent<Body>(out var component))
		{
			component.shock = 20f;
			component.Ragdoll();
		}
		if (!other.TryGetComponent<Limb>(out var component2))
		{
			return;
		}
		activated = true;
		component2.body.shock = 100f;
		component2.pain += 100f;
		component2.body.adrenaline += 80f;
		if (Random.Range(0f, 1f) < 0.5f)
		{
			component2.BreakBone();
		}
		origPos = component2.transform.position;
		component2.skinHealth -= Random.Range(70f, 100f) / component2.GetArmorReduction();
		component2.muscleHealth -= Random.Range(50f, 100f) / component2.GetArmorReduction();
		component2.bleedAmount += Random.Range(14f, 18f) / component2.GetArmorReduction();
		component2.DamageWearables(0.9f);
		if (component2.isHead)
		{
			component2.body.consciousness = 0f;
			component2.body.brainHealth -= Random.Range(0f, 40f);
			if (Random.value > 0.6f)
			{
				component2.body.RemoveEye();
			}
		}
		Sound.Play("beartrap", base.transform.position, twoDimensional: false, pitchShift: false);
		Sound.Play("gore", component2.transform.position);
		caughtLimb = component2;
		component2.rb.constraints = RigidbodyConstraints2D.FreezeAll;
		GetComponent<SpriteRenderer>().sprite = closeSprite;
		Object.Destroy(base.transform.GetChild(0).gameObject);
		PlayerCamera.main.shaker.Shake(50f);
	}

	private void Update()
	{
		if ((bool)caughtLimb && caughtLimb.body.standing)
		{
			caughtLimb.rb.constraints = RigidbodyConstraints2D.None;
			caughtLimb = null;
			Sound.Play("beartrapunlatch", base.transform.position);
			GetComponent<SpriteRenderer>().sprite = origSprite;
		}
		if ((bool)caughtLimb)
		{
			caughtLimb.rb.MovePosition(origPos);
		}
	}

	private void OnDestroy()
	{
		if ((bool)caughtLimb)
		{
			caughtLimb.rb.constraints = RigidbodyConstraints2D.None;
			caughtLimb = null;
		}
	}
}
