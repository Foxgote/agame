using UnityEngine;

public class GroundBlood : MonoBehaviour
{
	private void Start()
	{
		Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(base.transform.position) / WorldGeneration.CHUNKSIZE;
		WorldGeneration.world.ChunkUpdated[vector2Int.x, vector2Int.y].AddListener(CheckGround);
	}

	public void CheckGround()
	{
		if (WorldGeneration.world.GetBlock(base.transform.position) == 0)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(base.transform.position) / WorldGeneration.CHUNKSIZE;
		WorldGeneration.world.ChunkUpdated[vector2Int.x, vector2Int.y].RemoveListener(CheckGround);
	}
}
