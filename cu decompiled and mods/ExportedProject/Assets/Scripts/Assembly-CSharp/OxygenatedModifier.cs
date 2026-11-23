public class OxygenatedModifier : LayerModifier
{
	public OxygenatedModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		PlayerCamera.main.body.staminaGainMultiplier = 2f;
	}

	public override void Disable(WorldGeneration world)
	{
		PlayerCamera.main.body.staminaGainMultiplier = 1f;
	}
}
