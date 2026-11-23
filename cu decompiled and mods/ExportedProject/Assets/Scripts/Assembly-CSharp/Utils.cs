using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
	public static GameObject Create(string id, Vector2 pos, float rot)
	{
		return UnityEngine.Object.Instantiate(Resources.Load(id), pos, Quaternion.Euler(0f, 0f, rot)) as GameObject;
	}

	public static GameObject Create(string id, Transform trans)
	{
		return UnityEngine.Object.Instantiate(Resources.Load(id), trans) as GameObject;
	}

	public static bool GetBody(GameObject g, out Body body)
	{
		if (g.TryGetComponent<Body>(out body))
		{
			return true;
		}
		if (g.TryGetComponent<Limb>(out var component))
		{
			body = component.body;
			return true;
		}
		return false;
	}

	public static T PickRandom<T>(this T[] source)
	{
		return source[UnityEngine.Random.Range(0, source.Length)];
	}

	public static T PickRandom<T>(this List<T> source)
	{
		return source[UnityEngine.Random.Range(0, source.Count)];
	}

	public static void Shuffle<T>(this List<T> list, int seed)
	{
		System.Random random = new System.Random(seed);
		for (int num = list.Count - 1; num > 0; num--)
		{
			int num2 = random.Next(0, num + 1);
			int index = num;
			int index2 = num2;
			T val = list[num2];
			T val2 = list[num];
			T val3 = (list[index] = val);
			val3 = (list[index2] = val2);
		}
	}

	public static KeyValuePair<TKey, TValue> PickRandom<TKey, TValue>(this Dictionary<TKey, TValue> source)
	{
		return source.ElementAt(UnityEngine.Random.Range(0, source.Count));
	}

	public static Color WithAlpha(this Color color, float alpha)
	{
		color.a = Mathf.Clamp01(alpha);
		return color;
	}

	public static float Remap(this float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	public static Vector2 ImageSizeDelta(Texture tex, float pixelSize, float maxSize)
	{
		Vector2 result = (Vector3)new Vector2((float)tex.width * pixelSize, (float)tex.height * pixelSize);
		if (result.y > maxSize)
		{
			float num = maxSize / result.y;
			result *= num;
		}
		if (result.x > maxSize)
		{
			float num2 = maxSize / result.x;
			result *= num2;
		}
		return result;
	}

	public static float InQuad(float t)
	{
		return t * t;
	}

	public static float OutQuad(float t)
	{
		return 1f - InQuad(1f - t);
	}

	public static float InOutQuad(float t)
	{
		if ((double)t < 0.5)
		{
			return InQuad(t * 2f) / 2f;
		}
		return 1f - InQuad((1f - t) * 2f) / 2f;
	}

	public static float InCubic(float t)
	{
		return t * t * t;
	}

	public static float OutCubic(float t)
	{
		return 1f - InCubic(1f - t);
	}

	public static float InOutCubic(float t)
	{
		if ((double)t < 0.5)
		{
			return InCubic(t * 2f) / 2f;
		}
		return 1f - InCubic((1f - t) * 2f) / 2f;
	}

	public static float InQuart(float t)
	{
		return t * t * t * t;
	}

	public static float OutQuart(float t)
	{
		return 1f - InQuart(1f - t);
	}

	public static float InOutQuart(float t)
	{
		if ((double)t < 0.5)
		{
			return InQuart(t * 2f) / 2f;
		}
		return 1f - InQuart((1f - t) * 2f) / 2f;
	}

	public static float InQuint(float t)
	{
		return t * t * t * t * t;
	}

	public static float OutQuint(float t)
	{
		return 1f - InQuint(1f - t);
	}

	public static float InOutQuint(float t)
	{
		if ((double)t < 0.5)
		{
			return InQuint(t * 2f) / 2f;
		}
		return 1f - InQuint((1f - t) * 2f) / 2f;
	}

	public static float InSine(float t)
	{
		return 1f - (float)Math.Cos((double)t * Math.PI / 2.0);
	}

	public static float OutSine(float t)
	{
		return (float)Math.Sin((double)t * Math.PI / 2.0);
	}

	public static float InOutSine(float t)
	{
		return (float)(Math.Cos((double)t * Math.PI) - 1.0) / -2f;
	}

	public static float InExpo(float t)
	{
		return (float)Math.Pow(2.0, 10f * (t - 1f));
	}

	public static float OutExpo(float t)
	{
		return 1f - InExpo(1f - t);
	}

	public static float InOutExpo(float t)
	{
		if ((double)t < 0.5)
		{
			return InExpo(t * 2f) / 2f;
		}
		return 1f - InExpo((1f - t) * 2f) / 2f;
	}

	public static float InCirc(float t)
	{
		return 0f - ((float)Math.Sqrt(1f - t * t) - 1f);
	}

	public static float OutCirc(float t)
	{
		return 1f - InCirc(1f - t);
	}

	public static float InOutCirc(float t)
	{
		if ((double)t < 0.5)
		{
			return InCirc(t * 2f) / 2f;
		}
		return 1f - InCirc((1f - t) * 2f) / 2f;
	}

	public static float InElastic(float t)
	{
		return 1f - OutElastic(1f - t);
	}

	public static float OutElastic(float t)
	{
		float num = 0.3f;
		return (float)Math.Pow(2.0, -10f * t) * (float)Math.Sin((double)(t - num / 4f) * (Math.PI * 2.0) / (double)num) + 1f;
	}

	public static float InOutElastic(float t)
	{
		if ((double)t < 0.5)
		{
			return InElastic(t * 2f) / 2f;
		}
		return 1f - InElastic((1f - t) * 2f) / 2f;
	}

	public static float InBack(float t)
	{
		float num = 1.70158f;
		return t * t * ((num + 1f) * t - num);
	}

	public static float OutBack(float t)
	{
		return 1f - InBack(1f - t);
	}

	public static float InOutBack(float t)
	{
		if ((double)t < 0.5)
		{
			return InBack(t * 2f) / 2f;
		}
		return 1f - InBack((1f - t) * 2f) / 2f;
	}

	public static float InBounce(float t)
	{
		return 1f - OutBounce(1f - t);
	}

	public static float OutBounce(float t)
	{
		float num = 2.75f;
		float num2 = 7.5625f;
		if (t < 1f / num)
		{
			return num2 * t * t;
		}
		if (t < 2f / num)
		{
			t -= 1.5f / num;
			return num2 * t * t + 0.75f;
		}
		if ((double)t < 2.5 / (double)num)
		{
			t -= 2.25f / num;
			return num2 * t * t + 0.9375f;
		}
		t -= 2.625f / num;
		return num2 * t * t + 63f / 64f;
	}

	public static float InOutBounce(float t)
	{
		if ((double)t < 0.5)
		{
			return InBounce(t * 2f) / 2f;
		}
		return 1f - InBounce((1f - t) * 2f) / 2f;
	}
}
