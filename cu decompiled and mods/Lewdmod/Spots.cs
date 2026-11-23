using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000C6 RID: 198
	internal static class Spots
	{
		// Token: 0x06000468 RID: 1128 RVA: 0x00048064 File Offset: 0x00046264
		public static Vector2? RandomFloor(float minDistance, Vector2 from)
		{
			WorldGeneration world = WorldGeneration.world;
			for (int i = 0; i < 300; i++)
			{
				int num = Random.Range(8, (int)(world.width - 8U));
				int num2 = Random.Range(8, (int)(world.height - 8U));
				if (world.GetBlock(new Vector2Int(num, num2)) == 0 && world.GetBlock(new Vector2Int(num, num2 - 1)) != 0 && world.GetBlock(new Vector2Int(num, num2 + 1)) == 0 && (!FluidManager.main || FluidManager.main.GetLiquid(num, num2) == 0))
				{
					Vector2 vector = world.BlockToWorldPos(new Vector2Int(num, num2));
					if (Vector2.Distance(vector, from) >= minDistance)
					{
						return new Vector2?(vector);
					}
				}
			}
			return null;
		}
	}
}
