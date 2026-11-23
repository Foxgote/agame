using UnityEngine;

public class GeigeFruitScript : MonoBehaviour
{
	private float lastTime;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.relativeVelocity.magnitude > 15f && Time.time - lastTime > 1f)
		{
			lastTime = Time.time;
			Sound.Play("fireworkpop", base.transform.position, twoDimensional: true);
		}
	}
}
