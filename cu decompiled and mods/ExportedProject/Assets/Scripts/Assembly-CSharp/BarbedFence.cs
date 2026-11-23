using UnityEngine;

public class BarbedFence : MonoBehaviour
{
	private float soundPlayCooldown;

	public Sprite hitSprite;

	private void Update()
	{
		soundPlayCooldown -= Time.deltaTime;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent<Body>(out var component) && soundPlayCooldown <= 0f)
		{
			component.Ragdoll();
		}
		if (collision.TryGetComponent<Limb>(out var component2))
		{
			if (soundPlayCooldown <= 0f)
			{
				soundPlayCooldown = 10f;
				Sound.Play("fence", base.transform.position);
			}
			component2.skinHealth -= Random.Range(10f, 25f) / component2.GetArmorReduction();
			component2.bleedAmount += Random.Range(1f, 3f) / component2.GetArmorReduction();
			component2.pain += Random.Range(20f, 60f) / component2.GetArmorReduction();
			component2.DamageWearables(0.2f);
			component2.body.Scream();
			GetComponent<SpriteRenderer>().sprite = hitSprite;
		}
	}
}
