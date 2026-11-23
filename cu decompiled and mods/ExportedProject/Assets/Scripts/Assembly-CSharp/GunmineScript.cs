using UnityEngine;

public class GunmineScript : MonoBehaviour
{
	public float cooldown;

	private bool didReload;

	private void OnWillRenderObject()
	{
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, base.transform.up, 1f, LayerMask.GetMask("Body", "Limb"));
		if (!raycastHit2D)
		{
			cooldown -= Time.deltaTime;
		}
		if (cooldown < 1f && !didReload)
		{
			didReload = true;
			Sound.Play("reload", base.transform.position, twoDimensional: false, pitchShift: false);
		}
		if (!(cooldown > 0f) && (bool)raycastHit2D)
		{
			Sound.Play("rifleshot", base.transform.position, twoDimensional: true, pitchShift: false);
			TurretScript.Shoot(new FireInfo
			{
				pos = base.transform.position + base.transform.up * 0.25f,
				dir = base.transform.up,
				ignoreTrans = base.transform
			});
			GetComponentInChildren<ParticleSystem>().Play();
			didReload = false;
			cooldown = 6f;
		}
	}
}
