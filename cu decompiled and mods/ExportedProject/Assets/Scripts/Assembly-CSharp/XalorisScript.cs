using UnityEngine;

public class XalorisScript : MonoBehaviour
{
	private ParticleSystem sys;

	private float lastTime;

	private void Start()
	{
		sys = Object.Instantiate(Resources.Load<GameObject>("Special/XalorisParticle"), base.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
	}

	private void OnDestroy()
	{
		if ((bool)sys)
		{
			sys.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	private void OnWillRenderObject()
	{
		if (Time.time - lastTime > 0.5f)
		{
			lastTime = Time.time;
			if (Vector2.Distance(base.transform.position, PlayerCamera.main.body.transform.position) < 5.5f)
			{
				PlayerCamera.main.body.septicShock += 0.074f;
			}
		}
	}
}
