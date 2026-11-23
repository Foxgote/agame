public class RainyModifier : LayerModifier
{
	public RainyModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		world.forceRain = true;
		world.SetFog(0.8f);
	}

	public override void Disable(WorldGeneration world)
	{
		world.forceRain = false;
		world.SetFog(0f);
	}
}
