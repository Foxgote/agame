using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkScript : MonoBehaviour
{
	private bool hasRandomized;

	public Vector2Int pos;

	public List<ISimEntity> containedEntities;

	public TilemapRenderer renderer;

	private void Awake()
	{
		containedEntities = new List<ISimEntity>();
		renderer = GetComponent<TilemapRenderer>();
	}

	private void Update()
	{
		if (!renderer.enabled)
		{
			return;
		}
		foreach (ISimEntity containedEntity in containedEntities)
		{
			containedEntity?.ChunkUpdate();
		}
	}

	private void OnWillRenderObject()
	{
		if (!hasRandomized && !WorldGeneration.world.generatingWorld)
		{
			hasRandomized = true;
			StartCoroutine(RandomizeTiles());
		}
	}

	public void AddEntity(ISimEntity entity)
	{
		containedEntities.Add(entity);
	}

	public void RemoveEntity(ISimEntity entity)
	{
		containedEntities.Remove(entity);
	}

	private IEnumerator RandomizeTiles()
	{
		ulong tile = (ulong)UnityEngine.Random.Range(0, int.MaxValue);
		Tilemap map = GetComponent<Tilemap>();
		for (int i = 0; i < WorldGeneration.CHUNKSIZE; i++)
		{
			for (int j = 0; j < WorldGeneration.CHUNKSIZE; j++)
			{
				if (!WorldGeneration.world.GetBlockInfo(WorldGeneration.world.GetBlock(new Vector2Int(i + pos.x * WorldGeneration.CHUNKSIZE, j + pos.y * WorldGeneration.CHUNKSIZE))).noVariation)
				{
					tile = WorldGeneration.lehmer64(tile);
					long num = Math.Abs((long)tile % 2L) * 2 - 1;
					tile = WorldGeneration.lehmer64(tile);
					long num2 = Math.Abs((long)tile % 2L) * 2 - 1;
					map.SetTransformMatrix(new Vector3Int(i - WorldGeneration.world.HALFCHUNKSIZE, j - WorldGeneration.world.HALFCHUNKSIZE), Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, tile % 5 * 90), new Vector3(num, num2, 1f)));
				}
			}
			yield return null;
		}
		yield return null;
	}
}
