public class StableModifier : LayerModifier
{
	public StableModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		WorldGeneration.world.allowEarthquakes = false;
	}

	public override void Disable(WorldGeneration world)
	{
		WorldGeneration.world.allowEarthquakes = true;
	}
}
