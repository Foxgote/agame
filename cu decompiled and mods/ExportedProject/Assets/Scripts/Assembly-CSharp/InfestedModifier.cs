using UnityEngine;

public class InfestedModifier : LayerModifier
{
	public InfestedModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		world.DistributeEntities((GameObject)Resources.Load("shadecrawler"), 1.7f, 1.8f, 2f, 180f, 0f, spawnInGround: false, randomFlip: false, null, isTrap: true);
	}

	public override void Disable(WorldGeneration world)
	{
	}
}
