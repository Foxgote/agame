using UnityEngine;

public class AutoPump : MonoBehaviour
{
	private Item item;

	private float coolDown = 0.3f;

	private float wearCheckTime;

	private bool worn;

	private Body body => PlayerCamera.main.body;

	private void Start()
	{
		item = GetComponent<Item>();
	}

	private void Update()
	{
		coolDown -= Time.deltaTime;
		wearCheckTime -= Time.deltaTime;
		if (wearCheckTime < 0f)
		{
			wearCheckTime = 1f;
			worn = body.HasWearable(item);
		}
		if (worn && item.battery.hasCharge)
		{
			item.battery.DrainCharge(Time.deltaTime / 1200f);
			if (body.bloodPressure < 85f && coolDown <= 0f)
			{
				coolDown = 0.3f;
				body.bloodPressure += 44f;
				item.battery.DrainCharge(0.002f);
				Sound.Play("autopump", Vector2.zero, twoDimensional: true, pitchShift: true, null, 0.6f);
			}
		}
	}
}
