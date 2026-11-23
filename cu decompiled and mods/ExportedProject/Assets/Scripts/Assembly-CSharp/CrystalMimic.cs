using UnityEngine;

internal class CrystalMimic : CrystalEffect
{
	private bool activated;

	private int enemyCount;

	public override void Start()
	{
		enemyCount = Random.Range(1, 3);
	}

	public override void Update()
	{
		if (!activated)
		{
			crystal.transform.localScale = new Vector3(1f + Mathf.Sin(Time.time * 2f) * 0.1f, 1f);
		}
	}

	public override void Touched(GameObject touched)
	{
		if (!activated && Utils.GetBody(touched, out var _))
		{
			activated = true;
			Sound.Play("observerlaugh", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
			for (int i = 0; i < enemyCount; i++)
			{
				Utils.Create("crystalenemy", crystal.transform.position, 0f).GetComponent<CrystalEnemy>().SetColor(crystal.sprite.color);
			}
		}
	}

	public override void Hit(AttackInfo atk)
	{
		if (!activated)
		{
			activated = true;
			Sound.Play("observerlaugh", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
			for (int i = 0; i < enemyCount; i++)
			{
				Utils.Create("crystalenemy", crystal.transform.position, 0f).GetComponent<CrystalEnemy>().SetColor(crystal.sprite.color);
			}
		}
	}
}
