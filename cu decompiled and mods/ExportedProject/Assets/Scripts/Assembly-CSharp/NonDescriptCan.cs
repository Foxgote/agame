using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Saveable]
[JsonObject(MemberSerialization.OptIn)]
public class NonDescriptCan : MonoBehaviour
{
	private Item item;

	[JsonProperty]
	public float waterValue = -1f;

	[JsonProperty]
	public float fatValue = -100f;

	[JsonProperty]
	public float foodValue = -100f;

	[JsonProperty]
	public float happinessValue = -100f;

	[JsonProperty]
	public float sicknessValue = -100f;

	[HideInInspector]
	[JsonProperty]
	public List<string> liquidIds;

	[HideInInspector]
	[JsonProperty]
	public List<float> liquidWeights;

	private void Start()
	{
		item = GetComponent<Item>();
		if (waterValue < 0f)
		{
			waterValue = Random.Range(0f, 20f);
		}
		if (fatValue < -90f)
		{
			fatValue = Random.Range(-1f, 3f);
		}
		if (happinessValue < -90f)
		{
			happinessValue = Random.Range(-3f, 3.5f);
		}
		if (sicknessValue < -90f)
		{
			sicknessValue = Random.Range(-2f, 2f);
		}
		if (foodValue < 0f)
		{
			foodValue = Random.Range(0f, 20f);
		}
		if (liquidIds != null && liquidIds.Count != 0)
		{
			return;
		}
		liquidIds = new List<string>();
		liquidWeights = new List<float>();
		int num = 1;
		if (Random.value < 0.66f)
		{
			num++;
		}
		if (Random.value < 0.5f)
		{
			num++;
		}
		if (Random.value < 0.33f)
		{
			num++;
		}
		for (int i = 0; i < num; i++)
		{
			string text = "";
			do
			{
				text = Liquids.Registry.PickRandom().Key;
			}
			while (Liquids.DangerList.Contains(text));
			liquidIds.Add(text);
			liquidWeights.Add(Random.Range(25f, 175f));
		}
	}

	public void Eat(Body body)
	{
		if (item.condition > 0f)
		{
			item.condition -= 0.201f;
			body.Eat(foodValue, fatValue, 0);
			body.Drink(waterValue);
			body.happiness += happinessValue;
			body.sicknessAmount += sicknessValue;
			for (int i = 0; i < liquidIds.Count; i++)
			{
				Liquids.Registry[liquidIds[i]].onDrink(liquidWeights[i], body);
			}
			Sound.Play("eatFlesh", body.transform.position);
		}
	}
}
