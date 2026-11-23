using UnityEngine;

internal class CrystalTeleport : CrystalEffect
{
	public override void Start()
	{
		crystal.SetColor(crystal.sprite.color.WithAlpha(0.2f));
	}

	public override void Touched(GameObject touched)
	{
		if (!Utils.GetBody(touched, out var body))
		{
			return;
		}
		for (int i = 0; i < 1000; i++)
		{
			Vector2 vector = (Vector2)crystal.transform.position + Random.insideUnitCircle * 64f;
			if (!Physics2D.OverlapPoint(vector, LayerMask.GetMask("Ground")))
			{
				RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, Vector2.down, 999f, LayerMask.GetMask("Ground"));
				if ((bool)raycastHit2D)
				{
					Sound.Play("observerlaugh", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
					PlayerCamera.main.StartCoroutine("FlashBrief");
					body.consciousness = 100f;
					body.shock = 0f;
					body.rb.velocity = Vector2.zero;
					body.Stand();
					body.transform.position = raycastHit2D.point + Vector2.up * 3f;
					break;
				}
			}
		}
	}
}
