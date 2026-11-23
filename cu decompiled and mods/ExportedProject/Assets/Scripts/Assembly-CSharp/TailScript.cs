using UnityEngine;

public class TailScript : MonoBehaviour
{
	private Vector2 lastPos;

	private Body body;

	private void Start()
	{
		body = GetComponentInParent<Body>();
	}

	private void Update()
	{
		Vector2 vector = (Vector2)base.transform.position - lastPos;
		if (vector.magnitude > Time.deltaTime)
		{
			Vector2 normalized = (vector.normalized + Vector2.down * 0.1f).normalized;
			if (!body.isRight)
			{
				normalized.x *= -1f;
			}
			float b = Vector2.SignedAngle(Vector2.right, normalized);
			b = Mathf.LerpAngle(base.transform.localEulerAngles.z, b, Time.deltaTime * body.rb.velocity.magnitude * 0.14f);
			if (b > 180f)
			{
				b -= 360f;
			}
			base.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Clamp(b, -80f, 80f));
		}
		if (body.rb.velocity.magnitude < 1.4f)
		{
			base.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(base.transform.localEulerAngles.z, 24f, Time.deltaTime * 0.78f));
		}
		base.transform.localScale = new Vector3(Mathf.Min(1f / base.transform.parent.localScale.x, 1f), 1f, 1f);
		lastPos = base.transform.position;
	}
}
