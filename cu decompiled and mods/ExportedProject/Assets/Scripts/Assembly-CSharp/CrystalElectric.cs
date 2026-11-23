using UnityEngine;

internal class CrystalElectric : CrystalEffect
{
	private float cooldown;

	public override void Start()
	{
		crystal.SetColor(Color.yellow);
		GameObject gameObject = Utils.Create("Special/Tesla", crystal.transform);
		gameObject.transform.localScale = Vector3.one * crystal.crystalSize;
		gameObject.transform.localPosition = Vector2.zero;
	}

	public override void Update()
	{
		cooldown -= Time.deltaTime;
	}

	public override void Hit(AttackInfo atk)
	{
		if (atk.unarmed && cooldown <= 0f)
		{
			Shock(PlayerCamera.main.body);
		}
	}

	public void Shock(Body body)
	{
		body.Ragdoll();
		Limb[] limbs = body.limbs;
		foreach (Limb limb in limbs)
		{
			limb.muscleHealth -= Random.Range(0f, 25f);
			limb.skinHealth -= Random.Range(0f, 15f);
			limb.pain = 100f;
			limb.rb.velocity = (limb.transform.position - crystal.transform.position).normalized * 30f;
		}
		body.temperature += 4f;
		body.consciousness -= 70f;
		body.shock += 40f;
		body.respiratoryRate = 0f;
		body.adrenaline = 50f;
		body.fibrillationProgress = 0f;
		cooldown = 30f;
		Sound.Play("zap", crystal.transform.position);
		PlayerCamera.main.shaker.Shake();
	}

	public override void Touched(GameObject touched)
	{
		if (!(cooldown > 0f) && Utils.GetBody(touched, out var body))
		{
			Shock(body);
		}
	}
}
