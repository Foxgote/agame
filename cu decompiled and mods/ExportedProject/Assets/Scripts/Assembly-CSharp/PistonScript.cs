using UnityEngine;

public class PistonScript : MonoBehaviour
{
	private Rigidbody2D piston;

	private Collider2D overlap;

	private SpriteRenderer rend;

	private bool up;

	private const float maxUp = 4f;

	private const float velocity = 60f;

	private float delay;

	private float desiredDelay;

	private void Start()
	{
		piston = base.transform.GetChild(0).GetComponent<Rigidbody2D>();
		overlap = base.transform.GetChild(1).GetComponent<Collider2D>();
		rend = GetComponent<SpriteRenderer>();
		desiredDelay = Random.Range(1f, 2.5f);
	}

	private void Update()
	{
		delay -= Time.deltaTime;
		if (delay <= 0f && PlayerCamera.main.body.conscious)
		{
			delay = desiredDelay;
			up = !up;
			piston.collisionDetectionMode = ((up && Time.timeScale <= 5f) ? CollisionDetectionMode2D.Continuous : CollisionDetectionMode2D.None);
			if (up && rend.isVisible)
			{
				Sound.Play("piston", base.transform.position, twoDimensional: false, pitchShift: false);
			}
			else if (rend.isVisible)
			{
				Sound.Play("unpiston", base.transform.position, twoDimensional: false, pitchShift: false);
			}
			if (up && base.transform.up.y < 0.9f)
			{
				ContactFilter2D contactFilter = default;
				contactFilter.SetLayerMask(LayerMask.GetMask("Body"));
				Collider2D[] array = new Collider2D[1];
				if (overlap.OverlapCollider(contactFilter, array) > 0 && array[0].transform == PlayerCamera.main.body.transform)
				{
					PlayerCamera.main.body.Ragdoll();
					PlayerCamera.main.body.Scream();
				}
			}
		}
		if (Time.deltaTime > 0.2f)
		{
			piston.transform.localPosition = Vector2.up * 4f * (up ? 1f : 0f);
			piston.velocity = Vector2.zero;
		}
		else if (up && piston.transform.localPosition.y < 4f)
		{
			piston.velocity = base.transform.up * 60f;
		}
		else if (!up && piston.transform.localPosition.y > 0f)
		{
			piston.velocity = base.transform.up * -60f * 0.3f;
		}
		else
		{
			piston.velocity = Vector2.zero;
			piston.transform.localPosition = Vector2.up * 4f * (up ? 1f : 0f);
		}
	}
}
