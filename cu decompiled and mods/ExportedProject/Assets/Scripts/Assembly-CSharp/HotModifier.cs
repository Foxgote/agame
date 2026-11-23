public class HotModifier : LayerModifier
{
	public HotModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		world.temperatureOffset = 8f;
	}

	public override void Disable(WorldGeneration world)
	{
		world.temperatureOffset = 0f;
	}
}
