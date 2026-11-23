using UnityEngine;

public class RecipeBoxScript : MonoBehaviour
{
	public float timeOffset;

	private void Update()
	{
		if (Time.time >= timeOffset)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.deltaTime * 6f);
		}
	}
}
