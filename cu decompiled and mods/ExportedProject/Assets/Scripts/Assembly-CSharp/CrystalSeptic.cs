using UnityEngine;

internal class CrystalSeptic : CrystalEffect
{
	public override void Start()
	{
		crystal.SetColor(new Color32(128, 0, byte.MaxValue, 128));
		GameObject gameObject = Utils.Create("Special/crystalring", crystal.transform);
		gameObject.GetComponent<SpriteRenderer>().color = crystal.sprite.color.WithAlpha(0.1f);
		gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/crystalringmessy");
		gameObject.transform.localScale = Vector3.one * 24f * crystal.crystalSize;
		crystal.AddEffectAudio("crystalsepsisloop", 12f * crystal.crystalSize);
	}

	public override void Update()
	{
		if (Vector2.Distance(PlayerCamera.main.body.transform.position, crystal.transform.position) < 12f * crystal.crystalSize)
		{
			PlayerCamera.main.body.septicShock += Time.deltaTime * 0.5f;
		}
	}
}
