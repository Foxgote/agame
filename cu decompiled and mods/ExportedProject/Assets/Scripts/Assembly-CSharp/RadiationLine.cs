using UnityEngine;

public class RadiationLine : MonoBehaviour
{
	public static RadiationLine line;

	private float timeGone;

	public bool active;

	private Body body => PlayerCamera.main.body;

	private WorldGeneration world => WorldGeneration.world;

	private void Awake()
	{
		line = this;
		Deactivate();
	}

	public void Activate()
	{
		active = true;
	}

	public void Deactivate()
	{
		base.transform.position = Vector3.up * WorldGeneration.world.halfHeight + Vector3.up * 2f;
		timeGone = 0f;
		active = false;
	}

	private void Update()
	{
		if (!active)
		{
			return;
		}
		if (!world.worldExists || world.generatingWorld)
		{
			Deactivate();
			return;
		}
		timeGone += Time.deltaTime * (body.conscious ? 1f : 0.2f) * 1.5f;
		base.transform.position = Vector3.up * world.halfHeight + Vector3.down * timeGone + Vector3.down * Mathf.Sin(timeGone * 0.66f) * 3f;
		if (body.transform.position.y > base.transform.position.y)
		{
			float a = body.transform.position.y - base.transform.position.y;
			a = Mathf.Min(a, 10f);
			body.radiationSickness += a * Time.deltaTime * 0.03f;
			body.eyeScareTime = 1f;
			PlayerCamera.main.SetIrradiateIntensity(a * 0.05f);
		}
	}
}
