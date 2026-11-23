public class RichModifier : LayerModifier
{
	public RichModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		for (int i = 0; i < 2; i++)
		{
			world.GenerateOres(forceTitanium: true);
		}
		world.UpdateWorld();
	}

	public override void Disable(WorldGeneration world)
	{
	}
}
