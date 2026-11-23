public class ColdModifier : LayerModifier
{
	public ColdModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		world.temperatureOffset = -4f;
	}

	public override void Disable(WorldGeneration world)
	{
		world.temperatureOffset = 0f;
	}
}
