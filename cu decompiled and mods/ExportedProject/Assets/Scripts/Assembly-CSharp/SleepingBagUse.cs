using UnityEngine;

public class SleepingBagUse : MonoBehaviour
{
	private Body body => PlayerCamera.main.body;

	private void Update()
	{
		if (body.usingSleepingBag)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.unscaledDeltaTime * 5f);
			return;
		}
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(0.1f, 1f, 1f), Time.unscaledDeltaTime * 5f);
		if (base.transform.localScale.x < 0.2f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
