public class UnchippedModifier : LayerModifier
{
	private bool initiallyChipped;

	public UnchippedModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		initiallyChipped = WorldGeneration.unchipped;
		WorldGeneration.world.SetUnchipped(unchipped: true);
	}

	public override void Disable(WorldGeneration world)
	{
		WorldGeneration.world.SetUnchipped(initiallyChipped);
	}
}
