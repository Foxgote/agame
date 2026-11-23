public class BeautifulModifier : LayerModifier
{
	public BeautifulModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		PlayerCamera.main.body.happiness += 15f;
		PlayerCamera.main.body.layerModifierHappiness = 10f;
	}

	public override void Disable(WorldGeneration world)
	{
		PlayerCamera.main.body.layerModifierHappiness = 0f;
	}
}
