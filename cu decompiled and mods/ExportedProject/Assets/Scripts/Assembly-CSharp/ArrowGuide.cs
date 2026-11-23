using UnityEngine;

public class ArrowGuide : MonoBehaviour
{
	private Vector3 origPos;

	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		origPos = base.transform.position;
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		Vector3 vector = origPos;
		if (WorldGeneration.world.biomeDepth == 10)
		{
			vector.y *= -1f;
		}
		base.transform.position = vector + Vector3.up * Mathf.Sin(Time.time);
		spriteRenderer.flipY = WorldGeneration.world.biomeDepth == 10;
		spriteRenderer.enabled = WorldGeneration.world.biomeDepth != 9;
	}
}
