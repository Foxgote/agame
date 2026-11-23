using System.Collections.Generic;
using UnityEngine;

public class GrabberPlant : MonoBehaviour
{
	public Vector2 tipPos;

	private FastNoiseLite moveNoise;

	private float randOffset;

	private IKHandle ik;

	private AudioSource source;

	private float grabbedTime;

	private Rigidbody2D grabBody;

	private List<CircleCollider2D> cols;

	private void Start()
	{
		ik = GetComponent<IKHandle>();
		moveNoise = new FastNoiseLite();
		source = GetComponent<AudioSource>();
		grabbedTime = -100f;
		randOffset = Random.Range(-1000f, 1000f);
		float num = Random.Range(0.6f, 1.4f);
		cols = new List<CircleCollider2D>();
		foreach (IKSegment segment in ik.segments)
		{
			segment.length *= num;
		}
		ik.RecalculateLength();
		tipPos = Vector2.up;
		for (int i = 0; i < ik.segments.Count / 4; i++)
		{
			CircleCollider2D circleCollider2D = base.gameObject.AddComponent<CircleCollider2D>();
			circleCollider2D.radius = 1.5f;
			circleCollider2D.offset = Vector2.zero;
			cols.Add(circleCollider2D);
		}
	}

	private void Update()
	{
		if (!WorldGeneration.world.GetClosestChunkRenderer(WorldGeneration.world.WorldToBlockPos(base.transform.position)).enabled)
		{
			return;
		}
		Vector2 vector = tipPos;
		float num = 1f;
		if (Time.timeScale > 1f)
		{
			num += Time.timeScale * 0.1f;
		}
		if (Time.timeScale < 1f)
		{
			num = Time.timeScale;
		}
		Vector2 vector2 = new Vector2(moveNoise.GetNoise((Time.unscaledTime * num + randOffset) * 60f, 0f), moveNoise.GetNoise(Time.time * 60f, 500f)) * Time.unscaledDeltaTime * num * 18f * Mathf.Sin((Time.unscaledTime * num + randOffset) * 1.75f) * (grabBody ? 2f : 1f);
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.TransformPoint(tipPos), vector2.normalized, 2f, LayerMask.GetMask("Ground"));
		if ((bool)raycastHit2D)
		{
			vector2 += vector2.magnitude * raycastHit2D.normal;
		}
		tipPos += vector2;
		if (tipPos.magnitude > ik.maxLength)
		{
			tipPos = Vector2.ClampMagnitude(tipPos, ik.maxLength);
		}
		if (tipPos.y < 0f)
		{
			tipPos.y = 0f;
		}
		source.volume = (vector - tipPos).magnitude * 5f;
		ik.targetPos = (grabBody ? ((Vector2)base.transform.InverseTransformPoint(grabBody.transform.position)) : tipPos);
		if ((bool)Physics2D.OverlapPoint(base.transform.TransformPoint(tipPos), LayerMask.GetMask("Ground")))
		{
			tipPos = Vector2.Lerp(tipPos, Vector2.up * 2f, Time.deltaTime * 2f);
		}
		for (int i = 0; i < cols.Count; i++)
		{
			cols[i].offset = ik.segments[i * 4].pos;
		}
		if (Time.time - grabbedTime > 5f && Vector2.Distance(base.transform.TransformPoint(tipPos), PlayerCamera.main.body.transform.position) < 3.2f && PlayerCamera.main.body.conscious)
		{
			grabbedTime = Time.time;
			grabBody = PlayerCamera.main.body.limbs.PickRandom().rb;
			PlayerCamera.main.body.Ragdoll();
			PlayerCamera.main.body.Scream();
		}
		if ((bool)grabBody)
		{
			if (Time.timeScale > 1f)
			{
				PlayerCamera.main.SetTimeScale(PlayerCamera.SpeedType.Normal, switchSound: false);
			}
			PlayerCamera.main.body.shock = Mathf.Max(PlayerCamera.main.body.shock, 20f);
			PlayerCamera.main.body.eyePanicTime = 0.5f;
			if (Time.time - grabbedTime > 2f || !PlayerCamera.main.body.conscious)
			{
				grabBody = null;
			}
		}
	}

	private void FixedUpdate()
	{
		if ((bool)grabBody)
		{
			grabBody.MovePosition(base.transform.TransformPoint(tipPos));
		}
	}
}
