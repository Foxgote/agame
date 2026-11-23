public class FoggyModifier : LayerModifier
{
	public FoggyModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		world.SetFog(1f);
	}

	public override void Disable(WorldGeneration world)
	{
		world.SetFog(0f);
	}
}
