using UnityEngine;

public class ExplosionParams
{
	public RangeF muscleDamage = new RangeF(0f, 60f);

	public RangeF skinDamage = new RangeF(0f, 75f);

	public float skinDamageChance = 0.2f;

	public float boneBreakChance = 0.06f;

	public float dislocationChance = 0.135f;

	public float disfigureChance = 0.34f;

	public float bleedChance = 0.15f;

	public RangeF bleedAmount = new RangeF(4f, 30f);

	public float structuralDamage = 500f;

	public Vector2 position;

	public float range = 12f;

	public float velocity = 60f;

	public float shrapnelChance = 0.4f;

	public float dismemberChance;

	public string sound = "explosion";
}
