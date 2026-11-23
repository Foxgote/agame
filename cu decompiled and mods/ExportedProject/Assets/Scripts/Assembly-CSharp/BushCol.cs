using UnityEngine;

public class BushCol : MonoBehaviour
{
	private float time;

	private void Update()
	{
		time -= Time.deltaTime;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (time < 0f && (bool)collision.attachedRigidbody)
		{
			time = 0.5f;
			if (collision.attachedRigidbody.velocity.magnitude > 30f)
			{
				Sound.Play("bushbig", base.transform.position);
			}
			else
			{
				Sound.Play($"bush{Random.Range(1, 6)}", base.transform.position);
			}
		}
	}
}
