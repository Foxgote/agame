using UnityEngine;

public class SawbladeScript : MonoBehaviour
{
	public Sprite sawBloody;

	private BuildingEntity bld;

	private float limbCooldown;

	private void Start()
	{
		bld = GetComponent<BuildingEntity>();
	}

	private void OnWillRenderObject()
	{
		base.transform.eulerAngles += Vector3.forward * Time.deltaTime * 1800f;
		limbCooldown -= Time.deltaTime;
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.TryGetComponent<BuildingEntity>(out var component))
		{
			component.health -= Time.deltaTime * 50f;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Limb limb = Body.LimbFromObject(collision.gameObject, collision.GetContact(0).point);
		if ((bool)limb && limbCooldown <= 0f)
		{
			limb.body.SetVelocity((limb.body.transform.position - base.transform.position).normalized * 30f);
			limb.body.Ragdoll();
			limb.body.shock += 6f;
			limb.rb.velocity *= 1.7f;
			limb.skinHealth -= 70f / limb.GetArmorReduction();
			if (limb.muscleHealth <= 10f && Random.value < 0.5f && !limb.isAbdomen && !limb.isHead && !limb.isVital)
			{
				limb.Dismember();
			}
			limb.muscleHealth -= 30f / limb.GetArmorReduction();
			limb.bleedAmount += Random.Range(13f, 20f) / limb.GetArmorReduction();
			limb.body.adrenaline += 100f;
			limb.pain += 85f / limb.GetArmorReduction();
			limb.body.DoFurTuft();
			limb.body.DoGoreSound();
			limb.DamageWearables(1f);
			bld.health -= 10f;
			bld.health *= 0.25f;
			limbCooldown = 0.35f;
			GetComponent<SpriteRenderer>().sprite = sawBloody;
			Sound.Play("Saw", base.transform.position, twoDimensional: true);
		}
		if (collision.gameObject.TryGetComponent<Item>(out var component))
		{
			component.SetCondition(component.condition - 0.8f);
			component.rb.velocity = (component.transform.position - base.transform.position).normalized * 30f;
			Sound.Play("Saw", base.transform.position);
			bld.health -= 5f;
			bld.health *= 0.5f;
		}
	}
}
