using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CoilScript : MonoBehaviour
{
	public float cooldown;

	public ParticleSystem part;

	public Light2D light;

	private void Awake()
	{
		light = GetComponent<Light2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (cooldown <= 0f)
		{
			if (collision.collider.TryGetComponent<Body>(out var component))
			{
				component.Ragdoll();
			}
			if (collision.collider.TryGetComponent<Limb>(out var component2))
			{
				Shock(component2);
			}
		}
	}

	public void Shock(Limb limb)
	{
		if (cooldown <= 0f)
		{
			limb.body.Ragdoll();
			Limb[] limbs = limb.body.limbs;
			foreach (Limb limb2 in limbs)
			{
				limb2.muscleHealth -= Random.Range(0f, 40f);
				limb2.skinHealth -= Random.Range(0f, 25f);
				limb2.pain = 100f;
				limb2.rb.velocity = (limb2.transform.position - base.transform.position).normalized * 30f;
			}
			limb.body.temperature += 5f;
			limb.body.consciousness = 0f;
			limb.body.shock = 100f;
			limb.body.respiratoryRate = 0f;
			limb.body.adrenaline = 50f;
			limb.body.fibrillationProgress = 0f;
			cooldown = 30f;
			light.intensity = 1f;
			Sound.Play("zap", base.transform.position);
			PlayerCamera.main.shaker.Shake();
			part.Stop();
		}
	}

	private void OnWillRenderObject()
	{
		cooldown -= Time.deltaTime;
		if (cooldown <= 0f && !part.isPlaying)
		{
			part.Play();
		}
		light.intensity = Mathf.MoveTowards(light.intensity, 0f, Time.deltaTime * 6f);
	}
}
