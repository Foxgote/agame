using System.Collections;
using UnityEngine;

public class Vomiter : MonoBehaviour
{
	private Body body;

	[SerializeField]
	private float vomitTime;

	private float bloodVomitTime;

	public float badLookAmount;

	private bool vomiting;

	private float vomitPower;

	private void Start()
	{
		body = GetComponent<Body>();
	}

	private void Update()
	{
		if (body.sicknessAmount > 50f)
		{
			vomitTime += Time.deltaTime * body.sicknessAmount * 0.01f;
		}
		else
		{
			vomitTime += Time.deltaTime * body.sicknessAmount * 0.0025f;
		}
		if (body.TryGetComponent<Painkillers>(out var component) && component.actualOpiateReception < 0f)
		{
			vomitTime -= Mathf.Clamp(component.actualOpiateReception, -25f, 0f) * Time.deltaTime * 0.02f;
		}
		bloodVomitTime += body.internalBleeding * Time.deltaTime * 0.01f;
		if (bloodVomitTime > 15f)
		{
			bloodVomitTime = 0f;
			VomitBlood();
		}
		if (vomitTime > 60f)
		{
			vomitTime = 0f;
			Vomit();
		}
		badLookAmount = Mathf.MoveTowards(badLookAmount, 0f, Time.deltaTime);
	}

	private IEnumerator DoVomit()
	{
		float timer = 0f;
		while (timer < 5f)
		{
			if (body.conscious)
			{
				PlayerCamera.main.SetTimeScale(PlayerCamera.SpeedType.Normal, switchSound: false);
			}
			body.eyeScareTime = 0.5f;
			body.temporarySlowdown = timer * 0.15f;
			timer += Time.deltaTime;
			badLookAmount = timer * 0.4f;
			body.consciousness = Mathf.Min(body.consciousness, 100f - timer * 13.5f);
			yield return null;
		}
		if (body.alive)
		{
			body.DropItem(2);
			GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("vomitParticle"), body.limbs[0].transform);
			Object.Destroy(gameObject, 10f);
			body.happiness -= 2.5f;
			body.hunger -= 17f + vomitPower * 5f;
			body.thirst -= 10f + vomitPower * 5f;
			body.sicknessAmount -= 8f + vomitPower * 5f;
			body.talker.Invoke("Vomit", 1.5f);
			body.eatTime = 0.7f + vomitPower;
			body.bloodVolume--;
			if (vomitPower > 0f)
			{
				ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
				body.internalBleeding += vomitPower * 10f;
				ParticleSystem.EmissionModule emission = component.emission;
				ParticleSystem.Burst[] array = new ParticleSystem.Burst[emission.burstCount];
				emission.GetBursts(array);
				array[0].cycleCount += (int)(vomitPower * 15f);
				emission.SetBursts(array);
			}
			body.temporarySlowdown = 0.95f;
			Sound.Play("vomit" + Random.Range(1, 3), base.transform.position);
			if (body.sleeping && !body.GetComponent<SleepingPills>())
			{
				body.WakeUp();
			}
		}
		vomiting = false;
		vomitPower = 0f;
	}

	private IEnumerator DoBloodVomit()
	{
		float timer = 0f;
		while (timer < 5f)
		{
			if (body.conscious)
			{
				PlayerCamera.main.SetTimeScale(PlayerCamera.SpeedType.Normal, switchSound: false);
			}
			body.eyeScareTime = 0.5f;
			body.temporarySlowdown = timer * 0.15f;
			timer += Time.deltaTime;
			badLookAmount = timer * 0.4f;
			body.consciousness = Mathf.Min(body.consciousness, 100f - timer * 10f);
			yield return null;
		}
		if (body.alive)
		{
			body.DropItem(2);
			Object.Destroy(Object.Instantiate(Resources.Load<GameObject>("vomitBloodParticle"), body.limbs[0].transform), 10f);
			body.happiness -= 0.8f;
			body.eyeScareTime = 5f;
			body.temporarySlowdown = 0.6f;
			body.talker.Invoke("VomitBlood", 1.5f);
			body.thirst--;
			body.eatTime = 0.7f;
			body.bloodVolume--;
			Sound.Play("vomit" + Random.Range(1, 3), base.transform.position);
			if (body.sleeping && !body.GetComponent<SleepingPills>())
			{
				body.WakeUp();
			}
		}
	}

	public void Vomit()
	{
		if (vomiting)
		{
			vomitPower += 1f / (vomitPower + 1f);
			return;
		}
		if (body.conscious)
		{
			Sound.Play("vomitwarning", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
		}
		vomiting = true;
		StartCoroutine(DoVomit());
	}

	public void VomitBlood()
	{
		if (body.conscious)
		{
			Sound.Play("bloodvomitwarning", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
		}
		StartCoroutine(DoBloodVomit());
	}
}
