using UnityEngine;

public class Heater : MonoBehaviour
{
	public float maxDistance;

	public float desiredTemp;

	public float tempLerpStrength;

	private float lastTime;

	public bool cooker;

	private bool isHeater => desiredTemp >= 37f;

	private Body body => PlayerCamera.main.body;

	private void OnWillRenderObject()
	{
		if (Time.time - lastTime > 0.5f)
		{
			lastTime = Time.time;
			if (Vector2.Distance(base.transform.position, body.transform.position) < maxDistance && ((isHeater && body.temperature < desiredTemp) || (!isHeater && body.temperature > desiredTemp)))
			{
				body.temperature = Mathf.Lerp(body.temperature, desiredTemp, tempLerpStrength);
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (cooker && collision.gameObject.TryGetComponent<Item>(out var component) && component.Stats.HasTag("meat") && component.id != "steak")
		{
			(Object.Instantiate(Resources.Load("steak"), component.transform.position, component.transform.rotation) as GameObject).GetComponent<Item>().condition = component.condition * 0.3f;
			Object.Destroy(component.gameObject);
			Sound.Play("Scald", component.transform.position);
		}
	}
}
