using UnityEngine;

internal class CrystalShy : CrystalEffect
{
	private bool activated;

	public override void Touched(GameObject touched)
	{
		if (activated || !Utils.GetBody(touched, out var _))
		{
			return;
		}
		Collider2D[] array = Physics2D.OverlapCircleAll(crystal.transform.position, 64f, LayerMask.GetMask("Ground"));
		foreach (Collider2D collider2D in array)
		{
			if ((bool)collider2D.GetComponent<CrystalBehaviour>())
			{
				Vector2 vector = collider2D.transform.position;
				Vector2 vector2 = crystal.transform.position;
				Quaternion rotation = collider2D.transform.rotation;
				Quaternion rotation2 = crystal.transform.rotation;
				collider2D.transform.SetPositionAndRotation(vector2, rotation2);
				crystal.transform.SetPositionAndRotation(vector, rotation);
				Sound.Play("observerlaugh", crystal.transform.position, twoDimensional: true, pitchShift: false);
				break;
			}
		}
		activated = true;
	}
}
