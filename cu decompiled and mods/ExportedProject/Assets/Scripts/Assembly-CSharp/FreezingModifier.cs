public class FreezingModifier : LayerModifier
{
	public FreezingModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		world.temperatureOffset = -10f;
	}

	public override void Disable(WorldGeneration world)
	{
		world.temperatureOffset = 0f;
	}
}
