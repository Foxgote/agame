using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpikeStabberScript : MonoBehaviour
{
	private bool activated;

	private Light2D light;

	public Light2D bonusLight;

	public string sound = "spikestabber";

	public float damageMult = 1f;

	public int behaviour;

	private float randOffset;

	public Sprite hitSpikeSprite;

	public SpriteRenderer spikeRenderer;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ((bool)collision.GetComponent<Rigidbody2D>() && !collision.GetComponent<Rigidbody2D>().isKinematic && !activated)
		{
			Stab();
			if (collision.TryGetComponent<Body>(out var component))
			{
				component.Ragdoll();
			}
			if (collision.TryGetComponent<Item>(out var component2))
			{
				component2.SetCondition(0f);
			}
		}
	}

	private void Start()
	{
		light = GetComponent<Light2D>();
		randOffset = Random.Range(-100f, 100f);
	}

	private void OnWillRenderObject()
	{
		if (behaviour == 0)
		{
			light.enabled = !activated && (int)(Time.time + randOffset) % 2 == 0;
		}
		else if (behaviour == 1)
		{
			bonusLight.intensity = (activated ? 0f : Mathf.PingPong((Time.time + randOffset) * 0.34f, 0.2f));
		}
	}

	public void Stab()
	{
		activated = true;
		GetComponent<Animator>().Play("SpikeStab");
		Sound.Play(sound, base.transform.position);
		GetComponent<BuildingEntity>().description = Locale.GetBuilding("spikestabberdscused");
	}

	public void CheckStab()
	{
		RaycastHit2D[] array = Physics2D.RaycastAll(base.transform.position, base.transform.up, 6f);
		bool flag = false;
		RaycastHit2D[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit2D raycastHit2D = array2[i];
			if (!(raycastHit2D.transform != base.transform) || !raycastHit2D.transform.TryGetComponent<Limb>(out var component) || !(Random.Range(0f, 1f) < 0.5f))
			{
				continue;
			}
			component.rb.AddForce(base.transform.up * 300f * damageMult, ForceMode2D.Impulse);
			component.skinHealth -= Random.Range(50f, 80f) / component.GetArmorReduction() * damageMult;
			component.muscleHealth -= Random.Range(30f, 50f) / component.GetArmorReduction() * damageMult;
			component.bleedAmount += Random.Range(0f, 40f) / component.GetArmorReduction() * damageMult;
			component.pain += Random.Range(70f, 100f) / component.GetArmorReduction() * damageMult;
			component.body.shock = 70f * damageMult;
			component.body.adrenaline = 100f;
			component.DamageWearables(1f * damageMult);
			if (Random.Range(0f, 1f) < 0.3f / component.GetArmorReduction() * damageMult)
			{
				component.BreakBone();
			}
			if (component.isHead && damageMult > 0.5f)
			{
				component.body.consciousness = 0f;
				if (Random.Range(0f, 1f) < 0.5f * damageMult)
				{
					component.body.brainHealth -= Random.Range(5f, 70f) * damageMult / component.GetArmorReduction();
					component.body.Disfigure();
				}
				else
				{
					component.body.RemoveEye();
				}
			}
			if (component.isHead || component.isVital)
			{
				component.body.internalBleeding += Random.Range(16f, 30f) * damageMult / component.GetArmorReduction();
			}
			component.body.Scream();
			flag = true;
			PlayerCamera.main.shaker.Shake(50f);
		}
		if (flag)
		{
			Sound.Play("loudStab", base.transform.position);
			spikeRenderer.sprite = hitSpikeSprite;
		}
		Collider2D[] array3 = Physics2D.OverlapCircleAll(base.transform.position, 15f, LayerMask.GetMask("Body", "Limb"));
		foreach (Collider2D obj in array3)
		{
			if (obj.TryGetComponent<Body>(out var component2))
			{
				component2.eyeScareTime = 4f;
				component2.eyePanicTime = 0.5f;
			}
			if (obj.TryGetComponent<Limb>(out var component3))
			{
				component3.body.eyeScareTime = 4f;
				component3.body.eyePanicTime = 0.5f;
			}
		}
	}
}
