using UnityEngine;

internal class CrystalBlinding : CrystalEffect
{
	public override void Start()
	{
		crystal.SetColor(Color.white);
		crystal.light.intensity = 10f;
		crystal.light.volumeIntensityEnabled = true;
		crystal.light.volumeIntensity = 0.05f;
		crystal.light.falloffIntensity = 1f;
		crystal.light.pointLightOuterRadius *= 0.4f;
	}
}
