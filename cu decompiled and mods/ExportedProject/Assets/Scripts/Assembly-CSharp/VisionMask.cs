using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionMask : MonoBehaviour
{
	private Mesh mesh;

	public float farDistance = 50f;

	private void Awake()
	{
		mesh = new Mesh
		{
			name = "VisionMask"
		};
		GetComponent<MeshFilter>().mesh = mesh;
	}

	public void ClearMask()
	{
		mesh.Clear();
	}

	public void UpdateMask(Vector2 playerPos, List<Vector2> hitPoints)
	{
		if (hitPoints != null && hitPoints.Count >= 3)
		{
			mesh.Clear();
			List<Vector3> list = new List<Vector3>();
			int count = hitPoints.Count;
			for (int i = 0; i < count; i++)
			{
				Vector2 normalized = (hitPoints[i] - playerPos).normalized;
				list.Add(hitPoints[i] + normalized * farDistance);
			}
			list.AddRange(((IEnumerable<Vector2>)hitPoints).Select((Func<Vector2, Vector3>)((Vector2 p) => p)));
			List<int> list2 = new List<int>();
			for (int num = 0; num < count; num++)
			{
				int num2 = (num + 1) % count;
				list2.Add(num);
				list2.Add(num2);
				list2.Add(num2 + count);
				list2.Add(num);
				list2.Add(num2 + count);
				list2.Add(num + count);
			}
			mesh.SetVertices(list);
			mesh.SetTriangles(list2, 0);
			mesh.RecalculateNormals();
		}
	}
}
