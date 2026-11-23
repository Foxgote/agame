using UnityEngine;

public class BananaPlantSlip : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent<Body>(out var component) && Mathf.Abs(component.rb.velocity.x) > 5f)
		{
			component.Ragdoll();
			Limb[] limbs = component.limbs;
			foreach (Limb limb in limbs)
			{
				Vector2 velocity = limb.rb.velocity;
				velocity.x *= ((limb == component.limbs[11] || limb == component.limbs[14]) ? 1.9f : 1.4f);
				limb.rb.velocity = velocity;
			}
			Sound.Play("plantslip", base.transform.position);
			component.talker.Talk(Locale.GetCharacter("hitgroundhard"));
		}
	}
}
