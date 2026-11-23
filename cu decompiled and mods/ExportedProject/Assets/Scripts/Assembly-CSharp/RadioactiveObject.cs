using UnityEngine;

public class RadioactiveObject : MonoBehaviour
{
	public float radAtZero;

	private Body body => PlayerCamera.main.body;

	private void Update()
	{
		float num = Vector2.Distance(base.transform.position, body.transform.position) * 0.3f;
		float num2 = radAtZero * (1f / Mathf.Max(num * num, 1f));
		if (num2 > 0.1f)
		{
			body.radiationSickness += num2 * Time.deltaTime;
			PlayerCamera.main.SetIrradiateIntensity(num2 * 0.5f);
		}
	}
}
