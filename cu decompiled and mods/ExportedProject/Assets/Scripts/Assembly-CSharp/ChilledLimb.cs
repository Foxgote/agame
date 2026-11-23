using Newtonsoft.Json;
using UnityEngine;

[Saveable]
[JsonObject(MemberSerialization.OptIn)]
public class ChilledLimb : MonoBehaviour
{
	[JsonProperty]
	public float timeLeft;

	[JsonProperty]
	public float maxTime;

	private Limb lim;

	private void Awake()
	{
		lim = GetComponent<Limb>();
	}

	private void Update()
	{
		timeLeft -= Time.deltaTime;
		lim.muscleHealth += lim.MuscleHealRate * Time.deltaTime * 2f;
		lim.pain -= Time.deltaTime * 1.5f;
		if (timeLeft <= 0f)
		{
			Object.Destroy(this);
		}
	}
}
