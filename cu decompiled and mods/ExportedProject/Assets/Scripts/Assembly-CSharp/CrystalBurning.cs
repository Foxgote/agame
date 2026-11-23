using UnityEngine;

internal class CrystalBurning : CrystalEffect
{
	private float cooldown;

	public override void Start()
	{
		crystal.SetColor(new Color(1f, 0.5f, 0f, 0.5f));
		GameObject gameObject = Utils.Create("Special/CrystalFire", crystal.transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.GetComponent<CampfireAnimation>().upwardsParentOffset *= crystal.crystalSize - 0.66f;
		crystal.AddEffectAudio("campfireLoop", 10f * crystal.crystalSize);
	}

	public override void Update()
	{
		cooldown -= Time.deltaTime;
	}

	public override void Hit(AttackInfo atk)
	{
		if (atk.unarmed && cooldown <= 0f)
		{
			Burn(PlayerCamera.main.body);
		}
	}

	public void Burn(Body body)
	{
		body.Ragdoll();
		body.Scream();
		Limb closestLimb = body.GetClosestLimb(crystal.transform.position);
		closestLimb.skinHealth -= 60f;
		closestLimb.muscleHealth -= 5f;
		closestLimb.pain += 40f;
		body.temperature += 0.2f;
		body.adrenaline += 25f;
		crystal.build.health -= 20f;
		crystal.build.health *= 0.5f;
		cooldown = 0.75f;
		Sound.Play("Scald", crystal.transform.position);
		PlayerCamera.main.shaker.Shake(50f);
	}

	public override void Touched(GameObject touched)
	{
		if (!(cooldown > 0f) && Utils.GetBody(touched, out var body))
		{
			Burn(body);
		}
	}
}
