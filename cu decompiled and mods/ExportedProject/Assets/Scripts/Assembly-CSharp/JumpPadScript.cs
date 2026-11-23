using UnityEngine;
using UnityEngine.Rendering.Universal;

public class JumpPadScript : MonoBehaviour
{
	private Light2D light;

	private float cooldown;

	private SpriteRenderer child;

	public bool disabled;

	private void Start()
	{
		light = GetComponent<Light2D>();
		child = base.transform.GetChild(0).GetComponent<SpriteRenderer>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (disabled || !(cooldown <= 0f) || !collision.rigidbody || collision.rigidbody.isKinematic)
		{
			return;
		}
		Limb component;
		if ((bool)collision.gameObject.GetComponent<Body>() && cooldown <= 0f)
		{
			collision.gameObject.GetComponent<Body>().Ragdoll();
		}
		else if (collision.gameObject.TryGetComponent<Limb>(out component))
		{
			light.intensity = 1f;
			Limb[] limbs = component.body.limbs;
			foreach (Limb limb in limbs)
			{
				limb.rb.velocity = new Vector2(limb.rb.velocity.x, 65f);
			}
			component.body.Scream();
			Sound.Play("jumppad", base.transform.position);
			cooldown = 15f;
			PlayerCamera.main.shaker.Shake(70f);
		}
		else
		{
			light.intensity = 1f;
			collision.rigidbody.AddForce(Vector2.up * 350f, ForceMode2D.Impulse);
			Sound.Play("jumppad", base.transform.position);
			cooldown = 15f;
		}
	}

	private void Update()
	{
		light.intensity = Mathf.MoveTowards(light.intensity, 0f, Time.deltaTime * 3f);
		child.enabled = cooldown <= 0f || (int)(Time.time * 5f) % 2 == 0;
		cooldown -= Time.deltaTime;
	}
}
