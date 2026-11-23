using UnityEngine;

public class GuardSpawner : MonoBehaviour
{
	public SpriteRenderer door;

	[HideInInspector]
	public float openProgress;

	private Vector2 openPos = new Vector2(-3.5f, 0f);

	[Min(6f)]
	public float maxDistanceToSpawn;

	[Min(4f)]
	public float minDistanceToSpawn = 4f;

	[Min(0f)]
	public int guardsToSpawn;

	[Min(3f)]
	public float minTimeBetweenSpawn;

	public float additionalSpawnDelay;

	private Body body => PlayerCamera.main.body;

	private void Start()
	{
		openProgress = 5f;
	}

	private void Update()
	{
		if ((bool)EndLabScript.instance && EndLabScript.instance.doingEscape)
		{
			openProgress += Time.deltaTime;
		}
		float num = Vector2.Distance(base.transform.position, body.transform.position);
		if (!(num < minDistanceToSpawn) && !(num > maxDistanceToSpawn) && guardsToSpawn > 0 && !(openProgress < minTimeBetweenSpawn))
		{
			CreateGuard();
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.32f, 0.32f, 0.81f, 0.5f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector2.zero, maxDistanceToSpawn);
		Gizmos.color = new Color(0.8f, 0.32f, 0.37f, 0.3f);
		Gizmos.DrawWireSphere(Vector2.zero, minDistanceToSpawn);
	}

	private void OnWillRenderObject()
	{
		if (openProgress <= 1f)
		{
			door.sortingOrder = -300;
			float t = Utils.InOutCubic(openProgress);
			door.transform.localPosition = Vector2.Lerp(Vector2.zero, openPos, t);
			door.transform.localScale = Vector2.Lerp(Vector2.one, new Vector2(-1f, 1f), t);
		}
		else if (openProgress < 5f)
		{
			door.sortingOrder = -900;
			float t2 = Utils.OutBounce((openProgress - 1f) / 4f);
			door.transform.localPosition = Vector2.Lerp(openPos, Vector2.zero, t2);
			door.transform.localScale = Vector2.Lerp(new Vector2(-1f, 1f), Vector2.one, t2);
		}
		else
		{
			door.sortingOrder = -900;
			door.transform.localPosition = Vector2.zero;
			door.transform.localScale = Vector2.one;
		}
		door.sortingOrder = ((openProgress > 0.5f) ? (-900) : (-300));
	}

	public void CreateGuard()
	{
		openProgress = 0f;
		Sound.Play("endlabDoor", base.transform.position);
		GameObject gameObject = Utils.Create("guard", base.transform.position + Vector3.down * 0.75f, 0f);
		if (additionalSpawnDelay > 0f)
		{
			gameObject.GetComponent<GuardScript>().shootTime = additionalSpawnDelay;
		}
		guardsToSpawn--;
	}
}
