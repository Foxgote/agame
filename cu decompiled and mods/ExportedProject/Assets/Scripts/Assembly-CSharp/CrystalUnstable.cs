using UnityEngine;

internal class CrystalUnstable : CrystalEffect
{
	private float timer;

	private bool timerStarted;

	private Vector2 origPos;

	public override void Start()
	{
		crystal.SetColor(Color.red);
		origPos = crystal.transform.position;
	}

	public override void Touched(GameObject touched)
	{
		if (!timerStarted && Utils.GetBody(touched, out var body))
		{
			StartTimer(body);
		}
	}

	public override void Hit(AttackInfo atk)
	{
		StartTimer(PlayerCamera.main.body);
	}

	private void StartTimer(Body body)
	{
		timerStarted = true;
		body.talker.Talk("..!");
		body.eyeScareTime = 3f;
		Sound.Play("crystaltick", crystal.transform.position, twoDimensional: true, pitchShift: false, crystal.transform);
	}

	public override void Update()
	{
		if (timerStarted)
		{
			timer += Time.deltaTime;
			crystal.light.intensity += Time.deltaTime * 4f;
			crystal.transform.position = origPos + Random.insideUnitCircle * timer * 0.07f;
			if (timer > 5f)
			{
				Object.Destroy(crystal.gameObject);
				crystal.GetComponent<Collider2D>().enabled = false;
				WorldGeneration.CreateExplosion(new ExplosionParams
				{
					position = crystal.transform.position,
					range = 16f * Mathf.Lerp(crystal.crystalSize, 1f, 0.5f),
					structuralDamage = 1000f * crystal.crystalSize,
					boneBreakChance = 0.1f * crystal.crystalSize,
					dislocationChance = 0.05f * crystal.crystalSize,
					muscleDamage = new RangeF(0f * crystal.crystalSize, 30f * crystal.crystalSize),
					velocity = 20f * crystal.crystalSize,
					disfigureChance = 0.05f * crystal.crystalSize
				});
			}
		}
	}
}
