using UnityEngine;

internal class CrystalTemperature : CrystalEffect
{
	private SpriteRenderer sprite;

	public override void Start()
	{
		crystal.SetColor(new Color32(byte.MaxValue, 132, 0, byte.MaxValue));
		crystal.light.volumeIntensityEnabled = true;
		crystal.light.volumeIntensity = 0.3f;
		crystal.light.falloffIntensity = 1f;
		Heater heater = crystal.gameObject.AddComponent<Heater>();
		heater.maxDistance = 10f * crystal.crystalSize;
		heater.desiredTemp = Random.Range(20f, 50f);
		heater.tempLerpStrength = Random.Range(0.025f, 0.06f);
		if (heater.desiredTemp < 37f)
		{
			crystal.SetColor(Color.cyan);
		}
		GameObject gameObject = new GameObject("heatwave", typeof(SpriteRenderer));
		sprite = gameObject.GetComponent<SpriteRenderer>();
		sprite.sprite = crystal.sprite.sprite;
		sprite.color = crystal.sprite.color.WithAlpha(0.2f);
		sprite.transform.SetParent(crystal.transform);
		sprite.transform.localPosition = Vector3.zero;
		sprite.transform.localRotation = Quaternion.identity;
		sprite.material = crystal.sprite.material;
		sprite.sortingOrder = crystal.sprite.sortingOrder - 1;
		crystal.AddEffectAudio("crystaltemperatureloop", 10f * crystal.crystalSize);
	}

	public override void Update()
	{
		sprite.color = crystal.sprite.color.WithAlpha(0.25f);
		sprite.transform.localScale = Vector3.one * (1f + Mathf.Abs(Mathf.Sin(Time.time * 1.5f) * 0.5f));
	}
}
