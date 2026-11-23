public class ShinyModifier : LayerModifier
{
	public ShinyModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		WorldGeneration.world.specialLightEnable = true;
	}

	public override void Disable(WorldGeneration world)
	{
		WorldGeneration.world.specialLightEnable = false;
	}
}
