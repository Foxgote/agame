using System;
using UnityEngine;

[Serializable]
public struct RangeI
{
	public int min;

	public int max;

	public int RandomFromRange()
	{
		return UnityEngine.Random.Range(min, max);
	}

	public RangeI(int minn, int maxx)
	{
		min = minn;
		max = maxx;
	}
}
