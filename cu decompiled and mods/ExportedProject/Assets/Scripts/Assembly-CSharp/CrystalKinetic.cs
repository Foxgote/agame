using UnityEngine;

internal class CrystalKinetic : CrystalEffect
{
	private Vector2 origPos;

	private float cooldown;

	public override void Start()
	{
		origPos = crystal.transform.position;
	}

	public override void Touched(GameObject touched)
	{
		if (Utils.GetBody(touched, out var body) && cooldown <= 0f)
		{
			Vector2 velocity = (Vector2)(body.transform.position - crystal.transform.position).normalized * 50f;
			body.SetVelocity(velocity);
			body.slowdownAmount = 0.5f;
			cooldown = 1f;
			Sound.Play("mushroombounce", crystal.transform.position);
			PlayerCamera.main.shaker.Shake(100f);
		}
	}

	public override void Update()
	{
		cooldown = Mathf.MoveTowards(cooldown, 0f, Time.deltaTime * 0.25f);
		crystal.transform.position = origPos + Random.insideUnitCircle * 0.3f * (1f - cooldown);
	}
}
