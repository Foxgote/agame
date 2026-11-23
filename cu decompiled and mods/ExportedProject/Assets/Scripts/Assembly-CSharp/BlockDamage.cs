using UnityEngine;

public class BlockDamage
{
	public float damage;

	public Vector2Int pos;

	private SpriteRenderer spr;

	public void UpdateSprite()
	{
		if (spr == null)
		{
			GameObject gameObject = new GameObject("blockbreak", typeof(SpriteRenderer));
			spr = gameObject.GetComponent<SpriteRenderer>();
			spr.sortingOrder = 4000;
			spr.material = WorldGeneration.world.defaultMat;
			spr.transform.position = WorldGeneration.world.BlockToWorldPos(pos);
		}
		float num = damage / WorldGeneration.world.GetBlockInfo(WorldGeneration.world.GetBlock(pos)).health;
		if (num < 0.2f)
		{
			spr.sprite = WorldGeneration.world.blockDamageSprites[0];
		}
		else if (num < 0.4f)
		{
			spr.sprite = WorldGeneration.world.blockDamageSprites[1];
		}
		else if (num < 0.6f)
		{
			spr.sprite = WorldGeneration.world.blockDamageSprites[2];
		}
		else if (num < 0.8f)
		{
			spr.sprite = WorldGeneration.world.blockDamageSprites[3];
		}
		else if (num < 1f)
		{
			spr.sprite = WorldGeneration.world.blockDamageSprites[4];
		}
		else
		{
			Object.Destroy(spr.gameObject);
		}
	}

	public void DestroySprite()
	{
		if ((bool)spr)
		{
			Object.Destroy(spr.gameObject);
		}
	}
}
