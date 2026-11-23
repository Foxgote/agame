public class SterileModifier : LayerModifier
{
	public SterileModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		PlayerCamera.main.body.infectionChanceMultiplier = 0.2f;
	}

	public override void Disable(WorldGeneration world)
	{
		PlayerCamera.main.body.infectionChanceMultiplier = 1f;
	}
}
