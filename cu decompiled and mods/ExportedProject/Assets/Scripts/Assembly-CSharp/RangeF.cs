using System;
using UnityEngine;

[Serializable]
public struct RangeF
{
	public float min;

	public float max;

	public float RandomFromRange()
	{
		return UnityEngine.Random.Range(min, max);
	}

	public RangeF(float minn, float maxx)
	{
		min = minn;
		max = maxx;
	}

	public static RangeF operator +(RangeF a, float b)
	{
		return new RangeF(a.min + b, a.max + b);
	}

	public static RangeF operator -(RangeF a, float b)
	{
		return new RangeF(a.min - b, a.max - b);
	}

	public static RangeF operator *(RangeF a, float b)
	{
		return new RangeF(a.min * b, a.max * b);
	}

	public static RangeF operator /(RangeF a, float b)
	{
		return new RangeF(a.min / b, a.max / b);
	}

	public static RangeF operator +(RangeF a, RangeF b)
	{
		return new RangeF(a.min / b.min, a.max / b.max);
	}

	public static RangeF operator -(RangeF a, RangeF b)
	{
		return new RangeF(a.min - b.min, a.max - b.max);
	}
}
