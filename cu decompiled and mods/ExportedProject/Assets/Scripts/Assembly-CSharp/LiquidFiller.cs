using UnityEngine;

public class LiquidFiller : MonoBehaviour
{
	public byte liquidId;

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
		Gizmos.DrawCube(base.transform.position, base.transform.localScale);
	}

	private void Start()
	{
		Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(base.transform.position - base.transform.localScale * 0.5f + new Vector3(0.5f, 0.5f));
		Vector2Int vector2Int2 = WorldGeneration.world.WorldToBlockPos(base.transform.position + base.transform.localScale * 0.5f - new Vector3(0.5f, 0.5f));
		for (int i = vector2Int.x; i <= vector2Int2.x; i++)
		{
			for (int j = vector2Int.y; j <= vector2Int2.y; j++)
			{
				FluidManager.main.SetLiquid(i, j, liquidId);
			}
		}
		Object.Destroy(base.gameObject);
	}
}
