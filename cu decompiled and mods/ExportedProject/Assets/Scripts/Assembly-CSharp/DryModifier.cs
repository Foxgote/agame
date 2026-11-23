using System;

public class DryModifier : LayerModifier
{
	public DryModifier(int index)
		: base(index)
	{
	}

	public override void Initialize(WorldGeneration world)
	{
		if (PlayerCamera.main.body.charType != 2)
		{
			Array.Clear(FluidManager.main.fluid, 0, FluidManager.main.fluid.Length);
		}
	}

	public override void Disable(WorldGeneration world)
	{
	}
}
