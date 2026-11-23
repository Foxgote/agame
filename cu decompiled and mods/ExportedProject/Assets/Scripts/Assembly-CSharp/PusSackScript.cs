using UnityEngine;

public class PusSackScript : MonoBehaviour
{
	public Sprite breakSprite;

	private bool broke;

	private BuildingEntity build;

	private void Start()
	{
		build = GetComponent<BuildingEntity>();
	}

	private void OnWillRenderObject()
	{
		if (!(build.health < 140f) || broke)
		{
			return;
		}
		broke = true;
		GetComponent<SpriteRenderer>().sprite = breakSprite;
		Sound.Play("gore", base.transform.position);
		Utils.Create("BloodExplosion", base.transform.position, 0f);
		FluidManager.main.SetLiquid(base.transform.position, 7);
		if (!(Vector2.Distance(PlayerCamera.main.body.transform.position, base.transform.position) < 8f))
		{
			return;
		}
		PlayerCamera.main.body.dirtyness += 10f;
		PlayerCamera.main.body.eyeScareTime = 1.5f;
		PlayerCamera.main.body.eyeScareTime = 1.5f;
		PlayerCamera.main.body.rb.velocity += Vector2.up * 28f;
		Limb[] limbs = PlayerCamera.main.body.limbs;
		foreach (Limb limb in limbs)
		{
			if (!limb.dismembered || !(Random.value > 0.1f))
			{
				limb.furBloodAmount += Random.Range(0.1f, 1f);
			}
		}
	}
}
