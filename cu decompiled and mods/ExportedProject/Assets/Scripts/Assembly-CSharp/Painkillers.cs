using Newtonsoft.Json;
using UnityEngine;

[Saveable]
[JsonObject(MemberSerialization.OptIn)]
public class Painkillers : MonoBehaviour
{
	private Body body;

	[JsonProperty]
	public float opiateAmount;

	[JsonProperty]
	public float opiateTolerance;

	[JsonProperty]
	public float opiateReception;

	[JsonProperty]
	public float antagonistAmount;

	[JsonProperty]
	public float actualOpiateReception;

	[JsonProperty]
	public bool permanent;

	public float consciousnessReduction => Mathf.Max(0f, actualOpiateReception * 0.7f);

	private void Awake()
	{
		body = GetComponent<Body>();
	}

	private void Update()
	{
		antagonistAmount = Mathf.Max(antagonistAmount - Time.deltaTime, 0f);
		opiateReception = opiateAmount - opiateTolerance;
		actualOpiateReception = Mathf.MoveTowards(actualOpiateReception, opiateReception, Time.deltaTime * 5f);
		if (opiateTolerance < opiateAmount)
		{
			opiateTolerance = Mathf.MoveTowards(opiateTolerance, opiateAmount, Time.deltaTime * 0.08f);
		}
		else
		{
			opiateTolerance = Mathf.MoveTowards(opiateTolerance, opiateAmount, Time.deltaTime * 0.04f);
		}
		if (opiateReception > 45f)
		{
			opiateTolerance = Mathf.MoveTowards(opiateTolerance, opiateAmount, Time.deltaTime * 0.022f);
		}
		if (opiateReception < -30f)
		{
			opiateTolerance = Mathf.MoveTowards(opiateTolerance, opiateAmount, Time.deltaTime * 0.025f);
		}
		if (opiateTolerance < 0f)
		{
			opiateTolerance = 0f;
		}
		if (permanent && opiateTolerance < 40f)
		{
			opiateTolerance = 40f;
		}
		opiateAmount = Mathf.MoveTowards(opiateAmount, 0f, Time.deltaTime * 0.059f);
		if (opiateAmount > 100f)
		{
			opiateAmount = Mathf.MoveTowards(opiateAmount, 0f, Time.deltaTime * 0.05f);
		}
		if (opiateAmount > 130f)
		{
			opiateAmount = Mathf.MoveTowards(opiateAmount, 0f, Time.deltaTime * 0.07f);
		}
		if (opiateAmount > 160f)
		{
			opiateAmount = Mathf.MoveTowards(opiateAmount, 0f, Time.deltaTime * 0.09f);
		}
		if (opiateAmount > 195f)
		{
			opiateAmount = Mathf.MoveTowards(opiateAmount, 0f, Time.deltaTime * 0.11f);
		}
		if (opiateAmount > 235f)
		{
			opiateAmount = Mathf.MoveTowards(opiateAmount, 0f, Time.deltaTime * 0.13f);
		}
		if (opiateAmount > 260f)
		{
			opiateAmount = Mathf.MoveTowards(opiateAmount, 0f, Time.deltaTime * 0.15f);
		}
		if (opiateAmount > 300f)
		{
			opiateAmount = Mathf.MoveTowards(opiateAmount, 0f, Time.deltaTime * 0.18f);
		}
		if (antagonistAmount > 0f)
		{
			opiateAmount = Mathf.MoveTowards(opiateAmount, 0f, Time.deltaTime * 8f);
		}
		if (actualOpiateReception > 0f)
		{
			Limb[] limbs = body.limbs;
			for (int i = 0; i < limbs.Length; i++)
			{
				limbs[i].pain -= Time.deltaTime * (actualOpiateReception * 0.3f);
			}
		}
		if (actualOpiateReception > 0f)
		{
			body.opiateHappiness = actualOpiateReception;
		}
		else
		{
			body.opiateHappiness = actualOpiateReception * 1.66f;
			if (body.opiateHappiness < -80f)
			{
				body.opiateHappiness = -80f;
			}
		}
		if (actualOpiateReception > 30f)
		{
			body.energy -= Time.deltaTime * actualOpiateReception * 0.0025f;
		}
		if (actualOpiateReception > 5f && body.happiness > -60f)
		{
			body.happiness += Time.deltaTime * actualOpiateReception * 3E-05f;
		}
		if (actualOpiateReception < -16f)
		{
			body.sicknessAmount += Time.deltaTime * 0.09f;
		}
		if (actualOpiateReception < -15f && body.sleeping && body.energy > 45f && !body.GetComponent<SleepingPills>())
		{
			body.WakeUp();
		}
		if (actualOpiateReception < -34f)
		{
			body.TryStartFibrillation(forced: true);
		}
		if (opiateAmount == 0f && opiateTolerance == 0f)
		{
			body.opiateHappiness = 0f;
			Object.Destroy(this);
		}
	}
}
