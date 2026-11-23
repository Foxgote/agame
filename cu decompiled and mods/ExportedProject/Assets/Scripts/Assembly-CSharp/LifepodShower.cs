using UnityEngine;

public class LifepodShower : MonoBehaviour
{
	public bool activated;

	private float timeActive;

	private Body body => PlayerCamera.main.body;

	private void Awake()
	{
		timeActive = -100f;
	}

	private void Update()
	{
		if (Time.time - timeActive < 3f)
		{
			body.dirtyness -= Time.deltaTime * 16.7f;
			body.snowAmount += Time.deltaTime * 0.33f;
			if (body.snowAmount > 1f)
			{
				body.snowAmount = 1f;
			}
			body.eyeScareTime = 0.5f;
			Limb[] limbs = body.limbs;
			for (int i = 0; i < limbs.Length; i++)
			{
				limbs[i].SetDisinfect(120f);
			}
		}
	}

	public void Activate()
	{
		if (!activated)
		{
			activated = true;
			timeActive = Time.time;
			Sound.Play("shower", base.transform.position, twoDimensional: false, pitchShift: false);
			base.transform.GetComponentInChildren<ParticleSystem>().Play();
		}
	}
}
