using UnityEngine;

public class HitFlash : MonoBehaviour
{
	private float time;

	public Color clr;

	private void Update()
	{
		time += Time.deltaTime;
		GetComponent<SpriteRenderer>().color = new Color(clr.r, clr.g, clr.b, 0.6f - time * 4f);
		if (time > 0.15f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
