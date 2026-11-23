using UnityEngine;

internal class CrystalMetamorphic : CrystalEffect
{
	private bool activated;

	public override void Start()
	{
		crystal.build.health *= 10f;
		crystal.build.metallic = true;
		crystal.sprite.material = Resources.Load<Material>("Special/CrystalShimmer");
	}

	public override void Touched(GameObject touched)
	{
		if (!activated && Utils.GetBody(touched, out var _))
		{
			PlayerCamera.main.StartCoroutine("FlashBrief");
			crystal.build.health = 0f;
			int num = Random.Range(1, 4);
			for (int i = 0; i < num; i++)
			{
				Utils.Create(Item.GetRandomItem(), (Vector2)crystal.transform.position + Random.insideUnitCircle * 2f * crystal.crystalSize, Random.value * 360f).AddComponent<FreshItemDrop>();
			}
			Sound.Play("crystalenemylaugh", crystal.transform.position, twoDimensional: true, pitchShift: false);
			activated = true;
		}
	}
}
