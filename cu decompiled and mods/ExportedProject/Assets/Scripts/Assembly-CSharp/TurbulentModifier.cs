using UnityEngine;

public class TurbulentModifier : LayerModifier
{
	public TurbulentModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		Physics2D.gravity = new Vector2(0f, Random.Range(-30f, -45f));
	}

	public override void Disable(WorldGeneration world)
	{
		Physics2D.gravity = new Vector2(0f, -55f);
	}
}
