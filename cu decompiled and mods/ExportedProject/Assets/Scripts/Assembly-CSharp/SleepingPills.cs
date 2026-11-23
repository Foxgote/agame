using Newtonsoft.Json;
using UnityEngine;

[Saveable]
[JsonObject(MemberSerialization.OptIn)]
public class SleepingPills : MonoBehaviour
{
	[JsonProperty]
	public float amount;

	private Body body;

	private bool wasSleeping;

	private void Awake()
	{
		body = GetComponent<Body>();
	}

	private void Update()
	{
		amount -= Time.deltaTime;
		if (body.sleeping)
		{
			wasSleeping = true;
		}
		else if (wasSleeping)
		{
			wasSleeping = false;
			amount *= 0.75f;
		}
		if (amount > ((TryGetComponent<Painkillers>(out var component) && component.actualOpiateReception > 15f) ? 150f : 900f))
		{
			body.overdoseIndex = 2;
			body.bloodPressure -= Time.deltaTime * 2.5f;
			if (body.respiratoryRate > 70f)
			{
				body.respiratoryRate = Mathf.MoveTowards(body.respiratoryRate, 70f, Time.deltaTime * 15f);
			}
			if (!body.conscious)
			{
				body.bloodPressure -= Time.deltaTime * 2.5f;
				body.heartRate -= Time.deltaTime;
				if (body.respiratoryRate > 45f)
				{
					body.respiratoryRate = Mathf.MoveTowards(body.respiratoryRate, 40f, Time.deltaTime * 15f);
				}
			}
		}
		if (amount <= 0f)
		{
			Object.Destroy(this);
		}
	}
}
