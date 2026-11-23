using System.Collections.Generic;
using UnityEngine;

public class Climbable : MonoBehaviour
{
	public List<Vector2> points;

	[HideInInspector]
	private List<float> segmentLengths;

	[HideInInspector]
	public float totalLength;

	public float downwardsVelocity;

	[HideInInspector]
	public Bounds bounds;

	public static List<Climbable> allClimbables;

	[HideInInspector]
	public float lastStartedClimbing;

	public AudioClip[] climbSounds;

	[Tooltip("If true, automatically sets the point positions to the top and bottom of a tiled sprite.")]
	public bool autoSet;

	[Tooltip("If true, points will be automatically transformed from local space into worldspace")]
	public bool localPoints;

	private void Start()
	{
		if (autoSet)
		{
			points = new List<Vector2>();
			SpriteRenderer component = GetComponent<SpriteRenderer>();
			points.Add(base.transform.position + 0.5f * component.size.y * Vector3.down);
			points.Add(base.transform.position + 0.5f * component.size.y * Vector3.up);
		}
		if (localPoints)
		{
			for (int i = 0; i < points.Count; i++)
			{
				points[i] = base.transform.TransformPoint(points[i]);
			}
		}
		CalculateTotalLength();
		if (allClimbables == null)
		{
			allClimbables = new List<Climbable>();
		}
		allClimbables.Add(this);
	}

	public static Climbable GetClosestClimbable(Vector2 playerPos)
	{
		if (allClimbables == null)
		{
			return null;
		}
		float num = float.MaxValue;
		Climbable result = null;
		foreach (Climbable allClimbable in allClimbables)
		{
			float sqrMagnitude = (playerPos - (Vector2)allClimbable.bounds.ClosestPoint(playerPos)).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = allClimbable;
			}
		}
		return result;
	}

	public void OnDestroy()
	{
		allClimbables.Remove(this);
	}

	public void CalculateTotalLength()
	{
		segmentLengths = new List<float>();
		totalLength = 0f;
		if (points.Count < 2)
		{
			return;
		}
		for (int i = 0; i < points.Count - 1; i++)
		{
			float num = Vector2.Distance(points[i], points[i + 1]);
			segmentLengths.Add(num);
			totalLength += num;
		}
		bounds = new Bounds(points[0], Vector3.zero);
		foreach (Vector2 point in points)
		{
			bounds.Encapsulate(point);
		}
	}

	public Vector2 GetPositionAtDistance(float dist)
	{
		if (points.Count < 2)
		{
			return Vector2.zero;
		}
		float num = 0f;
		for (int i = 0; i < segmentLengths.Count; i++)
		{
			if (dist <= num + segmentLengths[i])
			{
				float t = (dist - num) / segmentLengths[i];
				return Vector2.Lerp(points[i], points[i + 1], t);
			}
			num += segmentLengths[i];
		}
		return points[points.Count - 1];
	}

	public ClimbableGrabInfo GetGrabInfo(Vector2 worldPos)
	{
		ClimbableGrabInfo result = new ClimbableGrabInfo
		{
			distanceToPlayer = float.MaxValue
		};
		float num = 0f;
		for (int i = 0; i < points.Count - 1; i++)
		{
			Vector2 vector = points[i];
			Vector2 vector2 = points[i + 1] - vector;
			float value = Vector2.Dot(worldPos - vector, vector2) / vector2.sqrMagnitude;
			value = Mathf.Clamp01(value);
			Vector2 vector3 = vector + value * vector2;
			float magnitude = (worldPos - vector3).magnitude;
			if (magnitude < result.distanceToPlayer)
			{
				result.distanceToPlayer = magnitude;
				result.position = vector3;
				result.pathDistance = num + value * segmentLengths[i];
			}
			num += segmentLengths[i];
		}
		return result;
	}
}
