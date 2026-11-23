using UnityEngine;

internal class CrystalDripping : CrystalEffect
{
	private float spawnTime;

	private byte liquid;

	public override void Start()
	{
		Transform transform = crystal.transform;
		transform.localScale *= new Vector2(0.7f, 1f);
		liquid = (byte)Random.Range(1, 3);
	}

	public override void Update()
	{
		spawnTime -= Time.deltaTime;
		if (spawnTime < 0f)
		{
			spawnTime = 1f;
			FluidManager.main.SetLiquid(crystal.transform.position, liquid);
		}
	}
}
