using UnityEngine;

internal class CrystalIrradiated : CrystalEffect
{
	public override void Start()
	{
		crystal.gameObject.AddComponent<RadioactiveObject>().radAtZero = 4f * crystal.crystalSize;
		crystal.SetColor(Color.green);
	}
}
