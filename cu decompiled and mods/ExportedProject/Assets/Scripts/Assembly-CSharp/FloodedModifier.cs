public class FloodedModifier : LayerModifier
{
	public FloodedModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		if (world.biomeDepth != 8)
		{
			world.PlaceLiquids(25f, 1, 600);
		}
	}

	public override void Disable(WorldGeneration world)
	{
	}
}
