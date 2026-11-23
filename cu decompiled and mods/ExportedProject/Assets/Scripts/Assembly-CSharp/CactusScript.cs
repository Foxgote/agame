using UnityEngine;

public class CactusScript : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.TryGetComponent<Body>(out var component))
		{
			component.shock = 30f;
			component.SetVelocity(Vector2.Lerp(component.rb.velocity, (component.transform.position - base.transform.position).normalized * 15f, 0.6f));
			Limb limb = component.limbs.PickRandom();
			limb.pain += 30f;
			limb.skinHealth -= 10f;
			limb.bleedAmount += 2f;
			component.DoGoreSound();
			GetComponent<BuildingEntity>().health -= 30f;
		}
	}
}
