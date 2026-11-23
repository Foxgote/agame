using System;
using UnityEngine;

public class BounceShroom : MonoBehaviour
{
	private float bounceTime;

	private void OnWillRenderObject()
	{
		float num = Mathf.Clamp01(1f - (Time.time - bounceTime) * 0.5f);
		Vector2 vector = new Vector2(1f + Mathf.Sin(Time.time * 25f) * num * 0.25f, 1f + Mathf.Sin(Time.time * 25f + MathF.PI / 2f) * num * 0.25f);
		base.transform.localScale = vector;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (Time.time - bounceTime > 0.25f)
		{
			Limb limb = Body.LimbFromObject(collision.gameObject, base.transform.position);
			if ((bool)limb)
			{
				limb.body.SetVelocity(new Vector2(limb.body.lastTimeStepVelocity.x, UnityEngine.Random.Range(50f, 70f) * (limb.body.standing ? 1f : 0.25f)));
				bounceTime = Time.time;
				Sound.Play("mushroombounce", base.transform.position, twoDimensional: true);
			}
			else if ((bool)collision.attachedRigidbody)
			{
				collision.attachedRigidbody.velocity = new Vector2(collision.attachedRigidbody.velocity.x, 67f);
				bounceTime = Time.time;
				Sound.Play("mushroombounce", base.transform.position);
			}
		}
	}
}
