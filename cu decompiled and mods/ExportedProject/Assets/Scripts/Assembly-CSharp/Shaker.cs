using UnityEngine;

public class Shaker
{
	public Vector2 velocity;

	public Vector2 pos;

	private float timesToUpdate;

	private const float frame = 1f / 60f;

	public float multiplier => PlayerCamera.screenShakeMult;

	public void Update()
	{
		timesToUpdate += Time.unscaledDeltaTime;
		timesToUpdate = Mathf.Min(timesToUpdate, 1f);
		while (timesToUpdate > 1f / 60f)
		{
			timesToUpdate -= 1f / 60f;
			pos += 0.025000002f * velocity;
			velocity /= 1.0333333f;
			velocity = Vector2.Lerp(velocity, Vector2.zero - pos * 50f, 0.116666675f);
		}
	}

	public void Shake(float amount = 200f)
	{
		amount *= multiplier;
		velocity += Random.insideUnitCircle.normalized * amount;
		if (amount > 50f)
		{
			pos += Random.insideUnitCircle;
		}
	}

	public void Velocity(Vector2 vel)
	{
		velocity += vel * multiplier;
	}
}
