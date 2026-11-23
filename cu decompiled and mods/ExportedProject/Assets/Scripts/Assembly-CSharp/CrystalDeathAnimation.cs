using UnityEngine;

public class CrystalDeathAnimation : MonoBehaviour
{
	private float time;

	private void Update()
	{
		time += Time.deltaTime;
		if (time >= 1f)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.transform.localScale = Vector3.one * (1f - Utils.InBack(time)) * Mathf.Clamp01(time * 4f);
		}
	}
}
