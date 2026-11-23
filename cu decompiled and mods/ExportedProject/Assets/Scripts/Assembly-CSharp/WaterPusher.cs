using UnityEngine;

public class WaterPusher : MonoBehaviour
{
	public Vector2 direction;

	private static float desiredSpeed = 15f;

	private Body body => PlayerCamera.main.body;

	public void OnTriggerStay2D(Collider2D collision)
	{
		if (!collision.attachedRigidbody)
		{
			return;
		}
		collision.attachedRigidbody.velocity = Vector2.Lerp(collision.attachedRigidbody.velocity, direction * desiredSpeed, 0.04f);
		if (collision.gameObject == body.gameObject && !body.hasScubaGear)
		{
			float num = body.transform.position.y - body.col.size.y * 0.5f;
			float num2 = Mathf.Clamp01((base.transform.position.y - num) * 0.16f + 0.2f);
			body.liquidSlipTime = Mathf.MoveTowards(PlayerCamera.main.body.liquidSlipTime, 0.9f, Time.fixedDeltaTime * 0.66f * ((base.transform.position.y > body.transform.position.y) ? 1f : 0.33f));
			body.liquidRagdollBar -= Time.fixedDeltaTime * num2 * 0.5f;
			if (body.liquidRagdollBar <= 0f)
			{
				body.Ragdoll();
				body.shock = 30f;
			}
		}
	}
}
