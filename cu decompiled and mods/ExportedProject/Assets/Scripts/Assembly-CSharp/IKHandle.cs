using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IKHandle : MonoBehaviour
{
	public Vector2 targetPos;

	public static int ITERATIONS = 1;

	public List<IKSegment> segments = new List<IKSegment>();

	public Vector2 rootPos;

	public bool doPos;

	private static Camera mc;

	public bool alwaysUpdate;

	public LineRenderer lineRenderer { get; private set; }

	public float maxLength { get; private set; }

	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.positionCount = segments.Count + 1;
		targetPos = base.transform.position;
		for (int i = 0; i < lineRenderer.positionCount; i++)
		{
			lineRenderer.SetPosition(i, base.transform.position);
		}
		RecalculateLength();
		mc = Camera.main;
	}

	public void RecalculateLength()
	{
		maxLength = segments.Sum((IKSegment x) => x.length);
	}

	private void Update()
	{
		if (alwaysUpdate || (!(WorldGeneration.world == null) && WorldGeneration.world.GetClosestChunkRenderer(WorldGeneration.world.WorldToBlockPos(base.transform.position)).enabled))
		{
			if (doPos)
			{
				targetPos = Vector2.Lerp(targetPos, Camera.main.ScreenToWorldPoint(Input.mousePosition), Time.unscaledDeltaTime * 4f);
			}
			UpdateIK();
			lineRenderer.SetPosition(0, rootPos);
			for (int i = 0; i < segments.Count; i++)
			{
				lineRenderer.SetPosition(i + 1, new Vector3(segments[i].pos.x, segments[i].pos.y, (float)(-i) * 0.02f));
			}
		}
	}

	public void UpdateIK()
	{
		for (int i = 0; i < ITERATIONS; i++)
		{
			bool flag = true;
			for (int num = segments.Count - 1; num > 0; num--)
			{
				if (flag)
				{
					segments[num].pos = targetPos;
				}
				if (num > 0)
				{
					Vector2 vector = (segments[num - 1].pos - segments[num].pos).normalized * segments[num].length;
					segments[num - 1].pos = vector + segments[num].pos;
				}
				flag = false;
			}
			for (int j = 0; j < segments.Count - 1; j++)
			{
				if (j > 0)
				{
					Vector2 vector2 = (segments[j].pos - segments[j - 1].pos).normalized * segments[j].length;
					segments[j].pos = vector2 + segments[j - 1].pos;
				}
				else
				{
					Vector2 vector3 = (segments[j].pos - rootPos).normalized * segments[j].length;
					segments[j].pos = vector3 + rootPos;
				}
			}
		}
	}
}
