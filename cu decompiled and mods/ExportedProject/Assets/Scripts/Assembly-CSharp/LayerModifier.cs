public abstract class LayerModifier
{
	public static LayerModifier[] availableModifiers = new LayerModifier[19]
	{
		new FoggyModifier(0),
		new WarmModifier(1),
		new HotModifier(2),
		new ColdModifier(3),
		new FreezingModifier(4),
		new FloodedModifier(5),
		new DryModifier(6),
		new RainyModifier(7)
		{
			hideOnFirstLayer = true
		},
		new InfestedModifier(8),
		new IonizedModifier(9)
		{
			hideOnFirstLayer = true
		},
		new UnchippedModifier(10)
		{
			hideOnFirstLayer = true
		},
		new TurbulentModifier(11)
		{
			hideOnFirstLayer = true
		},
		new ShinyModifier(12),
		new OxygenatedModifier(13),
		new SterileModifier(14),
		new StableModifier(15),
		new BeautifulModifier(16),
		new UnsightlyModifier(17)
		{
			hideOnFirstLayer = true
		},
		new RichModifier(18)
	};

	public bool active;

	public int modifierIndex;

	public bool hideOnFirstLayer;

	public abstract void Initialize(WorldGeneration world);

	public abstract void Disable(WorldGeneration world);

	public LayerModifier(int index)
	{
		modifierIndex = index;
	}
}
