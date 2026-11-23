using UnityEngine;

public class ScarfPhysics : MonoBehaviour
{
	[HideInInspector]
	public Vector2[] points;

	[HideInInspector]
	public Vector2[] velocities;

	public float maxDistance;

	public int pointAmount;

	public float maxVelocity;

	public LineRenderer pointRenderer;

	public float springStrength;

	public float speedDamp;

	private float timestep;

	public float updateRate = 120f;

	public bool groundCollide = true;

	private float refSize = 1f;

	private void Start()
	{
		points = new Vector2[pointAmount];
		velocities = new Vector2[pointAmount];
		pointRenderer.positionCount = pointAmount;
		for (int i = 0; i < points.Length; i++)
		{
			points[i] = base.transform.position;
		}
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		timestep += Time.deltaTime;
		while (timestep > 1f / updateRate)
		{
			timestep -= 1f / updateRate;
			for (int i = 0; i < points.Length; i++)
			{
				float num = 1f / updateRate;
				Vector2 vector = points[i];
				velocities[i] += Physics2D.gravity * num;
				if (velocities[i].magnitude > maxVelocity)
				{
					velocities[i] = Vector2.ClampMagnitude(velocities[i], maxVelocity);
				}
				Vector2 vector2 = ((i != 0) ? points[i - 1] : ((Vector2)base.transform.position));
				if (Vector2.Distance(points[i], vector2) > maxDistance * refSize)
				{
					velocities[i] += Vector2.ClampMagnitude(vector2 - points[i], 10f) * num * springStrength;
					Vector2 vector3 = vector2;
					if (i > 0)
					{
						points[i - 1] = points[i] + Vector2.Lerp(Vector2.ClampMagnitude(vector3 - points[i], maxDistance * refSize), vector3 - points[i], 0.7f);
						points[i] = vector3 + Vector2.Lerp(Vector2.ClampMagnitude(points[i] - vector3, maxDistance * refSize), points[i] - vector3, 0.3f);
					}
					else
					{
						points[i] = vector3 + Vector2.Lerp(Vector2.ClampMagnitude(points[i] - vector3, maxDistance * refSize), points[i] - vector3, 0f);
					}
				}
				velocities[i] /= 1f + speedDamp * num;
				points[i] += velocities[i] * num;
				if (groundCollide && i > 1)
				{
					RaycastHit2D raycastHit2D = Physics2D.Linecast(vector, points[i], LayerMask.GetMask("Ground"));
					if ((bool)raycastHit2D)
					{
						velocities[i] = Vector3.Reflect((points[i] - vector).normalized, raycastHit2D.normal) * 0.7f;
						points[i] = (Vector2)Vector3.ProjectOnPlane(points[i] - raycastHit2D.point, raycastHit2D.normal) + raycastHit2D.point + raycastHit2D.normal * 0.02f;
					}
				}
				if (Vector2.Distance(points[i], vector2) > 3f * refSize)
				{
					for (int j = 0; j < pointAmount; j++)
					{
						points[j] = base.transform.position;
					}
				}
				pointRenderer.SetPosition(i, points[i]);
			}
		}
	}
}
