using UnityEngine;

public class OilPipeScript : MonoBehaviour
{
	private float lastOilTime;

	private float coolTime;

	private void Start()
	{
		coolTime = Random.Range(0.6f, 2.5f);
	}

	private void Update()
	{
		if (Time.time - lastOilTime > coolTime)
		{
			lastOilTime = Time.time;
			Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(base.transform.position);
			FluidManager.main.fluid[vector2Int.x, vector2Int.y] = 3;
		}
	}
}
