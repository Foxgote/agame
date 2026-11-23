using UnityEngine;

public class TechballScript : MonoBehaviour
{
	private Rigidbody2D rb;

	private float timeBeforeJump;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.constraints = RigidbodyConstraints2D.FreezeAll;
	}

	private void Update()
	{
		timeBeforeJump -= Time.deltaTime;
		if (Vector2.Distance(base.transform.position, PlayerCamera.main.body.transform.position) < 12f)
		{
			rb.constraints = RigidbodyConstraints2D.None;
		}
		if (rb.constraints != RigidbodyConstraints2D.FreezeAll && timeBeforeJump < 0f)
		{
			Vector2 insideUnitCircle = Random.insideUnitCircle;
			rb.velocity += insideUnitCircle * 20f;
			timeBeforeJump = Random.Range(0.25f, 1.75f);
			Sound.Play("techbounce", base.transform.position, twoDimensional: false, pitchShift: true, base.transform, insideUnitCircle.magnitude);
		}
	}
}
