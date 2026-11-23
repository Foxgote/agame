using UnityEngine;
using UnityEngine.Rendering.Universal;

internal class CrystalGravity : CrystalEffect
{
	private float forceMult = 1f;

	private float ringTime;

	private SpriteRenderer ring;

	public override void Start()
	{
		forceMult = ((Random.value > 0.5f) ? 1f : (-1f));
		crystal.GetComponent<Light2D>().volumeIntensityEnabled = true;
		crystal.GetComponent<Light2D>().volumeIntensity = 0.1f;
		crystal.AddEffectAudio("crystalgravityloop", 17f * crystal.crystalSize);
	}

	public override void Update()
	{
		Collider2D[] array = Physics2D.OverlapCircleAll(crystal.transform.position, 16f * crystal.crystalSize);
		foreach (Collider2D collider2D in array)
		{
			if ((bool)collider2D.attachedRigidbody && collider2D.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic)
			{
				collider2D.attachedRigidbody.velocity += (Vector2)(collider2D.transform.position - crystal.transform.position).normalized * Time.deltaTime * forceMult * 45f;
			}
		}
		if ((bool)ring)
		{
			if (forceMult > 0f)
			{
				ring.transform.localScale = Vector3.Lerp(ring.transform.localScale, Vector3.one * 32f * crystal.crystalSize, Time.deltaTime * 5f);
			}
			else
			{
				ring.transform.localScale = Vector3.Lerp(ring.transform.localScale, Vector3.zero, Time.deltaTime * 5f);
			}
			if (ringTime < 0.5f)
			{
				ring.color = ring.color.WithAlpha(Mathf.Lerp(ring.color.a, 1f, Time.deltaTime * 4f));
			}
			else
			{
				ring.color = ring.color.WithAlpha(Mathf.Lerp(ring.color.a, 0f, Time.deltaTime * 6f));
			}
		}
		ringTime += Time.deltaTime;
		if (ringTime > 1f)
		{
			if (!ring)
			{
				GameObject gameObject = Utils.Create("Special/crystalring", crystal.transform.position, 0f);
				gameObject.transform.SetParent(crystal.transform, worldPositionStays: true);
				gameObject.transform.localScale = ((forceMult > 0f) ? Vector3.zero : (Vector3.one * 32f * crystal.crystalSize));
				ring = gameObject.GetComponent<SpriteRenderer>();
				ring.color = crystal.sprite.color.WithAlpha(0f);
			}
			else
			{
				Object.Destroy(ring);
			}
			ringTime = 0f;
		}
	}
}
