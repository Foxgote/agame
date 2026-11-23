using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
[Saveable]
public class Antidepressants : MonoBehaviour
{
	private Body body;

	[JsonProperty]
	public float amount;

	[JsonProperty]
	public float currentAmount;

	private void Awake()
	{
		body = GetComponent<Body>();
	}

	public void TakeDose(float amt = 100f)
	{
		amount += amt;
		if (Random.Range(0f, 1f) < 0.3f)
		{
			body.sicknessAmount += amt * 0.3f;
		}
	}

	public void Update()
	{
		currentAmount = Mathf.MoveTowards(currentAmount, amount, Time.deltaTime * 5f);
		if (amount > 0f)
		{
			amount -= Time.deltaTime * 0.185f;
			body.antidepressantHappiness = (0f - body.happiness) * 0.6f * Mathf.Clamp01(currentAmount * 0.0166f);
			body.happiness += currentAmount * 3E-05f * Time.deltaTime;
			if (currentAmount >= 250f)
			{
				body.bloodPressure -= Time.deltaTime * 3f;
				body.overdoseIndex = 2;
			}
		}
		else
		{
			body.antidepressantHappiness = 0f;
			Object.Destroy(this);
		}
	}
}
