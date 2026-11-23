using UnityEngine;

public class BoughtItem : MonoBehaviour
{
	public float time = 300f;

	private void Update()
	{
		time -= Time.deltaTime;
		if (time < 0f)
		{
			Object.Destroy(this);
		}
	}
}
