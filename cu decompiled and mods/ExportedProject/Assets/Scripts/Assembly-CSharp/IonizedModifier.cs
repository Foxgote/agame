using UnityEngine;

public class IonizedModifier : LayerModifier
{
	public IonizedModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		world.DistributeEntities((GameObject)Resources.Load("radbarrel"), 1.2f, 1.3f, 1.5f, 0f, 0f, spawnInGround: false, randomFlip: false, null, isTrap: true);
		world.DistributeEntities((GameObject)Resources.Load("antirad"), 0.25f, 0.3f, 1f, 0f, 0f, spawnInGround: false, randomFlip: false, null, isTrap: true);
	}

	public override void Disable(WorldGeneration world)
	{
	}
}
