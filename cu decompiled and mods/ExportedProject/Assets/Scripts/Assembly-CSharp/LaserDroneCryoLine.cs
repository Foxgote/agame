using UnityEngine;

public class LaserDroneCryoLine : MonoBehaviour
{
	private LineRenderer line;

	private float lifetime;

	private Vector2 start;

	private Vector2 end;

	private void Start()
	{
		line = GetComponent<LineRenderer>();
		start = line.GetPosition(0);
		end = line.GetPosition(1);
	}

	private void Update()
	{
		lifetime += Time.deltaTime * 8f;
		if (lifetime >= 1f)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			line.SetPosition(0, Vector2.Lerp(start, end, lifetime));
		}
	}
}
