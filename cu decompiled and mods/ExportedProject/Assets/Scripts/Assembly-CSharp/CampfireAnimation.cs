using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CampfireAnimation : MonoBehaviour
{
	private Sprite[] fireSprites;

	private float index;

	private SpriteRenderer rend;

	private SpriteRenderer parentRend;

	private Light2D light;

	public Vector3 relativePos;

	public float upwardsParentOffset;

	private void Start()
	{
		fireSprites = Resources.LoadAll<Sprite>("Sprites/campfireLoop");
		rend = GetComponent<SpriteRenderer>();
		parentRend = base.transform.parent.GetComponent<SpriteRenderer>();
		light = GetComponent<Light2D>();
	}

	private void Update()
	{
		rend.sortingOrder = parentRend.sortingOrder - 1;
		base.transform.eulerAngles = Vector3.zero;
		base.transform.position = base.transform.parent.position + base.transform.parent.up * upwardsParentOffset + relativePos;
		light.intensity = 1f + Mathf.PerlinNoise1D(Time.time * 5f);
		rend.sprite = fireSprites[(int)index];
		index += Time.deltaTime * 28f;
		if ((int)index >= fireSprites.Length)
		{
			index = 0f;
		}
	}
}
