using UnityEngine;

internal class CrystalFragile : CrystalEffect
{
	public override void Start()
	{
		crystal.build.health = 10f;
	}

	public override void Touched(GameObject touched)
	{
		if ((bool)touched.GetComponent<Item>() || (bool)touched.GetComponent<Body>())
		{
			Sound.Play("glass", crystal.transform.position);
			crystal.build.health = 0f;
		}
	}
}
