using System.Collections;
using UnityEngine;

public class LifepodPump : MonoBehaviour
{
	public Vector2Int pumpMin;

	public Vector2Int pumpMax;

	public Vector2Int pumpOut;

	private void Awake()
	{
		if (Random.value < 0.2f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		pumpMin = WorldGeneration.world.WorldToBlockPos(new Vector2(base.transform.position.x - 8.5f, base.transform.position.y - 0.5f));
		pumpMax = WorldGeneration.world.WorldToBlockPos(new Vector2(base.transform.position.x + 0.5f, base.transform.position.y - 0.5f));
		pumpOut = WorldGeneration.world.WorldToBlockPos(new Vector2(base.transform.position.x + 2.5f, base.transform.position.y - 2.5f));
		StartCoroutine(PumpLoop());
	}

	private void OnDestroy()
	{
		StopCoroutine(PumpLoop());
	}

	public IEnumerator PumpLoop()
	{
		int randomWait = Random.Range(0, 60);
		while (randomWait > 0)
		{
			randomWait--;
			yield return null;
		}
		int index = pumpMin.x;
		while (true)
		{
			byte liquid = FluidManager.main.GetLiquid(index, pumpMax.y);
			if (liquid > 0)
			{
				FluidManager.main.SetLiquid(index, pumpMax.y, 0);
				FluidManager.main.SetLiquid(pumpOut.x, pumpOut.y, liquid);
			}
			index++;
			if (index > pumpMax.x)
			{
				index = pumpMin.x;
			}
			yield return new WaitForSeconds(0.2f);
		}
	}
}
